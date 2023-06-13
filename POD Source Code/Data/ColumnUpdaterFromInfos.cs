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
