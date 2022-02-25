namespace POD.Controls
{
    partial class RegressionAnalysisChart
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegressionAnalysisChart));
            this.ToggleImageList = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // ToggleImageList
            // 
            this.ToggleImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ToggleImageList.ImageStream")));
            this.ToggleImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.ToggleImageList.Images.SetKeyName(0, "00single.png");
            this.ToggleImageList.Images.SetKeyName(1, "01multiple.png");
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList ToggleImageList;

    }
}
