using System;
using Microsoft.SPOT;
using System.IO.Ports;

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
namespace Cet.Develop.NETMF.IO
{
    /// <summary>
    /// Aggregation of the ordinary parameters used by the <see cref="SerialPort"/>
    /// </summary>
    public class SerialPortParams
    {
        /// <summary>
        /// Create an instance by giving the parameters explicitly
        /// </summary>
        /// <param name="baudRate"></param>
        /// <param name="parity"></param>
        /// <param name="dataBits"></param>
        /// <param name="stopBits"></param>
        public SerialPortParams(
            int baudRate,
            Parity parity,
            int dataBits,
            StopBits stopBits)
        {
            this.BaudRate = baudRate;
            this.Parity = parity;
            this.DataBits = dataBits;
            this.StopBits = stopBits;
        }



        /// <summary>
        /// Create an instance by giving a string representation of the parameters
        /// </summary>
        /// <param name="setting"></param>
        /// <remarks>
        /// The string must be in the format: "9600,E,8,1"
        /// </remarks>
        public SerialPortParams(string setting)
        {
            //split the given string
            string[] tokens = setting.Split(',');

            //baud-rate
            this.BaudRate = int.Parse(tokens[0]);

            //parity
            switch (tokens[1])
            {
                case "N":
                    this.Parity = Parity.None;
                    break;

                case "E":
                    this.Parity = Parity.Even;
                    break;

                case "O":
                    this.Parity = Parity.Odd;
                    break;

                case "M":
                    this.Parity = Parity.Mark;
                    break;

                case "S":
                    this.Parity = Parity.Space;
                    break;

                default:
                    throw new NotSupportedException("Parity value not supported");
            }

            //data-bits
            this.DataBits = int.Parse(tokens[2]);

            //stop-bits
            switch (tokens[3])
            {
                case "0":
                    this.StopBits = StopBits.None;
                    break;

                case "1":
                    this.StopBits = StopBits.One;
                    break;

                case "2":
                    this.StopBits = StopBits.Two;
                    break;

                default:
                    throw new NotSupportedException("StopBits value not supported");
            }
        }



        /// <summary>
        /// The baud-rate to be used by the serial port
        /// </summary>
        public readonly int BaudRate;

        /// <summary>
        /// The number of the bits composing a symbol
        /// </summary>
        public readonly int DataBits;

        /// <summary>
        /// The parity option to be addaed to each symbol
        /// </summary>
        public readonly Parity Parity;

        /// <summary>
        /// The number of stop-bits after each symbol
        /// </summary>
        public readonly StopBits StopBits;

    }
}
