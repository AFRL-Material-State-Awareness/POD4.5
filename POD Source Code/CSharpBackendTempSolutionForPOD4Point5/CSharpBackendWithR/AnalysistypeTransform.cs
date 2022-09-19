using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Data;
using System.Linq;
namespace CSharpBackendWithR
{
    public class AnalysistypeTransform
    {
        private HMAnalysisObject newPFAnalysisObject;
        private AHatAnalysisObject newAHatAnalysisObject;
        private REngineObject analysisEngine;
        private int srsOrRSS;//0=simple random sampling, 1=ranked set sampling
        private HMAnalysisObject resultsHMAnalysis;
        private AHatAnalysisObject resultsAHatAnalysis;
        //controls
        private HitMissAnalysisRControl newHitMissControl;
        private AHatAnalysisRControl newAHatControl;
        public AnalysistypeTransform(REngineObject analysisEngine, HMAnalysisObject newHitMissAnalysisObjectInput=null, AHatAnalysisObject newAhatAnalysisObjectInput=null)
        {
            this.analysisEngine = analysisEngine;
            this.newPFAnalysisObject = newHitMissAnalysisObjectInput;
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
                this.resultsHMAnalysis = ExecutePFAnalysisTrans();
            }
            else if (this.srsOrRSS == 1)
            {
                this.resultsHMAnalysis = ExecuteRankedSetSampling();
            }
            //HMAnalsysResults = this.resultsHMAnalysis;
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
        
        private HMAnalysisObject ExecutePFAnalysisTrans()
        {
            //create the class for hitMissControl
            //HitMissAnalysisRControl newHitMissControl = new HitMissAnalysisRControl(analysisEngine);
            //Get crack max and min class
            //create a for loop here to do both the standard and log transform for the graph
            //overwrite the max and min class each iteration
            MaxAndMinClass newMaxMin = new MaxAndMinClass();
            //write the max and min crack size to the pfanalysisObject
            //TODO: maybe Flaws temp doesn't need to exist? (code breaking here currently)
            //newMaxMin.calcCrackRange(newPFAnalysisObject.FlawsTemp);
            newMaxMin.calcCrackRange(this.newPFAnalysisObject.Flaws);
            this.newPFAnalysisObject.Crckmax = newMaxMin.maxAndMinListControl["Max"];
            this.newPFAnalysisObject.Crckmin = newMaxMin.maxAndMinListControl["Min"];
            newHitMissControl.ExecuteAnalysis(this.newPFAnalysisObject);
            ReturnHitMissObjects();
            //clear all contents in R and restart the global environment
            //this.analysisEngine.clearGlobalIInREngineObject();
            //this.analysisEngine.RDotNetEngine.Evaluate("rm(hitMissDF)");
            //Console.WriteLine(newPFAnalysisObject);
            //finish analysis
            //return object to UI
            return this.newPFAnalysisObject;
        }
        private HMAnalysisObject ExecuteRankedSetSampling()
        {
            //analysisEngine.RDotNetEngine.Evaluate("str(as.list(.GlobalEnv))");
            //Get crack max and min class
            MaxAndMinClass newMaxMin = new MaxAndMinClass();
            newMaxMin.calcCrackRange(newPFAnalysisObject.Flaws);
            this.newPFAnalysisObject.Crckmax = newMaxMin.maxAndMinListControl["Max"];
            newPFAnalysisObject.Crckmin = newMaxMin.maxAndMinListControl["Min"];
            //create the class for hitMissControl
            //HitMissAnalysisRControl this.newHitMissControl = new HitMissAnalysisRControl(analysisEngine);
            this.newHitMissControl.ExecuteRSS(this.newPFAnalysisObject);
            ReturnHitMissObjects();
            //clear all contents in R and restart the global environment
            //this.analysisEngine.clearGlobalIInREngineObject();
            //this.analysisEngine.RDotNetEngine.Evaluate("rm(hitMissDF)");
            //Console.WriteLine(newPFAnalysisObject);
            return this.newPFAnalysisObject;
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
            //return parameters unique to transform panel analysis
            this.newAHatAnalysisObject.AHatResultsLinear = this.newAHatControl.GetLinearFitTableForUI();
            this.newAHatAnalysisObject.AHatResultsResidUncensored = this.newAHatControl.GetResidualUncensoredTableForUI();
            this.newAHatAnalysisObject.AHatResultsResid = this.newAHatControl.GetResidualTableForUI();
            //get lambda from box-cox if applicable
            if (this.newAHatAnalysisObject.ModelType >= 5 && this.newAHatAnalysisObject.ModelType <= 7)
            {
                this.newAHatAnalysisObject.Lambda = this.newAHatControl.GetBoxCoxLamda();
            }

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
            this.newPFAnalysisObject.LogitFitTable = this.newHitMissControl.GetLogitFitTableForUI();
            this.newPFAnalysisObject.ResidualTable = this.newHitMissControl.GetResidualFitTableForUI();
            this.newPFAnalysisObject.IterationTable = this.newHitMissControl.GetIterationTableForUI();
            //normal tranformation finished! Start log tranformation table
            Dictionary<string, double> finalAValuesDict = this.newHitMissControl.GetKeyA_Values();
            this.newPFAnalysisObject.A25 = finalAValuesDict["a25"];
            this.newPFAnalysisObject.A50 = finalAValuesDict["a50"];
            this.newPFAnalysisObject.A90 = finalAValuesDict["a90"];
            this.newPFAnalysisObject.Sighat = finalAValuesDict["sigmahat"];
            this.newPFAnalysisObject.A9095 = finalAValuesDict["a9095"];
            this.newPFAnalysisObject.Muhat = finalAValuesDict["a50"];
            //store the original dataframe in the HM analysis object
            this.newPFAnalysisObject.HitMissDataOrig = this.newHitMissControl.GetOrigHitMissDF();
            //store the covariance matrix values to return to UI
            this.newPFAnalysisObject.CovarianceMatrix = this.newHitMissControl.GetCovarianceMatrixValues();
            //get the goodness of fit
            this.newPFAnalysisObject.GoodnessOfFit = this.newHitMissControl.GetGoodnessOfFit();
            //set the separated flag to warn user if necessary
            if (this.newPFAnalysisObject.RegressionType == "Logistic Regression")
            {
                this.newPFAnalysisObject.Is_Separated = this.newHitMissControl.GetSeparationFlag();
            }
            //used for error if the alogrithm doesn't converge
            this.newPFAnalysisObject.Failed_To_Converge = this.newHitMissControl.GetConvergenceFlag();
        }
        public void ReturnSignalResponseObjects()
        {
            this.newAHatAnalysisObject.AHatResultsPOD = this.newAHatControl.GetLogitFitTableForUI();
            this.newAHatAnalysisObject.AHatResultsLinear = this.newAHatControl.GetLinearFitTableForUI();
            this.newAHatAnalysisObject.AHatResultsResidUncensored = this.newAHatControl.GetResidualUncensoredTableForUI();
            this.newAHatAnalysisObject.AHatResultsResid = this.newAHatControl.GetResidualTableForUI();
            this.newAHatAnalysisObject.AHatThresholdsTable = this.newAHatControl.GetThresholdsTableForUI();
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
        static void printDT(DataTable data)
        {
            //Console.WriteLine();
            Debug.WriteLine('\n');
            Dictionary<string, int> colWidths = new Dictionary<string, int>();

            foreach (DataColumn col in data.Columns)
            {
                //Console.Write(col.ColumnName);
                Debug.Write(col.ColumnName);
                var maxLabelSize = data.Rows.OfType<DataRow>()
                        .Select(m => (m.Field<object>(col.ColumnName)?.ToString() ?? "").Length)
                        .OrderByDescending(m => m).FirstOrDefault();

                colWidths.Add(col.ColumnName, maxLabelSize);
                for (int i = 0; i < maxLabelSize - col.ColumnName.Length + 10; i++) Debug.Write(" ");
            }

            //Console.WriteLine();
            Debug.WriteLine('\n');
            int rowCounter = 0;
            int limit = 5;
            foreach (DataRow dataRow in data.Rows)
            {
                for (int j = 0; j < dataRow.ItemArray.Length; j++)
                {
                    //Console.Write(dataRow.ItemArray[j]);
                    Debug.Write((dataRow.ItemArray[j]).ToString());
                    for (int i = 0; i < colWidths[data.Columns[j].ColumnName] - dataRow.ItemArray[j].ToString().Length + 10; i++) Debug.Write(" ");
                }
                //Console.WriteLine();
                Debug.WriteLine('\n');
                rowCounter = rowCounter + 1;
                if (rowCounter >= limit)
                {
                    break;
                }
            }
            Debug.WriteLine('\n');
        }
    }
}
