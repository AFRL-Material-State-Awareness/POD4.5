// <copyright file="REngineObject.cs" company="The University of Dayton">
// Copyright © 2025 The University of Dayton. All rights reserved.
//
// Licensed under the GNU General Public License, Version 3.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.gnu.org/licenses/gpl-3.0.txt
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using RDotNet;
using System.Windows.Forms;
using POD;

namespace CSharpBackendWithR
{
    /// <summary>
    /// This class controls everything related to the R backend including initializing the R.NET engine, importing the necessary libaries, and running the appropriate scripts
    /// </summary>
    public class REngineObject : IREngineObject
    {
        private REngine rEngine;
        private string applicationPathScripts;
        private string applicationPath;
        private string forwardSlashAppPath;
        string rPath;
        string homePath;

        public REngineObject()
        {
            this.applicationPathScripts = Path.GetFullPath(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\..\..\..");
            this.applicationPath = Path.GetFullPath(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\..\..\..");
            this.forwardSlashAppPath = this.applicationPathScripts.Replace(@"\", "/");
            try
            {
                this.rEngine = initializeRDotNet();
            }
            catch
            {
                this.applicationPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                this.rEngine = initializeRDotNet();
            }

            SetLibraryPathEnv();
            InitializeRLibraries();
            InitializeRScripts();
            PythonLoaded = true;
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
                if (RSetupEnvironmentException.GetType().Name == "ArgumentException")
                {
                    if (RSetupEnvironmentException.Message.Contains("This 64-bit process failed to load the library R.dll."))
                    {
                        StartREngine(rVersion, out engine, "x64");
                    }
                    else
                    {
                        throw RSetupEnvironmentException;
                    }
                }
                else
                {
                    StartREngine(rVersion, out engine, "x64");
                }
            }
            engine.Initialize();
            return engine;
        }

        private void StartREngine(string rVersion, out REngine engine, string programType = "i386")
        {
            this.rPath = $@"{this.applicationPath}\R-{rVersion}\bin\{programType}";
            this.homePath = $@"{this.applicationPath}\R-{rVersion}\";

            REngine.SetEnvironmentVariables(this.rPath, this.homePath);
            engine = REngine.GetInstance();
        }

        public DataTable rDataFrameToDataTable(RDotNet.DataFrame myDataFrame)
        {
            //DataTable dtable = new DataTable();
            //int rowCount = myDataFrame.RowCount;
            //int colCount = myDataFrame.ColumnCount;

            //// 1. Prepare columns and pull data into C# arrays
            //List<double[]> columnData = new List<double[]>();

            //for (int j = 0; j < colCount; ++j)
            //{
            //    dtable.Columns.Add(myDataFrame.ColumnNames[j], typeof(double));

            //    // Instead of GetColumn(j), we access the DataFrame as a list 
            //    // and grab the j-th element (the vector)
            //    double[] data = myDataFrame[j].AsNumeric().ToArray();
            //    columnData.Add(data);
            //}

            //// 2. Populate the DataTable using the local C# arrays
            //// This is much faster and prevents the Stack Overflow
            //for (int i = 0; i < rowCount; i++)
            //{
            //    DataRow newRow = dtable.NewRow();
            //    for (int j = 0; j < colCount; j++)
            //    {
            //        newRow[j] = columnData[j][i];
            //    }
            //    dtable.Rows.Add(newRow);
            //}

            //return dtable;

            // 1. Quick Meta-data extraction
            int rowCount = myDataFrame.RowCount;
            int colCount = myDataFrame.ColumnCount;
            string[] names = myDataFrame.ColumnNames;

            DataTable dtable = new DataTable();

            // 2. Transpose data into C# arrays once
            // We do this because R is column-major, but DataTable is row-major.
            double[][] columnData = new double[colCount][];
            for (int j = 0; j < colCount; j++)
            {
                dtable.Columns.Add(names[j], typeof(double));
                columnData[j] = myDataFrame[j].AsNumeric().ToArray();
            }

            // 3. Optimized Bulk Load
            dtable.BeginLoadData();
            try
            {
                // Reuse this buffer for every row to save the Garbage Collector
                object[] rowBuffer = new object[colCount];

                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = 0; j < colCount; j++)
                    {
                        rowBuffer[j] = columnData[j][i];
                    }
                    // LoadDataRow is the fastest way to inject an object array
                    dtable.LoadDataRow(rowBuffer, true);
                }
            }
            finally
            {
                dtable.EndLoadData();
            }

            return dtable;
        }

        ///// <summary>
        ///// https://stackoverflow.com/questions/45537671/dataframe-to-datatable-r-net-fastly
        ///// Author username: jdweng
        ///// Author URL: https://stackoverflow.com/users/5015238/jdweng
        ///// </summary>
        //public DataTable rDataFrameToDataTable(RDotNet.DataFrame myDataFrame)
        //{
        //    DataTable dtable = new DataTable();
        //    for (int i = 0; i < myDataFrame.ColumnCount; ++i)
        //    {
        //        dtable.Columns.Add(myDataFrame.ColumnNames[i]);
        //        dtable.Columns[i].DataType = typeof(Double);
        //    }
        //    for (int i = 0; i < myDataFrame.RowCount; i++)
        //    {
        //        DataRow newRow = dtable.Rows.Add();
        //        for (int j = 0; j < myDataFrame.ColumnCount; j++)
        //        {
        //            newRow[j] = myDataFrame[i, j];
        //        }
        //    }
        //    return dtable;
        //}

        private void SetLibraryPathEnv()
        {
            try
            {
                // Direct interpolation into the R function call
                this.rEngine.SafeEvaluate($".libPaths('{this.forwardSlashAppPath}/R - 4.1.2/library')");
            }
            catch
            {
                this.forwardSlashAppPath = $"{this.forwardSlashAppPath}/..";

                // Using interpolation to handle the complex 'assign' call
                this.rEngine.SafeEvaluate($"assign('.lib.loc', '{this.forwardSlashAppPath}/R_4.1_LibPath', envir = environment(.libPaths))");
            }
        }

        private void InitializeRScripts()
        {
            bool scriptsLoaded = false;
            try
            {
                string filepath1 = this.forwardSlashAppPath;
                EvaluateRScripts(filepath1, ref scriptsLoaded);
            }
            catch {{ }}
            if (!scriptsLoaded)
            {
                try
                {
                    string filepath2 = $"{this.forwardSlashAppPath}/..";
                    EvaluateRScripts(filepath2, ref scriptsLoaded);
                }
                catch {{ }}
            }
            if (!scriptsLoaded)
            {
                try
                {
                    string applicationPathScriptsExe = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    string forwardSlashAppPathExe = applicationPathScriptsExe.Replace(@"\", "/");
                    EvaluateRScripts(forwardSlashAppPathExe, ref scriptsLoaded);
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

        private void EvaluateRScripts(String path, ref bool scriptsLoaded)
        {
            this.rEngine.SafeEvaluate($"source('{path}/RCode/RBackend/SignalResponseCode/BoxCoxTransform.R')");
            this.rEngine.SafeEvaluate($"source('{path}/RCode/RBackend/HitMiss/WaldCI_RObject.R')");
            this.rEngine.SafeEvaluate($"source('{path}/RCode/RBackend/HitMiss/HMLogitApproximationRObject.R')");
            this.rEngine.SafeEvaluate($"source('{path}/RCode/RBackend/HitMiss/GenNormFitClassR.R')");
            this.rEngine.SafeEvaluate($"source('{path}/RCode/RBackend/HitMiss/LinearComboGeneratorClassR.R')");
            this.rEngine.SafeEvaluate($"source('{path}/RCode/RBackend/HitMiss/LRConfIntRObject.R')");
            this.rEngine.SafeEvaluate($"source('{path}/RCode/RBackend/HitMiss/MLRConfIntRObject.R')");
            this.rEngine.SafeEvaluate($"source('{path}/RCode/RBackend/HitMiss/GenAValuesOnPODCurveRObject.R')");
            this.rEngine.SafeEvaluate($"source('{path}/RCode/RBackend/HitMiss/RSSComponentsObject.R')");
            this.rEngine.SafeEvaluate($"source('{path}/RCode/RBackend/HitMiss/RankedSetSamplingMainRObject.R')");
            this.rEngine.SafeEvaluate($"source('{path}/RCode/RBackend/HitMiss/RankedSetRegGen.R')");
            this.rEngine.SafeEvaluate($"source('{path}/RCode/RBackend/HitMiss/GenPODCurveRSS.R')");
            this.rEngine.SafeEvaluate($"source('{path}/RCode/RBackend/HitMiss/GenRSS_A_Values.R')");
            this.rEngine.SafeEvaluate($"source('{path}/RCode/RBackend/HitMiss/MainRSSamplingDataInR.R')");
            this.rEngine.SafeEvaluate($"source('{path}/RCode/RBackend/HitMiss/HitMissMainAnalysisRObject.R')");
            this.rEngine.SafeEvaluate($"source('{path}/RCode/RBackend/HitMiss/miniMcprofile.R')");
            this.rEngine.SafeEvaluate($"source('{path}/RCode/RBackend/HitMiss/HMFirthApproximationRObject.R')");
            this.rEngine.SafeEvaluate($"source('{path}/RCode/RBackend/SignalResponseCode/SignalResponseMainAnalysisRObject.R')");
            this.rEngine.SafeEvaluate($"source('{path}/RCode/RBackend/SignalResponseCode/GenPODSignalResponeRObject.R')");
            this.rEngine.SafeEvaluate($"source('{path}/RCode/RBackend/SignalResponseCode/PrepareDataWithMultipleResponsesRObject.R')");
            this.rEngine.SafeEvaluate($"source('{path}/RCode/RBackend/SignalResponseCode/RecalculatePODGhostCurve.R')");
            this.rEngine.SafeEvaluate($"source('{path}/RCode/RBackend/SignalResponseCode/GenFrequencyTableR.R')");

            scriptsLoaded = true;
        }

        private void InitializeRLibraries()
        {
            try
            {
                this.rEngine.SafeEvaluate("options( warn = -1 )");
                this.rEngine.SafeEvaluate("library(foreach)");
                this.rEngine.SafeEvaluate("library(logistf)");
                this.rEngine.SafeEvaluate("library(methods)");
                this.rEngine.SafeEvaluate("library(MASS)");
                this.rEngine.SafeEvaluate("library(splines)");
                this.rEngine.SafeEvaluate("library(parallel)");
                this.rEngine.SafeEvaluate("library(gridExtra)");
                this.rEngine.SafeEvaluate("library(nlme)");
                this.rEngine.SafeEvaluate("library(pracma)");
                this.rEngine.SafeEvaluate("library(carData)");
                this.rEngine.SafeEvaluate("suppressPackageStartupMessages(library(car))");
                this.rEngine.SafeEvaluate("library(survival)");
                this.rEngine.SafeEvaluate("suppressPackageStartupMessages(library(corrplot))");
                try
                {
                    this.rEngine.SafeEvaluate("library(roxygen2)");
                }
                catch
                {
                    this.rEngine.SafeEvaluate($"library.dynam('roxygen2', 'roxygen2', lib.loc = '{this.applicationPath}/R_4.1_LibPath')");
                }
                var endingVar = 0.0;
            }
            catch (Exception failedLibrariesLoad)
            {
                if (failedLibrariesLoad.GetType().Name == "EvaluationException")
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
            //this.rEngine.SafeEvaluate("use_python('C:/Users/gohmancm/AppData/Local/Continuum/anaconda3')");
            //this.rEngine.SafeEvaluate("use_python('C:/ProgramData/Anaconda3')");
            //this.rEngine.SafeEvaluate("use_condaenv('C:/ProgramData/Anaconda3')");
            this.rEngine.SafeEvaluate("use_python('C:/Users/colin/AppData/Local/Programs/Python/Python310-32')");
        }
        //used to initialize helper python scripts
        private void InitializePythonScripts()
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            Debug.WriteLine("starting python scripts");

            // Cleaned up pathing for the Python source calls
            this.rEngine.SafeEvaluate($"source_python('{this.forwardSlashAppPath}/RCode/RBackend/PythonRankedSetSampling/RSSRowSorter.py')");
            this.rEngine.SafeEvaluate($"source_python('{this.forwardSlashAppPath}/RCode/RBackend/PythonRankedSetSampling/CyclesArrayGenerator.py')");
            this.rEngine.SafeEvaluate($"source_python('{this.forwardSlashAppPath}/RCode/RBackend/PythonRankedSetSampling/MainRSSamplingClass.py')");
            this.rEngine.SafeEvaluate($"source_python('{this.forwardSlashAppPath}/RCode/RBackend/PythonRankedSetSampling/RSS2DArrayGenerator.py')");

            Debug.WriteLine("finished loading python scripts");
            watch.Stop();

            // Combined the math and string building into one clean line
            Debug.WriteLine($"The runtime was: {watch.ElapsedMilliseconds / 1000.00} seconds");
        }
        /// <summary>
        /// used to get the rEngine in its current state
        /// </summary>
        public REngine RDotNetEngine => this.rEngine;

        /// <summary>
        /// used to clear the global environment for the rEngine objects and recalls all scripts
        /// </summary>
        public void ClearGlobalIInREngineObject()
        {
            this.rEngine.ClearGlobalEnvironment();
            InitializeRScripts();
        }

        public static bool REngineRunning = false;
        public static bool PythonLoaded { get; set; }
    }

    // Interface used to Mock the REngineObject class in order to apply unit testing
    public interface IREngineObject
    {
        REngine RDotNetEngine { get; }
        DataTable rDataFrameToDataTable(RDotNet.DataFrame myDataFrame);
    }

    //custom exception handling classes to aid the user in figuring out why the application backend didn't initialize properly
    [Serializable]
    public class FailedLoadingLibrariesException : Exception
    {
        public FailedLoadingLibrariesException() : base(String.Format("Failed to load 1 or more library necessary for R Engine")) { }
    }

    [Serializable]
    public class FailedLoadingRScriptsException : Exception
    {
        public FailedLoadingRScriptsException() : base(String.Format("Failed to load 1 or more R scripts into the R Engine global environment")) { }
    }
}