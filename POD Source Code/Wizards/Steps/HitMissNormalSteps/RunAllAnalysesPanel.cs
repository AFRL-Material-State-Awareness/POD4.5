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
    public partial class RunAllAnalysesPanel : WizardPanel
    {
        public RunAllAnalysesPanel(PODToolTip tooltip)
            : base(tooltip)
        {
            StepToolTip = new PODToolTip();

            InitializeComponent();
        }
        
    }
}
