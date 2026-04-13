using System.Windows;

namespace Notatnik
{
    public partial class InputDialog : Window
    {
        public string InputText => textInput.Text;

        public InputDialog(string prompt, string defaultValue = "")
        {
            InitializeComponent();
            labelPrompt.Content = prompt;
            textInput.Text = defaultValue;
            textInput.SelectAll();
            textInput.Focus();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
