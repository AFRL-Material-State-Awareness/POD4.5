using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POD.Controls;

namespace POD.Wizards.Steps.HitMissNormalSteps
{
    public partial class FullRegressionTitle : WizardTitle
    {
        public FullRegressionTitle(PODToolTip tooltip) : base(tooltip)
        {
            InitializeComponent();

            StepToolTip = new PODToolTip();

            Header.Text = "Hit Miss Regression";
            SubHeader.Text = "Select your transform and model for your POD analysis.";
        }
    }
}
