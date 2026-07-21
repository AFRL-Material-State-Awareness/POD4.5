// <copyright file="FullRegressionBar.cs" company="The University of Dayton">
// Copyright © 2025 The University of Dayton. All rights reserved.
//
// Licensed under the GNU General Public License, Version 3.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.gnu.org/licenses/gpl-3.0.txt
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

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
        PODBooleanButton _qqButton;
        PODBooleanButton _hideAllButton;
        PODButton _cycleButton;

        private PODBooleanButton _snapToGridButton;
        private bool _isRBusy = false;

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
                _qqButton.PerformClick();
                return true;
            }
            if (keyData == (Keys.Control | Keys.D4))
            {
                _podButton.PerformClick();
                return true;
            }
            if (keyData == (Keys.Control | Keys.D5))
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

            _qqButton = new PODBooleanButton("Show QQ", "Hide QQ", true, "Show or hide QQ side chart. (Ctrl + 3)", StepToolTip);
            AddLeftButton(_qqButton, QQ_Click);

            _podButton = new PODBooleanButton("Show POD Curve", "Hide POD Curve", true, "Show or hide POD side chart. (Ctrl + 4)", StepToolTip);
            AddLeftButton(_podButton, Pod_Click);

            _thresholdButton = new PODBooleanButton("Show Threshold", "Hide Threshold", true, "Show or hide Threshold side chart. (Ctrl + 5)", StepToolTip);
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
            // 1. Immediate exit if R is currently thinking.
            // This prevents "stacking" of clicks.
            if (_isRBusy) return;

            try
            {
                _isRBusy = true;

                // 2. Temporarily disable the button to prevent double-clicks
                _cycleButton.Enabled = false;

                // 3. Force the UI to process its current message queue 
                // This clears out any "ghost" clicks before we start the R work.
                Application.DoEvents();

                // 4. Run the R logic (STAYS ON UI THREAD)
                MyPanel.CycleTransforms();
            }
            catch (Exception exp)
            {
                MessageBox.Show($"CycleTransforms: {exp.Message}");
            }
            finally
            {
                // 5. Release the gate
                _isRBusy = false;
                _cycleButton.Enabled = true;
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
            if (_qqButton.ButtonState == _hideAllButton.ButtonState)
                UpdateChartAndButton(MyPanel.QQIndex, _qqButton);

            _hideAllButton.ButtonState = !_hideAllButton.ButtonState;
        }

        private void Threshold_Click(object sender, EventArgs e)
        {
            UpdateChartAndButton(MyPanel.ThresholdIndex, _thresholdButton);
        }

        private void QQ_Click(object sender, EventArgs e)
        {
            UpdateChartAndButton(MyPanel.QQIndex, _qqButton);
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
