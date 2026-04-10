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
            Loaded += (s, ev) => RefreshFileTree();
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
                MessageBox.Show("Musisz być zalogowany, aby wysłać pliki.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
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

            try
            {
                string folder = FileStorageHelper.GetOrgDataFolder(user.organizationId.Value);
                FileStorageHelper.UploadFolderToDatabase(user.organizationId.Value, dbUser.Id);
                MessageBox.Show($"Pliki z folderu '{folder}' zostały wysłane do bazy danych.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas wysyłania plików: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                System.Diagnostics.Debug.WriteLine($"SendFile error: {ex}");
            }
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

            try
            {
                string folder = FileStorageHelper.GetOrgDataFolder(user.organizationId.Value);
                FileStorageHelper.DownloadDatabaseToFolder(user.organizationId.Value);
                RefreshFileTree();
                MessageBox.Show($"Pliki zostały pobrane do folderu '{folder}'.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas pobierania plików: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                System.Diagnostics.Debug.WriteLine($"GetFile error: {ex}");
            }
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

        private void RefreshFileTree()
        {
            fileTree.Items.Clear();
            var user = Session.LoggedInUser;
            if (user == null || user.organizationId == null || user.organizationId == 0)
                return;

            string rootFolder = FileStorageHelper.GetOrgDataFolder(user.organizationId.Value);
            if (!Directory.Exists(rootFolder))
                return;

            var rootItem = new TreeViewItem
            {
                Header = System.IO.Path.GetFileName(rootFolder),
                Tag = rootFolder,
                IsExpanded = true
            };
            PopulateTreeItem(rootItem, rootFolder);
            fileTree.Items.Add(rootItem);
        }

        private void PopulateTreeItem(TreeViewItem parent, string dirPath)
        {
            foreach (string subDir in Directory.GetDirectories(dirPath))
            {
                var dirItem = new TreeViewItem
                {
                    Header = System.IO.Path.GetFileName(subDir),
                    Tag = subDir,
                    IsExpanded = true
                };
                PopulateTreeItem(dirItem, subDir);
                parent.Items.Add(dirItem);
            }
            foreach (string filePath in Directory.GetFiles(dirPath))
            {
                parent.Items.Add(new TreeViewItem
                {
                    Header = System.IO.Path.GetFileName(filePath),
                    Tag = filePath
                });
            }
        }

        private void FileTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is TreeViewItem item && item.Items.Count == 0 && item.Tag is string filePath)
            {
                if (File.Exists(filePath))
                {
                    try
                    {
                        string content = File.ReadAllText(filePath);
                        notepad.WrittenText = content;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Nie można odczytać pliku: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                        System.Diagnostics.Debug.WriteLine($"FileTree_SelectedItemChanged error: {ex}");
                    }
                }
            }
        }
    }
}