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
    public class Barrier
        : IRenderable
    {

        public const int BarrierWidth = 6;
        public const int BarrierHeight = 3;
        private const int BarrierRowMask = 0x3F;

        public const int AllBricks = 0x1E | ((BarrierRowMask | (BarrierRowMask << BarrierWidth)) << BarrierWidth);

        public int BricksAlive = AllBricks;

        public int XLeft;
        public int YTop;


        public bool DetectCollision(GameStagePlay stage)
        {
            int xoffs, yoffs;

            //check missile collision
            Missile missile = stage.Missile;

            if (missile.IsActive &&
                (xoffs = (missile.XCenter - this.XLeft)) >= 0 &&
                xoffs < BarrierWidth &&
                (yoffs = (missile.YCenter - this.YTop)) >= 0 &&
                yoffs < BarrierHeight)
            {
                int mask = 1 << (yoffs * BarrierWidth + xoffs);

                if ((this.BricksAlive & mask) != 0)
                {
                    //got it!
                    this.ExplodeBricksAroundPoint(xoffs, yoffs);
                    this.ExplodeBricksAroundPoint(xoffs - 1, yoffs);
                    this.ExplodeBricksAroundPoint(xoffs + 1, yoffs);
                    this.ExplodeBricksAroundPoint(xoffs, yoffs - 1);
                    this.ExplodeBricksAroundPoint(xoffs, yoffs + 1);
                    missile.IsActive = false;
                    return true;
                }
            }

            //check bombs collision
            for (int i = stage.Bombs.Length - 1; i >= 0; i--)
            {
                Bomb bomb;
                if ((bomb = stage.Bombs[i]).IsActive &&
                    (xoffs = (bomb.XCenter - this.XLeft)) >= 0 &&
                    xoffs < BarrierWidth &&
                    (yoffs = (bomb.YCenter - this.YTop)) >= 0 &&
                    yoffs < BarrierHeight)
                {
                    int mask = 1 << (yoffs * BarrierWidth + xoffs);

                    if ((this.BricksAlive & mask) != 0)
                    {
                        //got it!
                        this.ExplodeBricksAroundPoint(xoffs, yoffs);
                        this.ExplodeBricksAroundPoint(xoffs - 1, yoffs);
                        this.ExplodeBricksAroundPoint(xoffs + 1, yoffs);
                        this.ExplodeBricksAroundPoint(xoffs, yoffs - 1);
                        this.ExplodeBricksAroundPoint(xoffs, yoffs + 1);
                        bomb.IsActive = false;
                        return true;
                    }
                }
            }

            return false;
        }


        private void ExplodeBricksAroundPoint(
            int xoffs,
            int yoffs)
        {
            if (xoffs >= 0 &&
                xoffs < BarrierWidth &&
                yoffs >= 0 &&
                yoffs < BarrierHeight)
            {
                int mask = 1 << (yoffs * BarrierWidth + xoffs);
                this.BricksAlive &= ~mask;
            }
        }


        public void OnRender(CompositionTargetBase target)
        {
            int bricks;
            if ((bricks = this.BricksAlive) != 0)
            {
                for (int y = 0; y < BarrierHeight; y++)
                {
                    int yy = this.YTop + y;

                    for (int x = 0; x < BarrierWidth; x++)
                    {
                        if ((bricks & 1) != 0)
                        {
                            target.DrawPixel(
                                this.XLeft + x,
                                yy,
                                Colors.Lime);
                        }

                        bricks >>= 1;
                    }
                }
            }
        }

    }
}
