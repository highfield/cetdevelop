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
    /// Represent the response status of a request,
    /// being a wrapper around the <see cref="CommDataBase"/> instance
    /// </summary>
    public class CommResponse
    {
        /// <summary>
        /// Not enough data to be parsed
        /// </summary>
        public const int Unknown = 0;

        /// <summary>
        /// The data have been parsed, but something indicates
        /// that the request/response must not taken in account
        /// (e.g. wrong logical address)
        /// </summary>
        public const int Ignore = 1;

        /// <summary>
        /// A severe/critical error has been found
        /// </summary>
        public const int Critical = 2;

        /// <summary>
        /// The data have been parsed successfully
        /// </summary>
        /// <remarks>
        /// Note that this indication means just the correct acknowledge
        /// by the protocol layer. However, the application layer could
        /// reveal a wrong query and refuse its servicing
        /// </remarks>
        public const int Ack = 3;



        public CommResponse(CommDataBase data)
            : this(data, Ack, null)
        {
        }



        public CommResponse(
            CommDataBase data,
            int status,
            string description = null)
        {
            this.Data = data;
            this.Status = status;
            this.Description = description;
        }



        /// <summary>
        /// The wrapped instance containing the data of the query
        /// </summary>
        public readonly CommDataBase Data;



        /// <summary>
        /// Indicate the status of the response
        /// </summary>
        public readonly int Status;



        /// <summary>
        /// Indicate the description related to the current status
        /// </summary>
        public readonly string Description;


#if NET45
        public override string ToString()
        {
            return string.Format(
                "{0}: sts={1}; descr={2}",
                this.GetType().Name,
                this.Status,
                this.Description
                );
        }
#endif
    }
}
