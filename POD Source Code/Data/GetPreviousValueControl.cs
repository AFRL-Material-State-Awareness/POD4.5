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
       
        public double GetPreviousValue(DataColumn column, string colType, ColumnInfo info, InfoType infoType, double defaultValue)
        {
            double prevValue = 0.0;
            double currentValue = 0.0;
            string prevString = "";

            if (colType == ExtColProperty.Min)
                prevString = ExtColProperty.MinPrev;
            else if (colType == ExtColProperty.Max)
                prevString = ExtColProperty.MaxPrev;
            else if (colType == ExtColProperty.Thresh)
                prevString = ExtColProperty.ThreshPrev;

            if (!Double.TryParse(GetExtendedProperty(column, colType), out currentValue))
                currentValue = 0.0;

            if (!column.ExtendedProperties.ContainsKey(prevString))
                column.ExtendedProperties[prevString] = defaultValue;

            if (!Double.TryParse(GetExtendedProperty(column, prevString), out prevValue))
                prevValue = 0.0;

            if (prevValue == defaultValue)
                prevValue = info.GetDoubleValue(infoType);
            else
                prevValue = currentValue;

            return prevValue;
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
        double GetPreviousValue(DataColumn column, string colType, ColumnInfo info, InfoType infoType, double defaultValue);
    }
}
