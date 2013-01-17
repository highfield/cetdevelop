using System.Threading;
using System.Net.Sockets;

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
namespace Cet.IO.Net
{
    /// <summary>
    /// Concrete implementation of a TCP-listener
    /// </summary>
    internal class TcpServer
        : IpServer
    {
        public TcpServer(
            Socket port,
            IProtocol protocol)
            : base(port, protocol)
        {
        }


        private const int CacheSize = 300;

        internal int IdleTimeout = 10;


        /// <summary>
        /// Running thread handler
        /// </summary>
        protected override void Worker()
        {
#if NET45
            System.Diagnostics.Trace.WriteLine("start");
#elif MF_FRAMEWORK_VERSION_V4_1 || MF_FRAMEWORK_VERSION_V4_2
            Microsoft.SPOT.Debug.Print("start");
#endif
            try
            {
                //start the local timer, which gets the session dying
                int counter = IdleTimeout;
                this.Port.ReceiveTimeout = 1000;
                int grace = 0;

                //create a writer for the incoming data
                ByteArrayWriter writer = null;
                var buffer = new byte[CacheSize];

                //loop, until the host closes, or the timer expires
                while (
                    this._closing == false &&
                    counter-- > 0)
                {
                    if (--grace == 0)
                        writer = null;

                    //look for incoming data
                    int length;

                    try
                    {
                        //read the data from the physical port
                        length = this.Port.Receive(
                            buffer,
                            SocketFlags.None);

                        //check whether the remote has closed
                        if (length == 0)
                            break;
                    }
                    catch (SocketException)
                    {
                        //no data received due socket timeout
                        length = 0;
                    }

                    if (length > 0)
                    {
                        grace = 2;

                        //append the data to the writer
                        if (writer == null)
                            writer = new ByteArrayWriter();

                        writer.WriteBytes(
                            buffer,
                            0,
                            length
                            );

                        //try to decode the incoming data
                        var data = new ServerCommData(this.Protocol);
                        data.IncomingData = writer.ToReader();

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
                            this.Port.Send(outgoing);

                            //reset the timer
                            counter = IdleTimeout;
                            writer = null;
                        }
                        else if (result.Status == CommResponse.Ignore)
                        {
                            writer = null;
                        }
                    }

                    Thread.Sleep(0);
                }
            }
            finally
            {
                //ensure the local socket disposal
                this.Port.Close();
            }

#if NET45
            System.Diagnostics.Trace.WriteLine("close");
#elif MF_FRAMEWORK_VERSION_V4_1 || MF_FRAMEWORK_VERSION_V4_2
            Microsoft.SPOT.Debug.Print("close");
#endif

            //marks the server not running
            this.IsRunning = false;
        }

    }
}
