//#define V2

using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Threading;

/*
 * Copyright 2011,2012 Mario Vernari (http://netmftoolbox.codeplex.com/)
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

/**
 * Changes:
 * V2 - Feb 07, 2012:
 *  - added IsEnabled property (default true) (thanks to dvanderwekke);
 *  - exposed constants in place of the state enum (more lightweight)
 *  - exposed the standard "NativeEventHandler" in place of a custom event
 **/
namespace Toolbox.NETMF.Hardware
{
    /// <summary>
    /// Represent an extension over an input port, with embedded auto-repeat capabilities.
    /// This kind of class is well suited for pushbuttons input managements
    /// </summary>
    public class AutoRepeatInputPort
        : InputPort
    {
#if V2
        /// <summary>
        /// The button has just been pressed. This state is always issued, but once only
        /// </summary>
        private const int Press = 0;

        /// <summary>
        /// The button has been hold down enough to begin the auto-repeat cycle1
        /// This state can be issued periodically
        /// </summary>
        private const int Tick = 1;

        /// <summary>
        /// The button has just been depressed. This state is always issued, but once only
        /// </summary>
        private const int Release = 2;
#else
        /// <summary>
        /// Enumeration of the possible states issued by the <see cref="AutoRepeatInputPort.StateChanged"/> event
        /// </summary>
        /// <remarks>
        /// Each state is better depicted considering a pushbutton acting on the input port
        /// </remarks>
        public enum AutoRepeatState
        {
            /// <summary>
            /// The button has just been pressed. This state is always issued, but once only
            /// </summary>
            Press,

            /// <summary>
            /// The button has been hold down enough to begin the auto-repeat cycle1
            /// This state can be issued periodically
            /// </summary>
            Tick,

            /// <summary>
            /// The button has just been depressed. This state is always issued, but once only
            /// </summary>
            Release,
        }
#endif


        /// <summary>
        /// Indicates the duration of the quantum for the input port sampling,
        /// and all the related calculations
        /// </summary>
        /// <remarks>It is recommended to leave this value as is</remarks>
        private const int QuantumDuration = 100; //ms



        /// <summary>
        /// The initial delay used as default.
        /// </summary>
        private const int DefaultInitialDelay = 2000 / QuantumDuration;



        /// <summary>
        /// The auto-repeat period used as default
        /// </summary>
        private const int DefaultAutoRepeatPeriod = 200 / QuantumDuration;



        /// <summary>
        /// Create and open an instance of an input port,
        /// with embedded auto-repeat capabilities
        /// </summary>
        /// <param name="port">The I/O pin selected for the input</param>
        /// <param name="resistor">The resistor wired-logic easing</param>
        /// <param name="activeLevel">The level on which the input has to be considered active</param>
        public AutoRepeatInputPort(
            Cpu.Pin port,
            Port.ResistorMode resistor,
            bool activeLevel)
            : base(port, false, resistor)
        {
            this.ActiveLevel = activeLevel;

            //create, then start the working thread
            this._workingThread = new Thread(this.Worker);
            this._workingThread.Start();
        }



        private Thread _workingThread;
        private int _initialDelayCount = DefaultInitialDelay;
        private int _autoRepeatPeriodCount = DefaultAutoRepeatPeriod;
        private bool _shutdown;
        private bool _isEnabled = true;



        /// <summary>
        /// Gets the active level defined for this instance
        /// </summary>
        public bool ActiveLevel { get; private set; }


#if V2
        /// <summary>
        /// Gets/sets whether the input port should be listen to for changes
        /// </summary>
        public bool IsEnabled
        {
            get { return this._isEnabled; }
            set
            {
                if (this._isEnabled != value)
                {
                    this._isEnabled = value;

                    if (this._shutdown == false)
                    {
                        //switch the working thread accordingly
                        if (this._isEnabled)
                            this._workingThread.Resume();
                        else
                            this._workingThread.Suspend();
                    }
                }
            }
        }
#endif


        /// <summary>
        /// Get/set the initial delay before the auto-repeat starts. 
        /// The value is expressed in milliseconds, and is rounded accordingly to the quantum
        /// </summary>
        /// <remarks>
        /// The minimum allowed value is zero, that is an immediate starting of the auto-repeat
        /// </remarks>
        public int InitialDelay
        {
            get { return this._initialDelayCount * QuantumDuration; }
            set
            {
                this._initialDelayCount = value >= 0
                    ? value / QuantumDuration
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
            get { return this._autoRepeatPeriodCount * QuantumDuration; }
            set
            {
                this._autoRepeatPeriodCount = value >= QuantumDuration
                    ? value / QuantumDuration
                    : QuantumDuration;
            }
        }



        /// <summary>
        /// the working thread handler, as the manager of the auto-repeat
        /// </summary>
        private void Worker()
        {
            bool prevActivity = false;
            int counter = 0;

            while (this._shutdown == false)
            {
                //check the current level at the input port
                if (this.Read() == this.ActiveLevel)
                {
                    //activity
                    if (prevActivity)
                    {
                        //activity in progress
                        if (--counter <= 0)
                        {
                            this.OnStateChanged(AutoRepeatState.Tick);
                            counter = this._autoRepeatPeriodCount;
                        }
                    }
                    else
                    {
                        //just pressed
                        this.OnStateChanged(AutoRepeatState.Press);
                        prevActivity = true;
                        counter = (this._initialDelayCount > 0)
                            ? this._initialDelayCount
                            : this._autoRepeatPeriodCount;
                    }
                }
                else if (prevActivity)
                {
                    //just dropped into the inactivity
                    this.OnStateChanged(AutoRepeatState.Release);
                    counter = 0;
                    prevActivity = false;
                }

                //pause for the quantum duration
                Thread.Sleep(QuantumDuration);
            }
        }



        protected override void Dispose(bool disposing)
        {
#if V2
            //if disabled, then resumes the thread
            if (this._isEnabled == false)
                this._workingThread.Resume();
#endif

            //shut-down the thread gracefully
            this._shutdown = true;
            this._workingThread.Join();

            //dispose the underlying stuffs
            base.Dispose(disposing);
        }


#if V2
        #region EVT StateChanged

        /// <summary>
        /// Notify any change occurring in the auto-repeat life-cycle
        /// </summary>
        public event NativeEventHandler StateChanged;



        protected virtual void OnStateChanged(
            uint count,
            uint state)
        {
            var handler = this.StateChanged;

            if (handler != null)
            {
                handler(
                    ,
                    );
            }
        }

        #endregion
#else
        #region EVT StateChanged

        /// <summary>
        /// Notify any change occurring in the auto-repeat life-cycle
        /// </summary>
        public event AutoRepeatEventHandler StateChanged;



        protected virtual void OnStateChanged(AutoRepeatState state)
        {
            var handler = this.StateChanged;

            if (handler != null)
            {
                handler(
                    this,
                    new AutoRepeatEventArgs(state));
            }
        }

        #endregion
#endif
    }


#if !V2
    /// <summary>
    /// The delegate behind the <see cref="AutoRepeatInputPort.StateChanged"/> event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void AutoRepeatEventHandler(object sender, AutoRepeatEventArgs e);



    /// <summary>
    /// Extension wrapper to the standard <see cref="Microsoft.SPOT.EventArgs"/> object, thus the state of the auto-repeat may be carried out to the host
    /// </summary>
    public class AutoRepeatEventArgs
        : EventArgs
    {
        public AutoRepeatEventArgs(AutoRepeatInputPort.AutoRepeatState state)
        {
            this.State = state;
        }



        public AutoRepeatInputPort.AutoRepeatState State { get; private set; }
    }
#endif
}
