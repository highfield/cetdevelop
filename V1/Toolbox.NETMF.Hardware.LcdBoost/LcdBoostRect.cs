using System;
using Microsoft.SPOT;

/*
 * Copyright 2012 Mario Vernari (http://netmftoolbox.codeplex.com/)
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
namespace Toolbox.NETMF.Hardware
{
    public sealed class LcdBoostRect
    {
        public bool LeftTopChanged;
        public bool WidthHeightChanged;



        private int _left;

        public int Left
        {
            get { return this._left; }
            set
            {
                if (this._left != value)
                {
                    this._left = value;
                    this.LeftTopChanged = true;
                }
            }
        }



        private int _top;

        public int Top
        {
            get { return this._top; }
            set
            {
                if (this._top != value)
                {
                    this._top = value;
                    this.LeftTopChanged = true;
                }
            }
        }



        private int _width;

        public int Width
        {
            get { return this._width; }
            set
            {
                if (this._width != value)
                {
                    if (value < 0)
                        throw new ArgumentException("Width cannot be negative");

                    this._width = value;
                    this.WidthHeightChanged = true;
                }
            }
        }



        private int _height;

        public int Height
        {
            get { return this._height; }
            set
            {
                if (this._height != value)
                {
                    if (value < 0)
                        throw new ArgumentException("Height cannot be negative");

                    this._height = value;
                    this.WidthHeightChanged = true;
                }
            }
        }



        public int Right
        {
            get { return this._left + this._width - 1; }
        }



        public int Bottom
        {
            get { return this._top + this._height - 1; }
        }



        public bool IsEmpty
        {
            get
            {
                return
                    this._width == 0 ||
                    this._height == 0;
            }
        }



        public void Inflate(
            int dx,
            int dy)
        {
            this.Left -= dx;
            this.Top -= dy;
            this.Width += dx + dx;
            this.Height += dy + dy;
        }



        public bool Contains(
            int x,
            int y)
        {
            return
                this._left <= x &&
                this._top <= y &&
                this.Right >= x &&
                this.Bottom >= y;
        }



        public LcdBoostRect Intersect(LcdBoostRect other)
        {
            var result = new LcdBoostRect();

            result._left = System.Math.Max(
                other._left, 
                this._left);

            result._top = System.Math.Max(
                other._top, 
                this._top);

            result._width = System.Math.Min(other.Right, this.Right) - result.Left + 1;
            result._height = System.Math.Min(other.Bottom, this.Bottom) - result.Top + 1;

            return result;
        }
    }
}
