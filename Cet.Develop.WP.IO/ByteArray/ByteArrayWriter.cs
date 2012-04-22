using System;
using System.Linq;
using System.Collections.Generic;

/*
 * Copyright 2012 Mario Vernari (http://cetdevelop.codeplex.com/)
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
namespace Cet.Develop.Windows.IO
{
    /// <summary>
    /// Wrapper around an ordinary byte array,
    /// which realizes a forward-only writer
    /// </summary>
    public class ByteArrayWriter
        : IEnumerable<byte>
    {
        /// <summary>
        /// Crea an empty instance
        /// </summary>
        public ByteArrayWriter()
        {
            this._buffer = new List<byte>();
            this._proxy = new byte[8];
        }



        /// <summary>
        /// Create an instance using the specified content,
        /// and sealing it as immutable
        /// </summary>
        /// <param name="initial"></param>
        public ByteArrayWriter(IEnumerable<byte> initial)
        {
            this._buffer = new List<byte>(initial);
        }



        private readonly List<byte> _buffer;
        private readonly byte[] _proxy;



        /// <summary>
        /// Indicate the current length of the written data
        /// </summary>
        public int Length
        {
            get { return this._buffer.Count; }
        }



        /// <summary>
        /// Provide a way to reset the writer
        /// </summary>
        /// <remarks>
        /// This functionaly is not allowed when the writer has been sealed
        /// </remarks>
        public void Reset()
        {
            this.CheckImmutable();
            this._buffer.Clear();
        }



        /// <summary>
        /// Create a <see cref="ByteArrayReader"/> using the current writer as source
        /// </summary>
        /// <returns></returns>
        public ByteArrayReader ToReader()
        {
            return new ByteArrayReader(this._buffer);
        }



        /// <summary>
        /// Add a byte to the writer
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>
        /// This functionality is not allowed when the writer has been sealed
        /// </remarks>
        public void WriteByte(byte value)
        {
            this.CheckImmutable();
            this._buffer.Add(value);
        }



        /// <summary>
        /// Add a series of bytes to the writer, from a byte array
        /// by specifying the starting index and the number involved
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>
        /// This functionality is not allowed when the writer has been sealed
        /// </remarks>
        public void WriteBytes(
            byte[] values,
            int offset,
            int count)
        {
            this.CheckImmutable();

            while (count-- > 0)
                this._buffer.Add(values[offset++]);
        }



        /// <summary>
        /// Add a series of bytes to the writer
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>
        /// This functionality is not allowed when the writer has been sealed
        /// </remarks>
        public void WriteBytes(IEnumerable<byte> values)
        {
            this.CheckImmutable();
            this._buffer.AddRange(values);
        }



        /// <summary>
        /// Add an <see cref="System.Int16"/> (Little-endian) to the writer
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>
        /// This functionality is not allowed when the writer has been sealed
        /// </remarks>
        public void WriteInt16LE(Int16 value)
        {
            this.CheckImmutable();
            ByteArrayHelpers.WriteInt16LE(
                this._proxy,
                0,
                value);

            this.WriteBytes(
                this._proxy,
                0,
                2);
        }



        /// <summary>
        /// Add an <see cref="System.Int16"/> (Big-endian) to the writer
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>
        /// This functionality is not allowed when the writer has been sealed
        /// </remarks>
        public void WriteInt16BE(Int16 value)
        {
            this.CheckImmutable();
            ByteArrayHelpers.WriteInt16BE(
                this._proxy,
                0,
                value);

            this.WriteBytes(
                this._proxy,
                0,
                2);
        }



        #region NON CLS-Compliant members

#pragma warning disable 3001, 3002, 0618

        /// <summary>
        /// Add an <see cref="System.UInt16"/> (Little-endian) to the writer
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>
        /// This functionality is not allowed when the writer has been sealed
        /// </remarks>
        public void WriteUInt16LE(UInt16 value)
        {
            this.CheckImmutable();
            ByteArrayHelpers.WriteUInt16LE(
                this._proxy,
                0,
                value);

            this.WriteBytes(
                this._proxy,
                0,
                2);
        }



        /// <summary>
        /// Add an <see cref="System.UInt16"/> (Big-endian) to the writer
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>
        /// This functionality is not allowed when the writer has been sealed
        /// </remarks>
        public void WriteUInt16BE(UInt16 value)
        {
            this.CheckImmutable();
            ByteArrayHelpers.WriteUInt16BE(
                this._proxy,
                0,
                value);

            this.WriteBytes(
                this._proxy,
                0,
                2);
        }

#pragma warning restore 3001, 3002, 0618

        #endregion



        /// <summary>
        /// Add an <see cref="System.Int32"/> (Little-endian) to the writer
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>
        /// This functionality is not allowed when the writer has been sealed
        /// </remarks>
        public void WriteInt32LE(Int32 value)
        {
            this.CheckImmutable();
            ByteArrayHelpers.WriteInt32LE(
                this._proxy,
                0,
                value);

            this.WriteBytes(
                this._proxy,
                0,
                4);
        }



        /// <summary>
        /// Add an <see cref="System.Int32"/> (Big-endian) to the writer
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>
        /// This functionality is not allowed when the writer has been sealed
        /// </remarks>
        public void WriteInt32BE(Int32 value)
        {
            this.CheckImmutable();
            ByteArrayHelpers.WriteInt32BE(
                this._proxy,
                0,
                value);

            this.WriteBytes(
                this._proxy,
                0,
                4);
        }



        private void CheckImmutable()
        {
            //if the instance has been created as immutable,
            //can't allow any modification
            if (this._proxy == null)
                throw new Exception();
        }



        #region IEnumerable members

        public IEnumerator<byte> GetEnumerator()
        {
            return this._buffer.GetEnumerator();
        }



        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this._buffer.GetEnumerator();
        }

        #endregion

    }
}
