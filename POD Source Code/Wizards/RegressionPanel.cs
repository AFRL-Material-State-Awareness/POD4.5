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
using CSharpBackendWithR;
namespace POD.Wizards
{
    using System.Data;
    using System.Windows.Forms.DataVisualization.Charting;

    using POD.Analyze;
    using POD.Data;
    using POD.Wizards.Steps.AHatVsANormalSteps;

    public partial class RegressionPanel : SideChartPanel
    {
        protected int _nineCharWidth = 100;
        protected Control _activeControl;
        //use to indicate if a known error was found - current only for hitmiss
        protected bool _knownErrorFound;
        public RegressionPanel()
            : base()
        {
            Initialize();
        }

        public RegressionPanel(PODToolTip tooltip) : base(tooltip)
        {
            StepToolTip = new PODToolTip();

            Initialize();
        }

        private void Initialize()
        {
            InitializeComponent();

            graphSplitter.Panel1MinSize = 70;
            graphSplitter.Panel2MinSize = 70;
            graphFlowPanel.MinimumSize = new System.Drawing.Size(70, 0);

            SetSideControls(graphFlowPanel, graphSplitter, graphSplitterOverlay);

            InputHideButton.BackgroundImage = ExpandContractList.Images[0];
            OutputHideButton.BackgroundImage = ExpandContractList.Images[0];

            Load += RegressionPanel_Load;
        }

        void RegressionPanel_Load(object sender, EventArgs e)
        {
            /*if (!DesignMode)
            {
                if (!ControlSizesFixed)
                {
                    if (Width != Parent.Width)
                        Width = Parent.Width;

                    FixPanelControlSizes();
                }
            }*/
        }

        public void Add_Error_OnChart(object sender, ErrorArgs e)
        {
            if (MainChart != null && Source.Name == e.AnalysisName)
            {
                MainChart.AddError(e);                
            }
        }

        public void Show_Error_OnChart(object sender, ErrorArgs e)
        {
            if (MainChart != null && Source.Name == e.AnalysisName)
            {
                MainChart.FinalizeErrors(e);
            }
        }

        protected void FixTableSize(TableLayoutPanel myPanel)
        {
            var isTooBig = false;

            foreach (int rowHeight in myPanel.GetRowHeights())
            {
                if (rowHeight < 0)
                {
                    isTooBig = true;
                    break;
                }
            }

            AdjustColumnsMode(isTooBig);

        }

        

        public void Show_Progress_OnChart(object sender, ErrorArgs e)
        {
            if (MainChart != null && Source.Name == e.AnalysisName)
            {
                MainChart.ShowProgressBar(e);
            }
        }

        public override WizardSource Source
        {
            get
            {
                return _source;
            }
            set
            {
                _source = value;

                
            }
        }

        protected virtual void AddOutputControls()
        {

        }

        protected virtual void AddInputControls()
        {

        }
        public static void PrepareLabelNumericPair(ref Label myLabel, string myName, ref LambdaNumericUpDown myNumeric)
        {
            myLabel = new Label();

            myNumeric = new LambdaNumericUpDown();

            myLabel.Text = myName;
            myLabel.AutoSize = true;
            myLabel.Dock = DockStyle.Fill;

            myNumeric.Dock = DockStyle.Fill;
            myNumeric.DecimalPlaces = 3;
            myNumeric.Minimum = -100;
            myNumeric.Maximum = 100;
            myNumeric.Value = .99m;
        }

        public static void PrepareLabelBoxPair(ref Label myLabel, string myName, ref TransformBox myBox)
        {
            myLabel = new Label();

            myBox = new TransformBox();

            myLabel.Text = myName;
            myLabel.AutoSize = true;
            myLabel.Dock = DockStyle.Fill;

            myBox.Dock = DockStyle.Fill;
            myBox.SelectedIndex = 1;
        }
        public static void PrepareLabelBoxPairYHat(ref Label myLabel, string myName, ref TransformBoxYHat myBoxYHat)
        {
            myLabel = new Label();

            myBoxYHat = new TransformBoxYHat();

            myLabel.Text = myName;
            myLabel.AutoSize = true;
            myLabel.Dock = DockStyle.Fill;

            myBoxYHat.Dock = DockStyle.Fill;
            myBoxYHat.SelectedIndex = 1;
        }
        public static void PrepareLabelBoxPairConfint(ref Label myLabel, string myName, ref ConfidenceBox myBox)
        {
            myLabel = new Label();

            myBox = new ConfidenceBox();

            myLabel.Text = myName;
            myLabel.AutoSize = true;
            myLabel.Dock = DockStyle.Fill;

            myBox.Dock = DockStyle.Fill;
            myBox.SelectedIndex = 1;
        }
        public static void PrepareLabelBoxPairSamplingType(ref Label myLabel, string myName, ref SamplingTypeBox myBox)
        {
            myLabel = new Label();

            myBox = new SamplingTypeBox();

            myLabel.Text = myName;
            myLabel.AutoSize = true;
            myLabel.Dock = DockStyle.Fill;

            myBox.Dock = DockStyle.Fill;
            myBox.SelectedIndex = 1;
        }

        protected void FixColumnWidth(TableLayoutPanel outputTablePanel, PODChartNumericUpDown myNumeric)
        {
            var graphics = myNumeric.CreateGraphics();

            var newLength = FontHelper.MeasureDisplayStringWidth(graphics, "0000000000.000", myNumeric.Font) + FontHelper.MeasureDisplayStringWidth(graphics, "00", myNumeric.Font) + 20;

            if (_nineCharWidth != newLength)
            {
                _nineCharWidth = newLength;
                ResizeControlsToAllFit(outputTablePanel);
            }

        }

        public void ResizeControlsToAllFit(TableLayoutPanel myTable)
        {
            

            foreach (Control control in myTable.Controls)
            {
                var numeric = control as PODImageNumericUpDown;

                if (numeric != null)
                {
                    var subtractWidth = _nineCharWidth - control.Size.Width;

                    if (subtractWidth < MainChartRightControlsSplitter.SplitterDistance)
                    {
                        MainChartRightControlsSplitter.SplitterDistance -= subtractWidth;
                        MainChartRightControlsSplitter.Refresh();
                    }

                    break;
                }

            }
        }

        /// <summary>
        /// Add event handling for the main chart. Make sure to assign MainChart in constrcutor before calling this.
        /// </summary>
        protected virtual void SetupChartEvents()
        {
            if (MainChart != null)
            {
                MainChart.RunAnalysisNeeded += mainChart_RunAnalysisNeeded;
                MainChart.LinesChanged += MainChart_LinesChanged;
            }
        }

        protected void mainChart_RunAnalysisNeeded(object sender, EventArgs e)
        {
            Analysis.AnalysisCalculationType = RCalculationType.Full;
            RunAnalysis();
        }

        public RegressionAnalysisChart MainChart { get; set; }

        protected void ForceUpdateAfterTransformChange()
        {
            if (MainChart != null)
            {
                MainChart.ForceReloadChartData();
                MainChart.ForceRefillSortList();
                MainChart.PickBestAxisRange();
                MainChart.ForceIncludedPointsUpdate();
                //if (MainChart.ChartAreas[0].AxisX.Maximum< MainChart.ChartAreas[0].AxisX.Minimum)
                //{
                //    //MainChart.ChartAreas[0].AxisX.Maximum = MainChart.ChartAreas[0].AxisX.Minimum;
                //    MainChart.ChartAreas[0].AxisX.Maximum = 1.0/Analysis.MinFlawSize;
                //    MainChart.ChartAreas[0].AxisX.Minimum = 0;
                //}
            }
            Analysis.AnalysisCalculationType = RCalculationType.Full;
            if (!Analysis.IsFrozen)
                RunAnalysis();
        }
        /*
        public void ForceUpdateAfterLambdaChange()
        {
            MainChart.ForceReloadChartData();
            MainChart.PickBestAxisRange();
            Analysis.AnalysisCalculationType = RCalculationType.Full;
            if (!Analysis.IsFrozen)
                RunAnalysis();
        }
        */

        protected virtual void SetupNumericControlEvents()
        {
        }

        

        public override void AddSideCharts()
        {
        }

        protected virtual void ColorNumericControls()
        {
        }
        
        public virtual void SetupAnalysisInput()
        {
        }

        public virtual void ProcessAnalysisOutput(Object sender, EventArgs e)
        {
        }

        protected virtual void SetupLabels()
        {
            
        }

        public virtual void RunAnalysis()
        {
            //ResumeDrawing();
            REngineObject.REngineRunning = true;
            var analysis = (Analysis)_source;

            analysis.AnalysisDone -= ProcessAnalysisOutput;
            analysis.AnalysisDone -= ProcessAnalysisOutput;
            analysis.AnalysisDone -= ProcessAnalysisOutput;
            analysis.AnalysisDone += ProcessAnalysisOutput;

            if (MainChart != null)
            {
                DisableInputControls();
                MainChart.PrepareForRunAnalysis();
                MainChart.ResetErrors();
                MainChart.ClearProgressBar();
            }

            SetupAnalysisInput();

            this.Cursor = Cursors.WaitCursor;

            Analysis.RunAnalysis();

            this.Cursor = Cursors.Default;

            //ProcessAnalysisOutput(Analysis, null);

            //RefreshValues();

        }

        protected virtual void DisableInputControls()
        {
            foreach (Annotation anno in MainChart.Annotations)
                anno.AllowMoving = false;

            //_activeControl = ActiveControl;
            
            foreach(Control control in inputTablePanel.Controls)
            {
                var label = control as Label;

                if (label == null)
                    control.Enabled = false;
            }
            WizActionBar.DisableNav();
        }

        protected virtual void EnableInputControls()
        {
            foreach (Annotation anno in MainChart.Annotations)
                anno.AllowMoving = true;

            MainChart.DrawBoundaryLines();

            foreach (Control control in inputTablePanel.Controls)
            {
                var label = control as Label;

                if (label == null)
                    control.Enabled = true;
            }

            if(_activeControl != null)
                _activeControl.Focus();

            WizActionBar.EnableNav();
        }
        /*
        protected void HandleNaN(ref double myValue, double mySafeValue)
        {
            if (Double.IsNaN(myValue))
                myValue = mySafeValue;
        } 
        */

        protected virtual void Panel_Resize(object sender, EventArgs e)
        {
            
            ResizeAlternateChart();

            //FixTableSize(outputTablePanel);

            /*var realHeight = RowHeight(outputTablePanel) + RowHeight(inputTablePanel);

            if ((realHeight > MainChart.Height && RightControlsTablePanel.ColumnStyles[1].Width == 0) ||
                (realHeight <= MainChart.Height && RightControlsTablePanel.ColumnStyles[1].Width != 0))
            {
                Shift_Inputs(null, null);
            }*/

        }

        public void LiteRefreshValues()
        {
            //Analysis.CalculateInitialValuesWithNewData();
            SetupSideCharts();

            if (MainChart != null)
            {
                //MainChart.LoadChartData(Analysis.Data);
                MainChart.PickBestAxisRange();
                MainChart.ForceResizeAnnotations();
            }

            ResumeDrawing();
        }

        public override void RefreshValues()
        {
            //SuspendDrawing();
            Analysis.Data.FilterTransformedDataByRanges = false;
            Analysis.CalculateInitialValuesWithNewData();
            SetupSideCharts();

            if (MainChart != null)
            {
                AddChartListeningForErrors();

                MainChart.LoadChartData(Analysis.Data);
                MainChart.PickBestAxisRange();
                MainChart.ForceResizeAnnotations();
            }

            SyncSideControls();
            ResumeDrawing();
        }

        public void UpdateAnalysis()
        {
            var analysis = _source as Analysis;
            analysis.AnalysisDone -= ProcessAnalysisOutput;
            analysis.AnalysisDone -= ProcessAnalysisOutput;
            analysis.AnalysisDone -= ProcessAnalysisOutput;

            RemoveChartListeningForErrors();
        }

        public void RemoveAllEvents(object sender, EventArgs e)
        {
             RemoveChartListeningForErrors();
        }

        protected void AddChartListeningForErrors()
        {
            RemoveChartListeningForErrors();

            Analysis.Python.OnAnalysisError += Add_Error_OnChart;
            Analysis.Python.OnProgressUpdate += Show_Progress_OnChart;
            Analysis.Python.OnAnalysisFinish += Show_Error_OnChart;

            Analysis.EventsNeedToBeCleared -= RemoveAllEvents;
            Analysis.EventsNeedToBeCleared += RemoveAllEvents;
        }

        protected void RemoveChartListeningForErrors()
        {
            Analysis.Python.OnAnalysisError -= Add_Error_OnChart;
            Analysis.Python.OnProgressUpdate -= Show_Progress_OnChart;
            Analysis.Python.OnAnalysisFinish -= Show_Error_OnChart;
        }

        protected virtual void SyncSideControls()
        {
            
        }

        protected void FixPutControlsHere()
        {
            PutControlsHerePanel.Controls.Clear();
            PutControlsHerePanel.Dock = DockStyle.None;
            PutControlsHerePanel.Size = new Size(0, 0);
        }

        
        protected virtual void MainChart_LinesChanged(object sender, EventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        protected virtual void MakeInputMatchOutputWidth(Control myControl)
        {
            inputTablePanel.ColumnStyles[0].SizeType = SizeType.Absolute;
            inputTablePanel.ColumnStyles[0].Width = myControl.Width + 6;

            inputTablePanel.ColumnStyles[1].SizeType = SizeType.Absolute;
            inputTablePanel.ColumnStyles[1].Width = myControl.Width - 6;
        }

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {
            
        }

        public int RowHeight(TableLayoutPanel myPanel)
        {
            int height = 0;

            foreach(int rowHeight in myPanel.GetRowHeights())
            {
                if (rowHeight > 0)
                    height += rowHeight;
            }

            return height;
        }

        private void AdjustColumnsMode(bool isTooBig)
        {
            inputTablePanel.SuspendLayout();
            outputTablePanel.SuspendLayout();
            RightControlsTablePanel.SuspendLayout();
            MainChartRightControlsSplitter.SuspendLayout();

            
            if (isTooBig)
            {
                RightControlsTablePanel.SetColumn(OutputLabelTable, 1);
                RightControlsTablePanel.SetColumn(outputTablePanel, 1);
                RightControlsTablePanel.SetRow(OutputLabelTable, 0);
                RightControlsTablePanel.SetRow(outputTablePanel, 1);
                RightControlsTablePanel.ColumnStyles[1].SizeType = SizeType.Percent;
                RightControlsTablePanel.ColumnStyles[1].Width = 100;
                var subtract = MainChartRightControlsSplitter.SplitterDistance - inputTablePanel.Width;
                if (subtract > 0)
                    MainChartRightControlsSplitter.SplitterDistance = subtract;
                ShiftOutputButton.BackgroundImage = ShiftLeftRightList.Images[1];
                ShiftInputButton.BackgroundImage = ShiftLeftRightList.Images[1];
            }
            else if (RightControlsTablePanel.ColumnStyles[1].SizeType == SizeType.Percent)
            {
                RightControlsTablePanel.SetColumn(OutputLabelTable, 0);
                RightControlsTablePanel.SetColumn(outputTablePanel, 0);
                RightControlsTablePanel.SetRow(OutputLabelTable, 2);
                RightControlsTablePanel.SetRow(outputTablePanel, 3);
                RightControlsTablePanel.ColumnStyles[1].SizeType = SizeType.Absolute;
                RightControlsTablePanel.ColumnStyles[1].Width = 0;
                MainChartRightControlsSplitter.SplitterDistance = MainChartRightControlsSplitter.SplitterDistance + inputTablePanel.Width;
                ShiftOutputButton.BackgroundImage = ShiftLeftRightList.Images[0];
                ShiftInputButton.BackgroundImage = ShiftLeftRightList.Images[0];
            }
            

            outputTablePanel.ResumeLayout();
            inputTablePanel.ResumeLayout();
            RightControlsTablePanel.ResumeLayout();
            MainChartRightControlsSplitter.ResumeLayout();
            
        }

        protected void Shift_Inputs(object sender, EventArgs e)
        {
            AdjustColumnsMode(RightControlsTablePanel.ColumnStyles[1].Width == 0);
            
        }

        private void ExpandContract_Inputs(object sender, EventArgs e)
        {
            ExpandContractPanel(inputTablePanel, RightControlsTablePanel, InputHideButton, null);
        }

        private void ExpandContract_Outputs(object sender, EventArgs e)
        {
            ExpandContractPanel(outputTablePanel, RightControlsTablePanel, OutputHideButton, null);
        }

        private void ExpandContractPanel(TableLayoutPanel myControlPanel, TableLayoutPanel myMainPanel, Button myCollaspeButton, Button myShiftButton)
        {
            myControlPanel.SuspendLayout();
            myMainPanel.SuspendLayout();

            if (myControlPanel.AutoSize == true)
            {
                myControlPanel.AutoSize = false;
                myControlPanel.Height = 0;
                myControlPanel.Dock = DockStyle.None;
                myCollaspeButton.BackgroundImage = ExpandContractList.Images[1];
                //outputTablePanel.AutoScroll = true;
                //outputTablePanel.AutoScrollMargin = new Size(0, 20);
            }
            else
            {
                myControlPanel.AutoSize = true;
                myControlPanel.Dock = DockStyle.Top;
                myCollaspeButton.BackgroundImage = ExpandContractList.Images[0];
                //outputTablePanel.AutoScroll = true;
                //outputTablePanel.AutoScrollMargin = new Size(0, 20);
            }

            myControlPanel.ResumeLayout();
            myMainPanel.ResumeLayout();
        }

        private void Panel_Paint(object sender, PaintEventArgs e)
        {
            
        }
        //get specific hit miss analysis error if possible
        public void SearhForHitMissErrors(double a9095Original)
        {
            //get the info for the current stats of the hit miss object
            CSharpBackendWithR.HMAnalysisObject currHitMissSettings = Analysis._finalAnalysis;
            if (!(Analysis.FailToConverge) && Double.IsNaN(a9095Original))
            {
                _knownErrorFound = true;
                Source.Python.AddErrorText("It appears the logistic regression converged, but A9095 is either very large number or infinity. " + '\n' +
                    "Try using Likelihood Ratio (LR) or Modified Likelihood Ratio (MLR)" + '\n' + "Confidence intervals and/or Ranked Set Sampling");
            }
            else if (Analysis.FailToConverge)
            {
                _knownErrorFound = true;
                Source.Python.AddErrorText("Maximum Likelihood Logistic Regression failed to converge. " + '\n' +
                    "Try the Firth Logistic Regression Model Type Instead");
            }
            else if (currHitMissSettings.RegressionType == "Firth Logistic Regression" && Double.IsNaN(a9095Original))
            {
                _knownErrorFound = true;
                if (currHitMissSettings.CIType == "LR" || currHitMissSettings.CIType == "MLR")
                {
                    Source.Python.AddErrorText("A9095 is either infinity or a very large number! You could try" + '\n' +
                        "to combine Ranked Set Sampling with the current Confidence Interval" + '\n' +
                        "(WARNING: can take anywhere from 5-20min)");
                }
                else if (currHitMissSettings.SrsOrRSS == 1 && (currHitMissSettings.CIType == "LR" || currHitMissSettings.CIType == "MLR"))
                {
                    Source.Python.AddErrorText("Error: A9095 doesn't exist or is too large to be useful" + '\n' +
                        "Try switching between LR and MLR with the current settings." + '\n' +
                        "Data's sample size may be too small and/or the data does not have enough overlap");
                }
                else
                {
                    Source.Python.AddErrorText("A9095 is either infinity or a very large number! Try Likelihood Ratio (LR) or" + '\n' +
                        "Modified Likelihood Ratio (MLR) Confidence Intervals for best results.");
                }

            }
        }
    }
}
