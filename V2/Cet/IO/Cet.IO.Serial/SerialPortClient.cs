using System;
using System.Threading;
using System.Diagnostics;

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
    /// Implementation of a serial port client
    /// </summary>
    internal class SerialPortClient
        : ICommClient
    {
        public SerialPortClient(
            ISerialPort port,
            SerialPortParams setting)
        {
            this.Port = port;
            this.Setting = setting;
        }


        public readonly ISerialPort Port;
        public readonly SerialPortParams Setting;

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
                try
                {
                    //set the proper parameters to the port
                    this.Port.BaudRate = this.Setting.BaudRate;
                    this.Port.Parity = this.Setting.Parity;
                    this.Port.DataBits = this.Setting.DataBits;
                    this.Port.StopBits = this.Setting.StopBits;
                    this.Port.RtsEnable = this.Setting.RtsEnable;
                }
                catch (Exception ex)
                {
                    return new CommResponse(
                        data,
                        CommResponse.Critical,
                        ex.Message);
                }

                //convert the request data as an ordinary byte array
                byte[] outgoing = ((IByteArray)data.OutgoingData).Data;

                //create a writer for accumulate the incoming data
                var incoming = new ByteArrayWriter();

                const int tempSize = 64;
                var temp = new byte[tempSize];

                //retries loop
                for (int attempt = 0, retries = data.Retries; attempt < retries; attempt++)
                {
                    try
                    {
                        //flush any residual content
                        this.Port
                            .DiscardInBuffer();

                        this.Port
                            .DiscardOutBuffer();

#if NET45
                        System.Diagnostics.Trace.WriteLine("TX: " + ByteArrayHelpers.ToHex(outgoing));
#endif

                        //phyiscal writing
                        this.Port
                            .Write(outgoing, 0, outgoing.Length);
                    }
                    catch (Exception ex)
                    {
                        return new CommResponse(
                            data,
                            CommResponse.Critical,
                            ex.Message);
                    }

                    incoming.Drop();

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
                            int length = this.Port.BytesToRead;

                            if (length > 0)
                            {
                                if (length > tempSize)
                                    length = tempSize;

                                try
                                {
                                    //read the incoming data from the physical port
                                    this.Port
                                        .Read(temp, 0, length);
                                }
                                catch (Exception ex)
                                {
                                    return new CommResponse(
                                        data,
                                        CommResponse.Critical,
                                        ex.Message);
                                }

                                //append data to the writer
                                incoming.WriteBytes(
                                    temp,
                                    0,
                                    length);

                                //try to decode the stream
                                data.IncomingData = incoming.ToReader();

#if NET45
                                System.Diagnostics.Trace.WriteLine("RX: " + ByteArrayHelpers.ToHex(incoming.ToByteArray()));
#endif

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
