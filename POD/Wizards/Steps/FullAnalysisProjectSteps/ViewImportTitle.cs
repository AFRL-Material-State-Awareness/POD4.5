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

namespace POD.Wizards.Steps.FullAnalysisProjectSteps
{
    public partial class ViewImportTitle : WizardTitle
    {
        public ViewImportTitle(PODToolTip tooltip) : base(tooltip)
        {
            InitializeComponent();

            StepToolTip = new PODToolTip();

            Header.Text = "Configure Data";
            SubHeader.Text = "Setup names, units and default values for the new data.";
        }
    }
}
