// <copyright file="TransitionType_Flash.cs" company="The University of Dayton">
// Copyright © 2025 The University of Dayton. All rights reserved.
//
// Licensed under the GNU General Public License, Version 3.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.gnu.org/licenses/gpl-3.0.txt
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Transitions
{
    /// <summary>
    /// This transition type 'flashes' the properties a specified number of times, ending
    /// up by reverting them to their initial values. You specify the number of bounces and
    /// the length of each bounce. 
    /// </summary>
    public class TransitionType_Flash : TransitionType_UserDefined
    {
        #region Public methods

        /// <summary>
        /// You specify the number of bounces and the time taken for each bounce.
        /// </summary>
        public TransitionType_Flash(int iNumberOfFlashes, int iFlashTime)
        {
            // This class is derived from the user-defined transition type.
            // Here we set up a custom "user-defined" transition for the 
            // number of flashes passed in...
            double dFlashInterval = 100.0 / iNumberOfFlashes;

            // We set up the elements of the user-defined transition...
            IList<TransitionElement> elements = new List<TransitionElement>();
            for(int i=0; i<iNumberOfFlashes; ++i)
            {
                // Each flash consists of two elements: one going to the destination value, 
                // and another going back again...
                double dFlashStartTime = i * dFlashInterval;
                double dFlashEndTime = dFlashStartTime + dFlashInterval;
                double dFlashMidPoint = (dFlashStartTime + dFlashEndTime) / 2.0;
                elements.Add(new TransitionElement(dFlashMidPoint, 100, InterpolationMethod.EaseInEaseOut));
                elements.Add(new TransitionElement(dFlashEndTime, 0, InterpolationMethod.EaseInEaseOut));
            }

            base.setup(elements, iFlashTime * iNumberOfFlashes);
        }

        #endregion

    }
}
