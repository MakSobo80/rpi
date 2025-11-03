using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using System.Web;
using System.Text.Json.Serialization;
using static Notatnik.Session;
using System.Windows;

namespace Notatnik
{
    internal static class Session
    {
        static readonly HttpClient http = new();
        private const string ClientId = "Ov23lixBJaaA5LSXOjY2";
        private const string ClientSecret = "84b829953644741c72cec3f2a9554de2d8ff59a2";
        private const string GitHubAuthorizeUrl = "https://github.com/login/oauth/authorize";
        private const string GitHubTokenUrl = "https://github.com/login/oauth/access_token";
        private const string GitHubUserApiUrl = "https://api.github.com/user";

        public static User? LoggedInUser { get; set; }

        public static async Task<bool> LoginWithGithub()
        {
            var scope = "read:user";

            int port = 48123;
            string redirectUri = $"http://127.0.0.1:{port}/callback";
            string state = Guid.NewGuid().ToString("N");

            string authorizeUrl =
                GitHubAuthorizeUrl +
                $"?client_id={Uri.EscapeDataString(ClientId)}" +
                $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                $"&scope={Uri.EscapeDataString(scope)}" +
                $"&state={Uri.EscapeDataString(state)}";

            using var listener = new HttpListener();
            listener.Prefixes.Add($"{redirectUri}/");
            listener.Start();

            Process.Start(new ProcessStartInfo { FileName = authorizeUrl, UseShellExecute = true });

            var ctx = await listener.GetContextAsync();
            var query = HttpUtility.ParseQueryString(ctx.Request.Url!.Query);
            string code = query["code"]!;
            string returnedState = query["state"]!;

            var responseBytes = Encoding.UTF8.GetBytes("You may now close this window.");
            ctx.Response.ContentLength64 = responseBytes.Length;
            await ctx.Response.OutputStream.WriteAsync(responseBytes);
            ctx.Response.Close();
            listener.Stop();

            var content = new StringContent(
                $"client_id={Uri.EscapeDataString(ClientId)}" +
                $"&client_secret={Uri.EscapeDataString(ClientSecret)}" +
                $"&code={Uri.EscapeDataString(code)}" +
                $"&redirect_uri={Uri.EscapeDataString(redirectUri)}",
                Encoding.UTF8, "application/x-www-form-urlencoded");

            var req = new HttpRequestMessage(HttpMethod.Post, GitHubTokenUrl) { Content = content };
            req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var resp = await http.SendAsync(req);
            var body = await resp.Content.ReadAsStringAsync();
            var token = JsonSerializer.Deserialize<Token>(body)!;

            LoggedInUser = await FetchGitHubUserAsync(token.AccessToken!);
            if(LoggedInUser == null)
            {
                return false;
            }
            SaveSession(token.AccessToken!);

            return true;
        }

        public static bool ContinueWithoutLogin()
        {
            Session.LoggedInUser = null;
            return true;
        }

        public static bool SaveSession(string token)
        {
            string filePath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Notatnik",
                "session.json");

            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath)!);

            var sessionData = new SessionFile
            {
                AccessToken = token
            };

            string json = JsonSerializer.Serialize(sessionData, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            System.IO.File.WriteAllText(filePath, json);

            return true;
        }

        public static async Task<bool> LoadSession()
        {
            string filePath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Notatnik",
                "session.json");

            if (!System.IO.File.Exists(filePath))
                return false;

            string json = System.IO.File.ReadAllText(filePath);
            var sessionData = JsonSerializer.Deserialize<SessionFile>(json);

            if (sessionData == null)
            {
                MessageBox.Show("Failed to load session data. Please log in again.");
                return false;
            }

            var userTask = await FetchGitHubUserAsync(sessionData.AccessToken!);
            LoggedInUser = userTask;

            return true;
        }

        private static async Task<User?> FetchGitHubUserAsync(string accessToken)
        {
            var userReq = new HttpRequestMessage(HttpMethod.Get, GitHubUserApiUrl);
            userReq.Headers.UserAgent.ParseAdd("GithubAuthCodeFlowDemo/1.0");
            userReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var meResp = await http.SendAsync(userReq);
            meResp.EnsureSuccessStatusCode();

            var responseContent = await meResp.Content.ReadAsStringAsync();
            var user = JsonSerializer.Deserialize<User>(responseContent);
            return user;
        }

        public class User
        {
            [JsonPropertyName("login")] public string? Login { get; set; }
            [JsonPropertyName("name")] public string? Name { get; set; }
            [JsonPropertyName("avatar_url")] public string? AvatarUrl { get; set; }
            [JsonPropertyName("id")] public int? Id { get; set; }
            public int? organizationId { get; set; }
            public bool? isManager { get; set; }
        }

        private class SessionFile
        {
            [JsonPropertyName("token")] public string? AccessToken { get; set; }
        }
    }
}
