using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POD.Docks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.Drawing;
using System.IO;
using System.Reflection;
using CSharpBackendWithR;
namespace POD.Docks
{
    public enum HelpView
    {
        Small,
        Large,
        Dual
    }

    public class DocksManager
    {
        QuickHelpDock _quickHelpDock;
        HelpView _helpView = HelpView.Small;
        private WizardDock previousDoc;
        public QuickHelpDock QuickHelpDock
        {
            get { return _quickHelpDock; }
            set { _quickHelpDock = value; }
        }

        WizardProgressDock _wizardProgDock;
        SnapshotDock _snapshotDock;
        List<WizardDock> _wizardDocks;
        ReportDock _reportDock;
        ProjectDock _projectDock;
        PDFDisplay _show1823ADock;

        public event EventHandler AllWizardsClosed;

        public PDFDisplay Show1823ADock
        {
            get { return _show1823ADock; }
            set { _show1823ADock = value; }
        }
        PDFDisplay _showQuickHelp;

        public PDFDisplay ShowQuickHelp
        {
            get { return _showQuickHelp; }
            set { _showQuickHelp = value; }
        }

        public WizardProgressDock WizardProgressDock
        {
            get { return _wizardProgDock; }
            set { _wizardProgDock = value; }
        }

        HandbookHelpDock _handbookDock;

        public HandbookHelpDock HandbookDock
        {
            get { return _handbookDock; }
            set { _handbookDock = value; }
        }
        DockPanel _panel;
        Dictionary<string, PodDock> _docks;
        DeserializeDockContent _deserializeDockContent;
        private WizardDock _lastActiveDock;
        private WizardDock _lastClosed;

        public ProjectDock ProjectDock
        {
            get { return _projectDock; }
            set { _projectDock = value; }
        }

        public DocksManager()
        {

        }

        public DocksManager(DockPanel myPanel)
        {
            InitializeDocks();

            _panel = myPanel;
            _deserializeDockContent = new DeserializeDockContent(GetContentFromPersistString);

            _panel.ActiveDocumentChanged += myWizard_GotFocused;

            BuildDockList();
        }

        public void RemoveWizardDock(WizardDock myDock)
        {
            if(_docks.ContainsKey(myDock.Label) == true)
            {
                _docks.Remove(myDock.Label);
            }

            _wizardDocks.Remove(myDock);
        }

        private void BuildDockList()
        {
            _docks.Add(_handbookDock.Label, _handbookDock);
            _docks.Add(_wizardProgDock.Label, _wizardProgDock);
            _docks.Add(_projectDock.Label, _projectDock);
            _docks.Add(_snapshotDock.Label, _snapshotDock);
            _docks.Add(_reportDock.Label, _reportDock);
            _docks.Add(_quickHelpDock.Label, _quickHelpDock);
            _docks.Add(_show1823ADock.Label, _show1823ADock);
            _docks.Add(_showQuickHelp.Label, _showQuickHelp);

            foreach (WizardDock dock in _wizardDocks)
            {
                _docks.Add(dock.Label, dock);
            }
        }

        private void InitializeDocks()
        {

            _docks = new Dictionary<string, PodDock>();

            _handbookDock = new HandbookHelpDock();
            _wizardProgDock = new WizardProgressDock();
            _wizardDocks = new List<WizardDock>();
            _projectDock = new ProjectDock();
            _snapshotDock = new SnapshotDock();
            _reportDock = new ReportDock();
            _quickHelpDock = new QuickHelpDock();
            _show1823ADock = new PDFDisplay(_handbookDock.HelpName);
            _showQuickHelp = new PDFDisplay(_quickHelpDock.HelpName);

            _quickHelpDock.LinkWindow.BackColor = Color.FromArgb(215, 237, 255);

            _show1823ADock.PdfFilename = "MIL-HDBK-1823A (POD).pdf";
            _showQuickHelp.PdfFilename = "POD v4 Quick Help.pdf";

            _handbookDock.NeedViewer += GetViewer;
            _quickHelpDock.NeedViewer += GetViewer;
        }

        private void GetViewer(object sender, EventArgs e)
        {
            var rtfDock = sender as RTFViewerDock;
            var position = rtfDock.ListPanel.AutoScrollPosition;

            position.Y = -position.Y;

            if(rtfDock != null)
            {
                if (rtfDock.Label == _handbookDock.Label)
                    rtfDock.PDFViewer = _show1823ADock;
                else
                    rtfDock.PDFViewer = _showQuickHelp;

                if (rtfDock.PDFViewer.IsFloat)
                {
                    if (rtfDock.PDFViewer != _show1823ADock && _show1823ADock.IsHidden)
                        _show1823ADock.Show();
                    else if (rtfDock.PDFViewer != _showQuickHelp && _showQuickHelp.IsHidden)
                        _showQuickHelp.Show();
                }
                
                rtfDock.PDFViewer.Show();

                rtfDock.ListPanel.AutoScrollPosition = position;
            }
        }

        public void HelpView_Switch(object sender, EventArgs e)
        {
            //GotNextHelpView();

            //switch(_helpView)
            //{
            //    case HelpView.Small:
            //        _show1823ADock.DockTo(_projectDock.Pane, DockStyle.Bottom, 0);
            //        _showQuickHelp.DockTo(_projectDock.Pane, DockStyle.Fill, 0);
            //        break;
            //    case HelpView.Large:
            //        _show1823ADock.DockTo(_wizardProgDock.Pane, DockStyle.Bottom, 0);
            //        _showQuickHelp.DockTo(_wizardProgDock.Pane, DockStyle.Fill, 0);
            //        break;
            //    case HelpView.Dual:
            //        //_show1823ADock.DockTo(_quickHelpDock.Pane, DockStyle.Bottom, 0);
            //        //_showQuickHelp.DockTo(_quickHelpDock.Pane, DockStyle.Fill, 0);
            //        //_show1823ADock.Show(_panel, DockState.Float);
            //        //_showQuickHelp.Show(_panel, DockState.Float);

            //        Screen[] screens = Screen.AllScreens;
                    
            //        if(screens.Length > 1)
            //        {
            //            _show1823ADock.FloatAt(new Rectangle(screens[1].WorkingArea.Left, screens[1].WorkingArea.Top,
            //                                                 screens[1].WorkingArea.Width, screens[1].WorkingArea.Height));

            //            if (screens[1].WorkingArea.Width > screens[1].WorkingArea.Height)
            //                _showQuickHelp.DockTo(_show1823ADock.Pane, DockStyle.Left, 0);
            //            else
            //                _showQuickHelp.DockTo(_show1823ADock.Pane, DockStyle.Top, 0);
            //        }

            //        break;
            //}
            
        }

        private void GotNextHelpView()
        {
            if (_helpView == HelpView.Small)
                _helpView = HelpView.Large;
            else if (_helpView == HelpView.Large)
                _helpView = HelpView.Dual;
            else if(_helpView == HelpView.Dual)
                _helpView = HelpView.Small;
        }

        public void ShowDocksDefault()
        {
            _projectDock.Show(_panel, DockState.DockLeft);            
            _handbookDock.Show(_panel, DockState.DockRight);
            _quickHelpDock.Show(_panel, DockState.DockRight);
            _wizardProgDock.Show(_panel, DockState.DockLeft);
            _show1823ADock.Show(_panel, DockState.DockLeft);
            _showQuickHelp.Show(_panel, DockState.DockLeft);

            
            

            //_show1823ADock.Show(_panel, DockState.Float);
            //_showQuickHelp.Show(_panel, DockState.Float);

            _show1823ADock.DockTo(_handbookDock.Pane, DockStyle.Bottom, 0);
            _showQuickHelp.DockTo(_show1823ADock.Pane, DockStyle.Fill, 0);
            _wizardProgDock.DockTo(_handbookDock.Pane, DockStyle.Bottom, 0);

            _quickHelpDock.DockTo(_handbookDock.Pane, DockStyle.Left, 0);

            //_show1823ADock.FitWidth();
            //_showQuickHelp.FitWidth();

            _show1823ADock.SwitchHelpView += Dock_SwitchHelp;
            _showQuickHelp.SwitchHelpView += Dock_SwitchHelp;
            _quickHelpDock.SwitchHelpView += Dock_SwitchHelp;
            _handbookDock.SwitchHelpView += Dock_SwitchHelp;
            _show1823ADock.SwitchBack += SwitchBack_Dock;
            _showQuickHelp.SwitchBack += SwitchBack_Dock;
            _quickHelpDock.SwitchBack += SwitchBack_Dock;
            _handbookDock.SwitchBack += SwitchBack_Dock;

            //so zoom levels stay the same
            _show1823ADock.Sibling = _showQuickHelp;
            _showQuickHelp.Sibling = _show1823ADock;

            _projectDock.CloseButtonVisible = false;
            _handbookDock.CloseButtonVisible = false;
            _quickHelpDock.CloseButtonVisible = false;
            _wizardProgDock.CloseButtonVisible = false;
        }

        private void SwitchBack_Dock(object sender, EventArgs e)
        {
            if(_lastActiveDock != null)
            {
                _lastActiveDock.Show();
            }
        }

        public void AutoDetectHelpPlacement()
        {
            HelpView view = HelpView.Small;

            /*Optimized = true;

            Screen[] screens = Screen.AllScreens;
            

            if (screens[0].Bounds.Width > 1920)
                view = HelpView.Large;

            if (screens.Length > 1)
                view = HelpView.Dual;

            Dock_SwitchHelp(this, view);*/

            Dock_SwitchHelp(this, view);

        }

        public void Dock_SwitchHelp(object sender, HelpView view)
        {
            Screen screen = Screen.FromControl(_projectDock);
            Screen[] screens = Screen.AllScreens;

            if (screens.Length == 1 && view == HelpView.Dual)
                return;

            switch(view)
            {
                case HelpView.Small:
                    _handbookDock.Show(_panel, DockState.DockRight);
                    _quickHelpDock.DockTo(_handbookDock.Pane, DockStyle.Bottom, 0);
                    if (_panel.ActiveDocumentPane != null)
                    {
                        _show1823ADock.DockTo(_panel.ActiveDocumentPane, DockStyle.Fill, -1);//_panel.ActiveDocumentPane.NestedPanesContainer.NestedPanes.Count.Count - 1);
                        _showQuickHelp.DockTo(_panel.ActiveDocumentPane, DockStyle.Fill, -1);//_panel.ActiveDocumentPane.NestedPanesContainer.NestedPanes.Count - 1);
                        _show1823ADock.FitWidth();
                        _showQuickHelp.FitWidth();
                    }
                    _panel.DockRightPortion = 250.0;
                    Optimized = true;
                    break;
                case HelpView.Large:
                    _handbookDock.Show(_panel, DockState.DockRight);                    
                    _show1823ADock.DockTo(_handbookDock.Pane, DockStyle.Bottom, 0);
                    _showQuickHelp.DockTo(_show1823ADock.Pane, DockStyle.Fill, 0);
                    _quickHelpDock.DockTo(_handbookDock.Pane, DockStyle.Left, 0);
                    _show1823ADock.ActualWidth();
                    _showQuickHelp.ActualWidth();
                    _panel.DockRightPortion = 750.0;
                    Optimized = true;
                    break;
                case HelpView.Dual:

                    

                    //_show1823ADock.DockTo(_quickHelpDock.Pane, DockStyle.Bottom, 0);
                    //_showQuickHelp.DockTo(_quickHelpDock.Pane, DockStyle.Fill, 0);
                    //_show1823ADock.Show(_panel, DockState.Float);
                    //_showQuickHelp.Show(_panel, DockState.Float);
                    _quickHelpDock.DockTo(_handbookDock.Pane, DockStyle.Top, 0);

                    

                    var mainScreenIndex = screens.ToList().IndexOf(screen);
                    var secondScreenIndex = 1;

                    if (mainScreenIndex != 0)
                        secondScreenIndex = 0;

                    if (screens.Length > 1)
                    {
                        var mainScreen = screens[mainScreenIndex];
                        var secondaryScreen = screens[secondScreenIndex];
                    
                    
                        _show1823ADock.FloatAt(new Rectangle(secondaryScreen.WorkingArea.Left, secondaryScreen.WorkingArea.Top,
                                                             secondaryScreen.WorkingArea.Width, secondaryScreen.WorkingArea.Height));

                        if (secondaryScreen.WorkingArea.Width > secondaryScreen.WorkingArea.Height)
                            _showQuickHelp.DockTo(_show1823ADock.Pane, DockStyle.Left, 0);
                        else
                            _showQuickHelp.DockTo(_show1823ADock.Pane, DockStyle.Top, 0);
                    }
                    else
                    {
                        var mainScreen = screens[mainScreenIndex];

                        _show1823ADock.FloatAt(new Rectangle(mainScreen.WorkingArea.Left, mainScreen.WorkingArea.Top,
                                                             mainScreen.WorkingArea.Width, mainScreen.WorkingArea.Height));

                        if (mainScreen.WorkingArea.Width > mainScreen.WorkingArea.Height)
                            _showQuickHelp.DockTo(_show1823ADock.Pane, DockStyle.Left, 0);
                        else
                            _showQuickHelp.DockTo(_show1823ADock.Pane, DockStyle.Top, 0);
                    }

                    
                    _show1823ADock.FitWidth();
                    _showQuickHelp.FitWidth();
                    _panel.DockRightPortion = 250.0;
                    Optimized = true;
                    break;
            }

            _handbookDock.FitWidth();
            _quickHelpDock.FitWidth();
        }

        public void SetLeftSideDockHeights()
        {
            //main form must be visible and maximized before this is called
            _wizardProgDock.Pane.SetNestedDockingProportion((double)_wizardProgDock.DefaultHeight / (double)_wizardProgDock.DockPanel.Height);
            //_projectDock.DockPanel.DockBottomPortion = .1;
        }

        private void CloseDocks()
        {
            for (int index = _panel.Contents.Count - 1; index >= 0; index--)
            {
                if (_panel.Contents[index] is IDockContent)
                {
                    IDockContent content = (IDockContent)_panel.Contents[index];
                    content.DockHandler.Close();
                }
            }

            foreach(PodDock dock in _docks.Values)
            {
                dock.Controls.Clear();
                dock.DockHandler.Close();
            }

            _docks.Clear();
            _wizardDocks.Clear();

            _projectDock = null;
            _handbookDock = null;
            _quickHelpDock = null;
            _wizardProgDock = null;
            _reportDock = null;
            _snapshotDock = null;
        }

        public void LoadPositionList(string myFileName)
        {
            string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), myFileName);

            CloseDocks();

            //try
            //{

                if (File.Exists(configFile))
                {
                    _panel.LoadFromXml(configFile, _deserializeDockContent);
                }
            //}
           // catch(Exception exp)
            //{
            //    MessageBox.Show(exp.Message);
            //}

                //CloseWizardDocks();
        }

        public void SavePositionList(string myFileName)
        {
            string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), myFileName);

            _panel.SaveAsXml(configFile);
        }

        public void RemoveWizardDocks(List<WizardDock> myDocks)
        {
            foreach(WizardDock dock in myDocks)
            {
                dock.Close();
                RemoveWizardDock(dock);
            }
        }

        public void Show(WizardDock myWizard, bool activate = true)
        {
            if (myWizard != null)
            {
                if (!activate)
                    myWizard.Visible = false;

                if (!myWizard.HasStep)
                    myWizard.AddSteps();

                myWizard.Show(_panel, DockState.Document);
                
                if(activate)
                    Activate(myWizard);

                if(myWizard.Width > 300)
                    myWizard.Step.FixPanelControlSizes();
            }    
        }

        public bool Add(WizardDock myWizard)
        {
            var alreadyThere = true;

            if (myWizard != null)
            {
                if (_docks.ContainsKey(myWizard.Label) == false)
                {
                    alreadyThere = false;

                    _wizardDocks.Add(myWizard);
                    _docks.Add(myWizard.Label, myWizard);

                    myWizard.FormClosing += Wizard_Closing;
                    myWizard.FormClosed += myWizard_FormClosed;
                    myWizard.DockStateChanged += myWizard_DockStateChanged;
                    myWizard.VisibleChanged += myWizard_VisibleChanged;
                    
                }
            }

            return alreadyThere;
        }

        private void myWizard_VisibleChanged(object sender, EventArgs e)
        {
            
            WizardDock dock = sender as WizardDock;
            
            if (dock != null && dock.IsHidden && HasNoWizardsLeft)
            {
                RaiseAllWizardsClosed(dock);
            }

            if (dock.Visible)
            {
                if (dock == previousDoc)
                {
                    return;
                }
                else if (dock != previousDoc && REngineObject.REngineRunning)
                {
                    MessageBox.Show("Cannot change tabs while analyis is running!");
                    previousDoc.Activate();
                }

                previousDoc = dock;
            }

        }

        private void RaiseAllWizardsClosed(WizardDock dock)
        {
            if(AllWizardsClosed != null)
            {
                AllWizardsClosed.Invoke(dock, null);
            }
        }

        private void myWizard_DockStateChanged(object sender, EventArgs e)
        {
            WizardDock dock = sender as WizardDock;

            if (dock != null && dock.IsHidden && HasNoWizardsLeft)
            {
                _wizardProgDock.Clear();
                _handbookDock.Clear();
                _quickHelpDock.Clear();
            }
        }

        private void myWizard_GotFocused(object sender, EventArgs e)
        {
            var wizardDock = _panel.ActiveDocument as WizardDock;

            if (wizardDock != null)
            {
                //Activate(wizardDock);
                _lastActiveDock = wizardDock;

                _wizardProgDock.Clear();
                _handbookDock.Clear();
                _quickHelpDock.Clear();

                UpdateDocks(wizardDock);
            }
        }

        void myWizard_FormClosed(object sender, FormClosedEventArgs e)
        {
            _lastClosed = sender as WizardDock;

            if (HasNoWizardsLeft)
            {
                _wizardProgDock.Clear();
                _handbookDock.Clear();
                _quickHelpDock.Clear();
            }

            _lastClosed = null;

            //UpdateDocks(_panel.ActiveDocument as WizardDock);
        }

        void Wizard_Closing(object sender, FormClosingEventArgs e)
        {
            //clear controls and clean up references

            ((WizardDock)sender).Controls.Clear();

            _wizardDocks.Remove((WizardDock)sender);
            _docks.Remove(((WizardDock)sender).Label);

            
        }

        private IDockContent GetContentFromPersistString(string persistString)
        {
            PodDock nextDock = null;

            foreach (PodDock dock in _docks.Values)
            {
                if (persistString == dock.Label)
                    nextDock = dock;
            }

            if (nextDock == null)
            {
                switch (persistString)
                {
                    case Globals.HandbookDockLabel:
                        _handbookDock = new HandbookHelpDock();
                        nextDock = _handbookDock;
                        break;
                    case Globals.ReportDockLabel:
                        _reportDock = new ReportDock();
                        nextDock = _reportDock;
                        break;
                    case Globals.ProjectDockLabel:
                        _projectDock = new ProjectDock();
                        nextDock = _projectDock;
                        break;
                    case Globals.ProgressDockLabel:
                        _wizardProgDock = new WizardProgressDock();
                        nextDock = _wizardProgDock;
                        break;
                    case Globals.QuickDockLabel:
                        _quickHelpDock = new QuickHelpDock();
                        nextDock = _quickHelpDock;
                        break;
                    default:
                        nextDock = new WizardDock(persistString);
                        Add((WizardDock)nextDock);
                        return nextDock;
                }

                _docks.Add(nextDock.Label, nextDock);
            }

            return nextDock;
        }

        public void Activate(WizardDock myWizard)
        {
            if (myWizard != null)
            {
                if (myWizard.Step != null)
                {


                    StepArgs args = new StepArgs(myWizard.Step.Index, myWizard.Step.HelpFile, myWizard.ProgressStepListNode);

                    Activate(args);

                    var index = _panel.ActiveDocumentPane.Contents.IndexOf(myWizard);

                    if (_panel.ActiveDocumentPane.Contents.Contains(_show1823ADock))
                    {
                        var index1823 = _panel.ActiveDocumentPane.Contents.IndexOf(_show1823ADock);
                        var indexQuick = _panel.ActiveDocumentPane.Contents.IndexOf(_showQuickHelp);

                        if (index >= index1823)
                            index = index1823 - 1;

                        if (index >= indexQuick)
                            index = indexQuick - 1;

                        if (index < 0)
                            index = 0;
                    }

                    //if (!myWizard.Visible)
                    myWizard.DockTo(_panel.ActiveDocumentPane, DockStyle.Fill, index);

                    //if(myWizard.IsHidden)
                    myWizard.Show();
                    //else if (!myWizard.IsActivated)
                    //    myWizard.ActiveMdiChild
                }
            }
        }

        public void UpdateDocks(WizardDock myWizard)
        {
            if (myWizard != null && myWizard.Step != null)
            {
                StepArgs args = new StepArgs(myWizard.Step.Index, myWizard.Step.HelpFile, myWizard.ProgressStepListNode);

                Activate(args);
            }
        }

        public void Activate(StepArgs myArgs)
        {
            if (myArgs != null)
            {
                _wizardProgDock.UpdateList(myArgs.ListNode);
                _wizardProgDock.UpdateIndex(myArgs.Index);
                _quickHelpDock.SetRTF(myArgs.HelpFile);
                _handbookDock.SetRTF(myArgs.HelpFile);
            }
        }

        //closing docks that shouldn't be opened after loading the docks from file
        private void CloseWizardDocks()
        {
            for (int i = 0; i < _docks.Values.Count; i++ )
            {
                PodDock dock = _docks.Values.ElementAt(i);
                if (_wizardDocks.Contains(dock) == true)
                {
                    dock.Controls.Clear();
                    dock.DockHandler.Close();
                    _docks.Remove(dock.Label);
                    i--;
                }
            }
        }

        public List<WizardDock> WizardDocksList
        {
            get
            {
                return _wizardDocks.ToList();
            }
        }

        public bool Optimized { get; set; }

        public bool HasNoWizardsLeft
        {
            get
            {
                if(_panel.ActiveDocumentPane == null)
                    return false;

                if (_panel.ActiveDocumentPane.ActiveContent == null)
                    return true;

                var index1823 = _panel.ActiveDocumentPane.Contents.IndexOf(_show1823ADock);
                var indexQuick = _panel.ActiveDocumentPane.Contents.IndexOf(_showQuickHelp);

                var totalHidden = 0;

                foreach(DockContent dock in _panel.ActiveDocumentPane.Contents)
                {
                    if (dock.IsHidden || dock == _lastClosed)
                        totalHidden++;
                }

                if (index1823 != -1)
                    totalHidden++;

                if (indexQuick != -1)
                    totalHidden++;

                if (totalHidden == _panel.ActiveDocumentPane.Contents.Count)
                    return true;

                return false;
            }
        }

        public void LoadPDFFiles()
        {
        }

        public void CloseAllWizardDocks()
        {
            if (_panel.ActiveDocumentPane != null)
            {
                var closing = new List<WizardDock>();

                foreach (var content in _panel.ActiveDocumentPane.Contents)
                {
                    var wizard = content as WizardDock;

                    closing.Add(wizard);
                }

                foreach (var dock in closing)
                    dock.Close();
            }
        }
    }
}
