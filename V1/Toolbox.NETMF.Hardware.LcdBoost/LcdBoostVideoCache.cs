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
    public class LcdBoostVideoCache
    {
        internal LcdBoostVideoCache(
            int width,
            int height)
        {
            this.Width = width;
            this.Height = height;

            //allocate the buffers
            this.Data = new byte[height][];

            for (int i = 0, count = height; i < count; i++)
                this.Data[i] = new byte[width];

            //create the blank-video char array
            this._videoBlank = new byte[width];
            for (int i = 0, length = this._videoBlank.Length; i < length; i++)
                this._videoBlank[i] = 0x20; //space
        }



        private readonly byte[] _videoBlank;

        public readonly byte[][] Data;
        public readonly int Width;
        public readonly int Height;



        /// <summary>
        /// Clear the local video cache
        /// </summary>
        public void Clear()
        {
            //fill the video buffer with spaces
            for (int i = 0, count = this.Height; i < count; i++)
            {
                Array.Copy(
                    this._videoBlank,
                    this.Data[i],
                    this.Width);
            }
        }
    }
}
