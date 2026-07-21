// <copyright file="ColumnUpdaterFromInfos.cs" company="The University of Dayton">
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

using POD;
using POD.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class ColumnUpdaterFromInfos : IColumnUpdaterFromInfos
    {
        private IGetPreviousValueControl _getPreviousValue;
        public ColumnUpdaterFromInfos(IGetPreviousValueControl getPreviousValue = null)
        {
            _getPreviousValue = getPreviousValue ?? new GetPreviousValueControl();
        }
        public void UpdateColumnFromInfo(DataColumn column, ColumnInfo info)
        {
            column.ColumnName = info.NewName;

            var prevMin = _getPreviousValue.GetPreviousValue(column, ExtColProperty.Min, info, InfoType.Min, ExtColProperty.MinDefault);
            var prevMax = _getPreviousValue.GetPreviousValue(column, ExtColProperty.Max, info, InfoType.Max, ExtColProperty.MaxDefault);
            var prevThresh = _getPreviousValue.GetPreviousValue(column, ExtColProperty.Thresh, info, InfoType.Threshold, ExtColProperty.ThreshDefault);

            column.ExtendedProperties[ExtColProperty.MinPrev] = prevMin.ToString();
            column.ExtendedProperties[ExtColProperty.MaxPrev] = prevMax.ToString();
            column.ExtendedProperties[ExtColProperty.ThreshPrev] = prevThresh.ToString();

            column.ExtendedProperties[ExtColProperty.Min] = info.Min.ToString();
            column.ExtendedProperties[ExtColProperty.Max] = info.Max.ToString();
            column.ExtendedProperties[ExtColProperty.Thresh] = info.Threshold.ToString();
            column.ExtendedProperties[ExtColProperty.Unit] = info.Unit;
        }
    }
    public interface IColumnUpdaterFromInfos
    {
        void UpdateColumnFromInfo(DataColumn column, ColumnInfo info);
    }
}
