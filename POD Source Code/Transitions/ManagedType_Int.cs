// <copyright file="ManagedType_Int.cs" company="The University of Dayton">
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
    /// Manages transitions for int properties.
    /// </summary>
    internal class ManagedType_Int : IManagedType
    {
		#region IManagedType Members

		/// <summary>
		/// Returns the type we are managing.
		/// </summary>
		public Type getManagedType()
		{
			return typeof(int);
		}

		/// <summary>
		/// Returns a copy of the int passed in.
		/// </summary>
		public object copy(object o)
		{
			int value = (int)o;
			return value;
		}

		/// <summary>
		/// Returns the value between the start and end for the percentage passed in.
		/// </summary>
		public object getIntermediateValue(object start, object end, double dPercentage)
		{
			int iStart = (int)start;
			int iEnd = (int)end;
			return Utility.interpolate(iStart, iEnd, dPercentage);
		}

		#endregion
	}
}
