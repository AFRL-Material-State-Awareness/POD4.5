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
    public partial class PODThresholdChart : DataPointChart
    {
        double _a50 = 0.0;
        double _a90 = 0.0;
        double _a9095 = 0.0;
        double _threshold = 0.0;

        public PODThresholdChart()
        {
            InitializeComponent();

            //all charts should be added through SetupChart()
            Series.Clear();
        }

        public override void SetupChart(string flawName, string flawUnit, List<string> responseNames, List<string> responseUnits)
        {
            base.SetupChart(flawName, flawUnit, responseNames, responseUnits);

            Series series;

            if (Series.FindByName(PODThresholdChartLabels.POD9095_All) == null)
            {
                //draw the 90/95 line
                Series.Add(new Series(PODThresholdChartLabels.POD9095_All));
                series = Series.Last();
                series.ChartType = SeriesChartType.Line;
                series.IsXValueIndexed = false;
                series.YValuesPerPoint = 1;
                series.BorderWidth = 3;
                series.Color = Globals.AlphaOverWhiteToOpaque(Color.FromArgb(50, ChartColors.POD9095Color));
                series.BorderDashStyle = ChartDashStyle.Solid;
                series.IsVisibleInLegend = false;
            }

            if (Series.FindByName(PODThresholdChartLabels.POD90_All) == null)
            {
                //draw POD Line
                Series.Add(new Series(PODThresholdChartLabels.POD90_All));
                series = Series.Last();
                series.ChartType = SeriesChartType.Line;
                series.IsXValueIndexed = false;
                series.YValuesPerPoint = 1;
                series.BorderWidth = 3;
                series.Color = Globals.AlphaOverWhiteToOpaque(Color.FromArgb(50, ChartColors.POD90Color));
                series.BorderDashStyle = ChartDashStyle.Solid;
                series.IsVisibleInLegend = false;
            }

            if (Series.FindByName(PODThresholdChartLabels.a9095Area) == null)
            {
                //add a90_95 area
                Series.Add(PODThresholdChartLabels.a9095Area);
                series = Series[PODThresholdChartLabels.a9095Area];
                series.ChartType = SeriesChartType.Area;
                series.IsXValueIndexed = false;
                series.YValuesPerPoint = 1;
                series.BorderWidth = 2;
                series.BorderDashStyle = ChartDashStyle.Solid;
                series.Color = System.Drawing.Color.FromArgb(ChartColors.AreaAlpha, ChartColors.a9095Color);
                //series.SmartLabelStyle.AllowOutsidePlotArea = LabelOutsidePlotAreaStyle.No;
                series.SmartLabelStyle.MovingDirection = LabelAlignmentStyles.TopRight;
                series.SmartLabelStyle.CalloutStyle = LabelCalloutStyle.Box;
                series.SmartLabelStyle.CalloutBackColor = Color.White;
                series.SmartLabelStyle.CalloutLineAnchorCapStyle = LineAnchorCapStyle.None;
                //series.SmartLabelStyle.CalloutLineWidth = 0;
            }

            if (Series.FindByName(PODThresholdChartLabels.a9095Line) == null)
            {
                //add a90_95 line
                Series.Add(PODThresholdChartLabels.a9095Line);
                series = Series[PODThresholdChartLabels.a9095Line];
                series.ChartType = SeriesChartType.Line;
                series.IsXValueIndexed = false;
                series.YValuesPerPoint = 1;
                series.BorderWidth = 2;
                series.BorderDashStyle = ChartDashStyle.Dot;
                series.Color = System.Drawing.Color.FromArgb(ChartColors.LineAlpha, ChartColors.a9095Color);
                //series.SmartLabelStyle.AllowOutsidePlotArea = LabelOutsidePlotAreaStyle.No;
                series.SmartLabelStyle.MovingDirection = LabelAlignmentStyles.TopRight;
                series.SmartLabelStyle.CalloutStyle = LabelCalloutStyle.Box;
                series.SmartLabelStyle.CalloutBackColor = Color.White;
                series.SmartLabelStyle.CalloutLineAnchorCapStyle = LineAnchorCapStyle.None;
                //series.SmartLabelStyle.CalloutLineWidth = 0;
            }

            if (Series.FindByName(PODThresholdChartLabels.a90Area) == null)
            {
                //add a90 area
                Series.Add(PODThresholdChartLabels.a90Area);
                series = Series[PODThresholdChartLabels.a90Area];
                series.ChartType = SeriesChartType.Area;
                series.IsXValueIndexed = false;
                series.YValuesPerPoint = 1;
                series.BorderWidth = 2;
                series.BorderDashStyle = ChartDashStyle.Solid;
                series.Color = System.Drawing.Color.FromArgb(ChartColors.AreaAlpha, ChartColors.a90Color);
                //series.SmartLabelStyle.AllowOutsidePlotArea = LabelOutsidePlotAreaStyle.No;
                series.SmartLabelStyle.MovingDirection = LabelAlignmentStyles.TopRight;
                series.SmartLabelStyle.CalloutStyle = LabelCalloutStyle.Box;
                series.SmartLabelStyle.CalloutBackColor = Color.White;
                series.SmartLabelStyle.CalloutLineAnchorCapStyle = LineAnchorCapStyle.None;
                //series.SmartLabelStyle.CalloutLineWidth = 0;
            }

            if (Series.FindByName(PODThresholdChartLabels.a90Line) == null)
            {
                //add a90 line
                Series.Add(PODThresholdChartLabels.a90Line);
                series = Series[PODThresholdChartLabels.a90Line];
                series.ChartType = SeriesChartType.Line;
                series.IsXValueIndexed = false;
                series.YValuesPerPoint = 1;
                series.BorderWidth = 2;
                series.BorderDashStyle = ChartDashStyle.Dot;
                series.Color = System.Drawing.Color.FromArgb(ChartColors.LineAlpha, ChartColors.a90Color);
                //series.SmartLabelStyle.AllowOutsidePlotArea = LabelOutsidePlotAreaStyle.No;
                series.SmartLabelStyle.MovingDirection = LabelAlignmentStyles.TopRight;
                series.SmartLabelStyle.CalloutStyle = LabelCalloutStyle.Box;
                series.SmartLabelStyle.CalloutBackColor = Color.White;
                series.SmartLabelStyle.CalloutLineAnchorCapStyle = LineAnchorCapStyle.None;
                //series.SmartLabelStyle.CalloutLineWidth = 0;
            }

            if (Series.FindByName(PODThresholdChartLabels.a50Area) == null)
            {
                //add a50 area
                Series.Add(PODThresholdChartLabels.a50Area);
                series = Series[PODThresholdChartLabels.a50Area];
                series.ChartType = SeriesChartType.Area;
                series.IsXValueIndexed = false;
                series.YValuesPerPoint = 1;
                series.BorderWidth = 2;
                series.BorderDashStyle = ChartDashStyle.Solid;
                series.Color = System.Drawing.Color.FromArgb(ChartColors.AreaAlpha, ChartColors.a50Color);
                //series.SmartLabelStyle.AllowOutsidePlotArea = LabelOutsidePlotAreaStyle.No;
                series.SmartLabelStyle.MovingDirection = LabelAlignmentStyles.TopRight;
                series.SmartLabelStyle.CalloutStyle = LabelCalloutStyle.Box;
                series.SmartLabelStyle.CalloutBackColor = Color.White;
                series.SmartLabelStyle.CalloutLineAnchorCapStyle = LineAnchorCapStyle.None;
                //series.SmartLabelStyle.CalloutLineWidth = 0;
            }

            if (Series.FindByName(PODThresholdChartLabels.a50Line) == null)
            {
                //add a50 line
                Series.Add(PODThresholdChartLabels.a50Line);
                series = Series[PODThresholdChartLabels.a50Line];
                series.ChartType = SeriesChartType.Line;
                series.IsXValueIndexed = false;
                series.YValuesPerPoint = 1;
                series.BorderWidth = 2;
                series.BorderDashStyle = ChartDashStyle.Dot;
                series.Color = System.Drawing.Color.FromArgb(ChartColors.LineAlpha, ChartColors.a50Color);
                //series.SmartLabelStyle.AllowOutsidePlotArea = LabelOutsidePlotAreaStyle.No;
                series.SmartLabelStyle.MovingDirection = LabelAlignmentStyles.TopRight;
                series.SmartLabelStyle.CalloutStyle = LabelCalloutStyle.Box;
                series.SmartLabelStyle.CalloutBackColor = Color.White;
                series.SmartLabelStyle.CalloutLineAnchorCapStyle = LineAnchorCapStyle.None;
                //series.SmartLabelStyle.CalloutLineWidth = 0;
            }

            if (Series.FindByName(PODThresholdChartLabels.POD9095) == null)
            {
                //draw the 90/95 line
                Series.Add(new Series(PODThresholdChartLabels.POD9095));
                series = Series.Last();
                series.ChartType = SeriesChartType.Line;
                series.IsXValueIndexed = false;
                series.YValuesPerPoint = 1;
                series.BorderWidth = 3;
                series.Color = ChartColors.POD9095Color;
                series.BorderDashStyle = ChartDashStyle.Solid;
                series.IsVisibleInLegend = false;
            }

            if (Series.FindByName(PODThresholdChartLabels.POD90) == null)
            {
                //draw POD Line
                Series.Add(new Series(PODThresholdChartLabels.POD90));
                series = Series.Last();
                series.ChartType = SeriesChartType.Line;
                series.IsXValueIndexed = false;
                series.YValuesPerPoint = 1;
                series.BorderWidth = 3;
                series.Color = ChartColors.POD90Color;
                series.BorderDashStyle = ChartDashStyle.Solid;
                series.IsVisibleInLegend = false;
            }

            if (Series.FindByName(PODThresholdChartLabels.ThresholdLine) == null)
            {
                //draw Threshold Line
                Series.Add(new Series(PODThresholdChartLabels.ThresholdLine));
                series = Series.Last();
                series.ChartType = SeriesChartType.Line;
                series.IsXValueIndexed = false;
                series.YValuesPerPoint = 1;
                series.BorderWidth = 2;
                series.Color = ChartColors.ThresholdColor;
                series.BorderDashStyle = ChartDashStyle.Dash;
                series.IsVisibleInLegend = false;
            }

            ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            ChartAreas[0].AxisX.MajorGrid.Enabled = false;

            //Titles[0].Text = "Threshold Plot";
            //ChartAreas[0].AxisX.Title = "Decision Threshold";
            //ChartAreas[0].AxisY.Title = "Crack Size (mils)";

            XAxisNameIsUnitlessConstant = true;
            YAxisNameIsUnitlessConstant = true;
            XAxisTitle = "POD Threshold";
            SingleSeriesCount = 9;
            YAxisTitle = flawName + "(" + flawUnit + ")"; 
            ChartTitle = "Threshold Curve";
        }

        public Series POD90
        {
            get
            {
                return Series[PODThresholdChartLabels.POD90];
            }
        }

        public Series POD9095
        {
            get
            {
                return Series[PODThresholdChartLabels.POD9095];
            }
        }

        public Series POD90_All
        {
            get
            {
                return Series[PODThresholdChartLabels.POD90_All];
            }
        }

        public Series POD9095_All
        {
            get
            {
                return Series[PODThresholdChartLabels.POD9095_All];
            }
        }

        public Series a50Area
        {
            get
            {
                return Series[PODThresholdChartLabels.a50Area];
            }
        }

        public Series a90Area
        {
            get
            {
                return Series[PODThresholdChartLabels.a90Area];
            }
        }

        public Series a9095Area
        {
            get
            {
                return Series[PODThresholdChartLabels.a9095Area];
            }
        }

        public Series a50Line
        {
            get
            {
                return Series[PODThresholdChartLabels.a50Line];
            }
        }

        public Series a90Line
        {
            get
            {
                return Series[PODThresholdChartLabels.a90Line];
            }
        }

        public Series a9095Line
        {
            get
            {
                return Series[PODThresholdChartLabels.a9095Line];
            }
        }

        public Series ThresholdLine
        {
            get
            {
                return Series[PODThresholdChartLabels.ThresholdLine];
            }
        }

        public void FillChart(AnalysisData myData)
        {
            DataView view = myData.ThresholdPlotTable.DefaultView;

            POD90.Points.DataBindXY(view, "threshold", view, "level");
            POD9095.Points.DataBindXY(view, "threshold", view, "confidence");

            DataView all_view = myData.ThresholdPlotTable_All.DefaultView;
            POD90_All.Points.DataBindXY(all_view, "threshold", all_view, "level");
            POD9095_All.Points.DataBindXY(all_view, "threshold", all_view, "confidence");

            //no point in plotting a 90/95 that couldn't be calculated (zeroed out by analysis code)
            if (POD9095.Points.Count > 0 && POD9095.Points.First().YValues[0] + POD9095.Points.Last().YValues[0] == 0.0)
            {
                POD9095.Points.Clear();
                POD9095_All.Points.Clear();
            }
        }

        public override string TooltipText
        {
            get
            {
                var text = "";

                text += "Threshold Parameters" + Environment.NewLine;
                text += "POD Decision:\t" + _threshold.ToString("F3") + " " + YAxisUnit + Environment.NewLine;
                text += "a50:\t\t" + _a50.ToString("F3") + " " + XAxisUnit + Environment.NewLine;
                text += "a90:\t\t" + _a90.ToString("F3") + " " + XAxisUnit + Environment.NewLine;
                text += "a90/95:\t\t" + _a9095.ToString("F3") + " " + XAxisUnit + Environment.NewLine;

                return text;
            }
        }

        public void UpdateLevelConfidenceLines(double myA50, double myA90, double myA9095, double myThreshold)
        {
            Series a90AreaData = a90Area;
            Series a9095AreaData = a9095Area;
            Series a50AreaData = a50Area;
            Series thresholdLineData = ThresholdLine;
            Series a90LineData = a90Line;
            Series a9095LineData = a9095Line;
            Series a50LineData = a50Line;

            double minY = ChartAreas[0].AxisY.Minimum;
            double maxY = ChartAreas[0].AxisY.Maximum;
            double minX = ChartAreas[0].AxisX.Minimum;

            double a50Y = myA50;
            double a90Y = myA90;
            double a9095Y = myA9095;

            double threshold50 = myThreshold;
            double threshold90 = myThreshold;//myA90 * mySlope + myIntercept;
            double threshold9095 = myThreshold;//myA9095 * mySlope + myIntercept;

            _a50 = myA50;
            _a90 = myA90;
            _a9095 = myA9095;
            _threshold = myThreshold;

            Series line;

            line = a50AreaData;

            line.Points.Clear();
            line.Points.AddXY(threshold50, minY);
            line.Points.AddXY(threshold50, a50Y);
            line.Points.AddXY(minX, a50Y);

            line = a50LineData;

            line.Points.Clear();
            line.Points.AddXY(threshold50, a50Y);
            line.Points.AddXY(minX, a50Y);

            line = a90AreaData;

            line.Points.Clear();
            line.Points.AddXY(threshold90, minY);
            line.Points.AddXY(threshold90, a90Y);
            line.Points.AddXY(minX, a90Y);
            //line.Points[0].Label = "90";

            line = a90LineData;

            line.Points.Clear();
            line.Points.AddXY(threshold90, a90Y);
            line.Points.AddXY(minX, a90Y);

            //no point in plotting confidence lines for the 90/95 that couldn't be calculated
            if (POD9095.Points.Count > 0)
            {
                line = a9095AreaData;

                line.Points.Clear();
                line.Points.AddXY(threshold9095, minY);
                line.Points.AddXY(threshold9095, a9095Y);
                line.Points.AddXY(minX, a9095Y);
                //line.Points[0].Label = "90/95";

                line = a9095LineData;

                line.Points.Clear();
                line.Points.AddXY(threshold9095, a9095Y);
                line.Points.AddXY(minX, a9095Y);                
            }
            else
            {
                a9095AreaData.Points.Clear();
                a9095LineData.Points.Clear();
            }

            line = thresholdLineData;

            line.Points.Clear();
            line.Points.AddXY(myThreshold, minY);
            line.Points.AddXY(myThreshold, maxY);

        }
    }
}
