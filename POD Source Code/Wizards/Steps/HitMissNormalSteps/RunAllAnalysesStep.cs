using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POD.Controls;
namespace POD.Wizards.Steps.HitMissNormalSteps
{
    public partial class RunAllAnalysesStep : WizardStep
    {
        public RunAllAnalysesStep(WizardSource myAnalysis)
        {
            InitializeComponent();
            StepToolTip = new PODToolTip();

            Title = new FullRegressionTitle(StepToolTip);
            Panel = new RunAllAnalysesPanel(StepToolTip);
            //Panel = new FullRegressionPanel(StepToolTip);
            ActionBar = new RunAllAnalysesBar(StepToolTip);
            //ActionBar = new FullRegressionBar(StepToolTip);
            //_overlay = new FullRegressionTutorial();

            Source = myAnalysis;
        }
    }
}
