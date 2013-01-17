using System.IO.Ports;
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
namespace Cet.IO.Serial
{
    /// <summary>
    /// Wrapper around the <see cref="SerialPort"/> class,
    /// so that it implements the <see cref="ISerialPort"/> interface
    /// </summary>
    public class SerialPortEx
        : SerialPort, ISerialPort
    {
#if MF_FRAMEWORK_VERSION_V4_1 || MF_FRAMEWORK_VERSION_V4_2
        /// <summary>
        /// Constructor for the class
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="baudRate"></param>
        /// <param name="parity"></param>
        /// <param name="dataBits"></param>
        /// <param name="stopBits"></param>
        public SerialPortEx(
            string portName,
            int baudRate,
            Parity parity,
            int dataBits,
            StopBits stopBits)
            : base(portName, baudRate, parity, dataBits, stopBits)
        {
        }


        /// <summary>
        /// Gets the port name
        /// </summary>
        /// <remarks>
        /// This member overrides the underlying property,
        /// because the MF allows only to read the value
        /// </remarks>
        public new string PortName
        {
            get { return base.PortName; }
            set { throw new NotSupportedException(); }
        }


        /// <summary>
        /// Dummy RTS line control
        /// </summary>
        /// <remarks>
        /// Provided here just for compatibility.
        /// </remarks>
        public bool RtsEnable { get; set; }


        /// <summary>
        /// Write a series of byte to the port
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <remarks>
        /// This member overrides the underlying method,
        /// because the MF returns the actual bytes written
        /// but the interface needs "void"
        /// </remarks>
        public new void Write(
            byte[] buffer, 
            int offset, 
            int count)
        {
            base.Write(
                buffer, 
                offset, 
                count);
        }
#endif

        /// <summary>
        /// Offer an easier way to set the serial port parameters
        /// </summary>
        /// <param name="prm"></param>
        public void SetParams(SerialPortParams prm)
        {
            this.BaudRate = prm.BaudRate;
            this.Parity = prm.Parity;
            this.DataBits = prm.DataBits;
            this.StopBits = prm.StopBits;
        }

    }
}
