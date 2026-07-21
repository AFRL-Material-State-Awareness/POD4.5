// <copyright file="ManagedType_Float.cs" company="The University of Dayton">
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
    internal class ManagedType_Float : IManagedType
    {
        #region IManagedType Members

        /// <summary>
        /// Returns the type we're managing.
        /// </summary>
        public Type getManagedType()
        {
            return typeof(float);
        }

        /// <summary>
        /// Returns a copy of the float passed in.
        /// </summary>
        public object copy(object o)
        {
            float f = (float)o;
            return f;
        }

        /// <summary>
        /// Returns the interpolated value for the percentage passed in.
        /// </summary>
        public object getIntermediateValue(object start, object end, double dPercentage)
        {
            float fStart = (float)start;
            float fEnd = (float)end;
            return Utility.interpolate(fStart, fEnd, dPercentage);
        }

        #endregion
    }
}
