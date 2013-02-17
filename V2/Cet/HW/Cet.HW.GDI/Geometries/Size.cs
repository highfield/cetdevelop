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
    /// Stores an ordered pair of integers, which specify a Height and Width.
    /// </summary>
    public struct Size
    {
        /// <summary>
        /// Initializes a new instance of the Size structure from the specified dimensions.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Size(
            int width,
            int height)
        {
            if (width < 0)
                throw new ArgumentException("Width can't be negative.");

            if (height < 0)
                throw new ArgumentException("Height can't be negative.");

            this._width = width;
            this._height = height;
        }


        #region PROP Width

        private int _width;

        /// <summary>
        /// Gets or sets the horizontal component of this Size structure.
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
        /// Gets or sets the vertical component of this Size structure.
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


        #region PROP IsEmpty

        /// <summary>
        /// Tests whether this Size structure has width and height of 0.
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

    }
}
