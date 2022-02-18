using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POD.Wizards;
using WeifenLuo.WinFormsUI.Docking;
using Transitions;

namespace POD.Docks
{
    public delegate void SwapStepsEventHandler(object sender, SwapStepsEventArgs e);

    /// <summary>
    /// This is a Document Dock that holds a Step of a given Wizard. All Document Docks hold a Wizard Step.
    /// </summary>
    public partial class WizardDock : PodDock
    {


        #region Fields
        /// <summary>
        /// The Step that displays on the Dock.
        /// </summary>
        private WizardStep _step;
        private bool _skipAnimations;
        [NonSerialized]
        public EventHandler NeedSteps = null;

        public bool HasStep
        {
            get
            {
                return _step != null;
            }
        }

        public bool SkipAnimations
        {
            get { return _skipAnimations; }
            set
            {
                _skipAnimations = value;

                if (_skipAnimations == true)
                {
                    Bitmap image = new Bitmap(Step.Width, Step.Height);

                    Step.DrawPanelToBitmap(image, new Rectangle(0, 0, Step.Width, Step.Height));
                    movePanelBox.Height = Step.Height;
                    movePanelBox.Width = Step.Width;
                    movePanelBox.Left = 0;
                    movePanelBox.Top = 0;
                    movePanelBox.Image = image;
                    movePanelBox.BringToFront();
                    movePanelBox.Show();

                    Cursor.Current = Cursors.WaitCursor;
                }
                else
                {
                    movePanelBox.SendToBack();
                    movePanelBox.Hide();

                    Cursor.Current = Cursors.Default;
                }


            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new Dock with the default Label.
        /// </summary>
        public WizardDock()
        {
            InitializeComponent();

            Label = Globals.WizardDockLabel;
            movePanelBox.SendToBack();
            moveBarBox.SendToBack();

            movePanelBox.Padding = new Padding(0, 0, 0, 0);
            movePanelBox.Margin = new Padding(0, 0, 0, 0);

            nextPanelBox.Padding = new Padding(0, 0, 0, 0);
            nextPanelBox.Margin = new Padding(0, 0, 0, 0);
        }
        /// <summary>
        /// Create a new Dock with a given Label.
        /// </summary>
        /// <param name="myLabel"></param>
        public WizardDock(string myLabel)
        {
            InitializeComponent();

            Label = myLabel;
            Text = myLabel;

            movePanelBox.SendToBack();
            moveBarBox.SendToBack();

            movePanelBox.Padding = new Padding(0, 0, 0, 0);
            movePanelBox.Margin = new Padding(0, 0, 0, 0);
        }

        private SwapStepsEventArgs _args;
        #endregion

        #region Properties
        /// <summary>
        /// Get/set the current Wizard Step to display.
        /// </summary>
        public WizardStep Step
        {
            get
            {
                if(_step == null)
                    AddSteps();

                return _step; 
            }
            set
            {
                WizardStep temp = _step;

                _step = value;

                if (HasStep)
                {
                    movePanelBox.BringToFront();
                    _step.Visible = false;
                    Controls.Add(_step);
                    _step.Dock = DockStyle.Fill;
                    _step.SendToBack();
                    _step.Visible = true;

                    Text = _step.TabName;
                    Label = _step.Source.Name;

                    if (temp != _step && temp != null && Controls.Contains(temp) == true)
                        Controls.Remove(temp);

                    _step.PrepareGUI();
                }

                
            }
        }
        /// <summary>
        /// Get the Node whose children are to create a Step List for the Wizard Progress Dock.
        /// </summary>
        public TreeNode ProgressStepListNode
        {
            get
            {
                return _step.ProgressStepListNode;
            }

        }

        public SwapStepsEventHandler SwapSteps;
        #endregion

        #region Methods
        public void BringMovePanelToFront()
        {
            Bitmap image = new Bitmap(Step.Panel.Width, Step.Panel.Height);
            Step.DrawPanelToBitmap(image, new Rectangle(Step.Panel.Left, Step.Panel.Top, Step.Panel.Width, Step.Panel.Height));
            movePanelBox.Image = image;
            movePanelBox.Height = Step.Panel.Height;
            movePanelBox.Width = Step.Panel.Width;
            movePanelBox.Left = 0;
            movePanelBox.Top = 0;
            movePanelBox.Visible = true;
            movePanelBox.BringToFront();
            movePanelBox.Show();
        }
        public void SendMovePanelToBack()
        {
            movePanelBox.SendToBack();
            movePanelBox.Hide();
            movePanelBox.Visible = false;
            movePanelBox.Height = Step.Height;
            movePanelBox.Width = Step.Width;
            movePanelBox.Left = 0;
            movePanelBox.Top = 0;
        }
        #endregion

        #region Event Handling

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            _step.CloseOpenedContextMenu();
        }

        public void OnTransitionSteps(object sender, SwapStepsEventArgs args)
        {
            WizardStep wizTo = args.TransitionTo;
            WizardStep wizFrom = args.TransitionFrom;
            Transition transition;
            Panel emptyPanel = new Panel();

            emptyPanel.Height = Step.Height;
            emptyPanel.Width = Step.Width;

            if (wizTo != wizFrom)
            {
                //switch animation
                transition = new Transition(new TransitionType_EaseInEaseOut(400));
            }
            else
            {
                //bounce animation
                transition = new Transition(new TransitionType_Bounce(400));
            }


            transition.TransitionCompletedEvent += t_TransitionCompletedEvent;

            _args = args;

            wizTo.Height = wizFrom.Height;
            wizTo.Width = wizFrom.Width;

            wizFrom.Show();
            wizTo.Show();

            WizardStepAnimator panelAni = new WizardStepAnimator(transition, args);
            WizardStepAnimator titleAni = new WizardStepAnimator(transition, args);
            WizardStepAnimator barAni = new WizardStepAnimator(transition, args);

            SuspendLayout();

            titleAni.SetupSizes(moveTitleBox, nextTitleBox, wizFrom.TitleWidth, wizFrom.TitleHeight);
            barAni.SetupSizes(moveBarBox, nextBarBox, wizFrom.BarWidth, wizFrom.BarHeight);
            panelAni.SetupSizes(movePanelBox, nextPanelBox, wizFrom.PanelWidth, wizFrom.PanelHeight);

            if (wizTo != wizFrom)
            {

                titleAni.SetupMoveLocations(wizTo.DrawTitleToBitmap, wizFrom.DrawTitleToBitmap, -wizFrom.TitleHeight, wizFrom.TitleLeft, wizFrom.TitleTop, wizFrom.TitleLeft);
                titleAni.SetupNextLocations(wizTo.DrawTitleToBitmap, wizFrom.TitleTop, wizFrom.TitleLeft, args.Direction);

                barAni.SetupMoveLocations(wizTo.DrawBarToBitmap, wizFrom.DrawBarToBitmap, wizFrom.Height, wizFrom.BarLeft, wizFrom.BarTop, wizFrom.BarLeft);
                barAni.SetupNextLocations(wizTo.DrawBarToBitmap, wizFrom.BarTop, wizFrom.BarLeft, args.Direction);

                panelAni.SetupMoveLocations(wizTo.DrawPanelToBitmap, wizFrom.DrawPanelToBitmap, wizFrom.PanelTop, wizFrom.PanelWidth, wizFrom.PanelTop, wizFrom.PanelLeft);
                panelAni.SetupNextLocations(wizTo.DrawPanelToBitmap, wizFrom.PanelTop, wizFrom.PanelLeft, args.Direction);
            }
            else
            {
                int bounce = 6; //size of bounce inpixels
                int bounce1;
                int bounce2;
                int bouncePrevPanel;
                int bounceNextPanel;


                if (args.Direction == 1)
                {
                    bounce1 = bounce;
                    bounce2 = 0;
                    bouncePrevPanel = bounce;
                    bounceNextPanel = 0;
                }
                else
                {
                    bounce1 = 0;
                    bounce2 = -bounce;
                    bouncePrevPanel = 0;
                    bounceNextPanel = -bounce;
                }

                titleAni.SetupMoveLocations(wizTo.DrawTitleToBitmap, wizFrom.DrawTitleToBitmap, wizFrom.TitleTop - bounce1, wizFrom.TitleLeft, wizFrom.TitleTop - bounce2, wizFrom.TitleLeft);
                titleAni.SetupNextLocations(emptyPanel.DrawToBitmap, wizFrom.TitleTop, wizFrom.TitleLeft, 1);

                barAni.SetupMoveLocations(wizTo.DrawBarToBitmap, wizFrom.DrawBarToBitmap, wizFrom.BarTop + bounce1, wizFrom.BarLeft, wizFrom.BarTop + bounce2, wizFrom.BarLeft);
                barAni.SetupNextLocations(emptyPanel.DrawToBitmap, wizFrom.BarTop, wizFrom.BarLeft, 1);

                panelAni.SetupMoveLocations(wizTo.DrawPanelToBitmap, wizFrom.DrawPanelToBitmap, wizFrom.PanelTop, wizFrom.PanelLeft-bouncePrevPanel, wizFrom.PanelTop, wizFrom.PanelLeft-bounceNextPanel);
                panelAni.SetupNextLocations(emptyPanel.DrawToBitmap, wizFrom.PanelTop, wizFrom.PanelLeft, 1);

                ResumeLayout();
            }

            titleAni.AddAnimations();
            barAni.AddAnimations();
            panelAni.AddAnimations();

            ResumeLayout();

            transition.run();
        }

        void t_TransitionCompletedEvent(object sender, Transition.Args e)
        {
            if (SwapSteps != null)
                SwapSteps.Invoke(sender, _args);

            //if (SkipAnimations == false)
            //{
            if (Controls.GetChildIndex(Step) != 0)
                Step.BringToFront();
            //}

            movePanelBox.Hide();
            moveBarBox.Hide();
            moveTitleBox.Hide();
            nextTitleBox.Hide();
            nextBarBox.Hide();
            nextPanelBox.Hide();
        }

        #endregion

        public StepArgs CreateStepArgs()
        {
            if (_step == null)
                return null;

            StepArgs args = new StepArgs(_step.Index, _step.HelpFile, _step.ProgressStepListNode);

            return args;
        }


        public void RefreshValues()
        {
            Step.RefreshValues();
        }

        private void WizardDock_Activated(object sender, EventArgs e)
        {
            _step.PrepareGUI();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //see if actionbar can do something with the keys
            var result = Step.ActionBar.SendKeys(keyData);

            //if not then see if panel can do something with the keys
            if(!result)
                result = Step.Panel.SendKeys(keyData);

            //if neither then pass the keys onward
            if (result)
                return result;
            else
                return base.ProcessCmdKey(ref msg, keyData);
        }


        public void AddSteps()
        {
            if (NeedSteps != null)
            {
                NeedSteps.Invoke(this, null);
            }
        }
    }
}
