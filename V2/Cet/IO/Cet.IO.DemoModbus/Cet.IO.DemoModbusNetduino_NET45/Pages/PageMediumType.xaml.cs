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
    /// Interaction logic for PageMediumType.xaml
    /// </summary>
    public partial class PageMediumType : Page
    {
        public PageMediumType()
        {
            InitializeComponent();
            this.Loaded += ThisLoaded;
        }


        void ThisLoaded(object sender, RoutedEventArgs e)
        {
            //http://stackoverflow.com/questions/6803073/get-local-ip-address-c-sharp
            bool hasNetwork = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
            this.BtnTcp.IsEnabled = hasNetwork;
            this.BtnUdp.IsEnabled = hasNetwork;
        }


        private void MediumTcpClick(object sender, RoutedEventArgs e)
        {
            HardwareModel.Instance.MediumType = CommMediumType.Tcp;
            this.NavigationService
                .Navigate(new Uri("/Pages/PageNetworkSettings.xaml", UriKind.Relative));
        }


        private void MediumUdpClick(object sender, RoutedEventArgs e)
        {
            HardwareModel.Instance.MediumType = CommMediumType.Udp;
            this.NavigationService
                .Navigate(new Uri("/Pages/PageNetworkSettings.xaml", UriKind.Relative));
        }


        private void MediumRtuClick(object sender, RoutedEventArgs e)
        {
            HardwareModel.Instance.MediumType = CommMediumType.Rtu;
            this.NavigationService
                .Navigate(new Uri("/Pages/PageSerialSettings.xaml", UriKind.Relative));
        }

    }
}
