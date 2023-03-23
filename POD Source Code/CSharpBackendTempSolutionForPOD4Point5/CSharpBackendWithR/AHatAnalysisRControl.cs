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
        private GenerateRTransformDF_AHAT GenerateDataFrameInR;
        public AHatAnalysisRControl(REngineObject myREngineObject)
        {
            this.myREngineObject = myREngineObject;
            this.myREngine = myREngineObject.RDotNetEngine;
        }
        //convert to a dataframe by adding strings
        private void createDataFrameinGlobalEnvr(AHatAnalysisObject newAHatAnalysis, bool usingAll=false)
        {

            //create private variable names to shorten length
            List<double> cracks = newAHatAnalysis.Flaws;
            List<double> cracksCensored = newAHatAnalysis.FlawsCensored;
            List<double> signalResponse = newAHatAnalysis.Responses[newAHatAnalysis.SignalResponseName];
            List<int> indices = new List<int>();
            if (usingAll)
            {
                //cracks.Clear();
                //cracksCensored.Clear();
                //signalResponse.Clear();
                //create private variable names to shorten length
                cracks = newAHatAnalysis.Flaws_All;
                cracksCensored = newAHatAnalysis.FlawsCensored;
                signalResponse = newAHatAnalysis.Responses_all[newAHatAnalysis.SignalResponseName];
            }
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
                        if (signalResponse[i] == j)
                        {
                            this.myREngine.Evaluate("event<- c(event, 0)");
                            updatedEvent = true;
                            break;
                        }
                    }
                    foreach (double j in responsesLeftCensored)
                    {
                        if (signalResponse[i] == j)
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
            this.GenerateDataFrameInR = new GenerateRTransformDF_AHAT(this.myREngineObject, newAHatAnalysis, usingAll);
            this.GenerateDataFrameInR.GenerateTransformDataframe();
            try
            {
                BuildDataFrame(newAHatAnalysis);
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Testing failure");
            }
            finally
            {
                this.myREngine.Evaluate("rm(Index)");
                this.myREngine.Evaluate("rm(x)");
                this.myREngine.Evaluate("rm(event)");
                this.myREngine.Evaluate("rm(bc)");
            }

        }
        private void BuildDataFrame(AHatAnalysisObject newAHatAnalysis)
        {
            //add the index and flaws to the RDataframe
            this.myREngine.Evaluate("AHatDFTest<-data.frame(Index,x)");
            //append the number of reponses (most of the time it will just be 1
            for(int i=0; i< newAHatAnalysis.Responses.Count; i++)
            {
                this.myREngine.Evaluate("AHatDFTest <- cbind(AHatDFTest, y" + i.ToString() + ")");
                //destroy the reponse array after it's been added
                this.myREngine.Evaluate("rm(y"+i.ToString()+")");
            }
            //finally append the event variable
            this.myREngine.Evaluate("AHatDFTest <- cbind(AHatDFTest, event)");
        }
        public void ExecuteAnalysis(AHatAnalysisObject newTranformAnalysis)
        {
            this.createDataFrameinGlobalEnvr(newTranformAnalysis);
            this.myREngine.Evaluate("fullAnalysis<-TRUE");
            //execute class with appropriate parameters
            InitializeRClassForSignalResponse(newTranformAnalysis.Pod_threshold, newTranformAnalysis.ModelType);
            this.myREngine.Evaluate("newSRAnalysis$executeAhatvsA()");
            RecalculateGhostCurve(ref newTranformAnalysis, true);
            //remove misc variables from the global environment
            this.myREngine.Evaluate("rm(lambdaInput)");
            this.myREngine.Evaluate("rm(fullAnalysis)");
        }
        public void ExecuteTransforms(AHatAnalysisObject newTranformAnalysis)
        {
            this.createDataFrameinGlobalEnvr(newTranformAnalysis);
            this.myREngine.Evaluate("fullAnalysis<-FALSE");
            //execute class with appropriate parameters
            InitializeRClassForSignalResponse(newTranformAnalysis.Pod_threshold, newTranformAnalysis.ModelType);
            this.myREngine.Evaluate("newSRAnalysis$executeAhatvsA()");
            //remove misc variables from the global environment
            this.myREngine.Evaluate("rm(lambdaInput)");
            this.myREngine.Evaluate("rm(fullAnalysis)");
        }
        public void ExecutethresholdChange(AHatAnalysisObject newTranformAnalysis)
        {
            this.createDataFrameinGlobalEnvr(newTranformAnalysis);
            this.myREngine.Evaluate("fullAnalysis<-FALSE");
            //execute class with appropriate parameters
            InitializeRClassForSignalResponse(newTranformAnalysis.Pod_threshold, newTranformAnalysis.ModelType);
            this.myREngine.Evaluate("newSRAnalysis$prepareData()");
            this.myREngine.Evaluate("newSRAnalysis$performTransforms()");
            this.myREngine.Evaluate("ahatvACensored<-newSRAnalysis$genAhatVersusACensored()");
            this.myREngine.Evaluate("newSRAnalysis$genAvaluesAndMatrix(ahatvACensored)");
            this.myREngine.Evaluate("newSRAnalysis$genPODCurve()");
            RecalculateGhostCurve(ref newTranformAnalysis);
            //remove misc variables from the global environment
            this.myREngine.Evaluate("rm(lambdaInput)");
            this.myREngine.Evaluate("rm(ahatvACensored)");
            this.myREngine.Evaluate("rm(fullAnalysis)");
        }
        private void InitializeRClassForSignalResponse(double pod_threshold, int modelType)
        {
            
            this.myREngine.Evaluate("newSRAnalysis<-AHatAnalysis$new(signalRespDF=AHatDFTest, y_dec=" + pod_threshold + ", " +
                "modelType=" + modelType + ", lambda=lambdaInput)");
            this.myREngine.Evaluate("newSRAnalysis$generateDefaultValues()");
        }
        /// <summary>
        /// this function call is used when the user changes the threshold when the ghost curve is present(points omitted)
        /// the AHATDFTest variable is overwritten with no omitted points and calculated below.
        /// </summary>
        /// <param name="newTranformAnalysis"></param>
        private void RecalculateGhostCurve(ref AHatAnalysisObject newTranformAnalysis, bool recreateThreshTable=false)
        {
            this.createDataFrameinGlobalEnvr(newTranformAnalysis, true);
            this.myREngine.Evaluate("recalcPODClass<-RecalcOriginalPOD$new(signalRespDFFull=AHatDFTest,y_dec=" + newTranformAnalysis.Pod_threshold+", modelType="+
                newTranformAnalysis.ModelType+ ", lambda=lambdaInput, tau = newSRAnalysis$getTau())");
            
            //recalculates the POD curve with or without the threshold table
            //when the user changes the threshold, the threshold table is not regenerated
            if (recreateThreshTable)
                this.myREngine.Evaluate("recalcPODClass$recalcPOD(TRUE)");
            else
                this.myREngine.Evaluate("recalcPODClass$recalcPOD()");
            

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
        public DataTable GetThresholdsTable_ALL_ForUI()
        {
            RDotNet.DataFrame returnDataFrameThresh;
            if (Convert.ToBoolean(myREngine.Evaluate("exists(\"recalcPODClass\")").AsLogical()[0]))
                returnDataFrameThresh = myREngine.Evaluate("recalcPODClass$getGhostThresholdDF()").AsDataFrame();
            else
                returnDataFrameThresh = myREngine.Evaluate("newSRAnalysis$getThresholdDF()").AsDataFrame();
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
        public DataTable GetRecalcPODCurveAll()
        {
            //ShowResults();
            RDotNet.DataFrame returnDataFrame;
            if (Convert.ToBoolean(myREngine.Evaluate("exists(\"recalcPODClass\")").AsLogical()[0]))
                returnDataFrame = myREngine.Evaluate("recalcPODClass$getPODCurveAll()").AsDataFrame();
            else
                returnDataFrame = myREngine.Evaluate("newSRAnalysis$getResults()").AsDataFrame();
            DataTable AHatPODTable = myREngineObject.rDataFrameToDataTable(returnDataFrame);
            return AHatPODTable;
        }
        public DataTable GetLogitFitTableForUIAll()
        {
            //ShowResults();
            RDotNet.DataFrame returnDataFrame = myREngine.Evaluate("newSRAnalysis$getResultsAll()").AsDataFrame();
            DataTable AHatPODTable = myREngineObject.rDataFrameToDataTable(returnDataFrame);
            return AHatPODTable;
        }

        public DataTable GetNormalityTableForUI()
        {
            RDotNet.DataFrame returnDataFrame = myREngine.Evaluate("newSRAnalysis$getFreqTable()").AsDataFrame();
            DataTable AHatNormalityTable = myREngineObject.rDataFrameToDataTable(returnDataFrame);
            return AHatNormalityTable;
        }
        public DataTable GetNormalCurveTableForUI()
        {
            RDotNet.DataFrame returnDataFrame = myREngine.Evaluate("newSRAnalysis$getNormalCurveTable()").AsDataFrame();
            DataTable AHatnormalCurvetable = myREngineObject.rDataFrameToDataTable(returnDataFrame);
            return AHatnormalCurvetable;
        }
        public double GetRSquaredValue()
        {
            double rSqaured= this.myREngine.Evaluate("newSRAnalysis$getRSquared()").AsNumeric()[0];
            return rSqaured;
        }
        public double GetBoxCoxLamda()
        {
            double lambda = Convert.ToDouble(myREngine.Evaluate("newSRAnalysis$getLambda()").AsNumeric()[0]);
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
            try
            {
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
                //lack of fit test
                double lackOfFitDegFreedom = this.myREngine.Evaluate("newSRAnalysis$getLinearTestResults()[[4]][[2]]").AsNumeric()[0];
                AllLinearTestResults.Add(lackOfFitDegFreedom);
                double lackOfFitFCalc = this.myREngine.Evaluate("newSRAnalysis$getLinearTestResults()[[4]][[3]]").AsNumeric()[0];
                AllLinearTestResults.Add(lackOfFitFCalc);
                double lackOfFitPValue = this.myREngine.Evaluate("newSRAnalysis$getLinearTestResults()[[4]][[4]]").AsNumeric()[0];
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
            myREngine.Evaluate("newSRAnalysis$plotSimdata(newSRAnalysis$getLinearModel())");
            myREngine.Evaluate("newSRAnalysis$plotPOD(newSRAnalysis$getResults(),newSRAnalysis$getLinearTestResults())");
        }
    }
}
