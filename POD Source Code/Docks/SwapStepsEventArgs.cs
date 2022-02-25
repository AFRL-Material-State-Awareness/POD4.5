using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using POD;
using POD.Wizards;

namespace POD.Docks
{
    

    /// <summary>
    /// Holds information about a Wizard finishing that is relavent to events.
    /// </summary>
    public class SwapStepsEventArgs : EventArgs
    {
        public WizardStep TransitionFrom;
        public WizardStep TransitionTo;
        public StepArgs Args;
        public int Direction;

        public SwapStepsEventArgs(WizardStep prevStep, WizardStep nextStep, StepArgs e, int direction)
        {
            this.TransitionFrom = prevStep;
            this.TransitionTo = nextStep;
            this.Args = e;
            this.Direction = direction;
        }
    }
}
