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
    public partial class ViewImportStep : WizardStep
    {
        public ViewImportStep(WizardSource myProject)
        {
            InitializeComponent();

            StepToolTip = new PODToolTip();

            Title = new ViewImportTitle(StepToolTip);
            Panel = new ViewImportPanel(StepToolTip);
            ActionBar = new ViewImportBar(StepToolTip);
            _overlay = new ViewImportTutorial();

            Source = myProject;

            //InitializeStep();
        }
    }
}
