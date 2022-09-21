using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using POD.Controls;
using POD.Analyze;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;
using System.ComponentModel;
using System.Diagnostics;

namespace POD.Wizards.Steps.AHatVsANormalSteps
{
    using System.Data;
    using System.Windows.Forms.DataVisualization.Charting;
    //using CSharpBackendWithR;
    using POD.Analyze;
    using POD.Data;

    public partial class ChooseTransformPanel : WizardPanel
    {
        private List<Control> _inputWithLabels = new List<Control>();
        private List<Control> _outputWithLabels = new List<Control>();
        private Analyze.Analysis _analysis;
        private BackgroundWorker analysisLauncher;
        List<Color> _colors = new List<Color>();
        List<TransformChart> _charts = new List<TransformChart>();
        DataTable _backupPOD = null;
        DataTable _backupUncensored = null;
        DataTable _backupPartial = null;
        double _a50Backup = 0.0;
        double _a9095Backup = 0.0;
        double _a90Backup = 0.0;

        public ChooseTransformPanel(PODToolTip tooltip)
            : base(tooltip)
        {
            InitializeComponent();

            StepToolTip = new PODToolTip();

            Load += ChooseTransformPanel_Load;

            _colors = DataPointChart.GetLargeColorList(this.DesignMode);

            if (!DesignMode)
            {
                var temp = _colors[1];
                _colors[1] = _colors[2];
                _colors[2] = temp;
            }

            Paint += ChooseTransformPanel_Paint;

            graphLayoutPanel.Resize += graphLayoutPanel_Resize;

            SetupNumericControlPrecision();


        }

        void graphLayoutPanel_Resize(object sender, EventArgs e)
        {
            graphLayoutPanel.SuspendLayout();

            ResizeChartsBasedOnCount();

            graphLayoutPanel.ResumeLayout();
        }

        void ChooseTransformPanel_Paint(object sender, PaintEventArgs e)
        {
            //if(analysisLauncher == null)
            //    RunAnalyses();
        }

        private void SetupAnalysisLauncher()
        {
            analysisLauncher = new BackgroundWorker();

            analysisLauncher.WorkerSupportsCancellation = true;
            analysisLauncher.WorkerReportsProgress = true;

            analysisLauncher.DoWork += new DoWorkEventHandler(Background_StartAnalysis);
            analysisLauncher.ProgressChanged += new ProgressChangedEventHandler(Background_AnalysisProgressChanged);
            analysisLauncher.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Background_FinishedAnalysis);
        }

        private void Background_FinishedAnalysis(object sender, RunWorkerCompletedEventArgs e)
        {
            if (analysisLauncher != null)
            {
                analysisLauncher.Dispose();
                analysisLauncher = null;
            }

            SelectChartBasedOnAnalysis();

            AnalysisData.DuplicateTable(_backupPOD, _analysis.Data.PodCurveTable);
            AnalysisData.DuplicateTable(_backupUncensored, _analysis.Data.ResidualUncensoredTable);
            AnalysisData.DuplicateTable(_backupPartial, _analysis.Data.ResidualPartialCensoredTable);
            _analysis.RestoreBackup(_a50Backup, _a90Backup, _a9095Backup);

            _updatingTransformResults = false;

            _analysis.LockBusy = false;

            
        }

        private void Background_AnalysisProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            
        }

        private void ProcessAnalysisOutput(object sender, EventArgs e)
        {
            
        }

        private void Background_StartAnalysis(object sender, DoWorkEventArgs e)
        {
            Stopwatch watch = new Stopwatch();
                        
            var xAxisIndex = 0;
            var yAxisIndex = 0;

            var sortedSelectedY = GetSortedSelectedIndicies(YAxisTransformList);
            var sortedSelectedX = GetSortedSelectedIndicies(XAxisTransformList);

            var xRowCount = XAxisTransformList.Rows.Count;

            var initialX = _analysis.InFlawTransform;
            var initialY = _analysis.InResponseTransform;

            _topX = sortedSelectedX.Max() + 1;
            _topY = sortedSelectedY.Max() + 1;

            //var fixList = new List<FixPoint>();
            //_analysis.Data.UpdateIncludedPointsBasedFlawRange(_analysis.InFlawMax, _analysis.InFlawMin, fixList);

            _backupPOD = new DataTable();
            _backupUncensored = new DataTable();
            _backupPartial = new DataTable();
            AnalysisData.DuplicateTable(_analysis.Data.PodCurveTable, _backupPOD);
            AnalysisData.DuplicateTable(_analysis.Data.ResidualUncensoredTable, _backupUncensored);
            AnalysisData.DuplicateTable(_analysis.Data.ResidualPartialCensoredTable, _backupPartial);
            
            _a50Backup = _analysis.OutResponseDecisionPODA50Value;
            _a90Backup = _analysis.OutResponseDecisionPODLevelValue;
            _a9095Backup = _analysis.OutResponseDecisionPODConfidenceValue;

            _analysis.LockBusy = true;

            foreach (var xTrans in AllXTransforms)
            {
                yAxisIndex = 0;

                this.Invoke((MethodInvoker)delegate()
                {
                    Parent.Cursor = Cursors.WaitCursor;
                });

                foreach (var yTrans in AllYTransforms)
                {
                    ///while (_analysis.AnalysisRunning) ;                   
                    if (yAxisIndex < _topY && xAxisIndex < _topX)
                    {
                        //_analysis.Data.FilterTransformedDataByRanges = true;
                        _analysis.InFlawTransform = xTrans.TransformType;
                        _analysis.InResponseTransform = yTrans.TransformType;

                        if (_analysis.InResponseTransform == TransformTypeEnum.BoxCox)
                        {
                            _analysis.SetUpLambda();
                        }
                        //assign the analysis as transform only
                        Analysis.AnalysisCalculationType = RCalculationType.Transform;
                        var yColumnIndex = yAxisIndex;
                        var xColumnIndex = xAxisIndex;

                        try
                        {
                            _analysis.RunOnlyFitAnalysis();
                        }
                        catch(Exception exp)
                        {
                            MessageBox.Show("RunOnlyFitAnalysis: " + exp.Message);
                        }

                        this.Invoke((MethodInvoker)delegate()
                        {
                            var chart = _charts[yAxisIndex * xRowCount + xAxisIndex];

                            //watch.Restart();
                            

                            //watch.Stop();
                            //MessageBox.Show(xTrans.ConfIntervalType.ToString() + " " + yTrans.ConfIntervalType.ToString() + " " + watch.ElapsedMilliseconds + "ms");
                            chart.FillFromAnalysis(_analysis.Data, _colors, xColumnIndex, yColumnIndex);
                            UpdateChartsWithCurrentView(chart);
                            chart.Update();
                        });
                    }


                    yAxisIndex++;
                }

                xAxisIndex++;
            }
            
            //var vaariable=1;

            this.Invoke((MethodInvoker)delegate()
            {
                Parent.Cursor = Cursors.Default;
            });

            _analysis.InFlawTransform = initialX;
            _analysis.InResponseTransform = initialY;

            this.Invoke((MethodInvoker)delegate()
            {
                Refresh();
            });
        }

        private int _rowIndex;
        private int _topX;
        private int _topY;

        internal override void PrepareGUI()
        {
            base.PrepareGUI();


        }

        public override void RefreshValues()
        {
            if(_source != null)
            {
                SelectChartBasedOnAnalysis();

                var yAxisIndex = 0;
                var xAxisIndex = 0;
                var xRowCount = XAxisTransformList.Rows.Count;

                foreach (var xTrans in AllXTransforms)
                {
                    yAxisIndex = 0;

                    foreach (var yTrans in AllYTransforms)
                    {
                        var yColumnIndex = yAxisIndex;
                        var xColumnIndex = xAxisIndex;

                        var chart = _charts[yAxisIndex * xRowCount + xAxisIndex];
                        //chart.AddEmptySeriesToForceDraw();
                        yAxisIndex++;
                    }

                    xAxisIndex++;
                }

                try
                {
                    //Analysis clone = _analysis.CreateDuplicate();

                    //_analysis.RaiseCreatedAnalysis(clone);

                    FlawMinNum.Value = Convert.ToDecimal(_analysis.InFlawMin);
                    FlawMaxNum.Value = Convert.ToDecimal(_analysis.InFlawMax);

                    ResponseMinNum.Value = Convert.ToDecimal(_analysis.InResponseMin);
                    ResponseMaxNum.Value = Convert.ToDecimal(_analysis.InResponseMax);

                    _analysis.Data.ForceRefillSortList();
                    var fixPoints = new List<FixPoint>();
                    _analysis.Data.UpdateIncludedPointsBasedFlawRange(_analysis.TransformValueForXAxis(_analysis.InFlawMax), _analysis.TransformValueForXAxis(_analysis.InFlawMin), fixPoints);
                }
                catch
                {

                }

                ResizeChartsBasedOnCount();

                //Refresh();

                RunAnalyses();
                
            }
        }

        public bool ChangeView()
        {
            if (!_updatingTransformResults)
            {
                _viewResiduals = !_viewResiduals;

                UpdateChartsWithCurrentView();
            }

            return _viewResiduals;
        }

        private void UpdateChartsWithCurrentView()
        {
            if (_viewResiduals)
            {
                SwitchToResiduals();
            }
            else
            {
                SwitchToNormal();
            }
        }

        private void UpdateChartsWithCurrentView(TransformChart chart)
        {
            if (_viewResiduals)
            {
                SwitchToResiduals(chart);
            }
            else
            {
                SwitchToNormal(chart);
            }
        }

        private void SwitchToNormal()
        {
            foreach(Control control in graphLayoutPanel.Controls)
            {
                var chart = control as TransformChart;

                if(chart != null)
                {
                    SwitchToNormal(chart);
                }
            }
        }

        private void SwitchToNormal(TransformChart chart)
        {
            chart.SwitchToNormal();

            _analysis.InFlawTransform = chart.XTransform;
            _analysis.InResponseTransform = chart.YTransform;
            _analysis.UpdatePythonTransforms();

            chart.RelabelAxes(_analysis.Data);
        }

        private void SwitchToResiduals()
        {
            foreach (Control control in graphLayoutPanel.Controls)
            {
                var chart = control as TransformChart;

                if (chart != null)
                {
                    SwitchToResiduals(chart);
                }
            }
        }

        private void SwitchToResiduals(TransformChart chart)
        {
            chart.SwitchToResidual();

            _analysis.InFlawTransform = chart.XTransform;
            _analysis.InResponseTransform = chart.YTransform;
            _analysis.UpdatePythonTransforms();

            chart.RelabelAxes(_analysis.Data);
        }

        private void SelectChartBasedOnAnalysis()
        {
            var yTransforms = AllYTransforms;
            var xTransforms = AllXTransforms;

            DeselectAllCharts();

            for(int i = 0; i < yTransforms.Count; i++)
            {
                for(int j = 0; j < xTransforms.Count; j++)
                {
                    var chart = graphLayoutPanel.GetControlFromPosition(j, i) as DataPointChart;

                    if(yTransforms[i].TransformType == _analysis.Data.ResponseTransform && 
                       xTransforms[j].TransformType == _analysis.Data.FlawTransform)
                    {
                        chart.SelectChart();
                    }
                }
            }
        }

        /// <summary>
        /// Sets up properties for right side numeric controls.  Only call after InitializeComponent().
        /// </summary>
        protected override void SetupNumericControlPrecision()
        {
            var list = new List<PODNumericUpDown> { FlawMinNum, FlawMaxNum, ResponseMinNum, ResponseMaxNum };

            foreach (var number in list)
            {
                number.Maximum = decimal.MaxValue;
                number.Minimum = decimal.MinValue;
                number.DecimalPlaces = 3;
            }
        }

        private TransformChart CreateNewChart()
        {
            var chart = new TransformChart();
            
            chart.SetupChart(Analysis.Data.AvailableFlawNames[0], Analysis.Data.AvailableFlawUnits[0],
                             Analysis.Data.AvailableResponseNames, Analysis.Data.AvailableResponseUnits);

            chart.AutoNameYAxis = false;

            chart.HideLegend();
            chart.ShowChartTitle = false;

            chart.Padding = new Padding(0, 0, 0, 0);
            chart.Margin = new Padding(0, 0, 0, 0);

            //chart.IsSquare = true;

            //chart.XAxisTitle = _analysis.Data.ActivatedFlawName;

            //var newYTitle = chart.YAxisTitle = "Combined";

            //if (_analysis.Data.ActivatedResponseNames.Count == 1)
            //    newYTitle = _analysis.Data.ActivatedResponseNames[0];

            //chart.YAxisTitle = newYTitle;
            //chart.OriginalYAxisTitle = newYTitle;

            //chart.XAxisUnit = _analysis.Data.

            chart.Selectable = true;
            chart.CanUnselect = false;

            chart.MouseClick += chart_MouseClick;

            

            return chart;
        }


        void chart_MouseClick(object sender, MouseEventArgs e)
        {
            var chart = sender as DataPointChart;

            if (chart == null)
                return;

            foreach(var control in graphLayoutPanel.Controls)
            {
                var compare = control as DataPointChart;

                if(compare != null && compare != chart && compare.IsSelected)
                {
                    compare.ForceSelectionOff();
                }
            }
        }

        private void ResizeChartsBasedOnCount()
        {
            
            var numberOfCharts = graphLayoutPanel.Controls.Count;

            if (numberOfCharts > 0)
            {
                var sqrRtCount = Convert.ToInt32(Math.Ceiling(Math.Sqrt(numberOfCharts)));
                var xSelected = GetSortedSelectedIndicies(XAxisTransformList);
                var ySelected = GetSortedSelectedIndicies(YAxisTransformList);
                var widthDiv = xSelected.Count;
                var heightDiv = ySelected.Count;

                if(widthDiv == 1 && heightDiv > 1)
                    widthDiv++;
                else if(heightDiv == 1 && widthDiv > 1)
                    heightDiv++;

                var newWidth = graphLayoutPanel.Width / widthDiv;// -graphLayoutPanel.Margin.Left - graphLayoutPanel.Margin.Right;
                var newHeight = graphLayoutPanel.Height / heightDiv;// -graphLayoutPanel.Margin.Top - graphLayoutPanel.Margin.Bottom;

                var column = 0;
                var row = 0;

                foreach (var xTrans in AllXTransforms)
                {
                    column = 0;

                    foreach (var yTrans in AllYTransforms)
                    {
                        TransformChart chart = null;

                        chart = graphLayoutPanel.GetControlFromPosition(column, row) as TransformChart;

                        if (chart != null)
                        {
                            chart.Width = newWidth;
                            chart.Height = newHeight;
                        }

                        column++;
                    }
                    row++;
                }

                for (int i = 0; i < AllXTransforms.Count; i++)
                {
                    if (ySelected.Contains(i))
                    {
                        graphLayoutPanel.RowStyles[i].Height = newHeight;

                        foreach (Control control in graphLayoutPanel.Controls)
                        {
                            if (graphLayoutPanel.GetPositionFromControl(control).Row == i)
                                control.Visible = true;
                        }
                    }
                    else
                    {
                        graphLayoutPanel.RowStyles[i].Height = 0;

                        foreach (Control control in graphLayoutPanel.Controls)
                        {
                            if (graphLayoutPanel.GetPositionFromControl(control).Row == i)
                                control.Visible = false;
                        }
                    }
                }

                for (int i = 0; i < AllYTransforms.Count; i++)
                {
                    if (xSelected.Contains(i))
                    {
                        graphLayoutPanel.ColumnStyles[i].Width = newWidth;

                        foreach (Control control in graphLayoutPanel.Controls)
                        {
                            if (graphLayoutPanel.GetPositionFromControl(control).Column == i)
                                control.Visible = true;
                        }
                    }
                    else
                    {
                        graphLayoutPanel.ColumnStyles[i].Width = 0;

                        foreach (Control control in graphLayoutPanel.Controls)
                        {
                            if (graphLayoutPanel.GetPositionFromControl(control).Column == i)
                                control.Visible = false;
                        }
                    }
                }

                //if (newHeight / newWidth > 1.5)
                //    newHeight = Convert.ToInt32(newWidth * 1.5);
                //else if (newWidth / newHeight > 1.5)
                //    newWidth = Convert.ToInt32(newHeight * 1.5);

                //if (_xTransforms.Count > _yTransforms.Count)
                //    graphLayoutPanel.FlowDirection = FlowDirection.TopDown;
                //else
                //    graphLayoutPanel.FlowDirection = FlowDirection.LeftToRight;
                


                foreach (Control control in graphLayoutPanel.Controls)
                {
                    
                }

                //graphLayoutPanel.Width = widthDiv * _yTransforms.Count;
                //graphLayoutPanel.Height = heightDiv * _yTransforms.Count;
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

                _analysis = _source as Analysis;

                
            }
        }

        public override bool Stuck
        {
            get
            {
                if(analysisLauncher != null)
                {
                    return _updatingTransformResults;
                }
                else
                {
                    return false;
                }
            }
        }

        public void UpdateAnalysis()
        {
            if (analysisLauncher == null || !analysisLauncher.IsBusy)
            {
                if (analysisLauncher != null)
                {
                    analysisLauncher.Dispose();
                    analysisLauncher = null;
                }

                ClearAllChartEvents();

                _analysis.AnalysisDone -= ProcessAnalysisOutput;
                _analysis.AnalysisDone -= ProcessAnalysisOutput;
                _analysis.AnalysisDone -= ProcessAnalysisOutput;

                _analysis.InFlawTransform = SelectedXAxisTransform;
                _analysis.InResponseTransform = SelectedYAxisTransform;

            }
        }

        private void ClearAllChartEvents()
        {
            
        }
        
        void ChooseTransformPanel_Load(object sender, EventArgs e)
        {
            InitializeAxisList(XAxisTransformList);
            InitializeAxisList(YAxisTransformList);            

            _charts.Clear();

            foreach (DataGridViewRow xRow in XAxisTransformList.Rows)
            {
                foreach(DataGridViewRow yRow in YAxisTransformList.Rows)
                {
                    _charts.Add(CreateNewChart());

                    var chart = _charts.Last();

                    graphLayoutPanel.Controls.Add(chart, yRow.Index, xRow.Index);

                    chart.ChartToolTip = StepToolTip;
                    StepToolTip.SetToolTip(chart, xRow.Cells[0].Value + "-" + yRow.Cells[0].Value);

                }
            }

            ResizeChartsBasedOnCount();

            //ALL TOOLTIPS MUST BE DONE IN LOAD FUNCTION!
            //StepToolTip.SetToolTip(XAxisTransformList, "Select one or more transforms to view.");
            //StepToolTip.SetToolTip(YAxisTransformList, "Select one or more transforms to view.");
            StepToolTip.SetToolTip(ResponseMinNum, Globals.SplitIntoLines("Minimum response that is indistinguishable from noise. Used to censor data."));
            StepToolTip.SetToolTip(ResponseMaxNum, Globals.SplitIntoLines("Maximum response of the inspection system. Used to censor data."));
            StepToolTip.SetToolTip(FlawMinNum, Globals.SplitIntoLines("Flaw's minimum range."));
            StepToolTip.SetToolTip(FlawMaxNum, Globals.SplitIntoLines("Flaw's maximum range."));
        }

        private void RunAnalyses()
        {
            if (XAxisTransformList.SelectedRows.Count == 0 ||
                YAxisTransformList.SelectedRows.Count == 0)
                return;

            if (analysisLauncher == null)
            {
                SetupAnalysisLauncher();
            }

            if (analysisLauncher.IsBusy == false)
            {

                _updatingTransformResults = true;
                analysisLauncher.RunWorkerAsync();
            }
        }

        private void InitializeAxisList(PODListBox listBox)
        {

            if (listBox.Rows.Count == 0)
            {
                listBox.SelectionChanged -= myList_SelectionChanged;
                listBox.CellMouseEnter -= myList_CellEnter;
                listBox.MouseLeave -= myList_Leave;
                listBox.MouseUp -= myList_MouseUp;
                listBox.Rows.Clear();


                listBox.Columns.Clear();

                var column = new DataGridViewTextBoxColumn();
                var columnQuestion = new DataGridViewImageColumn();


                column.FillWeight = 100;
                columnQuestion.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                listBox.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                listBox.ShowCellToolTips = true;

                listBox.Columns.Add(column);
                listBox.Columns.Add(columnQuestion);

                var row = listBox.CreateNewCloneRow(listBox.Columns.Count);
                row.Cells[0].Value = new TransformObj(TransformTypeEnum.Linear);
                row.Cells[0].ToolTipText = Column0ToolTip(listBox);
                row.Cells[1].Value = Properties.Resources.question;
                row.Cells[1].ToolTipText = Column1ToolTip(listBox);
                listBox.Rows.Add(row);
                row = listBox.CreateNewCloneRow(listBox.Columns.Count);
                row.Cells[0].Value = new TransformObj(TransformTypeEnum.Log);
                row.Cells[0].ToolTipText = Column0ToolTip(listBox);
                row.Cells[1].Value = Properties.Resources.question;
                row.Cells[1].ToolTipText = Column1ToolTip(listBox);
                listBox.Rows.Add(row);
                row = listBox.CreateNewCloneRow(listBox.Columns.Count);
                row.Cells[0].Value = new TransformObj(TransformTypeEnum.Inverse);
                row.Cells[0].ToolTipText = Column0ToolTip(listBox);
                row.Cells[1].Value = Properties.Resources.question;
                row.Cells[1].ToolTipText = Column1ToolTip(listBox);
                listBox.Rows.Add(row);

                //if (listBox == YAxisTransformList)
                //{
                    row = listBox.CreateNewCloneRow(listBox.Columns.Count);
                    row.Cells[0].Value = new TransformObj(TransformTypeEnum.BoxCox);
                    row.Cells[0].ToolTipText = Column0ToolTip(listBox);
                    row.Cells[1].Value = Properties.Resources.question;
                    row.Cells[1].ToolTipText = Column1ToolTip(listBox);
                    listBox.Rows.Add(row);
                //}

                listBox.Rows[0].Selected = true;
                listBox.Rows[1].Selected = true;
                //remove the box-cox for the x transform since it doesn't applly
                //listbox items do not work properly if the listbox is not 'nxn'
                if (listBox == XAxisTransformList)
                {
                    listBox.Rows[3].Visible = false;
                }

                listBox.SelectionChanged += myList_SelectionChanged;
                listBox.CellMouseEnter += myList_CellEnter;
                listBox.MouseLeave += myList_Leave;
                listBox.MouseUp += myList_MouseUp;

                listBox.FitAllRows(6);
            }
        }

        public string Column1ToolTip(PODListBox list)
        {
            var tooltip = Globals.SplitIntoLines("Highlight charts using this transform.");

            return tooltip;
        }

        public string Column0ToolTip(PODListBox list)
        {
            var tooltip = Globals.SplitIntoLines("Select one or more transforms to view.");

            return tooltip;
        }

        private void myList_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void myList_Leave(object sender, EventArgs e)
        {
            RemoveHighlightAllCharts();
        }

        private void myList_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            _rowIndex = e.RowIndex;

            if (e.ColumnIndex == 1)
            {
                PODListBox box = sender as PODListBox;

                if (box == null)
                    return;

                RemoveHighlightAllCharts();

                if (!box.Rows[e.RowIndex].Selected || (Control.MouseButtons & MouseButtons.Left) != 0)
                    return;


                HighlightChartBySelectedListRow(box, _rowIndex);
            }
            else if(e.ColumnIndex == 0)
            {
                RemoveHighlightAllCharts();
                RemoveHighlightAllCharts();
                RemoveHighlightAllCharts();
            }
        }

        private void HighlightChartBySelectedListRow(PODListBox box, int selectedIndex)
        {
            //var found = false;
            var selectedIndexes = GetSortedSelectedIndicies(box);

            var row = selectedIndex * YAxisTransformList.Rows.Count;
            var col = selectedIndex;

            if (box == XAxisTransformList)
            {
                for (int i = 0; i < XAxisTransformList.Rows.Count; i++)
                {
                    var chart = graphLayoutPanel.GetControlFromPosition(selectedIndex, i) as DataPointChart;
                    chart.HighlightChart();
                }
            }
            else
            {
                for (int i = 0; i < YAxisTransformList.Rows.Count; i++)
                {
                    var chart = graphLayoutPanel.GetControlFromPosition(i, selectedIndex) as DataPointChart;
                    chart.HighlightChart();
                }
            }
        }

        private static List<int> GetSortedSelectedIndicies(PODListBox box)
        {
            var selectedIndexes = new List<int>();

            foreach (DataGridViewRow dataRow in box.SelectedRows)
            {
                selectedIndexes.Add(dataRow.Index);
            }

            selectedIndexes.Sort();
            return selectedIndexes;
        }

        private void RemoveHighlightAllCharts()
        {
            foreach (Control control in graphLayoutPanel.Controls)
            {
                var chart = control as DataPointChart;
                if (chart.IsHighlighted)
                    chart.RemoveChartHighlight();
            }
        }

        private void DeselectAllCharts()
        {
            foreach (Control control in graphLayoutPanel.Controls)
            {
                var chart = control as DataPointChart;
                if (chart.IsSelected)
                    chart.ForceSelectionOff();
            }
        }

        void myList_SelectionChanged(object sender, EventArgs e)
        {
            if (XAxisTransformList.SelectedRows.Count == 0 || YAxisTransformList.SelectedRows.Count == 0)
                return;

            if (analysisLauncher != null && analysisLauncher.IsBusy)
                return;

            var sortedSelectedY = GetSortedSelectedIndicies(YAxisTransformList);
            var sortedSelectedX = GetSortedSelectedIndicies(XAxisTransformList);

            if (_topX < sortedSelectedX.Max() + 1 || _topY < sortedSelectedY.Max() + 1)
                RunAnalyses();

            SelectVisibleCharts();
            ResizeChartsBasedOnCount();
            //RunAnalyses();
        }

        private void SelectVisibleCharts()
        {
        }

        public List<TransformObj> SelectedXAxisTransforms
        {
            get
            {
                return GetTransformListFromListBox(XAxisTransformList);
            }
        }

        private List<TransformObj> GetTransformListFromListBox(PODListBox listBox)
        {
            var list = new List<TransformObj>();

            foreach(DataGridViewRow row in listBox.Rows)
            {
                if (row.Selected)
                    list.Add(row.Cells[0].Value as TransformObj);
            }

            return list;
        }

        public List<TransformObj> SelectedYAxisTransforms
        {
            get
            {
                return GetTransformListFromListBox(YAxisTransformList);
            }
        }

        private void Range_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                _analysis.InFlawMin = Convert.ToDouble(FlawMinNum.Value);
                _analysis.InFlawMax = Convert.ToDouble(FlawMaxNum.Value);

                _analysis.InResponseMin = Convert.ToDouble(ResponseMinNum.Value);
                _analysis.InResponseMax = Convert.ToDouble(ResponseMaxNum.Value);
            }
            catch 
            {

            }

            RefreshValues();            
        }

        private void Numeric_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        public TransformTypeEnum SelectedXAxisTransform
        {
            get
            {
                var box = XAxisTransformList;
                var getRowInsteadOfColumn = false;

                return GetTransformEnumFromSelectedChart(box, getRowInsteadOfColumn);
            }
        }

        public TransformTypeEnum SelectedYAxisTransform
        {
            get
            {
                var box = YAxisTransformList;
                var getRowInsteadOfColumn = true;

                return GetTransformEnumFromSelectedChart(box, getRowInsteadOfColumn);
            }
        }

        private TransformTypeEnum GetTransformEnumFromSelectedChart(PODListBox box, bool getRowInsteadOfColumn)
        {
            var chart = SelectedChart;
            var rowIndex = 0;

            if (getRowInsteadOfColumn == true)
            {
                rowIndex = graphLayoutPanel.GetPositionFromControl(chart).Row;
            }
            else
            {
                rowIndex = graphLayoutPanel.GetPositionFromControl(chart).Column;
            }

            var transform = box.Rows[rowIndex].Cells[0].Value as TransformObj;

            return transform.TransformType;
        }

        public DataPointChart SelectedChart
        {
            get
            {
                DataPointChart selected = graphLayoutPanel.GetControlFromPosition(0,0) as DataPointChart;

                foreach(var control in graphLayoutPanel.Controls)
                {
                    var chart = control as DataPointChart;

                    if(chart != null && chart.IsSelected)
                    {
                        selected = chart;
                        break;
                    }
                }

                return selected;
            }
        }

        public List<TransformObj> AllXTransforms
        {
            get
            {
                return GetAllTransformsFromListBox(XAxisTransformList);
            }
        }

        public List<TransformObj> AllYTransforms
        {
            get
            {
                return GetAllTransformsFromListBox(YAxisTransformList);
            }
        }

        private List<TransformObj> GetAllTransformsFromListBox(PODListBox listBox)
        {
            var list = new List<TransformObj>();

            foreach (DataGridViewRow row in listBox.Rows)
            {
                list.Add(row.Cells[0].Value as TransformObj);
            }

            return list;
        }

        public bool _updatingTransformResults { get; set; }

        public bool _viewResiduals { get; set; }
    }
}
