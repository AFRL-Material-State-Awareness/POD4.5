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
    public partial class CreateAnalysesStep : WizardStep, IDisposable
    {
        public CreateAnalysesStep(WizardSource myProject)
        {
            InitializeComponent();

            StepToolTip = new PODToolTip();

            Title = new CreateAnalysesTitle(StepToolTip);
            Panel = new CreateAnalysesPanel(StepToolTip);
            ActionBar = new CreateAnalysesBar(StepToolTip);
            _overlay = new CreateAnalysesTutorial();

            Source = myProject;

            //InitializeStep();
        }
    }
}
