
namespace POD.Wizards.Steps.HitMissNormalSteps
{
    partial class RunAllAnalysesPanel
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            this.podChart1 = new POD.Controls.PODChart();
            this.podTableLayoutPanel1 = new POD.Controls.PODTableLayoutPanel(this.components);
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.podChart1)).BeginInit();
            this.podTableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // podChart1
            // 
            this.podChart1.CanUnselect = false;
            chartArea1.Name = "PODAllAnalyses";
            this.podChart1.ChartAreas.Add(chartArea1);
            this.podChart1.ChartTitle = "";
            this.podChart1.ChartToolTip = null;
            this.podChart1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.podChart1.IsSelected = false;
            this.podChart1.IsSquare = false;
            this.podChart1.Location = new System.Drawing.Point(247, 3);
            this.podChart1.Name = "podChart1";
            this.podTableLayoutPanel1.SetRowSpan(this.podChart1, 4);
            this.podChart1.Selectable = true;
            this.podChart1.ShowChartTitle = true;
            this.podChart1.SingleSeriesCount = 1;
            this.podChart1.Size = new System.Drawing.Size(456, 444);
            this.podChart1.TabIndex = 0;
            this.podChart1.TabStop = false;
            this.podChart1.Text = "podChartAll";
            this.podChart1.XAxisTitle = "";
            this.podChart1.XAxisUnit = "";
            this.podChart1.YAxisTitle = "";
            this.podChart1.YAxisUnit = "";
            // 
            // podTableLayoutPanel1
            // 
            this.podTableLayoutPanel1.ColumnCount = 2;
            this.podTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.podTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.podTableLayoutPanel1.Controls.Add(this.podChart1, 1, 0);
            this.podTableLayoutPanel1.Controls.Add(this.radioButton1, 0, 0);
            this.podTableLayoutPanel1.Controls.Add(this.radioButton2, 0, 2);
            this.podTableLayoutPanel1.Controls.Add(this.checkedListBox1, 0, 3);
            this.podTableLayoutPanel1.Controls.Add(this.groupBox1, 0, 1);
            this.podTableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.podTableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.podTableLayoutPanel1.Name = "podTableLayoutPanel1";
            this.podTableLayoutPanel1.RowCount = 4;
            this.podTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.podTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.podTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.podTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.podTableLayoutPanel1.Size = new System.Drawing.Size(706, 450);
            this.podTableLayoutPanel1.TabIndex = 1;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(3, 3);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(103, 17);
            this.radioButton1.TabIndex = 1;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "POD Percentiles";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(3, 132);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(97, 17);
            this.radioButton2.TabIndex = 2;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "All POD curves";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(3, 155);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(238, 292);
            this.checkedListBox1.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(3, 26);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(238, 100);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // RunAllAnalysesPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.podTableLayoutPanel1);
            this.Name = "RunAllAnalysesPanel";
            this.Size = new System.Drawing.Size(706, 450);
            ((System.ComponentModel.ISupportInitialize)(this.podChart1)).EndInit();
            this.podTableLayoutPanel1.ResumeLayout(false);
            this.podTableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.PODChart podChart1;
        private Controls.PODTableLayoutPanel podTableLayoutPanel1;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}
