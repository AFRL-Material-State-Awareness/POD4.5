using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POD.Controls;
using System.Windows.Forms;

namespace POD.Wizards.Steps.HitMissNormalSteps
{
    public partial class ChooseTransformBar : WizardActionBar
    {
        PODBooleanButton _viewButton;
        PODBooleanButton _modelButton;
        //PODBooleanButton _normalityButton;
        //PODBooleanButton _equalVarianceButton;
        //PODBooleanButton _podButton;
        //PODBooleanButton _thresholdButton;
        //PODBooleanButton _hideAllButton;
        //PODButton _boxcoxButton;

        //private PODBooleanButton _snapToGridButton;

        public override bool SendKeys(Keys keyData)
        {
            var result = base.SendKeys(keyData);

            if (result)
                return true;

            //if (keyData == (Keys.Control | Keys.T))
            //{
            //    _viewButton.PerformClick();
            //    return true;
            //}

            if (keyData == (Keys.Control | Keys.M))
            {
                _modelButton.PerformClick();
                return true;
            }
            
            return false;
        }

        public ChooseTransformBar(PODToolTip tooltip) : base(tooltip)
        {
            InitializeComponent();

            StepToolTip = new PODToolTip();

            //_viewButton = new PODBooleanButton("Show Residuals", "Show Fits", false, "Show fit of data or residuals of the fit. (Ctrl + T)", StepToolTip);
            //AddLeftButton(_viewButton, View_Click);

            _modelButton = new PODBooleanButton("Overlay Models", "Separate Models", false, "Overlay model results for easier comparison. (Ctrl + M)", StepToolTip);
            AddLeftButton(_modelButton, Model_Click);

            prevButton.Enabled = false;

            //_normalityButton = new PODBooleanButton("Show Normality", "Hide Normality", false);
            ////AddLeftButton(_normalityButton, Normality_Click);

            //_equalVarianceButton = new PODBooleanButton("Show Equal Variance", "Hide Equal Variance", false);
            ////AddLeftButton(_equalVarianceButton, EqualVariance_Click);

            //_podButton = new PODBooleanButton("Show POD", "Hide POD", true);
            //AddLeftButton(_podButton, Pod_Click);

            //_thresholdButton = new PODBooleanButton("Show Threshold", "Hide Threshold", true);
            //AddLeftButton(_thresholdButton, Threshold_Click);

            //_hideAllButton = new PODBooleanButton("Show All", "Hide All", true);
            //AddLeftButton(_hideAllButton, HideAll_Click);

            //_boxcoxButton = new PODButton("Apply Box Cox");
            ////AddLeftButton(_boxcoxButton, BoxCox_Click);

            //_snapToGridButton = new PODBooleanButton("Freeform", "Snap to Grid", false);
            ////this.AddLeftButton(_snapToGridButton, SnapGrid_Click);

            //_normalityButton.Enabled = false;
            //_equalVarianceButton.Enabled = false;
            ////_hideAllButton.Enabled = false;
            //_boxcoxButton.Enabled = false;
            //_snapToGridButton.Enabled = false;
            AddSolveAllModelsButton();
            AddIconsToButtons();

            AddEventToRightButton(nextButton, nextButton_Click);
            //AddEventToRightButton(finishButton, finishButton_Click);
        }

        private void View_Click(object sender, EventArgs e)
        {
            _viewButton.ButtonState = MyPanel.ChangeView();
        }

        private void Model_Click(object sender, EventArgs e)
        {
            _modelButton.ButtonState = MyPanel.ChangeModelCompare();
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            MyPanel.UpdateAnalysis();
        }
        /*
        private void finishButton_Click(object sender, EventArgs e)
        {
            Analysis.RunAnalysis();
            if (!MyPanel.Stuck)
            {
                if (Source.FinishArg == null)
                {
                    Source.FinishArg = new AnalysisFinishArgs();
                }
            }
        }
        */

        //private void SnapGrid_Click(object sender, EventArgs e)
        //{
        //    //throw new NotImplementedException();
        //}

        //private void BoxCox_Click(object sender, EventArgs e)
        //{
        //    Analysis.InResponseMax -= 10;
        //    Analysis.InResponseMin += 10;

        //    MyPanel.RunAnalysis();            
        //}

        //private void HideAll_Click(object sender, EventArgs e)
        //{
        //    UpdateChartAndButton(MyPanel.LinearityIndex, _linearityButton);
        //    UpdateChartAndButton(MyPanel.PodIndex, _podButton);
        //    UpdateChartAndButton(MyPanel.ThresholdIndex, _thresholdButton);

        //    _hideAllButton.ButtonState = !_hideAllButton.ButtonState;
        //}

        //private void Threshold_Click(object sender, EventArgs e)
        //{
        //    UpdateChartAndButton(MyPanel.ThresholdIndex, _thresholdButton);
        //}

        //private void Pod_Click(object sender, EventArgs e)
        //{
        //    UpdateChartAndButton(MyPanel.PodIndex, _podButton);
        //}

        //private void EqualVariance_Click(object sender, EventArgs e)
        //{
        //    UpdateChartAndButton(MyPanel.EqualVarianceIndex, _equalVarianceButton);
        //}

        //private void Normality_Click(object sender, EventArgs e)
        //{
        //    UpdateChartAndButton(MyPanel.NormalityIndex, _normalityButton);
        //}

        //private void Linear_Click(object sender, EventArgs e)
        //{
        //    UpdateChartAndButton(MyPanel.LinearityIndex, _linearityButton);
        //}

        //private void CheckSideCharts()
        //{
        //    MyPanel.CheckSideCharts();
        //}

        //private void UpdateChartAndButton(int myIndex, PODBooleanButton myButton)
        //{
        //    myButton.ButtonState = MyPanel.DisplayChart(myIndex);
        //}

    }
}
