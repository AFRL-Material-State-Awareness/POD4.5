using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POD.Controls;

namespace POD.Wizards.Steps.AHatVsANormalSteps
{
    public partial class DocumentRemovedTitle : WizardTitle
    {
        public DocumentRemovedTitle(PODToolTip tooltip)
            : base(tooltip)
        {
            InitializeComponent();

            StepToolTip = new PODToolTip();

            Header.Text = "Document and Export";
            SubHeader.Text = "Document why points were removed from the analysis. Export to Excel if needed.";
        }
    }
}
