using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using POD.Controls;
using POD.Analyze;
using System.Windows.Forms.VisualStyles;

namespace POD.Wizards.Steps.FullAnalysisProjectSteps
{
    using POD.Data;

    

    public partial class CreateAnalysesPanel : WizardPanel
    {
        private int _numberOfDataSources;
        private Dictionary<string, Color> _dataSourceColors;
        private bool _useAutoName;
        private EditAnalysisDialog editDialog;
        //private ToolStripMenuItem _availableVBarToolStrip = null;
        //private ToolStripMenuItem _analysisVBarToolStrip = null;
        //private ToolStripMenuItem _analysesVBarToolStrip = null;
        private PODButton _addButton = null;
        private Dictionary<string, List<int>> _selectedAvailableList = new Dictionary<string, List<int>>();
        private Dictionary<string, List<int>> _selectedFlawList = new Dictionary<string, List<int>>();
        private AnalysisList _removedAnalyses = new AnalysisList();
        private Dictionary<PODTreeNode, PODTreeNode> _removedTrackConnectingNode = new Dictionary<PODTreeNode, PODTreeNode>();
        private AnalysisList _currentAnalyses = new AnalysisList();
        private Analysis _editedAnalysis = null;
        private List<Color> _colors = null;

        public AnalysisList RemovedAnalysisList
        {
            get
            {
                return _removedAnalyses;
            }
        }

        public override bool SendKeys(Keys keyData)
        {
            var result = base.SendKeys(keyData);

            if (result)
                return true;

            if (keyData == (Keys.Control | Keys.A | Keys.Shift))
            {
                _selectNoneButton.PerformClick();
                return true;
            }
            if (keyData == (Keys.Control | Keys.H | Keys.Shift))
            {
                _checkNoneButton.PerformClick();
                return true;
            }
            if (keyData == (Keys.Control | Keys.G))
            {
                _addButton.PerformClick();
                return true;
            }
            if (keyData == (Keys.Control | Keys.A))
            {
                _selectNoneButton.PerformClick();
                return true;
            }
            if (keyData == (Keys.Control | Keys.I))
            {
                _invertSelectButton.PerformClick();
                return true;
            }
            if (keyData == (Keys.Control | Keys.D))
            {
                _deleteSelectedButton.PerformClick();
                return true;
            }
            if (keyData == (Keys.Control | Keys.H))
            {
                _checkAllButton.PerformClick();
                return true;
            }
            if (keyData == (Keys.Delete) && ActiveControl == analysesTree)
            {
                _deleteSelectedButton.PerformClick();
                return true;
            }

            return false;
        }

        public CreateAnalysesPanel(PODToolTip tooltip) : base(tooltip)
        {
            InitializeComponent();

            StepToolTip = new PODToolTip();

            AvailableVBar.ToolTip = StepToolTip;
            AnalysesVBar.ToolTip = StepToolTip;

            //listBox2.Dropped += Items_Dropped;
            //availableDataList.DragLeave += Items_Dropping;

            AvailableVBar.AddLabel("Analyses");
            _addButton = AvailableVBar.AddButton("Create Analysis", CopyAndAdd_Click, "Add a new analysis to the project. (Ctrl + G)");
            AvailableVBar.AddLabel("Responses");
            _selectAllButton = AvailableVBar.AddButton("Select All", All_Click, "Select all Responses from the list. (Ctrl + A)");
            _selectNoneButton = AvailableVBar.AddButton("Select None", Clear_Click, "Unselect all Responses from the list. (Ctrl + Shift + A)");
            _invertSelectButton = AvailableVBar.AddButton("Invert Selection", Invert_Click, "Invert selection of Responses. (Ctrl + I)");

            AnalysesVBar.AddLabel("Analyses");
            _deleteSelectedButton = AnalysesVBar.AddButton("Delete Selected", DeleteSelected_Click, "Delete selected analysis from project. (Ctrl + D)");
            AnalysesVBar.AddLabel("Open After Finishing");
            _checkAllButton = AnalysesVBar.AddButton("Check All", OpenAllOnFinish_Click, "Check all analyses to be opened after finishing this step. (Ctrl + H)");
            _checkNoneButton = AnalysesVBar.AddButton("Check None", OpenNoneOnFinish_Click, "Uncheck all analyses from being opened after finishing this step. (Ctrl + Shift + H)");

            ////contextMenu.ShowImageMargin = true;

            var availableList = new List<ButtonHolder>();
            var analysesList = new List<ButtonHolder>();

            //parent.DropDownItems.Add(contextMenu.Items.Add("Add", _addButton.Image, CopyAndAdd_Click));
            //parent.DropDownItems.Add(contextMenu.Items.Add("Select None", _selectNoneButton.Image, Clear_Click));
            //parent.DropDownItems.Add(contextMenu.Items.Add("Select All", _selectAllButton.Image, All_Click));
            //parent.DropDownItems.Add(contextMenu.Items.Add("Select Invert", _invertSelectButton.Image, Invert_Click));

            availableList.Add(new ButtonHolder("Add", _addButton.Image, CopyAndAdd_Click));
            availableList.Add(new ButtonHolder("Select None", _selectNoneButton.Image, Clear_Click));
            availableList.Add(new ButtonHolder("Select All", _selectAllButton.Image, All_Click));
            availableList.Add(new ButtonHolder("Select Invert", _invertSelectButton.Image, Invert_Click));

            //MakeChildItemImagesClear(parent);

            //parent = (ToolStripMenuItem)contextMenu.Items.Add("Analysis");

            //_analysisVBarToolStrip = parent;

            //parent.DropDownItems.Add(contextMenu.Items.Add("Update", null, AddNew_Click));
            //parent.DropDownItems.Add(contextMenu.Items.Add("Remove", null, ClearSelected_Click));
            //parent.DropDownItems.Add(contextMenu.Items.Add("Select None", null, Clear2_Click));
            //parent.DropDownItems.Add(contextMenu.Items.Add("Select All", null, All2_Click));
            //parent.DropDownItems.Add(contextMenu.Items.Add("Select Invert", null, Invert2_Click));

            //MakeChildItemImagesClear(parent);

            //parent = (ToolStripMenuItem)contextMenu.Items.Add("Analyses");

            //_analysesVBarToolStrip = parent;

            //parent.DropDownItems.Add(contextMenu.Items.Add("Edit", null, CopyBack_Click));
            //parent.DropDownItems.Add(contextMenu.Items.Add("Remove", _deleteSelectedButton.Image, DeleteSelected_Click));
            //parent.DropDownItems.Add(contextMenu.Items.Add("Open On Finish", null, OpenSelectedOnFinish_Click));
            //parent.DropDownItems.Add(contextMenu.Items.Add("Open All On Finish", _checkAllButton.Image, OpenAllOnFinish_Click));
            //parent.DropDownItems.Add(contextMenu.Items.Add("Open None On Finish", _checkNoneButton.Image, OpenNoneOnFinish_Click));

            analysesList.Add(new ButtonHolder("Remove", _deleteSelectedButton.Image, DeleteSelected_Click));
            analysesList.Add(new ButtonHolder("Open All On Finish", _checkAllButton.Image, OpenAllOnFinish_Click));
            analysesList.Add(new ButtonHolder("Open None On Finish", _checkNoneButton.Image, OpenNoneOnFinish_Click));

            var availablePanel = ContextMenuStripConnected.MakeNewMenuFlowLayoutPanel("Available_Data");
            var analysesPanel = ContextMenuStripConnected.MakeNewMenuFlowLayoutPanel("Available_Data");
            var availableHost = new ToolStripControlHost(availablePanel);
            var analysesHost = new ToolStripControlHost(analysesPanel);

            foreach(var item in availableList)
            {
                ContextMenuStripConnected.AddButtonToMenu(availablePanel, item, StepToolTip);
            }

            ContextMenuStripConnected.ForcePanelToDraw(availablePanel);

            foreach (var item in analysesList)
            {
                ContextMenuStripConnected.AddButtonToMenu(analysesPanel, item, StepToolTip);
            }

            ContextMenuStripConnected.ForcePanelToDraw(analysesPanel);

            contextMenu.Items.Add(availableHost);
            contextMenu.Items.Add(analysesHost);
            ControlStrip.Add(availableHost);
            ControlStrip.Add(analysesHost);
            TextStrip.Add(availableHost);
            TextStrip.Add(analysesHost);

            //MakeChildItemImagesClear(parent);

            ContextMenuStrip = contextMenu;

            analysesChart.Titles.Add("Selected Analysis Data");
            analysisChart.Titles.Add("Selected Analysis Data");
            availableChart.Titles.Add("Selected Analysis Data");

            //_analysisVBarToolStrip.Visible = false;

            _numberOfDataSources = 0;
            _dataSourceColors = new Dictionary<string, Color>();

            Load += CreateAnalysesPanel_Load;

            _removedAnalysisNodes = new List<PODTreeNode>();
            _removedAnalyses = new AnalysisList();
            _currentAnalyses = new AnalysisList();

            this.availableChart.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
            this.availableChart.ChartAreas[0].AxisY.MajorGrid.LineWidth = 0;

            this.analysesChart.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
            this.analysesChart.ChartAreas[0].AxisY.MajorGrid.LineWidth = 0;

            _colors = DataPointChart.GetLargeColorList(this.DesignMode);


            //this.Layout += CreateAnalysesPanel_Layout;
        }

        

        private static void MakeChildItemImagesClear(ToolStripMenuItem parent)
        {
            foreach (ToolStripMenuItem item in parent.DropDownItems)
            {
                item.ImageScaling = ToolStripItemImageScaling.None;
            }
        }

        public void UndoDeletedAnalysis()
        {
            if (_removedAnalysisNodes.Count > 0)
            {
                var restore = _removedAnalysisNodes[_removedAnalysisNodes.Count - 1];

                if(!restore.IsNewAnalysis)
                    _removedAnalyses.RemoveAt(_removedAnalyses.Names.IndexOf(restore.OriginalAnalysisName));

                RestoreNode(restore);

                _removedAnalysisNodes.RemoveAt(_removedAnalysisNodes.Count - 1);
                _removedTrackConnectingNode.Remove(restore);
            }
        }

        private void RestoreNode(PODTreeNode restoreNode)
        {
            if (_removedAnalysisNodes.Contains(restoreNode) && _removedTrackConnectingNode.ContainsKey(restoreNode))
            {
                var insertAt = 0;
                var before = _removedTrackConnectingNode[restoreNode];

                if (before != null)
                {
                    var foundBefore = false;

                    while(foundBefore == false)
                    {                            
                        foreach (PODTreeNode node in analysesTree.Nodes)
                        {
                            if (node.OriginalAnalysisName == before.OriginalAnalysisName)
                            {
                                insertAt = node.Index+1;
                                foundBefore = true;
                                break;
                            }
                        }

                        if(foundBefore == false)
                            before = _removedTrackConnectingNode[before];

                        if (before == null)
                        {
                            insertAt = 0;
                            foundBefore = true;
                        }
                    }


                }

                this.analysesTree.Nodes.Insert(insertAt, restoreNode);
                this.analysesTree.SelectedNode = restoreNode;
            }
        }

        void CreateAnalysesPanel_Load(object sender, EventArgs e)
        {
            //let labels get cut off rather than make rows taller to fit
            //int[] heights = availableTableLayout.GetRowHeights();
            //var topLabelHeight = label1.Height;
            //createTableLayout.RowStyles[0].Height = topLabelHeight;
            //createTableLayout.RowStyles[0].SizeType = SizeType.Absolute;

            //only want one column
            while (AvailableFlawsListBox.Columns.Count > 1)
                AvailableFlawsListBox.Columns.RemoveAt(AvailableFlawsListBox.Columns.Count - 1);

            while (AvailableSourcesListBox.Columns.Count > 1)
                AvailableSourcesListBox.Columns.RemoveAt(AvailableSourcesListBox.Columns.Count - 1);

            while (AvailableResponsesListBox.Columns.Count > 1)
                AvailableResponsesListBox.Columns.RemoveAt(AvailableResponsesListBox.Columns.Count - 1);

            StepToolTip.SetToolTip(AnalysisNameTextBox, "Edit name of currently selected analysis.");
        }

        //void CreateAnalysesPanel_Layout(object sender, LayoutEventArgs e)
        //{
        //    availableDataList.BringToFront();
        //}

        private void SwitchVBarToDialog()
        {
            _inDialog = true;
            AvailableFlawsListBox.MultiSelect = false;

            _addButton.Click -= CopyAndAdd_Click;
            _addButton.Click += Copy_Click;

            //_analysisVBarToolStrip.Visible = true;
            //_analysesVBarToolStrip.Visible = false;

            //_availableVBarToolStrip.DropDownItems[0].Click -= CopyAndAdd_Click;
            //_availableVBarToolStrip.DropDownItems[0].Click += Copy_Click;
        }

        private void SwitchVBarToMain()
        {
            _inDialog = false;
            AvailableFlawsListBox.MultiSelect = true;

            _addButton.Click -= Copy_Click;
            _addButton.Click += CopyAndAdd_Click;

            //_availableVBarToolStrip.DropDownItems[0].Click -= Copy_Click;
            //_availableVBarToolStrip.DropDownItems[0].Click += CopyAndAdd_Click;

            //_analysisVBarToolStrip.Visible = false;
            //_analysesVBarToolStrip.Visible = true;
        }

        private void Items_Dropping(object sender, EventArgs e)
        {
            _useAutoName = UsingAutoName;
        }

        private PODListBox GetCurrentDataList()
        {
            return AvailableResponsesListBox;
        }

        private void CopyAndAdd_Click(object sender, EventArgs e)
        {
            Copy_Click(sender, e);
            //AddNew_Click(sender, e);
        }

        private void OpenNoneOnFinish_Click(object sender, EventArgs e)
        {
            foreach (TreeNode node in analysesTree.Nodes)
            {
                node.Checked = false;
            }

            analysesTree.Select();
        }

        private void OpenAllOnFinish_Click(object sender, EventArgs e)
        {
            foreach (TreeNode node in analysesTree.Nodes)
            {
                node.Checked = true;
            }

            analysesTree.Select();
        }

        private void OpenSelectedOnFinish_Click(object sender, EventArgs e)
        {
            TreeNode node = analysesTree.SelectedNode;
            List<string> names = new List<string>();

            if (node != null)
            {
                node.Checked = true;

                analysesTree.Select();
            }
        }

        private void Invert3_Click(object sender, EventArgs e)
        {

        }

        private void Clear3_Click(object sender, EventArgs e)
        {

        }

        private void All3_Click(object sender, EventArgs e)
        {

        }

        private void DeleteSelected_Click(object sender, EventArgs e)
        {
            PODTreeNode node = analysesTree.SelectedNode as PODTreeNode;
            List<PODListBoxItem> names = new List<PODListBoxItem>();

            if (node != null)
            {
                /*if (node.Parent != null)
                {
                    foreach (PODTreeNode child in node.Parent.Nodes)
                    {
                        if (child != node)
                        {
                            names.Add(new PODListBoxItem(child.RowColor, child.ResponseLabel, child.FlawLabel, child.DataSource));
                        }
                    }

                    node.Parent.Text = PODListBox.CreateAutoName(names);
                }*/

                if (node.Parent == null)
                {
                    analysesTree.SelectedNode = node.NextNode;


                    _removedAnalysisNodes.Add(node);

                    var index = _currentAnalyses.Names.IndexOf(node.OriginalAnalysisName);

                    if (index >= 0)
                    {
                        _removedAnalyses.Add(_currentAnalyses[index]);
                    }

                    if (!_removedTrackConnectingNode.ContainsKey(node))
                        _removedTrackConnectingNode.Add(node, node.PrevNode as PODTreeNode);
                    else
                        _removedTrackConnectingNode[node] = node.PrevNode as PODTreeNode;

                    if (node.PrevNode != null)
                        analysesTree.SelectedNode = node.PrevNode;

                    analysesTree.Nodes.Remove(node);

                    analysesTree.Select();
                }

            }
        }

        private void CopyBack_Click(object sender, EventArgs e)
        {
            /*PODTreeNode node = analysesTree.SelectedNode as PODTreeNode;
            List<PODListBoxItem> responses = new List<PODListBoxItem>();
            PODListBoxItem flaw = null;
            

            if (node != null)
            {
                _editedAnalysis = null;

                foreach (PODTreeNode child in node.Nodes)
                {
                    responses.Add(new PODListBoxItem(child.RowColor, child.ResponseLabel, child.FlawLabel, child.DataSource));

                    if (flaw == null)
                        flaw = new PODListBoxItem(Color.Black, string.Empty, child.FlawLabel, child.DataSource);
                }

                EditListBox.Rows.Clear();
                CopyToAnalysisListBox(flaw, responses);

                AnalysisNameTBox.Text = node.Text;
                All2_Click(sender, e);

                var analysisIndex = _currentAnalyses.Names.IndexOf(node.Text);

                if (analysisIndex >= 0)
                    _editedAnalysis = _currentAnalyses[analysisIndex];
                
                AvailableBlendBox.CaptureOldStateImage(availableTableLayout);

                AvailableBlendBox.BringToFront();
                AvailableBlendBox.Visible = true;

                VBarBlendBox.CaptureOldStateImage(AvailableVBar);
                VBarBackPanel.MinimumSize = new Size(AvailableVBar.Width, AvailableVBar.Height);
                

                VBarBlendBox.BringToFront();
                VBarBlendBox.Visible = true;

                SwitchVBarToDialog();

                editDialog = new EditAnalysisDialog(AvailableVBar, availableTableLayout,
                                                    AnalysisVBar, analysisTableLayout);

                editDialog.ActiveControl = AnalysisNameTBox;
                AnalysisNameTBox.SelectAll();
                editDialog.OkayButton.Click += AddNew_Click;

                var result = editDialog.ShowDialog();
                
                SwitchVBarToMain();

                if (result == DialogResult.OK)
                {
                    //putting new node where old node was
                    var nodeIndex = node.Index;

                    var newNode = analysesTree.Nodes[analysesTree.Nodes.Count - 1];

                    node.Remove();

                    analysesTree.Nodes.Remove(newNode);

                    analysesTree.Nodes.Insert(nodeIndex, newNode);

                    analysesTree.SelectedNode = newNode;
                }

    
                AvailableBackPanel.Controls.Add(availableTableLayout);                
                //VBarBackPanel.Controls.Add(AvailableVBar);
                AvailableVBar.BringToFront();
                AvailableBlendBox.Visible = false;
                AvailableBlendBox.Image = null;
                AvailableBlendBox.BackgroundImage = null;
                AvailableBlendBox.SendToBack();
                AvailableBlendBox.MinimumSize = new System.Drawing.Size(0, 0);
                VBarBackPanel.MinimumSize = new Size(0, 0);
                VBarBlendBox.Visible = false;
                VBarBlendBox.SendToBack();

                AvailableResponsesListBox.BringToFront();
                AvailableResponsesListBox.Invalidate();
            }*/
        }

        private void Invert2_Click(object sender, EventArgs e)
        {
            InvertList(EditListBox);
        }

        private void Clear2_Click(object sender, EventArgs e)
        {
            EditListBox.ClearSelection();
            EditListBox.CurrentCell = null;

            //while (listBox2.SelectedRows.Count > 0)
           // {
            //    listBox2.Rows.Remove(listBox2.SelectedRows[0]);
            //}
        }

        private void All2_Click(object sender, EventArgs e)
        {
            SelectAllList(EditListBox);
        }

        /// <summary>
        /// Must call before changing items in listBox2!
        /// </summary>
        private bool UsingAutoName
        {
            get
            {
                string beforeAutoName = EditListBox.AutoName;
                string currentName = AnalysisNameTBox.Text;

                return (beforeAutoName == currentName) || currentName == string.Empty;
            }
        }

        private void ClearSelected_Click(object sender, EventArgs e)
        {
            bool useAutoName = UsingAutoName;

            foreach (DataGridViewRow row in EditListBox.SelectedRows)
            {
                EditListBox.Rows.Remove(row);
            }

            EditListBox.ClearSelection();

            //while (listBox2.SelectedRows.Count > 0)
            //{
            //    listBox2.Rows.Remove(listBox2.SelectedRows[0]);
            //}

            //if using autoname
            if (useAutoName)
                AnalysisNameTBox.Text = EditListBox.AutoName;

            if (editDialog != null && editDialog.Visible)
                editDialog.ActiveControl = AnalysisNameTBox;

            AnalysisNameTBox.SelectAll();
        }

        private void AddNew_Click(object sender, EventArgs e)
        {
            if (EditListBox.Rows.Count > 0)
            {
                analysesTree.BeginUpdate();

                if (editDialog != null && editDialog.Visible == true)
                {
                    editDialog.DialogResult = DialogResult.OK;
                    editDialog.Close();
                }

                bool useAutoName = UsingAutoName;
                string analysisName = AnalysisNameTBox.Text;
                string constantSource = string.Empty;
                string constantSourceOriginal = string.Empty;
                string constantFlaw = string.Empty;
                string constantFlawOriginal = string.Empty;

                PODListBoxItemWithProps analysisConstantsItem = (PODListBoxItemWithProps)EditListBox.Rows[0].Cells[0].Value;

                constantSource = analysisConstantsItem.DataSourceName;
                constantSourceOriginal = analysisConstantsItem.DataSourceOriginalName;
                constantFlaw = analysisConstantsItem.FlawColumnName;
                constantFlawOriginal = analysisConstantsItem.FlawOriginalName;
                
                PODTreeNode analysisNode = null;
                var isUsingCustomName = GenerateUniqueName(ref analysisName);

                analysisNode = new PODTreeNode(new PODListBoxItem(Color.Black, string.Empty, string.Empty, constantFlaw, constantFlawOriginal, constantSource, constantSourceOriginal), analysisName, isUsingCustomName);
                analysisNode.IsNewAnalysis = true;
                this.analysesTree.Nodes.Add(analysisNode);
                
                foreach (DataGridViewRow row in EditListBox.Rows)
                {
                    var obj = (PODListBoxItemWithProps)row.Cells[0].Value;
                    PODTreeNode node = new PODTreeNode(obj, obj.ToPODItem().ToString(), false);

                    node.IsHidden = true;

                    analysisNode.Nodes.Add(node);
                }

                All2_Click(sender, e);
                ClearSelected_Click(sender, e);

                //always remove name after adding the analysis
                AnalysisNameTBox.Text = string.Empty;
                
                analysesTree.CollapseAll();

                analysisNode.Expand();
                analysesTree.SelectedNode = analysisNode;

                analysesTree.Select();

                analysesTree.EndUpdate();
            }
        }

        /// <summary>
        /// Generate a name that is unique to the analysis tree by appending an index to it
        /// </summary>
        /// <param name="analysisName"></param>
        /// <returns></returns>
        private bool GenerateUniqueName(ref string analysisName)
        {
            var currentNodeNames = new List<string>();
            var postfix = "";

            foreach (PODTreeNode node in analysesTree.Nodes)
            {
                currentNodeNames.Add(node.NewAnalysisName);
            }

            var isUsingCustomName = false;
            var newName = analysisName;
            int postIndex = 2;

            while (currentNodeNames.Contains(newName))
            {
                postfix = "_" + postIndex.ToString("##");
                newName = analysisName + postfix;
                isUsingCustomName = true;
                postIndex++;
            }

            analysisName = newName;

            return isUsingCustomName;
        }

        private string GetEverythingButSource(string sourceName, string name)
        {
            return name.Substring(sourceName.Length + 1);
        }

        private void Invert_Click(object sender, EventArgs e)
        {
            this.InvertList(this.GetCurrentDataList());
        }

        private void InvertList(PODListBox listBox)
        {
            for (int i = 0; i < listBox.Rows.Count; i++)
            {
                listBox.SetSelected(i, listBox.GetSelected(i) != true);
            }

            if(listBox.RowCount > 0)
                listBox.FirstDisplayedScrollingRowIndex = 0;
        }

        private void All_Click(object sender, EventArgs e)
        {
            SelectAllList(this.GetCurrentDataList());
        }

        private void SelectAllList(Controls.PODListBox myListBox)
        {
            for (int i = 0; i < myListBox.Rows.Count; i++)
            {
                myListBox.SetSelected(i, true);
            }
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            this.GetCurrentDataList().ClearSelection();
        }

        private void Copy_Click(object sender, EventArgs e)
        {
            bool useAutoName = true;// UsingAutoName;
            List<PODListBoxItemWithProps> names = new List<PODListBoxItemWithProps>();
            All2_Click(sender, e);
            
            PODListBox control = this.GetCurrentDataList();
            PODListBoxItemWithProps flaw = null;
            Project project = Project;
            //PODTreeNode podNode as analysesTree.Selected;
            DataSource source = project.Sources[0];
            //Disabled the ability for the user to have multiple hit miss responses in one analysis.
            if (control.SelectedRows.Count > 1 && source.AnalysisDataType == AnalysisDataTypeEnum.HitMiss)
            {
                //testing
                MessageBox.Show("Cannot add multiple responses in one Analysis when using HitMiss Data");
                return;
            }
            if (analysesTree.GetNodeCount(false) >= 20)
            {
                MessageBox.Show("The maximum amount of analyses that can fit into one project in PODv4.5.1 is 20.");
                return;
            }
            var indices = control.GetSelectedIndicies();

            if (AvailableFlawsListBox.Rows.Count > 0)
            {
                foreach (int flawIndex in SelectedFlawIndicies)
                {

                    flaw = AvailableFlawsListBox.Rows[flawIndex].Cells[0].Value as PODListBoxItemWithProps;

                    //names.AddRange(from DataGridViewRow index in indices select (PODListBoxItem)control.Rows[index.Index][0]);

                    foreach (int dataIndex in indices)
                    {
                        names.Add(control.Rows[dataIndex].Cells[0].Value as PODListBoxItemWithProps);
                    }

                    //reverse the list because SlectedRows row order is reversed from what we want
                    //names.Reverse();

                    CopyToAnalysisListBox(flaw, names);

                    Invert2_Click(sender, e);

                    if (useAutoName == true)
                        AnalysisNameTBox.Text = EditListBox.AutoName;

                    

                    if (editDialog != null && editDialog.Visible)
                        editDialog.ActiveControl = AnalysisNameTBox;

                    AnalysisNameTBox.SelectAll();

                    if(_inDialog == false)
                        AddNew_Click(sender, e);
                }

                //Clear_Click(sender, e);
            }
        }

        private void CopyToAnalysisListBox(PODListBoxItemWithProps flaw, List<PODListBoxItemWithProps> names)
        {
            var multipleSources = false;
            var flawName  = string.Empty;
            var primarySource = string.Empty;
            var originalFlaw = "";
            var flawDidntMatch = false;
            var indicies = GetIndiciesFromNames(names);

            if (flaw != null)
                flawName = flaw.FlawColumnName;

            if(flawName == string.Empty)
            {
                MessageBox.Show(
                    string.Format(
                        "Need at least one Flaw column."));

                return;
            }

            if (EditListBox.Rows.Count > 0)
            {
                originalFlaw = (EditListBox.Rows[0].Cells[0].Value as PODListBoxItemWithProps).FlawColumnName;

                if (flawName != originalFlaw)
                {                    
                    flawDidntMatch = true;                    
                }
            }

            int index = 0;

            foreach (PODListBoxItemWithProps name in names)
            {
                var cantAdd = false;

                foreach (DataGridViewRow row in EditListBox.Rows)
                {
                    var name2 = (PODListBoxItemWithProps)row.Cells[0].Value;

                    //get the primary source of existing items
                    if (primarySource == string.Empty)
                        primarySource = name2.DataSourceName;

                    if (flawDidntMatch)
                    {
                        cantAdd = true;
                    }

                    if (name.ResponseColumnName == name2.ResponseColumnName)
                    {                        
                        cantAdd = true;
                    }
                    
                    if (name.DataSourceName != primarySource)
                    {
                        cantAdd = true;
                        multipleSources = true;
                    }
                }

                //if there are no existing items then compare with new items
                if (primarySource == string.Empty)
                    primarySource = name.DataSourceName;

                //make sure new item is from primary source
                if(name.DataSourceName != primarySource)
                {
                    cantAdd = true;
                    multipleSources = true;
                }

                //make sure flaw is from the primary source
                if (flaw.DataSourceName != primarySource)
                {
                    cantAdd = true;
                    multipleSources = true;
                }

                if (cantAdd == false)
                {
                    var newItem = new PODListBoxItemWithProps(name.RowColor, ColType.Response, name.ResponseColumnName, name.ResponseOriginalName, name.DataSourceName, name.Unit, name.Min, name.Max, name.Threshold);

                    newItem.FlawColumnName = flaw.FlawColumnName;
                    newItem.FlawOriginalName = flaw.FlawOriginalName;

                    if (indicies[index] < EditListBox.Rows.Count)
                        EditListBox.Rows.Insert(indicies[index], newItem);
                    else
                        EditListBox.Rows.Add(newItem);
                }

                index++;
            }

            while (EditListBox.Columns.Count > 1)
                EditListBox.Columns.RemoveAt(EditListBox.Columns.Count - 1);

            if (multipleSources)
            {
                MessageBox.Show(
                    string.Format(
                        "Mixing data sources in one analysis is not allowed. All data not from {0} will be removed.", primarySource));                
            }
            else if(flawDidntMatch)
            {
                MessageBox.Show("Only add response columns with the flaw column: " + originalFlaw + ".");

                foreach(DataGridViewRow row in AvailableFlawsListBox.Rows)
                {
                    var matches = (row.Cells[0].Value as PODListBoxItemWithProps).FlawColumnName == originalFlaw;

                    row.Selected = matches;                         
                }
            }
        }

        private List<int> GetIndiciesFromNames(List<PODListBoxItemWithProps> names)
        {
            var list = GetCurrentDataList();
            var indices = new List<int>();
            var responses = new List<string>();

            if (names.Count > 0)
            {
                var sourceName = names[0].DataSourceName;
                var source = Project.Sources[sourceName];

                responses = source.ResponseLabels;

                foreach (PODListBoxItemWithProps newItem in names)
                {
                    foreach (string name in responses)
                    {
                        if (newItem.ResponseColumnName == name)
                        {
                            indices.Add(responses.IndexOf(name));
                            break;
                        }
                    }
                }
            }

            return indices;
        }

        private List<PODListBoxItemWithProps> SelectedAvailableNames
        {
            get
            {
                return this.GetCurrentDataList().GetSelectedRowsWithProps();
            }
        }

        private List<int> SelectedAvailableIndicies
        {
            get
            {
                return this.GetCurrentDataList().GetSelectedIndicies();
            }
        }

        private List<PODListBoxItem> SelectedFlawNames
        {
            get
            {
                return AvailableFlawsListBox.GetSelectedRows();
            }
        }

        private List<int> SelectedFlawIndicies
        {
            get
            {
                return AvailableFlawsListBox.GetSelectedIndicies();
            }
        }

        private List<PODListBoxItemWithProps> SelectedAnalysisNames
        {
            get
            {
                return EditListBox.GetSelectedRowsWithProps();
            }
        }

        private void FillChart(string myAnalysisName, List<PODListBoxItemWithProps> myNames, DataPointChart myChart, PODListBoxItemWithProps myFlawItem)
        {
            //int index;
            List<DataSource> sources = new List<DataSource>();

            Project project = Project;

            if (myNames.Count > 0 && AvailableFlawsListBox.SingleSelectedItem != null)
            {
                var dataSourceName = myNames[0].DataSourceName;
                var dataSource = project.Sources[dataSourceName];

                if (myFlawItem == null)
                    myFlawItem = myNames[0];
                
                myChart.DataBindings.Clear();
                myChart.Series.Clear();

                if (myChart.Titles.Count > 0)
                    myChart.Titles[0].Text = myAnalysisName;

                if (myNames.Count > 0)
                {                    
                    foreach (var item in myNames)
                    {
                        myChart.FillFromSource(dataSource, myFlawItem.FlawOriginalName, item.ResponseOriginalName, _colors);
                    }
                }

                var newChart = myChart as DataPointChart;

                if (newChart != null)
                {
                    newChart.CleanUpDataSeries();
                    newChart.ShowLegend();
                    newChart.FixUpLegend(newChart.Series.Count > 1);
                }
            }
        }

        private void FillChart(List<PODListBoxItemWithProps> myNames, DataPointChart myChart, PODListBoxItemWithProps myFlawItem)
        {
            FillChart("Selected Analysis", myNames, myChart, myFlawItem);
        }

        /*private void Items_Dropped(object sender, DroppedEventArgs e)
        {
            if (_useAutoName == true)
                AnalysisNameTBox.Text = EditListBox.AutoName;

            if (editDialog != null && editDialog.Visible)
                editDialog.ActiveControl = AnalysisNameTBox;

            AnalysisNameTBox.SelectAll();
        }*/

        private void Source_SelectedChanged(object sender, EventArgs e)
        {
            if (UpdateLists)
            {
                _lastFlawUsed = "";

                AvailableResponsesListBox.SuspendDrawing();
                AvailableFlawsListBox.SuspendDrawing();
                //availableDataList.BeginUpdate();

                _numberOfDataSources = Project.Sources.Count;
                var randomGen = new Random();
                KnownColor[] names = (KnownColor[])Enum.GetValues(typeof(KnownColor));
                names = CleanupColors(names);

                UpdateLists = false;
                AvailableResponsesListBox.Rows.Clear();
                AvailableFlawsListBox.Rows.Clear();
                

                //availableDataList.Rows.Clear();

                if (Project.Sources.Count > 0)
                {
                    var i = AvailableSourcesListBox.SingleSelectedIndex;

                    if (i >= 0)
                    {
                        var sourceName = Project.Sources[i].SourceName;
                        //KnownColor randomColorName;
                        //Color newColor;

                        //if (_dataSourceColors.ContainsKey(sourceName))
                        //{
                        //    newColor = _dataSourceColors[sourceName];
                        //}
                        //else
                        //{
                        //    randomColorName = names[randomGen.Next(names.Length)];
                        //    newColor = Color.FromKnownColor(randomColorName);
                        //    _dataSourceColors.Add(sourceName, newColor);
                        //}

                        var rowList = new List<DataGridViewRow>();
                        var newRow = AvailableFlawsListBox.CreateNewCloneRow();

                        var infos = Project.Sources[i].ColumnInfos(ColType.Flaw);

                        foreach (var info in infos)
                        {
                            //var flawLabel = String.Format("{0}.{1}", sourceName, label);
                            var newItem = new PODListBoxItemWithProps(Color.Black, ColType.Flaw, info.NewName, info.OriginalName, sourceName, info.Unit, info.Min, info.Max, info.Threshold);
                            newItem.HideExtendedText = true;

                            var row = (DataGridViewRow)newRow.Clone();

                            row.Cells[0].Value = newItem;

                            PODListBox.RemoveExtraColumns(row);

                            rowList.Add(row);
                        }

                        AvailableFlawsListBox.Rows.AddRange(rowList.ToArray());

                        if (AvailableFlawsListBox.Rows.Count > 0)
                        {
                            var flawList = _selectedFlawList[sourceName];

                            if (flawList != null)
                            {
                                if (flawList.Count == 0)
                                {
                                    AvailableFlawsListBox.Rows[0].Selected = true;
                                    flawList.Add(0);
                                }

                                foreach (DataGridViewRow row in AvailableFlawsListBox.Rows)
                                {
                                    row.Selected = flawList.Contains(row.Index);
                                }
                            }
                        }

                        AvailableFlawsListBox.FitAllRows();

                        rowList.Clear();
                        AvailableResponsesListBox.Rows.Clear();

                        infos = Project.Sources[i].ColumnInfos(ColType.Response);

                        foreach (var info in infos)
                        {
                            var listItem = new PODListBoxItemWithProps(Color.Black, ColType.Response, info.NewName, info.OriginalName, sourceName, info.Unit, info.Min, info.Max, info.Threshold);
                            listItem.HideExtendedText = true;

                            var row = (DataGridViewRow)newRow.Clone();

                            row.Cells[0].Value = listItem;

                            PODListBox.RemoveExtraColumns(row);

                            rowList.Add(row);
                        }

                        AvailableResponsesListBox.Rows.AddRange(rowList.ToArray());

                        UpdateLists = true;

                        //select whatever is in the selection history
                        Flaw_SelectedChanged(AvailableFlawsListBox, null);
                        FillChart(SelectedAvailableNames, availableChart, AvailableFlawsListBox.SingleSelectedItemWithProps);

                    }
                }
                
                AvailableFlawsListBox.ResumeDrawing();
                AvailableResponsesListBox.ResumeDrawing();
            }
        }

        public override bool NeedsRefresh(WizardSource mySource)
        {
            //always fix the existing analyses to be updated with the latest column names
            _currentAnalyses = new AnalysisList();
            Project.RequestAnalyses(ref _currentAnalyses);

            foreach (Analysis existingAnalysis in _currentAnalyses)
            {
                var existingName = existingAnalysis.Name;

                foreach (PODTreeNode node in analysesTree.Nodes)
                {
                    if (node.OriginalAnalysisName == existingName)
                    {
                        UpdateNodeWithNewColumnNames(node);
                        break;
                    }
                }
            }

            bool needsRefresh = false;

            foreach(DataSource source in  Project.Sources)
            {
                if(_selectedAvailableList.ContainsKey(source.SourceName) == false)
                {
                    needsRefresh = true;
                    break;
                }

                if (_selectedFlawList.ContainsKey(source.SourceName) == false)
                {
                    needsRefresh = true;
                    break;
                }
            }

            if (needsRefresh)
                return needsRefresh;

            foreach(string key in _selectedAvailableList.Keys)
            {
                var hasKey = false;

                foreach (DataSource source in Project.Sources)
                {
                    if(source.SourceName == key)
                    {
                        hasKey = true;
                        break;
                    }
                }

                if (hasKey == false)
                {
                    needsRefresh = true;
                    break;
                }
            }

            if (needsRefresh)
                return needsRefresh;

            foreach (string key in _selectedFlawList.Keys)
            {
                var hasKey = false;

                foreach (DataSource source in Project.Sources)
                {
                    if (source.SourceName == key)
                    {
                        hasKey = true;
                        break;
                    }
                }

                if (hasKey == false)
                {
                    needsRefresh = true;
                    break;
                }
            }

            return needsRefresh;
        }

        private void UpdateNodeWithNewColumnNames(PODTreeNode node)
        {
            var infos = Project.Infos.GetFromOriginalName(node.DataOriginal);

            UpdateNodeColumnsFromInfos(node, infos, ColType.Flaw);
            UpdateNodeColumnsFromInfos(node, infos, ColType.Response);

            var childListBoxItems = new List<PODListBoxItem>();

            //update the child nodes
            foreach(var child in node.Nodes)
            {
                var childNode = child as PODTreeNode;

                UpdateNodeWithNewColumnNames(childNode);

                var childItem = childNode.CreateListBox();

                childNode.Text = childItem.ToString();

                childListBoxItems.Add(childItem);
            }

            //only update if it is a parent node
            if (node.Nodes.Count > 0)
            {
                //get the new auto-name based on column renames
                node.AnalysisAutoName = PODListBox.CreateAutoName(childListBoxItems);
                //update node's text with latest name
                node.Text = node.NewAnalysisName;
            }

        }

        private void UpdateNodeColumnsFromInfos(PODTreeNode node, SourceInfo infos, ColType type)
        {
            var originals = infos.Originals(type);
            var names = infos.NewNames(type);

            //create pairs to enumerate over using LINQ, C# 4.0 feature, zipping
            var pairs = names.Zip(originals, (n, o) => new { Name = n, Original = o });

            foreach (var pair in pairs)
            {
                if (pair.Original == node.GetOriginalLabel(type))
                    node.Label(type, pair.Name);
            }
        }

        public override void RefreshValues()
        {
            NeedsRefresh(Source);

            if (true)
            {
                analysesTree.SuspendDrawing();
                analysesTree.SuspendLayout();
                AvailableSourcesListBox.SuspendDrawing();
                //availableDataList.BeginUpdate();
                var rowList = new List<DataGridViewRow>();
                var newRow = AvailableFlawsListBox.CreateNewCloneRow();

                var tempIndex = AvailableSourcesListBox.SingleSelectedIndex;

                _numberOfDataSources = Project.Sources.Count;
                var randomGen = new Random();

                UpdateLists = false;
                AvailableSourcesListBox.Rows.Clear();

                if (Project.Sources.Count > 0)
                {
                    for (int i = 0; i < _numberOfDataSources; i++)
                    {
                        var sourceName = Project.Sources[i].SourceName;
                        var sourceNameOriginal = Project.Sources[i].SourceName;

                        var newItem = new PODListBoxItem(Color.Black, string.Empty, string.Empty, string.Empty, string.Empty, sourceName, sourceNameOriginal);

                        if (_selectedAvailableList.ContainsKey(sourceName) == false)
                        {
                            _selectedAvailableList.Add(sourceName, new List<int>());
                        }

                        if (_selectedFlawList.ContainsKey(sourceName) == false)
                        {
                            _selectedFlawList.Add(sourceName, new List<int>());
                        }

                        var row = (DataGridViewRow)newRow.Clone();

                        row.Cells[0].Value = newItem;

                        PODListBox.RemoveExtraColumns(row);

                        rowList.Add(row);
                    }

                    AvailableSourcesListBox.Rows.AddRange(rowList.ToArray());
                }

                if (tempIndex >= 0 && tempIndex < AvailableSourcesListBox.Rows.Count)
                    AvailableSourcesListBox.SingleSelectedIndex = tempIndex;
                else
                    AvailableSourcesListBox.SingleSelectedIndex = 0;

                AvailableSourcesListBox.FitAllRows();

                UpdateLists = true;

                Source_SelectedChanged(AvailableSourcesListBox, null);

                if (analysesTree.SelectedNode == null && analysesTree.Nodes.Count > 0)
                {
                    analysesTree.SelectedNode = analysesTree.Nodes[0];
                    analysesTree.Select();
                }
                
                //availableDataList.EndUpdate();
                AvailableSourcesListBox.ResumeDrawing();

                AddMissingExistingAnalyses();

                RemoveCorruptedAnalyses();

                analysesTree.ResumeLayout();
                analysesTree.ResumeDrawing();
            }
        }

        private void RemoveCorruptedAnalyses()
        {
                        

            foreach(PODTreeNode node in analysesTree.Nodes)
            {
                PODListBoxItemWithProps flawNode = null;
                List<PODListBoxItem> list = new List<PODListBoxItem>();

                var info = GetInfoFromNode(node, ColType.Flaw);

                if (info != null)
                {
                    flawNode = new PODListBoxItemWithProps(Color.Black, ColType.Flaw, info.NewName, info.OriginalName, node.DataOriginal, info.Unit, info.Min, info.Max, info.Threshold);
                    flawNode.HideExtendedText = true;
                }
                else
                {
                    analysesTree.Nodes.Remove(node);
                    break;
                }

                var needToDelete = new List<PODTreeNode>();

                if (node.Nodes.Count > 0)
                {
                    //list.AddRange(from TreeNode child in node.Nodes select child.Text);
                    foreach (TreeNode child in node.Nodes)
                    {
                        var podChild = child as PODTreeNode;

                        if (podChild != null)
                        {
                            info = GetInfoFromNode(podChild, ColType.Response);
                            if (info != null)
                            {
                                var newResponse = new PODListBoxItemWithProps(Color.Black, ColType.Response, info.NewName, info.OriginalName, podChild.DataOriginal, info.Unit, info.Min, info.Max, info.Threshold);
                                newResponse.HideExtendedText = true;
                                list.Add(newResponse);
                            }
                            else
                            {
                                needToDelete.Add(podChild);
                            }
                        }
                    }

                    if (needToDelete.Count > 0)
                    {
                        foreach (PODTreeNode child in needToDelete)
                            node.Nodes.Remove(child);

                        foreach (PODListBoxItemWithProps item in list)
                        {
                            item.FlawColumnName = node.FlawLabel;
                            item.FlawOriginalName = node.FlawOriginal;
                        }


                        if (!node.HasCustomName)
                        {
                            node.AnalysisAutoName = PODListBox.CreateAutoName(list);
                            node.Text = node.AnalysisAutoName;
                        }

                        foreach (PODListBoxItemWithProps item in list)
                        {
                            item.FlawColumnName = "";
                            item.FlawOriginalName = "";
                        }
                    }

                }
                else
                {
                    info = GetInfoFromNode(node, ColType.Response);
                    if (info != null)
                    {
                        var newResponse = new PODListBoxItemWithProps(Color.Black, ColType.Response, info.NewName, info.OriginalName, node.DataOriginal, info.Unit, info.Min, info.Max, info.Threshold);
                        newResponse.HideExtendedText = true;
                        list.Add(newResponse);
                    }
                    else
                    {
                        analysesTree.Nodes.Remove(node);

                        break;
                    }
                }

                if (analysesTree.SelectedNode == node)
                    AnalysisNameTBox.Text = AnalysisNameTextBox.Text = GetNameWithoutSource(node.Text);
                
            }
        }

        private void AddMissingExistingAnalyses()
        {
            PODTreeNode analysisNode = null;
            PODTreeNode existNode = null;
            var exists = false;
            _currentAnalyses = new AnalysisList();

            foreach (PODTreeNode node in analysesTree.Nodes)
            {
                UpdateNodeWithNewColumnNames(node);
            }

            Project.RequestAnalyses(ref _currentAnalyses);

            foreach(Analysis child in _currentAnalyses)
            {            
                var analysisName = child.Name;

                exists = false;

                foreach (PODTreeNode node in analysesTree.Nodes)
                {
                    if (node.OriginalAnalysisName == analysisName)
                    {
                        exists = true;
                        existNode = node;
                        break;
                    }
                }


                if (exists == false && !_removedAnalyses.Contains(child))
                {
                    var flawName = child.Data.ActivatedFlawName;
                    var originalFlawName = child.Data.ActivatedOriginalFlawName;

                    analysisNode = new PODTreeNode(new PODListBoxItem(Color.Black, string.Empty, string.Empty, 
                                                                  flawName, originalFlawName, 
                                                                  child.SourceName, child.SourceName), analysisName, child.UsingCustomName);

                    analysisNode.IsNewAnalysis = false;

                    //UpdateNodeWithNewColumnNames(analysisNode);

                    
                    this.analysesTree.Nodes.Add(analysisNode);                    

                    var responses = child.Data.ActivatedResponseNames;
                    var originals = child.Data.ActivatedOriginalResponseNames;

                    //create pairs to enumerate over using LINQ, C# 4.0 feature, zipping
                    var responsePairs = responses.Zip(originals, (r, o) => new { Response = r, Original = o });

                    foreach (var response in responsePairs)
                    {
                        var podItem = new PODListBoxItem(Color.Black, response.Response, response.Original,
                                                         flawName, originalFlawName,
                                                         child.SourceName, child.SourceName);

                        PODTreeNode node = new PODTreeNode(podItem, podItem.ToString(), false);

                        node.IsHidden = true;

                        UpdateNodeWithNewColumnNames(node);
                        
                        analysisNode.Nodes.Add(node);
                    }

                    //make sure analysis has the most up to date name
                    UpdateNodeWithNewColumnNames(analysisNode);

                    All2_Click(null, null);
                    ClearSelected_Click(null, null);

                    //always remove name after adding the analysis
                    AnalysisNameTBox.Text = string.Empty;
                }
                //else
                //{
                //    analysisNode = existNode;
                //}

                

                
                
            }

            //analysesTree.CollapseAll();

            //if (analysisNode != null)
            //{
            //    analysisNode.Expand();
            //    analysesTree.SelectedNode = analysisNode;
            //}

            //analysesTree.Select();

            analysesTree.EndUpdate();
        }

        private KnownColor[] CleanupColors(KnownColor[] names)
        {
            var removeNames = new List<KnownColor> 
            {
                                      KnownColor.FloralWhite, KnownColor.Gainsboro, KnownColor.GhostWhite,
                                      KnownColor.Honeydew, KnownColor.Ivory, KnownColor.Lavender, KnownColor.LavenderBlush,
                                      KnownColor.LemonChiffon, KnownColor.LightCyan, KnownColor.LightGoldenrodYellow,
                                      KnownColor.LightGray, KnownColor.LightYellow, KnownColor.Linen, KnownColor.MintCream,
                                      KnownColor.MistyRose, KnownColor.Moccasin, KnownColor.NavajoWhite,
                                      KnownColor.OldLace, KnownColor.PaleGoldenrod, KnownColor.PapayaWhip,
                                      KnownColor.PeachPuff, KnownColor.Pink, KnownColor.PowderBlue, KnownColor.SeaShell,
                                      KnownColor.Silver, KnownColor.Snow, KnownColor.Transparent, KnownColor.White,
                                      KnownColor.WhiteSmoke, KnownColor.Gray, KnownColor.GreenYellow, KnownColor.Khaki, KnownColor.LawnGreen, 
                                      KnownColor.LightBlue,KnownColor.LightCoral,KnownColor.LightGreen,KnownColor.LightPink,
                                      KnownColor.LightSalmon,KnownColor.LightSeaGreen,KnownColor.LightSkyBlue,KnownColor.LightSlateGray,
                                      KnownColor.LightSteelBlue,KnownColor.Lime,KnownColor.LimeGreen,KnownColor.MediumAquamarine,
                                      KnownColor.MediumOrchid,KnownColor.MediumPurple,KnownColor.MediumSeaGreen,KnownColor.MediumSlateBlue,
                                      KnownColor.MediumSpringGreen,KnownColor.MediumTurquoise,KnownColor.Orchid,KnownColor.PaleGreen,
                                      KnownColor.PaleTurquoise,KnownColor.PaleVioletRed,KnownColor.Plum, KnownColor.RosyBrown,KnownColor.Salmon,
                                      KnownColor.SandyBrown,KnownColor.SkyBlue,KnownColor.SpringGreen,KnownColor.Tan,KnownColor.Thistle,
                                      KnownColor.Turquoise,KnownColor.Violet,KnownColor.Wheat,KnownColor.Yellow,KnownColor.YellowGreen,
                                      KnownColor.AliceBlue,KnownColor.AntiqueWhite,KnownColor.Aqua,KnownColor.Aquamarine,KnownColor.Azure,
                                      KnownColor.Beige,KnownColor.Bisque,KnownColor.BlanchedAlmond,KnownColor.BurlyWood,KnownColor.Chartreuse,
                                      KnownColor.Cornsilk,KnownColor.Cyan,KnownColor.DarkGray,KnownColor.DarkKhaki,KnownColor.DarkSalmon,
                                      KnownColor.DarkSeaGreen,KnownColor.DarkTurquoise,KnownColor.DeepSkyBlue,
                                      KnownColor.ActiveBorder, KnownColor.ActiveCaption, KnownColor.ActiveCaptionText, KnownColor.AppWorkspace, 
                                      KnownColor.ButtonFace, KnownColor.ButtonHighlight, KnownColor.ButtonShadow, KnownColor.Control, KnownColor.ControlDark, 
                                      KnownColor.ControlDarkDark, KnownColor.ControlLight, KnownColor.ControlLightLight, KnownColor.ControlText, 
                                      KnownColor.Desktop, KnownColor.GradientActiveCaption, KnownColor.GradientInactiveCaption, KnownColor.Highlight,
                                      KnownColor.HighlightText, KnownColor.InactiveBorder, KnownColor.InactiveCaption, KnownColor.InactiveCaptionText, 
                                      KnownColor.Info, KnownColor.InfoText, KnownColor.Menu, KnownColor.MenuBar, KnownColor.MenuHighlight, KnownColor.MenuText, 
                                      KnownColor.ScrollBar, KnownColor.Window, KnownColor.WindowFrame, KnownColor.WindowText
            };

            var nameList = names.ToList();
            var tempNameList = names.ToList();

            foreach (var knownColor in tempNameList.Where(knownColor => removeNames.Contains(knownColor)))
            {
                nameList.Remove(knownColor);
            }

            return nameList.ToArray();
        }

        public override bool Stuck
        {
            get
            {
                return analysesTree.Nodes.Count == 0;
            }
        }

        public override bool CheckStuck()
        {
            if (Stuck)
            {
                MessageBox.Show("You need to create at least one analysis.");
            }

            return Stuck;
        }

        private void AnalysisLayout_Resize(object sender, EventArgs e)
        {
            /*if(listBox2.Height > listBox2.Rows.Count * listBox2.ItemHeight+4)
            {
                analysisChart.IsSquare = true;
                analysisLayoutTable.RowStyles[2].Height = analysisChart.Height;
            }
            else
            {
                analysisChart.IsSquare = false;
                analysisLayoutTable.RowStyles[2].Height = analysisLayoutTable.Height - ((listBox2.Rows.Count * listBox2.ItemHeight+4) + textBox1.Height + 18);
                analysisChart.Height = Convert.ToInt32(analysisLayoutTable.RowStyles[2].Height);
            }*/
        }

        private void Available_SelectedChanged(object sender, EventArgs e)
        {
            if (UpdateLists)
            {
                var item = AvailableSourcesListBox.SingleSelectedItem as PODListBoxItem;

                if (item != null && AvailableResponsesListBox.Rows.Count > 0)
                {
                    var list = _selectedAvailableList[item.DataSourceName];

                    if (list != null)
                    {
                        list.Clear();
                        list.AddRange(SelectedAvailableIndicies);
                    }
                }

                FillChart(SelectedAvailableNames, availableChart, AvailableFlawsListBox.SingleSelectedItemWithProps);
            }
        }

        private void Flaw_SelectedChanged(object sender, EventArgs e)
        {
            if (UpdateLists)
            {
                var item = AvailableSourcesListBox.SingleSelectedItem as PODListBoxItem;

                

                if (item != null && AvailableResponsesListBox.Rows.Count > 0)
                {
                    var flawList = _selectedFlawList[item.DataSourceName];

                    UpdateLists = false;

                    if (SelectedFlawIndicies.Count == 0)
                    {
                        if (flawList.Count == 0)
                        {
                            //alwasy need at least one selected
                            AvailableFlawsListBox.Rows[0].Selected = true;
                        }
                        else
                        {
                            AvailableFlawsListBox.Rows[flawList[0]].Selected = true;
                        }
                    }                    

                    UpdateLists = true;

                    

                    if (flawList != null)
                    {
                        flawList.Clear();
                        flawList.AddRange(SelectedFlawIndicies);
                    }
                }

                FillChart(SelectedAvailableNames, availableChart, AvailableFlawsListBox.SingleSelectedItemWithProps);                

                if (item != null && AvailableResponsesListBox.Rows.Count > 0)
                {
                    var list = _selectedAvailableList[item.DataSourceName];

                    if (list != null && list.Count > 0)
                    {
                        UpdateLists = false;
                        foreach (DataGridViewRow row in AvailableResponsesListBox.Rows)
                        {
                            row.Selected = list.Contains(row.Index);
                        }
                        UpdateLists = true;
                    }
                    else
                    {
                        AvailableResponsesListBox.Rows[0].Selected = true;
                    }
                }
            }
        }

        private void Analysis_SelectedChanged(object sender, EventArgs e)
        {
            //FillChart(SelectedAnalysisNames, analysisChart, null);
        }

        private void AnalysisTree_NodeSelected(object sender, TreeViewEventArgs e)
        {
            PODTreeNode node = analysesTree.SelectedNode as PODTreeNode;

            if (node == null)
                return;

            List<PODListBoxItem> list = new List<PODListBoxItem>();

            if (node != null)
            {
                PODListBoxItemWithProps flawNode = null;

                var info = GetInfoFromNode(node, ColType.Flaw);

                if(info != null)
                {
                    flawNode = new PODListBoxItemWithProps(Color.Black, ColType.Flaw, info.NewName, info.OriginalName, node.DataOriginal, info.Unit, info.Min, info.Max, info.Threshold);
                    flawNode.HideExtendedText = true;
                }
                else
                {
                    analysesTree.Nodes.Remove(node);

                    MessageBox.Show("Node is no longer valid because flaw data is no longer available.", "Analysis Deleted");

                    return;
                }

                var needToDelete = new List<PODTreeNode>();

                if (node.Nodes.Count > 0)
                {
                    //list.AddRange(from TreeNode child in node.Nodes select child.Text);
                    foreach (TreeNode child in node.Nodes)
                    {
                        var podChild = child as PODTreeNode;

                        if (podChild != null)
                        {
                            info = GetInfoFromNode(podChild, ColType.Response);
                            if (info != null)
                            {
                                var newResponse = new PODListBoxItemWithProps(Color.Black, ColType.Response, info.NewName, info.OriginalName, podChild.DataOriginal, info.Unit, info.Min, info.Max, info.Threshold);
                                newResponse.HideExtendedText = true;
                                list.Add(newResponse);
                            }
                            else
                            {                                
                                needToDelete.Add(podChild);
                            }
                        }
                    }

                    if(needToDelete.Count > 0)
                    {
                        foreach (PODTreeNode child in needToDelete)
                            node.Nodes.Remove(child);

                        foreach(PODListBoxItemWithProps item in list)
                        {
                            item.FlawColumnName = node.FlawLabel;
                            item.FlawOriginalName = node.FlawOriginal;
                        }


                        node.AnalysisAutoName = PODListBox.CreateAutoName(list);
                        node.Text = node.AnalysisAutoName;

                        foreach (PODListBoxItemWithProps item in list)
                        {
                            item.FlawColumnName = "";
                            item.FlawOriginalName = "";
                        }
                    }

                }
                else
                {
                    info = GetInfoFromNode(node, ColType.Response);
                    if (info != null)
                    {
                        var newResponse = new PODListBoxItemWithProps(Color.Black, ColType.Response, info.NewName, info.OriginalName, node.DataOriginal, info.Unit, info.Min, info.Max, info.Threshold);
                        newResponse.HideExtendedText = true;
                        list.Add(newResponse);
                    }
                    else
                    {
                        analysesTree.Nodes.Remove(node);

                        MessageBox.Show("Node is no longer valid because response data is no longer available.", "Analysis Deleted");

                        return;
                    }
                }

                var listWithProps = new List<PODListBoxItemWithProps>();

                foreach(PODListBoxItem item in list)
                {
                    listWithProps.Add(item as PODListBoxItemWithProps);
                }

                FillChart(node.Text, listWithProps, analysesChart, flawNode);

                var textNode = GetTopNode(analysesTree, node);

                AnalysisNameTextBox.Text = GetNameWithoutSource(textNode.Text);
                //SelectNameForEdit(AnalysisNameTextBox);
            }

            if (analysesTree.SelectedNode != null && analysesTree.SelectedNode.Parent == null && analysesTree.SelectedNode.Nodes.Count == 0)
            {
                analysesTree.Nodes.Remove(analysesTree.SelectedNode);
            }
        }

        private ColumnInfo GetInfoFromNode(PODTreeNode node, ColType colType)
        {
            DataSource source = Project.Sources[node.DataSource];
            SourceInfo info = Project.Infos.GetFromOriginalName(node.DataOriginal);

            var index = 0;

            if (colType == ColType.Flaw)
                index = info.Originals(colType).IndexOf(node.FlawOriginal);
            else
                index = info.Originals(colType).IndexOf(node.ResponseOriginal);

            ColumnInfo col = null;
            
            if(index >= 0)
                col = info.GetInfos(colType)[index];

            return col;

        }

        private PODTreeNode GetTopNode(MixedCheckBoxesTreeView myListBox, PODTreeNode node)
        {
            if (node == null)
                node = myListBox.Nodes[0] as PODTreeNode;

            while (node.Parent != null)
                node = node.Parent as PODTreeNode;

            return node;
        }

        

        private void Name_KeyDown(object sender, KeyEventArgs e)
        {
            HandleNameTextBoxKeyPresses(AnalysisNameTextBox, analysesTree, e);
        }
        
        private void HandleNameTextBoxKeyPresses(TextBox myTextBox, MixedCheckBoxesTreeView myListBox, KeyEventArgs e)
        {
            if (myListBox.Nodes.Count > 0)
            {
                var node = myListBox.SelectedNode as PODTreeNode;


                node = GetTopNode(myListBox, node);

                var selectedNode = node;

                if (e.KeyCode == Keys.Up)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    
                    myListBox.SelectedNode = node.PrevNode;

                    if (myListBox.SelectedNode == null)
                        myListBox.SelectedNode = myListBox.Nodes[myListBox.Nodes.Count - 1];

                    myTextBox.Text = GetNameWithoutSource(myListBox.SelectedNode.Text);

                    myTextBox.SelectAll();
                }
                else if(e.KeyCode == Keys.Escape)
                {
                    myTextBox.Text = GetNameWithoutSource(myListBox.SelectedNode.Text);

                    myTextBox.SelectAll();
                }
                else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
                {
                    //have to handle this first
                    if (e.KeyCode == Keys.Enter)
                    {
                        //get the current text of the node
                        var oldName = selectedNode.Text;
                        //get the name created from the text box
                        var newName = GetNameFromTextBox(myTextBox);

                        //if the name has actually changed
                        if (newName != oldName)
                        {
                            /*//get the name of the analysis that is going to be renamed
                            var analysisName = selectedNode.AnalysisName;
                            //get a alist of all of the names of existing analyses
                            var names = _currentAnalyses.Names;
                            //get the location of the analysis in the list
                            var analysisIndex = names.IndexOf(analysisName);

                            //if the analysis exists
                            if (analysisIndex >= 0)
                            {
                                //remove analysis name since we are replacing it
                                names.Remove(analysisName);

                                while (names.IndexOf(newName) != -1)
                                {
                                    newName = newName + " Copy";
                                }
                                                                
                                //_currentAnalyses[analysisIndex].Name = newName;
                            }*/

                            foreach(PODTreeNode treeNode in analysesTree.Nodes)
                            {
                                if(treeNode != selectedNode)
                                {
                                    if(treeNode.Text == newName)
                                    {
                                        newName = newName + " Copy";
                                        break;
                                    }
                                }
                            }

                            selectedNode.Text = newName;
                            selectedNode.CustomAnalysisName = newName;
                        }
                    }

                    e.Handled = true;
                    e.SuppressKeyPress = true;

                    myListBox.SelectedNode = node.NextNode;

                    if (myListBox.SelectedNode == null)
                         myListBox.SelectedNode = myListBox.Nodes[0];

                    

                    myTextBox.Text = GetNameWithoutSource(myListBox.SelectedNode.Text);

                    myTextBox.SelectAll();
                }
            }
        }

        private string GetNameFromTextBox(TextBox myTextBox)
        {
            var newName = _lastSourceUsed + ".";

            if (_lastFlawUsed.Length > 0)
                newName = newName + _lastFlawUsed + ".";

            newName = newName + myTextBox.Text;
            return newName;
        }

        private string GetNameWithoutSource(string analysisName)
        {
            foreach(var dataSource in Project.Sources)
            {
                var sourceName = dataSource.SourceName;
                var startIndex = analysisName.IndexOf(sourceName);

                if(startIndex == 0)
                {
                    analysisName = analysisName.Substring(sourceName.Length + 1);
                    _lastSourceUsed = sourceName;

                    if(dataSource.FlawLabels.Count > 1)
                    {
                        foreach (var flawName in dataSource.FlawLabels)
                        {
                            startIndex = analysisName.IndexOf(flawName);

                            if(startIndex == 0)
                            {
                                analysisName = analysisName.Substring(flawName.Length + 1);
                                _lastFlawUsed = flawName;
                            }
                        }
                    }
                    else
                    {
                        _lastFlawUsed = "";
                    }

                    break;
                }
            }

            return analysisName;
        }

        private void SelectNameForEdit(TextBox myNameBox)
        {            
            //select the name so the user can change the name without having to move the mouse
            //to the name box
            myNameBox.Select();
            myNameBox.SelectAll();
        }

        private void TextBox_MouseDown(object sender, MouseEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox != null && _activeBefore != textBox)
            {
                SelectNameForEdit(textBox);
                _activeBefore = textBox;
            }
        }

        private void AnalysisTree_BeforeSelected(object sender, TreeViewCancelEventArgs e)
        {
            /*TreeNode node = analysesTree.SelectedNode;

            if (node != null)
            {
                if (node.Nodes.Count == 0)
                {
                    analysesTree.SelectedNode = node.Parent;

                    return;
                }
            }*/
        }

        public AnalysisList ExistingOrNewAnalysisList
        {
            get
            {
                
                AnalysisList analyses = new AnalysisList();
                Project project = (Project)Source;

                var currentNames = _currentAnalyses.Names;

                //only bother if we are actually have something in the tree
                if (!Stuck)
                {
                    //for each tree in the node
                    foreach (TreeNode node in analysesTree.Nodes)
                    {
                        Analysis analysis;
                        DataSource source;
                        SourceInfo info;
                        PODTreeNode podNode = node as PODTreeNode;
                        string flawName = string.Empty;
                        List<string> responses = new List<string>();

                        foreach (TreeNode child in node.Nodes)
                        {
                            var childPODNode = child as PODTreeNode;

                            if (childPODNode != null)
                            {
                                responses.Add(childPODNode.ResponseLabel);
                                //should always be the same string
                                flawName = podNode.FlawLabel;

                            }
                        }


                        if (podNode.IsNewAnalysis == false && currentNames.Contains(podNode.OriginalAnalysisName))
                        {
                            analysis = _currentAnalyses[currentNames.IndexOf(podNode.OriginalAnalysisName)];

                            //fix the data source first so it matches with the current data source
                            analysis.Data.UpdateSourceFromInfos(Project.Infos.GetFromOriginalName(podNode.DataOriginal));
                            analysis.UpdateRangesFromData();
                        }
                        else
                        {
                            source = project.Sources[podNode.DataSource];
                            info = project.Infos.GetFromOriginalName(source.SourceName);

                            if (source.AnalysisDataType == AnalysisDataTypeEnum.AHat)
                            {
                                analysis = new AHatAnalysis(source, info, flawName, responses);
                            }
                            else
                            {
                                analysis = new HitMissAnalysis(source, info, flawName, responses);
                            }
                        }

                        analysis.AutoOpen = node.Checked;

                        

                        analysis.Name = podNode.NewAnalysisName;
                        analysis.UsingCustomName = podNode.HasCustomName;

                        

                        //analysis.Data.ActivateFlaw(flawName);
                        //analysis.Data.ActivateResponses(responses);
                        analyses.Add(analysis);

                        //it's been added so it is no longer new
                        podNode.IsNewAnalysis = false;
                        podNode.OriginalAnalysisName = analysis.Name;
                    }

                    //clear deletion tracking lists since they won't recoverable any longer
                    _removedAnalysisNodes.Clear();
                    _removedTrackConnectingNode.Clear();
                }
                    
                return analyses;
            }
        }

        private void DrawItem_Handler(object sender, DrawItemEventArgs e)
        {
            var listBox = (PODListBox)sender;
            int maxItem = listBox.Rows.Count - 1;            
            var currentItem = (PODListBoxItem)listBox.Rows[e.Index].Cells[0].Value;

            Bitmap bitmap = new Bitmap(e.Bounds.Width, e.Bounds.Height);
            using (Graphics graph = Graphics.FromImage(bitmap))
            {

                graph.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                Brush backBrush = new SolidBrush(listBox.BackColor);

                graph.FillRectangle(backBrush, 0.0F, 0.0F, e.Bounds.Width, e.Bounds.Height);
                graph.DrawString(currentItem.ResponseColumnName, this.GetCurrentDataList().Font, new SolidBrush(currentItem.RowColor), 0.0F, 0.0F);

                e.Graphics.DrawImageUnscaled(bitmap, e.Bounds);
            }
        }

        private void AnalaysisName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                AddNew_Click(null, null);
            }
        }

        private void AvailableData_DoubleClick(object sender, MouseEventArgs e)
        {
            Copy_Click(null, null);
        }

        private void Analysis_DoubleClick(object sender, MouseEventArgs e)
        {
            ClearSelected_Click(null, null);
        }

        private void availableTableLayout_Paint(object sender, PaintEventArgs e)
        {

        }

        private List<PODListBoxItem> _items = new List<PODListBoxItem>();
        private bool _inDialog;
        private string _lastSourceUsed;
        private string _lastFlawUsed;
        private Control _activeBefore;
        private List<PODTreeNode> _removedAnalysisNodes;
        private PODButton _selectNoneButton;
        private PODButton _invertSelectButton;
        private PODButton _deleteSelectedButton;
        private PODButton _checkAllButton;
        private PODButton _checkNoneButton;
        private PODButton _selectAllButton;







        public bool UpdateLists { get; set; }

        private void AnalysisName_Click(object sender, EventArgs e)
        {

        }

        private void Analysis_MouseEnter(object sender, EventArgs e)
        {
            _activeBefore = ActiveControl;
        }

        private void Analysis_MouseLeave(object sender, EventArgs e)
        {

        }

        private void Control_MouseHover(object sender, EventArgs e)
        {
            var control = sender as Control;

            if (control != null)
            {
                //StepToolTip.Show(StepToolTip.GetToolTip(control), control);
                //StepToolTip.Hide(control);
                StepToolTip.Active = false;
                StepToolTip.Active = true;
            }
        }
    }
}
