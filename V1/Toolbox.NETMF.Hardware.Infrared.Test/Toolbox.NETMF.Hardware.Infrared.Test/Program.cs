//#define MAERKLIN
#define SONY

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

using SecretLabs.NETMF.Hardware;
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
#if MAERKLIN
    public class Program
    {
        /**
         * Maerklin remote commander codes:
         * light:   1.T.11000.1010000
         * btn 1:   1.T.11000.1010001
         * btn 2:   1.T.11000.1010010
         * btn 3:   1.T.11000.1010011
         * btn 4:   1.T.11000.1010100
         * btn -:   1.T.11000.0010001
         * btn +:   1.T.11000.0010000
         * dir:     1.T.11000.0001101
         **/
        private const int Address = 0x18;

        private const int SelectLight = 0x50;
        private const int SelectTrain1 = 0x51;
        private const int SelectTrain2 = 0x52;
        private const int SelectTrain3 = 0x53;
        private const int SelectTrain4 = 0x54;

        private const int SpeedDown = 0x11;
        private const int SpeedUp = 0x10;

        private const int Direction = 0x0D;



        public static void Main()
        {
            //create the infrared transmitter driver
            var irtx = new InfraredTransmitter(Pins.GPIO_PIN_D8);

            //create the codec to be used
            var codec = new InfraredCodecRC5(irtx);
            codec.ExtendedMode = true;

            //define the button for decrement speed
            var btn_dec = new InterruptPort(
                Pins.GPIO_PIN_D0,
                true,
                Port.ResistorMode.PullUp,
                Port.InterruptMode.InterruptEdgeLow
                );

            btn_dec.OnInterrupt += (a_, b_, dt_) =>
            {
                codec.Send(Address, SpeedDown);
            };

            //define the button for increment speed
            var btn_inc = new InterruptPort(
                Pins.GPIO_PIN_D1,
                true,
                Port.ResistorMode.PullUp,
                Port.InterruptMode.InterruptEdgeLow
                );

            btn_inc.OnInterrupt += (a_, b_, dt_) =>
            {
                codec.Send(Address, SpeedUp);
            };

            //define the button for the direction
            var btn_dir = new InterruptPort(
                Pins.GPIO_PIN_D2,
                true,
                Port.ResistorMode.PullUp,
                Port.InterruptMode.InterruptEdgeLow
                );

            btn_dir.OnInterrupt += (a_, b_, dt_) =>
            {
                codec.Send(Address, Direction);
            };

            Thread.Sleep(Timeout.Infinite);
        }

    }
#endif

#if SONY
    public class Program
    {
        public static void Main()
        {
            //create the infrared transmitter driver
            var irtx = new InfraredTransmitter(Pins.GPIO_PIN_D10);

            //create the codec to be used
            var codec = new InfraredCodecSonySIRC(irtx);

            while (true)
            {
                codec.Send(0x15, 0x3A);
                Thread.Sleep(100);
            };
        }

    }
#endif
}
