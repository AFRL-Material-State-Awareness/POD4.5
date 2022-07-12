using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpBackendWithR
{
    public class AnalysistypeTransform
    {
        private HMAnalysisObjectTransform newPFAnalysisObject;
        //private ahatAnalysisObject ahatAnalysisObject;
        private REngineObject analysisEngine;
        private int srsOrRSS;//0=simple random sampling, 1=ranked set sampling
        private HMAnalysisObjectTransform resultsHMAnalsys;
        public AnalysistypeTransform(HMAnalysisObjectTransform newPFAnalysisObjectInput, REngineObject analysisEngine)
        {
            this.newPFAnalysisObject = newPFAnalysisObjectInput;
            this.analysisEngine = analysisEngine;
            this.srsOrRSS = newPFAnalysisObjectInput.SrsOrRSS;
            this.resultsHMAnalsys = newPFAnalysisObjectInput;
        }
        public void ExecuteReqSampleAnalysisType()
        {
            if (this.srsOrRSS == 0)
            {
                resultsHMAnalsys = ExecutePFAnalysisTrans();
            }
            else if (this.srsOrRSS == 1)
            {
                resultsHMAnalsys = ExecuteRankedSetSampling();
            }
            HMAnalsysResults = resultsHMAnalsys;
        }
        private HMAnalysisObjectTransform ExecutePFAnalysisTrans()
        {
            //create the class for hitMissControl
            HitMissAnalysisRControl newHitMissControl = new HitMissAnalysisRControl(analysisEngine);
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
            newPFAnalysisObject.Crckmax = newMaxMin.maxAndMinListControl["Max"];
            newPFAnalysisObject.Crckmin = newMaxMin.maxAndMinListControl["Min"];
            newHitMissControl.ExecuteAnalysis(newPFAnalysisObject);
            newPFAnalysisObject.LogitFitTable = newHitMissControl.getLogitFitTableForUI();
            //normal tranformation finished! Start log tranformation table
            Dictionary<string, double> finalAValuesDict=newHitMissControl.getKeyA_Values();
            newPFAnalysisObject.A25 = finalAValuesDict["a25"];
            newPFAnalysisObject.A50 = finalAValuesDict["a50"];
            newPFAnalysisObject.A90 = finalAValuesDict["a90"];
            newPFAnalysisObject.Sighat = finalAValuesDict["sigmahat"];
            newPFAnalysisObject.A9095 = finalAValuesDict["a9095"];
            newPFAnalysisObject.Muhat = finalAValuesDict["a50"];
            //store the original dataframe in the HM analysis object
            newPFAnalysisObject.HitMissDataOrig =newHitMissControl.getOrigHitMissDF();
            //clear all contents in R and restart the global environment
            analysisEngine.clearGlobalIInREngineObject();
            //Console.WriteLine(newPFAnalysisObject);

            


            //finish analysis

            //return object to UI
            return newPFAnalysisObject;
        }
        private HMAnalysisObjectTransform ExecuteRankedSetSampling()
        {
            //Get crack max and min class
            MaxAndMinClass newMaxMin = new MaxAndMinClass();
            //sets the flaw sizes in flaws temp
            newPFAnalysisObject.CurrFlawsAnalysis();
            newMaxMin.calcCrackRange(newPFAnalysisObject.FlawsTemp);
            newPFAnalysisObject.Crckmax = newMaxMin.maxAndMinListControl["Max"];
            newPFAnalysisObject.Crckmin = newMaxMin.maxAndMinListControl["Min"];
            //create the class for hitMissControl
            HitMissAnalysisRControl newHitMissControl = new HitMissAnalysisRControl(analysisEngine);
            newHitMissControl.ExecuteRSS(newPFAnalysisObject);
            //newHitMissControl.getRSS_Results();
            newPFAnalysisObject.LogitFitTable = newHitMissControl.getLogitFitTableForUI();
            //normal tranformation finished! Start log tranformation table
            Dictionary<string, double> finalAValuesDict = newHitMissControl.getKeyA_Values();
            newPFAnalysisObject.A25 = finalAValuesDict["a25"];
            newPFAnalysisObject.A50 = finalAValuesDict["a50"];
            newPFAnalysisObject.A90 = finalAValuesDict["a90"];
            newPFAnalysisObject.Sighat = finalAValuesDict["sigmahat"];
            newPFAnalysisObject.A9095 = finalAValuesDict["a9095"];
            newPFAnalysisObject.Muhat = finalAValuesDict["a50"];
            //clear all contents in R and restart the global environment
            analysisEngine.clearGlobalIInREngineObject();
            Console.WriteLine(newPFAnalysisObject);
            return newPFAnalysisObject;
        }

        //public ahatAnalsyisObject executeaHatAnalysis(ahatAnalsyisObject newPFAnalysisObjectInput){
        // TODO
        //}

        public HMAnalysisObjectTransform HMAnalsysResults
        {
            set { this.resultsHMAnalsys = value; }
            get { return this.resultsHMAnalsys; }
        }
    }
}
