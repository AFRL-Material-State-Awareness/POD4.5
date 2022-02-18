using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POD.Analyze;
using POD.Controls;

namespace POD.Wizards.Steps.HitMissNormalSteps
{
    public partial class QuickRegressionStep : WizardStep
    {
        public QuickRegressionStep(WizardSource myAnalysis)
        {
            InitializeComponent();

            StepToolTip = new PODToolTip();

            Title = new FullRegressionTitle(StepToolTip);
            Panel = new QuickRegressionPanel(StepToolTip);
            ActionBar = new QuickRegressionBar(StepToolTip);
            _overlay = new FullRegressionTutorial();

            Source = myAnalysis;

            //((FullRegressionPanel)_panel).SetAnalysisData(Analysis);

            //InitializeStep();
        }
    }
}
