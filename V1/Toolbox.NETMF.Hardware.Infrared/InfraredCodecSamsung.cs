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
    /// Class acting as driver for the Samsung IR protocol
    /// </summary>
    /// <seealso cref="http://rusticengineering.com/2011/02/09/infrared-room-control-with-samsung-ir-protocol/"/>
    /// <seealso cref="http://www.techdesign.be/projects/011/011_waves.htm"/>
    /// <seealso cref="http://arduinostuff.blogspot.it/2011/06/samsung-remote-ir-codes.html"/>
    public class InfraredCodecSamsung
        : InfraredCodecBase
    {
        private const float CarrierFrequency = 36.0f;   //kHz
        private const float PulseDuty = 0.5f;



        /// <summary>
        /// Create a new instance of codec
        /// </summary>
        /// <param name="transmitter">A valid reference for the transmitter to be used</param>
        public InfraredCodecSamsung(InfraredTransmitter transmitter)
            : base(transmitter, CarrierFrequency, PulseDuty)
        {
        }



        /// <summary>
        /// Send a Samsung message
        /// </summary>
        /// <param name="address">Specifies the address in the message</param>
        /// <param name="command">Specifies the command to be sent</param>
        public void Send(
            int address,
            int command)
        {
            //place the "START" pattern
            this.MarkStart();

            //... 31 or 32 bits

            //stop-bit
            this.Modulate(true);

            //send
            this.Transmitter
                .Send(this);
        }



        /// <summary>
        /// Mark the start pattern of the SIRC message
        /// </summary>
        private void MarkStart()
        {
            //"START": 160 pulses + 160 blanks = 320 as total
            this.TotalPulseCount = 320;
            this.InitialPulseCount = 160;
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
            //each pulse is 27.78us (=1/36kHz)
            if (value)
            {
                //logic "ONE": 20 pulses + 20 blanks = 40 as total
                this.TotalPulseCount = 40;
                this.InitialPulseCount = 20;
                this.FinalPulseCount = 0;
            }
            else
            {
                //logic "ZERO": 20 pulses + 60 blanks = 80 as total
                this.TotalPulseCount = 80;
                this.InitialPulseCount = 20;
                this.FinalPulseCount = 0;
            }

            //append the defined pattern to the stream
            this.Transmitter
                .Append(this);
        }
    }
}
