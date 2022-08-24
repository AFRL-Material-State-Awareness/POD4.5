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
using CSharpBackendWithR;
namespace POD.Wizards.Steps.HitMissNormalSteps
{
    using System.Data;
    using System.Threading;
    using System.Windows.Forms.DataVisualization.Charting;

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

            /*if (!DesignMode)
            {
                InitializeAxisList(XAxisTransformList);
                InitializeAxisList(YAxisTransformList);
            }*/

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

            UpdateModelCompares();

            _updatingTransformResults = false;

            _analysis.IsBusy = false;
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

            _analysis.IsBusy = true;

            //_analysis.SetREngine(new CSharpBackendWithR.REngineObject());
                        
            var xAxisIndex = 0;
            var yAxisIndex = 0;

            var sortedSelectedY = GetSortedSelectedIndicies(ModelList);
            var sortedSelectedX = GetSortedSelectedIndicies(XAxisTransformList);

            var xRowCount = XAxisTransformList.Rows.Count;

            var initialX = _analysis.InFlawTransform;
            var initialY = _analysis.InHitMissModel;

            _topX = sortedSelectedX.Max() + 1;
            _topY = sortedSelectedY.Max() + 1;

            //var fixList = new List<FixPoint>();
            //_analysis.Data.UpdateIncludedPointsBasedFlawRange(_analysis.InFlawMax, _analysis.InFlawMin, fixList);
            REngineObject.REngineRunning = true;
            
            foreach (var xTrans in AllXTransforms)
            {
                yAxisIndex = 0;

                this.Invoke((MethodInvoker)delegate()
                {
                    Parent.Cursor = Cursors.WaitCursor;
                });

                foreach (var yTrans in AllModels)
                {

                    if (yAxisIndex < _topY && xAxisIndex < _topX)
                    {

                        _analysis.Data.FilterTransformedDataByRanges = true;
                        _analysis.InFlawTransform = xTrans.TransformType;
                        _analysis.InResponseTransform = TransformTypeEnum.Linear;
                        _analysis.InHitMissModel = yTrans.ModelType;

                        var yColumnIndex = yAxisIndex;
                        var xColumnIndex = xAxisIndex;

                        try
                        {
                            _analysis.RunOnlyFitAnalysis();
                        }
                        catch(Exception eProblem)
                        {
                            MessageBox.Show(eProblem.ToString());
                        }

                        this.Invoke((MethodInvoker)delegate()
                        {
                            var chart = _charts[yAxisIndex * xRowCount + xAxisIndex];

                            //watch.Restart();
                            //watch.Stop();
                            //MessageBox.Show(xTrans.ConfIntervalType.ToString() + " " + yTrans.ConfIntervalType.ToString() + " " + watch.ElapsedMilliseconds + "ms");
                            chart.FillFromAnalysis(_analysis.Data, _analysis.InHitMissModel, _colors, xColumnIndex, yColumnIndex);
                            UpdateChartsWithCurrentView(chart);
                            chart.Update();
                        });

                    }


                    yAxisIndex++;
                }

                xAxisIndex++;
            }
            if (Analysis.SeparatedFlag)
            {
                MessageBox.Show("Warning: Hit Miss data has total(or almost total) separation!"+'\n'+ "Please consider using the following metrics:"+'\n' +"-Firth Logistic Regression" +'\n' +
                    "-Likelihood Ratio(LR)" + '\n' + "-Modified Likelihood Ratio (MLR)" + '\n' + "-Ranked Set Sampling" + '\n' + "-Or a combination of these.");
            }
            REngineObject.REngineRunning = false;
            this.Invoke((MethodInvoker)delegate()
            {
                Parent.Cursor = Cursors.Default;
            });

            _analysis.InFlawTransform = initialX;
            _analysis.InHitMissModel = initialY;

            this.Invoke((MethodInvoker)delegate()
            {
                Refresh();
            });
        }

        private int _rowIndex;

        internal override void PrepareGUI()
        {
            base.PrepareGUI();


        }

        public override void RefreshValues()
        {
            if(_source != null)
            {
                //*******
                SelectChartBasedOnAnalysis();

                var yAxisIndex = 0;
                var xAxisIndex = 0;
                var xRowCount = XAxisTransformList.Rows.Count;

                foreach (var xTrans in AllXTransforms)
                {
                    yAxisIndex = 0;

                    foreach (var yTrans in AllModels)
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
                    FlawMinNum.Value = Convert.ToDecimal(_analysis.InFlawMin);
                    FlawMaxNum.Value = Convert.ToDecimal(_analysis.InFlawMax);

                    _analysis.Data.ForceRefillSortList();
                    var fixPoints = new List<FixPoint>();
                    _analysis.Data.UpdateIncludedPointsBasedFlawRange(_analysis.TransformValueForXAxis(_analysis.InFlawMax), _analysis.TransformValueForXAxis(_analysis.InFlawMin), fixPoints);
                }
                catch
                {

                }

                ResizeChartsBasedOnCount();

                RunAnalyses();     
                
                
            }
        }

        private void UpdateModelCompares()
        {
            for (int i = 0; i < AllXTransforms.Count; i++)
            {
                var chartTop = graphLayoutPanel.GetControlFromPosition(i, 0) as TransformChart;
                var chartBottom = graphLayoutPanel.GetControlFromPosition(i, 1) as TransformChart;

                CopyPoints(chartTop, chartBottom);
                CopyPoints(chartBottom, chartTop);
            }
        }

        private static void CopyPoints(TransformChart copyFrom, TransformChart copyTo)
        {
            copyTo.ModelCompare.Points.Clear();

            foreach (DataPoint point in copyFrom.FlawEstimate.Points)
            {
                copyTo.ModelCompare.Points.AddXY(point.XValue, point.YValues.First());
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

        public bool ChangeModelCompare()
        {
            if (!_updatingTransformResults && !_viewResiduals)
            {
                _compareModels = !_compareModels;

                UpdateChartsWithCurrentModelCompare();
            }

            return _compareModels;
        }

        private void UpdateChartsWithCurrentModelCompare()
        {
            if (_compareModels)
            {
                CompareModels();
            }
            else
            {
                IndividualModels();
            }
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

                    if (_compareModels)
                        chart.ShowModels();
                    else
                        chart.HideModels();
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
                    chart.HideModels();
                }
            }
        }

        private void CompareModels()
        {
            foreach (Control control in graphLayoutPanel.Controls)
            {
                var chart = control as TransformChart;

                if (chart != null)
                {
                    chart.ShowModels();
                }
            }
        }

        private void IndividualModels()
        {
            foreach (Control control in graphLayoutPanel.Controls)
            {
                var chart = control as TransformChart;

                if (chart != null)
                {
                    chart.HideModels();
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
            //gets all transforms
            var yTransforms = AllModels;
            var xTransforms = AllXTransforms;

            DeselectAllCharts();
            //Ask tom About this
            for(int i = 0; i < yTransforms.Count; i++)
            {
                for(int j = 0; j < xTransforms.Count; j++)
                {
                    var chart = graphLayoutPanel.GetControlFromPosition(j, i) as DataPointChart;

                    if(yTransforms[i].ModelType == _analysis.InHitMissModel && 
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
            var list = new List<PODNumericUpDown> { FlawMinNum, FlawMaxNum };

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

            chart.AutoNameYAxis = false;
            chart.SetupChart(Analysis.Data.AvailableFlawNames[0], Analysis.Data.AvailableFlawUnits[0],
                             Analysis.Data.AvailableResponseNames, Analysis.Data.AvailableResponseUnits);

            

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
                var ySelected = GetSortedSelectedIndicies(ModelList);
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
                    row = 0;

                    foreach (var yTrans in AllModels)
                    {
                        TransformChart chart = null;

                        chart = graphLayoutPanel.GetControlFromPosition(column, row) as TransformChart;

                        chart.Width = newWidth;
                        chart.Height = newHeight;                        

                        row++;
                    }

                    column++;
                }

                for (int i = 0; i < AllModels.Count; i++)
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

                for (int i = 0; i < AllXTransforms.Count; i++)
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
                _analysis.InHitMissModel = SelectedModel;
                _analysis.InResponseTransform = TransformTypeEnum.Linear;

            }
        }

        private void ClearAllChartEvents()
        {
            
        }

        

        void ChooseTransformPanel_Load(object sender, EventArgs e)
        {
            InitializeAxisList(XAxisTransformList);
            InitializeModelList(ModelList);            

            _charts.Clear();

            foreach (DataGridViewRow xRow in ModelList.Rows)
            {
                foreach (DataGridViewRow yRow in XAxisTransformList.Rows)
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
            //StepToolTip.SetToolTip(XAxisTransformList, Column0ToolTip(XAxisTransformList));
            //StepToolTip.SetToolTip(ModelList, Column0ToolTip(ModelList));
            StepToolTip.SetToolTip(FlawMinNum, Globals.SplitIntoLines("Flaw's minimum range."));
            StepToolTip.SetToolTip(FlawMaxNum, Globals.SplitIntoLines("Flaw's maximum range."));


        }

        

        private void RunAnalyses()
        {
            if (XAxisTransformList.SelectedRows.Count == 0 ||
                ModelList.SelectedRows.Count == 0)
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

                listBox.Rows[0].Selected = true;
                listBox.Rows[1].Selected = true;

                listBox.SelectionChanged += myList_SelectionChanged;
                listBox.CellMouseEnter += myList_CellEnter;
                listBox.MouseLeave += myList_Leave;
                listBox.MouseUp += myList_MouseUp;

                listBox.FitAllRows(6);
            }
        }

        private void InitializeModelList(POD.Controls.PODListBox listBox)
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
            listBox.ShowCellToolTips = true;

            listBox.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;


            listBox.Columns.Add(column);
            listBox.Columns.Add(columnQuestion);

            var row = listBox.CreateNewCloneRow(listBox.Columns.Count);
            row.Cells[0].Value = new PFModelObj(PFModelEnum.Normal);
            row.Cells[0].ToolTipText = Column0ToolTip(listBox);
            row.Cells[1].Value = Properties.Resources.question;
            row.Cells[1].ToolTipText = Column1ToolTip(listBox);
            listBox.Rows.Add(row);
            row = listBox.CreateNewCloneRow(listBox.Columns.Count);
            row.Cells[0].Value = new PFModelObj(PFModelEnum.Odds);
            row.Cells[0].ToolTipText = Column0ToolTip(listBox);
            row.Cells[1].Value = Properties.Resources.question;
            listBox.Rows.Add(row);

            listBox.Rows[0].Selected = true;
            listBox.Rows[1].Selected = true;

            listBox.SelectionChanged += myList_SelectionChanged;
            listBox.CellMouseEnter += myList_CellEnter;
            listBox.MouseLeave += myList_Leave;
            listBox.MouseUp += myList_MouseUp;

            listBox.FitAllRows(6);
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

            var colIndex = e.ColumnIndex;
            var list = sender as PODListBox;

            //if (list != null)
            //{
            //    if (colIndex == 1)
            //        list.Rows[_rowIndex].Cells[colIndex].ToolTipText = Column1ToolTip(list);
            //    else
            //        list.Rows[_rowIndex].Cells[colIndex].ToolTipText = Column0ToolTip(list);
            //}

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

            var row = selectedIndex * XAxisTransformList.Rows.Count;
            var col = selectedIndex;

            if (box == XAxisTransformList)
            {
                for (int i = 0; i < ModelList.Rows.Count; i++)
                {
                    var chart = graphLayoutPanel.GetControlFromPosition(selectedIndex, i) as DataPointChart;
                    chart.HighlightChart();
                }
            }
            else
            {
                for (int i = 0; i < XAxisTransformList.Rows.Count; i++)
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
            if (XAxisTransformList.SelectedRows.Count == 0 || ModelList.SelectedRows.Count == 0)
                return;

            if (analysisLauncher != null && analysisLauncher.IsBusy)
                return;

            var sortedSelectedY = GetSortedSelectedIndicies(ModelList);
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
                return GetTransformListFromListBox(ModelList);
            }
        }

        private void Range_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                _analysis.InFlawMin = Convert.ToDouble(FlawMinNum.Value);
                _analysis.InFlawMax = Convert.ToDouble(FlawMaxNum.Value);
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

        public PFModelEnum SelectedModel
        {
            get
            {
                var box = ModelList;
                var getRowInsteadOfColumn = true;

                return GetModelEnumFromSelectedChart(box, getRowInsteadOfColumn);
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

        private PFModelEnum GetModelEnumFromSelectedChart(PODListBox box, bool getRowInsteadOfColumn)
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

            var model = box.Rows[rowIndex].Cells[0].Value as PFModelObj;

            return model.ModelType;
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

        public List<PFModelObj> AllModels
        {
            get
            {
                return GetAllModelsFromListBox(ModelList);
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

        private List<PFModelObj> GetAllModelsFromListBox(PODListBox listBox)
        {
            var list = new List<PFModelObj>();

            foreach (DataGridViewRow row in listBox.Rows)
            {
                list.Add(row.Cells[0].Value as PFModelObj);
            }

            return list;
        }

        public bool _updatingTransformResults { get; set; }

        public bool _viewResiduals;
        public bool _compareModels;
        private int _topX;
        private int _topY;

        public string Column1ToolTip(PODListBox list)
        {
            var tooltip = "";

            if(list == XAxisTransformList)
            {
                tooltip = Globals.SplitIntoLines("Highlight charts using this transform.");
            }
            else if (list == ModelList)
            {
                tooltip = Globals.SplitIntoLines("Hightlight charts using this model.");
            }

            return tooltip;
        }

        public string Column0ToolTip(PODListBox list)
        {
            var tooltip = "";

            if (list == XAxisTransformList)
            {
                tooltip = Globals.SplitIntoLines("Select one or more transforms to view.");
            }
            else if (list == ModelList)
            {
                tooltip = Globals.SplitIntoLines("Select one or more models to view.");
            }

            return tooltip;
        }
    }
}
