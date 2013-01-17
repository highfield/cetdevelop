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
 * 20/Apr/2012
 *  Class creation
 **/
namespace Cet.IO.Protocols
{
    /// <summary>
    /// Modbus codec for commands: reading multiple discrete data
    /// </summary>
    public class ModbusCodecForceMultipleCoils
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

            ModbusCodecBase.PushDiscretes(
                command,
                body);

            return true;
        }


        public override bool ClientDecode(
            ModbusCommand command,
            ByteArrayReader body)
        {
            return ModbusCodecBase.PopRequestHeader(
                command,
                body);
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
            return
                ModbusCodecBase.PopRequestHeader(command, body) &&
                ModbusCodecBase.PopDiscretes(command, body);
        }

        #endregion

    }
}
