using System;

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
namespace Cet.IO
{
    public static partial class ByteArrayHelpers
    {
        /// <summary>
        /// Create a copy of the given byte array
        /// </summary>
        /// <param name="source">The source byte-array</param>
        /// <returns>The resulting copy</returns>
        public static byte[] ArrayCopy(byte[] source)
        {
            int count = source.Length;
            var target = new byte[count];

            Array.Copy(
                source,
                target,
                count);

            return target;
        }


        /// <summary>
        /// Create a new byte array starting from the defined
        /// segment of the given one
        /// </summary>
        /// <param name="source">The source byte-array</param>
        /// <param name="offset">The index of the first byte to be considered</param>
        /// <param name="count">The number of the bytes to be considered</param>
        /// <returns>The resulting byte-array</returns>
        public static byte[] ArrayCopy(
            byte[] source,
            int offset,
            int count)
        {
            var target = new byte[count];

            Array.Copy(
                source,
                offset,
                target,
                0,
                count);

            return target;
        }

    }
}
