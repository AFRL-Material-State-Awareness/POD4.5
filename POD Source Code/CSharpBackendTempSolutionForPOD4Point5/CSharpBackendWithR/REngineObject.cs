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
            InitializeRLibraries();
            InitializeRScripts();
            //add back in later
            //SetUsePython();
            //InitializePythonScripts();
        }
        
        private REngine initializeRDotNet()
        {
            string rPath;
            string homePath;
            REngine engine;
            //initialize the R engine
            try
            {
                rPath = this.applicationPath + @"\R-3.5.3\bin\i386";
                homePath = this.applicationPath + @"\R-3.5.3\";
                REngine.SetEnvironmentVariables(rPath, homePath);
                engine = REngine.GetInstance();
            }
            catch(Exception e)
            {
                rPath = this.applicationPath + @"\R-3.5.3\bin\x64";
                homePath = this.applicationPath + @"\R-3.5.3\";
                REngine.SetEnvironmentVariables(rPath, homePath);
                engine = REngine.GetInstance();
            }

            //string rPath = this.applicationPath + @"\R-3.5.3\bin\i386";
            //string homePath = this.applicationPath + @"\R-3.5.3\";
            //string rPath = this.applicationPath + @"\R-3.5.3\bin\x64";
            //string homePath = this.applicationPath + @"\R-3.5.3\";
            //string rPath = this.applicationPath + @"C:\Program Files\R\R-3.5.3\bin\x64";
            //string homePath = this.applicationPath + @"C:\Program Files\R\R-3.5.3\";
            //string rPath = this.applicationPath;
            //string homePath = this.applicationPath;
            //REngine.SetEnvironmentVariables(rPath, homePath);
            //REngine engine = REngine.GetInstance();
            engine.Initialize();
            //ensure the REngine global environment is cleared when the program starts
            engine.ClearGlobalEnvironment();
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
        //This function will need to be rerun everytime the global environment is cleared
        public void InitializeRScripts()
        {
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
                //this.rEngine.Evaluate("setwd('" + this.forwardSlashAppPath + "')");
                //this.rEngine.Evaluate("source('/RCode/RBackend/HitMiss/WaldCI_RObject.R')");
            }
            catch(RDotNet.EvaluationException tryADifferentPath)
            {
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
                //this.rEngine.Evaluate("setwd('" + this.forwardSlashAppPath + "')");
                //this.rEngine.Evaluate("source('/RCode/RBackend/HitMiss/WaldCI_RObject.R')");
            }



        }
        private void InitializeRLibraries()
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
            //this.rEngine.Evaluate("library('RSSampling')");//LICENSED UNDER GPLv2 only ***RESOLVED: Replaced with python scripts
            this.rEngine.Evaluate("library(MASS)");
            this.rEngine.Evaluate("library(mcprofile)"); //used for LR and MLR confidence intervals
            //this.rEngine.Evaluate("library(glmnet)"); //LICENSED UNDER GPLv2 only, may end up not using this
            //this.rEngine.Evaluate("library(logistf)");
            this.rEngine.Evaluate("library(parallel)");
            //used to interact with the python scripts
            this.rEngine.Evaluate("library(reticulate)");//caution: Licensed under Apache 2.0
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
            this.rEngine.ClearGlobalEnvironment();
            InitializeRScripts();
            //InitializePythonScripts();
        }    
    }
}
