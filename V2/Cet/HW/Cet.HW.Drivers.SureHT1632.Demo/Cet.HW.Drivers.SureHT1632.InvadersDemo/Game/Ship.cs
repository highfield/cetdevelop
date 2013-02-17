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
    public enum ShipStatus
    {
        Alive,
        StartExplosion,
        ExplodeExpand,
        ExplodeMax,
        ExplodeCollapse,
        Dead,
    }


    public class Ship
        : IRenderable
    {
        private const int ExplosionTime = 3;
        private int _explosionClock;

        public int XCenter;
        public int YCenter;

        public bool IsAlive = true;
        public ShipStatus Status;


        public bool DetectCollision(GameStagePlay stage)
        {
            if (this.IsAlive)
            {
                foreach (Bomb bomb in stage.Bombs)
                {
                    if (bomb.IsActive &&
                        bomb.YCenter <= this.YCenter &&
                        bomb.YCenter >= this.YCenter - 1 &&
                        bomb.XCenter >= this.XCenter - 1 &&
                        bomb.XCenter <= this.XCenter + 1)
                    {
                        //got it!
                        this.IsAlive = false;
                        this.Status = ShipStatus.StartExplosion;
                        bomb.IsActive = false;
                        return true;
                    }
                }
            }

            return false;
        }


        public void Update(GameStagePlay stage)
        {
            var context = stage.Context;

            if (this.IsAlive)
            {
                if (context.IsEvenClock)
                {
                    if (context.LeftButton)
                    {
                        if (this.XCenter > 1)
                            this.XCenter--;
                    }
                    else if (context.RightButton)
                    {
                        if (this.XCenter < (stage.ViewportRight - 1))
                            this.XCenter++;
                    }
                    else if (context.FireButton)
                    {
                        //check missile availability
                        if (stage.Missile.IsActive == false)
                        {
                            //fire it!
                            stage.Missile.XCenter = this.XCenter;
                            stage.Missile.YCenter = this.YCenter - 1;
                            stage.Missile.IsActive = true;
                        }
                    }
                }
            }
            else if (this.Status != ShipStatus.Dead)
            {
                switch (this.Status)
                {
                    case ShipStatus.StartExplosion:
                        this.Status = ShipStatus.ExplodeExpand;
                        this._explosionClock = ExplosionTime;

                        //play related sound
                        context.PlaySound(context.SoundShipDestroyed);
                        break;

                    case ShipStatus.ExplodeExpand:
                        if (--this._explosionClock == 0)
                        {
                            this.Status = ShipStatus.ExplodeMax;
                            this._explosionClock = ExplosionTime;
                        }
                        break;

                    case ShipStatus.ExplodeMax:
                        if (--this._explosionClock == 0)
                        {
                            this.Status = ShipStatus.ExplodeCollapse;
                            this._explosionClock = ExplosionTime;
                        }
                        break;

                    case ShipStatus.ExplodeCollapse:
                        if (--this._explosionClock == 0)
                        {
                            this.IsAlive = stage.DecreaseShips();
                            this.Status = this.IsAlive
                                ? ShipStatus.Alive
                                : ShipStatus.Dead;
                        }
                        break;
                }
            }
        }


        public void OnRender(CompositionTargetBase target)
        {
            switch (this.Status)
            {
                case ShipStatus.Alive:
                    target.DrawPixel(this.XCenter, this.YCenter, Colors.Yellow);
                    target.DrawPixel(this.XCenter, this.YCenter - 1, Colors.Yellow);
                    target.DrawPixel(this.XCenter - 1, this.YCenter, Colors.Yellow);
                    target.DrawPixel(this.XCenter + 1, this.YCenter, Colors.Yellow);
                    break;


                case ShipStatus.ExplodeExpand:
                case ShipStatus.ExplodeCollapse:
                    target.DrawPixel(this.XCenter, this.YCenter, Colors.Red);
                    target.DrawPixel(this.XCenter + 1, this.YCenter, Colors.Red);

                    target.DrawPixel(this.XCenter - 1, this.YCenter - 1, Colors.Yellow);
                    target.DrawPixel(this.XCenter - 1, this.YCenter + 1, Colors.Yellow);
                    target.DrawPixel(this.XCenter + 1, this.YCenter - 1, Colors.Yellow);
                    break;


                case ShipStatus.ExplodeMax:
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
