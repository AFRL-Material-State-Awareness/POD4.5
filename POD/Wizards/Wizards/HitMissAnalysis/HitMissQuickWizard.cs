using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POD.Analyze;
using POD.Wizards.Steps.FullAnalysisProjectSteps;

namespace POD.Wizards
{
    public class HitMissQuickWizard : AnalysisWizard
    {
        public HitMissQuickWizard(POD.Analyze.HitMissAnalysis myAnalysis, ref ControlOrganize myOrganize)
        {
            _source = myAnalysis;
            _howToDisplay = myOrganize;

            StartNewStepList();
            //AddStep(new Steps.HitMissNormalSteps.ChooseTransformStep(myAnalysis));
            AddStep(new Steps.HitMissNormalSteps.QuickRegressionStep(myAnalysis));
            CreateWizardProgressStepListNode();

            CurrentStep = FirstStep;
        }
    }
}
