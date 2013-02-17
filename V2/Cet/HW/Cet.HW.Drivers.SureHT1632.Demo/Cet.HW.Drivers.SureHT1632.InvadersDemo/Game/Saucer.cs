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
    public enum SaucerStatus
    {
        Inactive,
        Active,
        StartExplosion,
        ExplodeExpand,
        ExplodeMax,
        ExplodeCollapse,
    }


    public class Saucer
        : IRenderable
    {

        private const double Probability = 0.006;

        private const int ExplosionTime = 3;
        private int _explosionClock;

        public SaucerStatus Status;
        public int PromisedScore;

        public int XCenter;
        public int YCenter;

        private int _xLimit;


        public bool DetectCollision(GameStagePlay stage)
        {
            if (this.Status == SaucerStatus.Active)
            {
                Missile missile = stage.Missile;

                if (missile.IsActive &&
                    missile.YCenter >= this.YCenter &&
                    missile.YCenter <= this.YCenter + 1 &&
                    missile.XCenter >= this.XCenter - 1 &&
                    missile.XCenter <= this.XCenter + 2)
                {
                    //got it!
                    this.Status = SaucerStatus.StartExplosion;
                    missile.IsActive = false;

                    //decide score, then accumulate it
                    this.PromisedScore = 10 * (1 + stage.Context.Rnd.Next(9));
                    stage.Context.CurrentScore += this.PromisedScore;
                    return true;
                }
            }

            return false;
        }


        public void Update(GameStagePlay stage)
        {
            switch (this.Status)
            {
                case SaucerStatus.Inactive:
                    double hash;
                    if ((hash = stage.Context.Rnd.NextDouble()) < Probability)
                    {
                        if (hash < Probability / 2)
                        {
                            //will flight from left to right
                            this.XCenter = -8;
                            this._xLimit = stage.ViewportRight + 4;
                        }
                        else
                        {
                            //will flight from right to left
                            this.XCenter = stage.ViewportRight + 4;
                            this._xLimit = -8;
                        }

                        this.Status = SaucerStatus.Active;
                    }
                    break;


                case SaucerStatus.Active:
                    if (stage.Context.IsEvenClock)
                    {
                        if (this.XCenter < this._xLimit)
                            this.XCenter++;
                        else
                            this.XCenter--;

                        if (this.XCenter == this._xLimit)
                        {
                            //sauce gone off screen being not hit
                            this.Status = SaucerStatus.Inactive;
                        }
                        else
                        {
                            //play releated sound
                            stage.Context.PlaySound(stage.Context.SoundSaucer);
                        }
                    }
                    break;


                case SaucerStatus.StartExplosion:
                    this.Status = SaucerStatus.ExplodeExpand;
                    this._explosionClock = ExplosionTime;

                    //play related sound
                    stage.Context.PlaySound(stage.Context.SoundShipDestroyed);
                    break;

                case SaucerStatus.ExplodeExpand:
                    if (--this._explosionClock == 0)
                    {
                        this.Status = SaucerStatus.ExplodeMax;
                        this._explosionClock = ExplosionTime;
                    }
                    break;

                case SaucerStatus.ExplodeMax:
                    if (--this._explosionClock == 0)
                    {
                        this.Status = SaucerStatus.ExplodeCollapse;
                        this._explosionClock = ExplosionTime;
                    }
                    break;

                case SaucerStatus.ExplodeCollapse:
                    if (--this._explosionClock == 0)
                    {
                        this.Status = SaucerStatus.Inactive;
                        stage.ShowSaucerWorth();
                    }
                    break;
            }
        }


        public void OnRender(CompositionTargetBase target)
        {
            switch (this.Status)
            {
                case SaucerStatus.Inactive:
                    break;

                case SaucerStatus.Active:
                    //cabin
                    target.DrawPixel(this.XCenter, this.YCenter, Colors.Lime);
                    target.DrawPixel(this.XCenter + 1, this.YCenter, Colors.Lime);

                    //rockets
                    int y = this.YCenter + 1;
                    int color1 = (this.XCenter & 1) != 0 ? Colors.Red : Colors.Yellow;
                    int color2 = (this.XCenter & 1) == 0 ? Colors.Red : Colors.Yellow;

                    target.DrawPixel(this.XCenter - 1, y, color1);
                    target.DrawPixel(this.XCenter, y, color2);
                    target.DrawPixel(this.XCenter + 1, y, color1);
                    target.DrawPixel(this.XCenter + 2, y, color2);
                    break;


                case SaucerStatus.ExplodeExpand:
                case SaucerStatus.ExplodeCollapse:
                    target.DrawPixel(this.XCenter, this.YCenter, Colors.Red);
                    target.DrawPixel(this.XCenter + 1, this.YCenter, Colors.Red);

                    target.DrawPixel(this.XCenter - 1, this.YCenter - 1, Colors.Yellow);
                    target.DrawPixel(this.XCenter - 1, this.YCenter + 1, Colors.Yellow);
                    target.DrawPixel(this.XCenter + 1, this.YCenter - 1, Colors.Yellow);
                    break;


                case SaucerStatus.ExplodeMax:
                    target.DrawPixel(this.XCenter, this.YCenter, Colors.Red);
                    target.DrawPixel(this.XCenter - 1, this.YCenter, Colors.Red);
                    target.DrawPixel(this.XCenter, this.YCenter - 1, Colors.Red);
                    target.DrawPixel(this.XCenter + 1, this.YCenter - 1, Colors.Red);

                    target.DrawPixel(this.XCenter - 1, this.YCenter - 2, Colors.Yellow);
                    target.DrawPixel(this.XCenter + 1, this.YCenter - 2, Colors.Yellow);
                    target.DrawPixel(this.XCenter + 2, this.YCenter - 2, Colors.Yellow);
                    target.DrawPixel(this.XCenter + 1, this.YCenter - 3, Colors.Yellow);
                    target.DrawPixel(this.XCenter - 2, this.YCenter + 1, Colors.Yellow);
                    target.DrawPixel(this.XCenter + 2, this.YCenter + 1, Colors.Yellow);
                    break;
            }
        }

    }
}
