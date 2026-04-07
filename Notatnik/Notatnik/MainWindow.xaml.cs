using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace Notatnik
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Notepad notepad;

        public MainWindow()
        {
            InitializeComponent();
            notepad = new(TextContent, Pointer);
            TextContent.TextChanged += (object sender, TextChangedEventArgs e) =>
            {
                notepad.WrittenText = TextContent.Text;
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OpenFile(object sender, EventArgs e)
        {
            notepad.OpenFile();
        }

        private void SaveFile(object sender, EventArgs e)
        {
            notepad.SaveFile();
        }

        private void SendFile(object sender, RoutedEventArgs e)
        {
            var user = Session.LoggedInUser;
            if (user == null)
            {
                MessageBox.Show("Musisz być zalogowany, aby wysłać plik.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (user.organizationId == null || user.organizationId == 0)
            {
                MessageBox.Show("Nie należysz do żadnej organizacji.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var dbUser = Database.GetUser(user.Login!);
            if (dbUser == null)
            {
                MessageBox.Show("Nie znaleziono użytkownika w bazie danych.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new OpenFileDialog
            {
                Title = "Wybierz plik do wysłania",
                Filter = "Wszystkie pliki (*.*)|*.*"
            };

            if (dialog.ShowDialog() != true)
                return;

            string filePath = dialog.FileName;
            string fileName = System.IO.Path.GetFileName(filePath);
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            Database.AddFile(fileName, fileBytes, dbUser.Id, user.organizationId.Value);
            MessageBox.Show($"Plik '{fileName}' został wysłany do bazy danych.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void GetFile(object sender, RoutedEventArgs e)
        {
            var user = Session.LoggedInUser;
            if (user == null)
            {
                MessageBox.Show("Musisz być zalogowany, aby pobierać pliki.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (user.organizationId == null || user.organizationId == 0)
            {
                MessageBox.Show("Nie należysz do żadnej organizacji.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var files = Database.GetFilesForOrganization(user.organizationId.Value);
            if (files.Count == 0)
            {
                MessageBox.Show("Brak plików w Twojej organizacji.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var selectWindow = new Window
            {
                Title = "Pobierz plik",
                Width = 400,
                Height = 300,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this
            };

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var listBox = new ListBox { Margin = new Thickness(10) };
            foreach (var f in files)
                listBox.Items.Add(new ListBoxItem { Content = f.Name.Trim(), Tag = f.Id });
            Grid.SetRow(listBox, 0);

            var btn = new Button
            {
                Content = "Pobierz wybrany plik",
                Margin = new Thickness(10),
                Padding = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Center
            };
            Grid.SetRow(btn, 1);

            btn.Click += (s, ev) =>
            {
                if (listBox.SelectedItem is not ListBoxItem selected)
                {
                    MessageBox.Show("Wybierz plik z listy.", "Informacja");
                    return;
                }

                int fileId = (int)selected.Tag;
                var fileData = Database.GetFileById(fileId);
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
                    selectWindow.Close();
                }
            };

            grid.Children.Add(listBox);
            grid.Children.Add(btn);
            selectWindow.Content = grid;
            selectWindow.ShowDialog();
        }

        private void OpenAdmin(object sender, RoutedEventArgs e)
        {
            var adminWindow = new WindowAdmin();
            adminWindow.Show();
            this.Close();
        }

        private void UkryjWszystkie()
        {
            int i = 0;

            Frame[] frames = new Frame[9];
            frames[0] = frameheader;
            frames[1] = framerule;
            frames[2] = framecodeblock;
            frames[3] = frameimage;
            frames[4] = frametable;
            frames[5] = frametasklist;
            frames[6] = frametextblock;
            frames[7] = framelistitem;
            frames[8] = framelist;

            while (i < frames.Length)
            {
                frames[i].Visibility = Visibility.Hidden;
                i++;
            }
        }

        private void selectheader(object sender, RoutedEventArgs e)
        {
            UkryjWszystkie();
            frameheader.Visibility = Visibility.Visible;

        }
        private void selectrule(object sender, RoutedEventArgs e)
        {
            UkryjWszystkie();
            framerule.Visibility = Visibility.Visible;
        }
        private void selectcodeblock(object sender, RoutedEventArgs e)
        {
            UkryjWszystkie();
            framecodeblock.Visibility = Visibility.Visible;
        }
        private void selectimage(object sender, RoutedEventArgs e)
        {
            UkryjWszystkie();
            frameimage.Visibility = Visibility.Visible;
        }
        private void selecttable(object sender, RoutedEventArgs e)
        {
            UkryjWszystkie();
            frametable.Visibility = Visibility.Visible;
        }
        private void selecttasklist(object sender, RoutedEventArgs e)
        {
            UkryjWszystkie();
            frametasklist.Visibility = Visibility.Visible;
        }
        private void selectTextBlock(object sender, RoutedEventArgs e)
        {
            UkryjWszystkie();
            frametextblock.Visibility = Visibility.Visible;
        }
        private void selectListItem(object sender, RoutedEventArgs e)
        {
            UkryjWszystkie();
            framelistitem.Visibility = Visibility.Visible;
        }
        private void selectlist(object sender, RoutedEventArgs e)
        {
            UkryjWszystkie();
            framelist.Visibility = Visibility.Visible;
        }

        private void Utworz_elem(object sender, RoutedEventArgs e)
        {

        }
    }
}
