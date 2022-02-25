using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POD.Wizards.Steps.FullAnalysisProjectSteps;
using System.Windows.Forms;

namespace POD.Wizards
{
    public class FullAnalysisProjectWizard : ProjectWizard
    {
        public FullAnalysisProjectWizard(Project myProject, ref ControlOrganize myOrganize) : base(myProject)
        {
            _source = myProject;
            _howToDisplay = myOrganize;            
        }

        public override void AddSteps()
        {
            StartNewStepList();
            AddStep(new ProjectPropertiesStep(_source));
            AddStep(new PasteDataStep(_source));
            AddStep(new ViewImportStep(_source));
            AddStep(new CreateAnalysesStep(_source));
            CreateWizardProgressStepListNode();

            CurrentStep = FirstStep;
        }
    }
}
