using System;
using Microsoft.SPOT.Hardware;

/*
 * Copyright 2012 Stefan Thoolen (http://netmftoolbox.codeplex.com/)
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
    /// Rotary Encoder
    /// </summary>
    public class RotaryEncoder
        : NumericInput
    {
        /// <summary>Reference to the interruptport on the A-pin</summary>
        private InterruptPort _PinA;
        /// <summary>Reference to the interruptport on the B-pin</summary>
        private InterruptPort _PinB;

        /// <summary>Stores the odd value of pin A</summary>
        private bool _OddA = false;
        /// <summary>Stores the odd value of pin B</summary>
        private bool _OddB = false;
        /// <summary>Swaps at each measurement, we act when we got two measurements</summary>
        private bool _Even = true;


        /// <summary>
        /// Initiates a rotary encoder
        /// </summary>
        /// <param name="PinA">Pin A</param>
        /// <param name="PinB">Pin B</param>
        public RotaryEncoder(Cpu.Pin PinA, Cpu.Pin PinB)
        {
            this._PinA = new InterruptPort(PinA, false, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeLow);
            this._PinB = new InterruptPort(PinB, false, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeLow);
            this._PinA.OnInterrupt += new NativeEventHandler(_Pin_OnInterrupt);
            this._PinB.OnInterrupt += new NativeEventHandler(_Pin_OnInterrupt);
        }

        /// <summary>
        /// One of the pins has triggered an interrupt
        /// </summary>
        /// <param name="PinId">The Id of the pin</param>
        /// <param name="Value">It's new value</param>
        /// <param name="Time">Timestamp of the event</param>
        private void _Pin_OnInterrupt(uint PinId, uint Value, DateTime Time)
        {
            // Takes the last value
            bool CurrA = this._PinA.Read();
            bool CurrB = this._PinB.Read();

            // This is an odd measurement, we store the data so we can compare it with the measurement next time
            this._Even = !this._Even;
            if (!this._Even)
            {
                this._OddA = CurrA;
                this._OddB = CurrB;
                return;
            }

            // Was there an actual change?
            if (CurrA == this._OddA && CurrB == this._OddB)
                return;

            // If pin A is high and pin B just went low, we're moving counterclockwise
            // If Pin A is high and pin B didn't went low, we're moving clockwise
            // If pin A was low and pin B just went high, we're moving counterclockwise
            // If pin A was low and pin B didn't went high, we're moving clockwise
            int delta;

            if (this._OddA && !CurrB)
                delta = -1;
            else if (this._OddA)
                delta = 1;
            else if (CurrB)
                delta = -1;
            else
                delta = 1;

            this.Value += delta;
        }



        /// <summary>
        /// Frees all pins and disposes this object
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //release the ports
                this._PinA.Dispose();
                this._PinB.Dispose();
            }

            //dispose the underlying stuffs
            base.Dispose(disposing);
        }

    }
}