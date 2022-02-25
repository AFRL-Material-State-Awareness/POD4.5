namespace POD.Controls
{
    partial class PODImageNumericUpDown
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
            this.RatingImages = new System.Windows.Forms.ImageList(this.components);
            this.ImageBox = new System.Windows.Forms.PictureBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.ValueNumeric = new POD.Controls.PODNumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ValueNumeric)).BeginInit();
            this.SuspendLayout();
            // 
            // RatingImages
            // 
            this.RatingImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
            this.RatingImages.ImageSize = new System.Drawing.Size(20, 20);
            this.RatingImages.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // ImageBox
            // 
            this.ImageBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ImageBox.Location = new System.Drawing.Point(52, 0);
            this.ImageBox.Margin = new System.Windows.Forms.Padding(0);
            this.ImageBox.MaximumSize = new System.Drawing.Size(60, 60);
            this.ImageBox.MinimumSize = new System.Drawing.Size(20, 20);
            this.ImageBox.Name = "ImageBox";
            this.ImageBox.Size = new System.Drawing.Size(20, 20);
            this.ImageBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.ImageBox.TabIndex = 1;
            this.ImageBox.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.ValueNumeric, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ImageBox, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 2);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(74, 22);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // ValueNumeric
            // 
            this.ValueNumeric.AutoSize = true;
            this.ValueNumeric.Dock = System.Windows.Forms.DockStyle.Top;
            this.ValueNumeric.Location = new System.Drawing.Point(0, 0);
            this.ValueNumeric.Margin = new System.Windows.Forms.Padding(0);
            this.ValueNumeric.Name = "ValueNumeric";
            this.ValueNumeric.Size = new System.Drawing.Size(52, 22);
            this.ValueNumeric.TabIndex = 0;
            this.ValueNumeric.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // PODImageNumericUpDown
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.Name = "PODImageNumericUpDown";
            this.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.Size = new System.Drawing.Size(74, 26);
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ValueNumeric)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected PODNumericUpDown ValueNumeric;
        protected System.Windows.Forms.PictureBox ImageBox;
        public System.Windows.Forms.ImageList RatingImages;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
