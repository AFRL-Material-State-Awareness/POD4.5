using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDotNet;
namespace CSharpBackendWithR
{
    class AHatAnalysisRControl
    {
        private REngineObject myREngineObject;
        private REngine myREngine;
        public AHatAnalysisRControl(REngineObject myREngineObject)
        {
            this.myREngineObject = myREngineObject;
            this.myREngine = myREngineObject.RDotNetEngine;
        }
        //convert to a dataframe by adding strings
        private void createDataFrameinGlobalEnvr(AHatAnalysisObject newAHatAnalysis)
        {
            //create private variable names to shorten length
            List<double> cracks = newAHatAnalysis.Flaws;
            //TODO: FIX MEEE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            newAHatAnalysis.SignalResponseName = "y";
            List<double> SignalResponse = newAHatAnalysis.Responses_all[newAHatAnalysis.SignalResponseName];
            List<int> indices = new List<int>();
            //needed for the r code 
            //this.myREngine.Evaluate("normSampleSize<-" + a_x_n.ToString());
            if (newAHatAnalysis.ModelType == 1)
            {
                this.myREngine.Evaluate("isLog=FALSE");
                //create index column for dataframe
                for (int i = 1; i <= cracks.Count; i++)
                {
                    indices.Add(i);
                }
                //initialize the matrices used to create the input dataframe
                this.myREngine.Evaluate("Index<-matrix(" + indices[0].ToString() + ")");
                this.myREngine.Evaluate("x<-c(" + cracks[0].ToString() + ")");
                this.myREngine.Evaluate("y<-c(" + SignalResponse[0].ToString() + ")");
                //acumulate r matrices in order to create the dataframe
                for (int i = 1; i < cracks.Count; i++)
                {
                    this.myREngine.Evaluate("Index<-c(Index," + indices[i].ToString() + ")");
                    this.myREngine.Evaluate("x<-c(x," + cracks[i].ToString() + ")");
                    this.myREngine.Evaluate("y<-c(y," + SignalResponse[i].ToString() + ")");
                };
                //build the dataframe in the global environment
                //this dataframe will remain in the global environment
                this.myREngine.Evaluate("AHatDF<-data.frame(Index,x,y)");
                this.myREngine.Evaluate("rm(Index)");
                this.myREngine.Evaluate("rm(x)");
                this.myREngine.Evaluate("rm(y)");
            }
            /*
            else if (newAHatAnalysis.ModelType == 2)
            {
                this.myREngine.Evaluate("isLog=TRUE");
                //create index column for dataframe
                for (int i = 1; i <= cracks.Count; i++)
                {
                    indices.Add(i);
                }
                //initialize the matrices used to create the input dataframe
                this.myREngine.Evaluate("Index<-matrix(" + indices[0].ToString() + ")");
                this.myREngine.Evaluate("x<-c(log(" + cracks[0].ToString() + "))");
                this.myREngine.Evaluate("y<-c(" + SignalResponse[0].ToString() + ")");
                //acumulate r matrices in order to create the dataframe
                for (int i = 1; i < cracks.Count; i++)
                {
                    this.myREngine.Evaluate("Index<-c(Index," + indices[i].ToString() + ")");
                    this.myREngine.Evaluate("x<-c(x,log(" + cracks[i].ToString() + "))");
                    this.myREngine.Evaluate("y<-c(y," + SignalResponse[i].ToString() + ")");
                };
                //build the dataframe in the global environment
                //this dataframe will remain in the global environment
                this.myREngine.Evaluate("hitMissDF<-data.frame(Index,x,y)");
                this.myREngine.Evaluate("rm(Index)");
                this.myREngine.Evaluate("rm(x)");
                this.myREngine.Evaluate("rm(y)");
            }
            */
            else
            {
                throw new Exception("model type not found exception for AHAT! (currently only supports linear)");
            }
        }
        public void ExecuteAnalysis(AHatAnalysisObject newTranformAnalysis)
        {
            this.createDataFrameinGlobalEnvr(newTranformAnalysis);
            //System.Diagnostics.Debug.WriteLine(this.myREngine.Evaluate("str('str(as.list(.GlobalEnv)')").ToString());
            //this.myREngine.Evaluate("str('str(as.list(.GlobalEnv)')");
            //execute class with appropriate parameters
            this.myREngine.Evaluate("newSRAnalysis<-AHatAnalysis$new(SignalRespDF=AHatDF, y_dec='" + newTranformAnalysis.YDecision+")");
            this.myREngine.Evaluate("newSRAnalysis$executeAhatvsA()");
        }
        public DataTable GetLogitFitTableForUI()
        {
            ShowResults();
            RDotNet.DataFrame returnDataFrame = myREngine.Evaluate("newSRAnalysis$getResults()").AsDataFrame();
            DataTable AHatPODTable = myREngineObject.rDataFrameToDataTable(returnDataFrame);
            return AHatPODTable;
        }
        public Dictionary<string, double> GetKeyA_Values()
        {
            Dictionary<string, double> aValuesDict = new Dictionary<string, double>();
            List<string> aValuesStrings = new List<string> { "a25", "a50", "a90", "sigmahat", "a9095" };
            RDotNet.NumericVector aValuesVector;
            //current list used returns a25, a50 (muhat), a90, sigmahat (SE of a90), and a9095 
            for (int i = 1; i <= 5; i++)
            {
                this.myREngine.Evaluate("print(newSRAnalysis$getKeyAValues()[" + i + "])").AsList();
                aValuesVector = this.myREngine.Evaluate("newSRAnalysis$getKeyAValues()[" + i + "]").AsNumeric();
                aValuesDict.Add(key: aValuesStrings[i - 1], value: (double)aValuesVector[0]);
            }            return aValuesDict;
        }
        public List<double> GetLinearTestMetrics()
        {
            //maybe make this a dictionary intead when implmemented?
            List<double> AllLinearTestResults = new List<double>();
            return AllLinearTestResults;
        }
        public void ShowResults()
        {
            myREngine.Evaluate("newSRAnalysis$plotPOD(newSRAnalysis$getResults(),n ewSRAnalysis$getLinearTestResults())");
        }
    }
}
