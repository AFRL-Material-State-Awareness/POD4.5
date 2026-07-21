// <copyright file="CreateAnalysesBar.cs" company="The University of Dayton">
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
using POD.Analyze;
using POD.Controls;

namespace POD.Wizards.Steps.FullAnalysisProjectSteps
{
    public partial class CreateAnalysesBar : WizardActionBar
    {
        PODButton UndoDeletes;

        public override bool SendKeys(Keys keyData)
        {
            var result = base.SendKeys(keyData);

            if (result)
                return true;

            if (keyData == (Keys.Control | Keys.R))
            {
                UndoDeletes.PerformClick();
                return true;
            }

            return false;
        }

        public CreateAnalysesBar(PODToolTip tooltip) : base(tooltip)
        {
            InitializeComponent();

            StepToolTip = new PODToolTip();

            nextButton.Enabled = false;

            UndoDeletes = new PODButton("Restore Analysis", "Restore a deleted analysis. (Ctrl + R)", StepToolTip);

            AddLeftButton(UndoDeletes, UndoDeletes_Click);

            //want this to fire first before any other functions
            AddEventToRightButton(finishButton, finishButton_Click);

            AddIconsToButtons();
            //AddEventToRightButton(prevButton, PrevButton_Click);
        }

        private void UndoDeletes_Click(object sender, EventArgs e)
        {
            MyPanel.UndoDeletedAnalysis();
        }


        void finishButton_Click(object sender, EventArgs e)
        {
            //call panel function to create analysis for each item in the analysis tree
            MyPanel.CheckStuck();

            if (!MyPanel.Stuck)
            {
                AnalysisList analyses = MyPanel.ExistingOrNewAnalysisList;
                AnalysisList removed = MyPanel.RemovedAnalysisList;

                if (Source.FinishArg == null)
                {
                    Source.FinishArg = new ProjectFinishArgs();
                }

                ProjectFinishArgs args = (ProjectFinishArgs)Source.FinishArg;

                args.UpdateAnalyses(analyses, removed);
            }

        }

        public override bool ProcessKeyboardShortCuts(ref Message msg, Keys keyData)
        {
            //if (keyData == ShortCutKey(Keys.R))
            //{
            //    MyPanel.UndoDeletedAnalysis();

            //    return true;
            //}

            return base.ProcessKeyboardShortCuts(ref msg, keyData);
        }
    }
}
