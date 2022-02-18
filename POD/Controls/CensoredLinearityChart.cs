using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using POD.Data;

namespace POD.Controls
{
    public partial class CensoredLinearityChart : LinearityChart
    {
        double _slope = 0.0;
        double _intercept = 0.0;
        double _fitError = 0.0;
        double _repeatError = 0.0;

        public CensoredLinearityChart()
        {
            InitializeComponent();

            //all charts should be added through SetupChart()
            Series.Clear();
            Legends.Clear();
        }

        public override void SetupChart(string flawName, string flawUnit, List<string> responseNames, List<string> responseUnits)
        {
            base.SetupChart(flawName, flawUnit, responseNames, responseUnits);

            Series series;

            //Series.Clear();

            /*if (Series.FindByName(LinearityChartLabels.LeftCensor) == null)
            {
                //draw left censor line
                Series.Add(new Series(LinearityChartLabels.LeftCensor));
                series = Series.Last();
                series.ChartType = SeriesChartType.Line;
                series.IsXValueIndexed = false;
                series.YValuesPerPoint = 1;
                series.BorderWidth = 2;
                series.Color = Color.FromArgb(ChartColors.BoundaryAreaAlpha*4, ChartColors.LeftCensorColor);
                series.BorderDashStyle = ChartDashStyle.Dash;
                series.IsVisibleInLegend = false;
            }

            if (Series.FindByName(LinearityChartLabels.RightCensor) == null)
            {
                //draw right censor line
                Series.Add(new Series(LinearityChartLabels.RightCensor));
                series = Series.Last();
                series.ChartType = SeriesChartType.Line;
                series.IsXValueIndexed = false;
                series.YValuesPerPoint = 1;
                series.BorderWidth = 2;
                series.Color = Color.FromArgb(ChartColors.BoundaryAreaAlpha*4, ChartColors.RightCensorColor);
                series.BorderDashStyle = ChartDashStyle.Dash;
                series.IsVisibleInLegend = false;
            }*/

            this.YAxis.Title = "Residuals";

            if (Series.FindByName(LinearityChartLabels.CompleteCensored) == null)
            {
                //draw residual for completely censored data
                Series.Add(new Series(LinearityChartLabels.CompleteCensored));
                series = Series.Last();
                series.ChartType = SeriesChartType.Point;
                series.YValuesPerPoint = 1;
                series.Color = Color.FromArgb(ChartColors.LineAlpha, ChartColors.CensoredPoints);
                series.MarkerStyle = MarkerStyle.Circle;
                series.IsVisibleInLegend = false;
            }

            if (Series.FindByName(LinearityChartLabels.PartialCensored) == null)
            {
                //draw residual for partially censored data
                Series.Add(new Series(LinearityChartLabels.PartialCensored));
                series = Series.Last();
                series.ChartType = SeriesChartType.Point;
                series.YValuesPerPoint = 1;
                series.Color = Color.FromArgb(ChartColors.LineAlpha, ChartColors.SemiCensoredPoints);
                series.MarkerStyle = MarkerStyle.Circle;
                series.IsVisibleInLegend = false;
            }

            //add legend to explain different colored points
            if (Legends.FindByName(LinearityChartLabels.LegendTitle) == null)
            {
                Legends.Add(new Legend(LinearityChartLabels.LegendTitle));
                Legend legend = Legends.Last();
                legend.Alignment = StringAlignment.Center;
                legend.Docking = Docking.Top;
            }

            AutoNameYAxis = false;
            YAxisNameIsUnitlessConstant = true;
            YAxisTitle = "Residuals";
            XAxisTitle = flawName;
            XAxisUnit = flawUnit;
            ChartTitle = "Residuals";
        }

        public override void ClearEverythingButPoints()
        {
            base.ClearEverythingButPoints();

            Series[LinearityChartLabels.CompleteCensored].Points.Clear();
            Series[LinearityChartLabels.PartialCensored].Points.Clear();
        }

        public Series LeftCensor
        {
            get
            {
                return Series[LinearityChartLabels.LeftCensor];
            }
        }

        public Series RightCensor
        {
            get
            {
                return Series[LinearityChartLabels.RightCensor];
            }
        }

        public Series CompleteCensored
        {
            get
            {
                return Series[LinearityChartLabels.CompleteCensored];
            }
        }

        public Series PartialCensored
        {
            get
            {
                return Series[LinearityChartLabels.PartialCensored];
            }
        }

        public override string TooltipText
        {
            get
            {
                var text = "";

                text += "Linear Fit Parameters" + Environment.NewLine;
                text += "Slope:\t\t" + _slope.ToString("F3") + Environment.NewLine;
                text += "Intercept:\t" + _intercept.ToString("F3") + Environment.NewLine;
                text += "Residual Error:\t" + _fitError.ToString("F3") + Environment.NewLine;
                text += "Repeat Error:\t" + _repeatError.ToString("F3");

                return text;
            }
        }

        public void FillChart(AnalysisData myData, double mySlope, double myIntercept, double fitError, double repeatError)
        {
            double xMin = myData.InvertTransformedFlaw(myData.UncensoredFlawRangeMin);
            double xMax = myData.InvertTransformedFlaw(myData.UncensoredFlawRangeMax);

            FlawEstimate.Points.Clear();
            FlawEstimate.Points.AddXY(xMin, 0.0);
            FlawEstimate.Points.AddXY(xMax, 0.0);

            _slope = mySlope;
            _intercept = myIntercept;
            _fitError = fitError;
            _repeatError = repeatError;

            DataView view = myData.ResidualUncensoredTable.DefaultView;
            Uncensored.Points.DataBindXY(view, "flaw", view, "t_diff");

            var uncensoredMax = Double.NegativeInfinity;
            var uncensoredMin = Double.PositiveInfinity;

            if (Uncensored.Points.Count > 0)
            {
                uncensoredMax = Uncensored.Points.FindMaxByValue("Y1").YValues[0];
                uncensoredMin = Uncensored.Points.FindMinByValue("Y1").YValues[0];
            }

            view = myData.ResidualPartialCensoredTable.DefaultView;
            PartialCensored.Points.DataBindXY(view, "flaw", view, "t_diff");

            var censoredMax = Double.NegativeInfinity;
            var censoredMin = Double.PositiveInfinity;

            if (PartialCensored.Points.Count > 0)
            {
                censoredMax = PartialCensored.Points.FindMaxByValue("Y1").YValues[0];
                censoredMin = PartialCensored.Points.FindMinByValue("Y1").YValues[0];
            }

            var globalResponseMax = (uncensoredMax > censoredMax) ? uncensoredMax : censoredMax;
            var globalResponseMin = (uncensoredMin < censoredMin) ? uncensoredMin : censoredMin;

            if(globalResponseMax < globalResponseMin)
            {
                globalResponseMax = 1.0;
                globalResponseMin = -1.0;
            }

            AxisObject yAxis = new AxisObject();
            AxisObject xAxis = new AxisObject();

            AnalysisData.GetBufferedRange(xAxis, xMin, xMax, true);
            AnalysisData.GetBufferedRange(yAxis, globalResponseMin, globalResponseMax, false);//myData.ResponseTransform == TransformTypeEnum.Linear);

            //xAxis.Interval /= 2.0;
            //yAxis.Interval /= 2.0;

            SetXAxisRange(xAxis, myData);
            SetYAxisRange(yAxis, myData);

            RelabelAxes(xAxis, yAxis,
                        null, null, 10, 10, false, true);

            //view = myData.ResidualFullCensoredTable.DefaultView;
            //CompleteCensored.Points.DataBindXY(view, "t_flaw", view, "t_diff");

            double flawRangeMin = ChartAreas[0].AxisX.Minimum;
            double flawRangeMax = ChartAreas[0].AxisX.Maximum;

            double fitMax = flawRangeMax * mySlope + myIntercept;
            double fitMin = flawRangeMin * mySlope + myIntercept;

            

            

            /*LeftCensor.Points.Clear();
            LeftCensor.Points.AddXY(flawRangeMin, myResponseMax - fitMin);
            LeftCensor.Points.AddXY(flawRangeMax, myResponseMax - fitMax);

            RightCensor.Points.Clear();
            RightCensor.Points.AddXY(flawRangeMin, myResponseMin - fitMin);
            RightCensor.Points.AddXY(flawRangeMax, myResponseMin - fitMax);*/
        }
    }
}
