using System;
using System.Linq;

/*
 * Copyright 2012 Mario Vernari (http://cetdevelop.codeplex.com/)
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
namespace Cet.Develop.Windows.IO.Protocols
{
    public class ModbusRtuCodec
        : ModbusCodecBase, IProtocolCodec
    {
        #region IProtocolCodec members

        void IProtocolCodec.ClientEncode(CommDataBase data)
        {
            var client = (ModbusClient)data.OwnerProtocol;
            var command = (ModbusCommand)data.UserData;
            var fncode = command.FunctionCode;

            //encode the command body, if applies
            var body = new ByteArrayWriter();
            var codec = CommandCodecs[fncode];
            if (codec != null)
                codec.ClientEncode(command, body);

            //create a writer for the outgoing data
            var writer = new ByteArrayWriter();

            //unit identifier (address)
            writer.WriteByte(client.Address);

            //function code
            writer.WriteByte(fncode);

            //body data
            writer.WriteBytes(body);

            //CRC-16
            ushort crc = ByteArrayHelpers.CalcCRC16(
                writer.ToArray(),
                0,
                writer.Length);

            writer.WriteInt16LE((short)crc);

            data.OutgoingData = writer.ToReader();
        }



        CommResponse IProtocolCodec.ClientDecode(CommDataBase data)
        {
            var client = (ModbusClient)data.OwnerProtocol;
            var command = (ModbusCommand)data.UserData;
            var incoming = data.IncomingData;
            var bodyLen = incoming.Length - 4;

            //validate address first
            if (bodyLen >= 0 &&
                incoming.ReadByte() == client.Address)
            {
                //extract function code
                var fncode = incoming.ReadByte();

                //extract the message body
                var body = new ByteArrayReader(incoming.ReadBytes(bodyLen));

                //calculate the CRC-16 over the received stream
                ushort crc = ByteArrayHelpers.CalcCRC16(
                    incoming.ToArray(),
                    2,
                    incoming.Length - 2);

                //validate the CRC-16
                if (incoming.ReadInt16LE() == (short)crc)
                {
                    //message looks consistent (although the body can be empty)
                    if ((fncode & 0x7F) == command.FunctionCode)
                    {
                        if (fncode <= 0x7F)
                        {
                            //
                            //encode the command body, if applies
                            var codec = CommandCodecs[fncode];
                            if (codec != null)
                                codec.ClientDecode(command, body);

                            return new CommResponse(
                                data,
                                CommResponse.Ack);
                        }
                        else
                        {
                            //exception
                            command.ExceptionCode = incoming.ReadByte();

                            return new CommResponse(
                                data,
                                CommResponse.Critical);
                        }
                    }
                }
            }

            return new CommResponse(
                data,
                CommResponse.Unknown);
        }

        #endregion

    }
}
