// <copyright file="LinearityChart.cs" company="The University of Dayton">
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

namespace POD.Controls
{
    public partial class LinearityChart : DataPointChart
    {


        public LinearityChart()
        {
            InitializeComponent();

            Selectable = false;
            Enabled = true;

            //all charts should be added through SetupChart()
            Series.Clear();
        }

        public override void SetupChart(string flawName, string flawUnit, List<string> responseNames, List<string> responseUnits)
        {
            base.SetupChart(flawName, flawUnit, responseNames, responseUnits);

            Series series;

            if (Series.FindByName(LinearityChartLabels.FlawEstimate) == null)
            {
                //draw the residual flaw estimate line (basically straight line at 0)
                Series.Add(new Series(LinearityChartLabels.FlawEstimate));
                series = Series.Last();
                series.IsVisibleInLegend = false;
                series.ChartType = SeriesChartType.Line;
                series.IsXValueIndexed = false;
                series.YValuesPerPoint = 1;
                series.BorderWidth = 3;
                series.Color = Color.FromArgb(ChartColors.LineAlpha, ChartColors.FitColor);
            }

            if (Series.FindByName(LinearityChartLabels.Uncensored) == null)
            {
                //draw residual for uncensored data
                Series.Add(new Series(LinearityChartLabels.Uncensored));
                series = Series.Last();
                series.ChartType = SeriesChartType.Point;
                series.YValuesPerPoint = 1;
                series.Color = Color.FromArgb(ChartColors.LineAlpha, ChartColors.UncensoredPoints);
                series.MarkerStyle = MarkerStyle.Circle;
                series.IsVisibleInLegend = false;
            }

            this.SingleSeriesCount = 3;

            XAxisTitle = flawName;
            XAxisUnit = flawUnit;
            ForceZeroXAxis = true;
            YAxisTitle = $"Residual";
            YAxisUnit = responseUnits[0];
            ChartTitle = "Linearity";
        }

        public override void ClearEverythingButPoints()
        {

            base.ClearEverythingButPoints();

            MakeFlatLine(Series[LinearityChartLabels.FlawEstimate].Points);

            Series[LinearityChartLabels.Uncensored].Points.Clear();
        }

        

        public Series FlawEstimate
        {
            get
            {
                return Series[LinearityChartLabels.FlawEstimate];
            }
        }

        public Series Uncensored
        {
            get
            {
                return Series[LinearityChartLabels.Uncensored];
            }
        }

        
    }
}
