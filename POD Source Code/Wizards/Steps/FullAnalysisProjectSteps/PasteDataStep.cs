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
    public partial class PasteDataStep : WizardStep
    {
        public PasteDataStep(WizardSource myProject) : base(myProject)
        {
            InitializeComponent();

            StepToolTip = new PODToolTip();

            Title = new PasteDataTitle(StepToolTip);
            Panel = new PasteDataPanel(StepToolTip);
            ActionBar = new PasteDataBar(StepToolTip);
            _overlay = new PasteDataTutorial();

            Source = myProject;

            //InitializeStep();
        }
    }
}
