namespace POD.Controls
{
    partial class PODInfoNumericUpDown
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PODInfoNumericUpDown));
            this.CopyAllButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ValueNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // podNumericUpDown1
            // 
            this.ValueNumeric.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ValueNumeric.Size = new System.Drawing.Size(208, 20);
            // 
            // pictureBox1
            // 
            this.ImageBox.Location = new System.Drawing.Point(0, 0);
            this.ImageBox.Visible = false;
            // 
            // RatingImages
            // 
            this.RatingImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("RatingImages.ImageStream")));
            this.RatingImages.Images.SetKeyName(0, "00linearFit.png");
            this.RatingImages.Images.SetKeyName(1, "01pod.png");
            this.RatingImages.Images.SetKeyName(2, "02censorLeft.png");
            this.RatingImages.Images.SetKeyName(3, "03censorRight.png");
            this.RatingImages.Images.SetKeyName(4, "04cracksMin.png");
            this.RatingImages.Images.SetKeyName(5, "05cracksMax.png");
            this.RatingImages.Images.SetKeyName(6, "06a50.png");
            this.RatingImages.Images.SetKeyName(7, "07a90.png");
            this.RatingImages.Images.SetKeyName(8, "08a9095.png");
            this.RatingImages.Images.SetKeyName(9, "09decision.png");
            this.RatingImages.Images.SetKeyName(10, "empty.png");
            // 
            // CopyAllButton
            // 
            this.CopyAllButton.BackgroundImage = global::POD.Controls.Properties.Resources.CopyToAll;
            this.CopyAllButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.CopyAllButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CopyAllButton.Location = new System.Drawing.Point(208, 0);
            this.CopyAllButton.Margin = new System.Windows.Forms.Padding(0);
            this.CopyAllButton.Name = "CopyAllButton";
            this.CopyAllButton.Size = new System.Drawing.Size(20, 20);
            this.CopyAllButton.TabIndex = 2;
            this.CopyAllButton.UseVisualStyleBackColor = true;
            this.CopyAllButton.Click += new System.EventHandler(this.CopyAll_Click);
            // 
            // PODInfoNumericUpDown
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.CopyAllButton);
            this.Name = "PODInfoNumericUpDown";
            this.Size = new System.Drawing.Size(228, 20);
            this.Controls.SetChildIndex(this.CopyAllButton, 0);
            this.Controls.SetChildIndex(this.ImageBox, 0);
            this.Controls.SetChildIndex(this.ValueNumeric, 0);
            ((System.ComponentModel.ISupportInitialize)(this.ValueNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Button CopyAllButton;
    }
}
