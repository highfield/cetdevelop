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
    public class GameStageIntro
        : GameStageBase
    {
        private const string Title1 = "etduaders";


        public GameStageIntro(GameContext context)
            : base(context)
        {
        }


        private int _frame;
        private Point _posT0;
        private Point _posT1;
        private Point _posAlien;


        public override GameStageBase OnCompose(CompositionTargetBase target)
        {
            GameStageBase result = null;

            //the game begins on any button pressed
            if (this.Context.FireButton ||
                this.Context.RightButton ||
                this.Context.LeftButton)
            {
                if (this.Context.Status == GameStatus.Idle)
                    result = new GameStagePlay(this.Context);
            }
            else if (this.Context.Status == GameStatus.RegComplete)
            {
                //this prevents the game to begin when 
                //a button is still pressed since a prev stage
                this.Context.Status = GameStatus.Idle;
            }

            bool flasher = (this.Context.Clock & 8) != 0;

            if (this._frame == 0)
            {
                this._posT0 = new Point(34, 4);
                this._posT1 = new Point(40, 4);
                this._posAlien = new Point(-25, 4);
            }

            if (this._frame < 24)
            {
                target.DrawString("O", AlienFont.Instance, Brushes.Lime, _posT0);
                target.DrawString(Title1, Fonts.Fixed5x7, Brushes.Lime, _posT1);
                _posT0.X--;
                _posT1.X--;
            }
            else if (this._frame < 24)
            {
                target.DrawString("O", AlienFont.Instance, Brushes.Lime, _posT0);
                target.DrawString(Title1, Fonts.Fixed5x7, Brushes.Lime, _posT1);
            }
            else if (this._frame < 24 + 8)
            {
                target.DrawString("O", AlienFont.Instance, Brushes.Lime, _posT0);
                target.DrawString(Title1, Fonts.Fixed5x7, Brushes.Lime, _posT1);
                _posT0.X++;
                _posT1.X++;
            }
            else if (this._frame < 32 + 34)
            {
                string alien = string.Empty + (char)(0x50 + (this._frame & 1));
                target.DrawString(alien, AlienFont.Instance, Brushes.Yellow, _posAlien);
                target.DrawString("O", AlienFont.Instance, Brushes.Lime, _posT0);
                target.DrawString(Title1, Fonts.Fixed5x7, Brushes.Lime, _posT1);
                _posAlien.X++;
            }
            else if (this._frame < 66 + 34)
            {
                string alien = string.Empty + (char)(0x50 + (this._frame & 1));
                target.DrawString(alien, AlienFont.Instance, Brushes.Yellow, _posAlien);
                target.DrawString("O", AlienFont.Instance, Brushes.Lime, _posT0);
                target.DrawString(Title1, Fonts.Fixed5x7, Brushes.Lime, _posT1);
                _posAlien.X--;
                _posT0.X--;
            }
            else if (this._frame < 100 + 34)
            {
                string alien = string.Empty + (char)(0x50 + (this._frame & 1));
                target.DrawString(alien, AlienFont.Instance, Brushes.Yellow, _posAlien);
                target.DrawString("N", AlienFont.Instance, Brushes.Lime, _posT0);
                target.DrawString(Title1, Fonts.Fixed5x7, Brushes.Lime, _posT1);
                _posAlien.X++;
                _posT0.X++;
            }
            else if (this._frame < 134 + 100)
            {
                string alien = string.Empty + (char)(0x50 + (this._frame & 1));
                target.DrawString(alien, AlienFont.Instance, Brushes.Yellow, _posAlien);
                target.DrawString("N", AlienFont.Instance, Brushes.Lime, _posT0);
                target.DrawString(Title1, Fonts.Fixed5x7, Brushes.Lime, _posT1);
                _posAlien.X--;
                _posT0.X--;
                _posT1.X--;
            }
            else if (this._frame < 234 + 60)
            {
                if (this._frame == 234)
                {
                    this._posT0.X = 15;
                    this._posT0.Y = 0;
                    this._posT1.X = -15;
                    this._posT1.Y = 8;
                }

                //alien #2 is wider and yields less points
                string alien2 = flasher ? "RS" : "TU";
                target.DrawString(alien2, AlienFont.Instance, Brushes.Yellow, this._posT0);
                target.DrawString(AlienLarge.Score + "p", Fonts.Fixed5x7, Brushes.Lime, this._posT0.X + 12, this._posT0.Y);

                //alien #1 worth more
                string alien1 = flasher ? "P" : "Q";
                target.DrawString(alien1, AlienFont.Instance, Brushes.Yellow, this._posT1.X + 19, this._posT1.Y);
                target.DrawString(AlienSmall.Score + "p", Fonts.Fixed5x7, Brushes.Lime, this._posT1.X, this._posT1.Y + 1);

                if (this.Context.IsEvenClock)
                {
                    this._posT0.X--;
                    this._posT1.X++;
                }
            }
            else if (this._frame < 294 + 80)
            {
                if (flasher)
                {
                    target.DrawString("In ert", Fonts.Fixed5x7, Brushes.Lime, new Point(-1, 0));
                    target.DrawString("$", Fonts.Fixed5x7, Brushes.Red, new Point(11, 0));
                    target.DrawString("coin", Fonts.Fixed5x7, Brushes.Yellow, new Point(4, 9));
                }
            }
            else
            {
                this._frame = -1;
            }

            this._frame++;

            return result;
        }

    }
}
