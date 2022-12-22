using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using RDotNet;
namespace CSharpBackendWithR
{
    /// <summary>
    /// This class controls everything related to the R backend including initializing the R.NET engine, importing the necessary libaries, and running the appropriate scripts
    /// </summary>
    public class REngineObject
    {
        private REngine rEngine;
        private string applicationPathScripts;
        private string applicationPath;
        private string forwardSlashAppPath;
        string rPath;
        string homePath;
        public REngineObject()
        {
            this.applicationPathScripts = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)+ @"\..\..\..";
            //this.applicationPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\..\..\..\..";
            this.applicationPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\..\..\..";
            //convert the application path to forward slashes (when using file paths in r)
            this.forwardSlashAppPath = this.applicationPathScripts.Replace(@"\", "/");
            try
            {
                //create variable for r engine
                this.rEngine = initializeRDotNet();
            }
            catch
            {
                this.applicationPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                this.rEngine = initializeRDotNet();
            }
            //set the path for the r libraries-used to aid the user in setting up the r backend
            SetLibraryPathEnv();

            InitializeRLibraries();
            InitializeRScripts();
            PythonLoaded = true;
            /*
            try
            {
                //add back in later
                SetUsePython();
                InitializePythonScripts();
                PythonLoaded = true;
            }
            catch
            {
                PythonLoaded = false;
            }
            */
        }
        
        private REngine initializeRDotNet()
        {
            
            REngine engine;
            string rVersion = "4.1.2";
            try
            {
                StartREngine(rVersion, out engine);
            }
            catch (Exception RSetupEnvironmentException)
            {
                if(RSetupEnvironmentException.GetType().Name== "ArgumentException")
                {
                    throw RSetupEnvironmentException;
                }
                else {
                    StartREngine(rVersion, out engine, "x64");
                }
                    
            }
            engine.Initialize();
            //ensure the REngine global environment is cleared when the program starts
            //engine.ClearGlobalEnvironment();
            return engine;
        }
        private void StartREngine(string rVersion, out REngine engine, string programType = "i386")
        {
            this.rPath = this.applicationPath + @"\R-"+rVersion+@"\bin\" + programType;
            this.homePath = this.applicationPath + @"\R-" + rVersion + @"\";
            REngine.SetEnvironmentVariables(this.rPath, this.homePath);
            engine = REngine.GetInstance();
        }
        /// <summary>
        /// https://stackoverflow.com/questions/45537671/dataframe-to-datatable-r-net-fastly
        /// Author username: jdweng
        /// Author URL: https://stackoverflow.com/users/5015238/jdweng
        /// </summary>
        public DataTable rDataFrameToDataTable(RDotNet.DataFrame myDataFrame)
        {
            DataTable dtable = new DataTable();
            for (int i = 0; i < myDataFrame.ColumnCount; ++i)
            {
                dtable.Columns.Add(myDataFrame.ColumnNames[i]);
                dtable.Columns[i].DataType = typeof(Double);
            }
            for (int i = 0; i < myDataFrame.RowCount; i++)
            {
                DataRow newRow = dtable.Rows.Add();
                for (int j = 0; j < myDataFrame.ColumnCount; j++)
                {
                    newRow[j] = myDataFrame[i, j];
                }
            }
            return dtable;
        }
        
        //this function is used to set the library path (libraries are contained within the program)
        private void SetLibraryPathEnv()
        {
            try
            {
                this.rEngine.Evaluate(".libPaths('" + this.forwardSlashAppPath + "/R - 4.1.2/library')");   
            }
            catch
            {
                this.forwardSlashAppPath = this.forwardSlashAppPath + "/..";
                this.rEngine.Evaluate("assign('.lib.loc','"+ this.forwardSlashAppPath + "/R_4.1_LibPath')" + "', envir = environment(.libPaths))");
            }
        }
        //This function will need to be rerun everytime the global environment is cleared
        private void InitializeRScripts()
        {
            bool scriptsLoaded = false;
            try
            {
                EvaluateRScripts(ref scriptsLoaded);
            }
            catch
            {
                //check other paths
            }
            if (!scriptsLoaded)
            {
                try
                {
                    this.forwardSlashAppPath = this.forwardSlashAppPath + "/..";
                    //this may get used instead of the platform target is purely x86
                    EvaluateRScripts(ref scriptsLoaded);
                }
                catch
                {
                    //check other paths
                }             
            }
            if (!scriptsLoaded)
            {
                try
                {
                    //this try is when the user has installed PODv4.5
                    string applicationPathScriptsExe = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    //convert the application path to forward slashes (when using file paths in r)
                    string forwardSlashAppPathExe = applicationPathScriptsExe.Replace(@"\", "/");
                    //import necessary R classes for analysis
                    this.rEngine.Evaluate("source('" + forwardSlashAppPathExe + "/RCode/RBackend/HitMiss/WaldCI_RObject.R')");
                    this.rEngine.Evaluate("source('" + forwardSlashAppPathExe + "/RCode/RBackend/HitMiss/HMLogitApproximationRObject.R')");
                    this.rEngine.Evaluate("source('" + forwardSlashAppPathExe + "/RCode/RBackend/HitMiss/GenNormFitClassR.R')");
                    this.rEngine.Evaluate("source('" + forwardSlashAppPathExe + "/RCode/RBackend/HitMiss/LinearComboGeneratorClassR.R')");
                    this.rEngine.Evaluate("source('" + forwardSlashAppPathExe + "/RCode/RBackend/HitMiss/LRConfIntRObject.R')");
                    this.rEngine.Evaluate("source('" + forwardSlashAppPathExe + "/RCode/RBackend/HitMiss/MLRConfIntRObject.R')");
                    this.rEngine.Evaluate("source('" + forwardSlashAppPathExe + "/RCode/RBackend/HitMiss/GenAValuesOnPODCurveRObject.R')");
                    this.rEngine.Evaluate("source('" + forwardSlashAppPathExe + "/RCode/RBackend/HitMiss/GenAValuesOnPODCurveRObject.R')");
                    //Ranked Set Sampling scripts
                    this.rEngine.Evaluate("source('" + forwardSlashAppPathExe + "/RCode/RBackend/HitMiss/RSSComponentsObject.R')");
                    this.rEngine.Evaluate("source('" + forwardSlashAppPathExe + "/RCode/RBackend/HitMiss/RankedSetSamplingMainRObject.R')");
                    this.rEngine.Evaluate("source('" + forwardSlashAppPathExe + "/RCode/RBackend/HitMiss/RankedSetRegGen.R')");
                    this.rEngine.Evaluate("source('" + forwardSlashAppPathExe + "/RCode/RBackend/HitMiss/GenPODCurveRSS.R')");
                    this.rEngine.Evaluate("source('" + forwardSlashAppPathExe + "/RCode/RBackend/HitMiss/GenRSS_A_Values.R')");
                    this.rEngine.Evaluate("source('" + forwardSlashAppPathExe + "/RCode/RBackend/HitMiss/MainRSSamplingDataInR.R')");
                    //this is the main Hit miss R analysis object
                    this.rEngine.Evaluate("source('" + forwardSlashAppPathExe + "/RCode/RBackend/HitMiss/HitMissMainAnalysisRObject.R')");
                    //minimcprofile(used for parallel processing)
                    this.rEngine.Evaluate("source('" + forwardSlashAppPathExe + "/RCode/RBackend/HitMiss/miniMcprofile.R')");
                    //Firth script classs- added june 6th
                    this.rEngine.Evaluate("source('" + forwardSlashAppPathExe + "/RCode/RBackend/HitMiss/HMFirthApproximationRObject.R')");
                    //AHat Analysis R scripts
                    this.rEngine.Evaluate("source('" + forwardSlashAppPathExe + "/RCode/RBackend/SignalResponseCode/SignalResponseMainAnalysisRObject.R')");
                    this.rEngine.Evaluate("source('" + forwardSlashAppPathExe + "/RCode/RBackend/SignalResponseCode/GenPODSignalResponeRObject.R')");
                    this.rEngine.Evaluate("source('" + forwardSlashAppPathExe + "/RCode/RBackend/SignalResponseCode/PrepareDataWithMultipleResponsesRObject.R')");
                    scriptsLoaded = true;
                }
                catch (Exception failedScriptsLoad)
                {
                    if (failedScriptsLoad.GetType().Name == "EvaluationException")
                    {
                        throw new FailedLoadingRScriptsException();
                    }
                    else
                    {
                        throw new Exception("Uknown error occured");
                    }
                }
            }
        }
        //replace and debug with this later in order to make code more compact
        private void EvaluateRScripts(ref bool scriptsLoaded)
        {
            //import necessary R classes for analysis
            this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/HitMiss/WaldCI_RObject.R')");
            this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/HitMiss/HMLogitApproximationRObject.R')");
            this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/HitMiss/GenNormFitClassR.R')");
            this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/HitMiss/LinearComboGeneratorClassR.R')");
            this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/HitMiss/LRConfIntRObject.R')");
            this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/HitMiss/MLRConfIntRObject.R')");
            this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/HitMiss/GenAValuesOnPODCurveRObject.R')");
            this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/HitMiss/GenAValuesOnPODCurveRObject.R')");
            //Ranked Set Sampling scripts
            this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/HitMiss/RSSComponentsObject.R')");
            this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/HitMiss/RankedSetSamplingMainRObject.R')");
            this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/HitMiss/RankedSetRegGen.R')");
            this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/HitMiss/GenPODCurveRSS.R')");
            this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/HitMiss/GenRSS_A_Values.R')");
            this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/HitMiss/MainRSSamplingDataInR.R')");
            //this is the main Hit miss R analysis object
            this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/HitMiss/HitMissMainAnalysisRObject.R')");
            //minimcprofile(used for parallel processing)
            this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/HitMiss/miniMcprofile.R')");
            //Firth script classs- added june 6th
            this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/HitMiss/HMFirthApproximationRObject.R')");
            //AHat Analysis R scripts
            this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/SignalResponseCode/SignalResponseMainAnalysisRObject.R')");
            this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/SignalResponseCode/GenPODSignalResponeRObject.R')");
            this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/SignalResponseCode/PrepareDataWithMultipleResponsesRObject.R')");
            scriptsLoaded = true;
        }
        private void InitializeRLibraries()
        {
            try
            {
                //supress warnings in r (change to zero if wanting to see them)
                this.rEngine.Evaluate("options( warn = -1 )");
                //dependency of MCProfile
                //this.rEngine.Evaluate("library(ggplot2)");
                //dependency of glmnet
                this.rEngine.Evaluate("library(foreach)");
                //dependency for logistf
                this.rEngine.Evaluate("library(logistf)");
                this.rEngine.Evaluate("library(methods)");
                this.rEngine.Evaluate("library(MASS)");
                //used for LR and MLR confidence intervals *** uses a package licensed under GPLv2 only-The necessary functions are included in the minimcprofile.R script
                //this.rEngine.Evaluate("library(mcprofile)"); 
                this.rEngine.Evaluate("library(splines)");
                this.rEngine.Evaluate("library(parallel)");
                //used to interact with the python scripts
                //this.rEngine.Evaluate("library(reticulate)");//caution: Licensed under Apache 2.0
                //this.rEngine.Evaluate("print(packageVersion('reticulate'))");
                //the following libraries are used for signal response
                this.rEngine.Evaluate("library(gridExtra)");
                this.rEngine.Evaluate("library(nlme)");
                this.rEngine.Evaluate("library(pracma)");
                this.rEngine.Evaluate("library(carData)");
                this.rEngine.Evaluate("suppressPackageStartupMessages(library(car))");
                this.rEngine.Evaluate("library(survival)");
                this.rEngine.Evaluate("suppressPackageStartupMessages(library(corrplot))");
                try
                {
                    this.rEngine.Evaluate("library(roxygen2)");
                }
                catch
                {
                    this.rEngine.Evaluate("library.dynam('roxygen2', 'roxygen2', lib.loc = '"+this.applicationPath+"/R_4.1_LibPath')");
                }
                var endingVar = 0.0;
            }
            catch(Exception failedLibrariesLoad)
            {
                if(failedLibrariesLoad.GetType().Name== "EvaluationException")
                {
                    throw new FailedLoadingLibrariesException();
                }
                else
                {
                    throw new Exception("Uknown error occured");
                }
            }
            

        }
        private void SetUsePython()
        {
            //this.rEngine.Evaluate("use_python('C:/Users/gohmancm/AppData/Local/Continuum/anaconda3')");
            //this.rEngine.Evaluate("use_python('C:/ProgramData/Anaconda3')");
            //this.rEngine.Evaluate("use_condaenv('C:/ProgramData/Anaconda3')");
            this.rEngine.Evaluate("use_python('C:/Users/colin/AppData/Local/Programs/Python/Python310-32')");
        }
        //used to initialize helper python scripts
        private void InitializePythonScripts()
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            Debug.WriteLine("starting python scripts");
            this.rEngine.Evaluate("source_python('"+this.forwardSlashAppPath+ "/RCode/RBackend/PythonRankedSetSampling/RSSRowSorter.py')");
            this.rEngine.Evaluate("source_python('" + this.forwardSlashAppPath + "/RCode/RBackend/PythonRankedSetSampling/CyclesArrayGenerator.py')");
            this.rEngine.Evaluate("source_python('" + this.forwardSlashAppPath + "/RCode/RBackend/PythonRankedSetSampling/MainRSSamplingClass.py')");
            this.rEngine.Evaluate("source_python('" + this.forwardSlashAppPath + "/RCode/RBackend/PythonRankedSetSampling/RSS2DArrayGenerator.py')");
            Debug.WriteLine("finished loading python scripts");
            watch.Stop();
            var time = watch.ElapsedMilliseconds / 1000.00;
            Debug.WriteLine("The runtime was: " + time + " seconds");
        }
        /// <summary>
        /// used to get the rEngine in its current state
        /// </summary>
        public REngine RDotNetEngine => this.rEngine;
        /// <summary>
        /// used to clear the global environment for the rEngine objects and recalls all scripts
        /// </summary>
        public void clearGlobalIInREngineObject()
        {
            this.rEngine.ClearGlobalEnvironment();
            InitializeRScripts();
            InitializePythonScripts();
        }
        public static bool REngineRunning=false;
        public static bool PythonLoaded { get; set; }
    }
    //custom exception handling classes to aid the user in figuring out why the application backend didn't initialize properly
    [Serializable]
    public class FailedLoadingLibrariesException : Exception
    {
        public FailedLoadingLibrariesException()
            : base(String.Format("Failed to load 1 or more library necessary for R Engine"))
        {
        }
    }
    [Serializable]
    public class FailedLoadingRScriptsException : Exception
    {
        public FailedLoadingRScriptsException()
            : base(String.Format("Failed to load 1 or more R scripts into the R Engine global environment"))
        {
        }
    }
}
