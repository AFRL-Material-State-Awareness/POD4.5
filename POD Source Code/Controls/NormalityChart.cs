// <copyright file="NormalityChart.cs" company="The University of Dayton">
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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using POD.Data;
namespace POD.Controls
{
    public partial class NormalityChart : DataPointChart
    {
        Series bars;
        Series normalCurve;
        public NormalityChart()
        {
            InitializeComponent();

            //all charts should be added through SetupChart()
            Series.Clear();
        }

        public override void SetupChart(string flawName, string flawUnit, List<string> responseNames, List<string> responseUnits)
        {
            if (Series.FindByName(POD.NormalityChart.NormalityHistogram) == null)
            {
                this.bars = this.Series.Add(POD.NormalityChart.NormalityHistogram);
                this.Series[POD.NormalityChart.NormalityHistogram].Color = POD.ChartColors.a90Color;
                this.Series[POD.NormalityChart.NormalityHistogram].IsVisibleInLegend = false;
            }

            if (Series.FindByName(POD.NormalityChart.NormalCurve) == null)
            {
                this.normalCurve = this.Series.Add(POD.NormalityChart.NormalCurve);
                this.Series[POD.NormalityChart.NormalCurve].ChartType = SeriesChartType.Line;
                this.Series[POD.NormalityChart.NormalCurve].BorderWidth = 4;
                this.Series[POD.NormalityChart.NormalCurve].Color = POD.ChartColors.a9095Color;
            }

            this.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            this.ChartAreas[0].AxisY.MinorGrid.Enabled = false;
            this.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            this.ChartAreas[0].AxisX.MinorGrid.Enabled = false;
            this.SingleSeriesCount = 3;

            XAxisTitle = $"Residual ({responseUnits[0]})";
            YAxisTitle = "Frequency";
            YAxisNameIsUnitlessConstant = true;
            ChartTitle = "Normality";
        }
        public void FillChart(DataTable normalityTable, DataTable normalCurveTable)
        {
            DataTable freqTable = normalityTable;
            var range = normalityTable.Columns[0].ColumnName;
            var freq = normalityTable.Columns[1].ColumnName;
            var view = normalityTable.DefaultView;
            //plot response freqencies
            NormalityHistogram.Points.DataBindXY(view, range, view, freq);

            var reponse = normalCurveTable.Columns[0].ColumnName;
            var normalDist = normalCurveTable.Columns[1].ColumnName;
            var view2 = normalCurveTable.DefaultView;
            //plot the fitted normal curve
            NormalCurve.Points.DataBindXY(view2, reponse, view2, normalDist);
        }
        public Series NormalityHistogram
        {
            get { return Series[POD.NormalityChart.NormalityHistogram]; }
        }
        public Series NormalCurve
        {
            get { return Series[POD.NormalityChart.NormalCurve]; }
        }
        public override string TooltipText
        {
            get
            {
                var text = "";

                text += "Normality of the transformed responses.\n";
                text += "The responses should roughly be in the shape of a normal distribution.\n";
                text += "If sample size is small, this may not be possible.";

                return text;
            }
        }
    }

}







