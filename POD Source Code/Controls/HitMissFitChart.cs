// <copyright file="HitMissFitChart.cs" company="The University of Dayton">
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
using POD.Data;
using CSharpBackendWithR;

using System.Diagnostics;
namespace POD.Controls
{
    public partial class HitMissFitChart : LinearityChart
    {
        double _mu = 0.0;
        double _sigma = 0.0;

        public HitMissFitChart()
        {
            InitializeComponent();

            //all charts should be added through SetupChart()
            //Series.Clear();

            _transformer = new DataTransformer(null, TransformTypeEnum.Linear, 0, 1, null, TransformTypeEnum.Linear, 0, 1);

        }

        public override string TooltipText
        {
            get
            {
                var text = "Model Parameters\n";

                text += $"mu:\t{_mu:F3} {XAxisUnit}\n";
                text += $"sigma:\t{_sigma:F3} {XAxisUnit}";

                return text;
            }
        }


        public void FillChart(IAnalysisData myData, double mu, double sigma)
        {
            //FillChart(myData, myData.ResidualUncensoredTable, "flaw", "t_fit", "flaw", "hitrate", 0, 1.0, mu, sigma, myData.OriginalData);
            FillChart(myData, myData.ResidualUncensoredTable, "transformFlaw", "t_fit", "transformFlaw", "hitrate", 0, 1.0, mu, sigma, myData.OriginalData);
            //FillChart(myData, myData.ResidualUncensoredTable, "flaw", "t_fit", "flaw", "hitrate", 0, 1.0, mu, sigma, myData.OriginalData);
        }

        public void FillChart(IAnalysisData myData, string my90X, string my90Y, string my95X, string my95Y, double mu, double sigma)
        {
            FillChart(myData, myData.ResidualUncensoredTable, my90X, my90Y, my95X, my95Y, 0.0, 1.0, mu, sigma);
        }

        public override void SetupChart(string flawName, string flawUnit, List<string> responseNames, List<string> responseUnits)
        {
            base.SetupChart(flawName, flawUnit, responseNames, responseUnits);

            AutoNameYAxis = false;
            YAxisNameIsUnitlessConstant = true;
            YAxisTitle = "Response";
            XAxisTitle = flawName;
            XAxisUnit = flawUnit;
            ChartTitle = "Model Fit";
        }

        public void FillChart(IAnalysisData myData, DataTable myTable,  string myFitX, string myFitY, 
            string myHitRateX, string myHitRateY, double myYMin, double myYMax, double mu, double sigma, DataTable origData = null)
        {
            DataView origView = origData.DefaultView;
            DataView view = myTable.DefaultView;

            ClearIntervals(XAxis);

            _mu = mu;
            _sigma = sigma;

            FlawEstimate.Points.DataBindXY(view, myFitX, view, myFitY);
            Uncensored.Points.DataBindXY(origView, myHitRateX, origView, myHitRateY);

            ChartAreas[0].AxisY.Minimum = myYMin;
            ChartAreas[0].AxisY.Maximum = myYMax;
            ChartAreas[0].AxisY.Interval = .2;

            var uncensoredMax = Double.NegativeInfinity;
            var uncensoredMin = Double.PositiveInfinity;

            if (Uncensored.Points.Count > 0)
            {
                uncensoredMax = Uncensored.Points.FindMaxByValue("X").XValue;
                uncensoredMin = Uncensored.Points.FindMinByValue("X").XValue;
            }

            if (uncensoredMax < uncensoredMin)
            {
                uncensoredMax = 1.0;
                uncensoredMin = -1.0;
            }

            _transformer.UpdateAxisTransformParameters(null, myData.ResponseTransform, myYMin, myYMax, null, myData.FlawTransform, uncensoredMin, uncensoredMax);

            AxisObject yAxis = new AxisObject(AxisKind.Y);
            AxisObject xAxis = new AxisObject(AxisKind.X);

            //setup axis for typical hitmiss y-axis
            yAxis.Interval = .5;
            yAxis.Min = 0.0;
            yAxis.Max = 1.0;
            yAxis.IntervalOffset = 0.0;
            yAxis.BufferPercentage = 0.0;

            AnalysisData.GetBufferedRange(this, xAxis, uncensoredMin, uncensoredMax, AxisKind.X);
            //AnalysisData.GetBufferedRange(yAxis, myYMin, myYMax);

            


            //xAxis.Interval /= 2.0;
            //yAxis.Interval /= 2.0;

            SetXAxisRange(xAxis, myData, false, false);
            SetYAxisRange(yAxis, myData, false, true);

            //RelabelAxes(xAxis, yAxis,
            //            myData.InvertTransformValueForXAxis, null, 10, 3, false, false, myData.FlawTransform, TransformTypeEnum.Linear);

            yAxis.Interval = .25;
            yAxis.IntervalOffset = 0.0;

            RelabelAxesBetter(xAxis, yAxis, myData.InvertTransformValueForXAxis, null,
                              Globals.GetLabelIntervalBasedOnChartSize(this, AxisKind.X), 5, false, false,
                              myData.FlawTransform,
                              TransformTypeEnum.Linear, myData.TransformValueForXAxis, null, false, true);
        }
    }
}
