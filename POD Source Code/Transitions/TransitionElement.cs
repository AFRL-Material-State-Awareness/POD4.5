// <copyright file="TransitionElement.cs" company="The University of Dayton">
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
    public enum InterpolationMethod
    {
        Linear,
        Accleration,
        Deceleration,
        EaseInEaseOut
    }

    public class TransitionElement
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public TransitionElement(double endTime, double endValue, InterpolationMethod interpolationMethod)
        {
            EndTime = endTime;
            EndValue = endValue;
            InterpolationMethod = interpolationMethod;
        }

        /// <summary>
        /// The percentage of elapsed time, expressed as (for example) 75 for 75%.
        /// </summary>
        public double EndTime { get; set; }

        /// <summary>
        /// The value of the animated properties at the EndTime. This is the percentage 
        /// movement of the properties between their start and end values. This should
        /// be expressed as (for example) 75 for 75%.
        /// </summary>
        public double EndValue { get; set; }

        /// <summary>
        /// The interpolation method to use when moving between the previous value
        /// and the current one.
        /// </summary>
        public InterpolationMethod InterpolationMethod { get; set; }
    }
}
