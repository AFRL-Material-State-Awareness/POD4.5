namespace POD.Controls
{
    partial class OverviewChart
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
            this.mainTable = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.dataPointChart1 = new POD.Controls.DataPointChart();
            this.mainTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataPointChart1)).BeginInit();
            this.SuspendLayout();
            // 
            // mainTable
            // 
            this.mainTable.ColumnCount = 1;
            this.mainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTable.Controls.Add(this.label1, 0, 0);
            this.mainTable.Controls.Add(this.dataPointChart1, 0, 1);
            this.mainTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTable.Location = new System.Drawing.Point(0, 0);
            this.mainTable.Name = "mainTable";
            this.mainTable.RowCount = 2;
            this.mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTable.Size = new System.Drawing.Size(326, 339);
            this.mainTable.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(320, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "label1";
            // 
            // dataPointChart1
            // 
            this.dataPointChart1.CanUnselect = false;
            this.dataPointChart1.ChartTitle = "";
            this.dataPointChart1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataPointChart1.IsSelected = false;
            this.dataPointChart1.IsSquare = true;
            this.dataPointChart1.Location = new System.Drawing.Point(3, 19);
            this.dataPointChart1.Name = "dataPointChart1";
            this.dataPointChart1.Selectable = false;
            this.dataPointChart1.ShowChartTitle = true;
            this.dataPointChart1.SingleSeriesCount = 1;
            this.dataPointChart1.Size = new System.Drawing.Size(320, 320);
            this.dataPointChart1.TabIndex = 1;
            this.dataPointChart1.Text = "dataPointChart1";
            this.dataPointChart1.XAxisTitle = "";
            this.dataPointChart1.XAxisUnit = "";
            this.dataPointChart1.YAxisTitle = "";
            this.dataPointChart1.YAxisUnit = "";
            // 
            // OverviewChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.mainTable);
            this.Name = "OverviewChart";
            this.Size = new System.Drawing.Size(326, 339);
            this.mainTable.ResumeLayout(false);
            this.mainTable.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataPointChart1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainTable;
        private System.Windows.Forms.Label label1;
        private DataPointChart dataPointChart1;
    }
}
