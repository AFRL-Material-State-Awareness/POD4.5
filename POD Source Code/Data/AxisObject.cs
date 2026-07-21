// <copyright file="AxisObject.cs" company="The University of Dayton">
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POD.Data
{
    public class AxisObject : IAxisObject
    {

        public double Max { get; set; }
        public double Min { get; set; }
        public double IntervalOffset { get; set; }
        public double Interval { get; set; }
        public double BufferPercentage { get; set; }
        public AxisKind Axis { get; set; }

        public AxisObject(AxisKind kind)
        {
            Max = 1.0;
            Min = -1.0;
            IntervalOffset = 0.0;
            Interval = .25;
            BufferPercentage = 5.0;
            Axis = kind;
        }
        public AxisObject Clone()
        {
            var axis = new AxisObject(Axis);

            axis.Max = Max;
            axis.Min = Min;
            axis.IntervalOffset = IntervalOffset;
            axis.Interval = Interval;
            axis.BufferPercentage = BufferPercentage;

            return axis;
        }
    }

    public interface IAxisObject
    {
        double Max { get; set; }
        double Min { get; set; }
        double IntervalOffset { get; set; }
        double Interval { get; set; }
        double BufferPercentage { get; set; }
        AxisKind Axis { get; set; }

        AxisObject Clone();
    }
}
