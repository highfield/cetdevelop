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
    /// Modbus codec for commands: writing single discrete datum
    /// </summary>
    public class ModbusCodecWriteSingleDiscrete
        : ModbusCommandCodec
    {
        #region Client codec

        public override bool ClientEncode(
            ModbusCommand command,
            ByteArrayWriter body)
        {
            body.WriteUInt16BE((ushort)command.Offset);

            var value = command.Data[0] != 0
                ? 0xFF
                : 0;

            body.WriteInt16BE((short)value);
            return true;
        }


        public override bool ClientDecode(
            ModbusCommand command,
            ByteArrayReader body)
        {
            return true;
        }

        #endregion


        #region Server codec

        public override bool ServerEncode(
            ModbusCommand command,
            ByteArrayWriter body)
        {
            ModbusCodecBase.PushRequestHeader(
                command,
                body);

            return true;
        }


        public override bool ServerDecode(
            ModbusCommand command,
            ByteArrayReader body)
        {
            if (body.CanRead(4))
            {
                command.Offset = body.ReadUInt16BE();
                command.Count = 1;
                command.QueryTotalLength += 4;

                command.Data = new ushort[1];
                command.Data[0] = body.ReadUInt16BE() != 0
                    ? (ushort)0xFFFF
                    : (ushort)0;

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
