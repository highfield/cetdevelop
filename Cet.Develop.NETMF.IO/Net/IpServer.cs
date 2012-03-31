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
    /// Base/abstract implementation of an IP-listener
    /// </summary>
    internal abstract class IpServer
        : ICommServer
    {
        protected IpServer(
            Socket port,
            IProtocol protocol)
        {
            this.Port = port;
            this.Protocol = protocol;
        }



        public readonly Socket Port;
        public readonly IProtocol Protocol;

        private Thread _thread;
        protected bool _closing;



        /// <summary>
        /// Start the listener session
        /// </summary>
        public void Start()
        {
            this._thread = new Thread(this.Worker);
            this._thread.Start();
        }


        /// <summary>
        /// Handler for the session thread
        /// </summary>
        protected abstract void Worker();



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
