// <copyright file="ProjectPropertiesBar.cs" company="The University of Dayton">
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

namespace POD.Wizards.Steps.FullAnalysisProjectSteps
{
    public partial class ProjectPropertiesBar : WizardActionBar
    {
        PODButton _useLastButton;
        PODBooleanButton _showProperties;

        public override bool SendKeys(Keys keyData)
        {
            var result = base.SendKeys(keyData);

            if (result)
                return true;

            if (keyData == (Keys.Control | Keys.L))
            {
                _useLastButton.PerformClick();
                return true;
            }

            if (keyData == (Keys.Control | Keys.OemOpenBrackets))
            {
                _showProperties.PerformClick();
                return true;
            }

            return false;
        }

        public ProjectPropertiesBar(PODToolTip tooltip) : base(tooltip)
        {
            InitializeComponent();

            StepToolTip = new PODToolTip();

            _useLastButton = new PODButton("Use Last", "Auto-fill with last values that were used. (Ctrl + L)", StepToolTip);
            _showProperties = new PODBooleanButton("Show Properties", "Hide Properties", true, "Show/hide Project Properties. (Ctrl + [)", StepToolTip);

            prevButton.Enabled = false;

            this.AddLeftButton(_useLastButton, UseLast_Click);
            this.AddLeftButton(_showProperties, ShowProperties_Click);

            //make sure data is copied to source objects
            //when next button is pressed
            AddEventToRightButton(nextButton, NextButton_Click);

            AddIconsToButtons();
        }

        private void ShowProperties_Click(object sender, EventArgs e)
        {
            _showProperties.ButtonState = MyPanel.HideProperties(_showProperties.ButtonState);
        }

        private void UseLast_Click(object sender, EventArgs e)
        {
            MyPanel.UseLastValues();
        }

        void NextButton_Click(object sender, EventArgs e)
        {
            MyPanel.CopySettingsToProject();
        }

        public override bool ProcessKeyboardShortCuts(ref Message msg, Keys keyData)
        {
            //if (keyData == ShortCutKey(Keys.L))
            //{
            //    MyPanel.UseLastValues();

            //    return true;
            //}

            return base.ProcessKeyboardShortCuts(ref msg, keyData);
        }
    }
}
