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
    public partial class ProjectPropertiesTitle : WizardTitle
    {
        public ProjectPropertiesTitle(PODToolTip tooltip) : base(tooltip)
        {
            InitializeComponent();

            StepToolTip = new PODToolTip();

            Header.Text = "Project Setup";
            SubHeader.Text = "Enter information about the project. Double click charts to open analyses.";

        }
    }
}
