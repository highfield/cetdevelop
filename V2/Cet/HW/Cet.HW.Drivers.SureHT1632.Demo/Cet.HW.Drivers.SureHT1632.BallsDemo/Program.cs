using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

using Cet.HW.GDI;

/*
 * Copyright 2012, 2013 by Mario Vernari, Cet Electronics
 * Part of "Cet Open Toolbox" (http://cetdevelop.codeplex.com/)
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
namespace Cet.HW.Drivers.BallsDemo
{
    public class Program
    {
        private const int BallCount = 15;

        private static readonly IRenderable[] _renderables = new IRenderable[BallCount];
        private static ICompositionRenderer _renderer;
        private static CompositionTargetBase _composition;

        private static Timer _clock;

        //just for timing measurements
        private static OutputPort _qtest = new OutputPort(Pins.GPIO_PIN_D2, false);


        public static void Main()
        {
            //create a led-matrix driver instance
            _renderer = new SureHT1632(
                cspin: Pins.GPIO_PIN_D10,
                clkpin: Pins.GPIO_PIN_D9);

            //creates the target composition instance
            _composition = _renderer.CreateTarget();

            //creates a series of bouncing-ball instances
            var rnd = new Random();

            for (int i = 0; i < BallCount; i++)
            {
                _renderables[i] = new Ball(rnd.Next());
            }

            //start the timer for the demo loop
            const int LoopPeriod = 50; //ms

            _clock = new Timer(
                ClockTick,
                null,
                LoopPeriod,
                LoopPeriod);

            //keep alive
            Thread.Sleep(Timeout.Infinite);
        }


        private static void ClockTick(object state)
        {
            _qtest.Write(true);

            //wipes out the composition target
            _composition.Clear();

            //renders the objects against the target
            for (int i = 0; i < BallCount; i++)
            {
                _renderables[i].OnRender(_composition);
            }

            //dumps the composition to the physical device
            _renderer.Dump(_composition);

            _qtest.Write(false);
        }

    }
}
