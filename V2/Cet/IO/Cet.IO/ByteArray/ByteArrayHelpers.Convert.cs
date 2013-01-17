using System;

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
    /// Helpers for byte arrays
    /// </summary>
    public static partial class ByteArrayHelpers
    {
        /// <summary>
        /// Concat more byte-arrays together
        /// </summary>
        /// <param name="segments"></param>
        /// <returns>The resulting byte-array</returns>
        public static byte[] Concat(params byte[][] segments)
        {
            //count the overall buffer size
            int length = 0;

            for (int i = segments.Length - 1; i >= 0; i--)
            {
                length += segments[i].Length;
            }

            var buffer = new byte[length];
            int offset = length - 1;

            //scan the segment array and compose the resulting buffer
            for (int i = segments.Length - 1; i >= 0; i--)
            {
                byte[] segm = segments[i];
                int count = segments.Length;
                offset -= count;

                Array.Copy(
                    segm,
                    0,
                    buffer,
                    offset,
                    count);
            }

            return buffer;
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
        /// <param name="value">The value to be written</param>
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
        /// <param name="value">The value to be written</param>
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


        /// <summary>
        /// Read an <see cref="System.Int64"/> from the given byte array
        /// starting from the specified offset, 
        /// and composed as the Little-endian format
        /// </summary>
        /// <param name="buffer">The source byte array</param>
        /// <param name="offset">The index of the first byte to be considered</param>
        /// <returns>The resulting value</returns>
        public static Int64 ReadInt64LE(
            byte[] buffer,
            int offset)
        {
            return
                (uint)ReadInt32LE(buffer, offset) |
                ((long)ReadInt32LE(buffer, offset + 4) << 32);
        }


        /// <summary>
        /// Write an <see cref="System.Int64"/> to the given byte array
        /// starting from the specified offset, 
        /// and composed as the Little-endian format
        /// </summary>
        /// <param name="buffer">The source byte array</param>
        /// <param name="offset">The index of the first byte to be considered</param>
        /// <returns>The resulting value</returns>
        public static void WriteInt64LE(
            byte[] buffer,
            int offset,
            Int64 value)
        {
            buffer[offset] = (byte)value;
            buffer[offset + 1] = (byte)(value >> 8);
            buffer[offset + 2] = (byte)(value >> 16);
            buffer[offset + 3] = (byte)(value >> 24);
            buffer[offset + 4] = (byte)(value >> 32);
            buffer[offset + 5] = (byte)(value >> 40);
            buffer[offset + 6] = (byte)(value >> 48);
            buffer[offset + 7] = (byte)(value >> 56);
        }


        /// <summary>
        /// Read an <see cref="System.Int64"/> from the given byte array
        /// starting from the specified offset, 
        /// and composed as the Big-endian format
        /// </summary>
        /// <param name="buffer">The source byte array</param>
        /// <param name="offset">The index of the first byte to be considered</param>
        /// <returns>The resulting value</returns>
        public static Int64 ReadInt64BE(
            byte[] buffer,
            int offset)
        {
            return
                (uint)ReadInt32BE(buffer, offset + 4) |
                ((long)ReadInt32BE(buffer, offset) << 32);
        }


        /// <summary>
        /// Write an <see cref="System.Int64"/> to the given byte array
        /// starting from the specified offset, 
        /// and composed as the Big-endian format
        /// </summary>
        /// <param name="buffer">The source byte array</param>
        /// <param name="offset">The index of the first byte to be considered</param>
        /// <returns>The resulting value</returns>
        public static void WriteInt64BE(
            byte[] buffer,
            int offset,
            Int64 value)
        {
            buffer[offset] = (byte)(value >> 56);
            buffer[offset + 1] = (byte)(value >> 48);
            buffer[offset + 2] = (byte)(value >> 40);
            buffer[offset + 3] = (byte)(value >> 32);
            buffer[offset + 4] = (byte)(value >> 24);
            buffer[offset + 5] = (byte)(value >> 16);
            buffer[offset + 6] = (byte)(value >> 8);
            buffer[offset + 7] = (byte)value;
        }


        #region Fixed-point converters

        /// <summary>
        /// Read an <see cref="System.Single"/> from the given byte array
        /// starting from the specified offset, 
        /// and composed as the Little-endian format
        /// </summary>
        /// <param name="buffer">The source byte array</param>
        /// <param name="offset">The index of the first byte to be considered</param>
        /// <returns>The resulting value</returns>
        public static float Read87LE(
            byte[] buffer,
            int offset)
        {
            var temp =
                buffer[offset]
                | (buffer[offset + 1] << 8);

            return (float)(temp / 128f);
        }


        /// <summary>
        /// Write an <see cref="System.Single"/> to the given byte array
        /// starting from the specified offset, 
        /// and composed as the Little-endian format
        /// </summary>
        /// <param name="buffer">The source byte array</param>
        /// <param name="offset">The index of the first byte to be considered</param>
        /// <param name="value">The value to be written</param>
        public static void Write87LE(
            byte[] buffer,
            int offset,
            Single value)
        {
            int ivalue = (int)(value * 128f);
            buffer[offset] = (byte)ivalue;
            buffer[offset + 1] = (byte)(ivalue >> 8);
        }


        /// <summary>
        /// Read an <see cref="System.Single"/> from the given byte array
        /// starting from the specified offset, 
        /// and composed as the Big-endian format
        /// </summary>
        /// <param name="buffer">The source byte array</param>
        /// <param name="offset">The index of the first byte to be considered</param>
        /// <returns>The resulting value</returns>
        public static float Read87BE(
            byte[] buffer,
            int offset)
        {
            var temp =
                buffer[offset + 1] |
                (buffer[offset] << 8);

            return (float)(temp / 128f);
        }


        /// <summary>
        /// Write an <see cref="System.Single"/> to the given byte array
        /// starting from the specified offset, 
        /// and composed as the Big-endian format
        /// </summary>
        /// <param name="buffer">The source byte array</param>
        /// <param name="offset">The index of the first byte to be considered</param>
        /// <param name="value">The value to be written</param>
        public static void Write87BE(
            byte[] buffer,
            int offset,
            float value)
        {
            int ivalue = (int)(value * 128f);
            buffer[offset] = (byte)(ivalue >> 8);
            buffer[offset + 1] = (byte)ivalue;
        }


        /// <summary>
        /// Read an <see cref="System.Single"/> from the given byte array
        /// starting from the specified offset, 
        /// and composed as the Little-endian format
        /// </summary>
        /// <param name="buffer">The source byte array</param>
        /// <param name="offset">The index of the first byte to be considered</param>
        /// <returns>The resulting value</returns>
        public static float Read1912LE(
            byte[] buffer,
            int offset)
        {
            var temp =
                buffer[offset] |
                (buffer[offset + 1] << 8) |
                (buffer[offset + 2] << 16) |
                (buffer[offset + 3] << 24);

            return (float)(temp / 4096f);
        }


        /// <summary>
        /// Write an <see cref="System.Single"/> to the given byte array
        /// starting from the specified offset, 
        /// and composed as the Little-endian format
        /// </summary>
        /// <param name="buffer">The source byte array</param>
        /// <param name="offset">The index of the first byte to be considered</param>
        /// <param name="value">The value to be written</param>
        public static void Write1912LE(
            byte[] buffer,
            int offset,
            Single value)
        {
            int ivalue = (int)(value * 4096f);
            buffer[offset] = (byte)ivalue;
            buffer[offset + 1] = (byte)(ivalue >> 8);
            buffer[offset + 2] = (byte)(ivalue >> 16);
            buffer[offset + 3] = (byte)(ivalue >> 24);
        }


        /// <summary>
        /// Read an <see cref="System.Single"/> from the given byte array
        /// starting from the specified offset, 
        /// and composed as the Big-endian format
        /// </summary>
        /// <param name="buffer">The source byte array</param>
        /// <param name="offset">The index of the first byte to be considered</param>
        /// <returns>The resulting value</returns>
        public static float Read1912BE(
            byte[] buffer,
            int offset)
        {
            var temp =
                buffer[offset + 3] |
                (buffer[offset + 2] << 8) |
                (buffer[offset + 1] << 16) |
                (buffer[offset] << 24);

            return (float)(temp / 4096f);
        }


        /// <summary>
        /// Write an <see cref="System.Single"/> to the given byte array
        /// starting from the specified offset, 
        /// and composed as the Big-endian format
        /// </summary>
        /// <param name="buffer">The source byte array</param>
        /// <param name="offset">The index of the first byte to be considered</param>
        /// <param name="value">The value to be written</param>
        public static void Write1912BE(
            byte[] buffer,
            int offset,
            Single value)
        {
            int ivalue = (int)(value * 4096f);
            buffer[offset] = (byte)(ivalue >> 24);
            buffer[offset + 1] = (byte)(ivalue >> 16);
            buffer[offset + 2] = (byte)(ivalue >> 8);
            buffer[offset + 3] = (byte)ivalue;
        }

        #endregion

    }
}
