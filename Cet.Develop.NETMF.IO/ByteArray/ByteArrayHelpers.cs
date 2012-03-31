using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

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
namespace Cet.Develop.NETMF.IO
{
    /// <summary>
    /// Helpers for the byte arrays
    /// </summary>
    public static class ByteArrayHelpers
    {
        /// <summary>
        /// Create a copy of the given byte array
        /// </summary>
        /// <param name="source">The source byte-array</param>
        /// <returns>The resulting copy</returns>
        public static byte[] ToArray(this byte[] source)
        {
            return source.ToArray(
                0,
                source.Length);
        }



        /// <summary>
        /// Create a new byte array starting from the defined
        /// segment of the given one
        /// </summary>
        /// <param name="source">The source byte-array</param>
        /// <param name="offset">The index of the first byte to be considered</param>
        /// <param name="count">The number of the bytes to be considered</param>
        /// <returns>The resulting byte-array</returns>
        public static byte[] ToArray(
            this byte[] source,
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



        /// <summary>
        /// Calculate the LRC of the given byte array
        /// </summary>
        /// <param name="buffer">The source byte array</param>
        /// <param name="offset">The index of the first byte to be considered</param>
        /// <param name="count">The number of the bytes to be considered</param>
        /// <returns>The resulting value</returns>
        public static byte CalcLRC(
            byte[] buffer,
            int offset,
            int count)
        {
            int lrc = 0;

            for (int i = offset, upper = offset + count; i < upper; i++)
            {
                lrc ^= buffer[i];
            }

            return (byte)lrc;
        }



        /// <summary>
        /// Read an <see cref="System.Int16"/> from the given byte array
        /// starting from the specified offset, 
        /// and composed as the Little-endian format
        /// </summary>
        /// <param name="buffer">The source byte array</param>
        /// <param name="offset">The index of the first byte to be considered</param>
        /// <returns>The resulting value</returns>
        public static Int16 ReadInt16LE(
            byte[] buffer,
            int offset)
        {
            return (Int16)(
                buffer[offset] |
                (buffer[offset + 1] << 8));
        }



        /// <summary>
        /// Write an <see cref="System.Int16"/> to the given byte array
        /// starting from the specified offset, 
        /// and composed as the Little-endian format
        /// </summary>
        /// <param name="buffer">The source byte array</param>
        /// <param name="offset">The index of the first byte to be considered</param>
        /// <returns>The resulting value</returns>
        public static void WriteInt16LE(
            byte[] buffer,
            int offset,
            Int16 value)
        {
            buffer[offset] = (byte)value;
            buffer[offset + 1] = (byte)(value >> 8);
        }



        /// <summary>
        /// Read an <see cref="System.Int16"/> from the given byte array
        /// starting from the specified offset, 
        /// and composed as the Big-endian format
        /// </summary>
        /// <param name="buffer">The source byte array</param>
        /// <param name="offset">The index of the first byte to be considered</param>
        /// <returns>The resulting value</returns>
        public static Int16 ReadInt16BE(
            byte[] buffer,
            int offset)
        {
            return (Int16)(
                buffer[offset + 1] |
                (buffer[offset] << 8));
        }



        /// <summary>
        /// Write an <see cref="System.Int16"/> to the given byte array
        /// starting from the specified offset, 
        /// and composed as the Big-endian format
        /// </summary>
        /// <param name="buffer">The source byte array</param>
        /// <param name="offset">The index of the first byte to be considered</param>
        /// <returns>The resulting value</returns>
        public static void WriteInt16BE(
            byte[] buffer,
            int offset,
            Int16 value)
        {
            buffer[offset] = (byte)(value >> 8);
            buffer[offset + 1] = (byte)value;
        }



        #region NON CLS-Compliant members

#pragma warning disable 3001, 3002

        /// <summary>
        /// Read an <see cref="System.UInt16"/> from the given byte array
        /// starting from the specified offset, 
        /// and composed as the Little-endian format
        /// </summary>
        /// <param name="buffer">The source byte array</param>
        /// <param name="offset">The index of the first byte to be considered</param>
        /// <returns>The resulting value</returns>
        public static UInt16 ReadUInt16LE(
            byte[] buffer,
            int offset)
        {
            return (UInt16)(
                buffer[offset] |
                (buffer[offset + 1] << 8));
        }



        /// <summary>
        /// Write an <see cref="System.UInt16"/> to the given byte array
        /// starting from the specified offset, 
        /// and composed as the Little-endian format
        /// </summary>
        /// <param name="buffer">The source byte array</param>
        /// <param name="offset">The index of the first byte to be considered</param>
        /// <returns>The resulting value</returns>
        public static void WriteUInt16LE(
            byte[] buffer,
            int offset,
            UInt16 value)
        {
            buffer[offset] = (byte)value;
            buffer[offset + 1] = (byte)(value >> 8);
        }



        /// <summary>
        /// Read an <see cref="System.UInt16"/> from the given byte array
        /// starting from the specified offset, 
        /// and composed as the Big-endian format
        /// </summary>
        /// <param name="buffer">The source byte array</param>
        /// <param name="offset">The index of the first byte to be considered</param>
        /// <returns>The resulting value</returns>
        public static UInt16 ReadUInt16BE(
            byte[] buffer,
            int offset)
        {
            return (UInt16)(
                buffer[offset + 1] |
                (buffer[offset] << 8));
        }



        /// <summary>
        /// Write an <see cref="System.UInt16"/> to the given byte array
        /// starting from the specified offset, 
        /// and composed as the Big-endian format
        /// </summary>
        /// <param name="buffer">The source byte array</param>
        /// <param name="offset">The index of the first byte to be considered</param>
        /// <returns>The resulting value</returns>
        public static void WriteUInt16BE(
            byte[] buffer,
            int offset,
            UInt16 value)
        {
            buffer[offset] = (byte)(value >> 8);
            buffer[offset + 1] = (byte)value;
        }

#pragma warning restore 3001, 3002

        #endregion



        /// <summary>
        /// Read an <see cref="System.Int32"/> from the given byte array
        /// starting from the specified offset, 
        /// and composed as the Little-endian format
        /// </summary>
        /// <param name="buffer">The source byte array</param>
        /// <param name="offset">The index of the first byte to be considered</param>
        /// <returns>The resulting value</returns>
        public static Int32 ReadInt32LE(
            byte[] buffer,
            int offset)
        {
            return (Int32)(
                buffer[offset] |
                (buffer[offset + 1] << 8) |
                (buffer[offset + 2] << 16) |
                (buffer[offset + 3] << 24));
        }



        /// <summary>
        /// Write an <see cref="System.Int32"/> to the given byte array
        /// starting from the specified offset, 
        /// and composed as the Little-endian format
        /// </summary>
        /// <param name="buffer">The source byte array</param>
        /// <param name="offset">The index of the first byte to be considered</param>
        /// <returns>The resulting value</returns>
        public static void WriteInt32LE(
            byte[] buffer,
            int offset,
            Int32 value)
        {
            buffer[offset] = (byte)value;
            buffer[offset + 1] = (byte)(value >> 8);
            buffer[offset + 2] = (byte)(value >> 16);
            buffer[offset + 3] = (byte)(value >> 24);
        }



        /// <summary>
        /// Read an <see cref="System.Int32"/> from the given byte array
        /// starting from the specified offset, 
        /// and composed as the Big-endian format
        /// </summary>
        /// <param name="buffer">The source byte array</param>
        /// <param name="offset">The index of the first byte to be considered</param>
        /// <returns>The resulting value</returns>
        public static Int32 ReadInt32BE(
            byte[] buffer,
            int offset)
        {
            return (Int32)(
                buffer[offset + 3] |
                (buffer[offset + 2] << 8) |
                (buffer[offset + 1] << 16) |
                (buffer[offset] << 24));
        }



        /// <summary>
        /// Write an <see cref="System.Int32"/> to the given byte array
        /// starting from the specified offset, 
        /// and composed as the Big-endian format
        /// </summary>
        /// <param name="buffer">The source byte array</param>
        /// <param name="offset">The index of the first byte to be considered</param>
        /// <returns>The resulting value</returns>
        public static void WriteInt32BE(
            byte[] buffer,
            int offset,
            Int32 value)
        {
            buffer[offset] = (byte)(value >> 24);
            buffer[offset + 1] = (byte)(value >> 16);
            buffer[offset + 2] = (byte)(value >> 8);
            buffer[offset + 3] = (byte)value;
        }

    }
}
