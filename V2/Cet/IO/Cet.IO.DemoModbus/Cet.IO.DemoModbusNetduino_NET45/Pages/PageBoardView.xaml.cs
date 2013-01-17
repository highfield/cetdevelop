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
using System.Windows.Threading;

namespace Cet.IO.DemoModbusNetduino
{
    /// <summary>
    /// Interaction logic for PageBoardView.xaml
    /// </summary>
    public partial class PageBoardView : Page
    {
        public PageBoardView()
        {
            InitializeComponent();
            this.Loaded += ThisLoaded;
            this.PART_EnablePolling.Checked += EnablePollingChecked;
            this.PART_EnablePolling.Unchecked += EnablePollingUnchecked;
        }


        void ThisLoaded(object sender, RoutedEventArgs e)
        {
            if (HardwareModel.Instance.MediumType == CommMediumType.Rtu)
            {
                this.PART_Settings.Text = HardwareModel.Instance.SerialSettings;
                this.PART_Port.Text = HardwareModel.Instance.SerialPort;
            }
            else
            {
                this.PART_Settings.Text = HardwareModel.Instance.NetworkIP;
                this.PART_Port.Text = HardwareModel.Instance.NetworkPort.ToString();
            }

            switch (HardwareModel.Instance.Role)
            {
                case Role.Master:
                    this.IcUpper.ItemTemplate = (DataTemplate)this.Resources["dtplBoolInput"];
                    this.IcUpper.ItemsSource = HardwareModel.Instance
                        .Discretes
                        .Take(6)
                        .Reverse()
                        .ToArray();

                    this.IcLower.ItemTemplate = (DataTemplate)this.Resources["dtplBoolOutput"];
                    this.IcLower.ItemsSource = HardwareModel
                        .Instance
                        .Coils
                        .Reverse()
                        .ToArray();

                    this.IcAnalogs.ItemTemplate = (DataTemplate)this.Resources["dtplAnalogOutput"];
                    this.IcAnalogs.ItemsSource = HardwareModel.Instance.Analogs;

                    this.CtrPoller.Visibility = System.Windows.Visibility.Visible;
                    break;


                case Role.Slave:
                    this.IcUpper.ItemTemplate = (DataTemplate)this.Resources["dtplBoolInput"];
                    this.IcUpper.ItemsSource = HardwareModel.Instance
                        .Discretes
                        .Take(6)
                        .Reverse()
                        .ToArray();

                    this.IcLower.ItemTemplate = (DataTemplate)this.Resources["dtplBoolOutput"];
                    this.IcLower.ItemsSource = HardwareModel
                        .Instance
                        .Coils
                        .Reverse()
                        .ToArray();

                    this.IcAnalogs.ItemTemplate = (DataTemplate)this.Resources["dtplAnalogInput"];
                    this.IcAnalogs.ItemsSource = HardwareModel.Instance.Analogs;

                    //start listener
                    switch (HardwareModel.Instance.MediumType)
                    {
                        case CommMediumType.Tcp:
                            HardwareModel.Instance.StartWorker(new SlaveTcpWorker());
                            break;

                        case CommMediumType.Udp:
                            HardwareModel.Instance.StartWorker(new SlaveUdpWorker());
                            break;

                        case CommMediumType.Rtu:
                            HardwareModel.Instance.StartWorker(new SlaveRtuWorker());
                            break;
                    }

                    this.CtrPoller.Visibility = System.Windows.Visibility.Collapsed;
                    break;
            }
        }


        void EnablePollingChecked(object sender, RoutedEventArgs e)
        {
            switch (HardwareModel.Instance.MediumType)
            {
                case CommMediumType.Tcp:
                    HardwareModel.Instance.StartWorker(new MasterTcpWorker());
                    break;

                case CommMediumType.Udp:
                    HardwareModel.Instance.StartWorker(new MasterUdpWorker());
                    break;

                case CommMediumType.Rtu:
                    HardwareModel.Instance.StartWorker(new MasterRtuWorker());
                    break;
            }
        }


        void EnablePollingUnchecked(object sender, RoutedEventArgs e)
        {
            HardwareModel.Instance.StopWorker();
        }

    }
}
