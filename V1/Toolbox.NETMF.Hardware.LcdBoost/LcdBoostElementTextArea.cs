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
    public class LcdBoostElementTextArea
        : ILcdBoostElement
    {
        public const byte Transparent = 0x0F;
        public const byte Opaque = 0x20;

        private byte[][] _cache;
        private bool _changed = true;

        public readonly LcdBoostRect Bounds = new LcdBoostRect();
        public bool IsHidden { get; set; }



        private string _text;

        public string Text
        {
            get { return this._text; }
            set
            {
                if (this._text != value)
                {
                    this._text = value;
                    this._changed = true;
                }
            }
        }



        private byte _background = Opaque;

        public byte Background
        {
            get { return this._background; }
            set
            {
                if (this._background != value)
                {
                    this._background = value;
                    this._changed = true;
                }
            }
        }



        private void Recalc()
        {
            int width = this.Bounds.Width;
            int height = this.Bounds.Height;

            //create the cache jagged array
            this._cache = new byte[height][];

            if (width > 0 &&
                height > 0)
            {
                //fills the background of just one row
                byte[] vrow = new byte[width];
                this._cache[0] = vrow;
                for (int x = 0; x < width; x++)
                    vrow[x] = this._background;

                //create then fills the remaining rows
                for (int y = 1; y < height; y++)
                {
                    var temp = new byte[width];
                    this._cache[y] = temp;
                    Array.Copy(
                        vrow,
                        temp,
                        width);
                }

                //draw the text onto the cache surface
                if (this._text != null)
                {
                    int x = 0;
                    int y = 0;
                    vrow = this._cache[y];

                    for (int i = 0, count = this._text.Length; i < count; i++)
                    {
                        var code = (int)this._text[i];

                        if (code != 0x0D)   //CR?
                        {
                            //nope: append to the current row
                            vrow[x++] = (byte)code;
                            if (x < width)
                                continue;
                        }

                        //consider a new line
                        y++;
                        x = 0;

                        //out of vertical space?
                        if (y >= height)
                            break;
                        else
                            vrow = this._cache[y];
                    }
                }
            }

            //the cache has been rebuilt
            this._changed = false;
            this.Bounds.WidthHeightChanged = false;
        }



        #region ILcdBoostElement members

        void ILcdBoostElement.Render(
            LcdBoostVideoCache cache,
            LcdBoostRect container
            )
        {
            //rebuild the local cache when something has been changed
            if (this._changed ||
                this.Bounds.WidthHeightChanged)
            {
                this.Recalc();
            }

            /**
             * calculate actual rect bounds to be used
             **/

            //gets the upper-left point from where the
            //data of the local cache have to be read from
            int xs = System.Math.Max(0, -this.Bounds.Left);
            int ys = System.Math.Max(0, -this.Bounds.Top);

            //gets the upper-left point from where the
            //data of the global cache have to be written to
            int xd = System.Math.Max(0, this.Bounds.Left) + container.Left;
            int yd = System.Math.Max(0, this.Bounds.Top) + container.Top;

            //gets the actual box size to be considered
            int w = System.Math.Min(container.Right, container.Left + this.Bounds.Right) - xd + 1;
            int h = System.Math.Min(container.Bottom, container.Top + this.Bounds.Bottom) - yd + 1;

            /**
             * hard-copy the local cache onto the global
             **/

            if (this._background == Transparent)
            {
                //must take in account every single byte
                //to check whether is transparent
                for (int iy = 0; iy < h; iy++)
                {
                    byte[] src_row = this._cache[iy + ys];
                    byte[] dest_row = cache.Data[iy + yd];

                    for (int ix = 0; ix < w; ix++)
                    {
                        byte code = src_row[ix + xs];
                        if (code != Transparent)
                            dest_row[ix + xd] = code;
                    }
                }
            }
            else
            {
                //it assumes that the whole area is opaque
                //thus the copy is much simpler (and faster)
                for (int iy = 0; iy < h; iy++)
                {
                    byte[] src_row = this._cache[iy + ys];
                    byte[] dest_row = cache.Data[iy + yd];
                    Array.Copy(
                        src_row,
                        xs,
                        dest_row,
                        xd,
                        w);
                }
            }
        }

        #endregion
    }
}
