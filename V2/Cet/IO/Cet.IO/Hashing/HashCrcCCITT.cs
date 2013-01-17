
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
    /// Hashing algorithm for calculating the CRC-CCITT (0x1021)
    /// </summary>
    public class HashCrcCCITT
        : HashAlgorithmBase
    {
        /// <summary>
        /// Create an istance for the CRC-CCITT calculation
        /// </summary>
        /// <param name="seed">The initial value to be used for the calculation</param>
        public HashCrcCCITT(ushort seed)
        {
            this._seed = seed;
        }


        private ushort _seed;


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
            ushort crc = this._seed;

            for (int i = 0; i < count; i++)
            {
                int temp = ((crc >> 8) ^ buffer[offset + i]) | (crc << 8);
                temp ^= (byte)temp >> 4;
                temp ^= temp << 12;
                crc = (ushort)(temp ^ (byte)temp << 5);
            }

            return (int)crc;
        }

    }
}
