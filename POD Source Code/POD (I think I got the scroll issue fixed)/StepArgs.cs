using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POD
{
    public class StepArgs : EventArgs
    {
    
        public StepArgs(int myIndex)
        {
            Index = myIndex;
            HelpFile = string.Empty;
        }

        public StepArgs(int myIndex, string myHelpFile)
        {
            Index = myIndex;
            HelpFile = myHelpFile;
        }

        public StepArgs(int myIndex, string myHelpFile, TreeNode myNode)
        {
            Index = myIndex;
            HelpFile = myHelpFile;
            ListNode = myNode;
        }

        /// <summary>
        /// An index of a Wizard Step.
        /// </summary>
        public int Index;

        /// <summary>
        /// the name of the help file to load into RTF viewer
        /// </summary>
        public string HelpFile;

        /// <summary>
        /// Node who's children make up a list of step for the wizard
        /// </summary>
        public TreeNode ListNode;
    }
}
