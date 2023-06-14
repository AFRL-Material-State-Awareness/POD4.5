using POD;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    /// <summary>
    /// This class controls the value of the input double based on the Excel column property
    /// </summary>
    public class UpdaterExcelPropertyValue : IUpdaterExcelPropertyValue
    {
        IGetExtendedPropertyControl _getExtendPropControl;
        public UpdaterExcelPropertyValue(IGetExtendedPropertyControl getExtendPropControl = null)
        {
            _getExtendPropControl = getExtendPropControl ?? new GetExtendedPropertyControl();
        }
        public double GetUpdatedValue(string myExtColProperty, double currentValue, DataColumn column)
        {
            string prevString;
            if (myExtColProperty == ExtColProperty.Min)
                prevString = ExtColProperty.MinPrev;
            else if (myExtColProperty == ExtColProperty.Max)
                prevString = ExtColProperty.MaxPrev;
            else if (myExtColProperty == ExtColProperty.Thresh)
                prevString = ExtColProperty.ThreshPrev;
            else
                throw new ArgumentException("ExtColProprty: " + myExtColProperty + " is not valid.");

            double.TryParse(_getExtendPropControl.GetExtendedProperty(column, prevString), out double prevValue);
            double.TryParse(_getExtendPropControl.GetExtendedProperty(column, myExtColProperty), out double newTableValue);
            if (currentValue == prevValue)
                return newTableValue;
            else
                return currentValue;
        }
        public double GetNewValue(string myExtColProperty, DataColumn column)
        {
            double.TryParse(column.ExtendedProperties[myExtColProperty]?.ToString(), out double newTableValue);
            return newTableValue;
        }


    }
    public class GetExtendedPropertyControl : IGetExtendedPropertyControl
    {
        public string GetExtendedProperty(DataColumn column, string colType)
        {
            if (column?.ExtendedProperties.ContainsKey(colType) == true)
                return column.ExtendedProperties[colType].ToString();
            else if(column?.ExtendedProperties.ContainsKey(colType) == false)
            {
                column.ExtendedProperties[colType] = ExtColProperty.GetDefaultValue(colType);
                return column.ExtendedProperties[colType].ToString();
            }
            else
                return ExtColProperty.GetDefaultValue(colType);
        }
    }
    public interface IUpdaterExcelPropertyValue
    {
        double GetUpdatedValue(string myExtColProperty, double currentValue, DataColumn column);
        double GetNewValue(string myExtColProperty, DataColumn column);
    }
    public interface IGetExtendedPropertyControl
    {
        string GetExtendedProperty(DataColumn column, string colType);
    }

}
