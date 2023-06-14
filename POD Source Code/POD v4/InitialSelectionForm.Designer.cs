namespace POD
{
    partial class InitialSelectionForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InitialSelectionForm));
            this.layoutTable = new System.Windows.Forms.TableLayoutPanel();
            this.NewProjectLabel = new System.Windows.Forms.Label();
            this.HitMissQuickAnalysisLabel = new System.Windows.Forms.Label();
            this.OpenProjectLabel = new System.Windows.Forms.Label();
            this.StartLabel = new System.Windows.Forms.Label();
            this.OverviewLabel = new System.Windows.Forms.Label();
            this.StartTutorialLabel = new System.Windows.Forms.Label();
            this.OverviewTextBox = new System.Windows.Forms.RichTextBox();
            this.AHatQuickAnalysisLabel = new System.Windows.Forms.Label();
            this.RecentLabel = new System.Windows.Forms.Label();
            this.HelpViewLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.HelpBox = new System.Windows.Forms.PictureBox();
            this.LargeViewButton = new System.Windows.Forms.RadioButton();
            this.DualViewButton = new System.Windows.Forms.RadioButton();
            this.DefaultViewButton = new System.Windows.Forms.RadioButton();
            this.labelToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.layoutTable.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HelpBox)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutTable
            // 
            this.layoutTable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.layoutTable.ColumnCount = 4;
            this.layoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.layoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.layoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.layoutTable.Controls.Add(this.NewProjectLabel, 0, 2);
            this.layoutTable.Controls.Add(this.HitMissQuickAnalysisLabel, 0, 4);
            this.layoutTable.Controls.Add(this.OpenProjectLabel, 0, 5);
            this.layoutTable.Controls.Add(this.StartLabel, 0, 0);
            this.layoutTable.Controls.Add(this.OverviewLabel, 1, 0);
            this.layoutTable.Controls.Add(this.StartTutorialLabel, 0, 6);
            this.layoutTable.Controls.Add(this.OverviewTextBox, 1, 1);
            this.layoutTable.Controls.Add(this.AHatQuickAnalysisLabel, 0, 3);
            this.layoutTable.Controls.Add(this.RecentLabel, 0, 7);
            this.layoutTable.Controls.Add(this.HelpViewLabel, 2, 0);
            this.layoutTable.Controls.Add(this.panel1, 2, 2);
            this.layoutTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutTable.Location = new System.Drawing.Point(0, 0);
            this.layoutTable.Name = "layoutTable";
            this.layoutTable.RowCount = 8;
            this.layoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.layoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.layoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.layoutTable.Size = new System.Drawing.Size(714, 501);
            this.layoutTable.TabIndex = 0;
            // 
            // NewProjectLabel
            // 
            this.NewProjectLabel.AutoSize = true;
            this.NewProjectLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NewProjectLabel.ForeColor = System.Drawing.Color.White;
            this.NewProjectLabel.Location = new System.Drawing.Point(8, 44);
            this.NewProjectLabel.Margin = new System.Windows.Forms.Padding(8);
            this.NewProjectLabel.Name = "NewProjectLabel";
            this.NewProjectLabel.Size = new System.Drawing.Size(89, 16);
            this.NewProjectLabel.TabIndex = 1;
            this.NewProjectLabel.Text = "New Project...";
            this.NewProjectLabel.Click += new System.EventHandler(this.NewProject_Click);
            this.NewProjectLabel.MouseEnter += new System.EventHandler(this.Label_Enter);
            this.NewProjectLabel.MouseLeave += new System.EventHandler(this.Label_Leave);
            // 
            // HitMissQuickAnalysisLabel
            // 
            this.HitMissQuickAnalysisLabel.AutoSize = true;
            this.HitMissQuickAnalysisLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HitMissQuickAnalysisLabel.ForeColor = System.Drawing.Color.White;
            this.HitMissQuickAnalysisLabel.Location = new System.Drawing.Point(8, 108);
            this.HitMissQuickAnalysisLabel.Margin = new System.Windows.Forms.Padding(8);
            this.HitMissQuickAnalysisLabel.Name = "HitMissQuickAnalysisLabel";
            this.HitMissQuickAnalysisLabel.Size = new System.Drawing.Size(156, 16);
            this.HitMissQuickAnalysisLabel.TabIndex = 1;
            this.HitMissQuickAnalysisLabel.Text = "Hit/Miss Quick Analysis...";
            this.HitMissQuickAnalysisLabel.Click += new System.EventHandler(this.NewQuick_Click);
            this.HitMissQuickAnalysisLabel.MouseEnter += new System.EventHandler(this.Label_Enter);
            this.HitMissQuickAnalysisLabel.MouseLeave += new System.EventHandler(this.Label_Leave);
            // 
            // OpenProjectLabel
            // 
            this.OpenProjectLabel.AutoSize = true;
            this.OpenProjectLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OpenProjectLabel.ForeColor = System.Drawing.Color.White;
            this.OpenProjectLabel.Location = new System.Drawing.Point(8, 140);
            this.OpenProjectLabel.Margin = new System.Windows.Forms.Padding(8);
            this.OpenProjectLabel.Name = "OpenProjectLabel";
            this.OpenProjectLabel.Size = new System.Drawing.Size(184, 16);
            this.OpenProjectLabel.TabIndex = 1;
            this.OpenProjectLabel.Text = "Open Previously Saved File...";
            this.OpenProjectLabel.Click += new System.EventHandler(this.OpenProject_Click);
            this.OpenProjectLabel.Paint += new System.Windows.Forms.PaintEventHandler(this.PaintForm);
            this.OpenProjectLabel.MouseEnter += new System.EventHandler(this.Label_Enter);
            this.OpenProjectLabel.MouseLeave += new System.EventHandler(this.Label_Leave);
            // 
            // StartLabel
            // 
            this.StartLabel.AutoSize = true;
            this.StartLabel.BackColor = System.Drawing.Color.Transparent;
            this.StartLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StartLabel.ForeColor = System.Drawing.Color.White;
            this.StartLabel.Location = new System.Drawing.Point(3, 8);
            this.StartLabel.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
            this.StartLabel.Name = "StartLabel";
            this.layoutTable.SetRowSpan(this.StartLabel, 2);
            this.StartLabel.Size = new System.Drawing.Size(49, 20);
            this.StartLabel.TabIndex = 0;
            this.StartLabel.Text = "Start";
            // 
            // OverviewLabel
            // 
            this.OverviewLabel.AutoSize = true;
            this.OverviewLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OverviewLabel.ForeColor = System.Drawing.Color.White;
            this.OverviewLabel.Location = new System.Drawing.Point(203, 0);
            this.OverviewLabel.Name = "OverviewLabel";
            this.OverviewLabel.Size = new System.Drawing.Size(64, 16);
            this.OverviewLabel.TabIndex = 2;
            this.OverviewLabel.Text = "Overview";
            // 
            // StartTutorialLabel
            // 
            this.StartTutorialLabel.AutoSize = true;
            this.StartTutorialLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StartTutorialLabel.ForeColor = System.Drawing.Color.White;
            this.StartTutorialLabel.Location = new System.Drawing.Point(8, 172);
            this.StartTutorialLabel.Margin = new System.Windows.Forms.Padding(8);
            this.StartTutorialLabel.Name = "StartTutorialLabel";
            this.StartTutorialLabel.Size = new System.Drawing.Size(92, 16);
            this.StartTutorialLabel.TabIndex = 1;
            this.StartTutorialLabel.Text = "Start Tutorial...";
            this.StartTutorialLabel.Click += new System.EventHandler(this.StartTutorial_Click);
            this.StartTutorialLabel.MouseEnter += new System.EventHandler(this.Label_Enter);
            this.StartTutorialLabel.MouseLeave += new System.EventHandler(this.Label_Leave);
            // 
            // OverviewTextBox
            // 
            this.OverviewTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OverviewTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.OverviewTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OverviewTextBox.Location = new System.Drawing.Point(203, 19);
            this.OverviewTextBox.Margin = new System.Windows.Forms.Padding(3, 3, 20, 6);
            this.OverviewTextBox.Name = "OverviewTextBox";
            this.OverviewTextBox.ReadOnly = true;
            this.layoutTable.SetRowSpan(this.OverviewTextBox, 7);
            this.OverviewTextBox.Size = new System.Drawing.Size(359, 480);
            this.OverviewTextBox.TabIndex = 3;
            this.OverviewTextBox.Text = "";
            this.OverviewTextBox.Enter += new System.EventHandler(this.TextBox_Enter);
            // 
            // AHatQuickAnalysisLabel
            // 
            this.AHatQuickAnalysisLabel.AutoSize = true;
            this.AHatQuickAnalysisLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AHatQuickAnalysisLabel.ForeColor = System.Drawing.Color.White;
            this.AHatQuickAnalysisLabel.Location = new System.Drawing.Point(8, 76);
            this.AHatQuickAnalysisLabel.Margin = new System.Windows.Forms.Padding(8);
            this.AHatQuickAnalysisLabel.Name = "AHatQuickAnalysisLabel";
            this.AHatQuickAnalysisLabel.Size = new System.Drawing.Size(165, 16);
            this.AHatQuickAnalysisLabel.TabIndex = 1;
            this.AHatQuickAnalysisLabel.Text = "aHat vs a Quick Analysis...";
            this.AHatQuickAnalysisLabel.Click += new System.EventHandler(this.NewQuick_Click);
            this.AHatQuickAnalysisLabel.MouseEnter += new System.EventHandler(this.Label_Enter);
            this.AHatQuickAnalysisLabel.MouseLeave += new System.EventHandler(this.Label_Leave);
            // 
            // RecentLabel
            // 
            this.RecentLabel.AutoSize = true;
            this.RecentLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RecentLabel.ForeColor = System.Drawing.Color.White;
            this.RecentLabel.Location = new System.Drawing.Point(3, 208);
            this.RecentLabel.Margin = new System.Windows.Forms.Padding(3, 12, 3, 8);
            this.RecentLabel.Name = "RecentLabel";
            this.RecentLabel.Size = new System.Drawing.Size(67, 20);
            this.RecentLabel.TabIndex = 0;
            this.RecentLabel.Text = "Recent";
            // 
            // HelpViewLabel
            // 
            this.HelpViewLabel.AutoSize = true;
            this.HelpViewLabel.BackColor = System.Drawing.Color.Transparent;
            this.HelpViewLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HelpViewLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HelpViewLabel.ForeColor = System.Drawing.Color.White;
            this.HelpViewLabel.Location = new System.Drawing.Point(585, 8);
            this.HelpViewLabel.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
            this.HelpViewLabel.Name = "HelpViewLabel";
            this.layoutTable.SetRowSpan(this.HelpViewLabel, 2);
            this.HelpViewLabel.Size = new System.Drawing.Size(106, 20);
            this.HelpViewLabel.TabIndex = 4;
            this.HelpViewLabel.Text = "Help View";
            this.HelpViewLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.HelpBox);
            this.panel1.Controls.Add(this.LargeViewButton);
            this.panel1.Controls.Add(this.DualViewButton);
            this.panel1.Controls.Add(this.DefaultViewButton);
            this.panel1.Location = new System.Drawing.Point(585, 39);
            this.panel1.Name = "panel1";
            this.layoutTable.SetRowSpan(this.panel1, 6);
            this.panel1.Size = new System.Drawing.Size(106, 463);
            this.panel1.TabIndex = 7;
            // 
            // HelpBox
            // 
            this.HelpBox.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("HelpBox.BackgroundImage")));
            this.HelpBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.HelpBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.HelpBox.InitialImage = null;
            this.HelpBox.Location = new System.Drawing.Point(0, 348);
            this.HelpBox.Name = "HelpBox";
            this.HelpBox.Size = new System.Drawing.Size(106, 115);
            this.HelpBox.TabIndex = 7;
            this.HelpBox.TabStop = false;
            this.HelpBox.Click += new System.EventHandler(this.Open_Help);
            this.HelpBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Help_MouseDown);
            this.HelpBox.MouseEnter += new System.EventHandler(this.Help_MouseEnter);
            this.HelpBox.MouseLeave += new System.EventHandler(this.Help_MouseLeave);
            this.HelpBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Help_MouseUp);
            // 
            // LargeViewButton
            // 
            this.LargeViewButton.AutoSize = true;
            this.LargeViewButton.CheckAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.LargeViewButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.LargeViewButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LargeViewButton.ForeColor = System.Drawing.Color.White;
            this.LargeViewButton.Image = ((System.Drawing.Image)(resources.GetObject("LargeViewButton.Image")));
            this.LargeViewButton.Location = new System.Drawing.Point(0, 146);
            this.LargeViewButton.Name = "LargeViewButton";
            this.LargeViewButton.Size = new System.Drawing.Size(106, 73);
            this.LargeViewButton.TabIndex = 6;
            this.LargeViewButton.Text = "Large";
            this.LargeViewButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LargeViewButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.LargeViewButton.UseVisualStyleBackColor = true;
            this.LargeViewButton.CheckedChanged += new System.EventHandler(this.Update_HelpView);
            this.LargeViewButton.MouseEnter += new System.EventHandler(this.HelpView_Enter);
            this.LargeViewButton.MouseLeave += new System.EventHandler(this.HelpView_Leave);
            // 
            // DualViewButton
            // 
            this.DualViewButton.AutoSize = true;
            this.DualViewButton.CheckAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.DualViewButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.DualViewButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DualViewButton.ForeColor = System.Drawing.Color.White;
            this.DualViewButton.Image = ((System.Drawing.Image)(resources.GetObject("DualViewButton.Image")));
            this.DualViewButton.Location = new System.Drawing.Point(0, 73);
            this.DualViewButton.Name = "DualViewButton";
            this.DualViewButton.Size = new System.Drawing.Size(106, 73);
            this.DualViewButton.TabIndex = 6;
            this.DualViewButton.Text = "Dual";
            this.DualViewButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.DualViewButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.DualViewButton.UseVisualStyleBackColor = true;
            this.DualViewButton.CheckedChanged += new System.EventHandler(this.Update_HelpView);
            this.DualViewButton.MouseEnter += new System.EventHandler(this.HelpView_Enter);
            this.DualViewButton.MouseLeave += new System.EventHandler(this.HelpView_Leave);
            // 
            // DefaultViewButton
            // 
            this.DefaultViewButton.AutoSize = true;
            this.DefaultViewButton.CheckAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.DefaultViewButton.Checked = true;
            this.DefaultViewButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.DefaultViewButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DefaultViewButton.ForeColor = System.Drawing.Color.White;
            this.DefaultViewButton.Image = ((System.Drawing.Image)(resources.GetObject("DefaultViewButton.Image")));
            this.DefaultViewButton.Location = new System.Drawing.Point(0, 0);
            this.DefaultViewButton.Name = "DefaultViewButton";
            this.DefaultViewButton.Size = new System.Drawing.Size(106, 73);
            this.DefaultViewButton.TabIndex = 5;
            this.DefaultViewButton.TabStop = true;
            this.DefaultViewButton.Text = "Default";
            this.DefaultViewButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.DefaultViewButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.DefaultViewButton.UseVisualStyleBackColor = true;
            this.DefaultViewButton.CheckedChanged += new System.EventHandler(this.Update_HelpView);
            this.DefaultViewButton.MouseEnter += new System.EventHandler(this.HelpView_Enter);
            this.DefaultViewButton.MouseLeave += new System.EventHandler(this.HelpView_Leave);
            // 
            // labelToolTip
            // 
            this.labelToolTip.Active = false;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "POD help logo.png");
            this.imageList1.Images.SetKeyName(1, "POD help logo mouse over.png");
            this.imageList1.Images.SetKeyName(2, "POD help logo mouse down.png");
            // 
            // InitialSelectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(714, 501);
            this.Controls.Add(this.layoutTable);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InitialSelectionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "UDRI POD v4.5.3 - Welcome!";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SelectionForm_Closing);
            this.Shown += new System.EventHandler(this.frmMain_Shown);
            this.layoutTable.ResumeLayout(false);
            this.layoutTable.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HelpBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel layoutTable;
        private System.Windows.Forms.Label StartLabel;
        private System.Windows.Forms.Label NewProjectLabel;
        private System.Windows.Forms.Label HitMissQuickAnalysisLabel;
        private System.Windows.Forms.Label OpenProjectLabel;
        private System.Windows.Forms.Label RecentLabel;
        private System.Windows.Forms.Label OverviewLabel;
        private System.Windows.Forms.Label StartTutorialLabel;
        private System.Windows.Forms.ToolTip labelToolTip;
        private System.Windows.Forms.RichTextBox OverviewTextBox;
        private System.Windows.Forms.Label AHatQuickAnalysisLabel;
        private System.Windows.Forms.Label HelpViewLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton DefaultViewButton;
        private System.Windows.Forms.RadioButton DualViewButton;
        private System.Windows.Forms.RadioButton LargeViewButton;
        private System.Windows.Forms.PictureBox HelpBox;
        private System.Windows.Forms.ImageList imageList1;


    }
}