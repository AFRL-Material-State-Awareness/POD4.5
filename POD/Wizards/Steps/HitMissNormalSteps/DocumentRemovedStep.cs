using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POD.Analyze;
using POD.Controls;

namespace POD.Wizards.Steps.HitMissNormalSteps
{
    public partial class DocumentRemovedStep : WizardStep
    {
        public DocumentRemovedStep(WizardSource myAnalysis)
        {
            InitializeComponent();

            StepToolTip = new PODToolTip();

            Title = new DocumentRemovedTitle(StepToolTip);
            Panel = new DocumentRemovedPanel(StepToolTip);
            ActionBar = new DocumentRemovedBar(StepToolTip);
            _overlay = new DocumentRemovedTutorial();

            Source = myAnalysis;

            //((FullRegressionPanel)_panel).SetAnalysisData(Analysis);

            //InitializeStep();
        }
    }
}
