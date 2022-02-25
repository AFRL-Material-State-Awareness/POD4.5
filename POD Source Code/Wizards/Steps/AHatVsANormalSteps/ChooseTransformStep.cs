using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POD.Controls;

namespace POD.Wizards.Steps.AHatVsANormalSteps
{
    public partial class ChooseTransformStep : WizardStep
    {
        public ChooseTransformStep(WizardSource myAnalysis)
        {
            InitializeComponent();

            StepToolTip = new PODToolTip();

            Title = new ChooseTransformTitle(StepToolTip);
            Panel = new ChooseTransformPanel(StepToolTip);
            ActionBar = new ChooseTransformBar(StepToolTip);
            _overlay = new ChooseTransformTutorial();

            Source = myAnalysis;

            //((FullRegressionPanel)_panel).SetAnalysisData(Analysis);

            //InitializeStep();
        }
    }
}
