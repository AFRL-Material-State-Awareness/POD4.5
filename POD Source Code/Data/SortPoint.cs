// <copyright file="SortPoint.cs" company="The University of Dayton">
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
using static POD.Data.SortPoint;

namespace POD.Data
{
    public enum Flag
    {
        InBounds,
        OutBounds,
        None
    }

    public class FixPoint
    {
        int _pointIndex;

        public int PointIndex
        {
            get { return _pointIndex; }
            set { _pointIndex = value; }
        }
        int _seriesIndex;

        public int SeriesIndex
        {
            get { return _seriesIndex; }
            set { _seriesIndex = value; }
        }
        Flag _flag;

        public Flag Flag
        {
            get { return _flag; }
            set { _flag = value; }
        }

        public FixPoint(int pointIndex, int seriesIndex, Flag flag)
        {
            _pointIndex = pointIndex;
            _seriesIndex = seriesIndex;
            _flag = flag;
        }
    }

    public class SortPoint : IComparable<SortPoint>
    {
        public int ColIndex { get; set; }
        public int RowIndex { get; set; }

        public string SeriesName { get; set; }

        public int SeriesIndex { get; set; }

        public int SeriesPtIndex { get; set; }

        public double XValue { get; set; }

        public double YValue { get; set; }

        public int CompareTo(SortPoint otherSortPoint)
        {
            if (otherSortPoint == null)
            {
                return 1;
            }

            int result = XValue.CompareTo(otherSortPoint.XValue);
            if (result == 0)
            {
                result = YValue.CompareTo(otherSortPoint.YValue);

                if (result == 0)
                {
                    //only compare series name if it is defined
                    if (SeriesName.Length > 0)
                    {
                        result = SeriesName.CompareTo(otherSortPoint.SeriesName);
                    }

                    if (result == 0)
                    {
                        result = ColIndex.CompareTo(otherSortPoint.ColIndex);

                        if (result == 0)
                        {
                            result = RowIndex.CompareTo(otherSortPoint.RowIndex);
                        }
                    }
                }
            }
            return result;
        }
    }
}
