using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using static Notatnik.Session;
using static System.Net.WebRequestMethods;

namespace Notatnik
{
    internal static class Session
    {
        static readonly HttpClient http = new();
        private static string ClientId;
        private static string ClientSecret;
        private const string GitHubAuthorizeUrl = "https://github.com/login/oauth/authorize";
        private const string GitHubTokenUrl = "https://github.com/login/oauth/access_token";
        private const string GitHubUserApiUrl = "https://api.github.com/user";

        public static User? LoggedInUser { get; set; }

        static Session()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);

            IConfigurationRoot configuration = builder.Build();

            ClientId = configuration["GitHub:ClientId"] ?? string.Empty;
            ClientSecret = configuration["GitHub:ClientSecret"];
        }

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
            FetchUserDataFromDatabase();

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

            FetchUserDataFromDatabase();

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

        private static void FetchUserDataFromDatabase()
        {
            if (LoggedInUser == null)
                return;
            if (!Database.UserExists(LoggedInUser!.Login!))
            {
                Database.RegisterUser(LoggedInUser.Login!);
            }
            var dbUser = Database.GetUser(LoggedInUser.Login);
            if (dbUser != null)
            {
                LoggedInUser.organizationId = dbUser.OrganizationId;
                LoggedInUser.isManager = dbUser.IsManager;
            }
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
