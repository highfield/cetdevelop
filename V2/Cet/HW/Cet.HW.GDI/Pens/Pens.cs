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
    /// Pre-defined pens
    /// </summary>
    public static class Pens
    {
        public static readonly Pen Transparent = new Pen(Colors.Transparent);

        public static readonly Pen Black = new Pen(Colors.Black);
        public static readonly Pen Blue = new Pen(Colors.Blue);
        public static readonly Pen Lime = new Pen(Colors.Lime);
        public static readonly Pen Aqua = new Pen(Colors.Aqua);
        public static readonly Pen Red = new Pen(Colors.Red);
        public static readonly Pen Fuchsia = new Pen(Colors.Fuchsia);
        public static readonly Pen Yellow = new Pen(Colors.Yellow);
        public static readonly Pen White = new Pen(Colors.White);

    }
}
