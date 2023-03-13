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
            if (!this.DesignMode)
                PrepareChart();
        }
        private void PrepareChart()
        {
            this.Series.Clear();
            this.bars = this.Series.Add(POD.NormalityChart.NormalityHistogram);
            this.Series[POD.NormalityChart.NormalityHistogram].Color = POD.ChartColors.a90Color;
            this.Series[POD.NormalityChart.NormalityHistogram].IsVisibleInLegend = false;
            this.normalCurve = this.Series.Add(POD.NormalityChart.NormalCurve);
            this.Series[POD.NormalityChart.NormalCurve].ChartType= SeriesChartType.Line;
            this.Series[POD.NormalityChart.NormalCurve].BorderWidth = 4;
            this.Series[POD.NormalityChart.NormalCurve].Color = POD.ChartColors.a9095Color;
            this.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            this.ChartAreas[0].AxisY.MinorGrid.Enabled = false;
            this.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            this.ChartAreas[0].AxisX.MinorGrid.Enabled = false;
            this.ChartAreas[0].AxisX.Title = "Responses";
            this.ChartAreas[0].AxisY.Title = "Count";
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

                text += "Normality of the transformed responses."+ Environment.NewLine;
                text += "The responses should roughly be in the shape of a normal distribution." + Environment.NewLine;
                text += "If sample size is small, this may not be possible.";

                return text;
            }
        }
    }

}







