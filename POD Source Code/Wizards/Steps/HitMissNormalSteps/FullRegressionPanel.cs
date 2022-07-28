using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POD.Controls;

namespace POD.Wizards.Steps.HitMissNormalSteps
{
    using System.Data;
    using System.Windows.Forms.DataVisualization.Charting;

    using POD.Analyze;
    using POD.Data;

    public partial class FullRegressionPanel : RegressionPanel
    {
        private TransformBox _xTransformBox;
        private Label _xTransformLabel;

        private ConfidenceBox _confIntBox;
        private Label _confIntLabel;
        //private TransformBox _yTransformBox;

        public FullRegressionPanel(PODToolTip tooltip) : base(tooltip)
        {
            InitializeComponent();

            StepToolTip = new PODToolTip();

            //assign main chart for base class
            MainChart = mainChart;

            AddSideCharts();
            SetSideControls(graphFlowPanel, graphSplitter, graphSplitterOverlay);

            SidePanel.Controls.Clear();
            CheckSideCharts();

            FixPutControlsHere();
            AddInputControls();
            AddOutputControls();            
            ColorNumericControls();
            SetupNumericControlPrecision();
            SetupNumericControlEvents();
            SetupChartEvents();
            SetupLabels();

            mainChart.FreezeThresholdLine(.5);

            
        }

        protected override void SetupLabels()
        {
            var labels = new List<Control>
            {
                a50label,
                a90Label,
                a90_95Label,
                podMuLabel,
                podSigmaLabel,
                CovV11OutLabel, 
                CovV12OutLabel,
                CovV22OutLabel,
                lackOfFitTestLabel,
                ModelLabel,
                AMinInputLabel,
                AMaxInputLabel,
                _xTransformLabel
            };

            foreach (Label label in labels)
            {
                label.TextAlign = ContentAlignment.MiddleRight;
            }
        }

        public override void FixPanelControlSizes()
        {
            if (!ControlSizesFixed)
            {
                base.FixPanelControlSizes();

                //SuspendDrawing();

                //default to showing the charts (must match with Full RegressionBar initial button states)                
                DisplayChart(LinearityIndex);
                DisplayChart(PodIndex);
                SetSideChartSize(2);

                FixColumnWidth(outputTablePanel, a50Out);
                MakeInputMatchOutputWidth(a50label);

                //ResizeControlsToAllFit(inputTablePanel);
                //ResizeControlsToAllFit(outputTablePanel);
                //MakeInputMatchOutputWidth();

                FixTableSize(outputTablePanel);


                //ResumeDrawing();
            }
        }

        
              

        protected override void AddOutputControls()
        {
            base.AddOutputControls();

            var list = new List<Control> { PodModelParametersHeader,
                                           a50label, a50Out,
                                           a90Label, a90Out,
                                           a90_95Label, a90_95Out,                                           
                                           podMuLabel, MuOut,
                                           podSigmaLabel, SigmaOut,

                                           CovarianceHeader,
                                           CovV11OutLabel, covV11Out,
                                           CovV12OutLabel, covV12Out,
                                           CovV22OutLabel, covV22Out,

                                           TestOfAssumptionsHeader,
                                           lackOfFitTestLabel, likelihoodRatioTestOut,
                                           TestColorMap};

            foreach (Control control in list)
            {
                control.Dock = DockStyle.Fill;
            }

            outputTablePanel.Controls.AddRange(list.ToArray());

            outputTablePanel.SetColumnSpan(TestColorMap, 2);
            outputTablePanel.SetColumnSpan(PodModelParametersHeader, 2);
            outputTablePanel.SetColumnSpan(CovarianceHeader, 2);            
            outputTablePanel.SetColumnSpan(TestOfAssumptionsHeader, 2);

            a50Out.PartType = ChartPartType.A50;
            a90Out.PartType = ChartPartType.A90;
            a90_95Out.PartType = ChartPartType.A9095;
            SigmaOut.PartType = ChartPartType.POD;
            MuOut.PartType = ChartPartType.POD;
            covV11Out.PartType = ChartPartType.Undefined;
            covV12Out.PartType = ChartPartType.Undefined;
            covV22Out.PartType = ChartPartType.Undefined;

            outputTablePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100.0F));
            outputTablePanel.RowCount = outputTablePanel.RowStyles.Count;

            a50Out.TooltipForNumeric = Globals.SplitIntoLines("Estimated flaw size for 50% POD.");
            a90Out.TooltipForNumeric = Globals.SplitIntoLines("Estimated flaw size for 90% POD.");
            a90_95Out.TooltipForNumeric = Globals.SplitIntoLines("95% confidence bound on a90.");
            MuOut.TooltipForNumeric = Globals.SplitIntoLines("Mu parameter of the POD model.");
            SigmaOut.TooltipForNumeric = Globals.SplitIntoLines("Sigma parameter of the POD model.");
            covV11Out.TooltipForNumeric = Globals.SplitIntoLines("V11 of variance-covariance matrix for POD model paramters.");
            covV12Out.TooltipForNumeric = Globals.SplitIntoLines("V12 of variance-covariance matrix for POD model paramters.");
            covV22Out.TooltipForNumeric = Globals.SplitIntoLines("V22 of variance-covariance matrix for POD model paramters.");
            likelihoodRatioTestOut.TooltipForNumeric = Globals.SplitIntoLines("Likelihood Ratio calculated test statistic.");

        }

        protected override void AddInputControls()
        {
            base.AddInputControls();

            //set image list so context menu has same pictures
            MainChart.ContextMenuImageList = aMaxControl.RatingImages;

            IntitalizeTransformBoxes();
            InitializeCITypeBox();
            var list = new List<Control> {
                                           AxisTransformsHeader,
                                           _xTransformLabel, _xTransformBox,
                                           _confIntLabel, _confIntBox,
                                           PODModelTypeHeader,
                                           ModelLabel, ModelBox,

                                           FlawRangeHeader,
                                           AMaxInputLabel, aMaxControl, 
                                           AMinInputLabel, aMinControl };

            foreach (Control control in list)
            {
                control.Dock = DockStyle.Fill;
            }

            inputTablePanel.Controls.AddRange(list.ToArray());

            inputTablePanel.SetColumnSpan(AxisTransformsHeader, 2);
            inputTablePanel.SetColumnSpan(FlawRangeHeader, 2);
            inputTablePanel.SetColumnSpan(PODModelTypeHeader, 2);

            aMinControl.PartType = ChartPartType.CrackMin;
            aMaxControl.PartType = ChartPartType.CrackMax;

            inputTablePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100.0F));
            inputTablePanel.RowCount = inputTablePanel.RowStyles.Count;


            StepToolTip.SetToolTip(_xTransformBox, Globals.SplitIntoLines("Transform to apply to the flaws."));
            StepToolTip.SetToolTip(ModelBox, Globals.SplitIntoLines("Model to use."));
            aMaxControl.TooltipForNumeric = Globals.SplitIntoLines("Flaw's maximum range.");
            aMinControl.TooltipForNumeric = Globals.SplitIntoLines("Flaw's minimum range.");
        }

        private void IntitalizeTransformBoxes()
        {
            PrepareLabelBoxPair(ref _xTransformLabel, "Flaw", ref _xTransformBox);
            //PrepareLabelBoxPairConfint(ref _confIntLabel, "Confidence Interval", ref _confIntBox);
            ModelBox.SelectedIndex = 0;
        }
        public void InitializeCITypeBox()
        {
            PrepareLabelBoxPairConfint(ref _confIntLabel, "Confidence Interval Type", ref _confIntBox);
            _confIntBox.SelectedIndex = 0;
        }
        /// <summary>
        /// Sets up event handling for right side numeric controls. Only call after InitializeComponent().
        /// </summary>
        protected override void SetupNumericControlEvents()
        {
            aMaxControl.NumericUpDown.ValueChanged += this.aMaxControl_ValueChanged;
            aMinControl.NumericUpDown.ValueChanged += this.aMinControl_ValueChanged;
            _xTransformBox.SelectedIndexChanged += XTransformBox_ValueChanged;
            ModelBox.SelectedIndexChanged += ModelBox_ValueChanged;
            _confIntBox.SelectedIndexChanged += ConfIntBox_ValueChanged;
        }
        private void ConfIntBox_ValueChanged(object sender, EventArgs e)
        {
            Analysis.InConfIntervalType = _confIntBox.SelectedConfInt;
            var x = Convert.ToDouble(Analysis.TransformValueForXAxis(aMaxControl.Value));
            mainChart.SetAMaxBoundary(x, false);
            x = Convert.ToDouble(Analysis.TransformValueForXAxis(aMinControl.Value));
            mainChart.SetAMinBoundary(x, false);

            ForceUpdateAfterTransformChange();
        }

        private void XTransformBox_ValueChanged(object sender, EventArgs e)
        {
            Analysis.InFlawTransform = _xTransformBox.SelectedTransform;

            var x = Convert.ToDouble(Analysis.TransformValueForXAxis(aMaxControl.Value));
            mainChart.SetAMaxBoundary(x, false);
            x = Convert.ToDouble(Analysis.TransformValueForXAxis(aMinControl.Value));
            mainChart.SetAMinBoundary(x, false);

            ForceUpdateAfterTransformChange();
        }

        private void ModelBox_ValueChanged(object sender, EventArgs e)
        {
            Analysis.InHitMissModel = ModelBox.SelectedModel;

            var x = Convert.ToDouble(Analysis.TransformValueForXAxis(aMaxControl.Value));
            mainChart.SetAMaxBoundary(x, false);
            x = Convert.ToDouble(Analysis.TransformValueForXAxis(aMinControl.Value));
            mainChart.SetAMinBoundary(x, false);

            ForceUpdateAfterTransformChange();
        }

        /// <summary>
        /// Sets up properties for right side numeric controls.  Only call after InitializeComponent().
        /// </summary>
        protected override void SetupNumericControlPrecision()
        {
            var list = new List<PODNumericUpDown> { aMaxControl.NumericUpDown, aMinControl.NumericUpDown, likelihoodRatioTestOut.NumericUpDown};
            
            foreach (var number in list)
            {
                number.Maximum = decimal.MaxValue;
                number.Minimum = decimal.MinValue;
                number.DecimalPlaces = 3;
            }
        }

        /// <summary>
        /// Adds side charts available to the panel.  Only call after InitializeComponent().
        /// </summary>
        public override void AddSideCharts()
        {
            SideCharts.Add(linearityChart);
            //SideCharts.Add(normalityChart);
            //SideCharts.Add(equalVarianceChart);
            SideCharts.Add(podChart);
            //SideCharts.Add(thresholdChart);
        }

        protected sealed override void ColorNumericControls()
        {
            /*aMaxControl.BackColor = Globals.AlphaOverWhiteToOpaque(Color.FromArgb(ChartColors.ControlBackColorAlpha, ChartColors.aMaxColor));
            aMinControl.BackColor = Globals.AlphaOverWhiteToOpaque(Color.FromArgb(ChartColors.ControlBackColorAlpha, ChartColors.aMinColor));
            a50Out.BackColor = Globals.AlphaOverWhiteToOpaque(Color.FromArgb(ChartColors.ControlBackColorAlpha, ChartColors.a50Color));
            a90Out.BackColor = Globals.AlphaOverWhiteToOpaque(Color.FromArgb(ChartColors.ControlBackColorAlpha, ChartColors.a90Color));
            a90_95Out.BackColor = Globals.AlphaOverWhiteToOpaque(Color.FromArgb(ChartColors.ControlBackColorAlpha, ChartColors.a9095Color));
            MuOut.BackColor = Globals.AlphaOverWhiteToOpaque(Color.FromArgb(ChartColors.ControlBackColorAlpha, ChartColors.FitColor));
            SigmaOut.BackColor = Globals.AlphaOverWhiteToOpaque(Color.FromArgb(ChartColors.ControlBackColorAlpha, ChartColors.FitColor));
            likelihoodRatioTestOut.BackColor = Globals.AlphaOverWhiteToOpaque(Color.FromArgb(ChartColors.ControlBackColorAlpha, ChartColors.TestUnknownColor));*/
        }

        public override void SetupAnalysisInput()
        {
            Analysis.InFlawTransform = _xTransformBox.SelectedTransform;
            Analysis.InHitMissModel = ModelBox.SelectedModel;
            Analysis.InFlawMax = Convert.ToDouble(aMaxControl.Value);
            Analysis.InFlawMin = Convert.ToDouble(aMinControl.Value);
            var xAxis = Analysis.Data.GetXBufferedRange(null, false);
            Analysis.InFlawCalcMax = Convert.ToDouble(Analysis.TransformValueForXAxis(xAxis.Max));
            Analysis.InFlawCalcMin = Convert.ToDouble(Analysis.TransformValueForXAxis(xAxis.Min));
            Analysis.InResponseDecisionMin = mainChart.ChartAreas[0].AxisY.Minimum;
            Analysis.InResponseDecisionMax = mainChart.ChartAreas[0].AxisY.Maximum;
            //prototype for the UI
            Analysis.InConfIntervalType = _confIntBox.SelectedConfInt;
            Analysis.InResponseDecisionIncCount = 21;
            
        }

        protected override void SyncSideControls()
        {
            Analysis.IsFrozen = true;

            mainChart.DrawBoundaryLines();

            _xTransformBox.SelectedTransform = Analysis.InFlawTransform;
            ModelBox.SelectedModel = Analysis.InHitMissModel;

            MainChart.SetAMaxBoundary(Analysis.TransformValueForXAxis(Analysis.InFlawMax), false);
            MainChart.SetAMinBoundary(Analysis.TransformValueForXAxis(Analysis.InFlawMin), false);
            aMaxControl.Value = Convert.ToDecimal(Analysis.InFlawMax);
            aMinControl.Value = Convert.ToDecimal(Analysis.InFlawMin);

            Analysis.IsFrozen = false;
        }

        public override void ProcessAnalysisOutput(Object sender, EventArgs e)
        {
            if (MainChart != null)
                MainChart.ClearProgressBar();

            EnableInputControls();
            //copy parameters from the Analysis class in order to print them to the user interface
            double a90Transformed = Analysis.OutResponseDecisionPODLevelValue;
            double a9095Transformed = Analysis.OutResponseDecisionPODConfidenceValue;
            double a50Transformed = Analysis.OutResponseDecisionPODA50Value;
            double a90Original = Analysis.OutResponseDecisionPODLevelValue;
            double a9095Original = Analysis.OutResponseDecisionPODConfidenceValue;
            double a50Original = Analysis.OutResponseDecisionPODA50Value;
            double podMu = Analysis.OutPODMu;
            double podSigma = Analysis.OutPODSigma;
            double covV11 = Analysis.OutPFCovarianceV11;
            double covV12 = Analysis.OutPFCovarianceV12;
            double covV22 = Analysis.OutPFCovarianceV22;
            double lackOfFit = Analysis.OutTestLackOfFit;

            try
            {
                a90Transformed = Convert.ToDouble(Analysis.TransformValueForXAxis(Convert.ToDecimal(a90Transformed)));
                a9095Transformed = Convert.ToDouble(Analysis.TransformValueForXAxis(Convert.ToDecimal(a9095Transformed)));
                a50Transformed = Convert.ToDouble(Analysis.TransformValueForXAxis(Convert.ToDecimal(a50Transformed)));
                a90Out.Value = Convert.ToDecimal(a90Original);
                a90_95Out.Value = Convert.ToDecimal(a9095Original);
                a50Out.Value = Convert.ToDecimal(a50Original);
                MuOut.Value = Convert.ToDecimal(Analysis.OutPODMu);
                SigmaOut.Value = Convert.ToDecimal(Analysis.OutPODSigma);
                likelihoodRatioTestOut.Value = Convert.ToDecimal(lackOfFit);

                covV11Out.Value = Convert.ToDecimal(Analysis.OutPFCovarianceV11);
                covV12Out.Value = Convert.ToDecimal(Analysis.OutPFCovarianceV12);
                covV22Out.Value = Convert.ToDecimal(Analysis.OutPFCovarianceV22);
            }
            catch
            {
                //MessageBox.Show("Analysis Error caused invalid output values that are out of range.");
                Source.Python.AddErrorText("Output values out of range.");

                MainChart.ClearEverythingButPoints();
                linearityChart.ClearEverythingButPoints();
                podChart.ClearEverythingButPoints();

                return;
            }
            
            podChart.FillChart(Analysis.Data);
            podChart.SetXAxisRange(Analysis.Data.GetUncensoredXBufferedRange(podChart, false), Analysis.Data, true);
            podChart.UpdateLevelConfidenceLines(a50Original,
                                                a90Original,
                                                a9095Original);

            StepToolTip.SetToolTip(podChart, podChart.TooltipText);
            podChart.ChartToolTip = StepToolTip;

            TestRating lackOfFitRating = Analysis.GetTestRatingFromLabel(Analysis.OutTestLackOfFitRating);
            likelihoodRatioTestOut.Rating = lackOfFitRating;
            

            mainChart.UpdateBestFitLine();

            mainChart.UpdateLevelConfidenceLines(a50Transformed,
                                                 a90Transformed,
                                                 a9095Transformed);
            linearityChart.FillChart(Analysis.Data, Analysis.OutPODMu, Analysis.OutPODSigma);
            StepToolTip.SetToolTip(linearityChart, linearityChart.TooltipText);
            linearityChart.ChartToolTip = StepToolTip;
        }
        
            

        public int LinearityIndex
        {
            get { return SideCharts.IndexOf(linearityChart); }
        }

        public int NormalityIndex
        {
            get { return SideCharts.IndexOf(normalityChart); }
        }

        public int EqualVarianceIndex
        {
            get { return SideCharts.IndexOf(equalVarianceChart); }
        }

        public int PodIndex
        {
            get { return SideCharts.IndexOf(podChart); }
        }

        public int ThresholdIndex
        {
            get { return SideCharts.IndexOf(thresholdChart); }
        }

        private void aMaxControl_ValueChanged(object sender, EventArgs e)
        {
            //controlValueChanged = true;

            var x = Convert.ToDouble(Analysis.TransformValueForXAxis(aMaxControl.Value));
            mainChart.SetAMaxBoundary(x, true);

            RunAnalysis();

            //controlValueChanged = false;
        }

        private void aMinControl_ValueChanged(object sender, EventArgs e)
        {
            //controlValueChanged = true;

            var x = Convert.ToDouble(Analysis.TransformValueForXAxis(aMinControl.Value));
            mainChart.SetAMinBoundary(x, true);

            RunAnalysis();

            //controlValueChanged = false;
        }

        protected override void MainChart_LinesChanged(object sender, EventArgs e)
        {
            double value = 0.0;

            if (mainChart.FindValue(ControlLine.AMax, ref value))
                this.aMaxControl.Value = Analysis.InvertTransformValueForXAxis(Convert.ToDecimal(value));

            if (mainChart.FindValue(ControlLine.AMin, ref value))
                this.aMinControl.Value = Analysis.InvertTransformValueForXAxis(Convert.ToDecimal(value));
        }

        
    }
}
