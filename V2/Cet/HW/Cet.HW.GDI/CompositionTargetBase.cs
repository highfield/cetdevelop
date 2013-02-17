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
    /// Provides an abstraction of the rendering target device
    /// </summary>
    public abstract class CompositionTargetBase
    {
        protected CompositionTargetBase() { }


        /// <summary>
        /// Gets the size of the physical viewport
        /// </summary>
        public abstract Size ViewportSize { get; }


        /// <summary>
        /// Clears the entire composition target
        /// </summary>
        public abstract void Clear();


        #region Pixel

        /// <summary>
        /// Draws a pixel at the specified coordinates and using the specified color
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="color"></param>
        public abstract void DrawPixel(
            Point pt, 
            int color);

        
        /// <summary>
        /// Draws a pixel at the specified coordinates and using the specified color
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        public abstract void DrawPixel(
            int x, 
            int y, 
            int color);

        #endregion


        #region Line

        /// <summary>
        /// Draws a straight line between two points, using the specified pen
        /// </summary>
        /// <param name="pen"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public abstract void DrawLine(
            Pen pen,
            Point a, 
            Point b);


        /// <summary>
        /// Draws a straight line between two points, using the specified pen
        /// </summary>
        /// <param name="pen"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public abstract void DrawLine(
            Pen pen,
            int x1, 
            int y1, 
            int x2, 
            int y2);

        #endregion


        #region Rectangle

        /// <summary>
        /// Draws a rectangle figure, using the specified pen
        /// </summary>
        /// <param name="pen"></param>
        /// <param name="rect"></param>
        public abstract void DrawRectangle(
            Pen pen,
            Rectangle rect);


        /// <summary>
        /// Draws a rectangle figure, using the specified pen
        /// </summary>
        /// <param name="pen"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public abstract void DrawRectangle(
            Pen pen,
            int x, 
            int y, 
            int width, 
            int height);


        /// <summary>
        /// Fills a rectangle figure, using the specified brush
        /// </summary>
        /// <param name="brush"></param>
        /// <param name="rect"></param>
        public abstract void FillRectangle(
            Brush brush,
            Rectangle rect);


        /// <summary>
        /// Fills a rectangle figure, using the specified brush
        /// </summary>
        /// <param name="brush"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public abstract void FillRectangle(
            Brush brush,
            int x,
            int y,
            int width,
            int height);

        #endregion


        #region Ellipse

        /// <summary>
        /// Draws an ellipse figure, using the specified pen
        /// </summary>
        /// <param name="pen"></param>
        /// <param name="rect"></param>
        public abstract void DrawEllipse(
            Pen pen,
            Rectangle rect);


        /// <summary>
        /// Draws an ellipse figure, using the specified pen
        /// </summary>
        /// <param name="pen"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public abstract void DrawEllipse(
            Pen pen,
            int x,
            int y,
            int width,
            int height);


        /// <summary>
        /// Fills an ellipse figure, using the specified brush
        /// </summary>
        /// <param name="brush"></param>
        /// <param name="rect"></param>
        public abstract void FillEllipse(
            Brush brush,
            Rectangle rect);


        /// <summary>
        /// Fills an ellipse figure, using the specified brush
        /// </summary>
        /// <param name="brush"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public abstract void FillEllipse(
            Brush brush,
            int x,
            int y,
            int width,
            int height);

        #endregion


        #region Text

        /// <summary>
        /// Draws the specified text string at the specified location with the specified Brush.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="brush"></param>
        /// <param name="pt"></param>
        public abstract void DrawString(
            string text,
            Font font,
            Brush brush,
            Point pt);


        /// <summary>
        /// Draws the specified text string at the specified location with the specified Brush.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="brush"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public abstract void DrawString(
            string text,
            Font font,
            Brush brush,
            int x, 
            int y);


        /// <summary>
        /// Measures the specified string when drawn
        /// </summary>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <returns>The size of the text</returns>
        public abstract Size MeasureString(
            string text,
            Font font);

        #endregion


        #region Sprite

        /// <summary>
        /// Draws a sprite, beginning at the specified coordinates
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="pt"></param>
        public abstract void DrawSprite(
            Sprite sprite,
            Point pt);


        /// <summary>
        /// Draws a sprite, beginning at the specified coordinates
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public abstract void DrawSprite(
            Sprite sprite, 
            int x, 
            int y);

        #endregion


        #region Cache hashing

        private int _hash;


        /// <summary>
        /// Changes the local hash so that the host may be notified
        /// </summary>
        protected void ChangeHash()
        {
            this._hash++;
        }


        /// <summary>
        /// Gets the actual hash for this instance
        /// </summary>
        /// <returns></returns>
        public int GetCacheHash()
        {
            return this._hash ^ this.GetHashCode();
        }

        #endregion

    }
}
