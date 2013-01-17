using System;
using System.Threading;
using System.IO.Ports;

using Cet.IO.Protocols;

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
namespace Cet.IO.Serial
{
    /// <summary>
    /// Concrete implementation of a server serial port
    /// </summary>
    internal class SerialPortServer
        : ICommServer
    {
        public SerialPortServer(
            ISerialPort port,
            IProtocol protocol)
        {
            this.Port = port;
            this.Protocol = protocol;
        }


        private const int CacheSize = 300;

        public readonly ISerialPort Port;
        public readonly IProtocol Protocol;

        internal int IdleTimeout = 10;

        private Thread _thread;
        protected bool _closing;


        /// <summary>
        /// Indicate whether the server is running
        /// </summary>
        public bool IsRunning { get; private set; }


        /// <summary>
        /// Start the listener session
        /// </summary>
        public void Start()
        {
            //marks the server running
            this.IsRunning = true;

            this._thread = new Thread(this.Worker);
            this._thread.Start();
        }


        /// <summary>
        /// Running thread handler
        /// </summary>
        private void Worker()
        {
#if NET45
            System.Diagnostics.Trace.WriteLine("start");
#elif MF_FRAMEWORK_VERSION_V4_1 || MF_FRAMEWORK_VERSION_V4_2
            Microsoft.SPOT.Debug.Print("start");
#endif
            //start the local timer, which gets the session dying
            int counter = IdleTimeout;
            int grace = 0;

            //create a writer for the incoming data
            ByteArrayWriter writer = null;
            var buffer = new byte[CacheSize];

            using (Timer timer = new Timer(
                _ => 
                { 
                    counter--; 
                    if (--grace == 0) 
                        writer = null; 
                },
                state: null,
                dueTime: 1000,
                period: 1000))
            {
                //loop until the host closes, or the timer expires
                while (
                    this.Port.IsOpen &&
                    this._closing == false &&
                    counter > 0)
                {
                    //look for incoming data
                    int length = this.Port.BytesToRead;

                    if (length > 0)
                    {
                        grace = 2;
                        if (length > CacheSize)
                            length = CacheSize;

                        //read the data from the physical port
                        this.Port.Read(
                            buffer,
                            0,
                            length);

                        //append the data to the writer
                        if (writer == null)
                            writer = new ByteArrayWriter();

                        writer.WriteBytes(
                            buffer,
                            0,
                            length);

                        //try to decode the incoming data
                        var data = new ServerCommData(this.Protocol);
                        data.IncomingData = writer.ToReader();
#if NET45
                        System.Diagnostics.Trace.WriteLine("RX: " + ByteArrayHelpers.ToHex(((IByteArray)data.IncomingData).Data));
#elif MF_FRAMEWORK_VERSION_V4_1 || MF_FRAMEWORK_VERSION_V4_2
                        Microsoft.SPOT.Debug.Print("RX: " + ByteArrayHelpers.ToHex(((IByteArray)data.IncomingData).Data));
#endif

                        CommResponse result = this.Protocol
                            .Codec
                            .ServerDecode(data);

                        if (result.Status == CommResponse.Ack)
                        {
                            //the command is recognized, so call the host back
                            this.OnServeCommand(data);

                            //encode the host data
                            this.Protocol
                                .Codec
                                .ServerEncode(data);

                            //return the resulting data to the remote caller
                            byte[] outgoing = ((IByteArray)data.OutgoingData).Data;

                            if (this.Port.IsOpen)
                            {
                                this.Port.Write(
                                    outgoing,
                                    0,
                                    outgoing.Length);
                            }
                            else
                            {
                                break;
                            }

                            //reset the timer
                            counter = IdleTimeout;
                            writer = null;
                        }
                        //else if (result.Status == CommResponse.Ignore)
                        //{
                        //    writer = null;
                        //}
                    }

                    Thread.Sleep(0);
                }
            }

#if NET45
            System.Diagnostics.Trace.WriteLine("close");
#elif MF_FRAMEWORK_VERSION_V4_1 || MF_FRAMEWORK_VERSION_V4_2
            Microsoft.SPOT.Debug.Print("close");
#endif

            //marks the server not running
            this.IsRunning = false;
        }


        /// <summary>
        /// Close/abort the listener session
        /// </summary>
        public void Abort()
        {
            this._closing = true;

            if (this._thread != null &&
                this._thread.IsAlive)
            {
                this._thread.Join();
            }
        }


        #region EVT ServeCommand

        public event ServeCommandHandler ServeCommand;


        protected virtual void OnServeCommand(ServerCommData data)
        {
            var handler = this.ServeCommand;

            if (handler != null)
            {
                handler(
                    this,
                    new ServeCommandEventArgs(data));
            }
        }

        #endregion

    }
}
