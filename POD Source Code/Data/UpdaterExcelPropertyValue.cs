using POD;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class UpdaterExcelPropertyValue : IUpdaterExcelPropertyValue
    {
        IGetExtendedPropertyControl _getExtendPropControl;
        public UpdaterExcelPropertyValue(IGetExtendedPropertyControl getExtendPropControl = null)
        {
            _getExtendPropControl = getExtendPropControl;
        }
        public double GetUpdatedValue(string myExtColProperty, double currentValue, DataColumn column)
        {
            double newValue;
            var prevString = "";

            if (myExtColProperty == ExtColProperty.Min)
                prevString = ExtColProperty.MinPrev;
            else if (myExtColProperty == ExtColProperty.Max)
                prevString = ExtColProperty.MaxPrev;
            else if (myExtColProperty == ExtColProperty.Thresh)
                prevString = ExtColProperty.ThreshPrev;

            if (prevString == "")
            {
                throw new Exception("ExtColProprty: " + myExtColProperty + " is not valid.");
            }

            double prevValue = 0.0;
            double newTableValue = 0.0;


            if (!Double.TryParse(_getExtendPropControl.GetExtendedProperty(column, prevString), out prevValue))
                prevValue = 0.0;

            if (!Double.TryParse(_getExtendPropControl.GetExtendedProperty(column, myExtColProperty), out newTableValue))
                newTableValue = 0.0;

            if (currentValue == prevValue)
                newValue = newTableValue;
            else
                newValue = currentValue;
            return newValue;
        }


    }
    public class GetExtendedPropertyControl : IGetExtendedPropertyControl
    {
        public string GetExtendedProperty(DataColumn column, string colType)
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
    public interface IUpdaterExcelPropertyValue
    {
        double GetUpdatedValue(string myExtColProperty, double currentValue, DataColumn column);
    }
    public interface IGetExtendedPropertyControl
    {
        string GetExtendedProperty(DataColumn column, string colType);
    }

}
