using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POD.Controls;
using System.Drawing;
using POD.Analyze;
using System.Threading;

namespace POD.Wizards
{
    /// <summary>
    /// This class handles managing the button layouts, reacting to button clicks and managing the context menu
    /// found in the panel. This contains general code that applies to all Wziard steps. Code specific to a particular
    /// step should go to the respective child class.
    /// </summary>
    public partial class WizardActionBar : WizardUI
    {
        public EventHandler NeedSwitchHelpView;
        public ToolStripControlHost Actions { get; set; }
        static ToolStripControlHost _currentActions = null;

        static ToolStripControlHost _navigateButtons = null;

        public static ToolStripControlHost NavigateButtons
        {
            get { return WizardActionBar._navigateButtons; }
            set { WizardActionBar._navigateButtons = value; }
        }

        public static ToolStripControlHost CurrentActions
        {
            get { return WizardActionBar._currentActions; }
            set { WizardActionBar._currentActions = value; }
        }

        public override bool SendKeys(Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.F))
            {
                finishButton.PerformClick();
                return true;
            }
            if (keyData == (Keys.Control | Keys.P))
            {
                prevButton.PerformClick();
                return true;
            }
            if (keyData == (Keys.Control | Keys.N))
            {
                nextButton.PerformClick();
                return true;
            }
            if (keyData == (Keys.Control | Keys.Alt | Keys.D))
            {
                snapShotButton.PerformClick();
                return true;
            }
            if (keyData == (Keys.Control | Keys.OemCloseBrackets))
            {
                switchViewButton.PerformClick();
                return true;
            }

            return false;
        }

        protected bool ResizeCharts(Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.D1 | Keys.Shift))
            {
                MyPanel.SetSideChartSize(1);
                return true;
            }
            if (keyData == (Keys.Control | Keys.D2 | Keys.Shift))
            {
                MyPanel.SetSideChartSize(2);
                return true;
            }
            if (keyData == (Keys.Control | Keys.D3 | Keys.Shift))
            {
                MyPanel.SetSideChartSize(3);
                return true;
            }
            if (keyData == (Keys.Control | Keys.D4 | Keys.Shift))
            {
                MyPanel.SetSideChartSize(4);
                return true;
            }
            if (keyData == (Keys.Control | Keys.D5 | Keys.Shift))
            {
                MyPanel.SetSideChartSize(5);
                return true;
            }
            if (keyData == (Keys.Control | Keys.D6 | Keys.Shift))
            {
                MyPanel.SetSideChartSize(6);
                return true;
            }

            return false;
        }

        public bool ScrollToChart(Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.D1 | Keys.Alt | Keys.Shift))
            {
                return MyPanel.ScrollToChart(0);
            }
            if (keyData == (Keys.Control | Keys.D2 | Keys.Alt | Keys.Shift))
            {
                return MyPanel.ScrollToChart(1);
            }
            if (keyData == (Keys.Control | Keys.D3 | Keys.Alt | Keys.Shift))
            {
                return MyPanel.ScrollToChart(2);
            }
            if (keyData == (Keys.Control | Keys.D4 | Keys.Alt | Keys.Shift))
            {
                return MyPanel.ScrollToChart(3);
            }
            if (keyData == (Keys.Control | Keys.D5 | Keys.Alt | Keys.Shift))
            {
                return MyPanel.ScrollToChart(4);
            }
            if (keyData == (Keys.Control | Keys.D6 | Keys.Alt | Keys.Shift))
            {
                return MyPanel.ScrollToChart(5);
            }

            return false;
        }


        protected List<List<ToolStripItem>> _menuItems;        
        protected PODButton finishButton;
        protected PODButton nextButton;
        protected PODButton prevButton;
        protected PODButton resetButton;
        protected PODButton cancelButton;
        protected PODButton snapShotButton;
        protected PODButton switchViewButton;
        // used to solve all models at once in either hit miss or signal response
        //protected PODButton _solveAllModelsButton;
        //protected ToolTip _solveAllModelsToolTip;
        //private PODBooleanButton _snapToGridButton;

        private WizardPanel _panel;
        private WizardTitle _title;
        protected ButtonHandlerList _leftButtons;
        protected ButtonHandlerList _rightButtons;
        private ContextMenuStrip leftOverflowMenu;        
        private ContextMenuStrip rightOverflowMenu;
        private string _lastActionClicked = "";

        public string LastActionClicked
        {
            get { return _lastActionClicked; }
            set { _lastActionClicked = value; }
        }

        public string LastNavigateClicked
        {
            get { return _panel.GetLastClick(); }
            set { _panel.SetLastClicked(value); }
        }

        public WizardActionBar()
        {
            Initialize();

            ActionIcons = new ActionIconsList().List;
        }
             
        public WizardActionBar(PODToolTip tooltip)
        {
            StepToolTip = new PODToolTip();

            Initialize();

            ActionIcons = new ActionIconsList().List;
        }

        private void Initialize()
        {
            InitializeComponent();

            if (StepToolTip == null)
                StepToolTip = new PODToolTip();

            BackColor = Color.FromArgb(Globals.ActionBarColor);

            //int stdWidth = Globals.StdWidth(this);
            //int stdHeight = Globals.StdWidth(this);
            
            //Width = stdWidth;
            //Height = stdHeight;
           // MaximumSize = new Size(0, stdHeight);



            _rightButtons = new ButtonHandlerList();
            _leftButtons = new ButtonHandlerList();

            switchViewButton = new PODButton("Switch Help View", "Switch help windows between pre-defined views." + Environment.NewLine + "(small screen, large screen, dual screen) (Ctrl + ])", StepToolTip);
            snapShotButton = new PODButton("Duplicate", "Duplicate analysis and add to project. (Ctrl + Alt + D)", StepToolTip);
            cancelButton = new PODButton("Cancel", "Cancel all changes.", StepToolTip);
            resetButton = new PODButton("Reset", "Reset changes on the current step.", StepToolTip);
            prevButton = new PODButton("Previous", "Go to the previous step. (Ctrl + P)", StepToolTip);
            nextButton = new PODButton("Next", "Go to the next step. (Ctrl + N)", StepToolTip);
            finishButton = new PODButton("Finish", "Auto-complete remaining steps. (Ctrl + F)", StepToolTip);

            //snapShotButton.Enabled = false;

            _menuItems = new List<List<ToolStripItem>>();

            AddRightButtons();

            AddIconsToButtons();
        }


        protected void AddIconsToButtons()
        {
            foreach(Control control in rightFlowPanel.Controls)
            {
                AddIconToButton(control);
            }

            foreach (Control control in leftFlowPanel.Controls)
            {
                AddIconToButton(control);
            }
        }

        private void AddIconToButton(Control control)
        {
            PODButton button = control as PODButton;

            if (button != null)
            {
                var key = button.Name + ".png";

                if (ActionIcons.Images.ContainsKey(key) && button.Image == null)
                {
                    button.Image = ActionIcons.Images[key];

                    button.ImageAlign = ContentAlignment.MiddleCenter;
                    button.TextAlign = ContentAlignment.BottomCenter;
                    button.TextImageRelation = TextImageRelation.ImageAboveText;

                }
            }
        }

        public WizardTitle WizTitle
        {
            get { return _title; }
            set { _title = value; }
        }

        public void AddRightButtons()
        {
            AddRightButton(finishButton, Finish_Click);
            AddRightButton(nextButton, Next_Click);
            //AddRightButton(resetButton, Reset_Click);
            AddRightButton(prevButton, Prev_Click);
            //AddRightButton(cancelButton, Cancel_Click);
            AddRightButton(snapShotButton, Snapshot_Click);
            //AddRightButton(switchViewButton, SwitchView_Click);
        }

        

        public void UpdateButtonText(PODButton myButton, String myLabel)
        {
            int i = 0;

            foreach (ButtonHandler button in _leftButtons.Values)
            {
                if (myButton == button.Button)
                {

                    foreach (List<ToolStripItem> list in _menuItems)
                    {
                        ToolStripMenuItem item = (ToolStripMenuItem)list[list.Count - 1];

                        item.DropDownItems[i].Text = myLabel;
                    }

                    myButton.Text = myLabel;

                    break;
                }

                i++;
            }
        }

        public WizardPanel WizPanel
        {
            get { return _panel; }
            set { _panel = value; }
        }

        public void SetSource(WizardSource mySource)
        {
            _source = mySource;

            var analysis = _source as Analysis;

            //projects and quick analyses cannot be duplicated
            if (analysis == null || analysis.AnalysisType == AnalysisTypeEnum.Quick)
                snapShotButton.Enabled = false;
            else
                snapShotButton.Enabled = true;

        }

        public bool RemoveLeftButton(String myLabel)
        {
            return RemoveButton(_leftButtons, myLabel);
        }

        public bool RemoveRightButton(String myLabel)
        {
            return RemoveButton(_rightButtons, myLabel);
        }

        private bool RemoveButton(ButtonHandlerList myList, String myLabel)
        {
            if (myList.ContainsKey(myLabel) == true)
            {
                myList.Remove(myLabel);

                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddEventToRightButton(PODButton myButton, EventHandler myEvent)
        {
            AddEventToButton(_rightButtons, myButton, myEvent);
        }

        public void AddEventToLeftButton(PODButton myButton, EventHandler myEvent)
        {
            AddEventToButton(_leftButtons, myButton, myEvent);
        }

        private void AddEventToButton(ButtonHandlerList myList, PODButton myButton, EventHandler myEvent)
        {
            foreach(ButtonHandler handle in myList.Values)
            {
                if(handle.Button == myButton)
                {
                    handle.AddEvent(myEvent);
                    break;
                }
            }
        }

        public int AddLeftButton(PODButton myButton, EventHandler myHandler)
        {
            return AddButton(_leftButtons, leftFlowPanel, myButton, myHandler);
        }

        public int AddRightButton(PODButton myButton, EventHandler myHandler)
        {
            return AddButton(_rightButtons, rightFlowPanel, myButton, myHandler);
        }

        private int AddButton(ButtonHandlerList myList, FlowLayoutPanel myPanel, PODButton myButton, EventHandler myHandler)
        {
            myList.Add(myButton.Name, new ButtonHandler(myButton, myHandler));

            myPanel.Controls.Add(myButton);

            myButton.BackColor = BackColor;

            if(myPanel != null)
                myPanel.MaximumSize = new Size(0, myButton.Height);

            return myList.Count - 1;
        }
        /*
        protected void AddSolveAllModelsButton()
        {
            //add in the button used to solve all models with default transform settings
            _solveAllModelsButton = new PODButton("Solve All Models");
            this._solveAllModelsToolTip = new ToolTip(this.components);
            this._solveAllModelsToolTip.SetToolTip(this._solveAllModelsButton, "Solve all models contained within the project manager with default parameters");
            AddLeftButton(_solveAllModelsButton, SolveAllModels_Click);
        }
        */
        private void SwitchView_Click(object sender, EventArgs e)
        {
            if(NeedSwitchHelpView != null)
            {
                NeedSwitchHelpView.Invoke(sender, e);
            }
        }
        
        public void Snapshot_Click(object sender, EventArgs e)
        {
            OnSnapshotClick(sender, e);
        }        

        private void Cancel_Click(object sender, EventArgs e)
        {
            OnCancelClick(sender, e);
        }

        private void Reset_Click(object sender, EventArgs e)
        {
            OnResetClick(sender, e);

            WizPanel.RefreshValues();
        }

        private void Prev_Click(object sender, EventArgs e)
        {
            OnPrevClick(sender, e);
        }

        private void Next_Click(object sender, EventArgs e)
        {
            OnNextClick(sender, e);
        }

        private void Finish_Click(object sender, EventArgs e)
        {
            OnFinishClick(sender, e);
        }

        private void ShowOverflowMenu(PODOverButton myButton, FlowLayoutPanel myPanel, ContextMenuStrip myMenu, ButtonHandlerList myList)
        {
            ToolStripMenuItem item;

            myMenu.Items.Clear();
            myMenu.ShowImageMargin = true;

            int shownIndex = (myPanel.Width - myButton.Width) / Globals.StdWidth(this);

            for (int i = myPanel.Controls.Count - 1; i > shownIndex; i--)
            {
                item = new ToolStripMenuItem();
                PODButton podButton = (PODButton)myPanel.Controls[i];

                item.Text = podButton.Text;
                item.Image = podButton.Image;
                item.ImageScaling = ToolStripItemImageScaling.None;

                foreach (EventHandler handle in myList[podButton.Name].Handler)
                {
                    item.Click += handle;
                }

                myMenu.Items.Add(item);
            }

            myMenu.Show(myButton, new Point(0, 0));
        }

        private void LeftOverflow_Click(object sender, EventArgs e)
        {
            ShowOverflowMenu(leftOverflowButton, leftFlowPanel, leftOverflowMenu, _leftButtons);
        }

        private void RightOverflow_Click(object sender, EventArgs e)
        {
            ShowOverflowMenu(rightOverflowButton, rightFlowPanel, rightOverflowMenu, _rightButtons);
        }

        protected void SolveAllModels_Click(object sender, EventArgs e)
        {
            //execute solving all analysis from here
            Analysis.RunAnalysis();
            //finishButton+= finishButton_Click;
        }

        internal void SyncStepContextMenu(List<ToolStripItem> myMenu)
        {
            ToolStripMenuItem item;

            ToolStripMenuItem parent = new ToolStripMenuItem();

            ////parent.Text = "Navigate";
            ////parent.Enabled = false;
            ////myMenu.Add(parent);

            var actionPanel = ContextMenuStripConnected.MakeNewMenuFlowLayoutPanel("Action");
            var actionButtons = new ToolStripControlHost(actionPanel);
            

        //if (NavigateButtons == null)
        //{
            var navigatePanel = ContextMenuStripConnected.MakeNewMenuFlowLayoutPanel("Navigate");                
            var navigateButtons = new ToolStripControlHost(navigatePanel);

            foreach (ButtonHandler button in _rightButtons.Values)
            {
                var navButton = new PODButton(StepToolTip, StepToolTip.GetToolTip(button.Button));
                navButton.TextImageRelation = TextImageRelation.Overlay;

                item = new ToolStripMenuItem();

                navButton.Text = "";// button.Button.Text;
                navButton.Name = navigatePanel.Name + "_" + button.Button.Text;
                navButton.BackgroundImage = button.Button.Image;
                navButton.BackgroundImageLayout = ImageLayout.Center;
                //navButton.ImageAlign = ContentAlignment.MiddleCenter;// = ToolStripItemImageScaling.None;
                navButton.Width = button.Button.Image.Width + 10;
                navButton.Height = button.Button.Image.Height + 10;
                navButton.Padding = new Padding(0, 0, 0, 0);
                navButton.Margin = new Padding(0, 0, 0, 0);
                navButton.Enabled = button.Button.Enabled;

                foreach (EventHandler handle in button.Handler)
                {
                    navButton.Click += handle;
                }

                //navButton.Click += SaveNavigateAsLast;
                //navButton.Click += ContextMenuStripConnected.CloseEverythingElse;
                //navButton.Click += RefreshActions;
                //navButton.Click += navButton_Click;

                navigatePanel.Controls.Add(navButton);
                navigatePanel.Controls.SetChildIndex(navButton, 0);
                
                ContextMenuStripConnected.ForcePanelToDraw(navigatePanel);
                //myMenu.Add(item);

                ////parent.DropDownItems.Add(item);

                ////parent.Enabled = true;
            }

            //NavigateButtons = navigateButtons;
        //}

            myMenu.Insert(0, navigateButtons);

            //if (_leftButtons.Values.Count > 0)
            //    myMenu.Add(new ToolStripSeparator());

            ////parent.DropDownItems.Add("-");

            ////parent = new ToolStripMenuItem();
            ////parent.Text = "Actions";
            ////parent.Enabled = false;
            ////myMenu.Add(parent);
            
            foreach (ButtonHandler button in _leftButtons.Values)
            {
                var actButton = new PODButton(StepToolTip, StepToolTip.GetToolTip(button.Button));
                actButton.TextImageRelation = TextImageRelation.Overlay;

                item = new ToolStripMenuItem();

                actButton.Text = "";// button.Button.Text;
                actButton.Name = actionPanel.Name + "_" + button.Button.Text;

                if (button.Button.Image != null)
                {
                    actButton.BackgroundImage = button.Button.Image;
                    actButton.BackgroundImageLayout = ImageLayout.Center;
                    //navButton.ImageAlign = ContentAlignment.MiddleCenter;// = ToolStripItemImageScaling.None;
                    actButton.Width = button.Button.Image.Width + 10;
                    actButton.Height = button.Button.Image.Height + 10;
                }

                actButton.Padding = new Padding(0, 0, 0, 0);
                actButton.Margin = new Padding(0, 0, 0, 0);
                actButton.Enabled = button.Button.Enabled;

                foreach (EventHandler handle in button.Handler)
                {
                    actButton.Click += handle;
                }

                //actButton.Click += SaveActionAsLast;
                //actButton.Click += ContextMenuStripConnected.CloseEverythingElse;

                ContextMenuStripConnected.ForcePanelToDraw(actionPanel);
                ////parent.DropDownItems.Add(item);

                ////parent.Enabled = true;

                //myMenu.Add(item);

                actionPanel.Controls.Add(actButton);
                //actionPanel.Controls.SetChildIndex(actButton, 0);
            }

            if(actionPanel.Controls.Count > 0)
                myMenu.Insert(1, actionButtons);

            Actions = actionButtons;

            //no point in having a step list with 1 step
            if (Source.ProgressStepListNode != null && Source.ProgressStepListNode.Nodes.Count > 1)
            {
                if (Source.ProgressStepListNode.Nodes.Count > 0)
                    myMenu.Add(new ToolStripSeparator());

                foreach (TreeNode node in Source.ProgressStepListNode.Nodes)
                {
                    item = new ToolStripMenuItem();

                    item.Text = node.Text;
                    item.Click += JumpToStep;
                    item.Name = parent.Text + "_" + item.Text;

                    ////parent.DropDownItems.Add(item);

                    ////parent.Enabled = true;

                    myMenu.Add(item);
                }
            }

            _menuItems.Add(myMenu);
        }

        

        

        //private void RefreshActions(object sender, EventArgs e)
        //{
        //    var actions = CurrentActions;
        //    var button = sender as Button;

        //    if (button != null)
        //    {
        //        var menu = button.Parent.Parent as ContextMenuStripConnected;
        //        var removeList = new List<ToolStripItem>();

        //        if (menu != null)
        //        {
        //            ToolStripControlHost remove = null;

        //            foreach (var item in menu.Items)
        //            {
        //                var container = item as ToolStripControlHost;

        //                if (container != null)
        //                {
        //                    if(container.Control != button.Parent)
        //                    {
        //                        remove = container;
        //                        break;
        //                    }
        //                }
        //            }

        //            if (remove != null)
        //            {
        //                var width = menu.Width;
        //                menu.SuspendLayout();
        //                var index = menu.Items.IndexOf(remove);
        //                menu.Items.Remove(remove);
        //                menu.Items.Insert(index, CurrentActions);                        
        //                menu.ResumeLayout(true);
        //                menu.Width = width;
                        
        //            }
        //        }
        //    }
        //}

        

        void navButton_Click(object sender, EventArgs e)
        {
            //var button = sender as Button;

            //button.Parent.Parent.Hide();
        }

        private void SaveActionAsLast(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;

            if(item != null)
            {
                LastActionClicked = item.Name;
            }
        }

        private void SaveNavigateAsLast(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;

            if (item != null)
            {
                LastNavigateClicked = item.Name;
            }
        }

        void JumpToStep(object sender, EventArgs e)
        {
            if (Source != null)
            {
                var item = (ToolStripMenuItem)sender;
                int index = -1;

                foreach (TreeNode node in Source.ProgressStepListNode.Nodes)
                {
                    if (item.Text == node.Text)
                        index = node.Index;
                }

                if (index >= 0)
                {
                    StepArgs stepArgs = new StepArgs(index);
                    Source.OnJumpTo(Source, stepArgs);
                }
            }
        }

        private void ActionBar_Resize(object sender, EventArgs e)
        {
            int stdWidth = Globals.StdWidth(this);
            int rightWidth = _rightButtons.Count * stdWidth + 30;
            int leftWidth = mainTableLayout.Width - rightWidth - 10;

            if (mainTableLayout.Width - stdWidth < rightWidth)
            {
                leftWidth = stdWidth;
                rightWidth = mainTableLayout.Width - leftWidth - 10;
            }

            if (rightWidth < 5)
                rightWidth = 5;

            if (leftWidth < 5)
                leftWidth = 5;

            mainTableLayout.ColumnStyles[1].SizeType = SizeType.Absolute;
            mainTableLayout.ColumnStyles[1].Width = rightWidth;
            mainTableLayout.ColumnStyles[0].SizeType = SizeType.Absolute;
            mainTableLayout.ColumnStyles[0].Width = leftWidth;

            CheckForOverflow(leftFlowPanel, _leftButtons, leftOverflowButton);
            CheckForOverflow(rightFlowPanel, _rightButtons, rightOverflowButton);            
        }

        private void CheckForOverflow(FlowLayoutPanel myPanel, ButtonHandlerList myButtons, PODButton myOverflowButton)
        {
            int stdWidth = Globals.StdWidth(this);

            if (myPanel.Width < myButtons.Count * stdWidth)
            {
                myOverflowButton.Visible = true;
            }
            else
            {
                myOverflowButton.Visible = false;
            }
        }

        private void mainTableLayout_Paint(object sender, PaintEventArgs e)
        {
            
        }

        //using dynamic rather than casting everytime from a base class 
        //the action bar child class will only ever work with one corresponding panel
        public dynamic MyPanel
        {
            get
            {
                return (dynamic)WizPanel;
            }
        }

        internal void UpdateMenuStrip(ToolStripItemCollection list)
        {

            if (_leftButtons.Count == list.Count)
            {
                for (int i = 0; i < _leftButtons.Count; i++)
                {
                    ToolStripMenuItem item = (ToolStripMenuItem)list[i];

                    item.Text = _leftButtons.Values.ElementAt(i).Button.Text;
                }
            }
        }

        public void DeactivateNextButton()
        {
            //rightFlowPanel.Controls.Remove(nextButton);
            nextButton.Enabled = false;
        }

        public void ActivateNextButton()
        {
            //rightFlowPanel.Controls.Add(nextButton);
            //rightFlowPanel.Controls.SetChildIndex(nextButton, rightFlowPanel.Controls.IndexOf(finishButton));

            nextButton.Enabled = true;
        }

        internal void PressNextButton()
        {
            nextButton.PerformClick();            
        }

        protected void OnSnapshotClick(object sender, EventArgs e)
        {
            if (Source != null && !Source.IsBusy && !Source.LockBusy)
            {
                var analysis = _source as Analysis;

                if (analysis != null)
                {
                    var clone = analysis.CreateDuplicate();
                    analysis.RaiseCreatedAnalysis(clone);
                }
                //_source.OnAddSnapshot(Source, null);
            }
        }

        protected void OnCancelClick(object sender, EventArgs e)
        {
            if (Source != null && !Source.IsBusy && !Source.LockBusy)
            {
                _source.OnWizardCancel(Source, null);
            }
        }

        protected void OnResetClick(object sender, EventArgs e)
        {
            if (Source != null && !Source.IsBusy && !Source.LockBusy)
            {
                _source.OnStepReset(Source, null);
            }
        }

        protected void OnPrevClick(object sender, EventArgs e)
        {
            if (Source != null && !Source.IsBusy && !Source.LockBusy)
            {
                StepArgs stepArgs = new StepArgs(_panel.Index);
                Source.OnStepPrev(Source, stepArgs);
            }
        }

        protected void OnNextClick(object sender, EventArgs e)
        {
            if (Source != null && !Source.IsBusy && !Source.LockBusy)
            {              
                StepArgs stepArgs = new StepArgs(_panel.Index);
                Source.OnStepNext(Source, stepArgs);
            }
        }

        protected void OnFinishClick(object sender, EventArgs e)
        {
            if (Source != null && !Source.IsBusy && !Source.LockBusy)
            {
                Source.OnWizardFinish(Source, Source.FinishArg);
            }
        }

        internal void PressPreviousButton()
        {
            prevButton.PerformClick();
        }

        internal void PressFinishButton()
        {
            finishButton.PerformClick();
        }

        public void SyncClicks()
        {
            SyncClicksWithButtons(_rightButtons);
            SyncClicksWithButtons(_leftButtons);
        }

        private void SyncClicksWithButtons(ButtonHandlerList myButtons)
        {
            foreach(ButtonHandler handle in myButtons.Values)
            {
                foreach(EventHandler hEvent in handle.Handler)
                {
                    handle.Button.Click += hEvent;
                }
            }
        }

        public Keys ShortCutKey(Keys key)
        {
            if (ShowKeyboardCues)
                return key;
            else
                return (Keys.Control | key);
        }

        public virtual bool ProcessKeyboardShortCuts(ref Message msg, Keys keyData)
        {


            //if (keyData == ShortCutKey(Keys.N))
            //{
            //    PressNextButton();

            //    return true;
            //}
            //else if (keyData == ShortCutKey(Keys.P))
            //{
            //    PressPreviousButton();

            //    return true;
            //}
            //else if (keyData == ShortCutKey(Keys.F))
            //{
            //    PressFinishButton();

            //    return true;
            //}

            return false;
        }

        
    }
}
