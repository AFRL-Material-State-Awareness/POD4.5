using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpBackendWithR
{
    class AnalysistypeTransform
    {
        private HMAnalysisObjectTransform newPFAnalysisObject;
        //private ahatAnalysisObject ahatAnalysisObject;
        private REngineObject analysisEngine;

        public AnalysistypeTransform(HMAnalysisObjectTransform newPFAnalysisObject, REngineObject analysisEngine)
        {
            this.newPFAnalysisObject = newPFAnalysisObject;
            this.analysisEngine = analysisEngine;
        }

        public HMAnalysisObjectTransform executePFAnalysisTrans()
        {
            //create the clas for hitMissControl
            HitMissAnalysisRControl newHitMissControl = new HitMissAnalysisRControl(analysisEngine);
            //pass in the pfobject intance with the parameters set from the UI

            //passfailCensorClass -- doesn't apply to pass fail
            //TODO: will creat a class that marks which flaws are being included in the analysis in the event that the user censors the data

            
            //Get crack max and min class
            //create a for loop here to do both the standard and log transform for the graph
            //overwrite the max and min class each iteration
            MaxAndMinClass newMaxMin = new MaxAndMinClass();
            
            //write the max and min crack size to the pfanalysisObject
            if (newPFAnalysisObject.ModelType == 0)
            {
                //sets the flaw sizes in flaws temp
                newPFAnalysisObject.CurrFlawsAnalysis();
                newMaxMin.calcCrackRange(newPFAnalysisObject.FlawsTemp);
                newPFAnalysisObject.Crckmax = newMaxMin.maxAndMinListControl["Max"];
                newPFAnalysisObject.Crckmin = newMaxMin.maxAndMinListControl["Min"];
                newHitMissControl.ExecuteAnalysis(newPFAnalysisObject);
                newPFAnalysisObject.LogitFitTable = newHitMissControl.getLogitFitTableForUI();
                //normal tranformation finished! Start log tranformation table
                //newPFAnalysisObject.ModelType = 1;
                Dictionary<string, double> finalAValuesDict=newHitMissControl.getKeyA_Values();
                newPFAnalysisObject.A25 = finalAValuesDict["a25"];
                newPFAnalysisObject.A50 = finalAValuesDict["a50"];
                newPFAnalysisObject.A90 = finalAValuesDict["a90"];
                newPFAnalysisObject.Sighat = finalAValuesDict["sigmahat"];
                newPFAnalysisObject.A9095 = finalAValuesDict["a9095"];
                newPFAnalysisObject.Muhat = finalAValuesDict["a50"];
                //clear all contents in R and restart the global environment
                analysisEngine.clearGlobalIInREngineObject();
                Console.WriteLine(newPFAnalysisObject);
                newPFAnalysisObject.ModelType = 1;
            }
            else if (newPFAnalysisObject.ModelType == 1)
            {
                Console.WriteLine("performing log analysis");
                newPFAnalysisObject.CurrFlawsAnalysis();
                newMaxMin.calcCrackRange(newPFAnalysisObject.FlawsTemp);
                newPFAnalysisObject.Crckmax = newMaxMin.maxAndMinListControl["Max"];
                newPFAnalysisObject.Crckmin = newMaxMin.maxAndMinListControl["Min"];
                newHitMissControl.ExecuteAnalysis(newPFAnalysisObject);
                newPFAnalysisObject.LogLogitFitTable = newHitMissControl.getLogitFitTableForUI();
                //reset modeltype variable
                newPFAnalysisObject.ModelType = 0;
            }
            //pass fail solve class for muhat and sigma hat

            //pass fail residuals

            //repeat the process with a log transform
            //finish analysis

            //return object to UI
            return newPFAnalysisObject;




        }

        //public ahatAnalsyisObject executeaHatAnalysis(ahatAnalsyisObject newPFAnalysisObject){
        // TODO
        //}
    }
}
