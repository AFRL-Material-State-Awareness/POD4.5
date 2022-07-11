using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POD.Wizards;
using POD.Analyze;
using POD.Docks;
using POD.Data;
using WeifenLuo.WinFormsUI.Docking;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using POD.ExcelData;
using SpreadsheetLight;
using System.Data;

using CSharpBackendWithR;
namespace POD
{
    /// <summary>
    /// Wizard Controller is in charge managing the project and associated analyses.
    /// It also provides a way for the project and analyses to interact with outside
    /// forms.
    /// </summary>
    [Serializable]
    public class WizardController : SkillTypeHolder
    {
        #region Fields

        /// <summary>
        /// A pair of analyses with the corresponding wizard and dock
        /// </summary>
        private WizardDockList _wizards;
        /// <summary>
        /// Allows outside classes to react to a project being changed
        /// </summary>
        [NonSerialized]
        public EventHandler ProjectUpdated;
        /// <summary>
        /// Allows outside classes to react to an analysis being changed
        /// </summary>
        [NonSerialized]
        public EventHandler AnalysisUpdated;
        [NonSerialized]
        public EventHandler NeedSwitchHelpView;
        /// <summary>
        /// Decides how the main wizard controls should be displayed
        /// </summary>
        public ControlOrganize HowToDisplay = ControlOrganize.NaviBottom;
        /// <summary>
        /// Allows outside classes to react to the current step of the wizard being changed
        /// </summary>
        [NonSerialized]
        public StepEventHandler WizardPositionChanged;
        [NonSerialized]
        public StepEventHandler WizardFinalPositionChanged;
        [NonSerialized]
        public StepEventHandler WizardPositionChanging;
        [NonSerialized]
        public EventHandler ExportProject;
        [NonSerialized]
        public GetProjectInfoHandler NeedProjectInfo;
        public GetProjectInfoHandler NeedOpenAnalysis;
        /// <summary>
        /// IronPython engine
        /// </summary>
        [NonSerialized]
        IPy4C _py;
        #endregion
        ///Ask Tom about nonserialized
        ///create a private variable for the Rengine
        [NonSerialized]
        REngineObject _r;
        #region Constructors
        /// <summary>
        /// Create a controller with an empty project and no analyses
        /// </summary>
        public WizardController()
        {
            _wizards = new WizardDockList();
        }     
        #endregion

        #region Properties
        /// <summary>
        /// Get a pair of analysis names found in the project.
        /// </summary>
        public List<string> AnalysisNames
        {
            get
            {
                return _wizards.Names;
            }

        }        
        /// <summary>
        /// Get the project.
        /// </summary>
        public Project Project
        {
            get { return _wizards.Project.Project; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Add a new analysis to the project.
        /// </summary>
        /// <param name="myAnalysis">the analysis to add</param>
        public void AddAnalysis(Analysis myAnalysis)
        {            
            AddAnalysis(myAnalysis, null);            
        }

        // <summary>
        /// Removes the analysis from the project.
        /// </summary>
        /// <param name="analysis"></param>
        private void DeleteAnalysis(Analysis myAnalysis)
        {
            //remove the old version
            if (_wizards.Names.Contains(myAnalysis.Name))
            {
                var dock = _wizards[myAnalysis.Name];

                dock.Analysis.ClearAllEvents();

                _wizards.Remove(dock);

                dock.Dock.Close();
            }
        }

        // <summary>
        /// Removes the analysis from the project.
        /// </summary>
        /// <param name="analysis"></param>
        private void DeleteProject()
        {
            if (_wizards.Project != null)
            {
                _wizards.Project.Dock.Close();
                _wizards.Project = null;
            }
        }

        private void InsertAnalysis(Analysis myAnalysis, int listIndex)
        {
            WizardDockPair pair = null;
            WizardDock myDock = null;

            //if it's not there already
            if (!_wizards.Names.Contains(myAnalysis.Name))
            {
                myAnalysis.SetPythonEngine(_py);
                myAnalysis.SetREngine(_r);
                myAnalysis.ProjectInfoNeeded += GetProjectInfo;
                myAnalysis.ExportProject += ExportProjectToExcel;
                myAnalysis.CreatedAnalysis += CreatedAnalysis_Analysis;

                pair = new WizardDockPair(Wizard.GenerateWizard(myAnalysis, ref HowToDisplay), myDock);

                if (listIndex > -1 && listIndex < _wizards.Count)
                    _wizards.Insert(listIndex, pair);
                else
                    _wizards.Add(pair);

                AddEventHandling(pair);
            }
            else
            {
                pair = _wizards[myAnalysis.Name];
            }

            pair.SyncDock();
            ResyncWorkbookNames();

            OpenExistingAnalsyis(this, new GetProjectInfoArgs(myAnalysis.Name));
        }

        public void AddAnalysis(Analysis myAnalysis, WizardDock myDock)
        {
            WizardDockPair pair = null;

            //if it's not there already
            if (!_wizards.Names.Contains(myAnalysis.Name))
            {
                myAnalysis.SetPythonEngine(_py);
                myAnalysis.SetREngine(_r);
                myAnalysis.ProjectInfoNeeded += GetProjectInfo;
                myAnalysis.ExportProject += ExportProjectToExcel;
                myAnalysis.CreatedAnalysis += CreatedAnalysis_Analysis;

                pair = new WizardDockPair(Wizard.GenerateWizard(myAnalysis, ref HowToDisplay), myDock);
                _wizards.Add(pair);

                if(pair.Wizard != null && pair.Dock != null)
                    AddEventHandling(pair);
            }
            else
            {
                pair = _wizards[myAnalysis.Name];
            }

            if (pair.Wizard != null && pair.Dock != null)
            {
                pair.SyncDock();
                ResyncWorkbookNames();
            }
        }

        private void CreatedAnalysis_Analysis(object sender, AnalysisListArg e)
        {
            var createdFrom = sender as Analysis;
            var listIndex = -1;

            foreach(Analysis analysis in e.Analyses)
            {
                var form = new NewSourceForm(analysis.Name, AnalysisNames);

                form.ShowDialog();

                analysis.Name = form.NewName;
                analysis.UsingCustomName = true;

                if(createdFrom != null)
                {
                    listIndex = AnalysisNames.IndexOf(createdFrom.Name);

                    if (listIndex != -1)
                        listIndex++;
                }
                
                InsertAnalysis(analysis, listIndex);
            }

            OnProjectUpdated(this, e);
        }

        

        private void ExportProjectToExcel(object sender, EventArgs e)
        {
            if(ExportProject != null)
            {
                ExportProject.Invoke(sender, e);
            }
        }

        private void GetProjectInfo(object sender, GetProjectInfoArgs e)
        {
            if(NeedProjectInfo != null)
            {
                NeedProjectInfo.Invoke(sender, e);
            }
            else
            {
                e.ProjectFileName = Project.GenerateFileName();
            }

            e.SourceTable = Project.Sources[e.SourceName].Source.Table;
        }

        private void ResyncWorkbookNames()
        {
            int pairIndex = 1;

            foreach(WizardDockPair pair in _wizards)
            {
                pair.Analysis.WorksheetName = string.Format("{0:00000}", pairIndex);

                pairIndex++;
            }
        }

        public void AddProject(Project myProject)
        {
            AddProject(myProject, null);
        }

        public void AddProject(Project myProject, WizardDock myDock)
        {
            _wizards.Project = new WizardDockPair(Wizard.GenerateWizard(myProject, ref HowToDisplay), myDock);

            AnalysisType = myProject.AnalysisType;
            SkillLevel = myProject.SkillLevel;

            _wizards.Project.Project.NeedAnalyses += NeedAnalaysesAssociatedWithProject;
            _wizards.Project.Project.NeedOpenAnalysis += OpenExistingAnalsyis;

            _wizards.Project.Project.SetPythonEngine(_py);
            _wizards.Project.Project.SetREngine(_r);
            AddEventHandling(_wizards.Project);


            _wizards.Project.SyncDock();

            OnProjectUpdated(this, null);
        }

        private void OpenExistingAnalsyis(object sender, GetProjectInfoArgs e)
        {
            if(NeedOpenAnalysis != null)
            {
                NeedOpenAnalysis.Invoke(sender, e);
            }
        }

        private void NeedAnalaysesAssociatedWithProject(object sender, AnalysisListArg e)
        {
            var project = sender as Project;

            e.Analyses.Clear();

            foreach(WizardDockPair pair in _wizards)
            {
                e.Analyses.Add(pair.Analysis);
            }
        }

        private void AddEventHandling(WizardDockPair myPair)
        {
            if (myPair == _wizards.Project)
            {
                myPair.SourceModified += Analyses_Created;
            }
            else
            {
                myPair.SourceModified += Analysis_Modified;
            }

            //var analysisType = AnalysisTypeEnum.Full;
            //var skillLevel = SkillLevelEnum.Normal;

            
            myPair.Source.SkillLevel = SkillLevel;
            myPair.Source.AnalysisType = AnalysisType;
            myPair.Source.JumpTo += JumpToIndex;
            myPair.Source.NeedSwitchHelpView += SwitchHelpView;

            myPair.StepChanged += Step_Changed;
            myPair.StepChanging += Step_Changing;            
        }

        private void SwitchHelpView(object sender, EventArgs e)
        {
            if(NeedSwitchHelpView != null)
            {
                NeedSwitchHelpView.Invoke(sender, e);
            }
        }

        /*/// <summary>
        /// Raise event to update all of the docks on the main window to match the contents of this dock.
        /// </summary>
        /// <param name="myDock">te dock to syncronize with</param>
        public void UpdateRelatedDocks(WizardDock myDock)
        {
            Wizard wizard = null;

            //if (myDock == _wizards.Project.Dock)
            //{
            //    wizard = _wizards.Project.Wizard;
            //}
            //else
            //{
                WizardDockPair pair = _wizards[myDock];

                if (pair != null)
                    wizard = _wizards[myDock].Wizard;
            //}

            if (wizard != null)
                Step_Changing(wizard, new StepArgs(wizard.CurrentStep.Index, wizard.CurrentStep.HelpFile, myDock.ProgressStepListNode));
        }*/

        /// <summary>
        /// Press Previous or Next button until the step at the specified index is reached
        /// or the Wizard becomes Stuck.
        /// </summary>
        /// <param name="myWizardDock">the Dock associated with the Wizard</param>
        /// <param name="myIndex">the index of the step to move to</param>
        public void ChangeStep(WizardDock myWizardDock, int myIndex)
        {
            WizardDockPair wizard;
            int beforeIndex = -1;
            int afterIndex = 1;

            //project or analysis Dock?
            if (myWizardDock == _wizards.Project.Dock)
            {
                wizard = _wizards.Project;
            }
            else
            {
                wizard = _wizards[myWizardDock];
            }

            //don't want show transition animations here
            wizard.Dock.SkipAnimations = true;

            //Move forward or backward?
            if (myIndex > wizard.Dock.Step.Index)
            {
                while (myIndex != wizard.Dock.Step.Index && beforeIndex != afterIndex)
                {
                    beforeIndex = wizard.Dock.Step.Index;

                    wizard.Wizard.CurrentStep.PressNextButton();

                    afterIndex = wizard.Dock.Step.Index;
                }
            }
            else
            {
                while (myIndex != wizard.Dock.Step.Index && beforeIndex != afterIndex)
                {
                    StepArgs stepArgs = new StepArgs(wizard.Dock.Step.Index);
                    beforeIndex = stepArgs.Index;

                    wizard.Wizard.CurrentStep.PressPreviousButton();

                    afterIndex = wizard.Dock.Step.Index;
                }
            }

            //start showing them again
            wizard.Dock.SkipAnimations = false;

            var args = new StepArgs(afterIndex);
            OnWizardFinalPositionUpdated(this, args);
        }
      
        /// <summary>
        /// Create a dock for the specified analysis.
        /// </summary>
        /// <param name="mySourceName">name of the analysis</param>
        /// <returns>the analysis wizard's dock</returns>
        public WizardDockPair CreateWizardDock(string mySourceName)
        {
            var pair = _wizards[mySourceName];

            if (pair != null)
            {
                return CreateWizardDock(pair.Source);
            }

            return null;
        }
        /// <summary>
        /// Create a dock for the specified analysis.
        /// </summary>
        /// <param name="mySource">the analysis</param>
        /// <returns>the analysis wizard's dock</returns>
        public WizardDockPair CreateWizardDock(WizardSource mySource)
        {
            if (mySource != null)
            {
                WizardDockPair pair = _wizards[mySource];

                if (pair != null)
                {
                    pair.SyncDock();
                    return pair; 
                }

                return null;
            }

            return null;
        }
        

        /// <summary>
        /// Sets the controller's project.
        /// </summary>
        /// <param name="myProject"></param>
        
        #endregion

        #region Event Handling
        /// <summary>
        /// Fired when analyses have been created by the project wizard finishing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Analyses_Created(object sender, FinishArgs e)
        {
            ProjectFinishArgs args = (ProjectFinishArgs)e;

            //finish the wizard by adding the analyses that were generated
            //to the controller
            if (args != null && args.Analyses != null)
            {
                foreach (Analysis analysis in args.Removed)
                {
                    DeleteAnalysis(analysis);
                }

                foreach (Analysis analysis in args.Analyses)
                {
                    AddAnalysis(analysis);
                }

                args.Removed.Clear();
            }

            OnProjectUpdated(this, args);
        }


        /// <summary>
        /// Fired when analysis has modified by the analysis wizard finishing.
        /// </summary>
        /// <param name="sender">default sender</param>
        /// <param name="e">Analysis Finish argument</param>
        protected void Analysis_Modified(object sender, FinishArgs e)
        {
            AnalysisFinishArgs args = (AnalysisFinishArgs)e;

            //force project properties step to have the latest values every time a wizard is finished
            _wizards.Project.Wizard.FirstStep.RefreshValues();

            //let Wizard Controller know
            OnAnalysisUpdated(this, args);
        }
        /// <summary>
        /// Notify the Wizard Controller that the analysis has been updated.
        /// </summary>
        /// <param name="sender">the asssociated WizardDock pair</param>
        /// <param name="e">Analysis Finish argument</param>
        protected void OnAnalysisUpdated(object sender, EventArgs e)
        {
            if (AnalysisUpdated != null)
            {
                AnalysisUpdated.Invoke(sender, e);
            }
        }
        /// <summary>
        /// Notify the Wizard Controller that the project has been updated.
        /// </summary>
        /// <param name="sender">the asssociated WizardDock pair</param>
        /// <param name="e">Project Finish argument</param>
        protected void OnProjectUpdated(object sender, EventArgs e)
        {
            if (ProjectUpdated != null)
            {
                ProjectUpdated.Invoke(sender, e);
            }
        }
        /// <summary>
        /// Notify the Wizard Controller that the current step of the Wizard has changed 
        /// </summary>
        /// <param name="sender">default sender</param>
        /// <param name="e">Step argument</param>
        private void OnWizardPositionUpdated(object sender, StepArgs e)
        {
            if (WizardPositionChanged != null)
                WizardPositionChanged.Invoke(sender, e);
        }
        /// <summary>
        /// Notify the Wizard Controller that the current step of the Wizard has changed 
        /// </summary>
        /// <param name="sender">default sender</param>
        /// <param name="e">Step argument</param>
        private void OnWizardFinalPositionUpdated(object sender, StepArgs e)
        {
            if (WizardFinalPositionChanged != null)
                WizardFinalPositionChanged.Invoke(sender, e);
        }
        /// <summary>
        /// Update the argument with the current step index then notify Wizard Controller.
        /// </summary>
        /// <param name="sender">the default sender</param>
        /// <param name="e">Step argument</param>
        private void Step_Changed(object sender, StepArgs e)
        {
            if (sender != null)
            {
                e.Index = ((Wizard)sender).CurrentStep.Index;
                ((Wizard)sender).CurrentStep.RefreshValues();
            }

            OnWizardPositionUpdated(sender, e);
        }
        private void Step_Changing(object sender, StepArgs e)
        {
            if (WizardPositionChanging != null)
                WizardPositionChanging.Invoke(sender, e);
        }

        private void JumpToIndex(object sender, StepArgs e)
        {
            var source = (WizardSource)sender;

            ChangeStep(_wizards[source].Dock, e.Index);
        }
        #endregion        
     
        public bool Serialize(string myFile)
        {
            var stream = File.Open(myFile, FileMode.Create);
            var bin = new BinaryFormatter();
            var analyses = new List<Analysis>();

            foreach(WizardDockPair pair in _wizards)
            {
                analyses.Add(pair.Analysis);
            }

            if (_wizards.Project.Project.AnalysisType == AnalysisTypeEnum.Quick)
            {
                if(analyses[0].Data.ActivatedFlaws.Rows.Count == 0)
                {
                    MessageBox.Show("Please run a valid analysis with at least 8 data points \nbefore saving your quick analysis.", "POD v4 Error");
                    return false;
                }
            }

            bin.Serialize(stream, CreateInfoString(myFile, _wizards.Project.Project, analyses, false));

            bin.Serialize(stream, _wizards.Project.Source);

            foreach(WizardDockPair pair in _wizards)
            {
                bin.Serialize(stream, pair.Source);
            }

            stream.Close();

            return true;
        }

        public static string CreateInfoString(string myFile, Project project, List<Analysis> myAnalyses, bool useFileTimeStamp)
        {

            if (project == null || myAnalyses == null)
                return "";

            if (project.AnalysisType != AnalysisTypeEnum.Quick)
            {

                var analysisInfo = "ANALYSIS INFO:" + Environment.NewLine;
                var fileInfo = "FILE INFO:" + Environment.NewLine;

                var fileAccessDate = "";
                var fileAccessTime = "";

                if (useFileTimeStamp)
                {
                    fileAccessDate = File.GetLastAccessTime(myFile).ToLongDateString();
                    fileAccessTime = File.GetLastAccessTime(myFile).ToLongTimeString();

                }
                else
                {
                    fileAccessDate = DateTime.Now.ToLongDateString();
                    fileAccessTime = DateTime.Now.ToLongTimeString();
                }

                fileInfo += "File Location:\t" + Path.GetDirectoryName(myFile) + Environment.NewLine;
                fileInfo += "Last Accessed:\t" + fileAccessDate + ", " + fileAccessTime + Environment.NewLine + Environment.NewLine;

                foreach (Analysis analysis in myAnalyses)
                {
                    analysisInfo += "Name:\t" + analysis.Name + Environment.NewLine +
                                    "Type:\t" + Globals.GetLongVersion(analysis.AnalysisDataType) + Environment.NewLine + Environment.NewLine;
                }

                var projectInfo = fileInfo + "PROJECT INFO:" + Environment.NewLine;

                var name = project.Properties.Name;
                var parent = project.Properties.Parent;
                var analyst = project.Properties.Analyst.Name;
                var analystCompany = project.Properties.Analyst.Company;
                var customer = project.Properties.Customer.Name;
                var customerCompany = project.Properties.Customer.Company;
                var notes = project.Properties.Notes;

                var sourceCount = project.Sources.Count.ToString();

                var sourceNames = new List<string>();
                var sourceType = new List<string>();
                var headerNames = new List<string>();
                var rowCounts = new List<string>();

                var sourceInfo = "DATA SOURCE INFO:" + Environment.NewLine +
                                 "# of Data Sources:\t" + sourceCount + Environment.NewLine;

                projectInfo += "Project Name:\t\t" + name + Environment.NewLine +
                              "Parent Project Name:\t" + parent + Environment.NewLine +
                              "Analyst:\t\t\t" + analyst + Environment.NewLine +
                              "Analyst Company:\t" + analystCompany + Environment.NewLine +
                              "Customer POC:\t\t" + customer + Environment.NewLine +
                              "Customer Company:\t" + customerCompany + Environment.NewLine +
                              "Notes:" + Environment.NewLine + notes + Environment.NewLine + Environment.NewLine;


                foreach (DataSource source in project.Sources)
                {
                    sourceNames.Add(source.SourceName);
                    sourceType.Add(source.AnalysisDataType.ToString());

                    if (source.Original != null)
                    {

                        var headerName = "";

                        if (source.Original.Columns.Count > 0)
                        {
                            foreach (DataColumn col in source.Original.Columns)
                            {
                                headerName += col.ColumnName + ", ";
                            }

                            headerName = headerName.Substring(0, headerName.Length - 2);
                        }
                        else
                        {
                            headerName = "No data table associated with this source.";
                        }

                        headerNames.Add(headerName);

                        rowCounts.Add(source.Original.Rows.Count.ToString());
                    }
                    else
                    {
                        headerNames.Add("No data table associated with this source.");

                        rowCounts.Add("0");
                    }

                    var currentSourceInfo = "Source Name:\t\t" + sourceNames.Last() + Environment.NewLine +
                                            "Source Type:\t\t" + sourceType.Last() + Environment.NewLine +
                                            "Row Count:\t\t" + rowCounts.Last() + Environment.NewLine +
                                            "Column Headers:" + Environment.NewLine + headerNames.Last() + Environment.NewLine + Environment.NewLine;

                    sourceInfo += currentSourceInfo;
                    
                }

                sourceInfo += analysisInfo;

                projectInfo += sourceInfo;

                return projectInfo;
            }
            else
            {
                Analysis analysis = myAnalyses[0];
                string projectInfo = "";

                projectInfo += "Operator:\t\t" + analysis.Operator + Environment.NewLine + Environment.NewLine;
                projectInfo += "Specimen Set:\t\t" + analysis.SpecimenSet + Environment.NewLine;
                projectInfo += "Specimen Range:\t" + analysis.InFlawMin.ToString("F3") + " - " + analysis.InFlawMax.ToString("F3") +
                               " (" + analysis.SpecimentUnit + ")" + Environment.NewLine + Environment.NewLine;

                if(analysis.Data.DataType == AnalysisDataTypeEnum.AHat)
                {
                    projectInfo += "Instrument:\t\t" + analysis.Instrument + Environment.NewLine;
                    projectInfo += "Instrument Range:\t" + analysis.InResponseMin.ToString("F3") + " - " + analysis.InResponseMax.ToString("F3") + 
                                   " (" + analysis.InstrumentUnit + ")" + Environment.NewLine + Environment.NewLine;
                }

                projectInfo += "Data Point Count:\t" + analysis.Data.ActivatedSpecimenIDs.Rows.Count + Environment.NewLine + Environment.NewLine;

                projectInfo += "ID\t\tFlaw\t\tResponse" + Environment.NewLine + Environment.NewLine;

                var index = 0;
                foreach(DataRow row in analysis.Data.ActivatedSpecimenIDs.Rows)
                {
                    projectInfo += row[0].ToString() + "\t\t";

                    var flaw = analysis.Data.ActivatedFlaws.Rows[index][0].ToString();
                    var flawValue = 0.0;

                    if(Double.TryParse(flaw, out flawValue))
                        projectInfo += analysis.Data.InvertTransformedFlaw(flawValue).ToString("F3") + "\t\t";
                    else
                        projectInfo += flaw + "\t\t";

                    var response = analysis.Data.ActivatedResponses.Rows[index][0].ToString();
                    var responseValue = 0.0;

                    if (Double.TryParse(response, out responseValue))
                        projectInfo += analysis.Data.InvertTransformedResponse(responseValue).ToString("F3") + Environment.NewLine;
                    else
                        projectInfo += response + Environment.NewLine;

                    index++;
                }

                return projectInfo;

            }

            
        }

        public static string DeserializeProjectInfo(string myFile)
        {
            var stream = File.Open(myFile, FileMode.Open);
            var bin = new BinaryFormatter();
            var analyses = new List<Analysis>();

            Project project = null;

            
            try
            {
                project = (Project)bin.Deserialize(stream);
            }
            catch
            {
                stream.Position = 0;
                string info = (String)bin.Deserialize(stream);

                stream.Close();

                return info;
            }

            try
            {
                while (true)
                {
                    var analysis = (Analysis)bin.Deserialize(stream);

                    analyses.Add(analysis);
                }
            }
            catch (SerializationException exp)
            {
                
            }
            finally
            {
                stream.Close();
            }

            return CreateInfoString(myFile, project, analyses, true);
        }

        public void Deserialize(string myFile, List<WizardDock> myDocks)
        {
            Stream stream = File.Open(myFile, FileMode.Open);
            BinaryFormatter bin = new BinaryFormatter();
            List<string> dockNames = new List<string>();
            

            foreach(WizardDock dock in myDocks)
            {
                dockNames.Add(dock.Label);
            }

            Project project = null;

            try
            {
                project = (Project)bin.Deserialize(stream);
            }
            catch
            {
                try
                {
                    stream.Position = 0;
                    var info = (String)bin.Deserialize(stream);
                    project = (Project)bin.Deserialize(stream);
                }
                catch(Exception exp)
                {
                    MessageBox.Show(exp.Message, "Error Loading File");

                    stream.Close();
                }
            }

            if (project != null)
            {

                if (dockNames.Contains(project.Name) == true)
                {
                    WizardDock dock = myDocks[dockNames.IndexOf(project.Name)];
                    AddProject(project, dock);
                }
                else
                {
                    AddProject(project);
                }

                try
                {
                    Analysis analysis = (Analysis)bin.Deserialize(stream);

                    while (true)
                    {
                        if (dockNames.Contains(analysis.Name) == true)
                        {
                            WizardDock dock = myDocks[dockNames.IndexOf(analysis.Name)];
                            AddAnalysis(analysis, dock);
                        }
                        else
                        {
                            AddAnalysis(analysis);
                        }

                        analysis = (Analysis)bin.Deserialize(stream);
                    }
                }
                catch
                {
                     
                }
                finally
                {
                    stream.Close();
                }
            }
        }

        private void AddProject(POD.Project project, bool createDock)
        {
            if(!createDock)
            {
                AddGUIlessProject(project);
            }
            else
            {
                AddProject(project);
            }
        }

        private void AddGUIlessProject(POD.Project project)
        {
            _wizards.Project = new WizardDockPair(Wizard.GenerateWizard(project, ref HowToDisplay), null);

            AnalysisType = project.AnalysisType;
            SkillLevel = project.SkillLevel;

            _wizards.Project.Project.NeedAnalyses += NeedAnalaysesAssociatedWithProject;
            _wizards.Project.Project.NeedOpenAnalysis += OpenExistingAnalsyis;

            _wizards.Project.Project.SetPythonEngine(_py);
            _wizards.Project.Project.SetREngine(_r);
            AddEventHandling(_wizards.Project);


            _wizards.Project.SyncDock();

            OnProjectUpdated(this, null);
        }

        private void AddAnalysis(Analysis analysis, bool createDock)
        {
            if (!createDock)
            {

            }
            else
            {
                AddAnalysis(analysis);
            }
        }

        public List<WizardDock> AllWizardDocks
        {
            get
            {
                List<WizardDock> docks = new List<WizardDock>();

                docks.Add(_wizards.Project.Dock);

                foreach(WizardDockPair pair in _wizards)
                {
                    docks.Add(pair.Dock);
                }

                return docks;
            }
        }

        public List<string> DataSourceNames
        {
            get
            {
                return _wizards.Project.Project.Sources.Names;
            }
        }

        public void SetPythonEngine(IPy4C myPy)
        {
            _py = myPy;
        }
        //set R Engine along with python
        public void SetREngine(REngineObject myREngine)
        {
            _r = myREngine;
        }
        public bool AutoOpen(string myName)
        {
            return _wizards[myName].Analysis.AutoOpen;
        }

        public void WriteToExcel(ExcelExport myWriter)
        {
            Application.UseWaitCursor = true;

            Project.WriteToExcel(myWriter);

            myWriter.Workbook.AddWorksheet("Analysis Table of Contents");

            int rowIndex = 1;
            int colIndex = 1;

            myWriter.SetCellValue(rowIndex, colIndex++, "Analysis Name");
            myWriter.SetCellValue(rowIndex, colIndex++, "Worksheet Name");
            myWriter.SetCellValue(rowIndex, colIndex++, "Info Link");
            myWriter.SetCellValue(rowIndex, colIndex++, "Results Link");
            myWriter.SetCellValue(rowIndex, colIndex++, "Residuals Link");
            myWriter.SetCellValue(rowIndex, colIndex++, "POD Link");
            myWriter.SetCellValue(rowIndex, colIndex++, "Additional Link");
            myWriter.SetCellValue(rowIndex, colIndex++, "Removed Points Link");

            rowIndex++;
            
            foreach (WizardDockPair pair in _wizards)
            {
                colIndex = 1;

                myWriter.SetCellValue(rowIndex, colIndex++, pair.Analysis.Name);
                myWriter.SetCellValue(rowIndex, colIndex++, pair.Analysis.WorksheetName);
                myWriter.InsertAnalysisWorksheetLink(rowIndex, colIndex++, pair.Analysis.WorksheetName, "Info");
                myWriter.InsertAnalysisWorksheetLink(rowIndex, colIndex++, pair.Analysis.WorksheetName, "Results");
                myWriter.InsertAnalysisWorksheetLink(rowIndex, colIndex++, pair.Analysis.WorksheetName, "Residuals");
                myWriter.InsertAnalysisWorksheetLink(rowIndex, colIndex++, pair.Analysis.WorksheetName, "POD");
                myWriter.InsertAnalysisWorksheetLink(rowIndex, colIndex++, pair.Analysis.WorksheetName, pair.AdditionalWorksheet1Name);
                myWriter.InsertAnalysisWorksheetLink(rowIndex, colIndex++, pair.Analysis.WorksheetName, "Removed Points");

                rowIndex++;
            }

            myWriter.Workbook.AutoFitColumn(1, colIndex-1);

            Project.WriteDataToExcel(myWriter);

            foreach(WizardDockPair pair in _wizards)
            {
                pair.Analysis.WriteToExcel(myWriter);
            }

            Project.WriteDataPropertiesToExcel(myWriter);

            myWriter.Workbook.SelectWorksheet("Project");

            Application.UseWaitCursor = false;
        }

        public void ClosePyEngine()
        {
            _py.Close();
        }

        public void ClearEverything()
        {
            DeleteProject();

            var analyses = new List<Analysis>();

            foreach (WizardDockPair pair in _wizards)
            {
                analyses.Add(pair.Analysis);
            }

            foreach(Analysis analysis in analyses)
            {
                DeleteAnalysis(analysis);
            }
        }

        public WizardDock ProjectDock
        {
            get
            {
                return _wizards.Project.Dock;
            }
        }
    }
}
