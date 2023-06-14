
namespace POD
{
    partial class LicensingInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LicensingInfo));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.LicenseBodyRTF = new System.Windows.Forms.RichTextBox();
            this.LicenseLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.LicenseBodyRTF, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.LicenseLabel, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(469, 426);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // LicenseBodyRTF
            // 
            this.LicenseBodyRTF.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.tableLayoutPanel1.SetColumnSpan(this.LicenseBodyRTF, 2);
            this.LicenseBodyRTF.Location = new System.Drawing.Point(3, 41);
            this.LicenseBodyRTF.Name = "LicenseBodyRTF";
            this.LicenseBodyRTF.ReadOnly = true;
            this.LicenseBodyRTF.Size = new System.Drawing.Size(463, 382);
            this.LicenseBodyRTF.TabIndex = 0;
            this.LicenseBodyRTF.Text = resources.GetString("LicenseBodyRTF.Text");
            // 
            // LicenseLabel
            // 
            this.LicenseLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.LicenseLabel.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.LicenseLabel, 2);
            this.LicenseLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 24.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LicenseLabel.Location = new System.Drawing.Point(166, 0);
            this.LicenseLabel.Name = "LicenseLabel";
            this.LicenseLabel.Size = new System.Drawing.Size(137, 38);
            this.LicenseLabel.TabIndex = 1;
            this.LicenseLabel.Text = "License";
            this.LicenseLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // LicensingInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(493, 450);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LicensingInfo";
            this.Text = "LicensingInfo POD v4.5.3";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.RichTextBox LicenseBodyRTF;
        private System.Windows.Forms.Label LicenseLabel;
    }
}