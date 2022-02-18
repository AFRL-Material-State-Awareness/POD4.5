namespace POD.Controls
{
    partial class EditAnalysisDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            this.createTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.AvailableVBar = new POD.Controls.SimpleActionBar();
            this.AnalysisVBar = new POD.Controls.SimpleActionBar();
            this.label3 = new System.Windows.Forms.Label();
            this.AnalysesVBar = new POD.Controls.SimpleActionBar();
            this.analysesTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.analysesChart = new POD.Controls.DataPointChart();
            this.analysesTree = new POD.Controls.MixedCheckBoxesTreeView();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.UpdateBtn = new System.Windows.Forms.Button();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.createTableLayout.SuspendLayout();
            this.analysesTableLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.analysesChart)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // createTableLayout
            // 
            this.createTableLayout.ColumnCount = 6;
            this.createTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.createTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.createTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.createTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.createTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 0F));
            this.createTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 2F));
            this.createTableLayout.Controls.Add(this.label1, 1, 0);
            this.createTableLayout.Controls.Add(this.label2, 3, 0);
            this.createTableLayout.Controls.Add(this.AvailableVBar, 0, 1);
            this.createTableLayout.Controls.Add(this.AnalysisVBar, 2, 1);
            this.createTableLayout.Controls.Add(this.label3, 5, 0);
            this.createTableLayout.Controls.Add(this.AnalysesVBar, 4, 1);
            this.createTableLayout.Controls.Add(this.analysesTableLayout, 5, 1);
            this.createTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.createTableLayout.Location = new System.Drawing.Point(0, 0);
            this.createTableLayout.Name = "createTableLayout";
            this.createTableLayout.RowCount = 3;
            this.createTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.createTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.createTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.createTableLayout.Size = new System.Drawing.Size(810, 478);
            this.createTableLayout.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(9, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(392, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Available Data";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Left;
            this.label2.Location = new System.Drawing.Point(413, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Analysis Name";
            // 
            // AvailableVBar
            // 
            this.AvailableVBar.AutoSize = true;
            this.AvailableVBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AvailableVBar.Location = new System.Drawing.Point(3, 16);
            this.AvailableVBar.Name = "AvailableVBar";
            this.AvailableVBar.Size = new System.Drawing.Size(1, 459);
            this.AvailableVBar.TabIndex = 5;
            // 
            // AnalysisVBar
            // 
            this.AnalysisVBar.AutoSize = true;
            this.AnalysisVBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AnalysisVBar.Location = new System.Drawing.Point(407, 16);
            this.AnalysisVBar.Name = "AnalysisVBar";
            this.AnalysisVBar.Size = new System.Drawing.Size(1, 459);
            this.AnalysisVBar.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(811, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(1, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Analyses";
            // 
            // AnalysesVBar
            // 
            this.AnalysesVBar.AutoSize = true;
            this.AnalysesVBar.Dock = System.Windows.Forms.DockStyle.Left;
            this.AnalysesVBar.Location = new System.Drawing.Point(811, 16);
            this.AnalysesVBar.Name = "AnalysesVBar";
            this.AnalysesVBar.Size = new System.Drawing.Size(0, 459);
            this.AnalysesVBar.TabIndex = 7;
            // 
            // analysesTableLayout
            // 
            this.analysesTableLayout.ColumnCount = 1;
            this.analysesTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.analysesTableLayout.Controls.Add(this.analysesChart, 0, 2);
            this.analysesTableLayout.Controls.Add(this.analysesTree, 0, 1);
            this.analysesTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.analysesTableLayout.Location = new System.Drawing.Point(811, 16);
            this.analysesTableLayout.Name = "analysesTableLayout";
            this.analysesTableLayout.RowCount = 3;
            this.analysesTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.analysesTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.analysesTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.analysesTableLayout.Size = new System.Drawing.Size(1, 459);
            this.analysesTableLayout.TabIndex = 2;
            // 
            // analysesChart
            // 
            this.analysesChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.analysesChart.BorderlineColor = System.Drawing.Color.Transparent;
            this.analysesChart.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            this.analysesChart.CanUnselect = false;
            chartArea1.Name = "ChartArea1";
            this.analysesChart.ChartAreas.Add(chartArea1);
            this.analysesChart.IsSelected = false;
            this.analysesChart.IsSquare = false;
            legend1.Alignment = System.Drawing.StringAlignment.Center;
            legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
            legend1.Enabled = false;
            legend1.Name = "Legend1";
            legend1.TableStyle = System.Windows.Forms.DataVisualization.Charting.LegendTableStyle.Wide;
            this.analysesChart.Legends.Add(legend1);
            this.analysesChart.Location = new System.Drawing.Point(3, 232);
            this.analysesChart.Name = "analysesChart";
            this.analysesChart.Selectable = false;
            this.analysesChart.Size = new System.Drawing.Size(1, 224);
            this.analysesChart.TabIndex = 2;
            // 
            // analysesTree
            // 
            this.analysesTree.CheckBoxes = true;
            this.analysesTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.analysesTree.FullRowSelect = true;
            this.analysesTree.HideSelection = false;
            this.analysesTree.Location = new System.Drawing.Point(3, 3);
            this.analysesTree.Name = "analysesTree";
            this.analysesTree.Size = new System.Drawing.Size(1, 223);
            this.analysesTree.TabIndex = 4;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.UpdateBtn);
            this.flowLayoutPanel1.Controls.Add(this.CancelBtn);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 478);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(810, 39);
            this.flowLayoutPanel1.TabIndex = 8;
            // 
            // UpdateBtn
            // 
            this.UpdateBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.UpdateBtn.Location = new System.Drawing.Point(748, 3);
            this.UpdateBtn.Name = "UpdateBtn";
            this.UpdateBtn.Size = new System.Drawing.Size(59, 33);
            this.UpdateBtn.TabIndex = 0;
            this.UpdateBtn.Text = "Update";
            this.UpdateBtn.UseVisualStyleBackColor = true;
            // 
            // CancelBtn
            // 
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Location = new System.Drawing.Point(683, 3);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(59, 33);
            this.CancelBtn.TabIndex = 1;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.Click += new System.EventHandler(this.CloseDialog_Click);
            // 
            // EditAnalysisDialog
            // 
            this.AcceptButton = this.UpdateBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelBtn;
            this.ClientSize = new System.Drawing.Size(810, 517);
            this.Controls.Add(this.createTableLayout);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "EditAnalysisDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Modify Your Analysis";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.From_Closed);
            this.createTableLayout.ResumeLayout(false);
            this.createTableLayout.PerformLayout();
            this.analysesTableLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.analysesChart)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel createTableLayout;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private SimpleActionBar AvailableVBar;
        private SimpleActionBar AnalysisVBar;
        private System.Windows.Forms.Label label3;
        private SimpleActionBar AnalysesVBar;
        private System.Windows.Forms.TableLayoutPanel analysesTableLayout;
        private DataPointChart analysesChart;
        private MixedCheckBoxesTreeView analysesTree;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button UpdateBtn;
        private System.Windows.Forms.Button CancelBtn;
    }
}