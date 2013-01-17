using System;
using System.Collections.Generic;

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
    /// <summary>
    /// Wrapper around an ordinary byte array,
    /// which realizes a forward-only writer
    /// </summary>
    public partial class ByteArrayWriter
        : IEnumerable<byte>, IByteArray
    {

        /// <summary>
        /// Add a <see cref="System.Single"/> (Little-endian) to the writer
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>
        /// This functionality is not allowed when the writer has been sealed
        /// </remarks>
        public void WriteSingleLE(Single value)
        {
            this.CheckImmutable();
            ByteArrayHelpers.WriteSingleLE(
                this._proxy,
                0,
                value);

            this.WriteBytes(
                this._proxy,
                0,
                4);
        }


        /// <summary>
        /// Add a <see cref="System.Single"/> (Big-endian) to the writer
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>
        /// This functionality is not allowed when the writer has been sealed
        /// </remarks>
        public void WriteSingleBE(Single value)
        {
            this.CheckImmutable();
            ByteArrayHelpers.WriteSingleBE(
                this._proxy,
                0,
                value);

            this.WriteBytes(
                this._proxy,
                0,
                4);
        }


        /// <summary>
        /// Add a <see cref="System.Double"/> (Little-endian) to the writer
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>
        /// This functionality is not allowed when the writer has been sealed
        /// </remarks>
        public void WriteDoubleLE(Double value)
        {
            this.CheckImmutable();
            ByteArrayHelpers.WriteDoubleLE(
                this._proxy,
                0,
                value);

            this.WriteBytes(
                this._proxy,
                0,
                8);
        }


        /// <summary>
        /// Add a <see cref="System.Double"/> (Big-endian) to the writer
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>
        /// This functionality is not allowed when the writer has been sealed
        /// </remarks>
        public void WriteDoubleBE(Double value)
        {
            this.CheckImmutable();
            ByteArrayHelpers.WriteDoubleBE(
                this._proxy,
                0,
                value);

            this.WriteBytes(
                this._proxy,
                0,
                8);
        }


        #region IEnumerable members

        public IEnumerator<byte> GetEnumerator()
        {
            for (int i = 0; i < this._length; i++)
                yield return this._buffer[i];
        }


        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<byte>)this)
                .GetEnumerator();
        }

        #endregion


        /// <summary>
        /// Facility for data exchange
        /// </summary>
        /// <remarks>
        /// Avoid to use this member unless strictly necessary
        /// </remarks>
        byte[] IByteArray.Data
        {
            get { return this._buffer; }
        }

    }
}
