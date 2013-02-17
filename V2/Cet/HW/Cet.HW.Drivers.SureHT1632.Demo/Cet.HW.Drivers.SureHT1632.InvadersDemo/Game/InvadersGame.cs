using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
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
namespace Cet.HW.Drivers.Invaders
{
    public class InvadersGame
    {
        public InvadersGame(
            ICompositionRenderer renderer,
            GameContext context)
        {
            this._renderer = renderer;
            this._context = context;

            //creates the target composition instance
            this._composition = renderer.CreateTarget();

            //consider the intro stage first
            this._nextStage = new GameStageIntro(context);
        }


        private readonly ICompositionRenderer _renderer;
        private readonly GameContext _context;
        private readonly CompositionTargetBase _composition;

        private GameStageBase _currentStage;
        private GameStageBase _nextStage;


        public void Update()
        {
            //update the desired stage when requested
            if (this._nextStage != null)
            {
                this._currentStage = this._nextStage;
                this._currentStage.OnLoad(this._composition);
            }

            //wipes out the composition target
            this._composition.Clear();

            //compose the graphic scene
            this._nextStage = this._currentStage.OnCompose(this._composition);

            //dumps the composition to the physical device
            this._renderer.Dump(this._composition);
        }

    }
}
