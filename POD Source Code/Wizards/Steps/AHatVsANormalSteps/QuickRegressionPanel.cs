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
using POD.ExcelData;

namespace POD.Wizards.Steps.AHatVsANormalSteps
{
    using System.Data;
    using System.Windows.Forms.DataVisualization.Charting;
    using CSharpBackendWithR;
    using POD.Analyze;
    using POD.Data;

    public partial class QuickRegressionPanel : RegressionPanel
    {
        private TransformBox _xTransformBox;
        private Label _xTransformLabel;
        private TransformBoxYHat _yTransformBox;
        private Label _yTransformLabel;
        SimpleActionBar _tableBar;
        PODButton _updateButton;
        PODButton _deleteButton;
        PODButton _addButton;
        PODButton _pasteButton;
        NewResponseRangeForm _form;
        //used to set lambda for box-cox transformation
        private Label _labelForLamdaInput;
        private LambdaNumericUpDown _boxCoxLambdaQuick;

        private List<Control> _inputWithLabels = new List<Control>();
        private List<Control> _outputWithLabels = new List<Control>();
        //used to keep track of the previous value of lambda in case the user tries to enter 0 into the numeric text box
        private decimal _previousLambda = 1.0m;
        // keeps track of previous threshold to ensure it does not become negative when usng log 'y' transform
        private decimal _previousThreshold = 1.01m;
        public override bool SendKeys(Keys keyData)
        {
            var result = base.SendKeys(keyData);

            if (result)
                return true;

            return false;
        }

        public QuickRegressionPanel(PODToolTip tooltip) : base(tooltip)
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

            ControlSizesFixed = false;

            dataGridView1.ToolTip = StepToolTip;
            dataGridView1.RunAnalysis += Grid_RunAnalysis;

            

            CreateButtons();
        }

        private void Grid_RunAnalysis(object sender, EventArgs e)
        {
            UpdateAnalysisFromTable();
        }

        void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            //UpdateAnalysisFromTable();
        }

        public bool HideTable(bool performHide)
        {
            if(performHide)
                DataTableLayout.Visible = false;
            else
                DataTableLayout.Visible = true;

            return !performHide;
        }

        public void ExportToExcel()
        {
            ExcelExport writer = new ExcelExport();

            Analysis.WriteQuickAnalysis(writer, dataGridView1.DataTable, Analysis.Operator, Analysis.SpecimenSet, Analysis.SpecimentUnit,
                                        Analysis.InFlawMin, Analysis.InFlawMax, Analysis.Instrument, Analysis.InstrumentUnit,
                                        Analysis.InResponseMin, Analysis.InResponseMax);

            writer.SaveToFileWithDefaultName(Analysis.CreateQuickAHatAnalysisName() + Globals.FileTimeStamp);

        }

        private void CreateButtons()
        {
            _tableBar = new SimpleActionBar();

            _tableBar.ToolTip = new PODToolTip();

            _tableBar.RemovePadding();

            _updateButton = _tableBar.AddButton("Refresh Charts", Update_Click, "Refresh charts with data from the table. (Ctrl + Enter)");
            _addButton = _tableBar.AddButton("Insert Row", Add_Click, "Insert new row below the current row. (Ctrl + '='");
            _deleteButton = _tableBar.AddButton("Delete Row", Delete_Click, "Delete the current row from the table. (Delete)");            
            _pasteButton = _tableBar.AddButton("Paste", PasteButton_Click, "Append data from clipboard into the bottom of the table. (Ctrl + V)");

            var availableList = new List<ButtonHolder>();

            availableList.Add(new ButtonHolder(_updateButton.Name, _updateButton.Image, Update_Click, "Refresh charts with data from the table. (Ctrl + Enter)"));
            availableList.Add(new ButtonHolder(_addButton.Name, _addButton.Image, Add_Click, "Insert new row below the current row. (Ctrl + '='"));
            availableList.Add(new ButtonHolder(_deleteButton.Name, _deleteButton.Image, Delete_Click, "Delete the current row from the table. (Delete)"));
            availableList.Add(new ButtonHolder(_pasteButton.Name, _pasteButton.Image, PasteButton_Click, "Append data from clipboard into the bottom of the table. (Ctrl + V)"));

            var tablePanel = ContextMenuStripConnected.MakeNewMenuFlowLayoutPanel("Table_Data");
            

            foreach (var item in availableList)
            {
                ContextMenuStripConnected.AddButtonToMenu(tablePanel, item, StepToolTip);
            }

            var tableHost = new ToolStripControlHost(tablePanel);

            ContextMenuStripConnected.ForcePanelToDraw(tablePanel);

            contextMenu.Items.Add(tableHost);
            ControlStrip.Add(tableHost);
            TextStrip.Add(tableHost);

            ContextMenuStrip = contextMenu;

            _tableBar.Dock = DockStyle.Fill;
            _tableBar.MinimumSize = new Size(availableList.Count * _updateButton.Width, _updateButton.Height);
            DataTableLayout.MinimumSize = new Size(availableList.Count * _updateButton.Width, _updateButton.Height);
            DataTableLayout.Controls.Add(_tableBar, 0, 1);
        }

        private void PasteButton_Click(object sender, EventArgs e)
        {
            PasteFromClipboard();
        }

        private void Add_Click(object sender, EventArgs e)
        {
            AddRow();
            ActiveControl = dataGridView1;
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            DeleteRow();
            ActiveControl = dataGridView1;
        }

        private void Update_Click(object sender, EventArgs e)
        {
            UpdateAnalysisFromTable();
            ActiveControl = dataGridView1;
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

        public void PasteFromClipboard()
        {
            dataGridView1.PasteFromClipboard();
        }

        void FullRegressionPanel_VisibleChanged(object sender, EventArgs e)
        {
            
        }

        public void CycleTransforms()
        {

            if (Analysis.Data.RowCount > 0)
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
                else if (_xTransformBox.SelectedTransform == TransformTypeEnum.Linear && _yTransformBox.SelectedTransform == TransformTypeEnum.Log) //lin Boxcox
                {
                    _xTransformBox.SelectedTransform = TransformTypeEnum.Linear; //lin Boxcox
                    _yTransformBox.SelectedTransform = TransformTypeEnum.BoxCox; 
                }
                else if (_xTransformBox.SelectedTransform == TransformTypeEnum.Linear && _yTransformBox.SelectedTransform == TransformTypeEnum.BoxCox) //log Boxcox
                {
                    _xTransformBox.SelectedTransform = TransformTypeEnum.Log; //log Boxcox
                    _yTransformBox.SelectedTransform = TransformTypeEnum.BoxCox;
                }
                else if (_xTransformBox.SelectedTransform == TransformTypeEnum.Log && _yTransformBox.SelectedTransform == TransformTypeEnum.BoxCox) //linlog
                {
                    _xTransformBox.SelectedTransform = TransformTypeEnum.Linear;
                    _yTransformBox.SelectedTransform = TransformTypeEnum.Linear; //linlin
                }
                /*
                else if (_xTransformBox.SelectedTransform == TransformTypeEnum.Inverse && _yTransformBox.SelectedTransform == TransformTypeEnum.Log) //invlog
                {
                    _yTransformBox.SelectedTransform = TransformTypeEnum.Linear; //invlin
                }
                else if (_xTransformBox.SelectedTransform == TransformTypeEnum.Inverse && _yTransformBox.SelectedTransform == TransformTypeEnum.Linear) //invlin
                {
                    _yTransformBox.SelectedTransform = TransformTypeEnum.Log; //invlog
                }
                else if (_xTransformBox.SelectedTransform == TransformTypeEnum.Log && _yTransformBox.SelectedTransform == TransformTypeEnum.Inverse) //loginv
                {
                    _xTransformBox.SelectedTransform = TransformTypeEnum.Linear; //lininv
                }
                else if (_xTransformBox.SelectedTransform == TransformTypeEnum.Linear && _yTransformBox.SelectedTransform == TransformTypeEnum.Inverse) //lininv
                {
                    _xTransformBox.SelectedTransform = TransformTypeEnum.Log; //loginv
                }
                */
            }

        }

        public override void RefreshValues()
        {
            //MessageBox.Show("Called RefreshValues()");

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

            //dataGridView1.DataSource = Analysis.Data.QuickTable;

            dataGridView1.PrepareDataColumns(Analysis.Data.QuickTable.Columns);
            
            int rowIndex = 0;
            string id = "";
            double flaw = 0.0;
            double response = 0.0;

            //if only have enter new row
            if(dataGridView1.Rows.Count == 1 )
            {
                if(Analysis.UsingInitialGuesses)
                {
                    Analysis.ClearInitialGuesses();
                    Analysis.UserSuppliedRanges = true;

                    _form = new NewResponseRangeForm();

                    _form.ShowDialog();

                    Analysis.InResponseMin = _form.MinResponseValue;
                    Analysis.InResponseMax = _form.MaxResponseValue;
                    Analysis.InResponseDecision = (_form.MaxResponseValue - _form.MinResponseValue) * .15 + _form.MinResponseValue;
                    Analysis.InFlawMin = _form.MinFlawValue;
                    Analysis.InFlawMax = _form.MaxFlawValue;
                    Analysis.Operator = _form.Operator;
                    Analysis.SpecimenSet = _form.SpecimenSet;
                    Analysis.SpecimentUnit = _form.SpecimenUnits;
                    Analysis.Instrument = _form.Instrument;
                    Analysis.InstrumentUnit = _form.InstrumentUnits;
                }
            }
                       

            if (Analysis.Data.ActivatedSpecimenIDs.Rows.Count > 0 && dataGridView1.Rows.Count - 1 != Analysis.Data.ActivatedSpecimenIDs.Rows.Count)
            {
                dataGridView1.Rows.Clear();

                dataGridView1.Rows.Add(Analysis.Data.ActivatedSpecimenIDs.Rows.Count);

                foreach (DataRow row in Analysis.Data.ActivatedSpecimenIDs.Rows)
                {
                    Analysis.Data.GetRow(rowIndex, out id, out flaw, out response);

                    var newRow = dataGridView1.Rows[rowIndex];

                    newRow.Cells[0].Value = id;
                    newRow.Cells[1].Value = flaw.ToString();
                    newRow.Cells[2].Value = response.ToString();

                    rowIndex++;
                }
            }
            
            DataTableLayout.Width = dataGridView1.ColumnCount * 100;
            foreach(DataGridViewColumn col in dataGridView1.Columns)
            {
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                //col.Width = 100;
                col.FillWeight = 1.0F / dataGridView1.Columns.Count;

                if (col.Index == dataGridView1.Columns.Count - 1)
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView1.GridCheckForProblems();
            //UpdateAnalysisFromTable();

            SyncSideControls();
            PrepBeforeQuickRunAnalysis();
            //ResumeDrawing();

            //MessageBox.Show("Finished RefreshValues()");
        }

         

        public void UpdateAnalysisFromTable()
        {
            //MessageBox.Show("Called UpdateAnalysisFromTable()");

            if (dataGridView1.CheckForValidDataGrid(mainChart, Analysis.Name))
            {

                var index = 0;

                dataGridView1.AllowUserToAddRows = false;

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {

                    double flaw = 0.0;
                    double response = 0.0;

                    if (row.Cells[1].Value == null || !double.TryParse(row.Cells[1].Value.ToString(), out flaw) || flaw == double.NaN)
                        continue;

                    if (row.Cells[2].Value == null || !double.TryParse(row.Cells[2].Value.ToString(), out response))
                        response = double.NaN;

                    string id = "";

                    if (row.Cells[0].Value != null)
                        id = row.Cells[0].Value.ToString();
                    else
                        continue;

                    Analysis.Data.AddData(id, flaw, response, index);
                    index++;
                }

                while (Analysis.Data.RowCount > index)
                    Analysis.Data.DeleteRow(Analysis.Data.RowCount - 1);

                dataGridView1.AllowUserToAddRows = true;

                if (dataGridView1.RowCount > 1)
                {
                    //used to fix null reference exception
                    //Analysis.AnalysisDataType = AnalysisDataTypeEnum.AHat;
                    Analysis.Data.DataType= AnalysisDataTypeEnum.AHat;
                    //***********
                    Analysis.Data.RecreateTables();
                    Analysis.ForceUpdateInputsFromData(true, AnalysisDataTypeEnum.AHat);
                    //Analysis.Data.RecreateTables();
                    MainChart.LoadChartData(Analysis.Data);
                    Analysis.Data.ForceRefillSortListAndClearPoints();

                    MainChart.ForceResizeAnnotations();
                    SyncSideControls();
                    MainChart.BuildColorMap();
                    MainChart.DeterminePointsInThreshold();
                    PrepBeforeQuickRunAnalysis();

                    RunAnalysis();
                }
            }

            //MessageBox.Show("Finished UpdateAnalysisFromTable()");
        }

        private void PrepBeforeQuickRunAnalysis()
        {
            MainChart.PickBestAxisRange(Analysis.TransformValueForYAxis(Analysis.InResponseMin), Analysis.TransformValueForYAxis(Analysis.InResponseMax),
                                                Analysis.TransformValueForXAxis(Analysis.InFlawMin), Analysis.TransformValueForXAxis(Analysis.InFlawMax));
            MainChart.DeterminePointsInThreshold();
            MainChart.RefreshStripLines();
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
                repeatabilityErrorLabel,
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
            if(Analysis.Data.LambdaValue != 0)
                _boxCoxLambdaQuick.Value = Convert.ToDecimal(Analysis.Data.LambdaValue);

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

            if (_yTransformBox.SelectedIndex != 3)
            {
                _labelForLamdaInput.Enabled = false;
                _boxCoxLambdaQuick.Enabled = false;
            }
            else
            {
                _labelForLamdaInput.Enabled = true;
                _boxCoxLambdaQuick.Enabled = true;
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
                repeatabilityErrorLabel, repeatabilityErrorOut,

                LinearFitStdErrorHeader,
                modelMStdErrLabel, modelMStdErrOut,
                modelBStdErrLabel, modelBStdErrOut,
                modelErrorStdErrLabel, modelErrorStdErrOut,
                
                TestOfAssumptionsHeader,
                normalityTestLabel, normalityTestOut,
                equalVarianceTestLabel, equalVarianceTestOut,
                lackOfFitTestLabel, lackOfFitTestOut,
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
            repeatabilityErrorOut.PartType = ChartPartType.Undefined;
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
            repeatabilityErrorOut.TooltipForNumeric = Globals.SplitIntoLines("Pooled standard deviation of the repeated aHat values for each flaw.");
            modelMStdErrOut.TooltipForNumeric = Globals.SplitIntoLines("Standard deviation of the estimate of the slope. Indicates degree of precision.");
            modelBStdErrOut.TooltipForNumeric = Globals.SplitIntoLines("Standard deviation of the estimate of the intercept. Indicates degree of precision.");
            modelErrorStdErrOut.TooltipForNumeric = Globals.SplitIntoLines("Standard deviation of the estimate of the residual error. Indicates degree of precision.");
            normalityTestOut.TooltipForNumeric = Globals.SplitIntoLines("Normality hypothesis test for the model fit. High p values indicate data compatible with assumption.");
            equalVarianceTestOut.TooltipForNumeric = Globals.SplitIntoLines("Equal variance hypothesis test for the model fit. High p values indicate data compatible with assumption.");
            lackOfFitTestOut.TooltipForNumeric = Globals.SplitIntoLines("Lack of fit hypothesis test for the model fit. High p values indicate data compatible with assumption.");


        }

        protected override void AddInputControls()
        {
            base.AddInputControls();

            //set image list so context menu has same pictures
            DataPointChart.ContextMenuImageList = aMaxControl.RatingImages;

            inputTablePanel.Controls.Clear();

            IntitalizeTransformBoxes();
            InitializeNumericLambda();
            _inputWithLabels = new List<Control>
            {
                AxisTransformsHeader,
                _xTransformLabel, _xTransformBox,
                _yTransformLabel, _yTransformBox,
                _labelForLamdaInput, _boxCoxLambdaQuick,

                FlawRangeHeader,
                AMaxInputLabel, aMaxControl,
                AMinInputLabel, aMinControl,

                ResponseRangeHeader,
                RightCensorInputLabel, rightCensorControl,
                LeftCensorInputLabel, leftCensorControl,

                PODDecisionHeader,
                thresholdLabel, thresholdControl
            };

            foreach (Control control in _inputWithLabels)
            {
                control.Dock = DockStyle.Fill;
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
            StepToolTip.SetToolTip(_boxCoxLambdaQuick, Globals.SplitIntoLines("Set Lamda value for Box-cox (Used only when reponse is set to Box-Cox)"));
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
            PrepareLabelNumericPair(ref _labelForLamdaInput, "Lambda Value", ref _boxCoxLambdaQuick);
            _boxCoxLambdaQuick.Enabled = false;
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
            _boxCoxLambdaQuick.ValueChanged += this.NumericUpDown_ValueChanged;
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
                var x = Convert.ToDouble(Analysis.TransformValueForXAxis(aMaxControl.Value));
                mainChart.SetAMinBoundary(x, false);
                x = Convert.ToDouble(Analysis.TransformValueForXAxis(aMinControl.Value));
                mainChart.SetAMaxBoundary(x, false);
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
                var y = Convert.ToDouble(Analysis.TransformValueForYAxis(leftCensorControl.Value));
                mainChart.SetRightCensorBoundary(y, false);
                y = Convert.ToDouble(Analysis.TransformValueForYAxis(rightCensorControl.Value));
                mainChart.SetLeftCensorBoundary(y, false);
                y = Convert.ToDouble(Analysis.TransformValueForYAxis(thresholdControl.Value));
                mainChart.SetThresholdBoundary(y, false);
            }
            //get temporary lambda if box-cox is selected
            if (Analysis.InResponseTransform == TransformTypeEnum.BoxCox && Analysis.Data.AHATAnalysisObject.Lambda ==  0.0)
            {
                double lambdaTemp;
                AHatAnalysisObject currAnalysis = Analysis.Data.AHATAnalysisObject;
                List<double> tempFlaws = currAnalysis.Flaws;
                List<double> tempResponses = currAnalysis.Responses[currAnalysis.SignalResponseName];
                TemporaryLambdaCalc TempLambda = new TemporaryLambdaCalc(tempFlaws, tempResponses, Analysis.RDotNet);
                lambdaTemp = TempLambda.CalcTempLambda();
                Analysis.InLambdaValue = lambdaTemp;

                _labelForLamdaInput.Enabled = true;
                _boxCoxLambdaQuick.Enabled = true;
                _boxCoxLambdaQuick.Value = Convert.ToDecimal(lambdaTemp);
                //keep from running the analyis twice
                //return;
            }
            else
            {
                _labelForLamdaInput.Enabled = false;
                _boxCoxLambdaQuick.Enabled = false;
            }
            ForceUpdateAfterTransformChange();
        }

        protected override void SetupNumericControlPrecision()
        {
            var list = new List<PODNumericUpDown> { aMaxControl.NumericUpDown, aMinControl.NumericUpDown, leftCensorControl.NumericUpDown, 
                                                    rightCensorControl.NumericUpDown, thresholdControl.NumericUpDown,
                                                    modelMOut.NumericUpDown, modelMStdErrOut.NumericUpDown, modelBOut.NumericUpDown, modelBStdErrOut.NumericUpDown, 
                                                    modelErrorOut.NumericUpDown, modelErrorStdErrOut.NumericUpDown, 
                                                    normalityTestOut.NumericUpDown, equalVarianceTestOut.NumericUpDown, lackOfFitTestOut.NumericUpDown, 
                                                    repeatabilityErrorOut.NumericUpDown, MuOut.NumericUpDown, SigmaOut.NumericUpDown};


            foreach (var number in list)
            {
                number.Maximum = decimal.MaxValue;
                number.Minimum = decimal.MinValue;
                number.DecimalPlaces = 3;
            }
        }

        public void DeleteRow()
        {
            dataGridView1.DeleteSelectedRow();
        }

        public void AddRow()
        {
            dataGridView1.AddRow();
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

            Analysis.ForceUpdateInputsFromData(true, AnalysisDataTypeEnum.AHat);

            if (yMin <= 0.0 && _yTransformBox.SelectedTransform == TransformTypeEnum.Log)
                yMin = 1.0;

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
            double repeatabilityError = Analysis.OutRSquaredValue;
            double normality = Analysis.OutTestNormality_p;
            double lackOfFit = Analysis.OutTestLackOfFit_p;
            double equalVariance = Analysis.OutTestEqualVariance_p;
            double mu = Analysis.OutResponseDecisionPODA50Value;
            double sigma = Analysis.OutResponseDecisionPODSigma;
            bool lackOfFitCalculated = Analysis.OutTestLackOfFitCalculated;

            

            /*HandleNaN(ref a90Transformed, 0.0);
            HandleNaN(ref a9095Transformed, 0.0);
            HandleNaN(ref a50Transformed, 0.0);
            HandleNaN(ref a90Original, 0.0);
            HandleNaN(ref a9095Original, 0.0);
            HandleNaN(ref a50Original, 0.0);
            HandleNaN(ref responseMinTransformed, 0.0);
            HandleNaN(ref responseMaxTransformed, 0.0);

            HandleNaN(ref modelM, 0.0);
            HandleNaN(ref modelB, 0.0);
            HandleNaN(ref modelError, 0.0);
            HandleNaN(ref modelMStdError, 0.0);
            HandleNaN(ref modelBStdError, 0.0);
            HandleNaN(ref modelErrorStdError, 0.0);
            HandleNaN(ref repeatabilityError, 0.0);

            HandleNaN(ref normality, 0.0);
            HandleNaN(ref lackOfFit, 0.0);
            HandleNaN(ref equalVariance, 0.0);*/

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
                repeatabilityErrorOut.Value = Convert.ToDecimal(repeatabilityError);
                SigmaOut.Value = Convert.ToDecimal(sigma);
                MuOut.Value = Convert.ToDecimal(mu);

                normalityTestOut.Value = Convert.ToDecimal(normality);
                lackOfFitTestOut.Value = Convert.ToDecimal(lackOfFit);
                equalVarianceTestOut.Value = Convert.ToDecimal(equalVariance);

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

            normalityTestOut.Rating = normalityRating;
            lackOfFitTestOut.Rating = lackOfFitRating;
            equalVarianceTestOut.Rating = equalVarianceRating;
            
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
            linearityChart.FillChart(Analysis.Data, Analysis.OutModelSlope, Analysis.OutModelIntercept, Analysis.OutModelResidualError, Analysis.OutRSquaredValue);

            StepToolTip.SetToolTip(linearityChart, linearityChart.TooltipText);
            linearityChart.ChartToolTip = StepToolTip;

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

        public override void RunAnalysis(bool disableControls = true)
        {
            //ResumeDrawing();

            if (dataGridView1.CheckForValidDataGrid(mainChart, Analysis.Name))
            {
                var analysis = (Analysis)_source;

                analysis.AnalysisDone -= ProcessAnalysisOutput;
                analysis.AnalysisDone -= ProcessAnalysisOutput;
                analysis.AnalysisDone -= ProcessAnalysisOutput;
                analysis.AnalysisDone += ProcessAnalysisOutput;

                if (MainChart != null)
                {
                    PrepBeforeQuickRunAnalysis();
                    if(disableControls)
                        DisableInputControls();
                    MainChart.PrepareForRunAnalysis();
                    MainChart.ResetErrors();
                    MainChart.ClearProgressBar();
                }

                SetupAnalysisInput();

                this.Cursor = Cursors.WaitCursor;

                Analysis.RunAnalysis(true);

                this.Cursor = Cursors.Default;
            }

            

            //ProcessAnalysisOutput(Analysis, null);

            //RefreshValues();

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

        private void leftCensorControl_ValueChanged(object sender, EventArgs e)
        {
            //controlValueChanged = true;

            var y = Convert.ToDouble(Analysis.TransformValueForYAxis(leftCensorControl.Value));
            mainChart.SetLeftCensorBoundary(y, true);

            RunAnalysis();

            //controlValueChanged = false;
        }

        private void rightControl_ValueChanged(object sender, EventArgs e)
        {
            //controlValueChanged = true;

            var y = Convert.ToDouble(Analysis.TransformValueForYAxis(rightCensorControl.Value));
            mainChart.SetRightCensorBoundary(y, true);

            RunAnalysis();

            //controlValueChanged = false;
        }

        private void thresholdControl_ValueChanged(object sender, EventArgs e)
        {
            if(thresholdControl.Value < 0 && _yTransformBox.SelectedIndex == 1)
            {
                thresholdControl.Value = Convert.ToDecimal(_previousThreshold);
            }
            var y = Convert.ToDouble(Analysis.TransformValueForYAxis(thresholdControl.Value));
            mainChart.SetThresholdBoundary(y, true);
            //set to calculate threshold change only
            Analysis.AnalysisCalculationType = RCalculationType.ThresholdChange;
            RunAnalysis(false);
            _previousThreshold = thresholdControl.Value;
        }
        private void NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            Analysis.AnalysisCalculationType = RCalculationType.Full;
            if (_boxCoxLambdaQuick.Value == 0.0m)
            {
                MessageBox.Show("Setting lambda as 0 is the same as taking a log transform of y! If lambda is already close to 0, use log transform instead.");
                _boxCoxLambdaQuick.Value = _previousLambda;
                return;
            }
            double customLambda = Convert.ToDouble(_boxCoxLambdaQuick.Value);
            Analysis.InLambdaValue = customLambda;


            ForceUpdateAfterTransformChange();
            _previousLambda = _boxCoxLambdaQuick.Value;
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
                if (mainChart.FindValue(ControlLine.AMax, ref value))
                    this.aMinControl.Value = Analysis.InvertTransformValueForXAxis(Convert.ToDecimal(value));

                if (mainChart.FindValue(ControlLine.AMin, ref value))
                    this.aMaxControl.Value = Analysis.InvertTransformValueForXAxis(Convert.ToDecimal(value));
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
                if (mainChart.FindValue(ControlLine.LeftCensor, ref value))
                    this.rightCensorControl.Value = Analysis.InvertTransformValueForYAxis(Convert.ToDecimal(value));

                if (mainChart.FindValue(ControlLine.RightCensor, ref value))
                    this.leftCensorControl.Value = Analysis.InvertTransformValueForYAxis(Convert.ToDecimal(value));

                if (mainChart.FindValue(ControlLine.Threshold, ref value))
                    this.thresholdControl.Value = Analysis.InvertTransformValueForYAxis(Convert.ToDecimal(value));
            }
        }        
    }
}
