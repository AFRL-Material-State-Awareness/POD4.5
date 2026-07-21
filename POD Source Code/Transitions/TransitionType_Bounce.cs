// <copyright file="TransitionType_Bounce.cs" company="The University of Dayton">
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
    /// This transition bounces the property to a destination value and back to the
    /// original value. It is accelerated to the destination and then decelerated back
    /// as if being dropped with gravity and bouncing back against gravity.
    /// </summary>
    public class TransitionType_Bounce : TransitionType_UserDefined
    {
        #region Public methods

        /// <summary>
        /// Constructor. You pass in the total time taken for the bounce.
        /// </summary>
        public TransitionType_Bounce(int iTransitionTime)
        {
            // We create a custom "user-defined" transition to do the work...
            IList<TransitionElement> elements = new List<TransitionElement>();
            elements.Add(new TransitionElement(50, 100, InterpolationMethod.Accleration));
            elements.Add(new TransitionElement(100, 0, InterpolationMethod.Deceleration));
            base.setup(elements, iTransitionTime);
        }

        #endregion
    }
}
