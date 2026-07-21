// <copyright file="TransitionType_CriticalDamping.cs" company="The University of Dayton">
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
    /// This transition animates with an exponential decay. This has a damping effect
    /// similar to the motion of a needle on an electomagnetically controlled dial.
    /// </summary>
	public class TransitionType_CriticalDamping : ITransitionType
	{
		#region Public methods

		/// <summary>
		/// Constructor. You pass in the time that the transition 
		/// will take (in milliseconds).
		/// </summary>
		public TransitionType_CriticalDamping(int iTransitionTime)
		{
			if (iTransitionTime <= 0)
			{
				throw new Exception("Transition time must be greater than zero.");
			}
			m_dTransitionTime = iTransitionTime;
		}

		#endregion

		#region ITransitionMethod Members

		/// <summary>
		/// </summary>
		public void onTimer(int iTime, out double dPercentage, out bool bCompleted)
		{
			// We find the percentage time elapsed...
			double dElapsed = iTime / m_dTransitionTime;
			dPercentage = (1.0 - Math.Exp(-1.0 * dElapsed * 5)) / 0.993262053;

			if (dElapsed >= 1.0)
			{
				dPercentage = 1.0;
				bCompleted = true;
			}
			else
			{
				bCompleted = false;
			}
		}

		#endregion

		#region Private data

		private double m_dTransitionTime = 0.0;

		#endregion
	}
}
