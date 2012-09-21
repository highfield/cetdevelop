using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

/*
 * Copyright 2011 Mario Vernari (http://netmftoolbox.codeplex.com/)
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
            int duty = 0;
            
            //define the PWM out
            var pwm = new PWM(Pins.GPIO_PIN_D5);
            pwm.SetDutyCycle((uint)duty);

            //define the input port for the "decrement" button
            var btnDec = new AutoRepeatInputPort(
                port: Pins.GPIO_PIN_D8,
                resistor: Port.ResistorMode.PullUp,
                activeLevel: false);

            btnDec.InitialDelay = 1000;
            btnDec.StateChanged += (s, e) =>
            {
                if (e.State != AutoRepeatInputPort.AutoRepeatState.Release)
                {
                    duty--;
                    if (duty < 0)
                        duty = 0;

                    pwm.SetDutyCycle((uint)duty);
                }
            };

            //define the input port for the "increment" button
            var btnInc = new AutoRepeatInputPort(
                port: Pins.GPIO_PIN_D9,
                resistor: Port.ResistorMode.PullUp,
                activeLevel: false);

            btnInc.InitialDelay = 1000;
            btnInc.StateChanged += (s, e) =>
            {
                if (e.State != AutoRepeatInputPort.AutoRepeatState.Release)
                {
                    duty++;
                    if (duty > 100)
                        duty = 100;

                    pwm.SetDutyCycle((uint)duty);
                }
            };

            //do nothing
            Thread.Sleep(Timeout.Infinite);
        }

    }
}
