namespace POD.Wizards
{
    partial class NewHitMissRangeForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewHitMissRangeForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label7 = new System.Windows.Forms.Label();
            this.MinFlaw = new POD.Controls.PODChartNumericUpDown();
            this.MaxFlaw = new POD.Controls.PODChartNumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.ApplyButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.operatorComboBox = new System.Windows.Forms.ComboBox();
            this.specimenComboBox = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.SpecimenUnitComboBox = new System.Windows.Forms.ComboBox();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MinFlaw)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MaxFlaw)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label7, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.MinFlaw, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.MaxFlaw, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.ApplyButton, 0, 11);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.label8, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label9, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.operatorComboBox, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.specimenComboBox, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label12, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.SpecimenUnitComboBox, 3, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 12;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(330, 253);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label7, 4);
            this.label7.Dock = System.Windows.Forms.DockStyle.Left;
            this.label7.Location = new System.Drawing.Point(3, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(310, 47);
            this.label7.TabIndex = 27;
            this.label7.Text = "Please provide information about the person collecting the data, the specimen set" +
    " and instrument used.";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // MinFlaw
            // 
            this.MinFlaw.AutoSize = true;
            this.MinFlaw.DecimalPlaces = 3;
            this.MinFlaw.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MinFlaw.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.MinFlaw.InterceptArrowKeys = true;
            this.MinFlaw.Location = new System.Drawing.Point(82, 193);
            this.MinFlaw.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.MinFlaw.Minimum = new decimal(new int[] {
            1000000000,
            0,
            0,
            -2147483648});
            this.MinFlaw.Name = "MinFlaw";
            this.MinFlaw.PartType = POD.ChartPartType.Undefined;
            this.MinFlaw.ReadOnly = false;
            this.MinFlaw.Size = new System.Drawing.Size(135, 20);
            this.MinFlaw.TabIndex = 6;
            this.MinFlaw.TooltipForNumeric = "";
            this.MinFlaw.Value = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.MinFlaw.Enter += new System.EventHandler(this.Numeric_Entered);
            this.MinFlaw.Validating += new System.ComponentModel.CancelEventHandler(this.Numeric_Validating);
            this.MinFlaw.Validated += new System.EventHandler(this.Numeric_Validated);
            // 
            // MaxFlaw
            // 
            this.MaxFlaw.AutoSize = true;
            this.MaxFlaw.DecimalPlaces = 3;
            this.MaxFlaw.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MaxFlaw.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.MaxFlaw.InterceptArrowKeys = true;
            this.MaxFlaw.Location = new System.Drawing.Point(82, 167);
            this.MaxFlaw.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.MaxFlaw.Minimum = new decimal(new int[] {
            1000000000,
            0,
            0,
            -2147483648});
            this.MaxFlaw.Name = "MaxFlaw";
            this.MaxFlaw.PartType = POD.ChartPartType.Undefined;
            this.MaxFlaw.ReadOnly = false;
            this.MaxFlaw.Size = new System.Drawing.Size(135, 20);
            this.MaxFlaw.TabIndex = 5;
            this.MaxFlaw.TooltipForNumeric = "";
            this.MaxFlaw.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.MaxFlaw.Enter += new System.EventHandler(this.Numeric_Entered);
            this.MaxFlaw.Validating += new System.ComponentModel.CancelEventHandler(this.Numeric_Validating);
            this.MaxFlaw.Validated += new System.EventHandler(this.Numeric_Validated);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Location = new System.Drawing.Point(3, 190);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(73, 26);
            this.label6.TabIndex = 24;
            this.label6.Text = "Minimum";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(3, 164);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 26);
            this.label5.TabIndex = 23;
            this.label5.Text = "Maximum";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel1.SetColumnSpan(this.label4, 4);
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(0, 148);
            this.label4.Margin = new System.Windows.Forms.Padding(0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(330, 16);
            this.label4.TabIndex = 22;
            this.label4.Text = "Specimens Flaw Range";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ApplyButton
            // 
            this.ApplyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.ApplyButton, 4);
            this.ApplyButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ApplyButton.Location = new System.Drawing.Point(252, 227);
            this.ApplyButton.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.ApplyButton.Name = "ApplyButton";
            this.ApplyButton.Size = new System.Drawing.Size(75, 23);
            this.ApplyButton.TabIndex = 9;
            this.ApplyButton.Text = "Apply";
            this.toolTip1.SetToolTip(this.ApplyButton, "Applies current settings to the quick analysis.");
            this.ApplyButton.UseVisualStyleBackColor = true;
            this.ApplyButton.Click += new System.EventHandler(this.ApplyButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label3, 4);
            this.label3.Dock = System.Windows.Forms.DockStyle.Left;
            this.label3.Location = new System.Drawing.Point(3, 101);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(274, 47);
            this.label3.TabIndex = 21;
            this.label3.Text = "Please provide flaw range of the specimens used before continuing.";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(28, 54);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(48, 13);
            this.label8.TabIndex = 32;
            this.label8.Text = "Operator";
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 81);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(73, 13);
            this.label9.TabIndex = 30;
            this.label9.Text = "Specimen Set";
            // 
            // operatorComboBox
            // 
            this.operatorComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.operatorComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.operatorComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.operatorComboBox.FormattingEnabled = true;
            this.operatorComboBox.Location = new System.Drawing.Point(82, 50);
            this.operatorComboBox.MaximumSize = new System.Drawing.Size(200, 0);
            this.operatorComboBox.Name = "operatorComboBox";
            this.operatorComboBox.Size = new System.Drawing.Size(135, 21);
            this.operatorComboBox.TabIndex = 0;
            this.toolTip1.SetToolTip(this.operatorComboBox, "Name of operator that collected data.");
            // 
            // specimenComboBox
            // 
            this.specimenComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.specimenComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.specimenComboBox.FormattingEnabled = true;
            this.specimenComboBox.Location = new System.Drawing.Point(82, 77);
            this.specimenComboBox.MaximumSize = new System.Drawing.Size(200, 0);
            this.specimenComboBox.Name = "specimenComboBox";
            this.specimenComboBox.Size = new System.Drawing.Size(135, 21);
            this.specimenComboBox.TabIndex = 1;
            this.toolTip1.SetToolTip(this.specimenComboBox, "Name of specimen set that data was taken from.");
            this.specimenComboBox.SelectedValueChanged += new System.EventHandler(this.Specimen_SelectedValueChanged);
            // 
            // label12
            // 
            this.label12.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(223, 81);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(31, 13);
            this.label12.TabIndex = 29;
            this.label12.Text = "Units";
            // 
            // SpecimenUnitComboBox
            // 
            this.SpecimenUnitComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.SpecimenUnitComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.SpecimenUnitComboBox.FormattingEnabled = true;
            this.SpecimenUnitComboBox.Location = new System.Drawing.Point(260, 77);
            this.SpecimenUnitComboBox.MaximumSize = new System.Drawing.Size(200, 0);
            this.SpecimenUnitComboBox.Name = "SpecimenUnitComboBox";
            this.SpecimenUnitComboBox.Size = new System.Drawing.Size(67, 21);
            this.SpecimenUnitComboBox.TabIndex = 2;
            this.toolTip1.SetToolTip(this.SpecimenUnitComboBox, "Specimen\'s flaw units.");
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Check.png");
            this.imageList1.Images.SetKeyName(1, "X.png");
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // NewHitMissRangeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 253);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewHitMissRangeForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Enter Response Range Values";
            this.TopMost = true;
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MinFlaw)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MaxFlaw)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button ApplyButton;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Label label3;
        private Controls.PODChartNumericUpDown MinFlaw;
        private Controls.PODChartNumericUpDown MaxFlaw;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox operatorComboBox;
        private System.Windows.Forms.ComboBox specimenComboBox;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox SpecimenUnitComboBox;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ErrorProvider errorProvider1;

    }
}