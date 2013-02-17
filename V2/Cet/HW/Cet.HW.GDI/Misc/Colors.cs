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
    public static class Colors
    {
        public const int Transparent = 0x00000000;
        public const int Opaque = unchecked((int)0xFF000000);

        //color constants from http://en.wikipedia.org/wiki/Web_colors
        public const int Black = 0x00000000 | Opaque;
        public const int Blue = 0x000000FF | Opaque;
        public const int Lime = 0x0000FF00 | Opaque;
        public const int Aqua = 0x0000FFFF | Opaque;
        public const int Red = 0x00FF0000 | Opaque;
        public const int Fuchsia = 0x00FF00FF | Opaque;
        public const int Yellow = 0x00FFFF00 | Opaque;
        public const int White = 0x00FFFFFF | Opaque;

    }
}
