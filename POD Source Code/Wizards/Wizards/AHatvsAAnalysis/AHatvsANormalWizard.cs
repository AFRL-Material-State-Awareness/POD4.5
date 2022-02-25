using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POD.Analyze;
using POD.Wizards.Steps.FullAnalysisProjectSteps;

namespace POD.Wizards
{
    public class AHatvsANormalWizard : AnalysisWizard
    {
        public AHatvsANormalWizard(AHatAnalysis myAnalysis, ref ControlOrganize myOrganize)
        {
            _source = myAnalysis;
            _howToDisplay = myOrganize;
        }

        public override void AddSteps()
        {
            StartNewStepList();

            AddStep(new Steps.AHatVsANormalSteps.ChooseTransformStep(_source));
            AddStep(new Steps.AHatVsANormalSteps.FullRegressionStep(_source));
            AddStep(new Steps.AHatVsANormalSteps.DocumentRemovedStep(_source));

            CreateWizardProgressStepListNode();

            CurrentStep = FirstStep;
        }
    }
}
