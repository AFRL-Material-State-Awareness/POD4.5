using POD.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    [Serializable]
    public class UpdateTableControl : IUpdateTableControl
    {
        private IAnalysisData _data;
        public UpdateTableControl(IAnalysisData dataIn)
        {
            _data = dataIn;
        }
        public void UpdateTable(int rowIndex, int colIndex, Flag bounds)
        {
            switch (bounds)
            {
                case Flag.InBounds:
                    _data.TurnOnPoint(colIndex, rowIndex);
                    break;
                case Flag.OutBounds:
                    _data.TurnOffPoint(colIndex, rowIndex);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("bounds must be either InBounds or OutBounds");
            }
        }
    }
    public interface IUpdateTableControl
    {
        void UpdateTable(int rowIndex, int colIndex, Flag bounds);
    }
}
