using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using Cet.HW.GDI;
using System.IO.Ports;

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
    public enum GameStatus
    {
        Idle,
        Playing,
        Registering,
        RegComplete,
    }


    public class GameContext
        : IDisposable
    {
        public GameContext()
        {
            //init buttons for game commanding
            this._btnFire = new InputPort(Pins.GPIO_PIN_D5, true, Port.ResistorMode.PullUp);
            this._btnLeft = new InputPort(Pins.GPIO_PIN_D6, true, Port.ResistorMode.PullUp);
            this._btnRight = new InputPort(Pins.GPIO_PIN_D7, true, Port.ResistorMode.PullUp);

            //init random number generator
            this.Rnd = new Random((int)DateTime.Now.Ticks);

            //create the sound patterns
            this.CreateSoundsPattern();

            //init and open serial port for sounds
            this._uart = new SerialPort("COM1", 9600, Parity.Odd, 8, StopBits.One);
            this._uart.Open();
        }


        //declare button ports
        private InputPort _btnFire;
        private InputPort _btnLeft;
        private InputPort _btnRight;

        //declare serial port used for sound
        private SerialPort _uart;

        //random number generator
        public Random Rnd;

        public int Clock;
        public bool IsEvenClock;

        public bool LeftButton;
        public bool RightButton;
        public bool FireButton;

        public GameStatus Status;

        public int ShipsLeft;
        public int CurrentLevel;

        public int CurrentScore;
        public int HighScore;

        public readonly TopScoreFrugalList TopScores = new TopScoreFrugalList();


        public void Update()
        {
            this.Clock++;
            this.IsEvenClock = (this.Clock & 1) == 0;

            if (this.IsEvenClock)
            {
                //read the buttons' state
                this.FireButton = !this._btnFire.Read();
                this.LeftButton = !this._btnLeft.Read();
                this.RightButton = !this._btnRight.Read();
            }
        }


        #region Sound player

        //NOTE: since the UART is set at 9600 bps, each byte takes about 1 ms
        private const int LengthSoundAlienDestroyed = 60;
        private const int LengthSoundShipDestroyed = 500;
        private const int LengthSoundSaucer = 25;

        public readonly byte[] SoundAlienDestroyed = new byte[LengthSoundAlienDestroyed];
        public readonly byte[] SoundShipDestroyed = new byte[LengthSoundShipDestroyed];
        public readonly byte[] SoundSaucer = new byte[LengthSoundSaucer];


        private void CreateSoundsPattern()
        {
            //alien destroyed
            for (int k = 0; k < LengthSoundAlienDestroyed; k++)
            {
                int value;

                if (k < 25)
                {
                    value = 0x55;
                }
                else if (k < 35)
                {
                    value = 0x66;
                }
                else
                {
                    value = 0x3C;
                }

                SoundAlienDestroyed[k] = (byte)value;
            }

            //ship destroyed
            for (int k = 0; k < LengthSoundShipDestroyed; k++)
            {
                int value = this.Rnd.Next();

                if (k > 250)
                {
                    var thr = (k - 250) / 250.0;
                    for (int n = 0; n < 8; n++)
                    {
                        if (this.Rnd.NextDouble() < thr)
                        {
                            value &= ~(1 << n);
                        }
                    }
                }

                SoundShipDestroyed[k] = (byte)value;
            }

            //saucer
            for (int k = 0; k < LengthSoundSaucer; k++)
            {
                SoundSaucer[k] = 0x55;
            }
        }


        public void PlaySound(byte[] pattern)
        {
            this._uart.Write(
                pattern,
                0,
                pattern.Length);
        }

        #endregion


        public void Dispose()
        {
            //TODO
        }

    }
}
