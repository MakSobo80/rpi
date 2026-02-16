using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using static System.Net.WebRequestMethods;

namespace Notatnik
{
    public partial class LoginWindow : Window
    {
        static readonly HttpClient http = new();
        private const string ClientId = "Ov23lixBJaaA5LSXOjY2";
        private const string ClientSecret = "84b829953644741c72cec3f2a9554de2d8ff59a2";
        private const string GitHubAuthorizeUrl = "https://github.com/login/oauth/authorize";
        private const string GitHubTokenUrl = "https://github.com/login/oauth/access_token";
        private const string GitHubUserApiUrl = "https://api.github.com/user";

        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void GitHubLogin_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = await LoginWithGithub();
            this.Close();
        }

        public async Task<bool> LoginWithGithub()
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

            var userReq = new HttpRequestMessage(HttpMethod.Get, GitHubUserApiUrl);
            userReq.Headers.UserAgent.ParseAdd("GithubAuthCodeFlowDemo/1.0");
            userReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

            var meResp = await http.SendAsync(userReq);
            meResp.EnsureSuccessStatusCode();

            var responseContent = await meResp.Content.ReadAsStringAsync();
            var user = JsonSerializer.Deserialize<GitHubUser>(responseContent);
            SessionData.CurrentUser = user;
            SaveUserSession(user!, token.AccessToken!);

            return true;
        }

        private void ContinueWithoutLogin_Click(object sender, RoutedEventArgs e)
        {
            SessionData.CurrentUser = null;
            this.DialogResult = true;
            this.Close();
        }

        private void SaveUserSession(GitHubUser user, string accessToken)
        {
            string filePath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Notatnik",
                "session.json");

            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath)!);

            var sessionData = new SessionFile
            {
                User = user,
                AccessToken = accessToken
            };

            string json = JsonSerializer.Serialize(sessionData, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            System.IO.File.WriteAllText(filePath, json);
        }

    }

    public sealed class Token
    {
        [JsonPropertyName("access_token")] public string? AccessToken { get; set; }
        [JsonPropertyName("token_type")] public string? TokenType { get; set; }
        [JsonPropertyName("scope")] public string? Scope { get; set; }
    }

    public class GitHubUser
    {
        [JsonPropertyName("login")] public string? Login { get; set; }
        [JsonPropertyName("name")] public string? Name { get; set; }
        [JsonPropertyName("avatar_url")] public string? AvatarUrl { get; set; }
    }

    public static class SessionData
    {
        public static GitHubUser? CurrentUser { get; set; }
    }
}
