using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POD.Controls;

namespace POD.Wizards.Steps.AHatVsANormalSteps
{
    public partial class FullRegressionTitle : WizardTitle
    {
        public FullRegressionTitle(PODToolTip tooltip) : base(tooltip)
        {
            InitializeComponent();

            StepToolTip = new PODToolTip();

            Header.Text = "Linear Regression";
            SubHeader.Text = "Change boundaries to affect the fit.";
        }
    }
}
