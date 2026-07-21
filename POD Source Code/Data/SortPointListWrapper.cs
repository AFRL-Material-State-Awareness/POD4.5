// <copyright file="SortPointListWrapper.cs" company="The University of Dayton">
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
using System.Data;
using static POD.Data.SortPoint;
using POD.Data;

namespace Data
{
    public class SortPointListWrapper : ISortPointListWrapper
    {
        private List<SortPoint> _sortPointList;
        public SortPointListWrapper(List<SortPoint> sortPointInput)
        {
            _sortPointList = sortPointInput;
        }
        public int GetCountOfList() => _sortPointList.Count;
        public bool HasAnyPoints() => _sortPointList.Any();
        public List<SortPoint> SortPointList =>_sortPointList;
        public int BinarySearch(SortPoint item)
        {
            return _sortPointList.BinarySearch(item);
        }
        //public int SeriesPtIndex { get { return _sortPointList.SeriesPtIndex; } }
    }
    public interface ISortPointListWrapper
    {
        int GetCountOfList();
        bool HasAnyPoints();
        List<SortPoint> SortPointList { get; }
        int BinarySearch(SortPoint item);
        //int SeriesPtIndex { get; }
    }
}
