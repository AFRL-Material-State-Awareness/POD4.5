using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using POD.Analyze;
using POD.Controls;

namespace POD.Wizards
{
    public partial class WizardPanel : WizardUI
    {
        List<ToolStripItem> _controlStrip;
        public int Index; //index of panel's location in the wizard
        public bool ControlSizesFixed;        

        public List<ToolStripItem> ControlStrip
        {
            get { return _controlStrip; }
            set { _controlStrip = value; }
        }

        protected virtual void SetupNumericControlPrecision()
        {
        }

        List<ToolStripItem> _textStrip;

        private WizardActionBar _actionBar;

        public List<ToolStripItem> TextStrip
        {
            get { return _textStrip; }
            set { _textStrip = value; }
        }

        public virtual void FixPanelControlSizes()
        {
            ControlSizesFixed = true;
        }

        public WizardPanel()
        {
            Initialize();
        }

        public WizardPanel(PODToolTip tooltip)
        {
            StepToolTip = new PODToolTip();

            //PODToolTip.Active = true;
            //PODToolTip.ShowAlways = true;

            Initialize();
        }

        private void Initialize()
        {
            InitializeComponent();

            

            _controlStrip = new List<ToolStripItem>();
            _textStrip = new List<ToolStripItem>();

            //contextMenu.ShowImageMargin = false;

            _textStrip.Add(new ToolStripSeparator());
            _textStrip.Add(new ToolStripMenuItem("Cu&t", null, itemCut_Click, "Cut"));
            _textStrip.Add(new ToolStripMenuItem("&Copy", null, itemCopy_Click, "Copy"));
            _textStrip.Add(new ToolStripMenuItem("&Paste", null, itemPaste_Click, "Paste"));
            _textStrip.Add(new ToolStripMenuItem("&Undo", null, itemUndo_Click, "Undo"));

            //ControlAdded += WizardPanel_ControlAdded;

            Load += WizardPanel_Load;
        }

        /*void WizardPanel_ControlAdded(object sender, ControlEventArgs e)
        {
            e.Control.Click += Control_Click;

            FixMenu(e.Control.Controls);
        }

        private void FixMenu(ControlCollection controlCollection)
        {
            foreach(Control control in controlCollection)
            {
                control.Click += Control_Click;

                FixMenu(control.Controls);
            }
        }

        void Control_Click(object sender, EventArgs e)
        {
            var menu = this.ContextMenuStrip as POD.Controls.ContextMenuStripConnected;

            menu.CloseEverything();
        }*/

        void WizardPanel_Load(object sender, EventArgs e)
        {
            PrepareGUI();
        }

        public ContextMenuStrip PODContextMenu
        {
            get { return contextMenu; }
        }

        public WizardActionBar WizActionBar
        {
            get
            {
                return _actionBar;
            }
            set
            {
                _actionBar = value;
            }
        }

        protected void itemCut_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;

            if (ActiveControl != null)
                SendMessage(ActiveControl.Handle, TextBoxMessages.WM_CUT, 0, 0);
        }

        protected void itemCopy_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;

            if (ActiveControl != null)
                SendMessage(ActiveControl.Handle, TextBoxMessages.WM_COPY, 0, 0);
        }

        protected void itemPaste_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;

            if (ActiveControl != null)
                SendMessage(ActiveControl.Handle, TextBoxMessages.WM_PASTE, 0, 0);
        }

        protected void itemUndo_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;

            if (ActiveControl != null)
                SendMessage(ActiveControl.Handle, TextBoxMessages.EM_UNDO, 0, 0);
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);



        public virtual void RefreshValues()
        {
            //Refresh();
        }

        public virtual bool Stuck
        {
            get
            {
                return false;
            }
        }

        public virtual bool CheckStuck()
        {
            return Stuck;
        }

        public dynamic MyActionBar
        {
            get
            {
                return WizActionBar;
            }
        }


        internal string GetLastClick()
        {
            return Source.GetNavigateLastClick();
        }

        internal void SetLastClicked(string value)
        {
            Source.SetNavigateLastClick(value);
        }

        internal virtual void PrepareGUI()
        {
            
        }

        public virtual void RefreshOverview()
        {
            
        }
    }

    public static class TextBoxMessages
    {
        public const int EM_UNDO = 0x00C7;
        public const int WM_CUT = 0x0300;
        public const int WM_COPY = 0x0301;
        public const int WM_PASTE = 0x0302;
    }
}
