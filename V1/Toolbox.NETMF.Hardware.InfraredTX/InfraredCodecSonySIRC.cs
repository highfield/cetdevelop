using System;
using Microsoft.SPOT;

/*
 * Copyright 2012 Mario Vernari (http://netmftoolbox.codeplex.com/)
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
namespace Toolbox.NETMF.Hardware
{
    /// <summary>
    /// Class acting as driver for the Sony SIRC IR protocol
    /// </summary>
    /// <seealso cref="http://www.sbprojects.com/knowledge/ir/sirc.php"/>
    public class InfraredCodecSonySIRC
        : InfraredCodecBase
    {
        private const float CarrierFrequency = 40.0f;   //kHz
        private const float PulseDuty = 0.5f;



        /// <summary>
        /// Create a new instance of codec
        /// </summary>
        /// <param name="transmitter">A valid reference for the transmitter to be used</param>
        public InfraredCodecSonySIRC(InfraredTransmitter transmitter)
            : base(transmitter, CarrierFrequency, PulseDuty)
        {
        }



        /// <summary>
        /// Send a Sony SIRC message
        /// </summary>
        /// <param name="address">Specifies the address in the message</param>
        /// <param name="command">Specifies the command to be sent</param>
        public void Send(
            int address,
            int command)
        {
            //place the "START" pattern
            this.MarkStart();

            //command (7 bits, LSB first)
            for (int i = 0; i < 7; i++)
            {
                this.Modulate((command & 0x01) != 0);
                command >>= 1;
            }

            //address (5 bits, LSB first)
            for (int i = 0; i < 5; i++)
            {
                this.Modulate((address & 0x01) != 0);
                address >>= 1;
            }

            //send
            this.Transmitter
                .Send(this);
        }



        /// <summary>
        /// Mark the start pattern of the SIRC message
        /// </summary>
        private void MarkStart()
        {
            //"START": 96 pulses + 24 blanks = 120 as total
            this.TotalPulseCount = 120;
            this.InitialPulseCount = 96;
            this.FinalPulseCount = 0;

            //append the defined pattern to the stream
            this.Transmitter
                .Append(this);
        }



        /// <summary>
        /// Provide the modulation for the logic bit
        /// </summary>
        /// <param name="value">The logic value to be modulated</param>
        private void Modulate(bool value)
        {
            //each pulse is 25us (=1/40kHz), thus a period of 600us is 24 pulses
            if (value)
            {
                //logic "ONE": 48 pulses + 24 blanks = 72 as total
                this.TotalPulseCount = 72;
                this.InitialPulseCount = 48;
                this.FinalPulseCount = 0;
            }
            else
            {
                //logic "ZERO": 24 pulses + 24 blanks = 48 as total
                this.TotalPulseCount = 48;
                this.InitialPulseCount = 24;
                this.FinalPulseCount = 0;
            }

            //append the defined pattern to the stream
            this.Transmitter
                .Append(this);
        }
    }
}
