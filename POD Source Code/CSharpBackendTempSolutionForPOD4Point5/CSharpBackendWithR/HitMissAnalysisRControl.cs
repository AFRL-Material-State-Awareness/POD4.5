using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using RDotNet;
namespace CSharpBackendWithR
{
    class HitMissAnalysisRControl
    {
        private REngineObject myREngineObject;
        private REngine myREngine;
        public HitMissAnalysisRControl(REngineObject myREngineObject)
        {
            this.myREngineObject = myREngineObject;
            this.myREngine = myREngineObject.RDotNetEngine;
        }
        //convert to a dataframe by adding strings
        private void createDataFrameinGlobalEnvr(HMAnalysisObject newTranformAnalysis)
        {
            //create private variable names to shorten length
            List<double> cracks = newTranformAnalysis.Flaws;
            //TODO: FIX MEEE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            newTranformAnalysis.HitMiss_name = "y";
            List<double> hitMiss = newTranformAnalysis.Responses[newTranformAnalysis.HitMiss_name];
            int a_x_n = newTranformAnalysis.A_x_n;
            List<int> indices = new List<int>();
            //needed for the r code 
            this.myREngine.Evaluate("normSampleSize<-" + a_x_n.ToString());
            if (newTranformAnalysis.ModelType == 1)
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
                this.myREngine.Evaluate("y<-c(" + hitMiss[0].ToString() + ")");
                //acumulate r matrices in order to create the dataframe
                for (int i = 1; i < cracks.Count; i++)
                {
                    this.myREngine.Evaluate("Index<-c(Index," + indices[i].ToString() + ")");
                    this.myREngine.Evaluate("x<-c(x," + cracks[i].ToString() + ")");
                    this.myREngine.Evaluate("y<-c(y," + hitMiss[i].ToString() + ")");
                };
                //build the dataframe in the global environment
                //this dataframe will remain in the global environment
                this.myREngine.Evaluate("hitMissDF<-data.frame(Index,x,y)");
                this.myREngine.Evaluate("rm(Index)");
                this.myREngine.Evaluate("rm(x)");
                this.myREngine.Evaluate("rm(y)");
            }
            else if (newTranformAnalysis.ModelType == 2)
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
                this.myREngine.Evaluate("y<-c(" + hitMiss[0].ToString() + ")");
                //acumulate r matrices in order to create the dataframe
                for (int i = 1; i < cracks.Count; i++)
                {
                    this.myREngine.Evaluate("Index<-c(Index," + indices[i].ToString() + ")");
                    this.myREngine.Evaluate("x<-c(x,log(" + cracks[i].ToString() + "))");
                    this.myREngine.Evaluate("y<-c(y," + hitMiss[i].ToString() + ")");
                };
                //build the dataframe in the global environment
                //this dataframe will remain in the global environment
                this.myREngine.Evaluate("hitMissDF<-data.frame(Index,x,y)");
                this.myREngine.Evaluate("rm(Index)");
                this.myREngine.Evaluate("rm(x)");
                this.myREngine.Evaluate("rm(y)");
            }
            else
            {
                throw new Exception("model type not found exception! (currently only supports linear and log)");
            }
            
            
        }
        public void ExecuteAnalysis(HMAnalysisObject newTranformAnalysis)
        {
            
            this.createDataFrameinGlobalEnvr(newTranformAnalysis);
            //System.Diagnostics.Debug.WriteLine(this.myREngine.Evaluate("str('str(as.list(.GlobalEnv)')").ToString());
            //this.myREngine.Evaluate("str('str(as.list(.GlobalEnv)')");
            //execute class with appropriate parameters
            this.myREngine.Evaluate("newAnalysis<-HMAnalysis$new(hitMissDF=hitMissDF, modelType='"+newTranformAnalysis.RegressionType+"',"
                +"CIType='"+newTranformAnalysis.CIType+
                "', N=nrow(hitMissDF), normSampleAmount=normSampleSize)");
            this.myREngine.Evaluate("newAnalysis$detAnalysisApproach()");
        }

        public void ExecuteRSS(HMAnalysisObject newTranformAnalysis)
        {
            //create and load the dataframe into the r.NET global environment
            this.createDataFrameinGlobalEnvr(newTranformAnalysis);
            //create and initialize the RSSComponent object(see S4 class in the R code)
            this.myREngine.Evaluate("newRSSComponent<-RSSComponents$new()");
            this.myREngine.Evaluate("newRSSComponent$initialize(maxResamples="+newTranformAnalysis.MaxResamples+", set_mInput="+ newTranformAnalysis.Set_m 
                + ", set_rInput="+ newTranformAnalysis.Set_r + ", regressionType='"+newTranformAnalysis.RegressionType + "', excludeNAInput=TRUE)");
            //execute the main HM class with appropriate parameters(note that the RSS object is added in this case)
            this.myREngine.Evaluate("newAnalysis<-HMAnalysis$new(hitMissDF=hitMissDF, modelType='" + newTranformAnalysis.RegressionType + "',"
                + "CIType='" + newTranformAnalysis.CIType +
                "', N=nrow(hitMissDF), normSampleAmount=normSampleSize, rankedSetSampleObject= newRSSComponent)");
            this.myREngine.Evaluate("newAnalysis$initializeRSS()");
        }
        public DataTable GetLogitFitTableForUI()
        {
            //ShowResults();
            RDotNet.DataFrame returnDataFrame = myREngine.Evaluate("newAnalysis$getResults()").AsDataFrame();
            DataTable LogitFitDataTable= myREngineObject.rDataFrameToDataTable(returnDataFrame);
            return LogitFitDataTable;
        }
        public List<double> GetCovarianceMatrixValues()
        {
            RDotNet.DataFrame returnCovMatrix = myREngine.Evaluate("newAnalysis$getCovMatrix()").AsDataFrame();
            int matrixColumns = Convert.ToInt32(myREngine.Evaluate("ncol(newAnalysis$getCovMatrix())").AsNumeric()[0]);
            int matrixRows = Convert.ToInt32(myREngine.Evaluate("nrow(newAnalysis$getCovMatrix())").AsNumeric()[0]);
            List<double> CovarianceMatrix = new List<double>();
            //covariance matrix is realtively small, so double for loop should be okay
            for (int i=0; i< matrixRows; i++)
            {
                for(int j=0; j<matrixColumns; j++)
                {
                    CovarianceMatrix.Add(Convert.ToDouble(returnCovMatrix[i][j]));
                }
            }
            return CovarianceMatrix;
        }
        public double GetGoodnessOfFit()
        {
            double goodFit=Convert.ToDouble(myREngine.Evaluate("newAnalysis$getGoodnessOfFit()").AsNumeric()[0]);
            return goodFit;
        }
        public DataTable GetOrigHitMissDF()
        {
            myREngine.Evaluate("hitMissDF$index=NULL");
            myREngine.Evaluate("names(hitMissDF)[names(hitMissDF) == 'x'] = 'flaw'");
            myREngine.Evaluate("names(hitMissDF)[names(hitMissDF) == 'y'] = 'hitrate'");
            RDotNet.DataFrame origDataFrame = myREngine.Evaluate("hitMissDF").AsDataFrame();
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
                //this.myREngine.Evaluate("print(newAnalysis$getKeyAValues()[" + i + "])").AsList();
                aValuesVector = this.myREngine.Evaluate("newAnalysis$getKeyAValues()["+i+"]").AsNumeric();
                aValuesDict.Add(key: aValuesStrings[i-1], value: (double)aValuesVector[0]);
            }
            //Console.WriteLine(aValuesDict);
            return aValuesDict;
        }
        public void ShowResults()
        {
            //plots the dataset model fit, the POD curve, and the CI curve
            //this is for debugging only, delete in final program
            //this.myREngine.Evaluate("str('str(as.list(.GlobalEnv)')");
            //this.myREngine.Evaluate("newAnalysis$plotSimdata(newAnalysis$getResults())");
            //this.myREngine.Evaluate("newAnalysis$plotCI(newAnalysis$getResults())");
            //this.myREngine.Evaluate("print(newAnalysis$getResults())");
        }
    }
}
