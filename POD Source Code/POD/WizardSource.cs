using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//TODO: could make r engine global instead
using CSharpBackendWithR;
namespace POD
{

    /// <summary>
    /// The object the wizard is manipulating. Either an analysis or project object.
    /// </summary>
    [Serializable]
    abstract public class WizardSource : PODObject
    {


        #region Fields
        /// <summary>
        /// Data passed to method handling the finish event.
        /// </summary>
        [NonSerialized]
        private FinishArgs _finishArg;
        /// <summary>
        /// Name of the source.
        /// </summary>
        private String _name;
        /// <summary>
        /// The Node whose children are used to create the Step List for the Wizard Progress Dock
        /// </summary>
        [NonSerialized]
        private TreeNode _progressListNode;
        /// <summary>
        /// Move to a specific index.
        /// </summary>
        [NonSerialized]
        public StepEventHandler JumpTo;
        /// <summary>
        /// Next button clicked.
        /// </summary>
        [NonSerialized]
        public StepEventHandler StepNext;
        /// <summary>
        /// Previous button clicked.
        /// </summary>
        [NonSerialized]
        public StepEventHandler StepPrevious;
        /// <summary>
        /// Wizard finished.
        /// </summary>
        [NonSerialized]
        public FinishEventHandler WizardFinished;
        [NonSerialized]
        public EventHandler NeedSwitchHelpView;

        /// <summary>
        /// Name of the Navigate menu item last clicked;
        /// </summary>
        private string _lastNavigateClicked = "";

        /// <summary>
        ///     Used to call Python code/libraries
        /// </summary>
        [NonSerialized]
        protected IPy4C _python;

        //return the python engine class
        public IPy4C Python
        {
            get { return _python; }
        }
        /// <summary>
        /// used to call the r code
        /// </summary>
        /// 
        [NonSerialized]
        protected REngineObject _rDotNet;
        public REngineObject RDotNet
        {
            get { return _rDotNet;  }
        }

        public virtual string AdditionalWorksheet1Name
        {
            get
            {
                return Globals.NotApplicable;
            }
        }
        
        /// <summary>
        /// Reference to the CPodDoc Python class
        /// </summary>
        [NonSerialized]
        protected dynamic _podDoc;

        /// <summary>
        /// Reference to the object transform class in csharpbackend
        /// </summary>
        [NonSerialized]
        protected dynamic _hmAnalysisObject;

        #endregion

        #region Constructors
        #endregion

        #region Properties
        /// <summary>
        /// Contains data needed by the Finish event handling method.
        /// </summary>

        public FinishArgs FinishArg
        {
            get
            {
                return _finishArg;
            }
            set
            {
                _finishArg = value;
            }
        }
        /// <summary>
        /// Get/set name of the wizard's source.
        /// </summary>
        public virtual String Name
        {
            get { return _name; }
            set { _name = value; }
        }        
        /// <summary>
        /// Get/set the Node whose children are used to make the Step List for the Wizard Progress Dock.
        /// </summary>
        
        public TreeNode ProgressStepListNode
        {
            get
            {
                return _progressListNode;
            }

            set
            {
                _progressListNode = value;
            }
        }
        #endregion

        #region Methods
        #endregion

        #region Event Handling
        /// <summary>
        /// Raise the add snapshot event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnAddSnapshot(object sender, EventArgs e)
        {

        }


        public void SwitchHelpView(object sender, EventArgs e)
        {
            if(NeedSwitchHelpView != null)
            {
                NeedSwitchHelpView.Invoke(sender, e);
            }
        }

        /// <summary>
        /// Raise the next step event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnStepNext(object sender, StepArgs e)
        {
            if (StepNext != null)
                StepNext.Invoke(sender, e);
        }
        /// <summary>
        /// Raise the previous step event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnStepPrev(object sender, StepArgs e)
        {
            if (StepPrevious != null)
                StepPrevious.Invoke(sender, e);
        }
        /// <summary>
        /// Raise the step reset event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnStepReset(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// Raise the wizard cancel event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnWizardCancel(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// Raise the wizard finish event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnWizardFinish(object sender, FinishArgs e)
        {
            if (WizardFinished != null)
            {
                WizardFinished.Invoke(sender, e);
            }
        }
        public void OnJumpTo(Object sender, StepArgs e)
        {
            if(JumpTo != null)
            {
                JumpTo.Invoke(sender, e);
            }
        }

        public virtual void SetPythonEngine(IPy4C myPy)
        {
            _python = myPy;
            //initializes a new instance of the cPODDoc class in the .py file
            if(_podDoc == null)
                _podDoc = _python.CPodDoc(Name);
        }
        public virtual void SetREngine(REngineObject myREngine)
        {
            _rDotNet = myREngine;
            if (_hmAnalysisObject == null)
            {
                _hmAnalysisObject = new HMAnalysisObjectTransform(Name);
            }
        }
    
        #endregion

        /// <summary>
        /// Nulls the finish argument so we can check again
        /// if the Finish button was pressed early.
        /// </summary>
        public void NullFinishArguments()
        {
            FinishArg = null;
        }

        public string GetNavigateLastClick()
        {
            return _lastNavigateClicked;
        }

        public void SetNavigateLastClick(string value)
        {
            _lastNavigateClicked = value;
        }

        public bool LockBusy { get; set; }

        public bool IsBusy { get; set; }

        public virtual string GenerateFileName()
        {
            return "";
        }
    }
}
