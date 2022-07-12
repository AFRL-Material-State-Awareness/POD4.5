using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POD.Data;
using System.Data;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;

namespace POD.Controls
{
    public class TransformChart : DataPointChart
    {
        List<MarkerStyle> _styles = new List<MarkerStyle>();
        List<Series> _normalSeries = new List<Series>();
        bool _showResiduals = false;
        AxisObject _residualXAxis = null;
        AxisObject _residualYAxis = null;
        AxisObject _normalXAxis = null;
        AxisObject _normalYAxis = null;
        private TransformTypeEnum xTransform;
        private TransformTypeEnum yTransform;
        private PFModelEnum hitmissModel;
        private string _originalTitle;
        private bool _compareModels = false;

        public string OriginalYAxisTitle
        {
            set { _originalTitle = value; }
        }

        public TransformChart()
        {
            _styles.Add(MarkerStyle.Circle);
            _styles.Add(MarkerStyle.Diamond);
            _styles.Add(MarkerStyle.Square);
            _styles.Add(MarkerStyle.Triangle);
            _styles.Add(MarkerStyle.Cross);

            AddAllSeries();

            PrePaint += LineChartPrePaint;
            PostPaint += LineChartPostPaint;
        }

        public override void SetupChart(string flawName, string flawUnit, List<string> responseNames, List<string> responseUnits)
        {
            AutoNameYAxis = true;

            XAxisTitle = flawName;
            XAxisUnit = flawUnit;

            if (responseNames.Count == 1)
            {
                SingleSeriesCount = 5;
                YAxisTitle = responseNames[0];
                YAxisUnit = responseUnits[0];

            }
            else
            {
                var newTitle = "Combined Responses" + " (" + responseUnits[0] + ")";
                YAxisNameIsUnitlessConstant = true;
                YAxisTitle = newTitle;
                OriginalYAxisTitle = newTitle;
            }

            
        }

        private void AddAllSeries()
        {
            Series series = null;

            if (Series.FindByName(LinearityChartLabels.ModelCompare) == null)
            {
                //draw the residual flaw estimate line (basically straight line at 0)
                Series.Add(new Series(LinearityChartLabels.ModelCompare));
                series = Series.Last();
                series.IsVisibleInLegend = false;
                series.ChartType = SeriesChartType.Line;
                series.BorderDashStyle = ChartDashStyle.Dash;
                series.MarkerColor = Color.Transparent;
                series.MarkerStyle = MarkerStyle.None;
                series.IsXValueIndexed = false;
                series.YValuesPerPoint = 1;
                series.BorderWidth = 3;
                series.Color = Globals.AlphaOverWhiteToOpaque(Color.FromArgb(ChartColors.ModelCompareAlpha, ChartColors.FitColor));
            }

            if (Series.FindByName(LinearityChartLabels.FlawResidual) == null)
            {
                //draw the residual flaw estimate line (basically straight line at 0)
                Series.Add(new Series(LinearityChartLabels.FlawResidual));
                series = Series.Last();
                series.IsVisibleInLegend = false;
                series.ChartType = SeriesChartType.Line;
                series.IsXValueIndexed = false;
                series.YValuesPerPoint = 1;
                series.BorderWidth = 3;
                series.Color = Color.FromArgb(ChartColors.LineAlpha, ChartColors.FitColor);
            }

            if (Series.FindByName(LinearityChartLabels.FlawEstimate) == null)
            {
                //draw the residual flaw estimate line (basically straight line at 0)
                Series.Add(new Series(LinearityChartLabels.FlawEstimate));
                series = Series.Last();
                series.IsVisibleInLegend = false;
                series.ChartType = SeriesChartType.Line;
                series.IsXValueIndexed = false;
                series.YValuesPerPoint = 1;
                series.BorderWidth = 3;
                series.Color = Color.FromArgb(ChartColors.LineAlpha, ChartColors.FitColor);
            }

            if (Series.FindByName(LinearityChartLabels.Uncensored) == null)
            {
                //draw residual for uncensored data
                Series.Add(new Series(LinearityChartLabels.Uncensored));
                series = Series.Last();
                series.ChartType = SeriesChartType.Point;
                series.YValuesPerPoint = 1;
                series.Color = Color.FromArgb(ChartColors.ModelCompareAlpha, ChartColors.UncensoredPoints);
                series.MarkerStyle = MarkerStyle.Circle;
                series.MarkerSize = 8;
            }

            if (Series.FindByName(LinearityChartLabels.Original) == null)
            {
                //draw the residual flaw estimate line (basically straight line at 0)
                Series.Add(new Series(LinearityChartLabels.Original));
                series = Series.Last();
                series.IsVisibleInLegend = false;
                series.ChartType = SeriesChartType.Point;
                series.IsXValueIndexed = false;
                series.YValuesPerPoint = 1;
                series.Color = Color.FromArgb(ChartColors.LineAlpha, ChartColors.UncensoredPoints);
                series.MarkerStyle = MarkerStyle.Circle;
                series.MarkerSize = 8;
            }

            //if (Series.FindByName(LinearityChartLabels.PartialCensored) == null)
            //{
            //    //draw residual for partially censored data
            //    Series.Add(new Series(LinearityChartLabels.PartialCensored));
            //    series = Series.Last();
            //    series.ChartType = SeriesChartType.Point;
            //    series.YValuesPerPoint = 1;
            //    series.Color = Color.FromArgb(ChartColors.LineAlpha, ChartColors.SemiCensoredPoints);
            //    series.MarkerStyle = MarkerStyle.Circle;
            //}

            

            _residualXAxis = new AxisObject(); ;
            _residualYAxis = new AxisObject(); ;
            _normalXAxis = new AxisObject(); ;
            _normalYAxis = new AxisObject(); ;
        }

        public void SwitchModelCompare()
        {
            _compareModels = !_compareModels;

            SwitchSeriesForCompare();
        }

        public void SwitchSeriesForCompare()
        {
            ModelCompare.Enabled = _compareModels;
        }

        public void ShowModels()
        {
            _compareModels = true;

            SwitchSeriesForCompare();
        }

        public void HideModels()
        {
            _compareModels = false;

            SwitchSeriesForCompare();
        }

        public void SwitchView()
        {
            _showResiduals = !_showResiduals;

            SwitchSeriesForView();
        }

        public void SwitchToNormal()
        {
            _showResiduals = false;

            SwitchSeriesForView();
        }

        public void SwitchToResidual()
        {
            _showResiduals = true;

            SwitchSeriesForView();
        }

        private void SwitchSeriesForView()
        {
            //foreach (Series series in _normalSeries)
            //    series.Enabled = !_showResiduals;

            Original.Enabled = !_showResiduals;
            FlawEstimate.Enabled = !_showResiduals;

            //PartialCensored.Enabled = _showResiduals;
            Uncensored.Enabled = _showResiduals;
            FlawResidual.Enabled = _showResiduals;

            if (_showResiduals)
            {
                YAxisTitle = "Residuals";
            }
            else
            {
                YAxisTitle = _originalTitle;
            }

            
        }

        public void FillFromAnalysis(AnalysisData data, List<Color> colors, int colorIndex, int styleIndex)
        {
            xTransform = data.FlawTransform;
            yTransform = data.ResponseTransform;

            Series.Clear();

            AddAllSeries();

            addOriginalSeries(data, colors, colorIndex, styleIndex);

            AddFitSeries(data);

            AddResidualSeries(data, colors, colorIndex, styleIndex);
            
        }

        public void FillFromAnalysis(AnalysisData data, PFModelEnum model, List<Color> colors, int colorIndex, int styleIndex)
        {
            xTransform = data.FlawTransform;
            yTransform = TransformTypeEnum.Linear;
            hitmissModel = model;
            
            Series.Clear();

            AddAllSeries();

            addOriginalHitMissSeries(data, colors, colorIndex, styleIndex);

            AddHitMissFitSeries(data);

            //AddHitMissResidualSeries(data, colors, colorIndex, styleIndex);

            ModelCompare.Enabled = _compareModels;
        }

        private void addOriginalHitMissSeries(AnalysisData data, List<Color> colors, int colorIndex, int styleIndex)
        {
            var view = data.OriginalData.AsDataView();

            //Original.Points.DataBindXY(view, "t_flaw", view, "hitrate");
            Original.Points.DataBindXY(view, "flaw", view, "hitrate");

            Original.Enabled = false;

            //var index = 0;

            ColorSeries(colors, colorIndex, styleIndex, Original);

            //XAxisTitle = data.ActivatedFlawName;
            //XAxisUnit = flaws[flawIndex].Unit;

            //YAxisTitle = data.ActivatedResponseNames[index];
            //YAxisUnit = responses[responseIndex].Unit;

            UpdateChartTitle();

            GetAxisRangeForHitMissOriginals(data);

            ModelCompare.Enabled = _compareModels;
        }

        private void addOriginalSeries(AnalysisData data, List<Color> colors, int colorIndex, int styleIndex)
        {
            var view = data.ResidualRawTable.AsDataView();

            Original.Points.DataBindXY(view, "t_flaw", view, "t_response");
            Original.Enabled = false;

            //var index = 0;

            ColorSeries(colors, colorIndex, styleIndex, Original);

            //XAxisTitle = data.ActivatedFlawName;
            //XAxisUnit = flaws[flawIndex].Unit;

            //YAxisTitle = data.ActivatedResponseNames[index];
            //YAxisUnit = responses[responseIndex].Unit;

            UpdateChartTitle();

            GetAxisRangeForOriginals(data);
        }

        private void GetAxisRangeForHitMissOriginals(AnalysisData myData)
        {
            double xMin = myData.InvertTransformedFlaw(myData.UncensoredFlawRangeMin);
            double xMax = myData.InvertTransformedFlaw(myData.UncensoredFlawRangeMax);

            xMin = myData.TransformValueForXAxis(xMin);
            xMax = myData.TransformValueForXAxis(xMax);

            if (xMin > xMax)
            {
                var temp = xMin;
                xMin = xMax;
                xMax = temp;
            }

            _normalYAxis = new AxisObject();
            _normalXAxis = new AxisObject();

            _normalYAxis.Interval = .5;
            _normalYAxis.IntervalOffset = 0.0;
            _normalYAxis.Max = 1.0;
            _normalYAxis.Min = 0.0;
            _normalYAxis.BufferPercentage = 0.0;

            AnalysisData.GetBufferedRange(this, _normalXAxis, xMin, xMax, AxisKind.X);
        }

        private void GetAxisRangeForOriginals(AnalysisData myData)
        {
            double xMin = myData.InvertTransformedFlaw(myData.UncensoredFlawRangeMin);
            double xMax = myData.InvertTransformedFlaw(myData.UncensoredFlawRangeMax);

            xMin = myData.TransformValueForXAxis(xMin);
            xMax = myData.TransformValueForXAxis(xMax);

            if (xMin > xMax)
            {
                var temp = xMin;
                xMin = xMax;
                xMax = temp;
            }

            _normalYAxis = new AxisObject();
            _normalXAxis = new AxisObject();

            var uncensoredMax = Double.NegativeInfinity;
            var uncensoredMin = Double.PositiveInfinity;

            if (Original.Points.Count > 0)
            {
                uncensoredMax = Original.Points.FindMaxByValue("Y1").YValues[0];
                uncensoredMin = Original.Points.FindMinByValue("Y1").YValues[0];
            }

            var globalResponseMax = uncensoredMax;
            var globalResponseMin = uncensoredMin;

            AnalysisData.GetBufferedRange(this, _normalXAxis, xMin, xMax, AxisKind.X);
            AnalysisData.GetBufferedRange(this, _normalYAxis, globalResponseMin, globalResponseMax, AxisKind.Y);
        }

        private void AddHitMissResidualSeries(AnalysisData myData, List<Color> colors, int colorIndex, int styleIndex)
        {

            Series series = null;


            DataView view = myData.ResidualUncensoredTable.DefaultView;
            //Uncensored.Points.DataBindXY(view, "t_flaw", view, "diff");
            Uncensored.Points.DataBindXY(view, "flaw", view, "Confidence_Interval");

            Uncensored.Enabled = false;

            series = Uncensored;
            ColorSeries(colors, colorIndex, styleIndex, series);

            GetAxisRangeForHitMissResiduals(myData);

        }

        private void AddResidualSeries(AnalysisData myData, List<Color> colors, int colorIndex, int styleIndex)
        {

            Series series = null;
            

            DataView view = myData.ResidualRawTable.DefaultView;
            Uncensored.Points.DataBindXY(view, "t_flaw", view, "t_diff");
            Uncensored.Enabled = false;

            series = Uncensored;
            ColorSeries(colors, colorIndex, styleIndex, series);

            GetAxisRangeForResiduals(myData);

        }

        private void GetAxisRangeForHitMissResiduals(AnalysisData myData)
        {
            double xMin = myData.InvertTransformedFlaw(myData.UncensoredFlawRangeMin);
            double xMax = myData.InvertTransformedFlaw(myData.UncensoredFlawRangeMax);

            xMin = myData.TransformValueForXAxis(xMin);
            xMax = myData.TransformValueForXAxis(xMax);

            if (xMin > xMax)
            {
                var temp = xMin;
                xMin = xMax;
                xMax = temp;
            }

            FlawResidual.Points.Clear();

            FlawResidual.Points.AddXY(xMin, 0.0);
            FlawResidual.Points.AddXY(xMax, 0.0);

            _residualYAxis = new AxisObject();
            _residualXAxis = new AxisObject();

            _residualYAxis.Interval = .5;
            _residualYAxis.IntervalOffset = 0.0;
            _residualYAxis.Max = 1.0;
            _residualYAxis.Min = -1.0;
            _residualYAxis.BufferPercentage = 0.0;

            AnalysisData.GetBufferedRange(this, _residualXAxis, xMin, xMax, AxisKind.X);

        }

        private void GetAxisRangeForResiduals(AnalysisData myData)
        {
            double xMin = myData.InvertTransformedFlaw(myData.UncensoredFlawRangeMin);
            double xMax = myData.InvertTransformedFlaw(myData.UncensoredFlawRangeMax);

            xMin = myData.TransformValueForXAxis(xMin);
            xMax = myData.TransformValueForXAxis(xMax);

            if(xMin > xMax)
            {
                var temp = xMin;
                xMin = xMax;
                xMax = temp;
            }

            FlawResidual.Points.Clear();

            FlawResidual.Points.AddXY(xMin, 0.0);
            FlawResidual.Points.AddXY(xMax, 0.0);

            _residualYAxis = new AxisObject();
            _residualXAxis = new AxisObject();

            var uncensoredMax = Double.NegativeInfinity;
            var uncensoredMin = Double.PositiveInfinity;

            if (Uncensored.Points.Count > 0)
            {
                uncensoredMax = Uncensored.Points.FindMaxByValue("Y1").YValues[0];
                uncensoredMin = Uncensored.Points.FindMinByValue("Y1").YValues[0];
            }

            //var censoredMax = Double.NegativeInfinity;
            //var censoredMin = Double.PositiveInfinity;

            //if (PartialCensored.Points.Count > 0)
            //{
            //    censoredMax = PartialCensored.Points.FindMaxByValue("Y1").YValues[0];
            //    censoredMin = PartialCensored.Points.FindMinByValue("Y1").YValues[0];
            //}

            //var globalResponseMax = (uncensoredMax > censoredMax) ? uncensoredMax : censoredMax;
            //var globalResponseMin = (uncensoredMin < censoredMin) ? uncensoredMin : censoredMin;

            //if (globalResponseMax < globalResponseMin)
            //{
            //    globalResponseMax = 1.0;
            //    globalResponseMin = -1.0;
            //}

            //if (Math.Abs(globalResponseMin) > globalResponseMax)
            //    globalResponseMax = Math.Abs(globalResponseMin);
            //else
            //    globalResponseMin = -globalResponseMax;

            var globalResponseMax = uncensoredMax;
            var globalResponseMin = uncensoredMin;

            if (Math.Abs(globalResponseMin) > globalResponseMax)
                globalResponseMax = Math.Abs(globalResponseMin);
            else
                globalResponseMin = -globalResponseMax;

            AnalysisData.GetBufferedRange(this, _residualXAxis, xMin, xMax, AxisKind.X);
            AnalysisData.GetBufferedRange(this, _residualYAxis, globalResponseMin, globalResponseMax, AxisKind.Y);//myData.ResponseTransform == TransformTypeEnum.Linear);

        }

        public void RelabelAxes(AnalysisData data)
        {
            if(_showResiduals)
                SwitchToResidualAxes(data);
            else
                SwitchToNormalAxes(data);
        }

        public void SwitchToResidualAxes(AnalysisData data)
        {
            SetXAxisRange(_residualXAxis, data);
            SetYAxisRange(_residualYAxis, data);

            RelabelAxesBetter(_residualXAxis, _residualYAxis, data.InvertTransformedFlaw, data.InvertTransformedResponse, 
                              Globals.GetLabelIntervalBasedOnChartSize(this, AxisKind.X), Globals.GetLabelIntervalBasedOnChartSize(this, AxisKind.Y), false, true, 
                              XTransform, YTransform, data.TransformValueForXAxis, data.TransformValueForYAxis);
        }

        public void SwitchToNormalAxes(AnalysisData data)
        {
            SetXAxisRange(_normalXAxis, data);
            SetYAxisRange(_normalYAxis, data);

            RelabelAxesBetter(_normalXAxis, _normalYAxis, data.InvertTransformedFlaw, data.InvertTransformedResponse, 
                              Globals.GetLabelIntervalBasedOnChartSize(this, AxisKind.X), Globals.GetLabelIntervalBasedOnChartSize(this, AxisKind.Y), false, false, 
                              XTransform, YTransform, data.TransformValueForXAxis, data.TransformValueForYAxis);
        }

        private void ColorSeries(List<Color> colors, int colorIndex, int styleIndex, Series series)
        {
            series.ChartType = SeriesChartType.Point;
            series.Color = colors[colorIndex];

            foreach (DataPoint point in series.Points)
            {
                point.MarkerStyle = _styles[styleIndex % _styles.Count];
                //point.MarkerSize = 10;
            }
        }

        private void AddHitMissFitSeries(AnalysisData data)
        {
            _normalSeries.Add(FlawEstimate);

            FlawEstimate.Points.Clear();

            Series fitLine = Series[LinearityChartLabels.FlawEstimate];

            DataView view = data.ResidualUncensoredTable.DefaultView;

            //fitLine.Points.DataBindXY(view, "t_flaw", view, "t_fit");
            fitLine.Points.DataBindXY(view, "flaw", view, "t_fit");
        }

        private void AddFitSeries(AnalysisData data)
        {
            _normalSeries.Add(FlawEstimate);

            FlawEstimate.Points.Clear();

            Series fitLine = Series[LinearityChartLabels.FlawEstimate];

            DataView view = data.ResidualRawTable.DefaultView;

            fitLine.Points.DataBindXY(view, "t_flaw", view, "t_fit");
        }

        public Series FlawEstimate
        {
            get
            {
                return Series[LinearityChartLabels.FlawEstimate];
            }
        }

        public Series FlawResidual
        {
            get
            {
                return Series[LinearityChartLabels.FlawResidual];
            }
        }

        public Series Uncensored
        {
            get
            {
                return Series[LinearityChartLabels.Uncensored];
            }
        }

        public Series Original
        {
            get
            {
                return Series[LinearityChartLabels.Original];
            }
        }

        public Series ModelCompare
        {
            get
            {


                return Series[LinearityChartLabels.ModelCompare];
            }
        }

        //public Series PartialCensored
        //{
        //    get
        //    {
        //        return Series[LinearityChartLabels.PartialCensored];
        //    }
        //}

        public TransformTypeEnum XTransform
        {
            get
            {
                return xTransform;
            }
        }

        public TransformTypeEnum YTransform
        {
            get
            {
                if (_showResiduals)
                    return TransformTypeEnum.Linear;
                else
                    return yTransform;
            }
        }


        
    }
}
