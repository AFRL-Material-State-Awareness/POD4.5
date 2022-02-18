using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POD.Analyze;
using POD.Controls;

namespace POD.Wizards
{
    public partial class WizardTitle : WizardUI
    {
        public WizardTitle()
        {
            InitializeComponent();
        }

        public WizardTitle(PODToolTip tooltip) : base(tooltip)
        {
            InitializeComponent();

            StepToolTip = new PODToolTip();
        }

        public Label Header
        {
            get { return _header; }
            set { _header = value; }
        }

        public Label SubHeader
        {
            get { return _subHeader; }
            set { _subHeader = value; }
        }
    }
}
