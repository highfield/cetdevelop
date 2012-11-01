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
    public class InfraredTransmitter
        : IDisposable
    {
        /// <summary>
        /// Create an instance of driver as low-level modulator
        /// for infrared-signal output SPI-based
        /// </summary>
        /// <param name="enable">
        /// The output pin to be used as enable for the physical driver
        /// </param>
        public InfraredTransmitter(Cpu.Pin enable)
        {
            //begin the enable port owning
            this._enable = new OutputPort(
                enable,
                false);
        }



        private ushort[] _outputBuffer = new ushort[100];
        private int _outputCount;
        private OutputPort _enable;
        private InfraredCodecBase _lastCodec;



        /// <summary>
        /// Adds a pattern of pulses and blanks to the output buffer
        /// </summary>
        /// <param name="codec">The codec used for the buffer composition</param>
        internal void Append(InfraredCodecBase codec)
        {
            //check whether the last accessed codec is the same
            if (this._lastCodec != codec)
            {
                //reset the buffer counter
                this._outputCount = 0;
                this._lastCodec = codec;
            }

            //calculate the new count of the outgoing data
            int newCount = this._outputCount + codec.TotalPulseCount;

            //resize the output buffer, if won't fit the newer data
            if (this._outputBuffer.Length < newCount)
            {
                var buffer = new ushort[newCount + 100];
                Array.Copy(
                    this._outputBuffer,
                    buffer,
                    this._outputBuffer.Length);

                this._outputBuffer = buffer;
            }

            /**
             * N = TotalPulseCount
             * offset:  0   +1   +2   +3    ...    N-2  N-1   N
             *      ---------------------------------------------
             *      ##|    |    |    |    |       |    |    |###
             *      ---------------------------------------------
             *        ____________  logic bit  ______________
             **/
            const ushort Blank = 0;

            int blankFirst = this._outputCount + codec.InitialPulseCount;
            int blankLast = newCount - codec.FinalPulseCount - 1;

            //append pulses and blanks to the buffer
            while (this._outputCount < newCount)
            {
                ushort pattern = (this._outputCount >= blankFirst && this._outputCount <= blankLast)
                    ? Blank
                    : codec.PulseMask;

                this._outputBuffer[_outputCount++] = pattern;
            }
        }



        /// <summary>
        /// Send the overall output buffer composed so far
        /// </summary>
        /// <param name="codec">The codec used for the buffer composition</param>
        internal void Send(InfraredCodecBase codec)
        {
            //open the SPI using the specified configuration
            using (var spi = new SPI(codec.Config))
            {
                //enable the physical transmitter driver
                this._enable.Write(true);

                //shift out the whole buffer
                spi.WriteRead(
                    this._outputBuffer,
                    0,
                    this._outputCount,
                    null,
                    0,
                    0,
                    0);

                //shut down the physical driver
                this._enable.Write(false);
            }

            this._lastCodec = null;
        }



        #region IDisposable Members

        private bool _disposed = false;



        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }



        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    //release the output
                    this._enable.Dispose();
                }

                this._disposed = true;
            }
        }



        ~InfraredTransmitter()
        {
            Dispose(false);
        }

        #endregion
    }
}
