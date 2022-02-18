using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using Transitions;
using System.IO;

namespace POD.Docks
{
    public partial class QuickHelpDock : RTFViewerDock
    {
        public QuickHelpDock() : base()
        {
            InitializeComponent();

            Label = Globals.QuickDockLabel;
            Text = Label;
            HelpName = "Quick Help Viewer";
        }

        

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void RichTextBox_MouseClick(object sender, MouseEventArgs e)
        {
            
        }     
   
       
    }
}
