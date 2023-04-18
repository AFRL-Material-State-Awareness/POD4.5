using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDotNet;
namespace CSharpBackendWithR
{
    class GenerateRTransformDF_HitMiss
    {
        private IREngineObject myREngineObject;
        private REngine myREngine;
        private HMAnalysisObject newHMAnalysis;
        private List<double> cracks;
        private List<double> hitMiss;
        private List<int> indices;
        public GenerateRTransformDF_HitMiss(IREngineObject myREngineObjectInput, HMAnalysisObject newHMnalysisInput)
        {
            this.myREngineObject = myREngineObjectInput;
            this.myREngine = this.myREngineObject.RDotNetEngine;
            this.newHMAnalysis = newHMnalysisInput;
            this.cracks = newHMAnalysis.Flaws;
            this.hitMiss = newHMAnalysis.Responses[newHMnalysisInput.HitMiss_name];
            this.indices = new List<int>();
        }
        public void GenerateTransformDataframe()
        {
            //generate the indices for the DF
            GenerateIndices();
            //set lambda as 0 by default - for boxcox tranformations only
            this.myREngine.Evaluate("lambdaInput<-0");
            //needed for the r code 
            this.myREngine.Evaluate("normSampleSize<-" + newHMAnalysis.A_x_n.ToString());
            //create the appropriate dataframe based on the transform type
            switch (newHMAnalysis.ModelType)
            {
                case 1:
                    NoTransformDF();
                    break;
                case 2:
                    LogXTransform();
                    break;
                case 3:
                    InverseXTransform();
                    break;
                default:
                    throw new Exception("model type not found exception! (currently only supports linear and log)");
            }
        }
        private void GenerateIndices()
        {
            //create index column for dataframe
            for (int i = 1; i <= this.cracks.Count; i++)
            {
                this.indices.Add(i);
            }
        }
        private void NoTransformDF()
        {
            //used when generate normal crack distributions
            this.myREngine.Evaluate("isLog=FALSE");
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
        }
        private void LogXTransform()
        {
            //used when generate normal crack distributions
            this.myREngine.Evaluate("isLog=TRUE");
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
        }
        private void InverseXTransform()
        {
            //used when generate normal crack distributions
            this.myREngine.Evaluate("isLog=FALSE");
            //initialize the matrices used to create the input dataframe
            this.myREngine.Evaluate("Index<-matrix(" + this.indices[0].ToString() + ")");
            this.myREngine.Evaluate("x<-c(1.0/" + this.cracks[0].ToString() + ")");
            this.myREngine.Evaluate("y<-c(" + this.hitMiss[0].ToString() + ")");
            //acumulate r matrices in order to create the dataframe
            for (int i = 1; i < this.cracks.Count; i++)
            {
                this.myREngine.Evaluate("Index<-c(Index," + this.indices[i].ToString() + ")");
                this.myREngine.Evaluate("x<-c(x, 1.0/" + this.cracks[i].ToString() + ")");
                this.myREngine.Evaluate("y<-c(y, " + this.hitMiss[i].ToString() + ")");
            }
        }
    }
    
}
