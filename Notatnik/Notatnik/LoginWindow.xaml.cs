using System.Windows;

namespace Notatnik
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void GitHubLogin_Click(object sender, RoutedEventArgs e)
        {
            // Logika logowania przez GitHub
            this.DialogResult = true; // Ustaw na true po udanym logowaniu
            this.Close();
        }

        private void ContinueWithoutLogin_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true; // Ustaw na true, jeśli użytkownik kontynuuje bez logowania
            this.Close();
        }
    }
}
