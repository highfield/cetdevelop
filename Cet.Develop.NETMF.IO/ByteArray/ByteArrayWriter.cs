using System;
using Microsoft.SPOT;
using System.Collections;

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

/**
 * 09/Apr/2012:
 *  Added the ToArray method.
 **/
namespace Cet.Develop.NETMF.IO
{
    /// <summary>
    /// Wrapper around an ordinary byte array,
    /// which realizes a forward-only writer
    /// </summary>
    public class ByteArrayWriter
        : IEnumerable, IByteArray
    {
        private const int ChunkSize = 0x100;



        /// <summary>
        /// Crea an empty instance
        /// </summary>
        public ByteArrayWriter()
        {
            this._buffer = new byte[ChunkSize];
            this._proxy = new byte[8];
        }



        /// <summary>
        /// Create an instance using the specified content,
        /// and sealing it as immutable
        /// </summary>
        /// <param name="initial"></param>
        public ByteArrayWriter(byte[] initial)
        {
            this._length = initial.Length;
            this._buffer = new byte[this._length];
            Array.Copy(
                initial,
                this._buffer,
                this._length);
        }



        private byte[] _buffer;
        private int _length;
        private readonly byte[] _proxy;



        /// <summary>
        /// Indicate the current length of the written data
        /// </summary>
        public int Length
        {
            get { return this._length; }
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
            this._length = 0;
        }



        /// <summary>
        /// Create a <see cref="ByteArrayReader"/> using the current writer as source
        /// </summary>
        /// <returns></returns>
        public ByteArrayReader ToReader()
        {
            return new ByteArrayReader(((IByteArray)this).Data);
        }



        /// <summary>
        /// Create a byte-array using the current writer as source
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            return ((IByteArray)this).Data;
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
            this.Allocate(1);
            this._buffer[this._length++] = value;
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
            this.Allocate(count);

            Array.Copy(
                values,
                offset,
                this._buffer,
                this._length,
                count);

            this._length += count;
        }



        /// <summary>
        /// Add a series of bytes to the writer
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>
        /// This functionality is not allowed when the writer has been sealed
        /// </remarks>
        public void WriteBytes(byte[] values)
        {
            this.WriteBytes(
                values,
                0,
                values.Length);
        }



        /// <summary>
        /// Add the content of the given reader to the writer
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>
        /// This functionality is not allowed when the writer has been sealed
        /// </remarks>
        public void WriteBytes(ByteArrayReader reader)
        {
            this.WriteBytes(((IByteArray)reader).Data);
        }



        /// <summary>
        /// Add the content of another writer to the writer
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>
        /// This functionality is not allowed when the writer has been sealed
        /// </remarks>
        public void WriteBytes(ByteArrayWriter writer)
        {
            this.WriteBytes(((IByteArray)writer).Data);
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



        /// <summary>
        /// Allocate the specified number of bytes in the underlying
        /// buffer. Get the buffer longer when needed
        /// </summary>
        /// <param name="count"></param>
        private void Allocate(int count)
        {
            this.CheckImmutable();

            var size = this._buffer.Length;
            var newLen = this._length + count;
            if (newLen < size)
                return;

            do
            {
                size += ChunkSize;
            } while (size < newLen);

            var temp = new byte[size];

            Array.Copy(
                this._buffer,
                temp,
                this._buffer.Length);

            this._buffer = temp;
        }



        private void CheckImmutable()
        {
            //if the instance has been created as immutable,
            //can't allow any modification
            if (this._proxy == null)
                throw new Exception();
        }



        #region IEnumerable members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < this._length; i++)
                yield return this._buffer[i];
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
            get 
            {
                var temp = new byte[this._length];
                Array.Copy(
                    this._buffer,
                    temp,
                    this._length);

                return temp; 
            }
        }
    }
}
