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
        System.Windows.Forms.DataVisualization.Charting.Series bars;
        public NormalityChart()
        {
            InitializeComponent();
            if (!this.DesignMode)
                PrepareChart();
        }
        private void PrepareChart()
        {
            this.Series.Clear();
            bars = this.Series.Add(POD.NormalityChart.NormalityHistogram);
            this.Series[POD.NormalityChart.NormalityHistogram].Color = POD.ChartColors.a90Color;
            this.Series[POD.NormalityChart.NormalityHistogram].IsVisibleInLegend = false;
            this.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            this.ChartAreas[0].AxisY.MinorGrid.Enabled = false;
            this.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            this.ChartAreas[0].AxisX.MinorGrid.Enabled = false;
            this.ChartAreas[0].AxisX.Title = "Responses";
            this.ChartAreas[0].AxisY.Title = "Count";
        }
        public void FillChart(DataTable NormalityTable)
        {
            DataTable freqTable = NormalityTable;
            var range = NormalityTable.Columns[0].ColumnName;
            var freq = NormalityTable.Columns[1].ColumnName;
            var view = NormalityTable.DefaultView;
            NormalityHistogram.Points.DataBindXY(view, range, view, freq);

            //for (int i = 0; i < freq; i++)
            //{
            //    bars.Points.AddXY(range[i], freq[i]);
            //}

        }
        public Series NormalityHistogram
        {
            get { return Series[POD.NormalityChart.NormalityHistogram]; }
        }
    }
}







