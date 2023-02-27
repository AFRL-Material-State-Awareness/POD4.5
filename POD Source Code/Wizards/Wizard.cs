using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POD.Analyze;
using System.Windows.Forms;
using POD.Controls;

namespace POD.Wizards
{
    /// <summary>
    /// Base class for all Wizards. Provides basic functionality for the Wizards like
    /// manipulating the Step List.
    /// </summary>
    abstract public class Wizard
    {
        #region Fields
        //protected List<ContextMenuStripConnected> _openStrips = new List<ContextMenuStripConnected>();
        /// <summary>
        /// The Wizard's current Step.
        /// </summary>
        protected WizardStep _currentStep;
        /// <summary>
        /// List of Wizard Steps.
        /// </summary>
        protected WizardStepList _list;
        /// <summary>
        /// Did the Wizard try to go past the first step?
        /// </summary>
        protected bool _passedStart;
        /// <summary>
        /// Did the Wizard try to go past the last step?
        /// </summary>
        protected bool _passedEnd;
        /// <summary>
        /// The object the Wizard is manipulating.
        /// </summary>
        protected WizardSource _source;
        protected ControlOrganize _howToDisplay;
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new Wizard with an empty list.
        /// </summary>
        public Wizard()
        {
            _list = new WizardStepList();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Get/set the current Step of the Wizard.
        /// </summary>
        public WizardStep CurrentStep
        {
            get { return _currentStep; }
            set
            {
                _currentStep = value;
            }
        }
        /// <summary>
        /// Get the first Step found in the Step List.
        /// </summary>
        public WizardStep FirstStep
        {
            get
            {
                return _list.First();
            }
        }
        /// <summary>
        /// Get the last Step of the Step List.
        /// </summary>
        public WizardStep LastStep
        {
            get
            {
                return _list.Last();
            }
        }
        /// <summary>
        /// Get the name of the Wizard.
        /// </summary>
        public string Name
        {
            get
            {
                return _source.Name;
            }
        }
        /// <summary>
        /// Get if the Wizard tried to go passed the end of the Wizard on the last Step movement.
        /// </summary>
        public bool PassedEnd
        {
            get { return _passedEnd; }
            set { _passedEnd = value; }
        }
        /// <summary>
        /// Get if the Wizard tried to go passed the start of the Wizard on the last Step movement.
        /// </summary>
        public bool PassedStart
        {
            get { return _passedStart; }
            set { _passedStart = value; }
        }
        /// <summary>
        /// Get the object that the Wizard is manipulating.
        /// </summary>
        public WizardSource Source
        {
            get
            {
                return _source;
            }
        }
        /// <summary>
        /// Get the Index of the current Step.
        /// </summary>
        public int StepIndex
        {
            get
            {
                return _currentStep.Index;
            }
        }
        /// <summary>
        /// Is the current Step stuck and cannot continue to the next step found in the list?
        /// </summary>
        public bool Stuck
        {
            get
            {
                return CurrentStep.Stuck;
            }
        }
        #endregion

        #region Methods

        public virtual void AddSteps()
        {

        }

        /// <summary>
        /// Add a new Step to the end of the Step List.
        /// </summary>
        /// <param name="myStep">the Step to add</param>
        protected void AddStep(WizardStep myStep)
        {
            myStep.Index = _list.Count;
            myStep.UpdateTitleBarToolTip();
            _list.Add(myStep);
            //myStep.ContextMenuList = _openStrips;
        }
        /// <summary>
        /// Creates the Step List Node based on the current Steps in the Wizard.
        /// </summary>
        public void CreateWizardProgressStepListNode()
        {
            TreeNode start = new TreeNode();

            foreach (WizardStep step in _list)
            {
                start.Nodes.Add(new TreeNode(step.Header));
            }

            Source.ProgressStepListNode = start;

            foreach (WizardStep step in _list)
            {
                step.InitializeStep(this.GetType().Name, ref _howToDisplay);
            }
        }
        /// <summary>
        /// Generate an appropriate new wizard based on the POD object
        /// </summary>
        /// <param name="mySource">the source used to create the wizard</param>
        /// <returns></returns>
        public static Wizard GenerateWizard(PODObject mySource, ref ControlOrganize myOrganize)
        {
            Wizard wizard = null;

            try
            {

                switch (mySource.WizardType)
                {
                    case WizardEnum.ProjectTutorial:
                        wizard = new FullAnalysisProjectWizard((Project)mySource, ref myOrganize);
                        break;
                    case WizardEnum.ProjectTraining:
                        wizard = new FullAnalysisProjectWizard((Project)mySource, ref myOrganize);
                        break;
                    case WizardEnum.ProjectNormal:
                        wizard = new FullAnalysisProjectWizard((Project)mySource, ref myOrganize);
                        break;
                    case WizardEnum.AHatTutorial:
                        break;
                    case WizardEnum.AHatBeginner:
                        wizard = new AHatvsANormalWizard((AHatAnalysis)mySource, ref myOrganize);
                        break;
                    case WizardEnum.AHatIntermediate:
                        wizard = new AHatvsANormalWizard((AHatAnalysis)mySource, ref myOrganize);
                        break;
                    case WizardEnum.AHatExpert:
                        wizard = new AHatvsANormalWizard((AHatAnalysis)mySource, ref myOrganize);
                        break;
                    case WizardEnum.HitMissTutorial:
                        wizard = new HitMissNormalWizard((HitMissAnalysis)mySource, ref myOrganize);
                        break;
                    case WizardEnum.HitMissBeginner:
                        wizard = new HitMissNormalWizard((HitMissAnalysis)mySource, ref myOrganize);
                        break;
                    case WizardEnum.HitMissIntermediate:
                        wizard = new HitMissNormalWizard((HitMissAnalysis)mySource, ref myOrganize);
                        break;
                    case WizardEnum.HitMissExpert:
                        wizard = new HitMissNormalWizard((HitMissAnalysis)mySource, ref myOrganize);
                        break;
                    case WizardEnum.HitMissSolveAllAnalyses:
                        break;
                    case WizardEnum.QuickAHatTutorial:
                        break;
                    case WizardEnum.QuickAHatBeginner:
                        wizard = new AHatvsAQuickWizard((AHatAnalysis)mySource, ref myOrganize);
                        break;
                    case WizardEnum.QuickHitMissTutorial:

                        break;
                    case WizardEnum.QuickHitMissBeginner:
                        wizard = new HitMissQuickWizard((HitMissAnalysis)mySource, ref myOrganize);
                        break;
                    case WizardEnum.None:
                        break;
                    default:
                        break;
                }
            }
            catch
            {
                MessageBox.Show("Invalid analysis type. Wizard creation failed.", "POD v4 Error");
            }

            return wizard;
        }
        /// <summary>
        /// Attempt to move the current Step to the next step in the list.
        /// </summary>
        public void NextStep()
        {
            _currentStep = _list.NextStep(_currentStep, ref _passedEnd);

            if (!_currentStep.Stuck)
                _currentStep.PrepareGUI();
        }
        /// <summary>
        /// Attempt to move the current step to the previous step in the list.
        /// </summary>
        public void PrevStep()
        {
            _currentStep = _list.PrevStep(_currentStep, ref _passedStart);

            _currentStep.PrepareGUI();
        }
        ///// <summary>
        ///// Refresh all the controls of the current Step with current Wizard values.
        ///// </summary>
        //public void RefreshValues()
        //{
        //    if (CurrentStep != null)
        //        CurrentStep.RefreshValues();
        //}
        /// <summary>
        /// Create a new Step List.
        /// </summary>
        protected void StartNewStepList()
        {
            _list = new WizardStepList();
        }
        public void DeleteSteps()
        {

            foreach (var step in _list)
            {
                if((step is Steps.HitMissNormalSteps.ChooseTransformStep) ||
                    (step is POD.Wizards.Steps.HitMissNormalSteps.FullRegressionStep) ||
                    (step is Steps.HitMissNormalSteps.DocumentRemovedStep) ||
                    (step is Steps.AHatVsANormalSteps.ChooseTransformStep) ||
                    (step is Steps.AHatVsANormalSteps.FullRegressionStep) ||
                    (step is Steps.AHatVsANormalSteps.DocumentRemovedStep)
                    )
                {
                step.Dispose();
                }
            }
            //the list needs to retain the disposed objects
            //the steps will be overwritten if the given analysis is reopened
            //_list.Clear();
        }

        #endregion

        #region Event Handling
        #endregion

        public void FixPanelControlSizes()
        {
            foreach(WizardStep step in _list)
            {
                step.FixPanelControlSizes();
            }
        }
    }
}
