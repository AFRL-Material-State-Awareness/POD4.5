namespace POD.Docks
{
    partial class PDFDisplay
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PDFDisplay));
            this.podPdfViewer1 = new POD.Controls.PODPdfViewer();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnZoomOut = new System.Windows.Forms.ToolStripButton();
            this.btnZoonIn = new System.Windows.Forms.ToolStripButton();
            this.btnDynamic = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnActural = new System.Windows.Forms.ToolStripButton();
            this.btnFitPage = new System.Windows.Forms.ToolStripButton();
            this.btnFitWidth = new System.Windows.Forms.ToolStripButton();
            this.btnSmllMntr = new System.Windows.Forms.ToolStripButton();
            this.btnDualMntr = new System.Windows.Forms.ToolStripButton();
            this.btnLrgMntr = new System.Windows.Forms.ToolStripButton();
            this.RtnToAnlysBtn = new System.Windows.Forms.ToolStripButton();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this._blendBox)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _blendBox
            // 
            this._blendBox.Size = new System.Drawing.Size(876, 684);
            // 
            // podPdfViewer1
            // 
            this.podPdfViewer1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.podPdfViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.podPdfViewer1.Location = new System.Drawing.Point(0, 25);
            this.podPdfViewer1.MultiPagesThreshold = 60;
            this.podPdfViewer1.Name = "podPdfViewer1";
            this.podPdfViewer1.PdfFileName = null;
            this.podPdfViewer1.Size = new System.Drawing.Size(876, 659);
            this.podPdfViewer1.TabIndex = 0;
            this.podPdfViewer1.Text = "podPdfViewer1";
            this.podPdfViewer1.Threshold = 60;
            this.toolTip1.SetToolTip(this.podPdfViewer1, "Double click to switch back analysis/project.");
            this.podPdfViewer1.ZoomMode = Spire.PdfViewer.Forms.ZoomMode.FitWidth;
            this.podPdfViewer1.Click += new System.EventHandler(this.podPdfViewer1_Click);
            this.podPdfViewer1.DoubleClick += new System.EventHandler(this.pdf_DoubleClick);
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnZoomOut,
            this.btnZoonIn,
            this.btnDynamic,
            this.toolStripSeparator2,
            this.btnActural,
            this.btnFitPage,
            this.btnFitWidth,
            this.btnSmllMntr,
            this.btnDualMntr,
            this.btnLrgMntr,
            this.RtnToAnlysBtn});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(876, 25);
            this.toolStrip1.Stretch = true;
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnZoomOut.Image = ((System.Drawing.Image)(resources.GetObject("btnZoomOut.Image")));
            this.btnZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(23, 22);
            this.btnZoomOut.Text = "Zoom Out";
            this.btnZoomOut.ToolTipText = "Zoom Out ";
            // 
            // btnZoonIn
            // 
            this.btnZoonIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnZoonIn.Image = ((System.Drawing.Image)(resources.GetObject("btnZoonIn.Image")));
            this.btnZoonIn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoonIn.Name = "btnZoonIn";
            this.btnZoonIn.Size = new System.Drawing.Size(23, 22);
            this.btnZoonIn.Text = "ZoonIn";
            this.btnZoonIn.ToolTipText = "Zoon In ";
            // 
            // btnDynamic
            // 
            this.btnDynamic.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnDynamic.Enabled = false;
            this.btnDynamic.Image = ((System.Drawing.Image)(resources.GetObject("btnDynamic.Image")));
            this.btnDynamic.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDynamic.Name = "btnDynamic";
            this.btnDynamic.Size = new System.Drawing.Size(23, 22);
            this.btnDynamic.Text = "Zoom Dynamic";
            this.btnDynamic.ToolTipText = "Zoom Dynamic";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnActural
            // 
            this.btnActural.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnActural.Image = ((System.Drawing.Image)(resources.GetObject("btnActural.Image")));
            this.btnActural.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnActural.Name = "btnActural";
            this.btnActural.Size = new System.Drawing.Size(23, 22);
            this.btnActural.Text = "Actual";
            this.btnActural.ToolTipText = "Actual Size ";
            // 
            // btnFitPage
            // 
            this.btnFitPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnFitPage.Image = ((System.Drawing.Image)(resources.GetObject("btnFitPage.Image")));
            this.btnFitPage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFitPage.Name = "btnFitPage";
            this.btnFitPage.Size = new System.Drawing.Size(23, 22);
            this.btnFitPage.Text = "FitPage";
            this.btnFitPage.ToolTipText = "Fit Page";
            // 
            // btnFitWidth
            // 
            this.btnFitWidth.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnFitWidth.Image = ((System.Drawing.Image)(resources.GetObject("btnFitWidth.Image")));
            this.btnFitWidth.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFitWidth.Name = "btnFitWidth";
            this.btnFitWidth.Size = new System.Drawing.Size(23, 22);
            this.btnFitWidth.Text = "FitWidth";
            this.btnFitWidth.ToolTipText = "Fit Width";
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
            // PDFDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(876, 684);
            this.Controls.Add(this.podPdfViewer1);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PDFDisplay";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "1823A Help Viewer";
            this.Controls.SetChildIndex(this._blendBox, 0);
            this.Controls.SetChildIndex(this.toolStrip1, 0);
            this.Controls.SetChildIndex(this.podPdfViewer1, 0);
            ((System.ComponentModel.ISupportInitialize)(this._blendBox)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls.PODPdfViewer podPdfViewer1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnZoomOut;
        private System.Windows.Forms.ToolStripButton btnZoonIn;
        private System.Windows.Forms.ToolStripButton btnDynamic;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton btnActural;
        private System.Windows.Forms.ToolStripButton btnFitPage;
        private System.Windows.Forms.ToolStripButton btnFitWidth;
        private System.Windows.Forms.ToolStripButton btnDualMntr;
        private System.Windows.Forms.ToolStripButton btnLrgMntr;
        private System.Windows.Forms.ToolStripButton btnSmllMntr;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripButton RtnToAnlysBtn;


    }
}