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
    public abstract class SlaveWorkerBase
        : IWorker
    {
        protected SlaveWorkerBase() { }


        public abstract void Run(CancellationToken token);


        /// <summary>
        /// Host server for the incoming request from a remote master
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ListenerServeCommand(object sender, ServeCommandEventArgs e)
        {
            var command = (ModbusCommand)e.Data.UserData;

            //take the proper function command handler
            switch (command.FunctionCode)
            {
                case ModbusCommand.FuncReadInputDiscretes:
                    for (int i = 0; i < command.Count; i++)
                    {
                        InputPort input = HardwareModel.Instance.Discretes[i + command.Offset];
                        if (input != null)
                            command.Data[i] = (ushort)(input.Read() ? 1 : 0);
                    }
                    break;


                case ModbusCommand.FuncWriteCoil:
                    bool state = command.Data[0] != 0;
                    HardwareModel.Instance.Coils[command.Offset].Write(state);
                    break;


                case ModbusCommand.FuncForceMultipleCoils:
                    for (int i = 0; i < command.Count; i++)
                        HardwareModel.Instance.Coils[i + command.Offset].Write(command.Data[i] != 0);
                    break;


                case ModbusCommand.FuncReadInputRegisters:
                    for (int i = 0; i < command.Count; i++)
                    {
                        var analog = HardwareModel.Instance.Analogs[i + command.Offset];
                        command.Data[i] = (ushort)analog.Read();
                    }
                    break;


                case ModbusCommand.FuncReadCoils:
                case ModbusCommand.FuncReadMultipleRegisters:
                case ModbusCommand.FuncWriteMultipleRegisters:
                case ModbusCommand.FuncWriteSingleRegister:
                case ModbusCommand.FuncReadExceptionStatus:
                    //TODO
                    break;


                default:
                    //return an exception
                    command.ExceptionCode = ModbusCommand.ErrorIllegalFunction;
                    break;
            }
        }

    }
}
