// <copyright file="ManagedType_Double.cs" company="The University of Dayton">
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
	/// Manages transitions for double properties.
	/// </summary>
	internal class ManagedType_Double : IManagedType
	{
		#region IManagedType Members

		/// <summary>
		///  Returns the type managed by this class.
		/// </summary>
		public Type getManagedType()
		{
			return typeof(double);
		}

		/// <summary>
		/// Returns a copy of the double passed in.
		/// </summary>
		public object copy(object o)
		{
			double d = (double)o;
			return d;
		}

		/// <summary>
		/// Returns the value between start and end for the percentage passed in.
		/// </summary>
		public object getIntermediateValue(object start, object end, double dPercentage)
		{
			double dStart = (double)start;
			double dEnd = (double)end;
			return Utility.interpolate(dStart, dEnd, dPercentage);
		}

		#endregion
	}
}
