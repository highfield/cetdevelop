using System;
using Microsoft.SPOT;

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
    public enum ScorerStatus
    {
        Intro,
        Play,
        GameOver,
    }


    public class InvadersScorer
    {
        private const string TextToSlide = "*** Netduaders - an Invaders-like game by Mario Vernari ***";

        private static readonly string RegisterStrip = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789." + (char)CharCodeDel + (char)CharCodeEnd;

        private const int CharCodeShip = 1;
        private const int CharCodeDel = 2;
        private const int CharCodeEnd = 3;


        public InvadersScorer(
            ICompositionRenderer renderer,
            GameContext context)
        {
            this._renderer = renderer;
            this._context = context;

            //creates the target composition instance
            this._composition = renderer.CreateTarget();

            //create the "ship" custom character
            ((BoostHD44780)renderer).DefineCustomCharacter(
                CharCodeShip,
                new byte[8] { 0x00, 0x04, 0x04, 0x0E, 0x1F, 0x1F, 0x1F, 0x00 }
                );

            //create the "del" custom character
            ((BoostHD44780)renderer).DefineCustomCharacter(
                CharCodeDel,
                new byte[8] { 0x18, 0x14, 0x14, 0x18, 0x04, 0x04, 0x07, 0x00 }
                );

            //create the "end" custom character
            ((BoostHD44780)renderer).DefineCustomCharacter(
                CharCodeEnd,
                new byte[8] { 0x1C, 0x14, 0x14, 0x1D, 0x06, 0x06, 0x05, 0x00 }
                );
        }


        private readonly ICompositionRenderer _renderer;
        private readonly GameContext _context;
        private readonly CompositionTargetBase _composition;

        private GameStatus _oldStatus = GameStatus.Playing; //must differ from the actual status init
        private bool _isButtonPressed;
        private int _counter;
        private TopScoreEntry _scoreEntry;


        public void Update()
        {
            //wipes out the composition target
            this._composition.Clear();

            switch (this._context.Status)
            {
                case GameStatus.Idle:
                case GameStatus.RegComplete:
                    this.UpdateIdle();
                    break;

                case GameStatus.Playing:
                    this.UpdatePlay();
                    break;

                case GameStatus.Registering:
                    this.UpdateRegister();
                    break;
            }

            //dumps the composition to the physical device
            this._renderer.Dump(this._composition);
        }


        private void UpdateIdle()
        {
            if (this._oldStatus != GameStatus.Idle)
            {
                this._oldStatus = GameStatus.Idle;

                //init local stage
                this._counter = 0;
            }

            if (this._counter < 60)
            {
                //show top scores
                this._composition.DrawString("Hall", null, Brushes.Black, 0, 0);
                this._composition.DrawString("of", null, Brushes.Black, 1, 1);
                this._composition.DrawString("Fame", null, Brushes.Black, 0, 2);

                for (int i = 0, count = this._context.TopScores.Count; i < count; i++)
                {
                    TopScoreEntry item = this._context.TopScores.GetItem(i);

                    this._composition.DrawString(
                        "00000",
                        null,
                        Brushes.Black,
                        14,
                        i);

                    this._composition.DrawString(
                        item.Name,
                        null,
                        Brushes.Black,
                        8,
                        i);

                    var text = item.Score.ToString();
                    Size sz = this._composition.MeasureString(text, null);

                    this._composition.DrawString(
                        text,
                        null,
                        Brushes.Black,
                        19 - sz.Width,
                        i);

                }
            }
            else if (this._counter < 150)
            {
                //show ads
                this._composition.DrawString("Just a Netduino 2,", null, Brushes.Black, 0, 0);
                this._composition.DrawString("a led-matrix, a LCD,", null, Brushes.Black, 0, 1);
                this._composition.DrawString("a few hardware more,", null, Brushes.Black, 0, 2);
                this._composition.DrawString("and have lot of fun!", null, Brushes.Black, 0, 3);
            }
            else
            {
                this._counter = 0;
            }

            this._counter++;
        }


        private void UpdatePlay()
        {
            if (this._oldStatus != GameStatus.Playing)
            {
                this._oldStatus = GameStatus.Playing;

                //init local stage
                this._counter = 25;
            }

            //draw the current score and the hightest one
            this._composition.DrawString(
                "Sc:00000   Hi:00000",
                null,
                Brushes.Black,
                0,
                0);

            var text = this._context.CurrentScore.ToString();
            Size sz = this._composition.MeasureString(text, null);

            this._composition.DrawString(
                text,
                null,
                Brushes.Black,
                9 - sz.Width,
                0);

            text = this._context.HighScore.ToString();
            sz = this._composition.MeasureString(text, null);

            this._composition.DrawString(
                text,
                null,
                Brushes.Black,
                20 - sz.Width,
                0);

            //indicate the current level
            this._composition.DrawString(
                "Level: " + this._context.CurrentLevel,
                null,
                Brushes.Black,
                0,
                3);

            //indicate the ships left
            this._composition.DrawString(
                (char)CharCodeShip + string.Empty,
                null,
                Brushes.Black,
                17,
                3);

            text = this._context.ShipsLeft.ToString();
            sz = this._composition.MeasureString(text, null);

            this._composition.DrawString(
                text,
                null,
                Brushes.Black,
                20 - sz.Width,
                3);

            //sliding ad
            if ((this._context.Clock & 7) == 0)
            {
                if (--this._counter < -TextToSlide.Length)
                    this._counter = 25;
            }

            this._composition.DrawString(
                TextToSlide,
                null,
                Brushes.Black,
                this._counter,
                2);
        }


        private void UpdateRegister()
        {
            if (this._oldStatus != GameStatus.Registering)
            {
                this._oldStatus = GameStatus.Registering;

                //init local stage
                this._counter = 0;
                var entry = new TopScoreEntry();
                entry.Score = this._context.CurrentScore;
                if (this._context.TopScores.Add(entry) >= 0)
                {
                    //the current score enters in the hall of fame
                    this._scoreEntry = entry;
                    this._context.HighScore = this._context.TopScores.GetItem(0).Score;
                }
                else
                {
                    //skip registration and quit to the intro
                    this._context.Status = GameStatus.RegComplete;
                    return;
                }
            }

            var flasher = (this._context.Clock & 4) != 0;

            if (this._context.LeftButton)
            {
                if (this._isButtonPressed == false)
                {
                    this._isButtonPressed = true;
                    if (this._counter > 0)
                        this._counter--;
                }
            }
            else if (this._context.RightButton)
            {
                if (this._isButtonPressed == false)
                {
                    this._isButtonPressed = true;
                    if (this._counter < 38)  //strip max index
                        this._counter++;
                }
            }
            else if (this._context.FireButton)
            {
                if (this._isButtonPressed == false)
                {
                    this._isButtonPressed = true;

                    //gets the char at cursor
                    char selected = RegisterStrip[this._counter];
                    switch ((int)selected)
                    {
                        case CharCodeDel:
                            int length;
                            if ((length = this._scoreEntry.Name.Length) > 0)
                            {
                                this._scoreEntry.Name = this._scoreEntry.Name.Substring(0, length - 1);
                            }
                            break;

                        case CharCodeEnd:
                            //registration complete: back to the intro
                            this._context.Status = GameStatus.RegComplete;
                            break;

                        default:
                            if (this._scoreEntry.Name.Length < 4)
                                this._scoreEntry.Name += selected;
                            break;
                    }
                }
            }
            else
            {
                this._isButtonPressed = false;
            }

            //registration title
            this._composition.DrawString(
                "Enter your name:",
                null,
                Brushes.Black,
                2,
                0);

            //chararcter strips
            this._composition.DrawString(
                RegisterStrip.Substring(0, 20),
                null,
                Brushes.Black,
                0,
                1);

            this._composition.DrawString(
                RegisterStrip.Substring(20),
                null,
                Brushes.Black,
                0,
                2);

            //place the flashing cursor over the current character
            if (flasher)
            {
                this._composition.DrawPixel(
                    this._counter % 20,
                    1 + this._counter / 20,
                    Colors.White);
            }

            //display the user name
            this._composition.DrawString(
                ">>>____<<<",
                null,
                Brushes.Black,
                5,
                3);

            this._composition.DrawString(
                this._scoreEntry.Name,
                null,
                Brushes.Black,
                8,
                3);

        }

    }
}
