using System;
using System.Collections.Generic;
using System.Text;

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
        public AnalysistypeTransform(REngineObject analysisEngine, HMAnalysisObject newPFAnalysisObjectInput=null, AHatAnalysisObject newAhatAnalysisObjectInput=null)
        {
            this.analysisEngine = analysisEngine;
            this.newPFAnalysisObject = newPFAnalysisObjectInput;
            this.srsOrRSS = newPFAnalysisObjectInput.SrsOrRSS;
            this.resultsHMAnalysis = newPFAnalysisObjectInput;
            this.newAHatAnalysisObject = newAhatAnalysisObjectInput;
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
        private HMAnalysisObject ExecutePFAnalysisTrans()
        {
            //create the class for hitMissControl
            HitMissAnalysisRControl newHitMissControl = new HitMissAnalysisRControl(analysisEngine);
            //Get crack max and min class
            //create a for loop here to do both the standard and log transform for the graph
            //overwrite the max and min class each iteration
            MaxAndMinClass newMaxMin = new MaxAndMinClass();
            //write the max and min crack size to the pfanalysisObject
            //sets the flaw sizes in flaws temp
            this.newPFAnalysisObject.CurrFlawsAnalysis();
            //TODO: maybe Flaws temp doesn't need to exist? (code breaking here currently)
            //newMaxMin.calcCrackRange(newPFAnalysisObject.FlawsTemp);
            newMaxMin.calcCrackRange(this.newPFAnalysisObject.Flaws);
            this.newPFAnalysisObject.Crckmax = newMaxMin.maxAndMinListControl["Max"];
            this.newPFAnalysisObject.Crckmin = newMaxMin.maxAndMinListControl["Min"];
            newHitMissControl.ExecuteAnalysis(this.newPFAnalysisObject);
            this.newPFAnalysisObject.LogitFitTable = newHitMissControl.GetLogitFitTableForUI();
            //normal tranformation finished! Start log tranformation table
            Dictionary<string, double> finalAValuesDict=newHitMissControl.GetKeyA_Values();
            this.newPFAnalysisObject.A25 = finalAValuesDict["a25"];
            this.newPFAnalysisObject.A50 = finalAValuesDict["a50"];
            this.newPFAnalysisObject.A90 = finalAValuesDict["a90"];
            this.newPFAnalysisObject.Sighat = finalAValuesDict["sigmahat"];
            this.newPFAnalysisObject.A9095 = finalAValuesDict["a9095"];
            this.newPFAnalysisObject.Muhat = finalAValuesDict["a50"];
            //store the original dataframe in the HM analysis object
            this.newPFAnalysisObject.HitMissDataOrig =newHitMissControl.GetOrigHitMissDF();
            //store the covariance matrix values to return to UI
            this.newPFAnalysisObject.CovarianceMatrix = newHitMissControl.GetCovarianceMatrixValues();
            //clear all contents in R and restart the global environment
            analysisEngine.clearGlobalIInREngineObject();
            //Console.WriteLine(newPFAnalysisObject);
            //finish analysis

            //return object to UI
            return this.newPFAnalysisObject;
        }
        private HMAnalysisObject ExecuteRankedSetSampling()
        {
            //Get crack max and min class
            MaxAndMinClass newMaxMin = new MaxAndMinClass();
            //sets the flaw sizes in flaws temp
            newPFAnalysisObject.CurrFlawsAnalysis();
            newMaxMin.calcCrackRange(newPFAnalysisObject.Flaws);
            newPFAnalysisObject.Crckmax = newMaxMin.maxAndMinListControl["Max"];
            newPFAnalysisObject.Crckmin = newMaxMin.maxAndMinListControl["Min"];
            //create the class for hitMissControl
            HitMissAnalysisRControl newHitMissControl = new HitMissAnalysisRControl(analysisEngine);
            newHitMissControl.ExecuteRSS(newPFAnalysisObject);
            //newHitMissControl.getRSS_Results();
            newPFAnalysisObject.LogitFitTable = newHitMissControl.GetLogitFitTableForUI();
            //normal tranformation finished! Start log tranformation table
            Dictionary<string, double> finalAValuesDict = newHitMissControl.GetKeyA_Values();
            newPFAnalysisObject.A25 = finalAValuesDict["a25"];
            newPFAnalysisObject.A50 = finalAValuesDict["a50"];
            newPFAnalysisObject.A90 = finalAValuesDict["a90"];
            newPFAnalysisObject.Sighat = finalAValuesDict["sigmahat"];
            newPFAnalysisObject.A9095 = finalAValuesDict["a9095"];
            newPFAnalysisObject.Muhat = finalAValuesDict["a50"];
            //clear all contents in R and restart the global environment
            analysisEngine.clearGlobalIInREngineObject();
            //Console.WriteLine(newPFAnalysisObject);
            return newPFAnalysisObject;
        }

        private AHatAnalysisObject ExecuteaHatAnalysis()
        {
            //create the class for A Hat Control
            AHatAnalysisRControl newAHatControl = new AHatAnalysisRControl(analysisEngine);
            //pass in the pfobject intance with the parameters set from the UI

            //passfailCensorClass -- doesn't apply to pass fail
            //TODO: will create a class that marks which flaws are being included in the analysis in the event that the user censors the data

            //Get crack max and min class
            //create a for loop here to do both the standard and log transform for the graph
            //overwrite the max and min class each iteration
            MaxAndMinClass newMaxMin = new MaxAndMinClass();
            //write the max and min crack size to the pfanalysisObject
            //sets the flaw sizes in flaws temp
            newPFAnalysisObject.CurrFlawsAnalysis();
            //TODO: maybe Flaws temp doesn't need to exist? (code breaking here currently)
            //newMaxMin.calcCrackRange(newPFAnalysisObject.FlawsTemp);
            newMaxMin.calcCrackRange(newPFAnalysisObject.Flaws);
            this.newAHatAnalysisObject.Crckmax = newMaxMin.maxAndMinListControl["Max"];
            newAHatAnalysisObject.Crckmin = newMaxMin.maxAndMinListControl["Min"];
            //execute analysis and return parameters
            newAHatControl.ExecuteAnalysis(this.newAHatAnalysisObject);
            newAHatAnalysisObject.AHatResultsPOD = newAHatControl.GetLogitFitTableForUI();
            //normal tranformation finished! Start log tranformation table
            Dictionary<string, double> finalAValuesDict = newAHatControl.GetKeyA_Values();
            newPFAnalysisObject.A25 = finalAValuesDict["a25"];
            newPFAnalysisObject.A50 = finalAValuesDict["a50"];
            newPFAnalysisObject.A90 = finalAValuesDict["a90"];
            newPFAnalysisObject.Sighat = finalAValuesDict["sigmahat"];
            newPFAnalysisObject.A9095 = finalAValuesDict["a9095"];
            newPFAnalysisObject.Muhat = finalAValuesDict["a50"];
            //clear all contents in R and restart the global environment
            analysisEngine.clearGlobalIInREngineObject();
            //return the completed aHatAnalysisObject
            return newAHatAnalysisObject;
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
