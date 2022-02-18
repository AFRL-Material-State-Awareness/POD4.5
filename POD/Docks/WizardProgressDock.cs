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

namespace POD.Docks
{
    /// <summary>
    /// Provides a list of steps that are associated with the current active Wizard.
    /// Current step is highlighted. And the user can click on step to jump to
    /// a given step in the wizard.
    /// </summary>
    public partial class WizardProgressDock : PodDock
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new Wizard Pogress Dock.
        /// </summary>
        public WizardProgressDock()
        {
            InitializeComponent();

            Label = Globals.ProgressDockLabel;
            Text = Label;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Get the TreeView control that holds the list of steps.
        /// </summary>
        public TreeView Tree
        {
            get
            {
                return treeView1;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Clear the list of Wizard steps.
        /// </summary>
        public void Clear()
        {
            if(treeView1.IsDisposed == false)
                treeView1.Nodes.Clear();
        }
        /// <summary>
        /// Return Selected Node font to Regular.
        /// </summary>
        private void ClearSelectedNode()
        {
            if (treeView1.SelectedNode != null)
                treeView1.SelectedNode.NodeFont = new Font(treeView1.Font, FontStyle.Regular);
        }
        /// <summary>
        /// Select the Node at the given index.
        /// </summary>
        /// <param name="myIndex">the index of the Node to select</param>
        public void UpdateIndex(int myIndex)
        {
            if (treeView1.Nodes.Count > myIndex)
            {
                if (treeView1.SelectedNode != treeView1.Nodes[myIndex])
                {
                    ClearSelectedNode();
                    treeView1.SelectedNode = treeView1.Nodes[myIndex];
                }
            }
        }
        /// <summary>
        /// Set all of the children Nodes as Nodes in the Tree.
        /// </summary>
        /// <param name="myStartTreeNode">child Nodes of this Node are used to create the Step List that the user sees</param>
        public void UpdateList(TreeNode myStartTreeNode)
        {
            bool isSameList = true;

            if (myStartTreeNode.Nodes.Count == treeView1.Nodes.Count)
            {
                foreach (TreeNode node in myStartTreeNode.Nodes)
                {
                    if (node.Text != treeView1.Nodes[node.Index].Text)
                    {
                        isSameList = false;
                        break;
                    }
                }
            }
            else
            {
                isSameList = false;
            }

            if (isSameList == false)
            {
                //BlendBox.CaptureOldStateImage(treeView1);

                treeView1.BeginUpdate();

                treeView1.Nodes.Clear();

                foreach (TreeNode node in myStartTreeNode.Nodes)
                {
                    treeView1.Nodes.Add(node);
                }

                treeView1.EndUpdate();

                //BlendBox.CaptureNewImage(treeView1);

                //BlendToNextImage();
            }
        }
        #endregion

        #region Event Handling
        /// <summary>
        /// Return previously Selected Node to Regular before new Node is selected.
        /// </summary>
        /// <param name="sender">default sender</param>
        /// <param name="e">default arguments</param>
        private void Item_Select(object sender, TreeViewCancelEventArgs e)
        {
            ClearSelectedNode();
        }
        /// <summary>
        /// If a Node has been selected, set font to Bold.
        /// </summary>
        /// <param name="sender">default sender</param>
        /// <param name="e">default arguments</param>
        private void Item_Selected(object sender, TreeViewEventArgs e)
        {
            if (e != null && e.Node != null)
            {
                e.Node.NodeFont = new Font(treeView1.Font, FontStyle.Bold);
                e.Node.Text = e.Node.Text;
            }
        }
        #endregion


        public void ClearNodes()
        {
            treeView1.BeginUpdate();

            foreach(TreeNode node in treeView1.Nodes)
            {
                node.NodeFont = new Font(treeView1.Font, FontStyle.Regular);
            }

            treeView1.EndUpdate();
        }

        public int DefaultHeight
        {
            get
            {
                return treeView1.ItemHeight * 8;
            }
        }
    }
}
