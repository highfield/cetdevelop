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
    /// Defines a brush of a single color.
    /// Brushes are used to fill graphics shapes, such as rectangles, ellipses, pies, polygons, and paths.
    /// This class cannot be inherited.
    /// </summary>
    public sealed class SolidBrush
        : Brush
    {
        /// <summary>
        /// Initializes a new SolidBrush object of the specified color.
        /// </summary>
        /// <param name="color"></param>
        public SolidBrush(int color)
        {
            this.Color = color;
        }


        /// <summary>
        /// Gets or sets the color of this SolidBrush object.
        /// </summary>
        public int Color;

    }
}
