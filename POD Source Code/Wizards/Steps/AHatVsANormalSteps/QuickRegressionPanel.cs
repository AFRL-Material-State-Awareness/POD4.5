// <copyright file="QuickRegressionPanel.cs" company="The University of Dayton">
// Copyright © 2025 The University of Dayton. All rights reserved.
//
// Licensed under the GNU General Public License, Version 3.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.gnu.org/licenses/gpl-3.0.txt
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

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
using System.Diagnostics;

namespace POD.Wizards.Steps.AHatVsANormalSteps
{
    using System.Data;
    using System.Threading;
    using System.Windows.Forms.DataVisualization.Charting;
    using CSharpBackendWithR;
    using POD.Analyze;
    using POD.Data;

    /// <summary>
    /// Represents a panel for performing a "quick" regression analysis.
    /// This panel provides an interactive grid for data input and controls for adjusting analysis parameters in real-time.
    /// It is designed for rapid, exploratory data analysis as part of the a-hat vs. a regression workflow.
    /// </summary>
    public partial class QuickRegressionPanel : RegressionPanel
    {
        private TransformBoxX _xTransformBox;
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

        // used to snap the left censor line back to a valid negative position on a log scale
        private decimal _lastValidNegativeLeftCensor = -10.0m;

        /// <summary>
        /// Processes keyboard shortcuts for the panel.
        /// </summary>
        /// <param name="keyData">The key data to process.</param>
        /// <returns><c>true</c> if the key was processed; otherwise, <c>false</c>.</returns>
        public override bool SendKeys(Keys keyData)
        {
            var result = base.SendKeys(keyData);

            if (result)
                return true;

            return false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuickRegressionPanel"/> class.
        /// </summary>
        /// <param name="tooltip">The tooltip provider for the controls.</param>
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

            this.DisplayPanel.BackgroundControl = this.MainChart;
        }

        /// <summary>
        /// Handles the RunAnalysis event from the data grid, triggering an update of the analysis.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> that contains no event data.</param>
        private void Grid_RunAnalysis(object sender, EventArgs e)
        {
            //Debug.WriteLine("[DEBUG-QRP] Grid_RunAnalysis triggered.");
            UpdateAnalysisFromTable();
        }

        /// <summary>
        /// Handles the UserAddedRow event of the dataGridView1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DataGridViewRowEventArgs"/> instance containing the event data.</param>
        void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            //UpdateAnalysisFromTable();
        }

        /// <summary>
        /// Shows or hides the data table layout panel.
        /// </summary>
        /// <param name="performHide">If set to <c>true</c>, hides the table; otherwise, shows it.</param>
        /// <returns>The new visibility state of the table.</returns>
        public bool HideTable(bool performHide)
        {
            if (performHide)
                DataTableLayout.Visible = false;
            else
                DataTableLayout.Visible = true;

            return !performHide;
        }

        /// <summary>
        /// Exports the current analysis data to an Excel file.
        /// </summary>
        public void ExportToExcel()
        {
            ExcelExport writer = new ExcelExport();

            Analysis.WriteQuickAnalysis(writer, dataGridView1.DataTable, Analysis.Operator, Analysis.SpecimenSet, Analysis.SpecimentUnit,
                                        Analysis.InFlawMin, Analysis.InFlawMax, Analysis.Instrument, Analysis.InstrumentUnit,
                                        Analysis.InResponseMin, Analysis.InResponseMax);

            writer.SaveToFileWithDefaultName(Analysis.CreateQuickAHatAnalysisName() + Globals.FileTimeStamp);

        }

        /// <summary>
        /// Creates and configures the buttons for the data table's action bar and context menu.
        /// </summary>
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

        /// <summary>
        /// Handles the Click event of the Paste button. Pastes data from the clipboard into the grid.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> that contains no event data.</param>
        private void PasteButton_Click(object sender, EventArgs e)
        {
            PasteFromClipboard();
        }

        /// <summary>
        /// Handles the Click event of the Add button. Adds a new row to the grid.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> that contains no event data.</param>
        private void Add_Click(object sender, EventArgs e)
        {
            AddRow();
            ActiveControl = dataGridView1;
        }

        /// <summary>
        /// Handles the Click event of the Delete button. Deletes the selected row from the grid.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> that contains no event data.</param>
        private void Delete_Click(object sender, EventArgs e)
        {
            DeleteRow();
            ActiveControl = dataGridView1;
        }

        /// <summary>
        /// Handles the Click event of the Update button. Triggers an analysis update from the table data.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> that contains no event data.</param>
        private void Update_Click(object sender, EventArgs e)
        {
            UpdateAnalysisFromTable();
            ActiveControl = dataGridView1;
        }

        /// <summary>
        /// Fixes the sizes of the controls on the panel. This is typically called once after initialization.
        /// It sets up side chart visibility and sizes, and adjusts column widths in the output table.
        /// </summary>
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

        /// <summary>
        /// Pastes data from the system clipboard into the data grid.
        /// </summary>
        public void PasteFromClipboard()
        {
            dataGridView1.PasteFromClipboard();
        }

        /// <summary>
        /// Handles the VisibleChanged event for the panel.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> that contains no event data.</param>
        void FullRegressionPanel_VisibleChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Cycles through a predefined sequence of axis transformations (e.g., Lin-Lin, Log-Lin, Log-Log)
        /// and re-runs the analysis for each combination.
        /// </summary>
        public void CycleTransforms()
        {
            if (Analysis.Data.RowCount > 0)
            {
                try
                {
                    // Detach event handlers to prevent multiple analysis runs
                    _xTransformBox.SelectedIndexChanged -= this.TransformBox_ValueChanged;
                    _yTransformBox.SelectedIndexChanged -= this.TransformBox_ValueChanged;

                    if (_xTransformBox.SelectedTransform == TransformTypeEnum.Linear && _yTransformBox.SelectedTransform == TransformTypeEnum.Linear) //linlin
                    {
                        _xTransformBox.SelectedTransform = TransformTypeEnum.Log; //loglin
                        _yTransformBox.SelectedTransform = TransformTypeEnum.Linear;
                    }
                    else if (_xTransformBox.SelectedTransform == TransformTypeEnum.Log && _yTransformBox.SelectedTransform == TransformTypeEnum.Linear) //loglin
                    {
                        _xTransformBox.SelectedTransform = TransformTypeEnum.Log;
                        _yTransformBox.SelectedTransform = TransformTypeEnum.Log; //loglog
                    }
                    else if (_xTransformBox.SelectedTransform == TransformTypeEnum.Log && _yTransformBox.SelectedTransform == TransformTypeEnum.Log) //loglog
                    {
                        _xTransformBox.SelectedTransform = TransformTypeEnum.Linear; //linlog
                        _yTransformBox.SelectedTransform = TransformTypeEnum.Log;
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
                    else if (_xTransformBox.SelectedTransform == TransformTypeEnum.Log && _yTransformBox.SelectedTransform == TransformTypeEnum.BoxCox) //log boxcox
                    {
                        _xTransformBox.SelectedTransform = TransformTypeEnum.Linear;
                        _yTransformBox.SelectedTransform = TransformTypeEnum.Linear; //linlin
                    }

                    // Manually trigger the logic that was in the event handler
                    TransformBox_ValueChanged(this, EventArgs.Empty);
                }
                catch (Exception exp)
                {
                    MessageBox.Show($"CycleTransforms: {exp.Message}");
                }
                finally
                {
                    // Re-attach event handlers
                    _xTransformBox.SelectedIndexChanged += this.TransformBox_ValueChanged;
                    _yTransformBox.SelectedIndexChanged += this.TransformBox_ValueChanged;
                }
            }
        }

        /// <summary>
        /// Refreshes the panel's values and controls based on the current state of the Analysis object.
        /// This method is called to initialize the UI when data is first loaded or significantly changed.
        /// </summary>
        public override void RefreshValues()
        {
            //Debug.WriteLine("[DEBUG-QRP] RefreshValues() START");

            //SuspendDrawing();
            Analysis.Data.FilterTransformedDataByRanges = false;
            Analysis.CalculateInitialValuesWithNewData();

            // This ensures that if the analysis is already using a Box-Cox transform when loaded,
            // the necessary parameters are calculated and the DisplayTransformer is initialized correctly.
            if (Analysis.InResponseTransform == TransformTypeEnum.BoxCox)
            {
                Analysis.CalculateAndSetBoxCoxParameters();
            }

            SetupSideCharts();

            if (MainChart != null)
            {
                AddChartListeningForErrors();

                MainChart.LoadChartData(Analysis.Data, Analysis.Transformer);
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
            if (dataGridView1.Rows.Count == 1)
            {
                Analysis.Data.ForceRefillSortListAndClearPoints();
                if (Analysis.UsingInitialGuesses)
                {
                    Analysis.ClearInitialGuesses();
                    Analysis.UserSuppliedRanges = true;

                    _form = new NewResponseRangeForm();
                    _form.ShowDialog();

                    Analysis.InResponseMin = _form.MinResponseValue;
                    Analysis.InResponseMax = _form.MaxResponseValue;
                    Analysis.InResponseDecision = (_form.MaxResponseValue - _form.MinResponseValue) * .40 + _form.MinResponseValue;
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
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                //col.Width = 100;
                col.FillWeight = 1.0F / dataGridView1.Columns.Count;

                if (col.Index == dataGridView1.Columns.Count - 1)
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView1.GridCheckForProblems();

            // FIX: Removed the redundant call to UpdateAnalysisFromTable() which was being called twice here.
            UpdateAnalysisFromTable();
            //Debug.WriteLine("[DEBUG-QRP] RefreshValues() END");
        }

        /// <summary>
        /// Updates the analysis object with data from the grid, recalculates initial parameters,
        /// and triggers a new analysis run.
        /// </summary>
        public void UpdateAnalysisFromTable()
        {
            //Debug.WriteLine("[DEBUG-QRP] UpdateAnalysisFromTable() START");
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

                Analysis.Data.ForceRefillSortListAndClearPoints();

                if (dataGridView1.RowCount > 1)
                {
                    Analysis.Data.DataType = AnalysisDataTypeEnum.AHat;
                    Analysis.Data.RecreateTables();

                    // Initialize ranges from the fresh grid data
                    Analysis.ForceUpdateInputsFromData(true, AnalysisDataTypeEnum.AHat);

                    if (Analysis.InResponseTransform == TransformTypeEnum.BoxCox)
                        Analysis.CalculateAndSetBoxCoxParameters();

                    MainChart.LoadChartData(Analysis.Data, Analysis.Transformer);
                    MainChart.ForceResizeAnnotations();

                    // FIX: Must synchronize the UI Controls BEFORE running the analysis. 
                    // This ensures the numeric text boxes contain the correctly calculated bounds from ForceUpdateInputsFromData.
                    SyncSideControls();

                    MainChart.BuildColorMap();
                    MainChart.DeterminePointsInThreshold();

                    //Debug.WriteLine("[DEBUG-QRP] UpdateAnalysisFromTable() calling RunAnalysis()");
                    RunAnalysis();
                }
            }
            //Debug.WriteLine("[DEBUG-QRP] UpdateAnalysisFromTable() END");
        }

        /// <summary>
        /// Prepares the main chart for a quick analysis run by picking the best axis ranges safely.
        /// </summary>
        private void PrepBeforeQuickRunAnalysis()
        {
            //Debug.WriteLine("[DEBUG-QRP] PrepBeforeQuickRunAnalysis() START");

            double transYMin = Analysis.TransformValueForYAxis(Analysis.InResponseMin);
            double transYMax = Analysis.TransformValueForYAxis(Analysis.InResponseMax);
            double transXMin = Analysis.TransformValueForXAxis(Analysis.InFlawMin);
            double transXMax = Analysis.TransformValueForXAxis(Analysis.InFlawMax);

            //Debug.WriteLine("[DEBUG-QRP] Raw Boundaries - YMin: {Analysis.InResponseMin}, YMax: {Analysis.InResponseMax}, XMin: {Analysis.InFlawMin}, XMax: {Analysis.InFlawMax}");

            // FIX: Pass double.MaxValue for mathematically invalid log boundaries to prevent chart blowup.
            if (Analysis.InResponseTransform == TransformTypeEnum.Log)
            {
                if (Analysis.InResponseMin <= 0) transYMin = double.MaxValue;
                if (Analysis.InResponseMax <= 0) transYMax = double.MinValue;
            }
            if (Analysis.InFlawTransform == TransformTypeEnum.Log)
            {
                if (Analysis.InFlawMin <= 0) transXMin = double.MaxValue;
                if (Analysis.InFlawMax <= 0) transXMax = double.MinValue;
            }

            //Debug.WriteLine("[DEBUG-QRP] Calling MainChart.PickBestAxisRange with -> YMin: {transYMin}, YMax: {transYMax}");

            // Let the chart safely compute limits. 
            MainChart.PickBestAxisRange(transYMin, transYMax, transXMin, transXMax);

            //Debug.WriteLine("[DEBUG-QRP] PrepBeforeQuickRunAnalysis() END");
        }

        /// <summary>
        /// Sets up the alignment for all labels on the panel.
        /// </summary>
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

            foreach (Label label in labels)
            {
                label.TextAlign = ContentAlignment.MiddleRight;
            }
        }

        /// <summary>
        /// Synchronizes the input controls on the side panel with the current values in the Analysis object.
        /// </summary>
        protected override void SyncSideControls()
        {
            //Debug.WriteLine("[DEBUG-QRP] SyncSideControls() START");
            Analysis.IsFrozen = true;
            mainChart.DrawBoundaryLines();

            // FIX: Temporarily unhook value changed events so syncing doesn't trigger rogue RunAnalysis calls
            aMaxControl.NumericUpDown.ValueChanged -= this.aMaxControl_ValueChanged;
            aMinControl.NumericUpDown.ValueChanged -= this.aMinControl_ValueChanged;
            leftCensorControl.NumericUpDown.ValueChanged -= this.leftCensorControl_ValueChanged;
            rightCensorControl.NumericUpDown.ValueChanged -= this.rightControl_ValueChanged;
            thresholdControl.NumericUpDown.ValueChanged -= this.thresholdControl_ValueChanged;

            _xTransformBox.SelectedTransform = Analysis.InFlawTransform;
            _yTransformBox.SelectedTransform = Analysis.InResponseTransform;
            if (Analysis.Data.LambdaValue != 0)
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

            // REHOOK
            aMaxControl.NumericUpDown.ValueChanged += this.aMaxControl_ValueChanged;
            aMinControl.NumericUpDown.ValueChanged += this.aMinControl_ValueChanged;
            leftCensorControl.NumericUpDown.ValueChanged += this.leftCensorControl_ValueChanged;
            rightCensorControl.NumericUpDown.ValueChanged += this.rightControl_ValueChanged;
            thresholdControl.NumericUpDown.ValueChanged += this.thresholdControl_ValueChanged;

            Analysis.IsFrozen = false;
            //Debug.WriteLine("[DEBUG-QRP] SyncSideControls() END");
        }

        /// <summary>
        /// Handles the Load event of the panel.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> that contains no event data.</param>
        void FullRegressionPanel_Load(object sender, EventArgs e)
        {
            if (Visible == true)
            {

            }
        }

        /// <summary>
        /// Adds and configures the output controls (read-only numeric boxes for results) to the output panel.
        /// </summary>
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

        /// <summary>
        /// Adds and configures the input controls (transform selectors, numeric up/downs for boundaries) to the input panel.
        /// </summary>
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

        /// <summary>
        /// Initializes the transform selection boxes for the flaw (X) and response (Y) axes.
        /// </summary>
        private void IntitalizeTransformBoxes()
        {
            PrepareLabelBoxPair(ref _xTransformLabel, "Flaw", ref _xTransformBox);
            PrepareLabelBoxPairYHat(ref _yTransformLabel, "Response", ref _yTransformBox);

            _xTransformBox.SelectedIndex = 0;
            _yTransformBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Initializes the numeric input control for the Box-Cox lambda value.
        /// </summary>
        private void InitializeNumericLambda()
        {
            PrepareLabelNumericPair(ref _labelForLamdaInput, "Lambda Value", ref _boxCoxLambdaQuick);
            _boxCoxLambdaQuick.Enabled = false;
        }
        /// <summary>
        /// Sets up the event handlers for all numeric input controls on the panel.
        /// </summary>
        protected override void SetupNumericControlEvents()
        {
            aMaxControl.NumericUpDown.ValueChanged -= this.aMaxControl_ValueChanged;
            aMinControl.NumericUpDown.ValueChanged -= this.aMinControl_ValueChanged;
            leftCensorControl.NumericUpDown.ValueChanged -= this.leftCensorControl_ValueChanged;
            rightCensorControl.NumericUpDown.ValueChanged -= this.rightControl_ValueChanged;
            thresholdControl.NumericUpDown.ValueChanged -= this.thresholdControl_ValueChanged;
            _xTransformBox.SelectedIndexChanged -= this.TransformBox_ValueChanged;
            _yTransformBox.SelectedIndexChanged -= this.TransformBox_ValueChanged;
            _boxCoxLambdaQuick.ValueChanged -= this.BoxCoxLambdaBox_ValueChanged;

            aMaxControl.NumericUpDown.ValueChanged += this.aMaxControl_ValueChanged;
            aMinControl.NumericUpDown.ValueChanged += this.aMinControl_ValueChanged;
            leftCensorControl.NumericUpDown.ValueChanged += this.leftCensorControl_ValueChanged;
            rightCensorControl.NumericUpDown.ValueChanged += this.rightControl_ValueChanged;
            thresholdControl.NumericUpDown.ValueChanged += this.thresholdControl_ValueChanged;
            _xTransformBox.SelectedIndexChanged += this.TransformBox_ValueChanged;
            _yTransformBox.SelectedIndexChanged += this.TransformBox_ValueChanged;
            _boxCoxLambdaQuick.ValueChanged += this.BoxCoxLambdaBox_ValueChanged;
        }

        /// <summary>
        /// Checks if a double value is valid (i.e., not NaN or Infinity).
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns><c>true</c> if the value is valid; otherwise, <c>false</c>.</returns>
        private bool IsValidDouble(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }

        /// <summary>
        /// Handles the event when the user changes the selected transform for the X (Flaw) or Y (Response) axis.
        /// This asynchronous method orchestrates the update of the analysis and UI in response to a transform change.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> that contains no event data.</param>
        /// <remarks>
        /// The method performs the following key steps:
        /// 1.  Updates the `Analysis` object with the new transform types from the UI's transform boxes.
        /// 2.  Handles special UI logic for the Box-Cox transform, enabling/disabling the Lambda input control
        ///     and calculating the initial lambda value. It temporarily detaches the lambda value-changed event
        ///     handler to prevent a recursive analysis trigger.
        /// 3.  Suspends chart drawing, recalculates and sets the new positions of all boundary lines (min/max flaw,
        ///     min/max response, threshold) on the main chart based on the new transformations. It correctly handles
        ///     both normal and inverse transforms.
        /// 4.  Introduces a small delay to ensure the UI has processed the preceding events.
        /// 5.  Calls `UpdateAnalysisFromTable()` to trigger a full recalculation and refresh of the analysis and charts.
        /// </remarks>
        private async void TransformBox_ValueChanged(object sender, EventArgs e)
        {
            //Debug.WriteLine("[DEBUG-QRP] TransformBox_ValueChanged START");
            mainChart.SuspendDrawing();

            try
            {
                Analysis.InFlawTransform = _xTransformBox.SelectedTransform;
                Analysis.InResponseTransform = _yTransformBox.SelectedTransform;

                MainChart.UpdateTransformer(Analysis.Transformer);
            }
            finally
            {
                mainChart.ResumeDrawing();
            }

            //used to prevent the lambda numeric up down event from firing when the user selects boxcox
            if (_yTransformBox.SelectedTransform == TransformTypeEnum.BoxCox)
            {
                _boxCoxLambdaQuick.ValueChanged -= this.BoxCoxLambdaBox_ValueChanged;
            }
            //get temporary lambda if box-cox is selected
            if (Analysis.InResponseTransform == TransformTypeEnum.BoxCox)
            {
                Analysis.CalculateAndSetBoxCoxParameters();
                double lambdaTemp = UndoLambda501(Analysis.InLambdaValue);
                _boxCoxLambdaQuick.Value = Convert.ToDecimal(lambdaTemp);

                _labelForLamdaInput.Enabled = true;
                _boxCoxLambdaQuick.Enabled = true;
            }
            else
            {
                _labelForLamdaInput.Enabled = false;
                _boxCoxLambdaQuick.Enabled = false;
            }

            mainChart.SuspendDrawing(); // Suspend again for boundary updates
            try
            {
                if (Analysis.InFlawTransform != TransformTypeEnum.Inverse)
                {
                    var x = Convert.ToDouble(Analysis.TransformValueForXAxis(aMaxControl.Value));
                    if (IsValidDouble(x))
                        mainChart.SetAMaxBoundary(x, false);

                    x = Convert.ToDouble(Analysis.TransformValueForXAxis(aMinControl.Value));
                    if (IsValidDouble(x))
                        mainChart.SetAMinBoundary(x, false);
                }
                else
                {
                    var x = Convert.ToDouble(Analysis.TransformValueForXAxis(aMaxControl.Value));
                    if (IsValidDouble(x))
                        mainChart.SetAMinBoundary(x, false);

                    x = Convert.ToDouble(Analysis.TransformValueForXAxis(aMinControl.Value));
                    if (IsValidDouble(x))
                        mainChart.SetAMaxBoundary(x, false);
                }

                if (Analysis.InResponseTransform != TransformTypeEnum.Inverse)
                {
                    var y = Convert.ToDouble(Analysis.TransformValueForYAxis(leftCensorControl.Value));
                    if (IsValidDouble(y))
                        mainChart.SetLeftCensorBoundary(y, false);

                    y = Convert.ToDouble(Analysis.TransformValueForYAxis(rightCensorControl.Value));
                    if (IsValidDouble(y))
                        mainChart.SetRightCensorBoundary(y, false);

                    y = Convert.ToDouble(Analysis.TransformValueForYAxis(thresholdControl.Value));
                    if (IsValidDouble(y))
                        mainChart.SetThresholdBoundary(y, false);
                }
                else
                {
                    var y = Convert.ToDouble(Analysis.TransformValueForYAxis(leftCensorControl.Value));
                    if (IsValidDouble(y))
                        mainChart.SetRightCensorBoundary(y, false);

                    y = Convert.ToDouble(Analysis.TransformValueForYAxis(rightCensorControl.Value));
                    if (IsValidDouble(y))
                        mainChart.SetLeftCensorBoundary(y, false);

                    y = Convert.ToDouble(Analysis.TransformValueForYAxis(thresholdControl.Value));
                    if (IsValidDouble(y))
                        mainChart.SetThresholdBoundary(y, false);
                }
            }
            finally
            {
                mainChart.ResumeDrawing();
            }

            ValidateInputControls();

            // A small delay to allow the UI to process events before running analysis.
            //await Task.Delay(10);
            UpdateAnalysisFromTable();

            //Debug.WriteLine("[DEBUG-QRP] TransformBox_ValueChanged END");

            if (_yTransformBox.SelectedTransform == TransformTypeEnum.BoxCox)
            {
                _boxCoxLambdaQuick.ValueChanged += this.BoxCoxLambdaBox_ValueChanged;
            }
        }

        // FIX: Clamps visual boundary without altering backend data to match FullRegression behavior.
        /// <summary>
        /// Validates the input controls, ensuring that boundaries are moved to valid positions if 
        /// they fall into mathematically undefined regions (e.g., negative values for Log transforms).
        /// </summary>
        protected override void ValidateInputControls()
        {
            //Debug.WriteLine("[DEBUG-QRP] ValidateInputControls() START. ControlValue={leftCensorControl.Value}");
            if (Analysis.InResponseTransform == TransformTypeEnum.Log)
            {
                if (leftCensorControl.Value < 1.0m)
                {
                    //Debug.WriteLine("[DEBUG-QRP] ValidateInputControls() SNAPBACK to {_lastValidNegativeLeftCensor}");
                    mainChart.SetLeftCensorBoundary(Analysis.TransformValueForYAxis((double)_lastValidNegativeLeftCensor), true);
                }
            }
            //Debug.WriteLine("[DEBUG-QRP] ValidateInputControls() END");
        }

        /// <summary>
        /// Sets the precision (number of decimal places) for all numeric input and output controls.
        /// </summary>
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

        /// <summary>
        /// Deletes the currently selected row from the data grid.
        /// </summary>
        public void DeleteRow()
        {
            dataGridView1.DeleteSelectedRow();
        }

        /// <summary>
        /// Adds a new row to the data grid.
        /// </summary>
        public void AddRow()
        {
            dataGridView1.AddRow();
        }

        /// <summary>
        /// Adds the side charts (Linearity, POD, Threshold) to the panel's side chart collection.
        /// </summary>
        public override void AddSideCharts()
        {
            SideCharts.Add(linearityChart);
            //SideCharts.Add(normalityChart);
            //SideCharts.Add(equalVarianceChart);
            SideCharts.Add(podChart);
            SideCharts.Add(thresholdChart);
        }

        /// <summary>
        /// Colors the numeric controls. This method is currently empty.
        /// </summary>
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

        /// <summary>
        /// Sets up the input parameters for the analysis based on the current state of the UI controls.
        /// </summary>
        public override void SetupAnalysisInput()
        {
            //Debug.WriteLine("[DEBUG-QRP] SetupAnalysisInput() START");
            double yMin = mainChart.ChartAreas[0].AxisY.Minimum;

            // FIX: Removed call to Analysis.ForceUpdateInputsFromData. This method must strictly
            // MOVE UI values into backend variables, not tell the backend to recalculate them.

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
            //Debug.WriteLine("[DEBUG-QRP] SetupAnalysisInput() END");
        }

        /// <summary>
        /// Processes the output from a completed analysis run, updating all output controls and side charts with the new results.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> that contains no event data.</param>
        public override void ProcessAnalysisOutput(Object sender, EventArgs e)
        {
            //Debug.WriteLine("[DEBUG-QRP] ProcessAnalysisOutput() START");
            if (MainChart != null)
                MainChart.ClearProgressBar();

            EnableInputControls();

            if (Analysis.Data.MissingData)
            {
                return;
            }

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
            if (a50Original < 0.0)
            {
                a50Original = 0.0;
                showA50Error = true;

            }
            if (a90Original < 0.0)
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
            if (showA50Error)
                Source.Python.AddErrorText("Calculated a50 is less than zero. Consider adjusting your threshold.");
            else if (showA90Error)
                Source.Python.AddErrorText("Calculated a90 is less than zero. Consider adjusting your threshold.");
            else if (showA9095Error)
                Source.Python.AddErrorText("Calculated a90/95 is less than zero. Consider adjusting your threshold.");

            string controlName = "";

            try
            {

                double tempA90 = Convert.ToDouble(Analysis.TransformValueForXAxis(Convert.ToDecimal(a90Transformed)));
                if (IsValidDouble(tempA90)) a90Transformed = tempA90;

                double tempA9095 = Convert.ToDouble(Analysis.TransformValueForXAxis(Convert.ToDecimal(a9095Transformed)));
                if (IsValidDouble(tempA9095)) a9095Transformed = tempA9095;

                double tempA50 = Convert.ToDouble(Analysis.TransformValueForXAxis(Convert.ToDecimal(a50Transformed)));
                if (IsValidDouble(tempA50)) a50Transformed = tempA50;


                controlName = GetOutputControlLabelText(a90Out);
                a90Out.Value = Convert.ToDecimal(a90Original);

                controlName = GetOutputControlLabelText(a90_95Out);
                a90_95Out.Value = Convert.ToDecimal(a9095Original);

                controlName = GetOutputControlLabelText(a50Out);
                a50Out.Value = Convert.ToDecimal(a50Original);

                responseMinTransformed = Convert.ToDouble(Analysis.TransformValueForYAxis(Convert.ToDecimal(responseMinTransformed)));
                responseMaxTransformed = Convert.ToDouble(Analysis.TransformValueForYAxis(Convert.ToDecimal(responseMaxTransformed)));

                controlName = GetOutputControlLabelText(modelMOut);
                modelMOut.Value = Convert.ToDecimal(modelM);

                controlName = GetOutputControlLabelText(modelBOut);
                modelBOut.Value = Convert.ToDecimal(modelB);

                controlName = GetOutputControlLabelText(modelErrorOut);
                modelErrorOut.Value = Convert.ToDecimal(modelError);

                controlName = GetOutputControlLabelText(modelMStdErrOut);
                modelMStdErrOut.Value = Convert.ToDecimal(modelMStdError);

                controlName = GetOutputControlLabelText(modelBStdErrOut);
                modelBStdErrOut.Value = Convert.ToDecimal(modelBStdError);

                controlName = GetOutputControlLabelText(modelErrorStdErrOut);
                modelErrorStdErrOut.Value = Convert.ToDecimal(modelErrorStdError);

                controlName = GetOutputControlLabelText(repeatabilityErrorOut);
                repeatabilityErrorOut.Value = Convert.ToDecimal(repeatabilityError);

                controlName = GetOutputControlLabelText(SigmaOut);
                SigmaOut.Value = Convert.ToDecimal(sigma);

                controlName = GetOutputControlLabelText(MuOut);
                MuOut.Value = Convert.ToDecimal(mu);

                normalityTestOut.Value = Convert.ToDecimal(normality);
                lackOfFitTestOut.Value = Convert.ToDecimal(lackOfFit);
                equalVarianceTestOut.Value = Convert.ToDecimal(equalVariance);

            }
            catch (Exception exp)
            {
                POD.Globals.ShowErrorMessageBox(exp);
                //MessageBox.Show("Analysis Error caused invalid output values that are out of range.");
                Source.Python.AddErrorText($"{controlName} value is out of range.");

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


            if (IsValidDouble(a50Transformed) && IsValidDouble(a90Transformed) && IsValidDouble(a9095Transformed))
            {
                mainChart.UpdateLevelConfidenceLines(a50Transformed,
                                                     a90Transformed,
                                                     a9095Transformed,
                                                     Analysis.OutModelSlope, Analysis.OutModelIntercept);
            }

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
            //Debug.WriteLine("[DEBUG-QRP] ProcessAnalysisOutput() END");
        }

        /// <summary>
        /// Gets the text from the label associated with a given output control.
        /// </summary>
        /// <param name="ctrl">The control to find the label for.</param>
        /// <returns>The text of the associated label.</returns>
        private string GetOutputControlLabelText(Control ctrl)
        {
            return GetControlLabelText(ctrl, _outputWithLabels);
        }

        /// <summary>
        /// Gets the text from the label associated with a given input control.
        /// </summary>
        /// <param name="ctrl">The control to find the label for.</param>
        /// <returns>The text of the associated label.</returns>
        private string GetInputControlLabelText(Control ctrl)
        {
            return GetControlLabelText(ctrl, _inputWithLabels);
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

        /// <summary>
        /// Overrides the base RunAnalysis method to set the analysis type to QuickAnalysis
        /// and then proceeds with the analysis run.
        /// </summary>
        /// <param name="disableControls">If set to <c>true</c>, input controls are disabled during the analysis.</param>
        public override void RunAnalysis(bool disableControls = true)
        {
            //Debug.WriteLine($"[DEBUG-QRP] RunAnalysis() START. DisableControls={disableControls}");
            //ResumeDrawing();

            Analysis.UsingQuickAnalysis = true;

            if (dataGridView1.CheckForValidDataGrid(mainChart, Analysis.Name))
            {
                var analysis = (Analysis)_source;

                analysis.AnalysisDone -= ProcessAnalysisOutput;
                analysis.AnalysisDone -= ProcessAnalysisOutput;
                analysis.AnalysisDone -= ProcessAnalysisOutput;
                analysis.AnalysisDone += ProcessAnalysisOutput;

                if (MainChart != null)
                {
                    // FIX: Populate Analysis from the UI Controls BEFORE telling chart to scale.
                    //Debug.WriteLine("[DEBUG-QRP] RunAnalysis() calling SetupAnalysisInput()");
                    SetupAnalysisInput();

                    //Debug.WriteLine("[DEBUG-QRP] RunAnalysis() calling PrepBeforeQuickRunAnalysis()");
                    PrepBeforeQuickRunAnalysis();

                    if (disableControls)
                        DisableInputControls();

                    MainChart.PrepareForRunAnalysis();
                    MainChart.ResetErrors();
                    MainChart.ClearProgressBar();
                }

                this.Cursor = Cursors.WaitCursor;

                Analysis.RunAnalysis();

                this.Cursor = Cursors.Default;
            }



            //ProcessAnalysisOutput(Analysis, null);

            //RefreshValues();
            //Debug.WriteLine("[DEBUG-QRP] RunAnalysis() END");
        }

        /// <summary>
        /// Handles the ValueChanged event for the 'aMax' control. Updates the chart boundary and reruns the analysis.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> that contains no event data.</param>
        private void aMaxControl_ValueChanged(object sender, EventArgs e)
        {
            //controlValueChanged = true;

            var x = Convert.ToDouble(Analysis.TransformValueForXAxis(aMaxControl.Value));
            mainChart.SetAMaxBoundary(x, true);

            RunAnalysis();

            //controlValueChanged = false;
        }

        /// <summary>
        /// Handles the ValueChanged event for the 'aMin' control. Updates the chart boundary and reruns the analysis.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> that contains no event data.</param>
        private void aMinControl_ValueChanged(object sender, EventArgs e)
        {
            //controlValueChanged = true;

            var x = Convert.ToDouble(Analysis.TransformValueForXAxis(aMinControl.Value));
            mainChart.SetAMinBoundary(x, true);

            RunAnalysis();

            //controlValueChanged = false;
        }

        /// <summary>
        /// Handles the ValueChanged event for the 'leftCensor' control. Updates the chart boundary and reruns the analysis.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> that contains no event data.</param>
        private void leftCensorControl_ValueChanged(object sender, EventArgs e)
        {
            //Debug.WriteLine($"[DEBUG-QRP] leftCensorControl_ValueChanged() START. Value: {leftCensorControl.Value}");
            //controlValueChanged = true;

            if (leftCensorControl.Value < 0.0m)
            {
                _lastValidNegativeLeftCensor = leftCensorControl.Value;
                //Debug.WriteLine($"[DEBUG-QRP] _lastValidNegativeLeftCensor recorded as: {_lastValidNegativeLeftCensor}");
                ValidateInputControls();
            }

            var y = Convert.ToDouble(Analysis.TransformValueForYAxis(leftCensorControl.Value));
            mainChart.SetLeftCensorBoundary(y, true);

            Analysis.AnalysisCalculationType = RCalculationType.Full;
            RunAnalysis();

            ValidateInputControls();

            //controlValueChanged = false;
            //Debug.WriteLine("[DEBUG-QRP] leftCensorControl_ValueChanged() END");
        }

        /// <summary>
        /// Handles the ValueChanged event for the 'rightCensor' control. Updates the chart boundary and reruns the analysis.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> that contains no event data.</param>
        private void rightControl_ValueChanged(object sender, EventArgs e)
        {
            //controlValueChanged = true;

            var y = Convert.ToDouble(Analysis.TransformValueForYAxis(rightCensorControl.Value));
            mainChart.SetRightCensorBoundary(y, true);

            RunAnalysis();

            //controlValueChanged = false;
        }

        /// <summary>
        /// Handles the ValueChanged event for the 'threshold' control. Updates the chart boundary and reruns the analysis with a threshold change calculation type.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> that contains no event data.</param>
        private void thresholdControl_ValueChanged(object sender, EventArgs e)
        {
            if (thresholdControl.Value < 0 && _yTransformBox.SelectedIndex == 1)
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

        /// <summary>
        /// Handles the ValueChanged event for the Box-Cox lambda numeric up/down control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> that contains no event data.</param>
        private void BoxCoxLambdaBox_ValueChanged(object sender, EventArgs e)
        {
            Analysis.AnalysisCalculationType = RCalculationType.Full;
            if (_boxCoxLambdaQuick.Value == 0.0m)
            {
                MessageBox.Show("Setting lambda as 0 is the same as taking a log transform of y! If lambda is already close to 0, use log transform instead.");
                _boxCoxLambdaQuick.Value = _previousLambda;
                return;
            }
            double customLambda = Convert.ToDouble(_boxCoxLambdaQuick.Value);
            Analysis.UpdateBoxCoxLambda(customLambda);

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
                var y = Convert.ToDouble(Analysis.TransformValueForYAxis(leftCensorControl.Value));
                mainChart.SetLeftCensorBoundary(y, false);
                y = Convert.ToDouble(Analysis.TransformValueForYAxis(rightCensorControl.Value));
                mainChart.SetRightCensorBoundary(y, false);
                y = Convert.ToDouble(Analysis.TransformValueForYAxis(thresholdControl.Value));
                mainChart.SetThresholdBoundary(y, false);
            }

            ForceUpdateAfterTransformChange();
            _previousLambda = _boxCoxLambdaQuick.Value;
        }

        /// <summary>
        /// Handles the LinesChanged event from the main chart, which occurs when a user drags a boundary line.
        /// This method updates the corresponding numeric input controls with the new, inverse-transformed values.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> that contains no event data.</param>
        protected override void MainChart_LinesChanged(object sender, EventArgs e)
        {
            // Temporarily detach events to prevent infinite update loops during floating point line drags
            aMaxControl.NumericUpDown.ValueChanged -= this.aMaxControl_ValueChanged;
            aMinControl.NumericUpDown.ValueChanged -= this.aMinControl_ValueChanged;
            leftCensorControl.NumericUpDown.ValueChanged -= this.leftCensorControl_ValueChanged;
            rightCensorControl.NumericUpDown.ValueChanged -= this.rightControl_ValueChanged;
            thresholdControl.NumericUpDown.ValueChanged -= this.thresholdControl_ValueChanged;

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

                if (this.leftCensorControl.Value < 0.0m)
                {
                    this._lastValidNegativeLeftCensor = leftCensorControl.Value;
                }
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

            aMaxControl.NumericUpDown.ValueChanged += this.aMaxControl_ValueChanged;
            aMinControl.NumericUpDown.ValueChanged += this.aMinControl_ValueChanged;
            leftCensorControl.NumericUpDown.ValueChanged += this.leftCensorControl_ValueChanged;
            rightCensorControl.NumericUpDown.ValueChanged += this.rightControl_ValueChanged;
            thresholdControl.NumericUpDown.ValueChanged += this.thresholdControl_ValueChanged;
        }
    }
}