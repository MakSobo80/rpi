using System.Text.Json;
using System.Windows;

namespace Notatnik
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            var user = LoadUserSession();
            if (user != null)
            {
                var mainWindow = new MainWindow();
                Application.Current.MainWindow = mainWindow;
                mainWindow.Show();
            }
            else
            {
                var loginWindow = new LoginWindow();
                if (loginWindow.ShowDialog() == true)
                {
                    var mainWindow = new MainWindow();
                    Application.Current.MainWindow = mainWindow;
                    if(SessionData.CurrentUser != null)
                        MessageBox.Show("Zalogowano jako: " + SessionData.CurrentUser?.Login);
                    else
                        MessageBox.Show("Kontynuowano bez logowania");
                    mainWindow.Show();
                }
                else
                {
                    Application.Current.Shutdown();
                }
            }
        }



        private GitHubUser? LoadUserSession()
        {
            string filePath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Notatnik",
                "session.json");

            MessageBox.Show(filePath);

            if (!System.IO.File.Exists(filePath))
                return null;

            string json = System.IO.File.ReadAllText(filePath);
            var sessionData = JsonSerializer.Deserialize<SessionFile>(json);

            if (sessionData != null)
            {
                SessionData.CurrentUser = sessionData.User;
                return sessionData.User;
            }

            return null;
        }
    }
    public class SessionFile
    {
        public GitHubUser? User { get; set; }
        public string? AccessToken { get; set; }
    }
}
