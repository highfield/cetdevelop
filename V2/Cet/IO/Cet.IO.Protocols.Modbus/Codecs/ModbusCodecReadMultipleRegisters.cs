using System;

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

/**
 * 09/Apr/2012
 *  Class name changed due improved abstraction (RTU support)
 **/
namespace Cet.IO.Protocols
{
    /// <summary>
    /// Modbus codec for commands: writing multiple register data
    /// </summary>
    public class ModbusCodecReadMultipleRegisters
        : ModbusCommandCodec
    {
        #region Client codec

        public override bool ClientEncode(
            ModbusCommand command,
            ByteArrayWriter body)
        {
            ModbusCodecBase.PushRequestHeader(
                command,
                body);

            return true;
        }


        public override bool ClientDecode(
            ModbusCommand command,
            ByteArrayReader body)
        {
            var success = false;

            if (body.CanRead(1))
            {
                var count = body.ReadByte();

                if (body.CanRead(count))
                {
                    count /= 2;
                    command.Data = new ushort[count];
                    for (int i = 0; i < count; i++)
                        command.Data[i] = body.ReadUInt16BE();

                    success = true;
                }
            }

            return success;
        }

        #endregion


        #region Server codec

        public override bool ServerEncode(
            ModbusCommand command,
            ByteArrayWriter body)
        {
            var count = command.Count;
            body.WriteByte((byte)(count * 2));
            for (int i = 0; i < count; i++)
                body.WriteUInt16BE(command.Data[i]);

            return true;
        }


        public override bool ServerDecode(
            ModbusCommand command,
            ByteArrayReader body)
        {
            if (ModbusCodecBase.PopRequestHeader(command, body))
            {
                command.Data = new ushort[command.Count];
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

    }
}
