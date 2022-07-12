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
        private string applicationPath;
        private string forwardSlashAppPath;
        public REngineObject()
        {
            this.applicationPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)+ @"\..\..\..";
            //convert the application path to forward slashes (when using file paths in r)
            this.forwardSlashAppPath = applicationPath.Replace(@"\", "/");
            //create variable for r engine
            this.rEngine = initializeRDotNet();
            InitializeRLibraries();
            InitializeRScripts();
            //add back in later
            //InitializePythonScripts();
        }
        
        private REngine initializeRDotNet()
        {
            //initialize the R engine
            //for this github commit, the R exe used is located just outside the github folder
            string rPath = this.applicationPath + @"\R-3.5.3\bin\i386";
            string homePath = this.applicationPath + @"\R-3.5.3\";
            REngine.SetEnvironmentVariables(rPath, homePath);
            REngine engine = REngine.GetInstance();
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
            
            //import necessary R classes for analysis
            this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/WaldCI_RObject.R')");
            this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/HMLogitApproximationRObject.R')");
            this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/GenNormFitClassR.R')");
            this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/LinearComboGeneratorClassR.R')");
            this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/LRConfIntRObject.R')");
            this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/MLRConfIntRObject.R')");
            this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/GenAValuesOnPODCurveRObject.R')");
            this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/GenAValuesOnPODCurveRObject.R')");
            //Ranked Set Sampling scripts
            this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/RSSComponentsObject.R')");
            this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/RankedSetSamplingMainRObject.R')");
            this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/RankedSetRegGen.R')");
            this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/GenPODCurveRSS.R')");
            this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/GenRSS_A_Values.R')");
            //this is the main Hit miss R analysis object
            this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/HitMissMainAnalysisRObject.R')");
            //minimcprofile(used for parallel processing
            this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/miniMcprofile.R')");
            //Firth script classs- added june 6th
            this.rEngine.Evaluate("source('" + this.forwardSlashAppPath + "/RCode/RBackend/HMFirthApproximationRObject.R')");
            //supress warnings in r (change to zero if wanting to see them)
            this.rEngine.Evaluate("options( warn = 0 )");

        }
        private void InitializeRLibraries()
        {
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
            this.rEngine.Evaluate("library(logistf)");
            //used to interact with the python scripts
            this.rEngine.Evaluate("library(reticulate)");//caution: Licensed under Apache 2.0
        }
        //used to initialize helper python scripts
        private void InitializePythonScripts()
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            Debug.WriteLine("starting python scripts");
            this.rEngine.Evaluate("use_python('C:/Users/gohmancm/AppData/Local/Continuum/anaconda3')");
            this.rEngine.Evaluate("source_python('"+forwardSlashAppPath+ "/RCode/RBackend/PythonRankedSetSampling/RSSRowSorter.py')");
            this.rEngine.Evaluate("source_python('" + forwardSlashAppPath + "/RCode/RBackend/PythonRankedSetSampling/CyclesArrayGenerator.py')");
            this.rEngine.Evaluate("source_python('" + forwardSlashAppPath + "/RCode/RBackend/PythonRankedSetSampling/MainRSSamplingClass.py')");
            this.rEngine.Evaluate("source_python('" + forwardSlashAppPath + "/RCode/RBackend/PythonRankedSetSampling/RSS2DArrayGenerator.py')");
            //this.rEngine.Evaluate("source_python('" + forwardSlashAppPath + "/RCode/RBackend/PythonRankedSetSampling/RSSDataFrameGenerator.py')");
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
