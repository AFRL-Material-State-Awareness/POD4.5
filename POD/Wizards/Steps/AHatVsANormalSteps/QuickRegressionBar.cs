using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POD.Controls;
using System.Windows.Forms;

namespace POD.Wizards.Steps.AHatVsANormalSteps
{
    public partial class QuickRegressionBar : WizardActionBar
    {
        PODBooleanButton _linearityButton;
        //PODBooleanButton _normalityButton;
        //PODBooleanButton _equalVarianceButton;
        PODBooleanButton _podButton;
        PODBooleanButton _thresholdButton;
        PODBooleanButton _hideAllButton;
        //PODButton _boxcoxButton;        
        PODButton _cycleButton;
        PODButton _exportButton;
        PODBooleanButton _hideTable;

        //private PODBooleanButton _snapToGridButton;

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
            if (keyData == (Keys.Control | Keys.D3))
            {
                _thresholdButton.PerformClick();
                return true;
            }
            if (keyData == (Keys.Control | Keys.D0))
            {
                _hideAllButton.PerformClick();
                return true;
            }
            if (keyData == (Keys.Control | Keys.T))
            {
                _cycleButton.PerformClick();
                return true;
            }
            if (keyData == (Keys.Control | Keys.OemOpenBrackets))
            {
                _hideTable.PerformClick();
                return true;
            }
            if (keyData == (Keys.Control | Keys.E))
            {
                _exportButton.PerformClick();
                return true;
            }

            return false;
        }

        public QuickRegressionBar(PODToolTip tooltip) : base(tooltip)
        {
            InitializeComponent();

            StepToolTip = new PODToolTip();

            _linearityButton = new PODBooleanButton("Show Residual", "Hide Residual", true, "Show or hide Residual side chart. (Ctrl + 1)", StepToolTip);
            AddLeftButton(_linearityButton, Linear_Click);

            //_normalityButton = new PODBooleanButton("Show Normality", "Hide Normality", false);
            //AddLeftButton(_normalityButton, Normality_Click);
            
            //_equalVarianceButton = new PODBooleanButton("Show Equal Variance", "Hide Equal Variance", false);
            //AddLeftButton(_equalVarianceButton, EqualVariance_Click);

            _podButton = new PODBooleanButton("Show POD Curve", "Hide POD Curve", true, "Show or hide POD side chart. (Ctrl + 2)", StepToolTip);
            AddLeftButton(_podButton, Pod_Click);

            _thresholdButton = new PODBooleanButton("Show Threshold", "Hide Threshold", true, "Show or hide Threshold side chart. (Ctrl + 3)", StepToolTip);
            AddLeftButton(_thresholdButton, Threshold_Click);

            _hideAllButton = new PODBooleanButton("Show All Charts", "Hide All Charts", true, "Show or hide all side charts. (Ctrl + 0)", StepToolTip);
            AddLeftButton(_hideAllButton, HideAll_Click);

            _cycleButton = new PODButton("Cycle Transforms", "Cycle through log and linear combinations. (Ctrl + T)", StepToolTip);
            AddLeftButton(_cycleButton, CycleButton_Click);

            _hideTable = new PODBooleanButton("Show Data Table", "Hide Data Table", true, "Show or hide data input table. (Ctrl + [)", StepToolTip);
            AddLeftButton(_hideTable, HideTable_Click);

            _exportButton = new PODButton("Export to Excel", "Export Input to Excel. (Ctrl + E)", StepToolTip);
            AddLeftButton(_exportButton, Export_Click);
            
            //_boxcoxButton = new PODButton("Apply Box Cox");
            //AddLeftButton(_boxcoxButton, BoxCox_Click);

            //_snapToGridButton = new PODBooleanButton("Freeform", "Snap to Grid", false);
            //this.AddLeftButton(_snapToGridButton, SnapGrid_Click);

            //_normalityButton.Enabled = false;
            //_equalVarianceButton.Enabled = false;
            //_hideAllButton.Enabled = false;
            //_boxcoxButton.Enabled = false;
            //_snapToGridButton.Enabled = false;

            nextButton.Enabled = false;
            finishButton.Enabled = false;
            prevButton.Enabled = false;

            AddIconsToButtons();

            AddEventToRightButton(finishButton, finishButton_Click);

            AddEventToRightButton(prevButton, prevButton_Click);
        }

        private void HideTable_Click(object sender, EventArgs e)
        {
            _hideTable.ButtonState = MyPanel.HideTable(_hideTable.ButtonState);
        }

        private void CycleButton_Click(object sender, EventArgs e)
        {
            MyPanel.CycleTransforms();
        }

        private void Export_Click(object sender, EventArgs e)
        {
            MyPanel.ExportToExcel();
        }

        private void prevButton_Click(object sender, EventArgs e)
        {
            MyPanel.UpdateAnalysis();
        }

        private void finishButton_Click(object sender, EventArgs e)
        {
            if (!MyPanel.Stuck)
            {
                if (Source.FinishArg == null)
                {
                    Source.FinishArg = new AnalysisFinishArgs();
                }
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
            UpdateChartAndButton(MyPanel.LinearityIndex, _linearityButton);
            UpdateChartAndButton(MyPanel.PodIndex, _podButton);
            UpdateChartAndButton(MyPanel.ThresholdIndex, _thresholdButton);

            _hideAllButton.ButtonState = !_hideAllButton.ButtonState;
        }

        private void Threshold_Click(object sender, EventArgs e)
        {
            UpdateChartAndButton(MyPanel.ThresholdIndex, _thresholdButton);
        }

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
