using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POD.Wizards;
using POD.Analyze;
using POD.Docks;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace POD
{
    /// <summary>
    /// Holds a Wizard on the Dock that is displaying the current Step of the Wizard.
    /// Mostly handles high level event handling and communication between the Dock and Wizard.
    /// Also, fires events meant for the Wizard Controller.
    /// </summary>
    [Serializable]
    public class WizardDockPair// : ISerializable
    {
        #region Fields
        private bool reloading;
        /// <summary>
        /// The dock that holds the wizard. The dock contains the current step of the wizard.
        /// </summary>
        private WizardDock _dock;
        /// <summary>
        /// A collection of Steps and a Source that is used with a Dock to modify the Source.
        /// </summary>
        private Wizard _wizard;
        /// <summary>
        /// Fires event to the controller when an analysis wizard is finished.
        /// </summary>
        public FinishEventHandler SourceModified;
        /// <summary>
        /// Fires event to controller after next/prev button pressed
        /// and the next step has been selected.
        /// </summary>
        public StepEventHandler StepChanged;
        public StepEventHandler StepChanging;
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new pair with a Wizard with a new Dock.
        /// </summary>
        /// <param name="myWizard">the Wizard to display on the Dock<</param>
        public WizardDockPair(Wizard myWizard)
        {
            _wizard = myWizard;
            _dock = Dock;
            reloading = false;
            AddEvents();
        }

        /// <summary>
        /// Create a new pair with a Wizard and its Dock.
        /// </summary>
        /// <param name="myWizard">the Wizard to display on the Dock</param>
        /// <param name="myDock">the Dock that displays the Wizard</param>
        public WizardDockPair(Wizard myWizard, WizardDock myDock)
        {
            _wizard = myWizard;

            if (_wizard == null)
                return;

            if (myDock != null)
            {
                _dock = myDock;
                _dock.FormClosed += Dock_FormClosed;
            }
            else
            {
                _dock = Dock;
            }

            AddEvents();
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Wizard's Source object cast as an Analysis. Should be certain Source is
        /// of type Analysis before using this.
        /// </summary>
        public Analysis Analysis
        {
            get
            {
                return (Analysis)_wizard.Source;
            }
        }

        /// <summary>
        /// If the Dock isn't defined create a new one then return it.
        /// </summary>
        public WizardDock Dock
        {
            get
            {
                if (_dock == null)
                {
                    _dock = new WizardDock();
                    _dock.FormClosed += Dock_FormClosed;
                }

                return _dock;
            }
            set
            {
                if (Dock.IsDisposed)
                {
                    _dock = value;
                }
            }
        }

        /// <summary>
        /// The Wizard's Source object cast as a Project. Should be certain Source is
        /// of type Project before using this.
        /// </summary>
        public Project Project
        {
            get
            {
                if (_wizard != null)
                    return (Project)_wizard.Source;
                else
                    return null;
                
            }
        }

        /// <summary>
        /// The Wizard's Source object.
        /// </summary>
        public WizardSource Source
        {
            get
            {
                if (_wizard != null)
                    return _wizard.Source;
                else
                    return null;
            }
        }

        /// <summary>
        /// The Wizard of the Wizard/Dock pair.
        /// </summary>
        public Wizard Wizard
        {
            get
            {
                return _wizard;
            }
            set
            {
                this._wizard = value;
            }
        }

        public bool Reloading
        {
            get { return this.reloading; }
            set { this.reloading = value; }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Adds event handlers to the Source so it can react to low level events
        /// fired by the steps in the Wizard.
        /// </summary>
        private void AddEvents()
        {
            _dock.NeedSteps += CreateSteps;

            //handle changing steps
            Source.StepNext += new StepEventHandler(Step_Next);
            Source.StepPrevious += new StepEventHandler(Step_Previous);

            //handle the wizard finishing
            switch (Source.ObjectType)
            {
                case PODObjectTypeEnum.Project:
                    Project.WizardFinished += new FinishEventHandler(Project_Finished);
                    break;

                case PODObjectTypeEnum.Analysis:
                    Analysis.WizardFinished += new FinishEventHandler(Analysis_Finished);
                    break;
            }
        }

        private void CreateSteps(object sender, EventArgs e)
        {
            var dock = sender as WizardDock;

            _wizard.AddSteps();

            if (dock != null)
            {
                dock.Step = _wizard.CurrentStep;
            }
        }

        /// <summary>
        /// Cycles through the steps of a Wizard by repeatedly pressing the Next button
        /// until the end is reached or a step is Stuck.
        /// </summary>
        /// <returns>Returns if it stopped at a Stuck Step</returns>
        private bool MoveToEnd()
        {
            WizardStep before;
            WizardStep after;

            Dock.SkipAnimations = true;

            if (Wizard.CurrentStep != Wizard.LastStep)
            {

                //move to next step until it can't be done
                do
                {
                    before = Wizard.CurrentStep;

                    before.PressNextButton();

                    after = Wizard.CurrentStep;
                }
                while (after != before);                
            }
            else
            {
                //force one more press to do a check
                Wizard.CurrentStep.PressNextButton();
            }


            //if()

            Dock.SkipAnimations = false;

            //don't point in trying to finsih the project
            //if the wizard is stuck on a step
            if (Wizard.CurrentStep.Stuck == true)
            {
                SyncDock();
                OnStepChanging(Wizard, new StepArgs(Wizard.CurrentStep.Index, Wizard.CurrentStep.HelpFile, Wizard.CurrentStep.ProgressStepListNode));

                return true;
            }

            return false;
        }

        /// <summary>
        /// Display the latest version of the Wizard's first step on the Dock.
        /// </summary>
        internal void SetWizardToFirstStep()
        {
            //Dock.Pane.SuspendDrawing();
            Wizard.CurrentStep = Wizard.FirstStep;
            SyncDock();
            //Dock.Pane.ResumeDrawing();
        }

        /// <summary>
        /// Make sure the Dock is displaying the latest version of the Wizard for the
        /// current step.
        /// </summary>
        internal void SyncDock()
        {
            Dock.Step = Wizard.CurrentStep;            
        }

        #endregion

        #region Event Handling
        /// <summary>
        /// Fired after the Finish button is pressed. Moves through the steps
        /// until the end is reached or a Stuck point is reached.
        /// </summary>
        /// <param name="sender">default sender</param>
        /// <param name="e">Finish arguments</param>
        private void Analysis_Finished(object sender, FinishArgs e)
        {
            AnalysisFinishArgs args = (AnalysisFinishArgs)e;

            if (MoveToEnd())
                return;
            
            if (e == null)
            {                             
                Wizard.CurrentStep.PressFinishButton();
                return;
            }

            if (e != null)
            {

                //add code here to finish analysis wizard
                Wizard.CurrentStep.Analysis.NullFinishArguments();

            }

            OnSourceModified(this, e);
            SetWizardToFirstStep();

            this.DeleteSteps();

            Dock.Hide();
        }
        private void DeleteSteps()
        {
            Wizard.DeleteSteps();
            Dock.DeleteSteps();
            //Dock.Dispose();
            //Dock = null;
        }

        /// <summary>
        /// Makes sure references that track the Dock are removed when the Dock is closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Dock_FormClosed(object sender, FormClosedEventArgs e)
        {
            _dock = null;
        }

        /// <summary>
        /// Fire the AnalysisModified event for the controller.
        /// </summary>
        /// <param name="sender">the related WizardAnalsyisPair</param>
        /// <param name="e">Finish arguments</param>
        private void OnSourceModified(Object sender, FinishArgs args)
        {

            if (SourceModified != null)
            {
                SourceModified.Invoke(sender, args);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">The Wizard which has index of step that it is currently on</param>
        /// <param name="e">Step argument which has index of the step it was when the button was originallly pressed</param>
        private void OnStepChanged(object sender, StepArgs e)
        {
            if (StepChanged != null)
            {
                StepChanged.Invoke(sender, e);
            }
        }

        /// <summary>
        /// Fired when the finish button is pushed by a project step. Moves through the steps
        /// until the end is reached or a Stuck point is reached.
        /// </summary>
        /// <param name="sender">default sender</param>
        /// <param name="e">FinishArgs which has a pair of newly created/modified analyses</param>
        private void Project_Finished(object sender, FinishArgs e)
        {
            ProjectFinishArgs args = null;
            if (e is ProjectFinishArgs)
            {
                args = (ProjectFinishArgs)e;
            }
            else
            {
                //SetWizardToFirstStep();
                this.Wizard.CurrentStep.RefreshValues();
                return;
            }
            if (MoveToEnd())
                return;
            //if e = null, then finish was pressed before the end
            //so we need to press the finish button now that
            //we are at the right step
            //which will call this event again but then
            //e will have the analyses we want to create
            if (e == null)
            {
                Wizard.CurrentStep.PressFinishButton();
                return;
            }

            //finish the wizard by adding the analyses that were generated
            if (args != null && args.Analyses != null)
            {
                foreach (Analysis analysis in args.Analyses)
                {
                    analysis.SkillLevel = Project.SkillLevel;
                    analysis.AnalysisType = Project.AnalysisType;
                }

                Wizard.CurrentStep.Project.NullFinishArguments();
            }
            Application.UseWaitCursor = true;
            Cursor.Current = Cursors.WaitCursor;
            OnSourceModified(this, e);

            SetWizardToFirstStep();
            this.Wizard.CurrentStep.RefreshValues();
            //Dock.Hide();
            //Dock.Show();
            
            Application.UseWaitCursor = false;
            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// Fired when an wizard step pushes the next button.
        /// </summary>
        /// <param name="sender">default sender</param>
        /// <param name="e">Step argument</param>
        public void Step_Next(object sender, StepArgs e)
        {
            WizardStep prevStep = Wizard.CurrentStep;
            WizardStep nextStep = null;

            Wizard.NextStep();

            nextStep = Wizard.CurrentStep;
            //Needs to stay in the invent the user imports data and wants to change the project name
            if (Wizard.Stuck && Wizard.CurrentStep is POD.Wizards.Steps.FullAnalysisProjectSteps.ProjectPropertiesStep)
                nextStep = prevStep;

            if (Dock.SkipAnimations == false)
            {
                Dock.SwapSteps = new SwapStepsEventHandler(DoneSwappingSteps);
                Dock.OnTransitionSteps(sender, new SwapStepsEventArgs(prevStep, nextStep, e, 0));

                if(prevStep != nextStep)
                    OnStepChanging(sender, new StepArgs(nextStep.Index, nextStep.HelpFile, Dock.ProgressStepListNode));
            }
            else
            {

                SyncDock();
                //issue #5 occuring here
                OnStepChanged(Wizard, e);
            }

        }

        private void OnStepChanging(object sender, StepArgs e)
        {
            if (StepChanging != null)
            {
                StepChanging.Invoke(sender, e);
            }
        }

        private void DoneSwappingSteps(object sender, SwapStepsEventArgs e)
        {
            SyncDock();
            OnStepChanged(Wizard, e.Args);
        }

        /// <summary>
        /// Fired when an wizard step pushes the previous button.
        /// </summary>
        /// <param name="sender">default sender</param>
        /// <param name="e">Step argument</param>
        public void Step_Previous(object sender, StepArgs e)
        {
            WizardStep prevStep = Wizard.CurrentStep;
            WizardStep nextStep = null;

            Wizard.PrevStep();

            nextStep = Wizard.CurrentStep;

            if (Dock.SkipAnimations == false)
            {
                Dock.SwapSteps = new SwapStepsEventHandler(DoneSwappingSteps);
                Dock.OnTransitionSteps(sender, new SwapStepsEventArgs(prevStep, nextStep, e, 1));

                if (prevStep != nextStep)
                    OnStepChanging(sender, new StepArgs(nextStep.Index, nextStep.HelpFile, Dock.ProgressStepListNode));
            }
            else
            {
                SyncDock();
                OnStepChanged(Wizard, e);
            }
        }

        #endregion


        public string AdditionalWorksheet1Name
        {
            get
            {
                return Wizard.Source.AdditionalWorksheet1Name;
            }
        }
    }
}
