using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POD.Analyze;
using POD.Wizards.Steps.FullAnalysisProjectSteps;

namespace POD.Wizards
{
    public class AHatvsAQuickWizard : AnalysisWizard
    {
        public AHatvsAQuickWizard(AHatAnalysis myAnalysis, ref ControlOrganize myOrganize)
        {
            _source = myAnalysis;
            _howToDisplay = myOrganize;

            StartNewStepList();

            //AddStep(new Steps.AHatVsANormalSteps.ChooseTransformStep(myAnalysis));
            AddStep(new Steps.AHatVsANormalSteps.QuickRegressionStep(myAnalysis));


            CreateWizardProgressStepListNode();

            CurrentStep = FirstStep;

        }
    }
}
