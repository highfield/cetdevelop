using System;

using Cet.IO.Hashing;

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
namespace Cet.IO.Protocols
{
    public class ModbusRtuCodec
        : ModbusCodecBase, IProtocolCodec
    {
        #region Crc16 function access

        private static HashCrc16 _crc16;

        /// <summary>
        /// Give the function for calculating the CRC16 (0xA001)
        /// over a byte array
        /// </summary>
        public static HashCrc16 Crc16
        {
            get
            {
                if (_crc16 == null)
                    _crc16 = new HashCrc16();

                return _crc16;
            }
        }

        #endregion


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
            ushort crc;
            unchecked
            {
                crc = (ushort)ModbusRtuCodec.Crc16.Compute(
                    ((IByteArray)writer).Data,
                    0,
                    writer.Length
                    );
            }

            writer.WriteUInt16LE(crc);

            data.OutgoingData = writer.ToReader();
        }


        CommResponse IProtocolCodec.ClientDecode(CommDataBase data)
        {
            var client = (ModbusClient)data.OwnerProtocol;
            var command = (ModbusCommand)data.UserData;
            ByteArrayReader incoming = data.IncomingData;
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
                ushort crc;
                unchecked
                {
                    crc = (ushort)ModbusRtuCodec.Crc16.Compute(
                        ((IByteArray)incoming).Data,
                        0,
                        incoming.Length - 2);
                }

                //validate the CRC-16
                if (incoming.ReadUInt16LE() == crc)
                {
                    //message looks consistent (although the body can be empty)
                    if ((fncode & 0x7F) == command.FunctionCode)
                    {
                        if (fncode <= 0x7F)
                        {
                            //encode the command body, if applies
                            var codec = CommandCodecs[fncode];
                            if (codec != null &&
                                codec.ClientDecode(command, body))
                            {
                                return new CommResponse(
                                    data,
                                    CommResponse.Ack);
                            }
                        }
                        else
                        {
                            //exception
                            command.ExceptionCode = body.ReadByte();

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


        void IProtocolCodec.ServerEncode(CommDataBase data)
        {
            var server = (ModbusServer)data.OwnerProtocol;
            var command = (ModbusCommand)data.UserData;
            var fncode = command.FunctionCode;

            //encode the command body, if applies
            var body = new ByteArrayWriter();
            var codec = CommandCodecs[fncode];
            if (codec != null)
                codec.ServerEncode(command, body);

            //calculate length field
            var length = (command.ExceptionCode == 0)
                ? 2 + body.Length
                : 3;

            //create a writer for the outgoing data
            var writer = new ByteArrayWriter();

            //unit identifier (address)
            writer.WriteByte(server.Address);

            if (command.ExceptionCode == 0)
            {
            //function code
            writer.WriteByte(fncode);

            //body data
            writer.WriteBytes(body);
            }
            else
            {
                //function code
                writer.WriteByte((byte)(command.FunctionCode | 0x80));

                //exception code
                writer.WriteByte(command.ExceptionCode);
            }

            //CRC-16
            ushort crc;
            unchecked
            {
                crc = (ushort)ModbusRtuCodec.Crc16.Compute(
                    ((IByteArray)writer).Data,
                    0,
                    writer.Length);
            }

            writer.WriteUInt16LE(crc);

            data.OutgoingData = writer.ToReader();
        }


        CommResponse IProtocolCodec.ServerDecode(CommDataBase data)
        {
            var server = (ModbusServer)data.OwnerProtocol;
            ByteArrayReader incoming = data.IncomingData;

            //validate header first
            var length = incoming.Length;
            if (length < 4)
                goto LabelUnknown;

            //address
            var address = incoming.ReadByte();

            if (address == server.Address)
            {
                //function code
                var fncode = incoming.ReadByte();

                if (fncode < CommandCodecs.Length)
                {
                    //create a new command
                    var command = new ModbusCommand(fncode);
                    data.UserData = command;
                    command.QueryTotalLength = 2; //= addr + fn (no crc)

                    //get the command codec
                    var codec = CommandCodecs[fncode];

                    //decode the command, where possible
                    var body = new ByteArrayReader(incoming.ReadBytes(length - 4));

                    if (codec.ServerDecode(command, body))
                    {
                        //calculate the CRC-16 over the received stream
                        ushort crcCalc;
                        unchecked
                        {
                            crcCalc = (ushort)ModbusRtuCodec.Crc16.Compute(
                                ((IByteArray)incoming).Data,
                                0,
                                command.QueryTotalLength);
                        }

                        //validate the CRC-16
                        ushort crcRead = ByteArrayHelpers.ReadUInt16LE(
                            ((IByteArray)incoming).Data,
                            command.QueryTotalLength);

                        if (crcRead == crcCalc)
                        {
                            return new CommResponse(
                                data,
                                CommResponse.Ack);
                        }
                    }
                }
            }

            //exception
            return new CommResponse(
                data,
                CommResponse.Ignore);

        LabelUnknown:
            return new CommResponse(
                data,
                CommResponse.Unknown);
        }

        #endregion

    }
}
