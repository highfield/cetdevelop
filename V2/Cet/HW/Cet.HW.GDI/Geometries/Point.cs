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
    /// Represents an ordered pair of integer x- and y-coordinates 
    /// that defines a point in a two-dimensional plane.
    /// </summary>
    public struct Point
    {
        /// <summary>
        /// Initializes a new instance of the Point class with the specified coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Point(
            int x,
            int y)
        {
            this.X = x;
            this.Y = y;
        }


        /// <summary>
        /// Gets or sets the x-coordinate of this Point.
        /// </summary>
        public int X;


        /// <summary>
        /// Gets or sets the y-coordinate of this Point.
        /// </summary>
        public int Y;


        /// <summary>
        /// Translates this Point by the specified amount.
        /// </summary>
        /// <param name="dx">The amount to offset the x-coordinate.</param>
        /// <param name="dy">The amount to offset the y-coordinate.</param>
        public void Offset(
            int dx,
            int dy)
        {
            this.X += dx;
            this.Y += dy;
        }

    }
}
