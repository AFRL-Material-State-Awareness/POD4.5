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
        private TransformBoxX _xTransformBox;
        private Label _xTransformLabel;
        
        private ConfidenceBox _confIntBox;
        private Label _confIntLabel;

        private SamplingTypeBox _sampleTypeBox;
        private Label _sampleTypeLabel;

        //rich text box used to give the user helpful hints with the Analysis
        private RichTextBox _helpfulRTF;
        //use to keep track of the previously selected confidence interval
        private ConfidenceIntervalTypeEnum previousConfInt= ConfidenceIntervalTypeEnum.StandardWald;
        //flags used to prevent the analysis from running again if the user selects NO
        private bool noConfIntRepeat = false;
        private bool noSamplingRepeat = false;
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
        public void DisposeAllExceptMainChart()
        {
            Analysis.AnalysisDone -= ProcessAnalysisOutput;
            _xTransformBox.Dispose();

            _confIntBox.Dispose();

            _sampleTypeBox.Dispose();
            _helpfulRTF.Dispose();
            //StepToolTip.Dispose();
            ///worked correctly when last tried here///
            ///private POD.Controls.PODChartNumericUpDown aMaxControl;
            aMinControl.Dispose();
            //a50label.Dispose();
            a50Out.Dispose();
            //a90Label.Dispose();
            a90Out.Dispose();
            //a90_95Label.Dispose();
            a90_95Out.Dispose();
            SigmaOut.Dispose();
            MuOut.Dispose();
            covV22Out.Dispose();
            covV11Out.Dispose();
            CovV12OutLabel.Dispose();
            covV12Out.Dispose();
            ModelBox.Dispose();
            TestColorMap.Dispose();
            likelihoodRatioTestOut.Dispose();
            FlawRangeHeader.Dispose();
            TestOfAssumptionsHeader.Dispose();
            CovarianceHeader.Dispose();
            AxisTransformsHeader.Dispose();
            PodModelParametersHeader.Dispose();
            PODModelTypeHeader.Dispose();
            DisposeChartsRecursively();
            DisposeLabels();

            DisposeOfParentPanels();


        }
        private void DisposeOfParentPanels()
        {
            base.DisposeOfRegressionObjects();
        }
        private void DisposeChartsRecursively()
        {

            //base.Dispose();
            MainChart.DisposeAllRegressionItems();
            MainChart.Dispose();
        }
        private void DisposeLabels()
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
                _xTransformLabel,
                _confIntLabel,
                _sampleTypeLabel
            };

            foreach (Label label in labels)
            {
                label.Dispose();
            }
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
            PrepareRichTextBox();
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
                                           TestColorMap, 
                                           _helpfulRTF};

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
            DataPointChart.ContextMenuImageList = aMaxControl.RatingImages;

            IntitalizeTransformBoxes();
            InitializeCITypeBox();
            InitializeSamplingTypeBox();
            var list = new List<Control> {
                                           AxisTransformsHeader,
                                           _xTransformLabel, _xTransformBox,
                                           _confIntLabel, _confIntBox,
                                           _sampleTypeLabel, _sampleTypeBox,
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

        public void CycleTransforms()
        {
            try
            {
                if (_xTransformBox.SelectedTransform == TransformTypeEnum.Linear) 
                {
                    _xTransformBox.SelectedTransform = TransformTypeEnum.Log; 
                }
                else if (_xTransformBox.SelectedTransform == TransformTypeEnum.Log ) 
                {
                    _xTransformBox.SelectedTransform = TransformTypeEnum.Linear; 
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show("CycleTransforms: " + exp.Message);
            }
        }

        private void IntitalizeTransformBoxes()
        {
            PrepareLabelBoxPair(ref _xTransformLabel, "Flaw", ref _xTransformBox);
            ModelBox.SelectedIndex = 0;
        }
        public void InitializeCITypeBox()
        {
            PrepareLabelBoxPairConfint(ref _confIntLabel, "Conf-Int Type", ref _confIntBox);
            _confIntBox.SelectedIndex = 0;
        }
        public void InitializeSamplingTypeBox()
        {
            PrepareLabelBoxPairSamplingType(ref _sampleTypeLabel, "Sampling Type", ref _sampleTypeBox);
            _sampleTypeBox.SelectedIndex = 0;
        }
        public void PrepareRichTextBox()
        {
            _helpfulRTF = new RichTextBox();
            _helpfulRTF.Text = "This text box displays information about transform types, confidence intervals, sampling types, and more!";
            //_helpfulRTF.Width = 209;
            _helpfulRTF.Height = 250;
            _helpfulRTF.RightMargin = 0;
            _helpfulRTF.ReadOnly = true;
            RightControlsTablePanel.SetColumnSpan(_helpfulRTF, 2);
            _helpfulRTF.Font = new Font("Microsoft Sans Serif", 12.0f);
            colorTextInHelpfulRTF();
        }
        private void DefaultRTFMessage()
        {
            _helpfulRTF.Text = "This text box displays information about transform types, confidence intervals, sampling types, and more!";
            _helpfulRTF.SelectAll();
            _helpfulRTF.SelectionColor = Color.DarkGray;
            _helpfulRTF.DeselectAll();
        }
        private void colorTextInHelpfulRTF()
        {
            _helpfulRTF.SelectAll();
            _helpfulRTF.SelectionColor = Color.DarkGray;
            _helpfulRTF.DeselectAll();
        }
        private void colorBackTextInHelpfulRTF()
        {
            _helpfulRTF.SelectAll();
            _helpfulRTF.SelectionColor = Color.Black;
            _helpfulRTF.DeselectAll();
        }
        /// <summary>
        /// Sets up event handling for right side numeric controls. Only call after InitializeComponent().
        /// </summary>
        protected override void SetupNumericControlEvents()
        {
            aMaxControl.NumericUpDown.ValueChanged += this.aMaxControl_ValueChanged;
            aMinControl.NumericUpDown.ValueChanged += this.aMinControl_ValueChanged;
            _xTransformBox.SelectedIndexChanged += XTransformBox_ValueChanged;
            _xTransformBox.ListItemSelectionChanged += XTransformBox_Mousehover;
            ModelBox.SelectedIndexChanged += ModelBox_ValueChanged;
            ModelBox.ListItemSelectionChanged += ModelBox_Mousehover;
            _confIntBox.SelectedIndexChanged += ConfIntBox_ValueChanged; 
            _confIntBox.ListItemSelectionChanged += ConfIntBox_Mousehover;
            _sampleTypeBox.SelectedIndexChanged += SamplingTypeBox_ValueChanged;
            _sampleTypeBox.ListItemSelectionChanged += SamplingTypeBox_Mousehover;


        }
       
        private void XTransformBox_ValueChanged(object sender, EventArgs e)
        {
            Analysis.InFlawTransform = _xTransformBox.SelectedTransform;
            if (Analysis.InFlawTransform != TransformTypeEnum.Inverse)
            {
                var x = Convert.ToDouble(Analysis.TransformValueForXAxis(aMaxControl.Value));
                mainChart.SetAMaxBoundary(x, false);
                x = Convert.ToDouble(Analysis.TransformValueForXAxis(aMinControl.Value));
                mainChart.SetAMinBoundary(x, false);
            }
            else
            {
                var x = Convert.ToDouble(Analysis.TransformValueForXAxis(Analysis.MinFlawSize- (.02)*Analysis.MinFlawSize));
                mainChart.SetAMaxBoundary(x, false);
                x = Convert.ToDouble(Analysis.TransformValueForXAxis(aMinControl.Value));
                mainChart.SetAMinBoundary(x, false);
            }
            ForceUpdateAfterTransformChange();
        }
        private void XTransformBox_Mousehover(object sender, ComboBoxListEx.ListItemSelectionChangedEventArgs e)
        {
            _helpfulRTF.Text = " ";
            colorBackTextInHelpfulRTF();
            switch (e.ItemIndex)
            {
                case 0:
                    _helpfulRTF.Text = "Perform a linear transform on the flaws (i.e. no transform).";
                    break;
                case 1:
                    _helpfulRTF.Text = "Perform a log (base e) transform on the flaws.";
                    break;
                case 2:
                    _helpfulRTF.Text = "Perform an inverse (1/x) transform on the flaws.";
                    break;
                default:
                    DefaultRTFMessage();
                    break;

            }
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
        private void ModelBox_Mousehover(object sender, ComboBoxListEx.ListItemSelectionChangedEventArgs e)
        {
            if (_helpfulRTF.IsDisposed)
            {
                return;
            }
            _helpfulRTF.Text = " ";
            colorBackTextInHelpfulRTF();
            switch (e.ItemIndex)
            {
                case 0:
                    _helpfulRTF.Text = "Calculate the POD curve using traditional logistic regression. This algorithm uses an iterative re-weighted " +
                        "least squares algorithm until convergence is found.";
                    break;
                case 1:
                    _helpfulRTF.Text = "Calculate the POD curve using Firth's bias reduction. This is an alternative to traditional logistic regression that " +
                        "utilizes a penalized likelihood approach. This is especially useful for handling separated data and/or rare events where traditional" +
                        "logistic regression does not converge.";
                    break;
                default:
                    DefaultRTFMessage();
                    break;

            }
        }

        private void ConfIntBox_ValueChanged(object sender, EventArgs e)
        {

            DialogResult confIntDialogResults = CheckLongRuntime();
            if (confIntDialogResults == DialogResult.Yes && !(noConfIntRepeat))
            {
                Analysis.InConfIntervalType = _confIntBox.SelectedConfInt;
                var x = Convert.ToDouble(Analysis.TransformValueForXAxis(aMaxControl.Value));
                mainChart.SetAMaxBoundary(x, false);
                x = Convert.ToDouble(Analysis.TransformValueForXAxis(aMinControl.Value));
                mainChart.SetAMinBoundary(x, false);

                ForceUpdateAfterTransformChange();
            }
            else if (noConfIntRepeat == true)
            {
                noConfIntRepeat = false;
            }
            else
            {
                noConfIntRepeat = true;
                _confIntBox.SelectedConfInt = previousConfInt;
                
            }
            previousConfInt = _confIntBox.SelectedConfInt;
        }
        /// <summary>
        /// This event handler function diplays useful information when the user hovers over the confidence box options
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfIntBox_Mousehover(object sender, ComboBoxListEx.ListItemSelectionChangedEventArgs e)
        {
            _helpfulRTF.Text = " ";
            colorBackTextInHelpfulRTF();
            switch (e.ItemIndex)
            {

                case 0:
                    _helpfulRTF.Text = "Performs a standard wald 95% confidence interval on the POD curve. This is the defualt option.";
                    break;
                case 1:
                    _helpfulRTF.Text = "Performs modified wald confidence interval. Calculated the same way as standard wald, but first generates " +
                        "normally distributed crack sizes to address smoothing issues with the confidence interval curve when linear interpolating.";
                    break;
                case 2:
                    _helpfulRTF.Text = "Performs a likelihood ratio confidence interval. This is the theoretically correct way to calculate the confidence " +
                        "interval when dealing with binomial data. It is compuational intense optimization that takes around 5-10 seconds to run. ";
                    break;
                case 3:
                    _helpfulRTF.Text = "Performs a modified likelihood ratio confidence interval. Similar to the likelihood ratio confidence interval " +
                        "except that it uses higher order approximation to deal with potential smoothing issues. It is compuational intense optimization that takes around 10-15 seconds to run. ";
                    break;
                default:
                    DefaultRTFMessage();
                    break;

            }
        }

        private void SamplingTypeBox_ValueChanged(object sender, EventArgs e)
        {
            
            DialogResult samplingDialogResult = CheckLongRuntime();
            
            if(samplingDialogResult == DialogResult.Yes && !(noSamplingRepeat))
            {
                Analysis.InSamplingType = _sampleTypeBox.SelectedSamplingType;
                var x = Convert.ToDouble(Analysis.TransformValueForXAxis(aMaxControl.Value));
                mainChart.SetAMaxBoundary(x, false);
                x = Convert.ToDouble(Analysis.TransformValueForXAxis(aMinControl.Value));
                mainChart.SetAMinBoundary(x, false);

                ForceUpdateAfterTransformChange();
                
            }
            else if (noSamplingRepeat==true)
            {
                noSamplingRepeat = false;
            }
            else
            {
                noSamplingRepeat = true;
                _sampleTypeBox.SelectedSamplingType = SamplingTypeEnum.SimpleRandomSampling;
            }
        }
        /// <summary>
        /// displays useful information when the user hovers over the sampling type box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SamplingTypeBox_Mousehover(object sender, ComboBoxListEx.ListItemSelectionChangedEventArgs e)
        {
            _helpfulRTF.Text = " ";
            colorBackTextInHelpfulRTF();
            switch (e.ItemIndex)
            {
                case 0:
                    _helpfulRTF.Text = "Performs a HitMiss Analysis using simple random sampling (i.e. the data 'as is')";
                    break;
                case 1:
                    _helpfulRTF.Text = "Performes a HitMiss Analysis using ranked set sampling. Ranked set sampling is an alternative to " +
                        "simple random sampling in which a certain number of 'resamples' are generated (30 by default) using ranked set sampling techniques " +
                        "(see manual for more information). This metric is especially useful when the original dataset has a relatively small sample size";
                    break;
                default:
                    _helpfulRTF.Text = "This text box displays information about transform types, confidence intervals, sampling types, and more!";
                    break;

            }
        }
        /// <summary>
        /// this function is used as a check to warn the user ahead of time if the analysis config selected is going to take an excessive amount of time
        /// </summary>
        /// <returns></returns>
        public DialogResult CheckLongRuntime()
        {
            //dialog will be yes by default in case the user uses either standard wald or modified wald with ranked set sampling
            DialogResult dialogResult = DialogResult.Yes;
            if (_confIntBox.SelectedConfInt.ToString() == "StandardWald" && _sampleTypeBox.SelectedSamplingType.ToString() == "RankedSetSampling")
            {
                MessageBox.Show("WARNING: Using standard wald with ranked set sampling can produce wacky results." + '\n' +
                    "It is recommended to use modified wald instead.");
            }
            else if (_confIntBox.SelectedConfInt.ToString() == "LR" && _sampleTypeBox.SelectedSamplingType.ToString() == "RankedSetSampling")
            {
                dialogResult = MessageBox.Show("WARNING: You've selected Likelihood Ratio (LR) confidence interval " +
                    "with Ranked Set Sampling. This process" +
                    "could take anywhere from 5-10min to complete." + '\n' +
                    "Do you still want to proceed?", "Time To Execute Warning", MessageBoxButtons.YesNo);
            }
            else if (_confIntBox.SelectedConfInt.ToString() == "MLR" && _sampleTypeBox.SelectedSamplingType.ToString() == "RankedSetSampling")
            {
                dialogResult = MessageBox.Show("WARNING: You've selected Modified Likelihood Ratio (MLR) confidence interval " +
                    "with Ranked Set Sampling. This process" +
                    "could take anywhere from 10-20min to complete." + '\n' +
                    "Do you still want to proceed?", "Time To Execute Warning", MessageBoxButtons.YesNo);
            }
            return dialogResult;
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
            _confIntBox.SelectedConfInt = Analysis.InConfIntervalType;
            _sampleTypeBox.SelectedSamplingType = Analysis.InSamplingType;


            MainChart.SetAMaxBoundary(Analysis.TransformValueForXAxis(Analysis.InFlawMax), false);
            MainChart.SetAMinBoundary(Analysis.TransformValueForXAxis(Analysis.InFlawMin), false);
            aMaxControl.Value = Convert.ToDecimal(Analysis.InFlawMax);
            aMinControl.Value = Convert.ToDecimal(Analysis.InFlawMin);
            /*
            if (Analysis.InFlawTransform != TransformTypeEnum.Inverse)
            {
                aMaxControl.Value = Convert.ToDecimal(Analysis.InFlawMax);
                aMinControl.Value = Convert.ToDecimal(Analysis.InFlawMin);
            }
            else
            {
                aMaxControl.Value = Convert.ToDecimal(1.0 / Analysis.MinFlawSize);
                aMinControl.Value = Convert.ToDecimal(Analysis.InFlawMin);
            }
            */

            Analysis.IsFrozen = false;
        }

        public override void ProcessAnalysisOutput(Object sender, EventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine(Globals.AnnoyingBug);
            //System.Diagnostics.Debug.WriteLine($"Check if annotations is null 3:{MainChart.Annotations}");
            //Globals.AnnoyingBug += 1;
            //System.Diagnostics.Debug.WriteLine('\n');
            //bool analysisFailed = false;
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
                //ADD click to continue? in order to ensure the user doesn't think the program is broken
                _knownErrorFound = false;
                if (Analysis.AnalysisDataType.ToString() == "HitMiss")
                {
                    SearhForHitMissErrors(a9095Original);
                }
                if (!_knownErrorFound)
                {
                    //Source.Python.AddErrorText("Output values out of range.");
                    Source.Python.AddErrorText("DEFAULT UNKNOWN ERROR: Contact support or a Statistician if Necessary");
                }

                /*
                MainChart.ClearEverythingButPoints();
                mainChart.UpdateBestFitLine();
                //linearityChart.ClearEverythingButPoints();
                podChart.ClearEverythingButPoints();
                */
                //if there is an error, allow the user to open another analysis tab since it is no longer running
                CSharpBackendWithR.REngineObject.REngineRunning = false;
                Analysis.ErrorInAnalysis = true;
                try
                {
                    ReattemptOutput(ref a90Transformed, ref a9095Transformed, ref a50Transformed, ref a50Original, ref a90Original, ref a9095Original, ref  lackOfFit);
                }
                finally
                {
                    //give the user as much data as possible without messing up charts
                    podChart.FillChart(Analysis.Data, false, Analysis.ErrorInAnalysis);
                    podChart.SetXAxisRange(Analysis.Data.GetUncensoredXBufferedRange(podChart, false), Analysis.Data, true);
                    podChart.UpdateLevelConfidenceLines(a50Original,
                                                        a90Original,
                                                        a9095Original);
                    StepToolTip.SetToolTip(podChart, podChart.TooltipText);
                    podChart.ChartToolTip = StepToolTip;

                    likelihoodRatioTestOut.Rating = Analysis.GetTestRatingFromLabel(Analysis.OutTestLackOfFitRating);

                    mainChart.UpdateBestFitLine();

                    linearityChart.FillChart(Analysis.Data, Analysis.OutPODMu, Analysis.OutPODSigma);
                }
                return;
            }
            //finished the analysis without issues! Set the error in analysis to false
            Analysis.ErrorInAnalysis = false;
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

            CSharpBackendWithR.REngineObject.REngineRunning = false;
        }
        //get as many valid values as possible (error value defaults to -9999999999.0m;)
        private void ReattemptOutput(ref double a90Transformed, ref double a9095Transformed, ref double a50Transformed, ref double a50Original,ref double a90Original,
            ref double a9095Original, ref double lackOfFit)
        {
            a90Transformed = Convert.ToDouble(Analysis.TransformValueForXAxis(Convert.ToDecimal(a90Transformed)));
            if (Double.IsNaN(a9095Original) && Double.IsNaN(a9095Transformed))
            {
                a9095Original = -9999999999.0;
                a9095Transformed = -9999999999.0;
                a90_95Out.Value = Convert.ToDecimal(-9999999999.0);
            }    
            else
                a9095Transformed = Convert.ToDouble(Analysis.TransformValueForXAxis(Convert.ToDecimal(a9095Transformed)));
            try
            {
            a50Transformed = Convert.ToDouble(Analysis.TransformValueForXAxis(Convert.ToDecimal(a50Transformed)));
            }
            catch
            {
                a50Transformed= -9999999999.0;
            }
            try
            {
                a90Out.Value = Convert.ToDecimal(a90Original);
            }
            catch
            {
                a90Out.Value = -9999999999.0m;
            }
            try
            {
                a50Out.Value = Convert.ToDecimal(a50Original);
            }
            catch
            {
                a50Out.Value= -9999999999.0m;
            }
            try
            {
                MuOut.Value = Convert.ToDecimal(Analysis.OutPODMu);
            }
            catch
            {
                MuOut.Value = -9999999999.0m;
            }
            try
            {
                SigmaOut.Value = Convert.ToDecimal(Analysis.OutPODSigma);
            }
            catch
            {
                SigmaOut.Value = -9999999999.0m;
            }
            try
            {
                likelihoodRatioTestOut.Value = Convert.ToDecimal(lackOfFit);
            }
            catch
            {
                likelihoodRatioTestOut.Value = -9999999999.0m;
            }
            try
            {
                covV11Out.Value = Convert.ToDecimal(Analysis.OutPFCovarianceV11);
            }
            catch
            {
                covV11Out.Value = -9999999999.0m;
            }
            try
            {
                covV12Out.Value = Convert.ToDecimal(Analysis.OutPFCovarianceV12);
            }
            catch
            {
                covV12Out.Value = -9999999999.0m;
            }
            try
            {
                covV22Out.Value = Convert.ToDecimal(Analysis.OutPFCovarianceV22);
            }
            catch
            {
                covV22Out.Value = -9999999999.0m;
            }
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
            //if(Analysis.InFlawTransform == TransformTypeEnum.Inverse)
            //{
            //    mainChart.SetAMaxBoundary(1/Analysis.MinFlawSize, true);
            //}
            //else
            //{
                mainChart.SetAMaxBoundary(x, true);
            //}
            Analysis.AnalysisCalculationType = RCalculationType.Full;
            RunAnalysis();

            //controlValueChanged = false;
        }

        private void aMinControl_ValueChanged(object sender, EventArgs e)
        {
            //controlValueChanged = true;

            var x = Convert.ToDouble(Analysis.TransformValueForXAxis(aMinControl.Value));
            mainChart.SetAMinBoundary(x, true);
            Analysis.AnalysisCalculationType = RCalculationType.Full;
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
