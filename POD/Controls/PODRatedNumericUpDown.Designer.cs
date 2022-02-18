namespace POD.Controls
{
    partial class PODRatedNumericUpDown
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PODRatedNumericUpDown));
            ((System.ComponentModel.ISupportInitialize)(this.ValueNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // RatingImages
            // 
            this.RatingImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("RatingImages.ImageStream")));
            this.RatingImages.Images.SetKeyName(0, "01Best.png");
            this.RatingImages.Images.SetKeyName(1, "02Good.png");
            this.RatingImages.Images.SetKeyName(2, "03Okay.png");
            this.RatingImages.Images.SetKeyName(3, "04Meh.png");
            this.RatingImages.Images.SetKeyName(4, "05Bad.png");
            this.RatingImages.Images.SetKeyName(5, "06Worst.png");
            this.RatingImages.Images.SetKeyName(6, "07Undefined.png");
            // 
            // PODRatedNumericUpDown
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "PODRatedNumericUpDown";
            ((System.ComponentModel.ISupportInitialize)(this.ValueNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}
