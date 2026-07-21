// <copyright file="WizardController.cs" company="The University of Dayton">
// Copyright © 2025 The University of Dayton. All rights reserved.
//
// Licensed under the GNU General Public License, Version 3.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.gnu.org/licenses/gpl-3.0.txt
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

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
using System.Security;
using System.Diagnostics;

namespace POD
{
    namespace Security
    {
        public class WizardControllerSerializationBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                // The most secure approach is to check for exact Type Names (Equals).
                // We use StartsWith for Generics (List, Dictionary) because their full type strings
                // include versioning and are extremely long/dynamic.

                bool isAuthorized =
                // --- Core POD Types (Sorted Alphabetically) ---
                typeName.Equals("Data.UpdateTableControl") ||
                typeName.Equals("POD.Analyze.AHatAnalysis") ||
                typeName.Equals("POD.Analyze.Analysis") ||
                typeName.Equals("POD.Analyze.HitMissAnalysis") ||
                typeName.Equals("POD.AnalysisDataTypeEnum") ||
                typeName.Equals("POD.AnalysisTypeEnum") ||
                typeName.Equals("POD.ConfidenceIntervalTypeEnum") ||
                typeName.Equals("POD.Data.AnalysisData") ||
                typeName.Equals("POD.Data.ColumnInfo") ||
                typeName.Equals("POD.Data.DataPointIndex") ||
                typeName.Equals("POD.Data.DataSource") ||
                typeName.Equals("POD.Data.DataSources") ||
                typeName.Equals("POD.Data.DataTableWithRanges") ||
                typeName.Equals("POD.Data.SourceInfo") ||
                typeName.Equals("POD.Data.SourceInfos") ||
                typeName.Equals("POD.Data.TableRange") ||
                typeName.Equals("POD.FlipBinarySign") ||
                typeName.Equals("POD.HitMissRegressionType") ||
                typeName.Equals("POD.Person") ||
                typeName.Equals("POD.PODObjectTypeEnum") ||
                typeName.Equals("POD.Project") ||
                typeName.Equals("POD.ProjectProperties") ||
                typeName.Equals("POD.RCalculationType") ||
                typeName.Equals("POD.SamplingTypeEnum") ||
                typeName.Equals("POD.SkillLevelEnum") ||
                typeName.Equals("POD.TransformTypeEnum") ||

                // --- R/Python Backend Types (Sorted Alphabetically) ---
                typeName.Equals("CSharpBackendWithR.AHatAnalysisObject") ||
                typeName.Equals("CSharpBackendWithR.BoxCoxAnalysisResult") ||
                typeName.Equals("CSharpBackendWithR.BoxCoxTransformer") ||
                typeName.Equals("CSharpBackendWithR.DataTransformer") ||
                typeName.Equals("CSharpBackendWithR.HMAnalysisObject") ||
                typeName.Equals("CSharpBackendWithR.TransformationDataSnapshot") ||

                // --- System Types (Generic Containers - Sorted Alphabetically) ---
                typeName.StartsWith("System.Collections.Generic.Dictionary") ||
                typeName.StartsWith("System.Collections.Generic.GenericEqualityComparer") ||
                typeName.StartsWith("System.Collections.Generic.KeyValuePair") ||
                typeName.StartsWith("System.Collections.Generic.List") ||

                // --- System Types (Data/Primitives - Sorted Alphabetically) ---
                typeName.Contains("System.Lazy") ||
                typeName.StartsWith("System.Data.DataColumn") ||
                typeName.StartsWith("System.Data.DataRow") ||
                typeName.StartsWith("System.Data.DataTable") ||
                typeName.Equals("System.DateTime") ||
                typeName.Equals("System.String") ||
                typeName.Equals("System.UnitySerializationHolder") ||
                typeName.Equals("System.Version");

                if (isAuthorized)
                {
                    return Type.GetType($"{typeName}, {assemblyName}");
                }

                // Copy missing type to clipboard to help developer expand the whitelist
                try
                {
                    if (Debugger.IsAttached)
                    {
                        Clipboard.SetText($"\r\n\t\t\t\ttypeName.Equals(\"{typeName}\") ||");
                    }
                }
                catch { /* Ignore if not in STA thread */ }

                // Block everything else and exit the software
                string errorMsg = $"Unauthorized type found in file: {typeName}.\r\n\r\n" +
                                  "This file appears to be corrupted or tampered with. " +
                                  "To ensure system security, the application will now close.";

                if (Debugger.IsAttached)
                {
                    // Block everything else and exit the software
                    errorMsg = $"Add {typeName} to the WizardControllerSerializationBinder.\r\n\r\n" +
                                "This likely happened because you forgot to add a new class\r\n" +
                                "that appears in one of the save file objects to the binder.\r\n" +
                                "The required line has been copied to the clipboard.";
                }
                

                // This throw is unreachable due to Environment.Exit, but kept for compiler satisfaction
                throw new SecurityException(errorMsg);
            }
        }
    }

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
        public void AddAnalysis(Analysis myAnalysis, int isQuickAHat = 0)
        {
            if (isQuickAHat == 1)
            {
                myAnalysis.Data.DataType = AnalysisDataTypeEnum.AHat;
            }
            AddAnalysis(myAnalysis, null);
        }

        /// <summary>
        /// Disposes of all the user objects in both the dock and wizard
        /// </summary>
        /// <param name="pair"></param>
        private void DisposeWizardDockPair(WizardDockPair pair)
        {
            pair.Dock.Dispose();
            pair.Wizard.DeleteSteps();
            //pair.Wizard.DeleteTreeNodes();
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
                _wizards.Project.Dock.Dispose();
                _wizards.Project.Wizard.DeleteProjWizardSteps();
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

                if (pair.Wizard != null && pair.Dock != null)
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

            foreach (Analysis analysis in e.Analyses)
            {
                var form = new NewSourceForm(analysis.Name, AnalysisNames);

                form.ShowDialog();

                analysis.Name = form.NewName;
                analysis.UsingCustomName = true;

                if (createdFrom != null)
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
            if (ExportProject != null)
            {
                ExportProject.Invoke(sender, e);
            }
        }

        private void GetProjectInfo(object sender, GetProjectInfoArgs e)
        {
            if (NeedProjectInfo != null)
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

            foreach (WizardDockPair pair in _wizards)
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
            if (NeedOpenAnalysis != null)
            {
                NeedOpenAnalysis.Invoke(sender, e);
            }
        }

        private void NeedAnalaysesAssociatedWithProject(object sender, AnalysisListArg e)
        {
            var project = sender as Project;

            e.Analyses.Clear();

            foreach (WizardDockPair pair in _wizards)
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
            //var skillLevel = SkillLevelEnum.LogisticRegression;


            myPair.Source.SkillLevel = SkillLevel;
            myPair.Source.AnalysisType = AnalysisType;
            myPair.Source.JumpTo += JumpToIndex;
            myPair.Source.NeedSwitchHelpView += SwitchHelpView;

            myPair.StepChanged += Step_Changed;
            myPair.StepChanging += Step_Changing;
        }

        private void SwitchHelpView(object sender, EventArgs e)
        {
            if (NeedSwitchHelpView != null)
            {
                NeedSwitchHelpView.Invoke(sender, e);
            }
        }

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
                    if (pair.Wizard.CurrentStep != null)
                    {
                        if (pair.Wizard.CurrentStep.IsDisposed)
                        {
                            ControlOrganize control = ControlOrganize.NaviBottom;
                            Analysis tempAnalysis = pair.Analysis;
                            if (tempAnalysis.AnalysisDataType == AnalysisDataTypeEnum.HitMiss)
                            {
                                pair.Reloading = true;
                                pair.Wizard = new HitMissNormalWizard((HitMissAnalysis)tempAnalysis, ref control);
                            }
                            else if (tempAnalysis.AnalysisDataType == AnalysisDataTypeEnum.AHat)
                            {
                                pair.Reloading = true;
                                pair.Wizard = new AHatvsANormalWizard((AHatAnalysis)tempAnalysis, ref control);
                            }

                        }
                    }
                    pair.SyncDock();
                    return pair;
                }

                return null;
            }

            return null;
        }

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
            using (var stream = File.Open(myFile, FileMode.Create))
            {
                var bin = new BinaryFormatter();
                // GUARD: Whitelist types allowed in the file
                bin.Binder = new Security.WizardControllerSerializationBinder();

                var analyses = new List<Analysis>();

                foreach (WizardDockPair pair in _wizards)
                {
                    analyses.Add(pair.Analysis);
                }

                if (_wizards.Project.Project.AnalysisType == AnalysisTypeEnum.Quick)
                {
                    if (analyses[0].Data.ActivatedFlaws.Rows.Count == 0)
                    {
                        MessageBox.Show("Please run a valid analysis with at least 8 data points \nbefore saving your quick analysis.", "POD v4 Error");
                        return false;
                    }
                }

                // Sequence of objects in the binary stream
                bin.Serialize(stream, CreateInfoString(myFile, _wizards.Project.Project, analyses, false));
                bin.Serialize(stream, _wizards.Project.Source);

                foreach (WizardDockPair pair in _wizards)
                {
                    bin.Serialize(stream, pair.Source);
                }
            }

            return true;
        }

        public static string CreateInfoString(string myFile, Project project, List<Analysis> myAnalyses, bool useFileTimeStamp)
        {
            if (project == null || myAnalyses == null)
                return "";

            if (project.AnalysisType != AnalysisTypeEnum.Quick)
            {
                var analysisInfo = $"ANALYSIS INFO:\n";
                var fileInfo = $"FILE INFO:\n";

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

                fileInfo += $"File Location:\t{Path.GetDirectoryName(myFile)}\n";
                fileInfo += $"Last Accessed:\t{fileAccessDate}, {fileAccessTime}\n\n";

                foreach (Analysis analysis in myAnalyses)
                {
                    analysisInfo += $"Name:\t{analysis.Name}\n" +
                                    $"Type:\t{Globals.GetLongVersion(analysis.AnalysisDataType)}\n\n";
                }

                var projectInfo = $"{fileInfo}PROJECT INFO:\n";

                var props = project.Properties;
                var sourceInfo = $"DATA SOURCE INFO:\n" +
                                 $"# of Data Sources:\t{project.Sources.Count}\n";

                projectInfo += $"Project Name:\t\t{props.Name}\n" +
                               $"Parent Project Name:\t{props.Parent}\n" +
                               $"Analyst:\t\t\t{props.Analyst.Name}\n" +
                               $"Analyst Company:\t{props.Analyst.Company}\n" +
                               $"Customer POC:\t\t{props.Customer.Name}\n" +
                               $"Customer Company:\t{props.Customer.Company}\n" +
                               $"Notes:\n{props.Notes}\n\n";

                foreach (DataSource source in project.Sources)
                {
                    var headerName = "";

                    if (source.Original != null)
                    {
                        if (source.Original.Columns.Count > 0)
                        {
                            foreach (DataColumn col in source.Original.Columns)
                            {
                                headerName += $"{col.ColumnName}, ";
                            }
                            headerName = headerName.Substring(0, headerName.Length - 2);
                        }
                        else
                        {
                            headerName = "No data table associated with this source.";
                        }

                        sourceInfo += $"Source Name:\t\t{source.SourceName}\n" +
                                      $"Source Type:\t\t{source.AnalysisDataType}\n" +
                                      $"Row Count:\t\t{source.Original.Rows.Count}\n" +
                                      $"Column Headers:\n{headerName}\n\n";
                    }
                    else
                    {
                        sourceInfo += $"Source Name:\t\t{source.SourceName}\n" +
                                      $"Source Type:\t\t{source.AnalysisDataType}\n" +
                                      $"Row Count:\t\t0\n" +
                                      $"Column Headers:\nNo data table associated with this source.\n\n";
                    }
                }

                return $"{projectInfo}{sourceInfo}{analysisInfo}";
            }
            else
            {
                Analysis analysis = myAnalyses[0];
                string projectInfo = "";

                projectInfo += $"Operator:\t\t{analysis.Operator}\n\n";
                projectInfo += $"Specimen Set:\t\t{analysis.SpecimenSet}\n";
                projectInfo += $"Specimen Range:\t{analysis.InFlawMin:F3} - {analysis.InFlawMax:F3} ({analysis.SpecimentUnit})\n\n";

                if (analysis.Data.DataType == AnalysisDataTypeEnum.AHat)
                {
                    projectInfo += $"Instrument:\t\t{analysis.Instrument}\n";
                    projectInfo += $"Instrument Range:\t{analysis.InResponseMin:F3} - {analysis.InResponseMax:F3} ({analysis.InstrumentUnit})\n\n";
                }

                projectInfo += $"Data Point Count:\t{analysis.Data.ActivatedSpecimenIDs.Rows.Count}\n\n";
                projectInfo += $"ID\t\tFlaw\t\tResponse\n\n";

                var index = 0;
                foreach (DataRow row in analysis.Data.ActivatedSpecimenIDs.Rows)
                {
                    projectInfo += $"{row[0]}\t\t";

                    var flaw = analysis.Data.ActivatedFlaws.Rows[index][0].ToString();
                    if (Double.TryParse(flaw, out double flawValue))
                        projectInfo += $"{analysis.Data.InvertTransformedFlaw(flawValue):F3}\t\t";
                    else
                        projectInfo += $"{flaw}\t\t";

                    var response = analysis.Data.ActivatedResponses.Rows[index][0].ToString();
                    if (Double.TryParse(response, out double responseValue))
                        projectInfo += $"{analysis.Data.InvertTransformedResponse(responseValue):F3}\n";
                    else
                        projectInfo += $"{response}\n";

                    index++;
                }

                return projectInfo;
            }
        }

        public static string DeserializeProjectInfo(string myFile)
        {
            using (var stream = File.Open(myFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var bin = new BinaryFormatter();
                // GUARD
                bin.Binder = new Security.WizardControllerSerializationBinder();

                var analyses = new List<Analysis>();
                Project project = null;

                try
                {
                    object firstObj = bin.Deserialize(stream);

                    if (firstObj is Project proj)
                    {
                        project = proj;
                    }
                    else
                    {
                        // Assume first object was the info string, move to Project
                        project = (Project)bin.Deserialize(stream);
                        // If it fails here, the catch block resets and returns the string
                    }
                }
                catch (SecurityException secExp)
                {
                    // Catch security violation, inform user and exit software gracefully
                    ShowSecurityWarningAndExit(secExp.Message);
                    return string.Empty;
                }
                catch
                {
                    stream.Position = 0;
                    string info = (String)bin.Deserialize(stream);
                    return info;
                }

                try
                {
                    while (stream.Position < stream.Length)
                    {
                        var analysis = (Analysis)bin.Deserialize(stream);
                        analyses.Add(analysis);
                    }
                }
                catch (SecurityException secExp)
                {
                    ShowSecurityWarningAndExit(secExp.Message);
                }
                catch (SerializationException) { /* End of Stream */ }

                return CreateInfoString(myFile, project, analyses, true);
            }
        }

        public void Deserialize(string myFile, List<WizardDock> myDocks)
        {
            using (Stream stream = File.Open(myFile, FileMode.Open))
            {
                BinaryFormatter bin = new BinaryFormatter();
                // GUARD
                bin.Binder = new Security.WizardControllerSerializationBinder();

                List<string> dockNames = myDocks.Select(d => d.Label).ToList();
                Project project = null;

                try
                {
                    object firstObj = bin.Deserialize(stream);
                    if (firstObj is Project)
                    {
                        project = (Project)firstObj;
                    }
                    else if (firstObj is string)
                    {
                        // Skip the info string and get the project
                        project = (Project)bin.Deserialize(stream);
                    }
                }
                catch (SecurityException secExp)
                {
                    ShowSecurityWarningAndExit(secExp.Message);
                    return;
                }
                catch (Exception exp)
                {
                    MessageBox.Show(exp.Message, "Error Loading File");
                    return;
                }

                if (project != null)
                {
                    if (dockNames.Contains(project.Name))
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
                        while (stream.Position < stream.Length)
                        {
                            Analysis analysis = (Analysis)bin.Deserialize(stream);
                            analysis.CheckLoadHealth();

                            if (dockNames.Contains(analysis.Name))
                            {
                                WizardDock dock = myDocks[dockNames.IndexOf(analysis.Name)];
                                AddAnalysis(analysis, dock);
                            }
                            else
                            {
                                AddAnalysis(analysis);
                            }
                        }
                    }
                    catch (SecurityException secExp)
                    {
                        ShowSecurityWarningAndExit(secExp.Message);
                    }
                    catch (SerializationException) { /* End of objects */ }
                }
            }
        }

        private static void ShowSecurityWarningAndExit(string errorMsg)
        {
            MessageBox.Show(errorMsg, "Security Violation", MessageBoxButtons.OK, MessageBoxIcon.Stop);

            Application.Exit();
            Environment.Exit(0);
        }

        private void AddProject(Project project, bool createDock)
        {
            if (!createDock)
            {
                AddGUIlessProject(project);
            }
            else
            {
                AddProject(project);
            }
        }

        private void AddGUIlessProject(Project project)
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

                foreach (WizardDockPair pair in _wizards)
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

        public void RemoveAutoOpen(string myName)
        {
            _wizards[myName].Analysis.AutoOpen = false;
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

            myWriter.Workbook.AutoFitColumn(1, colIndex - 1);

            Project.WriteDataToExcel(myWriter);

            foreach (WizardDockPair pair in _wizards)
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
                //release all the user objects for the wizard dock pair
                DisposeWizardDockPair(pair);
            }

            foreach (Analysis analysis in analyses)
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