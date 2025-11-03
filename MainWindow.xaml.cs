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
        public MainWindow()
        {
            InitializeComponent();
        }

    
    private void selectheader(object sender, RoutedEventArgs e)
        {
            frameheader.Visibility = Visibility.Visible;
            framerule.Visibility = Visibility.Hidden;
            framecodeblock.Visibility = Visibility.Hidden;
            frameimage.Visibility = Visibility.Hidden;
            frametable.Visibility = Visibility.Hidden;
            frametasklist.Visibility = Visibility.Hidden;
            frametextblock.Visibility = Visibility.Hidden;
            framelistitem.Visibility = Visibility.Hidden;
            framelist.Visibility = Visibility.Hidden;

        }
        private void selectrule(object sender, RoutedEventArgs e)
        {
            frameheader.Visibility = Visibility.Hidden;
            framerule.Visibility = Visibility.Visible;
            framecodeblock.Visibility = Visibility.Hidden;
            frameimage.Visibility = Visibility.Hidden;
            frametable.Visibility = Visibility.Hidden;
            frametasklist.Visibility = Visibility.Hidden;
            frametextblock.Visibility = Visibility.Hidden;
            framelistitem.Visibility = Visibility.Hidden;
            framelist.Visibility = Visibility.Hidden;
        }
        private void selectcodeblock(object sender, RoutedEventArgs e)
        {
            frameheader.Visibility = Visibility.Hidden;
            framerule.Visibility = Visibility.Hidden;
            framecodeblock.Visibility = Visibility.Visible;
            frameimage.Visibility = Visibility.Hidden;
            frametable.Visibility = Visibility.Hidden;
            frametasklist.Visibility = Visibility.Hidden;
            frametextblock.Visibility = Visibility.Hidden;
            framelistitem.Visibility = Visibility.Hidden;
            framelist.Visibility = Visibility.Hidden;
        }
        private void selectimage(object sender, RoutedEventArgs e)
        {
            frameheader.Visibility = Visibility.Hidden;
            framerule.Visibility = Visibility.Hidden;
            framecodeblock.Visibility = Visibility.Hidden;
            frameimage.Visibility = Visibility.Visible;
            frametable.Visibility = Visibility.Hidden;
            frametasklist.Visibility = Visibility.Hidden;
            frametextblock.Visibility = Visibility.Hidden;
            framelistitem.Visibility = Visibility.Hidden;
            framelist.Visibility = Visibility.Hidden;
        }
        private void selecttable(object sender, RoutedEventArgs e)
        {
            frameheader.Visibility = Visibility.Hidden;
            framerule.Visibility = Visibility.Hidden;
            framecodeblock.Visibility = Visibility.Hidden;
            frameimage.Visibility = Visibility.Hidden;
            frametable.Visibility = Visibility.Visible;
            frametasklist.Visibility = Visibility.Hidden;
            frametextblock.Visibility = Visibility.Hidden;
            framelistitem.Visibility = Visibility.Hidden;
            framelist.Visibility = Visibility.Hidden;
        }
        private void selecttasklist(object sender, RoutedEventArgs e)
        {
            frameheader.Visibility = Visibility.Hidden;
            framerule.Visibility = Visibility.Hidden;
            framecodeblock.Visibility = Visibility.Hidden;
            frameimage.Visibility = Visibility.Hidden;
            frametable.Visibility = Visibility.Hidden;
            frametasklist.Visibility = Visibility.Visible;
            frametextblock.Visibility = Visibility.Hidden;
            framelistitem.Visibility = Visibility.Hidden;
            framelist.Visibility = Visibility.Hidden;
        }
        private void selectTextBlock(object sender, RoutedEventArgs e)
        {
            frameheader.Visibility = Visibility.Hidden;
            framerule.Visibility = Visibility.Hidden;
            framecodeblock.Visibility = Visibility.Hidden;
            frameimage.Visibility = Visibility.Hidden;
            frametable.Visibility = Visibility.Hidden;
            frametasklist.Visibility = Visibility.Hidden;
            frametextblock.Visibility = Visibility.Visible;
            framelistitem.Visibility = Visibility.Hidden;
            framelist.Visibility = Visibility.Hidden;
        }
        private void selectListItem(object sender, RoutedEventArgs e)
        {
            frameheader.Visibility = Visibility.Hidden;
            framerule.Visibility = Visibility.Hidden;
            framecodeblock.Visibility = Visibility.Hidden;
            frameimage.Visibility = Visibility.Hidden;
            frametable.Visibility = Visibility.Hidden;
            frametasklist.Visibility = Visibility.Hidden;
            frametextblock.Visibility = Visibility.Hidden;
            framelistitem.Visibility = Visibility.Visible;
            framelist.Visibility = Visibility.Hidden;
        }
        private void selectlist(object sender, RoutedEventArgs e)
        {
            frameheader.Visibility = Visibility.Hidden;
            framerule.Visibility = Visibility.Hidden;
            framecodeblock.Visibility = Visibility.Hidden;
            frameimage.Visibility = Visibility.Hidden;
            frametable.Visibility = Visibility.Hidden;
            frametasklist.Visibility = Visibility.Hidden;
            frametextblock.Visibility = Visibility.Hidden;
            framelistitem.Visibility = Visibility.Hidden;
            framelist.Visibility = Visibility.Visible;
        }


    }
}