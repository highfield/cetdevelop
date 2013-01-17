using Cet.IO.Protocols;
using Cet.IO.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;


namespace Cet.IO.DemoModbusNetduino
{
    public abstract class MasterWorkerBase
        : IWorker
    {
        protected MasterWorkerBase() { }


        public abstract void Run(CancellationToken token);


        protected void RunCore(
            ICommClient medium,
            ModbusClient driver,
            CancellationToken token)
        {
            Action<bool> setPollingActivity = act => HardwareModel.Instance.IsPollingEnabled = act;

            int stage = 0;

            while (token.IsCancellationRequested == false)
            {
                //turn the polling activity on
                App.Current.Dispatcher.BeginInvoke(
                    setPollingActivity,
                    true);

                if (stage == 0)
                {
                    //compose the Modbus command to be submitted
                    var command = new ModbusCommand(ModbusCommand.FuncReadInputDiscretes);
                    command.Offset = 0;
                    command.Count = 8;

                    //execute the command synchronously
                    CommResponse result = driver
                        .ExecuteGeneric(medium, command);

                    if (result.Status == CommResponse.Ack)
                    {
                        //command successfully
                        for (int i = 0; i < command.Count; i++)
                            HardwareModel.Instance.Coils[i].Write(command.Data[i] != 0);

                        stage = 1;
                    }
                    else
                    {
                        //some error
                        Console.WriteLine("Status={0}; Error={1}", result.Status, command.ExceptionCode);
                    }
                }
                else if (stage == 1)
                {
                    //compose the Modbus command to be submitted
                    var command = new ModbusCommand(ModbusCommand.FuncReadInputRegisters);
                    command.Offset = 0;
                    command.Count = 6;

                    //execute the command synchronously
                    CommResponse result = driver
                        .ExecuteGeneric(medium, command);

                    if (result.Status == CommResponse.Ack)
                    {
                        //command successfully
                        for (int i = 0; i < command.Count; i++)
                            HardwareModel.Instance.Analogs[i].Value = command.Data[i] / 1023.0;

                        stage = 2;
                    }
                    else
                    {
                        //some error
                        Console.WriteLine("Status={0}; Error={1}", result.Status, command.ExceptionCode);
                    }
                }
                else if (stage == 2)
                {
                    //compose the Modbus command to be submitted
                    var command = new ModbusCommand(ModbusCommand.FuncForceMultipleCoils);
                    command.Offset = 0;
                    command.Count = 6;

                    //attach the Netduino's input values as data
                    command.Data = new ushort[command.Count];
                    for (int i = 0; i < command.Count; i++)
                        command.Data[i] = (ushort)(HardwareModel.Instance.Discretes[i].Read() ? 1 : 0);

                    //execute the command synchronously
                    CommResponse result = driver
                        .ExecuteGeneric(medium, command);

                    if (result.Status == CommResponse.Ack)
                    {
                        //command successfully
                        stage = 0;
                    }
                    else
                    {
                        //some error
                        Console.WriteLine("Status={0}; Error={1}", result.Status, command.ExceptionCode);
                    }
                }

                //turn the polling activity off
                App.Current.Dispatcher.Invoke(
                    setPollingActivity, 
                    false);

                //just a small delay
                Thread.Sleep(100);
            }
        }

    }
}
