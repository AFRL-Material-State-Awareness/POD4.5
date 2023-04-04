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

        public override void DeleteSteps()
        {
            foreach (var step in _list)
            {
                if (step is Steps.AHatVsANormalSteps.ChooseTransformStep ||
                    step is Steps.AHatVsANormalSteps.DocumentRemovedStep)
                {
                    step.Dispose();
                }
                else if (step is Steps.AHatVsANormalSteps.FullRegressionStep)
                {
                    // you cannot outright dispose of the entire fullregression step
                    // if you try to and reopen the analysis, the mainchart with still be disposed
                    // when raiseanalysisdone is invoked
                    ((Steps.AHatVsANormalSteps.FullRegressionPanel)step.Panel).DisposeAllExceptMainChart();
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
