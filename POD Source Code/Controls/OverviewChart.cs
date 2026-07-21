// <copyright file="OverviewChart.cs" company="The University of Dayton">
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

namespace POD.Controls
{
    public partial class OverviewChart : UserControl
    {
        string _chartName;
        string _analysisName;
        public string FullAnalysisName;

        public OverviewChart()
        {
            InitializeComponent();
        }

        public OverviewChart(string chartName, string analysisName, DataPointChart chart)
        {
            InitializeComponent();

            _chartName = chartName.Trim();
            _analysisName = analysisName.Trim();
            Chart = chart;
            UpdateLabel();
        }

        public DataPointChart Chart
        {
            set
            {
                var chart = mainTable.GetControlFromPosition(0, 1);

                if (chart != null)
                    mainTable.Controls.Remove(chart);

                mainTable.Controls.Add(value);
                mainTable.SetColumn(value, 0);
                mainTable.SetRow(value, 1);

                value.Dock = DockStyle.Fill;
            }
            get
            {
                return mainTable.GetControlFromPosition(0, 1) as DataPointChart;
            }
        }

        public string ChartName
        {
            set
            {
                _chartName = value.Trim();
                UpdateLabel();
            }
            get
            {
                return _chartName;
            }
        }

        private void UpdateLabel()
        {
            if (_analysisName != null && _analysisName.Length > 0)
            {
                label1.Text = _analysisName;
            }
            else
            {
                if (_chartName != null && _chartName.Length > 0)
                {
                    label1.Text = _chartName;
                }
                else
                {
                    label1.Text = "";
                }
            }
        }

        public string AnalysisName
        {
            set
            {
                _analysisName = value.Trim();
                UpdateLabel();
            }
            get
            {
                return _analysisName;
            }
        }
    }
}
