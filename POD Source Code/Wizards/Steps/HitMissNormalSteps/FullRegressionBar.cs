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
        PODBooleanButton _podButton;
        PODBooleanButton _hideAllButton;
        PODButton _cycleButton;
        // Solve all models contained within the project manager with default parameters


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

            _podButton = new PODBooleanButton("Show POD Curve", "Hide POD Curve", true, "Show or hide POD side chart. (Ctrl + 2)", StepToolTip);
            AddLeftButton(_podButton, Pod_Click);

            _hideAllButton = new PODBooleanButton("Show All Charts", "Hide All Charts", true, "Show or hide all side charts. (Ctrl + 0)", StepToolTip);
            AddLeftButton(_hideAllButton, HideAll_Click);

            _cycleButton = new PODButton("Cycle Transforms", "Cycle through log and linear combinations. (Ctrl + T)", StepToolTip);
            AddLeftButton(_cycleButton, CycleButton_Click);

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

        private void CycleButton_Click(object sender, EventArgs e)
        {
            try
            {
                MyPanel.CycleTransforms();
            }
            catch (Exception exp)
            {
                MessageBox.Show($"CycleTransforms: {exp.Message}");
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
