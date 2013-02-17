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
    public class CompositionTargetSureHT1632
        : CompositionTargetBase
    {

        public const int RowCount = 16;
        public const int ColumnCount = 32;

        private const int RowMask = RowCount - 1;
        private const int RowMaskNot = ~RowMask;
        private const int ColumnMask = ColumnCount - 1;
        private const int ColumnMaskNot = ~ColumnMask;

        private const int CacheSize = RowCount * ColumnCount;

        private const int OpaqueMask = unchecked((int)0x80000000);


        static CompositionTargetSureHT1632()
        {
            for (int i = 0; i < ColumnCount; i++)
            {
                _blenderRed[i] = Colors.Red;
                _blenderGreen[i] = Colors.Lime;
                _blenderYellow[i] = Colors.Yellow;
            }
        }


        private static readonly int[] _blenderBlack = new int[ColumnCount];
        private static readonly int[] _blenderRed = new int[ColumnCount];
        private static readonly int[] _blenderGreen = new int[ColumnCount];
        private static readonly int[] _blenderYellow = new int[ColumnCount];

        private static readonly Rectangle _viewport = new Rectangle(
            0,
            0,
            ColumnCount - 1,
            RowCount - 1
            );

        private readonly int[] _cache = new int[CacheSize];


        public override Size ViewportSize
        {
            get { return new Size(ColumnCount, RowCount); }
        }


        public override void Clear()
        {
            Array.Clear(
                this._cache,
                0,
                CacheSize);

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
            if ((x & ColumnMaskNot) == 0 &&
                (y & RowMaskNot) == 0 &&
                (color & OpaqueMask) != 0)
            {
                this._cache[x + y * ColumnCount] = color;
                this.ChangeHash();
            }
        }

        #endregion


        private void DrawPoint(
            Pen pen,
            int x,
            int y)
        {
            if (pen.Thickness == 1)
            {
                this.DrawPixel(x, y, pen.Color);
            }
            else
            {
                float offset = 0.5f * (1 - pen.Thickness);
                int edge = pen.Thickness - 1;

                this.FillRectangle(
                    new SolidBrush(pen.Color),
                    (int)(x + offset),
                    (int)(y + offset),
                    edge,
                    edge);
            }
        }


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
                    this.DrawPoint(pen, x1, y1);
                }
                else if ((x1 & ColumnMaskNot) == 0)
                {
                    //vertical
                    int ymin = y1 < y2 ? y1 : y2;
                    int ymax = y1 > y2 ? y1 : y2;
                    if (ymax >= RowCount)
                        ymax = RowCount - 1;

                    for (int y = ymin >= 0 ? ymin : 0; y <= ymax; y++)
                    {
                        this.DrawPoint(pen, x1, y);
                        //this._cache[x1 + y * ColumnCount] = pen.Color;
                    }
                }
            }
            else if (
                y1 == y2 &&
                (y1 & RowMaskNot) == 0)
            {
                //horizontal
                int xmin = x1 < x2 ? x1 : x2;
                int xmax = x1 > x2 ? x1 : x2;
                if (xmax >= ColumnCount)
                    xmax = ColumnCount - 1;

                for (int x = xmin >= 0 ? xmin : 0; x <= xmax; x++)
                {
                    this.DrawPoint(pen, x, y1);
                    //this._cache[x + y1 * ColumnCount] = pen.Color;
                }
            }
            else
            {
                //diagonal
                int dx = x2 - x1;
                int dy = y2 - y1;
                float slope = (float)dy / dx;

                //float xstep, ystep;
                //int count;

                if (slope < -1.0f ||
                    slope > 1.0f)
                {
                    //almost vertical
                    //count = 1 + (dy >= 0 ? dy : -dy);
                    //ystep = y2 > y1 ? 1 : -1;
                    //xstep = ystep / slope;

                    if (y1 < y2)
                    {
                        int ystart = y1 >= 0 ? y1 : 0;
                        int ystop = y2 <= RowCount ? y2 : RowCount;

                        for (int y = ystart; y <= ystop; y++)
                        {
                            this.DrawPoint(
                                pen,
                                (int)(x1 + 0.5f + (y - y1) / slope),
                                y);
                        }
                    }
                    else
                    {
                        int ystart = y2 >= 0 ? y2 : 0;
                        int ystop = y1 <= RowCount ? y1 : RowCount;

                        for (int y = ystart; y <= ystop; y++)
                        {
                            this.DrawPoint(
                                pen,
                                (int)(x2 + 0.5f + (y - y2) / slope),
                                y);
                        }
                    }
                }
                else
                {
                    //almost horizontal
                    //count = 1 + (dx >= 0 ? dx : -dx);
                    //xstep = x2 > x1 ? 1 : -1;
                    //ystep = slope * xstep;

                    if (x1 < x2)
                    {
                        int xstart = x1 >= 0 ? x1 : 0;
                        int xstop = x2 <= ColumnCount ? x2 : ColumnCount;

                        for (int x = xstart; x <= xstop; x++)
                        {
                            this.DrawPoint(
                                pen,
                                x,
                                (int)(y1 + 0.5f + slope * (x - x1))
                                );
                        }
                    }
                    else
                    {
                        int xstart = x2 >= 0 ? x2 : 0;
                        int xstop = x1 <= ColumnCount ? x1 : ColumnCount;

                        for (int x = xstart; x <= xstop; x++)
                        {
                            this.DrawPoint(
                                pen,
                                x,
                                (int)(y2 + 0.5f + slope * (x - x2))
                                );
                        }
                    }
                }

                //TODO: should be optimized
                //for (int j = 0; j < count; j++)
                //{
                //    this.DrawPoint(
                //        pen,
                //        (int)(j * xstep + x1 + 0.5f),
                //        (int)(j * ystep + y1 + 0.5f)
                //        );
                //}
            }

            this.ChangeHash();
        }

        #endregion


        #region Rectangle

        public override void DrawRectangle(
            Pen pen,
            Rectangle rect)
        {
            if ((pen.Color & OpaqueMask) == 0)
                return;

            //upper edge
            this.DrawLine(
                pen,
                rect.Left,
                rect.Top,
                rect.Right,
                rect.Top);

            //lower edge
            this.DrawLine(
                pen,
                rect.Left,
                rect.Bottom,
                rect.Right,
                rect.Bottom);

            //left edge
            this.DrawLine(
                pen,
                rect.Left,
                rect.Top,
                rect.Left,
                rect.Bottom);

            //right edge
            this.DrawLine(
                pen,
                rect.Right,
                rect.Top,
                rect.Right,
                rect.Bottom);

            this.ChangeHash();
        }


        public override void DrawRectangle(
            Pen pen,
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

            this.DrawRectangle(
                pen,
                rect);
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
                int[] blender = (color & Colors.Red) == Colors.Red
                    ? ((color & Colors.Lime) == Colors.Lime ? _blenderYellow : _blenderRed)
                    : ((color & Colors.Lime) == Colors.Lime ? _blenderGreen : _blenderBlack);

                int length = rt.Width + 1;
                for (int y = rt.Top, ym = rt.Bottom; y <= ym; y++)
                {
                    Array.Copy(
                        blender,
                        0,
                        this._cache,
                        rt.Left + y * ColumnCount,
                        length);
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

            Size fsize = font.BoxSize;
            int length = text.Length;

            if (text == null ||
                length == 0 ||
                x >= ColumnCount ||
                y >= RowCount ||
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
                    if (x >= ColumnCount)
                    {
                        break;
                    }

                    int code = (int)text[i];
                    int[] pattern = font.GetPattern(code);

                    for (int col = 0; col < fsize.Width; col++)
                    {
                        int xx = x + col;
                        if ((xx & ColumnMaskNot) != 0)
                            continue;

                        int currentPattern = pattern[col];
                        for (int row = 0; row < fsize.Height; row++)
                        {
                            int yy = y + row;
                            if ((yy & RowMaskNot) == 0 &&
                                (currentPattern & 1) != 0)
                            {
                                this._cache[xx + yy * ColumnCount] = color;
                            }

                            currentPattern >>= 1;
                        }
                    }
                }

                x += wt;
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
            else
            {
                Size fsize = font.BoxSize;
                int width = length * fsize.Width + (length - 1) * font.Spacing;

                return new Size(
                    width,
                    fsize.Height);
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
        /// <param name="red"></param>
        /// <param name="green"></param>
        internal void GetBuffers(
            byte[] red,
            byte[] green)
        {
            for (int col = 0; col < ColumnCount; col++)
            {
                //upper half of the matrix (32 cols by 8 rows)
                this.GetBufferColumn(
                    col,
                    ref red[col],
                    ref green[col]
                    );

                //lower half of the matrix (32 cols by 8 rows)
                this.GetBufferColumn(
                    col + 256,
                    ref red[col + ColumnCount],
                    ref green[col + ColumnCount]
                    );
            }
        }


        /// <summary>
        /// Conversion of a single section of the cache
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="red"></param>
        /// <param name="green"></param>
        private void GetBufferColumn(
            int offset,
            ref byte red,
            ref byte green)
        {
            //int row0 = this._cache[offset];
            //int row1 = this._cache[offset + ColumnCount];
            //int row2 = this._cache[offset + ColumnCount * 2];
            //int row3 = this._cache[offset + ColumnCount * 3];
            //int row4 = this._cache[offset + ColumnCount * 4];
            //int row5 = this._cache[offset + ColumnCount * 5];
            //int row6 = this._cache[offset + ColumnCount * 6];
            //int row7 = this._cache[offset + ColumnCount * 7];
            int row7 = this._cache[offset];
            int row6 = this._cache[offset + ColumnCount];
            int row5 = this._cache[offset + ColumnCount * 2];
            int row4 = this._cache[offset + ColumnCount * 3];
            int row3 = this._cache[offset + ColumnCount * 4];
            int row2 = this._cache[offset + ColumnCount * 5];
            int row1 = this._cache[offset + ColumnCount * 6];
            int row0 = this._cache[offset + ColumnCount * 7];

            int tempRed =
                ((row0 & Colors.Red) == Colors.Red ? 1 : 0) +
                ((row1 & Colors.Red) == Colors.Red ? 2 : 0) +
                ((row2 & Colors.Red) == Colors.Red ? 4 : 0) +
                ((row3 & Colors.Red) == Colors.Red ? 8 : 0) +
                ((row4 & Colors.Red) == Colors.Red ? 16 : 0) +
                ((row5 & Colors.Red) == Colors.Red ? 32 : 0) +
                ((row6 & Colors.Red) == Colors.Red ? 64 : 0) +
                ((row7 & Colors.Red) == Colors.Red ? 128 : 0);

            int tempGreen =
                ((row0 & Colors.Lime) == Colors.Lime ? 1 : 0) +
                ((row1 & Colors.Lime) == Colors.Lime ? 2 : 0) +
                ((row2 & Colors.Lime) == Colors.Lime ? 4 : 0) +
                ((row3 & Colors.Lime) == Colors.Lime ? 8 : 0) +
                ((row4 & Colors.Lime) == Colors.Lime ? 16 : 0) +
                ((row5 & Colors.Lime) == Colors.Lime ? 32 : 0) +
                ((row6 & Colors.Lime) == Colors.Lime ? 64 : 0) +
                ((row7 & Colors.Lime) == Colors.Lime ? 128 : 0);

            red = (byte)tempRed;
            green = (byte)tempGreen;
        }

        #endregion
    }
}
