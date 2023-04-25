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
    public partial class PODChart : DataPointChart
    {
        double _a50 = 0.0;
        double _a90 = 0.0;
        double _a9095 = 0.0;

        public PODChart()
        {
            InitializeComponent();

            //all charts should be added through SetupChart()
            Series.Clear();

            DoPostPaint = true;

            //PrePaint += LineChartPrePaint;
            //PostPaint += LineChartPostPaint;
        }

        public override void SetupChart(string flawName, string flawUnit, List<string> responseNames, List<string> responseUnits)
        {
            base.SetupChart(flawName, flawUnit, responseNames, responseUnits);

            Series series;

            DoPostPaint = true;

            if (Series.FindByName(PODChartLabels.POD9095_All) == null)
            {
                //draw the 90/95 line
                Series.Add(new Series(PODChartLabels.POD9095_All));
                series = Series.Last();
                series.ChartType = SeriesChartType.Line;
                series.MarkerStyle = MarkerStyle.None;
                //series.BorderDashStyle = ChartDashStyle.Dot;
                series.MarkerSize = 0;
                series.IsXValueIndexed = false;
                series.YValuesPerPoint = 1;
                series.BorderWidth = 3;
                series.Color = Globals.AlphaOverWhiteToOpaque(Color.FromArgb(50, ChartColors.POD9095Color));
                series.BorderDashStyle = ChartDashStyle.Solid;
                series.IsVisibleInLegend = false;
            }

            if (Series.FindByName(PODChartLabels.POD_All) == null)
            {
                //draw POD Line
                Series.Add(new Series(PODChartLabels.POD_All));
                series = Series.Last();
                series.ChartType = SeriesChartType.Line;
                series.MarkerStyle = MarkerStyle.None;
                //series.BorderDashStyle = ChartDashStyle.Dot;
                series.MarkerSize = 0;
                series.IsXValueIndexed = false;
                series.YValuesPerPoint = 1;
                series.BorderWidth = 3;
                series.Color = Globals.AlphaOverWhiteToOpaque(Color.FromArgb(50, ChartColors.POD90Color));
                series.BorderDashStyle = ChartDashStyle.Solid;
                series.IsVisibleInLegend = false;
            }

            if (Series.FindByName(PODChartLabels.a9095Area) == null)
            {
                //add a90_95 area under the curve
                Series.Add(PODChartLabels.a9095Area);
                series = Series.Last();
                series.ChartType = SeriesChartType.Area;
                series.IsXValueIndexed = false;
                series.YValuesPerPoint = 1;
                series.BorderWidth = 2;
                series.BorderDashStyle = ChartDashStyle.Solid;
                series.Color = Color.FromArgb(ChartColors.AreaAlpha, ChartColors.a9095Color);
                //series.SmartLabelStyle.AllowOutsidePlotArea = LabelOutsidePlotAreaStyle.No;
                series.SmartLabelStyle.MovingDirection = LabelAlignmentStyles.TopRight;
                series.SmartLabelStyle.CalloutStyle = LabelCalloutStyle.Box;
                series.SmartLabelStyle.CalloutBackColor = Color.White;
                series.SmartLabelStyle.CalloutLineAnchorCapStyle = LineAnchorCapStyle.None;
                //series.SmartLabelStyle.CalloutLineWidth = 0;
            }

            if (Series.FindByName(PODChartLabels.a9095Line) == null)
            {
                //add a90_95 line
                Series.Add(PODChartLabels.a9095Line);
                series = Series.Last();
                series.ChartType = SeriesChartType.Line;
                series.IsXValueIndexed = false;
                series.YValuesPerPoint = 1;
                series.BorderWidth = 2;
                series.BorderDashStyle = ChartDashStyle.Dot;
                series.Color = Color.FromArgb(ChartColors.LineAlpha, ChartColors.a9095Color);
                //series.SmartLabelStyle.AllowOutsidePlotArea = LabelOutsidePlotAreaStyle.No;
                series.SmartLabelStyle.MovingDirection = LabelAlignmentStyles.TopRight;
                series.SmartLabelStyle.CalloutStyle = LabelCalloutStyle.Box;
                series.SmartLabelStyle.CalloutBackColor = Color.White;
                series.SmartLabelStyle.CalloutLineAnchorCapStyle = LineAnchorCapStyle.None;
                //series.SmartLabelStyle.CalloutLineWidth = 0;
            }

            if (Series.FindByName(PODChartLabels.a90Area) == null)
            {
                //add a90 area under the curve
                Series.Add(PODChartLabels.a90Area);
                series = Series.Last();
                series.ChartType = SeriesChartType.Area;
                series.IsXValueIndexed = false;
                series.YValuesPerPoint = 1;
                series.BorderWidth = 2;
                series.BorderDashStyle = ChartDashStyle.Solid;
                series.Color = Color.FromArgb(ChartColors.AreaAlpha, ChartColors.a90Color);
                //series.SmartLabelStyle.AllowOutsidePlotArea = LabelOutsidePlotAreaStyle.No;
                series.SmartLabelStyle.MovingDirection = LabelAlignmentStyles.TopRight;
                series.SmartLabelStyle.CalloutStyle = LabelCalloutStyle.Box;
                series.SmartLabelStyle.CalloutBackColor = Color.White;
                series.SmartLabelStyle.CalloutLineAnchorCapStyle = LineAnchorCapStyle.None;
                //series.SmartLabelStyle.CalloutLineWidth = 0;
            }

            if (Series.FindByName(PODChartLabels.a90Line) == null)
            {
                //add a90 line
                Series.Add(PODChartLabels.a90Line);
                series = Series.Last();
                series.ChartType = SeriesChartType.Line;
                series.IsXValueIndexed = false;
                series.YValuesPerPoint = 1;
                series.BorderWidth = 2;
                series.BorderDashStyle = ChartDashStyle.Dot;
                series.Color = Color.FromArgb(ChartColors.LineAlpha, ChartColors.a90Color);
                //series.SmartLabelStyle.AllowOutsidePlotArea = LabelOutsidePlotAreaStyle.No;
                series.SmartLabelStyle.MovingDirection = LabelAlignmentStyles.TopRight;
                series.SmartLabelStyle.CalloutStyle = LabelCalloutStyle.Box;
                series.SmartLabelStyle.CalloutBackColor = Color.White;
                series.SmartLabelStyle.CalloutLineAnchorCapStyle = LineAnchorCapStyle.None;
                //series.SmartLabelStyle.CalloutLineWidth = 0;
            }

            if (Series.FindByName(PODChartLabels.a50Area) == null)
            {
                //add a50 area under the curve
                Series.Add(PODChartLabels.a50Area);
                series = Series.Last();
                series.ChartType = SeriesChartType.Area;
                series.IsXValueIndexed = false;
                series.YValuesPerPoint = 1;
                series.BorderWidth = 2;
                series.BorderDashStyle = ChartDashStyle.Solid;
                series.Color = Color.FromArgb(ChartColors.AreaAlpha, ChartColors.a50Color);
                //series.SmartLabelStyle.AllowOutsidePlotArea = LabelOutsidePlotAreaStyle.No;
                series.SmartLabelStyle.MovingDirection = LabelAlignmentStyles.TopRight;
                series.SmartLabelStyle.CalloutStyle = LabelCalloutStyle.Box;
                series.SmartLabelStyle.CalloutBackColor = Color.White;
                series.SmartLabelStyle.CalloutLineAnchorCapStyle = LineAnchorCapStyle.None;
                //series.SmartLabelStyle.CalloutLineWidth = 0;
            }

            if (Series.FindByName(PODChartLabels.a50Line) == null)
            {
                //add a50 line
                Series.Add(PODChartLabels.a50Line);
                series = Series.Last();
                series.ChartType = SeriesChartType.Line;
                series.IsXValueIndexed = false;
                series.YValuesPerPoint = 1;
                series.BorderWidth = 2;
                series.BorderDashStyle = ChartDashStyle.Dot;
                series.Color = Color.FromArgb(ChartColors.LineAlpha, ChartColors.a50Color);
                //series.SmartLabelStyle.AllowOutsidePlotArea = LabelOutsidePlotAreaStyle.No;
                series.SmartLabelStyle.MovingDirection = LabelAlignmentStyles.TopRight;
                series.SmartLabelStyle.CalloutStyle = LabelCalloutStyle.Box;
                series.SmartLabelStyle.CalloutBackColor = Color.White;
                series.SmartLabelStyle.CalloutLineAnchorCapStyle = LineAnchorCapStyle.None;
                //series.SmartLabelStyle.CalloutLineWidth = 0;
            }

            if (Series.FindByName(PODChartLabels.a90Horizontal) == null)
            {
                //add a90 horizontal line
                Series.Add(PODChartLabels.a90Horizontal);
                series = Series.Last();
                series.ChartType = SeriesChartType.Line;
                series.IsXValueIndexed = false;
                series.YValuesPerPoint = 1;
                series.BorderWidth = 2;
                series.BorderDashStyle = ChartDashStyle.Dash;
                series.Color = Color.FromArgb(ChartColors.LineAlpha, ChartColors.a90Color);
                //series.SmartLabelStyle.AllowOutsidePlotArea = LabelOutsidePlotAreaStyle.No;
                series.SmartLabelStyle.MovingDirection = LabelAlignmentStyles.TopRight;
                series.SmartLabelStyle.CalloutStyle = LabelCalloutStyle.Box;
                series.SmartLabelStyle.CalloutBackColor = Color.White;
                series.SmartLabelStyle.CalloutLineAnchorCapStyle = LineAnchorCapStyle.None;
                //series.SmartLabelStyle.CalloutLineWidth = 0;
            }

            if (Series.FindByName(PODChartLabels.a50Horizontal) == null)
            {
                //add a50 horizontal line
                Series.Add(PODChartLabels.a50Horizontal);
                series = Series.Last();
                series.ChartType = SeriesChartType.Line;
                series.IsXValueIndexed = false;
                series.YValuesPerPoint = 1;
                series.BorderWidth = 2;
                series.BorderDashStyle = ChartDashStyle.Dash;
                series.Color = Color.FromArgb(ChartColors.LineAlpha, ChartColors.a50Color);
                //series.SmartLabelStyle.AllowOutsidePlotArea = LabelOutsidePlotAreaStyle.No;
                series.SmartLabelStyle.MovingDirection = LabelAlignmentStyles.TopRight;
                series.SmartLabelStyle.CalloutStyle = LabelCalloutStyle.Box;
                series.SmartLabelStyle.CalloutBackColor = Color.White;
                series.SmartLabelStyle.CalloutLineAnchorCapStyle = LineAnchorCapStyle.None;
            }

            if (Series.FindByName(PODChartLabels.POD9095) == null)
            {
                //draw the 90/95 line
                Series.Add(new Series(PODChartLabels.POD9095));
                series = Series.Last();
                series.ChartType = SeriesChartType.Line;
                series.MarkerStyle = MarkerStyle.None;
                series.MarkerSize = 0;
                series.IsXValueIndexed = false;
                series.YValuesPerPoint = 1;
                series.BorderWidth = 3;
                series.Color = Color.FromArgb(ChartColors.LineAlpha, ChartColors.POD9095Color);
                series.BorderDashStyle = ChartDashStyle.Solid;
                series.IsVisibleInLegend = false;
            }

            if (Series.FindByName(PODChartLabels.POD) == null)
            {
                //draw POD Line
                Series.Add(new Series(PODChartLabels.POD));
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

        public override void ClearEverythingButPoints()
        {
            base.ClearEverythingButPoints();

            Series[PODChartLabels.POD].Points.Clear();
            Series[PODChartLabels.POD9095].Points.Clear();
            Series[PODChartLabels.POD_All].Points.Clear();
            Series[PODChartLabels.POD9095_All].Points.Clear();
            Series[PODChartLabels.a50Line].Points.Clear();
            Series[PODChartLabels.a50Area].Points.Clear();
            Series[PODChartLabels.a90Line].Points.Clear();
            Series[PODChartLabels.a90Area].Points.Clear();
            Series[PODChartLabels.a9095Line].Points.Clear();
            Series[PODChartLabels.a9095Area].Points.Clear();
        }

        public Series POD
        {
            get
            {
                return Series[PODChartLabels.POD];
            }
        }

        public Series POD9095
        {
            get
            {
                return Series[PODChartLabels.POD9095];
            }
        }

        public Series POD_All
        {
            get
            {
                return Series[PODChartLabels.POD_All];
            }
        }

        public Series POD9095_All
        {
            get
            {
                return Series[PODChartLabels.POD9095_All];
            }
        }

        public override string TooltipText
        {
            get
            {
                var text = "";

                text += "POD Parameters" + Environment.NewLine;
                text += "a50:\t" + _a50.ToString("F3") + " " + XAxisUnit + Environment.NewLine;
                text += "a90:\t" + _a90.ToString("F3") + " " + XAxisUnit + Environment.NewLine;
                text += "a90/95:\t" + _a9095.ToString("F3") + " " + XAxisUnit + Environment.NewLine;

                return text;
            }
        }

        public void FillChart(IAnalysisData myData, bool usingLoadedFromFileData = false, bool analysisFailed=false)
        {

            if (!usingLoadedFromFileData)
            {
                var flaw = myData.PodCurveTable.Columns[0].ColumnName;
                var pod = myData.PodCurveTable.Columns[1].ColumnName;
                var conf = myData.PodCurveTable.Columns[2].ColumnName;
                switch (myData.DataType)
                {
                    case AnalysisDataTypeEnum.AHat:
                        FillChart(myData.PodCurveTable, flaw, pod, flaw, conf, 0.0, 1.0);
                        var view = myData.PodCurveTable_All.DefaultView;
                        POD_All.Points.DataBindXY(view, flaw, view, pod);
                        POD9095_All.Points.DataBindXY(view, flaw, view, conf);
                        break;
                    case AnalysisDataTypeEnum.HitMiss:
                        FillChart(myData.PodCurveTable, flaw, pod, flaw, conf, 0.0, 1.0, analysisFailed);
                        break;
                }
            }
            else
            {
                var flaw = myData.PodCurveTable.Columns[0].ColumnName;
                var pod = myData.PodCurveTable.Columns[1].ColumnName;
                var conf = myData.PodCurveTable.Columns[2].ColumnName;

                switch (myData.DataType)
                {
                    case AnalysisDataTypeEnum.AHat:
                        FillChart(myData.PodCurveTable, flaw, pod, flaw, conf, 0.0, 1.0);                        
                        break;
                    case AnalysisDataTypeEnum.HitMiss:
                        FillChart(myData.PodCurveTable, flaw, pod, flaw, conf, 0.0, 1.0, analysisFailed);
                        break;
                }
            }

            //no point in plotting a 90/95 that couldn't be calculated (zeroed out by analysis code)
            if (POD9095.Points.Count > 0 && POD9095.Points.First().YValues[0] == .5 && POD9095.Points.Last().YValues[0] == 0.5)
            {
                POD9095.Points.Clear();
                POD9095_All.Points.Clear();
            }
        }

        public void FillChart(AnalysisData myData, string my90X, string my90Y, string my95X, string my95Y)
        {
            FillChart(myData.PodCurveTable, my90X, my90Y, my95X, my95Y, 0.0, 1.0);
        }

        public void FillChart(DataTable myTable, string my90X, string my90Y, string my95X, string my95Y, double myYMin, double myYMax, bool analysisFailed = false)
        {
            DataView view = myTable.DefaultView;

            POD.Points.DataBindXY(view, my90X, view, my90Y);
            //only plot the confidence interval curve if the analysis doesn't fail
            if(!analysisFailed)
                POD9095.Points.DataBindXY(view, my95X, view, my95Y);

            

            ChartAreas[0].AxisY.Minimum = myYMin;
            ChartAreas[0].AxisY.Maximum = myYMax;

            Series[PODChartLabels.a50Area].Points.Clear();
            Series[PODChartLabels.a90Area].Points.Clear();
            Series[PODChartLabels.a9095Area].Points.Clear();
            Series[PODChartLabels.a50Line].Points.Clear();
            Series[PODChartLabels.a90Line].Points.Clear();
            Series[PODChartLabels.a9095Line].Points.Clear();
            Series[PODChartLabels.a50Horizontal].Points.Clear();
            Series[PODChartLabels.a90Horizontal].Points.Clear();
        }

        public void UpdateLevelConfidenceLines(double myA50, double myA90, double myA90_95)
        {
            Series a50Area = Series[PODChartLabels.a50Area];
            Series a90Area = Series[PODChartLabels.a90Area];
            Series a9095Area = Series[PODChartLabels.a9095Area];
            Series a50Line = Series[PODChartLabels.a50Line];
            Series a90Line = Series[PODChartLabels.a90Line];
            Series a9095Line = Series[PODChartLabels.a9095Line];
            Series a50Horizontal = Series[PODChartLabels.a50Horizontal];
            Series a90Horizontal = Series[PODChartLabels.a90Horizontal];

            double minY = ChartAreas[0].AxisY.Minimum;
            double minX = ChartAreas[0].AxisX.Minimum;
            double maxX = ChartAreas[0].AxisX.Maximum;

            double a50Y = .5;
            double a90Y = .9;
            double a90_95Y = .9;

            Series line = a50Area;

            _a50 = myA50;
            _a90 = myA90;
            _a9095 = myA90_95;

            line.Points.Clear();

            for (int i = 0; i < POD.Points.Count; i++)
            {
                if (POD.Points[i].YValues[0] > .5)
                    break;
                else
                {
                    line.Points.AddXY(POD.Points[i].XValue, POD.Points[i].YValues[0]);
                }
            }

            line.Points.AddXY(myA50, a50Y);

            line = a50Line;

            line.Points.Clear();
            line.Points.AddXY(myA50, minY);
            line.Points.AddXY(myA50, a50Y);
            //line.Points.AddXY(minX, a50Y);
            //line.Points[0].Label = "50";*/

            //List<double> yValues = new List<double>();
            //List<double> xValues = new List<double>();

            line = a90Area;
            line.Points.Clear();

            for (int i = 0; i < POD.Points.Count; i++ )
            {
                if (POD.Points[i].YValues[0] > a90Y)
                    break;
                else
                {
                    line.Points.AddXY(POD.Points[i].XValue, POD.Points[i].YValues[0]);
                }
            }

            line.Points.AddXY(myA90, a90Y);

            line = a90Line;

            line.Points.Clear();

            line.Points.AddXY(myA90, minY);
            line.Points.AddXY(myA90, a90Y);
            //line.Points.AddXY(minX, a90Y);
            //line.Points[0].Label = "90";*/

            //clear these now because they might not get used
            a9095Area.Points.Clear();
            a9095Line.Points.Clear();

            //if 90/95 curve was calculated by the analysis
            if (POD9095.Points.Count > 0 && POD.Points.Count == POD.Points.Count)
            {
                line = a9095Area;

                for (int i = 0; i < POD.Points.Count; i++)
                {
                    if (POD9095.Points[i].YValues[0] > a90_95Y)
                        break;
                    else
                    {
                        double value = POD.Points[i].YValues[0];
                        if (value > a90_95Y)
                            value = a90_95Y;
                        line.Points.AddXY(POD.Points[i].XValue, value);
                    }
                }


                line.Points.AddXY(myA90_95, a90_95Y);

                line = a9095Line;

                line.Points.Clear();
                line.Points.AddXY(myA90_95, minY);
                line.Points.AddXY(myA90_95, a90_95Y);
                //line.Points.AddXY(minX, a90_95Y);
                //line.Points[0].Label = "90/95";*/
            }

            line = a50Horizontal;

            line.Points.Clear();
            line.Points.AddXY(minX, .5);
            line.Points.AddXY(maxX, .5);

            line = a90Horizontal;

            line.Points.Clear();
            line.Points.AddXY(minX, .9);
            line.Points.AddXY(maxX, .9);

        }

        
    }
}
