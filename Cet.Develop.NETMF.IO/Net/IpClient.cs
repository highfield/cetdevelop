using System;
using Microsoft.SPOT;
using System.Threading;
using System.Net.Sockets;
using System.Net;

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
    /// Implementation of a socket client (either TCP, or UDP)
    /// </summary>
    internal class IpClient
        : ICommClient
    {
        public IpClient(Socket port)
        {
            this.Port = port;
        }


        public readonly Socket Port;

        public int Latency { get; set; }



        /// <summary>
        /// Entry-point for submitting a query to the remote device
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public CommResponse Query(ClientCommData data)
        {
            lock (this.Port)
            {
                //convert the request data as an ordinary byte array
                byte[] outgoing = data
                    .OutgoingData
                    .ToArray();

                //create a writer for accumulate the incoming data
                var incoming = new ByteArrayWriter();

                const int tempSize = 64;
                var temp = new byte[tempSize];

                //retries loop
                for (int attempt = 0, retries = data.Retries; attempt < retries; attempt++)
                {
                    //phyiscal writing
                    this.Port
                        .Send(outgoing);

                    incoming.Reset();

                    //start the local timer
                    bool timeoutExpired;
                    int totalTimeout = this.Latency + data.Timeout;

                    using (Timer timer = new Timer(
                        _ => timeoutExpired = true,
                        state: null,
                        dueTime: totalTimeout,
                        period: Timeout.Infinite))
                    {
                        //reception loop, until a valid response or timeout
                        timeoutExpired = false;
                        while (timeoutExpired == false)
                        {
                            var length = this.Port.Available;

                            if (length > 0)
                            {
                                if (length > tempSize)
                                    length = tempSize;

                                //read the incoming data from the physical port
                                this.Port
                                    .Receive(temp, length, SocketFlags.None);

                                //append data to the writer
                                incoming.WriteBytes(
                                    temp,
                                    0,
                                    length);

                                //try to decode the stream
                                data.IncomingData = incoming.ToReader();

                                CommResponse result = data
                                    .OwnerProtocol
                                    .Codec
                                    .ClientDecode(data);

                                //exit whether any concrete result: either good or bad
                                if (result.Status == CommResponse.Ack)
                                {
                                    return result;
                                }
                                else if (result.Status == CommResponse.Critical)
                                {
                                    return result;
                                }
                                else if (result.Status != CommResponse.Unknown)
                                {
                                    break;
                                }
                            }

                            Thread.Sleep(0);

                            //stop immediately if the host asked to abort

                            //TODO
                        }
                    }   //using (timer)
                }       //for

                //no attempt was successful
                return new CommResponse(
                    data,
                    CommResponse.Critical);
            }   //lock
        }

    }
}
