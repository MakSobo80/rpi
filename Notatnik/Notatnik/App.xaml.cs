using System.Text.Json;
using System.Windows;

namespace Notatnik
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            bool sessionLoaded = await Session.LoadSession();
            if (sessionLoaded)
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
                    if(Session.LoggedInUser != null)
                    {
                        var org = Database.GetOrganization(Session.LoggedInUser.organizationId);
                        var orgName = org?.Name ?? "brak";
                        MessageBox.Show("Zalogowano jako: " + Session.LoggedInUser.Login + " z organizacji " + orgName);
                    }
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

    }
}
