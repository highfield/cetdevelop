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
    /// A demo of a simple pong-like game
    /// </summary>
    /// <remarks>
    /// The aim of the demo is demonstrating how simpler is
    /// creating even pretty complex games, by taking advantage
    /// of the declarative approach, instead of the imperative one.
    /// 
    /// The paddle is controlled by any numeric port
    /// 
    /// NOTE: although this demo can work on any lcd size,
    /// has been tailored for a 20x4 char matrix LCD module
    /// </remarks>
    class DemoPong
    {
        private const int BrickRows = 2;
        private const int BrickColumns = 16;
        private const int TotalBricks = BrickRows * BrickColumns;

        private const int AvailablePaddles = 5;

        private static LcdBoostElementCharPoint[][] _elem_bricks;
        private static LcdBoostElementTextArea _elem_paddle;
        private static LcdBoostElementTextArea _elem_score;
        private static LcdBoostElementTextArea _elem_paddles_left;
        private static LcdBoostElementCharPoint _elem_ball;
        private static LcdBoostElementTextArea _elem_gameover;

        private static Random _rnd;


        public static void Run(
            LcdBoostProxy lcd,
            NumericInput paddle)
        {
            //init the random number generator using a pseudo random seed
            _rnd = new Random(DateTime.Now.Millisecond);

            //define an array for containing all the UI-elements
            var elements = new ILcdBoostElement[TotalBricks + 6];
            var elem_count = 0;

            /**
             * Allocates the resources to be used in the program
             * this is very similar to the "sprite" approach
             **/

            //right-side wall
            var wall = new LcdBoostElementTextArea();
            wall.Text = "||||";
            wall.Bounds.Left = 16;
            wall.Bounds.Top = 0;
            wall.Bounds.Width = 1;
            wall.Bounds.Height = 4;
            elements[elem_count++] = wall;


            //number of paddles left
            _elem_paddles_left = new LcdBoostElementTextArea();
            _elem_paddles_left.Bounds.Left = 17;
            _elem_paddles_left.Bounds.Top = 3;
            _elem_paddles_left.Bounds.Width = 3;
            _elem_paddles_left.Bounds.Height = 1;
            elements[elem_count++] = _elem_paddles_left;


            //game over indication
            _elem_gameover = new LcdBoostElementTextArea();
            _elem_gameover.Text = "-= GAME OVER =-";
            _elem_gameover.IsHidden = true;
            _elem_gameover.Bounds.Left = 0;
            _elem_gameover.Bounds.Top = 2;
            _elem_gameover.Bounds.Width = _elem_gameover.Text.Length;
            _elem_gameover.Bounds.Height = 1;
            elements[elem_count++] = _elem_gameover;


            //bricks walls
            _elem_bricks = new LcdBoostElementCharPoint[BrickRows][];

            for (int y = 0; y < BrickRows; y++)
            {
                _elem_bricks[y] = new LcdBoostElementCharPoint[BrickColumns];

                for (int x = 0; x < BrickColumns; x++)
                {
                    var brick = new LcdBoostElementCharPoint();
                    brick.Face = '=';
                    brick.X = x;
                    brick.Y = y;
                    _elem_bricks[y][x] = brick;
                    elements[elem_count++] = brick;
                }
            }


            //gamer's (moving) paddle
            _elem_paddle = new LcdBoostElementTextArea();
            _elem_paddle.Text = "____";
            _elem_paddle.Bounds.Left = 8;
            _elem_paddle.Bounds.Top = 3;
            _elem_paddle.Bounds.Width = 4;
            _elem_paddle.Bounds.Height = 1;
            elements[elem_count++] = _elem_paddle;


            //controller for the paddle position
            paddle.Value = _elem_paddle.Bounds.Left;
            paddle.SetRange(
                minValue: 0,
                maxValue: lcd.Driver.Width - 4 - _elem_paddle.Bounds.Width
                );
            paddle.StatusChanged += (s_, e_) =>
            {
                _elem_paddle.Bounds.Left = paddle.Value;
            };


            //score indication
            _elem_score = new LcdBoostElementTextArea();
            _elem_score.Text = ":";
            _elem_score.Bounds.Left = 17;
            _elem_score.Bounds.Top = 0;
            _elem_score.Bounds.Width = 3;
            _elem_score.Bounds.Height = 1;
            elements[elem_count++] = _elem_score;


            //ball
            _elem_ball = new LcdBoostElementCharPoint();
            _elem_ball.Face = 'o';
            elements[elem_count++] = _elem_ball;


            /**
             * Create a new session of game.
             * The session contains only the dynamic data used
             * by the game. The logic is meant to be outside.
             * That's much like a functional approach
             **/
            var session = new GameSession();
            session.NewBall();


            /**
             * Master clock for the game refresh
             * 
             * Notice that the clock is handling the display refresh
             * at 10 fps, but the game logic has to be slowed down
             * because would be too fast to play
             **/
            int clock_counter = 0;
            var clock = new Timer(
                _ =>
                {
                    //invoke the game logic
                    Worker(
                        session,
                        clock_counter++
                        );

                    //refresh display
                    lcd.Dump(elements);
                },
                null,
                100,
                100);


            Thread.Sleep(Timeout.Infinite);
        }


        /// <summary>
        /// The game logic
        /// </summary>
        /// <param name="session"></param>
        /// <param name="clock_counter"></param>
        private static void Worker(
            GameSession session,
            int clock_counter)
        {
            //stop logic when there'are no more paddles left
            if (session.PaddlesLeft <= 0)
                return;

            //timer scaling to slow down the game logic
            if ((clock_counter & 0x07) != 0)
                return;

            //retrieve the current ball coords
            int x = _elem_ball.X;
            int y = _elem_ball.Y;

            //check for brick collision
            if (y < BrickRows &&
                session.BallSpeed.DY < 0)
            {
                var brick = _elem_bricks[y][x];
                if (brick.IsHidden == false)
                {
                    //brick hit
                    brick.IsHidden = true;

                    //increment score
                    _elem_score.Text = (++session.Score).ToString();

                    //invert vertical ball speed
                    session.BallSpeed.InvertY();
                }
            }

            //check for paddle collision
            if (y == 3)
            {
                if (_elem_paddle.Bounds.Contains(x, y))
                {
                    //paddle hit
                    session.BallSpeed.InvertY();
                }
                else
                {
                    //ball lost
                    session.PaddlesLeft--;
                    if (session.PaddlesLeft == 0)
                    {
                        //no more paddles left, thus the game is over!
                        _elem_gameover.IsHidden = false;
                    }
                    else
                    {
                        //offer a new ball
                        session.NewBall();
                    }
                    return;
                }
            }

            //propose next ball's position
            x += session.BallSpeed.DX;
            y += session.BallSpeed.DY;

            //check game field's bounds
            if (x < 0)
            {
                x = 0;
                session.BallSpeed.InvertX();
            }
            else if (x > 15)
            {
                x = 15;
                session.BallSpeed.InvertX();
            }

            if (y < 0)
            {
                y = 0;
                session.BallSpeed.InvertY();
            }
            else if (y > 3)
            {
                y = 3;
                session.BallSpeed.InvertY();
            }

            //sets the actual ball's coords
            _elem_ball.X = x;
            _elem_ball.Y = y;
        }


        /// <summary>
        /// 
        /// </summary>
        private class GameSession
        {
            /// <summary>
            /// Overall score
            /// </summary>
            public int Score;

            /// <summary>
            /// Paddles left
            /// </summary>
            public int PaddlesLeft = AvailablePaddles;

            /// <summary>
            /// 2D vector defining the ball speed
            /// </summary>
            public readonly Vector BallSpeed = new Vector();


            /// <summary>
            /// Init for a new incoming ball
            /// </summary>
            public void NewBall()
            {
                int n = _rnd.Next(10000);

                //gets the new ball position
                _elem_ball.X = 2 + n % 12;
                _elem_ball.Y = 0;

                //gets the new ball speed
                this.BallSpeed.DX = n < 5000 ? -1 : 1;
                this.BallSpeed.DY = 1;

                //update the paddles left box
                _elem_paddles_left.Text = "_" + this.PaddlesLeft.ToString() + "_";
            }
        }


        /// <summary>
        /// Simple class for defining a 2D vector
        /// </summary>
        private class Vector
        {
            public int DX;
            public int DY;

            public void InvertX()
            {
                this.DX = -this.DX;
            }

            public void InvertY()
            {
                this.DY = -this.DY;
            }
        }

    }
}
