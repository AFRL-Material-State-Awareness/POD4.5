using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using POD.Data;

namespace POD.Controls
{
    public enum ControlLine
    {
        AMax,
        AMin,
        LeftCensor,
        RightCensor,
        Threshold
    }

    public delegate void LinesChangedEventHandler(object sender, EventArgs e);

    public partial class RegressionAnalysisChart : DataPointChart
    {
        private const double FLOATING_POINT_EQ_THRESHOLD = 0.01d;

        private Dictionary<string, string> _originalSeriesNames = new Dictionary<string, string>();

        
        protected VerticalLineAnnotation _aMaxLine = null;
        protected StripLine _aMaxStrip;
        protected VerticalLineAnnotation _aMinLine = null;
        protected StripLine _aMinStrip;
        protected AnalysisData _analysisData;
        protected DataTable _flaws;
        protected DataTable _names;
        protected StripLine _leftCensorStrip;
        protected DataTable _metadatas;
        protected DataTable _responses;
        protected StripLine _rightCensorStrip;
        protected DataTable _specimenIDs;
        private bool _thresholdFreeze;
        private double _thresholdFreezeValue;
        protected HorizontalLineAnnotation _thresholdLine = null;
        private bool _painted;
        public event LinesChangedEventHandler LinesChanged;
        public event EventHandler RunAnalysisNeeded;
        protected TextAnnotation _equation;
        private PolygonAnnotation _equBox;
        //private AnnotationGroup _equGroup;
        private bool madeLineBigger;
        private LineAnnotation madeWhichOneBigger;
        private string _positionString;
        private Color _boxColor;
        private int _lastX;
        private int _lastY;
        protected System.Windows.Forms.ContextMenuStrip _lastMenu;
        private bool NoMovingLines;
        
        private List<HitTestResult> hittestresults = new List<HitTestResult>(5);

        public RegressionAnalysisChart()
        {
            
            InitializeComponent();

            Selectable = false;

            AutoNameYAxis = false;

            YAxisTitle = _yAxisTitle;

            ShowChartTitle = false;

            PostPaint += RegressionAnalysisChart_PostPaint;
            MouseClick += RegressionAnalysisChart_MouseClick;
            MouseUp += RegressionAnalysisChart_MouseUp;
            MouseDoubleClick += RegressionAnalysisChart_MouseDoubleClick;
            MouseMove += RegressionAnalysisChart_MouseMove;
            MouseDown += RegressionAnalysisChart_MouseDown;
            Resize += RegressionAnalysisChart_Resize;
            ResizeRedraw = true;
            AnnotationPositionChanging += Annotation_Moving;
            MouseLeave += RegressionAnalysisChart_MouseLeave;

            AnnotationPositionChanged += RegressionAnalysisChart_AnnotationPositionChanged;

            DoubleBuffered = true;

            for (int i = 0; i < 5; i++ )
                hittestresults.Add(new HitTestResult());
            GetToolTipText += RegressionAnalysisChart_GetToolTipText;

            if(!DesignMode)
                ShowLegend();

            if(!DesignMode)
                AddEmptySeriesToForceDraw();
        }

        void RegressionAnalysisChart_MouseLeave(object sender, EventArgs e)
        {
            foreach(Annotation anno in Annotations)
            {
                var line = anno as LineAnnotation;

                if(line != null)
                {
                    line.Width = 3;
                }
            }
        }

        

        private void RemoveEmptySeries()
        {
            if (Series.FindByName("Empty") != null)
                Series.Remove(Series["Empty"]);
        }

        

        

        public override void ClearEverythingButPoints()
        {

            base.ClearEverythingButPoints();

            Series[PODRegressionLabels.a50Line].Points.Clear();
            Series[PODRegressionLabels.a90Line].Points.Clear();
            Series[PODRegressionLabels.a9095Line].Points.Clear();
            Series[PODRegressionLabels.BestFitLine].Points.Clear();
        }

        protected void Annotation_Moved(object sender, EventArgs e)
        {
            var moved = (Annotation) sender;

            Axis xAxis = ChartAreas[0].AxisX;
            Axis yAxis = ChartAreas[0].AxisY;

            if (moved.X < xAxis.Minimum)
            {
                moved.X = xAxis.Minimum;
            }
            else if (moved.X > xAxis.Maximum)
            {
                moved.X = xAxis.Maximum;
            }

            if (moved.Y < yAxis.Minimum)
            {
                moved.Y = yAxis.Minimum;
            }
            else if (moved.Y > yAxis.Maximum)
            {
                moved.Y = yAxis.Maximum;
            }

            OnLinesChanged(EventArgs.Empty);
        }

        protected void Annotation_Moving(object sender, AnnotationPositionChangingEventArgs e)
        {
            Axis xAxis = ChartAreas[0].AxisX;
            Axis yAxis = ChartAreas[0].AxisY;

            if (e.NewLocationX < xAxis.Minimum)
            {
                e.NewLocationX = xAxis.Minimum;
            }
            else if (e.NewLocationX > xAxis.Maximum)
            {
                e.NewLocationX = xAxis.Maximum;
            }

            if (e.NewLocationY < yAxis.Minimum)
            {
                e.NewLocationY = yAxis.Minimum;
            }
            else if (e.NewLocationY > yAxis.Maximum)
            {
                e.NewLocationY = yAxis.Maximum;
            }

            OnLinesChanged(EventArgs.Empty);
        }
                
        void RegressionAnalysisChart_GetToolTipText(object sender, ToolTipEventArgs e)
        {
            if(e.HitTestResult.ChartElementType == ChartElementType.DataPoint)
            {
                //e.Text = "5";

                
            }
        }

        public virtual List<ToolStripItem> BuildMenuItems(double x, double y)
        {
            var setAMax = new ToolStripMenuItem("Set Flaw Max. here");

            

            setAMax.Click += (sender, e) => SetAMaxBoundaryMenu(sender, e, new DataPoint(x, y));

            var setAMin = new ToolStripMenuItem("Set Flaw Min. here");

            setAMin.Click += (sender, e) => SetAMinBoundaryMenu(sender, e, new DataPoint(x, y));

            if (ContextMenuImageList != null)
            {
                setAMax.Image = ContextMenuImageList.Images[5];
                setAMin.Image = ContextMenuImageList.Images[4];
            }

            var menuItems = new List<ToolStripItem>
            {
                //new ToolStripSeparator(),
                setAMax,
                setAMin
            };

            return menuItems;
        }

        //public new bool CanUnselect { get; set; }

        protected List<ContextMenuStrip> CreatePointMenu(double x, double y, bool isDataPoint = false, string mySeriesName = "", string myName = "", int myPointIndex = -1, Color myColor = default(Color), FlowLayoutPanel panel = null, int rowIndex = -1, int colIndex = -1)
        {
            List<ToolStripItem> actionMenuItems = BuildMenuItems(x, y);
            List<ToolStripItem> pointMenuItems = new List<ToolStripItem>();


            if (isDataPoint)
            {
                var responseToggle = new ToolStripMenuItem("Toggle Response On/Off");

                responseToggle.Image = ToggleImageList.Images[0];

                responseToggle.Click += (sender, e) => ToggleResponseMenu(sender, e, new DataPoint(x, y), mySeriesName, rowIndex, colIndex);

                var allResponsesToggle = new ToolStripMenuItem("Toggle All Responses");

                allResponsesToggle.Image = ToggleImageList.Images[1];

                allResponsesToggle.Click += (sender, e) => ToggleAllResponsesMenu(sender, e, new DataPoint(x, y));

                actionMenuItems.Add(responseToggle);
                actionMenuItems.Add(allResponsesToggle);
            }

            if (_positionString == null)
                _positionString = "";

            if(_positionString.Length > 0)
            { 
                var labels = _positionString.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                labels = labels.Reverse().ToArray();

                foreach (string label in labels)
                {
                    var dataPointLabel = new ToolStripMenuItem(label);

                    if (label == labels.Last())
                    {
                        var bitmap = new Bitmap(20, 20);

                        Graphics g = Graphics.FromImage(bitmap);

                        g.FillRectangle(new SolidBrush(myColor), 0, 0, 19, 19);

                        dataPointLabel.Image = bitmap;
                        dataPointLabel.ImageScaling = ToolStripItemImageScaling.SizeToFit;
                    }

                    //dataPointLabel.Enabled = false;
                    //dataPointLabel.Paint += dataPointLabel_Paint;

                    if (panel != null)
                    {
                        var control = new Label();
                        control.Text = dataPointLabel.Text;
                        control.AutoSize = true;
                        control.Click += control_Click;
                        panel.Controls.Add(control);
                        panel.Controls.SetChildIndex(control, 0);
                    }

                    pointMenuItems.Insert(0, dataPointLabel);
                }
            

                if (panel != null)
                {
                    panel.Click += panel_Click;
                    if (panel.Controls.Count > 2)
                    {
                        panel.Controls[0].Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
                        panel.Controls[1].Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
                        panel.Controls[2].Margin = new System.Windows.Forms.Padding(2, 0, 2, 4);
                    }
                    else
                    {
                        panel.Controls[0].Margin = new System.Windows.Forms.Padding(2, 2, 2, 4);
                    }
                }
            }

            //menuItems1.Add(new ToolStripSeparator());
            

            var actionMenu = new ContextMenuStrip();
            var pointMenu = new ContextMenuStrip();

            actionMenu.Items.AddRange(actionMenuItems.ToArray());
            pointMenu.Items.AddRange(pointMenuItems.ToArray());

            var menus = new List<ContextMenuStrip>(new ContextMenuStrip[] { actionMenu, pointMenu });

            

            return menus;
        }

        void control_Click(object sender, EventArgs e)
        {
            var control = sender as Control;

            var contextMenu = control.Parent.Parent as ContextMenuStrip;

            if(_lastMenu != null)
            {
                _lastMenu = null;
            }

            contextMenu.Close();
        }

        void panel_Click(object sender, EventArgs e)
        {
            var panel = sender as Panel;

            var contextMenu = panel.Parent as ContextMenuStrip;

            contextMenu.Close();
        }

        void dataPointLabel_Paint(object sender, PaintEventArgs e)
        {
            ToolStripMenuItem menu = sender as ToolStripMenuItem;

            if (menu != null)
                menu.ForeColor = Color.Black;
        }

        public void DeterminePointsInThreshold()
        {
            MoveLine(_aMaxLine, _aMinLine);

            
        }

        public void DrawEquation()
        {
            if(_equation == null)
            {
                //FindForm().SizeChanged += RegressionAnalysisChart_Resize;

                //_equGroup = new AnnotationGroup();

                _equBox = new PolygonAnnotation();

                _equBox.Name = "EquationBox";
                _equBox.BackColor = Color.White;
                _equBox.ForeColor = Color.Black;

                _equBox.GraphicsPath.AddRectangle(new Rectangle(0, 0, 100, 100));
                _equBox.AxisX = XAxis;
                _equBox.AxisY = YAxis;
                _equBox.Alignment = ContentAlignment.TopLeft;

                _equation = new TextAnnotation();
                _equation.Name = "EquationName";
                _equation.AxisX = XAxis;
                _equation.AxisY = YAxis;
                _equation.Alignment = ContentAlignment.TopLeft;
                //_equation.X = double.NaN;
                //_equation.Y = double.NaN;
                _equation.ForeColor = Color.Black;
                _equation.Font = new Font("Arial", 14);
                //_equation.Visible = false;

                //Annotations.Insert(0, _equation);
                Annotations.Add(_equBox);
                Annotations.Add(_equation);
            }
        }

        /*protected void UpdateEquationLocation(ChartPaintEventArgs e)
        {
            if (_equation != null)
            {
                System.Drawing.Font drawFont = new System.Drawing.Font(_equation.Font, FontStyle.Regular);
                System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);

                e.ChartGraphics.Graphics.DrawString(_equation.Text, drawFont, drawBrush, new PointF(100.0F, 100.0F));
            }
        }*/

        protected void UpdateEquationLocation(ChartPaintEventArgs e)
        {
            if (_equation != null)
            {
                var chartArea = ChartAreas[0];

                double heightPixel = 100.0 / Height;
                double widthPixel = 100.0 / Width;

                if (Width > 100)
                {

                    double hPercent = 25 * heightPixel;
                    double wPercent = 25 * widthPixel;

                    var plotWidthStart = chartArea.AxisX.ValueToPosition(chartArea.AxisX.Minimum);
                    var plotHeightStart = chartArea.AxisY.ValueToPosition(chartArea.AxisY.Maximum);

                    var newHeight = plotHeightStart + hPercent;

                    if (newHeight > 100.0)
                        newHeight = 100.0;

                    var newWidth = plotWidthStart + wPercent;

                    if (newWidth > 100.0)
                        newWidth = 100.0;

                    var xValue = chartArea.AxisX.PositionToValue(newWidth);
                    var yValue = chartArea.AxisY.PositionToValue(newHeight);

                    if (_equation.X != xValue)
                    {
                        _equation.X = xValue;
                        _equBox.X = xValue;// -(widthPixel * .5);
                    }

                    if (_equation.Y != yValue)
                    {
                        _equation.Y = yValue;
                        _equBox.Y = yValue;// -(heightPixel * .5);
                    }

                    _equation.Text = _equation.Text;

                    _equation.ResizeToContent();

                    _equBox.Width = _equation.Width - (widthPixel * 1.0);
                    _equBox.Height = _equation.Height;
                }

                //var height = (60d / Height) * 100;
                //var width = (400d / Width) * 100;

                //if(_equation.Width != width)
                //   _equation.Width = width;
                //if(_equation.Height != height)
                //    _equation.Height = height;
            }
        }

        public void DrawAMaxBoundLine()
        {
            if (_aMaxLine == null)
            {
                _aMaxLine = new VerticalLineAnnotation();

                Axis xAxis = ChartAreas[0].AxisX;
                Axis yAxis = ChartAreas[0].AxisY;

                var x = GetInitialAMaxLocationFromData();

                _aMaxLine.AxisX = xAxis;
                _aMaxLine.AxisY = yAxis;
                _aMaxLine.IsSizeAlwaysRelative = false;
                _aMaxLine.AllowMoving = true;
                //_aMaxLine.AnchorX = x;
                _aMaxLine.X = x;
                _aMaxLine.IsInfinitive = true;
                _aMaxLine.ClipToChartArea = ChartAreas[0].Name;
                _aMaxLine.LineColor = Color.FromArgb(ChartColors.LineAlpha, ChartColors.aMaxColor);
                _aMaxLine.LineDashStyle = ChartDashStyle.Dash;
                _aMaxLine.LineWidth = 3;

                Annotations.Add(_aMaxLine);
            }
        }

        public void DrawAMinBoundLine()
        {
            if (_aMinLine == null)
            {
                _aMinLine = new VerticalLineAnnotation();

                Axis xAxis = ChartAreas[0].AxisX;
                Axis yAxis = ChartAreas[0].AxisY;

                var x = GetInitialAMinLocationFromData();

                _aMinLine.AxisX = xAxis;
                _aMinLine.AxisY = yAxis;
                _aMinLine.IsSizeAlwaysRelative = false;
                _aMinLine.AllowMoving = true;
                //_aMinLine.AnchorX = x;
                _aMinLine.X = x;
                _aMinLine.IsInfinitive = true;
                _aMinLine.ClipToChartArea = ChartAreas[0].Name;
                _aMinLine.LineColor = Color.FromArgb(ChartColors.LineAlpha, ChartColors.aMinColor);
                _aMinLine.LineDashStyle = ChartDashStyle.Dash;
                _aMinLine.LineWidth = 3;

                Annotations.Add(_aMinLine);
            }
        }

        private double GetInitialAMaxLocationFromData()
        {
            var yValues =
                   (from DataRow row in this._analysisData.ActivatedFlaws.Rows
                    from obj in row.ItemArray
                    select Convert.ToDouble(obj)).ToList();

            double y = 1;

            if (yValues.Count > 0)
            {
                y = yValues.Max();
                double range = y - yValues.Min();
                double buffer = range * .05;

                y += buffer;
            }

            return y;
        }

        private double GetInitialAMinLocationFromData()
        {
            var yValues =
                    (from DataRow row in this._analysisData.ActivatedFlaws.Rows
                     from obj in row.ItemArray
                     select Convert.ToDouble(obj)).ToList();


            double y = 0;

            if (yValues.Count > 0)
            {
                y = yValues.Min();
                double range = yValues.Max() - y;
                double buffer = range * .05;

                y -= buffer;
            }

            return y;
        }

        public virtual void DrawBoundaryLines()
        {
            DrawAMinBoundLine();
            DrawAMaxBoundLine();
            DrawThresholdBoundLine();
            
        }

        public void DrawThresholdBoundLine()
        {
            if (_thresholdLine == null)
            {
                _thresholdLine = new HorizontalLineAnnotation();

                Axis xAxis = ChartAreas[0].AxisX;
                Axis yAxis = ChartAreas[0].AxisY;

                double y = yAxis.Maximum/4;

                double x = xAxis.Maximum;
                //x = Math.Pow(10, Math.Ceiling(Math.Log10(x)));

                _thresholdLine.AxisX = xAxis;
                _thresholdLine.AxisY = yAxis;
                _thresholdLine.IsSizeAlwaysRelative = false;
                _thresholdLine.AllowMoving = true;
                //_thresholdLine.AnchorY = y;
                _thresholdLine.Y = y;
                _thresholdLine.IsInfinitive = true;
                _thresholdLine.ClipToChartArea = ChartAreas[0].Name;
                _thresholdLine.LineColor = Color.FromArgb(ChartColors.LineAlpha, ChartColors.ThresholdColor);
                _thresholdLine.LineDashStyle = ChartDashStyle.Dash;
                _thresholdLine.LineWidth = 3;

                Annotations.Add(_thresholdLine);
            }

            if (_thresholdFreeze)
            {
                _thresholdLine.AllowMoving = false;
                _thresholdLine.Y = _thresholdFreezeValue;
                _thresholdLine.Width = 2;
            }
        }

        

        

        public virtual bool FindValue(ControlLine line, ref double myValue)
        {
            double value;
            double anchorValue;
            bool foundLine = true;

            switch (line)
            {
                case ControlLine.AMin:
                    value = _aMinLine.X;
                    anchorValue = _aMinLine.AnchorX;
                    break;
                case ControlLine.AMax:
                    value = _aMaxLine.X;
                    anchorValue = _aMaxLine.AnchorX;
                    break;
                case ControlLine.Threshold:
                    value = _thresholdLine.Y;
                    anchorValue = _thresholdLine.AnchorY;
                    break;
                default:
                    value = Double.NaN;
                    anchorValue = Double.NaN;
                    foundLine = false;
                    break;
            }

            myValue = Double.IsNaN(value) ? anchorValue : value;

            return foundLine;
        }

        private void FixColor(int seriesIndex, int seriesPtIndex, Flag bounds)
        {
            if (IsDataSeriesInRange(seriesIndex) && IsPointIndexInRange(seriesIndex, seriesPtIndex))
            {
                switch (bounds)
                {
                    case Flag.InBounds:

                        if (_colorMap.Count > 0)
                        {
                            var color = _colorMap[Series[seriesIndex].Name];

                            if(color == Color.Gray)
                            {

                            }

                            var point = Series[seriesIndex].Points[seriesPtIndex];
                            if(!point.IsEmpty)
                                point.Color = color;
                        }
                        break;
                    case Flag.OutBounds:
                        {
                            var point = Series[seriesIndex].Points[seriesPtIndex];
                            if (!point.IsEmpty)
                                Series[seriesIndex].Points[seriesPtIndex].Color = Color.Gray;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("bounds must be either InBounds or OutBounds");
                }
            }
        }

        private bool IsPointIndexInRange(int seriesIndex, int seriesPtIndex)
        {
            return (seriesPtIndex >= 0 && seriesPtIndex < Series[seriesIndex].Points.Count);
        }

        private bool IsDataSeriesInRange(int seriesIndex)
        {
            return (seriesIndex >= DataSeriesStartIndex && seriesIndex <= DataSeriesEndIndex);
        }

        public void FreezeThresholdLine(double myThresholdValue)
        {
            _thresholdFreezeValue = myThresholdValue;
            _thresholdFreeze = true;
        }

        /*public new bool IsSelected
        {
            get { return isSelected; }

            set
            {
                if (Selectable)
                {
                    if (mouseInside == false)
                    {
                        isSelected = value;

                        if (isSelected == false)
                            BorderlineColor = Color.Transparent;
                    }
                }
                else
                {
                    BorderlineColor = Color.FromKnownColor(KnownColor.ControlDark);
                }
            }
        }*/

        /*public new bool IsSquare { get; set; }*/

        private void KeepAMinMaxInOrder()
        {
            if(_aMinLine == null)
                DrawAMinBoundLine();

            if(_aMaxLine == null)
                DrawAMaxBoundLine();

            if (_aMinLine.X > _aMaxLine.X)
            {
                double temp = _aMinLine.X;
                _aMinLine.X = _aMaxLine.X;
                _aMaxLine.X = temp;
            }
        }

        protected virtual void KeepLinesInOrder()
        {
            KeepAMinMaxInOrder();
        }

        public void LoadChartData(AnalysisData data)
        {
            _flaws = data.ActivatedFlaws;
            _responses = data.ActivatedResponses;
            _names = data.ActivatedSpecimenIDs;
            _analysisData = data;

            SingleSeriesCount = 5; //9095 line, 90 line, 50 line, fit line, series 1

            

            _analysisData.CreateNewSortList();

            if (Series.Count <= 1)
                Series.Clear();

            Series series;

            if (Series.FindByName(PODRegressionLabels.a9095Line) == null)
            {
                //add a90_95 line series
                Series.Add(PODRegressionLabels.a9095Line);
                series = Series[PODRegressionLabels.a9095Line];
                series.ChartType = SeriesChartType.Line;
                series.IsXValueIndexed = false;
                series.YValuesPerPoint = 1;
                series.BorderWidth = 3;
                series.BorderDashStyle = ChartDashStyle.Dot;
                series.Color = Color.FromArgb(ChartColors.LineAlpha, ChartColors.a9095Color);
                series.SmartLabelStyle.MovingDirection = LabelAlignmentStyles.TopRight;
                series.SmartLabelStyle.CalloutStyle = LabelCalloutStyle.Box;
                series.SmartLabelStyle.CalloutBackColor = Color.White;
                series.SmartLabelStyle.CalloutLineAnchorCapStyle = LineAnchorCapStyle.None;
                series.IsVisibleInLegend = false;
            }

            if (Series.FindByName(PODRegressionLabels.a90Line) == null)
            {
                //add a90 line series
                Series.Add(PODRegressionLabels.a90Line);
                series = Series[PODRegressionLabels.a90Line];
                series.ChartType = SeriesChartType.Line;
                series.IsXValueIndexed = false;
                series.YValuesPerPoint = 1;
                series.BorderWidth = 3;
                series.BorderDashStyle = ChartDashStyle.Dot;
                series.Color = Color.FromArgb(ChartColors.LineAlpha, ChartColors.a90Color);
                series.SmartLabelStyle.MovingDirection = LabelAlignmentStyles.TopRight;
                series.SmartLabelStyle.CalloutStyle = LabelCalloutStyle.Box;
                series.SmartLabelStyle.CalloutBackColor = Color.White;
                series.SmartLabelStyle.CalloutLineAnchorCapStyle = LineAnchorCapStyle.None;
                series.IsVisibleInLegend = false;
            }

            if (Series.FindByName(PODRegressionLabels.a50Line) == null)
            {
                //add a50 line series
                Series.Add(PODRegressionLabels.a50Line);
                series = Series[PODRegressionLabels.a50Line];
                series.ChartType = SeriesChartType.Line;
                series.IsXValueIndexed = false;
                series.YValuesPerPoint = 1;
                series.BorderWidth = 3;
                series.BorderDashStyle = ChartDashStyle.Dot;
                series.Color = Color.FromArgb(ChartColors.LineAlpha, ChartColors.a50Color);
                series.SmartLabelStyle.MovingDirection = LabelAlignmentStyles.TopRight;
                series.SmartLabelStyle.CalloutStyle = LabelCalloutStyle.Box;
                series.SmartLabelStyle.CalloutBackColor = Color.White;
                series.SmartLabelStyle.CalloutLineAnchorCapStyle = LineAnchorCapStyle.None;
                series.IsVisibleInLegend = false;
            }

            var a50Index = Series.IndexOf(PODRegressionLabels.a50Line);

            //get rid of all previous series
            if(Series.FindByName(PODRegressionLabels.BestFitLine) != null)
            {
                var seriesList = new List<Series>();

                var bestFitIndex = Series.IndexOf(PODRegressionLabels.BestFitLine);

                if (a50Index + 1 < bestFitIndex)
                {
                    for (int i = a50Index + 1; i < bestFitIndex; i++)
                    {
                        seriesList.Add(Series[i]);
                    }

                    while (seriesList.Count > 0)
                    {
                        var deleteThis = seriesList[0];
                        Series.Remove(deleteThis);
                        seriesList.Remove(deleteThis);
                    }

                    //reset the color map
                    _colorMap.Clear();
                    _painted = false;
                }
            }
            
            foreach (string name in data.ActivatedResponseNames)
            {
                Series.Insert(a50Index+1, new Series(name));                    
                Series[name].ChartType = SeriesChartType.Point;
                Series[name].MarkerSize = 9;
                a50Index++;
            }

            if (data.ActivatedResponseNames.Count == 0 || data.RowCount == 0)
            {
                AddEmptySeriesToForceDraw();
            }
            else
            {
                RemoveEmptySeries();
            }


            //add best fit line series
            if (Series.FindByName(PODRegressionLabels.BestFitLine) == null)
            {
                Series.Add(PODRegressionLabels.BestFitLine);
                series = Series[PODRegressionLabels.BestFitLine];
                series.ChartType = SeriesChartType.Line;
                series.IsXValueIndexed = false;
                series.YValuesPerPoint = 1;
                series.BorderWidth = 3;
                series.Color = Color.FromArgb(ChartColors.LineAlpha, ChartColors.FitColor);
                series.IsVisibleInLegend = false;
            }

            SetupStripLines();

            AutoNameYAxis = true;

            XAxisTitle = data.AvailableFlawNames[0];
            XAxisUnit = data.AvailableFlawUnits[0];
            YAxisTitle = data.ActivatedResponseNames[0];
            YAxisUnit = data.AvailableResponseUnits[0];

            ChartAreas[0].AxisX.IsLogarithmic = false;
            //this.ChartAreas[0].AxisX.MinorTickMark.Interval = 1;
            //this.ChartAreas[0].AxisX.MinorTickMark.Enabled = true;
            //this.ChartAreas[0].AxisX.LabelStyle.Interval = 1;
            ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            ChartAreas[0].AxisY.MajorGrid.Enabled = false;

            ReloadChartData(_flaws.Rows, _responses, _names);

            //CleanUpDataSeries();

            ForceIncludedPointsUpdate();
        }

        public void ForceReloadChartData()
        {
            _flaws = _analysisData.ActivatedFlaws;
            _responses = _analysisData.ActivatedResponses;

            ReloadChartData(_flaws.Rows, _responses, _names);
        }

        private void SetupStripLines()
        {
            if (_aMinStrip == null)
            {
                _aMinStrip = new StripLine();
                _aMinStrip.BackColor = Color.FromArgb(ChartColors.BoundaryAreaAlpha, ChartColors.aMinColor);
                ChartAreas[0].AxisX.StripLines.Add(_aMinStrip);
            }

            if (_aMaxStrip == null)
            {
                _aMaxStrip = new StripLine();
                _aMaxStrip.BackColor = Color.FromArgb(ChartColors.BoundaryAreaAlpha, ChartColors.aMaxColor);
                ChartAreas[0].AxisX.StripLines.Add(_aMaxStrip);
            }

            if (_leftCensorStrip == null)
            {
                _leftCensorStrip = new StripLine();
                _leftCensorStrip.BackColor = Color.FromArgb(ChartColors.BoundaryAreaAlpha, ChartColors.LeftCensorColor);
                ChartAreas[0].AxisY.StripLines.Add(_leftCensorStrip);
            }

            if (_rightCensorStrip == null)
            {
                _rightCensorStrip = new StripLine();
                _rightCensorStrip.BackColor = Color.FromArgb(ChartColors.BoundaryAreaAlpha, ChartColors.RightCensorColor);
                ChartAreas[0].AxisY.StripLines.Add(_rightCensorStrip);
            }
            
        }

        protected void MoveBoundaryLine<T>(T line, double myX, double myY, bool myTrigger)
        {
            var newLine = line as LineAnnotation;

            if (newLine != null)
            {
                if (!Double.IsNaN(myX))
                {
                    newLine.AnchorX = myX;
                    newLine.X = myX;
                }

                if (!Double.IsNaN(myY))
                {
                    newLine.AnchorY = myY;
                    newLine.Y = myY;
                }


                if (myTrigger)
                {
                    DeterminePointsInThreshold();
                    OnLinesChanged(EventArgs.Empty);
                }
            }
        }

        

        private void MoveLine(VerticalLineAnnotation lineAbove, VerticalLineAnnotation lineBelow)
        {
            var aboveX = lineAbove.AnchorX;
            var belowX = lineBelow.AnchorX;
            var fixPoints = new List<FixPoint>();

            if (Double.IsNaN(aboveX) || Double.IsNaN(belowX))
                return;

            _analysisData.UpdateIncludedPointsBasedFlawRange(aboveX, belowX, fixPoints);

            foreach (FixPoint fix in fixPoints)
            {
                FixColor(fix.SeriesIndex, fix.PointIndex, fix.Flag);
            }
        }

        

        public virtual void OnLinesChanged(EventArgs e)
        {
            if (LinesChanged != null)
            {
                DeterminePointsInThreshold();
                LinesChanged(this, e);
            }
        }

        public void PickBestAxisRange(double myTransformedResponseMin, double myTransformedResponseMax, double myTransformedFlawMin, double myTransformedFlawMax)
        {
            AxisObject yAxis = new AxisObject();
            AxisObject xAxis = new AxisObject();

            //myResponseMin = _analysisData.TransformValueForYAxis(myResponseMin);
            //myResponseMax = _analysisData.TransformValueForYAxis(myResponseMax);

            //myFlawMin = _analysisData.TransformValueForXAxis(myFlawMin);
            //myFlawMax = _analysisData.TransformValueForXAxis(myFlawMax);

            _analysisData.GetXYBufferedRanges(this, xAxis, yAxis, true);

            

            if(yAxis.Max < myTransformedResponseMax)
                yAxis.Max = myTransformedResponseMax;

            if (yAxis.Min > myTransformedResponseMin)
                yAxis.Min = myTransformedResponseMin;

            if (xAxis.Max < myTransformedFlawMax)
                xAxis.Max = myTransformedFlawMax;

            if (xAxis.Min > myTransformedFlawMin)
                xAxis.Min = myTransformedFlawMin;

            if (yAxis.Max == 1.1 && yAxis.Min == -0.1)
            {
                yAxis.Max = 1.0;
                yAxis.Min = 0.0;
                yAxis.Interval = .25;
            }
            else
            {
                AnalysisData.GetBufferedRange(this, yAxis, yAxis.Min, yAxis.Max, AxisKind.Y);
            }

            AnalysisData.GetBufferedRange(this, xAxis, xAxis.Min, xAxis.Max, AxisKind.X);

            RelabelAxesBetter(xAxis, yAxis, _analysisData.InvertTransformValueForXAxis,
                              _analysisData.InvertTransformValueForYAxis, Globals.GetLabelIntervalBasedOnChartSize(this, AxisKind.X), Globals.GetLabelIntervalBasedOnChartSize(this, AxisKind.Y), false, false, _analysisData.FlawTransform, _analysisData.ResponseTransform, _analysisData.TransformValueForXAxis, _analysisData.TransformValueForYAxis);

            //ChartAreas[0].AxisX.LabelAutoFitStyle = LabelAutoFitStyles.DecreaseFont;
            //ChartAreas[0].AxisY.LabelAutoFitStyle = LabelAutoFitStyles.DecreaseFont;


        }

        public void PickBestAxisRange()
        {
            AxisObject yAxis = new AxisObject();
            AxisObject xAxis = new AxisObject();

            _analysisData.GetXYBufferedRanges(this, xAxis, yAxis, true);

            //RelabelAxes(xAxis, yAxis, _analysisData.InvertTransformValueForXAxis, 
            //            _analysisData.InvertTransformValueForYAxis, 10, 10);

            RelabelAxesBetter(xAxis, yAxis, _analysisData.InvertTransformValueForXAxis,
                              _analysisData.InvertTransformValueForYAxis, Globals.GetLabelIntervalBasedOnChartSize(this, AxisKind.X), Globals.GetLabelIntervalBasedOnChartSize(this, AxisKind.Y), false, false, _analysisData.FlawTransform, _analysisData.ResponseTransform, _analysisData.TransformValueForXAxis, _analysisData.TransformValueForYAxis);

            //ChartAreas[0].AxisX.LabelAutoFitStyle = LabelAutoFitStyles.DecreaseFont;
            //ChartAreas[0].AxisY.LabelAutoFitStyle = LabelAutoFitStyles.DecreaseFont;


        }

        

        public void PrepareForRunAnalysis()
        {
            KeepLinesInOrder();
            SyncLines();
            DeterminePointsInThreshold();
            UpdateStripLines();
        }

        protected void RaiseRunAnalysis()
        {
            PrepareForRunAnalysis();

            if (RunAnalysisNeeded != null)
                Invoke(RunAnalysisNeeded);
        }

        private void RegressionAnalysisChart_AnnotationPositionChanged(object sender, EventArgs e)
        {
            RaiseRunAnalysis();
        }

        void RegressionAnalysisChart_MouseUp(object sender, MouseEventArgs e)
        {
            
        }

        void RegressionAnalysisChart_MouseDown(object sender, MouseEventArgs e)
        {
            Point pos = e.Location;
            var result = HitTest(pos.X, pos.Y);

            if (result.ChartElementType == ChartElementType.Annotation)
            {
                var anno = result.Object as LineAnnotation;

                if (anno != null && anno.AllowMoving)
                {
                    //if (anno.LineWidth == 7)
                    //    anno.LineWidth = 5;
                }
            }
        }

        void RegressionAnalysisChart_MouseMove(object sender, MouseEventArgs e)
        {
            if (_analysisData == null)
                return;

            Point pos = e.Location;

            hittestresults[0] = HitTest(pos.X, pos.Y);
            hittestresults[1] = HitTest(pos.X+1, pos.Y);
            hittestresults[2] = HitTest(pos.X-1, pos.Y);
            //hittestresults[3] = HitTest(pos.X+2, pos.Y);
            //hittestresults[4] = HitTest(pos.X-2, pos.Y);
            //hittestresults[5] = HitTest(pos.X, pos.Y-2);
            //hittestresults[6] = HitTest(pos.X, pos.Y+2);
            hittestresults[3] = HitTest(pos.X, pos.Y+1);
            hittestresults[4] = HitTest(pos.X, pos.Y-1);
            //hittestresults[9] = HitTest(pos.X, pos.Y - 3);
            //hittestresults[10] = HitTest(pos.X, pos.Y + 3);
            //hittestresults[11] = HitTest(pos.X, pos.Y + 3);
            //hittestresults[12] = HitTest(pos.X, pos.Y - 3);

            var allEmpty = true;

            foreach(HitTestResult result in hittestresults)
            {
                if(result.ChartElementType == ChartElementType.Annotation)
                {            
                    var anno = result.Object as LineAnnotation;

                    if (anno != null && anno.AllowMoving)
                    {
                        if (madeWhichOneBigger != null && anno != madeWhichOneBigger)
                            madeWhichOneBigger.LineWidth = 3;

                        if (anno == _aMinLine || anno == _aMaxLine)
                            Cursor = Cursors.VSplit;
                        else
                            Cursor = Cursors.HSplit;

                        if(anno.LineWidth != 5)
                            anno.LineWidth = 5;
                        madeLineBigger = true;
                        madeWhichOneBigger = anno;
                        allEmpty = false;
                        break;
                    }
                    else
                    {
                        //we've stumbled on a non-line annotation so we should ignore this event and not change anything
                        return;
                    }
                }
                else if (result == hittestresults[0])
                {
                    if (result.ChartElementType == ChartElementType.DataPoint)
                    {
                        try
                        {
                            var dataPoint = result.Object as DataPoint;
                            var xInvt = _analysisData.InvertTransformValueForXAxis(dataPoint.XValue);
                            var yInvt = _analysisData.InvertTransformValueForYAxis(dataPoint.YValues[0]);

                            dataPoint.ToolTip = dataPoint.ToolTip.Trim();

                            var oldLabel = dataPoint.ToolTip;

                            var index = oldLabel.LastIndexOf("=");

                            //if there is a point listed in the text
                            if (index != -1)
                                oldLabel = oldLabel.Substring(0, index + 1);

                            _positionString = string.Format("Series: {3}" + "\n" + "ID: {2}" + "\n" + "Value: ({0:0.###}, {1:0.###})", xInvt, yInvt, dataPoint.GetCustomProperty("Name"), result.Series.Name);

                            //var newLabel = oldLabel + _positionString;

                            //dataPoint.ToolTip += "Value: " + _positionString; 
                            dataPoint.ToolTip = _positionString;
                        }
                        catch (Exception exp)
                        {
                            MessageBox.Show("RegressionAnalysisChart_MouseMove: " + exp.Message);
                        }

                    }
                    else if (result.ChartElementType == ChartElementType.PlottingArea || result.ChartElementType == ChartElementType.StripLines)
                    {
                        var xInvt = _analysisData.InvertTransformValueForXAxis(this.XAxis.PixelPositionToValue(e.X));
                        var yInvt = _analysisData.InvertTransformValueForYAxis(this.YAxis.PixelPositionToValue(e.Y));

                        _positionString = string.Format("Value: ({0:0.###}, {1:0.###})", xInvt, yInvt);
                    }
                }
            }

            if (allEmpty && madeLineBigger == true)
            {
                Cursor = Cursors.Default;

                madeWhichOneBigger.LineWidth = 3;
                madeLineBigger = false;
                madeWhichOneBigger = null;
            }
        }

        private void RegressionAnalysisChart_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            RegressionAnalysisChart_MouseClick(sender, e);
            //clicking on the same pixel when opened menu will close it
            //if (_lastX == e.X && _lastY == e.Y)
            //{
            //    _lastX = -1;
            //    _lastY = -1;

            //    if(_lastMenu != null)
            //        _lastMenu.Close();
            //    return;
            //}
            //else
            //{
            //    _lastX = e.X;
            //    _lastY = e.Y;
            //}
        }

        private void RegressionAnalysisChart_MouseClick(object sender, MouseEventArgs e)
        {
            Point pos = e.Location;
            HitTestResult result = HitTest(pos.X, pos.Y);
            var panel = new FlowLayoutPanel();
            panel.FlowDirection = FlowDirection.TopDown;
            //panel.BorderStyle = BorderStyle.FixedSingle;
            //panel.AutoSize = true;
            //panel.Dock = DockStyle.Fill;

            if (_lastMenu != null && _lastMenu.Visible == false)// && _lastMenu.Visible == true)
            {
                _lastX = -1;
                _lastY = -1;
                _lastMenu.Close();
                _lastMenu = null;

                return;
            }

            //clicking on the same pixel when opened menu will close it
            //if (_lastX == e.X && _lastY == e.Y)
            //{
            //    _lastX = -1;
            //    _lastY = -1;
            //    return;
            //}

            var line = result.Object as LineAnnotation;

            //if there is an error and the user isn't moving a line that would cause an analysis to be run
            if (_error != null && line == null)
            {
                ClearErrors();

                if(result.ChartElementType == ChartElementType.Annotation)
                {
                    return;
                }
            }

            if (madeWhichOneBigger != null && madeWhichOneBigger.AllowMoving)
            {
                NoMovingLines = false;
            }
            else
                NoMovingLines = true;

            if (NoMovingLines)
            {
                if ((result.ChartElementType == ChartElementType.PlottingArea ||
                    result.ChartElementType == ChartElementType.StripLines))
                {
                    double xVal = result.ChartArea.AxisX.PixelPositionToValue(pos.X);
                    //xVal = Math.Pow(10, xVal);
                    double yVal = result.ChartArea.AxisY.PixelPositionToValue(pos.Y);

                    var dp = new DataPoint(xVal, yVal);
                    var menus = CreatePointMenu(dp.XValue, dp.YValues.First(), false, "", "", -1, Color.Black, panel);
                    var actionMenu = menus[0];
                    var pointMenu = menus[1];

                    var host = new ToolStripControlHost(panel);
                    _boxColor = Color.Black;
                    panel.BackColor = Color.FromKnownColor(KnownColor.White);
                    foreach (Control child in panel.Controls)
                        child.BackColor = Color.FromKnownColor(KnownColor.Transparent);

                    panel.Paint += panel_Paint;

                    actionMenu.Items.Insert(0, host);

                    panel.Width = actionMenu.Width;
                    host.Width = panel.Width;

                    actionMenu.AutoSize = false;
                    actionMenu.AutoSize = true;

                    _lastMenu = actionMenu;

                    //pointMenu.Show(this, new Point(pos.X, pos.Y - pointMenu.Height ));
                    actionMenu.Show(this, new Point(pos.X + 5, pos.Y - pointMenu.Height));
                }
                else if (result.ChartElementType == ChartElementType.DataPoint)
                {
                    var dp = result.Object as DataPoint;
                    string seriesName = result.Series.Name;
                    string pointName = dp.GetCustomProperty("Name");
                    int pointIndex = result.PointIndex;
                    Color color = result.Series.Points[pointIndex].Color;
                    int rowIndex = Convert.ToInt32(dp.GetCustomProperty("Row Index"));
                    int colIndex = Convert.ToInt32(dp.GetCustomProperty("Column Index"));

                    //if for some reason color isn't available at the point level
                    if (color.A == 0)
                        color = _colorMap[seriesName];

                    var menus = CreatePointMenu(dp.XValue, dp.YValues.First(), true, seriesName, pointName, pointIndex, color, panel, rowIndex, colIndex);
                    var actionMenu = menus[0];
                    var pointMenu = menus[1];
                    var host = new ToolStripControlHost(panel);
                    _boxColor = color;
                    panel.BackColor = Color.FromKnownColor(KnownColor.White);
                    foreach (Control child in panel.Controls)
                        child.BackColor = Color.FromKnownColor(KnownColor.Transparent);

                    panel.Paint += panel_Paint;

                    actionMenu.Items.Insert(0, host);

                    panel.Width = actionMenu.Width;
                    host.Width = panel.Width;

                    actionMenu.AutoSize = false;
                    actionMenu.AutoSize = true;

                    if (_lastMenu != null)
                    {
                        _lastMenu = null;
                    }

                    //pointMenu.Show(this, new Point(pos.X, pos.Y - pointMenu.Height ));
                    actionMenu.Show(this, new Point(pos.X + 10, pos.Y - pointMenu.Height));
                }
            }
            else
            {
                FixMovingLines();
            }

            _lastX = e.X;
            _lastY = e.Y;
        }

        

        void panel_Paint(object sender, PaintEventArgs e)
        {
            Rectangle rect = (sender as Panel).ClientRectangle;
            rect.Width -= 2;
            rect.Height -= 2;
            rect.X = 1;
            rect.Y = 1;
            var pen = new Pen(_boxColor);
            pen.Width = 2;
            e.Graphics.DrawRectangle(pen, rect);
        }

        private void RegressionAnalysisChart_PostPaint(object sender, ChartPaintEventArgs e)
        {
            if (e.ChartElement is Series && ((Series)e.ChartElement).Name != "Empty" &&
                ((Series) e.ChartElement).Name == _analysisData.ActivatedResponseNames.Last() && !_painted)
            {
                DrawBoundaryLines();

                DrawEquation();

                BuildColorMap();
                //this.DeterminePointsInThreshold();

                _analysisData.RefillSortList();

                //OnLinesChanged(EventArgs.Empty);
                _painted = true;

                //PickBestAxisRange();

                RaiseRunAnalysis();

                //Refresh();
                //Refresh();

                
            }

            if(e.ChartElement is Chart)
            {
                UpdateEquationLocation(e);
            }
        }

        

        

        private void RegressionAnalysisChart_Resize(object sender, EventArgs e)
        {
            ChartAreas[0].RecalculateAxesScale();

            Invalidate();
            
            ForceUpdateEquation();
        }

        protected void ForceUpdateEquation()
        {
            if (_equation != null)
            {
                _equation.Text = _equation.Text;
                _equation.ResizeToContent();
            }
        }

        //public override void SelectChart()
        //{
        //    base.SelectChart();

        //    RegressionAnalysisChart_MouseClick(this, null);
        //}
        
        public void SetAMaxBoundary(double myX, bool triggerEvent)
        {
            MoveBoundaryLine(_aMaxLine, myX, Double.NaN, triggerEvent);
        }

        public void SetAMaxBoundaryMenu(Object sender, EventArgs e, DataPoint dataPoint)
        {
            SetAMaxBoundary(dataPoint.XValue, true);

            if (_lastMenu != null)
            {
                _lastMenu = null;
            }

            RaiseRunAnalysis();
        }

        public void SetAMinBoundary(double myX, bool triggerEvent)
        {
            MoveBoundaryLine(_aMinLine, myX, Double.NaN, triggerEvent);
        }

        public void SetAMinBoundaryMenu(Object sender, EventArgs e, DataPoint dataPoint)
        {
            SetAMinBoundary(dataPoint.XValue, true);

            if (_lastMenu != null)
            {
                _lastMenu = null;
            }

            RaiseRunAnalysis();
        }

        public void SetThresholdBoundary(double myY, bool triggerEvent)
        {
            MoveBoundaryLine(_thresholdLine, Double.NaN, myY, triggerEvent);
        }

        public void SetThresholdBoundaryMenu(Object sender, EventArgs e, DataPoint dataPoint)
        {
            SetThresholdBoundary(dataPoint.YValues.First(), true);

            if (_lastMenu != null)
            {
                _lastMenu = null;
            }

            RaiseRunAnalysis();
        }

        protected virtual void SyncLines()
        {
            MoveBoundaryLine(_aMinLine, _aMinLine.X, _aMinLine.Y, true);
            MoveBoundaryLine(_aMaxLine, _aMaxLine.X, _aMaxLine.Y, true);
            MoveBoundaryLine(_thresholdLine, _thresholdLine.X, _thresholdLine.Y, true);
        }

        private void ToggleAllResponses(object sender, EventArgs e, DataPoint dataPoint)
        {
            List<FixPoint> fixPoints = new List<FixPoint>();

            _analysisData.ToggleAllResponses(dataPoint.XValue, fixPoints);

            foreach (FixPoint fix in fixPoints)
            {
                FixColor(fix.SeriesIndex, fix.PointIndex, fix.Flag);
            }

            DeterminePointsInThreshold();
            Invalidate();
        }

        

        private void ToggleAllResponsesMenu(object sender, EventArgs e, DataPoint dataPoint)
        {
            ToggleAllResponses(sender, e, dataPoint);

            RaiseRunAnalysis();
        }

        private void ToggleResponse(object sender, EventArgs e, DataPoint dataPoint, string seriesName, int rowIndex, int colIndex)
        {
            List<FixPoint> fixPoints = new List<FixPoint>();

            _analysisData.ToggleResponse(dataPoint.XValue, dataPoint.YValues.First(), seriesName, rowIndex, colIndex, fixPoints);

            foreach(FixPoint fix in fixPoints)
            {
                FixColor(fix.SeriesIndex, fix.PointIndex, fix.Flag);
            }
        }

        


        private void ToggleResponseMenu(object sender, EventArgs e, DataPoint dataPoint, string seriesName, int rowIndex, int colIndex)
        {
            ToggleResponse(sender, e, dataPoint, seriesName, rowIndex, colIndex);

            RaiseRunAnalysis();
        }

        public void UnfreezeThresholdLine()
        {
            _thresholdFreeze = false;
        }

        public void UpdateBestFitLine()
        {
            Series fitLine = Series[PODRegressionLabels.BestFitLine];

            DataView view = _analysisData.ResidualUncensoredTable.DefaultView;
            
            this.Invoke((MethodInvoker)delegate()
            {
                try
                {
                    fitLine.Points.DataBindXY(view, "Flaw", view, "fit");
                }
                catch(Exception fixme) {
                    //MessageBox.Show("This One!");
                    fitLine.Points.DataBindXY(view, "Flaw", view, "t_fit");
                }
                //fitLine.Points.DataBindXY(view, "t_flaw", view, "t_fit");
                //fitLine.Points.DataBindXY(view, "flaw", view, "t_fit");
            });
            
        }

        public void UpdateLevelConfidenceLines(double myA50, double myA90, double myA90_95, double myFitM, double myFitB)
        {
            Series a50Line = Series[PODRegressionLabels.a50Line];
            Series a90Line = Series[PODRegressionLabels.a90Line];
            Series a90_95Line = Series[PODRegressionLabels.a9095Line];

            double minY = ChartAreas[0].AxisY.Minimum;
            double minX = ChartAreas[0].AxisX.Minimum;

            double a50Y = myFitM*myA50 + myFitB;
            double a90Y = myFitM*myA90 + myFitB;
            double a90_95Y = myFitM*myA90_95 + myFitB;

            Series line = a50Line;

            line.Points.Clear();
            line.Points.AddXY(myA50, minY);
            line.Points.AddXY(myA50, a50Y);
            //line.Points.AddXY(minX, a50Y);
            line.Points[0].Label = "50";

            line = a90Line;

            line.Points.Clear();
            line.Points.AddXY(myA90, minY);
            line.Points.AddXY(myA90, a90Y);
            //line.Points.AddXY(minX, a90Y);
            line.Points[0].Label = "90";

            line = a90_95Line;

            line.Points.Clear();

            //only draw line if the analysis could calculate it
            if (myA90_95 != 0.0)
            {
                line.Points.AddXY(myA90_95, minY);
                line.Points.AddXY(myA90_95, a90Y);
                //line.Points.AddXY(minX, a90_95Y);
                line.Points[0].Label = "90/95";
            }
        }

        public void UpdateLevelConfidenceLines(double myA50, double myA90, double myA90_95)
        {
            Series a50Line = Series[PODRegressionLabels.a50Line];
            Series a90Line = Series[PODRegressionLabels.a90Line];
            Series a90_95Line = Series[PODRegressionLabels.a9095Line];

            double minY = ChartAreas[0].AxisY.Minimum;
            double minX = ChartAreas[0].AxisX.Minimum;

            double a50Y = .5;
            double a90Y = .9;

            Series line = a50Line;

            line.Points.Clear();
            line.Points.AddXY(myA50, minY);
            line.Points.AddXY(myA50, a50Y);
            //line.Points.AddXY(minX, a50Y);
            line.Points[0].Label = "50";

            line = a90Line;

            line.Points.Clear();
            line.Points.AddXY(myA90, minY);
            line.Points.AddXY(myA90, a90Y);
            //line.Points.AddXY(minX, a90Y);
            line.Points[0].Label = "90";

            line = a90_95Line;

            line.Points.Clear();
            line.Points.AddXY(myA90_95, minY);
            line.Points.AddXY(myA90_95, a90Y);
            //line.Points.AddXY(minX, a90_95Y);
            line.Points[0].Label = "90/95";
        }

        public void RefreshStripLines()
        {
            UpdateStripLines();
        }

        protected virtual void UpdateStripLines()
        {
            //fix any potential errors that would cause an invalid strip chart to be displayed
            if (Double.IsNaN(_aMaxLine.X) || ChartAreas[0].AxisX.Maximum < _aMaxLine.X)
            {
                _aMaxLine.X = ChartAreas[0].AxisX.Maximum;
                _aMaxLine.AnchorX = _aMaxLine.X;
            }

            if (Double.IsNaN(_aMinLine.X) || ChartAreas[0].AxisX.Minimum > _aMinLine.X)
            {
                _aMinLine.X = ChartAreas[0].AxisX.Minimum;
                _aMinLine.AnchorX = _aMinLine.X;
            }

            _aMaxStrip.IntervalOffset = _aMaxLine.X;
            _aMaxStrip.StripWidth = ChartAreas[0].AxisX.Maximum - _aMaxLine.X;
            _aMinStrip.IntervalOffset = ChartAreas[0].AxisX.Minimum;
            _aMinStrip.StripWidth = _aMinLine.X - ChartAreas[0].AxisX.Minimum;
        }

        

        
        public string EquationText
        {
            get
            {
                return _equation.Text;
            }
        }

        public void ForceIncludedPointsUpdate()
        {
            /*for (int i = _prevAbove; i < sortByX.Count; i++)
            {
                FixColor(sortByX[i].SeriesName, sortByX[i].SeriesPtIndex, Flag.OutBounds);
            }

            for (int i = 0; i < _prevBelow; i++)
            {
                FixColor(sortByX[i].SeriesName, sortByX[i].SeriesPtIndex, Flag.OutBounds);
            }*/

            foreach(var point in _analysisData.TurnedOffPoints)
            {
                //convert from table column index to series index
                var index = point.ColumnIndex + DataSeriesStartIndex;

                FixColor(index, point.RowIndex, Flag.OutBounds);
            }
        }

        public int DataSeriesStartIndex
        {
            get
            {
                return Series.IndexOf(PODRegressionLabels.a50Line) + 1;
            }
        }

        public int DataSeriesEndIndex
        {
            get
            {
                return Series.IndexOf(PODRegressionLabels.BestFitLine) - 1;
            }
        }

        public override void ForceResizeAnnotations()
        {
            base.ForceResizeAnnotations();

            ForceUpdateEquation();
        }

        public void ForceRefillSortList()
        {
            _analysisData.ForceRefillSortList();
        }

        private void FixMovingLines()
        {
            if (madeWhichOneBigger != null && madeWhichOneBigger.Width == 5)
            {
                Cursor = Cursors.Default;

                madeWhichOneBigger.LineWidth = 3;
                madeLineBigger = false;
                madeWhichOneBigger = null;
            }
        }

        
    }

    

    
}