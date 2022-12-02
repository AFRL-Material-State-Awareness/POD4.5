using POD.Data;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using System.Data;

namespace POD.Controls
{
    public class PODChartAll : DataPointChart
    {
        public PODChartAll()
        {
            ChartAreas.Clear();
            Series.Clear();
            //this.Palette = ChartColorPalette.Excel;
        }
        public void InitSetupChart(int numOfAnalysis)
        {
            List<string> myPOD = new List<string>();
            myPOD.Add("pod");
            List<string> myPODUnits = new List<string>();
            myPODUnits.Add("");
            SetupChart("flaw", "", myPOD, myPODUnits);
            for (int i =0; i<numOfAnalysis-1; i++)
            {
                AddSeries(i+2);
            }
        }
        public override void SetupChart(string flawName, string flawUnit, List<string> responseNames, List<string> responseUnits)
        {
            base.SetupChart(flawName, flawUnit, responseNames, responseUnits);

            Series series;

            DoPostPaint = true;

            if (Series.FindByName(PODAllChartLabels.POD) == null)
            {
                //draw POD Line
                Series.Add(new Series(PODAllChartLabels.POD));
                series = Series.Last();
                series.ChartType = SeriesChartType.Line;
                series.MarkerStyle = MarkerStyle.None;
                series.MarkerSize = 0;
                series.IsXValueIndexed = false;
                series.YValuesPerPoint = 1;
                series.BorderWidth = 3;
                series.Color = Color.FromArgb(ChartColors.LineAlpha, ChartColors.POD90Color);
                series.BorderDashStyle = ChartDashStyle.Solid;
                series.IsVisibleInLegend = false;
                //series.YAxisType = AxisType.Primary;
            }



            ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            ChartAreas[0].AxisX.MajorGrid.Enabled = false;

            

            AutoNameYAxis = false;
            YAxisNameIsUnitlessConstant = true;
            YAxisTitle = "POD";
            XAxisTitle = flawName;
            XAxisUnit = flawUnit;
            ChartTitle = "POD Curve";
        }
        public void AddSeries(int index)
        {
            Random RandGen = new Random();
            Series series;
            //draw POD Line
            Series.Add(new Series(PODAllChartLabels.POD+index.ToString()));
            series = Series.Last();
            series.ChartType = SeriesChartType.Line;
            series.MarkerStyle = MarkerStyle.None;
            series.MarkerSize = 0;
            series.IsXValueIndexed = false;
            series.YValuesPerPoint = 1;
            series.BorderWidth = 3;
            series.Color = Color.FromArgb(RandGen.Next(40, 210), RandGen.Next(60, 245), RandGen.Next(50, 220));
            series.BorderDashStyle = ChartDashStyle.Solid;
            series.IsVisibleInLegend = false;
            //series.YAxisType = AxisType.Secondary;
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
        public void SetXAxisWindow(double flawsMin, double flawsMax, double responseMin, double responseMax)
        {
            XAxis.Minimum = flawsMin - flawsMin*.1;
            XAxis.Maximum = flawsMax + flawsMax * .1;
            YAxis.Minimum = responseMin - responseMin * .1;
            YAxis.Maximum = responseMax + responseMax * .1;
            
        }
        public void FillChartAll(List<DataTable> myData)
        {
            //FillChart(myData[0], "flaw", "pod", 0);
            for (int i=0; i< myData.Count;i++)
            {
                FillChart(myData[i], "flaw", "pod", i);
                //FillChartMore(myData[0],"pod", i);
            }

        }
        public void FillChart(DataTable myTable, string my90X, string my90Y, int index)
        {
            DataView view = myTable.DefaultView;

            Series[index].Points.DataBindXY(view, my90X, view, my90Y);
            
            //POD9095.Points.DataBindXY(view, my95X, view, my95Y);
        }
        public void FillChartMore(DataTable myTable, string my90Y, int index)
        {
            DataView view = myTable.DefaultView;

            Series[index].Points.DataBindY(view, my90Y);

            //POD9095.Points.DataBindXY(view, my95X, view, my95Y);
        }
        /*
        public Series CurrPOD
        {
            get
            {
                return Series[PODAllChartLabels.POD];
            }
        }
        */
    }
}
