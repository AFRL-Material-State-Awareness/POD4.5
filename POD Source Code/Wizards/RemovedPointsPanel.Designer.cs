namespace POD.Wizards
{
    partial class RemovedPointsPanel
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
            this.RemovedPointSplitter = new POD.Controls.SideSplitter();
            this.RemovedPointsTabeLayout = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.RemovedPointsList = new POD.Controls.PODListBox();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DocumentTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.CurrentCommentTextBox = new System.Windows.Forms.TextBox();
            this.TemplateCommentList = new POD.Controls.PODListBox();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.RemovedPointSplitter)).BeginInit();
            this.RemovedPointSplitter.Panel1.SuspendLayout();
            this.RemovedPointSplitter.Panel2.SuspendLayout();
            this.RemovedPointSplitter.SuspendLayout();
            this.RemovedPointsTabeLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RemovedPointsList)).BeginInit();
            this.DocumentTableLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TemplateCommentList)).BeginInit();
            this.SuspendLayout();
            // 
            // RemovedPointSplitter
            // 
            this.RemovedPointSplitter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RemovedPointSplitter.Location = new System.Drawing.Point(0, 0);
            this.RemovedPointSplitter.Margin = new System.Windows.Forms.Padding(0);
            this.RemovedPointSplitter.Name = "RemovedPointSplitter";
            // 
            // RemovedPointSplitter.Panel1
            // 
            this.RemovedPointSplitter.Panel1.Controls.Add(this.RemovedPointsTabeLayout);
            // 
            // RemovedPointSplitter.Panel2
            // 
            this.RemovedPointSplitter.Panel2.Controls.Add(this.DocumentTableLayout);
            this.RemovedPointSplitter.Size = new System.Drawing.Size(1193, 717);
            this.RemovedPointSplitter.SplitterDistance = 397;
            this.RemovedPointSplitter.TabIndex = 1;
            // 
            // RemovedPointsTabeLayout
            // 
            this.RemovedPointsTabeLayout.AutoSize = true;
            this.RemovedPointsTabeLayout.ColumnCount = 1;
            this.RemovedPointsTabeLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.RemovedPointsTabeLayout.Controls.Add(this.label1, 0, 0);
            this.RemovedPointsTabeLayout.Controls.Add(this.RemovedPointsList, 0, 1);
            this.RemovedPointsTabeLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RemovedPointsTabeLayout.Location = new System.Drawing.Point(0, 0);
            this.RemovedPointsTabeLayout.Margin = new System.Windows.Forms.Padding(0);
            this.RemovedPointsTabeLayout.Name = "RemovedPointsTabeLayout";
            this.RemovedPointsTabeLayout.RowCount = 2;
            this.RemovedPointsTabeLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.RemovedPointsTabeLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.RemovedPointsTabeLayout.Size = new System.Drawing.Size(397, 717);
            this.RemovedPointsTabeLayout.TabIndex = 0;
            this.RemovedPointsTabeLayout.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.ControlText;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.Window;
            this.label1.Location = new System.Drawing.Point(0, 3);
            this.label1.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(398, 20);
            this.label1.TabIndex = 7;
            this.label1.Text = "REMOVED POINTS";
            // 
            // RemovedPointsList
            // 
            this.RemovedPointsList.AllowUserToAddRows = false;
            this.RemovedPointsList.AllowUserToDeleteRows = false;
            this.RemovedPointsList.AllowUserToResizeColumns = false;
            this.RemovedPointsList.AllowUserToResizeRows = false;
            this.RemovedPointsList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.RemovedPointsList.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            this.RemovedPointsList.BackgroundColor = System.Drawing.SystemColors.Window;
            this.RemovedPointsList.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.RemovedPointsList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.RemovedPointsList.ColumnHeadersVisible = false;
            this.RemovedPointsList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn4,
            this.dataGridViewTextBoxColumn1});
            this.RemovedPointsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RemovedPointsList.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.RemovedPointsList.GridColor = System.Drawing.SystemColors.Window;
            this.RemovedPointsList.Location = new System.Drawing.Point(1, 24);
            this.RemovedPointsList.Margin = new System.Windows.Forms.Padding(1);
            this.RemovedPointsList.Name = "RemovedPointsList";
            this.RemovedPointsList.ReadOnly = true;
            this.RemovedPointsList.RowHeadersVisible = false;
            this.RemovedPointsList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.RemovedPointsList.SingleSelectedIndex = -1;
            this.RemovedPointsList.Size = new System.Drawing.Size(396, 692);
            this.RemovedPointsList.StandardTab = true;
            this.RemovedPointsList.TabIndex = 0;
            this.RemovedPointsList.Resize += new System.EventHandler(this.List_Resize);
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // DocumentTableLayout
            // 
            this.DocumentTableLayout.ColumnCount = 3;
            this.DocumentTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.DocumentTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.DocumentTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.DocumentTableLayout.Controls.Add(this.label4, 1, 3);
            this.DocumentTableLayout.Controls.Add(this.label2, 1, 0);
            this.DocumentTableLayout.Controls.Add(this.label3, 1, 1);
            this.DocumentTableLayout.Controls.Add(this.CurrentCommentTextBox, 1, 2);
            this.DocumentTableLayout.Controls.Add(this.TemplateCommentList, 1, 4);
            this.DocumentTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DocumentTableLayout.Location = new System.Drawing.Point(0, 0);
            this.DocumentTableLayout.Name = "DocumentTableLayout";
            this.DocumentTableLayout.RowCount = 5;
            this.DocumentTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.DocumentTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.DocumentTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.DocumentTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.DocumentTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.DocumentTableLayout.Size = new System.Drawing.Size(792, 717);
            this.DocumentTableLayout.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 62);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(330, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Comment Templates (Click to Copy to Comment, Shift+Click to Apply)";
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.SystemColors.ControlText;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.Window;
            this.label2.Location = new System.Drawing.Point(0, 3);
            this.label2.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(792, 20);
            this.label2.TabIndex = 8;
            this.label2.Text = "DOCUMENTATION";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(354, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Point Removal Comment (Enter to Apply, Shift+Enter to Add to Templates)";
            // 
            // CurrentCommentTextBox
            // 
            this.CurrentCommentTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CurrentCommentTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.CurrentCommentTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.CurrentCommentTextBox.Location = new System.Drawing.Point(3, 39);
            this.CurrentCommentTextBox.Name = "CurrentCommentTextBox";
            this.CurrentCommentTextBox.Size = new System.Drawing.Size(786, 20);
            this.CurrentCommentTextBox.TabIndex = 1;
            this.CurrentCommentTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyDown);
            // 
            // TemplateCommentList
            // 
            this.TemplateCommentList.AllowUserToAddRows = false;
            this.TemplateCommentList.AllowUserToDeleteRows = false;
            this.TemplateCommentList.AllowUserToResizeColumns = false;
            this.TemplateCommentList.AllowUserToResizeRows = false;
            this.TemplateCommentList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.TemplateCommentList.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            this.TemplateCommentList.BackgroundColor = System.Drawing.SystemColors.Window;
            this.TemplateCommentList.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.TemplateCommentList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.TemplateCommentList.ColumnHeadersVisible = false;
            this.TemplateCommentList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn6,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn7,
            this.dataGridViewTextBoxColumn5,
            this.dataGridViewTextBoxColumn3});
            this.TemplateCommentList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TemplateCommentList.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.TemplateCommentList.GridColor = System.Drawing.SystemColors.Window;
            this.TemplateCommentList.Location = new System.Drawing.Point(3, 78);
            this.TemplateCommentList.MultiSelect = false;
            this.TemplateCommentList.Name = "TemplateCommentList";
            this.TemplateCommentList.ReadOnly = true;
            this.TemplateCommentList.RowHeadersVisible = false;
            this.TemplateCommentList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.TemplateCommentList.SingleSelectedIndex = -1;
            this.TemplateCommentList.Size = new System.Drawing.Size(786, 636);
            this.TemplateCommentList.StandardTab = true;
            this.TemplateCommentList.TabIndex = 2;
            this.TemplateCommentList.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Cell_Clicked);
            this.TemplateCommentList.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Cell_Clicked);
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // RemovedPointsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.RemovedPointSplitter);
            this.Name = "RemovedPointsPanel";
            this.Size = new System.Drawing.Size(1193, 717);
            this.RemovedPointSplitter.Panel1.ResumeLayout(false);
            this.RemovedPointSplitter.Panel1.PerformLayout();
            this.RemovedPointSplitter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.RemovedPointSplitter)).EndInit();
            this.RemovedPointSplitter.ResumeLayout(false);
            this.RemovedPointsTabeLayout.ResumeLayout(false);
            this.RemovedPointsTabeLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RemovedPointsList)).EndInit();
            this.DocumentTableLayout.ResumeLayout(false);
            this.DocumentTableLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TemplateCommentList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel RemovedPointsTabeLayout;
        private System.Windows.Forms.Label label1;
        private Controls.PODListBox RemovedPointsList;
        private POD.Controls.SideSplitter RemovedPointSplitter;
        private System.Windows.Forms.TableLayoutPanel DocumentTableLayout;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox CurrentCommentTextBox;
        private Controls.PODListBox TemplateCommentList;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
    }
}
