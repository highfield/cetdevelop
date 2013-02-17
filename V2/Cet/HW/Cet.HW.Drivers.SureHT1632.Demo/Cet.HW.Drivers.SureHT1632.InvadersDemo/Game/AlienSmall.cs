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
    public class AlienSmall
        : AlienBase
    {

        public const int Score = 30;


        public override bool DetectCollision(GameStagePlay stage)
        {
            if (this.IsAlive)
            {
                Missile missile = stage.Missile;

                if (missile.IsActive)
                {
                    switch (this.Status)
                    {
                        case AlienStatus.Walk1:
                            if (missile.YCenter <= this.YCenter &&
                                missile.YCenter >= this.YCenter - 1 &&
                                missile.XCenter >= this.XCenter - 1 &&
                                missile.XCenter <= this.XCenter + 1)
                            {
                                //got it!
                                this.IsAlive = false;
                                this.Status = AlienStatus.StartExplosion;
                                missile.IsActive = false;
                                stage.Context.CurrentScore += Score;
                                return true;
                            }
                            break;


                        case AlienStatus.Walk2:
                            if (missile.YCenter <= this.YCenter &&
                                missile.YCenter >= this.YCenter - 1 &&
                                missile.XCenter == this.XCenter)
                            {
                                //got it!
                                this.IsAlive = false;
                                this.Status = AlienStatus.StartExplosion;
                                missile.IsActive = false;
                                stage.Context.CurrentScore += Score;
                                return true;
                            }
                            break;
                    }
                }
            }

            return false;
        }


        public override void OnRender(CompositionTargetBase target)
        {
            if (this.IsAlive)
            {
                //alien walking
                switch (this.Status)
                {
                    case AlienStatus.Walk1:
                        //legs
                        target.DrawPixel(this.XCenter - 1, this.YCenter, Colors.Lime);
                        target.DrawPixel(this.XCenter + 1, this.YCenter, Colors.Lime);

                        //head
                        target.DrawPixel(this.XCenter, this.YCenter - 1, Colors.Red);
                        break;


                    case AlienStatus.Walk2:
                        //leg(s)
                        target.DrawPixel(this.XCenter, this.YCenter, Colors.Lime);

                        //head
                        target.DrawPixel(this.XCenter, this.YCenter - 1, Colors.Red);
                        break;
                }
            }
            else
            {
                this.RenderDeath(target);
            }
        }

    }
}
