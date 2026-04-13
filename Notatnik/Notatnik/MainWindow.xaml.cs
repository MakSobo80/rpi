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
        private string? _currentFilePath;

        public MainWindow()
        {
            InitializeComponent();
            notepad = new(TextContent, Pointer);
            TextContent.TextChanged += (object sender, TextChangedEventArgs e) =>
            {
                notepad.WrittenText = TextContent.Text;
            };
            Loaded += (s, ev) => RefreshFileTree();
            if (Session.LoggedInUser == null)
            {
                menuSendFile.IsEnabled = false;
                menuGetFile.IsEnabled = false;
            }
        }

        private void SaveCurrentFile()
        {
            if (_currentFilePath == null) return;
            try
            {
                File.WriteAllText(_currentFilePath, notepad.WrittenText);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SaveCurrentFile error: {ex}");
            }
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
            SaveCurrentFile();

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
                var overwritten = FileStorageHelper.DownloadDatabaseToFolder(user.organizationId.Value);
                RefreshFileTree();
                if (overwritten.Count > 0)
                {
                    string fileList = string.Join("\n", overwritten.Select(f => "  • " + System.IO.Path.GetFileName(f)));
                    MessageBox.Show(
                        $"Pliki zostały pobrane do folderu '{folder}'.\n\nNastępujące lokalne pliki zostały zastąpione wersją z bazy danych:\n{fileList}",
                        "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"Pliki zostały pobrane do folderu '{folder}'.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                }
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

        private void RefreshFileTree()
        {
            fileTree.Items.Clear();
            var user = Session.LoggedInUser;
            if (user == null || user.organizationId == null || user.organizationId == 0)
                return;

            string rootFolder = FileStorageHelper.GetOrgDataFolder(user.organizationId.Value);
            if (!Directory.Exists(rootFolder))
                return;

            fileTree.ContextMenu = BuildRootContextMenu(rootFolder);
            PopulateTreeItem(null, rootFolder);
        }

        private void RefreshTree_Click(object sender, RoutedEventArgs e) => RefreshFileTree();

        private ContextMenu BuildRootContextMenu(string rootFolder)
        {
            var menu = new ContextMenu();

            var createFile = new MenuItem { Header = "Utwórz plik" };
            createFile.Click += (s, e) =>
            {
                var dlg = new InputDialog("Nazwa nowego pliku:", "") { Owner = this };
                if (dlg.ShowDialog() != true) return;
                string newName = dlg.InputText.Trim();
                if (string.IsNullOrEmpty(newName)) return;
                if (string.IsNullOrEmpty(System.IO.Path.GetExtension(newName)))
                    newName += ".md";
                string newFilePath = System.IO.Path.Combine(rootFolder, newName);
                try
                {
                    File.WriteAllBytes(newFilePath, Array.Empty<byte>());
                    RefreshFileTree();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Nie można utworzyć pliku: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            var addFile = new MenuItem { Header = "Dodaj plik" };
            addFile.Click += (s, e) =>
            {
                var dlg = new Microsoft.Win32.OpenFileDialog { Title = "Wybierz plik do dodania" };
                if (dlg.ShowDialog() != true) return;
                string dest = System.IO.Path.Combine(rootFolder, System.IO.Path.GetFileName(dlg.FileName));
                try
                {
                    File.Copy(dlg.FileName, dest, overwrite: true);
                    RefreshFileTree();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Nie można skopiować pliku: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            var createFolder = new MenuItem { Header = "Utwórz folder" };
            createFolder.Click += (s, e) =>
            {
                var dlg = new InputDialog("Nazwa nowego folderu:", "") { Owner = this };
                if (dlg.ShowDialog() != true) return;
                string newName = dlg.InputText.Trim();
                if (string.IsNullOrEmpty(newName)) return;
                string newFolderPath = System.IO.Path.Combine(rootFolder, newName);
                try
                {
                    Directory.CreateDirectory(newFolderPath);
                    RefreshFileTree();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Nie można utworzyć folderu: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            menu.Items.Add(createFolder);
            menu.Items.Add(createFile);
            menu.Items.Add(addFile);
            return menu;
        }

        private void PopulateTreeItem(TreeViewItem? parent, string dirPath)
        {
            foreach (string subDir in Directory.GetDirectories(dirPath))
            {
                var dirItem = new TreeViewItem
                {
                    Header = System.IO.Path.GetFileName(subDir),
                    Tag = subDir,
                    IsExpanded = true
                };
                dirItem.ContextMenu = BuildFolderContextMenu(dirItem);
                PopulateTreeItem(dirItem, subDir);
                if (parent == null) fileTree.Items.Add(dirItem);
                else parent.Items.Add(dirItem);
            }
            foreach (string filePath in Directory.GetFiles(dirPath))
            {
                var fileItem = new TreeViewItem
                {
                    Header = System.IO.Path.GetFileName(filePath),
                    Tag = filePath
                };
                fileItem.ContextMenu = BuildFileContextMenu(fileItem);
                if (parent == null) fileTree.Items.Add(fileItem);
                else parent.Items.Add(fileItem);
            }
        }

        private ContextMenu BuildFolderContextMenu(TreeViewItem item)
        {
            var menu = new ContextMenu();

            var addFile = new MenuItem { Header = "Dodaj plik" };
            addFile.Click += (s, e) =>
            {
                if (item.Tag is not string folderPath) return;
                var dlg = new Microsoft.Win32.OpenFileDialog { Title = "Wybierz plik do dodania" };
                if (dlg.ShowDialog() != true) return;
                string dest = System.IO.Path.Combine(folderPath, System.IO.Path.GetFileName(dlg.FileName));
                try
                {
                    File.Copy(dlg.FileName, dest, overwrite: true);
                    RefreshFileTree();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Nie można skopiować pliku: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            var rename = new MenuItem { Header = "Zmień nazwę" };
            rename.Click += (s, e) =>
            {
                if (item.Tag is not string folderPath) return;
                string currentName = System.IO.Path.GetFileName(folderPath);
                var dlg = new InputDialog("Nowa nazwa folderu:", currentName) { Owner = this };
                if (dlg.ShowDialog() != true) return;
                string newName = dlg.InputText.Trim();
                if (string.IsNullOrEmpty(newName) || newName == currentName) return;
                string newPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(folderPath)!, newName);
                try
                {
                    Directory.Move(folderPath, newPath);
                    RefreshFileTree();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Nie można zmienić nazwy folderu: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            var delete = new MenuItem { Header = "Usuń folder" };
            delete.Click += (s, e) =>
            {
                if (item.Tag is not string folderPath) return;
                string name = System.IO.Path.GetFileName(folderPath);
                var result = MessageBox.Show(
                    $"Czy na pewno chcesz usunąć folder '{name}' wraz z całą jego zawartością?",
                    "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result != MessageBoxResult.Yes) return;
                try
                {
                    Directory.Delete(folderPath, recursive: true);
                    RefreshFileTree();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Nie można usunąć folderu: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            var createFile = new MenuItem { Header = "Utwórz plik" };
            createFile.Click += (s, e) =>
            {
                if (item.Tag is not string folderPath) return;
                var dlg = new InputDialog("Nazwa nowego pliku:", "") { Owner = this };
                if (dlg.ShowDialog() != true) return;
                string newName = dlg.InputText.Trim();
                if (string.IsNullOrEmpty(newName)) return;
                if (string.IsNullOrEmpty(System.IO.Path.GetExtension(newName)))
                    newName += ".md";
                string newFilePath = System.IO.Path.Combine(folderPath, newName);
                try
                {
                    File.WriteAllBytes(newFilePath, Array.Empty<byte>());
                    RefreshFileTree();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Nie można utworzyć pliku: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            var createFolder = new MenuItem { Header = "Utwórz folder" };
            createFolder.Click += (s, e) =>
            {
                if (item.Tag is not string folderPath) return;
                var dlg = new InputDialog("Nazwa nowego folderu:", "") { Owner = this };
                if (dlg.ShowDialog() != true) return;
                string newName = dlg.InputText.Trim();
                if (string.IsNullOrEmpty(newName)) return;
                string newFolderPath = System.IO.Path.Combine(folderPath, newName);
                try
                {
                    Directory.CreateDirectory(newFolderPath);
                    RefreshFileTree();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Nie można utworzyć folderu: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            menu.Items.Add(createFolder);
            menu.Items.Add(createFile);
            menu.Items.Add(addFile);
            menu.Items.Add(rename);
            menu.Items.Add(new Separator());
            menu.Items.Add(delete);
            return menu;
        }

        private ContextMenu BuildFileContextMenu(TreeViewItem item)
        {
            var menu = new ContextMenu();

            var rename = new MenuItem { Header = "Zmień nazwę" };
            rename.Click += (s, e) =>
            {
                if (item.Tag is not string filePath) return;
                string currentName = System.IO.Path.GetFileName(filePath);
                var dlg = new InputDialog("Nowa nazwa pliku:", currentName) { Owner = this };
                if (dlg.ShowDialog() != true) return;
                string newName = dlg.InputText.Trim();
                if (string.IsNullOrEmpty(newName) || newName == currentName) return;
                string newPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(filePath)!, newName);
                try
                {
                    File.Move(filePath, newPath);
                    RefreshFileTree();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Nie można zmienić nazwy pliku: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            var delete = new MenuItem { Header = "Usuń plik" };
            delete.Click += (s, e) =>
            {
                if (item.Tag is not string filePath) return;
                string name = System.IO.Path.GetFileName(filePath);
                var result = MessageBox.Show(
                    $"Czy na pewno chcesz usunąć plik '{name}'?",
                    "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result != MessageBoxResult.Yes) return;
                try
                {
                    File.Delete(filePath);
                    RefreshFileTree();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Nie można usunąć pliku: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            menu.Items.Add(rename);
            menu.Items.Add(new Separator());
            menu.Items.Add(delete);
            return menu;
        }

        private void FileTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is TreeViewItem item && item.Items.Count == 0 && item.Tag is string filePath)
            {
                if (File.Exists(filePath))
                {
                    try
                    {
                        SaveCurrentFile();
                        string content = File.ReadAllText(filePath);
                        _currentFilePath = filePath;
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