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
    public class REngineObject
    {
        private REngine rEngine;
        private string applicationPathScripts;
        private string applicationPath;
        private string forwardSlashAppPath;
        
        public REngineObject()
        {
            this.applicationPathScripts = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)+ @"\..\..\..";
            this.applicationPath=@"C:\Program Files\R";
            //this.applicationPath = @"C:\Users\gohmancm\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\R";
            //convert the application path to forward slashes (when using file paths in r)
            this.forwardSlashAppPath = this.applicationPathScripts.Replace(@"\", "/");
            //create variable for r engine
            this.rEngine = initializeRDotNet();
            //set the path for the r libraries-used to aid the user in setting up the r backend
            //SetLibraryPathEnv();

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
            string rPath;
            string homePath;
            REngine engine;
            string rVersion = "4.1.2";
            //initialize the R engine
            if (rVersion == "3.5.3")
            {
                //NOTE installing all necessary packages for r 3.5.3 has become nearly impossible for the 'car' package
                try
                {
                    rPath = this.applicationPath + @"\R-3.5.3\bin\i386";
                    homePath = this.applicationPath + @"\R-3.5.3\";
                    REngine.SetEnvironmentVariables(rPath, homePath);
                    engine = REngine.GetInstance();
                }
                catch
                {
                    rPath = this.applicationPath + @"\R-3.5.3\bin\x64";
                    homePath = this.applicationPath + @"\R-3.5.3\";
                    REngine.SetEnvironmentVariables(rPath, homePath);
                    engine = REngine.GetInstance();
                }
            }
            else if(rVersion=="4.0.0")
            {
                try
                {
                    rPath = this.applicationPath + @"\R-4.0.0\bin\i386";
                    homePath = this.applicationPath + @"\R-4.0.0\";
                    REngine.SetEnvironmentVariables(rPath, homePath);
                    engine = REngine.GetInstance();
                }
                catch (Exception e)
                {
                    rPath = this.applicationPath + @"\R-4.0.0\bin\x64";
                    homePath = this.applicationPath + @"\R-4.0.0\";
                    REngine.SetEnvironmentVariables(rPath, homePath);
                    engine = REngine.GetInstance();
                }

            }
            else if (rVersion == "4.1.2")
            {
                try
                {
                    rPath = this.applicationPath + @"\R-4.1.2\bin\i386";
                    homePath = this.applicationPath + @"\R-4.1.2\";
                    REngine.SetEnvironmentVariables(rPath, homePath);
                    engine = REngine.GetInstance();
                }
                catch (Exception RSetupEnvironmentException)
                {
                    if(RSetupEnvironmentException.GetType().Name== "ArgumentException")
                    {
                        throw RSetupEnvironmentException;
                    }
                    else {
                        rPath = this.applicationPath + @"\R-4.1.2\bin\x64";
                        homePath = this.applicationPath + @"\R-4.1.2\";
                        REngine.SetEnvironmentVariables(rPath, homePath);
                        engine = REngine.GetInstance();
                    }
                    
                }
            }
            else
            {
                engine = null;
            }
            ValidateRInstalled(engine);
            engine.Initialize();
            //ensure the REngine global environment is cleared when the program starts
            //engine.ClearGlobalEnvironment();
            return engine;
        }
        /// <summary>
        /// https://stackoverflow.com/questions/45537671/dataframe-to-datatable-r-net-fastly
        /// Author username: jdweng
        /// </summary>
        public DataTable rDataFrameToDataTable(RDotNet.DataFrame myDataFrame)
        {
            DataTable dtable = new DataTable();
            for (int i = 0; i < myDataFrame.ColumnCount; ++i)
            {
                dtable.Columns.Add(myDataFrame.ColumnNames[i]);
                //if (dtable.Columns[i].ColumnName == "hitrate")
                //{
                //    dtable.Columns[i].DataType = typeof(Int32);
                //}
                //else
                //{
                    dtable.Columns[i].DataType = typeof(Double);
                //}
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

        public void SetLibraryPathEnv()
        {
            try
            {
                this.rEngine.Evaluate("assign(\".lib.loc\",'" + this.forwardSlashAppPath + "/RLibs/win-library/3.5')" + "', envir = environment(.libPaths))");
            }
            catch
            {
                this.forwardSlashAppPath = this.forwardSlashAppPath + "/..";
                this.rEngine.Evaluate("assign('.lib.loc','"+ this.forwardSlashAppPath + "/RLibs/win-library/3.5')" + "', envir = environment(.libPaths))");
            }
        }

        //This function will need to be rerun everytime the global environment is cleared
        public void InitializeRScripts()
        {
            bool scriptsLoaded = false;
            try
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
                //minimcprofile(used for parallel processing
                this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/HitMiss/miniMcprofile.R')");
                //Firth script classs- added june 6th
                this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/HitMiss/HMFirthApproximationRObject.R')");
                //AHat Analysis R scripts
                this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/SignalResponseCode/SignalResponseMainAnalysisRObject.R')");
                this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/SignalResponseCode/GenPODSignalResponeRObject.R')");
                this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/SignalResponseCode/PrepareDataWithMultipleResponsesRObject.R')");
                //this.rEngine.Evaluate("setwd('" + this.forwardSlashAppPath + "')");
                //this.rEngine.Evaluate("source('/RCode/RBackend/HitMiss/WaldCI_RObject.R')");
                scriptsLoaded = true;
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
                    //minimcprofile(used for parallel processing
                    this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/HitMiss/miniMcprofile.R')");
                    //Firth script classs- added june 6th
                    this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/HitMiss/HMFirthApproximationRObject.R')");
                    //AHat Analysis R scripts
                    this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/SignalResponseCode/SignalResponseMainAnalysisRObject.R')");
                    this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/SignalResponseCode/GenPODSignalResponeRObject.R')");
                    this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/SignalResponseCode/PrepareDataWithMultipleResponsesRObject.R')");
                    //this.rEngine.Evaluate("setwd('" + this.forwardSlashAppPath + "')");
                    //this.rEngine.Evaluate("source('/RCode/RBackend/HitMiss/WaldCI_RObject.R')");
                    //this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/SignalResponseCode/fakeScript.R')");
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
        private void InitializeRLibraries()
        {
            try
            {
                //supress warnings in r (change to zero if wanting to see them)
                this.rEngine.Evaluate("options( warn = -1 )");
                //dependency of MCProfile
                this.rEngine.Evaluate("library(ggplot2)");
                //dependency of glmnet
                this.rEngine.Evaluate("library(foreach)");
                //dependency for logistf
                this.rEngine.Evaluate("library(logistf)");
                this.rEngine.Evaluate("library(methods)");
                this.rEngine.Evaluate("library(MASS)");
                this.rEngine.Evaluate("library(mcprofile)"); //used for LR and MLR confidence intervals
                                                             //this.rEngine.Evaluate("library(glmnet)"); //LICENSED UNDER GPLv2 only, may end up not using this
                                                             //this.rEngine.Evaluate("library(logistf)");
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
                //temporary
                this.rEngine.Evaluate("library(ggResidpanel)");
                this.rEngine.Evaluate("suppressPackageStartupMessages(library(corrplot))");
                this.rEngine.Evaluate("library(roxygen2)");
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
            this.rEngine.Evaluate("use_condaenv('C:/ProgramData/Anaconda3')");
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
        //used to get the rEngine in its current state
        public REngine RDotNetEngine => this.rEngine; 
        //used to clear the global environment for the rEngine objects and recalls all scripts
        public void clearGlobalIInREngineObject()
        {
            //this.rEngine.ClearGlobalEnvironment();
            //InitializeRScripts();
            //InitializePythonScripts();
        }
        private void ValidateRInstalled(REngine engine)
        {
            if (engine == null)
            {
                throw new RNotInstalledException();
            }
        }
        public static bool REngineRunning=false;
        public static bool PythonLoaded { get; set; }
    }
    //custom exception handling classes to aid the user in figuring out why the application backend didn't initialize properly
    [Serializable]
    public class RNotInstalledException : Exception
    {
        //public InvalidStudentNameException() { }

        public RNotInstalledException()
            : base(String.Format("Correct R Version Not Installed"))
        {
        }
    }
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
