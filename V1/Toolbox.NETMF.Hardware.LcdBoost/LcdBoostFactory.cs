using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

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
namespace Toolbox.NETMF.Hardware
{
    public static class LcdBoostFactory
    {
        public static LcdBoostDriverHD44780 Create16x2(Cpu.Pin chipSelect)
        {
            return new LcdBoostDriverHD44780(
                chipSelect: chipSelect,
                logicalSize: 0x0210,
                physicalColumns: 0x10,
                physicalRow0: 0x00000000,
                physicalRow1: 0x00000100
                );
        }


        public static LcdBoostDriverHD44780 Create20x4(Cpu.Pin chipSelect)
        {
            return new LcdBoostDriverHD44780(
                chipSelect: chipSelect,
                logicalSize: 0x0414,
                physicalColumns: 0x14,
                physicalRow0: 0x02000000,
                physicalRow1: 0x03000100
                );
        }
    }
}
