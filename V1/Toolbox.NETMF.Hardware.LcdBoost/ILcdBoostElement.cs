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
    /// Fundamental interface for implementing any graphic element
    /// to be managed in the declarative-way by the LcdBoos library
    /// </summary>
    public interface ILcdBoostElement
    {
        /// <summary>
        /// Indicates whether the graphic element is hidden or not
        /// </summary>
        bool IsHidden { get; }

        /// <summary>
        /// Renders the element's cache onto the video viewport
        /// </summary>
        /// <param name="cache">The target video cache to be filled</param>
        /// <param name="container">The parent container bounding rectangle</param>
        void Render(
            LcdBoostVideoCache cache,
            LcdBoostRect container
            );
    }
}
