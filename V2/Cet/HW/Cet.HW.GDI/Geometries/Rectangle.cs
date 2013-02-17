using System;
using Microsoft.SPOT;

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
namespace Cet.HW.GDI
{
    /// <summary>
    /// Stores a set of four integers that represent the location and size of a rectangle
    /// </summary>
    /// <remarks>
    /// The rendering should consider the coordinate of a point as the "center" of a pixel
    /// or whatever is used as physical units. Thus, the "distance" of two points has to
    /// be considered as the physical distance between the related pixels (or else).
    /// 
    /// For instance, a rectangle having its X (left) edge at 10 and width defined as 5
    /// will have its right edge as 10+5 = 15.
    /// 
    ///    X=5       R=15
    ///     |         |
    ///     ***********
    ///     *         *
    ///     *         *
    ///     ***********
    ///     
    /// in the above rectangle, the upper edge has 11 pixels but the distance from
    /// the center of the upper-left pixel and the center of the upper-right is 10.
    /// </remarks>
    public struct Rectangle
    {
        /// <summary>
        /// Initializes a new instance of the Rectangle class with the specified location and size.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Rectangle(
            int left,
            int top,
            int width,
            int height)
        {
            if (width < 0)
                throw new ArgumentException("Width can't be negative.");

            if (height < 0)
                throw new ArgumentException("Height can't be negative.");

            this.X = left;
            this.Y = top;
            this._width = width;
            this._height = height;
        }


        /// <summary>
        /// Gets or sets the x-coordinate of the upper-left corner of this Rectangle structure.
        /// </summary>
        public int X;


        /// <summary>
        /// Gets or sets the y-coordinate of the upper-left corner of this Rectangle structure.
        /// </summary>
        public int Y;


        #region PROP Left

        /// <summary>
        /// Gets the x-coordinate of the left edge of this Rectangle structure.
        /// </summary>
        public int Left
        {
            get { return this.X; }
        }

        #endregion


        #region PROP Top

        /// <summary>
        /// Gets the y-coordinate of the top edge of this Rectangle structure.
        /// </summary>
        public int Top
        {
            get { return this.Y; }
        }

        #endregion


        #region PROP Width

        private int _width;

        /// <summary>
        /// Gets or sets the width of this Rectangle structure.
        /// </summary>
        public int Width
        {
            get { return this._width; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Width can't be negative.");

                this._width = value;
            }
        }

        #endregion


        #region PROP Height

        private int _height;

        /// <summary>
        /// Gets or sets the height of this Rectangle structure.
        /// </summary>
        public int Height
        {
            get { return this._height; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Height can't be negative.");

                this._height = value;
            }
        }

        #endregion


        #region PROP Right

        /// <summary>
        /// Gets the x-coordinate of the right edge of this Rectangle structure.
        /// </summary>
        public int Right
        {
            get { return this.X + this._width; }
        }

        #endregion


        #region PROP Bottom

        /// <summary>
        /// Gets the y-coordinate of the bottom edge of this Rectangle structure.
        /// </summary>
        public int Bottom
        {
            get { return this.Y + this._height; }
        }

        #endregion


        #region PROP IsEmpty

        /// <summary>
        /// Tests whether all numeric properties of this Rectangle have values of zero.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return
                    this._width == 0 ||
                    this._height == 0;
            }
        }

        #endregion


        /// <summary>
        /// Enlarges this Rectangle by the specified amount.
        /// </summary>
        /// <param name="dx">The amount to inflate this Rectangle horizontally.</param>
        /// <param name="dy">The amount to inflate this Rectangle vertically.</param>
        public void Inflate(
            int dx,
            int dy)
        {
            if (this.IsEmpty)
                return;

            var width = this._width + 2 * dx;

            if (width > 0)
            {
                this.X -= dx;
                this._width = width;
            }
            else
            {
                this.X += this._width / 2;
                this._width = 0;
            }

            var height = this._height + 2 * dy;

            if (height > 0)
            {
                this.Y -= dy;
                this._height = height;
            }
            else
            {
                this.Y += this._height / 2;
                this._height = 0;
            }
        }


        /// <summary>
        /// Determines if the specified point is contained within this Rectangle structure.
        /// </summary>
        /// <param name="pt">The Point to test.</param>
        /// <returns>This method returns true if the point represented by pt is contained within this Rectangle structure; otherwise false.</returns>
        public bool Contains(Point pt)
        {
            return
                this.IsEmpty == false &&
                pt.X >= this.X &&
                pt.Y >= this.Y &&
                pt.X <= this.Right &&
                pt.Y <= this.Bottom;
        }


        /// <summary>
        /// Determines if the rectangular region represented by rect is entirely contained within this Rectangle structure.
        /// </summary>
        /// <param name="rect">The Rectangle to test.</param>
        /// <returns>This method returns true if the rectangular region represented by rect is entirely contained within this Rectangle structure; otherwise false.</returns>
        public bool Contains(Rectangle rect)
        {
            return
                this.IsEmpty == false &&
                rect.IsEmpty == false &&
                rect.Right >= this.X &&
                rect.X <= this.Right &&
                rect.Bottom >= this.Y &&
                rect.Y <= this.Bottom;
        }


        /// <summary>
        /// Replaces this Rectangle with the intersection of itself and the specified Rectangle.
        /// </summary>
        /// <param name="rect">The Rectangle with which to intersect.</param>
        public void Intersect(Rectangle rect)
        {
            if (this.Contains(rect))
            {
                int left = this.X > rect.X ? this.X : rect.X;
                int top = this.Y > rect.Y ? this.Y : rect.Y;

                int right1 = this.Right;
                int right2 = rect.Right;
                this._width = (right1 < right2 ? right1 : right2) - left;

                int bottom1 = this.Bottom;
                int bottom2 = rect.Bottom;
                this._height = (bottom1 < bottom2 ? bottom1 : bottom2) - top;

                this.X = left;
                this.Y = top;
            }
            else
            {
                this = new Rectangle();
            }
        }


        /// <summary>
        /// Gets a Rectangle structure that contains the union of two Rectangle structures.
        /// </summary>
        /// <param name="a">A rectangle to union.</param>
        /// <param name="b">A rectangle to union.</param>
        /// <returns>A Rectangle structure that bounds the union of the two Rectangle structures.</returns>
        public static Rectangle Union(
            Rectangle a,
            Rectangle b)
        {
            if (a.IsEmpty || b.IsEmpty)
            {
                return new Rectangle();
            }
            else
            {
                int left = a.Left < b.Left ? a.Left : b.Left;
                int top = a.Top < b.Top ? a.Top : b.Top;

                int right1 = a.Right;
                int right2 = b.Right;
                int width = (right1 > right2 ? right1 : right2) - left;

                int bottom1 = a.Bottom;
                int bottom2 = b.Bottom;
                int height = (bottom1 > bottom2 ? bottom1 : bottom2) - top;

                return new Rectangle(
                    left, 
                    top, 
                    width, 
                    height);
            }
        }

    }
}
