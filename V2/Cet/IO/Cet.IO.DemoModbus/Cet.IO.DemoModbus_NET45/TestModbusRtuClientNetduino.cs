using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Cet.IO.Protocols;
using Cet.IO;
using Cet.IO.Serial;

/*
 * Copyright 2012, 2013 by Mario Vernari, Cet Electronics
 * Part of "Cet Open Toolbox" (http://cetdevelop.codeplex.com/)
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace Cet.IO.DemoModbus
{
    class TestModbusRtuClientNetduino
        : ModbusClient
    {
        private TestModbusRtuClientNetduino()
            : base(new ModbusRtuCodec())
        {
        }


        public string Name { get; set; }


        public async Task<CommResponse> ExecuteWriteMulti(ICommClient client)
        {
            Func<CommResponse> fn = () =>
            {
                //compose the Modbus command to be submitted
                var command = new ModbusCommand(ModbusCommand.FuncReadInputDiscretes);
                command.Offset = 0;
                command.Count = 4;

                return this.ExecuteGeneric(
                    client,
                    command);
            };

            return await Task.Run(() => fn());
        }


        public async static void DoTest()
        {
            using (var port= new SerialPortEx())
            {
                port.PortName = "COM3";
                port.Open();
                Console.WriteLine("PortOpen={0}", port.IsOpen);

                var portSetting = new SerialPortParams(
                    "38400,E,8,1",
                    rtsEnable: false);

                ICommClient portClient = port.GetClient(portSetting);

                var clients = Enumerable
                    .Range(0, 10)
                    .Select(_ =>
                        new TestModbusRtuClientNetduino
                        {
                            Name = "Modbus client #" + _,
                            Address = 0x05,
                        })
                        .ToList();

                CommResponse[] result = await Task.WhenAll(
                    clients
                    .Select(_ => _.ExecuteWriteMulti(portClient))
                    );

                foreach (var r in result)
                {
                    var owner = (TestModbusRtuClientNetduino)r.Data.OwnerProtocol;
                    string hex = null;

                    if (r.Data.IncomingData != null)
                    {
                        hex = ByteArrayHelpers.ToHex(r.Data.IncomingData.ToByteArray());
                    }

                    Console.WriteLine(
                        "Owner={0}; {1}; {2}",
                        owner.Name,
                        r.Status,
                        hex
                        );
                }
            }
        }

    }
}
