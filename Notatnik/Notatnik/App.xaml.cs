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

            // Sprawdź, czy istnieje zapisany użytkownik w sesji
            var user = LoadUserSession();
            if (user != null)
            {
                // Jeśli użytkownik jest zalogowany, otwórz MainWindow
                var mainWindow = new MainWindow();
                Application.Current.MainWindow = mainWindow;
                mainWindow.Show();
            }
            else
            {
                // Jeśli użytkownik nie jest zalogowany, otwórz LoginWindow
                var loginWindow = new LoginWindow();
                if (loginWindow.ShowDialog() == true)
                {
                    // Po zalogowaniu otwórz MainWindow
                    var mainWindow = new MainWindow();
                    Application.Current.MainWindow = mainWindow;
                    MessageBox.Show("Zalogowano jako: " + SessionData.CurrentUser?.Login);
                    mainWindow.Show();
                }
                else
                {
                    // Jeśli użytkownik zamknie LoginWindow bez logowania, zamknij aplikację
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
