using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Threading;

using SecretLabs.NETMF.Hardware;

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
    /// Allow to connect a normal brushed-motor, and use it as a rotary encoder
    /// </summary>
    public class DynamoEncoder
        : NumericInput, IPortHeartbeat
    {
        /// <summary>
        /// The absolute delta above which the sampler should
        /// adjust the public value
        /// </summary>
        private const int OuterThreshold = 30;



        /// <summary>
        /// Create a new instance of <see cref="DynamoEncoder"/>
        /// </summary>
        /// <param name="a">The analog port to be used as motor input</param>
        public DynamoEncoder(Cpu.Pin a)
        {
            //instantiate the phase-A input
            this._input = new AnalogInput(a);

            //backup the current input status
            this._steadyLevel = 0x0200;
            this._steadyDelay = 15;

            //subscribe to the heartbeat provider
            PortHeartbeatBroker.Instance
                .Subscribe(this);
        }



        private AnalogInput _input;
        private int _steadyDelay;
        private float _steadyLevel;



        /// <summary>
        /// The sampler of the analog input
        /// </summary>
        private void Sampler()
        {
            //read the current analog input
            int level = this._input.Read();

            //adjust the steady level
            this._steadyLevel = 0.9f * this._steadyLevel + 0.1f * level;

            if (--this._steadyDelay <= 0)
            {
                //calculates the relative delta between the read
                //and the steady level
                float delta = (level - this._steadyLevel) / OuterThreshold;

                if (delta > 1.0f ||
                    delta < -1.0f)
                {
                    //adjust the public value
                    this.Value = (int)(this.Value + delta);
                }
            }
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //remove the subscription
                PortHeartbeatBroker.Instance
                    .Unsubscribe(this);
            }

            //dispose the underlying stuffs
            base.Dispose(disposing);
        }



        #region IPortHeartbeat members

        bool IPortHeartbeat.IsEnabled
        {
            get { return true; }
        }



        void IPortHeartbeat.Tick()
        {
            this.Sampler();
        }

        #endregion
    }
}
