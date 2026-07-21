// <copyright file="QQChart.cs" company="The University of Dayton">
// Copyright © 2026 The University of Dayton. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using POD.Data; // Assuming this is where your ChartColors exist
using POD;

namespace POD.Controls
{
    public partial class QQChart : DataPointChart
    {
        private IAnalysisData _storedData = null;

        public QQChart()
        {
            InitializeComponent();

            // All charts should be added through SetupChart()
            Series.Clear();
        }

        public override void SetupChart(string flawName, string flawUnit, List<string> responseNames, List<string> responseUnits)
        {
            base.SetupChart(flawName, flawUnit, responseNames, responseUnits);

            Series series;

            // 1. Setup the 45-Degree Reference Line
            if (Series.FindByName(POD.QQChart.ReferenceLine) == null)
            {
                Series.Add(new Series(POD.QQChart.ReferenceLine));
                series = Series.Last();
                series.ChartType = SeriesChartType.Line;
                series.IsVisibleInLegend = false;
                series.IsXValueIndexed = false;
                series.YValuesPerPoint = 1;
                series.BorderWidth = 1;
                series.Color = Color.FromArgb(ChartColors.LineAlpha, ChartColors.FitColor);
                series.BorderDashStyle = ChartDashStyle.Solid;
            }

            // 2. Setup the Q-Q Scatter Points
            if (Series.FindByName(POD.QQChart.QQPoints) == null)
            {
                Series.Add(new Series(POD.QQChart.QQPoints));
                series = Series.Last();
                series.ChartType = SeriesChartType.Point;
                series.IsVisibleInLegend = false;
                series.YValuesPerPoint = 1;
                series.Color = Color.FromArgb(ChartColors.ModelCompareAlpha, ChartColors.UncensoredPoints);
                series.MarkerStyle = MarkerStyle.Circle;
                series.MarkerSize = 4;
            }

            // 3. Setup the Lower 95% Confidence Band
            if (Series.FindByName(POD.QQChart.LowerConfidence) == null)
            {
                Series.Add(new Series(POD.QQChart.LowerConfidence));
                series = Series.Last();
                series.ChartType = SeriesChartType.Line;
                series.IsVisibleInLegend = false;
                series.IsXValueIndexed = false;
                series.YValuesPerPoint = 1;
                series.BorderWidth = 1;
                series.Color = Color.DarkGray;
                series.BorderDashStyle = ChartDashStyle.Dash;
            }

            // 4. Setup the Upper 95% Confidence Band
            if (Series.FindByName(POD.QQChart.UpperConfidence) == null)
            {
                Series.Add(new Series(POD.QQChart.UpperConfidence));
                series = Series.Last();
                series.ChartType = SeriesChartType.Line;
                series.IsVisibleInLegend = false;
                series.IsXValueIndexed = false;
                series.YValuesPerPoint = 1;
                series.BorderWidth = 1;
                series.Color = Color.DarkGray;
                series.BorderDashStyle = ChartDashStyle.Dash;
            }

            // 5. Grid and UI Cleanup
            ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            ChartAreas[0].AxisX.MinorGrid.Enabled = false;
            ChartAreas[0].AxisY.MinorGrid.Enabled = false;

            this.SingleSeriesCount = 4;

            // 6. Standard Statistical Q-Q Labels
            XAxisNameIsUnitlessConstant = true;
            YAxisNameIsUnitlessConstant = true;
            XAxisTitle = "Theoretical (Gaussian) Quantiles";
            YAxisTitle = "Sample (Residual) Quantiles";
            ForceZeroXAxis = false;
            ForceZeroYAxis = false;
            ChartTitle = "Normal Q-Q Plot";

            ChartAreas[0].AxisX.LabelStyle.Format = "N0";
            ChartAreas[0].AxisY.LabelStyle.Format = "N0";
        }

        public void FillChart(IAnalysisData myData, bool usingLoadedFromFileData = false, bool analysisFailed = false)
        {
            // Reset transformers to linear for the Q-Q plot since standard residuals are already transformed in R
            _transformer.UpdateAxisTransformParameters(null, TransformTypeEnum.Linear, 0.0, 1.0, null, TransformTypeEnum.Linear, 0.0, 1.0);

            // Safety guard: if the analysis failed or the table doesn't exist, clear and abort
            if (analysisFailed || myData.QQTable == null || myData.QQTable.Rows.Count == 0)
            {
                ClearEverythingButPoints();
                return;
            }

            // Map strictly to the statistician's exact table headers
            var xCol = "theoretical_x";
            var yCol = "sample_y";
            var refCol = "reference_line";
            var lowerCol = "lower_ci_95";
            var upperCol = "upper_ci_95";

            DataView view = myData.QQTable.DefaultView;

            // 1. Bind the Actual Residual Scatter Points
            QQPoints.Points.DataBindXY(view, xCol, view, yCol);

            // 2. Bind the Baseline Normal Reference Line (now driven directly by R)
            ReferenceLine.Points.DataBindXY(view, xCol, view, refCol);

            // 3. Bind the 95% Confidence Intervals
            LowerConfidence.Points.DataBindXY(view, xCol, view, lowerCol);
            UpperConfidence.Points.DataBindXY(view, xCol, view, upperCol);

            // Cache the data reference for the OnResize override method
            _storedData = myData;

            // Apply the clean, single-source-of-truth helper scaling method
            ApplySymmetricalScale(myData);
        }

        public override void ClearEverythingButPoints()
        {
            base.ClearEverythingButPoints();
            ReferenceLine.Points.Clear();
            QQPoints.Points.Clear();
            LowerConfidence.Points.Clear();
            UpperConfidence.Points.Clear();
        }

        public Series QQPoints => Series[POD.QQChart.QQPoints];
        public Series ReferenceLine => Series[POD.QQChart.ReferenceLine];
        public Series LowerConfidence => Series[POD.QQChart.LowerConfidence];
        public Series UpperConfidence => Series[POD.QQChart.UpperConfidence];

        public override string TooltipText
        {
            get
            {
                var text = "Normal Q-Q Plot\n";
                text += "Evaluates if the model's errors are normally distributed.\n";
                text += "If the data is normal, points will closely hug the straight diagonal line.";
                return text;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            // 1. Let the base class resize and accidentally break the intervals
            base.OnResize(e);
            ApplySymmetricalScale(_storedData);
        }

        protected override void ApplyDynamicFontScaling()
        {
            if (ChartAreas.Count > 0 && this.Width > 0 && this.Height > 0)
            {
                // 1. Find the shortest edge
                float minDimension = Math.Min(this.Width, this.Height);

                if (minDimension < 200)
                {
                    XAxisTitle = "Gaussian Quantiles";
                    YAxisTitle = "Residual Quantiles";
                }
                else
                {
                    XAxisTitle = "Theoretical (Gaussian) Quantiles";
                    YAxisTitle = "Sample (Residual) Quantiles";
                }

                // 2. Baseline math: 8pt font at 300px
                float calculatedSize = (minDimension / 300f) * 8f;

                // 3. Clamp the sizes so titles never get unreadable or comically huge
                float titleFontSize = Math.Max(7.5f, Math.Min(11f, calculatedSize));

                var titleFont = new Font("Segoe UI", titleFontSize, FontStyle.Regular);

                // 4. Apply dynamically ONLY to Axis Titles
                ChartAreas[0].AxisX.TitleFont = titleFont;
                ChartAreas[0].AxisY.TitleFont = titleFont;

                // (Tick mark labels - LabelStyle.Font - are intentionally left untouched 
                //  so the data numbers remain at a static, highly readable size.)

                // 5. Apply to Chart Title (Proportionally larger and Bold)
                if (Titles.Count > 0)
                    Titles[0].Font = new Font("Segoe UI", titleFontSize + 1.5f, FontStyle.Regular);
            }
        }

        private void ApplySymmetricalScale(IAnalysisData myData)
        {
            // Guard check to protect against uninitialized data or early resize triggers
            if (myData?.QQTable == null || myData.QQTable.Rows.Count == 0)
                return;

            // 1. Extract the raw mins and maxes using her exact column names
            double minX = Convert.ToDouble(myData.QQTable.Compute("MIN(theoretical_x)", string.Empty));
            double maxX = Convert.ToDouble(myData.QQTable.Compute("MAX(theoretical_x)", string.Empty));

            double minY = Convert.ToDouble(myData.QQTable.Compute("MIN(sample_y)", string.Empty));
            double maxY = Convert.ToDouble(myData.QQTable.Compute("MAX(sample_y)", string.Empty));

            double minLower = Convert.ToDouble(myData.QQTable.Compute("MIN(lower_ci_95)", string.Empty));
            double maxUpper = Convert.ToDouble(myData.QQTable.Compute("MAX(upper_ci_95)", string.Empty));

            // 2. Find the absolute single largest value on the entire grid
            double maxAbsoluteValue = Math.Max(
                Math.Max(Math.Max(Math.Abs(minX), Math.Abs(maxX)), Math.Max(Math.Abs(minY), Math.Abs(maxY))),
                Math.Max(Math.Abs(minLower), Math.Abs(maxUpper))
            );

            // 3. Round UP to the nearest 0.5 step (e.g., 2.1 -> 2.5, 2.8 -> 3.0)
            double limit = Math.Ceiling(maxAbsoluteValue * 2.0) / 2.0;

            // 4. Calculate the offset for the X-axis (since base class doesn't auto-center X)
            double offsetX = limit % 1.0;

            // The base class automatically calculates and applies this exact offset for the Y-axis 
            // because 'myCenterYAtZero' is permanently true in SetYAxisRange. 
            // Passing it here causes a double-shift, so we leave it at 0!
            double offsetY = 0.0;

            // 5. Force a perfectly mirrored range from -limit to +limit with whole-number ticks
            AxisObject xAxis = new AxisObject(AxisKind.X) { Min = -limit, Max = limit, Interval = 1.0, IntervalOffset = offsetX };
            AxisObject yAxis = new AxisObject(AxisKind.Y) { Min = -limit, Max = limit, Interval = 1.0, IntervalOffset = offsetY };

            // 6. Hand off to the base class labeling framework
            SetXAxisRange(xAxis, myData, forceLinear: true, keepLabelCount: false, transformResidView: true);
            SetYAxisRange(yAxis, myData, forceLinear: true, keepLabelCount: false, transformResidView: true);
        }

        public override void PrepareCloneSeries(DataPointChart clone, double finalMinY)
        {
            base.PrepareCloneSeries(clone, finalMinY);

            // Grab the heavy line width the export engine just applied
            int exportLineWidth = clone.Series[POD.QQChart.ReferenceLine].BorderWidth;

            // Calculate a much thinner weight (max 1 or 2) so the dashes don't turn into ugly blocks
            int subtleLineWidth = Math.Max(1, exportLineWidth / 2);

            // Force all three dashed lines to stay thin on the exported image
            if (clone.Series.IndexOf(POD.QQChart.ReferenceLine) != -1)
                clone.Series[POD.QQChart.ReferenceLine].BorderWidth = subtleLineWidth;

            if (clone.Series.IndexOf(POD.QQChart.LowerConfidence) != -1)
                clone.Series[POD.QQChart.LowerConfidence].BorderWidth = subtleLineWidth;

            if (clone.Series.IndexOf(POD.QQChart.UpperConfidence) != -1)
                clone.Series[POD.QQChart.UpperConfidence].BorderWidth = subtleLineWidth;

            clone.XAxisTitle = "Theoretical (Gaussian) Quantiles";
            clone.YAxisTitle = "Sample (Residual) Quantiles";
        }
    }
}