﻿// <copyright file="AHatAnalysisRControl.cs" company="The University of Dayton">
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
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDotNet;
using System.Diagnostics; // Required for Debug.WriteLine
using POD;

namespace CSharpBackendWithR
{
    class AHatAnalysisRControl
    {
        private IREngineObject myREngineObject;
        private REngine myREngine;
        private GenerateRTransformDF_AHAT GenerateDataFrameInR;
        private readonly IDataTransformer _dataTransformer;

        public bool UsingAllPoints => GenerateDataFrameInR.UsingAllPoints;

        public AHatAnalysisRControl(IREngineObject myREngineObject, IDataTransformer dataTransformer)
        {
            this.myREngineObject = myREngineObject;
            this.myREngine = myREngineObject.RDotNetEngine;
            this._dataTransformer = dataTransformer;
        }


        /// <summary>
        /// Prints the contents of an R data frame to the debug output window.
        /// </summary>
        /// <param name="engine">The active RDotNet REngine instance.</param>
        /// <param name="rDataFrameName">The name of the data frame in the R environment.</param>
        public void PrintRDataFrame(REngine engine, string rDataFrameName)
        {
            try
            {
                // 1. Evaluate the R variable and cast it to a DataFrame object
                DataFrame df = engine.SafeEvaluate(rDataFrameName).AsDataFrame();

                if (df == null)
                {
                    Debug.WriteLine($"Error: R variable '{rDataFrameName}' is not a data frame or is null.");
                    return;
                }

                // 2. Print dimensions and column headers
                Debug.WriteLine($"--- Inspecting R DataFrame: '{rDataFrameName}' ---");
                Debug.WriteLine($"Dimensions: {df.RowCount} rows, {df.ColumnCount} columns");
                Debug.WriteLine($"Columns: {string.Join(", ", df.ColumnNames)}");
                Debug.WriteLine("-------------------------------------------------");

                // 3. Print the first 10 rows of data
                // Note: Logic limited to 10 rows if df.RowCount is larger
                int rowsToPrint = Math.Min(df.RowCount, 10);

                for (int i = 0; i < rowsToPrint; i++)
                {
                    var rowData = new List<string>();
                    for (int j = 0; j < df.ColumnCount; j++)
                    {
                        // Get data from cell [i, j] and format it
                        rowData.Add($"{df.ColumnNames[j]}='{df[i, j]}'");
                    }
                    Debug.WriteLine($"Row {i + 1}:  {string.Join(", ", rowData)}");
                }

                if (df.RowCount > 10)
                {
                    Debug.WriteLine("...");
                }
                Debug.WriteLine("--- End of DataFrame Inspection ---");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to inspect R DataFrame '{rDataFrameName}'. It may not exist yet.");
                Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        private void createDataFrameinGlobalEnvr(AHatAnalysisObject newAHatAnalysis, bool usingAll=false)
        {
            //Debug.WriteLine($"Created Data Frame at {DateTime.Now.ToLongTimeString()}");

            // NOTE: The logic in this function is tightly coupled with the 'UpdateCensoredData' method in 'Analysis.cs'.
            // This method consumes the data prepared by 'UpdateCensoredData'. Any changes to how this method
            // builds the R data frame should be checked against 'UpdateCensoredData' to ensure consistency.
            List<double> cracks = usingAll ? newAHatAnalysis.Flaws_All : newAHatAnalysis.Flaws;
            List<double> signalResponse = usingAll 
                ? newAHatAnalysis.Responses_all[newAHatAnalysis.SignalResponseName] 
                : newAHatAnalysis.Responses[newAHatAnalysis.SignalResponseName];

            // --- Optimized Event Vector Generation ---
            var eventCodes = new List<int>(cracks.Count);
            bool hasCensoring = newAHatAnalysis.ResponsesCensoredLeft.Any() || newAHatAnalysis.ResponsesCensoredRight.Any();

            if (hasCensoring)
            {
                var leftCensored = new HashSet<double>(newAHatAnalysis.ResponsesCensoredLeft[newAHatAnalysis.SignalResponseName]);
                var rightCensored = new HashSet<double>(newAHatAnalysis.ResponsesCensoredRight[newAHatAnalysis.SignalResponseName]);

                foreach (var response in signalResponse)
                {
                    if (rightCensored.Contains(response))
                        eventCodes.Add(0); // Right-censored
                    else if (leftCensored.Contains(response))
                        eventCodes.Add(0); // Left-censored
                    else
                        eventCodes.Add(1); // Uncensored
                }
            }
            else
            {
                // Mark all points as uncensored if no censoring data exists.
                eventCodes.AddRange(Enumerable.Repeat(1, cracks.Count));
            }

            IntegerVector eventVector = this.myREngine.SafeCreateIntegerVector(eventCodes);
            this.myREngine.SafeSetSymbol("event", eventVector);
            // --- End of Optimization ---

            this.GenerateDataFrameInR = new GenerateRTransformDF_AHAT(this.myREngineObject, newAHatAnalysis, usingAll, _dataTransformer);
            this.GenerateDataFrameInR.GenerateTransformDataframe();

            try
            {
                BuildDataFrame(newAHatAnalysis);

                //PrintRDataFrame(this.myREngine, "AHatDFTest");
            }
            catch (Exception exp)
            {
                POD.Globals.ShowErrorMessageBox(exp);
            }
            finally
            {
                var variablesToRemove = new List<string> { "Index", "x", "event", "bc" };
                for (int i = 0; i < newAHatAnalysis.Responses.Count; i++)
                {
                    variablesToRemove.Add($"y{i}");
                }

                this.myREngine.SafeEvaluate($"rm({string.Join(", ", variablesToRemove)})");
            }

        }
        private void BuildDataFrame(AHatAnalysisObject newAHatAnalysis)
        {
            //add the index and flaws to the RDataframe
            this.myREngine.SafeEvaluate("AHatDFTest<-data.frame(Index,x)");
            //append the number of reponses (most of the time it will just be 1
            for(int i=0; i< newAHatAnalysis.Responses.Count; i++)
            {
                this.myREngine.SafeEvaluate($"AHatDFTest <- cbind(AHatDFTest, y{i})");
            }
            //finally append the event variable
            this.myREngine.SafeEvaluate("AHatDFTest <- cbind(AHatDFTest, event)");
        }
        public void ExecuteAnalysis(AHatAnalysisObject newTranformAnalysis)
        {
            this.createDataFrameinGlobalEnvr(newTranformAnalysis);
            this.myREngine.SafeEvaluate("fullAnalysis<-TRUE");
            //execute class with appropriate parameters
            InitializeRClassForSignalResponse(newTranformAnalysis.Pod_threshold, newTranformAnalysis.ModelType);

            //Console.WriteLine("---------------------------AHatDFTest START--------------------------------");
            //CharacterVector lines = this.myREngine.SafeEvaluate("capture.output(print(AHatDFTest))").AsCharacter();
            //foreach (string line in lines)
            //{
            //    // Now 'line' is a clean string without the extra characters
            //    System.Diagnostics.Debug.WriteLine(line);
            //}

            //Console.WriteLine("---------------------------AHatDFTest END--------------------------------");
            //this.myREngine.SafeEvaluate("print(lambdaInput)");
            //Console.WriteLine($"{newTranformAnalysis.Pod_threshold}");

            this.myREngine.SafeEvaluate("newSRAnalysis$executeAhatvsA()");
            RecalculateGhostCurve(ref newTranformAnalysis, true);
            //remove misc variables from the global environment
            this.myREngine.SafeEvaluate("rm(lambdaInput)");
            this.myREngine.SafeEvaluate("rm(fullAnalysis)");
        }
        public void ExecuteTransforms(AHatAnalysisObject newTranformAnalysis)
        {
            this.createDataFrameinGlobalEnvr(newTranformAnalysis);
            this.myREngine.SafeEvaluate("fullAnalysis<-FALSE");
            //execute class with appropriate parameters
            InitializeRClassForSignalResponse(newTranformAnalysis.Pod_threshold, newTranformAnalysis.ModelType);
            this.myREngine.SafeEvaluate("newSRAnalysis$executeAhatvsA()");
            //remove misc variables from the global environment
            this.myREngine.SafeEvaluate("rm(lambdaInput)");
            this.myREngine.SafeEvaluate("rm(fullAnalysis)");
        }
        public void ExecutethresholdChange(AHatAnalysisObject newTranformAnalysis)
        {
            this.createDataFrameinGlobalEnvr(newTranformAnalysis);
            this.myREngine.SafeEvaluate("fullAnalysis<-FALSE");
            //execute class with appropriate parameters
            InitializeRClassForSignalResponse(newTranformAnalysis.Pod_threshold, newTranformAnalysis.ModelType);
            this.myREngine.SafeEvaluate("newSRAnalysis$prepareData()");
            this.myREngine.SafeEvaluate("newSRAnalysis$performTransforms()");
            this.myREngine.SafeEvaluate("ahatvACensored<-newSRAnalysis$genAhatVersusACensored()");
            this.myREngine.SafeEvaluate("newSRAnalysis$genAvaluesAndMatrix(ahatvACensored)");
            this.myREngine.SafeEvaluate("newSRAnalysis$genPODCurve()");
            RecalculateGhostCurve(ref newTranformAnalysis);
            //remove misc variables from the global environment
            this.myREngine.SafeEvaluate("rm(lambdaInput)");
            this.myREngine.SafeEvaluate("rm(ahatvACensored)");
            this.myREngine.SafeEvaluate("rm(fullAnalysis)");
        }
        private void InitializeRClassForSignalResponse(double pod_threshold, int modelType)
        {
            this.myREngine.SafeEvaluate($@"
                newSRAnalysis <- AHatAnalysis$new(
                    signalRespDF = AHatDFTest, 
                    y_dec = {pod_threshold}, 
                    modelType = {modelType}, 
                    lambda = lambdaInput
                )");
            this.myREngine.SafeEvaluate("newSRAnalysis$generateDefaultValues()");
        }
        /// <summary>
        /// this function call is used when the user changes the threshold when the ghost curve is present(points omitted)
        /// the AHATDFTest variable is overwritten with no omitted points and calculated below.
        /// </summary>
        /// <param name="newTranformAnalysis"></param>
        private void RecalculateGhostCurve(ref AHatAnalysisObject newTranformAnalysis, bool recreateThreshTable=false)
        {
            if(this.GenerateDataFrameInR.UsingAllPoints)
            {
                return;
            }

            this.createDataFrameinGlobalEnvr(newTranformAnalysis, true);
            this.myREngine.SafeEvaluate($@"
                recalcPODClass <- RecalcOriginalPOD$new(
                    signalRespDFFull = AHatDFTest, 
                    y_dec = {newTranformAnalysis.Pod_threshold}, 
                    modelType = {newTranformAnalysis.ModelType}, 
                    lambda = lambdaInput, 
                    tau = newSRAnalysis$getTau()
                )");
            //recalculates the POD curve with or without the threshold table
            //when the user changes the threshold, the threshold table is not regenerated
            if (recreateThreshTable)
                this.myREngine.SafeEvaluate("recalcPODClass$recalcPOD(TRUE)");
            else
                this.myREngine.SafeEvaluate("recalcPODClass$recalcPOD()");
            

        }
        public DataTable GetLinearFitTableForUI()
        {
            RDotNet.DataFrame returnDataFrameLinear = myREngine.SafeEvaluate("newSRAnalysis$getLinearModel()").AsDataFrame();
            DataTable AHatLinTable = myREngineObject.rDataFrameToDataTable(returnDataFrameLinear);
            return AHatLinTable;

        }
        public DataTable GetResidualUncensoredTableForUI()
        {
            RDotNet.DataFrame returnDataFrameResid = myREngine.SafeEvaluate("newSRAnalysis$getResidualTable()").AsDataFrame();
            DataTable AHatResiduals = myREngineObject.rDataFrameToDataTable(returnDataFrameResid);

            return AHatResiduals;
        }
        public DataTable GetResidualTableForUI()
        {
            RDotNet.DataFrame returnDataFrameResid = myREngine.SafeEvaluate("newSRAnalysis$getRawResidualTable()").AsDataFrame();
            DataTable AHatResiduals = myREngineObject.rDataFrameToDataTable(returnDataFrameResid);
            return AHatResiduals;
        }
        public DataTable GetThresholdsTableForUI()
        {
            RDotNet.DataFrame returnDataFrameThresh = myREngine.SafeEvaluate("newSRAnalysis$getThresholdDF()").AsDataFrame();
            DataTable AHatThresholds = myREngineObject.rDataFrameToDataTable(returnDataFrameThresh);
            return AHatThresholds;
        }
        public DataTable GetThresholdsTable_ALL_ForUI()
        {
            RDotNet.DataFrame returnDataFrameThresh;
            if (Convert.ToBoolean(myREngine.SafeEvaluate("exists(\"recalcPODClass\")").AsLogical()[0]))
                returnDataFrameThresh = myREngine.SafeEvaluate("recalcPODClass$getGhostThresholdDF()").AsDataFrame();
            else
                returnDataFrameThresh = myREngine.SafeEvaluate("newSRAnalysis$getThresholdDF()").AsDataFrame();
            DataTable AHatThresholds = myREngineObject.rDataFrameToDataTable(returnDataFrameThresh);
            return AHatThresholds;
        }
        public DataTable GetLogitFitTableForUI()
        {
            //ShowResults();
            RDotNet.DataFrame returnDataFrame = myREngine.SafeEvaluate("newSRAnalysis$getResults()").AsDataFrame();
            DataTable AHatPODTable = myREngineObject.rDataFrameToDataTable(returnDataFrame);
            return AHatPODTable;
        }
        public DataTable GetRecalcPODCurveAll()
        {
            //ShowResults();
            RDotNet.DataFrame returnDataFrame;
            if (Convert.ToBoolean(myREngine.SafeEvaluate("exists(\"recalcPODClass\")").AsLogical()[0]))
                returnDataFrame = myREngine.SafeEvaluate("recalcPODClass$getPODCurveAll()").AsDataFrame();
            else
                returnDataFrame = myREngine.SafeEvaluate("newSRAnalysis$getResults()").AsDataFrame();
            DataTable AHatPODTable = myREngineObject.rDataFrameToDataTable(returnDataFrame);
            return AHatPODTable;
        }
        public DataTable GetLogitFitTableForUIAll()
        {
            //ShowResults();
            RDotNet.DataFrame returnDataFrame = myREngine.SafeEvaluate("newSRAnalysis$getResultsAll()").AsDataFrame();
            DataTable AHatPODTable = myREngineObject.rDataFrameToDataTable(returnDataFrame);
            return AHatPODTable;
        }

        public DataTable GetNormalityTableForUI()
        {
            RDotNet.DataFrame returnDataFrame = myREngine.SafeEvaluate("newSRAnalysis$getFreqTable()").AsDataFrame();
            DataTable AHatNormalityTable = myREngineObject.rDataFrameToDataTable(returnDataFrame);
            return AHatNormalityTable;
        }

        public DataTable GetQQTableForUI()
        {
            RDotNet.DataFrame returnDataFrame = myREngine.SafeEvaluate("newSRAnalysis$getQQTable()").AsDataFrame();
            DataTable AHatQQTable = myREngineObject.rDataFrameToDataTable(returnDataFrame);
            return AHatQQTable;
        }

        public DataTable GetNormalCurveTableForUI()
        {
            RDotNet.DataFrame returnDataFrame = myREngine.SafeEvaluate("newSRAnalysis$getNormalCurveTable()").AsDataFrame();
            DataTable AHatnormalCurvetable = myREngineObject.rDataFrameToDataTable(returnDataFrame);
            return AHatnormalCurvetable;
        }
        public double GetRSquaredValue()
        {
            double rSqaured= this.myREngine.SafeEvaluate("newSRAnalysis$getRSquared()").AsNumeric()[0];
            return rSqaured;
        }
        public double GetBoxCoxLamda()
        {
            double lambda = Convert.ToDouble(myREngine.SafeEvaluate("newSRAnalysis$getLambda()").AsNumeric()[0]);
            return lambda;
        }
        public Dictionary<string, double> GetLinearModelStdErrors()
        {
            Dictionary<string, double> standardErrors = new Dictionary<string, double>();
            List<string> aValuesStrings = new List<string> { "slopeStdError", "interceptStdError","residualError", "residualStdError" };
            RDotNet.NumericVector aValuesVector;
            //current list used returns a25, a50 (muhat), a90, sigmahat (SE of a90), and a9095 
            for (int i = 1; i <= 4; i++)
            {
                aValuesVector = this.myREngine.SafeEvaluate($"newSRAnalysis$getRegressionStdErrs()[{i}]").AsNumeric();
                standardErrors.Add(key: aValuesStrings[i - 1], value: (double)aValuesVector[0]);
            }
            return standardErrors;
        }
        public Dictionary<string, double> GetKeyA_Values()
        {
            Dictionary<string, double> aValuesDict = new Dictionary<string, double>();
            List<string> aValuesStrings = new List<string> { "a25", "a50", "a90", "sigmahat", "a9095" };
            RDotNet.NumericVector aValuesVector;
            //current list used returns a25, a50 (muhat), a90, sigmahat (SE of a90), and a9095 
            for (int i = 1; i <= 5; i++)
            {
                aValuesVector = this.myREngine.SafeEvaluate($"newSRAnalysis$getKeyAValues()[{i}]").AsNumeric();
                aValuesDict.Add(key: aValuesStrings[i - 1], value: (double)aValuesVector[0]);
            }            
            return aValuesDict;
        }
        public List<double> GetLinearModelMetrics()
        {
            List<double> metrics = new List<double>();
            double modIntercept = this.myREngine.SafeEvaluate("newSRAnalysis$getModelIntercept()").AsNumeric()[0];
            double modSlope = this.myREngine.SafeEvaluate("newSRAnalysis$getModelSlope()").AsNumeric()[0];
            metrics.Add(modIntercept);
            metrics.Add(modSlope);
            return metrics;
        }
        public List<double> GetLinearTestMetrics()
        {
            //maybe make this a dictionary intead when implmemented?
            List<double> AllLinearTestResults = new List<double>();
            try
            {
                //shapiro-wilk normality test
                double shapiroStat = this.myREngine.SafeEvaluate("newSRAnalysis$getLinearTestResults()[[1]]$statistic[[1]]").AsNumeric()[0];
                AllLinearTestResults.Add(shapiroStat);
                double shapiroPValue = this.myREngine.SafeEvaluate("newSRAnalysis$getLinearTestResults()[[1]]$p.value").AsNumeric()[0];
                AllLinearTestResults.Add(shapiroPValue);
                //non-constance variance test
                double chiSqr = this.myREngine.SafeEvaluate("newSRAnalysis$getLinearTestResults()[[2]][[3]]").AsNumeric()[0];
                AllLinearTestResults.Add(chiSqr);
                double degreesFreedom = this.myREngine.SafeEvaluate("newSRAnalysis$getLinearTestResults()[[2]][[4]]").AsNumeric()[0];
                AllLinearTestResults.Add(degreesFreedom);
                double constVarPValue = this.myREngine.SafeEvaluate("newSRAnalysis$getLinearTestResults()[[2]][[5]]").AsNumeric()[0];
                AllLinearTestResults.Add(constVarPValue);
                //durbin watson test
                double durbinRVal = this.myREngine.SafeEvaluate("newSRAnalysis$getLinearTestResults()[[3]][[1]]").AsNumeric()[0];
                AllLinearTestResults.Add(durbinRVal);
                double durbinDW = this.myREngine.SafeEvaluate("newSRAnalysis$getLinearTestResults()[[3]][[2]]").AsNumeric()[0];
                AllLinearTestResults.Add(durbinDW);
                double durbinPValue = this.myREngine.SafeEvaluate("newSRAnalysis$getLinearTestResults()[[3]][[3]]").AsNumeric()[0];
                AllLinearTestResults.Add(durbinPValue);
                //lack of fit test
                double lackOfFitDegFreedom = this.myREngine.SafeEvaluate("newSRAnalysis$getLinearTestResults()[[4]][[2]]").AsNumeric()[0];
                AllLinearTestResults.Add(lackOfFitDegFreedom);
                double lackOfFitFCalc = this.myREngine.SafeEvaluate("newSRAnalysis$getLinearTestResults()[[4]][[3]]").AsNumeric()[0];
                AllLinearTestResults.Add(lackOfFitFCalc);
                double lackOfFitPValue = this.myREngine.SafeEvaluate("newSRAnalysis$getLinearTestResults()[[4]][[4]]").AsNumeric()[0];
                AllLinearTestResults.Add(lackOfFitPValue);
            }
            catch (RDotNet.EvaluationException)
            {
                //add an empty array of length 11 by default
                for(int i=0; i < 11; i++)
                {
                    AllLinearTestResults.Add(0);
                }
            }
            
            return AllLinearTestResults;
        }
        /// <summary>
        /// method is used for debugging only
        /// </summary>
        public void ShowResults()
        {
            myREngine.SafeEvaluate("newSRAnalysis$plotSimdata(newSRAnalysis$getLinearModel())");
            myREngine.SafeEvaluate("newSRAnalysis$plotPOD(newSRAnalysis$getResults(),newSRAnalysis$getLinearTestResults())");
        }
    }
}
