using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

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
namespace Toolbox.NETMF.Hardware
{
    /// <summary>
    /// This is a concrete implementation of the infrared-receiver
    /// for the Lego(C) Power-Functions(tm) protocol
    /// </summary>
    /// <remarks>
    /// For the protocol specs, please see:
    /// <see cref="http://powerfunctions.lego.com/en-us/ElementSpecs/8884.aspx#8884"/>
    /// </remarks>
    public class InfraredLegoReceiver
        : InfraredReceiverBase
    {
        /// <summary>
        /// The message total payload to be considered: 1 start + 16 bits
        /// Note that the stop bit is not considered because yields no edges
        /// </summary>
        private const int PayLoad = 1 + 16;

        /// <summary>
        /// The threshold for distinguish a logic "0" from a "1". See specs, page 13.
        /// </summary>
        /// <remarks>
        /// The value is expressed in "ticks" (1 tick = 100ns)
        /// </remarks>
        private const int Threshold01 = 5260;

        /// <summary>
        /// The threshold for distinguish a logic "1" from a "Start". See specs, page 13.
        /// </summary>
        /// <remarks>
        /// The value is expressed in "ticks" (1 tick = 100ns)
        /// </remarks>
        private const int Threshold1S = 9470;


        /// <summary>
        /// Constructor for the class
        /// </summary>
        /// <param name="trigger">The input pin to be used</param>
        /// <remarks>
        /// For the Lego protocol, a timeout of 16ms should be enough for any message.
        /// Also expected max 24 edges, which are a lot more than the actual message size.
        /// </remarks>
        public InfraredLegoReceiver(Cpu.Pin trigger)
            : base(trigger, 16, 32)
        {
        }


        /// <summary>
        /// The concrete implementation of the protocol decoder
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="count"></param>
        protected override void Decode(
            int[] samples,
            int count)
        {
            //checks there're enough edges
            if (count < PayLoad)
            {
                //Debug.Print("ERR: payload=" + count);
                return;
            }

            //checks the duration of the "start" bit
            int sample;
            if ((sample = samples[0]) < Threshold1S)
            {
                //Debug.Print("ERR: start=" + sample);
                return;
            }

            //decode the remaining 16 bits, upon the edges' intervals
            int raw = 0;
            int mask = 0x8000;

            for (int i = 1; i < PayLoad; i++)
            {
                if ((sample = samples[i]) < Threshold01)
                {
                    //bit = ZERO
                }
                else if (sample < Threshold1S)
                {
                    //bit = ONE
                    raw |= mask;
                }
                else
                {
                    //Debug.Print("ERR: bit=" + sample);
                    return;
                }

                mask >>= 1;
            }

            //fields decoding
            int n1 = (raw & 0xF000) >> 12;
            int n2 = (raw & 0x0F00) >> 8;
            int func = (raw & 0x00F0) >> 4;

            //checks the LRC
            int lrc = raw & 0x000F;
            if (lrc != (0x000F ^ func ^ n1 ^ n2))
            {
                //Debug.Print("ERR: lrc");
                return;
            }

            //compose the message to be notified
            var message = new LegoMessage();
            message.Toggle = (raw & 0x8000) != 0;
            message.Escape = (raw & 0x4000) != 0;
            message.Channel = (raw & 0x3000) >> 12;
            message.ExtraAddress = (raw & 0x0800) != 0;
            message.Mode = (raw & 0x0700) >> 8;
            message.Func = func;

            //fires the notification event, carrying the decoded message
            this.OnInfraredMessageDecoded(message);
        }

    }


    /// <summary>
    /// A generic class for representing any Power-Function
    /// message supported by the Lego protocol
    /// </summary>
    /// <remarks>
    /// For details see specs, pages 6..10
    /// </remarks>
    public class LegoMessage
    {
        /// <summary>
        /// Just a toggling bit for detecting duplicate messages
        /// </summary>
        public bool Toggle;

        /// <summary>
        /// Escape bit
        /// </summary>
        public bool Escape;

        /// <summary>
        /// Channel for sharing messages
        /// </summary>
        public int Channel;

        /// <summary>
        /// Extra address space
        /// </summary>
        public bool ExtraAddress;

        /// <summary>
        /// Message mode
        /// </summary>
        public int Mode;

        /// <summary>
        /// Function code
        /// </summary>
        public int Func;
    }
}
