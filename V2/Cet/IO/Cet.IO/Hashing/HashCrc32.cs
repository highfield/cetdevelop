
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
namespace Cet.IO.Hashing
{
    /// <summary>
    /// Hashing algorithm for calculating the CRC-32
    /// </summary>
    public class HashCrc32
        : HashAlgorithmBase
    {
        private const uint Polynom = 0xEDB88320;
        private const uint Seed = 0xFFFFFFFF;


        static HashCrc32()
        {
            //create the lookup table for faster calculation
            for (uint i = 0; i < 0x100; i++)
            {
                uint crc = i;

                for (int k = 7; k >= 0; k--)
                {
                    if ((crc & 1) != 0)
                    {
                        crc = (crc >> 1) ^ Polynom;
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }

                _table[i] = crc;
            }
        }


        private static uint[] _table = new uint[0x100];


        /// <summary>
        /// Calculate the hashing over a segment of a byte array
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int Compute(
            byte[] buffer, 
            int offset, 
            int count)
        {
            uint crc = Seed;

            for (int i = 0; i < count; i++)
            {
                var temp = (byte)(buffer[offset + i] ^ crc);
                crc = (crc >> 8) ^ _table[temp];
            }

            return (int)(crc ^ 0xFFFFFFFF);
        }

    }
}
