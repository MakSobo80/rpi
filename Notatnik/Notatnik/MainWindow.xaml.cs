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
            notepad = new Notepad(TextContent);
            TextContent.TextChanged += (object sender, TextChangedEventArgs e) =>
            {
                notepad.WrittenText = TextContent.Text;
            };
            var user = SessionData.CurrentUser;
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