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
    public class Program
    {
        public static void Main()
        {

            //dynamo
            var dynamo = new DynamoEncoder(Pins.GPIO_PIN_A2);
            dynamo.StatusChanged += (s_, e_) =>
            {
                //display the current value
                Debug.Print("Dynamo=" + dynamo.Value.ToString());
            };

            //rotary
            var rotary = new RotaryEncoder(
                Pins.GPIO_PIN_D6,
                Pins.GPIO_PIN_D7
                );
            rotary.StatusChanged += (s_, e_) =>
            {
                //display the current value
                Debug.Print("Rotary=" + rotary.Value.ToString());
            };

            //button
            var button = new PushButtonInputPort(
                Pins.GPIO_PIN_D0,
                Port.ResistorMode.PullUp,
                activeLevel: false
                );
            button.StatusChanged += (s_, e_) =>
            {
                //display the current value
                Debug.Print("Button=" + button.Read().ToString());
            };

            Thread.Sleep(Timeout.Infinite);
        }

    }
}
