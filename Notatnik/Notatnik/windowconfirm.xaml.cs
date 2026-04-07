using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace Notatnik
{
    /// <summary>
    /// Logika interakcji dla klasy windowconfirm.xaml
    /// </summary>
    public partial class windowconfirm : Window
    {
        private class FileItem
        {
            public int Id { get; set; }
            public string DisplayName { get; set; } = string.Empty;
            public string AuthorName { get; set; } = string.Empty;
            public int SizeBytes { get; set; }
        }

        public windowconfirm()
        {
            InitializeComponent();
            Loaded += WindowConfirm_Loaded;
        }

        private void WindowConfirm_Loaded(object sender, RoutedEventArgs e)
        {
            OdswiezListePlików();
        }

        private void OdswiezListePlików()
        {
            listFiles.Items.Clear();

            var user = Session.LoggedInUser;
            if (user == null || user.organizationId == null)
            {
                MessageBox.Show("Musisz być zalogowany i należeć do organizacji.", "Błąd",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var files = Database.GetFilesForOrganization(user.organizationId.Value);
            foreach (var f in files)
            {
                var dbAuthor = Database.GetUserById(f.AuthorId);
                listFiles.Items.Add(new FileItem
                {
                    Id = f.Id,
                    DisplayName = f.Name.Trim(),
                    AuthorName = dbAuthor?.Username?.Trim() ?? $"id:{f.AuthorId}",
                    SizeBytes = f.File?.Length ?? 0
                });
            }
        }

        private void ListFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listFiles.SelectedItem is FileItem item)
            {
                labelFileName.Content = $"Nazwa: {item.DisplayName}";
                labelFileAuthor.Content = $"Autor: {item.AuthorName}";
                labelFileSize.Content = $"Rozmiar: {item.SizeBytes} bajtów";
            }
            else
            {
                labelFileName.Content = "Nazwa: —";
                labelFileAuthor.Content = "Autor: —";
                labelFileSize.Content = "Rozmiar: —";
            }
        }

        private void Zatwierdz(object sender, RoutedEventArgs e)
        {
            if (listFiles.SelectedItem is not FileItem selected)
            {
                MessageBox.Show("Wybierz plik z listy.", "Informacja");
                return;
            }

            var fileData = Database.GetFileById(selected.Id);
            if (fileData == null)
            {
                MessageBox.Show("Nie można pobrać pliku.", "Błąd");
                return;
            }

            var saveDialog = new SaveFileDialog
            {
                FileName = fileData.Name.Trim(),
                Title = "Zapisz plik"
            };
            if (saveDialog.ShowDialog() == true)
            {
                System.IO.File.WriteAllBytes(saveDialog.FileName, fileData.File);
                MessageBox.Show("Plik został zapisany.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Odrzuc(object sender, RoutedEventArgs e)
        {
            if (listFiles.SelectedItem is not FileItem selected)
            {
                MessageBox.Show("Wybierz plik z listy.", "Informacja");
                return;
            }

            var result = MessageBox.Show(
                $"Czy na pewno chcesz usunąć plik '{selected.DisplayName}' z bazy danych?",
                "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Database.DeleteFile(selected.Id);
                OdswiezListePlików();
                labelFileName.Content = "Nazwa: —";
                labelFileAuthor.Content = "Autor: —";
                labelFileSize.Content = "Rozmiar: —";
            }
        }

        private void Powrot_Click(object sender, RoutedEventArgs e)
        {
            var adminWindow = new WindowAdmin();
            adminWindow.Show();
            this.Close();
        }
    }
}
