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
    public class Sprite
    {
        public Sprite(
            int width,
            int height)
        {
            if (width <= 0)
                throw new ArgumentException("Width must be positive.");

            if (height <= 0)
                throw new ArgumentException("Height must be positive.");

            this.Width = width;
            this.Height = height;
        }


        public readonly int Width;
        public readonly int Height;
        public readonly int[] Data;

    }
}
