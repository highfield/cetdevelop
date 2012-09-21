//#define DEMO_SCROLLER
//#define DEMO_GAME

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.IO.Ports;

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
    public class Program
    {
        public static void Main()
        {
            //create the LCD interface
            var driver = LcdBoostFactory.Create20x4(Pins.GPIO_PIN_D10);
            var lcd = new LcdBoostProxy(driver);

#if DEMO_SCROLLER
            //start the test
            DemoScroller.Run(
                lcd,
                new DynamoEncoder(Pins.GPIO_PIN_A2),
                new DynamoEncoder(Pins.GPIO_PIN_A3)
                );
#endif

#if DEMO_GAME
            //start the game
            DemoPong.Run(
                lcd,
                new DynamoEncoder(Pins.GPIO_PIN_A2)
                );
#endif

        }
    }
}
