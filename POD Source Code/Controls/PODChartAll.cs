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
        List<string> responseNamesLegend;
        public PODChartAll()
        {
            ChartAreas.Clear();
            Series.Clear();
            //this.Palette = ChartColorPalette.Excel;
            this.responseNamesLegend = new List<string>();
        }
        public void InitSetupChart(int numOfAnalysis)
        {
            List<string> myPOD = new List<string>();
            myPOD.Add("pod");
            List<string> myPODUnits = new List<string>();
            myPODUnits.Add("");
            SetupChart("flaw", "", myPOD, myPODUnits);
            for ( int i =0; i<numOfAnalysis-1; i++)
            {
                AddSeries(i+1);
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
                Series.Add(new Series(this.responseNamesLegend[0]));
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
        private void AddSeries(int index)
        {
            Random RandGen = new Random();
            Series series;
            //draw POD Line
            Series.Add(new Series(this.responseNamesLegend[index]));
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
        public void SetXAxisWindow(double flawsMin, double flawsMax, double responseMin, double responseMax)
        {
            this.XAxis.Minimum = flawsMin - flawsMin*.1;
            this.XAxis.Maximum = flawsMax + flawsMax * .1;
            this.YAxis.Minimum = responseMin - responseMin * .1;
            this.YAxis.Maximum = responseMax + responseMax * .1;
            
        }
        public void FillChartAll(List<DataTable> myData)
        {
            for (int i=0; i< myData.Count;i++)
            {
                FillChart(myData[i], "flaw", "pod", i);
            }
            SetLegend();
        }
        private void FillChart(DataTable myTable, string my90X, string my90Y, int index)
        {
            DataView view = myTable.DefaultView;

            Series[index].Points.DataBindXY(view, my90X, view, my90Y);           
        }
        private void SetLegend()
        {
            if (this.responseNamesLegend.Count > 0 && this.responseNamesLegend.Count==Series.Count)
            {
                for (int i=0; i< this.responseNamesLegend.Count; i++)
                {
                    // Create a new legend called "Legend2".
                    this.Legends.Add(new Legend(this.responseNamesLegend[i]));
                    // Set Docking of the Legend chart to the Default Chart Area.
                    this.Legends[this.responseNamesLegend[i]].Docking = Docking.Bottom;
                    // Assign the legend to Series1.
                    this.Series[i].Legend = this.responseNamesLegend[i];
                    this.Series[i].IsVisibleInLegend = true;
                }
            }
            else
            {
                new Exception("Error: no analysis names extracted or not equal to amont of analyses in the project!");
            }
        }
        public List<string> ResponseNamesAll
        {
            set { this.responseNamesLegend = value; }
        }
    }
}
