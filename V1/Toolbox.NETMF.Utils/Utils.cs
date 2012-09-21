using System;
using Microsoft.SPOT;

/*
 * Copyright 2012 Mario Vernari (http://netmftoolbox.codeplex.com/)
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
namespace Toolbox.NETMF
{
    class Utils
    {
        private static readonly char[] _tableToHex = "0123456789ABCDEF".ToCharArray();



        public static string ToHex(
            byte[] buffer,
            int offset,
            int count)
        {
            int totalLength = count * 3;
            var outputBuffer = new char[totalLength];

            //insert the remaining bytes
            int length = buffer.Length;
            for (int currentLength = 0, idx = offset; currentLength < totalLength && idx < length; currentLength += 3, idx++)
            {
                byte value = buffer[idx];
                outputBuffer[currentLength] = _tableToHex[value >> 4];
                outputBuffer[currentLength + 1] = _tableToHex[value & 0x0F];
                outputBuffer[currentLength + 2] = ' ';
            }

            return new String(outputBuffer);
        }

    }


#if false
    /// <summary>
    /// The delegate for a generic state changed event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void StatusChangedEventHandler(
        object sender, 
        StatusChangedEventArgs e);



    /// <summary>
    /// Extension wrapper to the standard <see cref="Microsoft.SPOT.EventArgs"/> object
    /// </summary>
    public class StatusChangedEventArgs
        : EventArgs
    {
        public int Data1;
        public float Data2;
    }
#endif
}
