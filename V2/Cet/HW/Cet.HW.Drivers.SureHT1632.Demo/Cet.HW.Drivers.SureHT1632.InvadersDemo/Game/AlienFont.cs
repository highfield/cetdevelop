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
namespace Cet.HW.Drivers.Invaders
{
    public class AlienFont
        : Font
    {
        private const int CharacterWidth = 8;
        private const int CharacterHeight = 8;


        private static readonly byte[] _charset = new byte[]
            {
                0x00, 0x00, 0xF0, 0xF8, 0xF8, 0xF8, 0xFE, 0xFF,     // J: ship (left half)
                0xFE, 0xF8, 0xF8, 0xF8, 0xF0, 0x00, 0x00, 0x00,     // K: ship (right half)
                0x00, 0x00, 0x70, 0xE8, 0x7C, 0x3E, 0x2F, 0x7F,     // L: saucer (left half)
                0x7F, 0x2F, 0x3E, 0x7C, 0xE8, 0x70, 0x00, 0x00,     // M: saucer (right half)
                0x7F, 0x04, 0x08, 0x10, 0x7F, 0x00, 0x00, 0x00,     // N: 'N'
                0x7F, 0x10, 0x08, 0x04, 0x7F, 0x00, 0x00, 0x00,     // O: 'N' (mirrored)
                0x18, 0x5C, 0xB6, 0x1F, 0x1F, 0xB6, 0x5C, 0x18,     // P: alien #1 (step A)
                0x98, 0x5C, 0xB6, 0x5F, 0x5F, 0xB6, 0x5C, 0x98,     // Q: alien #1 (step B)
                0x00, 0x00, 0x70, 0x18, 0x7D, 0xB6, 0xBC, 0x3C,     // R: alien #2 (step A, left half)
                0xBC, 0xB6, 0x7D, 0x18, 0x70, 0x00, 0x00, 0x00,     // S: alien #2 (step A, right half)
                0x00, 0x00, 0x1E, 0xB8, 0x7D, 0x36, 0x3C, 0x3C,     // T: alien #2 (step B, left half)
                0x3C, 0x36, 0x7D, 0xB8, 0x1E, 0x00, 0x00, 0x00,     // U: alien #2 (step B, right half)
                0x00, 0x00, 0x1C, 0x1E, 0x5E, 0xB6, 0x37, 0x5F,     // V: alien #3 (step A, left half)
                0x5F, 0x37, 0xB6, 0x5E, 0x1E, 0x1C, 0x00, 0x00,     // W: alien #3 (step A, right half)
                0x00, 0x00, 0x9C, 0x9E, 0x5E, 0x76, 0x37, 0x5F,     // X: alien #3 (step B, left half)
                0x5F, 0x37, 0x76, 0x5E, 0x9E, 0x9C, 0x00, 0x00,     // Y: alien #3 (step B, right half)
            };


        #region Singleton pattern

        private AlienFont() 
        {
            this.Spacing = 0;
        }


        public static readonly AlienFont Instance = new AlienFont();

        #endregion


        public override Size BoxSize
        {
            get { return new Size(CharacterWidth, CharacterHeight); }
        }


        public override int[] GetPattern(int code)
        {
            //seek the required pattern
            var offset = (code - (int)'J') * CharacterWidth;

            return new int[CharacterWidth]
                {
                    _charset[offset],
                    _charset[offset + 1],
                    _charset[offset + 2],
                    _charset[offset + 3],
                    _charset[offset + 4],
                    _charset[offset + 5],
                    _charset[offset + 6],
                    _charset[offset + 7],
                };
        }

    }
}
