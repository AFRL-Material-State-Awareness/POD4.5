namespace POD.Controls
{
    partial class PODChartNumericUpDown
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PODChartNumericUpDown));
            ((System.ComponentModel.ISupportInitialize)(this.ValueNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
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
            // PODChartNumericUpDown
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "PODChartNumericUpDown";
            ((System.ComponentModel.ISupportInitialize)(this.ValueNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}
