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
    /// Class acting as driver for the Philips RC-5 IR protocol
    /// </summary>
    /// <seealso cref="http://www.sbprojects.com/knowledge/ir/rc5.php"/>
    public class InfraredCodecRC5
        : InfraredCodecBase
    {
        private const float CarrierFrequency = 36.0f;   //kHz
        private const float PulseDuty = 0.5f;



        /// <summary>
        /// Create a new instance of codec
        /// </summary>
        /// <param name="transmitter">A valid reference for the transmitter to be used</param>
        public InfraredCodecRC5(InfraredTransmitter transmitter)
            : base(transmitter, CarrierFrequency, PulseDuty)
        {
            //the RC-5 protocol fixes a well-defined duration for any logic bit
            this.TotalPulseCount = 64;
        }



        private bool _toggle;

        public bool ExtendedMode;



        /// <summary>
        /// Send a RC-5 message
        /// </summary>
        /// <param name="address">Specifies the address in the message</param>
        /// <param name="command">Specifies the command to be sent</param>
        public void Send(
            int address,
            int command)
        {
            //S1: always "ONE"
            this.Modulate(true);

            if (this.ExtendedMode)
            {
                //place the 7th command bit, but inverted
                this.Modulate((command & 0x40) == 0);
            }
            else
            {
                //S2: always "ONE"
                this.Modulate(true);
            }

            //toggle
            this.Modulate(this._toggle);
            this._toggle = !this._toggle;

            //address (5 bits, MSB first)
            for (int i = 0; i < 5; i++)
            {
                this.Modulate((address & 0x10) != 0);
                address <<= 1;
            }

            //command (6 bits, MSB first)
            for (int i = 0; i < 6; i++)
            {
                this.Modulate((command & 0x20) != 0);
                command <<= 1;
            }

            //send
            this.Transmitter
                .Send(this);
        }



        /// <summary>
        /// Provide the modulation for the logic bit
        /// </summary>
        /// <param name="value">The logic value to be modulated</param>
        private void Modulate(bool value)
        {
            if (value)
            {
                //logic "ONE": 32 blanks + 32 pulses = 64 as total
                this.InitialPulseCount = 0;
                this.FinalPulseCount = 32;
            }
            else
            {
                //logic "ZERO": 32 pulses + 32 blanks = 64 as total
                this.InitialPulseCount = 32;
                this.FinalPulseCount = 0;
            }

            //append the defined pattern to the stream
            this.Transmitter
                .Append(this);
        }
    }
}
