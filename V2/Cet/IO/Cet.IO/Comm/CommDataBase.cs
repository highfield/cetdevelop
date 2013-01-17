using System;

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
namespace Cet.IO
{
    /// <summary>
    /// Base class for carrying data for any query against the "Comm" layer
    /// (both client and server)
    /// </summary>
    public class CommDataBase
    {
        protected CommDataBase(IProtocol protocol)
        {
            this.OwnerProtocol = protocol;
        }


        /// <summary>
        /// Reference to the protocol involved in the query
        /// </summary>
        public IProtocol OwnerProtocol { get; private set; }

        /// <summary>
        /// Allow to hold extra data around the query roundtrip
        /// </summary>
        public object UserData { get; set; }

        /// <summary>
        /// Data outgoing from the local application, toward the remote point
        /// </summary>
        public ByteArrayReader OutgoingData { get; set; }

        /// <summary>
        /// Data incoming from the remote point, toward the local application
        /// </summary>
        public ByteArrayReader IncomingData { get; set; }

    }
}
