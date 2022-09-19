using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POD.Controls;
using POD.Analyze;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;
using CSharpBackendWithR;
namespace POD.Wizards.Steps.AHatVsANormalSteps
{
    using System.Data;
    using System.Windows.Forms.DataVisualization.Charting;

    using POD.Analyze;
    using POD.Data;

    public partial class FullRegressionPanel : RegressionPanel
    {
        private TransformBox _xTransformBox;
        private Label _xTransformLabel;
        private TransformBoxYHat _yTransformBox;
        private Label _yTransformLabel;
        //used to set lambda for box-cox transformation
        private Label _labelForLamdaInput;
        private LambdaNumericUpDown _boxCoxLambda;

        private List<Control> _inputWithLabels = new List<Control>();
        private List<Control> _outputWithLabels = new List<Control>();
        private int _tabIndex;
        //used to keep track of the previous value of lambda in case the user tries to enter 0 into the numeric text box
        private decimal _previousLambda = 1.0m;
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
            SetupLabels();
            SetupNumericControlPrecision();
            SetupNumericControlEvents();
            SetupChartEvents();

            //Paint += FullRegressionPanel_Paint;
            ControlSizesFixed = false;

            
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
                DisplayChart(ThresholdIndex);
                SetSideChartSize(3);                

                //ResizeControlsToAllFit(inputTablePanel);
                //ResizeControlsToAllFit(outputTablePanel);
                //MakeInputMatchOutputWidth();

                FixColumnWidth(outputTablePanel, a50Out);
                MakeInputMatchOutputWidth(a50label);


                FixTableSize(outputTablePanel);
                //ResumeDrawing();
            }
        }

        void FullRegressionPanel_VisibleChanged(object sender, EventArgs e)
        {
            
        }

        

        protected override void SetupLabels()
        {
            var labels = new List<Control>
            {
                a50label,
                a90Label,
                a90_95Label,
                MuLabel,
                SigmaLabel,
                modelMLabel,            
                modelBLabel,         
                modelErrorLabel,
                rSqauredLabel,
                modelMStdErrLabel,
                modelBStdErrLabel,
                modelErrorStdErrLabel,
                normalityTestLabel,
                equalVarianceTestLabel,
                lackOfFitTestLabel,
                LeftCensorInputLabel,
                RightCensorInputLabel,
                AMinInputLabel,
                AMaxInputLabel,
                thresholdLabel,
                _xTransformLabel,
                _yTransformLabel,
                _labelForLamdaInput
            };

            foreach(Label label in labels)
            {
                label.TextAlign = ContentAlignment.MiddleRight;
            }
        }

        protected override void SyncSideControls()
        {
            Analysis.IsFrozen = true;
            mainChart.DrawBoundaryLines();

            _xTransformBox.SelectedTransform = Analysis.InFlawTransform;
            _yTransformBox.SelectedTransform = Analysis.InResponseTransform;

            if (Analysis.InFlawTransform != TransformTypeEnum.Inverse)
            {
                MainChart.SetAMaxBoundary(Analysis.TransformValueForXAxis(Analysis.InFlawMax), false);
                MainChart.SetAMinBoundary(Analysis.TransformValueForXAxis(Analysis.InFlawMin), false);
            }
            else
            {
                MainChart.SetAMaxBoundary(Analysis.TransformValueForXAxis(Analysis.InFlawMin), false);
                MainChart.SetAMinBoundary(Analysis.TransformValueForXAxis(Analysis.InFlawMax), false);
            }

            aMaxControl.Value = Convert.ToDecimal(Analysis.InFlawMax);
            aMinControl.Value = Convert.ToDecimal(Analysis.InFlawMin);

            if (Analysis.InResponseTransform != TransformTypeEnum.Inverse)
            {
                mainChart.SetThresholdBoundary(Analysis.TransformValueForYAxis(Analysis.InResponseDecision), false);
                mainChart.SetLeftCensorBoundary(Analysis.TransformValueForYAxis(Analysis.InResponseMin), false);
                mainChart.SetRightCensorBoundary(Analysis.TransformValueForYAxis(Analysis.InResponseMax), false);
            }
            else
            {
                mainChart.SetThresholdBoundary(Analysis.TransformValueForYAxis(Analysis.InResponseDecision), false);
                mainChart.SetLeftCensorBoundary(Analysis.TransformValueForYAxis(Analysis.InResponseMax), false);
                mainChart.SetRightCensorBoundary(Analysis.TransformValueForYAxis(Analysis.InResponseMin), false);
            }
            
            thresholdControl.Value = Convert.ToDecimal(Analysis.InResponseDecision);
            leftCensorControl.Value = Convert.ToDecimal(Analysis.InResponseMin);
            rightCensorControl.Value = Convert.ToDecimal(Analysis.InResponseMax);

            if(_yTransformBox.SelectedIndex.ToString() != "Boxcox")
            {
                _labelForLamdaInput.Enabled = false;
                _boxCoxLambda.Enabled = false;
            }
            else
            {
                _labelForLamdaInput.Enabled = true;
                _boxCoxLambda.Enabled = true;
            }

            Analysis.IsFrozen = false;
        }

        void FullRegressionPanel_Load(object sender, EventArgs e)
        {
            if (Visible == true)
            {
                
            }
        }

        protected override void AddOutputControls()
        {
            base.AddOutputControls();

            outputTablePanel.Controls.Clear();

            _outputWithLabels = new List<Control>
            {
                PodModelParametersHeader,
                a50label, a50Out,
                a90Label, a90Out,
                a90_95Label, a90_95Out,
                MuLabel, MuOut,
                SigmaLabel, SigmaOut,

                LinearFitEstimatesHeader,
                modelMLabel, modelMOut,                
                modelBLabel, modelBOut,                
                modelErrorLabel, modelErrorOut,
                rSqauredLabel, rSquaredValueOut,

                LinearFitStdErrorHeader,
                modelMStdErrLabel, modelMStdErrOut,
                modelBStdErrLabel, modelBStdErrOut,
                modelErrorStdErrLabel, modelErrorStdErrOut,
                
                TestOfAssumptionsHeader,
                normalityTestLabel, normalityTestOut,
                equalVarianceTestLabel, equalVarianceTestOut,
                lackOfFitTestLabel, lackOfFitTestOut,
                //autoCorrelationTestLabel, autoCorrelationTestOut,
                TestColorMap
            };

            outputTablePanel.SetColumnSpan(TestColorMap, 2);
            outputTablePanel.SetColumnSpan(PodModelParametersHeader, 2);
            outputTablePanel.SetColumnSpan(LinearFitEstimatesHeader, 2);
            outputTablePanel.SetColumnSpan(LinearFitStdErrorHeader, 2);
            outputTablePanel.SetColumnSpan(TestOfAssumptionsHeader, 2);

            foreach (Control control in _outputWithLabels)
            {
                control.Dock = DockStyle.Fill;
                control.TabIndex = _tabIndex++;
                //control.Padding = new Padding(0, 0, 100, 0);
            }

            outputTablePanel.Controls.AddRange(_outputWithLabels.ToArray());

            outputTablePanel.Dock = DockStyle.Fill;

            a50Out.PartType = ChartPartType.A50;
            a90Out.PartType = ChartPartType.A90;
            a90_95Out.PartType = ChartPartType.A9095;
            modelBOut.PartType = ChartPartType.LinearFit;
            modelMOut.PartType = ChartPartType.LinearFit;
            modelErrorOut.PartType = ChartPartType.Undefined;
            rSquaredValueOut.PartType = ChartPartType.Undefined;
            modelMStdErrOut.PartType = ChartPartType.Undefined;
            modelBStdErrOut.PartType = ChartPartType.Undefined;
            modelErrorStdErrOut.PartType = ChartPartType.Undefined;            
            SigmaOut.PartType = ChartPartType.POD;
            MuOut.PartType = ChartPartType.POD;

            

            outputTablePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100.0F));
            outputTablePanel.RowCount = outputTablePanel.RowStyles.Count;

            a50Out.TooltipForNumeric = Globals.SplitIntoLines("Estimated flaw size for 50% POD.");
            a90Out.TooltipForNumeric = Globals.SplitIntoLines("Estimated flaw size for 90% POD.");
            a90_95Out.TooltipForNumeric = Globals.SplitIntoLines("95% confidence bound on a90.");
            MuOut.TooltipForNumeric = Globals.SplitIntoLines("mu parameter of the POD model.");
            SigmaOut.TooltipForNumeric = Globals.SplitIntoLines("Sigma parameter of the POD model.");
            modelMOut.TooltipForNumeric = Globals.SplitIntoLines("Slope of the linear fit of the transformed data.");
            modelBOut.TooltipForNumeric = Globals.SplitIntoLines("Intercept of the linear fit of the transformed data.");
            modelErrorOut.TooltipForNumeric = Globals.SplitIntoLines("Standard deviation of the differences between the average aHat values and the linear fit.");
            rSquaredValueOut.TooltipForNumeric = Globals.SplitIntoLines("Pooled standard deviation of the repeated aHat values for each flaw.");
            modelMStdErrOut.TooltipForNumeric = Globals.SplitIntoLines("Standard deviation of the estimate of the slope. Indicates degree of precision.");
            modelBStdErrOut.TooltipForNumeric = Globals.SplitIntoLines("Standard deviation of the estimate of the intercept. Indicates degree of precision.");
            modelErrorStdErrOut.TooltipForNumeric = Globals.SplitIntoLines("Standard deviation of the estimate of the residual error. Indicates degree of precision.");
            normalityTestOut.TooltipForNumeric = Globals.SplitIntoLines("Normality hypothesis test for the model fit. High p values indicate data compatible with assumption.");
            equalVarianceTestOut.TooltipForNumeric = Globals.SplitIntoLines("Equal variance hypothesis test for the model fit. High p values indicate data compatible with assumption.");
            lackOfFitTestOut.TooltipForNumeric = Globals.SplitIntoLines("Lack of fit hypothesis test for the model fit. High p values indicate data compatible with assumption.");
            //autoCorrelationTestOut.TooltipForNumeric = Globals.SplitIntoLines("Durbin Watson test for auto-correlation. High p values indicate there is auto-correlation in the data.");
        }

        public void CycleTransforms()
        {

            if (Analysis.Data.RowCount > 0)
            {
                try
                {
                    if (_xTransformBox.SelectedTransform == TransformTypeEnum.Linear && _yTransformBox.SelectedTransform == TransformTypeEnum.Linear) //linlin
                    {
                        _xTransformBox.SelectedTransform = TransformTypeEnum.Log; //loglin
                    }
                    else if (_xTransformBox.SelectedTransform == TransformTypeEnum.Log && _yTransformBox.SelectedTransform == TransformTypeEnum.Linear) //loglin
                    {
                        _yTransformBox.SelectedTransform = TransformTypeEnum.Log; //loglog
                    }
                    else if (_xTransformBox.SelectedTransform == TransformTypeEnum.Log && _yTransformBox.SelectedTransform == TransformTypeEnum.Log) //loglog
                    {
                        _xTransformBox.SelectedTransform = TransformTypeEnum.Linear; //linlog
                    }
                    else if (_xTransformBox.SelectedTransform == TransformTypeEnum.Linear && _yTransformBox.SelectedTransform == TransformTypeEnum.Log) //linlog
                    {
                        _yTransformBox.SelectedTransform = TransformTypeEnum.Linear; //linlin
                    }
                }
                catch(Exception exp)
                {
                    MessageBox.Show("CycleTransforms: " + exp.Message);
                }
            }

        }

        protected override void AddInputControls()
        {
            base.AddInputControls();

            //set image list so context menu has same pictures
            MainChart.ContextMenuImageList = aMaxControl.RatingImages;

            inputTablePanel.Controls.Clear();

            IntitalizeTransformBoxes();
            InitializeNumericLambda();
            _inputWithLabels = new List<Control>
            {
                AxisTransformsHeader,
                _xTransformLabel, _xTransformBox,
                _yTransformLabel, _yTransformBox,
                _labelForLamdaInput, _boxCoxLambda,

                FlawRangeHeader,
                AMaxInputLabel, aMaxControl,
                AMinInputLabel, aMinControl,

                ResponseRangeHeader,
                RightCensorInputLabel, rightCensorControl,
                LeftCensorInputLabel, leftCensorControl,

                PODDecisionHeader,
                thresholdLabel, thresholdControl
            };

            _tabIndex = 0;

            foreach (Control control in _inputWithLabels)
            {
                control.Dock = DockStyle.Fill;
                control.TabIndex = _tabIndex++;
            }

            inputTablePanel.Controls.AddRange(_inputWithLabels.ToArray());

            inputTablePanel.SetColumnSpan(AxisTransformsHeader, 2);
            inputTablePanel.SetColumnSpan(FlawRangeHeader, 2);
            inputTablePanel.SetColumnSpan(ResponseRangeHeader, 2);
            inputTablePanel.SetColumnSpan(PODDecisionHeader, 2);

            thresholdControl.PartType = ChartPartType.Decision;
            leftCensorControl.PartType = ChartPartType.CensorLeft;
            rightCensorControl.PartType = ChartPartType.CensorRight;
            aMinControl.PartType = ChartPartType.CrackMin;
            aMaxControl.PartType = ChartPartType.CrackMax;

            inputTablePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100.0F));
            inputTablePanel.RowCount = inputTablePanel.RowStyles.Count;

            StepToolTip.SetToolTip(_xTransformBox, Globals.SplitIntoLines("Transform to apply to the flaws."));
            StepToolTip.SetToolTip(_yTransformBox, Globals.SplitIntoLines("Transform to apply to the responses."));
            StepToolTip.SetToolTip(_boxCoxLambda, Globals.SplitIntoLines("Set Lamda value for Box-cox (Used only when reponse is set to Box-Cox)"));
            aMaxControl.TooltipForNumeric = Globals.SplitIntoLines("Flaw's maximum range.");
            aMinControl.TooltipForNumeric = Globals.SplitIntoLines("Flaw's minimum range.");
            rightCensorControl.TooltipForNumeric = Globals.SplitIntoLines("Maximum response of the inspection system. Used to censor data.");
            leftCensorControl.TooltipForNumeric = Globals.SplitIntoLines("Minimum response that is indistinguishable from noise. Used to censor data.");
            thresholdControl.TooltipForNumeric = Globals.SplitIntoLines("Values above the threshold are hits. Values below are misses. aHat value where 50% POD occurs.");
        }

        private void IntitalizeTransformBoxes()
        {
            PrepareLabelBoxPair(ref _xTransformLabel, "Flaw", ref _xTransformBox);
            PrepareLabelBoxPairYHat(ref _yTransformLabel, "Response", ref _yTransformBox);

            _xTransformBox.SelectedIndex = 0;
            _yTransformBox.SelectedIndex = 0;
        }
        private void InitializeNumericLambda()
        {
            PrepareLabelNumericPair(ref _labelForLamdaInput, "Lambda Value", ref _boxCoxLambda);
        }
        
        protected override void SetupNumericControlEvents()
        {
            aMaxControl.NumericUpDown.ValueChanged += this.aMaxControl_ValueChanged;
            aMinControl.NumericUpDown.ValueChanged += this.aMinControl_ValueChanged;
            leftCensorControl.NumericUpDown.ValueChanged += this.leftCensorControl_ValueChanged;
            rightCensorControl.NumericUpDown.ValueChanged += this.rightControl_ValueChanged;
            thresholdControl.NumericUpDown.ValueChanged += this.thresholdControl_ValueChanged;
            _xTransformBox.SelectedIndexChanged += this.TransformBox_ValueChanged;
            _yTransformBox.SelectedIndexChanged += this.TransformBox_ValueChanged;
            _boxCoxLambda.ValueChanged += this.NumericUpDown_ValueChanged;
        }

        private void TransformBox_ValueChanged(object sender, EventArgs e)
        {
            Analysis.InFlawTransform = _xTransformBox.SelectedTransform;
            Analysis.InResponseTransform = _yTransformBox.SelectedTransform;

            if (Analysis.InFlawTransform != TransformTypeEnum.Inverse)
            {
                var x = Convert.ToDouble(Analysis.TransformValueForXAxis(aMaxControl.Value));
                mainChart.SetAMaxBoundary(x, false);
                x = Convert.ToDouble(Analysis.TransformValueForXAxis(aMinControl.Value));
                mainChart.SetAMinBoundary(x, false);
            }
            else
            {
                /*
                var x = Convert.ToDouble(Analysis.TransformValueForXAxis(aMaxControl.Value));
                mainChart.SetAMinBoundary(x, false);
                x = Convert.ToDouble(Analysis.TransformValueForXAxis(aMinControl.Value));
                mainChart.SetAMaxBoundary(x, false);
                */
                var x = Convert.ToDouble(Analysis.TransformValueForXAxis(aMaxControl.Value));
                mainChart.SetAMaxBoundary(x, false);
                x = Convert.ToDouble(Analysis.TransformValueForXAxis(aMinControl.Value));
                mainChart.SetAMinBoundary(x, false);
            }

            if (Analysis.InResponseTransform != TransformTypeEnum.Inverse)
            {
               
                var y = Convert.ToDouble(Analysis.TransformValueForYAxis(leftCensorControl.Value));
                mainChart.SetLeftCensorBoundary(y, false);
                y = Convert.ToDouble(Analysis.TransformValueForYAxis(rightCensorControl.Value));
                mainChart.SetRightCensorBoundary(y, false);
                y = Convert.ToDouble(Analysis.TransformValueForYAxis(thresholdControl.Value));
                mainChart.SetThresholdBoundary(y, false);
            }
            else
            {
                /*
                var y = Convert.ToDouble(Analysis.TransformValueForYAxis(leftCensorControl.Value));
                mainChart.SetRightCensorBoundary(y, false);
                y = Convert.ToDouble(Analysis.TransformValueForYAxis(rightCensorControl.Value));
                mainChart.SetLeftCensorBoundary(y, false);
                y = Convert.ToDouble(Analysis.TransformValueForYAxis(thresholdControl.Value));
                mainChart.SetThresholdBoundary(y, false);
                */
                var y = Convert.ToDouble(Analysis.TransformValueForYAxis(leftCensorControl.Value));
                mainChart.SetLeftCensorBoundary(y, false);
                y = Convert.ToDouble(Analysis.TransformValueForYAxis(rightCensorControl.Value));
                mainChart.SetRightCensorBoundary(y, false);
                y = Convert.ToDouble(Analysis.TransformValueForYAxis(thresholdControl.Value));
                mainChart.SetThresholdBoundary(y, false);
            }
            //get temporary lambda if box-cox is selected
            if (Analysis.InResponseTransform == TransformTypeEnum.BoxCox)
            {
                double lambdaTemp;
                AHatAnalysisObject currAnalysis = Analysis._finalAnalysisAHat;
                List<double> tempFlaws = currAnalysis.Flaws;
                List<double> tempResponses = currAnalysis.Responses[currAnalysis.SignalResponseName];
                TemporaryLambdaCalc TempLambda = new TemporaryLambdaCalc(tempFlaws, tempResponses, Analysis.RDotNet);
                lambdaTemp = TempLambda.CalcTempLambda();
                Analysis.SetTempLambda = lambdaTemp;

                _labelForLamdaInput.Enabled = true;
                _boxCoxLambda.Enabled = true;
                _boxCoxLambda.Value = Convert.ToDecimal(lambdaTemp);
                //keep from running the analyis twice
                //return;
            }
            else
            {
                _labelForLamdaInput.Enabled = false;
                _boxCoxLambda.Enabled = false;
            }

            ForceUpdateAfterTransformChange();
        }
        /*
        private void GetTempLambda()
        {
            //need to set the value of lambda beforehand in boxcox or bounds will be incorrect
            List<double> tempflaws=Analysis._finalAnalysisAHat.Flaws;
            List<double> tempResponses = Analysis._finalAnalysisAHat.Responses[Analysis._finalAnalysisAHat.SignalResponseName];
            Analysis.RDotNet.RDotNetEngine.Evaluate("");
        }
        */

        protected override void SetupNumericControlPrecision()
        {
            var list = new List<PODNumericUpDown> { aMaxControl.NumericUpDown, aMinControl.NumericUpDown, leftCensorControl.NumericUpDown, 
                                                    rightCensorControl.NumericUpDown, thresholdControl.NumericUpDown,
                                                    modelMOut.NumericUpDown, modelMStdErrOut.NumericUpDown, modelBOut.NumericUpDown, modelBStdErrOut.NumericUpDown, 
                                                    modelErrorOut.NumericUpDown, modelErrorStdErrOut.NumericUpDown, 
                                                    normalityTestOut.NumericUpDown, equalVarianceTestOut.NumericUpDown, lackOfFitTestOut.NumericUpDown,  //autoCorrelationTestOut.NumericUpDown,
                                                    rSquaredValueOut.NumericUpDown, MuOut.NumericUpDown, SigmaOut.NumericUpDown};


            foreach (var number in list)
            {
                number.Maximum = decimal.MaxValue;
                number.Minimum = decimal.MinValue;
                number.DecimalPlaces = 3;
            }
        }

        public override void AddSideCharts()
        {
            SideCharts.Add(linearityChart);
            //SideCharts.Add(normalityChart);
            //SideCharts.Add(equalVarianceChart);
            SideCharts.Add(podChart);
            SideCharts.Add(thresholdChart);
        }

        protected override void ColorNumericControls()
        {
            /*aMaxControl.BackColor = Globals.AlphaOverWhiteToOpaque(Color.FromArgb(ChartColors.ControlBackColorAlpha, ChartColors.aMaxColor));
            aMinControl.BackColor = Globals.AlphaOverWhiteToOpaque(Color.FromArgb(ChartColors.ControlBackColorAlpha, ChartColors.aMinColor));
            rightCensorControl.BackColor = Globals.AlphaOverWhiteToOpaque(Color.FromArgb(ChartColors.ControlBackColorAlpha, ChartColors.RightCensorColor));
            leftCensorControl.BackColor = Globals.AlphaOverWhiteToOpaque(Color.FromArgb(ChartColors.ControlBackColorAlpha, ChartColors.LeftCensorColor));
            thresholdControl.BackColor = Globals.AlphaOverWhiteToOpaque(Color.FromArgb(ChartColors.ControlBackColorAlpha, ChartColors.ThresholdColor));
            a50Out.BackColor = Globals.AlphaOverWhiteToOpaque(Color.FromArgb(ChartColors.ControlBackColorAlpha, ChartColors.a50Color));
            a90Out.BackColor = Globals.AlphaOverWhiteToOpaque(Color.FromArgb(ChartColors.ControlBackColorAlpha, ChartColors.a90Color));
            a90_95Out.BackColor = Globals.AlphaOverWhiteToOpaque(Color.FromArgb(ChartColors.ControlBackColorAlpha, ChartColors.a9095Color));
            modelMOut.BackColor = Globals.AlphaOverWhiteToOpaque(Color.FromArgb(ChartColors.ControlBackColorAlpha, ChartColors.FitColor));
            modelBOut.BackColor = Globals.AlphaOverWhiteToOpaque(Color.FromArgb(ChartColors.ControlBackColorAlpha, ChartColors.FitColor));
            modelErrorOut.BackColor = Globals.AlphaOverWhiteToOpaque(Color.FromArgb(ChartColors.ControlBackColorAlpha, ChartColors.FitColor));
            modelMStdErrOut.BackColor = Globals.AlphaOverWhiteToOpaque(Color.FromArgb(ChartColors.ControlBackColorAlpha, ChartColors.FitStdErrorColor));
            modelBStdErrOut.BackColor = Globals.AlphaOverWhiteToOpaque(Color.FromArgb(ChartColors.ControlBackColorAlpha, ChartColors.FitStdErrorColor));
            modelErrorStdErrOut.BackColor = Globals.AlphaOverWhiteToOpaque(Color.FromArgb(ChartColors.ControlBackColorAlpha, ChartColors.FitStdErrorColor));
            normalityTestOut.BackColor = Globals.AlphaOverWhiteToOpaque(Color.FromArgb(ChartColors.ControlBackColorAlpha, ChartColors.TestUnknownColor));
            lackOfFitTestOut.BackColor = Globals.AlphaOverWhiteToOpaque(Color.FromArgb(ChartColors.ControlBackColorAlpha, ChartColors.TestUnknownColor));
            equalVarianceTestOut.BackColor = Globals.AlphaOverWhiteToOpaque(Color.FromArgb(ChartColors.ControlBackColorAlpha, ChartColors.TestUnknownColor));
            rSquaredValueOut.BackColor = Globals.AlphaOverWhiteToOpaque(Color.FromArgb(ChartColors.ControlBackColorAlpha, ChartColors.FitColor));*/
        }        

        public override void SetupAnalysisInput()
        {
            double yMin = mainChart.ChartAreas[0].AxisY.Minimum;

            if (yMin <= 0.0 && _yTransformBox.SelectedTransform == TransformTypeEnum.Log)
                yMin = 1.0;
            // This may be where the problem occurs *****************************
            Analysis.InFlawTransform = _xTransformBox.SelectedTransform;
            Analysis.InResponseTransform = _yTransformBox.SelectedTransform;

            Analysis.InFlawMin = Convert.ToDouble(aMinControl.Value);
            Analysis.InFlawMax = Convert.ToDouble(aMaxControl.Value);
            var xAxis = Analysis.Data.GetXBufferedRange(null, false);
            Analysis.InFlawCalcMax = Convert.ToDouble(Analysis.TransformValueForXAxis(xAxis.Max));
            Analysis.InFlawCalcMin = Convert.ToDouble(Analysis.TransformValueForXAxis(xAxis.Min));
            Analysis.InResponseMin = Convert.ToDouble(leftCensorControl.Value);
            Analysis.InResponseMax = Convert.ToDouble(rightCensorControl.Value);
            Analysis.InResponseDecision = Convert.ToDouble(thresholdControl.Value);
            Analysis.InResponseDecisionMin = Convert.ToDouble(Analysis.InvertTransformValueForYAxis(Convert.ToDecimal(yMin)));
            Analysis.InResponseDecisionMax = Convert.ToDouble(Analysis.InvertTransformValueForYAxis(Convert.ToDecimal(mainChart.ChartAreas[0].AxisY.Maximum)));
            Analysis.InResponseDecisionIncCount = 21;
            
        }

        public override void ProcessAnalysisOutput(Object sender, EventArgs e)
        {
            if (MainChart != null)
                MainChart.ClearProgressBar();

            EnableInputControls();

            mainChart.UpdateBestFitLine();
            

            double a90Transformed = Analysis.OutResponseDecisionPODLevelValue;
            double a9095Transformed = Analysis.OutResponseDecisionPODConfidenceValue;
            double a50Transformed = Analysis.OutResponseDecisionPODA50Value;
            double a90Original = Analysis.OutResponseDecisionPODLevelValue;
            double a9095Original = Analysis.OutResponseDecisionPODConfidenceValue;
            double a50Original = Analysis.OutResponseDecisionPODA50Value;
            double responseMinTransformed = Analysis.InResponseMin;
            double responseMaxTransformed = Analysis.InResponseMax;
            double modelM = Analysis.OutModelSlope;
            double modelB = Analysis.OutModelIntercept;
            double modelError = Analysis.OutModelResidualError;
            double modelMStdError = Analysis.OutModelSlopeStdError;
            double modelBStdError = Analysis.OutModelInterceptStdError;
            double modelErrorStdError = Analysis.OutModelResidualErrorStdError;
            double rSquaredValue = Analysis.OutRepeatabilityError;
            double normality = Analysis.OutTestNormality_p;
            double lackOfFit = Analysis.OutTestLackOfFit_p;
            double equalVariance = Analysis.OutTestEqualVariance_p;
            //double autoCorrelation = Analysis.OutTestAutoCorrelation_p;
            double mu = Analysis.OutResponseDecisionPODA50Value;
            double sigma = Analysis.OutResponseDecisionPODSigma;
            bool lackOfFitCalculated = Analysis.OutTestLackOfFitCalculated;

            var showA50Error = false;
            var showA90Error = false;
            var showA9095Error = false;

            //check for all errors
            if(a50Original < 0.0)
            {
                a50Original = 0.0;
                showA50Error = true;
                
            }
            if(a90Original < 0.0)
            {
                a90Original = 0.0;
                showA90Error = true;
            }

            //don't check for = 0.0 since that means not calculated
            if (a9095Original < 0.0)
            {
                a9095Original = 0.0;
                showA9095Error = true;
            }

            //but just show the first one to fail
            if(showA50Error)
                Source.Python.AddErrorText("Calculated a50 is less than zero. Consider adjusting your threshold.");
            else if(showA90Error)
                Source.Python.AddErrorText("Calculated a90 is less than zero. Consider adjusting your threshold.");
            else if(showA9095Error)
                Source.Python.AddErrorText("Calculated a90/95 is less than zero. Consider adjusting your threshold.");

            try
            {

                a90Transformed = Convert.ToDouble(Analysis.TransformValueForXAxis(Convert.ToDecimal(a90Transformed)));
                a9095Transformed = Convert.ToDouble(Analysis.TransformValueForXAxis(Convert.ToDecimal(a9095Transformed)));
                a50Transformed = Convert.ToDouble(Analysis.TransformValueForXAxis(Convert.ToDecimal(a50Transformed)));
                a90Out.Value = Convert.ToDecimal(a90Original);
                a90_95Out.Value = Convert.ToDecimal(a9095Original);
                a50Out.Value = Convert.ToDecimal(a50Original);
                responseMinTransformed = Convert.ToDouble(Analysis.TransformValueForYAxis(Convert.ToDecimal(responseMinTransformed)));
                responseMaxTransformed = Convert.ToDouble(Analysis.TransformValueForYAxis(Convert.ToDecimal(responseMaxTransformed)));

                modelMOut.Value = Convert.ToDecimal(modelM);
                modelBOut.Value = Convert.ToDecimal(modelB);
                modelErrorOut.Value = Convert.ToDecimal(modelError);
                modelMStdErrOut.Value = Convert.ToDecimal(modelMStdError);
                modelBStdErrOut.Value = Convert.ToDecimal(modelBStdError);
                modelErrorStdErrOut.Value = Convert.ToDecimal(modelErrorStdError);
                rSquaredValueOut.Value = Convert.ToDecimal(rSquaredValue);
                SigmaOut.Value = Convert.ToDecimal(sigma);
                MuOut.Value = Convert.ToDecimal(mu);
                normalityTestOut.Value = Convert.ToDecimal(normality);
                lackOfFitTestOut.Value = Convert.ToDecimal(lackOfFit);
                equalVarianceTestOut.Value = Convert.ToDecimal(equalVariance);
                //autoCorrelationTestOut.Value = Convert.ToDecimal(autoCorrelation);

            }
            catch
            {
                //MessageBox.Show("Analysis Error caused invalid output values that are out of range.");
                Source.Python.AddErrorText("Output values out of range.");

                MainChart.ClearEverythingButPoints();
                linearityChart.ClearEverythingButPoints();
                thresholdChart.ClearEverythingButPoints();
                podChart.ClearEverythingButPoints();

                return;
            }


            TestRating normalityRating = Analysis.GetTestRatingFromLabel(Analysis.OutTestNormalityRating);
            TestRating equalVarianceRating = Analysis.GetTestRatingFromLabel(Analysis.OutTestEqualVarianceRating);
            TestRating lackOfFitRating = Analysis.GetTestRatingFromLabel(Analysis.OutTestLackOfFitRating);
            //TestRating autoCorrelationRating = Analysis.GetTestRatingFromLabel(Analysis.OutTestAutoCorrelationRating);
            normalityTestOut.Rating = normalityRating;
            lackOfFitTestOut.Rating = lackOfFitRating;
            equalVarianceTestOut.Rating = equalVarianceRating;
            //autoCorrelationTestOut.Rating = autoCorrelationRating;
            if (!lackOfFitCalculated)
            {
                lackOfFitTestOut.Rating = TestRating.Undefined;
            }



            mainChart.UpdateLevelConfidenceLines(a50Transformed,
                                                 a90Transformed,
                                                 a9095Transformed,
                                                 Analysis.OutModelSlope, Analysis.OutModelIntercept);

            mainChart.UpdateEquation(Analysis.InFlawTransform, Analysis.InResponseTransform);

            //linearityChart.XAxis.Interval = mainChart.XAxis.Interval;
            //linearityChart.SetXAxisRange(Analysis.Data.GetXBufferedRange(false));
            //linearityChart.XAxis.RoundAxisValues();
            //linearityChart.XAxis.LabelStyle.Format = "F1";
            //SetXAxisRange(Analysis.Data.GetXBufferedRange(true));
            linearityChart.FillChart(Analysis.Data, Analysis.OutModelSlope, Analysis.OutModelIntercept, Analysis.OutModelResidualError, Analysis.OutRepeatabilityError);

            StepToolTip.SetToolTip(linearityChart, linearityChart.TooltipText);

            podChart.FillChart(Analysis.Data);
            podChart.SetXAxisRange(Analysis.Data.GetUncensoredXBufferedRange(podChart, false), Analysis.Data, true);
            podChart.UpdateLevelConfidenceLines(a50Original,
                                                a90Original,
                                                a9095Original);

            StepToolTip.SetToolTip(podChart, podChart.TooltipText);
            podChart.ChartToolTip = StepToolTip;

            thresholdChart.SetXAxisRange(Analysis.Data.GetYBufferedRange(thresholdChart, false), Analysis.Data, true);
            thresholdChart.SetYAxisRange(Analysis.Data.GetUncensoredXBufferedRange(thresholdChart, false), Analysis.Data, true);
            thresholdChart.FillChart(Analysis.Data);
            thresholdChart.UpdateLevelConfidenceLines(a50Original,
                                                      a90Original,
                                                      a9095Original,
                                                      Analysis.InResponseDecision);

            StepToolTip.SetToolTip(thresholdChart, thresholdChart.TooltipText);
            thresholdChart.ChartToolTip = StepToolTip;
            REngineObject.REngineRunning = false;
            //RefreshValues();
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

            _activeControl = aMaxControl;

            var x = Convert.ToDouble(Analysis.TransformValueForXAxis(aMaxControl.Value));
            mainChart.SetAMaxBoundary(x, true);
            Analysis.AnalysisCalculationType = RCalculationType.Full;

            RunAnalysis();

            //controlValueChanged = false;
        }

        private void aMinControl_ValueChanged(object sender, EventArgs e)
        {
            //controlValueChanged = true;

            _activeControl = aMinControl;

            var x = Convert.ToDouble(Analysis.TransformValueForXAxis(aMinControl.Value));
            mainChart.SetAMinBoundary(x, true);
            Analysis.AnalysisCalculationType = RCalculationType.Full;
            RunAnalysis();

            //controlValueChanged = false;
        }

        private void leftCensorControl_ValueChanged(object sender, EventArgs e)
        {
            //controlValueChanged = true;

            _activeControl = leftCensorControl;

            var y = Convert.ToDouble(Analysis.TransformValueForYAxis(leftCensorControl.Value));
            mainChart.SetLeftCensorBoundary(y, true);
            Analysis.AnalysisCalculationType = RCalculationType.Full;
            RunAnalysis();

            //controlValueChanged = false;
        }

        private void rightControl_ValueChanged(object sender, EventArgs e)
        {
            //controlValueChanged = true;

            _activeControl = rightCensorControl;

            var y = Convert.ToDouble(Analysis.TransformValueForYAxis(rightCensorControl.Value));
            mainChart.SetRightCensorBoundary(y, true);
            Analysis.AnalysisCalculationType = RCalculationType.Full;
            RunAnalysis();

            //controlValueChanged = false;
        }

        private void thresholdControl_ValueChanged(object sender, EventArgs e)
        {
            //System.Diagnostics.Stopwatch myWatch = new System.Diagnostics.Stopwatch();
            //myWatch.Start();
            //controlValueChanged = true;

            _activeControl = thresholdControl;

            var y = Convert.ToDouble(Analysis.TransformValueForYAxis(thresholdControl.Value));
            mainChart.SetThresholdBoundary(y, true);
            //set to calculate threshold change only
            Analysis.AnalysisCalculationType = RCalculationType.ThresholdChange;
            RunAnalysis();
        }
        private void NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            Analysis.AnalysisCalculationType = RCalculationType.Full;
            if (_boxCoxLambda.Value==0.0m)
            {
                MessageBox.Show("Setting lambda as 0 is the same as taking a log transform of y! If lambda is already close to 0, use log transform instead.");
                _boxCoxLambda.Value = _previousLambda;
                return;
            }
            double customLambda = Convert.ToDouble(_boxCoxLambda.Value);
            Analysis.SetTempLambda = customLambda;


            ForceUpdateAfterTransformChange();
            _previousLambda = _boxCoxLambda.Value;
        }
        protected override void DisableInputControls()
        {
            //foreach (Annotation anno in MainChart.Annotations)
            //    anno.AllowMoving = false;

            ////_activeControl = ActiveControl;

            //foreach (Control control in inputTablePanel.Controls)
            //{
            //    var label = control as Label;

            //    if (label == null)
            //        control.Enabled = false;
            //}
        }

        protected override void EnableInputControls()
        {
            //foreach (Annotation anno in MainChart.Annotations)
            //    anno.AllowMoving = true;

            //MainChart.DrawBoundaryLines();

            //foreach (Control control in inputTablePanel.Controls)
            //{
            //    var label = control as Label;

            //    if (label == null)
            //        control.Enabled = true;
            //}

            //if (_activeControl != null)
            //    _activeControl.Focus();
        }

        protected override void MainChart_LinesChanged(object sender, EventArgs e)
        {
            double value = 0.0;

            if (Analysis.InFlawTransform != TransformTypeEnum.Inverse)
            {
                if (mainChart.FindValue(ControlLine.AMax, ref value))
                    this.aMaxControl.Value = Analysis.InvertTransformValueForXAxis(Convert.ToDecimal(value));

                if (mainChart.FindValue(ControlLine.AMin, ref value))
                    this.aMinControl.Value = Analysis.InvertTransformValueForXAxis(Convert.ToDecimal(value));
            }
            else
            {
                /*
                if (mainChart.FindValue(ControlLine.AMax, ref value))
                    this.aMinControl.Value = Analysis.InvertTransformValueForXAxis(Convert.ToDecimal(value));

                if (mainChart.FindValue(ControlLine.AMin, ref value))
                    this.aMaxControl.Value = Analysis.InvertTransformValueForXAxis(Convert.ToDecimal(value));
                */
                if (mainChart.FindValue(ControlLine.AMax, ref value))
                    this.aMaxControl.Value = Analysis.InvertTransformValueForXAxis(Convert.ToDecimal(value));

                if (mainChart.FindValue(ControlLine.AMin, ref value))
                    this.aMinControl.Value = Analysis.InvertTransformValueForXAxis(Convert.ToDecimal(value));
            }

            if (Analysis.InResponseTransform != TransformTypeEnum.Inverse)
            {
                if (mainChart.FindValue(ControlLine.LeftCensor, ref value))
                    this.leftCensorControl.Value = Analysis.InvertTransformValueForYAxis(Convert.ToDecimal(value));

                if (mainChart.FindValue(ControlLine.RightCensor, ref value))
                    this.rightCensorControl.Value = Analysis.InvertTransformValueForYAxis(Convert.ToDecimal(value));

                if (mainChart.FindValue(ControlLine.Threshold, ref value))
                    this.thresholdControl.Value = Analysis.InvertTransformValueForYAxis(Convert.ToDecimal(value));
            }
            else
            {
                /*
                if (mainChart.FindValue(ControlLine.LeftCensor, ref value))
                    this.rightCensorControl.Value = Analysis.InvertTransformValueForYAxis(Convert.ToDecimal(value));

                if (mainChart.FindValue(ControlLine.RightCensor, ref value))
                    this.leftCensorControl.Value = Analysis.InvertTransformValueForYAxis(Convert.ToDecimal(value));

                if (mainChart.FindValue(ControlLine.Threshold, ref value))
                    this.thresholdControl.Value = Analysis.InvertTransformValueForYAxis(Convert.ToDecimal(value));
                */
                if (mainChart.FindValue(ControlLine.LeftCensor, ref value))
                    this.leftCensorControl.Value = Analysis.InvertTransformValueForYAxis(Convert.ToDecimal(value));

                if (mainChart.FindValue(ControlLine.RightCensor, ref value))
                    this.rightCensorControl.Value = Analysis.InvertTransformValueForYAxis(Convert.ToDecimal(value));

                if (mainChart.FindValue(ControlLine.Threshold, ref value))
                    this.thresholdControl.Value = Analysis.InvertTransformValueForYAxis(Convert.ToDecimal(value));
            }
        }

        private void controlSplitter_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
