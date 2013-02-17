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
    /// Defines an object used to draw lines and curves. This class cannot be inherited.
    /// </summary>
    public sealed class Pen
    {
        /// <summary>
        /// Initializes a new instance of the Pen class with the specified color.
        /// </summary>
        /// <param name="color"></param>
        public Pen(int color)
            : this(color, 1)
        {
        }


        /// <summary>
        /// Initializes a new instance of the Pen class with the specified Color and Thickness properties.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="thickness"></param>
        public Pen(
            int color, 
            int thickness)
        {
            //thickness must be strictly positive
            if (thickness <= 0)
                throw new ArgumentOutOfRangeException("thickness");

            this.Color = color;
            this._thickness = thickness;
        }


        /// <summary>
        /// Gets or sets the color of this Pen.
        /// </summary>
        public int Color;


        private int _thickness;

        /// <summary>
        /// Gets or sets the thickness of this Pen.
        /// </summary>
        public int Thickness
        {
            get { return this._thickness; }
            set
            {
                //thickness must be strictly positive
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");

                this._thickness = value;
            }
        }

    }
}
