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
namespace Cet.IO.Protocols
{
    /// <summary>
    /// Provide the abstraction for any Modbus command codec
    /// </summary>
    public class ModbusCommandCodec
    {
        #region Client codec

        /// <summary>
        /// Encode the client-side command toward the remote slave device
        /// </summary>
        /// <param name="command"></param>
        /// <param name="body"></param>
        /// <returns>True when the function operated successfully</returns>
        public virtual bool ClientEncode(
            ModbusCommand command,
            ByteArrayWriter body)
        {
            return false;
        }

        /// <summary>
        /// Decode the incoming data from the remote slave device 
        /// to a client-side command
        /// </summary>
        /// <param name="command"></param>
        /// <param name="body"></param>
        /// <returns>True when the function operated successfully</returns>
        public virtual bool ClientDecode(
            ModbusCommand command,
            ByteArrayReader body)
        {
            return false;
        }

        #endregion


        #region Server codec

        /// <summary>
        /// Encode the server-side command toward the master remote device
        /// </summary>
        /// <param name="command"></param>
        /// <param name="body"></param>
        /// <returns>True when the function operated successfully</returns>
        public virtual bool ServerEncode(
            ModbusCommand command,
            ByteArrayWriter body)
        {
            return false;
        }

        /// <summary>
        /// Decode the incoming data from the remote master device 
        /// to a server-side command
        /// </summary>
        /// <param name="command"></param>
        /// <param name="body"></param>
        /// <returns>True when the function operated successfully</returns>
        public virtual bool ServerDecode(
            ModbusCommand command,
            ByteArrayReader body)
        {
            return false;
        }

        #endregion
    }
}
