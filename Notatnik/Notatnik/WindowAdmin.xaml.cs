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

namespace Notatnik
{
    /// <summary>
    /// Logika interakcji dla klasy WindowAdmin.xaml
    /// </summary>
    public partial class WindowAdmin : Window
    {
        public WindowAdmin()
        {
            InitializeComponent();
        }

        private void Notat_guzik(object sender, RoutedEventArgs e)
        {
            var mainwindow = new MainWindow();
            mainwindow.Show();

            this.Close();
        }

        private void Org_guzik(object sender, RoutedEventArgs e)
        {
            var orgwindow = new windoworg();
            orgwindow.Show();

            this.Close();
        }
    }
}
