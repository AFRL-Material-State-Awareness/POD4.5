namespace POD.Controls
{
    partial class SimpleActionBar
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SimpleActionBar));
            this.ActionIcons = new System.Windows.Forms.ImageList(this.components);
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
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
            this.ActionIcons.Images.SetKeyName(15, "Show POD Curve.png");
            this.ActionIcons.Images.SetKeyName(16, "Show Residual.png");
            this.ActionIcons.Images.SetKeyName(17, "Show Threshold.png");
            this.ActionIcons.Images.SetKeyName(18, "Overlay Models.png");
            this.ActionIcons.Images.SetKeyName(19, "Show Model Fit.png");
            this.ActionIcons.Images.SetKeyName(20, "Refresh Charts.png");
            this.ActionIcons.Images.SetKeyName(21, "Delete Row.png");
            this.ActionIcons.Images.SetKeyName(22, "Insert Row.png");
            this.ActionIcons.Images.SetKeyName(23, "Cycle Transforms.png");
            this.ActionIcons.Images.SetKeyName(24, "Select All.png");
            this.ActionIcons.Images.SetKeyName(25, "Select None.png");
            this.ActionIcons.Images.SetKeyName(26, "Invert Selection.png");
            this.ActionIcons.Images.SetKeyName(27, "Create Analysis.png");
            this.ActionIcons.Images.SetKeyName(28, "Delete Selected.png");
            this.ActionIcons.Images.SetKeyName(29, "Check All.png");
            this.ActionIcons.Images.SetKeyName(30, "Check None.png");
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(0, 0);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // SimpleActionBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "SimpleActionBar";
            this.Size = new System.Drawing.Size(0, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList ActionIcons;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.ToolTip tooltip;
    }
}
