namespace POD.Docks
{
    partial class RTFViewerDock
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RTFViewerDock));
            this.LinkPanel = new POD.Controls.LinkLayout();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnSmllMntr = new System.Windows.Forms.ToolStripButton();
            this.btnDualMntr = new System.Windows.Forms.ToolStripButton();
            this.btnLrgMntr = new System.Windows.Forms.ToolStripButton();
            this.RtnToAnlysBtn = new System.Windows.Forms.ToolStripButton();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this._blendBox)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _blendBox
            // 
            this._blendBox.Size = new System.Drawing.Size(523, 537);
            // 
            // LinkPanel
            // 
            this.LinkPanel.AutoSize = true;
            this.LinkPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.LinkPanel.BackColor = System.Drawing.Color.White;
            this.LinkPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.LinkPanel.Links = null;
            this.LinkPanel.Location = new System.Drawing.Point(3, 3);
            this.LinkPanel.Name = "LinkPanel";
            this.LinkPanel.Size = new System.Drawing.Size(0, 0);
            this.LinkPanel.TabIndex = 1;
            this.LinkPanel.WrapContents = false;
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSmllMntr,
            this.btnDualMntr,
            this.btnLrgMntr,
            this.RtnToAnlysBtn});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(523, 25);
            this.toolStrip1.Stretch = true;
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnSmllMntr
            // 
            this.btnSmllMntr.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSmllMntr.Image = ((System.Drawing.Image)(resources.GetObject("btnSmllMntr.Image")));
            this.btnSmllMntr.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSmllMntr.Name = "btnSmllMntr";
            this.btnSmllMntr.Size = new System.Drawing.Size(23, 22);
            this.btnSmllMntr.Text = "toolStripButton3";
            this.btnSmllMntr.ToolTipText = "Default Help View";
            this.btnSmllMntr.Click += new System.EventHandler(this.btnSmllMntr_Click);
            // 
            // btnDualMntr
            // 
            this.btnDualMntr.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnDualMntr.Image = ((System.Drawing.Image)(resources.GetObject("btnDualMntr.Image")));
            this.btnDualMntr.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDualMntr.Name = "btnDualMntr";
            this.btnDualMntr.Size = new System.Drawing.Size(23, 22);
            this.btnDualMntr.Text = "toolStripButton1";
            this.btnDualMntr.ToolTipText = "Dual Monitor Help View";
            this.btnDualMntr.Click += new System.EventHandler(this.btnDualMntr_Click);
            // 
            // btnLrgMntr
            // 
            this.btnLrgMntr.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnLrgMntr.Image = ((System.Drawing.Image)(resources.GetObject("btnLrgMntr.Image")));
            this.btnLrgMntr.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLrgMntr.Name = "btnLrgMntr";
            this.btnLrgMntr.Size = new System.Drawing.Size(23, 22);
            this.btnLrgMntr.Text = "toolStripButton2";
            this.btnLrgMntr.ToolTipText = "Large Monitor Help View";
            this.btnLrgMntr.Click += new System.EventHandler(this.btnLrgMntr_Click);
            // 
            // RtnToAnlysBtn
            // 
            this.RtnToAnlysBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.RtnToAnlysBtn.Image = ((System.Drawing.Image)(resources.GetObject("RtnToAnlysBtn.Image")));
            this.RtnToAnlysBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.RtnToAnlysBtn.Name = "RtnToAnlysBtn";
            this.RtnToAnlysBtn.Size = new System.Drawing.Size(23, 22);
            this.RtnToAnlysBtn.Text = "Return to Analysis (Double Click PDF)";
            this.RtnToAnlysBtn.Click += new System.EventHandler(this.RtnToAnlysBtn_Click);
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.BackColor = System.Drawing.SystemColors.Window;
            this.panel1.Controls.Add(this.LinkPanel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(523, 512);
            this.panel1.TabIndex = 4;
            // 
            // RTFViewerDock
            // 
            this.ClientSize = new System.Drawing.Size(523, 537);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "RTFViewerDock";
            this.Controls.SetChildIndex(this._blendBox, 0);
            this.Controls.SetChildIndex(this.toolStrip1, 0);
            this.Controls.SetChildIndex(this.panel1, 0);
            ((System.ComponentModel.ISupportInitialize)(this._blendBox)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls.LinkLayout LinkPanel;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnDualMntr;
        private System.Windows.Forms.ToolStripButton btnLrgMntr;
        private System.Windows.Forms.ToolStripButton btnSmllMntr;
        private System.Windows.Forms.ToolStripButton RtnToAnlysBtn;
        private System.Windows.Forms.Panel panel1;


    }
}
