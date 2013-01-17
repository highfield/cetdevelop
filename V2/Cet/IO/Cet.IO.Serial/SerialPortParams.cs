using System;
using System.IO.Ports;

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
    /// Hold the settings for a serial port (UART)
    /// </summary>
    public class SerialPortParams
    {
        /// <summary>
        /// Create the instance by explicit defining every parameter
        /// </summary>
        /// <param name="baudRate"></param>
        /// <param name="parity"></param>
        /// <param name="dataBits"></param>
        /// <param name="stopBits"></param>
        /// <param name="rtsEnable"></param>
        public SerialPortParams(
            int baudRate, 
            Parity parity,
            int dataBits,
            StopBits stopBits,
            bool rtsEnable)
        {
            this.BaudRate = baudRate;
            this.Parity = parity;
            this.DataBits = dataBits;
            this.StopBits = stopBits;
            this.RtsEnable = rtsEnable;
        }


        /// <summary>
        /// Create the instance by using a textual compact form
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="rtsEnable"></param>
        public SerialPortParams(
            string settings,
            bool rtsEnable)
        {
            SerialPortParams.ParseSettings(
                settings,
                out this.BaudRate,
                out this.Parity,
                out this.DataBits,
                out this.StopBits
                );

            this.RtsEnable = rtsEnable;
        }


        public readonly int BaudRate;
        public readonly int DataBits;
        public readonly Parity Parity;
        public readonly StopBits StopBits;


        /// <summary>
        /// Indicate the usage of the RTS (RS232) signal
        /// </summary>
        public readonly bool RtsEnable;



        /// <summary>
        /// Parser for the textual compact form
        /// </summary>
        /// <param name="settings">The settings as string (e.g. "9600,E,8,1")</param>
        private static void ParseSettings(
            string settings,
            out int baudRate,
            out Parity parity,
            out int dataBits,
            out StopBits stopBits)
        {
            //split the input string upon commas
            string[] tokens = settings.Split(',');

            //baud-rate
            baudRate = int.Parse(tokens[0]);

            //parity
            switch (tokens[1])
            {
                case "N":
                    parity = Parity.None;
                    break;

                case "E":
                    parity = Parity.Even;
                    break;

                case "O":
                    parity = Parity.Odd;
                    break;

                case "M":
                    parity = Parity.Mark;
                    break;

                case "S":
                    parity = Parity.Space;
                    break;

                default:
                    throw new NotSupportedException("Parity value not supported");
            }

            //data-bits
            dataBits = int.Parse(tokens[2]);

            //stop-bits
            switch (tokens[3])
            {
                case "0":
                    stopBits = StopBits.None;
                    break;

                case "1":
                    stopBits = StopBits.One;
                    break;

                case "2":
                    stopBits = StopBits.Two;
                    break;

                default:
                    throw new NotSupportedException("StopBits value not supported");
            }
        }


#if NET45
        public override string ToString()
        {
            return string.Format(
                "{0}: {1},{2},{3},{4}; RTS={5}",
                this.GetType().Name,
                this.BaudRate,
                this.Parity,
                this.DataBits,
                this.StopBits,
                this.RtsEnable
                );
        }
#endif

    }
}
