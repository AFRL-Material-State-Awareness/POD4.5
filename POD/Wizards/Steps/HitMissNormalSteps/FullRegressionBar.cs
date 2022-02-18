using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POD.Controls;

namespace POD.Wizards.Steps.HitMissNormalSteps
{
    public partial class FullRegressionBar : WizardActionBar
    {
        PODBooleanButton _linearityButton;
        //PODBooleanButton _normalityButton;
        //PODBooleanButton _equalVarianceButton;
        PODBooleanButton _podButton;
        //PODBooleanButton _thresholdButton;
        PODBooleanButton _hideAllButton;
        //PODButton _boxcoxButton;

        private PODBooleanButton _snapToGridButton;

        public override bool SendKeys(Keys keyData)
        {
            var result = base.SendKeys(keyData);

            if (result)
                return true;

            result = ScrollToChart(keyData);

            if (result)
                return true;

            result = ResizeCharts(keyData);

            if (result)
                return true;

            

            if (keyData == (Keys.Control | Keys.D1))
            {
                _linearityButton.PerformClick();
                return true;
            }
            if (keyData == (Keys.Control | Keys.D2))
            {
                _podButton.PerformClick();
                return true;
            }            
            if (keyData == (Keys.Control | Keys.D0))
            {
                _hideAllButton.PerformClick();
                return true;
            }

            return false;
        }

        

        public FullRegressionBar(PODToolTip tooltip) : base(tooltip)
        {
            InitializeComponent();

            StepToolTip = new PODToolTip();

            

            _linearityButton = new PODBooleanButton("Show Model Fit", "Hide Model Fit", true, "Show or hide Model Fit side chart. (Ctrl + 1)", StepToolTip);
            AddLeftButton(_linearityButton, Linear_Click);

            //_normalityButton = new PODBooleanButton("Show Normality", "Hide Normality", false);
            //AddLeftButton(_normalityButton, Normality_Click);

            //_equalVarianceButton = new PODBooleanButton("Show Equal Variance", "Hide Equal Variance", false);
            //AddLeftButton(_equalVarianceButton, EqualVariance_Click);

            _podButton = new PODBooleanButton("Show POD Curve", "Hide POD Curve", true, "Show or hide POD side chart. (Ctrl + 2)", StepToolTip);
            AddLeftButton(_podButton, Pod_Click);

            _hideAllButton = new PODBooleanButton("Show All Charts", "Hide All Charts", true, "Show or hide all side charts. (Ctrl + 0)", StepToolTip);
            AddLeftButton(_hideAllButton, HideAll_Click);

            //_thresholdButton = new PODBooleanButton("Show Threshold", "Hide Threshold", false);
            //AddLeftButton(_thresholdButton, Threshold_Click);

            

            //_boxcoxButton = new PODButton("Apply Box Cox");
            //AddLeftButton(_boxcoxButton, BoxCox_Click);

            //_snapToGridButton = new PODBooleanButton("Freeform", "Snap to Grid", false);
            //this.AddLeftButton(_snapToGridButton, SnapGrid_Click);

            //_normalityButton.Enabled = false;
            //_equalVarianceButton.Enabled = false;
            //_hideAllButton.Enabled = false;
            //_boxcoxButton.Enabled = false;
            //_snapToGridButton.Enabled = false;

            AddIconsToButtons();

            AddEventToRightButton(finishButton, finishButton_Click);
        }

        private void finishButton_Click(object sender, EventArgs e)
        {
            if (!MyPanel.Stuck)
            {
                if (Source.FinishArg == null)
                {
                    Source.FinishArg = new AnalysisFinishArgs();
                }

                MyPanel.UpdateAnalysis();
            }
        }

        private void SnapGrid_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void BoxCox_Click(object sender, EventArgs e)
        {
            Analysis.InResponseMax -= 10;
            Analysis.InResponseMin += 10;

            MyPanel.RunAnalysis();
        }

        private void HideAll_Click(object sender, EventArgs e)
        {
            if (_linearityButton.ButtonState == _hideAllButton.ButtonState) 
                UpdateChartAndButton(MyPanel.LinearityIndex, _linearityButton);
            if (_podButton.ButtonState == _hideAllButton.ButtonState) 
                UpdateChartAndButton(MyPanel.PodIndex, _podButton);

            _hideAllButton.ButtonState = !_hideAllButton.ButtonState;
        }

        //private void Threshold_Click(object sender, EventArgs e)
        //{
        //    UpdateChartAndButton(MyPanel.ThresholdIndex, _thresholdButton);
        //}

        private void Pod_Click(object sender, EventArgs e)
        {
            UpdateChartAndButton(MyPanel.PodIndex, _podButton);
        }

        //private void EqualVariance_Click(object sender, EventArgs e)
        //{
        //    UpdateChartAndButton(MyPanel.EqualVarianceIndex, _equalVarianceButton);
        //}

        //private void Normality_Click(object sender, EventArgs e)
        //{
        //    UpdateChartAndButton(MyPanel.NormalityIndex, _normalityButton);
        //}

        private void Linear_Click(object sender, EventArgs e)
        {
            UpdateChartAndButton(MyPanel.LinearityIndex, _linearityButton);
        }

        private void CheckSideCharts()
        {
            MyPanel.CheckSideCharts();
        }

        private void UpdateChartAndButton(int myIndex, PODBooleanButton myButton)
        {
            myButton.ButtonState = MyPanel.DisplayChart(myIndex);


        }
    }
}
