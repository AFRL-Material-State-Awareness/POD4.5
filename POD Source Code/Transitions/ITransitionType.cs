// <copyright file="ITransitionType.cs" company="The University of Dayton">
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
    public interface ITransitionType
    {
		/// <summary>
		/// Called by the Transition framework when its timer ticks to pass in the
		/// time (in ms) since the transition started. 
		/// 
		/// You should return (in an out parameter) the percentage movement towards 
		/// the destination value for the time passed in. Note: this does not need to
		/// be a smooth transition from 0% to 100%. You can overshoot with values
		/// greater than 100% or undershoot if you need to (for example, to have some
		/// form of "elasticity").
		/// 
		/// The percentage should be returned as (for example) 0.1 for 10%.
		/// 
		/// You should return (in an out parameter) whether the transition has completed.
		/// (This may not be at the same time as the percentage has moved to 100%.)
		/// </summary>
		void onTimer(int iTime, out double dPercentage, out bool bCompleted);
    }
}
