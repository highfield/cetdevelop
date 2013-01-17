using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
    /// Interaction logic for PageNetworkSettings.xaml
    /// </summary>
    public partial class PageNetworkSettings : Page
    {
        public PageNetworkSettings()
        {
            InitializeComponent();
            this.Loaded += ThisLoaded;
        }


        void ThisLoaded(object sender, RoutedEventArgs e)
        {
            if (HardwareModel.Instance.Role == Role.Master)
            {
                this.PART_TxtNetworkSettings.Text = HardwareModel.Instance.NetworkIP;
            }
            else
            {
                this.PART_TxtNetworkSettings.Text = GetLocalIPAddress();
                this.PART_TxtNetworkSettings.IsEnabled = false;
            }

            this.PART_TxtNetworkPort.Text = HardwareModel.Instance.NetworkPort.ToString();
            this.PART_TxtAddress.Text = HardwareModel.Instance.Address.ToString();
        }


        private void ButtonGoNext(object sender, RoutedEventArgs e)
        {
            if (HardwareModel.Instance.Role == Role.Master)
            {
                //validate ip
                IPAddress ip;
                if (IPAddress.TryParse(this.PART_TxtNetworkSettings.Text, out ip) == false)
                {
                    this.VE_TxtNetworkSettings.Text = "The text cannot be recognized as a valid IP";
                    return;
                }
            }

            //validate port
            int port;
            if (int.TryParse(this.PART_TxtNetworkPort.Text, out port) == false ||
                port < 0 ||
                port > 65535)
            {
                this.VE_TxtNetworkPort.Text = "The port must be an integer ranging 0..65535 inclusive";
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
            if (HardwareModel.Instance.Role == Role.Master)
                HardwareModel.Instance.NetworkIP = this.PART_TxtNetworkSettings.Text;
            HardwareModel.Instance.NetworkPort = port;
            HardwareModel.Instance.Address = address;

            this.NavigationService
                .Navigate(new Uri("/Pages/PageBoardView.xaml", UriKind.Relative));
        }


        /// <summary>
        /// Get the local machine IP address
        /// </summary>
        /// <returns></returns>
        /// <remarks>http://stackoverflow.com/questions/6803073/get-local-ip-address-c-sharp</remarks>
        private static string GetLocalIPAddress()
        {
            IPHostEntry host;
            string localIP = string.Empty;
            host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                }
            }

            return localIP;
        }

    }
}
