using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POD.Controls;

namespace POD.Wizards.Steps.AHatVsANormalSteps
{
    partial class ChooseTransformPanel
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.graphLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.YAxisTransformList = new POD.Controls.PODListBox();
            this.dataGridViewTextBoxColumn23 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn21 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn19 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn17 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn15 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn13 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label2 = new System.Windows.Forms.Label();
            this.InputLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.XAxisTransformList = new POD.Controls.PODListBox();
            this.dataGridViewTextBoxColumn24 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn22 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn20 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn18 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn16 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn14 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ResponseRangeHeader = new System.Windows.Forms.Label();
            this.FlawRangeHeader = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.FlawMaxNum = new POD.Controls.PODNumericUpDown();
            this.FlawMinNum = new POD.Controls.PODNumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.ResponseMaxNum = new POD.Controls.PODNumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.ResponseMinNum = new POD.Controls.PODNumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.YAxisTransformList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.XAxisTransformList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.FlawMaxNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.FlawMinNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ResponseMaxNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ResponseMinNum)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.graphLayoutPanel);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer1.Size = new System.Drawing.Size(739, 483);
            this.splitContainer1.SplitterDistance = 520;
            this.splitContainer1.TabIndex = 1;
            // 
            // graphLayoutPanel
            // 
            this.graphLayoutPanel.BackColor = System.Drawing.SystemColors.Window;
            this.graphLayoutPanel.ColumnCount = 4;
            this.graphLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.graphLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.graphLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.graphLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.graphLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graphLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.graphLayoutPanel.Name = "graphLayoutPanel";
            this.graphLayoutPanel.RowCount = 4;
            this.graphLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.graphLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.graphLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.graphLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.graphLayoutPanel.Size = new System.Drawing.Size(520, 483);
            this.graphLayoutPanel.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.YAxisTransformList, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.InputLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.XAxisTransformList, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.ResponseRangeHeader, 0, 9);
            this.tableLayoutPanel1.Controls.Add(this.FlawRangeHeader, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.FlawMaxNum, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.FlawMinNum, 1, 8);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 10);
            this.tableLayoutPanel1.Controls.Add(this.ResponseMaxNum, 1, 10);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 11);
            this.tableLayoutPanel1.Controls.Add(this.ResponseMinNum, 1, 11);
            this.tableLayoutPanel1.Controls.Add(this.label7, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label8, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.label9, 2, 4);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 13;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(215, 483);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // YAxisTransformList
            // 
            this.YAxisTransformList.AllowUserToAddRows = false;
            this.YAxisTransformList.AllowUserToDeleteRows = false;
            this.YAxisTransformList.AllowUserToResizeColumns = false;
            this.YAxisTransformList.AllowUserToResizeRows = false;
            this.YAxisTransformList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.YAxisTransformList.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            this.YAxisTransformList.BackgroundColor = System.Drawing.SystemColors.Window;
            this.YAxisTransformList.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.YAxisTransformList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.YAxisTransformList.ColumnHeadersVisible = false;
            this.YAxisTransformList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn23,
            this.dataGridViewTextBoxColumn21,
            this.dataGridViewTextBoxColumn19,
            this.dataGridViewTextBoxColumn17,
            this.dataGridViewTextBoxColumn15,
            this.dataGridViewTextBoxColumn13,
            this.dataGridViewTextBoxColumn11,
            this.dataGridViewTextBoxColumn9,
            this.dataGridViewTextBoxColumn7,
            this.dataGridViewTextBoxColumn5,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn1});
            this.tableLayoutPanel1.SetColumnSpan(this.YAxisTransformList, 3);
            this.YAxisTransformList.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.YAxisTransformList.GridColor = System.Drawing.SystemColors.Window;
            this.YAxisTransformList.Location = new System.Drawing.Point(3, 134);
            this.YAxisTransformList.Name = "YAxisTransformList";
            this.YAxisTransformList.ReadOnly = true;
            this.YAxisTransformList.RowHeadersVisible = false;
            this.YAxisTransformList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.YAxisTransformList.SingleSelectedIndex = -1;
            this.YAxisTransformList.Size = new System.Drawing.Size(209, 66);
            this.YAxisTransformList.StandardTab = true;
            this.YAxisTransformList.TabIndex = 2;
            // 
            // dataGridViewTextBoxColumn23
            // 
            this.dataGridViewTextBoxColumn23.Name = "dataGridViewTextBoxColumn23";
            this.dataGridViewTextBoxColumn23.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn21
            // 
            this.dataGridViewTextBoxColumn21.Name = "dataGridViewTextBoxColumn21";
            this.dataGridViewTextBoxColumn21.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn19
            // 
            this.dataGridViewTextBoxColumn19.Name = "dataGridViewTextBoxColumn19";
            this.dataGridViewTextBoxColumn19.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn17
            // 
            this.dataGridViewTextBoxColumn17.Name = "dataGridViewTextBoxColumn17";
            this.dataGridViewTextBoxColumn17.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn15
            // 
            this.dataGridViewTextBoxColumn15.Name = "dataGridViewTextBoxColumn15";
            this.dataGridViewTextBoxColumn15.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn13
            // 
            this.dataGridViewTextBoxColumn13.Name = "dataGridViewTextBoxColumn13";
            this.dataGridViewTextBoxColumn13.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn11
            // 
            this.dataGridViewTextBoxColumn11.Name = "dataGridViewTextBoxColumn11";
            this.dataGridViewTextBoxColumn11.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn9
            // 
            this.dataGridViewTextBoxColumn9.Name = "dataGridViewTextBoxColumn9";
            this.dataGridViewTextBoxColumn9.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label2, 2);
            this.label2.Location = new System.Drawing.Point(3, 118);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Response";
            // 
            // InputLabel
            // 
            this.InputLabel.AutoSize = true;
            this.InputLabel.BackColor = System.Drawing.SystemColors.ControlText;
            this.tableLayoutPanel1.SetColumnSpan(this.InputLabel, 3);
            this.InputLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InputLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InputLabel.ForeColor = System.Drawing.SystemColors.Window;
            this.InputLabel.Location = new System.Drawing.Point(0, 0);
            this.InputLabel.Margin = new System.Windows.Forms.Padding(0);
            this.InputLabel.Name = "InputLabel";
            this.InputLabel.Size = new System.Drawing.Size(215, 20);
            this.InputLabel.TabIndex = 4;
            this.InputLabel.Text = "INPUT";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 2);
            this.label1.Location = new System.Drawing.Point(3, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Flaw";
            // 
            // XAxisTransformList
            // 
            this.XAxisTransformList.AllowUserToAddRows = false;
            this.XAxisTransformList.AllowUserToDeleteRows = false;
            this.XAxisTransformList.AllowUserToResizeColumns = false;
            this.XAxisTransformList.AllowUserToResizeRows = false;
            this.XAxisTransformList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.XAxisTransformList.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            this.XAxisTransformList.BackgroundColor = System.Drawing.SystemColors.Window;
            this.XAxisTransformList.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.XAxisTransformList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.XAxisTransformList.ColumnHeadersVisible = false;
            this.XAxisTransformList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn24,
            this.dataGridViewTextBoxColumn22,
            this.dataGridViewTextBoxColumn20,
            this.dataGridViewTextBoxColumn18,
            this.dataGridViewTextBoxColumn16,
            this.dataGridViewTextBoxColumn14,
            this.dataGridViewTextBoxColumn12,
            this.dataGridViewTextBoxColumn10,
            this.dataGridViewTextBoxColumn8,
            this.dataGridViewTextBoxColumn6,
            this.dataGridViewTextBoxColumn4,
            this.dataGridViewTextBoxColumn2});
            this.tableLayoutPanel1.SetColumnSpan(this.XAxisTransformList, 3);
            this.XAxisTransformList.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.XAxisTransformList.GridColor = System.Drawing.SystemColors.Window;
            this.XAxisTransformList.Location = new System.Drawing.Point(3, 52);
            this.XAxisTransformList.Name = "XAxisTransformList";
            this.XAxisTransformList.ReadOnly = true;
            this.XAxisTransformList.RowHeadersVisible = false;
            this.XAxisTransformList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.XAxisTransformList.SingleSelectedIndex = -1;
            this.XAxisTransformList.Size = new System.Drawing.Size(209, 63);
            this.XAxisTransformList.StandardTab = true;
            this.XAxisTransformList.TabIndex = 1;
            // 
            // dataGridViewTextBoxColumn24
            // 
            this.dataGridViewTextBoxColumn24.Name = "dataGridViewTextBoxColumn24";
            this.dataGridViewTextBoxColumn24.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn22
            // 
            this.dataGridViewTextBoxColumn22.Name = "dataGridViewTextBoxColumn22";
            this.dataGridViewTextBoxColumn22.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn20
            // 
            this.dataGridViewTextBoxColumn20.Name = "dataGridViewTextBoxColumn20";
            this.dataGridViewTextBoxColumn20.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn18
            // 
            this.dataGridViewTextBoxColumn18.Name = "dataGridViewTextBoxColumn18";
            this.dataGridViewTextBoxColumn18.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn16
            // 
            this.dataGridViewTextBoxColumn16.Name = "dataGridViewTextBoxColumn16";
            this.dataGridViewTextBoxColumn16.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn14
            // 
            this.dataGridViewTextBoxColumn14.Name = "dataGridViewTextBoxColumn14";
            this.dataGridViewTextBoxColumn14.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn12
            // 
            this.dataGridViewTextBoxColumn12.Name = "dataGridViewTextBoxColumn12";
            this.dataGridViewTextBoxColumn12.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn10
            // 
            this.dataGridViewTextBoxColumn10.Name = "dataGridViewTextBoxColumn10";
            this.dataGridViewTextBoxColumn10.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn8
            // 
            this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            this.dataGridViewTextBoxColumn8.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // ResponseRangeHeader
            // 
            this.ResponseRangeHeader.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ResponseRangeHeader.AutoSize = true;
            this.ResponseRangeHeader.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel1.SetColumnSpan(this.ResponseRangeHeader, 3);
            this.ResponseRangeHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ResponseRangeHeader.Location = new System.Drawing.Point(0, 271);
            this.ResponseRangeHeader.Margin = new System.Windows.Forms.Padding(0);
            this.ResponseRangeHeader.Name = "ResponseRangeHeader";
            this.ResponseRangeHeader.Size = new System.Drawing.Size(215, 16);
            this.ResponseRangeHeader.TabIndex = 20;
            this.ResponseRangeHeader.Text = "Response Range";
            // 
            // FlawRangeHeader
            // 
            this.FlawRangeHeader.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FlawRangeHeader.AutoSize = true;
            this.FlawRangeHeader.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel1.SetColumnSpan(this.FlawRangeHeader, 3);
            this.FlawRangeHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FlawRangeHeader.Location = new System.Drawing.Point(0, 203);
            this.FlawRangeHeader.Margin = new System.Windows.Forms.Padding(0);
            this.FlawRangeHeader.Name = "FlawRangeHeader";
            this.FlawRangeHeader.Size = new System.Drawing.Size(215, 16);
            this.FlawRangeHeader.TabIndex = 19;
            this.FlawRangeHeader.Text = "Flaw Range";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 219);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(51, 26);
            this.label6.TabIndex = 9;
            this.label6.Text = "Maximum";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 245);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(51, 26);
            this.label5.TabIndex = 9;
            this.label5.Text = "Minimum";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FlawMaxNum
            // 
            this.FlawMaxNum.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.FlawMaxNum, 2);
            this.FlawMaxNum.Location = new System.Drawing.Point(60, 222);
            this.FlawMaxNum.Name = "FlawMaxNum";
            this.FlawMaxNum.Size = new System.Drawing.Size(152, 20);
            this.FlawMaxNum.TabIndex = 3;
            this.FlawMaxNum.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.FlawMaxNum.ValueChanged += new System.EventHandler(this.Range_ValueChanged);
            this.FlawMaxNum.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Numeric_KeyDown);
            // 
            // FlawMinNum
            // 
            this.FlawMinNum.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.FlawMinNum, 2);
            this.FlawMinNum.Location = new System.Drawing.Point(60, 248);
            this.FlawMinNum.Name = "FlawMinNum";
            this.FlawMinNum.Size = new System.Drawing.Size(152, 20);
            this.FlawMinNum.TabIndex = 4;
            this.FlawMinNum.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.FlawMinNum.ValueChanged += new System.EventHandler(this.Range_ValueChanged);
            this.FlawMinNum.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Numeric_KeyDown);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 287);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 26);
            this.label4.TabIndex = 9;
            this.label4.Text = "Maximum";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ResponseMaxNum
            // 
            this.ResponseMaxNum.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.ResponseMaxNum, 2);
            this.ResponseMaxNum.Location = new System.Drawing.Point(60, 290);
            this.ResponseMaxNum.Name = "ResponseMaxNum";
            this.ResponseMaxNum.Size = new System.Drawing.Size(152, 20);
            this.ResponseMaxNum.TabIndex = 5;
            this.ResponseMaxNum.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.ResponseMaxNum.ValueChanged += new System.EventHandler(this.Range_ValueChanged);
            this.ResponseMaxNum.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Numeric_KeyDown);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 313);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 26);
            this.label3.TabIndex = 9;
            this.label3.Text = "Minimum";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ResponseMinNum
            // 
            this.ResponseMinNum.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.ResponseMinNum, 2);
            this.ResponseMinNum.Location = new System.Drawing.Point(60, 316);
            this.ResponseMinNum.Name = "ResponseMinNum";
            this.ResponseMinNum.Size = new System.Drawing.Size(152, 20);
            this.ResponseMinNum.TabIndex = 6;
            this.ResponseMinNum.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.ResponseMinNum.ValueChanged += new System.EventHandler(this.Range_ValueChanged);
            this.ResponseMinNum.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Numeric_KeyDown);
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel1.SetColumnSpan(this.label7, 3);
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(0, 20);
            this.label7.Margin = new System.Windows.Forms.Padding(0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(215, 16);
            this.label7.TabIndex = 21;
            this.label7.Text = "Available Transforms";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(182, 36);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(30, 13);
            this.label8.TabIndex = 6;
            this.label8.Text = "View";
            this.label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(182, 118);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(30, 13);
            this.label9.TabIndex = 6;
            this.label9.Text = "View";
            this.label9.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // ChooseTransformPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.splitContainer1);
            this.Name = "ChooseTransformPanel";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.YAxisTransformList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.XAxisTransformList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.FlawMaxNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.FlawMinNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ResponseMaxNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ResponseMinNum)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private SplitContainer splitContainer1;
        private TableLayoutPanel tableLayoutPanel1;
        private Label label2;
        private Label InputLabel;
        private Label label1;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private PODNumericUpDown ResponseMaxNum;
        private PODNumericUpDown ResponseMinNum;
        private PODNumericUpDown FlawMinNum;
        private PODNumericUpDown FlawMaxNum;
        private PODListBox YAxisTransformList;
        private PODListBox XAxisTransformList;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private Label ResponseRangeHeader;
        private Label FlawRangeHeader;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private TableLayoutPanel graphLayoutPanel;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn9;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn10;
        private Label label7;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn11;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn12;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn13;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn14;
        private Label label8;
        private Label label9;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn15;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn16;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn17;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn18;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn19;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn20;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn21;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn22;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn23;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn24;



        //private ColorMap colorMap1;
    }
}
