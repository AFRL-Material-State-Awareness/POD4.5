using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POD.Controls;
using POD.Analyze;
using System.Reflection;

namespace POD.Wizards
{
    /// <summary>
    /// Defines a single step of a wizard. A step is made up of a title bar, panel and action bar
    /// This is the base class of all steps.
    /// </summary>
    public partial class WizardStep : WizardUI
    {
        #region Fields
        static List<ContextMenuStripConnected> _openStrips = new List<ContextMenuStripConnected>();
        static ContextMenuStripConnected _currentOpenMenu = new ContextMenuStripConnected();

        public static ContextMenuStripConnected OpenContextMenu
        {
            get { return WizardStep._currentOpenMenu; }
            set { WizardStep._currentOpenMenu = value; }
        }

        /// <summary>
        /// Action bar located at the bottom.
        /// </summary>
        private WizardActionBar _actionBar;

        public WizardActionBar ActionBar
        {
            get { return _actionBar; }
            set { _actionBar = value; }
        }
        /// <summary>
        /// A unique string used to identify this step.
        /// </summary>
        protected string _id;
        /// <summary>
        /// Overlay shown during the tutorial version.
        /// </summary>
        protected TutorialOverlay _overlay;
        /// <summary>
        /// Main panel located in the middle.
        /// </summary>
        private WizardPanel _panel;

        public WizardPanel Panel
        {
            get { return _panel; }
            set { _panel = value; }
        }
        /// <summary>
        /// Title bar located at the top.
        /// </summary>
        private WizardTitle _title;

        public WizardTitle Title
        {
            get { return _title; }
            set { _title = value; }
        }        
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new wizard step with a given source.
        /// </summary>
        /// <param name="mySource">the source that the step will interact with</param>
        public WizardStep(WizardSource mySource, ref ControlOrganize myOrganize)
        {
            InitializeComponent();

            /*_title = new WizardTitle();
            _panel = new WizardPanel();
            _actionBar = new WizardActionBar();
            _overlay = new TutorialOverlay();*/

            _source = mySource;            

            InitializeMainControls();
            PlaceMainControls(myOrganize);

            StepToolTip = new PODToolTip();
            
        }

        /// <summary>
        /// Create an empty wizard step.
        /// </summary>
        public WizardStep(WizardSource mySource)
        {
            InitializeComponent();

            _source = mySource;

            StepToolTip = new PODToolTip();
        }


        /// <summary>
        /// Create an empty wizard step.
        /// </summary>
        public WizardStep()
        {
            InitializeComponent();

            StepToolTip = new PODToolTip();
        }
        #endregion

        #region Properties

        

        /// <summary>
        /// Get the title of the step without the subtitle.
        /// </summary>
        public string Header
        {
            get
            {
                return _title.Header.Text;
            }
        }
        /// <summary>
        /// Get/set the steps index.
        /// </summary>
        public int Index
        {
            get
            {
                return _panel.Index;
            }
            set
            {
                _panel.Index = value;
            }
        }
        /// <summary>
        /// Get the TreeNode used to make the list for the Wizard Progress Dock.
        /// </summary>
        public TreeNode ProgressStepListNode
        {
            get
            {
                return Source.ProgressStepListNode;
            }
        }
        /// <summary>
        /// Did the step fullfill all requirements to move to the next step?
        /// </summary>
        public bool Stuck
        {
            get
            {
                return _panel.Stuck;
            }
        }
        /// <summary>
        /// Get the name of the wziard dock's tab if this step is the current step.
        /// </summary>
        public string TabName
        {
            get
            {
                if(_source != null)
                {
                    return _source.Name + " - " + _title.Header.Text;
                }

                return "Unknown - " + _title.Header.Text;
            }
        }
        public int PanelWidth { get { return _panel.Width; } }
        public int PanelHeight { get { return _panel.Height; } }
        public int PanelTop { get { return _panel.Top; } }
        public int PanelLeft { get { return _panel.Left; } }

        public int BarWidth { get { return _actionBar.Width; } }
        public int BarHeight { get { return _actionBar.Height; } }
        public int BarTop { get { return _actionBar.Top; } }
        public int BarLeft { get { return _actionBar.Left; } }

        public int TitleWidth { get { return _title.Width; } }
        public int TitleHeight { get { return _title.Height; } }
        public int TitleTop { get { return _title.Top; } }
        public int TitleLeft { get { return _title.Left; } }
        #endregion

        #region Methods

        public void FixPanelControlSizes()
        {
            _panel.FixPanelControlSizes();
        }

        /// <summary>
        /// Add context menu related to action bar to the controls on the panel.
        /// </summary>
        /// <param name="myControls">controls to add context menu to</param>
        private void AddContextMenuToControls(ControlCollection myControls)
        {
            bool worked;
            var temp = new ContextMenuStripConnected();
            ToolStripItem[] array = new ToolStripItem[_panel.ContextMenuStrip.Items.Count];

            for (int i = 0; i < myControls.Count; i++)
            {
                Control control = myControls[i];

                if (control.ContextMenuStrip == null)
                    control.ContextMenuStrip = new ContextMenuStripConnected();

                if (HasWorkingTextProperty(control) == true)
                {
                    control.ContextMenuStrip.Opening += ContextMenuStrip_OpeningText;
                    //control.Click += control_Click;
                }
                else
                {
                    control.ContextMenuStrip.Opening += ContextMenuStrip_Opening;
                    //control.Click += control_Click;
                }

                AddContextMenuToControls(control.Controls);
            }
        }

        //void control_Click(object sender, EventArgs e)
        //{
        //    var control = sender as Control;
    
        //    foreach(ContextMenuStripConnected strip in _openStrips)
        //    {
        //        strip.CloseEverything();
        //    }

        //    _openStrips.Clear();
        //}


        /// <summary>
        /// See if the step is stuck and allow the step to notify the user how to deal
        /// with the issue.
        /// </summary>
        /// <returns></returns>
        public bool CheckStuck()
        {
            return _panel.CheckStuck();
        }
        /// <summary>
        /// Does the control have text to edit?
        /// </summary>
        /// <param name="control">control to check</param>
        /// <returns>True if control has an editable text field</returns>
        public static bool HasWorkingTextProperty(Control control)
        {
            return control is TextBox || control is ComboBox;
        }
        /// <summary>
        /// Initialize main parts of the step.
        /// </summary>
        protected void InitializeMainControls()
        {
            _actionBar.SetSource(_source);
            _actionBar.WizPanel = _panel;
            _actionBar.WizTitle = _title;
            _actionBar.SyncClicks();
            _actionBar.StepToolTip = StepToolTip;

            _panel.Source = _source;
            _panel.WizActionBar = _actionBar;
            _panel.StepToolTip = StepToolTip;

            _title.StepToolTip = StepToolTip;
        }
        /// <summary>
        /// Initializes the step by setting the controls and refreshing them.
        /// </summary>
        public void InitializeStep(string myWizardName, ref ControlOrganize myOrganize)
        {
            InitializeMainControls();
            PlaceMainControls(myOrganize);
            HelpFile = myWizardName + "." + this.GetType().Name + ".txt";

            //RefreshValues();

        }

        public void UpdateMainPositions(ControlOrganize myOrganize)
        {
            switch (myOrganize)
            {
                case ControlOrganize.NaviTop:
                    layout.RowStyles[0].SizeType = SizeType.AutoSize;
                    layout.RowStyles[1].SizeType = SizeType.AutoSize;
                    layout.RowStyles[2].SizeType = SizeType.Percent;

                    layout.RowStyles[0].Height = 0.0F;
                    layout.RowStyles[1].Height = 0.0F;
                    layout.RowStyles[2].Height = 100.0F;

                    layout.Controls.Add(_title, 0, 0);
                    layout.Controls.Add(_panel, 0, 2);
                    layout.Controls.Add(_actionBar, 0, 1);

                    _panel.Dock = DockStyle.Fill;
                    _title.Dock = DockStyle.Top;
                    _actionBar.Dock = DockStyle.Top;
                    break;
                default:
                    layout.RowStyles[0].SizeType = SizeType.AutoSize;
                    layout.RowStyles[1].SizeType = SizeType.Percent;
                    layout.RowStyles[2].SizeType = SizeType.AutoSize;

                    layout.RowStyles[0].Height = 0.0F;
                    layout.RowStyles[1].Height = 100.0F;
                    layout.RowStyles[2].Height = 0.0F;

                    layout.Controls.Add(_title, 0, 0);
                    layout.Controls.Add(_panel, 0, 1);
                    layout.Controls.Add(_actionBar, 0, 2);

                    _title.Dock = DockStyle.Top;
                    _panel.Dock = DockStyle.Fill;                    
                    _actionBar.Dock = DockStyle.Bottom;
                    break;
            }
        }

        /// <summary>
        /// Place in the main controls in the form's layout and initialize
        /// the context menu.
        /// </summary>
        protected void PlaceMainControls(ControlOrganize myOrganize)
        {
            UpdateMainPositions(myOrganize);

            _actionBar.SyncStepContextMenu(_panel.ControlStrip);
            _actionBar.SyncStepContextMenu(_panel.TextStrip);  

            AddContextMenuToControls(_panel.Controls);

            if (_panel.ContextMenuStrip == null)
                _panel.ContextMenuStrip = new ContextMenuStripConnected();

            _panel.ContextMenuStrip.Opening += ContextMenuStrip_Opening;

            _title.SendToBack();
            _actionBar.SendToBack();
        }
        /// <summary>
        /// Progmatically press the step's next button.
        /// </summary>
        public void PressNextButton()
        {
            _actionBar.PressNextButton();
        }
        /// <summary>
        /// Progmatically press the step's previous button.
        /// </summary>
        public void PressPreviousButton()
        {
            _actionBar.PressPreviousButton();
        }
        /// <summary>
        /// Progmatically press the step's finish button.
        /// </summary>
        public void PressFinishButton()
        {
            _actionBar.PressFinishButton();
        }
        /// <summary>
        /// Force panel to reload values into controls.
        /// </summary>
        public void RefreshValues()
        {
            try
            {
                _panel.RefreshValues();
            }
            catch(Exception exp)
            {
                MessageBox.Show("RefreshValues: " + exp.Message);
            }

            try
            {
                _panel.FixPanelControlSizes();
            }
            catch (Exception exp)
            {
                MessageBox.Show("FixPanelControlSizes: " + exp.Message);
            }
            
        }
        public void DrawPanelToBitmap(Bitmap bmp, Rectangle rect)
        {
            _panel.DrawToBitmap(bmp, rect);
        }
        public void DrawBarToBitmap(Bitmap bmp, Rectangle rect)
        {
            _actionBar.DrawToBitmap(bmp, rect);
        }
        public void DrawTitleToBitmap(Bitmap bmp, Rectangle rect)
        {
            _title.DrawToBitmap(bmp, rect);
        }
        public void GoToNextStep()
        {

        }

        private void RefreshOpenContextMenu()
        {
            OpenContextMenu.ShowOnlyButtons = OpenContextMenu.Visible;

            if(OpenContextMenu.IsTextboxMenu)
            {
                RefreshFromItemList(OpenContextMenu, _panel.TextStrip);
            }
            else
            {
                RefreshFromItemList(OpenContextMenu, _panel.ControlStrip);
            }
        }
        #endregion

        #region Event Handling
        /// <summary>
        /// Build the context menu for non-text controls before it is shown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            var menu = sender as ContextMenuStripConnected;
            
            OpenContextMenu = menu;
            OpenContextMenu.IsTextboxMenu = false;
            OpenContextMenu.ShowOnlyButtons = false;

            _openStrips.Add(menu);

            RefreshNonTextMenu(menu);
            
            //AddLastClick(menu, _actionBar.LastActionClicked, "ActionLastClick", "Action");
            //AddLastClick(menu, _actionBar.LastNavigateClicked, "NavigateLastClick", "Navigate");
            //AddLastClickSeperator(menu);

            e.Cancel = false;

        }

        private void RefreshNonTextMenu(ContextMenuStripConnected menu)
        {
            menu.ShowImageMargin = false;
            RefreshFromItemList(menu, _panel.ControlStrip);
            
        }

        /// <summary>
        /// Build the context menu for text-based controls before it is shown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContextMenuStrip_OpeningText(object sender, CancelEventArgs e)
        {
            ContextMenuStripConnected menu = sender as ContextMenuStripConnected;

            OpenContextMenu = menu;
            OpenContextMenu.IsTextboxMenu = true;
            OpenContextMenu.ShowOnlyButtons = false;

            RefreshTextMenu(menu);

            //AddLastClick(menu, _actionBar.LastActionClicked, "ActionLastClick", "Action");
            //AddLastClick(menu, _actionBar.LastNavigateClicked, "NavigateLastClick", "Navigate");
            //AddLastClickSeperator(menu);

            e.Cancel = false;
        }

        private void RefreshTextMenu(ContextMenuStripConnected menu)
        {
            menu.ShowImageMargin = false;
            RefreshFromItemList(menu, _panel.TextStrip);
            
        }

        private void RefreshFromItemList(ContextMenuStripConnected menu, List<ToolStripItem> list)
        {                       
            menu.SuspendLayout();

            menu.Items.Clear();

            var itemsHeight = 0;

            foreach(var item in list)
            {
                menu.Items.Add(item);

                item.Size = item.GetPreferredSize(item.Size);

                if(item.Visible || item as ToolStripControlHost != null)
                    itemsHeight += item.Size.Height;

                var host = item as ToolStripControlHost;

                if(host != null)
                {
                    var flow = host.Control as FlowLayoutPanel;

                    if(flow != null)
                    {
                        foreach(Control ctrl in flow.Controls)
                        {
                            var button = ctrl as PODButton;

                            if (button != null)
                                button.PODToolTip = StepToolTip;
                        }
                    }
                }
            }

            menu.ShowOnlyButtons = menu.ShowOnlyButtons;

            menu.GetPreferredSize(new Size(0, itemsHeight));

            if (menu.Size.Height < itemsHeight + 10 || menu.Size.Width < 80)
                menu.MinimumSize = new Size(100, itemsHeight + 10);

            if (menu.MinimumSize.Height > itemsHeight + 10)
                menu.MinimumSize = new Size(100, itemsHeight + 10);

            //menu.ResumeDrawing();
            menu.ResumeLayout(true);

            
            
        }
        #endregion      
        
    
        private string _helpFile;

        public string HelpFile
        {
            get
            {
                if (_helpFile == null)
                    _helpFile = Globals.DefaultQuickHelpFile;

                return _helpFile; 
            
            }
            set { _helpFile = value; }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (_actionBar.ProcessKeyboardShortCuts(ref msg, keyData))
                return true;

            return base.ProcessCmdKey(ref msg, keyData);
        }


        internal void UpdateTitleBarToolTip()
        {
            if (_title != null)
            {
                StepToolTip.ShowAlways = true;
                StepToolTip.SetToolTip(_title.Header, _title.Header.Text);
                StepToolTip.SetToolTip(_title.SubHeader, _title.SubHeader.Text);
            }
        }

        public virtual void PrepareGUI()
        {
            RefreshOpenContextMenu();
            

            _panel.PrepareGUI();

            
        }

        

        public override WizardSource Source
        {
            get
            {
                return _source;
            }
            set
            {
                _source = value;

                if (ActionBar != null)
                {
                    ActionBar.NeedSwitchHelpView -= _source.SwitchHelpView;
                    ActionBar.NeedSwitchHelpView += _source.SwitchHelpView;
                }

                
            }
        }


        public void CloseOpenedContextMenu()
        {
            if(OpenContextMenu != null)
                OpenContextMenu.Close();
        }

        private void stepToolTip_Popup(object sender, PopupEventArgs e)
        {

        }

        public void RefreshOverview()
        {
            Panel.RefreshOverview();
        }
    }
}
