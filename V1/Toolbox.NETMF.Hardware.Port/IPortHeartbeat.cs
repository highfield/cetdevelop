using System;
using Microsoft.SPOT;

/*
 * Copyright 2012 Mario Vernari (http://netmftoolbox.codeplex.com/)
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
    /// The interface that must be implemented by any class
    /// needing an heartbeat-clock handling
    /// </summary>
    internal interface IPortHeartbeat
    {
        /// <summary>
        /// Gets whether the heartbeat should be actually
        /// fed to the subscribed instance
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// Handler for the heartbeat event
        /// </summary>
        void Tick();
    }
}
