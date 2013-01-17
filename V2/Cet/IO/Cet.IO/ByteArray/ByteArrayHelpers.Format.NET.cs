using System;
using System.Collections.Generic;
using System.Linq;

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
        /// Convert a byte-array to string containing its HEX-digit representation
        /// formatted as it were a "memory-dump"
        /// </summary>
        /// <param name="source">The byte-array to be converted.</param>
        /// <param name="address">The equivalent "address" from where the dump should start</param>
        /// <returns>The resulting string containing the HEX representation</returns>
        /// <remarks>
        /// <para>This way to format the byte-array is particularly useful for memory-dumps.</para>
        /// <para>The address is treated always as a 32-bit value, thus always 8-digits zeroed-padded.
        /// When the address value overflows, then rolls back to zero.</para>
        /// </remarks>
        /// <example>Here is an example:
        ///     <code>
        ///     //create a sequence of 40 bytes (shortened for ease of representation)
        ///     byte[] buffer = new byte[40] { 0, 1, 2, ... , 38, 39 };
        /// 
        ///     //convert the sequence, then display it
        ///     string s = ByteArray.DumpHex(0x134);
        ///     Console.WriteLine(s);
        ///
        ///     </code>
        ///     Yields:
        ///     <code>
        ///     00000130:             00 01 02 03-04 05 06 07 08 09 0A 0B
        ///     00000140: 0C 0D 0E 0F 10 11 12 13-14 15 16 17 18 19 1A 1B
        ///     00000150: 1C 1D 1E 1F 20 21 22 23-24 25 26 27
        ///     </code>
        /// </example>
        public static string DumpHex(
            this IEnumerable<byte> source,
            int address)
        {
            //convert the new-line seuqnce in a character-array
            char[] crlf = Environment.NewLine.ToCharArray();
            int crlfLen = crlf.Length;

            //count the involved characters
            var buffer = source.ToArray();
            int count = buffer.Length;

            //estimate (in excess) the allocation size for the resulting string
            int rows = 2 + count / 16;
            int total = rows * (8 + 1 + 3 * 16 + crlfLen);

            //favor operating on a char-array, for a better performance
            char[] ca = new char[total];
            int len = 0;

            //define the enumerator, and some flags
            IEnumerator<byte> buffEnum = source.GetEnumerator();
            bool eof = false;
            bool started = false;


            //define an handler for converting a value in a HEX-digits pair
            Action<byte> pushByte = (value) =>
            {
                ca[len] = _tbltohex[unchecked(value >> 4)];
                ca[len + 1] = _tbltohex[unchecked(value & 0x0F)];
                len += 2;
            };


            //define an handler for converting the address in a HEX-digits octet
            Action<uint> pushAddress = (addr) =>
            {
                pushByte(unchecked((byte)(addr >> 24)));
                pushByte(unchecked((byte)(addr >> 16)));
                pushByte(unchecked((byte)(addr >> 8)));
                pushByte(unchecked((byte)(addr)));
                ca[len] = ':';
                len++;
            };


            //cycle creating rows, until all the data is over
            var ptr = unchecked((uint)address);

            do
            {
                //calculate the "offset" address
                uint offset = ptr & (uint)0xFFFFFFF0;

                //the row begins with the address
                pushAddress(offset);

                //calculate how many "blanks" have to be inserted before the data bytes
                int numBlanks;

                if (started)
                {
                    //after the first row, there's no more need of blanks
                    numBlanks = 0;
                }
                else
                {
                    //the blanks are used only in the first row
                    numBlanks = unchecked((int)(address & 0x0F));

                    for (int i = 0; i < numBlanks; i++)
                    {
                        ca[len] = (i == 8) ? '-' : ' ';
                        ca[len + 1] = ' ';
                        ca[len + 2] = ' ';
                        len += 3;
                    }

                    started = true;
                }

                //add the data digits, up to the end of row
                for (int i = numBlanks; i < 0x10; i++)
                {
                    if (buffEnum.MoveNext() == false)
                    {
                        eof = true;
                        break;
                    }

                    //valid byte
                    ca[len] = (i == 8) ? '-' : ' ';
                    len++;
                    pushByte(buffEnum.Current);
                }

                //switch to a new line
                for (int i = 0; i < crlfLen; i++)
                    ca[len + i] = crlf[i];

                len += crlfLen;

                //update the offset
                unchecked
                {
                    ptr = offset + 0x10;
                }
            } while (eof == false);

            //adjust the char-array size
            Array.Resize(ref ca, len);

            //return the resulting string
            return new string(ca);
        }

    }
}
