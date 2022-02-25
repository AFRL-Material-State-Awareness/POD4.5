namespace POD.Docks
{
    partial class ReportDock
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportDock));
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.linkLayout1 = new POD.Controls.LinkLayout();
            ((System.ComponentModel.ISupportInitialize)(this._blendBox)).BeginInit();
            this.SuspendLayout();
            // 
            // _blendBox
            // 
            this._blendBox.Size = new System.Drawing.Size(354, 391);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox1.Location = new System.Drawing.Point(0, 0);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(354, 391);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = resources.GetString("richTextBox1.Text");
            // 
            // linkLayout1
            // 
            this.linkLayout1.AutoScroll = true;
            this.linkLayout1.BackColor = System.Drawing.Color.White;
            this.linkLayout1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.linkLayout1.Links = null;
            this.linkLayout1.Location = new System.Drawing.Point(0, 0);
            this.linkLayout1.Name = "linkLayout1";
            this.linkLayout1.Size = new System.Drawing.Size(200, 100);
            this.linkLayout1.TabIndex = 0;
            // 
            // ReportDock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(354, 391);
            this.Controls.Add(this.richTextBox1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ReportDock";
            this.Text = "Report Viewer";
            this.Controls.SetChildIndex(this.richTextBox1, 0);
            this.Controls.SetChildIndex(this._blendBox, 0);
            ((System.ComponentModel.ISupportInitialize)(this._blendBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private Controls.LinkLayout linkLayout1;
    }
}