using System;
using Microsoft.SPOT;

using Cet.HW.GDI;

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
namespace Cet.HW.Drivers
{
    public class CompositionTargetHD44780
        : CompositionTargetBase
    {

        private const byte PixelOff = 0x20;   //space
        private const byte PixelOn = 0xFF;    //dark box

        private const int OpaqueMask = unchecked((int)0x80000000);


        public CompositionTargetHD44780(
            int width,
            int height)
        {
            this._width = width;
            this._height = height;
            this._cache = new byte[height][];

            for (int i = 0; i < height; i++)
            {
                this._cache[i] = new byte[width];
            }

            //define a rectangle as the logical viewport
            this._viewport = new Rectangle(
                0,
                0,
                width - 1,
                height - 1
                );

            //create the blank-video char array
            this._videoBlank = new byte[width];
            for (int i = 0; i < width; i++)
                this._videoBlank[i] = 0x20; //space

            this.Clear();
        }


        private readonly int _width;
        private readonly int _height;
        private readonly byte[][] _cache;

        private readonly byte[] _videoBlank;
        private readonly Rectangle _viewport;


        public override Size ViewportSize
        {
            get { return new Size(this._width, this._height); }
        }


        public override void Clear()
        {
            //fill the video buffer with spaces
            for (int i = 0; i < this._height; i++)
            {
                Array.Copy(
                    this._videoBlank,
                    this._cache[i],
                    this._width);
            }

            this.ChangeHash();
        }


        #region Pixel

        public override void DrawPixel(
            Point pt,
            int color)
        {
            this.DrawPixel(
                pt.X,
                pt.Y,
                color);
        }


        public override void DrawPixel(
            int x,
            int y,
            int color)
        {
            if ((color & OpaqueMask) != 0 &&
                x >= 0 &&
                x < this._width &&
                y >= 0 &&
                y < this._height)
            {
                this._cache[y][x] = color == Colors.Black ? PixelOff : PixelOn;
                this.ChangeHash();
            }
        }

        #endregion


        #region Line

        public override void DrawLine(
            Pen pen,
            Point a,
            Point b)
        {
            this.DrawLine(
                pen,
                a.X,
                a.Y,
                b.X,
                b.Y);
        }


        public override void DrawLine(
            Pen pen,
            int x1,
            int y1,
            int x2,
            int y2)
        {
            if ((pen.Color & OpaqueMask) == 0)
                return;

            if (x1 == x2)
            {
                if (y1 == y2)
                {
                    //point
                    this.DrawPixel(x1, y1, pen.Color);
                }
                else if (x1 >= 0 && x1 < this._width)
                {
                    //vertical
                    int ymin = y1 < y2 ? y1 : y2;
                    int ymax = y1 > y2 ? y1 : y2;
                    if (ymax >= this._height)
                        ymax = this._height - 1;

                    for (int y = ymin >= 0 ? ymin : 0; y <= ymax; y++)
                    {
                        this._cache[y][x1] = pen.Color == Colors.Black ? PixelOff : PixelOn;
                    }
                }
            }
            else if (
                y1 == y2 &&
                y1 >= 0 &&
                y1 < this._height)
            {
                //horizontal
                int xmin = x1 < x2 ? x1 : x2;
                int xmax = x1 > x2 ? x1 : x2;
                if (xmax >= this._width)
                    xmax = this._width - 1;

                for (int x = xmin >= 0 ? xmin : 0; x <= xmax; x++)
                {
                    this._cache[y1][x] = pen.Color == Colors.Black ? PixelOff : PixelOn;
                }
            }
            else
            {
                //diagonal
                int dx = x2 - x1;
                int dy = y2 - y1;
                float slope = (float)dy / dx;

                float xstep, ystep;
                int count;

                if (slope < -1.0f ||
                    slope > 1.0f)
                {
                    //almost vertical
                    count = 1 + (dy >= 0 ? dy : -dy);
                    ystep = y2 > y1 ? 1 : -1;
                    xstep = ystep / slope;
                }
                else
                {
                    //almost horizontal
                    count = 1 + (dx >= 0 ? dx : -dx);
                    xstep = x2 > x1 ? 1 : -1;
                    ystep = slope * xstep;
                }

                //TODO: should be optimized
                for (int j = 0; j < count; j++)
                {
                    this.DrawPixel(
                        (int)(j * xstep + x1 + 0.5f),
                        (int)(j * ystep + y1 + 0.5f),
                        pen.Color);
                }
            }

            this.ChangeHash();
        }

        #endregion


        #region Rectangle

        public override void DrawRectangle(
            Pen pen,
            Rectangle rect)
        {
            this.DrawRectangle(
                pen,
                rect.X,
                rect.Y,
                rect.Width,
                rect.Height);
        }


        public override void DrawRectangle(
            Pen pen,
            int x,
            int y,
            int width,
            int height)
        {
            if ((pen.Color & OpaqueMask) == 0)
                return;

            int offset = pen.Thickness / 2;

            int left = x - offset;
            int right = x + width + offset;
            int top = y - offset;
            int bottom = y + height + offset;

            for (int k = 0; k < pen.Thickness; k++)
            {
                //upper edge
                this.DrawLine(
                    pen,
                    left,
                    top,
                    right,
                    top);

                //lower edge
                this.DrawLine(
                    pen,
                    left,
                    bottom,
                    right,
                    bottom);

                //left edge
                this.DrawLine(
                    pen,
                    left,
                    top,
                    left,
                    bottom);

                //right edge
                this.DrawLine(
                    pen,
                    right,
                    top,
                    right,
                    bottom);

                left++;
                top++;
                right--;
                bottom--;
            }

            this.ChangeHash();
        }


        public override void FillRectangle(
            Brush brush,
            Rectangle rect)
        {
            SolidBrush sb = brush as SolidBrush;
            int color;
            if (sb == null ||
                ((color = sb.Color) & OpaqueMask) == 0)
            {
                //no actual color
                return;
            }

            //interects the specified rectangle with the viewport
            Rectangle rt = rect;
            rt.Intersect(_viewport);

            if (rt.IsEmpty == false)
            {
                //selects the blender to use for faster filling
                byte code = color == Colors.Black ? PixelOff : PixelOn;

                for (int y = rt.Top, ym = rt.Bottom; y <= ym; y++)
                {
                    for (int x = rt.Left, xm = rt.Right; x <= xm; x++)
                    {
                        this._cache[y][x] = code;
                    }
                }

                this.ChangeHash();
            }
        }


        public override void FillRectangle(
            Brush brush,
            int x,
            int y,
            int width,
            int height)
        {
            var rect = new Rectangle(
                x,
                y,
                width,
                height);

            this.FillRectangle(
                brush,
                rect);
        }

        #endregion


        #region Ellipse

        public override void DrawEllipse(
            Pen pen,
            Rectangle rect)
        {
            this.DrawEllipse(
                pen,
                rect.X,
                rect.Y,
                rect.Width,
                rect.Height);
        }


        public override void DrawEllipse(
            Pen pen,
            int x,
            int y,
            int width,
            int height)
        {
            //TODO
            throw new NotImplementedException();
        }


        public override void FillEllipse(
            Brush brush,
            Rectangle rect)
        {
            this.FillEllipse(
                brush,
                rect.X,
                rect.Y,
                rect.Width,
                rect.Height);
        }


        public override void FillEllipse(
            Brush brush,
            int x,
            int y,
            int width,
            int height)
        {
            //TODO
            throw new NotImplementedException();
        }

        #endregion


        #region Text

        public override void DrawString(
            string text,
            Font font,
            Brush brush,
            Point pt)
        {
            this.DrawString(
                text,
                font,
                brush,
                pt.X,
                pt.Y);
        }


        public override void DrawString(
            string text,
            Font font,
            Brush brush,
            int x,
            int y)
        {
            SolidBrush sb = brush as SolidBrush;
            int color;
            if (sb == null ||
                ((color = sb.Color) & OpaqueMask) == 0)
            {
                //no actual color
                return;
            }

            if (font != null)
            {
                Size fsize = font.BoxSize;
                int length = text.Length;

                if (text == null ||
                    length == 0 ||
                    x >= this._width ||
                    y >= this._height ||
                    y <= -fsize.Height)
                {
                    //can skip the process
                    return;
                }

                int w1 = fsize.Width - 1;
                int wt = fsize.Width + font.Spacing;

                int right;
                for (int i = 0; i < length; i++)
                {
                    right = x + w1;

                    if (right >= 0)
                    {
                        if (x >= this._width)
                        {
                            break;
                        }

                        int code = (int)text[i];
                        int[] pattern = font.GetPattern(code);

                        for (int col = 0; col < fsize.Width; col++)
                        {
                            int xx = x + col;
                            if (xx < 0 || xx >= this._width)
                                continue;

                            int currentPattern = pattern[col];
                            for (int row = 0; row < fsize.Height; row++)
                            {
                                int yy = y + row;
                                if (yy >= 0 &&
                                    yy < this._height &&
                                    (currentPattern & 1) != 0)
                                {
                                    this._cache[yy][xx] = color == Colors.Black ? PixelOff : PixelOn;
                                }

                                currentPattern >>= 1;
                            }
                        }
                    }

                    x += wt;
                }
            }
            else
            {
                //no font specified, thus use the native chargen
                int length = text.Length;

                if (text == null ||
                    length == 0 ||
                    x >= this._width ||
                    y < 0 ||
                    y >= this._height)
                {
                    //can skip the process
                    return;
                }

                for (int i = 0; i < length; i++)
                {
                    int xx = x + i;
                    if (xx >= 0 && xx < this._width)
                    {
                        this._cache[y][xx] = (byte)text[i];
                    }
                }
            }

            this.ChangeHash();
        }


        public override Size MeasureString(
            string text,
            Font font)
        {
            int length = text.Length;

            if (text == null ||
                length == 0)
            {
                //nothing to consider
                return new Size();
            }
            else if (font != null)
            {
                Size fsize = font.BoxSize;
                int width = length * fsize.Width + (length - 1) * font.Spacing;

                return new Size(
                    width,
                    fsize.Height);
            }
            else
            {
                //no font specified, thus use the native chargen
                return new Size(length, 1);
            }
        }

        #endregion


        #region Sprite

        public override void DrawSprite(
            Sprite sprite,
            Point pt)
        {
            this.DrawSprite(
                sprite,
                pt.X,
                pt.Y);
        }


        public override void DrawSprite(
            Sprite sprite,
            int x,
            int y)
        {
            //TODO
            throw new NotImplementedException();
        }

        #endregion


        #region Driver specific

        /// <summary>
        /// Dumps the color components array for the driver
        /// </summary>
        internal byte[][] GetBuffers()
        {
            return this._cache;
        }

        #endregion
    }
}
