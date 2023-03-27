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
