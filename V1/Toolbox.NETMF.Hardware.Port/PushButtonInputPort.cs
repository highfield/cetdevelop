using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
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
    /// Represent an extension over an input port, with embedded auto-repeat capabilities.
    /// This kind of class is well suited for pushbuttons input managements
    /// </summary>
    public class PushButtonInputPort
        : InputPort, IPortHeartbeat
    {
        /// <summary>
        /// The initial delay used as default.
        /// </summary>
        private const int DefaultInitialDelay = 2000 / PortHeartbeatBroker.ClockInterval;



        /// <summary>
        /// The auto-repeat period used as default
        /// </summary>
        private const int DefaultAutoRepeatPeriod = 200 / PortHeartbeatBroker.ClockInterval;



        /// <summary>
        /// Create and open an instance of an input port,
        /// with embedded auto-repeat capabilities
        /// </summary>
        /// <param name="port">The I/O pin selected for the input</param>
        /// <param name="resistor">The resistor wired-logic easing</param>
        /// <param name="activeLevel">The level on which the input has to be considered active</param>
        public PushButtonInputPort(
            Cpu.Pin port,
            Port.ResistorMode resistor,
            bool activeLevel)
            : base(port, false, resistor)
        {
            this.ActiveLevel = activeLevel;

            //consider the button enabled by default
            this.IsEnabled = true;

            //subscribe to the heartbeat provider
            PortHeartbeatBroker.Instance
                .Subscribe(this);
        }



        private int _initialDelayCount = DefaultInitialDelay;
        private int _autoRepeatPeriodCount = DefaultAutoRepeatPeriod;
        private bool _prevActivity;
        private int _counter;



        /// <summary>
        /// Gets the active level defined for this instance
        /// </summary>
        public bool ActiveLevel { get; private set; }



        /// <summary>
        /// Gets/sets whether the input port should be listen to for changes
        /// </summary>
        public bool IsEnabled { get; set; }



        /// <summary>
        /// Get/set the initial delay before the auto-repeat starts. 
        /// The value is expressed in milliseconds, and is rounded accordingly to the quantum
        /// </summary>
        /// <remarks>
        /// The minimum allowed value is zero, that is an immediate starting of the auto-repeat
        /// </remarks>
        public int InitialDelay
        {
            get { return this._initialDelayCount * PortHeartbeatBroker.ClockInterval; }
            set
            {
                this._initialDelayCount = value >= 0
                    ? value / PortHeartbeatBroker.ClockInterval
                    : 0;
            }
        }



        /// <summary>
        /// Get/set the interval period of the auto-repeat.
        /// The value is expressed in milliseconds, and is rounded accordingly to the quantum
        /// </summary>
        /// <remarks>
        /// The minimum value is equal to the quantum (i.e. 100ms)
        /// </remarks>
        public int AutoRepeatPeriod
        {
            get { return this._autoRepeatPeriodCount * PortHeartbeatBroker.ClockInterval; }
            set
            {
                this._autoRepeatPeriodCount = value >= PortHeartbeatBroker.ClockInterval
                    ? value / PortHeartbeatBroker.ClockInterval
                    : PortHeartbeatBroker.ClockInterval;
            }
        }



        /// <summary>
        /// the working thread handler, as the manager of the auto-repeat
        /// </summary>
        private void Worker()
        {
            //check the current level at the input port
            if (this.Read() == this.ActiveLevel)
            {
                //activity
                if (this._prevActivity)
                {
                    //activity in progress
                    if (--this._counter <= 0)
                    {
                        this.OnStatusChanged();
                        this._counter = this._autoRepeatPeriodCount;
                    }
                }
                else
                {
                    //just pressed
                    this.OnStatusChanged();
                    this._prevActivity = true;
                    this._counter = (this._initialDelayCount > 0)
                        ? this._initialDelayCount
                        : this._autoRepeatPeriodCount;
                }
            }
            else if (this._prevActivity)
            {
                //just dropped into the inactivity
                this.OnStatusChanged();
                this._counter = 0;
                this._prevActivity = false;
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



        #region EVT StatusChanged

        public event StatusChangedEventHandler StatusChanged;



        protected virtual void OnStatusChanged()
        {
            var handler = this.StatusChanged;

            if (handler != null)
            {
                handler(
                    this,
                    new StatusChangedEventArgs());
            }
        }

        #endregion



        #region IPortHeartbeat members

        bool IPortHeartbeat.IsEnabled
        {
            get { return this.IsEnabled; }
        }



        void IPortHeartbeat.Tick()
        {
            this.Worker();
        }

        #endregion
    }
}
