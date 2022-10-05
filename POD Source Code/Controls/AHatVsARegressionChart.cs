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

namespace POD.Controls
{
    public partial class AHatVsARegressionChart : RegressionAnalysisChart
    {
        protected HorizontalLineAnnotation _leftCensorLine = null;
        protected HorizontalLineAnnotation _rightCensorLine = null;

        public AHatVsARegressionChart()
        {
            InitializeComponent();

            IsSquare = false;
            CanUnselect = false;
            Selectable = false;
        }

        public override void DrawBoundaryLines()
        {
            base.DrawBoundaryLines();

            this.DrawLeftCensorBoundLine();
            this.DrawRightCensorBoundLine();

            
        }

        protected override void SyncLines()
        {
            base.SyncLines();

            /*if (Double.IsNaN(_leftCensorLine.X))
                _leftCensorLine.X = 0.0;

            if (Double.IsNaN(_rightCensorLine.X))
                _rightCensorLine.X = 0.0;

            if (Double.IsNaN(_thresholdLine.X))
                _thresholdLine.X = 0.0;*/

            this.MoveBoundaryLine(this._leftCensorLine, _leftCensorLine.X, _leftCensorLine.Y, true);
            this.MoveBoundaryLine(this._rightCensorLine, _rightCensorLine.X, _rightCensorLine.Y, true);
            //this.MoveBoundaryLine(this._thresholdLine, _thresholdLine.X, _thresholdLine.Y, true);
        }

        public override bool FindValue(ControlLine line, ref double myValue)
        {
            double value;
            double anchorValue;

            bool foundLine = base.FindValue(line, ref myValue);

            if (foundLine == false)
            {
                foundLine = true;

                switch (line)
                {
                    case ControlLine.LeftCensor:
                        value = this._leftCensorLine.Y;
                        anchorValue = this._leftCensorLine.AnchorY;
                        break;
                    case ControlLine.RightCensor:
                        value = this._rightCensorLine.Y;
                        anchorValue = this._rightCensorLine.AnchorY;
                        break;
                    default:
                        value = Double.NaN;
                        anchorValue = Double.NaN;
                        foundLine = false;
                        break;
                }

                myValue = Double.IsNaN(value) ? anchorValue : value;
            }

            return foundLine;
        }

        protected override void UpdateStripLines()
        {
            base.UpdateStripLines();

            //fix any potential errors that would cause an invalid strip chart to be displayed
            if (Double.IsNaN(_rightCensorLine.Y) || ChartAreas[0].AxisY.Maximum < _rightCensorLine.Y)
                _rightCensorLine.Y = ChartAreas[0].AxisY.Maximum;
            
            if (Double.IsNaN(_leftCensorLine.Y) || ChartAreas[0].AxisY.Minimum > _leftCensorLine.Y)
                _leftCensorLine.Y = ChartAreas[0].AxisY.Minimum;

            _leftCensorStrip.IntervalOffset = ChartAreas[0].AxisY.Minimum;
            _leftCensorStrip.StripWidth = _leftCensorLine.Y - ChartAreas[0].AxisY.Minimum;
            _rightCensorStrip.IntervalOffset = _rightCensorLine.Y;
            _rightCensorStrip.StripWidth = ChartAreas[0].AxisY.Maximum - _rightCensorLine.Y;
        }

        protected override void KeepLinesInOrder()
        {
            base.KeepLinesInOrder();

            KeepLeftRightCensorInOrder();
        }

        public void SetLeftCensorBoundary(double myY, bool myTriggerEvent)
        {
            this.MoveBoundaryLine(this._leftCensorLine, Double.NaN, myY, myTriggerEvent);
        }

        public void DrawLeftCensorBoundLine()
        {
            if (this._leftCensorLine == null)
            {
                _leftCensorLine = new HorizontalLineAnnotation();

                var xAxis = this.ChartAreas[0].AxisX;
                var yAxis = this.ChartAreas[0].AxisY;

                var y = GetInitialLeftCensorLocationFromData();

                _leftCensorLine.AxisX = xAxis;
                _leftCensorLine.AxisY = yAxis;
                _leftCensorLine.IsSizeAlwaysRelative = false;
                _leftCensorLine.AllowMoving = true;
                //_leftCensorLine.AnchorY = y;
                _leftCensorLine.Y = y;
                _leftCensorLine.IsInfinitive = true;
                _leftCensorLine.ClipToChartArea = this.ChartAreas[0].Name;
                _leftCensorLine.LineColor = System.Drawing.Color.FromArgb(ChartColors.LineAlpha, ChartColors.LeftCensorColor);
                _leftCensorLine.LineDashStyle = ChartDashStyle.Dash;
                _leftCensorLine.LineWidth = 3;

                this.Annotations.Add(_leftCensorLine);
            }
        }

        public void DrawRightCensorBoundLine()
        {
            if (this._rightCensorLine == null)
            {
                _rightCensorLine = new HorizontalLineAnnotation();

                var xAxis = this.ChartAreas[0].AxisX;
                var yAxis = this.ChartAreas[0].AxisY;

                var y = GetInitialRightCensorLocationFromData();

                _rightCensorLine.AxisX = xAxis;
                _rightCensorLine.AxisY = yAxis;
                _rightCensorLine.IsSizeAlwaysRelative = false;
                _rightCensorLine.AllowMoving = true;
                //_rightCensorLine.AnchorY = y;
                _rightCensorLine.Y = y;
                _rightCensorLine.IsInfinitive = true;
                _rightCensorLine.ClipToChartArea = this.ChartAreas[0].Name;
                _rightCensorLine.LineColor = System.Drawing.Color.FromArgb(ChartColors.LineAlpha, ChartColors.RightCensorColor);
                _rightCensorLine.LineDashStyle = ChartDashStyle.Dash;
                _rightCensorLine.LineWidth = 3;

                this.Annotations.Add(_rightCensorLine);
            }
        }

        private double GetInitialRightCensorLocationFromData()
        {
            var yValues =
                   (from DataRow row in this._analysisData.ActivatedResponses.Rows
                    from obj in row.ItemArray
                    select Convert.ToDouble(obj)).ToList();

            double y = 1.0;

            if (yValues.Count > 0)
            {
                y = yValues.Max();
                double range = y - yValues.Min();
                double buffer = range * .05;

                y += buffer;
            }

            return y;
        }

        private double GetInitialLeftCensorLocationFromData()
        {
            var yValues =
                    (from DataRow row in this._analysisData.ActivatedResponses.Rows
                     from obj in row.ItemArray
                     select Convert.ToDouble(obj)).ToList();

            double y = 0.0;

            if (yValues.Count > 0)
            {
                y = yValues.Min();
                double range = yValues.Max() - y;
                double buffer = range * .05;

                y -= buffer;
            }

            //left censor less than zero doesn't make much sense
            //if (y < 0.0)
            //    y = 0.0;

            return y;
        }

        private void KeepLeftRightCensorInOrder()
        {
            if(_leftCensorLine == null)
                DrawLeftCensorBoundLine();

            if(_rightCensorLine == null)
                DrawRightCensorBoundLine();

            if (_leftCensorLine.Y > _rightCensorLine.Y)
            {
                double temp = _leftCensorLine.Y;
                _leftCensorLine.Y = _rightCensorLine.Y;
                _rightCensorLine.Y = temp;
            }
        }

        public void SetRightCensorBoundary(double myY, bool myTriggerEvent)
        {
            this.MoveBoundaryLine(this._rightCensorLine, Double.NaN, myY, myTriggerEvent);
        }

        public void SetLeftCensorBoundaryMenu(Object sender, System.EventArgs e, DataPoint dataPoint)
        {
            SetLeftCensorBoundary(dataPoint.YValues.First(), true);

            if (_lastMenu != null)
            {
                _lastMenu = null;
            }

            RaiseRunAnalysis();
        }

        public void SetRightCensorBoundaryMenu(Object sender, System.EventArgs e, DataPoint dataPoint)
        {
            SetRightCensorBoundary(dataPoint.YValues.First(), true);

            if (_lastMenu != null)
            {
                _lastMenu = null;
            }

            RaiseRunAnalysis();
        }

        public override List<ToolStripItem> BuildMenuItems(double x, double y)
        {
            var menuItems = base.BuildMenuItems(x, y);

            var setBoundaryThresholds = new ToolStripMenuItem("Set POD Threshold here");

            setBoundaryThresholds.Click += (sender, e) => this.SetThresholdBoundaryMenu(sender, e, new DataPoint(x, y));

            var setLeftCensor = new ToolStripMenuItem("Set Response Min. here");

            setLeftCensor.Click += (sender, e) => this.SetLeftCensorBoundaryMenu(sender, e, new DataPoint(x, y));

            var setRightCensor = new ToolStripMenuItem("Set Response Max. here");

            setRightCensor.Click += (sender, e) => this.SetRightCensorBoundaryMenu(sender, e, new DataPoint(x, y));

            if (ContextMenuImageList != null)
            {
                setBoundaryThresholds.Image = ContextMenuImageList.Images[9];
                setLeftCensor.Image = ContextMenuImageList.Images[2];
                setRightCensor.Image = ContextMenuImageList.Images[3];
            }

            var newItems = new List<ToolStripItem>();

            newItems.Add(setBoundaryThresholds);
            newItems.Add(setRightCensor);
            newItems.Add(setLeftCensor);
            
            menuItems.InsertRange(2, newItems);

            return menuItems;
        }



        public void UpdateEquation(TransformTypeEnum myXAxisTransform, TransformTypeEnum myYAxisTransform)
        {
            string text = "Fit: ";
            string yText = "";
            string xText = "";

            if (_equation != null)
            {
                switch(myXAxisTransform)
                {
                    case TransformTypeEnum.Log:
                        xText = "m·ln(x) + b";
                        break;
                    case TransformTypeEnum.Linear:
                        xText = "m·x + b";
                        break;
                    case TransformTypeEnum.Exponetial:
                        xText = "m(e^x) + b";
                        break;
                    case TransformTypeEnum.Inverse:
                        xText = "m(1/x) + b";
                        break;
                    default:
                        xText = "Custom";
                        break;
                }

                switch (myYAxisTransform)
                {
                    case TransformTypeEnum.Log:
                        yText = "ln(y)";
                        break;
                    case TransformTypeEnum.Linear:
                        yText = "y";
                        break;
                    case TransformTypeEnum.Exponetial:
                        yText = "e^y";
                        break;
                    case TransformTypeEnum.Inverse:
                        yText = "1/y";
                        break;
                    case TransformTypeEnum.BoxCox:
                        yText = "(y^(lambda)-1)/lambda";
                        break;
                    default:
                        yText = "Custom";
                        break;
                }

                //combine x and y equations
                text = yText + " = " + xText;

                _equation.Text = text;
            }
        }

        
    }
}
