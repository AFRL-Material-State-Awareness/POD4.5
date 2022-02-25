using POD.Controls;

namespace POD.Wizards.Steps.FullAnalysisProjectSteps
{
    partial class ProjectPropertiesPanel
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
            this.mainLayoutTable = new System.Windows.Forms.TableLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.customerComboBox = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.notesTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.projectComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.parentComboBox = new System.Windows.Forms.ComboBox();
            this.analystComboBox = new System.Windows.Forms.ComboBox();
            this.analystCoComboBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.customerCoComboBox = new System.Windows.Forms.ComboBox();
            this.overviewFlowPanel = new POD.Controls.PODFlowLayoutPanel();
            this.NoChartLabel = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.mainLayoutTable.SuspendLayout();
            this.overviewFlowPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainLayoutTable
            // 
            this.mainLayoutTable.ColumnCount = 4;
            this.mainLayoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.mainLayoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.mainLayoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.mainLayoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayoutTable.Controls.Add(this.label4, 0, 3);
            this.mainLayoutTable.Controls.Add(this.label9, 0, 0);
            this.mainLayoutTable.Controls.Add(this.customerComboBox, 1, 3);
            this.mainLayoutTable.Controls.Add(this.label6, 0, 4);
            this.mainLayoutTable.Controls.Add(this.notesTextBox, 1, 4);
            this.mainLayoutTable.Controls.Add(this.label2, 0, 1);
            this.mainLayoutTable.Controls.Add(this.projectComboBox, 1, 1);
            this.mainLayoutTable.Controls.Add(this.label3, 2, 1);
            this.mainLayoutTable.Controls.Add(this.label1, 0, 2);
            this.mainLayoutTable.Controls.Add(this.label7, 2, 2);
            this.mainLayoutTable.Controls.Add(this.parentComboBox, 3, 1);
            this.mainLayoutTable.Controls.Add(this.analystComboBox, 1, 2);
            this.mainLayoutTable.Controls.Add(this.analystCoComboBox, 3, 2);
            this.mainLayoutTable.Controls.Add(this.label5, 2, 3);
            this.mainLayoutTable.Controls.Add(this.customerCoComboBox, 3, 3);
            this.mainLayoutTable.Dock = System.Windows.Forms.DockStyle.Left;
            this.mainLayoutTable.Location = new System.Drawing.Point(0, 0);
            this.mainLayoutTable.Name = "mainLayoutTable";
            this.mainLayoutTable.RowCount = 5;
            this.mainLayoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainLayoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainLayoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainLayoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainLayoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayoutTable.Size = new System.Drawing.Size(522, 640);
            this.mainLayoutTable.TabIndex = 0;
            this.mainLayoutTable.Resize += new System.EventHandler(this.MainTable_Resize);
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 74);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Customer POC";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.mainLayoutTable.SetColumnSpan(this.label9, 4);
            this.label9.Dock = System.Windows.Forms.DockStyle.Left;
            this.label9.Location = new System.Drawing.Point(3, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(90, 13);
            this.label9.TabIndex = 1;
            this.label9.Text = "Project Properties";
            this.label9.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // customerComboBox
            // 
            this.customerComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.customerComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.customerComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customerComboBox.FormattingEnabled = true;
            this.customerComboBox.Location = new System.Drawing.Point(85, 70);
            this.customerComboBox.MaximumSize = new System.Drawing.Size(160, 0);
            this.customerComboBox.Name = "customerComboBox";
            this.customerComboBox.Size = new System.Drawing.Size(160, 21);
            this.customerComboBox.TabIndex = 4;
            this.customerComboBox.DropDownClosed += new System.EventHandler(this.comboBox1_DropDownClosed);
            this.customerComboBox.Leave += new System.EventHandler(this.comboBox1_DropDownClosed);
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(44, 101);
            this.label6.Margin = new System.Windows.Forms.Padding(3, 7, 3, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "Notes";
            // 
            // notesTextBox
            // 
            this.mainLayoutTable.SetColumnSpan(this.notesTextBox, 3);
            this.notesTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.notesTextBox.Location = new System.Drawing.Point(85, 97);
            this.notesTextBox.Multiline = true;
            this.notesTextBox.Name = "notesTextBox";
            this.notesTextBox.Size = new System.Drawing.Size(434, 540);
            this.notesTextBox.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Project Name*";
            // 
            // projectComboBox
            // 
            this.projectComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.projectComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.projectComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.projectComboBox.FormattingEnabled = true;
            this.projectComboBox.Location = new System.Drawing.Point(85, 16);
            this.projectComboBox.MaximumSize = new System.Drawing.Size(160, 0);
            this.projectComboBox.Name = "projectComboBox";
            this.projectComboBox.Size = new System.Drawing.Size(160, 21);
            this.projectComboBox.TabIndex = 0;
            this.projectComboBox.DropDownClosed += new System.EventHandler(this.comboBox1_DropDownClosed);
            this.projectComboBox.Leave += new System.EventHandler(this.comboBox1_DropDownClosed);
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(275, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Parent Project";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(38, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Analyst";
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(261, 47);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(88, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Analyst Company";
            // 
            // parentComboBox
            // 
            this.parentComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.parentComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.parentComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.parentComboBox.FormattingEnabled = true;
            this.parentComboBox.Location = new System.Drawing.Point(355, 16);
            this.parentComboBox.MaximumSize = new System.Drawing.Size(160, 0);
            this.parentComboBox.Name = "parentComboBox";
            this.parentComboBox.Size = new System.Drawing.Size(160, 21);
            this.parentComboBox.TabIndex = 1;
            this.parentComboBox.DropDownClosed += new System.EventHandler(this.comboBox1_DropDownClosed);
            this.parentComboBox.Leave += new System.EventHandler(this.comboBox1_DropDownClosed);
            // 
            // analystComboBox
            // 
            this.analystComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.analystComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.analystComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.analystComboBox.FormattingEnabled = true;
            this.analystComboBox.Location = new System.Drawing.Point(85, 43);
            this.analystComboBox.MaximumSize = new System.Drawing.Size(160, 0);
            this.analystComboBox.Name = "analystComboBox";
            this.analystComboBox.Size = new System.Drawing.Size(160, 21);
            this.analystComboBox.TabIndex = 2;
            this.analystComboBox.DropDownClosed += new System.EventHandler(this.comboBox1_DropDownClosed);
            this.analystComboBox.Leave += new System.EventHandler(this.comboBox1_DropDownClosed);
            // 
            // analystCoComboBox
            // 
            this.analystCoComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.analystCoComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.analystCoComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.analystCoComboBox.FormattingEnabled = true;
            this.analystCoComboBox.Location = new System.Drawing.Point(355, 43);
            this.analystCoComboBox.MaximumSize = new System.Drawing.Size(160, 0);
            this.analystCoComboBox.Name = "analystCoComboBox";
            this.analystCoComboBox.Size = new System.Drawing.Size(160, 21);
            this.analystCoComboBox.TabIndex = 3;
            this.analystCoComboBox.DropDownClosed += new System.EventHandler(this.comboBox1_DropDownClosed);
            this.analystCoComboBox.Leave += new System.EventHandler(this.comboBox1_DropDownClosed);
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(251, 74);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(98, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Customer Company";
            // 
            // customerCoComboBox
            // 
            this.customerCoComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.customerCoComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.customerCoComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customerCoComboBox.FormattingEnabled = true;
            this.customerCoComboBox.Location = new System.Drawing.Point(355, 70);
            this.customerCoComboBox.MaximumSize = new System.Drawing.Size(160, 0);
            this.customerCoComboBox.Name = "customerCoComboBox";
            this.customerCoComboBox.Size = new System.Drawing.Size(160, 21);
            this.customerCoComboBox.TabIndex = 5;
            this.customerCoComboBox.DropDownClosed += new System.EventHandler(this.comboBox1_DropDownClosed);
            this.customerCoComboBox.Leave += new System.EventHandler(this.comboBox1_DropDownClosed);
            // 
            // overviewFlowPanel
            // 
            this.overviewFlowPanel.AutoScroll = true;
            this.overviewFlowPanel.AutoSize = true;
            this.overviewFlowPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.overviewFlowPanel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.overviewFlowPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.overviewFlowPanel.Controls.Add(this.NoChartLabel);
            this.overviewFlowPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.overviewFlowPanel.Location = new System.Drawing.Point(3, 16);
            this.overviewFlowPanel.Name = "overviewFlowPanel";
            this.overviewFlowPanel.Size = new System.Drawing.Size(924, 621);
            this.overviewFlowPanel.TabIndex = 7;
            // 
            // NoChartLabel
            // 
            this.NoChartLabel.AutoSize = true;
            this.NoChartLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NoChartLabel.ForeColor = System.Drawing.Color.White;
            this.NoChartLabel.Location = new System.Drawing.Point(3, 0);
            this.NoChartLabel.Name = "NoChartLabel";
            this.NoChartLabel.Size = new System.Drawing.Size(867, 48);
            this.NoChartLabel.TabIndex = 0;
            this.NoChartLabel.Text = "Your project\'s analyses will show up here once they are created. This area can be" +
    " used as an analysis launcher.";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Dock = System.Windows.Forms.DockStyle.Left;
            this.label8.Location = new System.Drawing.Point(3, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(267, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "Project Overview (Double Click Chart to Open Analysis)";
            this.label8.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.overviewFlowPanel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label8, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(522, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(930, 640);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // ProjectPropertiesPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.mainLayoutTable);
            this.DoubleBuffered = true;
            this.Name = "ProjectPropertiesPanel";
            this.Size = new System.Drawing.Size(1452, 640);
            this.mainLayoutTable.ResumeLayout(false);
            this.mainLayoutTable.PerformLayout();
            this.overviewFlowPanel.ResumeLayout(false);
            this.overviewFlowPanel.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainLayoutTable;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox analystComboBox;
        private System.Windows.Forms.ComboBox projectComboBox;
        private System.Windows.Forms.ComboBox parentComboBox;
        private System.Windows.Forms.ComboBox customerComboBox;
        private System.Windows.Forms.ComboBox customerCoComboBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox notesTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox analystCoComboBox;
        private PODFlowLayoutPanel overviewFlowPanel;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label NoChartLabel;
    }
}
