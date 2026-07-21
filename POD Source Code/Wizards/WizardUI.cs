// <copyright file="WizardUI.cs" company="The University of Dayton">
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
using POD.Analyze;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using POD.Controls;

namespace POD.Wizards
{
    public class WizardUI : UserControl
    {
        protected WizardSource _source;
        public PODToolTip StepToolTip;

        public WizardUI()
        {
        }

        public virtual bool SendKeys(Keys keyData)
        {
            return false;
        }

        public WizardUI(PODToolTip tooltip)
        {
            StepToolTip = new PODToolTip();
        }

        public virtual bool NeedsRefresh(WizardSource mySource)
        {
            return true;
        }

        public virtual WizardSource Source
        {
            get
            {
                return _source;
            }
            set
            {
                


                _source = value;
            }
        }

        //child class should know if the source is a project or an analysis
        public Project Project 
        {
            get
            {
                return (Project)Source;
            }
        }

        //child class should know if the source is a project or an analysis
        public Analysis Analysis
        {
            get
            {
                return (Analysis)Source;
            }
        }

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);
        private const int WM_SETREDRAW = 11;
        private const int TVM_GETEXTENDEDSTYLE = 0x1100 + 45;
        private const int TVM_SETEXTENDEDSTYLE = 0x1100 + 44;
        private const int TVS_EX_DOUBLEBUFFER = 0x4;

        private int suspendCounter = 0;

        public void SuspendDrawing()
        {
            if (suspendCounter == 0)
                SendMessage(this.Handle, WM_SETREDRAW, false, 0);
            suspendCounter++;
        }

        public void ResumeDrawing()
        {
            suspendCounter--;
            if (suspendCounter == 0)
            {
                SendMessage(this.Handle, WM_SETREDRAW, true, 0);
                this.Refresh();
            }
        }
    }
}
