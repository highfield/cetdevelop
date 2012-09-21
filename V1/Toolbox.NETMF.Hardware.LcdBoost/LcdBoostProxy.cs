//#define CRONO

using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

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
    /// <summary>
    /// Logical proxy for a generic LCD driver.
    /// </summary>
    public sealed class LcdBoostProxy
    {
        /// <summary>
        /// Create a new instance of the class
        /// </summary>
        /// <param name="driver">Specifies the physical driver to be used. Can't be null</param>
        public LcdBoostProxy(ILcdBoostDriver driver)
        {
            this.Driver = driver;

            //driver initialization
            driver.Init();

            //create the cache host
            this._cache = new LcdBoostVideoCache(
                driver.Width,
                driver.Height
                );

            this._bounds.Width = driver.Width;
            this._bounds.Height = driver.Height;
        }



        private readonly LcdBoostVideoCache _cache;
        private readonly LcdBoostRect _bounds = new LcdBoostRect();

#if CRONO
        private OutputPort _test0 = new OutputPort(Pins.GPIO_PIN_D0, false);
        private OutputPort _test1 = new OutputPort(Pins.GPIO_PIN_D1, false);
#endif


        /// <summary>
        /// Gets the used driver reference
        /// </summary>
        public ILcdBoostDriver Driver { get; private set; }



        /// <summary>
        /// Refresh the LCD display
        /// </summary>
        /// <remarks>
        /// Typically, this method has to be called at a certain interval</remarks>
        public void Dump()
        {
            this.Driver
                .Dump(this._cache);
        }



        public void Dump(ILcdBoostElement[] elements)
        {
#if CRONO
            this._test0.Write(true);
#endif
            this._cache.Clear();
#if CRONO
            this._test1.Write(true);
#endif

            for (int i = 0, count = elements.Length; i < count; i++)
            {
                var elem = elements[i];
                if (elem.IsHidden)
                    continue;

                elem.Render(
                    this._cache,
                    this._bounds
                    );
            }

#if CRONO
            this._test1.Write(false);
#endif

            this.Driver
                .Dump(this._cache);

#if CRONO
            this._test0.Write(false);
#endif
        }



        /// <summary>
        /// Clear the entire screen
        /// </summary>
        public void Clear()
        {
            this._cache.Clear();
        }



        private int _cursorX;
        private int _cursorY;



        /// <summary>
        /// Set the starting (reference) position
        /// to be used by any further text operation
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetCursorPosition(
            int x,
            int y
            )
        {
            //adjust the x-coord
            if (x < 0)
                x = 0;
            else if (x >= this.Driver.Width)
                x = this.Driver.Width - 1;

            //adjust the y-coord
            if (y < 1)
                y = 0;
            else if (y >= this.Driver.Height)
                y = this.Driver.Height - 1;

            this._cursorX = x;
            this._cursorY = y;
        }



        /// <summary>
        /// Write a text string (as plain ASCII) on the video cache
        /// </summary>
        /// <param name="text">The string to be written</param>
        /// <remarks>
        /// The text starting reference has to be set by using the <see cref="SetCursorPosition"/>.
        /// After this call, the logical position is moved accordingly
        /// </remarks>
        public void Write(string text)
        {
            if (text == null)
                return;

            byte[] vrow = this._cache.Data[this._cursorY];
            int residue = this._cache.Width - this._cursorX;

            for (int i = 0, count = text.Length; i < count; i++)
            {
                vrow[this._cursorX] = (byte)(int)text[i];

                if (--residue == 0)
                    break;
                else
                    this._cursorX++;
            }
        }


    }
}
