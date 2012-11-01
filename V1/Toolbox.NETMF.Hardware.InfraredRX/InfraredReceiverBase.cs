using System;
using Microsoft.SPOT;
using System.Threading;
using Microsoft.SPOT.Hardware;

/*
 * Copyright 2012 Mario Vernari (http://cetdevelop.codeplex.com/)
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
    /// This class acts as base for a generic-protocol infrared-receiver
    /// </summary>
    /// <remarks>
    /// The input trigger is assumed to be demodulated by any IR-module
    /// such as the follows: http://www.vishay.com/ir-receiver-modules/
    /// </remarks>
    public abstract class InfraredReceiverBase
        : IDisposable
    {
        /// <summary>
        /// The receiver is disabled, and any activity will be ignored
        /// </summary>
        public const int StatusDisabled = 0;

        /// <summary>
        /// The receiver is idle, waiting for the leading edge
        /// </summary>
        public const int StatusIdle = 1;

        /// <summary>
        /// The receiver is currently collecting the signal samples
        /// </summary>
        public const int StatusRunning = 2;

        /// <summary>
        /// The timer is out, and the message is considered complete
        /// </summary>
        public const int StatusComplete = 3;

        /// <summary>
        /// Some error has been found during the receiver activity
        /// </summary>
        public const int StatusError = 4;


        /// <summary>
        /// Constructor of the base class
        /// </summary>
        /// <param name="trigger">The input pin for sampling the signal</param>
        /// <param name="messageTimeout">The max duration of a message (millisecs)</param>
        /// <param name="maxBufferSize">The expected length of the message in bits</param>
        /// <remarks>
        /// The <see cref="maxBufferSize"/> parameter should be defined large enough
        /// to hold the actual signal *edges*, other than the logical bits.
        /// Keep this value a little larger than expected to avoid some message to be discarded.
        /// </remarks>
        protected InfraredReceiverBase(
            Cpu.Pin trigger,
            int messageTimeout,
            int maxBufferSize)
        {
            if (messageTimeout <= 0)
                throw new ArgumentOutOfRangeException("messageTimeout");

            if (maxBufferSize <= 0)
                throw new ArgumentOutOfRangeException("maxBufferSize");

            //define the interrupt on the given port,
            //so that any falling edge will be recorded
            this._triggerPort = new InterruptPort(
                trigger,
                true,
                Port.ResistorMode.PullUp,
                Port.InterruptMode.InterruptEdgeLow);

            this._triggerPort.DisableInterrupt();
            this._triggerPort.OnInterrupt += this.TriggerOnInterrupt;

            this.MessageTimeout = messageTimeout;
            this.MaxBufferSize = maxBufferSize;
            this._samples = new int[maxBufferSize];

            //create the timer for detecting whan a message is complete
            this._timer = new Timer(
                this.TimerHandler,
                null,
                Timeout.Infinite,
                Timeout.Infinite);
        }


        private InterruptPort _triggerPort;
        private Timer _timer;
        private long _oldTicks;
        private readonly int[] _samples;
        private int _sampleCount;

        public readonly int MaxBufferSize;
        public readonly int MessageTimeout;


        /// <summary>
        /// Gets the status of the receiver's state-machine
        /// </summary>
        public int Status { get; private set; }


        #region PROP IsEnabled

        private bool _isEnabled;

        /// <summary>
        /// Gets and sets the enabling of the receiver
        /// </summary>
        public bool IsEnabled
        {
            get { return this._isEnabled; }
            set
            {
                if (this._disposed)
                    value = false;

                if (this._isEnabled != value)
                {
                    this._isEnabled = value;

                    //sets the state machine accordingly
                    if (this.IsEnabled)
                    {
                        if (this.Status == InfraredReceiverBase.StatusDisabled)
                            this.Start();
                    }
                    else
                    {
                        this.Stop(InfraredReceiverBase.StatusDisabled);
                    }
                }
            }
        }

        #endregion


        /// <summary>
        /// (Re)starts the receiver, so that a new message can be listen
        /// </summary>
        private void Start()
        {
            this.Status = InfraredReceiverBase.StatusIdle;
            this._triggerPort.EnableInterrupt();
        }


        /// <summary>
        /// Stops the receiver, allowing the new machine state to be considered
        /// </summary>
        /// <param name="status">The new machine state</param>
        private void Stop(int status)
        {
            this.Status = status;
            this._triggerPort.DisableInterrupt();
            this._timer.Change(
                Timeout.Infinite,
                Timeout.Infinite);
        }


        /// <summary>
        /// The port interrupt's handler, called on any falling edge
        /// </summary>
        /// <param name="data1"></param>
        /// <param name="data2"></param>
        /// <param name="time"></param>
        void TriggerOnInterrupt(
            uint data1,
            uint data2,
            DateTime time)
        {
            //cache the current ticks-count
            long ticks = time.Ticks;

            switch (this.Status)
            {
                case InfraredReceiverBase.StatusIdle:
                    //here is the very first edge, so switch the machine to the
                    //"running" state
                    this.Status = InfraredReceiverBase.StatusRunning;
                    this._sampleCount = 0;

                    //starts the timer
                    this._timer.Change(
                        this.MessageTimeout,
                        Timeout.Infinite);
                    break;


                case InfraredReceiverBase.StatusRunning:
                    if (this._sampleCount <= this.MaxBufferSize)
                    {
                        //place the sampled delta-ticks in the current buffer's cells
                        this._samples[this._sampleCount++] = (int)(ticks - this._oldTicks);
                    }
                    else
                    {
                        //no more rooms to hold new data: switch to the "error" state
                        this._triggerPort.DisableInterrupt();
                        this.Status = InfraredReceiverBase.StatusError;
                    }
                    break;
            }

            this._oldTicks = ticks;
        }


        /// <summary>
        /// The timer event handler
        /// </summary>
        /// <param name="state">(not used)</param>
        private void TimerHandler(object state)
        {
            bool success = true;

            //discard any unexpected state
            switch (this.Status)
            {
                case InfraredReceiverBase.StatusDisabled:
                    return;

                case InfraredReceiverBase.StatusError:
                    success = false;
                    break;
            }

            //stops the receiver, yielding the decoder an attempt
            //to analyze the collected data
            this.Stop(InfraredReceiverBase.StatusComplete);

            if (success)
            {
                //invoke the actual decoder
                this.Decode(
                    this._samples,
                    this._sampleCount
                    );
            }

            //yields another new message, if enabled
            if (this.IsEnabled)
                this.Start();
        }


        /// <summary>
        /// Abstract function for the actual decoder implementation
        /// </summary>
        /// <param name="samples">The samples buffer containing the edges intervals</param>
        /// <param name="count">The number of actual edges collected</param>
        protected abstract void Decode(
            int[] samples,
            int count);


        #region EVT InfraredMessageDecoded

        /// <summary>
        /// The event used for notify the host of a valid decoded message
        /// </summary>
        public event InfraredMessageDecodedDelegate InfraredMessageDecoded;


        /// <summary>
        /// CLR-pattern for firing the event notification
        /// </summary>
        /// <param name="message"></param>
        protected virtual void OnInfraredMessageDecoded(object message)
        {
            var handler = this.InfraredMessageDecoded;

            if (handler != null)
            {
                var args = new InfraredMessageDecodedEventArgs();
                args.Message = message;

                handler(
                    this,
                    args);
            }
        }

        #endregion


        #region IDisposable Members

        private bool _disposed = false;


        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// Disposal of the receiver, as well as local resources
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    //disable the receiver
                    this.Status = InfraredReceiverBase.StatusDisabled;

                    //release the port
                    this._triggerPort.OnInterrupt -= this.TriggerOnInterrupt;
                    this._triggerPort.Dispose();
                    this._triggerPort = null;

                    //release the timer
                    this._timer.Change(
                        Timeout.Infinite,
                        Timeout.Infinite);
                    this._timer = null;
                }

                this._disposed = true;
            }
        }


        ~InfraredReceiverBase()
        {
            this.Dispose(false);
        }

        #endregion

    }
}
