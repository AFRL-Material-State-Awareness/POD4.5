// <copyright file="QuickRegressionStep.cs" company="The University of Dayton">
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
using POD.Analyze;
using POD.Controls;

namespace POD.Wizards.Steps.HitMissNormalSteps
{
    public partial class QuickRegressionStep : WizardStep
    {
        public QuickRegressionStep(WizardSource myAnalysis)
        {
            InitializeComponent();

            StepToolTip = new PODToolTip();

            Title = new FullRegressionTitle(StepToolTip);
            Panel = new QuickRegressionPanel(StepToolTip);
            ActionBar = new QuickRegressionBar(StepToolTip);
            _overlay = new FullRegressionTutorial();

            Source = myAnalysis;

            //((FullRegressionPanel)_panel).SetAnalysisData(Analysis);

            //InitializeStep();
        }
    }
}
