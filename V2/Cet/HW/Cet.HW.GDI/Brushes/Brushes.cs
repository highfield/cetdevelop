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
    /// Pre-defined brushes
    /// </summary>
    public static class Brushes
    {
        public static readonly Brush Transparent = new SolidBrush(Colors.Transparent);

        public static readonly Brush Black = new SolidBrush(Colors.Black);
        public static readonly Brush Blue = new SolidBrush(Colors.Blue);
        public static readonly Brush Lime = new SolidBrush(Colors.Lime);
        public static readonly Brush Aqua = new SolidBrush(Colors.Aqua);
        public static readonly Brush Red = new SolidBrush(Colors.Red);
        public static readonly Brush Fuchsia = new SolidBrush(Colors.Fuchsia);
        public static readonly Brush Yellow = new SolidBrush(Colors.Yellow);
        public static readonly Brush White = new SolidBrush(Colors.White);

    }
}
