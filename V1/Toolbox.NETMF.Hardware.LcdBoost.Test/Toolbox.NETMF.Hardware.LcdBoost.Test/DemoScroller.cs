using System;
using Microsoft.SPOT;
using System.Threading;

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
    /// <summary>
    /// Basic demo of a double scroller
    /// </summary>
    /// <remarks>
    /// The test defines two boxes containing some long text.
    /// There are also two analog inputs to be used to obtain
    /// a numeric adjustable value, these numbers are going
    /// to be used a "scrollers", or "sliders", if you like more.
    /// The boxes have to be layered correctly, wrapping their text
    /// properly, and whithout any sort of problem.
    /// The behavior of this test have to be independent from
    /// the actual display features (size, kind, etc.)
    /// </remarks>
    class DemoScroller
    {
        public static void Run(
            LcdBoostProxy lcd,
            NumericInput slider1,
            NumericInput slider2)
        {
            //define an array for just two elements
            var elements = new ILcdBoostElement[2];

            //define text-area #1
            LcdBoostElementTextArea _te1;
            elements[0] = _te1 = new LcdBoostElementTextArea();
            _te1.Text = "The 74HC165; 74HCT165 are high-speed Si-gate CMOS devices that comply with JEDEC standard no. 7A.";
            _te1.Bounds.Left = 0;
            _te1.Bounds.Top = 0;
            _te1.Bounds.Width = 20;
            _te1.Bounds.Height = 4;

            //define text-area #2
            LcdBoostElementTextArea _te2;
            elements[1] = _te2 = new LcdBoostElementTextArea();
            _te2.Text = "The clock input is a gated-OR structure which allows one input to be used as an active LOW clock enable (CE) input.";
            _te2.Bounds.Left = 10;
            _te2.Bounds.Top = 0;
            _te2.Bounds.Width = 10;
            _te2.Bounds.Height = 10;


            //slider #1
            slider1.SetRange(1, 30);
            slider1.Value = _te1.Bounds.Width;
            slider1.StatusChanged += (s_, e_) =>
            {
                //the scroller adjust the text-area #1 width.
                _te1.Bounds.Width = slider1.Value;
            };

            //slider #2
            slider2.SetRange(-10, 10);
            slider2.StatusChanged += (s_, e_) =>
            {
                //the scroller adjust the text-area #2 top offset
                _te2.Bounds.Top = slider2.Value;
            };


            //display refreshing clock
            var clock = new Timer(
                _ => lcd.Dump(elements),
                null,
                100,
                100);


            Thread.Sleep(Timeout.Infinite);
        }
    }
}
