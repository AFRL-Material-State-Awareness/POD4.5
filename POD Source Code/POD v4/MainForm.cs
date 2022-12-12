using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POD.Docks;
using POD.Analyze;
using WeifenLuo.WinFormsUI.Docking;
using POD.Wizards;
using POD.ExcelData;
using POD.Data;
using System.IO;
using System.Windows.Forms.DataVisualization;
using System.Diagnostics;
//used for REngine
using CSharpBackendWithR;

namespace POD
{
    public partial class MainForm : Form
    {
        //Analysis _current;
        //Preferences _preferences;
        DocksManager _dockMgr;
        //FileManager _files;
        WizardController _controller;
        ExcelExport _export;
        PODStatusBar _bar = new PODStatusBar();
        LoadingForm _loader = null;
        private string _lastFile = "";
        InitialSelectionForm initialForm = null;
        bool _showQuickAnalysis = false;
        Analysis _analysis = null;
        WizardDock _dockToShow = null;
        IPy4C py = null;
        //create the R Engine object
        REngineObject REngineInstance = null;

        public MainForm()
        {
            InitializeComponent();

            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        }

        public MainForm(LoadingForm myForm)
        {
            InitializeComponent();

            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            _loader = myForm;

            _loader.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        }

        private void Dock_Paint(object sender, PaintEventArgs e)
        {
            if (_showQuickAnalysis && _dockToShow != null)
            {                
                _dockMgr.Show(_dockToShow);
                _dockToShow.RefreshValues();
                _showQuickAnalysis = false;
            }

            if (_dockMgr.Optimized == false)
            {
                _dockMgr.Dock_SwitchHelp(null, initialForm.SelectedView);
                initialForm.Dispose();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
 	         base.OnPaint(e);
        }
        //initialize loading everything in the Main form
        void MainForm_Load(object loadSender, EventArgs loadE)
        {

            //var pdf = new Controls.PODPdfViewer();

            //pdf.Dispose();

            _controller = new WizardController();           

            initialForm = new InitialSelectionForm();

            BackgroundWorker load_R = new BackgroundWorker();

            load_R.DoWork += Load_RDotNet;

            load_R.RunWorkerAsync();

            _loader.Visible = false;

            initialForm.ShowDialog();
            try
            {
                _loader.Visible = true;
            }
            catch(ObjectDisposedException)
            {
                //if the loader is disposed, there was an error and the application should be terminated
                initialForm.ClosedWithoutSelection = true;
            }

            ShowInTaskbar = true;
            //python engine
            while (py == null && _loader.Visible);
            //REngine (although this probably is quicker than the python engine
            while (REngineInstance == null && _loader.Visible);

            if (initialForm.ClosedWithoutSelection == true)
            {
                _loader.Visible = false;
                this.Close();
                return;
            }
            

            //always after Close() or will crash on close because of PDF viewer
            _dockMgr = new DocksManager(dockPanel1);

            _dockMgr.AllWizardsClosed += _dockMgr_AllWizardsClosed;

            Visible = false;
            _lastFile = "";

            Globals.CleanUpRandomImageFiles();
            
            Cursor.Current = Cursors.WaitCursor;
            Application.DoEvents();

            _dockMgr.ShowDocksDefault();
            AddDockEvents();

            
            _controller.SetPythonEngine(py);
            _controller.SetREngine(REngineInstance);
            _controller.ExportProject += MenuExportExcel_Click;
            _controller.NeedProjectInfo += ProjectName_Needed;
            

            Cursor.Current = Cursors.Default;
                       

            //need actionbar to be able to switch help view
            _controller.NeedSwitchHelpView += _dockMgr.HelpView_Switch;
            
            _export = new ExcelExport();
            
            Project proj = null;

            

            //_dockMgr.Dock_SwitchHelp(null, initialForm.SelectedView);

            if (initialForm.NewQuickAHatAnalysis || initialForm.NewQuickHitMissAnalysis)
            {
                proj = CreateNewQuickProject();

                exportToExcelToolStripMenuItem.Enabled = false;
            }

            if (initialForm.NewProject)
            {
                proj = CreateNewNormalProject();

                exportToExcelToolStripMenuItem.Enabled = true;

            }

            _controller.ProjectUpdated += Project_Updated;
            _controller.AnalysisUpdated += Analysis_Updated;
            _controller.WizardPositionChanged += Wizard_Updated;
            _controller.WizardFinalPositionChanged += WizardFinal_Updated;
            _controller.WizardPositionChanging += Wizard_Changing;
            _controller.NeedOpenAnalysis += OpenAnalysis_Needed;

            this.SizeChanged += MainForm_SizeChanged;
            this.ClientSizeChanged += MainForm_ClientSizeChanged;

            if (initialForm.NewProject)
            {
                ShowNewNormalProject(proj);
            }
            else if (initialForm.NewQuickAHatAnalysis || initialForm.NewQuickHitMissAnalysis)
            {
                ShowNewQuickProject(proj, initialForm.NewQuickHitMissAnalysis);
            }
            else if (initialForm.OpenedProject)
            {
                LoadFile(initialForm.SelectedFile);

                proj = _controller.Project;

                if (proj.AnalysisType != AnalysisTypeEnum.Quick)
                {
                    ShowOldNormalProject(proj);
                }
                else
                {
                    ShowOldQuickProject();
                }
            }

            WindowState = FormWindowState.Maximized;
            Visible = true;

            RedoMenuMRUList();

            _dockMgr.SetLeftSideDockHeights();

            _loader.Close();

            _loader.Dispose();
        }

        void _dockMgr_AllWizardsClosed(object sender, EventArgs e)
        {
            WizardDock dock = sender as WizardDock;

            if(dock != null && dock.IsHidden)
            {
                //ClearDocks();

                //var setupNode = _dockMgr.ProjectDock.Tree.Nodes[0].Nodes[0];

                //Tree_NodeMouseDoubleClick(setupNode, new TreeNodeMouseClickEventArgs(setupNode, MouseButtons.Left, 2, 0, 0));

                _controller.ProjectDock.RefreshValues();
                _controller.ProjectDock.Show();
            }
        }

        private void Load_RDotNet(object sender, DoWorkEventArgs e)
        {
            

            //this.Invoke((MethodInvoker)delegate()
            //{
            try
            {
                Stopwatch newTimer = new Stopwatch();
                newTimer.Start();
                //initialize the iron python engine either by .dll or by importing the .py modules
                py = new IPy4C();
                newTimer.Stop();
                //MessageBox.Show("Total time was:" + newTimer.ElapsedMilliseconds);
                //py = new IPy4C(PyTypeEnum.DLLFiles, false);
                //initialize the REngine Object to create an instance with the R.Net engine
                REngineInstance = new REngineObject();
                //give a notification to the user if python enhancements fail to load
                if (REngineObject.PythonLoaded == false)
                {
                    MessageBox.Show("Python engine could not be loaded! Application will continue without optimization.");
                }
                //forcing assemblies to load while the user is picking out a file to load    
                var wizard = new WizardActionBar();
                var export = new ExcelExport();
                var point = new FixPoint(0, 0, Flag.InBounds);
                var trans = new Transitions.Transition(new Transitions.TransitionType_Acceleration(500));
                var chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
                var report = new Report();

                chart.Dispose();
                wizard.Dispose();
            }
            catch(Exception exp) 
            {
                string myExceptionName = exp.GetType().Name;
                switch (myExceptionName)
                {
                    case "RNotInstalledException":
                        MessageBox.Show("Error, PODv4 application was unable to find the appropriate version of R. Make sure that you have the correct version of R installed",
                        "Error: Unable to initialize R Engine", MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1, (MessageBoxOptions)0x40000);
                        break;
                    case "ArgumentException":
                        MessageBox.Show(exp.Message,
                        "Bad Arguments Passed to R Environment", MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1, (MessageBoxOptions)0x40000);
                        break;
                    case "FailedLoadingLibrariesException":
                        MessageBox.Show(exp.Message + ". " + "Be sure to make sure the rsetup script was run, and there were no errors in installing any libraries.",
                        "Failed To Load R Libraries", MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1, (MessageBoxOptions)0x40000);
                        break;
                    case "FailedLoadingRScriptsException":
                        MessageBox.Show(exp.Message,
                            "Failed to Load R Scripts", MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1, (MessageBoxOptions)0x40000);
                        break;
                    default:
                        MessageBox.Show(exp.Message + ". " + "An unknown error has occurred in loading R.NET. Contact support for assistance",
                        "Uknown Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1, (MessageBoxOptions)0x40000);
                        break;
                }
                //MessageBox.Show(exp.Message);
                //MessageBox.Show("OOPS! Something Went Wrong in initializing the main form!");
                //MessageBox.Show("An unknown error has occurred in loading R.NET. Contact support for assistance");
                this.Invoke((MethodInvoker)delegate()
                {
                    _loader.Close();                   
                });

                return;
            }
        }

        private void OpenAnalysis_Needed(object sender, GetProjectInfoArgs e)
        {
            OpenWizardByName(e.SourceName, true);
        }

        private void ProjectName_Needed(object sender, GetProjectInfoArgs e)
        {
            e.ProjectFileName = RemoveTimeStamp(GetFileNameForSaving("Project Export", false));
        }

        private void ShowOldQuickProject()
        {
            _dockMgr.ProjectDock.Hide();
            _dockMgr.WizardProgressDock.Hide();

            var pair = _controller.CreateWizardDock("QuickAnalysis");

            if (pair != null)
            {
                var aDock = pair.Dock;

                _dockToShow = aDock;
                _dockToShow.CloseButton = false;
                _dockToShow.CloseButtonVisible = false;
                _showQuickAnalysis = true;

                aDock.Activate();
                aDock.Select();
            }
        }

        private void ShowOldNormalProject(Project proj)
        {
            CloseAllWizardDocks();

            WizardDock pDock = _controller.CreateWizardDock(proj.Properties.Name).Dock;
            _dockMgr.Add(pDock);
            _dockMgr.Show(pDock);
            pDock.RefreshValues();
            pDock.Visible = true;

            pDock.Activate();
            pDock.Select();
        }

        private void CloseAllWizardDocks()
        {
            _dockMgr.CloseAllWizardDocks();
        }

        private void ShowNewQuickProject(Project proj, bool isHitMiss)
        {
            _controller.AddProject(proj);

            var pDock = _controller.CreateWizardDock(proj.Properties.Name).Dock;
            _dockMgr.Add(pDock);
            //_dockMgr.Show(pDock);
            //pDock.Visible = true;

            _analysis = null;
            DataSource source = new DataSource("DataSource", "ID", "Flaw", "Response");
            SourceInfo info = new SourceInfo("DataSource", "DataSource", source);

            proj.Sources.Add(source);
            proj.Infos.Add(info);

            if (isHitMiss)
            {
                _analysis = new HitMissAnalysis(source, info, "Flaw", new List<String>(new string[] { "Response" }));

            }
            else
            {
                _analysis = new AHatAnalysis(source, info, "Flaw", new List<String>(new string[] { "Response" }));
                //switch to the most common transform

            }

            _analysis.ObjectType = PODObjectTypeEnum.Analysis;
            _analysis.AnalysisType = AnalysisTypeEnum.Quick;
            _analysis.SkillLevel = SkillLevelEnum.Normal;

            _analysis.Data.UpdateSourceFromInfos(info);
            _analysis.UpdateRangesFromData();

            if (!isHitMiss)
            {
                _analysis.Data.ResponseTransform = TransformTypeEnum.Linear;
                _analysis.InFlawTransform = _analysis.Data.ResponseTransform;
                _analysis.Data.FlawTransform = TransformTypeEnum.Linear;
                _analysis.InResponseTransform = _analysis.Data.FlawTransform;
            }

            _analysis.Name = "QuickAnalysis";

            Project_Updated(_controller, null);

            _dockMgr.ProjectDock.Hide();
            _dockMgr.WizardProgressDock.Hide();
            if (isHitMiss)
            {
                _controller.AddAnalysis(_analysis);

            }
            else
            {
                _controller.AddAnalysis(_analysis, 1);
            }
            var aDock = _controller.CreateWizardDock("QuickAnalysis").Dock;

            _dockToShow = aDock;
            _dockToShow.CloseButton = false;
            _dockToShow.CloseButtonVisible = false;
            _showQuickAnalysis = true;
        }

        private void ShowNewNormalProject(Project proj)
        {
            _controller.AddProject(proj);

            var pDock = _controller.CreateWizardDock(proj.Properties.Name).Dock;
            _dockMgr.Add(pDock);
            _dockMgr.Show(pDock);
            pDock.Step.RefreshValues();
            pDock.Visible = true;

            _dockMgr.ProjectDock.Show();
            _dockMgr.WizardProgressDock.Show();

            pDock.Activate();
            pDock.Select();
        }

        private static Project CreateNewNormalProject()
        {
            var proj = new Project();

            proj.SkillLevel = SkillLevelEnum.Normal;
            proj.AnalysisType = AnalysisTypeEnum.Full;

            proj.Properties.Name = "";
            proj.Properties.Parent = "";
            proj.Properties.Analyst.Name = "";
            proj.Properties.Analyst.Company = "";
            proj.Properties.Customer.Name = "";
            proj.Properties.Customer.Company = "";
            return proj;
        }

        private static Project CreateNewQuickProject()
        {
            var proj = new Project();

            proj.SkillLevel = SkillLevelEnum.Normal;
            proj.AnalysisType = AnalysisTypeEnum.Quick;

            proj.Properties.Name = "Quick Analysis Project";
            proj.Properties.Parent = "";
            proj.Properties.Analyst.Name = "";
            proj.Properties.Analyst.Company = "";
            proj.Properties.Customer.Name = "";
            proj.Properties.Customer.Company = "";
            return proj;
        }

        private void RedoMenuMRUList()
        {
            string mru = Globals.PODv4MRUFile;
            StreamReader sr = null;

            try
            {
                var lines = Globals.GetMRUList(Globals.PODv4MRUFile);

                var parent = recentToolStripSeperator.GetCurrentParent();

                var index = parent.Items.IndexOf(recentToolStripSeperator);
                var indexEnd = parent.Items.IndexOf(exitToolStripSeperator);

                //get rid of previous items in the list
                while(index+1 != indexEnd)
                {
                    parent.Items.RemoveAt(index + 1);
                    indexEnd = parent.Items.IndexOf(exitToolStripSeperator);
                }

                var itemsAdded = 0;

                foreach (string file in lines)
                {
                    ToolStripMenuItem item = new ToolStripMenuItem((itemsAdded + 1).ToString() + ") " + Path.GetFileName(file) + "...");
                    item.Tag = file;
                    item.ToolTipText = file;
                    var args = new EventArgs();

                    item.Click += (sender, e) => item_Click(item, args);

                    if (itemsAdded < 6)
                        parent.Items.Insert(index + 1 + itemsAdded, item);

                    itemsAdded++;
                }

            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message, "Error Loading MRU List");
            }
            finally
            {
                if(sr != null)
                    sr.Close();
            }
        }

        void item_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;

            if (item != null)
            {
                var name = item.Tag.ToString();

                if(AskSaveFileBeforeOpenNew())
                {
                    LoadFile(name);
                    UpdateMRUList(name);

                    RedoMenuMRUList();
                }

                
            }
        }

        private void FileMenu_Click(object sender, EventArgs e)
        {
            //RedoMenuMRUList();
        }

        private void FileMenu_MouseDown(object sender, MouseEventArgs e)
        {
            
        }

        void MainForm_ClientSizeChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            //MessageBox.Show("Client Resize");
        }

        void MainForm_SizeChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            //MessageBox.Show("Form Resize");
        }

        //void ErrorWriter_StringWritten(object sender, MyEvtArgs<string> e)
        //{
        //    MessageBox.Show(e.Value);
        //}

        //void OutputWriter_StringWritten(object sender, MyEvtArgs<string> e)
        //{
        //    MessageBox.Show(e.Value);
        //}

        private void AddDockEvents()
        {
            _dockMgr.ProjectDock.Tree.NodeMouseDoubleClick -= Tree_NodeMouseDoubleClick;
            _dockMgr.ProjectDock.Tree.NodeMouseDoubleClick += Tree_NodeMouseDoubleClick;
            _dockMgr.WizardProgressDock.Tree.AfterSelect -= Tree_AfterSelect;
            _dockMgr.WizardProgressDock.Tree.AfterSelect += Tree_AfterSelect;
        }

        private void Wizard_Changing(object sender, StepArgs e)
        {
            _dockMgr.Activate(e);
        }

        void Tree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeView tree = (TreeView)sender;
            TreeNode node = null;
            WizardDock wizardDock = dockPanel1.ActiveDocument as WizardDock;

            if (wizardDock != null)
            {

                if ((e.Action == TreeViewAction.ByMouse || e.Action == TreeViewAction.ByKeyboard) &&
                    wizardDock != null && wizardDock.Step != null)
                {

                    if (e != null)
                        node = e.Node;

                    if (node != null)
                    {
                        _controller.ChangeStep(wizardDock, node.Index);
                    }

                    _dockMgr.WizardProgressDock.UpdateIndex(wizardDock.Step.Index);

                    _dockMgr.UpdateDocks(wizardDock);

                }
            }
        }
        
        private void WizardDock_Focused(object sender, EventArgs e)
        {
            WizardDock wizardDock = dockPanel1.ActiveDocument as WizardDock;

            if (wizardDock != null)
            {
                if (wizardDock == _controller.ProjectDock)
                {
                    wizardDock.Step.RefreshOverview();
                    wizardDock.DockTo(wizardDock.Pane, DockStyle.Fill, 0);
                    //wizardDock.BringMovePanelToFront();
                    //wizardDock.Step.RefreshValues();
                    //wizardDock.SendMovePanelToBack();
                }

                //_dockMgr.Show(wizardDock);
                _dockMgr.UpdateDocks(wizardDock);

                
            }
        }

        private void WizardDock_Closed(object sender, EventArgs e)
        {
            
        }

        public void ClearDocks()
        {
            //_dockMgr.WizardProgressDock.Clear();
            //_dockMgr.QuickHelpDock.Clear();
            //_dockMgr.HandbookDock.Clear();
        }
        
        private void Wizard_Updated(object sender, StepArgs e)
        {
            WizardDock wizardDock = (WizardDock)dockPanel1.ActiveDocument;

            if (wizardDock != null) 
            {
                //_dockMgr.UpdateDocks(wizardDock);
            }
        }

        private void WizardFinal_Updated(object sender, StepArgs e)
        {
            WizardDock wizardDock = (WizardDock)dockPanel1.ActiveDocument;

            if (wizardDock != null)
            {
                _dockMgr.UpdateDocks(wizardDock);
            }
        }

        private void Analysis_Updated(object sender, EventArgs e)
        {
            //insert code to modify main forms in reaction to analysis being modified

            //last node on tree is still bold after finished button pressed
            _dockMgr.WizardProgressDock.ClearNodes();
        }

        void Tree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (REngineObject.REngineRunning)
            {
                MessageBox.Show("Cannot open another tab while current analysis is running");
                return;
            }
            string nodeLabel = e.Node.Text;
            
            string wizardLabel = string.Empty;

            if (e.Node.Parent != null)
            {
                string parentLabel = e.Node.Parent.Text;                

                if (parentLabel == "Analyses")
                {
                    wizardLabel = nodeLabel;
                }
                else if (parentLabel == _controller.Project.Name || parentLabel == Globals.UndefinedProjectName)
                {
                    if(nodeLabel == "Setup...")
                    {
                        wizardLabel = parentLabel;                        
                    }
                }

                OpenWizardByName(wizardLabel);
                return;
            }
        }

        private void OpenWizardByName(string wizardLabel, bool activate = true)
        {
            WizardDock dock = null;
            if (wizardLabel != string.Empty)
            {
                var activeDoc = dockPanel1.ActiveDocument as WizardDock;

                if (activeDoc != null)
                {
                    if (activeDoc.Label == wizardLabel)
                    {
                        _dockMgr.Activate(activeDoc);
                        //activeDoc.Visible = true;
                        return;
                    }
                        
                }

                var pair = _controller.CreateWizardDock(wizardLabel);
                dock = pair.Dock;

                if (dock.Step == null)
                    dock.AddSteps();

                var alreadyThere = _dockMgr.Add(dock);

                if (!alreadyThere)
                {
                    _dockMgr.Show(dock, activate);
                    dock.RefreshValues();

                    if (!activate)
                        _dockMgr.Activate(activeDoc);
                }
                else
                {
                    _dockMgr.Activate(dock);
                }
            }

            return;
        }
        private void OpenAnalysesAll()
        {

        }

        private void Project_Updated(object sender, EventArgs e)
        {

            _dockMgr.ProjectDock.Tree.BeginUpdate();

            WizardController control = (WizardController)sender;

            TreeNode top = _dockMgr.ProjectDock.Tree.Nodes[0];

            if (control.Project.Name.Length > 0)
                top.Text = control.Project.Name;
            else
                top.Text = Globals.UndefinedProjectName;

            top.Nodes[1].Nodes.Clear();

            //if (control.AnalysisNames != null)
            //{
            foreach (string name in control.AnalysisNames)
            {
                top.Nodes[1].Nodes.Add(name);
            }

            dockPanel1.Dock = DockStyle.Fill;

            //last node on tree is still bold after finished button pressed
            _dockMgr.WizardProgressDock.ClearNodes();

            top.Expand();
            top.Nodes[1].Expand();

            _dockMgr.ProjectDock.Tree.EndUpdate();

            foreach (TreeNode node in top.Nodes[1].Nodes)
            {
                if (control.AutoOpen(node.Text) == true)
                {
                    Tree_NodeMouseDoubleClick(node, new TreeNodeMouseClickEventArgs(node, MouseButtons.Left, 2, 0, 0));
                    break;
                }
            }

            foreach (TreeNode node in top.Nodes[1].Nodes)
            {
                control.RemoveAutoOpen(node.Text);
            }


            //if (top.Nodes[1].Nodes.Count > 0)
            //{
            //    var node = top.Nodes[1].Nodes[0];

            //    if (control.AutoOpen(node.Text) == true)
            //    {
            //        Tree_NodeMouseDoubleClick(node, new TreeNodeMouseClickEventArgs(node, MouseButtons.Left, 2, 0, 0));
            //    }
            //}



            //if (control.Project.Name.Length > 0)
            //    OpenWizardByName(control.Project.Name);
        }

        private void milHandbook1823aToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void MainForm_Closing(object sender, FormClosingEventArgs e)
        {
            Globals.CleanUpRandomImageFiles();

            if (initialForm != null && !initialForm.ClosedWithoutSelection)
            {

                var result = MessageBox.Show("Would you like to save your file before closing?", "Save " + _controller.Project.Name, MessageBoxButtons.YesNoCancel);

                if (result == DialogResult.Yes)
                {
                    if (_lastFile.Length > 0)
                    {
                        SaveFile(_lastFile);
                    }
                    else
                    {
                        var saveAsResult = SaveFileAs();

                        if (saveAsResult == System.Windows.Forms.DialogResult.Cancel)
                            e.Cancel = true;
                    }
                }

                if (result == DialogResult.Yes || result == DialogResult.No)
                {
                    if (_controller != null)
                        _controller.ClosePyEngine();
                }
                else
                {
                    e.Cancel = true;
                }

            }
            
        }

        private void Control_Removed(object sender, ControlEventArgs e)
        {
            
        }

        private void Content_Removed(object sender, DockContentEventArgs e)
        {
            ClearDocks();
        }

        private void MenuOpenFile_Click(object sender, EventArgs e)
        {
            //_dockMgr.RemoveWizardDocks(_controller.AllWizardDocks);
            if (AskSaveFileBeforeOpenNew())
            {
                OpenFileDialog dialog = new OpenFileDialog();

                dialog.Filter = "POD Projects (*.pod)|*.pod";
                dialog.Multiselect = false;
                dialog.FileName = Path.GetFileName(_lastFile);

                DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    this.Cursor = Cursors.WaitCursor;

                    string name = dialog.FileName;

                    LoadFile(name);

                    this.Cursor = Cursors.Default;
                }
            }            
        }

        private void LoadFile(string name)
        {
            this.Cursor = Cursors.WaitCursor;

            string shortName = name.Remove(name.Length - 4);
            
            var whatToClose = new List<WizardDock>();

            foreach (var document in dockPanel1.Documents)
            {
                var dock = document as WizardDock;

                if (dock != null)
                {
                    whatToClose.Add(dock);
                }
            }

            for (int i = 0; i < whatToClose.Count; i++)
            {
                whatToClose[i].Close();
            }

            AddDockEvents();
            ClearDocks();

            _controller.ClearEverything();

            _controller.Deserialize(name, _dockMgr.WizardDocksList);

            if (_dockMgr.WizardDocksList.Count > 0)
            {
                WizardDock dock = _dockMgr.WizardDocksList.Last();
                _dockMgr.Activate(dock.CreateStepArgs());
            }

            Project_Updated(_controller, null);

            _lastFile = name;

            UpdateMRUList(name);

            Project proj = _controller.Project;

            if(proj.AnalysisType == AnalysisTypeEnum.Quick)
            {
                if(Visible)
                {
                    _dockMgr.ProjectDock.Hide();
                    _dockMgr.WizardProgressDock.Hide();

                    WizardDock aDock = _controller.CreateWizardDock("QuickAnalysis").Dock;

                    aDock.CloseButton = false;
                    aDock.CloseButtonVisible = false;

                    _dockMgr.Add(aDock);
                    _dockMgr.Show(aDock);
                    aDock.Visible = true;
                    aDock.RefreshValues();                    
                    aDock.Activate();
                    aDock.Select();
                }

                exportToExcelToolStripMenuItem.Enabled = false;
            }
            else
            {
                if (Visible)
                {
                    _dockMgr.ProjectDock.Show();
                    _dockMgr.WizardProgressDock.Show();

                    WizardDock pDock = _controller.CreateWizardDock(proj.Properties.Name).Dock;
                    _dockMgr.Add(pDock);
                    _dockMgr.Show(pDock);
                    pDock.Visible = true;
                    pDock.RefreshValues();
                    pDock.Activate();
                    pDock.Select();
                }

                exportToExcelToolStripMenuItem.Enabled = true;
            }

            this.Cursor = Cursors.Default;
        }

        private void UpdateMRUList(string name)
        {
            Globals.UpdateMRUList(name, Globals.PODv4MRUFile);

            RedoMenuMRUList();
        }

        private void MenuSaveFile_Click(object sender, EventArgs e)
        {
            if (_lastFile.Length > 0)
            {
                SaveFile(_lastFile);
            }
            else
            {
                SaveFileAs();
            }
        }

        private void MenuSaveAsFile_Click(object sender, EventArgs e)
        {
            SaveFileAs();
        }

        private DialogResult SaveFileAsQuick()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            bool saved = false;

            dialog.Filter = "POD Projects (*.pod)|*.pod";
            dialog.AddExtension = false;
            dialog.DefaultExt = "pod";
            dialog.Title = "Save POD v4 File";
            dialog.OverwritePrompt = false;

            dialog.FileName = GetFileNameForSaving("Project", false);

            DialogResult result = dialog.ShowDialog();


            if (result == System.Windows.Forms.DialogResult.OK)
            {
                SaveFile(dialog.FileName);
                _lastFile = dialog.FileName;
                saved = true;
            }

            return result;
        }

        private DialogResult SaveFileAs()
        {
            SaveFileDialog dialog = new SaveFileDialog();

            dialog.Filter = "POD Projects (*.pod)|*.pod";
            dialog.AddExtension = false;
            dialog.DefaultExt = "pod";
            dialog.Title = "Save POD v4 File";

            dialog.FileName = GetFileNameForSaving("Project", true);

            DialogResult result = dialog.ShowDialog();


            if (result == System.Windows.Forms.DialogResult.OK)
            {
                SaveFile(dialog.FileName);
                _lastFile = dialog.FileName;
            }

            return result;
        }

        private void SaveFile(string myFileName)
        {
            if (myFileName.Length == 0)
            {
                SaveFileAs();
                return;
            }

            string name = myFileName;
            string shortName = Path.GetFileNameWithoutExtension(name);

            //_dockMgr.SavePositionList(shortName + ".pdl");

            var writeResult = false;

            try
            {
                writeResult = _controller.Serialize(name);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
                return;
            }

            if(writeResult)
                UpdateMRUList(name);
        }

        private void MenuExportExcel_Click(object sender, EventArgs e)
        {
            var defaultName = "Excel Export";
            var shouldSave = false;
            var name = GetFileNameForSaving(defaultName, true);

            name = _export.AskUserToSave(name, out shouldSave);
            if(name == null && !shouldSave)
            {
                MessageBox.Show("Cannot export to excel because file is being used by another process! If you have the file open in excel, " +
                    "close excel and retry.");
                return;
            }
            if (shouldSave)
            {
                this.Cursor = Cursors.WaitCursor;
                //keep the program from crashing
                try
                {
                    _controller.WriteToExcel(_export);
                }
                catch (InvalidOperationException operationException)
                {
                    MessageBox.Show("Unable to export Data to excel ERROR. " + '\n'+"Make sure that all Analyses are complete before exporting: " + '\n' + operationException);
                    this.Cursor = Cursors.Default;
                    return;
                }
                catch(Exception noExcelForYou)
                {
                    MessageBox.Show("Unable to export Data to excel ERROR: "+'\n'+ noExcelForYou);
                    this.Cursor = Cursors.Default;
                    return;
                }
                
                _export.SaveToFile(name);
                this.Cursor = Cursors.Default;
            }
        }

        private string GetFileNameForSaving(string defaultName, bool forceNewTimeStamp)
        {
            var proj = _controller.Project;
            var name = Path.GetFileNameWithoutExtension(_lastFile);
            var noTimeStamp = false;
            var original = name;

            name = RemoveTimeStamp(name);

            //if there was a timestamp, no flag to force new timestamp, and name is valid
            //return original name with original timestamp
            if (original != name && !forceNewTimeStamp && name != null && name.Length > 0)
                return original;

            if (name.Length > 0 && original == name)
                noTimeStamp = true;

            

            if (name == null || name.Length == 0)
            {
                if (proj.AnalysisType == AnalysisTypeEnum.Quick)
                {
                    WizardDock aDock = _controller.CreateWizardDock("QuickAnalysis").Dock;

                    name = aDock.Step.Source.GenerateFileName();
                }
                else
                {
                    name = proj.GenerateFileName();
                }
            }

            if (name == null || name.Length == 0)
                name = defaultName;

            if(noTimeStamp)
                return name;
            else
                return name + Globals.FileTimeStamp;
        }

        /// <summary>
        /// Removes the time stamp from a file name so a new tie stamp can be used.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string RemoveTimeStamp(string name)
        {
            var original = name;

            if(name.EndsWith(")"))
            {
                var last = name.LastIndexOf("(");

                if (last != -1)
                {
                    var date = name.Substring(last + 1);

                    if (date.Length > 2)
                    {
                        date = date.Substring(0, date.Length - 1);

                        if (date.LastIndexOf("-") == 8 && date.Length == 15)
                        {
                            var dateString = date.Substring(0, 8);
                            var timeString = date.Substring(9);
                            var dateValue = 1;
                            var timeValue = 1;

                            if (Int32.TryParse(dateString, out dateValue) && Int32.TryParse(timeString, out timeValue))
                            {
                                name = original.Substring(0, last).Trim();
                                return name;
                            }
                        }
                    }
                }
            }

            return original;
        }


        private void Exit_Application(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ProjectNew_Menu(object sender, EventArgs e)
        {
            if (AskSaveFileBeforeOpenNew())
            {
                var proj = CreateNewNormalProject();

                _controller.ClearEverything();

                ShowNewNormalProject(proj);

                _lastFile = "";
            }
        }

        private bool AskSaveFileBeforeOpenNew()
        {
            var result = MessageBox.Show("Would you like to save your file before continuing?", "Save " + _controller.Project.Name, MessageBoxButtons.YesNoCancel);

            if (result == DialogResult.Yes)
            {
                SaveFile(_lastFile);

                return true;
            }
            else if(result == DialogResult.No)
            {
                ///_analysis
                return true;
            }
            
            return false;            
        }

        private void aHat_New(object sender, EventArgs e)
        {
            if (AskSaveFileBeforeOpenNew())
            {
                var proj = CreateNewQuickProject();

                _controller.ClearEverything();

                ShowNewQuickProject(proj, false);

                ShowOldQuickProject();

                _lastFile = "";

                _dockMgr.Show(_dockToShow);
                _dockToShow.RefreshValues();
                _showQuickAnalysis = false;
            }
        }

        private void hitMiss_New(object sender, EventArgs e)
        {
            if (AskSaveFileBeforeOpenNew())
            {
                var proj = CreateNewQuickProject();

                _controller.ClearEverything();

                ShowNewQuickProject(proj, true);

                ShowOldQuickProject();

                _lastFile = "";

                _dockMgr.Show(_dockToShow);
                _dockToShow.RefreshValues();
                _showQuickAnalysis = false;
            }
        }

        private void About_Show(object sender, EventArgs e)
        {
            var form = new About();

            form.ShowDialog();
        }

        private void Mil1823A_Show(object sender, EventArgs e)
        {
            OpenExternalPDF("MIL-HDBK-1823A (POD).pdf");
        }

        

        private void PODManual_Show(object sender, EventArgs e)
        {
            OpenExternalPDF("POD v4 Users Manual UDRI.pdf");
        }

        private void QuickHelp_Show(object sender, EventArgs e)
        {
            OpenExternalPDF("POD v4 Quick Help.pdf");
        }

        private void PODv3_Show(object sender, EventArgs e)
        {
            OpenExternalPDF("POD v3 Report.pdf");
        }

        private void OpenExternalPDF(string path)
        {
            Process.Start(path);
        }

        private void licenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LicensingInfo form = new LicensingInfo();
            form.ShowDialog();
        }
    }
}
