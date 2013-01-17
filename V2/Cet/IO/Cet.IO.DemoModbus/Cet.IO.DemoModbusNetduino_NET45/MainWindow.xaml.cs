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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cet.IO.DemoModbusNetduino
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;
        }


        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Frame1.Navigating += Frame1_Navigating;
            this.Frame1.Navigate(new Uri("/Pages/PageSelectRole.xaml", UriKind.Relative));
        }


        void MainWindow_Closed(object sender, EventArgs e)
        {
            HardwareModel.Instance.StopWorker();
        }


        void Frame1_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                HardwareModel.Instance.StopWorker();
            }
        }


        private void GoBack(object sender, EventArgs e)
        {
            
        }

    }
}
