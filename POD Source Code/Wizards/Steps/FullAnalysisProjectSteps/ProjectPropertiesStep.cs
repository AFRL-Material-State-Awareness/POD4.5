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
    public partial class ProjectPropertiesStep : WizardStep
    {
        public ProjectPropertiesStep(WizardSource myProject) : base(myProject)
        {
            InitializeComponent();

            StepToolTip = new PODToolTip();

            Title = new ProjectPropertiesTitle(StepToolTip);
            Panel = new ProjectPropertiesPanel(StepToolTip);
            ActionBar = new ProjectPropertiesBar(StepToolTip);
            _overlay = new ProjectPropertiesTutorial();
            Source = myProject;

            //InitializeStep();
        }
    }
}
