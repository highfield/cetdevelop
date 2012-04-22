using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Threading.Tasks;

using Cet.Develop.Windows.IO.Net;
using Cet.Develop.Windows.IO.Protocols;


namespace Cet.Develop.Windows.IO
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            this.txtRemoteHost.Text = "192.168.0.99";
        }



        /// <summary>
        /// Handle the btnEcho_Click event by sending text to the echo server and outputting the response
        /// </summary>
        private async void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            // Make sure we can perform this action with valid data
            if (ValidateRemoteHost())
            {
                //disable UI
                this.txtRemoteHost.IsEnabled = false;
                this.btnConnect.IsEnabled = false;
                this.btnDisconnect.IsEnabled = true;

                //show progression status
                this.pbarConnect.Value = 0;
                this.pbarConnect.Visibility = System.Windows.Visibility.Visible;
                this.pbarConnect.IsIndeterminate = true;

                this.txtStatus.Text = "Connecting...";

                // Instantiate the SocketClient
                var connected = await this.ConnectAsync(
                    txtRemoteHost.Text,
                    502);

                //hide the progress bar
                this.pbarConnect.Visibility = System.Windows.Visibility.Collapsed;

                if (connected)
                {
                    this.txtStatus.Text = "Connection succeeded.";

                    App.Modbus = new Protocols.ModbusClient(new ModbusTcpCodec());
                    App.Modbus.Address = 1;

                    await TaskEx.Delay(1000);
                    NavigationService.Navigate(new Uri("/BoardLayoutPage.xaml", UriKind.Relative));
                }
                else
                {
                    this.txtStatus.Text = App.Client.LastError;
                    this.btnDisconnect.IsEnabled = false;
                }

                //enable UI
                this.txtRemoteHost.IsEnabled = true;
                this.btnConnect.IsEnabled = true;
#if false
                // Attempt to connect to the echo server
                Log(String.Format("Connecting to server '{0}' over port {1} (echo) ...", txtRemoteHost.Text, ECHO_PORT), true);
                string result = client.Connect(txtRemoteHost.Text, ECHO_PORT);
                Log(result, false);

                // Attempt to send our message to be echoed to the echo server
                Log(String.Format("Sending '{0}' to server ...", txtInput.Text), true);
                result = client.Send(txtInput.Text);
                Log(result, false);

                // Receive a response from the echo server
                Log("Requesting Receive ...", true);
                result = client.Receive();
                Log(result, false);

                // Close the socket connection explicitly
                client.Close();
#endif
            }

        }



        private void btnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            this.txtStatus.Text = "Connection closed.";
            App.Client.Close();
        }



        private async Task<bool> ConnectAsync(string host, int port)
        {
            return await TaskEx.Run(() => App.Client.Connect(host, port));
        }



        #region UI Validation

        /// <summary>
        /// Validates the txtRemoteHost TextBox
        /// </summary>
        /// <returns>True if the txtRemoteHost contains valid data, False otherwise</returns>
        private bool ValidateRemoteHost()
        {
            // The txtRemoteHost must contain some text
            if (String.IsNullOrWhiteSpace(txtRemoteHost.Text))
            {
                MessageBox.Show("Please enter a host name");
                return false;
            }

            return true;
        }

        #endregion
    }
}