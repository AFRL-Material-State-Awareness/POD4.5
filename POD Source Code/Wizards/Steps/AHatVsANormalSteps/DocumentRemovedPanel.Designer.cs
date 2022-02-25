namespace POD.Wizards.Steps.AHatVsANormalSteps
{
    partial class DocumentRemovedPanel
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
            this.RemovedPoints = new POD.Wizards.RemovedPointsPanel();
            this.SuspendLayout();
            // 
            // RemovedPoints
            // 
            this.RemovedPoints.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RemovedPoints.Location = new System.Drawing.Point(0, 0);
            this.RemovedPoints.Margin = new System.Windows.Forms.Padding(0);
            this.RemovedPoints.Name = "RemovedPoints";
            this.RemovedPoints.Size = new System.Drawing.Size(916, 601);
            this.RemovedPoints.TabIndex = 0;
            // 
            // DocumentRemovedPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.RemovedPoints);
            this.Name = "DocumentRemovedPanel";
            this.Size = new System.Drawing.Size(916, 601);
            this.ResumeLayout(false);

        }

        #endregion

        private RemovedPointsPanel RemovedPoints;
    }
}
