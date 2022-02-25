using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POD.Controls;

namespace POD.Wizards.Steps.AHatVsANormalSteps
{
    partial class FullRegressionPanel
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
            this.linearityChart = new POD.Controls.CensoredLinearityChart();
            this.normalityChart = new POD.Controls.DataPointChart();
            this.equalVarianceChart = new POD.Controls.DataPointChart();
            this.podChart = new POD.Controls.PODChart();
            this.thresholdChart = new POD.Controls.PODThresholdChart();
            this.mainChart = new POD.Controls.AHatVsARegressionChart();
            this.leftCensorControl = new POD.Controls.PODChartNumericUpDown();
            this.rightCensorControl = new POD.Controls.PODChartNumericUpDown();
            this.aMaxControl = new POD.Controls.PODChartNumericUpDown();
            this.AMaxInputLabel = new System.Windows.Forms.Label();
            this.aMinControl = new POD.Controls.PODChartNumericUpDown();
            this.AMinInputLabel = new System.Windows.Forms.Label();
            this.LeftCensorInputLabel = new System.Windows.Forms.Label();
            this.RightCensorInputLabel = new System.Windows.Forms.Label();
            this.thresholdLabel = new System.Windows.Forms.Label();
            this.thresholdControl = new POD.Controls.PODChartNumericUpDown();
            this.modelErrorOut = new POD.Controls.PODChartNumericUpDown();
            this.modelErrorLabel = new System.Windows.Forms.Label();
            this.modelBOut = new POD.Controls.PODChartNumericUpDown();
            this.a50label = new System.Windows.Forms.Label();
            this.modelBLabel = new System.Windows.Forms.Label();
            this.a50Out = new POD.Controls.PODChartNumericUpDown();
            this.modelMLabel = new System.Windows.Forms.Label();
            this.a90Label = new System.Windows.Forms.Label();
            this.a90Out = new POD.Controls.PODChartNumericUpDown();
            this.a90_95Label = new System.Windows.Forms.Label();
            this.a90_95Out = new POD.Controls.PODChartNumericUpDown();
            this.modelMOut = new POD.Controls.PODChartNumericUpDown();
            this.modelErrorStdErrOut = new POD.Controls.PODChartNumericUpDown();
            this.modelErrorStdErrLabel = new System.Windows.Forms.Label();
            this.modelBStdErrOut = new POD.Controls.PODChartNumericUpDown();
            this.modelBStdErrLabel = new System.Windows.Forms.Label();
            this.modelMStdErrOut = new POD.Controls.PODChartNumericUpDown();
            this.modelMStdErrLabel = new System.Windows.Forms.Label();
            this.normalityTestOut = new POD.Controls.PODRatedNumericUpDown();
            this.normalityTestLabel = new System.Windows.Forms.Label();
            this.lackOfFitTestLabel = new System.Windows.Forms.Label();
            this.lackOfFitTestOut = new POD.Controls.PODRatedNumericUpDown();
            this.equalVarianceTestLabel = new System.Windows.Forms.Label();
            this.equalVarianceTestOut = new POD.Controls.PODRatedNumericUpDown();
            this.TestColorMap = new POD.Controls.ColorMap();
            this.repeatabilityErrorLabel = new System.Windows.Forms.Label();
            this.repeatabilityErrorOut = new POD.Controls.PODChartNumericUpDown();
            this.PodModelParametersHeader = new System.Windows.Forms.Label();
            this.LinearFitEstimatesHeader = new System.Windows.Forms.Label();
            this.LinearFitStdErrorHeader = new System.Windows.Forms.Label();
            this.TestOfAssumptionsHeader = new System.Windows.Forms.Label();
            this.AxisTransformsHeader = new System.Windows.Forms.Label();
            this.FlawRangeHeader = new System.Windows.Forms.Label();
            this.ResponseRangeHeader = new System.Windows.Forms.Label();
            this.PODDecisionHeader = new System.Windows.Forms.Label();
            this.SigmaOut = new POD.Controls.PODChartNumericUpDown();
            this.SigmaLabel = new System.Windows.Forms.Label();
            this.MuOut = new POD.Controls.PODChartNumericUpDown();
            this.MuLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.graphSplitter)).BeginInit();
            this.graphSplitter.Panel2.SuspendLayout();
            this.graphSplitter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MainChartRightControlsSplitter)).BeginInit();
            this.MainChartRightControlsSplitter.Panel1.SuspendLayout();
            this.MainChartRightControlsSplitter.Panel2.SuspendLayout();
            this.MainChartRightControlsSplitter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.graphSplitterOverlay)).BeginInit();
            this.PutControlsHerePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.linearityChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.normalityChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.equalVarianceChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.podChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.thresholdChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mainChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftCensorControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightCensorControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.aMaxControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.aMinControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.thresholdControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.modelErrorOut)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.modelBOut)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.a50Out)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.a90Out)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.a90_95Out)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.modelMOut)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.modelErrorStdErrOut)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.modelBStdErrOut)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.modelMStdErrOut)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.normalityTestOut)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lackOfFitTestOut)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.equalVarianceTestOut)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repeatabilityErrorOut)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SigmaOut)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MuOut)).BeginInit();
            this.SuspendLayout();
            // 
            // graphSplitter
            // 
            // 
            // graphSplitter.Panel2
            // 
            this.graphSplitter.Panel2.Controls.Add(this.mainChart);
            // 
            // MainChartRightControlsSplitter
            // 
            // 
            // graphSplitterOverlay
            // 
            this.graphSplitterOverlay.BackColor = System.Drawing.Color.Red;
            // 
            // PutControlsHerePanel
            // 
            this.PutControlsHerePanel.Controls.Add(this.MuOut);
            this.PutControlsHerePanel.Controls.Add(this.MuLabel);
            this.PutControlsHerePanel.Controls.Add(this.SigmaOut);
            this.PutControlsHerePanel.Controls.Add(this.SigmaLabel);
            this.PutControlsHerePanel.Controls.Add(this.PODDecisionHeader);
            this.PutControlsHerePanel.Controls.Add(this.ResponseRangeHeader);
            this.PutControlsHerePanel.Controls.Add(this.FlawRangeHeader);
            this.PutControlsHerePanel.Controls.Add(this.TestOfAssumptionsHeader);
            this.PutControlsHerePanel.Controls.Add(this.LinearFitStdErrorHeader);
            this.PutControlsHerePanel.Controls.Add(this.repeatabilityErrorOut);
            this.PutControlsHerePanel.Controls.Add(this.repeatabilityErrorLabel);
            this.PutControlsHerePanel.Controls.Add(this.TestColorMap);
            this.PutControlsHerePanel.Controls.Add(this.equalVarianceTestOut);
            this.PutControlsHerePanel.Controls.Add(this.equalVarianceTestLabel);
            this.PutControlsHerePanel.Controls.Add(this.lackOfFitTestOut);
            this.PutControlsHerePanel.Controls.Add(this.LinearFitEstimatesHeader);
            this.PutControlsHerePanel.Controls.Add(this.AxisTransformsHeader);
            this.PutControlsHerePanel.Controls.Add(this.PodModelParametersHeader);
            this.PutControlsHerePanel.Controls.Add(this.lackOfFitTestLabel);
            this.PutControlsHerePanel.Controls.Add(this.normalityTestOut);
            this.PutControlsHerePanel.Controls.Add(this.normalityTestLabel);
            this.PutControlsHerePanel.Controls.Add(this.modelErrorStdErrOut);
            this.PutControlsHerePanel.Controls.Add(this.modelErrorStdErrLabel);
            this.PutControlsHerePanel.Controls.Add(this.modelBStdErrOut);
            this.PutControlsHerePanel.Controls.Add(this.modelBStdErrLabel);
            this.PutControlsHerePanel.Controls.Add(this.modelMStdErrOut);
            this.PutControlsHerePanel.Controls.Add(this.modelMStdErrLabel);
            this.PutControlsHerePanel.Size = new System.Drawing.Size(199, 522);
            // 
            // linearityChart
            // 
            this.linearityChart.BorderlineColor = System.Drawing.SystemColors.ControlDark;
            this.linearityChart.CanUnselect = false;
            this.linearityChart.ChartTitle = "";
            this.linearityChart.ChartToolTip = null;
            this.linearityChart.IsSelected = false;
            this.linearityChart.IsSquare = false;
            this.linearityChart.Location = new System.Drawing.Point(3, 3);
            this.linearityChart.Name = "linearityChart";
            this.linearityChart.Selectable = false;
            this.linearityChart.ShowChartTitle = true;
            this.linearityChart.SingleSeriesCount = 1;
            this.linearityChart.Size = new System.Drawing.Size(160, 160);
            this.linearityChart.TabIndex = 0;
            this.linearityChart.TabStop = false;
            this.linearityChart.Text = "aHatVsAChart1";
            this.linearityChart.XAxisTitle = "";
            this.linearityChart.XAxisUnit = "";
            this.linearityChart.YAxisTitle = "";
            this.linearityChart.YAxisUnit = "";
            // 
            // normalityChart
            // 
            this.normalityChart.BorderlineColor = System.Drawing.Color.Transparent;
            this.normalityChart.CanUnselect = false;
            this.normalityChart.ChartTitle = "";
            this.normalityChart.ChartToolTip = null;
            this.normalityChart.IsSelected = false;
            this.normalityChart.IsSquare = false;
            this.normalityChart.Location = new System.Drawing.Point(3, 169);
            this.normalityChart.Name = "normalityChart";
            this.normalityChart.Selectable = false;
            this.normalityChart.ShowChartTitle = true;
            this.normalityChart.SingleSeriesCount = 1;
            this.normalityChart.Size = new System.Drawing.Size(160, 160);
            this.normalityChart.TabIndex = 0;
            this.normalityChart.TabStop = false;
            this.normalityChart.Text = "aHatVsAChart1";
            this.normalityChart.XAxisTitle = "";
            this.normalityChart.XAxisUnit = "";
            this.normalityChart.YAxisTitle = "";
            this.normalityChart.YAxisUnit = "";
            // 
            // equalVarianceChart
            // 
            this.equalVarianceChart.BorderlineColor = System.Drawing.Color.Transparent;
            this.equalVarianceChart.CanUnselect = false;
            this.equalVarianceChart.ChartTitle = "";
            this.equalVarianceChart.ChartToolTip = null;
            this.equalVarianceChart.IsSelected = false;
            this.equalVarianceChart.IsSquare = false;
            this.equalVarianceChart.Location = new System.Drawing.Point(3, 335);
            this.equalVarianceChart.Name = "equalVarianceChart";
            this.equalVarianceChart.Selectable = false;
            this.equalVarianceChart.ShowChartTitle = true;
            this.equalVarianceChart.SingleSeriesCount = 1;
            this.equalVarianceChart.Size = new System.Drawing.Size(160, 160);
            this.equalVarianceChart.TabIndex = 0;
            this.equalVarianceChart.TabStop = false;
            this.equalVarianceChart.Text = "aHatVsAChart1";
            this.equalVarianceChart.XAxisTitle = "";
            this.equalVarianceChart.XAxisUnit = "";
            this.equalVarianceChart.YAxisTitle = "";
            this.equalVarianceChart.YAxisUnit = "";
            // 
            // podChart
            // 
            this.podChart.BorderlineColor = System.Drawing.Color.Transparent;
            this.podChart.CanUnselect = false;
            this.podChart.ChartTitle = "";
            this.podChart.ChartToolTip = null;
            this.podChart.IsSelected = false;
            this.podChart.IsSquare = false;
            this.podChart.Location = new System.Drawing.Point(3, 501);
            this.podChart.Name = "podChart";
            this.podChart.Selectable = false;
            this.podChart.ShowChartTitle = true;
            this.podChart.SingleSeriesCount = 1;
            this.podChart.Size = new System.Drawing.Size(160, 160);
            this.podChart.TabIndex = 1;
            this.podChart.TabStop = false;
            this.podChart.Text = "aHatVsAChart5";
            this.podChart.XAxisTitle = "";
            this.podChart.XAxisUnit = "";
            this.podChart.YAxisTitle = "";
            this.podChart.YAxisUnit = "";
            // 
            // thresholdChart
            // 
            this.thresholdChart.BorderlineColor = System.Drawing.Color.Transparent;
            this.thresholdChart.CanUnselect = false;
            this.thresholdChart.ChartTitle = "";
            this.thresholdChart.ChartToolTip = null;
            this.thresholdChart.IsSelected = false;
            this.thresholdChart.IsSquare = false;
            this.thresholdChart.Location = new System.Drawing.Point(3, 667);
            this.thresholdChart.Name = "thresholdChart";
            this.thresholdChart.Selectable = false;
            this.thresholdChart.ShowChartTitle = true;
            this.thresholdChart.SingleSeriesCount = 1;
            this.thresholdChart.Size = new System.Drawing.Size(160, 160);
            this.thresholdChart.TabIndex = 2;
            this.thresholdChart.TabStop = false;
            this.thresholdChart.Text = "aHatVsAChart6";
            this.thresholdChart.XAxisTitle = "";
            this.thresholdChart.XAxisUnit = "";
            this.thresholdChart.YAxisTitle = "";
            this.thresholdChart.YAxisUnit = "";
            // 
            // mainChart
            // 
            this.mainChart.BorderlineColor = System.Drawing.SystemColors.ControlDark;
            this.mainChart.CanUnselect = false;
            this.mainChart.ChartTitle = "";
            this.mainChart.ChartToolTip = null;
            this.mainChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainChart.IsSelected = false;
            this.mainChart.IsSquare = false;
            this.mainChart.Location = new System.Drawing.Point(0, 0);
            this.mainChart.Name = "mainChart";
            this.mainChart.Selectable = false;
            this.mainChart.ShowChartTitle = false;
            this.mainChart.SingleSeriesCount = 1;
            this.mainChart.Size = new System.Drawing.Size(634, 640);
            this.mainChart.TabIndex = 0;
            this.mainChart.TabStop = false;
            this.mainChart.Text = "aHatVsAChart1";
            this.mainChart.XAxisTitle = "";
            this.mainChart.XAxisUnit = "";
            this.mainChart.YAxisTitle = "";
            this.mainChart.YAxisUnit = "";
            // 
            // leftCensorControl
            // 
            this.leftCensorControl.AutoSize = true;
            this.leftCensorControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.leftCensorControl.DecimalPlaces = 0;
            this.leftCensorControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.leftCensorControl.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.leftCensorControl.InterceptArrowKeys = true;
            this.leftCensorControl.Location = new System.Drawing.Point(3, 133);
            this.leftCensorControl.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.leftCensorControl.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.leftCensorControl.Name = "leftCensorControl";
            this.leftCensorControl.PartType = POD.ChartPartType.Undefined;
            this.leftCensorControl.ReadOnly = false;
            this.leftCensorControl.Size = new System.Drawing.Size(153, 20);
            this.leftCensorControl.TabIndex = 5;
            this.leftCensorControl.TooltipForNumeric = "";
            this.leftCensorControl.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // rightCensorControl
            // 
            this.rightCensorControl.AutoSize = true;
            this.rightCensorControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.rightCensorControl.DecimalPlaces = 0;
            this.rightCensorControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightCensorControl.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.rightCensorControl.InterceptArrowKeys = true;
            this.rightCensorControl.Location = new System.Drawing.Point(3, 94);
            this.rightCensorControl.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.rightCensorControl.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.rightCensorControl.Name = "rightCensorControl";
            this.rightCensorControl.PartType = POD.ChartPartType.Undefined;
            this.rightCensorControl.ReadOnly = false;
            this.rightCensorControl.Size = new System.Drawing.Size(153, 20);
            this.rightCensorControl.TabIndex = 7;
            this.rightCensorControl.TooltipForNumeric = "";
            this.rightCensorControl.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // aMaxControl
            // 
            this.aMaxControl.AutoSize = true;
            this.aMaxControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(255)))));
            this.aMaxControl.DecimalPlaces = 0;
            this.aMaxControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.aMaxControl.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.aMaxControl.InterceptArrowKeys = true;
            this.aMaxControl.Location = new System.Drawing.Point(3, 16);
            this.aMaxControl.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.aMaxControl.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.aMaxControl.Name = "aMaxControl";
            this.aMaxControl.PartType = POD.ChartPartType.Undefined;
            this.aMaxControl.ReadOnly = false;
            this.aMaxControl.Size = new System.Drawing.Size(153, 20);
            this.aMaxControl.TabIndex = 0;
            this.aMaxControl.TooltipForNumeric = "";
            this.aMaxControl.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // AMaxInputLabel
            // 
            this.AMaxInputLabel.AutoSize = true;
            this.AMaxInputLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AMaxInputLabel.Location = new System.Drawing.Point(3, 0);
            this.AMaxInputLabel.Name = "AMaxInputLabel";
            this.AMaxInputLabel.Size = new System.Drawing.Size(153, 13);
            this.AMaxInputLabel.TabIndex = 1;
            this.AMaxInputLabel.Text = "Maximum";
            // 
            // aMinControl
            // 
            this.aMinControl.AutoSize = true;
            this.aMinControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(255)))));
            this.aMinControl.DecimalPlaces = 0;
            this.aMinControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.aMinControl.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.aMinControl.InterceptArrowKeys = true;
            this.aMinControl.Location = new System.Drawing.Point(3, 55);
            this.aMinControl.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.aMinControl.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.aMinControl.Name = "aMinControl";
            this.aMinControl.PartType = POD.ChartPartType.Undefined;
            this.aMinControl.ReadOnly = false;
            this.aMinControl.Size = new System.Drawing.Size(153, 20);
            this.aMinControl.TabIndex = 3;
            this.aMinControl.TooltipForNumeric = "";
            this.aMinControl.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // AMinInputLabel
            // 
            this.AMinInputLabel.AutoSize = true;
            this.AMinInputLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AMinInputLabel.Location = new System.Drawing.Point(3, 39);
            this.AMinInputLabel.Name = "AMinInputLabel";
            this.AMinInputLabel.Size = new System.Drawing.Size(153, 13);
            this.AMinInputLabel.TabIndex = 2;
            this.AMinInputLabel.Text = "Minimum";
            // 
            // LeftCensorInputLabel
            // 
            this.LeftCensorInputLabel.AutoSize = true;
            this.LeftCensorInputLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LeftCensorInputLabel.Location = new System.Drawing.Point(3, 117);
            this.LeftCensorInputLabel.Name = "LeftCensorInputLabel";
            this.LeftCensorInputLabel.Size = new System.Drawing.Size(153, 13);
            this.LeftCensorInputLabel.TabIndex = 4;
            this.LeftCensorInputLabel.Text = "Minimum";
            // 
            // RightCensorInputLabel
            // 
            this.RightCensorInputLabel.AutoSize = true;
            this.RightCensorInputLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RightCensorInputLabel.Location = new System.Drawing.Point(3, 78);
            this.RightCensorInputLabel.Name = "RightCensorInputLabel";
            this.RightCensorInputLabel.Size = new System.Drawing.Size(153, 13);
            this.RightCensorInputLabel.TabIndex = 6;
            this.RightCensorInputLabel.Text = "Maximum";
            // 
            // thresholdLabel
            // 
            this.thresholdLabel.AutoSize = true;
            this.thresholdLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.thresholdLabel.Location = new System.Drawing.Point(3, 156);
            this.thresholdLabel.Name = "thresholdLabel";
            this.thresholdLabel.Size = new System.Drawing.Size(153, 13);
            this.thresholdLabel.TabIndex = 8;
            this.thresholdLabel.Text = "Threshold";
            // 
            // thresholdControl
            // 
            this.thresholdControl.AutoSize = true;
            this.thresholdControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(255)))), ((int)(((byte)(235)))));
            this.thresholdControl.DecimalPlaces = 0;
            this.thresholdControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.thresholdControl.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.thresholdControl.InterceptArrowKeys = true;
            this.thresholdControl.Location = new System.Drawing.Point(3, 172);
            this.thresholdControl.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.thresholdControl.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.thresholdControl.Name = "thresholdControl";
            this.thresholdControl.PartType = POD.ChartPartType.Undefined;
            this.thresholdControl.ReadOnly = false;
            this.thresholdControl.Size = new System.Drawing.Size(153, 20);
            this.thresholdControl.TabIndex = 9;
            this.thresholdControl.TooltipForNumeric = "";
            this.thresholdControl.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // modelErrorOut
            // 
            this.modelErrorOut.AutoSize = true;
            this.modelErrorOut.DecimalPlaces = 3;
            this.modelErrorOut.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.modelErrorOut.InterceptArrowKeys = false;
            this.modelErrorOut.Location = new System.Drawing.Point(3, 211);
            this.modelErrorOut.Maximum = new decimal(new int[] {
            1215752192,
            23,
            0,
            0});
            this.modelErrorOut.Minimum = new decimal(new int[] {
            1215752192,
            23,
            0,
            -2147483648});
            this.modelErrorOut.Name = "modelErrorOut";
            this.modelErrorOut.PartType = POD.ChartPartType.Undefined;
            this.modelErrorOut.ReadOnly = true;
            this.modelErrorOut.Size = new System.Drawing.Size(153, 20);
            this.modelErrorOut.TabIndex = 15;
            this.modelErrorOut.TooltipForNumeric = "";
            this.modelErrorOut.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // modelErrorLabel
            // 
            this.modelErrorLabel.AutoSize = true;
            this.modelErrorLabel.Location = new System.Drawing.Point(3, 195);
            this.modelErrorLabel.Name = "modelErrorLabel";
            this.modelErrorLabel.Size = new System.Drawing.Size(43, 13);
            this.modelErrorLabel.TabIndex = 14;
            this.modelErrorLabel.Text = "Residual Error";
            // 
            // modelBOut
            // 
            this.modelBOut.AutoSize = true;
            this.modelBOut.DecimalPlaces = 3;
            this.modelBOut.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modelBOut.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.modelBOut.InterceptArrowKeys = false;
            this.modelBOut.Location = new System.Drawing.Point(3, 172);
            this.modelBOut.Maximum = new decimal(new int[] {
            1215752192,
            23,
            0,
            0});
            this.modelBOut.Minimum = new decimal(new int[] {
            1215752192,
            23,
            0,
            -2147483648});
            this.modelBOut.Name = "modelBOut";
            this.modelBOut.PartType = POD.ChartPartType.Undefined;
            this.modelBOut.ReadOnly = true;
            this.modelBOut.Size = new System.Drawing.Size(153, 20);
            this.modelBOut.TabIndex = 13;
            this.modelBOut.TooltipForNumeric = "";
            this.modelBOut.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // a50label
            // 
            this.a50label.AutoSize = true;
            this.a50label.Location = new System.Drawing.Point(3, 0);
            this.a50label.Name = "a50label";
            this.a50label.Size = new System.Drawing.Size(25, 13);
            this.a50label.TabIndex = 0;
            this.a50label.Text = "a50";
            // 
            // modelBLabel
            // 
            this.modelBLabel.AutoSize = true;
            this.modelBLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modelBLabel.Location = new System.Drawing.Point(3, 156);
            this.modelBLabel.Name = "modelBLabel";
            this.modelBLabel.Size = new System.Drawing.Size(153, 13);
            this.modelBLabel.TabIndex = 12;
            this.modelBLabel.Text = "Intercept";
            // 
            // a50Out
            // 
            this.a50Out.AutoSize = true;
            this.a50Out.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(255)))), ((int)(((byte)(175)))));
            this.a50Out.DecimalPlaces = 3;
            this.a50Out.Dock = System.Windows.Forms.DockStyle.Fill;
            this.a50Out.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.a50Out.InterceptArrowKeys = true;
            this.a50Out.Location = new System.Drawing.Point(3, 16);
            this.a50Out.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.a50Out.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.a50Out.Name = "a50Out";
            this.a50Out.PartType = POD.ChartPartType.Undefined;
            this.a50Out.ReadOnly = true;
            this.a50Out.Size = new System.Drawing.Size(153, 20);
            this.a50Out.TabIndex = 1;
            this.a50Out.TooltipForNumeric = "";
            this.a50Out.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // modelMLabel
            // 
            this.modelMLabel.AutoSize = true;
            this.modelMLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modelMLabel.Location = new System.Drawing.Point(3, 117);
            this.modelMLabel.Name = "modelMLabel";
            this.modelMLabel.Size = new System.Drawing.Size(153, 13);
            this.modelMLabel.TabIndex = 10;
            this.modelMLabel.Text = "Slope";
            // 
            // a90Label
            // 
            this.a90Label.AutoSize = true;
            this.a90Label.Dock = System.Windows.Forms.DockStyle.Fill;
            this.a90Label.Location = new System.Drawing.Point(3, 39);
            this.a90Label.Name = "a90Label";
            this.a90Label.Size = new System.Drawing.Size(153, 13);
            this.a90Label.TabIndex = 2;
            this.a90Label.Text = "a90";
            // 
            // a90Out
            // 
            this.a90Out.AutoSize = true;
            this.a90Out.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(255)))), ((int)(((byte)(205)))));
            this.a90Out.DecimalPlaces = 3;
            this.a90Out.Dock = System.Windows.Forms.DockStyle.Fill;
            this.a90Out.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.a90Out.InterceptArrowKeys = true;
            this.a90Out.Location = new System.Drawing.Point(3, 55);
            this.a90Out.Maximum = new decimal(new int[] {
            276447232,
            23283,
            0,
            0});
            this.a90Out.Minimum = new decimal(new int[] {
            276447232,
            23283,
            0,
            -2147483648});
            this.a90Out.Name = "a90Out";
            this.a90Out.PartType = POD.ChartPartType.Undefined;
            this.a90Out.ReadOnly = true;
            this.a90Out.Size = new System.Drawing.Size(153, 20);
            this.a90Out.TabIndex = 3;
            this.a90Out.TooltipForNumeric = "";
            this.a90Out.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // a90_95Label
            // 
            this.a90_95Label.AutoSize = true;
            this.a90_95Label.Dock = System.Windows.Forms.DockStyle.Fill;
            this.a90_95Label.Location = new System.Drawing.Point(3, 78);
            this.a90_95Label.Name = "a90_95Label";
            this.a90_95Label.Size = new System.Drawing.Size(153, 13);
            this.a90_95Label.TabIndex = 4;
            this.a90_95Label.Text = "a90/95";
            // 
            // a90_95Out
            // 
            this.a90_95Out.AutoSize = true;
            this.a90_95Out.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(255)))), ((int)(((byte)(235)))));
            this.a90_95Out.DecimalPlaces = 3;
            this.a90_95Out.Dock = System.Windows.Forms.DockStyle.Fill;
            this.a90_95Out.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.a90_95Out.InterceptArrowKeys = true;
            this.a90_95Out.Location = new System.Drawing.Point(3, 94);
            this.a90_95Out.Maximum = new decimal(new int[] {
            276447232,
            23283,
            0,
            0});
            this.a90_95Out.Minimum = new decimal(new int[] {
            276447232,
            23283,
            0,
            -2147483648});
            this.a90_95Out.Name = "a90_95Out";
            this.a90_95Out.PartType = POD.ChartPartType.Undefined;
            this.a90_95Out.ReadOnly = true;
            this.a90_95Out.Size = new System.Drawing.Size(153, 20);
            this.a90_95Out.TabIndex = 5;
            this.a90_95Out.TooltipForNumeric = "";
            this.a90_95Out.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // modelMOut
            // 
            this.modelMOut.AutoSize = true;
            this.modelMOut.DecimalPlaces = 3;
            this.modelMOut.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modelMOut.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.modelMOut.InterceptArrowKeys = false;
            this.modelMOut.Location = new System.Drawing.Point(3, 133);
            this.modelMOut.Maximum = new decimal(new int[] {
            1215752192,
            23,
            0,
            0});
            this.modelMOut.Minimum = new decimal(new int[] {
            1215752192,
            23,
            0,
            -2147483648});
            this.modelMOut.Name = "modelMOut";
            this.modelMOut.PartType = POD.ChartPartType.Undefined;
            this.modelMOut.ReadOnly = true;
            this.modelMOut.Size = new System.Drawing.Size(153, 20);
            this.modelMOut.TabIndex = 11;
            this.modelMOut.TooltipForNumeric = "";
            this.modelMOut.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // modelErrorStdErrOut
            // 
            this.modelErrorStdErrOut.AutoSize = true;
            this.modelErrorStdErrOut.DecimalPlaces = 3;
            this.modelErrorStdErrOut.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.modelErrorStdErrOut.InterceptArrowKeys = false;
            this.modelErrorStdErrOut.Location = new System.Drawing.Point(6, 95);
            this.modelErrorStdErrOut.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.modelErrorStdErrOut.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.modelErrorStdErrOut.Name = "modelErrorStdErrOut";
            this.modelErrorStdErrOut.PartType = POD.ChartPartType.Undefined;
            this.modelErrorStdErrOut.ReadOnly = true;
            this.modelErrorStdErrOut.Size = new System.Drawing.Size(120, 20);
            this.modelErrorStdErrOut.TabIndex = 11;
            this.modelErrorStdErrOut.TooltipForNumeric = "";
            this.modelErrorStdErrOut.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // modelErrorStdErrLabel
            // 
            this.modelErrorStdErrLabel.AutoSize = true;
            this.modelErrorStdErrLabel.Location = new System.Drawing.Point(3, 79);
            this.modelErrorStdErrLabel.Name = "modelErrorStdErrLabel";
            this.modelErrorStdErrLabel.Size = new System.Drawing.Size(73, 13);
            this.modelErrorStdErrLabel.TabIndex = 10;
            this.modelErrorStdErrLabel.Text = "Residual Error";
            // 
            // modelBStdErrOut
            // 
            this.modelBStdErrOut.AutoSize = true;
            this.modelBStdErrOut.DecimalPlaces = 3;
            this.modelBStdErrOut.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.modelBStdErrOut.InterceptArrowKeys = false;
            this.modelBStdErrOut.Location = new System.Drawing.Point(6, 56);
            this.modelBStdErrOut.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.modelBStdErrOut.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.modelBStdErrOut.Name = "modelBStdErrOut";
            this.modelBStdErrOut.PartType = POD.ChartPartType.Undefined;
            this.modelBStdErrOut.ReadOnly = true;
            this.modelBStdErrOut.Size = new System.Drawing.Size(120, 20);
            this.modelBStdErrOut.TabIndex = 9;
            this.modelBStdErrOut.TooltipForNumeric = "";
            this.modelBStdErrOut.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // modelBStdErrLabel
            // 
            this.modelBStdErrLabel.AutoSize = true;
            this.modelBStdErrLabel.Location = new System.Drawing.Point(3, 40);
            this.modelBStdErrLabel.Name = "modelBStdErrLabel";
            this.modelBStdErrLabel.Size = new System.Drawing.Size(49, 13);
            this.modelBStdErrLabel.TabIndex = 8;
            this.modelBStdErrLabel.Text = "Intercept";
            // 
            // modelMStdErrOut
            // 
            this.modelMStdErrOut.AutoSize = true;
            this.modelMStdErrOut.DecimalPlaces = 3;
            this.modelMStdErrOut.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.modelMStdErrOut.InterceptArrowKeys = false;
            this.modelMStdErrOut.Location = new System.Drawing.Point(6, 17);
            this.modelMStdErrOut.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.modelMStdErrOut.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.modelMStdErrOut.Name = "modelMStdErrOut";
            this.modelMStdErrOut.PartType = POD.ChartPartType.Undefined;
            this.modelMStdErrOut.ReadOnly = true;
            this.modelMStdErrOut.Size = new System.Drawing.Size(120, 20);
            this.modelMStdErrOut.TabIndex = 7;
            this.modelMStdErrOut.TooltipForNumeric = "";
            this.modelMStdErrOut.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // modelMStdErrLabel
            // 
            this.modelMStdErrLabel.AutoSize = true;
            this.modelMStdErrLabel.Location = new System.Drawing.Point(3, 1);
            this.modelMStdErrLabel.Name = "modelMStdErrLabel";
            this.modelMStdErrLabel.Size = new System.Drawing.Size(34, 13);
            this.modelMStdErrLabel.TabIndex = 6;
            this.modelMStdErrLabel.Text = "Slope";
            // 
            // normalityTestOut
            // 
            this.normalityTestOut.AutoSize = true;
            this.normalityTestOut.DecimalPlaces = 3;
            this.normalityTestOut.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.normalityTestOut.InterceptArrowKeys = false;
            this.normalityTestOut.Location = new System.Drawing.Point(6, 134);
            this.normalityTestOut.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.normalityTestOut.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.normalityTestOut.Name = "normalityTestOut";
            this.normalityTestOut.Rating = POD.TestRating.Undefined;
            this.normalityTestOut.ReadOnly = true;
            this.normalityTestOut.Size = new System.Drawing.Size(120, 20);
            this.normalityTestOut.TabIndex = 13;
            this.normalityTestOut.TooltipForNumeric = "";
            this.normalityTestOut.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // normalityTestLabel
            // 
            this.normalityTestLabel.AutoSize = true;
            this.normalityTestLabel.Location = new System.Drawing.Point(3, 118);
            this.normalityTestLabel.Name = "normalityTestLabel";
            this.normalityTestLabel.Size = new System.Drawing.Size(50, 13);
            this.normalityTestLabel.TabIndex = 12;
            this.normalityTestLabel.Text = "Normality";
            // 
            // lackOfFitTestLabel
            // 
            this.lackOfFitTestLabel.AutoSize = true;
            this.lackOfFitTestLabel.Location = new System.Drawing.Point(3, 158);
            this.lackOfFitTestLabel.Name = "lackOfFitTestLabel";
            this.lackOfFitTestLabel.Size = new System.Drawing.Size(57, 13);
            this.lackOfFitTestLabel.TabIndex = 12;
            this.lackOfFitTestLabel.Text = "Lack of Fit";
            // 
            // lackOfFitTestOut
            // 
            this.lackOfFitTestOut.AutoSize = true;
            this.lackOfFitTestOut.DecimalPlaces = 3;
            this.lackOfFitTestOut.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.lackOfFitTestOut.InterceptArrowKeys = false;
            this.lackOfFitTestOut.Location = new System.Drawing.Point(6, 174);
            this.lackOfFitTestOut.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.lackOfFitTestOut.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.lackOfFitTestOut.Name = "lackOfFitTestOut";
            this.lackOfFitTestOut.Rating = POD.TestRating.Undefined;
            this.lackOfFitTestOut.ReadOnly = true;
            this.lackOfFitTestOut.Size = new System.Drawing.Size(120, 20);
            this.lackOfFitTestOut.TabIndex = 13;
            this.lackOfFitTestOut.TooltipForNumeric = "";
            this.lackOfFitTestOut.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // equalVarianceTestLabel
            // 
            this.equalVarianceTestLabel.AutoSize = true;
            this.equalVarianceTestLabel.Location = new System.Drawing.Point(3, 198);
            this.equalVarianceTestLabel.Name = "equalVarianceTestLabel";
            this.equalVarianceTestLabel.Size = new System.Drawing.Size(79, 13);
            this.equalVarianceTestLabel.TabIndex = 12;
            this.equalVarianceTestLabel.Text = "Equal Variance";
            // 
            // equalVarianceTestOut
            // 
            this.equalVarianceTestOut.AutoSize = true;
            this.equalVarianceTestOut.DecimalPlaces = 3;
            this.equalVarianceTestOut.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.equalVarianceTestOut.InterceptArrowKeys = false;
            this.equalVarianceTestOut.Location = new System.Drawing.Point(6, 214);
            this.equalVarianceTestOut.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.equalVarianceTestOut.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.equalVarianceTestOut.Name = "equalVarianceTestOut";
            this.equalVarianceTestOut.Rating = POD.TestRating.Undefined;
            this.equalVarianceTestOut.ReadOnly = true;
            this.equalVarianceTestOut.Size = new System.Drawing.Size(120, 20);
            this.equalVarianceTestOut.TabIndex = 13;
            this.equalVarianceTestOut.TooltipForNumeric = "";
            this.equalVarianceTestOut.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // TestColorMap
            // 
            this.TestColorMap.Location = new System.Drawing.Point(4, 238);
            this.TestColorMap.Margin = new System.Windows.Forms.Padding(0);
            this.TestColorMap.Name = "TestColorMap";
            this.TestColorMap.Size = new System.Drawing.Size(140, 20);
            this.TestColorMap.TabIndex = 14;
            // 
            // repeatabilityErrorLabel
            // 
            this.repeatabilityErrorLabel.AutoSize = true;
            this.repeatabilityErrorLabel.Location = new System.Drawing.Point(4, 263);
            this.repeatabilityErrorLabel.Name = "repeatabilityErrorLabel";
            this.repeatabilityErrorLabel.Size = new System.Drawing.Size(67, 13);
            this.repeatabilityErrorLabel.TabIndex = 15;
            this.repeatabilityErrorLabel.Text = "Repeat Error";
            // 
            // repeatabilityErrorOut
            // 
            this.repeatabilityErrorOut.AutoSize = true;
            this.repeatabilityErrorOut.DecimalPlaces = 3;
            this.repeatabilityErrorOut.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.repeatabilityErrorOut.InterceptArrowKeys = false;
            this.repeatabilityErrorOut.Location = new System.Drawing.Point(6, 279);
            this.repeatabilityErrorOut.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.repeatabilityErrorOut.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.repeatabilityErrorOut.Name = "repeatabilityErrorOut";
            this.repeatabilityErrorOut.PartType = POD.ChartPartType.Undefined;
            this.repeatabilityErrorOut.ReadOnly = true;
            this.repeatabilityErrorOut.Size = new System.Drawing.Size(120, 20);
            this.repeatabilityErrorOut.TabIndex = 11;
            this.repeatabilityErrorOut.TooltipForNumeric = "";
            this.repeatabilityErrorOut.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // PodModelParametersHeader
            // 
            this.PodModelParametersHeader.AutoSize = true;
            this.PodModelParametersHeader.BackColor = System.Drawing.SystemColors.Control;
            this.PodModelParametersHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PodModelParametersHeader.Location = new System.Drawing.Point(8, 302);
            this.PodModelParametersHeader.Margin = new System.Windows.Forms.Padding(0);
            this.PodModelParametersHeader.Name = "PodModelParametersHeader";
            this.PodModelParametersHeader.Size = new System.Drawing.Size(171, 16);
            this.PodModelParametersHeader.TabIndex = 12;
            this.PodModelParametersHeader.Text = "POD Model Parameters";
            // 
            // LinearFitEstimatesHeader
            // 
            this.LinearFitEstimatesHeader.AutoSize = true;
            this.LinearFitEstimatesHeader.BackColor = System.Drawing.SystemColors.Control;
            this.LinearFitEstimatesHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LinearFitEstimatesHeader.Location = new System.Drawing.Point(8, 320);
            this.LinearFitEstimatesHeader.Margin = new System.Windows.Forms.Padding(0);
            this.LinearFitEstimatesHeader.Name = "LinearFitEstimatesHeader";
            this.LinearFitEstimatesHeader.Size = new System.Drawing.Size(144, 16);
            this.LinearFitEstimatesHeader.TabIndex = 12;
            this.LinearFitEstimatesHeader.Text = "Linear Fit Estimates";
            // 
            // LinearFitStdErrorHeader
            // 
            this.LinearFitStdErrorHeader.AutoSize = true;
            this.LinearFitStdErrorHeader.BackColor = System.Drawing.SystemColors.Control;
            this.LinearFitStdErrorHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LinearFitStdErrorHeader.Location = new System.Drawing.Point(8, 337);
            this.LinearFitStdErrorHeader.Margin = new System.Windows.Forms.Padding(0);
            this.LinearFitStdErrorHeader.Name = "LinearFitStdErrorHeader";
            this.LinearFitStdErrorHeader.Size = new System.Drawing.Size(141, 16);
            this.LinearFitStdErrorHeader.TabIndex = 16;
            this.LinearFitStdErrorHeader.Text = "Linear Fit Std. Error";
            // 
            // TestOfAssumptionsHeader
            // 
            this.TestOfAssumptionsHeader.AutoSize = true;
            this.TestOfAssumptionsHeader.BackColor = System.Drawing.SystemColors.Control;
            this.TestOfAssumptionsHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TestOfAssumptionsHeader.Location = new System.Drawing.Point(8, 353);
            this.TestOfAssumptionsHeader.Margin = new System.Windows.Forms.Padding(0);
            this.TestOfAssumptionsHeader.Name = "TestOfAssumptionsHeader";
            this.TestOfAssumptionsHeader.Size = new System.Drawing.Size(214, 16);
            this.TestOfAssumptionsHeader.TabIndex = 16;
            this.TestOfAssumptionsHeader.Text = "Assumption Tests (p-value)";
            // 
            // AxisTransformsHeader
            // 
            this.AxisTransformsHeader.AutoSize = true;
            this.AxisTransformsHeader.BackColor = System.Drawing.SystemColors.Control;
            this.AxisTransformsHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AxisTransformsHeader.Location = new System.Drawing.Point(8, 369);
            this.AxisTransformsHeader.Margin = new System.Windows.Forms.Padding(0);
            this.AxisTransformsHeader.Name = "AxisTransformsHeader";
            this.AxisTransformsHeader.Size = new System.Drawing.Size(86, 16);
            this.AxisTransformsHeader.TabIndex = 12;
            this.AxisTransformsHeader.Text = "Transforms";
            // 
            // FlawRangeHeader
            // 
            this.FlawRangeHeader.AutoSize = true;
            this.FlawRangeHeader.BackColor = System.Drawing.SystemColors.Control;
            this.FlawRangeHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FlawRangeHeader.Location = new System.Drawing.Point(9, 386);
            this.FlawRangeHeader.Margin = new System.Windows.Forms.Padding(0);
            this.FlawRangeHeader.Name = "FlawRangeHeader";
            this.FlawRangeHeader.Size = new System.Drawing.Size(90, 16);
            this.FlawRangeHeader.TabIndex = 17;
            this.FlawRangeHeader.Text = "Flaw Range";
            // 
            // ResponseRangeHeader
            // 
            this.ResponseRangeHeader.AutoSize = true;
            this.ResponseRangeHeader.BackColor = System.Drawing.SystemColors.Control;
            this.ResponseRangeHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ResponseRangeHeader.Location = new System.Drawing.Point(11, 404);
            this.ResponseRangeHeader.Margin = new System.Windows.Forms.Padding(0);
            this.ResponseRangeHeader.Name = "ResponseRangeHeader";
            this.ResponseRangeHeader.Size = new System.Drawing.Size(129, 16);
            this.ResponseRangeHeader.TabIndex = 18;
            this.ResponseRangeHeader.Text = "Response Range";
            // 
            // PODDecisionHeader
            // 
            this.PODDecisionHeader.AutoSize = true;
            this.PODDecisionHeader.BackColor = System.Drawing.SystemColors.Control;
            this.PODDecisionHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PODDecisionHeader.Location = new System.Drawing.Point(11, 422);
            this.PODDecisionHeader.Margin = new System.Windows.Forms.Padding(0);
            this.PODDecisionHeader.Name = "PODDecisionHeader";
            this.PODDecisionHeader.Size = new System.Drawing.Size(105, 16);
            this.PODDecisionHeader.TabIndex = 19;
            this.PODDecisionHeader.Text = "POD Decision";
            // 
            // SigmaOut
            // 
            this.SigmaOut.AutoSize = true;
            this.SigmaOut.DecimalPlaces = 3;
            this.SigmaOut.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.SigmaOut.InterceptArrowKeys = false;
            this.SigmaOut.Location = new System.Drawing.Point(9, 456);
            this.SigmaOut.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.SigmaOut.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.SigmaOut.Name = "SigmaOut";
            this.SigmaOut.PartType = POD.ChartPartType.POD;
            this.SigmaOut.ReadOnly = true;
            this.SigmaOut.Size = new System.Drawing.Size(120, 20);
            this.SigmaOut.TabIndex = 21;
            this.SigmaOut.TooltipForNumeric = "";
            this.SigmaOut.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // SigmaLabel
            // 
            this.SigmaLabel.AutoSize = true;
            this.SigmaLabel.Location = new System.Drawing.Point(6, 440);
            this.SigmaLabel.Name = "SigmaLabel";
            this.SigmaLabel.Size = new System.Drawing.Size(36, 13);
            this.SigmaLabel.TabIndex = 20;
            this.SigmaLabel.Text = "Sigma";
            // 
            // MuOut
            // 
            this.MuOut.AutoSize = true;
            this.MuOut.DecimalPlaces = 3;
            this.MuOut.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.MuOut.InterceptArrowKeys = false;
            this.MuOut.Location = new System.Drawing.Point(11, 499);
            this.MuOut.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.MuOut.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.MuOut.Name = "MuOut";
            this.MuOut.PartType = POD.ChartPartType.POD;
            this.MuOut.ReadOnly = true;
            this.MuOut.Size = new System.Drawing.Size(120, 20);
            this.MuOut.TabIndex = 23;
            this.MuOut.TooltipForNumeric = "";
            this.MuOut.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // MuLabel
            // 
            this.MuLabel.AutoSize = true;
            this.MuLabel.Location = new System.Drawing.Point(8, 483);
            this.MuLabel.Name = "MuLabel";
            this.MuLabel.Size = new System.Drawing.Size(22, 13);
            this.MuLabel.TabIndex = 22;
            this.MuLabel.Text = "Mu";
            // 
            // FullRegressionPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "FullRegressionPanel";
            this.graphSplitter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.graphSplitter)).EndInit();
            this.graphSplitter.ResumeLayout(false);
            this.MainChartRightControlsSplitter.Panel1.ResumeLayout(false);
            this.MainChartRightControlsSplitter.Panel2.ResumeLayout(false);
            this.MainChartRightControlsSplitter.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MainChartRightControlsSplitter)).EndInit();
            this.MainChartRightControlsSplitter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.graphSplitterOverlay)).EndInit();
            this.PutControlsHerePanel.ResumeLayout(false);
            this.PutControlsHerePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.linearityChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.normalityChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.equalVarianceChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.podChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.thresholdChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mainChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftCensorControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightCensorControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.aMaxControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.aMinControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.thresholdControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.modelErrorOut)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.modelBOut)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.a50Out)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.a90Out)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.a90_95Out)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.modelMOut)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.modelErrorStdErrOut)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.modelBStdErrOut)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.modelMStdErrOut)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.normalityTestOut)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lackOfFitTestOut)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.equalVarianceTestOut)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repeatabilityErrorOut)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SigmaOut)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MuOut)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.AHatVsARegressionChart mainChart;
        private Controls.CensoredLinearityChart linearityChart;
        private Controls.DataPointChart normalityChart;
        private Controls.DataPointChart equalVarianceChart;
        private Controls.PODChart podChart;
        private Controls.PODThresholdChart thresholdChart;
        private PODChartNumericUpDown aMaxControl;
        private Label AMaxInputLabel;
        private Label AMinInputLabel;
        private Label LeftCensorInputLabel;
        private PODChartNumericUpDown aMinControl;
        private PODChartNumericUpDown leftCensorControl;
        private Label RightCensorInputLabel;
        private PODChartNumericUpDown rightCensorControl;
        private Label a50label;
        private PODChartNumericUpDown a50Out;
        private Label a90Label;
        private PODChartNumericUpDown a90Out;
        private Label a90_95Label;
        private PODChartNumericUpDown a90_95Out;
        private Label thresholdLabel;
        private PODChartNumericUpDown thresholdControl;
        private PODChartNumericUpDown modelBOut;
        private Label modelBLabel;
        private Label modelMLabel;
        private PODChartNumericUpDown modelErrorStdErrOut;
        private Label modelErrorStdErrLabel;
        private PODChartNumericUpDown modelBStdErrOut;
        private Label modelBStdErrLabel;
        private PODChartNumericUpDown modelMStdErrOut;
        private Label modelMStdErrLabel;
        private PODChartNumericUpDown modelMOut;
        private PODChartNumericUpDown modelErrorOut;
        private Label modelErrorLabel;
        private PODRatedNumericUpDown normalityTestOut;
        private Label normalityTestLabel;
        private PODRatedNumericUpDown equalVarianceTestOut;
        private Label equalVarianceTestLabel;
        private PODRatedNumericUpDown lackOfFitTestOut;
        private Label lackOfFitTestLabel;
        private ColorMap TestColorMap;
        private Label repeatabilityErrorLabel;
        private PODChartNumericUpDown repeatabilityErrorOut;
        private Label PodModelParametersHeader;
        private Label TestOfAssumptionsHeader;
        private Label LinearFitStdErrorHeader;
        private Label LinearFitEstimatesHeader;
        private Label ResponseRangeHeader;
        private Label FlawRangeHeader;
        private Label AxisTransformsHeader;
        private Label PODDecisionHeader;
        private PODChartNumericUpDown SigmaOut;
        private Label SigmaLabel;
        private PODChartNumericUpDown MuOut;
        private Label MuLabel;
        //private ColorMap colorMap1;
    }
}
