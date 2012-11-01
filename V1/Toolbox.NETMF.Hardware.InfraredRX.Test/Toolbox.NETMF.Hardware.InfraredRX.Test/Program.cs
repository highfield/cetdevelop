using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

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
    public class Program
    {
        private static PWM _wheelRight = new PWM(Pins.GPIO_PIN_D5);
        private static PWM _wheelLeft = new PWM(Pins.GPIO_PIN_D6);
        private static OutputPort _red = new OutputPort(Pins.GPIO_PIN_D0, false);
        private static OutputPort _green = new OutputPort(Pins.GPIO_PIN_D1, false);


        public static void Main()
        {
            //create the infrared receiver listening to the port 8
            var irrx = new InfraredLegoReceiver(Pins.GPIO_PIN_D8);
            irrx.InfraredMessageDecoded += new InfraredMessageDecodedDelegate(irrx_InfraredMessageDecoded);
            irrx.IsEnabled = true;

            //do whatever

            Thread.Sleep(Timeout.Infinite);
        }


        static void irrx_InfraredMessageDecoded(
            object sender,
            InfraredMessageDecodedEventArgs e)
        {
            var message = (LegoMessage)e.Message;
            //Debug.Print(message.Mode + " " + message.Func);

            switch (message.Func & 0x03)
            {
                case 0:
                    _wheelLeft.SetDutyCycle(0);
                    _red.Write(false);
                    break;

                case 1:
                    _wheelLeft.SetDutyCycle(100);
                    break;

                case 2:
                    _red.Write(true);
                    break;
            }

            switch (message.Func & 0x0C)
            {
                case 0:
                    _wheelRight.SetDutyCycle(0);
                    _green.Write(false);
                    break;

                case 4:
                    _wheelRight.SetDutyCycle(100);
                    break;

                case 8:
                    _green.Write(true);
                    break;
            }
        }

    }
}
