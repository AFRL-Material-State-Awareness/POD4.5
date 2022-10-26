using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POD.Wizards
{
    public class HitMissSolveAllWizard : Wizard
    {
        public HitMissSolveAllWizard(POD.Analyze.RunAllAnalysis myAnalysis, ref ControlOrganize myOrganize)
        {
            _source = myAnalysis;
            _howToDisplay = myOrganize;

            StartNewStepList();
            //AddStep(new Steps.HitMissNormalSteps.ChooseTransformStep(myAnalysis));
            AddStep(new Steps.HitMissNormalSteps.RunAllAnalysesStep(myAnalysis));
            CreateWizardProgressStepListNode();

            CurrentStep = FirstStep;
        }
    }
}
