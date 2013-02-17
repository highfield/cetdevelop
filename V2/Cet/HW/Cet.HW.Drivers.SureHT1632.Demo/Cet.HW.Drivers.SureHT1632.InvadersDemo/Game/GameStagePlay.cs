using System;
using Microsoft.SPOT;

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
namespace Cet.HW.Drivers.Invaders
{
    public class GameStagePlay
        : GameStageBase
    {
        public GameStagePlay(GameContext context)
            : base(context)
        {
            this._pauseStageHandlers[(int)PauseStage.ShipsLeft] = this.ComposePauseShipsLeft;
            this._pauseStageHandlers[(int)PauseStage.SaucerHit] = this.ComposePauseSaucerHit;
            this._pauseStageHandlers[(int)PauseStage.ShowLevel] = this.ComposePauseShowLevel;
            this._pauseStageHandlers[(int)PauseStage.GameOver] = this.ComposePauseGameOver;
        }


        public int ViewportRight;
        public int ViewportBottom;


        #region Paused staging declarations

        private delegate void PauseStageComposerDelegate(CompositionTargetBase target);


        private enum PauseStage
        {
            None,
            ShipsLeft,
            SaucerHit,
            ShowLevel,
            GameOver,
        }


        private const int PausedTimeLong = 40;
        private const int PausedTimeShort = PausedTimeLong / 2;

        private int _pausedClock;
        private PauseStage _pauseStage;
        private readonly PauseStageComposerDelegate[] _pauseStageHandlers = new PauseStageComposerDelegate[5];
        private Rectangle _rectAlert;

        #endregion


        #region Game featuring objects

        private const int BarriersCount = 3;
        private const int AliensPerRow = 5;
        private const int BombsCount = 4;
        private const int RenderableCount = BarriersCount + 1 + 1 + 1 + AliensPerRow + AliensPerRow + BombsCount;

        public readonly Barrier[] Barriers = new Barrier[BarriersCount];
        public readonly Ship Ship = new Ship();
        public readonly Missile Missile = new Missile();
        public readonly Saucer Saucer = new Saucer();
        public readonly AlienSmall[] SmallAliens = new AlienSmall[AliensPerRow];
        public readonly AlienLarge[] LargeAliens = new AlienLarge[AliensPerRow];
        public readonly Bomb[] Bombs = new Bomb[BombsCount];

        private readonly IRenderable[] _renderables = new IRenderable[RenderableCount];

        public AlienDirection CurrentAlienDir;
        public AlienDirection NextAlienDir;

        private int _aliveAliensCount;
        public bool ShiftAliens;
        private int _clockMask;

        #endregion


        public override void OnLoad(CompositionTargetBase target)
        {
            //create and init the play-stage context
            Size vp = target.ViewportSize;
            this.ViewportRight = vp.Width - 1;
            this.ViewportBottom = vp.Height - 1;

            this._rectAlert = new Rectangle(
                2,
                2,
                this.ViewportRight - 4,
                this.ViewportBottom - 4);

            this.Context.Status = GameStatus.Playing;

            //create barriers
            for (int i = 0; i < BarriersCount; i++)
            {
                Barrier barrier = this.Barriers[i] = new Barrier();
                barrier.XLeft = 3 + i * 10;
                barrier.YTop = 10;
            }

            //create aliens
            for (int i = 0; i < AliensPerRow; i++)
            {
                AlienBase la = this.LargeAliens[i] = new AlienLarge();
                AlienBase sa = this.SmallAliens[i] = new AlienSmall();
                sa.AlienBelow = la;
            }

            //create bombs
            for (int i = 0; i < BombsCount; i++)
            {
                this.Bombs[i] = new Bomb();
            }

            //collect renderables
            int rc = 0;
            for (int i = 0; i < BarriersCount; i++)
            {
                this._renderables[rc++] = this.Barriers[i];
            }

            this._renderables[rc++] = this.Ship;
            this._renderables[rc++] = this.Missile;
            this._renderables[rc++] = this.Saucer;

            for (int i = 0; i < AliensPerRow; i++)
            {
                this._renderables[rc++] = this.SmallAliens[i];
                this._renderables[rc++] = this.LargeAliens[i];
            }

            for (int i = 0; i < BombsCount; i++)
            {
                this._renderables[rc++] = this.Bombs[i];
            }

            //sets the number of ships available
            this.Context.ShipsLeft = 3;

            //reset the score
            this.Context.CurrentScore = 0;

            //reset the level counter
            this.Context.CurrentLevel = 1;

            this.RestartLevel();
        }


        public override GameStageBase OnCompose(CompositionTargetBase target)
        {
            GameStageBase result = null;

            if (this._pausedClock == 0)
            {
                //here is the normal flow during the playable game
                this.ShiftAliens = (this.Context.Clock & this._clockMask) == 0;

                //update objects
                this.Ship.Update(this);
                this.Missile.Update(this);
                this.Saucer.Update(this);

                for (int i = 0; i < AliensPerRow; i++)
                {
                    this.SmallAliens[i].Update(this);
                    this.LargeAliens[i].Update(this);
                }

                for (int i = 0; i < BombsCount; i++)
                {
                    this.Bombs[i].Update(this);
                }

                //check collisions
                if (this.Ship.DetectCollision(this))
                {
                }

                if (this.Saucer.DetectCollision(this))
                {
                }

                for (int i = 0; i < AliensPerRow; i++)
                {
                    if (this.SmallAliens[i].DetectCollision(this))
                        break;

                    if (this.LargeAliens[i].DetectCollision(this))
                        break;
                }

                for (int i = 0; i < BarriersCount; i++)
                {
                    if (this.Barriers[i].DetectCollision(this))
                        break;
                }

                //update context for next frame
                this.CurrentAlienDir = this.NextAlienDir;

                if (this.CurrentAlienDir == AlienDirection.GroundReached)
                {
                    //aliens grounded: the game is over!
                    this._pausedClock = PausedTimeLong;
                    this._pauseStage = PauseStage.GameOver;
                }
            }
            //this happens when some event has paused the game
            else if (--this._pausedClock == 0)
            {
                //finalize the paused stage
                switch (this._pauseStage)
                {
                    case PauseStage.ShipsLeft:
                        //resume the game where was broken
                        this.ResumeLevel();
                        break;

                    case PauseStage.GameOver:
                        //back to the intro stage
                        result = new GameStageIntro(this.Context);
                        this.Context.Status = GameStatus.Registering;
                        break;
                }

                this._pauseStage = PauseStage.None;
            }

            //render the renderables
            for (int i = 0; i < RenderableCount; i++)
            {
                this._renderables[i].OnRender(target);
            }

            //when paused, compose the required stage
            if (this._pauseStage != PauseStage.None)
            {
                this._pauseStageHandlers[(int)this._pauseStage](target);
            }

            return result;
        }


        private void ComposePauseShipsLeft(CompositionTargetBase target)
        {
            //draw a big bordered box
            target.FillRectangle(
                Brushes.Black,
                this._rectAlert);

            target.DrawRectangle(
                Pens.Lime,
                this._rectAlert);

            //draw the ship symbol
            target.DrawString(
                "JK",   //ship
                AlienFont.Instance,
                Brushes.Lime,
                this._rectAlert.Left,
                this._rectAlert.Top + 2);

            //draw the number of ships left
            string text = this.Context.ShipsLeft.ToString();
            Size sz = target.MeasureString(text, Fonts.Fixed5x7);

            target.DrawString(
                text,
                Fonts.Fixed5x7,
                Brushes.Yellow,
                this._rectAlert.Right - 2 - sz.Width,
                this._rectAlert.Top + 2);
        }


        private void ComposePauseSaucerHit(CompositionTargetBase target)
        {
            //draw a big bordered box
            target.FillRectangle(
                Brushes.Black,
                this._rectAlert);

            target.DrawRectangle(
                Pens.Lime,
                this._rectAlert);

            //draw the ship symbol
            target.DrawString(
                "LM",   //saucer
                AlienFont.Instance,
                Brushes.Lime,
                this._rectAlert.Left,
                this._rectAlert.Top + 2);

            //draw the number of ships left
            string text = this.Saucer.PromisedScore.ToString();
            Size sz = target.MeasureString(text, Fonts.Fixed5x7);

            target.DrawString(
                text,
                Fonts.Fixed5x7,
                Brushes.Yellow,
                this._rectAlert.Right - 2 - sz.Width,
                this._rectAlert.Top + 2);
        }


        private void ComposePauseShowLevel(CompositionTargetBase target)
        {
            //draw a big bordered box
            target.FillRectangle(
                Brushes.Black,
                this._rectAlert);

            target.DrawRectangle(
                Pens.Lime,
                this._rectAlert);

            //draw a "Level" abbreviation
            target.DrawString(
                "Lv",
                Fonts.Fixed5x7,
                Brushes.Lime,
                this._rectAlert.Left + 2,
                this._rectAlert.Top + 2);

            //draw the number of the current level
            string text = this.Context.CurrentLevel.ToString();
            Size sz = target.MeasureString(text, Fonts.Fixed5x7);

            target.DrawString(
                text,
                Fonts.Fixed5x7,
                Brushes.Yellow,
                this._rectAlert.Right - 2 - sz.Width,
                this._rectAlert.Top + 2);
        }


        private void ComposePauseGameOver(CompositionTargetBase target)
        {
            //draw a big bordered box
            target.FillRectangle(
                Brushes.Black,
                this._rectAlert);

            target.DrawRectangle(
                Pens.Lime,
                this._rectAlert);

            //draw "game" then "over"
            var text = (this._pausedClock > PausedTimeShort)
                ? "GAME"
                : "OVER";

            target.DrawString(
                text,
                Fonts.Fixed5x7,
                Brushes.Red,
                this._rectAlert.Left + 2,
                this._rectAlert.Top + 2);
        }


        private void RestartLevel()
        {
            //init barriers
            for (int i = 0; i < BarriersCount; i++)
            {
                this.Barriers[i].BricksAlive = Barrier.AllBricks;
            }

            //init aliens
            this._aliveAliensCount = AliensPerRow + AliensPerRow;

            for (int i = 0; i < AliensPerRow; i++)
            {
                AlienBase alien = this.LargeAliens[i];
                alien.IsAlive = true;
                alien.Status = AlienStatus.Walk1;
                alien.XCenter = 4 + i * 5;
                alien.YCenter = 5;

                alien = this.SmallAliens[i];
                alien.IsAlive = true;
                alien.Status = AlienStatus.Walk1;
                alien.XCenter = 4 + i * 5;
                alien.YCenter = 2;
            }

            //reset aliens' speed to the slowest
            this._clockMask = 0x0F;

            //do init the remaining parameters
            this.ResumeLevel();

            //shows the current level
            this._pausedClock = PausedTimeLong;
            this._pauseStage = PauseStage.ShowLevel;
        }


        private void ResumeLevel()
        {
            //set the ship position at center
            this.Ship.XCenter = this.ViewportRight / 2;
            this.Ship.YCenter = this.ViewportBottom;

            //deactivate missile
            this.Missile.IsActive = false;

            //deactivate saucer
            this.Saucer.Status = SaucerStatus.Inactive;

            //deactivate bombs
            for (int i = 0; i < BombsCount; i++)
            {
                this.Bombs[i].IsActive = false;
            }
        }


        public Bomb GetBombSlot()
        {
            for (int i = this.Bombs.Length - 1; i >= 0; i--)
            {
                Bomb bomb;
                if ((bomb = this.Bombs[i]).IsActive == false)
                    return bomb;
            }

            //no free slots
            return null;
        }


        public void DecreaseAliens()
        {
            this._aliveAliensCount--;

            switch (this._aliveAliensCount)
            {
                case 5:
                case 2:
                    //increase aliens' speed upon the number of alives
                    this._clockMask >>= 1;
                    break;


                case 0:
                    //gain a level more
                    this.Context.CurrentLevel++;
                    this.RestartLevel();
                    break;
            }
        }


        public bool DecreaseShips()
        {
            this._pausedClock = PausedTimeLong;

            //any ship left?
            if (--this.Context.ShipsLeft > 0)
            {
                //resume game
                this._pauseStage = PauseStage.ShipsLeft;
                return true;
            }
            else
            {
                //no more ships: the game is over
                this._pauseStage = PauseStage.GameOver;
                return false;
            }
        }


        public void ShowSaucerWorth()
        {
            this._pausedClock = PausedTimeShort;
            this._pauseStage = PauseStage.SaucerHit;
        }

    }

#if false
    public class PlayStageContext
    {
        public const int AliensPerRow = 5;
        public const int BombsCount = 4;
        public const int RenderableCount = 1 + 1 + AliensPerRow + AliensPerRow + BombsCount;


        public PlayStageContext(GameStageContext context)
        {
            this.GameContext = context;
            this.Rnd = new Random(context.GetHashCode());
            Size vp = context.Target.ViewportSize;
            this.ViewportRight = vp.Width - 1;
            this.ViewportBottom = vp.Height - 1;
            this.AliveAliensCount = AliensPerRow + AliensPerRow;

            //create and init aliens
            for (int i = 0; i < AliensPerRow; i++)
            {
                AlienBase la = this.LargeAliens[i] = new AlienLarge();
                la.XCenter = 4 + i * 5;
                la.YCenter = 5;

                AlienBase sa = this.SmallAliens[i] = new AlienSmall();
                sa.XCenter = 4 + i * 5;
                sa.YCenter = 2;
                sa.AlienBelow = la;
            }

            //create and init bombs
            for (int i = 0; i < BombsCount; i++)
            {
                this.Bombs[i] = new Bomb();
            }
        }


        public readonly GameStageContext GameContext;
        public readonly Random Rnd;

        public readonly Ship Ship = new Ship();
        public readonly Missile Missile = new Missile();
        public readonly AlienSmall[] SmallAliens = new AlienSmall[AliensPerRow];
        public readonly AlienLarge[] LargeAliens = new AlienLarge[AliensPerRow];
        public readonly Bomb[] Bombs = new Bomb[BombsCount];

        public readonly int ViewportRight;
        public readonly int ViewportBottom;

        public AlienDirection CurrentAlienDir;
        public AlienDirection NextAlienDir;

        public int AliveAliensCount;
        public bool ShiftAliens;
        private int ClockMask = 0x0F;

        public int ShipsLeft = 3;


        public void Restart()
        {
            //set the ship position at center
            this.Ship.XCenter = this.ViewportRight / 2;
            this.Ship.YCenter = this.ViewportBottom;

            //deactivate missile and bombs
            this.Missile.IsActive = false;

            for (int i = 0; i < BombsCount; i++)
            {
                this.Bombs[i].IsActive = false;
            }
        }


        public void Update()
        {
        }


        public Bomb GetBombSlot()
        {
            for (int i = this.Bombs.Length - 1; i >= 0; i--)
            {
                Bomb bomb;
                if ((bomb = this.Bombs[i]).IsActive == false)
                    return bomb;
            }

            //no free slots
            return null;
        }


        public void DecreaseAliens()
        {
            this.AliveAliensCount--;

            //increase aliens' speed upon the surviveds
            switch (this.AliveAliensCount)
            {
                case 5:
                case 2:
                    this.ClockMask >>= 1;
                    break;
            }
        }


        public void DecreaseShips()
        {
        }

    }
#endif
}
