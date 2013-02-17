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
namespace Cet.HW.Drivers.Invaders
{
    public class Program
    {
        private static InvadersGame _game;
        private static InvadersScorer _scorer;

        private static GameContext _context;

        private static Timer _clock;

        //just for timing measurements
        private static OutputPort _qtest = new OutputPort(Pins.GPIO_PIN_D2, false);


        public static void Main()
        {
            //create the context for the game
            _context = new GameContext();

            //create a led-matrix driver instance
            var ledRenderer = new SureHT1632(
                cspin: Pins.GPIO_PIN_D10,
                clkpin: Pins.GPIO_PIN_D9);

            //create a lcd-module driver instance
            var lcdRenderer = new BoostHD44780(
                cspin: Pins.GPIO_PIN_D8,
                config: BoostHD44780.Layout20x4);

            //create the game manager
            _game = new InvadersGame(
                ledRenderer,
                _context);

            //create the score manager
            _scorer = new InvadersScorer(
                lcdRenderer,
                _context);

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

            //update the demo context
            _context.Update();

            //update the game manager
            _game.Update();

            //update the score manager
            _scorer.Update();

            _qtest.Write(false);
        }

    }
}
