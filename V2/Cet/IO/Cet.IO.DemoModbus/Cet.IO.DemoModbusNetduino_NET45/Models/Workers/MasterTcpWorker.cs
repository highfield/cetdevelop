using Cet.IO.Protocols;
using Cet.IO.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cet.IO.DemoModbusNetduino
{
    public class MasterTcpWorker
        : MasterWorkerBase
    {
        public override void Run(CancellationToken token)
        {
            //create a TCP socket
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                //refine the socket settings
                socket.SetSocketOption(
                    SocketOptionLevel.Tcp,
                    SocketOptionName.NoDelay,
                    true
                    );

                socket.SendTimeout = 2000;
                socket.ReceiveTimeout = 2000;

                //try the connection against the remote device
                var ipaddr = IPAddress.Parse(HardwareModel.Instance.NetworkIP);
                var ept = new IPEndPoint(ipaddr, HardwareModel.Instance.NetworkPort);
                socket.Connect(ept);

                //create a wrapper around the socket
                ICommClient medium = socket
                    .GetClient();

                //create a client driver
                var driver = new ModbusClient(new ModbusTcpCodec());
                driver.Address = (byte)HardwareModel.Instance.Address;

                //run actual task
                base.RunCore(
                    medium,
                    driver,
                    token);
            }
        }

    }
}
