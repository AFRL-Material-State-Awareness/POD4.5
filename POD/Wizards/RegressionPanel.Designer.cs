using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POD.Controls;

namespace POD.Wizards
{
    partial class RegressionPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegressionPanel));
            this.MainChartRightControlsSplitter = new System.Windows.Forms.SplitContainer();
            this.graphSplitter = new POD.Controls.SideSplitter();
            this.graphFlowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.graphSplitterOverlay = new POD.Controls.BlendPictureBox();
            this.RightControlsTablePanel = new System.Windows.Forms.TableLayoutPanel();
            this.OutputLabelTable = new System.Windows.Forms.TableLayoutPanel();
            this.ShiftOutputButton = new System.Windows.Forms.Button();
            this.OutputLabel = new System.Windows.Forms.Label();
            this.OutputHideButton = new System.Windows.Forms.Button();
            this.InputLabelTable = new System.Windows.Forms.TableLayoutPanel();
            this.InputLabel = new System.Windows.Forms.Label();
            this.ShiftInputButton = new System.Windows.Forms.Button();
            this.InputHideButton = new System.Windows.Forms.Button();
            this.inputTablePanel = new System.Windows.Forms.TableLayoutPanel();
            this.outputTablePanel = new System.Windows.Forms.TableLayoutPanel();
            this.PutControlsHerePanel = new System.Windows.Forms.Panel();
            this.ExpandContractList = new System.Windows.Forms.ImageList(this.components);
            this.ShiftLeftRightList = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.MainChartRightControlsSplitter)).BeginInit();
            this.MainChartRightControlsSplitter.Panel1.SuspendLayout();
            this.MainChartRightControlsSplitter.Panel2.SuspendLayout();
            this.MainChartRightControlsSplitter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.graphSplitter)).BeginInit();
            this.graphSplitter.Panel1.SuspendLayout();
            this.graphSplitter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.graphSplitterOverlay)).BeginInit();
            this.RightControlsTablePanel.SuspendLayout();
            this.OutputLabelTable.SuspendLayout();
            this.InputLabelTable.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainChartRightControlsSplitter
            // 
            this.MainChartRightControlsSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainChartRightControlsSplitter.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.MainChartRightControlsSplitter.IsSplitterFixed = true;
            this.MainChartRightControlsSplitter.Location = new System.Drawing.Point(0, 0);
            this.MainChartRightControlsSplitter.Name = "MainChartRightControlsSplitter";
            // 
            // MainChartRightControlsSplitter.Panel1
            // 
            this.MainChartRightControlsSplitter.Panel1.Controls.Add(this.graphSplitter);
            this.MainChartRightControlsSplitter.Panel1.Controls.Add(this.graphSplitterOverlay);
            this.MainChartRightControlsSplitter.Panel1MinSize = 70;
            // 
            // MainChartRightControlsSplitter.Panel2
            // 
            this.MainChartRightControlsSplitter.Panel2.AutoScrollMargin = new System.Drawing.Size(20, 0);
            this.MainChartRightControlsSplitter.Panel2.Controls.Add(this.RightControlsTablePanel);
            this.MainChartRightControlsSplitter.Panel2.Controls.Add(this.PutControlsHerePanel);
            this.MainChartRightControlsSplitter.Panel2MinSize = 70;
            this.MainChartRightControlsSplitter.Size = new System.Drawing.Size(1051, 640);
            this.MainChartRightControlsSplitter.SplitterDistance = 847;
            this.MainChartRightControlsSplitter.TabIndex = 1;
            // 
            // graphSplitter
            // 
            this.graphSplitter.BackColor = System.Drawing.SystemColors.Control;
            this.graphSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graphSplitter.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.graphSplitter.Location = new System.Drawing.Point(0, 0);
            this.graphSplitter.Name = "graphSplitter";
            // 
            // graphSplitter.Panel1
            // 
            this.graphSplitter.Panel1.Controls.Add(this.graphFlowPanel);
            this.graphSplitter.Panel1MinSize = 70;
            this.graphSplitter.Panel2MinSize = 70;
            this.graphSplitter.Size = new System.Drawing.Size(847, 640);
            this.graphSplitter.SplitterDistance = 210;
            this.graphSplitter.TabIndex = 99;
            // 
            // graphFlowPanel
            // 
            this.graphFlowPanel.AutoScroll = true;
            this.graphFlowPanel.BackColor = System.Drawing.SystemColors.Control;
            this.graphFlowPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graphFlowPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.graphFlowPanel.Location = new System.Drawing.Point(0, 0);
            this.graphFlowPanel.Name = "graphFlowPanel";
            this.graphFlowPanel.Size = new System.Drawing.Size(210, 640);
            this.graphFlowPanel.TabIndex = 0;
            this.graphFlowPanel.WrapContents = false;
            // 
            // graphSplitterOverlay
            // 
            this.graphSplitterOverlay.BackColor = System.Drawing.SystemColors.Control;
            this.graphSplitterOverlay.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.graphSplitterOverlay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graphSplitterOverlay.DrawVerticalLine = false;
            this.graphSplitterOverlay.Location = new System.Drawing.Point(0, 0);
            this.graphSplitterOverlay.MouseX = 0;
            this.graphSplitterOverlay.Name = "graphSplitterOverlay";
            this.graphSplitterOverlay.PossibleLines = ((System.Collections.Generic.List<int>)(resources.GetObject("graphSplitterOverlay.PossibleLines")));
            this.graphSplitterOverlay.Size = new System.Drawing.Size(847, 640);
            this.graphSplitterOverlay.TabIndex = 0;
            this.graphSplitterOverlay.TabStop = false;
            this.graphSplitterOverlay.Transparency = 0F;
            this.graphSplitterOverlay.VerticalLineX = 0;
            this.graphSplitterOverlay.Visible = false;
            // 
            // RightControlsTablePanel
            // 
            this.RightControlsTablePanel.AutoScrollMargin = new System.Drawing.Size(20, 0);
            this.RightControlsTablePanel.AutoSize = true;
            this.RightControlsTablePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.RightControlsTablePanel.ColumnCount = 2;
            this.RightControlsTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.RightControlsTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.RightControlsTablePanel.Controls.Add(this.OutputLabelTable, 0, 2);
            this.RightControlsTablePanel.Controls.Add(this.InputLabelTable, 0, 0);
            this.RightControlsTablePanel.Controls.Add(this.inputTablePanel, 0, 1);
            this.RightControlsTablePanel.Controls.Add(this.outputTablePanel, 0, 3);
            this.RightControlsTablePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RightControlsTablePanel.Location = new System.Drawing.Point(0, 0);
            this.RightControlsTablePanel.Name = "RightControlsTablePanel";
            this.RightControlsTablePanel.RowCount = 4;
            this.RightControlsTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.RightControlsTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.RightControlsTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.RightControlsTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.RightControlsTablePanel.Size = new System.Drawing.Size(200, 640);
            this.RightControlsTablePanel.TabIndex = 0;
            // 
            // OutputLabelTable
            // 
            this.OutputLabelTable.AutoSize = true;
            this.OutputLabelTable.ColumnCount = 3;
            this.OutputLabelTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.OutputLabelTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.OutputLabelTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.OutputLabelTable.Controls.Add(this.ShiftOutputButton, 0, 0);
            this.OutputLabelTable.Controls.Add(this.OutputLabel, 0, 0);
            this.OutputLabelTable.Controls.Add(this.OutputHideButton, 1, 0);
            this.OutputLabelTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OutputLabelTable.Location = new System.Drawing.Point(0, 20);
            this.OutputLabelTable.Margin = new System.Windows.Forms.Padding(0);
            this.OutputLabelTable.Name = "OutputLabelTable";
            this.OutputLabelTable.RowCount = 1;
            this.OutputLabelTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.OutputLabelTable.Size = new System.Drawing.Size(200, 20);
            this.OutputLabelTable.TabIndex = 1;
            // 
            // ShiftOutputButton
            // 
            this.ShiftOutputButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ShiftOutputButton.BackgroundImage")));
            this.ShiftOutputButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ShiftOutputButton.Location = new System.Drawing.Point(160, 0);
            this.ShiftOutputButton.Margin = new System.Windows.Forms.Padding(0);
            this.ShiftOutputButton.Name = "ShiftOutputButton";
            this.ShiftOutputButton.Size = new System.Drawing.Size(20, 20);
            this.ShiftOutputButton.TabIndex = 102;
            this.ShiftOutputButton.UseVisualStyleBackColor = true;
            this.ShiftOutputButton.Click += new System.EventHandler(this.Shift_Inputs);
            // 
            // OutputLabel
            // 
            this.OutputLabel.AutoSize = true;
            this.OutputLabel.BackColor = System.Drawing.SystemColors.ControlText;
            this.OutputLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OutputLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OutputLabel.ForeColor = System.Drawing.SystemColors.Window;
            this.OutputLabel.Location = new System.Drawing.Point(0, 0);
            this.OutputLabel.Margin = new System.Windows.Forms.Padding(0);
            this.OutputLabel.Name = "OutputLabel";
            this.OutputLabel.Size = new System.Drawing.Size(160, 20);
            this.OutputLabel.TabIndex = 1;
            this.OutputLabel.Text = "OUTPUT";
            // 
            // OutputHideButton
            // 
            this.OutputHideButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("OutputHideButton.BackgroundImage")));
            this.OutputHideButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.OutputHideButton.Location = new System.Drawing.Point(180, 0);
            this.OutputHideButton.Margin = new System.Windows.Forms.Padding(0);
            this.OutputHideButton.Name = "OutputHideButton";
            this.OutputHideButton.Size = new System.Drawing.Size(20, 20);
            this.OutputHideButton.TabIndex = 103;
            this.OutputHideButton.UseVisualStyleBackColor = true;
            this.OutputHideButton.Click += new System.EventHandler(this.ExpandContract_Outputs);
            // 
            // InputLabelTable
            // 
            this.InputLabelTable.AutoSize = true;
            this.InputLabelTable.ColumnCount = 3;
            this.InputLabelTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.InputLabelTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.InputLabelTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.InputLabelTable.Controls.Add(this.InputLabel, 0, 0);
            this.InputLabelTable.Controls.Add(this.ShiftInputButton, 1, 0);
            this.InputLabelTable.Controls.Add(this.InputHideButton, 2, 0);
            this.InputLabelTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InputLabelTable.Location = new System.Drawing.Point(0, 0);
            this.InputLabelTable.Margin = new System.Windows.Forms.Padding(0);
            this.InputLabelTable.Name = "InputLabelTable";
            this.InputLabelTable.RowCount = 1;
            this.InputLabelTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.InputLabelTable.Size = new System.Drawing.Size(200, 20);
            this.InputLabelTable.TabIndex = 0;
            // 
            // InputLabel
            // 
            this.InputLabel.AutoSize = true;
            this.InputLabel.BackColor = System.Drawing.SystemColors.ControlText;
            this.InputLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InputLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InputLabel.ForeColor = System.Drawing.SystemColors.Window;
            this.InputLabel.Location = new System.Drawing.Point(0, 0);
            this.InputLabel.Margin = new System.Windows.Forms.Padding(0);
            this.InputLabel.Name = "InputLabel";
            this.InputLabel.Size = new System.Drawing.Size(160, 20);
            this.InputLabel.TabIndex = 3;
            this.InputLabel.Text = "INPUT";
            // 
            // ShiftInputButton
            // 
            this.ShiftInputButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ShiftInputButton.BackgroundImage")));
            this.ShiftInputButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ShiftInputButton.Location = new System.Drawing.Point(160, 0);
            this.ShiftInputButton.Margin = new System.Windows.Forms.Padding(0);
            this.ShiftInputButton.Name = "ShiftInputButton";
            this.ShiftInputButton.Size = new System.Drawing.Size(20, 20);
            this.ShiftInputButton.TabIndex = 100;
            this.ShiftInputButton.UseVisualStyleBackColor = true;
            this.ShiftInputButton.Click += new System.EventHandler(this.Shift_Inputs);
            // 
            // InputHideButton
            // 
            this.InputHideButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("InputHideButton.BackgroundImage")));
            this.InputHideButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.InputHideButton.Location = new System.Drawing.Point(180, 0);
            this.InputHideButton.Margin = new System.Windows.Forms.Padding(0);
            this.InputHideButton.Name = "InputHideButton";
            this.InputHideButton.Size = new System.Drawing.Size(20, 20);
            this.InputHideButton.TabIndex = 101;
            this.InputHideButton.UseVisualStyleBackColor = true;
            this.InputHideButton.Click += new System.EventHandler(this.ExpandContract_Inputs);
            // 
            // inputTablePanel
            // 
            this.inputTablePanel.AutoSize = true;
            this.inputTablePanel.ColumnCount = 2;
            this.inputTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.inputTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.inputTablePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.inputTablePanel.Location = new System.Drawing.Point(0, 20);
            this.inputTablePanel.Margin = new System.Windows.Forms.Padding(0);
            this.inputTablePanel.Name = "inputTablePanel";
            this.inputTablePanel.RowCount = 10;
            this.inputTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.inputTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.inputTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.inputTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.inputTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.inputTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.inputTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.inputTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.inputTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.inputTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.inputTablePanel.Size = new System.Drawing.Size(200, 0);
            this.inputTablePanel.TabIndex = 2;
            // 
            // outputTablePanel
            // 
            this.outputTablePanel.AutoSize = true;
            this.outputTablePanel.ColumnCount = 2;
            this.outputTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.outputTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.outputTablePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.outputTablePanel.Location = new System.Drawing.Point(0, 40);
            this.outputTablePanel.Margin = new System.Windows.Forms.Padding(0);
            this.outputTablePanel.Name = "outputTablePanel";
            this.outputTablePanel.RowCount = 19;
            this.outputTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outputTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outputTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outputTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outputTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outputTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outputTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outputTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outputTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outputTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outputTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outputTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outputTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outputTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outputTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outputTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outputTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outputTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outputTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outputTablePanel.Size = new System.Drawing.Size(200, 0);
            this.outputTablePanel.TabIndex = 0;
            this.outputTablePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel3_Paint);
            // 
            // PutControlsHerePanel
            // 
            this.PutControlsHerePanel.AutoScroll = true;
            this.PutControlsHerePanel.AutoSize = true;
            this.PutControlsHerePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.PutControlsHerePanel.Location = new System.Drawing.Point(0, 0);
            this.PutControlsHerePanel.Margin = new System.Windows.Forms.Padding(0);
            this.PutControlsHerePanel.Name = "PutControlsHerePanel";
            this.PutControlsHerePanel.Size = new System.Drawing.Size(200, 0);
            this.PutControlsHerePanel.TabIndex = 1;
            // 
            // ExpandContractList
            // 
            this.ExpandContractList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ExpandContractList.ImageStream")));
            this.ExpandContractList.TransparentColor = System.Drawing.Color.Transparent;
            this.ExpandContractList.Images.SetKeyName(0, "closeup.png");
            this.ExpandContractList.Images.SetKeyName(1, "expand.png");
            // 
            // ShiftLeftRightList
            // 
            this.ShiftLeftRightList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ShiftLeftRightList.ImageStream")));
            this.ShiftLeftRightList.TransparentColor = System.Drawing.Color.Transparent;
            this.ShiftLeftRightList.Images.SetKeyName(0, "shift left.png");
            this.ShiftLeftRightList.Images.SetKeyName(1, "shift right.png");
            // 
            // RegressionPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.MainChartRightControlsSplitter);
            this.DoubleBuffered = true;
            this.Name = "RegressionPanel";
            this.Size = new System.Drawing.Size(1051, 640);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Panel_Paint);
            this.Resize += new System.EventHandler(this.Panel_Resize);
            this.MainChartRightControlsSplitter.Panel1.ResumeLayout(false);
            this.MainChartRightControlsSplitter.Panel2.ResumeLayout(false);
            this.MainChartRightControlsSplitter.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MainChartRightControlsSplitter)).EndInit();
            this.MainChartRightControlsSplitter.ResumeLayout(false);
            this.graphSplitter.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.graphSplitter)).EndInit();
            this.graphSplitter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.graphSplitterOverlay)).EndInit();
            this.RightControlsTablePanel.ResumeLayout(false);
            this.RightControlsTablePanel.PerformLayout();
            this.OutputLabelTable.ResumeLayout(false);
            this.OutputLabelTable.PerformLayout();
            this.InputLabelTable.ResumeLayout(false);
            this.InputLabelTable.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        protected SideSplitter graphSplitter;
        protected FlowLayoutPanel graphFlowPanel;
        protected TableLayoutPanel RightControlsTablePanel;
        protected TableLayoutPanel inputTablePanel;
        protected System.Windows.Forms.SplitContainer MainChartRightControlsSplitter;
        protected TableLayoutPanel outputTablePanel;
        protected BlendPictureBox graphSplitterOverlay;
        protected Panel PutControlsHerePanel;
        private Label OutputLabel;
        private Label InputLabel;
        private Button InputHideButton;
        private Button OutputHideButton;
        private ImageList ExpandContractList;
        private TableLayoutPanel InputLabelTable;
        private Button ShiftInputButton;
        private ImageList ShiftLeftRightList;
        private TableLayoutPanel OutputLabelTable;
        private Button ShiftOutputButton;
    }
}
