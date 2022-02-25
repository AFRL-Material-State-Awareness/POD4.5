namespace POD.Controls
{
    partial class AnalysisType
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnalysisType));
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.AnalysisTypeLabel = new System.Windows.Forms.Label();
            this.AnalysisTextBox = new System.Windows.Forms.RichTextBox();
            this.StartButton = new System.Windows.Forms.Button();
            this.SkillTextBox = new System.Windows.Forms.RichTextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.SkillLevelCombo = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.AnalysisTypeLabel, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.AnalysisTextBox, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.StartButton, 0, 5);
            this.tableLayoutPanel2.Controls.Add(this.SkillTextBox, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel1, 0, 2);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.Padding = new System.Windows.Forms.Padding(10);
            this.tableLayoutPanel2.RowCount = 7;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(267, 302);
            this.tableLayoutPanel2.TabIndex = 7;
            // 
            // AnalysisTypeLabel
            // 
            this.AnalysisTypeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AnalysisTypeLabel.AutoSize = true;
            this.AnalysisTypeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AnalysisTypeLabel.ForeColor = System.Drawing.Color.White;
            this.AnalysisTypeLabel.Location = new System.Drawing.Point(14, 10);
            this.AnalysisTypeLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.AnalysisTypeLabel.Name = "AnalysisTypeLabel";
            this.AnalysisTypeLabel.Size = new System.Drawing.Size(239, 18);
            this.AnalysisTypeLabel.TabIndex = 5;
            this.AnalysisTypeLabel.Text = "Full Analysis";
            this.AnalysisTypeLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // AnalysisTextBox
            // 
            this.AnalysisTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AnalysisTextBox.Location = new System.Drawing.Point(13, 31);
            this.AnalysisTextBox.Name = "AnalysisTextBox";
            this.AnalysisTextBox.Size = new System.Drawing.Size(241, 114);
            this.AnalysisTextBox.TabIndex = 8;
            this.AnalysisTextBox.Text = resources.GetString("AnalysisTextBox.Text");
            // 
            // StartButton
            // 
            this.StartButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.StartButton.AutoSize = true;
            this.StartButton.Location = new System.Drawing.Point(81, 261);
            this.StartButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(105, 31);
            this.StartButton.TabIndex = 1;
            this.StartButton.Text = "Start Full Analysis";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // SkillTextBox
            // 
            this.SkillTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SkillTextBox.Location = new System.Drawing.Point(13, 188);
            this.SkillTextBox.Name = "SkillTextBox";
            this.SkillTextBox.Size = new System.Drawing.Size(241, 65);
            this.SkillTextBox.TabIndex = 8;
            this.SkillTextBox.Text = "Tutorial helps you learn how to use the user interface with a pre-defined problem" +
    ". This is perfect for first time users.";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.SkillLevelCombo, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(13, 151);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(241, 31);
            this.tableLayoutPanel1.TabIndex = 9;
            // 
            // SkillLevelCombo
            // 
            this.SkillLevelCombo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SkillLevelCombo.FormattingEnabled = true;
            this.SkillLevelCombo.Items.AddRange(new object[] {
            "Tutorial",
            "Beginner",
            "Intermediate",
            "Expert"});
            this.SkillLevelCombo.Location = new System.Drawing.Point(70, 5);
            this.SkillLevelCombo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.SkillLevelCombo.Name = "SkillLevelCombo";
            this.SkillLevelCombo.Size = new System.Drawing.Size(167, 21);
            this.SkillLevelCombo.TabIndex = 2;
            this.SkillLevelCombo.Text = "Tutorial";
            this.SkillLevelCombo.SelectedIndexChanged += new System.EventHandler(this.SkillLevel_Changed);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(0, 0);
            this.label5.Margin = new System.Windows.Forms.Padding(0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(66, 31);
            this.label5.TabIndex = 5;
            this.label5.Text = "Skill Level";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // AnalysisType
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.tableLayoutPanel2);
            this.Name = "AnalysisType";
            this.Size = new System.Drawing.Size(267, 302);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label AnalysisTypeLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox SkillLevelCombo;
        private System.Windows.Forms.RichTextBox AnalysisTextBox;
        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.RichTextBox SkillTextBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
