using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using POD.Data;
using POD.Controls;
using System.Runtime.InteropServices;
using POD.Analyze;

namespace POD.Wizards
{
    public delegate void DataChartTableHandler(object sender, DataChartTableArg e);
    public delegate void DataViewsHandler(object sender, List<DataView> e);

    public partial class ViewInfoPanel : UserControl
    {
        /// <summary>
        /// View all graphs with no scrolling.
        /// </summary>
        bool _viewAll;
        /// <summary>
        /// Stack all graphs onto on graph.
        /// </summary>
        bool _stackAll;
        int _prevIndex;
        SourceInfo _info;
        DataSource _source;
        List<DataView> _views;
        DataTable _specTable;
        private int _seriesIndex;
        private int _pointIndex;
        PODToolTip StepToolTip;
        /// <summary>
        /// List of colors to use when coloring the data series
        /// </summary>
        List<Color> _colors = new List<Color>();

        public ViewInfoPanel()
        {
            InitializeComponent();

            StepToolTip = new PODToolTip();

            FlawsListBox.Columns.Clear();
            ResponsesListBox.Columns.Clear();

            FlawsListBox.Columns.Add(new DataGridViewColumn(new DataGridViewTextBoxCell()));
            ResponsesListBox.Columns.Add(new DataGridViewColumn(new DataGridViewTextBoxCell()));

            graphFlowPanel.Resize += graphFlowPanel_Resize;

            //graphFlowPanel.VerticalScroll.Visible = true;
            //graphFlowPanel.AutoScroll = false;
            //graphFlowPanel.HorizontalScroll.Visible = true;

            graphDataGridView.CellEnter += graphDataGridView_CellClick;
            graphDataGridView.DataSource = null;
            graphDataGridView.DefaultCellStyle.SelectionBackColor = Color.FromKnownColor(KnownColor.WindowText);
            graphDataGridView.DefaultCellStyle.SelectionForeColor = Color.FromKnownColor(KnownColor.Window);
            graphDataGridView.MultiSelect = false;
            graphDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            _viewAll = false;
            _stackAll = true;
            _updateSelection = true;

            _prevIndex = 0;

            Load += ViewInfoPanel_Load;

            //not added because flow panel is not receiving the event
            //graphFlowPanel.PreviewKeyDown += graphFlowPanel_PreviewKeyDown;

            _colors = DataPointChart.GetLargeColorList(this.DesignMode);

            _illegalCharacterTooltip = @"Name must be unique and cannot use the following characters:" + System.Environment.NewLine + @" ~, (, ), #, \, /, =, >, <, +, -, ,* ,% , &, |, ^, ', "", [, ] ";

            //ToolTipCtrl.SetToolTip(FlawNameTextBox, illegalCharacterStepToolTip);
            //ToolTipCtrl.SetToolTip(ResponseNameTextBox, illegalCharacterStepToolTip);

            //ColumnInfoPanel.Visible = false;
            //graphFlowPanel.Visible = false;

            //graphFlowPanel.VerticalScroll.Visible = true;
            //graphFlowPanel.HorizontalScroll.Visible = false;

            StepToolTip.ShowAlways = true;
            StepToolTip.Popup += StepToolTip_Popup;
        }

        void StepToolTip_Popup(object sender, PopupEventArgs e)
        {
            string tip = StepToolTip.GetToolTip(e.AssociatedControl);

            if (tip.Length == 0)
                e.Cancel = true;

            StepToolTip.ShowAlways = true;
        }

        void ViewInfoPanel_Load(object sender, EventArgs e)
        {
            //ALL TOOLTIPS MUST BE DONE IN LOAD FUNCTION!
            StepToolTip.SetToolTip(FlawsListBox, Globals.SplitIntoLines("List of available flaws for this data source."));
            StepToolTip.SetToolTip(FlawNameTextBox, Globals.SplitIntoLines("Name of the selected flaw. Enter, up, down, switches selected flaw."));
            StepToolTip.SetToolTip(FlawUnitTextBox, Globals.SplitIntoLines("Selected flaw's unit."));
            StepToolTip.SetToolTip(FlawUnitCheckBox, Globals.SplitIntoLines("Apply change to all available flaws?"));
            StepToolTip.SetToolTip(FlawMinimumNumeric, Globals.SplitIntoLines("Selected flaw's minimum range."));
            StepToolTip.SetToolTip(FlawMinimumCheckBox, Globals.SplitIntoLines("Apply change to all available flaws?"));
            StepToolTip.SetToolTip(FlawMaximumNumeric, Globals.SplitIntoLines("Selected flaw's maximum range."));
            StepToolTip.SetToolTip(FlawMaximumCheckBox, Globals.SplitIntoLines("Apply change to all available flaws?"));
            StepToolTip.SetToolTip(FlawNameCheckBox, Globals.SplitIntoLines("Not applicable."));

            StepToolTip.SetToolTip(ResponsesListBox, Globals.SplitIntoLines("List of available responses for this data source."));
            StepToolTip.SetToolTip(ResponseNameTextBox, Globals.SplitIntoLines("Name of the selected response. Enter, up, down, switches selected response."));
            StepToolTip.SetToolTip(ResponseUnitTextBox, Globals.SplitIntoLines("Selected responses's unit."));
            StepToolTip.SetToolTip(ResponseUnitCheckBox, Globals.SplitIntoLines("Apply change to all available responses?"));
            StepToolTip.SetToolTip(ResponseMinimumNumeric, Globals.SplitIntoLines("Selected responses's minimum response that is indistinguishable from noise. Used to censor data."));
            StepToolTip.SetToolTip(ResponseMinimumCheckBox, Globals.SplitIntoLines("Apply change to all available responses?"));
            StepToolTip.SetToolTip(ResponseMaximumNumeric, Globals.SplitIntoLines("Selected response's maximum response of the inspection system. Used to censor data."));
            StepToolTip.SetToolTip(ResponseMaximumCheckBox, Globals.SplitIntoLines("Apply change to all available responses?"));
            StepToolTip.SetToolTip(ResponseThresholdNumeric, Globals.SplitIntoLines("Selected responses's POD decision value. Values above the threshold are hits. Values below are misses."));
            StepToolTip.SetToolTip(ResponseThresholdCheckBox, Globals.SplitIntoLines("Apply change to all available responses?"));
            StepToolTip.SetToolTip(ResponseNameCheckBox, Globals.SplitIntoLines("Not applicable."));
        }

        void graphFlowPanel_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            e.IsInputKey = true;
        }
        
        public CheckBox GetCheckBox(Control myControl)
        {
            var row = ColumnInfoPanel.GetRow(myControl);
            var column = ColumnInfoPanel.GetColumn(myControl);

            //it's always the column left next to the control
            column--;

            var checkBox = ColumnInfoPanel.GetControlFromPosition(column, row);

            if (checkBox != null)
                return checkBox as CheckBox;
            else
                return null;
        }

        public SourceInfo Info
        {
            set
            {
                

                //if (_info != value || FlawsListBox.Rows.Count != _info.GetInfos(ColType.Flaw).Count || 
                //    ResponsesListBox.Rows.Count != _info.GetInfos(ColType.Response).Count)
                //{
                _info = value;

                _updateSelection = false;

                RefreshListBoxes();
            }
        }

        public void RefreshListBoxes()
        {
            _updateSelection = false;

            var prevFlawIndex = FlawsListBox.SingleSelectedIndex;
            var prevRespIndex = ResponsesListBox.SingleSelectedIndex;

            if (prevFlawIndex < 0)
                prevFlawIndex = 0;

            if (prevRespIndex < 0)
                prevRespIndex = 0;

            FillListBox(FlawsListBox, ColType.Flaw);
            FillListBox(ResponsesListBox, ColType.Response);

            FlawsListBox.SingleSelectedIndex = prevFlawIndex;
            ResponsesListBox.SingleSelectedIndex = prevRespIndex;

            ColumnInfoPanel.Height = ColumnInfoPanel.GetPreferredSize(panel1.Size).Height;

            _updateSelection = true;
        }

        public void RefreshMetaDataControls()
        {
            UpdateMetaDataControls(FlawsListBox.SingleSelectedIndex, ResponsesListBox.SingleSelectedIndex);
        }

        private void UpdateMetaDataControls(int flawIndex, int responseIndex)
        {
            FillFlawMetaDataControls(flawIndex);
            FillResponeMetaDataControls(responseIndex);
        }

        private void FillListBox(PODListBox myBox, ColType myType)
        {
            

            DataGridViewRow newRow;
            List<DataGridViewRow> rowList;
            List<ColumnInfo> infos;
            List<string> activeColumnNames = new List<string>();
            
            if(myType == ColType.Response)
                activeColumnNames = _source.ResponseLabels;
            else if(myType == ColType.Flaw)
                activeColumnNames = _source.FlawLabels;

            myBox.Rows.Clear();       

            newRow = myBox.CreateNewCloneRow();
            rowList = new List<DataGridViewRow>();

            infos = _info.GetInfos(myType);

            foreach (ColumnInfo info in infos)
            {
                if (activeColumnNames.Contains(info.NewName))
                {

                    PODListBoxItemWithProps newItem = new PODListBoxItemWithProps(Color.Black, myType, info.NewName,
                                                                                  string.Empty, info.Unit, info.Min, info.Max,
                                                                                  info.Threshold);

                    var row = (DataGridViewRow)newRow.Clone();

                    row.Cells[0].Value = newItem;

                    PODListBox.RemoveExtraColumns(row);

                    rowList.Add(row);
                }
            }

            myBox.Rows.AddRange(rowList.ToArray());

            myBox.FitAllRows(6);
        }

        public DataSource Source
        {
            set
            {
                _source = value;
            }

        }


        private void graphDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!HandleUserInteraction)
                return;

            UpdateSelectedPointFromTable(e.RowIndex);
        }

        private void UpdateSelectedPointFromTable(int rowIndex)
        {           
            ColorSelectedPoint(rowIndex);
        }

        private void ColorSelectedPoint(int pointIndex)
        {
            DataPointChart chart;

            foreach (Control control in graphFlowPanel.Controls)
            {
                chart = (DataPointChart)control;

                ColorSelectedPoint(chart, pointIndex, _seriesIndex);

            }

            _prevIndex = pointIndex;
            _pointIndex = pointIndex;
        }

        public static void EstimateMinMaxValues(DataSource source, SourceInfo info)
        {
            if (source != null && info != null)
            {
                double min = 0.0;
                double max = 0.0;
                var colType = ColType.Flaw;
                var labels = source.FlawLabels;
                var mins = source.Minimums(colType);
                var maxes = source.Maximums(colType);
                

                for (int i = 0; i < labels.Count; i++)
                {
                    var table = source.GetData(labels[i]);

                    Analysis.GetBufferedMinMax(table, out min, out max);

                    if (mins[i] == 0.0 && maxes[i] == 0.0)
                    {
                        info.SetDoubleProperty(i, colType, InfoType.Max, max);
                        info.SetDoubleProperty(i, colType, InfoType.Min, min);
                    }
                }

                colType = ColType.Response;

                labels = source.ResponseLabels;
                mins = source.Minimums(colType);
                maxes = source.Maximums(colType);

                for (int i = 0; i < labels.Count; i++)
                {
                    var table = source.GetData(labels[i]);

                    Analysis.GetBufferedMinMax(table, out min, out max);

                    if (mins[i] == 0.0 && maxes[i] == 0.0)
                    {
                        var thresh = (max - min) * .15 + min;

                        //change to hit/miss type ranges since that is what it most likely is
                        if(min == -.1 && max == 1.1)
                        {
                            min = 0.0;
                            max = 1.0;
                            thresh = .5;
                        }

                        info.SetDoubleProperty(i, colType, InfoType.Max, max);
                        info.SetDoubleProperty(i, colType, InfoType.Min, min);
                        info.SetDoubleProperty(i, colType, InfoType.Threshold, thresh);
                    }
                }
            }
        }

        private void ColorSelectedPoint(DataPointChart chart, int pointIndex, int seriesIndex)
        {
            chart.Series[chart.Series.Count - 1].Points.Clear();

            for (int i = 0; i < chart.Series.Count - 1; i++)
            {
                Series series = chart.Series[i];

                if (series.Points.Count > pointIndex)
                {
                    DataPoint point = series.Points[pointIndex].Clone();
                    point.SetCustomProperty("Original Series Index", i.ToString());
                    point.SetCustomProperty("Original Point Index", pointIndex.ToString());
                    point.MarkerBorderColor = Color.Black;
                    if (seriesIndex == i || _stackAll == false)
                    {
                        point.MarkerBorderWidth = 2;
                        point.MarkerSize = 12;
                    }
                    else
                    {
                        point.MarkerBorderWidth = 0;
                        point.MarkerSize = 10;
                    }
                    point.MarkerColor = chart.GetColor(series);

                    if (!point.IsEmpty && (seriesIndex == i || _stackAll == false))
                    {
                        point.MarkerStyle = MarkerStyle.Circle;
                        chart.Series[chart.Series.Count - 1].Points.Add(point);
                    }
                }
            }
        }

        void graphFlowPanel_Resize(object sender, EventArgs e)
        {
            int divideBy = 2;
            int extraSubtract = 0;

            //SuspendDrawing();

            graphFlowPanel.SuspendLayout();

            int scrollBarWidth = System.Windows.Forms.SystemInformation.VerticalScrollBarWidth;

            if (graphFlowPanel.Controls.Count == 1)
                divideBy = 1;

            //if (_stackAll == true)
            //{
            //    divideBy = 1;
            //}
            //else 

            if (_viewAll == true)
            {
                divideBy = Convert.ToInt32(Math.Ceiling(Math.Sqrt(graphFlowPanel.Controls.Count)));
            }

            if (_viewAll == false && graphFlowPanel.Controls.Count > 4)
                extraSubtract = 10;// scrollBarWidth;

            foreach (Control control in graphFlowPanel.Controls)
            {
                control.Height = (graphFlowPanel.Height / divideBy) - control.Margin.Left - control.Margin.Right - extraSubtract;// -(scrollBarWidth / divideBy + 10);
                control.Width = (graphFlowPanel.Width / divideBy) - control.Margin.Left - control.Margin.Right - extraSubtract;// -(scrollBarWidth / divideBy + 10);
            }

            //graphFlowPanel.VerticalScroll.Maximum = graphFlowPanel.Height;

            graphFlowPanel.ResumeLayout();

            //ColumnInfoPanel.Padding = new Padding(0, 0, 0, 0);

            /*if (ColumnInfoPanel.VerticalScroll.Visible == true)
                ColumnInfoPanel.Padding = new Padding(0, 0, scrollBarWidth, 0);
            else
                ColumnInfoPanel.Padding = new Padding(0, 0, 0, 0);*/

            //ResumeDrawing();
        }

        private void EditControl_MouseEnter(object sender, EventArgs e)
        {
            _activeBefore = ActiveControl;

            //StepToolTip.Show(StepToolTip.GetToolTip(sender as Control), sender as Control, 500);
        }

        public void FillGraphs()
        {
            var charts = new List<DataPointChart>();

            //_views = _source.GetAllGraphData();

            for (int i = 0; i < FlawsListBox.Rows.Count; i++)
            {
                for (int j = 0; j < ResponsesListBox.Rows.Count; j++)
                {
                    //if not stacking or first of stack
                    if (_stackAll == false || j == 0)
                    {
                        var chart = new DataPointChart();

                        chart.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
                        chart.ChartAreas[0].AxisY.MajorGrid.LineWidth = 0;

                        charts.Add(chart);

                        

                        //chart.Width = graphFlowPanel.Width;
                        //chart.Height = chart.Width;

                        chart.FillFromSource(_source, i, j, _colors);
                        //we have a selected series so it needs to be 2 instead of 1
                        //since don't want to count the selected series
                        //when figuring out the y-axis title
                        chart.SingleSeriesCount = 2;

                        UpdateFromUI(chart, i, j);

                        graphFlowPanel.Controls.Add(chart);

                        chart.Click += chart_Click;
                        chart.MouseDown += chart_MouseDown;
                        chart.PostPaint += chart_PostPaint;
                        //not added because chart is not receving events
                        //chart.KeyDown += chart_KeyDown;

                        chart.CleanUpDataSeries();

                        if (_stackAll == false)
                        {
                            Series selected = chart.Series.Add("Selected");
                            selected.ChartType = SeriesChartType.Point;
                        }
                    }
                    else
                    {
                        charts[i].FillFromSource(_source, i, j, _colors);
                        UpdateFromUI(charts[i], i, j);
                    }
                }

                if (_stackAll == true)
                {
                    Series selected = charts[i].Series.Add("Selected");
                    selected.ChartType = SeriesChartType.Point;
                    selected.IsVisibleInLegend = false;
                    var legend = new Legend("Default");
                    legend.Alignment = StringAlignment.Center;
                    legend.Docking = Docking.Top;
                    legend.IsDockedInsideChartArea = false;
                    legend.LegendStyle = LegendStyle.Table;
                    charts[i].Legends.Add(legend);
                }

            }

            SelectChart(GetChartIndexFromSelectedListBoxes());

            //DataSourceLabel.Text = "Data Source: " + project.Sources[_currentDataSourceIndex].SourceName;
        }

        void chart_KeyDown(object sender, KeyEventArgs e)
        {
            DataPointChart nextChart = null;
            DataPointChart currentChart = sender as DataPointChart;

            if (currentChart == null)
                return;

            if(e.KeyCode == Keys.Up || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || e.KeyCode == Keys.Down)
            {
                nextChart = ModifySelectedPointSeries(sender as DataPointChart, e.KeyCode);
            }
            if (nextChart == null)
                nextChart = currentChart;

            ColorSelectedPoint(nextChart, _seriesIndex, _pointIndex);
            UpdateChartAndRelatedControls(nextChart);
        }


        private DataPointChart ModifySelectedPointSeries(DataPointChart dataPointChart, Keys key)
        {
            DataPointChart nextChart = null;
            var chartIndex = 0;

            if (dataPointChart == null || graphDataGridView == null)
                return dataPointChart;

            if (graphDataGridView.Rows.Count == 0)
                return dataPointChart;

            chartIndex = graphFlowPanel.Controls.IndexOf(dataPointChart);

            if (chartIndex == -1)
                return dataPointChart;

            if (graphDataGridView.SelectedRows.Count == 0)
                graphDataGridView.Rows[0].Selected = true;
            
            switch(key)
            {
                case Keys.Up:
                    _pointIndex++;
                    break;
                case Keys.Down:
                    _pointIndex--;
                    break;
                case Keys.Left:
                    _seriesIndex--;
                    break;
                case Keys.Right:
                    _seriesIndex++;
                    break;
                default:
                    break;
            }

            //fix point index
            if (_pointIndex < 0)
            {
                _pointIndex = graphDataGridView.Rows.Count - 1;
                _seriesIndex--;
            }
            else if (_pointIndex == graphDataGridView.Rows.Count)
            {
                _pointIndex = 0;
                _seriesIndex++;
            }

            //fix series index
            if(_seriesIndex < 0)
            {
                _seriesIndex = 0;
                chartIndex--;
            }
            else if (_seriesIndex == dataPointChart.Series.Count)
            {
                _seriesIndex = 0;
                chartIndex++;
            }

            //fix chartIndex
            if(chartIndex < 0)
            {
                chartIndex = graphFlowPanel.Controls.Count - 1;
            }
            else if (chartIndex == graphFlowPanel.Controls.Count)
            {
                chartIndex = 0;
            }

            nextChart = graphFlowPanel.Controls[chartIndex] as DataPointChart;

            return nextChart;
        }
        
        private void UpdateFromUI(DataPointChart chart, int i, int j)
        {
            var flaw = FlawsListBox.Rows[i].Cells[0].Value as PODListBoxItemWithProps;
            var response = ResponsesListBox.Rows[j].Cells[0].Value as PODListBoxItemWithProps;

            chart.XAxisTitle = flaw.GetColumnName();
            chart.XAxisUnit = flaw.Unit;

            chart.YAxisTitle = response.GetColumnName();
            chart.YAxisUnit = response.Unit;

            if (StackAll)
            {
                chart.Series[j].Name = response.GetColumnName();
            }
        }

        void chart_PostPaint(object sender, ChartPaintEventArgs e)
        {
            var chart = sender as DataPointChart;

            chart.BuildColorMap();
            ColorSelectedPoint(chart, _pointIndex, _seriesIndex);

            if (e.ChartElement == chart.Titles[0])
            {
                FixUpLegend(chart);
            }
        }

        private void FixUpLegend(DataPointChart chart)
        {
            if (StackAll)
            {
                var legend = chart.Legends[0];

                if (legend.CustomItems.Count == 0)
                {

                    foreach (Series series in chart.Series)
                    {
                        if (series.Name != "Selected")
                        {
                            var item = new LegendItem();
                            item.ImageStyle = LegendImageStyle.Marker;
                            item.MarkerStyle = MarkerStyle.Circle;
                            item.MarkerSize = 8;
                            item.MarkerBorderWidth = 0;
                            item.MarkerColor = chart.GetColor(series);
                            item.Cells.Add(LegendCellType.SeriesSymbol, "", ContentAlignment.MiddleCenter);
                            item.Cells.Add(LegendCellType.Text, series.Name, ContentAlignment.MiddleCenter);
                            legend.CustomItems.Add(item);
                        }
                    }

                    chart.Refresh();
                }
            }
        }

        void chart_MouseDown(object sender, MouseEventArgs e)
        {
            if (!HandleUserInteraction)
                return;

            DataPointChart chart = (DataPointChart)sender;

            HitTestResult result = chart.HitTest(e.X, e.Y);
            
            if (result.ChartElementType == ChartElementType.DataPoint)
            {
                _seriesIndex = chart.Series.IndexOf(result.Series);
                _pointIndex = result.PointIndex;

                if (_seriesIndex == chart.Series.IndexOf("Selected"))
                {
                    var point = result.Object as DataPoint;
                    _seriesIndex = Convert.ToInt32(point.GetCustomProperty("Original Series Index"));
                    _pointIndex = Convert.ToInt32(point.GetCustomProperty("Original Point Index"));
                }

                ColorSelectedPoint(_pointIndex);



                UpdateChartAndRelatedControls(chart);

                /*NotFilling = true;

                if(graphDataGridView[0, _pointIndex].Selected == true)
                    graphDataGridView_CellClick(graphDataGridView, new DataGridViewCellEventArgs(0, _pointIndex));
                else
                    graphDataGridView[0, _pointIndex].Selected = true;*/

            }
        }

        private void UpdateChartAndRelatedControls(DataPointChart chart)
        {
            var index = graphFlowPanel.Controls.IndexOf(chart);
            if (_stackAll == true)
            {
                int saveIndex = _pointIndex;
                UpdateListBoxSelections(_seriesIndex, index);
                _pointIndex = saveIndex;
            }
            graphDataGridView[0, _pointIndex].Selected = true;
            chart.SelectChart();
        }

        public void InitializeDataTable()
        {
            /*//ClearDataTable();

            var table = GetSpecTable();

            //if not enough rows
            while (table.Rows.Count > graphDataGridView.Rows.Count)
                graphDataGridView.Rows.Add();

            //iff too many rows
            while (graphDataGridView.Rows.Count > table.Rows.Count)
                graphDataGridView.Rows.RemoveAt(graphDataGridView.Rows.Count - 1);*/
        }

        private DataTable GetSpecTable()
        {
            if (_specTable == null)
                _specTable = _source.GetSpecimenData();

            return _specTable;
        }

        void chart_Click(object sender, EventArgs e)
        {
            DataPointChart chart = (DataPointChart)sender;
            Int32 index = 0;
            DataPointChart child;

            if (!HandleUserInteraction)
                return;

            foreach (Control control in graphFlowPanel.Controls)
            {
                if (control == chart)
                {
                    index = graphFlowPanel.Controls.IndexOf(chart);
                }
                else
                {
                    child = (DataPointChart)control;
                    child.IsSelected = false;
                    //child.BorderlineColor = Color.Transparent;
                    //child.BorderlineWidth = 0;
                    //child.Update();
                }
            }

            if (_stackAll == false)
            {
                var responseIndex = index % _source.ResponseLabels.Count;
                var flawIndex = index / _source.ResponseLabels.Count;

                UpdateListBoxSelections(responseIndex, flawIndex);
            }
            else
            {
                FlawsListBox.SingleSelectedIndex = index;
            }

            chart.Select();
            //FillDataTableWithSelectedData(index);
            //SelectChart(index);
        }

        private void UpdateListBoxSelections(int responseIndex, int flawIndex)
        {
            _updateSelection = false;
            ResponsesListBox.SingleSelectedIndex = responseIndex;
            _updateSelection = true;
            FlawsListBox.SingleSelectedIndex = flawIndex;
        }

        private int GetChartIndexFromSelectedListBoxes()
        {
            var index = ResponsesListBox.SingleSelectedIndex + (FlawsListBox.SingleSelectedIndex * ResponsesListBox.Rows.Count);
            return index;
        }

        private void SelectChart(Int32 index)
        {
            if (ResponsesListBox.Rows.Count > 0)
            {

                if (_stackAll == true)
                    index = index / ResponsesListBox.Rows.Count;

                if (index >= 0 && index < graphFlowPanel.Controls.Count)
                {
                    var chart2 = graphFlowPanel.Controls[index] as DataPointChart;

                    if (chart2.IsSelected == false)
                    {

                        foreach (Control control in graphFlowPanel.Controls)
                        {
                            var chart = control as DataPointChart;

                            if (chart != null && chart != chart2)
                                chart.IsSelected = false;
                        }
                        
                        chart2.Select();
                        chart2.SelectChart();
                    }

                    
                }

                
            }
        }

        private void FillDataTableWithSelectedData(Int32 index)
        {
            if (index >= 0)
            {
                var specs = GetSpecTable();

                var data = GetDataViews();

                var prevIndex = -1;

                if(graphDataGridView.SelectedRows.Count > 0)
                    prevIndex = graphDataGridView.SelectedRows[0].Index;

                if (index < data.Count)
                {
                    var lastIndex = 0;

                    if (graphDataGridView.SelectedRows.Count > 0)
                        lastIndex = graphDataGridView.SelectedRows[0].Index;

                    InitializeDataTable();

                    var view = data[index].Table.Copy();

                    if (!view.Columns.Contains("ID"))
                    {
                        view.Columns.Add("ID", typeof(string));
                        view.Columns["ID"].SetOrdinal(0);
                    }
                    
                    for (int i = 0; i < _specTable.Rows.Count; i++)
                    {
                        view.Rows[i][0] = specs.Rows[i][0].ToString();
                    }

                    view.AcceptChanges();

                    graphDataGridView.Columns.Clear();
                    graphDataGridView.AutoGenerateColumns = true;

                    graphDataGridView.DataSource = view.AsDataView();

                    graphDataGridView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    graphDataGridView.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    graphDataGridView.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                    if (graphDataGridView.Rows.Count > lastIndex)
                    {
                        graphDataGridView.Rows[lastIndex].Selected = true;
                        UpdateSelectedPointFromTable(lastIndex);
                    }

                    if (prevIndex >= 0 && prevIndex < graphDataGridView.Rows.Count)
                    {
                        graphDataGridView.Rows[prevIndex].Selected = true;
                        graphDataGridView.CurrentCell = graphDataGridView.Rows[prevIndex].Cells[0];
                    }

                    //StringBuilder builder = new StringBuilder();

                    /*graphDataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
                    graphDataGridView.RowHeadersVisible = false;
                    graphDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                    graphDataGridView.ColumnHeadersVisible = false;

                    for (int i = 0; i < _specTable.Rows.Count; i++)
                    {
                        var name = "";

                        for (int j = 0; j < _specTable.Columns.Count; j++ )
                        {
                            name += specs.Rows[i][j];

                            if(j < _specTable.Columns.Count-1)
                            {
                                name += "-";
                            }
                        }

                        graphDataGridView[0, i].Value = name;
                        graphDataGridView[1, i].Value = view.Table.Rows[i][0];
                        graphDataGridView[2, i].Value = view.Table.Rows[i][1];
                    }

                    graphDataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader;
                    graphDataGridView.RowHeadersVisible = false;
                    graphDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                    graphDataGridView.ColumnHeadersVisible = true;*/
                }
            }
        }

        private List<DataView> GetDataViews()
        {
            if (_views == null)
                _views = _source.GetAllGraphData();

            return _views;
        }

        public FlowLayoutPanel GraphsPanel
        {
            get
            {
                return graphFlowPanel;
            }
        }

        public void RefreshChartSelection()
        {
            //select first graph
            if (graphFlowPanel.Controls.Count > 0)
            {
                SelectChart(GetChartIndexFromSelectedListBoxes());
            }
        }

        

        public void ClearGraphs()
        {
            graphFlowPanel.Controls.Clear();
        }

        internal void ResizePanel()
        {
            graphFlowPanel_Resize(this, null);
        }

        public void SwitchViewMode()
        {
            _viewAll = !_viewAll;

            

            //graphFlowPanel.VerticalScroll.Enabled = !_stackAll & !_viewAll;
            //graphFlowPanel.VerticalScroll.Maximum = graphFlowPanel.Height;
            //graphFlowPanel.AutoScroll = !_stackAll;
        }

        public bool ViewMode
        {
            get
            {
                return _viewAll;
            }
        }

        public bool StackAll
        {
            get
            {
                return _stackAll;
            }
        }

        internal void SwitchStackMode()
        {
            _stackAll = !_stackAll;

            SelectChart(GetChartIndexFromSelectedListBoxes());

            //graphFlowPanel.VerticalScroll.Enabled = !_stackAll & !_viewAll;
            //graphFlowPanel.VerticalScroll.Maximum = graphFlowPanel.Height;
            //graphFlowPanel.AutoScroll = !_stackAll;
        }

        internal void ClearDataTable()
        {
            graphDataGridView.Rows.Clear();
        }

        private void FlawListBox_SelectionChanged(object sender, EventArgs e)
        {
            if (!HandleUserInteraction)
                return;

            var index = FlawsListBox.SingleSelectedIndex;

            FillFlawMetaDataControls(index);
        }

        private void FillFlawMetaDataControls(int index)
        {
            if (index >= 0)
            {
                var info = _info.GetInfos(ColType.Flaw)[index];
                FlawNameTextBox.Text = info.NewName;
                FlawUnitTextBox.Text = info.Unit;
                FlawMaximumNumeric.Value = Convert.ToDecimal(info.Max);
                FlawMinimumNumeric.Value = Convert.ToDecimal(info.Min);

                UpdateSelection();

                FlawsListBox.Select();

                //SelectNameForEdit(FlawNameTextBox);
            }
        }

        private void UpdateSelection()
        {
            if (_updateSelection)
            {
                if (graphDataGridView.Rows.Count > 0)
                {
                    _seriesIndex = ResponsesListBox.SingleSelectedIndex;
                    ColorSelectedPoint(_pointIndex);
                }

                var dataIndex = GetChartIndexFromSelectedListBoxes();
                FillDataTableWithSelectedData(dataIndex);
                SelectChart(dataIndex);
            }
        }

        private void SelectNameForEdit(TextBox myNameBox)
        {
            //select the name so the user can change the name without having to move the mouse
            //to the name box
            myNameBox.Select();
            myNameBox.SelectAll();
        }

        private void ResponseListBox_SelectionChanged(object sender, EventArgs e)
        {
            if (!HandleUserInteraction)
                return;

            var index = ResponsesListBox.SingleSelectedIndex;

            FillResponeMetaDataControls(index);
        }

        private void FillResponeMetaDataControls(int index)
        {
            if (index >= 0)
            {
                var info = _info.GetInfos(ColType.Response)[index];
                ResponseNameTextBox.Text = info.NewName;
                ResponseUnitTextBox.Text = info.Unit;
                ResponseMaximumNumeric.Value = Convert.ToDecimal(info.Max);
                ResponseMinimumNumeric.Value = Convert.ToDecimal(info.Min);
                ResponseThresholdNumeric.Value = Convert.ToDecimal(info.Threshold);

                UpdateSelection();

                ResponsesListBox.Select();
            }
        }

        private void UpdateListBox(PODListBox myList)
        {            
            myList.AutoResizeRows();
            myList.FitAllRows(6);
            myList.Refresh();

            UpdateChartLabels();
        }

        private void UpdateChartLabels()
        {
            var index = 0;

            foreach(Control control in graphFlowPanel.Controls)
            {
                var chart = control as DataPointChart;
                
                if(!_stackAll)
                {
                    var newIndex = index / ResponsesListBox.Rows.Count;

                    chart.XAxisUnit = _info.Units(ColType.Flaw)[newIndex];
                    chart.XAxisTitle = _info.NewNames(ColType.Flaw)[newIndex];

                    newIndex = index % ResponsesListBox.Rows.Count;

                    chart.YAxisUnit = _info.Units(ColType.Response)[newIndex];
                    chart.YAxisTitle = _info.NewNames(ColType.Response)[newIndex];
                }
                else
                {
                    var newIndex = index;

                    chart.XAxisUnit = _info.Units(ColType.Flaw)[newIndex];
                    chart.XAxisTitle = _info.NewNames(ColType.Flaw)[newIndex];
                }

                index++;
            }
        }

        private void Name_Changed(object sender, EventArgs e)
        {
            if (!HandleUserInteraction)
                return;

           var textBox = sender as TextBox;

            if(textBox != null && _updateSelection)
            {
                if (textBox == FlawNameTextBox)
                {
                    UpdateListBoxName(textBox, FlawsListBox, ColType.Flaw);
                }
                else if (textBox == ResponseNameTextBox)
                {
                    UpdateListBoxName(textBox, ResponsesListBox, ColType.Response);
                }

            }            

        }

        private void UpdateListBoxName(TextBox textBox, PODListBox myBox, ColType myType)
        {
            var index = myBox.SingleSelectedIndex;

            var actualName = _info.SetName(index, myType, textBox.Text);
            
            var pod = myBox.Rows[index].Cells[0].Value as PODListBoxItemWithProps;

            pod.SetStringValue(InfoType.NewName, actualName);

            if (textBox.Text.Trim() != actualName.Trim())
            {
                //ToolTipCtrl.Show(ToolTipCtrl.GetToolTip(textBox), textBox);
                //textBox.Text = actualName.Trim();
                //textBox.SelectionStart = textBox.TextLength;
                nameChecker.SetError(textBox, _illegalCharacterTooltip);
                nameChecker.SetIconPadding(textBox, 3);

                textBox.MaximumSize = new Size(ResponseUnitTextBox.Width, 0);
            }
            else
            {
                textBox.MaximumSize = new Size(textBox.Parent.Width, 0);
                nameChecker.SetError(textBox, "");
                //ToolTipCtrl.Hide(textBox);
            }

            UpdateListBox(myBox);
            
        }

        private void Unit_Changed(object sender, EventArgs e)
        {
            
            if (!HandleUserInteraction)
                return;

            var textBox = sender as TextBox;
            CheckBox checkBox = null;
            var indices = new List<int>();

            if (textBox != null && _updateSelection)
            {
                checkBox = GetCheckBox(textBox);                

                if (textBox == FlawUnitTextBox)
                {
                    UpdateListBoxString(textBox, checkBox, FlawsListBox, ColType.Flaw, InfoType.Unit);
                }
                else if (textBox == ResponseUnitTextBox)
                {
                    UpdateListBoxString(textBox, checkBox, ResponsesListBox, ColType.Response, InfoType.Unit);
                }
            }
        }

        private void UpdateListBoxString(TextBox textBox, CheckBox checkBox, PODListBox myBox, ColType myType, InfoType myDataType)
        {
            var indices = GetRowsToUpdate(checkBox, myBox);

            foreach (int index in indices)
            {
                var actualName = _info.SetStringProperty(index, myType, myDataType, textBox.Text);

                var pod = myBox.Rows[index].Cells[0].Value as PODListBoxItemWithProps;

                pod.SetStringValue(myDataType, actualName);

                if (index == myBox.SingleSelectedIndex && actualName != textBox.Text)
                    textBox.Text = actualName;
            }

            UpdateListBox(myBox);
        }

        private void UpdateListBoxDouble(PODNumericUpDown numBox, CheckBox checkBox, PODListBox myBox, ColType myType, InfoType myDataType)
        {
            var indices = GetRowsToUpdate(checkBox, myBox);
            var boxValue = Convert.ToDouble(numBox.Value);

            foreach (int index in indices)
            {
                var actualValue = _info.SetDoubleProperty(index, myType, myDataType, boxValue);

                var pod = myBox.Rows[index].Cells[0].Value as PODListBoxItemWithProps;

                pod.SetDoubleValue(myDataType, actualValue);

                if (index == myBox.SingleSelectedIndex && actualValue != boxValue)
                    numBox.Value = Convert.ToDecimal(actualValue);
            }

            UpdateListBox(myBox);
        }

        

        private void Minimun_Changed(object sender, EventArgs e)
        {
            if (!HandleUserInteraction)
                return;

            var numeric = sender as PODNumericUpDown;
            CheckBox checkBox = null;

            if (numeric != null && _updateSelection)
            {
                var newValue = Convert.ToDouble(numeric.Value);
                checkBox = GetCheckBox(numeric);

                if (numeric == FlawMinimumNumeric)
                {
                    UpdateListBoxDouble(numeric, checkBox, FlawsListBox, ColType.Flaw, InfoType.Min);
                }
                else if(numeric == ResponseMinimumNumeric)
                {
                    UpdateListBoxDouble(numeric, checkBox, ResponsesListBox, ColType.Response, InfoType.Min);
                }
            }
        }

        private void Maximum_Changed(object sender, EventArgs e)
        {
            if (!HandleUserInteraction)
                return;

            var numeric = sender as PODNumericUpDown;
            CheckBox checkBox = null;

            if (numeric != null && _updateSelection)
            {
                var newValue = Convert.ToDouble(numeric.Value);
                checkBox = GetCheckBox(numeric);

                if (numeric == FlawMaximumNumeric)
                {
                    UpdateListBoxDouble(numeric, checkBox, FlawsListBox, ColType.Flaw, InfoType.Max);
                }
                else if (numeric == ResponseMaximumNumeric)
                {
                    UpdateListBoxDouble(numeric, checkBox, ResponsesListBox, ColType.Response, InfoType.Max);
                }
            }
        }

        private void Threshold_Changed(object sender, EventArgs e)
        {
            if (!HandleUserInteraction)
                return;

            var numeric = sender as PODNumericUpDown;
            CheckBox checkBox = null;

            if (numeric != null && _updateSelection)
            {
                var newValue = Convert.ToDouble(numeric.Value);
                checkBox = GetCheckBox(numeric);

                if (numeric == ResponseThresholdNumeric)
                {
                    UpdateListBoxDouble(numeric, checkBox, ResponsesListBox, ColType.Response, InfoType.Threshold);
                }
            }
        }

        private List<int> GetRowsToUpdate(CheckBox checkBox, PODListBox listBox)
        {
            var list = new List<int>();

            if (!checkBox.Checked)
                list.Add(listBox.SingleSelectedIndex);
            else
            {
                for (int i = 0; i < listBox.Rows.Count; i++)
                    list.Add(i);
            }

            return list;
        }



        internal void UpdateSourceFromInfo()
        {
            _info.UpdateDataSource(_source);
        }

        private void ListBox_Clicked(object sender, EventArgs e)
        {
            if (!HandleUserInteraction)
                return;

            var box = sender as PODListBox;

            if(box != null)
            {
                if (box == ResponsesListBox)
                    SelectNameForEdit(ResponseNameTextBox);
                else if (box == FlawsListBox)
                    SelectNameForEdit(FlawNameTextBox);
            }
            
        }
        
        private void TextBox_MouseDown(object sender, MouseEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox != null && _activeBefore != textBox)
            {
                SelectNameForEdit(textBox);
                _activeBefore = textBox;
            }
        }

        private void Numeric_MouseDown(object sender, MouseEventArgs e)
        {
            if (!HandleUserInteraction)
                return;

            var numeric = sender as PODNumericUpDown;

            if (numeric != null && _activeBefore != numeric)
            {
                if (ActiveControl != numeric)
                {
                    numeric.Select(0, numeric.Text.Length);
                    _activeBefore = numeric;
                }
            }
                        
        }

        private void FlawsName_KeyDown(object sender, KeyEventArgs e)
        {
            if (!HandleUserInteraction)
                return;

            HandleNameTextBoxKeyPresses(FlawNameTextBox, FlawsListBox, e);
        }

        private void ResponseName_KeyDown(object sender, KeyEventArgs e)
        {
            if (!HandleUserInteraction)
                return;

            HandleNameTextBoxKeyPresses(ResponseNameTextBox, ResponsesListBox, e);
        }

        private void HandleNameTextBoxKeyPresses(TextBox myTextBox, PODListBox myListBox, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                if (myListBox.SingleSelectedIndex > 0)
                    myListBox.SingleSelectedIndex--;
                else
                    myListBox.SingleSelectedIndex = myListBox.RowCount - 1;

                ScrollToCenter(myListBox);

                myTextBox.Select();
                myTextBox.SelectAll();
            }
            else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                if (myListBox.SingleSelectedIndex < myListBox.RowCount - 1)
                    myListBox.SingleSelectedIndex++;
                else
                    myListBox.SingleSelectedIndex = 0;

                ScrollToCenter(myListBox);

                myTextBox.Select();
                myTextBox.SelectAll();


            }
        }

        //code by TrevorSB (https://social.msdn.microsoft.com/profile/trevorsb/?ws=usercard-mini)
        //taken from https://social.msdn.microsoft.com/Forums/windows/en-us/7db968a9-38be-45fc-9dbf-e283f39d9e8c/datagridview-scrolling-to-selected-row
        private void ScrollToCenter(PODListBox myListBox)
        {
            myListBox.FirstDisplayedScrollingRowIndex = myListBox.SingleSelectedIndex;

            int halfWay = (myListBox.DisplayedRowCount(false) / 2);
            if (myListBox.FirstDisplayedScrollingRowIndex + halfWay > myListBox.SelectedRows[0].Index ||
                (myListBox.FirstDisplayedScrollingRowIndex + myListBox.DisplayedRowCount(false) - halfWay) <= myListBox.SelectedRows[0].Index)
            {
                int targetRow = myListBox.SelectedRows[0].Index;
                
                targetRow = Math.Max(targetRow-halfWay, 0);
                myListBox.FirstDisplayedScrollingRowIndex = targetRow;
            }
        }

        

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {

            //Se apertou o enter
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

            }

        }

        private const int TVM_GETEXTENDEDSTYLE = 0x1100 + 45;
        private const int TVM_SETEXTENDEDSTYLE = 0x1100 + 44;
        private const int TVS_EX_DOUBLEBUFFER = 0x4;

        private const int WM_SETREDRAW = 11;

        private int suspendCounter = 0;
        private Control _activeBefore;
        /// <summary>
        /// Should updating the selected index update the table and selected chart?
        /// </summary>
        private bool _updateSelection;
        private string _illegalCharacterTooltip;
        public int InputTableSplitterSize
        {
            get
            {
                return InputTableSplitter.SplitterDistance;
            }
        }
        public int TableGraphsSplitterSize
        {
            get
            {
                return TableGraphSplitter.SplitterDistance;
            }
        }

        /// <summary>
        /// Sends low level message to control.
        /// </summary>
        /// <param name="hWnd">window handle</param>
        /// <param name="wMsg">message</param>
        /// <param name="wParam">parameter 1</param>
        /// <param name="lParam">parameter 2</param>
        /// <returns>result</returns>
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);
        /// <summary>
        /// Resume the textbox's drawing.
        /// </summary>
        public void ResumeDrawing()
        {
            suspendCounter--;
            if (suspendCounter == 0)
            {
                SendMessage(this.Handle, WM_SETREDRAW, true, 0);
                this.Refresh();
            }
        }

        /// <summary>
        /// Suspend the textbox's drawing.
        /// </summary>
        public void SuspendDrawing()
        {
            if (suspendCounter == 0)
                SendMessage(this.Handle, WM_SETREDRAW, false, 0);
            suspendCounter++;
        }

        /// <summary>
        /// Forces double buffer.
        /// </summary>
        /// <param name="e">Default event args.</param>
        protected override void OnHandleCreated(EventArgs e)
        {
            SendMessage(Handle, TVM_SETEXTENDEDSTYLE, false, TVS_EX_DOUBLEBUFFER);
            base.OnHandleCreated(e);
        }

        private void GraphTableSplitter_Moved(object sender, SplitterEventArgs e)
        {
            graphFlowPanel_Resize(graphFlowPanel, null);
        }

        internal void SetSplitterSizes(int inputTableSize, int tableGraphSize)
        {
            InputTableSplitter.SplitterDistance = inputTableSize;
            TableGraphSplitter.SplitterDistance = tableGraphSize;
        }



        public bool HandleUserInteraction { get; set; }

        public void RefreshDataTable()
        {
            InitializeDataTable();

            //force latest data into grid
            _specTable = _source.GetSpecimenData();
            _views = _source.GetAllGraphData();

            FillDataTable();
        }

        internal void FillDataTable()
        {
            FillDataTableWithSelectedData(ResponsesListBox.SingleSelectedIndex);
        }

        private void MouseHover_Control(object sender, EventArgs e)
        {
            var control = sender as Control;

            if (control != null)
            {
                //StepToolTip.Show(StepToolTip.GetToolTip(control), control);
                //StepToolTip.Hide(control);
                StepToolTip.Active = false;
                StepToolTip.Active = true;
            }
        }
    }
}
