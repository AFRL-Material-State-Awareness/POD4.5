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

                if (_chartName != null && _chartName.Length > 0)
                {
                    //label1.Text += " (" + _chartName + ")";
                }
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
