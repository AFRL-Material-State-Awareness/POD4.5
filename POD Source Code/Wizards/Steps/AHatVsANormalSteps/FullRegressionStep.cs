using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POD.Analyze;
using POD.Controls;

namespace POD.Wizards.Steps.AHatVsANormalSteps
{
    public partial class FullRegressionStep : WizardStep
    {
        public FullRegressionStep(WizardSource myAnalysis)
        {
            InitializeComponent();

            StepToolTip = new PODToolTip();

            Title = new FullRegressionTitle(StepToolTip);
            Panel = new FullRegressionPanel(StepToolTip);
            ActionBar = new FullRegressionBar(StepToolTip);
            _overlay = new FullRegressionTutorial();

            Source = myAnalysis;

            

            //((FullRegressionPanel)_panel).SetAnalysisData(Analysis);

            //InitializeStep();
        }
    }
}
