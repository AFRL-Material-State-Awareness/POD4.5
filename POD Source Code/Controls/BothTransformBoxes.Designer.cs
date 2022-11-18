
namespace POD.Controls
{
    partial class BothTransformBoxes
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.podTableLayoutPanel1 = new POD.Controls.PODTableLayoutPanel(this.components);
            this.xTransform = new System.Windows.Forms.Label();
            this.yTransform = new System.Windows.Forms.Label();
            this.transformBox1 = new POD.Controls.TransformBox();
            this.transformBoxYHat1 = new POD.Controls.TransformBoxYHat();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.podTableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.Controls.Add(this.podTableLayoutPanel1);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(204, 92);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // podTableLayoutPanel1
            // 
            this.podTableLayoutPanel1.AutoSize = true;
            this.podTableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.podTableLayoutPanel1.ColumnCount = 2;
            this.podTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.podTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.podTableLayoutPanel1.Controls.Add(this.xTransform, 0, 0);
            this.podTableLayoutPanel1.Controls.Add(this.yTransform, 0, 1);
            this.podTableLayoutPanel1.Controls.Add(this.transformBox1, 1, 0);
            this.podTableLayoutPanel1.Controls.Add(this.transformBoxYHat1, 1, 1);
            this.podTableLayoutPanel1.Location = new System.Drawing.Point(6, 19);
            this.podTableLayoutPanel1.Name = "podTableLayoutPanel1";
            this.podTableLayoutPanel1.RowCount = 2;
            this.podTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.podTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.podTableLayoutPanel1.Size = new System.Drawing.Size(192, 54);
            this.podTableLayoutPanel1.TabIndex = 0;
            // 
            // xTransform
            // 
            this.xTransform.AutoSize = true;
            this.xTransform.Location = new System.Drawing.Point(3, 0);
            this.xTransform.Name = "xTransform";
            this.xTransform.Size = new System.Drawing.Size(59, 13);
            this.xTransform.TabIndex = 0;
            this.xTransform.Text = "xTransform";
            // 
            // yTransform
            // 
            this.yTransform.AutoSize = true;
            this.yTransform.Location = new System.Drawing.Point(3, 27);
            this.yTransform.Name = "yTransform";
            this.yTransform.Size = new System.Drawing.Size(59, 13);
            this.yTransform.TabIndex = 1;
            this.yTransform.Text = "yTransform";
            // 
            // transformBox1
            // 
            this.transformBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.transformBox1.FormattingEnabled = true;
            this.transformBox1.Location = new System.Drawing.Point(68, 3);
            this.transformBox1.Name = "transformBox1";
            this.transformBox1.Size = new System.Drawing.Size(121, 21);
            this.transformBox1.TabIndex = 2;
            // 
            // transformBoxYHat1
            // 
            this.transformBoxYHat1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.transformBoxYHat1.FormattingEnabled = true;
            this.transformBoxYHat1.Location = new System.Drawing.Point(68, 30);
            this.transformBoxYHat1.Name = "transformBoxYHat1";
            this.transformBoxYHat1.SelectedTransform = POD.TransformTypeEnum.Linear;
            this.transformBoxYHat1.Size = new System.Drawing.Size(121, 21);
            this.transformBoxYHat1.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "xTransform";
            // 
            // BothTransformBoxes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.groupBox1);
            this.Name = "BothTransformBoxes";
            this.Size = new System.Drawing.Size(210, 98);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.podTableLayoutPanel1.ResumeLayout(false);
            this.podTableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private PODTableLayoutPanel podTableLayoutPanel1;
        private System.Windows.Forms.Label xTransform;
        private System.Windows.Forms.Label yTransform;
        private TransformBox transformBox1;
        private TransformBoxYHat transformBoxYHat1;
        private System.Windows.Forms.Label label1;
    }
}
