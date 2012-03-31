using System;
using Microsoft.SPOT;
using System.Threading;
using System.Net.Sockets;
using System.Net;

using Cet.Develop.NETMF.IO.Protocols;

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
namespace Cet.Develop.NETMF.IO
{
    /// <summary>
    /// Concrete implementation of a UDP-listener
    /// </summary>
    internal class UdpServer
        : IpServer
    {
        public UdpServer(
            Socket port,
            IProtocol protocol)
            : base(port, protocol)
        {
        }



        /// <summary>
        /// Running thread handler
        /// </summary>
        protected override void Worker()
        {
            //loop, until the host closes
            while (this._closing == false)
            {
                //look for incoming data
                int length = this.Port.Available;

                if (length > 0)
                {
                    var buffer = new byte[length];
                    EndPoint remote = new IPEndPoint(IPAddress.Any, 0);

                    //read the data from the physical port
                    this.Port.ReceiveFrom(
                        buffer,
                        ref remote);

                    //try to decode the incoming data
                    var data = new ServerCommData(this.Protocol);
                    data.IncomingData = new ByteArrayReader(buffer);

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
                        byte[] outgoing = data
                            .OutgoingData
                            .ToArray();

                        this.Port.SendTo(
                            outgoing,
                            remote);
                    }
                }

                Thread.Sleep(0);
            }
        }
    }

}
