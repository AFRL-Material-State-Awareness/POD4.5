using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDotNet;
namespace CSharpBackendWithR
{
    class GenerateRTransformDF_AHAT
    {
        private REngineObject myREngineObject;
        private REngine myREngine;
        private AHatAnalysisObject newAHatAnalysis;
        private List<double> cracks;
        private List<double> cracksCensored;
        private List<double> signalResponse;
        private List<int> indices;
        public GenerateRTransformDF_AHAT(REngineObject myREngineObjectInput, AHatAnalysisObject newAHatAnalysisInput)
        {
            this.myREngineObject = myREngineObjectInput;
            this.myREngine = this.myREngineObject.RDotNetEngine;
            this.newAHatAnalysis = newAHatAnalysisInput;
            this.cracks = newAHatAnalysis.Flaws;
            this.cracksCensored = newAHatAnalysis.FlawsCensored;
            this.signalResponse = newAHatAnalysis.Responses[newAHatAnalysis.SignalResponseName];
            this.indices = new List<int>();
        }
        public void GenerateTransformDataframe()
        {
            //generate the indices for the DF
            GenerateIndices();
            //set lambda as 0 by default - for boxcox tranformations only
            this.myREngine.Evaluate("lambdaInput<-0");
            //don't think this is needed
            this.myREngine.Evaluate("isLog=FALSE");
            //create the appropriate dataframe based on the transform type
            switch (newAHatAnalysis.ModelType)
            {
                case 1:
                    NoTransformDF();
                    break;
                case 2:
                    XAxisOnlyLogTransform();
                    break;
                case 3:
                    YAxisOnlyLogTransform();
                    break;
                case 4:
                    YAndXLogTransform();
                    break;
                case 5:
                    LinearXBoxcox();
                    break;
                case 6:
                    LogXBoxcox();
                    break;
                case 7:
                    InverseXBoxcox();
                    break;
                case 8:
                    LinearXInverseY();
                    break;
                case 9:
                    LogXInverseY();
                    break;
                case 10:
                    InverseXLinearY();
                    break;
                case 11:
                    InverseXLogY();
                    break;
                case 12:
                    InverseXInverseY();
                    break;
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
            //initialize the matrices used to create the input dataframe
            this.myREngine.Evaluate("Index<-matrix(" + this.indices[0].ToString() + ")");
            this.myREngine.Evaluate("x<-c(" + this.cracks[0].ToString() + ")");
            this.myREngine.Evaluate("y<-c(" + this.signalResponse[0].ToString() + ")");
            //acumulate r matrices in order to create the dataframe
            for (int i = 1; i < this.cracks.Count; i++)
            {
                this.myREngine.Evaluate("Index<-c(Index," + this.indices[i].ToString() + ")");
                this.myREngine.Evaluate("x<-c(x," + this.cracks[i].ToString() + ")");
                this.myREngine.Evaluate("y<-c(y," + this.signalResponse[i].ToString() + ")");
            }
        }
        private void XAxisOnlyLogTransform()
        {
            //initialize the matrices used to create the input dataframe
            this.myREngine.Evaluate("Index<-matrix(" + this.indices[0].ToString() + ")");
            this.myREngine.Evaluate("x<-c(log(" + this.cracks[0].ToString() + "))");
            this.myREngine.Evaluate("y<-c(" + this.signalResponse[0].ToString() + ")");
            //acumulate r matrices in order to create the dataframe
            for (int i = 1; i < this.cracks.Count; i++)
            {
                this.myREngine.Evaluate("Index<-c(Index," + this.indices[i].ToString() + ")");
                this.myREngine.Evaluate("x<-c(x,log(" + this.cracks[i].ToString() + "))");
                this.myREngine.Evaluate("y<-c(y," + this.signalResponse[i].ToString() + ")");
            }
        }
        private void YAxisOnlyLogTransform()
        {
            //initialize the matrices used to create the input dataframe
            this.myREngine.Evaluate("Index<-matrix(" + this.indices[0].ToString() + ")");
            this.myREngine.Evaluate("x<-c(" + this.cracks[0].ToString() + ")");
            this.myREngine.Evaluate("y<-c(log(" + this.signalResponse[0].ToString() + "))");
            //acumulate r matrices in order to create the dataframe
            for (int i = 1; i < this.cracks.Count; i++)
            {
                this.myREngine.Evaluate("Index<-c(Index," + this.indices[i].ToString() + ")");
                this.myREngine.Evaluate("x<-c(x," + this.cracks[i].ToString() + ")");
                this.myREngine.Evaluate("y<-c(y, log(" + this.signalResponse[i].ToString() + "))");
            }
        }
        private void YAndXLogTransform()
        {
            //initialize the matrices used to create the input dataframe
            this.myREngine.Evaluate("Index<-matrix(" + this.indices[0].ToString() + ")");
            this.myREngine.Evaluate("x<-c(log(" + this.cracks[0].ToString() + "))");
            this.myREngine.Evaluate("y<-c(log(" + this.signalResponse[0].ToString() + "))");
            for (int i = 1; i < this.cracks.Count; i++)
            {
                this.myREngine.Evaluate("Index<-c(Index," + this.indices[i].ToString() + ")");
                this.myREngine.Evaluate("x<-c(x,log(" + this.cracks[i].ToString() + "))");
                this.myREngine.Evaluate("y<-c(y, log(" + this.signalResponse[i].ToString() + "))");
            }
        }
        private void LinearXBoxcox()
        {
            //initialize the matrices used to create the input dataframe
            this.myREngine.Evaluate("Index<-matrix(" + this.indices[0].ToString() + ")");
            this.myREngine.Evaluate("x<-c(" + this.cracks[0].ToString() + ")");
            this.myREngine.Evaluate("y<-c(" + this.signalResponse[0].ToString() + ")");
            //acumulate r matrices in order to create the dataframe
            for (int i = 1; i < cracks.Count; i++)
            {
                this.myREngine.Evaluate("Index<-c(Index," + this.indices[i].ToString() + ")");
                this.myREngine.Evaluate("x<-c(x," + this.cracks[i].ToString() + ")");
                this.myREngine.Evaluate("y<-c(y, " + this.signalResponse[i].ToString() + ")");
            }
            //box-cox tranform has been selected. Find the optimal value for lambda!
            this.myREngine.Evaluate("bc<-boxcox(y~x, plotit=FALSE)");
            //get the value of lambda
            this.myREngine.Evaluate("lambdaInput <- bc$x[which.max(bc$y)]");
            //tranform y-axis with lambda
            this.myREngine.Evaluate("y<-(y^lambdaInput-1)/lambdaInput");
        }
        private void LogXBoxcox()
        {
            //initialize the matrices used to create the input dataframe
            this.myREngine.Evaluate("Index<-matrix(" + indices[0].ToString() + ")");
            this.myREngine.Evaluate("x<-c(log(" + cracks[0].ToString() + "))");
            this.myREngine.Evaluate("y<-c(" + signalResponse[0].ToString() + ")");
            //acumulate r matrices in order to create the dataframe
            for (int i = 1; i < cracks.Count; i++)
            {
                this.myREngine.Evaluate("Index<-c(Index," + indices[i].ToString() + ")");
                this.myREngine.Evaluate("x<-c(x, log(" + cracks[i].ToString() + "))");
                this.myREngine.Evaluate("y<-c(y, " + signalResponse[i].ToString() + ")");
            }
            //box-cox tranform has been selected. Find the optimal value for lambda!
            this.myREngine.Evaluate("bc<-boxcox(y~x, plotit=FALSE)");
            //get the value of lambda
            this.myREngine.Evaluate("lambdaInput <- bc$x[which.max(bc$y)]");
            //tranform y-axis with lambda
            this.myREngine.Evaluate("y<-(y^lambdaInput-1)/lambdaInput");
        }
        private void InverseXBoxcox()
        {
            //initialize the matrices used to create the input dataframe
            this.myREngine.Evaluate("Index<-matrix(" + indices[0].ToString() + ")");
            this.myREngine.Evaluate("x<-c(1/(" + cracks[0].ToString() + "))");
            this.myREngine.Evaluate("y<-c(" + signalResponse[0].ToString() + ")");
            //acumulate r matrices in order to create the dataframe
            for (int i = 1; i < cracks.Count; i++)
            {
                this.myREngine.Evaluate("Index<-c(Index," + indices[i].ToString() + ")");
                this.myREngine.Evaluate("x<-c(x, 1/(" + cracks[i].ToString() + "))");
                this.myREngine.Evaluate("y<-c(y, " + signalResponse[i].ToString() + ")");
            }
            //box-cox tranform has been selected. Find the optimal value for lambda!
            this.myREngine.Evaluate("bc<-boxcox(y~x, plotit=FALSE)");
            //get the value of lambda
            this.myREngine.Evaluate("lambdaInput <- bc$x[which.max(bc$y)]");
            //tranform y-axis with lambda
            this.myREngine.Evaluate("y<-(y^lambdaInput-1)/lambdaInput");
        }
        private void LinearXInverseY()
        {
            //initialize the matrices used to create the input dataframe
            this.myREngine.Evaluate("Index<-matrix(" + this.indices[0].ToString() + ")");
            this.myREngine.Evaluate("x<-c(" + this.cracks[0].ToString() + ")");
            this.myREngine.Evaluate("y<-c(1/(" + this.signalResponse[0].ToString() + "))");
            //acumulate r matrices in order to create the dataframe
            for (int i = 1; i < this.cracks.Count; i++)
            {
                this.myREngine.Evaluate("Index<-c(Index," + this.indices[i].ToString() + ")");
                this.myREngine.Evaluate("x<-c(x," + this.cracks[i].ToString() + ")");
                this.myREngine.Evaluate("y<-c(y,1/(" + this.signalResponse[i].ToString() + "))");
            }
        }
        private void LogXInverseY()
        {
            //initialize the matrices used to create the input dataframe
            this.myREngine.Evaluate("Index<-matrix(" + this.indices[0].ToString() + ")");
            this.myREngine.Evaluate("x<-c(log(" + this.cracks[0].ToString() + "))");
            this.myREngine.Evaluate("y<-c(1/(" + this.signalResponse[0].ToString() + "))");
            //acumulate r matrices in order to create the dataframe
            for (int i = 1; i < this.cracks.Count; i++)
            {
                this.myREngine.Evaluate("Index<-c(Index," + this.indices[i].ToString() + ")");
                this.myREngine.Evaluate("x<-c(x,log(" + this.cracks[i].ToString() + "))");
                this.myREngine.Evaluate("y<-c(y,1/(" + this.signalResponse[i].ToString() + "))");
            }
        }
        private void InverseXLinearY()
        {
            //initialize the matrices used to create the input dataframe
            this.myREngine.Evaluate("Index<-matrix(" + this.indices[0].ToString() + ")");
            this.myREngine.Evaluate("x<-c(1/(" + this.cracks[0].ToString() + "))");
            this.myREngine.Evaluate("y<-c(" + this.signalResponse[0].ToString() + ")");
            //acumulate r matrices in order to create the dataframe
            for (int i = 1; i < this.cracks.Count; i++)
            {
                this.myREngine.Evaluate("Index<-c(Index," + this.indices[i].ToString() + ")");
                this.myREngine.Evaluate("x<-c(x,1/(" + this.cracks[i].ToString() + "))");
                this.myREngine.Evaluate("y<-c(y," + this.signalResponse[i].ToString() + ")");
            }
        }
        private void InverseXLogY()
        {
            //initialize the matrices used to create the input dataframe
            this.myREngine.Evaluate("Index<-matrix(" + this.indices[0].ToString() + ")");
            this.myREngine.Evaluate("x<-c(1/(" + this.cracks[0].ToString() + "))");
            this.myREngine.Evaluate("y<-c(log(" + this.signalResponse[0].ToString() + "))");
            //acumulate r matrices in order to create the dataframe
            for (int i = 1; i < this.cracks.Count; i++)
            {
                this.myREngine.Evaluate("Index<-c(Index," + this.indices[i].ToString() + ")");
                this.myREngine.Evaluate("x<-c(x,1/(" + this.cracks[i].ToString() + "))");
                this.myREngine.Evaluate("y<-c(y,log(" + this.signalResponse[i].ToString() + "))");
            }
        }
        private void InverseXInverseY()
        {
            //initialize the matrices used to create the input dataframe
            this.myREngine.Evaluate("Index<-matrix(" + this.indices[0].ToString() + ")");
            this.myREngine.Evaluate("x<-c(1/(" + this.cracks[0].ToString() + "))");
            this.myREngine.Evaluate("y<-c(1/(" + this.signalResponse[0].ToString() + "))");
            //acumulate r matrices in order to create the dataframe
            for (int i = 1; i < this.cracks.Count; i++)
            {
                this.myREngine.Evaluate("Index<-c(Index," + this.indices[i].ToString() + ")");
                this.myREngine.Evaluate("x<-c(x,1/(" + this.cracks[i].ToString() + "))");
                this.myREngine.Evaluate("y<-c(y,1/(" + this.signalResponse[i].ToString() + "))");
            }
        }
    }
    
}
