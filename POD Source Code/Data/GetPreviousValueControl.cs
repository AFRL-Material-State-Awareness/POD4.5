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
