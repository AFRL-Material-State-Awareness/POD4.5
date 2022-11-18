using POD.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace POD.Controls
{
    public class PODChartAll : PODChart
    {
        public PODChartAll()
        {
            ChartAreas.Clear();
        }
        public void FillChartAll(List<DataTable> myData, bool usingLoadedFromFileData = false)
        {
            
        }
        public void SetXAxisRange(AxisObject myAxis,double minFlaw, double maxFlaw,AnalysisData data,  TransformTypeEnum xTrans, TransformTypeEnum yTrans,bool forceLinear = false, bool keepLabelCount = false,
            bool transformResidView = false)
        {
            if (myAxis.Max < myAxis.Min)
            {
                myAxis.Max = maxFlaw;
                myAxis.Min = minFlaw;
                myAxis.Interval = .5;
            }

            CopyAxisObjectToAxis(ChartAreas[0].AxisX, myAxis);

            if (!forceLinear)
            {
                RelabelAxesBetter(myAxis, null, data.InvertTransformedFlaw, data.InvertTransformedResponse, Globals.GetLabelIntervalBasedOnChartSize(this, AxisKind.X), Globals.GetLabelIntervalBasedOnChartSize(this, AxisKind.Y),
                                    false, true, xTrans, yTrans, data.TransformValueForXAxis, data.TransformValueForYAxis, keepLabelCount, false);
            }
            else
            {
                RelabelAxesBetter(myAxis, null, data.DoNoTransform, data.DoNoTransform, Globals.GetLabelIntervalBasedOnChartSize(this, AxisKind.X), Globals.GetLabelIntervalBasedOnChartSize(this, AxisKind.Y),
                                    false, true, TransformTypeEnum.Linear, TransformTypeEnum.Linear, data.DoNoTransform, data.DoNoTransform, keepLabelCount, false);

            }
        }
    }
}
