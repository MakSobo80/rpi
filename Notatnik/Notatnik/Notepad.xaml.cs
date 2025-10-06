using Microsoft.Win32;
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
using System.IO;

namespace Notatnik
{
    public partial class Notepad : Window
    {
        //As when I am writing this UI is not implemented this var acts like text in TextBox (for OpenFile and SaveFile purposes)!
        public string wroteText = "";

        public List<Element> content = [];
        public User user = new();

        public Notepad()
        {
            InitializeComponent();
        }

        public void FormatText()
        {
            throw new NotImplementedException();
        }

        public string ParseIntoString()
        {
            throw new NotImplementedException();
        }

        public void OpenFile()
        {
            OpenFileDialog openFileDialog = new()
            {
                Title = "Select File",
                Filter = "All files (*.*)|*.*|Text File (*.txt)|*.txt|Markdown File (*.md)|*.md"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                string fileContent = File.ReadAllText(filePath);
                //TODO: CHANGE fileContent INTO LIST OF ELEMENTS AND SAVE IT AS SUCH
                wroteText = fileContent;
                MessageBox.Show($"{content}");
            }
        }

        public void SaveFile()
        {
            SaveFileDialog saveFileDialog = new() {
                Title = "Select Save File Destination",
                Filter = "All files (*.*)|*.*|Text File (*.txt)|*.txt|Markdown File (*.md)|*.md",
                FileName = "file.md"
            };
            if (saveFileDialog.ShowDialog() == true) { 
                string filePath = saveFileDialog.FileName;
                //TODO: Convert content into string using function
                string fileContent = wroteText;

                File.WriteAllText(filePath, fileContent);
                MessageBox.Show($"Saved file in: {filePath}");
            }
        }

        public void SendFile(int fileId, User user)
        {
            throw new NotImplementedException();
        }

        public void GetFile(int fileId)
        {
            throw new NotImplementedException();
        }

        public void AddElement(Element element)
        {
            throw new NotImplementedException();
        }

        public User Login()
        {
            throw new NotImplementedException();
        }
    }
}