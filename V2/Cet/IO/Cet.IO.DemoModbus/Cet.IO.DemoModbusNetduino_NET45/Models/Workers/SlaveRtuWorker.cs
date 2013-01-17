using Cet.IO.Protocols;
using Cet.IO.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Cet.IO.DemoModbusNetduino
{
    public class SlaveRtuWorker
        : SlaveWorkerBase
    {
        public override void Run(CancellationToken token)
        {
            //create an UART port
            using (var uart = new SerialPortEx())
            {
                //open the serial port
                uart.PortName = HardwareModel.Instance.SerialPort;
                uart.Open();

                var prm = new SerialPortParams(
                    HardwareModel.Instance.SerialSettings,
                    false);

                uart.SetParams(prm);

                //create a server driver
                var server = new ModbusServer(new ModbusRtuCodec());
                server.Address = (byte)HardwareModel.Instance.Address;

                while (token.IsCancellationRequested == false)
                {
                    //wait for an incoming connection
                    var listener = uart.GetServer(server);
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
