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
    public partial class PasteDataTitle : WizardTitle
    {
        public PasteDataTitle(PODToolTip tooltip) : base(tooltip)
        {
            InitializeComponent();

            StepToolTip = new PODToolTip();

            Header.Text = "Import Data";
            SubHeader.Text = "Paste Data from Excel or csv. Then use Data Source Editing Tools to define your columns.";
        }
    }
}
