using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

using Cet.HW.GDI;

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
namespace Cet.HW.Drivers.Demo
{
    public class Program
    {
        //lookup table
        private delegate void DemoTaskDelegate();

        private static readonly DemoTaskDelegate[] DemoTable = new DemoTaskDelegate[]
        {
            TestPoints,
            TestHLines,
            TestVLines,
            TestForwardLines1,
            TestForwardLines2,
            TestForwardLines3,
            TestBackwardLines1,
            TestBackwardLines2,
            TestBackwardLines3,
            TestLineClipping,
            TestColors,
            TestEmptyRectangles,
            TestFilledRectangles,
            TestRectangleClipping,
            TestString,
            TestStringClipping,
        };

        //declare buttons for browsing demos
        private static InputPort _btnLeft = new InputPort(Pins.GPIO_PIN_D6, true, Port.ResistorMode.PullUp);
        private static InputPort _btnRight = new InputPort(Pins.GPIO_PIN_D7, true, Port.ResistorMode.PullUp);

        private static int _demoIndex;

        private static ICompositionRenderer _renderer;
        private static CompositionTargetBase _composition;

        private static Timer _clock;
        private static int _counter;

        //just for timing measurements
        private static OutputPort _qtest = new OutputPort(Pins.GPIO_PIN_D2, false);


        public static void Main()
        {
            //create a led-matrix driver instance
            _renderer = new SureHT1632(
                cspin: Pins.GPIO_PIN_D10,
                clkpin: Pins.GPIO_PIN_D9);

            //creates the target composition instance
            _composition = _renderer.CreateTarget();

            //start the timer for the demo loop
            const int LoopPeriod = 100; //ms

            _clock = new Timer(
                ClockTick,
                null,
                LoopPeriod,
                LoopPeriod);

            //keep alive
            Thread.Sleep(Timeout.Infinite);
        }


        private static void ClockTick(object state)
        {
            _qtest.Write(true);

            if ((_counter & 3) == 0)
            {
                //check for browsing buttons
                if (_btnLeft.Read() == false)
                {
                    _demoIndex--;
                    if (_demoIndex < 0)
                        _demoIndex = DemoTable.Length - 1;
                }
                else if (_btnRight.Read() == false)
                {
                    _demoIndex++;
                    if (_demoIndex >= DemoTable.Length)
                        _demoIndex = 0;
                }
            }

            //wipes out the composition target
            _composition.Clear();

            //perform demo rendering
            DemoTable[_demoIndex]();

            //dumps the composition to the physical device
            _renderer.Dump(_composition);

            ++_counter;

            _qtest.Write(false);
        }


        private static void TestPoints()
        {
            int x = _counter & 31;

            _composition.DrawPixel(x, 0, Colors.Red);
            _composition.DrawPixel(x, 2, Colors.Red);
            _composition.DrawPixel(x, 4, Colors.Red);
            _composition.DrawPixel(x, 6, Colors.Red);
            _composition.DrawPixel(x, 8, Colors.Red);
            _composition.DrawPixel(x, 10, Colors.Red);
            _composition.DrawPixel(x, 12, Colors.Red);
            _composition.DrawPixel(x, 14, Colors.Red);

            _composition.DrawPixel(31 - x, 1, Colors.Lime);
            _composition.DrawPixel(31 - x, 3, Colors.Lime);
            _composition.DrawPixel(31 - x, 5, Colors.Lime);
            _composition.DrawPixel(31 - x, 7, Colors.Lime);
            _composition.DrawPixel(31 - x, 9, Colors.Lime);
            _composition.DrawPixel(31 - x, 11, Colors.Lime);
            _composition.DrawPixel(31 - x, 13, Colors.Lime);
            _composition.DrawPixel(31 - x, 15, Colors.Lime);
        }


        private static void TestHLines()
        {
            //thickness = 1, from left to right
            _composition.DrawLine(
                Pens.Lime,
                new Point(3, 1),
                new Point(28, 1)
                );

            //thickness = 2, from right to left
            _composition.DrawLine(
                new Pen(Colors.Red, 2),
                new Point(28, 4),
                new Point(3, 4)
                );

            //thickness = 3, from left to right
            _composition.DrawLine(
                new Pen(Colors.Lime, 3),
                new Point(3, 8),
                new Point(28, 8)
                );

            //thickness = 4, from right to left
            _composition.DrawLine(
                new Pen(Colors.Red, 4),
                new Point(28, 13),
                new Point(3, 13)
                );
        }


        private static void TestVLines()
        {
            //thickness = 1, from top to bottom
            _composition.DrawLine(
                Pens.Lime,
                new Point(2, 3),
                new Point(2, 12)
                );

            //thickness = 2, from bottom to top
            _composition.DrawLine(
                new Pen(Colors.Red, 2),
                new Point(8, 12),
                new Point(8, 3)
                );

            //thickness = 3, from top to bottom
            _composition.DrawLine(
                new Pen(Colors.Lime, 3),
                new Point(16, 3),
                new Point(16, 12)
                );

            //thickness = 4, from bottom to top
            _composition.DrawLine(
                new Pen(Colors.Red, 4),
                new Point(24, 12),
                new Point(24, 3)
                );
        }


        private static void TestForwardLines1()
        {
            //thickness = 1, 45 degs, from bottom-left to top-right
            _composition.DrawLine(
                Pens.Lime,
                new Point(2, 13),
                new Point(13, 2)
                );

            //thickness = 2, 45 degs, from top-right to bottom-left
            _composition.DrawLine(
                new Pen(Colors.Red, 2),
                new Point(21, 2),
                new Point(10, 13)
                );

            //thickness = 3, 45 degs, from bottom-left to top-right
            _composition.DrawLine(
                new Pen(Colors.Lime, 3),
                new Point(18, 13),
                new Point(29, 2)
                );
        }


        private static void TestForwardLines2()
        {
            //thickness = 1, 30 degs, from bottom-left to top-right
            _composition.DrawLine(
                Pens.Lime,
                new Point(2, 10),
                new Point(13, 5)
                );

            //thickness = 2, 30 degs, from top-right to bottom-left
            _composition.DrawLine(
                new Pen(Colors.Red, 2),
                new Point(29, 5),
                new Point(18, 10)
                );
        }


        private static void TestForwardLines3()
        {
            //thickness = 1, 60 degs, from bottom-left to top-right
            _composition.DrawLine(
                Pens.Lime,
                new Point(5, 13),
                new Point(10, 2)
                );

            //thickness = 2, 60 degs, from top-right to bottom-left
            _composition.DrawLine(
                new Pen(Colors.Red, 2),
                new Point(26, 2),
                new Point(21, 13)
                );
        }


        private static void TestBackwardLines1()
        {
            //thickness = 1, -45 degs, from top-left to bottom-right
            _composition.DrawLine(
                Pens.Lime,
                new Point(2, 2),
                new Point(13, 13)
                );

            //thickness = 2, -45 degs, from bottom-right to top-left
            _composition.DrawLine(
                new Pen(Colors.Red, 2),
                new Point(21, 13),
                new Point(10, 2)
                );

            //thickness = 3, -45 degs, from top-left to bottom-right
            _composition.DrawLine(
                new Pen(Colors.Lime, 3),
                new Point(18, 2),
                new Point(29, 13)
                );
        }


        private static void TestBackwardLines2()
        {
            //thickness = 1, -30 degs, from top-left to bottom-right
            _composition.DrawLine(
                Pens.Lime,
                new Point(2, 5),
                new Point(13, 10)
                );

            //thickness = 2, -30 degs, from bottom-right to top-left
            _composition.DrawLine(
                new Pen(Colors.Red, 2),
                new Point(29, 10),
                new Point(18, 5)
                );
        }


        private static void TestBackwardLines3()
        {
            //thickness = 1, -60 degs, from top-left to bottom-right
            _composition.DrawLine(
                Pens.Lime,
                new Point(5, 2),
                new Point(10, 13)
                );

            //thickness = 2, -60 degs, from bottom-right to top-left
            _composition.DrawLine(
                new Pen(Colors.Red, 2),
                new Point(26, 13),
                new Point(21, 2)
                );
        }


        private static void TestLineClipping()
        {
            //horizontal
            _composition.DrawLine(
                Pens.Lime,
                new Point(-10, 3),
                new Point(50, 3)
                );

            //horizontal (very long)
            _composition.DrawLine(
                Pens.Lime,
                new Point(-100000, 6),
                new Point(500000, 6)
                );

            //vertical
            _composition.DrawLine(
                Pens.Yellow,
                new Point(3, -10),
                new Point(3, 30)
                );

            //vertical (very long)
            _composition.DrawLine(
                Pens.Yellow,
                new Point(6, -100000),
                new Point(6, 300000)
                );

            //forward
            _composition.DrawLine(
                Pens.Red,
                new Point(16 + 16, 8 - 32),
                new Point(16 - 16, 8 + 32)
                );

            //forward (very long)
            _composition.DrawLine(
                Pens.Red,
                new Point(24 + 16000, 8 - 32000),
                new Point(24 - 16000, 8 + 32000)
                );

            //backward
            _composition.DrawLine(
                Pens.Yellow,
                new Point(16 + 16, 8 + 32),
                new Point(16 - 16, 8 - 32)
                );

            //backward (very long)
            _composition.DrawLine(
                Pens.Yellow,
                new Point(24 + 16000, 8 + 32000),
                new Point(24 - 16000, 8 - 32000)
                );
        }


        private static void TestColors()
        {
            //draw a couple of rectangle frames as background,
            //so that the transparency will be easy to test
            var rect = new Rectangle(2, 2, 27, 11);
            _composition.DrawRectangle(
                Pens.Lime,
                rect);

            rect.Inflate(-4, -4);
            _composition.DrawRectangle(
                Pens.Lime,
                rect);

            //draw many colored blocks as much the colors are
            rect = new Rectangle(0, 0, 7, 3);

            //opaque colors
            _composition.FillRectangle(new SolidBrush(Colors.Black), rect);
            rect.Y += 4;
            _composition.FillRectangle(new SolidBrush(Colors.Blue), rect);
            rect.Y += 4;
            _composition.FillRectangle(new SolidBrush(Colors.Lime), rect);
            rect.Y += 4;
            _composition.FillRectangle(new SolidBrush(Colors.Aqua), rect);
            rect.X += 8;
            rect.Y = 0;
            _composition.FillRectangle(new SolidBrush(Colors.Red), rect);
            rect.Y += 4;
            _composition.FillRectangle(new SolidBrush(Colors.Fuchsia), rect);
            rect.Y += 4;
            _composition.FillRectangle(new SolidBrush(Colors.Yellow), rect);
            rect.Y += 4;
            _composition.FillRectangle(new SolidBrush(Colors.White), rect);
            rect.X += 8;
            rect.Y = 0;

            //transparent colors
            _composition.FillRectangle(new SolidBrush(Colors.Black & ~Colors.Opaque), rect);
            rect.Y += 4;
            _composition.FillRectangle(new SolidBrush(Colors.Blue & ~Colors.Opaque), rect);
            rect.Y += 4;
            _composition.FillRectangle(new SolidBrush(Colors.Lime & ~Colors.Opaque), rect);
            rect.Y += 4;
            _composition.FillRectangle(new SolidBrush(Colors.Aqua & ~Colors.Opaque), rect);
            rect.X += 8;
            rect.Y = 0;
            _composition.FillRectangle(new SolidBrush(Colors.Red & ~Colors.Opaque), rect);
            rect.Y += 4;
            _composition.FillRectangle(new SolidBrush(Colors.Fuchsia & ~Colors.Opaque), rect);
            rect.Y += 4;
            _composition.FillRectangle(new SolidBrush(Colors.Yellow & ~Colors.Opaque), rect);
            rect.Y += 4;
            _composition.FillRectangle(new SolidBrush(Colors.White & ~Colors.Opaque), rect);
        }


        private static void TestEmptyRectangles()
        {
            //thin pen
            _composition.DrawRectangle(
                Pens.Lime,
                new Rectangle(4, 2, 7, 11)
                );

            _composition.DrawRectangle(
                Pens.Yellow,
                new Rectangle(2, 4, 11, 7)
                );

            //pen thickness = 2
            _composition.DrawRectangle(
                new Pen(Colors.Yellow, 2),
                new Rectangle(18, 2, 7, 4)
                );

            //pen thickness = 3
            _composition.DrawRectangle(
                new Pen(Colors.Red, 3),
                new Rectangle(22, 7, 7, 6)
                );
        }


        private static void TestFilledRectangles()
        {
            _composition.FillRectangle(
                Brushes.Lime,
                new Rectangle(4, 2, 7, 11)
                );

            _composition.FillRectangle(
                Brushes.Yellow,
                new Rectangle(2, 4, 11, 7)
                );

            //draw a framed+filled square
            var rect = new Rectangle(18, 2, 11, 11);

            _composition.DrawRectangle(
                Pens.Yellow,
                rect
                );

            rect.Inflate(-1, -1);
            _composition.FillRectangle(
                Brushes.Red,
                rect
                );
        }


        private static void TestRectangleClipping()
        {
            //frame only, thickness = 1
            _composition.DrawRectangle(
                Pens.Lime,
                new Rectangle(-8, -6, 15, 11)
                );

            //filled
            _composition.FillRectangle(
                Brushes.Yellow,
                new Rectangle(-8, 10, 15, 11)
                );

            //filled (huge)
            _composition.FillRectangle(
                Brushes.Red,
                new Rectangle(16, -50000, 100000, 200000)
                );

            //frame only, thickness = 2
            _composition.DrawRectangle(
                Pens.Lime,
                new Rectangle(24, -6, 15, 11)
                );

            //frame only, thickness = 3
            _composition.DrawRectangle(
                Pens.Lime,
                new Rectangle(24, 10, 15, 11)
                );
        }


        private static void TestString()
        {
            //text left-aligned
            var s = "Abc";
            _composition.DrawString(
                s,
                Fonts.Fixed5x7,
                Brushes.Yellow,
                new Point(0, 0)
                );

            //text right-aligned
            s = "Xyz";
            Size sz = _composition.MeasureString(
                s,
                Fonts.Fixed5x7);

            _composition.DrawString(
                s,
                Fonts.Fixed5x7,
                Brushes.Red,
                new Point(32 - sz.Width, 8)
                );
        }


        private static void TestStringClipping()
        {
            //text centered
            var s = "Netduino";
            Size sz = _composition.MeasureString(
                s,
                Fonts.Fixed5x7);

            _composition.DrawString(
                s,
                Fonts.Fixed5x7,
                Brushes.Yellow,
                new Point(16 - sz.Width / 2, 0)
                );

            //text (very long)
            s = "Lorem ipsum dolor sit amet, consectetur adipisicing elit," +
                " sed do eiusmod tempor incididunt ut labore et dolore magna aliqua." +
                " Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris" +
                " nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in" +
                " reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur." +
                " Excepteur sint occaecat cupidatat non proident," +
                " sunt in culpa qui officia deserunt mollit anim id est laborum.";

            sz = _composition.MeasureString(
                s,
                Fonts.Fixed5x7);

            _composition.DrawString(
                s,
                Fonts.Fixed5x7,
                Brushes.Red,
                new Point(16 - sz.Width / 2, 8)
                );
        }

#if false
        private static void TestStaticDiamond()
        {
            _composition.DrawLine(new Point(0, 7), new Point(15, 0), Colors.Green);
            _composition.DrawLine(new Point(31, 7), new Point(16, 0), CompositionTarget.Red);
            _composition.DrawLine(new Point(0, 8), new Point(15, 15), CompositionTarget.Red);
            _composition.DrawLine(new Point(31, 8), new Point(16, 15), CompositionTarget.Green);

            _composition.DrawLine(new Point(-200, -100), new Point(200, 100), CompositionTarget.Yellow);
        }


        private static void TestStaticLines()
        {
            _composition.DrawLine(new Point(0, 15), new Point(7, 0), CompositionTarget.Green);
            _composition.DrawLine(new Point(15, 15), new Point(8, 0), CompositionTarget.Red);
            _composition.DrawLine(new Point(16, 0), new Point(23, 15), CompositionTarget.Yellow);
            _composition.DrawLine(new Point(31, 0), new Point(24, 15), CompositionTarget.Green);

            _composition.DrawLine(new Point(1, 1), new Point(4, 1), CompositionTarget.Red);
            _composition.DrawLine(new Point(14, 1), new Point(11, 1), CompositionTarget.Green);

            _composition.DrawLine(new Point(18, 11), new Point(18, 14), CompositionTarget.Red);
            _composition.DrawLine(new Point(29, 14), new Point(29, 11), CompositionTarget.Green);

            _composition.DrawLine(new Point(16, 8), new Point(16, 8), CompositionTarget.Yellow);
        }


        private static void TestStaticRectangles()
        {
            _composition.DrawRectangle(new Rect(1, 1, 6, 6), CompositionTarget.Green, CompositionTarget.Transparent);
            _composition.DrawRectangle(new Rect(9, 1, 6, 6), CompositionTarget.Transparent, CompositionTarget.Red);
            _composition.DrawRectangle(new Rect(1, 9, 6, 6), CompositionTarget.Green, CompositionTarget.Red);
            _composition.DrawRectangle(new Rect(9, 9, 6, 6), CompositionTarget.Red, CompositionTarget.Yellow, 2);

            _composition.DrawRectangle(new Rect(18, -10, 30, 16), CompositionTarget.Green, CompositionTarget.Red);

            _composition.DrawRectangle(new Rect(22, 8, 30, 16), CompositionTarget.Yellow, CompositionTarget.Green, 3);
            _composition.DrawRectangle(new Rect(18, 10, 20, 16), CompositionTarget.Transparent, CompositionTarget.Red, 2);
        }


        private static void TestStaticString()
        {
            //target.DrawRectangle(new Rect(8, 0, 16, 16), CompositionTarget.Green, CompositionTarget.Transparent);

            var s1 = "Happy";
            Size sz = target.MeasureString(s1);
            _composition.DrawString(s1, new Point(32 - sz.Width, -1), CompositionTarget.Green, CompositionTarget.Black);
            _composition.DrawString("new", new Point(0, 4), CompositionTarget.Yellow);

            var s2 = "year!";
            sz = _composition.MeasureString(s2);
            _composition.DrawString(s2, new Point(32 - sz.Width, 9), CompositionTarget.Red);
        }


        private static void TestDynamicRectangles()
        {
            if (_renderables == null)
            {
                var rnd = new Random();
                _renderables = new IRenderable[20];
                for (int i = 0; i < _renderables.Length; i++)
                    _renderables[i] = new Ball(rnd.Next());
            }

            _composition.Clear();

            for (int i = 0; i < _renderables.Length; i++)
            {
                _renderables[i].OnRender(_composition);
            }
        }


        private static void TestDynamicText()
        {
            if (_renderables == null)
            {
                var rnd = new Random();
                _renderables = new IRenderable[20];
                for (int i = 0; i < _renderables.Length; i++)
                    _renderables[i] = new Ball(rnd.Next());
            }

            _composition.Clear();

            for (int i = 0; i < _renderables.Length; i++)
            {
                _renderables[i].OnRender(_composition);
            }
        }
#endif
    }
}
