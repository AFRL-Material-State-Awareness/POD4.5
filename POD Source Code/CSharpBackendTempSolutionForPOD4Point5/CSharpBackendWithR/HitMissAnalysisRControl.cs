﻿// <copyright file="HitMissAnalysisRControl.cs" company="The University of Dayton">
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
using System.Collections.Generic;
using System.Text;
using System.Data;
using RDotNet;
using System.Threading;
using POD;

namespace CSharpBackendWithR
{
    class HitMissAnalysisRControl
    {
        private IREngineObject myREngineObject;
        private REngine myREngine;
        private GenerateRTransformDF_HitMiss GenerateDataFrameInR;
        private readonly IDataTransformer _dataTransformer;

        public HitMissAnalysisRControl(IREngineObject myREngineObject, IDataTransformer dataTransformer)
        {
            this.myREngineObject = myREngineObject;
            this.myREngine = myREngineObject.RDotNetEngine;
            this._dataTransformer = dataTransformer;
        }
        //convert to a dataframe by adding strings
        private void createDataFrameinGlobalEnvr(HMAnalysisObject newTranformAnalysis)
        {
            int a_x_n = newTranformAnalysis.A_x_n;
            //needed for the r code 
            this.myREngine.SafeEvaluate($"normSampleSize<-{a_x_n}");
            this.GenerateDataFrameInR = new GenerateRTransformDF_HitMiss(this.myREngineObject, newTranformAnalysis, _dataTransformer);
            this.GenerateDataFrameInR.GenerateTransformDataframe();
            //build the dataframe in the global environment
            //this dataframe will remain in the global environment
            this.myREngine.SafeEvaluate("hitMissDF <- data.frame(Index, x, y)");
            this.myREngine.SafeEvaluate("rm(Index, x, y)");
        }
        /// <summary>
        /// Sends a full analysis to the R backend using simple random sampling
        /// </summary>
        /// <param name="newTranformAnalysis"></param>
        public void ExecuteAnalysis(HMAnalysisObject newTranformAnalysis)
        {
            this.createDataFrameinGlobalEnvr(newTranformAnalysis);

            string script = $@"
                newAnalysis <- HMAnalysis$new(
                    hitMissDF = hitMissDF, 
                    modelType = '{newTranformAnalysis.RegressionType}',
                    CIType = '{newTranformAnalysis.CIType}', 
                    N = nrow(hitMissDF), 
                    normSampleAmount = normSampleSize
                )";

            this.myREngine.SafeEvaluate(script);

            this.myREngine.SafeEvaluate("newAnalysis$executeFullAnalysis()");

            RGarbageCollection();
        }
        /// <summary>
        /// Executes analysis for the choose transform panel (omits metrics to speed up the analyses)
        /// </summary>
        /// <param name="newTranformAnalysis"></param>
        public void ExecuteTransformOnlyAnalysis(HMAnalysisObject newTranformAnalysis)
        {
            this.createDataFrameinGlobalEnvr(newTranformAnalysis);

            string initAnalysisScript = $@"
                newAnalysis <- HMAnalysis$new(
                    hitMissDF = hitMissDF, 
                    modelType = '{newTranformAnalysis.RegressionType}',
                    CIType = '{newTranformAnalysis.CIType}', 
                    N = nrow(hitMissDF), 
                    normSampleAmount = normSampleSize
                )";

            this.myREngine.SafeEvaluate(initAnalysisScript);
            this.myREngine.SafeEvaluate("newAnalysis$executeFitAnalysisOnly()");
        }

        /// <summary>
        /// executes analysis with ranked set sampling in R
        /// </summary>
        /// <param name="newTranformAnalysis"></param>
        public void ExecuteRSS(HMAnalysisObject newTranformAnalysis)
        {
            this.createDataFrameinGlobalEnvr(newTranformAnalysis);
            this.myREngine.SafeEvaluate("newRSSComponent <- RSSComponents$new()");

            string initRssScript = $@"
                newRSSComponent$initialize(
                    maxResamples = {newTranformAnalysis.MaxResamples}, 
                    set_mInput = {newTranformAnalysis.Set_m}, 
                    set_rInput = {newTranformAnalysis.Set_r}, 
                    regressionType = '{newTranformAnalysis.RegressionType}', 
                    excludeNAInput = TRUE
                )";
            this.myREngine.SafeEvaluate(initRssScript);

            string initAnalysisScript = $@"
                newAnalysis <- HMAnalysis$new(
                    hitMissDF = hitMissDF, 
                    modelType = '{newTranformAnalysis.RegressionType}',
                    CIType = '{newTranformAnalysis.CIType}',
                    N = nrow(hitMissDF), 
                    normSampleAmount = normSampleSize, 
                    rankedSetSampleObject = newRSSComponent
                )";
            this.myREngine.SafeEvaluate(initAnalysisScript);

            // 5. Cleanup and finalize
            this.myREngine.SafeEvaluate("rm(normSampleSize)");
            this.myREngine.SafeEvaluate("newAnalysis$initializeRSS()");

            RGarbageCollection();
        }
        /// <summary>
        /// call to perform garbage collection on R objects
        /// </summary>
        public void RGarbageCollection()
        {
            this.myREngine.SafeEvaluate("gc()");
        }
        /// <summary>
        /// return logistic regression table from R
        /// </summary>
        /// <returns></returns>
        public DataTable GetLogitFitTableForUI()
        {
            //ShowResults();
            RDotNet.DataFrame returnDataFrame = this.myREngine.SafeEvaluate("newAnalysis$getResults()").AsDataFrame();
            DataTable LogitFitDataTable= this.myREngineObject.rDataFrameToDataTable(returnDataFrame);
            return LogitFitDataTable;
        }
        /// <summary>
        /// Return residual table in R (will generate POD for the original crack sizes with a diff)
        /// </summary>
        /// <returns></returns>
        public DataTable GetResidualFitTableForUI()
        {
            RDotNet.DataFrame returnDataFrame = this.myREngine.SafeEvaluate("newAnalysis$getResidualTable()").AsDataFrame();
            DataTable residualDataTable = this.myREngineObject.rDataFrameToDataTable(returnDataFrame);
            //residualDataTable.Columns["pod"].ColumnName = "t_fit";
            return residualDataTable;
            
        }
        public DataTable GetIterationTableForUI()
        {
            RDotNet.DataFrame returnDataFrame = this.myREngine.SafeEvaluate("newAnalysis$getIterationTable()").AsDataFrame();
            DataTable IterationDatatable = this.myREngineObject.rDataFrameToDataTable(returnDataFrame);
            return IterationDatatable;
        }
        /// <summary>
        /// gets the values for the covariance matrix from R
        /// </summary>
        /// <returns></returns>
        public List<double> GetCovarianceMatrixValues()
        {
            // 1. Convert to matrix, transpose, and flatten in one R call.
            // This ensures Row-Major order (Row 1, then Row 2, etc.)
            var numericData = myREngine.SafeEvaluate("as.vector(t(as.matrix(newAnalysis$getCovMatrix())))").AsNumeric();

            // 2. Return an empty list if R returns null to prevent a crash
            if (numericData == null) return new List<double>();

            // 3. Cast to List (NumericVector implements IEnumerable<double>)
            return new List<double>(numericData);
        }
        /// <summary>
        /// gets teh goodness of fit value from R
        /// </summary>
        /// <returns></returns>
        public double GetGoodnessOfFit()
        {
            double goodFit = Convert.ToDouble(myREngine.SafeEvaluate("newAnalysis$getGoodnessOfFit()").AsNumeric()[0]);
            return goodFit;
        }

        public double GetAIC()
        {
            double aic = Convert.ToDouble(myREngine.SafeEvaluate("newAnalysis$getAIC()").AsNumeric()[0]);
            return aic;
        }

        public bool GetSeparationFlag()
        {
            int separated = Convert.ToInt32(myREngine.SafeEvaluate("newAnalysis$getSeparation()").AsNumeric()[0]);
            if (separated == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Flag generated in R to warn the user if the logistic regression does not converge
        /// </summary>
        /// <returns></returns>
        public bool GetConvergenceFlag()
        {
            int convergedFail = Convert.ToInt32(myREngine.SafeEvaluate("newAnalysis$getConvergedFail()").AsNumeric()[0]);
            if (convergedFail == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public DataTable GetOrigHitMissDF()
        {
            RDotNet.DataFrame origDataFrame = myREngine.SafeEvaluate("newAnalysis$getHitMissDF()").AsDataFrame();
            DataTable originalData = myREngineObject.rDataFrameToDataTable(origDataFrame);
            return originalData;
        }
        public Dictionary<string, double> GetKeyA_Values()
        {
            Dictionary<string, double> aValuesDict= new Dictionary<string, double>();
            List<string> aValuesStrings = new List<string> { "a25", "a50", "a90", "sigmahat", "a9095" };
            RDotNet.NumericVector aValuesVector;
            //current list used returns a25, a50 (muhat), a90, sigmahat (SE of a90), and a9095 
            for (int i= 1;i<=5; i++)
            {
                aValuesVector = this.myREngine.SafeEvaluate($"newAnalysis$getKeyAValues()[{i}]").AsNumeric();
                aValuesDict.Add(key: aValuesStrings[i-1], value: (double)aValuesVector[0]);
            }
            //Console.WriteLine(aValuesDict);
            return aValuesDict;
        }
        public void ShowResults()
        {
            //plots the dataset model fit, the POD curve, and the CI curve
            //this is for debugging only, delete in final program
            //this.myREngine.SafeEvaluate("str('str(as.list(.GlobalEnv)')");
            //this.myREngine.SafeEvaluate("newAnalysis$plotSimdata(newAnalysis$getResults())");
            //this.myREngine.SafeEvaluate("newAnalysis$plotCI(newAnalysis$getResults())");
            //this.myREngine.SafeEvaluate("print(newAnalysis$getResults())");
        }
    }
}
