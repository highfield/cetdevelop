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
    public class LcdBoostElementCharPoint
        : ILcdBoostElement
    {
        public char Face = '*';
        public int X;
        public int Y;

        public bool IsHidden { get; set; }



        #region ILcdBoostElement members

        void ILcdBoostElement.Render(
            LcdBoostVideoCache cache,
            LcdBoostRect container
            )
        {
            //gets the global cache point where the character
            //has to be written
            int xd = System.Math.Max(0, this.X) + container.Left;
            int yd = System.Math.Max(0, this.Y) + container.Top;

            /**
             * hard-copy the local cache onto the global
             **/

            var face = (int)this.Face;
            if (face != LcdBoostElementTextArea.Transparent)
            {
                //it assumes that the whole area is opaque
                //thus the copy is much simpler (and faster)
                if (container.Contains(xd, yd))
                {
                    cache.Data[yd][xd] = (byte)face;
                }
            }
        }

        #endregion
    }
}
