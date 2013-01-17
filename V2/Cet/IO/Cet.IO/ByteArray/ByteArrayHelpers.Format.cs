using System;

#if NET45
using System.Linq;
#endif

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
        private static byte[] _tblfromhex;
        private static readonly char[] _tbltohex = "0123456789ABCDEF".ToCharArray();



        private static void InitFromHexConverter()
        {
            //init tabella convertitore HEX --> byte-array
            _tblfromhex = new byte[0x100];

            for (int i = 0x30; i <= 0x39; i++)
                _tblfromhex[i] = unchecked((byte)(i - 0x30 + 1));

            for (int i = 0x41; i <= 0x46; i++)
                _tblfromhex[i] = unchecked((byte)(i - 0x41 + 11));

            for (int i = 0x61; i <= 0x66; i++)
                _tblfromhex[i] = unchecked((byte)(i - 0x61 + 11));
        }



        /// <summary>
        /// Parse a string containing an HEX representation to a byte-array
        /// </summary>
        /// <param name="source">The source string containing HEX characters.</param>
        /// <returns>The resulting byte-array</returns>
        /// <remarks>
        ///     <para>The conversion does not care about character casing.</para>
        ///     <para>Any character off the HEX-set is treated as separator.</para>
        ///     <para>The HEX-chars can be even without any separator (i.e. packed), 
        ///     however the conversion consider character pairs.</para>
        /// </remarks>
        /// <example>Sample behavior of the conversion:
        ///     <code>
        ///     byte[] b = ByteArrayHelpers.FromHex("0 zz@ q1 23 04 567 8! 9 abcdef");
        /// 
        ///     for (int i = 0; i &lt; b.Length; i++)
        ///     {
        ///         Console.Write(b[i].ToString() + " ");
        ///     }
        ///     Console.WriteLine();
        ///
        ///     </code>
        ///     Yields the result:
        ///     <code>
        ///     0 1 35 4 86 7 8 9 171 205 239
        ///     </code>
        /// </example>
        /// <seealso cref="ToHex"/>
        public static byte[] FromHex(string source)
        {
            if (_tblfromhex == null)
                InitFromHexConverter();

            //create a string reader to consume input chars
            var reader = new SimpleStringReader(source);

            //create a writer to accumulate the resulting bytes
            var writer = new ByteArrayWriter();

            //consume characters until the end of string
            int nibble = 0;
            int digit = 0;
            int code;
            while ((code = reader.GetNext()) >= 0)
            {
                if (code > 0)
                {
                    digit = (digit << 8) | (code - 1);
                    nibble++;
                }
                else if (nibble > 0)
                {
                    nibble = 2;
                }

                if (nibble == 2)
                {
                    writer.WriteByte((byte)digit);
                    digit = 0;
                    nibble = 0;
                }
            }

            return writer.ToByteArray();
        }


        /// <summary>
        /// Simple string read tailored for the <see cref="FromHex"/> function
        /// </summary>
        private class SimpleStringReader
        {
            public SimpleStringReader(string text)
            {
                this._buffer = text.ToCharArray();
                this._count = this._buffer.Length;
            }


            private readonly char[] _buffer;
            private readonly int _count;
            private int _position = -1;


            public int GetNext()
            {
                if (++this._position >= this._count)
                {
                    return -1;  //end of buffer
                }
                else
                {
                    //return the current character
                    return _tblfromhex[(int)this._buffer[this._position]];
                }
            }

        }


        /// <summary>
        /// Convert a byte-array to string containing its HEX-digit representation
        /// </summary>
        /// <param name="source">The byte-array to be converted.</param>
        /// <param name="separator">
        /// The optional characted to be used as separator. 
        /// Use (char)0 for yielding a packed (no separator) resulting string.
        /// </param>
        /// <returns>The resulting string containing the HEX representation</returns>
        /// <remarks>
        ///     <para>Each byte is represented as a character pair.</para>
        ///     <para>Whereas specified, pairs are separated by a custom character.</para>
        /// </remarks>
        /// <example>Sample behavior of the conversion:
        ///     <code>
        ///     byte[] b = { 12, 0, 34, 5, 67, 0, 89, 128, 192, 255 };
        /// 
        ///     string s = ByteArraHelpers.ToHex(b);
        ///
        ///     Console.WriteLine(s);
        ///
        ///     </code>
        ///     Yields the result:
        ///     <code>
        ///     0C 00 22 05 43 00 59 80 C0 FF
        ///     </code>
        /// </example>
        /// <seealso cref="FromHex"/>
        public static string ToHex(
            byte[] source,
            char separator = ' ')
        {
            //get the number of bytes involved
            int sourceCount = source.Length;

            if (sourceCount == 0)
            {
                //no data to convert, however returns an empty string
                return string.Empty;
            }
            else
            {
                //create a char-array to fit exactly the resulting string
                int targetCount = (int)separator != 0
                    ? (sourceCount * 3 - 1)
                    : (sourceCount * 2);

                var target = new char[targetCount];

                //convert the first byte
                byte value = source[0];
                target[0] = _tbltohex[unchecked(value >> 4)];
                target[1] = _tbltohex[unchecked(value & 0x0F)];
                int currentLength = 2;

                //convert the remaining buffer
                if ((int)separator != 0)
                {
                    //insert the separator after each hex-digits pair
                    for (int index = 1; index < sourceCount; index++)
                    {
                        value = source[index];
                        target[currentLength] = separator;
                        target[currentLength + 1] = _tbltohex[unchecked(value >> 4)];
                        target[currentLength + 2] = _tbltohex[unchecked(value & 0x0F)];
                        currentLength += 3;
                    }
                }
                else
                {
                    //no separation between digits: the result will be packed
                    for (int index = 1; index < sourceCount; index++)
                    {
                        value = source[index];
                        target[currentLength] = _tbltohex[unchecked(value >> 4)];
                        target[currentLength + 1] = _tbltohex[unchecked(value & 0x0F)];
                        currentLength += 2;
                    }
                }

                return new String(target);
            }
        }

    }
}
