using System;
using Microsoft.SPOT;

using Toolbox.NETMF.Hardware;


namespace NetduinoSpiBoost
{
    class TestImperative : TestBase
    {
        public TestImperative() : base(3000) { }

        protected override void Run(LcdBoostProxy lcd, bool firstPass)
        {
            if (firstPass == false)
                return;

            lcd.Clear();
            lcd.SetCursorPosition(3, 0);
            lcd.Write("Happy new year!" + (char)0x01);

            lcd.SetCursorPosition(2, 1);
            lcd.Write("Buon anno nuovo!" + (char)0x01);

            lcd.SetCursorPosition(1, 2);
            lcd.Write("Frohe neues Jahr!" + (char)0x01);

            lcd.SetCursorPosition(0, 3);
            lcd.Write("Gelukkig Nieuwjaar!" + (char)0x01);

            lcd.Dump();
        }
    }


    class TestBoxXY : TestBase
    {
        public TestBoxXY() : base(10000) { }

        ILcdBoostElement[] _elements = new ILcdBoostElement[1];
        LcdBoostElementTextArea _te;
        float _x, _y;
        float _xs, _ys;

        protected override void Run(LcdBoostProxy lcd, bool firstPass)
        {
            if (firstPass)
            {
                _elements[0] = _te = new LcdBoostElementTextArea();
                _te.Text = "This is a very very long text, which has to be wrapped inside the display.";
                _te.Width = 20;
                _te.Height = 4;

                var rnd = new System.Random();
                _xs = (rnd.Next(10) + 3) / 10f;
                _ys = (rnd.Next(10) + 3) / 10f;
            }

            lcd.Dump(_elements);

            _x += _xs;
            if (_x < -10f || _x > 10f)
                _xs = -_xs;

            _y += _ys;
            if (_y < -5f || _y > 5f)
                _ys = -_ys;

            _te.Left = (int)_x;
            _te.Top = (int)_y;
        }
    }


    class TestBoxWH : TestBase
    {
        public TestBoxWH() : base(15000) { }

        ILcdBoostElement[] _elements = new ILcdBoostElement[1];
        LcdBoostElementTextArea _te;
        int _x, _y;
        int _w, _h;
        int _phase;

        protected override void Run(LcdBoostProxy lcd, bool firstPass)
        {
            if (firstPass)
            {
                _elements[0] = _te = new LcdBoostElementTextArea();
                _te.Text = "The PS/2 mouse interface utilizes a bidirectional serial protocol to transmit movement and button-state data to the computer's auxiliary device controller (part of the keyboard controller).";
                _w = 20;
                _h = 4;
            }

            _te.Left = (int)_x;
            _te.Top = (int)_y;
            _te.Width = 20;
            _te.Height = 4;

            lcd.Dump(_elements);

            switch (_phase)
            {
                case 0: //shrink-H
                    if (_w > 10)
                    {
                        _x++;
                        _w--;
                    }
                    else
                        _phase = 1;
                    break;

                case 1: //expand-V
                    if (_h < 10)
                        _h++;
                    else
                        _phase = 2;
                    break;

                case 2: //scroll-V
                    if (_y < 10)
                        _y++;
                    else
                        _phase = 3;
                    break;
            }
        }
    }
}
