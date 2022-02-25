using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace POD.Docks
{
    public partial class HandbookHelpDock: RTFViewerDock
    {
        public HandbookHelpDock()
        {
            InitializeComponent();

            Label = Globals.HandbookDockLabel;
            Text = Label;
            HelpName = "1823A Help Viewer";

            //RTFBox.Rtf = Docks.Properties.Resources.G_1_2;
        }

        
    }
}
