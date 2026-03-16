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

        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void GitHubLogin_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = await Session.LoginWithGithub();
            this.Close();
        }

        

        private async void ContinueWithoutLogin_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = Session.ContinueWithoutLogin();
            this.Close();
        }

    }

    public sealed class Token
    {
        [JsonPropertyName("access_token")] public string? AccessToken { get; set; }
        [JsonPropertyName("token_type")] public string? TokenType { get; set; }
        [JsonPropertyName("scope")] public string? Scope { get; set; }
    }
}
