// <copyright file="TransitionType_Linear.cs" company="The University of Dayton">
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
	/// This class manages a linear transition. The percentage complete for the transition
	/// increases linearly with time.
	/// </summary>
    public class TransitionType_Linear : ITransitionType
    {
        #region Public methods

        /// <summary>
        /// Constructor. You pass in the time (in milliseconds) that the
        /// transition will take.
        /// </summary>
        public TransitionType_Linear(int iTransitionTime)
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
		/// We return the percentage completed.
		/// </summary>
		public void onTimer(int iTime, out double dPercentage, out bool bCompleted)
		{
			dPercentage = (iTime / m_dTransitionTime);
			if (dPercentage >= 1.0)
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
