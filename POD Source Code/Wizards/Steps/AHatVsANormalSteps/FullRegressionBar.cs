using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POD.Controls;
using System.Windows.Forms;

namespace POD.Wizards.Steps.AHatVsANormalSteps
{
    public partial class FullRegressionBar : WizardActionBar
    {
        PODBooleanButton _linearityButton;
        PODBooleanButton _podButton;
        PODBooleanButton _thresholdButton;
        PODBooleanButton _normalityButton;
        PODBooleanButton _hideAllButton;
        PODButton _cycleButton;

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
                _normalityButton.PerformClick();
                return true;
            }
            if (keyData == (Keys.Control | Keys.D3))
            {
                _podButton.PerformClick();
                return true;
            }
            if (keyData == (Keys.Control | Keys.D4))
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

            return false;
        }

        public FullRegressionBar(PODToolTip tooltip) : base(tooltip)
        {
            InitializeComponent();

            StepToolTip = new PODToolTip();

            

            _linearityButton = new PODBooleanButton("Show Residual", "Hide Residual", true, "Show or hide Residual side chart. (Ctrl + 1)", StepToolTip);
            AddLeftButton(_linearityButton, Linear_Click);

            _normalityButton = new PODBooleanButton("Show N&ormality", "Hide N&ormality", true, "Show Or hide the response Normality chart. (Ctrl + 2)", StepToolTip);
            AddLeftButton(_normalityButton, Normality_Click);
            
            _podButton = new PODBooleanButton("Show POD Curve", "Hide POD Curve", true, "Show or hide POD side chart. (Ctrl + 3)", StepToolTip);
            AddLeftButton(_podButton, Pod_Click);

            _thresholdButton = new PODBooleanButton("Show Threshold", "Hide Threshold", true, "Show or hide Threshold side chart. (Ctrl + 4)", StepToolTip);
            AddLeftButton(_thresholdButton, Threshold_Click);

            _hideAllButton = new PODBooleanButton("Show All Charts", "Hide All Charts", true, "Show or hide all side charts. (Ctrl + 0)", StepToolTip);
            AddLeftButton(_hideAllButton, HideAll_Click);

            _cycleButton = new PODButton("Cycle Transforms", "Cycle through log and linear combinations. (Ctrl + T)", StepToolTip);
            AddLeftButton(_cycleButton, CycleButton_Click);


            AddIconsToButtons();

            //AddEventToRightButton(finishButton, finishButton_Click);

            AddEventToRightButton(prevButton, prevButton_Click);
            AddEventToRightButton(nextButton, nextButton_Click);
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            MyPanel.UpdateAnalysis();
        }

        private void prevButton_Click(object sender, EventArgs e)
        {
            MyPanel.UpdateAnalysis();
        }
        

        

        private void CycleButton_Click(object sender, EventArgs e)
        {
            try
            {
                MyPanel.CycleTransforms();
            }
            catch(Exception exp)
            {
                MessageBox.Show("CycleTransforms: " + exp.Message);
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
            if (_normalityButton.ButtonState == _hideAllButton.ButtonState)
                UpdateChartAndButton(MyPanel.NormalityIndex, _normalityButton);
            if (_podButton.ButtonState == _hideAllButton.ButtonState)
                UpdateChartAndButton(MyPanel.PodIndex, _podButton);
            if (_thresholdButton.ButtonState == _hideAllButton.ButtonState)
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

        private void Normality_Click(object sender, EventArgs e)
        {
            UpdateChartAndButton(MyPanel.NormalityIndex, _normalityButton);
        }

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
