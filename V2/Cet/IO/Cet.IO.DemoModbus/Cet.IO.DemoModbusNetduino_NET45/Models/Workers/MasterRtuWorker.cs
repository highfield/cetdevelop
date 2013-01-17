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
    public class MasterRtuWorker
        : MasterWorkerBase
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

                //create a wrapper around the uart
                ICommClient medium = uart
                    .GetClient(prm);

                //create a client driver
                var driver = new ModbusClient(new ModbusRtuCodec());
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
