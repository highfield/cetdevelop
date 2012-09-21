using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

using SecretLabs.NETMF.Hardware.NetduinoPlus;

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
    public class InfraredCodecBase
    {
        /// <summary>
        /// Base class constructor for any infrared oriented codec
        /// </summary>
        /// <param name="transmitter">A valid infrared transmitter reference</param>
        /// <param name="frequency">The carrier frequency to be used</param>
        /// <param name="pulseDuty">The desired pulse-duty cycle</param>
        /// <remarks>
        /// The pulse duty-cycle is meant to be expressed as a value from 0.0 to 1.0,
        /// and is the ratio between the duration of the high-level and the pulse period
        /// </remarks>
        protected InfraredCodecBase(
            InfraredTransmitter transmitter,
            float frequency,
            float pulseDuty)
        {
            this.Transmitter = transmitter;

            //calculates the actual period for pushing out one
            //ushort value, interleave including
            float t_carrier = 1 / frequency;
            float t_ushort = t_carrier - 2e-3f;

            //calculates the equivalent SPI frequency
            //note that an "unshort" is 16 bits
            uint spi_freq = (uint)(16.0f / t_ushort);

            //prevent a frequency too low: seems that Netduino hangs
            //when the frequency is below a certain value
            if (spi_freq < 300)
                throw new Exception();

            this.Config = new SPI.Configuration(
                Pins.GPIO_NONE,    // SS-pin (not used)
                true,              // SS-pin active state (not used)
                0,                 // The setup time for the SS port (not used)
                0,                 // The hold time for the SS port (not used)
                true,              // The idle state of the clock (not used)
                true,              // The sampling clock edge (not used)
                spi_freq,          // The SPI clock rate in KHz
                SPI_Devices.SPI1   // The used SPI bus (refers to a MOSI MISO and SCLK pinset)
            );

            //calculate the pulse mask
            int mask = 1;

            for (int i = 1; i <= 16; i++)
            {
                if ((i / 16.0f) < pulseDuty)
                    mask = (mask << 1) | 1;
                else
                    break;
            }

            this.PulseMask = (ushort)mask;
        }



        /// <summary>
        /// The total number of "divisions" of the logic bit
        /// </summary>
        internal int TotalPulseCount;

        /// <summary>
        /// The number of initial pulses
        /// </summary>
        internal int InitialPulseCount;

        /// <summary>
        /// The number of final pulses
        /// </summary>
        internal int FinalPulseCount;

        /// <summary>
        /// The reference to the transmitter to be used
        /// </summary>
        internal readonly InfraredTransmitter Transmitter;

        /// <summary>
        /// this is the mask to be used in the buffer composition
        /// </summary>
        internal readonly ushort PulseMask;

        /// <summary>
        /// this is the SPI configuration to be used for the transfer
        /// </summary>
        internal readonly SPI.Configuration Config;
    }
}
