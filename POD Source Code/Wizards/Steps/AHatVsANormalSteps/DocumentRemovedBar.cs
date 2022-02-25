using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POD.Controls;
using System.Windows.Forms;

namespace POD.Wizards.Steps.AHatVsANormalSteps
{
    public partial class DocumentRemovedBar : WizardActionBar
    {
        //PODBooleanButton _viewButton;
        //PODBooleanButton _normalityButton;
        //PODBooleanButton _equalVarianceButton;
        //PODBooleanButton _podButton;
        //PODBooleanButton _thresholdButton;
        //PODBooleanButton _hideAllButton;
        //PODButton _boxcoxButton;
        PODButton _exportButton;
        PODButton _exportProjectButton;

        //private PODBooleanButton _snapToGridButton;

        public override bool SendKeys(Keys keyData)
        {
            var result = base.SendKeys(keyData);

            if (result)
                return true;

            if (keyData == (Keys.Control | Keys.Shift | Keys.E))
            {
                _exportProjectButton.PerformClick();
                return true;
            }
            if (keyData == (Keys.Control | Keys.E))
            {
                _exportButton.PerformClick();
                return true;
            }

            return false;
        }

        public DocumentRemovedBar(PODToolTip tooltip)
            : base(tooltip)
        {
            InitializeComponent();

            StepToolTip = new PODToolTip();

            //_viewButton = new PODBooleanButton("&Show Residuals", "&Show Fits", false, "Show fit of data or residuals of the fit. (Alt + S)", PODStepToolTip);
            //AddLeftButton(_viewButton, View_Click);

            //prevButton.Enabled = false;
            nextButton.Enabled = false;

            _exportButton = new PODButton("Export Analysis", "Export Analysis to Excel. (Ctrl + E)", StepToolTip);
            AddLeftButton(_exportButton, Export_Click);

            _exportProjectButton = new PODButton("Export Pr&oject", "Export Project to Excel. (Ctrl + Shift + E)", StepToolTip);
            AddLeftButton(_exportProjectButton, ExportProject_Click);

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

            AddIconsToButtons();

            //AddEventToRightButton(nextButton, nextButton_Click);

            //want this to fire first before any other functions
            AddEventToRightButton(finishButton, finishButton_Click);
        }

        private void ExportProject_Click(object sender, EventArgs e)
        {
            MyPanel.ExportProjectToExcel();
        }

        private void Export_Click(object sender, EventArgs e)
        {
            MyPanel.ExportToExcel();
        }

        private void finishButton_Click(object sender, EventArgs e)
        {
            MyPanel.CheckStuck();

            MyPanel.UpdateMRUList();

            if (!MyPanel.Stuck)
            {
                if (Source.FinishArg == null)
                {
                    Source.FinishArg = new AnalysisFinishArgs();
                }
            }
        }

        private void View_Click(object sender, EventArgs e)
        {
            //_viewButton.ButtonState = MyPanel.ChangeView();
        }

        //private void nextButton_Click(object sender, EventArgs e)
        //{
            //MyPanel.UpdateAnalysis();
        //}

        //private void finishButton_Click(object sender, EventArgs e)
        //{
        //    if (!MyPanel.Stuck)
        //    {
        //        if (Source.FinishArg == null)
        //        {
        //            Source.FinishArg = new AnalysisFinishArgs();
        //        }
        //    }
        //}

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
