// <copyright file="HitMissNormalWizard.cs" company="The University of Dayton">
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
using System.Windows.Forms;
using POD.Analyze;
using POD.Wizards.Steps.FullAnalysisProjectSteps;

namespace POD.Wizards
{
    public class HitMissNormalWizard : AnalysisWizard
    {
        public HitMissNormalWizard(POD.Analyze.HitMissAnalysis myAnalysis, ref ControlOrganize myOrganize)
        {
            _source = myAnalysis;
            _howToDisplay = myOrganize;

            StartNewStepList();
            AddStep(new Steps.HitMissNormalSteps.ChooseTransformStep(myAnalysis));
            AddStep(new Steps.HitMissNormalSteps.FullRegressionStep(myAnalysis));
            AddStep(new Steps.HitMissNormalSteps.DocumentRemovedStep(myAnalysis));
            CreateWizardProgressStepListNode();

            CurrentStep = FirstStep;
        }
        public override void DeleteSteps()
        {
                foreach (var step in _list)
                {

                    if (step is Steps.HitMissNormalSteps.ChooseTransformStep ||
                        step is Steps.HitMissNormalSteps.DocumentRemovedStep)
                    {
                        step.Dispose();
                    }
                    else if (step is Steps.HitMissNormalSteps.FullRegressionStep)
                    {
                        // you cannot outright dispose of the entire fullregression step
                        // if you try to and reopen the analysis, the mainchart with still be disposed
                        // when raiseanalysisdone is invoked
                        ((Steps.HitMissNormalSteps.FullRegressionPanel)step.Panel).DisposeAllExceptMainChart();
                        ((Steps.HitMissNormalSteps.FullRegressionPanel)step.Panel).WizActionBar.Dispose();
                        step.ActionBar.Dispose();
                        
                        step.Title.Dispose();
                        foreach (Control control in step.Controls)
                        {
                            control.Dispose();
                        }
                        
                    }
                }
        }
    }
}
