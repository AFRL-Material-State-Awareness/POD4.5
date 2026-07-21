// <copyright file="WizardStepList.cs" company="The University of Dayton">
// Copyright © 2025 The University of Dayton. All rights reserved.
//
// Licensed under the GNU General Public License, Version 3.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.gnu.org/licenses/gpl-3.0.txt
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

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
