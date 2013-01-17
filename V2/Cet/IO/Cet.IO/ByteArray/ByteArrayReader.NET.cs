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
    public partial class ByteArrayReader
        : IEnumerable<byte>, IByteArray
    {
        /// <summary>
        /// Read the a number of bytes from the current pointer,
        /// but without actually moving it
        /// </summary>
        /// <param name="count">The number of bytes requested</param>
        /// <returns></returns>
        public byte[] Peek(int count)
        {
            var result = new byte[count];

            Buffer.BlockCopy(
                this._buffer,
                this.Position,
                result,
                0,
                count);

            return result;
        }


        /// <summary>
        /// Read a <see cref="System.Single"/> (Little-endian),
        /// and move the pointer accordingly
        /// </summary>
        /// <returns></returns>
        public Single ReadSingleLE()
        {
            int ptr = this.Position + 1;
            this.Position += 4;
            return ByteArrayHelpers.ReadSingleLE(
                this._buffer,
                ptr);
        }


        /// <summary>
        /// Read a <see cref="System.Single"/> (Big-endian),
        /// and move the pointer accordingly
        /// </summary>
        /// <returns></returns>
        public Single ReadSingleBE()
        {
            int ptr = this.Position + 1;
            this.Position += 4;
            return ByteArrayHelpers.ReadSingleBE(
                this._buffer,
                ptr);
        }


        /// <summary>
        /// Read a <see cref="System.Double"/> (Little-endian),
        /// and move the pointer accordingly
        /// </summary>
        /// <returns></returns>
        public Double ReadDoubleLE()
        {
            int ptr = this.Position + 1;
            this.Position += 8;
            return ByteArrayHelpers.ReadDoubleLE(
                this._buffer,
                ptr);
        }


        /// <summary>
        /// Read a <see cref="System.Double"/> (Big-endian),
        /// and move the pointer accordingly
        /// </summary>
        /// <returns></returns>
        public Double ReadDoubleBE()
        {
            int ptr = this.Position + 1;
            this.Position += 8;
            return ByteArrayHelpers.ReadDoubleBE(
                this._buffer,
                ptr);
        }


        #region Safe-readers

        /// <summary>
        /// Try to read a <see cref="System.Int16"/> (Little-endian),
        /// </summary>
        /// <param name="value"></param>
        /// <returns>True if succeeded, false otherwise</returns>
        public bool TryReadInt16LE(out Int16 value)
        {
            if (this.CanRead(2))
            {
                value = this.ReadInt16LE();
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }


        /// <summary>
        /// Try to read a <see cref="System.Int16"/> (Big-endian),
        /// </summary>
        /// <param name="value"></param>
        /// <returns>True if succeeded, false otherwise</returns>
        public bool TryReadInt16BE(out Int16 value)
        {
            if (this.CanRead(2))
            {
                value = this.ReadInt16BE();
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }


        /// <summary>
        /// Try to read a fixed-point <see cref="System.Single"/> (Little-endian),
        /// </summary>
        /// <param name="value"></param>
        /// <returns>True if succeeded, false otherwise</returns>
        public bool TryRead1912LE(out Single value)
        {
            if (this.CanRead(4))
            {
                value = this.Read1912LE();
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }


        /// <summary>
        /// Try to read a fixed-point <see cref="System.Single"/> (Big-endian),
        /// </summary>
        /// <param name="value"></param>
        /// <returns>True if succeeded, false otherwise</returns>
        public bool TryRead1912BE(out Single value)
        {
            if (this.CanRead(4))
            {
                value = this.Read1912BE();
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }


        /// <summary>
        /// Try to read a <see cref="System.Int32"/> (Little-endian),
        /// </summary>
        /// <param name="value"></param>
        /// <returns>True if succeeded, false otherwise</returns>
        public bool TryReadInt32LE(out Int32 value)
        {
            if (this.CanRead(4))
            {
                value = this.ReadInt32LE();
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }


        /// <summary>
        /// Try to read a <see cref="System.Int32"/> (Big-endian),
        /// </summary>
        /// <param name="value"></param>
        /// <returns>True if succeeded, false otherwise</returns>
        public bool TryReadInt32BE(out Int32 value)
        {
            if (this.CanRead(4))
            {
                value = this.ReadInt32BE();
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }


        /// <summary>
        /// Try to read a <see cref="System.Single"/> (Little-endian),
        /// </summary>
        /// <param name="value"></param>
        /// <returns>True if succeeded, false otherwise</returns>
        public bool TryReadSingleLE(out Single value)
        {
            if (this.CanRead(4))
            {
                value = this.ReadSingleLE();
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }


        /// <summary>
        /// Try to read a <see cref="System.Single"/> (Big-endian),
        /// </summary>
        /// <param name="value"></param>
        /// <returns>True if succeeded, false otherwise</returns>
        public bool TryReadSingleBE(out Single value)
        {
            if (this.CanRead(4))
            {
                value = this.ReadSingleBE();
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }


        /// <summary>
        /// Try to read a fixed-point <see cref="System.Single"/> (Little-endian),
        /// </summary>
        /// <param name="value"></param>
        /// <returns>True if succeeded, false otherwise</returns>
        public bool TryRead87LE(out Single value)
        {
            if (this.CanRead(2))
            {
                value = this.Read87LE();
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }


        /// <summary>
        /// Try to read a fixed-point <see cref="System.Single"/> (Big-endian),
        /// </summary>
        /// <param name="value"></param>
        /// <returns>True if succeeded, false otherwise</returns>
        public bool TryRead87BE(out Single value)
        {
            if (this.CanRead(2))
            {
                value = this.Read87BE();
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }


        #region NON CLS-Compliant members

#pragma warning disable 3001, 3002

        /// <summary>
        /// Try to read a <see cref="System.UInt16"/> (Little-endian),
        /// </summary>
        /// <param name="value"></param>
        /// <returns>True if succeeded, false otherwise</returns>
        public bool TryReadUInt16LE(out UInt16 value)
        {
            if (this.CanRead(2))
            {
                value = this.ReadUInt16LE();
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }


        /// <summary>
        /// Try to read a <see cref="System.UInt16"/> (Big-endian),
        /// </summary>
        /// <param name="value"></param>
        /// <returns>True if succeeded, false otherwise</returns>
        public bool TryReadUInt16BE(out UInt16 value)
        {
            if (this.CanRead(2))
            {
                value = this.ReadUInt16BE();
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }

#pragma warning restore 3001, 3002

        #endregion

        #endregion


        #region IEnumerable members

        public IEnumerator<byte> GetEnumerator()
        {
            return ((IEnumerable<byte>)this._buffer)
                .GetEnumerator();
        }


        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this._buffer
                .GetEnumerator();
        }

        #endregion


        #region IByteArray members

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

        #endregion

    }
}
