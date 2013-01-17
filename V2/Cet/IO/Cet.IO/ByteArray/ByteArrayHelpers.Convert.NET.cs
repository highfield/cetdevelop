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
    public static partial class ByteArrayHelpers
    {
        /// <summary>
        /// Create a copy of the given byte array
        /// </summary>
        /// <param name="source">The source byte-array</param>
        /// <returns>The resulting copy</returns>
        public static byte[] ArrayCopy(byte[] source)
        {
            return ByteArrayHelpers.ArrayCopy(
                source,
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
        public static byte[] ArrayCopy(
            byte[] source,
            int offset,
            int count)
        {
            var target = new byte[count];

            Buffer.BlockCopy(
                source,
                offset,
                target,
                0,
                count);

            return target;
        }


        #region Enumerator converters

        /// <summary>
        /// Restituisce un enumeratore che fornisce i bytes per rappresentare uno short. Si considera il formato "little-endian" (Intel)
        /// </summary>
        /// <param name="value">Il valore da convertire</param>
        /// <returns>L'enumeratore con i bytes equivalenti</returns>
        public static IEnumerable<byte> EnumerateInt16LE(Int16 value)
        {
            unchecked
            {
                yield return (byte)value;
                yield return (byte)(value >> 8);
            }
        }


        /// <summary>
        /// Restituisce un enumeratore che fornisce i bytes per rappresentare uno short. Si considera il formato "big-endian" (Motorola)
        /// </summary>
        /// <param name="value">Il valore da convertire</param>
        /// <returns>L'enumeratore con i bytes equivalenti</returns>
        public static IEnumerable<byte> EnumerateInt16BE(Int16 value)
        {
            unchecked
            {
                yield return (byte)(value >> 8);
                yield return (byte)value;
            }
        }


        /// <summary>
        /// Restituisce un enumeratore che fornisce i bytes per rappresentare un intero. Si considera il formato "little-endian" (Intel)
        /// </summary>
        /// <param name="value">Il valore da convertire</param>
        /// <returns>L'enumeratore con i bytes equivalenti</returns>
        public static IEnumerable<byte> EnumerateInt32LE(Int32 value)
        {
            unchecked
            {
                yield return (byte)value;
                yield return (byte)(value >> 8);
                yield return (byte)(value >> 16);
                yield return (byte)(value >> 24);
            }
        }


        /// <summary>
        /// Restituisce un enumeratore che fornisce i bytes per rappresentare un intero. Si considera il formato "big-endian" (Motorola)
        /// </summary>
        /// <param name="value">Il valore da convertire</param>
        /// <returns>L'enumeratore con i bytes equivalenti</returns>
        public static IEnumerable<byte> EnumerateInt32BE(Int32 value)
        {
            unchecked
            {
                yield return (byte)(value >> 24);
                yield return (byte)(value >> 16);
                yield return (byte)(value >> 8);
                yield return (byte)value;
            }
        }


        /// <summary>
        /// Restituisce un enumeratore che fornisce i bytes per rappresentare un single. Si considera il formato "little-endian" (Intel)
        /// </summary>
        /// <param name="value">Il valore da convertire</param>
        /// <returns>L'enumeratore con i bytes equivalenti</returns>
        public static IEnumerable<byte> EnumerateSingleLE(Single value)
        {
            byte[] temp = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian)
            {
                //Intel
                yield return temp[0];
                yield return temp[1];
                yield return temp[2];
                yield return temp[3];
            }
            else
            {
                yield return temp[3];
                yield return temp[2];
                yield return temp[1];
                yield return temp[0];
            }
        }


        /// <summary>
        /// Restituisce un enumeratore che fornisce i bytes per rappresentare un single. Si considera il formato "big-endian" (Motorola)
        /// </summary>
        /// <param name="value">Il valore da convertire</param>
        /// <returns>L'enumeratore con i bytes equivalenti</returns>
        public static IEnumerable<byte> EnumerateSingleBE(Single value)
        {
            byte[] temp = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian)
            {
                //Intel
                yield return temp[3];
                yield return temp[2];
                yield return temp[1];
                yield return temp[0];
            }
            else
            {
                yield return temp[0];
                yield return temp[1];
                yield return temp[2];
                yield return temp[3];
            }
        }


        #region NON CLS-Compliant members

#pragma warning disable 3001, 3002

        /// <summary>
        /// Restituisce un enumeratore che fornisce i bytes per rappresentare uno short senza segno. Si considera il formato "little-endian" (Intel)
        /// </summary>
        /// <param name="value">Il valore da convertire</param>
        /// <returns>L'enumeratore con i bytes equivalenti</returns>
        /// <remarks>
        /// La funzione non viene garantita "thread-safe" per questioni di performance
        /// Introdotta solo per compatibilita' con vecchi prodotti: se ne sconsiglia l'utilizzo
        /// </remarks>
        public static IEnumerable<byte> EnumerateUInt16LE(UInt16 value)
        {
            unchecked
            {
                yield return (byte)value;
                yield return (byte)(value >> 8);
            }
        }


        /// <summary>
        /// Restituisce un enumeratore che fornisce i bytes per rappresentare uno short senza segno. Si considera il formato "big-endian" (Motorola)
        /// </summary>
        /// <param name="value">Il valore da convertire</param>
        /// <returns>L'enumeratore con i bytes equivalenti</returns>
        /// <remarks>
        /// La funzione non viene garantita "thread-safe" per questioni di performance
        /// Introdotta solo per compatibilita' con vecchi prodotti: se ne sconsiglia l'utilizzo
        /// </remarks>
        public static IEnumerable<byte> EnumerateUInt16BE(UInt16 value)
        {
            unchecked
            {
                yield return (byte)(value >> 8);
                yield return (byte)value;
            }
        }


#pragma warning restore 3001, 3002

        #endregion


        /// <summary>
        /// Restituisce un enumeratore che fornisce i bytes per rappresentare un single. Si considera il formato "little-endian" (Intel)
        /// </summary>
        /// <param name="value">Il valore da convertire</param>
        /// <returns>L'enumeratore con i bytes equivalenti</returns>
        public static IEnumerable<byte> Enumerate87LE(Single value)
        {
            unchecked
            {
                int ivalue = (int)(value * 128f);
                yield return (byte)ivalue;
                yield return (byte)(ivalue >> 8);
            }
        }


        /// <summary>
        /// Restituisce un enumeratore che fornisce i bytes per rappresentare un single. Si considera il formato "big-endian" (Motorola)
        /// </summary>
        /// <param name="value">Il valore da convertire</param>
        /// <returns>L'enumeratore con i bytes equivalenti</returns>
        public static IEnumerable<byte> Enumerate87BE(Single value)
        {
            unchecked
            {
                int ivalue = (int)(value * 128f);
                yield return (byte)(ivalue >> 8);
                yield return (byte)ivalue;
            }
        }


        /// <summary>
        /// Restituisce un enumeratore che fornisce i bytes per rappresentare un single. Si considera il formato "little-endian" (Intel)
        /// </summary>
        /// <param name="value">Il valore da convertire</param>
        /// <returns>L'enumeratore con i bytes equivalenti</returns>
        public static IEnumerable<byte> Enumerate1912LE(Single value)
        {
            unchecked
            {
                int ivalue = (int)(value * 4096f);
                yield return (byte)ivalue;
                yield return (byte)(ivalue >> 8);
                yield return (byte)(ivalue >> 16);
                yield return (byte)(ivalue >> 24);
            }
        }


        /// <summary>
        /// Restituisce un enumeratore che fornisce i bytes per rappresentare un single. Si considera il formato "big-endian" (Motorola)
        /// </summary>
        /// <param name="value">Il valore da convertire</param>
        /// <returns>L'enumeratore con i bytes equivalenti</returns>
        public static IEnumerable<byte> Enumerate1912BE(Single value)
        {
            unchecked
            {
                int ivalue = (int)(value * 4096f);
                yield return (byte)(ivalue >> 24);
                yield return (byte)(ivalue >> 16);
                yield return (byte)(ivalue >> 8);
                yield return (byte)ivalue;
            }
        }

        #endregion


        #region Floating-point converters

        /// <summary>
        /// Read an <see cref="System.Single"/> from the given byte array
        /// starting from the specified offset, 
        /// and composed as the Little-endian format
        /// </summary>
        /// <param name="buffer">The source byte array</param>
        /// <param name="offset">The index of the first byte to be considered</param>
        /// <returns>The resulting value</returns>
        public static Single ReadSingleLE(
            byte[] buffer,
            int offset)
        {
            byte[] temp = new byte[4];

            if (BitConverter.IsLittleEndian)
            {
                temp[0] = buffer[offset];
                temp[1] = buffer[offset + 1];
                temp[2] = buffer[offset + 2];
                temp[3] = buffer[offset + 3];
            }
            else
            {
                temp[3] = buffer[offset];
                temp[2] = buffer[offset + 1];
                temp[1] = buffer[offset + 2];
                temp[0] = buffer[offset + 3];
            }

            return BitConverter.ToSingle(temp, 0);
        }


        /// <summary>
        /// Write an <see cref="System.Single"/> to the given byte array
        /// starting from the specified offset, 
        /// and composed as the Little-endian format
        /// </summary>
        /// <param name="buffer">The source byte array</param>
        /// <param name="offset">The index of the first byte to be considered</param>
        /// <param name="value">The value to be written</param>
        public static void WriteSingleLE(
            byte[] buffer,
            int offset,
            Single value)
        {
            byte[] temp = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian)
            {
                buffer[offset] = temp[0];
                buffer[offset + 1] = temp[1];
                buffer[offset + 2] = temp[2];
                buffer[offset + 3] = temp[3];
            }
            else
            {
                buffer[offset] = temp[3];
                buffer[offset + 1] = temp[2];
                buffer[offset + 2] = temp[1];
                buffer[offset + 3] = temp[0];
            }
        }


        /// <summary>
        /// Read an <see cref="System.Single"/> from the given byte array
        /// starting from the specified offset, 
        /// and composed as the Big-endian format
        /// </summary>
        /// <param name="buffer">The source byte array</param>
        /// <param name="offset">The index of the first byte to be considered</param>
        /// <returns>The resulting value</returns>
        public static Single ReadSingleBE(
            byte[] buffer,
            int offset)
        {
            byte[] temp = new byte[4];

            if (BitConverter.IsLittleEndian)
            {
                temp[3] = buffer[offset];
                temp[2] = buffer[offset + 1];
                temp[1] = buffer[offset + 2];
                temp[0] = buffer[offset + 3];
            }
            else
            {
                temp[0] = buffer[offset];
                temp[1] = buffer[offset + 1];
                temp[2] = buffer[offset + 2];
                temp[3] = buffer[offset + 3];
            }

            return BitConverter.ToSingle(temp, 0);
        }


        /// <summary>
        /// Write an <see cref="System.Single"/> to the given byte array
        /// starting from the specified offset, 
        /// and composed as the Big-endian format
        /// </summary>
        /// <param name="buffer">The source byte array</param>
        /// <param name="offset">The index of the first byte to be considered</param>
        /// <param name="value">The value to be written</param>
        public static void WriteSingleBE(
            byte[] buffer,
            int offset,
            Single value)
        {
            byte[] temp = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian)
            {
                buffer[offset] = temp[3];
                buffer[offset + 1] = temp[2];
                buffer[offset + 2] = temp[1];
                buffer[offset + 3] = temp[0];
            }
            else
            {
                buffer[offset] = temp[0];
                buffer[offset + 1] = temp[1];
                buffer[offset + 2] = temp[2];
                buffer[offset + 3] = temp[3];
            }
        }


        /// <summary>
        /// Read an <see cref="System.Double"/> from the given byte array
        /// starting from the specified offset, 
        /// and composed as the Little-endian format
        /// </summary>
        /// <param name="buffer">The source byte array</param>
        /// <param name="offset">The index of the first byte to be considered</param>
        /// <returns>The resulting value</returns>
        public static Double ReadDoubleLE(
            byte[] buffer,
            int offset)
        {
            byte[] temp = new byte[8];

            if (BitConverter.IsLittleEndian)
            {
                temp[0] = buffer[offset];
                temp[1] = buffer[offset + 1];
                temp[2] = buffer[offset + 2];
                temp[3] = buffer[offset + 3];
                temp[4] = buffer[offset + 4];
                temp[5] = buffer[offset + 5];
                temp[6] = buffer[offset + 6];
                temp[7] = buffer[offset + 7];
            }
            else
            {
                temp[7] = buffer[offset];
                temp[6] = buffer[offset + 1];
                temp[5] = buffer[offset + 2];
                temp[4] = buffer[offset + 3];
                temp[3] = buffer[offset + 4];
                temp[2] = buffer[offset + 5];
                temp[1] = buffer[offset + 6];
                temp[0] = buffer[offset + 7];
            }

            return BitConverter.ToDouble(temp, 0);
        }


        /// <summary>
        /// Write an <see cref="System.Double"/> to the given byte array
        /// starting from the specified offset, 
        /// and composed as the Little-endian format
        /// </summary>
        /// <param name="buffer">The source byte array</param>
        /// <param name="offset">The index of the first byte to be considered</param>
        /// <param name="value">The value to be written</param>
        public static void WriteDoubleLE(
            byte[] buffer,
            int offset,
            Double value)
        {
            byte[] temp = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian)
            {
                buffer[offset] = temp[0];
                buffer[offset + 1] = temp[1];
                buffer[offset + 2] = temp[2];
                buffer[offset + 3] = temp[3];
                buffer[offset + 4] = temp[4];
                buffer[offset + 5] = temp[5];
                buffer[offset + 6] = temp[6];
                buffer[offset + 7] = temp[7];
            }
            else
            {
                buffer[offset] = temp[7];
                buffer[offset + 1] = temp[6];
                buffer[offset + 2] = temp[5];
                buffer[offset + 3] = temp[4];
                buffer[offset + 4] = temp[3];
                buffer[offset + 5] = temp[2];
                buffer[offset + 6] = temp[1];
                buffer[offset + 7] = temp[0];
            }
        }


        /// <summary>
        /// Read an <see cref="System.Double"/> from the given byte array
        /// starting from the specified offset, 
        /// and composed as the Big-endian format
        /// </summary>
        /// <param name="buffer">The source byte array</param>
        /// <param name="offset">The index of the first byte to be considered</param>
        /// <returns>The resulting value</returns>
        public static Double ReadDoubleBE(
            byte[] buffer,
            int offset)
        {
            byte[] temp = new byte[8];

            if (BitConverter.IsLittleEndian)
            {
                temp[7] = buffer[offset];
                temp[6] = buffer[offset + 1];
                temp[5] = buffer[offset + 2];
                temp[4] = buffer[offset + 3];
                temp[3] = buffer[offset + 4];
                temp[2] = buffer[offset + 5];
                temp[1] = buffer[offset + 6];
                temp[0] = buffer[offset + 7];
            }
            else
            {
                temp[0] = buffer[offset];
                temp[1] = buffer[offset + 1];
                temp[2] = buffer[offset + 2];
                temp[3] = buffer[offset + 3];
                temp[4] = buffer[offset + 4];
                temp[5] = buffer[offset + 5];
                temp[6] = buffer[offset + 6];
                temp[7] = buffer[offset + 7];
            }

            return BitConverter.ToDouble(temp, 0);
        }


        /// <summary>
        /// Write an <see cref="System.Double"/> to the given byte array
        /// starting from the specified offset, 
        /// and composed as the Big-endian format
        /// </summary>
        /// <param name="buffer">The source byte array</param>
        /// <param name="offset">The index of the first byte to be considered</param>
        /// <param name="value">The value to be written</param>
        public static void WriteDoubleBE(
            byte[] buffer,
            int offset,
            Double value)
        {
            byte[] temp = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian)
            {
                buffer[offset] = temp[7];
                buffer[offset + 1] = temp[6];
                buffer[offset + 2] = temp[5];
                buffer[offset + 3] = temp[4];
                buffer[offset + 4] = temp[3];
                buffer[offset + 5] = temp[2];
                buffer[offset + 6] = temp[1];
                buffer[offset + 7] = temp[0];
            }
            else
            {
                buffer[offset] = temp[0];
                buffer[offset + 1] = temp[1];
                buffer[offset + 2] = temp[2];
                buffer[offset + 3] = temp[3];
                buffer[offset + 4] = temp[4];
                buffer[offset + 5] = temp[5];
                buffer[offset + 6] = temp[6];
                buffer[offset + 7] = temp[7];
            }
        }

        #endregion

    }
}
