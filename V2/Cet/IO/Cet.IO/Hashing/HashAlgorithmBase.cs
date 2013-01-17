
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
    /// Provide an abstract base for most generic hashing algorithms
    /// </summary>
    public abstract class HashAlgorithmBase
    {
        protected HashAlgorithmBase() { }


        /// <summary>
        /// Calculate the hashing over an entire byte array
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public int Compute(byte[] buffer)
        {
            return this.Compute(
                buffer,
                0,
                buffer.Length);
        }
        

        /// <summary>
        /// Calculate the hashing over a segment of a byte array
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public abstract int Compute(
            byte[] buffer,
            int offset,
            int count);

    }
}
