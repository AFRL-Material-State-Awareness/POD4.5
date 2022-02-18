namespace POD.Wizards.Steps.FullAnalysisProjectSteps
{
    partial class PasteDataPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PasteDataPanel));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.contextMenuStrip1 = new POD.Controls.ContextMenuStripConnected(this.components);
            this.CellIcons = new System.Windows.Forms.ImageList(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.DataSourceTabs = new POD.Controls.PODTabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.DefaultDataGrid = new POD.Controls.DataGridViewDB();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.PaintControlBackPnl = new System.Windows.Forms.Panel();
            this.PaintControlTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.UndefinePnl = new System.Windows.Forms.Panel();
            this.UndefineRadio = new System.Windows.Forms.RadioButton();
            this.ResponsePnl = new System.Windows.Forms.Panel();
            this.DefineResponseRadio = new System.Windows.Forms.RadioButton();
            this.FlawPnl = new System.Windows.Forms.Panel();
            this.DefineFlawRadio = new System.Windows.Forms.RadioButton();
            this.IDColorPnl = new System.Windows.Forms.Panel();
            this.MetaDataPnl = new System.Windows.Forms.Panel();
            this.DefineMetadataRadio = new System.Windows.Forms.RadioButton();
            this.InactiveColorPnl = new System.Windows.Forms.Panel();
            this.IDPnl = new System.Windows.Forms.Panel();
            this.DefineIDRadio = new System.Windows.Forms.RadioButton();
            this.EditCellPnl = new System.Windows.Forms.Panel();
            this.InactiveRadio = new System.Windows.Forms.RadioButton();
            this.MetaDataColorPnl = new System.Windows.Forms.Panel();
            this.FlawColorPnl = new System.Windows.Forms.Panel();
            this.ResponseColorPnl = new System.Windows.Forms.Panel();
            this.UndefineColorPnl = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1.SuspendLayout();
            this.DataSourceTabs.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DefaultDataGrid)).BeginInit();
            this.PaintControlBackPnl.SuspendLayout();
            this.PaintControlTableLayout.SuspendLayout();
            this.UndefinePnl.SuspendLayout();
            this.ResponsePnl.SuspendLayout();
            this.FlawPnl.SuspendLayout();
            this.MetaDataPnl.SuspendLayout();
            this.IDPnl.SuspendLayout();
            this.EditCellPnl.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.IsTextboxMenu = false;
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.ShowOnlyButtons = false;
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // CellIcons
            // 
            this.CellIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("CellIcons.ImageStream")));
            this.CellIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.CellIcons.Images.SetKeyName(0, "Error.png");
            this.CellIcons.Images.SetKeyName(1, "Row Error.png");
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.DataSourceTabs, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.PaintControlBackPnl, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1223, 170);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoEllipsis = true;
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.SystemColors.WindowText;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.Window;
            this.label2.Location = new System.Drawing.Point(0, 67);
            this.label2.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(277, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "AVAILABLE DATA SOURCES";
            // 
            // DataSourceTabs
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.DataSourceTabs, 2);
            this.DataSourceTabs.Controls.Add(this.tabPage1);
            this.DataSourceTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DataSourceTabs.Location = new System.Drawing.Point(0, 92);
            this.DataSourceTabs.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.DataSourceTabs.Name = "DataSourceTabs";
            this.DataSourceTabs.SelectedIndex = 0;
            this.DataSourceTabs.Size = new System.Drawing.Size(1223, 78);
            this.DataSourceTabs.TabIndex = 4;
            this.DataSourceTabs.SelectedIndexChanged += new System.EventHandler(this.Grid_NewTabSelected);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.DefaultDataGrid);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1215, 52);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Data Source 01";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // DefaultDataGrid
            // 
            this.DefaultDataGrid.AllowUserToResizeRows = false;
            this.DefaultDataGrid.AutoGenerateColumns = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlDark;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Transparent;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DefaultDataGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.DefaultDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlDark;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Transparent;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DefaultDataGrid.DefaultCellStyle = dataGridViewCellStyle2;
            this.DefaultDataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DefaultDataGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.DefaultDataGrid.Location = new System.Drawing.Point(3, 3);
            this.DefaultDataGrid.MultiSelect = false;
            this.DefaultDataGrid.Name = "DefaultDataGrid";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlDark;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DefaultDataGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.DefaultDataGrid.RowHeadersVisible = false;
            this.DefaultDataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullColumnSelect;
            this.DefaultDataGrid.Size = new System.Drawing.Size(1209, 46);
            this.DefaultDataGrid.StandardTab = true;
            this.DefaultDataGrid.TabIndex = 0;
            this.DefaultDataGrid.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Cell_Clicked);
            this.DefaultDataGrid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.cell_DoubleClicked);
            this.DefaultDataGrid.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.Cell_CellPainting);
            this.DefaultDataGrid.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.ColumnHeader_Clicked);
            this.DefaultDataGrid.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Grid_Clicked);
            this.DefaultDataGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Grid_MouseDown);
            this.DefaultDataGrid.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Grid_MouseMove);
            this.DefaultDataGrid.MouseUp += new System.Windows.Forms.MouseEventHandler(this.CheckShift_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.SystemColors.WindowText;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.Window;
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(277, 20);
            this.label3.TabIndex = 0;
            this.label3.Text = "DATA SOURCE EDITING TOOLS";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Left;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(280, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(286, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Define purpose of columns by clicking on them.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // PaintControlBackPnl
            // 
            this.PaintControlBackPnl.BackColor = System.Drawing.SystemColors.WindowText;
            this.tableLayoutPanel1.SetColumnSpan(this.PaintControlBackPnl, 2);
            this.PaintControlBackPnl.Controls.Add(this.PaintControlTableLayout);
            this.PaintControlBackPnl.Location = new System.Drawing.Point(0, 20);
            this.PaintControlBackPnl.Margin = new System.Windows.Forms.Padding(0);
            this.PaintControlBackPnl.Name = "PaintControlBackPnl";
            this.PaintControlBackPnl.Padding = new System.Windows.Forms.Padding(5);
            this.PaintControlBackPnl.Size = new System.Drawing.Size(759, 42);
            this.PaintControlBackPnl.TabIndex = 2;
            // 
            // PaintControlTableLayout
            // 
            this.PaintControlTableLayout.AutoSize = true;
            this.PaintControlTableLayout.ColumnCount = 12;
            this.PaintControlTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.PaintControlTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.PaintControlTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.PaintControlTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.PaintControlTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.PaintControlTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.PaintControlTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.PaintControlTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.PaintControlTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.PaintControlTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.PaintControlTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.PaintControlTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.PaintControlTableLayout.Controls.Add(this.UndefinePnl, 11, 0);
            this.PaintControlTableLayout.Controls.Add(this.ResponsePnl, 9, 0);
            this.PaintControlTableLayout.Controls.Add(this.FlawPnl, 7, 0);
            this.PaintControlTableLayout.Controls.Add(this.IDColorPnl, 2, 0);
            this.PaintControlTableLayout.Controls.Add(this.MetaDataPnl, 5, 0);
            this.PaintControlTableLayout.Controls.Add(this.InactiveColorPnl, 0, 0);
            this.PaintControlTableLayout.Controls.Add(this.IDPnl, 3, 0);
            this.PaintControlTableLayout.Controls.Add(this.EditCellPnl, 1, 0);
            this.PaintControlTableLayout.Controls.Add(this.MetaDataColorPnl, 4, 0);
            this.PaintControlTableLayout.Controls.Add(this.FlawColorPnl, 6, 0);
            this.PaintControlTableLayout.Controls.Add(this.ResponseColorPnl, 8, 0);
            this.PaintControlTableLayout.Controls.Add(this.UndefineColorPnl, 10, 0);
            this.PaintControlTableLayout.Dock = System.Windows.Forms.DockStyle.Left;
            this.PaintControlTableLayout.Location = new System.Drawing.Point(5, 5);
            this.PaintControlTableLayout.Margin = new System.Windows.Forms.Padding(5);
            this.PaintControlTableLayout.Name = "PaintControlTableLayout";
            this.PaintControlTableLayout.RowCount = 1;
            this.PaintControlTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.PaintControlTableLayout.Size = new System.Drawing.Size(746, 32);
            this.PaintControlTableLayout.TabIndex = 0;
            // 
            // UndefinePnl
            // 
            this.UndefinePnl.AutoSize = true;
            this.UndefinePnl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.UndefinePnl.BackColor = System.Drawing.SystemColors.Window;
            this.UndefinePnl.Controls.Add(this.UndefineRadio);
            this.UndefinePnl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UndefinePnl.Location = new System.Drawing.Point(668, 0);
            this.UndefinePnl.Margin = new System.Windows.Forms.Padding(0);
            this.UndefinePnl.Name = "UndefinePnl";
            this.UndefinePnl.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.UndefinePnl.Size = new System.Drawing.Size(78, 32);
            this.UndefinePnl.TabIndex = 9;
            // 
            // UndefineRadio
            // 
            this.UndefineRadio.AutoSize = true;
            this.UndefineRadio.BackColor = System.Drawing.Color.Transparent;
            this.UndefineRadio.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UndefineRadio.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.UndefineRadio.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.UndefineRadio.Location = new System.Drawing.Point(4, 0);
            this.UndefineRadio.Margin = new System.Windows.Forms.Padding(0);
            this.UndefineRadio.Name = "UndefineRadio";
            this.UndefineRadio.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.UndefineRadio.Size = new System.Drawing.Size(74, 32);
            this.UndefineRadio.TabIndex = 0;
            this.UndefineRadio.TabStop = true;
            this.UndefineRadio.Text = "Undefine";
            this.UndefineRadio.UseVisualStyleBackColor = false;
            this.UndefineRadio.Click += new System.EventHandler(this.DefinitionButton_CheckChanged);
            // 
            // ResponsePnl
            // 
            this.ResponsePnl.AutoSize = true;
            this.ResponsePnl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ResponsePnl.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ResponsePnl.Controls.Add(this.DefineResponseRadio);
            this.ResponsePnl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ResponsePnl.Location = new System.Drawing.Point(525, 0);
            this.ResponsePnl.Margin = new System.Windows.Forms.Padding(0);
            this.ResponsePnl.Name = "ResponsePnl";
            this.ResponsePnl.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.ResponsePnl.Size = new System.Drawing.Size(117, 32);
            this.ResponsePnl.TabIndex = 7;
            // 
            // DefineResponseRadio
            // 
            this.DefineResponseRadio.AutoSize = true;
            this.DefineResponseRadio.BackColor = System.Drawing.SystemColors.Control;
            this.DefineResponseRadio.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DefineResponseRadio.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.DefineResponseRadio.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.DefineResponseRadio.Location = new System.Drawing.Point(4, 0);
            this.DefineResponseRadio.Margin = new System.Windows.Forms.Padding(0);
            this.DefineResponseRadio.Name = "DefineResponseRadio";
            this.DefineResponseRadio.Size = new System.Drawing.Size(113, 32);
            this.DefineResponseRadio.TabIndex = 0;
            this.DefineResponseRadio.TabStop = true;
            this.DefineResponseRadio.Text = "Define Response";
            this.DefineResponseRadio.UseVisualStyleBackColor = false;
            this.DefineResponseRadio.Click += new System.EventHandler(this.DefinitionButton_CheckChanged);
            // 
            // FlawPnl
            // 
            this.FlawPnl.AutoSize = true;
            this.FlawPnl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.FlawPnl.BackColor = System.Drawing.Color.LightSeaGreen;
            this.FlawPnl.Controls.Add(this.DefineFlawRadio);
            this.FlawPnl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FlawPnl.Location = new System.Drawing.Point(408, 0);
            this.FlawPnl.Margin = new System.Windows.Forms.Padding(0);
            this.FlawPnl.Name = "FlawPnl";
            this.FlawPnl.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.FlawPnl.Size = new System.Drawing.Size(91, 32);
            this.FlawPnl.TabIndex = 5;
            // 
            // DefineFlawRadio
            // 
            this.DefineFlawRadio.AutoSize = true;
            this.DefineFlawRadio.BackColor = System.Drawing.SystemColors.Control;
            this.DefineFlawRadio.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DefineFlawRadio.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.DefineFlawRadio.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.DefineFlawRadio.Location = new System.Drawing.Point(4, 0);
            this.DefineFlawRadio.Margin = new System.Windows.Forms.Padding(0);
            this.DefineFlawRadio.Name = "DefineFlawRadio";
            this.DefineFlawRadio.Size = new System.Drawing.Size(87, 32);
            this.DefineFlawRadio.TabIndex = 0;
            this.DefineFlawRadio.TabStop = true;
            this.DefineFlawRadio.Text = "Define Flaw";
            this.DefineFlawRadio.UseVisualStyleBackColor = false;
            this.DefineFlawRadio.Click += new System.EventHandler(this.DefinitionButton_CheckChanged);
            // 
            // IDColorPnl
            // 
            this.IDColorPnl.BackColor = System.Drawing.Color.Plum;
            this.IDColorPnl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.IDColorPnl.Location = new System.Drawing.Point(136, 0);
            this.IDColorPnl.Margin = new System.Windows.Forms.Padding(0);
            this.IDColorPnl.Name = "IDColorPnl";
            this.IDColorPnl.Size = new System.Drawing.Size(26, 32);
            this.IDColorPnl.TabIndex = 1;
            this.IDColorPnl.Paint += new System.Windows.Forms.PaintEventHandler(this.ColorLabel_Paint);
            // 
            // MetaDataPnl
            // 
            this.MetaDataPnl.AutoSize = true;
            this.MetaDataPnl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.MetaDataPnl.BackColor = System.Drawing.Color.LightGreen;
            this.MetaDataPnl.Controls.Add(this.DefineMetadataRadio);
            this.MetaDataPnl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MetaDataPnl.Location = new System.Drawing.Point(268, 0);
            this.MetaDataPnl.Margin = new System.Windows.Forms.Padding(0);
            this.MetaDataPnl.Name = "MetaDataPnl";
            this.MetaDataPnl.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.MetaDataPnl.Size = new System.Drawing.Size(114, 32);
            this.MetaDataPnl.TabIndex = 3;
            // 
            // DefineMetadataRadio
            // 
            this.DefineMetadataRadio.AutoSize = true;
            this.DefineMetadataRadio.BackColor = System.Drawing.SystemColors.Control;
            this.DefineMetadataRadio.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DefineMetadataRadio.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.DefineMetadataRadio.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.DefineMetadataRadio.Location = new System.Drawing.Point(4, 0);
            this.DefineMetadataRadio.Margin = new System.Windows.Forms.Padding(0);
            this.DefineMetadataRadio.Name = "DefineMetadataRadio";
            this.DefineMetadataRadio.Size = new System.Drawing.Size(110, 32);
            this.DefineMetadataRadio.TabIndex = 0;
            this.DefineMetadataRadio.TabStop = true;
            this.DefineMetadataRadio.Text = "Define Metadata";
            this.DefineMetadataRadio.UseVisualStyleBackColor = false;
            this.DefineMetadataRadio.Click += new System.EventHandler(this.DefinitionButton_CheckChanged);
            // 
            // InactiveColorPnl
            // 
            this.InactiveColorPnl.BackColor = System.Drawing.SystemColors.Control;
            this.InactiveColorPnl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InactiveColorPnl.Location = new System.Drawing.Point(0, 0);
            this.InactiveColorPnl.Margin = new System.Windows.Forms.Padding(0);
            this.InactiveColorPnl.Name = "InactiveColorPnl";
            this.InactiveColorPnl.Size = new System.Drawing.Size(28, 32);
            this.InactiveColorPnl.TabIndex = 0;
            this.InactiveColorPnl.Paint += new System.Windows.Forms.PaintEventHandler(this.ColorLabel_Paint);
            // 
            // IDPnl
            // 
            this.IDPnl.AutoSize = true;
            this.IDPnl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.IDPnl.BackColor = System.Drawing.Color.Plum;
            this.IDPnl.Controls.Add(this.DefineIDRadio);
            this.IDPnl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.IDPnl.Location = new System.Drawing.Point(162, 0);
            this.IDPnl.Margin = new System.Windows.Forms.Padding(0);
            this.IDPnl.Name = "IDPnl";
            this.IDPnl.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.IDPnl.Size = new System.Drawing.Size(80, 32);
            this.IDPnl.TabIndex = 2;
            // 
            // DefineIDRadio
            // 
            this.DefineIDRadio.AutoSize = true;
            this.DefineIDRadio.BackColor = System.Drawing.SystemColors.Control;
            this.DefineIDRadio.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DefineIDRadio.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.DefineIDRadio.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.DefineIDRadio.Location = new System.Drawing.Point(4, 0);
            this.DefineIDRadio.Margin = new System.Windows.Forms.Padding(0);
            this.DefineIDRadio.Name = "DefineIDRadio";
            this.DefineIDRadio.Size = new System.Drawing.Size(76, 32);
            this.DefineIDRadio.TabIndex = 0;
            this.DefineIDRadio.TabStop = true;
            this.DefineIDRadio.Text = "Define ID";
            this.DefineIDRadio.UseVisualStyleBackColor = false;
            this.DefineIDRadio.Click += new System.EventHandler(this.DefinitionButton_CheckChanged);
            // 
            // EditCellPnl
            // 
            this.EditCellPnl.AutoSize = true;
            this.EditCellPnl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.EditCellPnl.BackColor = System.Drawing.SystemColors.Control;
            this.EditCellPnl.Controls.Add(this.InactiveRadio);
            this.EditCellPnl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EditCellPnl.Location = new System.Drawing.Point(28, 0);
            this.EditCellPnl.Margin = new System.Windows.Forms.Padding(0);
            this.EditCellPnl.Name = "EditCellPnl";
            this.EditCellPnl.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.EditCellPnl.Size = new System.Drawing.Size(108, 32);
            this.EditCellPnl.TabIndex = 2;
            // 
            // InactiveRadio
            // 
            this.InactiveRadio.AutoSize = true;
            this.InactiveRadio.BackColor = System.Drawing.SystemColors.Control;
            this.InactiveRadio.Checked = true;
            this.InactiveRadio.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InactiveRadio.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.InactiveRadio.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.InactiveRadio.Location = new System.Drawing.Point(4, 0);
            this.InactiveRadio.Margin = new System.Windows.Forms.Padding(0);
            this.InactiveRadio.Name = "InactiveRadio";
            this.InactiveRadio.Size = new System.Drawing.Size(104, 32);
            this.InactiveRadio.TabIndex = 0;
            this.InactiveRadio.TabStop = true;
            this.InactiveRadio.Text = "Edit Cell Values";
            this.InactiveRadio.UseVisualStyleBackColor = false;
            this.InactiveRadio.Click += new System.EventHandler(this.EditCellValues_CheckChanged);
            // 
            // MetaDataColorPnl
            // 
            this.MetaDataColorPnl.BackColor = System.Drawing.Color.LightGreen;
            this.MetaDataColorPnl.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.MetaDataColorPnl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MetaDataColorPnl.Location = new System.Drawing.Point(242, 0);
            this.MetaDataColorPnl.Margin = new System.Windows.Forms.Padding(0);
            this.MetaDataColorPnl.Name = "MetaDataColorPnl";
            this.MetaDataColorPnl.Size = new System.Drawing.Size(26, 32);
            this.MetaDataColorPnl.TabIndex = 2;
            this.MetaDataColorPnl.Paint += new System.Windows.Forms.PaintEventHandler(this.ColorLabel_Paint);
            // 
            // FlawColorPnl
            // 
            this.FlawColorPnl.BackColor = System.Drawing.Color.LightSeaGreen;
            this.FlawColorPnl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FlawColorPnl.Location = new System.Drawing.Point(382, 0);
            this.FlawColorPnl.Margin = new System.Windows.Forms.Padding(0);
            this.FlawColorPnl.Name = "FlawColorPnl";
            this.FlawColorPnl.Size = new System.Drawing.Size(26, 32);
            this.FlawColorPnl.TabIndex = 4;
            this.FlawColorPnl.Paint += new System.Windows.Forms.PaintEventHandler(this.ColorLabel_Paint);
            // 
            // ResponseColorPnl
            // 
            this.ResponseColorPnl.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ResponseColorPnl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ResponseColorPnl.Location = new System.Drawing.Point(499, 0);
            this.ResponseColorPnl.Margin = new System.Windows.Forms.Padding(0);
            this.ResponseColorPnl.Name = "ResponseColorPnl";
            this.ResponseColorPnl.Size = new System.Drawing.Size(26, 32);
            this.ResponseColorPnl.TabIndex = 6;
            this.ResponseColorPnl.Paint += new System.Windows.Forms.PaintEventHandler(this.ColorLabel_Paint);
            // 
            // UndefineColorPnl
            // 
            this.UndefineColorPnl.BackColor = System.Drawing.SystemColors.Window;
            this.UndefineColorPnl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UndefineColorPnl.Location = new System.Drawing.Point(642, 0);
            this.UndefineColorPnl.Margin = new System.Windows.Forms.Padding(0);
            this.UndefineColorPnl.Name = "UndefineColorPnl";
            this.UndefineColorPnl.Size = new System.Drawing.Size(26, 32);
            this.UndefineColorPnl.TabIndex = 8;
            this.UndefineColorPnl.Paint += new System.Windows.Forms.PaintEventHandler(this.ColorLabel_Paint);
            // 
            // PasteDataPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "PasteDataPanel";
            this.Size = new System.Drawing.Size(1223, 170);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Grid_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Grid_KeyUp);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.DataSourceTabs.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DefaultDataGrid)).EndInit();
            this.PaintControlBackPnl.ResumeLayout(false);
            this.PaintControlBackPnl.PerformLayout();
            this.PaintControlTableLayout.ResumeLayout(false);
            this.PaintControlTableLayout.PerformLayout();
            this.UndefinePnl.ResumeLayout(false);
            this.UndefinePnl.PerformLayout();
            this.ResponsePnl.ResumeLayout(false);
            this.ResponsePnl.PerformLayout();
            this.FlawPnl.ResumeLayout(false);
            this.FlawPnl.PerformLayout();
            this.MetaDataPnl.ResumeLayout(false);
            this.MetaDataPnl.PerformLayout();
            this.IDPnl.ResumeLayout(false);
            this.IDPnl.PerformLayout();
            this.EditCellPnl.ResumeLayout(false);
            this.EditCellPnl.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private POD.Controls.ContextMenuStripConnected contextMenuStrip1;
        private Controls.DataGridViewDB DefaultDataGrid;
        private Controls.PODTabControl DataSourceTabs;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel UndefineColorPnl;
        private System.Windows.Forms.Panel ResponseColorPnl;
        private System.Windows.Forms.Panel FlawColorPnl;
        private System.Windows.Forms.Panel MetaDataColorPnl;
        private System.Windows.Forms.Panel IDColorPnl;
        private System.Windows.Forms.RadioButton InactiveRadio;
        private System.Windows.Forms.RadioButton DefineResponseRadio;
        private System.Windows.Forms.RadioButton DefineFlawRadio;
        private System.Windows.Forms.RadioButton DefineMetadataRadio;
        private System.Windows.Forms.RadioButton DefineIDRadio;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton UndefineRadio;
        private System.Windows.Forms.Panel InactiveColorPnl;
        private System.Windows.Forms.Panel PaintControlBackPnl;
        private System.Windows.Forms.TableLayoutPanel PaintControlTableLayout;
        private System.Windows.Forms.Panel ResponsePnl;
        private System.Windows.Forms.Panel UndefinePnl;
        private System.Windows.Forms.Panel FlawPnl;
        private System.Windows.Forms.Panel MetaDataPnl;
        private System.Windows.Forms.Panel EditCellPnl;
        private System.Windows.Forms.Panel IDPnl;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ImageList CellIcons;

    }
}
