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
    /// Provides an abstract base for any fixed-pitch font set
    /// </summary>
    public abstract class Font
    {
        protected Font() { }


        private int _spacing = 1;

        /// <summary>
        /// Defines the spacing between a character and the next one
        /// </summary>
        public int Spacing
        {
            get { return this._spacing; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value");

                this._spacing = value;
            }
        }


        /// <summary>
        /// Gets the character box size
        /// </summary>
        public abstract Size BoxSize { get; }


        /// <summary>
        /// Gets the bit-pattern matrix for the character relative to the specified code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// <remarks>
        /// The bitmap pattern should follow this rule:
        /// 
        ///       first pattern
        ///            |
        ///    bit 0-  **...
        ///    bit 1-  **..*
        ///    bit 2-  ...*.
        ///    bit 3-  ..*..
        ///    bit 4-  .*...
        ///    bit 5-  *..**
        ///    bit 6-  ...**
        ///      
        ///         and so away.
        /// </remarks>
        public abstract int[] GetPattern(int code);

    }
}
