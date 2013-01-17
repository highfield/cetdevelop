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
    public class SlaveUdpWorker
        : SlaveWorkerBase
    {
        public override void Run(CancellationToken token)
        {
            //create a UDP socket
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                //place it as listener on the specified port
                var ept = new IPEndPoint(
                    IPAddress.Any, 
                    HardwareModel.Instance.NetworkPort
                    );

                socket.Bind(ept);

                //create a server driver
                var server = new ModbusServer(new ModbusTcpCodec());
                server.Address = (byte)HardwareModel.Instance.Address;

                while (token.IsCancellationRequested == false)
                {
                    //wait for an incoming connection
                    var listener = socket.GetUdpListener(server);
                    listener.ServeCommand += new ServeCommandHandler(ListenerServeCommand);
                    listener.Start();

                    while (listener.IsRunning)
                    {
                        if (token.IsCancellationRequested)
                            listener.Abort();
                        else
                            Thread.Sleep(1);
                    }
                }
            }
        }

    }
}
