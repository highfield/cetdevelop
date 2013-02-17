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
namespace Cet.HW.Drivers.BallsDemo
{
    public class Ball
        : IRenderable
    {
        private const int EdgeLength = 3;


        public Ball(int hash)
        {
            //pick a random position as well as a random speed
            this._xpos = 8 + hash & 15;
            this._ypos = 4 + (hash >> 4) & 7;
            this._xspeed = ((hash >> 8) & 3) - 1.5f;
            this._yspeed = ((hash >> 10) & 3) - 1.5f;

            //define the ball rectangle
            this._rect = new Rectangle(
                (int)(this._xpos + 0.5f),
                (int)(this._ypos + 0.5f),
                EdgeLength,
                EdgeLength);

            //create both brush and pen for blending
            this._pen = new Pen(Colors.Lime);
            this._brush = new SolidBrush(Colors.Red);
        }


        private float _xpos;
        private float _ypos;
        private float _xspeed;
        private float _yspeed;

        private Rectangle _rect;

        private int _rightBound;
        private int _bottomBound;

        private Pen _pen;
        private Brush _brush;


        public void OnRender(CompositionTargetBase target)
        {
            //render the ball as a square at the current position
            this._rect.X = (int)(this._xpos + 0.5f);
            this._rect.Y = (int)(this._ypos + 0.5f);

            target.FillRectangle(
                this._brush,
                this._rect);

            target.DrawRectangle(
                this._pen,
                this._rect);

            //move the ball according to its speed
            this._xpos += this._xspeed;
            this._ypos += this._yspeed;

            //checks the viewport edges and makes the ball bouncing
            if (this._rightBound == 0)
            {
                Size vp = target.ViewportSize;
                this._rightBound = vp.Width - EdgeLength;
                this._bottomBound = vp.Height - EdgeLength;
            }

            if (this._xpos < 0)
            {
                this._xspeed = -this._xspeed;
                this._xpos += this._xspeed;
            }
            else if (this._xpos >= this._rightBound)
            {
                this._xspeed = -this._xspeed;
                this._xpos += this._xspeed;
            }

            if (this._ypos < 0)
            {
                this._yspeed = -this._yspeed;
                this._ypos += this._yspeed;
            }
            else if (this._ypos >= this._bottomBound)
            {
                this._yspeed = -this._yspeed;
                this._ypos += this._yspeed;
            }
        }

    }
}
