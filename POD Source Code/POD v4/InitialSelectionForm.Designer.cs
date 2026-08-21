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
            this.mainLayoutTable = new System.Windows.Forms.TableLayoutPanel();
            this.NewProjectLabel = new System.Windows.Forms.Label();
            this.HitMissQuickAnalysisLabel = new System.Windows.Forms.Label();
            this.OpenProjectLabel = new System.Windows.Forms.Label();
            this.StartLabel = new System.Windows.Forms.Label();
            this.OverviewLabel = new System.Windows.Forms.Label();
            this.StartTutorialLabel = new System.Windows.Forms.Label();
            this.OverviewTextBox = new System.Windows.Forms.RichTextBox();
            this.AHatQuickAnalysisLabel = new System.Windows.Forms.Label();
            this.RecentLabel = new System.Windows.Forms.Label();
            this.helpViewFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.DefaultViewButton = new System.Windows.Forms.RadioButton();
            this.DualViewButton = new System.Windows.Forms.RadioButton();
            this.LargeViewButton = new System.Windows.Forms.RadioButton();
            this.colorFilterLabel = new System.Windows.Forms.Label();
            this.colorFilterComboBox = new System.Windows.Forms.ComboBox();
            this.colorFilterImage = new System.Windows.Forms.PictureBox();
            this.HelpViewLabel = new System.Windows.Forms.Label();
            this.HelpBox = new System.Windows.Forms.PictureBox();
            this.mruTableLayoutPanel = new POD.Controls.PODTableLayoutPanel(this.components);
            this.labelToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.mainLayoutTable.SuspendLayout();
            this.helpViewFlowLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.colorFilterImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.HelpBox)).BeginInit();
            this.SuspendLayout();
            // 
            // mainLayoutTable
            // 
            this.mainLayoutTable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.mainLayoutTable.ColumnCount = 4;
            this.mainLayoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.mainLayoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.mainLayoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.mainLayoutTable.Controls.Add(this.NewProjectLabel, 0, 2);
            this.mainLayoutTable.Controls.Add(this.HitMissQuickAnalysisLabel, 0, 4);
            this.mainLayoutTable.Controls.Add(this.OpenProjectLabel, 0, 5);
            this.mainLayoutTable.Controls.Add(this.StartLabel, 0, 0);
            this.mainLayoutTable.Controls.Add(this.OverviewLabel, 1, 0);
            this.mainLayoutTable.Controls.Add(this.StartTutorialLabel, 0, 6);
            this.mainLayoutTable.Controls.Add(this.OverviewTextBox, 1, 1);
            this.mainLayoutTable.Controls.Add(this.AHatQuickAnalysisLabel, 0, 3);
            this.mainLayoutTable.Controls.Add(this.RecentLabel, 0, 7);
            this.mainLayoutTable.Controls.Add(this.helpViewFlowLayout, 2, 2);
            this.mainLayoutTable.Controls.Add(this.HelpViewLabel, 2, 0);
            this.mainLayoutTable.Controls.Add(this.HelpBox, 2, 9);
            this.mainLayoutTable.Controls.Add(this.mruTableLayoutPanel, 0, 8);
            this.mainLayoutTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainLayoutTable.Location = new System.Drawing.Point(0, 0);
            this.mainLayoutTable.MinimumSize = new System.Drawing.Size(809, 725);
            this.mainLayoutTable.Name = "mainLayoutTable";
            this.mainLayoutTable.RowCount = 10;
            this.mainLayoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainLayoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainLayoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainLayoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainLayoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainLayoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainLayoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainLayoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainLayoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainLayoutTable.Size = new System.Drawing.Size(809, 751);
            this.mainLayoutTable.TabIndex = 0;
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
            this.mainLayoutTable.SetRowSpan(this.StartLabel, 2);
            this.StartLabel.Size = new System.Drawing.Size(49, 20);
            this.StartLabel.TabIndex = 0;
            this.StartLabel.Text = "Start";
            // 
            // OverviewLabel
            // 
            this.OverviewLabel.AutoSize = true;
            this.OverviewLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OverviewLabel.ForeColor = System.Drawing.Color.White;
            this.OverviewLabel.Location = new System.Drawing.Point(297, 0);
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
            this.OverviewTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.OverviewTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OverviewTextBox.ForeColor = System.Drawing.Color.White;
            this.OverviewTextBox.Location = new System.Drawing.Point(297, 19);
            this.OverviewTextBox.Margin = new System.Windows.Forms.Padding(3, 3, 20, 6);
            this.OverviewTextBox.Name = "OverviewTextBox";
            this.OverviewTextBox.ReadOnly = true;
            this.mainLayoutTable.SetRowSpan(this.OverviewTextBox, 9);
            this.OverviewTextBox.Size = new System.Drawing.Size(344, 726);
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
            // helpViewFlowLayout
            // 
            this.helpViewFlowLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.helpViewFlowLayout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.helpViewFlowLayout.Controls.Add(this.DefaultViewButton);
            this.helpViewFlowLayout.Controls.Add(this.DualViewButton);
            this.helpViewFlowLayout.Controls.Add(this.LargeViewButton);
            this.helpViewFlowLayout.Controls.Add(this.colorFilterLabel);
            this.helpViewFlowLayout.Controls.Add(this.colorFilterComboBox);
            this.helpViewFlowLayout.Controls.Add(this.colorFilterImage);
            this.helpViewFlowLayout.Dock = System.Windows.Forms.DockStyle.Top;
            this.helpViewFlowLayout.Location = new System.Drawing.Point(661, 36);
            this.helpViewFlowLayout.Margin = new System.Windows.Forms.Padding(0);
            this.helpViewFlowLayout.Name = "helpViewFlowLayout";
            this.mainLayoutTable.SetRowSpan(this.helpViewFlowLayout, 7);
            this.helpViewFlowLayout.Size = new System.Drawing.Size(128, 591);
            this.helpViewFlowLayout.TabIndex = 7;
            // 
            // DefaultViewButton
            // 
            this.DefaultViewButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.DefaultViewButton.CheckAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.DefaultViewButton.Checked = true;
            this.DefaultViewButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DefaultViewButton.ForeColor = System.Drawing.Color.White;
            this.DefaultViewButton.Image = ((System.Drawing.Image)(resources.GetObject("DefaultViewButton.Image")));
            this.DefaultViewButton.Location = new System.Drawing.Point(3, 3);
            this.DefaultViewButton.Name = "DefaultViewButton";
            this.DefaultViewButton.Size = new System.Drawing.Size(117, 73);
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
            // DualViewButton
            // 
            this.DualViewButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.DualViewButton.CheckAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.DualViewButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DualViewButton.ForeColor = System.Drawing.Color.White;
            this.DualViewButton.Image = ((System.Drawing.Image)(resources.GetObject("DualViewButton.Image")));
            this.DualViewButton.Location = new System.Drawing.Point(3, 82);
            this.DualViewButton.Name = "DualViewButton";
            this.DualViewButton.Size = new System.Drawing.Size(117, 73);
            this.DualViewButton.TabIndex = 6;
            this.DualViewButton.Text = "Dual";
            this.DualViewButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.DualViewButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.DualViewButton.UseVisualStyleBackColor = true;
            this.DualViewButton.CheckedChanged += new System.EventHandler(this.Update_HelpView);
            this.DualViewButton.MouseEnter += new System.EventHandler(this.HelpView_Enter);
            this.DualViewButton.MouseLeave += new System.EventHandler(this.HelpView_Leave);
            // 
            // LargeViewButton
            // 
            this.LargeViewButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.LargeViewButton.CheckAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.LargeViewButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LargeViewButton.ForeColor = System.Drawing.Color.White;
            this.LargeViewButton.Image = ((System.Drawing.Image)(resources.GetObject("LargeViewButton.Image")));
            this.LargeViewButton.Location = new System.Drawing.Point(3, 161);
            this.LargeViewButton.Name = "LargeViewButton";
            this.LargeViewButton.Size = new System.Drawing.Size(117, 73);
            this.LargeViewButton.TabIndex = 6;
            this.LargeViewButton.Text = "Ultrawide";
            this.LargeViewButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LargeViewButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.LargeViewButton.UseVisualStyleBackColor = true;
            this.LargeViewButton.CheckedChanged += new System.EventHandler(this.Update_HelpView);
            this.LargeViewButton.MouseEnter += new System.EventHandler(this.HelpView_Enter);
            this.LargeViewButton.MouseLeave += new System.EventHandler(this.HelpView_Leave);
            // 
            // colorFilterLabel
            // 
            this.colorFilterLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.colorFilterLabel.BackColor = System.Drawing.Color.Transparent;
            this.colorFilterLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colorFilterLabel.ForeColor = System.Drawing.Color.White;
            this.colorFilterLabel.Location = new System.Drawing.Point(0, 237);
            this.colorFilterLabel.Margin = new System.Windows.Forms.Padding(0);
            this.colorFilterLabel.Name = "colorFilterLabel";
            this.colorFilterLabel.Size = new System.Drawing.Size(128, 36);
            this.colorFilterLabel.TabIndex = 8;
            this.colorFilterLabel.Text = "Color Filters";
            this.colorFilterLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // colorFilterComboBox
            // 
            this.colorFilterComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.colorFilterComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.colorFilterComboBox.FormattingEnabled = true;
            this.colorFilterComboBox.Items.AddRange(new object[] {
            "None",
            "Protanopia [Sim]",
            "Deuteranopia [Sim]",
            "Tritanopia [Sim]",
            "Protanopia [Correct]",
            "Deuteranopia [Correct]",
            "Tritanopia [Correct]"});
            this.colorFilterComboBox.Location = new System.Drawing.Point(3, 273);
            this.colorFilterComboBox.Margin = new System.Windows.Forms.Padding(3, 0, 3, 8);
            this.colorFilterComboBox.Name = "colorFilterComboBox";
            this.colorFilterComboBox.Size = new System.Drawing.Size(125, 21);
            this.colorFilterComboBox.TabIndex = 9;
            this.colorFilterComboBox.SelectedIndexChanged += new System.EventHandler(this.ColorFileComboBox_IndexChanged);
            // 
            // colorFilterImage
            // 
            this.colorFilterImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.colorFilterImage.Image = global::POD.Properties.Resources.colorfilter_preview;
            this.colorFilterImage.Location = new System.Drawing.Point(3, 302);
            this.colorFilterImage.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.colorFilterImage.Name = "colorFilterImage";
            this.colorFilterImage.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.colorFilterImage.Size = new System.Drawing.Size(125, 285);
            this.colorFilterImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.colorFilterImage.TabIndex = 10;
            this.colorFilterImage.TabStop = false;
            this.colorFilterImage.Visible = false;
            // 
            // HelpViewLabel
            // 
            this.HelpViewLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.HelpViewLabel.AutoSize = true;
            this.HelpViewLabel.BackColor = System.Drawing.Color.Transparent;
            this.HelpViewLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HelpViewLabel.ForeColor = System.Drawing.Color.White;
            this.HelpViewLabel.Location = new System.Drawing.Point(664, 8);
            this.HelpViewLabel.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
            this.HelpViewLabel.Name = "HelpViewLabel";
            this.mainLayoutTable.SetRowSpan(this.HelpViewLabel, 2);
            this.HelpViewLabel.Size = new System.Drawing.Size(122, 20);
            this.HelpViewLabel.TabIndex = 4;
            this.HelpViewLabel.Text = "Help View";
            this.HelpViewLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // HelpBox
            // 
            this.HelpBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.HelpBox.BackColor = System.Drawing.Color.Transparent;
            this.HelpBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.HelpBox.Image = global::POD.Properties.Resources.POD_help;
            this.HelpBox.InitialImage = null;
            this.HelpBox.Location = new System.Drawing.Point(664, 633);
            this.HelpBox.Name = "HelpBox";
            this.HelpBox.Size = new System.Drawing.Size(122, 115);
            this.HelpBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.HelpBox.TabIndex = 7;
            this.HelpBox.TabStop = false;
            this.HelpBox.Click += new System.EventHandler(this.Open_Help);
            this.HelpBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Help_MouseDown);
            this.HelpBox.MouseEnter += new System.EventHandler(this.Help_MouseEnter);
            this.HelpBox.MouseLeave += new System.EventHandler(this.Help_MouseLeave);
            this.HelpBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Help_MouseUp);
            // 
            // mruTableLayoutPanel
            // 
            this.mruTableLayoutPanel.ColumnCount = 1;
            this.mruTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mruTableLayoutPanel.Location = new System.Drawing.Point(3, 239);
            this.mruTableLayoutPanel.Name = "mruTableLayoutPanel";
            this.mruTableLayoutPanel.RowCount = 1;
            this.mainLayoutTable.SetRowSpan(this.mruTableLayoutPanel, 2);
            this.mruTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mruTableLayoutPanel.Size = new System.Drawing.Size(288, 509);
            this.mruTableLayoutPanel.TabIndex = 8;
            // 
            // labelToolTip
            // 
            this.labelToolTip.Active = false;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "POD help.png");
            this.imageList1.Images.SetKeyName(1, "POD help mouse over.png");
            this.imageList1.Images.SetKeyName(2, "POD help mouse down.png");
            // 
            // InitialSelectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(809, 751);
            this.Controls.Add(this.mainLayoutTable);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(825, 790);
            this.Name = "InitialSelectionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "UDRI POD Professional v4.5.X - Welcome!";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SelectionForm_Closing);
            this.Shown += new System.EventHandler(this.frmMain_Shown);
            this.mainLayoutTable.ResumeLayout(false);
            this.mainLayoutTable.PerformLayout();
            this.helpViewFlowLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.colorFilterImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.HelpBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainLayoutTable;
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
        private System.Windows.Forms.FlowLayoutPanel helpViewFlowLayout;
        private System.Windows.Forms.RadioButton DefaultViewButton;
        private System.Windows.Forms.RadioButton DualViewButton;
        private System.Windows.Forms.RadioButton LargeViewButton;
        private System.Windows.Forms.PictureBox HelpBox;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.PictureBox colorFilterImage;
        private System.Windows.Forms.ComboBox colorFilterComboBox;
        private System.Windows.Forms.Label colorFilterLabel;
        private Controls.PODTableLayoutPanel mruTableLayoutPanel;
    }
}