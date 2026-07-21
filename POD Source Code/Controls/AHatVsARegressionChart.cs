// <copyright file="AHatVsARegressionChart.cs" company="The University of Dayton">
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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Diagnostics;

namespace POD.Controls
{
    public partial class AHatVsARegressionChart : RegressionAnalysisChart
    {
        protected PODHorizontalLineAnnotation _leftCensorLine = null;
        protected PODHorizontalLineAnnotation _rightCensorLine = null;

        public AHatVsARegressionChart()
        {
            InitializeComponent();

            IsSquare = false;
            CanUnselect = false;
            Selectable = false;
            AnnotationPositionChanging += AHatVsARegressionChart_AnnotationPositionChanging;
        }

        private void AHatVsARegressionChart_AnnotationPositionChanging(object sender, AnnotationPositionChangingEventArgs e)
        {
            if (e.Annotation == _leftCensorLine)
            {
                _leftCensorLine.TransformValue = e.NewLocationY;
            }
            else if (e.Annotation == _rightCensorLine)
            {
                _rightCensorLine.TransformValue = e.NewLocationY;
            }
        }

        public override void DrawBoundaryLines()
        {
            base.DrawBoundaryLines();

            this.DrawLeftCensorBoundLine();
            this.DrawRightCensorBoundLine();

            
        }

        public override void PrepareCloneSeries(DataPointChart clone, double finalMinY)
        {
            base.PrepareCloneSeries(clone, finalMinY);

            List<string> removeAnnotations = new List<string>();
            List<string> removeStrips = new List<string>();

            if (_analysisData.ResponseCompleteAboveMaxCount == 0)
            {
                removeStrips.Add(LinearityChartLabels.RightCensorStrip);
                removeAnnotations.Add(LinearityChartLabels.RightCensor);
            }

            if (_analysisData.ResponseCompleteBelowMinCount == 0)
            {
                removeStrips.Add(LinearityChartLabels.LeftCensorStrip);
                removeAnnotations.Add(LinearityChartLabels.LeftCensor);
            }

            foreach (string stripName in removeStrips)
            {
                List<StripLine> s = clone.YAxis.StripLines.Where(x => ((NamedStripLine)x).Name == stripName).ToList();

                if (s.Count > 0)
                {
                    clone.YAxis.StripLines.Remove(s[0]);
                }
            }

            foreach (string annotationName in removeAnnotations)
            {
                Annotation s = clone.Annotations.FindByName(annotationName);

                s.Visible = false;
            }
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

            this.SetLeftCensorBoundary(_leftCensorLine.TransformValue, true);
            this.SetRightCensorBoundary(_rightCensorLine.TransformValue, true);
            //this.MoveBoundaryLine(this._thresholdLine, _thresholdLine.X, _thresholdLine.Y, true);
        }

        public override bool FindValue(ControlLine line, ref double myValue)
        {
            bool foundLine = base.FindValue(line, ref myValue);

            if (foundLine == false)
            {
                foundLine = true;

                switch (line)
                {
                    case ControlLine.LeftCensor:
                        myValue = this._leftCensorLine.TransformValue;
                        break;
                    case ControlLine.RightCensor:
                        myValue = this._rightCensorLine.TransformValue;
                        break;
                    default:
                        myValue = double.NaN;
                        foundLine = false;
                        break;
                }
            }

            return foundLine;
        }

        protected override void UpdateStripLines()
        {
            base.UpdateStripLines();

            if (_leftCensorLine == null || _rightCensorLine == null) return;

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
            if (_leftCensorLine != null)
            {
                _leftCensorLine.TransformValue = myY;

                var displayY = myY;
                if (double.IsNaN(displayY) || (ChartAreas.Count > 0 && ChartAreas[0].AxisY.Minimum > displayY))
                {
                    if (ChartAreas.Count > 0)
                    {
                        displayY = ChartAreas[0].AxisY.Minimum;
                    }
                }

                _leftCensorLine.Y = displayY;

                if (myTriggerEvent)
                {
                    DeterminePointsInThreshold();
                    OnLinesChanged(EventArgs.Empty);
                }
            }
        }

        public void DrawLeftCensorBoundLine()
        {
            if (this._leftCensorLine == null)
            {
                _leftCensorLine = new PODHorizontalLineAnnotation();

                var xAxis = this.ChartAreas[0].AxisX;
                var yAxis = this.ChartAreas[0].AxisY;

                var y = GetInitialLeftCensorLocationFromData();
                _leftCensorLine.TransformValue = y;

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
                _leftCensorLine.Name = LinearityChartLabels.LeftCensor;

                this.Annotations.Add(_leftCensorLine);
            }
        }

        public void DrawRightCensorBoundLine()
        {
            if (this._rightCensorLine == null)
            {
                _rightCensorLine = new PODHorizontalLineAnnotation();

                var xAxis = this.ChartAreas[0].AxisX;
                var yAxis = this.ChartAreas[0].AxisY;

                var y = GetInitialRightCensorLocationFromData();
                _rightCensorLine.TransformValue = y;

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
                _rightCensorLine.Name = LinearityChartLabels.RightCensor;

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

            if (_leftCensorLine.TransformValue > _rightCensorLine.TransformValue)
            {
                double temp = _leftCensorLine.TransformValue;
                SetLeftCensorBoundary(_rightCensorLine.TransformValue, false);
                SetRightCensorBoundary(temp, false);
            }
        }

        public void SetRightCensorBoundary(double myY, bool myTriggerEvent)
        {
            if (_rightCensorLine != null)
            {
                _rightCensorLine.TransformValue = myY;

                var displayY = myY;
                if (double.IsNaN(displayY) || (ChartAreas.Count > 0 && ChartAreas[0].AxisY.Maximum < displayY))
                {
                    if (ChartAreas.Count > 0)
                    {
                        displayY = ChartAreas[0].AxisY.Maximum;
                    }
                }

                _rightCensorLine.Y = displayY;

                if (myTriggerEvent)
                {
                    DeterminePointsInThreshold();
                    OnLinesChanged(EventArgs.Empty);
                }
            }
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

        public void UpdateEquation(TransformTypeEnum myXAxisTransform, TransformTypeEnum myYAxisTransform, int minimumCensor = 0, int maximumCensor = 0)
        {
            if (_equation is null)
                return;

            string yText = YTransformEquationText(myYAxisTransform);
            string xText = XTransformEquationText(myXAxisTransform);
            //combine x and y equations
            string text = $"{yText} = {xText}";

            if (maximumCensor > 0)
            {
                text += Environment.NewLine;
                text += $"Right Censor: {maximumCensor}";
            }
            if (minimumCensor > 0)
            {
                text += Environment.NewLine;
                text += $"Left Censor: {minimumCensor}";
            }

            _equation.Text = text;
        }

        private string XTransformEquationText(TransformTypeEnum transform)
        {
            switch (transform)
            {
                case TransformTypeEnum.Log:
                    return "m·ln(x) + b";
                case TransformTypeEnum.Linear:
                    return "m·x + b";
                case TransformTypeEnum.Exponetial:
                    return "m(e^x) + b";
                case TransformTypeEnum.Inverse:
                    return "m(1/x) + b";
                default:
                    return "Custom";
            }
        }
        private string YTransformEquationText(TransformTypeEnum transform)
        {
            switch (transform)
            {
                case TransformTypeEnum.Log:
                    return "ln(y)";
                case TransformTypeEnum.Linear:
                    return "y";
                case TransformTypeEnum.Exponetial:
                    return "e^y";
                case TransformTypeEnum.Inverse:
                    return "1/y";
                case TransformTypeEnum.BoxCox:
                    return "(y^(lambda)-1)/lambda";
                default:
                    return "Custom";
            }
        }      
    }
}
