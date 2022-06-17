using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POD.Controls;
using System.Windows.Forms;

namespace POD.Wizards
{
    partial class WizardActionBar
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizardActionBar));
            this.mainTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.leftFlowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.leftOverflowButton = new POD.Controls.PODOverButton();
            this.rightFlowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.rightOverflowButton = new POD.Controls.PODOverButton();
            this.leftOverflowMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.rightOverflowMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ActionIcons = new System.Windows.Forms.ImageList(this.components);
            this.tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.mainTableLayout.SuspendLayout();
            this.leftFlowPanel.SuspendLayout();
            this.rightFlowPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainTableLayout
            // 
            this.mainTableLayout.AutoSize = true;
            this.mainTableLayout.BackColor = System.Drawing.Color.LightSteelBlue;
            this.mainTableLayout.ColumnCount = 2;
            this.mainTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.mainTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.mainTableLayout.Controls.Add(this.leftFlowPanel, 0, 0);
            this.mainTableLayout.Controls.Add(this.rightFlowPanel, 1, 0);
            this.mainTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTableLayout.Location = new System.Drawing.Point(0, 0);
            this.mainTableLayout.Margin = new System.Windows.Forms.Padding(0);
            this.mainTableLayout.Name = "mainTableLayout";
            this.mainTableLayout.Padding = new System.Windows.Forms.Padding(3);
            this.mainTableLayout.RowCount = 1;
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTableLayout.Size = new System.Drawing.Size(56, 65);
            this.mainTableLayout.TabIndex = 1;
            this.mainTableLayout.Paint += new System.Windows.Forms.PaintEventHandler(this.mainTableLayout_Paint);
            // 
            // leftFlowPanel
            // 
            this.leftFlowPanel.AutoSize = true;
            this.leftFlowPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.leftFlowPanel.Controls.Add(this.leftOverflowButton);
            this.leftFlowPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.leftFlowPanel.Location = new System.Drawing.Point(3, 3);
            this.leftFlowPanel.Margin = new System.Windows.Forms.Padding(0);
            this.leftFlowPanel.Name = "leftFlowPanel";
            this.leftFlowPanel.Size = new System.Drawing.Size(25, 59);
            this.leftFlowPanel.TabIndex = 0;
            // 
            // leftOverflowButton
            // 
            this.leftOverflowButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.leftOverflowButton.Location = new System.Drawing.Point(0, 0);
            this.leftOverflowButton.Margin = new System.Windows.Forms.Padding(0);
            this.leftOverflowButton.Name = "leftOverflowButton";
            this.leftOverflowButton.PODToolTip = null;
            this.leftOverflowButton.Size = new System.Drawing.Size(25, 59);
            this.leftOverflowButton.TabIndex = 4;
            this.leftOverflowButton.Text = "...";
            this.leftOverflowButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.leftOverflowButton.TipForControl = "";
            this.leftOverflowButton.UseVisualStyleBackColor = true;
            this.leftOverflowButton.Click += new System.EventHandler(this.LeftOverflow_Click);
            // 
            // rightFlowPanel
            // 
            this.rightFlowPanel.AutoSize = true;
            this.rightFlowPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.rightFlowPanel.Controls.Add(this.rightOverflowButton);
            this.rightFlowPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.rightFlowPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.rightFlowPanel.Location = new System.Drawing.Point(28, 3);
            this.rightFlowPanel.Margin = new System.Windows.Forms.Padding(0);
            this.rightFlowPanel.Name = "rightFlowPanel";
            this.rightFlowPanel.Size = new System.Drawing.Size(25, 59);
            this.rightFlowPanel.TabIndex = 4;
            // 
            // rightOverflowButton
            // 
            this.rightOverflowButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rightOverflowButton.Location = new System.Drawing.Point(0, 0);
            this.rightOverflowButton.Margin = new System.Windows.Forms.Padding(0);
            this.rightOverflowButton.Name = "rightOverflowButton";
            this.rightOverflowButton.PODToolTip = null;
            this.rightOverflowButton.Size = new System.Drawing.Size(25, 59);
            this.rightOverflowButton.TabIndex = 4;
            this.rightOverflowButton.Text = "...";
            this.rightOverflowButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.rightOverflowButton.TipForControl = "";
            this.rightOverflowButton.UseVisualStyleBackColor = true;
            this.rightOverflowButton.Click += new System.EventHandler(this.RightOverflow_Click);
            // 
            // leftOverflowMenu
            // 
            this.leftOverflowMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.leftOverflowMenu.Name = "leftOverflowMenu";
            this.leftOverflowMenu.ShowImageMargin = false;
            this.leftOverflowMenu.Size = new System.Drawing.Size(36, 4);
            // 
            // rightOverflowMenu
            // 
            this.rightOverflowMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.rightOverflowMenu.Name = "rightOverflowMenu";
            this.rightOverflowMenu.ShowImageMargin = false;
            this.rightOverflowMenu.Size = new System.Drawing.Size(36, 4);
            // 
            // ActionIcons
            // 
            this.ActionIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ActionIcons.ImageStream")));
            this.ActionIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.ActionIcons.Images.SetKeyName(0, "Next.png");
            this.ActionIcons.Images.SetKeyName(1, "Previous.png");
            this.ActionIcons.Images.SetKeyName(2, "Finish.png");
            this.ActionIcons.Images.SetKeyName(3, "Duplicate.png");
            this.ActionIcons.Images.SetKeyName(4, "Use Last.png");
            this.ActionIcons.Images.SetKeyName(5, "Paste.png");
            this.ActionIcons.Images.SetKeyName(6, "New Source.png");
            this.ActionIcons.Images.SetKeyName(7, "Delete Source.png");
            this.ActionIcons.Images.SetKeyName(8, "Restore Source.png");
            this.ActionIcons.Images.SetKeyName(9, "Restore Analysis.png");
            this.ActionIcons.Images.SetKeyName(10, "Fit All Graphs.png");
            this.ActionIcons.Images.SetKeyName(11, "Group By Flaw.png");
            this.ActionIcons.Images.SetKeyName(12, "Show Fits.png");
            this.ActionIcons.Images.SetKeyName(13, "Show Residuals.png");
            this.ActionIcons.Images.SetKeyName(14, "Show All Charts.png");
            this.ActionIcons.Images.SetKeyName(15, "Show Residual.png");
            this.ActionIcons.Images.SetKeyName(16, "Show Threshold.png");
            this.ActionIcons.Images.SetKeyName(17, "Overlay Models.png");
            this.ActionIcons.Images.SetKeyName(18, "Show Model Fit.png");
            this.ActionIcons.Images.SetKeyName(19, "Refresh Charts.png");
            this.ActionIcons.Images.SetKeyName(20, "Delete Row.png");
            this.ActionIcons.Images.SetKeyName(21, "Insert Row.png");
            this.ActionIcons.Images.SetKeyName(22, "Cycle Transforms.png");
            this.ActionIcons.Images.SetKeyName(23, "Export to Excel.png");
            this.ActionIcons.Images.SetKeyName(24, "Show POD Curve.png");
            this.ActionIcons.Images.SetKeyName(25, "Select Empty.png");
            // 
            // WizardActionBar
            // 
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.mainTableLayout);
            this.Name = "WizardActionBar";
            this.Size = new System.Drawing.Size(56, 65);
            this.Resize += new System.EventHandler(this.ActionBar_Resize);
            this.mainTableLayout.ResumeLayout(false);
            this.mainTableLayout.PerformLayout();
            this.leftFlowPanel.ResumeLayout(false);
            this.rightFlowPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
            //al UI has been loaded!
        }

        #endregion

        private TableLayoutPanel mainTableLayout;
        private FlowLayoutPanel leftFlowPanel;
        private PODOverButton leftOverflowButton;
        private FlowLayoutPanel rightFlowPanel;
        private PODOverButton rightOverflowButton;
        private ImageList ActionIcons;
        public ToolTip tooltip;
    }
}
