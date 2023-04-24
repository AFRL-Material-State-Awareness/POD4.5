using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Data;
using System.Linq;

namespace CSharpBackendWithR
{
    public class AnalysisBackendControl
    {
        private HMAnalysisObject newHMAnalysisObject;
        private AHatAnalysisObject newAHatAnalysisObject;
        private IREngineObject analysisEngine;
        private int srsOrRSS;//0=simple random sampling, 1=ranked set sampling
        private HMAnalysisObject resultsHMAnalysis;
        private AHatAnalysisObject resultsAHatAnalysis;
        //controls
        private HitMissAnalysisRControl newHitMissControl;
        private AHatAnalysisRControl newAHatControl;
        public AnalysisBackendControl(IREngineObject analysisEngine, HMAnalysisObject newHitMissAnalysisObjectInput=null, AHatAnalysisObject newAhatAnalysisObjectInput=null)
        {
            this.analysisEngine = analysisEngine;
            this.newHMAnalysisObject = newHitMissAnalysisObjectInput;
            if (newHitMissAnalysisObjectInput != null)
            {
                this.srsOrRSS = newHitMissAnalysisObjectInput.SrsOrRSS;
                this.resultsHMAnalysis = newHitMissAnalysisObjectInput;
                this.newHitMissControl = new HitMissAnalysisRControl(analysisEngine);
                
            }
            this.newAHatAnalysisObject = newAhatAnalysisObjectInput;
            this.newAHatControl = new AHatAnalysisRControl(analysisEngine);
            this.resultsAHatAnalysis = newAhatAnalysisObjectInput;
        }
        public void ExecuteReqSampleAnalysisTypeHitMiss()
        {
            if (this.srsOrRSS == 0)
            {
                this.resultsHMAnalysis = ExecuteHMFullAnalysis();
            }
            else if (this.srsOrRSS == 1)
            {
                this.resultsHMAnalysis = ExecuteRankedSetSampling();
            }
            //HMAnalsysResults = this.resultsHMAnalysis;
        }
        public void ExecuteAnalysisTransforms_HM()
        {
            this.resultsHMAnalysis = ExecutePFAnalysisTrans();
        }
        public void ExecuteAnalysisAHat()
        {
            //more will added to this later
            this.resultsAHatAnalysis = ExecuteaHatAnalysis();

        }
        public void ExecuteAnalysisTransforms()
        {
            this.resultsAHatAnalysis = ExecuteTransformType();
        }
        public void ExecuteThresholdChange()
        {
            this.resultsAHatAnalysis = UpdateThresholdChange();
        }
        
        private HMAnalysisObject ExecuteHMFullAnalysis()
        {
            //create the class for hitMissControl
            //HitMissAnalysisRControl newHitMissControl = new HitMissAnalysisRControl(analysisEngine);
            //Get crack max and min class
            //create a for loop here to do both the standard and log transform for the graph
            //overwrite the max and min class each iteration
            MaxAndMinClass newMaxMin = new MaxAndMinClass();
            //write the max and min crack size to the pfanalysisObject
            newMaxMin.calcCrackRange(this.newHMAnalysisObject.Flaws);
            this.newHMAnalysisObject.Crckmax = newMaxMin.maxAndMinListControl["Max"];
            this.newHMAnalysisObject.Crckmin = newMaxMin.maxAndMinListControl["Min"];
            newHitMissControl.ExecuteAnalysis(this.newHMAnalysisObject);
            ReturnHitMissObjects();
            //clear all contents in R and restart the global environment
            //this.analysisEngine.clearGlobalIInREngineObject();
            //this.analysisEngine.RDotNetEngine.Evaluate("rm(hitMissDF)");
            //Console.WriteLine(newHMAnalysisObject);
            //finish analysis
            //return object to UI
            //this.newHMAnalysisObject.Confidence_level
            //analysisEngine.clearGlobalIInREngineObject();
            return this.newHMAnalysisObject;
        }
        private HMAnalysisObject ExecutePFAnalysisTrans()
        {
            //overwrite the max and min class each iteration
            MaxAndMinClass newMaxMin = new MaxAndMinClass();
            //write the max and min crack size to the pfanalysisObject
            newMaxMin.calcCrackRange(this.newHMAnalysisObject.Flaws);
            this.newHMAnalysisObject.Crckmax = newMaxMin.maxAndMinListControl["Max"];
            this.newHMAnalysisObject.Crckmin = newMaxMin.maxAndMinListControl["Min"];
            newHitMissControl.ExecuteTransformOnlyAnalysis(this.newHMAnalysisObject);
            ReturnTransformHitMissObjects();
            //return object to UI
            return this.newHMAnalysisObject;
        }
        private HMAnalysisObject ExecuteRankedSetSampling()
        {
            //analysisEngine.RDotNetEngine.Evaluate("str(as.list(.GlobalEnv))");
            //Get crack max and min class
            MaxAndMinClass newMaxMin = new MaxAndMinClass();
            newMaxMin.calcCrackRange(newHMAnalysisObject.Flaws);
            this.newHMAnalysisObject.Crckmax = newMaxMin.maxAndMinListControl["Max"];
            newHMAnalysisObject.Crckmin = newMaxMin.maxAndMinListControl["Min"];
            //create the class for hitMissControl
            //HitMissAnalysisRControl this.newHitMissControl = new HitMissAnalysisRControl(analysisEngine);
            this.newHitMissControl.ExecuteRSS(this.newHMAnalysisObject);
            ReturnHitMissObjects();
            //clear all contents in R and restart the global environment
            //this.analysisEngine.clearGlobalIInREngineObject();
            //this.analysisEngine.RDotNetEngine.Evaluate("rm(hitMissDF)");
            //Console.WriteLine(newHMAnalysisObject);
            return this.newHMAnalysisObject;
        }

        private AHatAnalysisObject ExecuteaHatAnalysis()
        {
            //create the class for A Hat Control
            //AHatAnalysisRControl newAHatControl = new AHatAnalysisRControl(analysisEngine);
            //pass in the pfobject intance with the parameters set from the UI
            //Get crack max and min class
            //overwrite the max and min class each iteration
            MaxAndMinClass newMaxMin = new MaxAndMinClass();
            //write the max and min crack size to the ahatAnalysis object
            newMaxMin.calcCrackRange(this.newAHatAnalysisObject.Flaws);
            this.newAHatAnalysisObject.Crckmax = newMaxMin.maxAndMinListControl["Max"];
            this.newAHatAnalysisObject.Crckmin = newMaxMin.maxAndMinListControl["Min"];
            //execute analysis and return parameters
            this.newAHatControl.ExecuteAnalysis(this.newAHatAnalysisObject);
            ReturnSignalResponseObjects();
            //clear all contents in R and restart the global environment/////Removed this for improved performance
            //analysisEngine.clearGlobalIInREngineObject();
            //return the completed aHatAnalysisObject
            return this.newAHatAnalysisObject;
        }
        private AHatAnalysisObject ExecuteTransformType()
        {
            //create the class for A Hat Control
            //AHatAnalysisRControl newAHatControl = new AHatAnalysisRControl(analysisEngine);
            //pass in the pfobject intance with the parameters set from the UI
            //Get crack max and min class
            //overwrite the max and min class each iteration
            MaxAndMinClass newMaxMin = new MaxAndMinClass();
            //write the max and min crack size to the ahatAnalysis object
            newMaxMin.calcCrackRange(this.newAHatAnalysisObject.Flaws);
            this.newAHatAnalysisObject.Crckmax = newMaxMin.maxAndMinListControl["Max"];
            this.newAHatAnalysisObject.Crckmin = newMaxMin.maxAndMinListControl["Min"];
            //execute analysis and return parameters
            this.newAHatControl.ExecuteTransforms(this.newAHatAnalysisObject);
            ReturnSignalResponseObjects();

            return this.newAHatAnalysisObject;
        }
        private AHatAnalysisObject UpdateThresholdChange()
        {
            //executes only when the user changes the threshold value in signal response
            MaxAndMinClass newMaxMin = new MaxAndMinClass();
            //write the max and min crack size to the ahatAnalysis object
            newMaxMin.calcCrackRange(this.newAHatAnalysisObject.Flaws);
            this.newAHatAnalysisObject.Crckmax = newMaxMin.maxAndMinListControl["Max"];
            this.newAHatAnalysisObject.Crckmin = newMaxMin.maxAndMinListControl["Min"];
            //execute analysis and return parameters (only need to get the new a values)
            this.newAHatControl.ExecutethresholdChange(this.newAHatAnalysisObject);
            ReturnThresholdChangeAValues();
            return this.newAHatAnalysisObject;
        }
        public void ReturnHitMissObjects()
        {
            this.newHMAnalysisObject.LogitFitTable = this.newHitMissControl.GetLogitFitTableForUI();
            this.newHMAnalysisObject.ResidualTable = this.newHitMissControl.GetResidualFitTableForUI();
            this.newHMAnalysisObject.IterationTable = this.newHitMissControl.GetIterationTableForUI();
            //normal tranformation finished! Start log tranformation table
            Dictionary<string, double> finalAValuesDict = this.newHitMissControl.GetKeyA_Values();
            this.newHMAnalysisObject.A25 = finalAValuesDict["a25"];
            this.newHMAnalysisObject.A50 = finalAValuesDict["a50"];
            this.newHMAnalysisObject.A90 = finalAValuesDict["a90"];
            this.newHMAnalysisObject.Sighat = finalAValuesDict["sigmahat"];
            this.newHMAnalysisObject.A9095 = finalAValuesDict["a9095"];
            this.newHMAnalysisObject.Muhat = finalAValuesDict["a50"];
            //store the original dataframe in the HM analysis object
            this.newHMAnalysisObject.HitMissDataOrig = this.newHitMissControl.GetOrigHitMissDF();
            //store the covariance matrix values to return to UI
            this.newHMAnalysisObject.CovarianceMatrix = this.newHitMissControl.GetCovarianceMatrixValues();
            //get the goodness of fit
            this.newHMAnalysisObject.GoodnessOfFit = this.newHitMissControl.GetGoodnessOfFit();
            //set the separated flag to warn user if necessary
            if (this.newHMAnalysisObject.RegressionType == "Logistic Regression")
            {
                this.newHMAnalysisObject.Is_Separated = this.newHitMissControl.GetSeparationFlag();
            }
            //used for error if the alogrithm doesn't converge
            this.newHMAnalysisObject.Failed_To_Converge = this.newHitMissControl.GetConvergenceFlag();
        }
        public void ReturnTransformHitMissObjects()
        {
            this.newHMAnalysisObject.LogitFitTable = this.newHitMissControl.GetLogitFitTableForUI();
            this.newHMAnalysisObject.ResidualTable = this.newHitMissControl.GetResidualFitTableForUI();
            //store the original dataframe in the HM analysis object
            this.newHMAnalysisObject.HitMissDataOrig = this.newHitMissControl.GetOrigHitMissDF();
            //set the separated flag to warn user if necessary
            if (this.newHMAnalysisObject.RegressionType == "Logistic Regression")
            {
                this.newHMAnalysisObject.Is_Separated = this.newHitMissControl.GetSeparationFlag();
            }
            //used for error if the alogrithm doesn't converge
            this.newHMAnalysisObject.Failed_To_Converge = this.newHitMissControl.GetConvergenceFlag();
        }
        public void ReturnSignalResponseObjects()
        {
            this.newAHatAnalysisObject.AHatResultsPOD = this.newAHatControl.GetLogitFitTableForUI();
            this.newAHatAnalysisObject.AHatResultsPOD_All = this.newAHatControl.GetRecalcPODCurveAll();
            this.newAHatAnalysisObject.AHatResultsLinear = this.newAHatControl.GetLinearFitTableForUI();
            this.newAHatAnalysisObject.AHatResultsResidUncensored = this.newAHatControl.GetResidualUncensoredTableForUI();
            this.newAHatAnalysisObject.AHatResultsResid = this.newAHatControl.GetResidualTableForUI();
            this.newAHatAnalysisObject.AHatThresholdsTable = this.newAHatControl.GetThresholdsTableForUI();
            this.newAHatAnalysisObject.AHatThresholdsTable_All = this.newAHatControl.GetThresholdsTable_ALL_ForUI();
            this.newAHatAnalysisObject.AHatNormalityTable = this.newAHatControl.GetNormalityTableForUI();
            this.newAHatAnalysisObject.AHatNormalCurveTable = this.newAHatControl.GetNormalCurveTableForUI();
            //get slope and intercept (need to add the errors for each as well)
            List<double> linearMetrics = this.newAHatControl.GetLinearModelMetrics();
            this.newAHatAnalysisObject.Intercept = linearMetrics[0];
            this.newAHatAnalysisObject.Slope = linearMetrics[1];
            //get r-squared value
            this.newAHatAnalysisObject.RSqaured = this.newAHatControl.GetRSquaredValue();
            //get lambda from box-cox if applicable
            if (this.newAHatAnalysisObject.ModelType >= 5 && this.newAHatAnalysisObject.ModelType <= 7)
            {
                this.newAHatAnalysisObject.Lambda = this.newAHatControl.GetBoxCoxLamda();
            }
            //get standard errors for linear model
            Dictionary<string, double> standardErrors = this.newAHatControl.GetLinearModelStdErrors();
            this.newAHatAnalysisObject.SlopeStdErr = standardErrors["slopeStdError"];
            this.newAHatAnalysisObject.InterceptStdErr = standardErrors["interceptStdError"];
            this.newAHatAnalysisObject.ResidualError = standardErrors["residualError"];
            this.newAHatAnalysisObject.ResidualStdErr = standardErrors["residualStdError"];
            //normal tranformation finished! Start log tranformation table
            Dictionary<string, double> finalAValuesDict = this.newAHatControl.GetKeyA_Values();
            this.newAHatAnalysisObject.A25 = finalAValuesDict["a25"];
            this.newAHatAnalysisObject.A50 = finalAValuesDict["a50"];
            this.newAHatAnalysisObject.A90 = finalAValuesDict["a90"];
            this.newAHatAnalysisObject.Sighat = finalAValuesDict["sigmahat"];
            this.newAHatAnalysisObject.A9095 = finalAValuesDict["a9095"];
            this.newAHatAnalysisObject.Muhat = finalAValuesDict["a50"];
            //get linear test results
            List<double> linTestResults = this.newAHatControl.GetLinearTestMetrics();
            this.newAHatAnalysisObject.ShapiroTestStat = linTestResults[0];
            this.newAHatAnalysisObject.ShapiroPValue = linTestResults[1];
            this.newAHatAnalysisObject.ChiSqValue = linTestResults[2];
            this.newAHatAnalysisObject.ChiSqDF = Convert.ToInt32(linTestResults[3]);
            this.newAHatAnalysisObject.ChiSqPValue = linTestResults[4];
            this.newAHatAnalysisObject.DurbinWatson_r = linTestResults[5];
            this.newAHatAnalysisObject.DurbinWatsonDW = linTestResults[6];
            this.newAHatAnalysisObject.DurbinWatsonPValue = linTestResults[7];
            this.newAHatAnalysisObject.LackOfFitDegFreedom = linTestResults[8];
            this.newAHatAnalysisObject.LackOfFitFCalc = linTestResults[9];
            this.newAHatAnalysisObject.LackOfFitPValue = linTestResults[10];
        }
        public void ReturnThresholdChangeAValues()
        {
            //store the new POD curve table
            this.newAHatAnalysisObject.AHatResultsPOD = this.newAHatControl.GetLogitFitTableForUI();
            this.newAHatAnalysisObject.AHatResultsPOD_All = this.newAHatControl.GetRecalcPODCurveAll();
            //normal tranformation finished! Start log tranformation table
            Dictionary<string, double> finalAValuesDict = this.newAHatControl.GetKeyA_Values();
            this.newAHatAnalysisObject.A25 = finalAValuesDict["a25"];
            this.newAHatAnalysisObject.A50 = finalAValuesDict["a50"];
            this.newAHatAnalysisObject.A90 = finalAValuesDict["a90"];
            this.newAHatAnalysisObject.Sighat = finalAValuesDict["sigmahat"];
            this.newAHatAnalysisObject.A9095 = finalAValuesDict["a9095"];
            this.newAHatAnalysisObject.Muhat = finalAValuesDict["a50"];
        }
        public HMAnalysisObject HMAnalsysResults
        {
            set { this.resultsHMAnalysis = value; }
            get { return this.resultsHMAnalysis; }
        }
        public AHatAnalysisObject AHatAnalysisResults
        {
            set { this.resultsAHatAnalysis = value; }
            get { return this.resultsAHatAnalysis; }
        }
    }
}
