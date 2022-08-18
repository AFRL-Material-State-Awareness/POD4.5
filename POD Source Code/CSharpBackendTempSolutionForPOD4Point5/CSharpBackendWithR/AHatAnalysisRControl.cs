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
            List<double> cracksCensored = newAHatAnalysis.FlawsCensored;
            
            List<double> SignalResponse = newAHatAnalysis.Responses[newAHatAnalysis.SignalResponseName];
            List<int> indices = new List<int>();
            //needed for the r code 
            //this.myREngine.Evaluate("normSampleSize<-" + a_x_n.ToString());
            if (newAHatAnalysis.ResponsesCensoredLeft.Count() != 0 || newAHatAnalysis.ResponsesCensoredRight.Count() != 0)
            {
                List<double> responsesLeftCensored = newAHatAnalysis.ResponsesCensoredLeft[newAHatAnalysis.SignalResponseName];
                List<double> responsesRightCensored = newAHatAnalysis.ResponsesCensoredRight[newAHatAnalysis.SignalResponseName];
                this.myREngine.Evaluate("event<- c()");
                bool updatedEvent = false;
                for (int i = 0; i < cracks.Count; i++)
                {
                    updatedEvent = false;
                    foreach (double j in responsesRightCensored)
                    {
                        if (SignalResponse[i] == j)
                        {
                            this.myREngine.Evaluate("event<- c(event, 0)");
                            updatedEvent = true;
                            break;
                        }
                    }
                    foreach (double j in responsesLeftCensored)
                    {
                        if (SignalResponse[i] == j)
                        {
                            this.myREngine.Evaluate("event<- c(event, 2)");
                            updatedEvent = true;
                            break;
                        }
                    }
                    if (updatedEvent == false)
                    {
                        this.myREngine.Evaluate("event<- c(event, 1)");
                    }
                }

            }
            //mark all the points as uncensored by default
            else
            {
                this.myREngine.Evaluate("event<- c()");
                for (int i = 0; i < cracks.Count; i++)
                {
                    this.myREngine.Evaluate("event<- c(event, 1)");
                }
            }
            switch (newAHatAnalysis.ModelType)
            {
                case 1:
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
                    }
                    break;
                case 2:
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
                    }
                    break;
                case 3:
                    //create index column for dataframe
                    for (int i = 1; i <= cracks.Count; i++)
                    {
                        indices.Add(i);
                    }
                    //initialize the matrices used to create the input dataframe
                    this.myREngine.Evaluate("Index<-matrix(" + indices[0].ToString() + ")");
                    this.myREngine.Evaluate("x<-c(" + cracks[0].ToString() + ")");
                    this.myREngine.Evaluate("y<-c(log(" + SignalResponse[0].ToString() + "))");
                    //acumulate r matrices in order to create the dataframe
                    for (int i = 1; i < cracks.Count; i++)
                    {
                        this.myREngine.Evaluate("Index<-c(Index," + indices[i].ToString() + ")");
                        this.myREngine.Evaluate("x<-c(x," + cracks[i].ToString() + ")");
                        this.myREngine.Evaluate("y<-c(y, log(" + SignalResponse[i].ToString() + "))");
                    }
                    break;
                case 4:
                    //create index column for dataframe
                    for (int i = 1; i <= cracks.Count; i++)
                    {
                        indices.Add(i);
                    }
                    //initialize the matrices used to create the input dataframe
                    this.myREngine.Evaluate("Index<-matrix(" + indices[0].ToString() + ")");
                    this.myREngine.Evaluate("x<-c(log(" + cracks[0].ToString() + "))");
                    this.myREngine.Evaluate("y<-c(log(" + SignalResponse[0].ToString() + "))");
                    //this.myREngine.Evaluate("x<-c(" + cracks[0].ToString() + ")");
                    //this.myREngine.Evaluate("y<-c(" + SignalResponse[0].ToString() + ")");
                    //acumulate r matrices in order to create the dataframe
                    for (int i = 1; i < cracks.Count; i++)
                    {
                        this.myREngine.Evaluate("Index<-c(Index," + indices[i].ToString() + ")");
                        this.myREngine.Evaluate("x<-c(x,log(" + cracks[i].ToString() + "))");
                        this.myREngine.Evaluate("y<-c(y, log(" + SignalResponse[i].ToString() + "))");
                        //this.myREngine.Evaluate("x<-c(x," + cracks[i].ToString() + ")");
                        //this.myREngine.Evaluate("y<-c(y," + SignalResponse[i].ToString() + ")");

                    }
                    break;
                default:
                    throw new Exception("model type not found exception for AHAT!");
            }
            //build the dataframe in the global environment
            //this dataframe will remain in the global environment
            this.myREngine.Evaluate("AHatDF<-data.frame(Index,x,y, event)");
            this.myREngine.Evaluate("rm(Index)");
            this.myREngine.Evaluate("rm(x)");
            this.myREngine.Evaluate("rm(y)");
            this.myREngine.Evaluate("rm(event)");
        }
        public void ExecuteAnalysis(AHatAnalysisObject newTranformAnalysis)
        {
            this.createDataFrameinGlobalEnvr(newTranformAnalysis);
            //System.Diagnostics.Debug.WriteLine(this.myREngine.Evaluate("str('str(as.list(.GlobalEnv)')").ToString());
            //this.myREngine.Evaluate("str('str(as.list(.GlobalEnv)')");
            //execute class with appropriate parameters
            this.myREngine.Evaluate("newSRAnalysis<-AHatAnalysis$new(signalRespDF=AHatDF, y_dec=" + newTranformAnalysis.Pod_threshold+", modelType="+newTranformAnalysis.ModelType+")");
            this.myREngine.Evaluate("newSRAnalysis$executeAhatvsA()");
        }
        public DataTable GetLinearFitTableForUI()
        {
            RDotNet.DataFrame returnDataFrameLinear = myREngine.Evaluate("newSRAnalysis$getLinearModel()").AsDataFrame();
            DataTable AHatLinTable = myREngineObject.rDataFrameToDataTable(returnDataFrameLinear);
            return AHatLinTable;

        }
        public DataTable GetResidualUncensoredTableForUI()
        {
            RDotNet.DataFrame returnDataFrameResid = myREngine.Evaluate("newSRAnalysis$getResidualTable()").AsDataFrame();
            DataTable AHatResiduals = myREngineObject.rDataFrameToDataTable(returnDataFrameResid);
            return AHatResiduals;
        }
        public DataTable GetResidualTableForUI()
        {
            RDotNet.DataFrame returnDataFrameResid = myREngine.Evaluate("newSRAnalysis$getResidualTable()").AsDataFrame();
            DataTable AHatResiduals = myREngineObject.rDataFrameToDataTable(returnDataFrameResid);
            return AHatResiduals;
        }
        public DataTable GetThresholdsTableForUI()
        {
            RDotNet.DataFrame returnDataFrameThresh = myREngine.Evaluate("newSRAnalysis$getThresholdDF()").AsDataFrame();
            DataTable AHatThresholds = myREngineObject.rDataFrameToDataTable(returnDataFrameThresh);
            return AHatThresholds;
        }
        public DataTable GetLogitFitTableForUI()
        {
            //ShowResults();
            RDotNet.DataFrame returnDataFrame = myREngine.Evaluate("newSRAnalysis$getResults()").AsDataFrame();
            DataTable AHatPODTable = myREngineObject.rDataFrameToDataTable(returnDataFrame);
            return AHatPODTable;
        }
        public double GetRSquaredValue()
        {
            double rSqaured= this.myREngine.Evaluate("newSRAnalysis$getRSquared()").AsNumeric()[0];
            return rSqaured;
        }
        public Dictionary<string, double> GetLinearModelStdErrors()
        {
            Dictionary<string, double> standardErrors = new Dictionary<string, double>();
            List<string> aValuesStrings = new List<string> { "slopeStdError", "interceptStdError","residualStdError" };
            RDotNet.NumericVector aValuesVector;
            //current list used returns a25, a50 (muhat), a90, sigmahat (SE of a90), and a9095 
            for (int i = 1; i <= 3; i++)
            {
                //this.myREngine.Evaluate("print(newSRAnalysis$getKeyAValues()[" + i + "])").AsList();
                aValuesVector = this.myREngine.Evaluate("newSRAnalysis$getRegressionStdErrs()[" + i + "]").AsNumeric();
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
                //this.myREngine.Evaluate("print(newSRAnalysis$getKeyAValues()[" + i + "])").AsList();
                aValuesVector = this.myREngine.Evaluate("newSRAnalysis$getKeyAValues()[" + i + "]").AsNumeric();
                aValuesDict.Add(key: aValuesStrings[i - 1], value: (double)aValuesVector[0]);
            }            
            return aValuesDict;
        }
        public List<double> GetLinearModelMetrics()
        {
            List<double> metrics = new List<double>();
            double modIntercept = this.myREngine.Evaluate("newSRAnalysis$getModelIntercept()").AsNumeric()[0];
            double modSlope = this.myREngine.Evaluate("newSRAnalysis$getModelSlope()").AsNumeric()[0];
            metrics.Add(modIntercept);
            metrics.Add(modSlope);
            return metrics;
        }
        public List<double> GetLinearTestMetrics()
        {
            //maybe make this a dictionary intead when implmemented?
            List<double> AllLinearTestResults = new List<double>();
            //shapiro-wilk normality test
            double shapiroStat = this.myREngine.Evaluate("newSRAnalysis$getLinearTestResults()[[1]]$statistic[[1]]").AsNumeric()[0];
            AllLinearTestResults.Add(shapiroStat);
            double shapiroPValue = this.myREngine.Evaluate("newSRAnalysis$getLinearTestResults()[[1]]$p.value").AsNumeric()[0];
            AllLinearTestResults.Add(shapiroPValue);
            //non-constance variance test
            double chiSqr = this.myREngine.Evaluate("newSRAnalysis$getLinearTestResults()[[2]][[3]]").AsNumeric()[0];
            AllLinearTestResults.Add(chiSqr);
            double degreesFreedom = this.myREngine.Evaluate("newSRAnalysis$getLinearTestResults()[[2]][[4]]").AsNumeric()[0];
            AllLinearTestResults.Add(degreesFreedom);
            double constVarPValue = this.myREngine.Evaluate("newSRAnalysis$getLinearTestResults()[[2]][[5]]").AsNumeric()[0];
            AllLinearTestResults.Add(constVarPValue);
            //durbin watson test
            double durbinRVal = this.myREngine.Evaluate("newSRAnalysis$getLinearTestResults()[[3]][[1]]").AsNumeric()[0];
            AllLinearTestResults.Add(durbinRVal);
            double durbinDW = this.myREngine.Evaluate("newSRAnalysis$getLinearTestResults()[[3]][[2]]").AsNumeric()[0];
            AllLinearTestResults.Add(durbinDW);
            double durbinPValue = this.myREngine.Evaluate("newSRAnalysis$getLinearTestResults()[[3]][[3]]").AsNumeric()[0];
            AllLinearTestResults.Add(durbinPValue);
            return AllLinearTestResults;
        }
        public void ShowResults()
        {
            myREngine.Evaluate("newSRAnalysis$plotSimdata(newSRAnalysis$getLinearModel())");
            myREngine.Evaluate("newSRAnalysis$plotPOD(newSRAnalysis$getResults(),newSRAnalysis$getLinearTestResults())");
        }
    }
}
