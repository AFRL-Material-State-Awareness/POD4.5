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
        public REngineObject()
        {
            applicationPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)+ @"\..\..\..";
            //create variable for r engine
            this.rEngine = initializeRDotNet();
            InitializeRLibraries();
            
        }
        
        private REngine initializeRDotNet()
        {
            //initialize the R engine
            //string rPath = @"C:\Users\gohmancm\Desktop\PODv4Point5FullProjectFolder\CSharpBackendTempSolution\R-3.5.3\bin\x64";
            //string homePath = @"C:\Users\gohmancm\Desktop\PODv4Point5FullProjectFolder\CSharpBackendTempSolution\R-3.5.3\";
            string rPath = applicationPath + @"\R-3.5.3\bin\i386";
            string homePath = applicationPath + @"\R-3.5.3\";
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
            this.rEngine.Evaluate("source('C:/Users/gohmancm/Desktop/PODv4Point5FullProjectFolder/PODv4Point5Attemp1/PODv4/POD Source Code/RCode/RBackend/HitMissMainAnalysisRObject.R')");
            this.rEngine.Evaluate("source('C:/Users/gohmancm/Desktop/PODv4Point5FullProjectFolder/PODv4Point5Attemp1/PODv4/POD Source Code/RCode/RBackend/WaldCI_RObject.R')");
            this.rEngine.Evaluate("source('C:/Users/gohmancm/Desktop/PODv4Point5FullProjectFolder/PODv4Point5Attemp1/PODv4/POD Source Code/RCode/RBackend/HMLogitApproximationRObject.R')");
            this.rEngine.Evaluate("source('C:/Users/gohmancm/Desktop/PODv4Point5FullProjectFolder/PODv4Point5Attemp1/PODv4/POD Source Code/RCode/RBackend/GenNormFitClassR.R')");
            this.rEngine.Evaluate("source('C:/Users/gohmancm/Desktop/PODv4Point5FullProjectFolder/PODv4Point5Attemp1/PODv4/POD Source Code/RCode/RBackend/LinearComboGeneratorClassR.R')");
            this.rEngine.Evaluate("source('C:/Users/gohmancm/Desktop/PODv4Point5FullProjectFolder/PODv4Point5Attemp1/PODv4/POD Source Code/RCode/RBackend/LRConfIntRObject.R')");
            this.rEngine.Evaluate("source('C:/Users/gohmancm/Desktop/PODv4Point5FullProjectFolder/PODv4Point5Attemp1/PODv4/POD Source Code/RCode/RBackend/MLRConfIntRObject.R')");
            this.rEngine.Evaluate("source('C:/Users/gohmancm/Desktop/PODv4Point5FullProjectFolder/PODv4Point5Attemp1/PODv4/POD Source Code/RCode/RBackend/GenAValuesOnPODCurveRObject.R')");
            //supress warnings in r (change to zero if wanting to see them
            this.rEngine.Evaluate("options( warn = -1 )");

        }
        public void InitializeRLibraries()
        {
            //dependency of MCProfile
            this.rEngine.Evaluate("library(ggplot2)");
            //dependency of glmnet
            this.rEngine.Evaluate("library(foreach)");
            //dependency for logistf
            this.rEngine.Evaluate("library(logistf)");
            this.rEngine.Evaluate("library(methods)");
            //this.rEngine.Evaluate("library('RSSampling')");//LICENSED UNDER GPLv2 only
            this.rEngine.Evaluate("library(MASS)");
            this.rEngine.Evaluate("library(mcprofile)"); //used for LR and MLR confidence intervals
            //this.rEngine.Evaluate("library(glmnet)"); //LICENSED UNDER GPLv2 only, may end up not using this
            this.rEngine.Evaluate("library(logistf)");
        }
        //used to get the rEngine in its current state
        public REngine RDotNetEngine => this.rEngine; 
        //used to clear the global environment for the rEngine objects and recalls all scripts
        public void clearGlobalIInREngineObject()
        {
            this.rEngine.ClearGlobalEnvironment();
            InitializeRScripts();
        }    
    }
}
