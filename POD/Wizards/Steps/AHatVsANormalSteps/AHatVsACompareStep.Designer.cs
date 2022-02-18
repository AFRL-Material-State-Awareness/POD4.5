namespace POD.Wizards.Steps.AHatVsANormalSteps
{
    partial class AHatVsACompareStep01
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.linearLinearChart = new POD.Controls.DataPointChart();
            this.linearLogChart = new POD.Controls.DataPointChart();
            this.logLogChart = new POD.Controls.DataPointChart();
            this.logLinearChart = new POD.Controls.DataPointChart();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.linearLinearChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.linearLogChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.logLogChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.logLinearChart)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.linearLinearChart, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.linearLogChart, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.logLogChart, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.logLinearChart, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(739, 483);
            this.tableLayoutPanel1.TabIndex = 1;
            this.tableLayoutPanel1.Click += new System.EventHandler(this.Panel_Click);
            // 
            // linearLinearChart
            // 
            chartArea1.Name = "ChartArea1";
            this.linearLinearChart.ChartAreas.Add(chartArea1);
            this.linearLinearChart.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.linearLinearChart.Legends.Add(legend1);
            this.linearLinearChart.Location = new System.Drawing.Point(3, 3);
            this.linearLinearChart.Name = "linearLinearChart";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series1.Legend = "Legend1";
            series1.Name = "36";
            this.linearLinearChart.Series.Add(series1);
            this.linearLinearChart.Size = new System.Drawing.Size(363, 235);
            this.linearLinearChart.TabIndex = 0;
            this.linearLinearChart.Text = "aHatVsAChart1";
            this.linearLinearChart.Click += new System.EventHandler(this.Panel_Click);
            // 
            // linearLogChart
            // 
            chartArea2.Name = "ChartArea1";
            this.linearLogChart.ChartAreas.Add(chartArea2);
            this.linearLogChart.Dock = System.Windows.Forms.DockStyle.Fill;
            legend2.Name = "Legend1";
            this.linearLogChart.Legends.Add(legend2);
            this.linearLogChart.Location = new System.Drawing.Point(372, 3);
            this.linearLogChart.Name = "linearLogChart";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series2.Legend = "Legend1";
            series2.Name = "36";
            this.linearLogChart.Series.Add(series2);
            this.linearLogChart.Size = new System.Drawing.Size(364, 235);
            this.linearLogChart.TabIndex = 1;
            this.linearLogChart.Text = "aHatVsAChart2";
            this.linearLogChart.Click += new System.EventHandler(this.Panel_Click);
            // 
            // logLogChart
            // 
            chartArea3.Name = "ChartArea1";
            this.logLogChart.ChartAreas.Add(chartArea3);
            this.logLogChart.Dock = System.Windows.Forms.DockStyle.Fill;
            legend3.Name = "Legend1";
            this.logLogChart.Legends.Add(legend3);
            this.logLogChart.Location = new System.Drawing.Point(372, 244);
            this.logLogChart.Name = "logLogChart";
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series3.Legend = "Legend1";
            series3.Name = "20";
            this.logLogChart.Series.Add(series3);
            this.logLogChart.Size = new System.Drawing.Size(364, 236);
            this.logLogChart.TabIndex = 2;
            this.logLogChart.Text = "aHatVsAChart3";
            this.logLogChart.Click += new System.EventHandler(this.Panel_Click);
            // 
            // logLinearChart
            // 
            chartArea4.Name = "ChartArea1";
            this.logLinearChart.ChartAreas.Add(chartArea4);
            this.logLinearChart.Dock = System.Windows.Forms.DockStyle.Fill;
            legend4.Name = "Legend1";
            this.logLinearChart.Legends.Add(legend4);
            this.logLinearChart.Location = new System.Drawing.Point(3, 244);
            this.logLinearChart.Name = "logLinearChart";
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series4.Legend = "Legend1";
            series4.Name = "20";
            this.logLinearChart.Series.Add(series4);
            this.logLinearChart.Size = new System.Drawing.Size(363, 236);
            this.logLinearChart.TabIndex = 3;
            this.logLinearChart.Text = "aHatVsAChart4";
            this.logLinearChart.Click += new System.EventHandler(this.Panel_Click);
            // 
            // AHatVsACompareStep01
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "AHatVsACompareStep01";
            this.Click += new System.EventHandler(this.Panel_Click);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.linearLinearChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.linearLogChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.logLogChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.logLinearChart)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Controls.DataPointChart linearLinearChart;
        private Controls.DataPointChart linearLogChart;
        private Controls.DataPointChart logLogChart;
        private Controls.DataPointChart logLinearChart;
    }
}
