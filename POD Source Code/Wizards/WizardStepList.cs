using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POD.Wizards
{
    public class WizardStepList : List<WizardStep>
    {
        public WizardStep NextStep(WizardStep myStep, ref bool myPassed)
        {
            int index = IndexOf(myStep);

            if (index < 0)
            {
                return myStep;
            }

            //don't go to next step if there is a problem
            if (myStep.CheckStuck() == false)
            {
                index++;

                if (index >= this.Count)
                {
                    myPassed = true;
                    index = Count - 1;
                }
                else
                {
                    myPassed = false;
                }
            }

            return this[index];
        }

        public WizardStep PrevStep(WizardStep myStep, ref bool myPassed)
        {
            int index = IndexOf(myStep);

            if (index < 0)
            {
                return myStep;
            }

            index--;

            if (index < 0)
            {
                myPassed = true;
                index = 0;
            }
            else
            {
                myPassed = false;
            }

            return this[index];
        }



    }
}
