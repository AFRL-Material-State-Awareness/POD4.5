// <copyright file="GetPreviousValueControl.cs" company="The University of Dayton">
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
    public class GetPreviousValueControl : IGetPreviousValueControl
    {
       
        public double GetPreviousValue(DataColumn column, string colType, IColumnInfo info, InfoType infoType, double defaultValue)
        {
            string prevString = AssignPrevString(colType);

            double.TryParse(GetExtendedProperty(column, colType), out double currentValue);
            if (column != null && !column.ExtendedProperties.ContainsKey(prevString))
                column.ExtendedProperties[prevString] = defaultValue;

            double.TryParse(GetExtendedProperty(column, prevString), out double prevValue);
            if (prevValue == defaultValue)
                return info.GetDoubleValue(infoType);
            else
                return currentValue;
        }
        private string AssignPrevString(string colType)
        {
            switch (colType)
            {
                case ExtColProperty.Min:
                    return ExtColProperty.MinPrev;
                case ExtColProperty.Max:
                    return ExtColProperty.MaxPrev;
                case ExtColProperty.Thresh:
                    return ExtColProperty.ThreshPrev;
                default:
                    return "";
            }
        }
        private static string GetExtendedProperty(DataColumn column, string colType)
        {
            string value = "";

            if (column == null)
            {
                value = ExtColProperty.GetDefaultValue(colType);
                return value;
            }

            if (column.ExtendedProperties.ContainsKey(colType))
                value = column.ExtendedProperties[colType].ToString();
            else
            {
                value = ExtColProperty.GetDefaultValue(colType);
                column.ExtendedProperties[colType] = value;
            }

            return value;
        }
    }
    public interface IGetPreviousValueControl
    {
        double GetPreviousValue(DataColumn column, string colType, IColumnInfo info, InfoType infoType, double defaultValue);
    }
}
