namespace POD.Wizards
{
    partial class WizardStep
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
            this.layout = new System.Windows.Forms.TableLayoutPanel();
            this.SuspendLayout();
            // 
            // layout
            // 
            this.layout.ColumnCount = 1;
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layout.Location = new System.Drawing.Point(0, 0);
            this.layout.Margin = new System.Windows.Forms.Padding(0);
            this.layout.Name = "layout";
            this.layout.RowCount = 3;
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layout.Size = new System.Drawing.Size(644, 337);
            this.layout.TabIndex = 0;
            // 
            // WizardStep
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layout);
            this.Name = "WizardStep";
            this.Size = new System.Drawing.Size(644, 337);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel layout;
    }
}
