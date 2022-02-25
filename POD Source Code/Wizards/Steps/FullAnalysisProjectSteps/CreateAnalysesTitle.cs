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
    public partial class CreateAnalysesTitle : WizardTitle
    {
        public CreateAnalysesTitle(PODToolTip tooltip) : base(tooltip)
        {
            InitializeComponent();

            Header.Text = "Create Analyses";
            SubHeader.Text = "Select flaws and responses from available data sources to create analyses.";
        }
    }
}
