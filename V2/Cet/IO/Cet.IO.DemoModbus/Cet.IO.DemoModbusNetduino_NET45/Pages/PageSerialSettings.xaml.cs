using System;
using System.Collections.Generic;
using System.IO.Ports;
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
    /// Interaction logic for PageSerialSettings.xaml
    /// </summary>
    public partial class PageSerialSettings : Page
    {
        public PageSerialSettings()
        {
            InitializeComponent();
            this.Loaded += ThisLoaded;
        }


        void ThisLoaded(object sender, RoutedEventArgs e)
        {
            this.PART_CboSerialPort.ItemsSource = SerialPort.GetPortNames();
            this.PART_CboSerialPort.SelectedItem = HardwareModel.Instance.SerialPort;
            this.PART_TxtSerialSettings.Text = HardwareModel.Instance.SerialSettings;
            this.PART_TxtAddress.Text = HardwareModel.Instance.Address.ToString();
        }


        private void ButtonPortRefresh(object sender, RoutedEventArgs e)
        {
            this.PART_CboSerialPort.ItemsSource = SerialPort.GetPortNames();
        }


        private void ButtonGoNext(object sender, RoutedEventArgs e)
        {
            //validate port
            string port;
            if ((port = this.PART_CboSerialPort.SelectedItem as string) == null ||
                SerialPort.GetPortNames().Contains(port) == false)
            {
                this.VE_CboSerialPort.Text = "Please select a valid port";
                return;
            }

            //validate address
            int address;
            if (int.TryParse(this.PART_TxtAddress.Text, out address) == false ||
                address < 0 ||
                address > 247)
            {
                this.VE_TxtAddress.Text = "The address must be an integer ranging 0..247 inclusive";
                return;
            }

            //commit entries
            HardwareModel.Instance.SerialSettings = this.PART_TxtSerialSettings.Text;
            HardwareModel.Instance.SerialPort = port;
            HardwareModel.Instance.Address = address;

            this.NavigationService
                .Navigate(new Uri("/Pages/PageBoardView.xaml", UriKind.Relative));
        }

    }
}
