using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

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
namespace Cet.Develop.NETMF.IO.Protocols
{
    public class ModbusTcpCodec
        : IProtocolCodec
    {
        static ModbusTcpCodec()
        {
            //fill the local array with the curretly supported commands
            CommandCodecs[ModbusCommand.FuncReadMultipleRegisters] = new ModbusTcpCodecReadMultipleRegisters();
            CommandCodecs[ModbusCommand.FuncWriteMultipleRegisters] = new ModbusTcpCodecWriteMultipleRegisters();
            CommandCodecs[ModbusCommand.FuncReadCoils] = new ModbusTcpCodecReadMultipleDiscretes();
            CommandCodecs[ModbusCommand.FuncReadInputDiscretes] = new ModbusTcpCodecReadMultipleDiscretes();
            CommandCodecs[ModbusCommand.FuncReadInputRegisters] = new ModbusTcpCodecReadMultipleRegisters();
            CommandCodecs[ModbusCommand.FuncWriteCoil] = new ModbusTcpCodecWriteSingleDiscrete();
            CommandCodecs[ModbusCommand.FuncWriteSingleRegister] = new ModbusTcpCodecWriteSingleRegister();
        }



        private static readonly ModbusCommandCodec[] CommandCodecs = new ModbusCommandCodec[24];



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

            //calculate length field
            var length = 2 + body.Length;

            //crea un writer per raccogliere i dati utili
            var writer = new ByteArrayWriter();

            //transaction-id (always zero)
            writer.WriteUInt16BE((ushort)command.TransId);

            //protocol-identifier (always zero)
            writer.WriteInt16BE(0);

            //message length
            writer.WriteInt16BE((short)length);

            //unit identifier (address)
            writer.WriteByte(client.Address);

            //function code
            writer.WriteByte(fncode);

            //body data
            writer.WriteBytes(body);

            data.OutgoingData = writer.ToReader();
        }



        CommResponse IProtocolCodec.ClientDecode(CommDataBase data)
        {
            var client = (ModbusClient)data.OwnerProtocol;
            var command = (ModbusCommand)data.UserData;
            var incoming = data.IncomingData;

            //validate header first
            if (incoming.Length >= 6 &&
                incoming.ReadUInt16BE() == (ushort)command.TransId &&
                incoming.ReadInt16BE() == 0     //protocol-identifier
                )
            {
                //message length
                var length = incoming.ReadInt16BE();

                //validate address
                if (incoming.Length >= (length + 6) &&
                    incoming.ReadByte() == client.Address
                    )
                {
                    //validate function code
                    var fncode = incoming.ReadByte();

                    if ((fncode & 0x7F) == command.FunctionCode)
                    {
                        if (fncode <= 0x7F)
                        {
                            //
                            var body = new ByteArrayReader(incoming.ReadToEnd());

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

            //crea un writer per raccogliere i dati utili
            var writer = new ByteArrayWriter();

            //transaction-id
            writer.WriteUInt16BE((ushort)command.TransId);

            //protocol-identifier
            writer.WriteInt16BE(0);

            //message length
            writer.WriteInt16BE((short)length);

            //unit identifier (address)
            writer.WriteByte(server.Address);

            if (command.ExceptionCode == 0)
            {
                //function code
                writer.WriteByte(command.FunctionCode);

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

            data.OutgoingData = writer.ToReader();
        }



        CommResponse IProtocolCodec.ServerDecode(CommDataBase data)
        {
            var server = (ModbusServer)data.OwnerProtocol;
            var incoming = data.IncomingData;

            //validate header first
            if (incoming.Length < 6)
                goto LabelUnknown;

            //transaction-id
            var transId = incoming.ReadUInt16BE();

            //protocol-identifier
            var protId = incoming.ReadInt16BE();
            if (protId != 0)
                goto LabelIgnore;

            //message length
            var length = incoming.ReadInt16BE();

            if (incoming.Length < (length + 6))
                goto LabelUnknown;

            //address
            var address = incoming.ReadByte();

            if (address == server.Address)
            {
                //function code
                var fncode = incoming.ReadByte();

                //create a new command
                var command = new ModbusCommand(fncode);
                data.UserData = command;
                command.TransId = transId;

                //
                var body = new ByteArrayReader(incoming.ReadToEnd());

                //encode the command body, if applies
                var codec = CommandCodecs[fncode];
                if (codec != null)
                    codec.ServerDecode(command, body);

                return new CommResponse(
                    data,
                    CommResponse.Ack);
            }

            //exception
        LabelIgnore:
            return new CommResponse(
                data,
                CommResponse.Ignore);

        LabelUnknown:
            return new CommResponse(
                data,
                CommResponse.Unknown);
        }

        #endregion



        /// <summary>
        /// Append the typical header for a request command (master-side)
        /// </summary>
        /// <param name="command"></param>
        /// <param name="body"></param>
        internal static void PushRequestHeader(
            ModbusCommand command,
            ByteArrayWriter body)
        {
            body.WriteUInt16BE((ushort)command.Offset);
            body.WriteInt16BE((short)command.Count);
        }



        /// <summary>
        /// Extract the typical header for a request command (server-side)
        /// </summary>
        /// <param name="command"></param>
        /// <param name="body"></param>
        internal static void PopRequestHeader(
            ModbusCommand command,
            ByteArrayReader body)
        {
            command.Offset = body.ReadUInt16BE();
            command.Count = body.ReadInt16BE();
        }



        /// <summary>
        /// Helper for packing the discrete data outgoing as a bit-array
        /// </summary>
        /// <param name="command"></param>
        /// <param name="body"></param>
        internal static void PushDiscretes(
            ModbusCommand command,
            ByteArrayWriter body)
        {
            var count = command.Count;
            body.WriteByte((byte)((count + 7) / 8));

            int i = 0;
            int cell = 0;
            for (int k = 0; k < count; k++)
            {
                if (command.Data[k] != 0)
                    cell |= (1 << i);

                if (++i == 8)
                {
                    body.WriteByte((byte)cell);
                    i = 0;
                    cell = 0;
                }
            }

            if (i > 0)
                body.WriteByte((byte)cell);
        }



        /// <summary>
        /// Helper for unpacking discrete data incoming as a bit-array
        /// </summary>
        /// <param name="command"></param>
        /// <param name="body"></param>
        internal static void PopDiscretes(
            ModbusCommand command,
            ByteArrayReader body)
        {
            var byteCount = body.ReadByte();

            var count = command.Count;
            command.Data = new ushort[count];

            int k = 0;
            while (count > 0)
            {
                byteCount--;
                int cell = body.ReadByte();

                int n = count <= 8 ? count : 8;
                count -= n;
                for (int i = 0; i < n; i++)
                    command.Data[k++] = (ushort)(cell & (1 << i));
            }
        }

    }
}
