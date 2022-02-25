using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POD.Controls;

namespace POD.Wizards.Steps.HitMissNormalSteps
{
    public partial class ChooseTransformTitle : WizardTitle
    {
        public ChooseTransformTitle(PODToolTip tooltip) : base(tooltip)
        {
            InitializeComponent();

            StepToolTip = new PODToolTip();

            Header.Text = "Choose Transform";
            SubHeader.Text = "Inspect each result from the possible transform combinations.";
        }
    }
}
