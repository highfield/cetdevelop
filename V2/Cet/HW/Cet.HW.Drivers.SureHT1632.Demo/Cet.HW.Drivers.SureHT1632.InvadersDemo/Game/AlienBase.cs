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
    public enum AlienStatus
    {
        Walk1,
        Walk2,
        StartExplosion,
        ExplodeExpand,
        ExplodeMax,
        ExplodeCollapse,
        Dead,
    }


    public enum AlienDirection
    {
        Rightward,
        RightEdge,
        Leftward,
        LeftEdge,
        GroundReached,
    }


    public abstract class AlienBase
        : IRenderable
    {
        private const int ExplosionTime = 3;
        private int _explosionClock;
        
        public int XCenter;
        public int YCenter;

        public bool IsAlive = true;
        public AlienStatus Status;

        public AlienBase AlienBelow;


        public abstract bool DetectCollision(GameStagePlay stage);


        public void Update(GameStagePlay stage)
        {
            if (this.IsAlive)
            {
                //time to move aliens?
                if (stage.ShiftAliens)
                {
                    //manage position
                    switch (stage.CurrentAlienDir)
                    {
                        case AlienDirection.Rightward:
                            this.XCenter++;
                            if (this.XCenter == (stage.ViewportRight - 2))
                            {
                                stage.NextAlienDir = AlienDirection.RightEdge;
                            }
                            break;


                        case AlienDirection.RightEdge:
                            this.YCenter++;
                            stage.NextAlienDir = (this.YCenter == stage.ViewportBottom)
                                ? AlienDirection.GroundReached
                                : AlienDirection.Leftward;
                            break;


                        case AlienDirection.Leftward:
                            this.XCenter--;
                            if (this.XCenter == 1)
                            {
                                stage.NextAlienDir = AlienDirection.LeftEdge;
                            }
                            break;


                        case AlienDirection.LeftEdge:
                            this.YCenter++;
                            stage.NextAlienDir = (this.YCenter == stage.ViewportBottom)
                                ? AlienDirection.GroundReached
                                : AlienDirection.Rightward;
                            break;
                    }

                    //update status
                    this.Status = this.Status == AlienStatus.Walk1
                        ? AlienStatus.Walk2
                        : AlienStatus.Walk1;

                    //may drop a bomb?
                    if (this.AlienBelow == null ||
                        this.AlienBelow.Status == AlienStatus.Dead)
                    {
                        //consider dropping one
                        Bomb bomb;
                        if (stage.Context.Rnd.NextDouble() > 0.8 &&
                            (bomb = stage.GetBombSlot()) != null)
                        {
                            //drop it!
                            bomb.XCenter = this.XCenter;
                            bomb.YCenter = this.YCenter;
                            bomb.IsActive = true;
                        }
                    }
                }
            }
            else if (this.Status != AlienStatus.Dead)
            {
                switch (this.Status)
                {
                    case AlienStatus.StartExplosion:
                        this.Status = AlienStatus.ExplodeExpand;
                        this._explosionClock = ExplosionTime;

                        //play related sound
                        stage.Context.PlaySound(stage.Context.SoundAlienDestroyed);
                        break;

                    case AlienStatus.ExplodeExpand:
                        if (--this._explosionClock == 0)
                        {
                            this.Status = AlienStatus.ExplodeMax;
                            this._explosionClock = ExplosionTime;
                        }
                        break;

                    case AlienStatus.ExplodeMax:
                        if (--this._explosionClock == 0)
                        {
                            this.Status = AlienStatus.ExplodeCollapse;
                            this._explosionClock = ExplosionTime;
                        }
                        break;

                    case AlienStatus.ExplodeCollapse:
                        if (--this._explosionClock == 0)
                        {
                            stage.DecreaseAliens();
                            this.Status = AlienStatus.Dead;
                        }
                        break;
                }
            }
        }


        public abstract void OnRender(CompositionTargetBase target);


        protected void RenderDeath(CompositionTargetBase target)
        {
            switch (this.Status)
            {
                case AlienStatus.ExplodeExpand:
                case AlienStatus.ExplodeCollapse:
                    target.DrawPixel(this.XCenter, this.YCenter, Colors.Red);
                    target.DrawPixel(this.XCenter + 1, this.YCenter, Colors.Red);

                    target.DrawPixel(this.XCenter - 1, this.YCenter - 1, Colors.Yellow);
                    target.DrawPixel(this.XCenter - 1, this.YCenter + 1, Colors.Yellow);
                    target.DrawPixel(this.XCenter + 1, this.YCenter - 1, Colors.Yellow);
                    break;


                case AlienStatus.ExplodeMax:
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
