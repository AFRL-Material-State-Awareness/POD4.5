using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POD.Wizards.Steps.HitMissNormalSteps
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
            this.linearityChart = new POD.Controls.HitMissFitChart();
            this.normalityChart = new POD.Controls.DataPointChart();
            this.equalVarianceChart = new POD.Controls.DataPointChart();
            this.podChart = new POD.Controls.PODChart();
            this.thresholdChart = new POD.Controls.PODThresholdChart();
            this.mainChart = new POD.Controls.HitMissRegressionChart();
            this.aMaxControl = new POD.Controls.PODChartNumericUpDown();
            this.AMaxInputLabel = new System.Windows.Forms.Label();
            this.aMinControl = new POD.Controls.PODChartNumericUpDown();
            this.AMinInputLabel = new System.Windows.Forms.Label();
            this.covV22Out = new POD.Controls.PODChartNumericUpDown();
            this.CovV22OutLabel = new System.Windows.Forms.Label();
            this.covV11Out = new POD.Controls.PODChartNumericUpDown();
            this.CovV12OutLabel = new System.Windows.Forms.Label();
            this.covV12Out = new POD.Controls.PODChartNumericUpDown();
            this.CovV11OutLabel = new System.Windows.Forms.Label();
            this.SigmaOut = new POD.Controls.PODChartNumericUpDown();
            this.a50label = new System.Windows.Forms.Label();
            this.podSigmaLabel = new System.Windows.Forms.Label();
            this.a50Out = new POD.Controls.PODChartNumericUpDown();
            this.podMuLabel = new System.Windows.Forms.Label();
            this.a90Label = new System.Windows.Forms.Label();
            this.a90Out = new POD.Controls.PODChartNumericUpDown();
            this.a90_95Label = new System.Windows.Forms.Label();
            this.a90_95Out = new POD.Controls.PODChartNumericUpDown();
            this.MuOut = new POD.Controls.PODChartNumericUpDown();
            this.ModelLabel = new System.Windows.Forms.Label();
            this.ModelBox = new POD.Controls.PFModelBox();
            this.TestColorMap = new POD.Controls.ColorMap();
            this.likelihoodRatioTestOut = new POD.Controls.PODRatedNumericUpDown();
            this.lackOfFitTestLabel = new System.Windows.Forms.Label();
            this.FlawRangeHeader = new System.Windows.Forms.Label();
            this.TestOfAssumptionsHeader = new System.Windows.Forms.Label();
            this.CovarianceHeader = new System.Windows.Forms.Label();
            this.AxisTransformsHeader = new System.Windows.Forms.Label();
            this.PodModelParametersHeader = new System.Windows.Forms.Label();
            this.PODModelTypeHeader = new System.Windows.Forms.Label();
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
            ((System.ComponentModel.ISupportInitialize)(this.aMaxControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.aMinControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.covV22Out)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.covV11Out)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.covV12Out)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SigmaOut)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.a50Out)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.a90Out)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.a90_95Out)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MuOut)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.likelihoodRatioTestOut)).BeginInit();
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
            // PutControlsHerePanel
            // 
            this.PutControlsHerePanel.AutoSize = false;
            this.PutControlsHerePanel.Controls.Add(this.PODModelTypeHeader);
            this.PutControlsHerePanel.Controls.Add(this.FlawRangeHeader);
            this.PutControlsHerePanel.Controls.Add(this.TestOfAssumptionsHeader);
            this.PutControlsHerePanel.Controls.Add(this.CovarianceHeader);
            this.PutControlsHerePanel.Controls.Add(this.AxisTransformsHeader);
            this.PutControlsHerePanel.Controls.Add(this.PodModelParametersHeader);
            this.PutControlsHerePanel.Controls.Add(this.TestColorMap);
            this.PutControlsHerePanel.Controls.Add(this.likelihoodRatioTestOut);
            this.PutControlsHerePanel.Controls.Add(this.lackOfFitTestLabel);
            this.PutControlsHerePanel.Controls.Add(this.ModelBox);
            this.PutControlsHerePanel.Controls.Add(this.ModelLabel);
            this.PutControlsHerePanel.Size = new System.Drawing.Size(199, 289);
            // 
            // linearityChart
            // 
            this.linearityChart.BorderlineColor = System.Drawing.SystemColors.ControlDark;
            this.linearityChart.CanUnselect = false;
            this.linearityChart.ChartTitle = "";
            this.linearityChart.IsSelected = false;
            this.linearityChart.IsSquare = false;
            this.linearityChart.Location = new System.Drawing.Point(3, 3);
            this.linearityChart.Name = "linearityChart";
            this.linearityChart.Selectable = false;
            this.linearityChart.ShowChartTitle = true;
            this.linearityChart.SingleSeriesCount = 1;
            this.linearityChart.Size = new System.Drawing.Size(160, 160);
            this.linearityChart.TabIndex = 0;
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
            this.normalityChart.IsSelected = false;
            this.normalityChart.IsSquare = false;
            this.normalityChart.Location = new System.Drawing.Point(3, 169);
            this.normalityChart.Name = "normalityChart";
            this.normalityChart.Selectable = false;
            this.normalityChart.ShowChartTitle = true;
            this.normalityChart.SingleSeriesCount = 1;
            this.normalityChart.Size = new System.Drawing.Size(160, 160);
            this.normalityChart.TabIndex = 0;
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
            this.equalVarianceChart.IsSelected = false;
            this.equalVarianceChart.IsSquare = false;
            this.equalVarianceChart.Location = new System.Drawing.Point(3, 335);
            this.equalVarianceChart.Name = "equalVarianceChart";
            this.equalVarianceChart.Selectable = false;
            this.equalVarianceChart.ShowChartTitle = true;
            this.equalVarianceChart.SingleSeriesCount = 1;
            this.equalVarianceChart.Size = new System.Drawing.Size(160, 160);
            this.equalVarianceChart.TabIndex = 0;
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
            this.podChart.IsSelected = false;
            this.podChart.IsSquare = false;
            this.podChart.Location = new System.Drawing.Point(3, 501);
            this.podChart.Name = "podChart";
            this.podChart.Selectable = false;
            this.podChart.ShowChartTitle = true;
            this.podChart.SingleSeriesCount = 1;
            this.podChart.Size = new System.Drawing.Size(160, 160);
            this.podChart.TabIndex = 1;
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
            this.thresholdChart.IsSelected = false;
            this.thresholdChart.IsSquare = false;
            this.thresholdChart.Location = new System.Drawing.Point(3, 667);
            this.thresholdChart.Name = "thresholdChart";
            this.thresholdChart.Selectable = false;
            this.thresholdChart.ShowChartTitle = true;
            this.thresholdChart.SingleSeriesCount = 1;
            this.thresholdChart.Size = new System.Drawing.Size(160, 160);
            this.thresholdChart.TabIndex = 2;
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
            this.mainChart.Text = "aHatVsAChart1";
            this.mainChart.XAxisTitle = "";
            this.mainChart.XAxisUnit = "";
            this.mainChart.YAxisTitle = "";
            this.mainChart.YAxisUnit = "";
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
            // covV22Out
            // 
            this.covV22Out.AutoSize = true;
            this.covV22Out.DecimalPlaces = 6;
            this.covV22Out.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.covV22Out.InterceptArrowKeys = false;
            this.covV22Out.Location = new System.Drawing.Point(3, 289);
            this.covV22Out.Maximum = new decimal(new int[] {
            1215752192,
            23,
            0,
            0});
            this.covV22Out.Minimum = new decimal(new int[] {
            1215752192,
            23,
            0,
            -2147483648});
            this.covV22Out.Name = "covV22Out";
            this.covV22Out.PartType = POD.ChartPartType.Undefined;
            this.covV22Out.ReadOnly = true;
            this.covV22Out.Size = new System.Drawing.Size(153, 20);
            this.covV22Out.TabIndex = 21;
            this.covV22Out.TooltipForNumeric = "";
            this.covV22Out.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // CovV22OutLabel
            // 
            this.CovV22OutLabel.AutoSize = true;
            this.CovV22OutLabel.Location = new System.Drawing.Point(3, 273);
            this.CovV22OutLabel.Name = "CovV22OutLabel";
            this.CovV22OutLabel.Size = new System.Drawing.Size(83, 13);
            this.CovV22OutLabel.TabIndex = 20;
            this.CovV22OutLabel.Text = "V22";
            // 
            // covV11Out
            // 
            this.covV11Out.AutoSize = true;
            this.covV11Out.DecimalPlaces = 6;
            this.covV11Out.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.covV11Out.InterceptArrowKeys = false;
            this.covV11Out.Location = new System.Drawing.Point(3, 211);
            this.covV11Out.Maximum = new decimal(new int[] {
            1215752192,
            23,
            0,
            0});
            this.covV11Out.Minimum = new decimal(new int[] {
            1215752192,
            23,
            0,
            -2147483648});
            this.covV11Out.Name = "covV11Out";
            this.covV11Out.PartType = POD.ChartPartType.Undefined;
            this.covV11Out.ReadOnly = true;
            this.covV11Out.Size = new System.Drawing.Size(153, 20);
            this.covV11Out.TabIndex = 19;
            this.covV11Out.TooltipForNumeric = "";
            this.covV11Out.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // CovV12OutLabel
            // 
            this.CovV12OutLabel.AutoSize = true;
            this.CovV12OutLabel.Location = new System.Drawing.Point(3, 234);
            this.CovV12OutLabel.Name = "CovV12OutLabel";
            this.CovV12OutLabel.Size = new System.Drawing.Size(83, 13);
            this.CovV12OutLabel.TabIndex = 18;
            this.CovV12OutLabel.Text = "V12";
            // 
            // covV12Out
            // 
            this.covV12Out.AutoSize = true;
            this.covV12Out.DecimalPlaces = 6;
            this.covV12Out.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.covV12Out.InterceptArrowKeys = false;
            this.covV12Out.Location = new System.Drawing.Point(3, 250);
            this.covV12Out.Maximum = new decimal(new int[] {
            1215752192,
            23,
            0,
            0});
            this.covV12Out.Minimum = new decimal(new int[] {
            1215752192,
            23,
            0,
            -2147483648});
            this.covV12Out.Name = "covV12Out";
            this.covV12Out.PartType = POD.ChartPartType.Undefined;
            this.covV12Out.ReadOnly = true;
            this.covV12Out.Size = new System.Drawing.Size(153, 20);
            this.covV12Out.TabIndex = 17;
            this.covV12Out.TooltipForNumeric = "";
            this.covV12Out.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // CovV11OutLabel
            // 
            this.CovV11OutLabel.AutoSize = true;
            this.CovV11OutLabel.Location = new System.Drawing.Point(3, 195);
            this.CovV11OutLabel.Name = "CovV11OutLabel";
            this.CovV11OutLabel.Size = new System.Drawing.Size(83, 13);
            this.CovV11OutLabel.TabIndex = 16;
            this.CovV11OutLabel.Text = "V11";
            // 
            // podSigmaOut
            // 
            this.SigmaOut.AutoSize = true;
            this.SigmaOut.DecimalPlaces = 3;
            this.SigmaOut.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SigmaOut.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.SigmaOut.InterceptArrowKeys = false;
            this.SigmaOut.Location = new System.Drawing.Point(3, 172);
            this.SigmaOut.Maximum = new decimal(new int[] {
            1215752192,
            23,
            0,
            0});
            this.SigmaOut.Minimum = new decimal(new int[] {
            1215752192,
            23,
            0,
            -2147483648});
            this.SigmaOut.Name = "podSigmaOut";
            this.SigmaOut.PartType = POD.ChartPartType.Undefined;
            this.SigmaOut.ReadOnly = true;
            this.SigmaOut.Size = new System.Drawing.Size(153, 20);
            this.SigmaOut.TabIndex = 13;
            this.SigmaOut.TooltipForNumeric = "";
            this.SigmaOut.Value = new decimal(new int[] {
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
            // podSigmaLabel
            // 
            this.podSigmaLabel.AutoSize = true;
            this.podSigmaLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.podSigmaLabel.Location = new System.Drawing.Point(3, 156);
            this.podSigmaLabel.Name = "podSigmaLabel";
            this.podSigmaLabel.Size = new System.Drawing.Size(153, 13);
            this.podSigmaLabel.TabIndex = 12;
            this.podSigmaLabel.Text = "Sigma";
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
            276447232,
            23283,
            0,
            0});
            this.a50Out.Minimum = new decimal(new int[] {
            276447232,
            23283,
            0,
            -2147483648});
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
            // podMuLabel
            // 
            this.podMuLabel.AutoSize = true;
            this.podMuLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.podMuLabel.Location = new System.Drawing.Point(3, 117);
            this.podMuLabel.Name = "podMuLabel";
            this.podMuLabel.Size = new System.Drawing.Size(153, 13);
            this.podMuLabel.TabIndex = 10;
            this.podMuLabel.Text = "Mu";
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
            // podMuOut
            // 
            this.MuOut.AutoSize = true;
            this.MuOut.DecimalPlaces = 3;
            this.MuOut.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MuOut.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.MuOut.InterceptArrowKeys = false;
            this.MuOut.Location = new System.Drawing.Point(3, 133);
            this.MuOut.Maximum = new decimal(new int[] {
            1215752192,
            23,
            0,
            0});
            this.MuOut.Minimum = new decimal(new int[] {
            1215752192,
            23,
            0,
            -2147483648});
            this.MuOut.Name = "podMuOut";
            this.MuOut.PartType = POD.ChartPartType.Undefined;
            this.MuOut.ReadOnly = true;
            this.MuOut.Size = new System.Drawing.Size(153, 20);
            this.MuOut.TabIndex = 11;
            this.MuOut.TooltipForNumeric = "";
            this.MuOut.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // ModelLabel
            // 
            this.ModelLabel.AutoSize = true;
            this.ModelLabel.Location = new System.Drawing.Point(3, 13);
            this.ModelLabel.Name = "ModelLabel";
            this.ModelLabel.Size = new System.Drawing.Size(36, 13);
            this.ModelLabel.TabIndex = 0;
            this.ModelLabel.Text = "Model";
            // 
            // ModelBox
            // 
            this.ModelBox.FormattingEnabled = true;
            this.ModelBox.Location = new System.Drawing.Point(6, 29);
            this.ModelBox.Name = "ModelBox";
            this.ModelBox.Size = new System.Drawing.Size(121, 21);
            this.ModelBox.TabIndex = 1;
            // 
            // TestColorMap
            // 
            this.TestColorMap.Location = new System.Drawing.Point(6, 92);
            this.TestColorMap.Margin = new System.Windows.Forms.Padding(0);
            this.TestColorMap.Name = "TestColorMap";
            this.TestColorMap.Size = new System.Drawing.Size(140, 20);
            this.TestColorMap.TabIndex = 17;
            // 
            // lackOfFitTestOut
            // 
            this.likelihoodRatioTestOut.AutoSize = true;
            this.likelihoodRatioTestOut.DecimalPlaces = 3;
            this.likelihoodRatioTestOut.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.likelihoodRatioTestOut.InterceptArrowKeys = false;
            this.likelihoodRatioTestOut.Location = new System.Drawing.Point(6, 69);
            this.likelihoodRatioTestOut.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.likelihoodRatioTestOut.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.likelihoodRatioTestOut.Name = "lackOfFitTestOut";
            this.likelihoodRatioTestOut.Rating = POD.TestRating.Undefined;
            this.likelihoodRatioTestOut.ReadOnly = true;
            this.likelihoodRatioTestOut.Size = new System.Drawing.Size(120, 20);
            this.likelihoodRatioTestOut.TabIndex = 16;
            this.likelihoodRatioTestOut.TooltipForNumeric = "";
            this.likelihoodRatioTestOut.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // lackOfFitTestLabel
            // 
            this.lackOfFitTestLabel.AutoSize = true;
            this.lackOfFitTestLabel.Location = new System.Drawing.Point(3, 53);
            this.lackOfFitTestLabel.Name = "lackOfFitTestLabel";
            this.lackOfFitTestLabel.Size = new System.Drawing.Size(83, 13);
            this.lackOfFitTestLabel.TabIndex = 15;
            this.lackOfFitTestLabel.Text = "Likelihood Ratio";
            // 
            // FlawRangeHeader
            // 
            this.FlawRangeHeader.AutoSize = true;
            this.FlawRangeHeader.BackColor = System.Drawing.SystemColors.Control;
            this.FlawRangeHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FlawRangeHeader.Location = new System.Drawing.Point(3, 188);
            this.FlawRangeHeader.Margin = new System.Windows.Forms.Padding(0);
            this.FlawRangeHeader.Name = "FlawRangeHeader";
            this.FlawRangeHeader.Size = new System.Drawing.Size(90, 16);
            this.FlawRangeHeader.TabIndex = 22;
            this.FlawRangeHeader.Text = "Flaw Range";
            // 
            // TestOfAssumptionsHeader
            // 
            this.TestOfAssumptionsHeader.AutoSize = true;
            this.TestOfAssumptionsHeader.BackColor = System.Drawing.SystemColors.Control;
            this.TestOfAssumptionsHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TestOfAssumptionsHeader.Location = new System.Drawing.Point(2, 155);
            this.TestOfAssumptionsHeader.Margin = new System.Windows.Forms.Padding(0);
            this.TestOfAssumptionsHeader.Name = "TestOfAssumptionsHeader";
            this.TestOfAssumptionsHeader.Size = new System.Drawing.Size(148, 16);
            this.TestOfAssumptionsHeader.TabIndex = 21;
            this.TestOfAssumptionsHeader.Text = "Test of Assumptions";
            // 
            // CovarianceHeader
            // 
            this.CovarianceHeader.AutoSize = true;
            this.CovarianceHeader.BackColor = System.Drawing.SystemColors.Control;
            this.CovarianceHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CovarianceHeader.Location = new System.Drawing.Point(3, 139);
            this.CovarianceHeader.Margin = new System.Windows.Forms.Padding(0);
            this.CovarianceHeader.Name = "CovarianceHeader";
            this.CovarianceHeader.Size = new System.Drawing.Size(132, 16);
            this.CovarianceHeader.TabIndex = 18;
            this.CovarianceHeader.Text = "Covariance Matrix";
            // 
            // AxisTransformsHeader
            // 
            this.AxisTransformsHeader.AutoSize = true;
            this.AxisTransformsHeader.BackColor = System.Drawing.SystemColors.Control;
            this.AxisTransformsHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AxisTransformsHeader.Location = new System.Drawing.Point(2, 171);
            this.AxisTransformsHeader.Margin = new System.Windows.Forms.Padding(0);
            this.AxisTransformsHeader.Name = "AxisTransformsHeader";
            this.AxisTransformsHeader.Size = new System.Drawing.Size(86, 16);
            this.AxisTransformsHeader.TabIndex = 19;
            this.AxisTransformsHeader.Text = "Transforms";
            // 
            // PodModelParametersHeader
            // 
            this.PodModelParametersHeader.AutoSize = true;
            this.PodModelParametersHeader.BackColor = System.Drawing.SystemColors.Control;
            this.PodModelParametersHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PodModelParametersHeader.Location = new System.Drawing.Point(3, 121);
            this.PodModelParametersHeader.Margin = new System.Windows.Forms.Padding(0);
            this.PodModelParametersHeader.Name = "PodModelParametersHeader";
            this.PodModelParametersHeader.Size = new System.Drawing.Size(171, 16);
            this.PodModelParametersHeader.TabIndex = 20;
            this.PodModelParametersHeader.Text = "POD Model Parameters";
            // 
            // PODModelTypeHeader
            // 
            this.PODModelTypeHeader.AutoSize = true;
            this.PODModelTypeHeader.BackColor = System.Drawing.SystemColors.Control;
            this.PODModelTypeHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PODModelTypeHeader.Location = new System.Drawing.Point(3, 204);
            this.PODModelTypeHeader.Margin = new System.Windows.Forms.Padding(0);
            this.PODModelTypeHeader.Name = "PODModelTypeHeader";
            this.PODModelTypeHeader.Size = new System.Drawing.Size(127, 16);
            this.PODModelTypeHeader.TabIndex = 23;
            this.PODModelTypeHeader.Text = "POD Model Type";
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
            ((System.ComponentModel.ISupportInitialize)(this.aMaxControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.aMinControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.covV22Out)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.covV11Out)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.covV12Out)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SigmaOut)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.a50Out)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.a90Out)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.a90_95Out)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MuOut)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.likelihoodRatioTestOut)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.HitMissRegressionChart mainChart;
        private Controls.HitMissFitChart linearityChart;
        private Controls.DataPointChart normalityChart;
        private Controls.DataPointChart equalVarianceChart;
        private Controls.PODChart podChart;
        private Controls.PODThresholdChart thresholdChart;
        private POD.Controls.PODChartNumericUpDown aMaxControl;
        private Label AMaxInputLabel;
        private Label AMinInputLabel;
        private POD.Controls.PODChartNumericUpDown aMinControl;
        private Label a50label;
        private POD.Controls.PODChartNumericUpDown a50Out;
        private Label a90Label;
        private POD.Controls.PODChartNumericUpDown a90Out;
        private Label a90_95Label;
        private POD.Controls.PODChartNumericUpDown a90_95Out;
        private POD.Controls.PODChartNumericUpDown SigmaOut;
        private Label podSigmaLabel;
        private Label podMuLabel;
        private POD.Controls.PODChartNumericUpDown MuOut;
        private POD.Controls.PODChartNumericUpDown covV22Out;
        private Label CovV22OutLabel;
        private POD.Controls.PODChartNumericUpDown covV11Out;
        private Label CovV12OutLabel;
        private POD.Controls.PODChartNumericUpDown covV12Out;
        private Label CovV11OutLabel;
        private Controls.PFModelBox ModelBox;
        //private Controls.ConfidenceBox ConfIntBox;
        private Label ModelLabel;
        private Controls.ColorMap TestColorMap;
        private Controls.PODRatedNumericUpDown likelihoodRatioTestOut;
        private Label lackOfFitTestLabel;
        private Label FlawRangeHeader;
        private Label TestOfAssumptionsHeader;
        private Label CovarianceHeader;
        private Label AxisTransformsHeader;
        private Label PodModelParametersHeader;
        private Label PODModelTypeHeader;
    }
}
