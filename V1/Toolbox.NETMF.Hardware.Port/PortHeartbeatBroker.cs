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
    /// This class acts as a heartbeat-clock provider
    /// for any instance which needs it.
    /// </summary>
    internal sealed class PortHeartbeatBroker
    {
        internal const int ClockInterval = 100;  //ms
        private const int MaxSubscribers = 16;



        #region Singleton pattern

        private PortHeartbeatBroker()
        {
            //create the subscriptions array
            this._subscriptions = new IPortHeartbeat[MaxSubscribers];
        }

        private static PortHeartbeatBroker _instance;

        public static PortHeartbeatBroker Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new PortHeartbeatBroker();

                return _instance;
            }
        }

        #endregion



        private Timer _clock;
        private readonly object _semaphore = new object();
        private IPortHeartbeat[] _subscriptions;
        private int _optimizedCount;



        /// <summary>
        /// Gets/sets the master clock enabling.
        /// Not actually meant to be driven externally
        /// </summary>
        private bool IsEnabled
        {
            get { return this._clock != null; }
            set
            {
                if (this.IsEnabled != value)
                {
                    if (value)
                    {
                        //start the clock timer
                        this._clock = new Timer(
                            this.TimerTick,
                            null,
                            ClockInterval,
                            ClockInterval);
                    }
                    else
                    {
                        //stop, then dispose the timer
                        var clock = this._clock;
                        this._clock = null;
                        clock.Change(-1, -1);
                        clock.Dispose();
                    }
                }
            }
        }



        /// <summary>
        /// The timer handler
        /// </summary>
        /// <param name="state">(not used)</param>
        private void TimerTick(object state)
        {
            //prevent spurious callbacks, once the timer has been disposed
            if (this._clock == null)
                return;

            lock (this._semaphore)
            {
                //scan for all the subscriptions
                for (int i = 0; i < this._optimizedCount; i++)
                {
                    //check for a subscribed instance, enabled for notification
                    IPortHeartbeat subscription = this._subscriptions[i];
                    if (subscription != null &&
                        subscription.IsEnabled)
                    {
                        subscription.Tick();
                    }
                }
            }
        }



        /// <summary>
        /// Calculate the hightest count to be used
        /// for looping the subscription array
        /// </summary>
        private void RecalcCount()
        {
            lock (this._semaphore)
            {
                int optCount = 0;
                
                for (int i = MaxSubscribers - 1; i >= 0; i--)
                {
                    if (this._subscriptions[i] != null)
                    {
                        optCount = i + 1;
                        break;
                    }
                }

                this._optimizedCount = optCount;

                //enable the master clock only 
                //if there's something to manage
                this.IsEnabled = optCount > 0;
            }
        }



        /// <summary>
        /// Request a subscription for the specified instance
        /// </summary>
        /// <param name="instance">The instance asking for its subscription</param>
        /// <returns>True, if the subscription has been succeeded</returns>
        public bool Subscribe(IPortHeartbeat instance)
        {
            for (int i = 0; i < MaxSubscribers; i++)
            {
                if (this._subscriptions[i] == null)
                {
                    //a free cell was found: fill it
                    this._subscriptions[i] = instance;
                    this.RecalcCount();
                    return true;
                }
            }

            //no more free rooms
            return false;
        }



        /// <summary>
        /// Remove the subscription for the specified instance
        /// </summary>
        /// <param name="instance">The instance being unsubscribed</param>
        /// <returns>True, if the removal has been succeeded</returns>
        public bool Unsubscribe(IPortHeartbeat instance)
        {
            for (int i = 0; i < MaxSubscribers; i++)
            {
                if (this._subscriptions[i] == instance)
                {
                    //subscription found: remove it
                    this._subscriptions[i] = null;
                    this.RecalcCount();
                    return true;
                }
            }

            //no match was found
            return false;
        }

    }
}
