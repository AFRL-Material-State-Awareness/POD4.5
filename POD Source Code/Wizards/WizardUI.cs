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
