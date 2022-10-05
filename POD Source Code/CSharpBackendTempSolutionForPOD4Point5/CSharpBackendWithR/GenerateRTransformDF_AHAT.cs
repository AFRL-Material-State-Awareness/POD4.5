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
        private double lambda;
        private List<double> cracks;
        private List<double> cracksCensored;
        private List<double> signalResponse;
        private List<int> indices;
        public GenerateRTransformDF_AHAT(REngineObject myREngineObjectInput, AHatAnalysisObject newAHatAnalysisInput)
        {
            this.myREngineObject = myREngineObjectInput;
            this.myREngine = this.myREngineObject.RDotNetEngine;
            this.newAHatAnalysis = newAHatAnalysisInput;
            this.lambda = newAHatAnalysisInput.Lambda;
            this.cracks = newAHatAnalysis.Flaws;
            this.cracksCensored = newAHatAnalysis.FlawsCensored;
            this.signalResponse = newAHatAnalysis.Responses[newAHatAnalysis.SignalResponseName];
            this.indices = new List<int>();
        }
        public void GenerateTransformDataframe()
        {
            //generate the indices for the DF
            GenerateIndices();
            createIndicesArray();
            //set lambda as 0 by default - for boxcox tranformations only
            this.myREngine.Evaluate("lambdaInput<-0");
            //
            //this.myREngine.Evaluate("isLog=FALSE");
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
        private void createIndicesArray()
        {
            //create an indices column for the r dataframe regardless of transform type
            this.myREngine.Evaluate("Index<-matrix(" + this.indices[0].ToString() + ")");
            for (int i = 1; i < this.cracks.Count; i++)
            {
                this.myREngine.Evaluate("Index<-c(Index," + this.indices[i].ToString() + ")");
            }
        }
        private void LinearSignalGeneration()
        {
            Dictionary<string, List<double>> responses = this.newAHatAnalysis.Responses;
            List<string> genYHatNames = new List<string>();
            for (int i = 0; i < responses.Count; i++)
            {
                genYHatNames.Add("y" + i.ToString());
            }
            int currResponse = 0;
            //iterate through the dictionary of responses in order to create the dataframe in r
            foreach (KeyValuePair<string, List<double>> entry in responses)
            {
                for (int i = 0; i < entry.Value.Count; i++)
                {
                    if (i == 0)
                    {
                        //initialize the matrices used to create the input dataframe
                        this.myREngine.Evaluate(genYHatNames[currResponse].ToString() + "<-c(" + entry.Value[i].ToString() + ")");
                        continue;
                    }
                    this.myREngine.Evaluate(genYHatNames[currResponse].ToString() + "<-c(" + genYHatNames[currResponse].ToString() + "," + entry.Value[i].ToString() + ")");
                }
                currResponse += 1;
            }
        }
        private void LogSignalGeneration()
        {
            Dictionary<string, List<double>> responses = this.newAHatAnalysis.Responses;
            List<string> genYHatNames = new List<string>();
            for (int i = 0; i < responses.Count; i++)
            {
                genYHatNames.Add("y" + i.ToString());
            }
            int currResponse = 0;
            //iterate through the dictionary of responses in order to create the dataframe in r
            foreach (KeyValuePair<string, List<double>> entry in responses)
            {
                for (int i = 0; i < entry.Value.Count; i++)
                {
                    if (i == 0)
                    {
                        //initialize the matrices used to create the input dataframe
                        this.myREngine.Evaluate(genYHatNames[currResponse].ToString() + "<-c(log(" + entry.Value[i].ToString() + "))");
                        continue;
                    }
                    this.myREngine.Evaluate(genYHatNames[currResponse].ToString() + "<-c(" + genYHatNames[currResponse].ToString() + ", log(" + entry.Value[i].ToString() + "))");
                }
                currResponse += 1;
            }
        }
        private void InverseSignalGeneration()
        {
            Dictionary<string, List<double>> responses = this.newAHatAnalysis.Responses;
            List<string> genYHatNames = new List<string>();
            for (int i = 0; i < responses.Count; i++)
            {
                genYHatNames.Add("y" + i.ToString());
            }
            int currResponse = 0;
            //iterate through the dictionary of responses in order to create the dataframe in r
            foreach (KeyValuePair<string, List<double>> entry in responses)
            {
                for (int i = 0; i < entry.Value.Count; i++)
                {
                    if (i == 0)
                    {
                        //initialize the matrices used to create the input dataframe
                        this.myREngine.Evaluate(genYHatNames[currResponse].ToString() + "<-c(1.0/" + entry.Value[i].ToString() + ")");
                        continue;
                    }
                    this.myREngine.Evaluate(genYHatNames[currResponse].ToString() + "<-c(" + genYHatNames[currResponse].ToString() + "," + entry.Value[i].ToString() + ")");
                }
                currResponse += 1;
            }
        }
        private void BoxCoxSignalGeneration()
        {
            Dictionary<string, List<double>> responses = this.newAHatAnalysis.Responses;
            List<string> genYHatNames = new List<string>();
            for (int i = 0; i < responses.Count; i++)
            {
                genYHatNames.Add("y" + i.ToString());
            }
            int currResponse = 0;
            //iterate through the dictionary of responses in order to create the dataframe in r
            foreach (KeyValuePair<string, List<double>> entry in responses)
            {
                for (int i = 0; i < entry.Value.Count; i++)
                {
                    if (i == 0)
                    {
                        //initialize the matrices used to create the input dataframe
                        this.myREngine.Evaluate(genYHatNames[currResponse].ToString() + "<-c(" + entry.Value[i].ToString() + ")");
                        continue;
                    }
                    this.myREngine.Evaluate(genYHatNames[currResponse].ToString() + "<-c(" + genYHatNames[currResponse].ToString() + "," + entry.Value[i].ToString() + ")");
                }
                currResponse += 1;
            }
            SetLambdaValueAndTransformInR();
        }
        private void LinearFlawGeneration()
        {
            //initialize the matrices used to create the input dataframe
            this.myREngine.Evaluate("x<-c(" + this.cracks[0].ToString() + ")");
            for (int i = 1; i < this.cracks.Count; i++)
            {
                this.myREngine.Evaluate("x<-c(x," + this.cracks[i].ToString() + ")");
            }
        }
        private void LogFlawGeneration()
        {
            //initialize the matrices used to create the input dataframe
            this.myREngine.Evaluate("x<-c(log(" + this.cracks[0].ToString() + "))");
            //acumulate r matrices in order to create the dataframe
            for (int i = 1; i < this.cracks.Count; i++)
            {
                this.myREngine.Evaluate("x<-c(x,log(" + this.cracks[i].ToString() + "))");
            }
        }
        private void InverseFlawGeneration()
        {
            //initialize the matrices used to create the input dataframe
            this.myREngine.Evaluate("x<-c(1/(" + cracks[0].ToString() + "))");
            //acumulate r matrices in order to create the dataframe
            for (int i = 1; i < cracks.Count; i++)
            {
                this.myREngine.Evaluate("x<-c(x, 1/(" + cracks[i].ToString() + "))");
            }
        }
        private void NoTransformDF()
        {
            LinearSignalGeneration();
            LinearFlawGeneration();
        }
        private void XAxisOnlyLogTransform()
        {
            LinearSignalGeneration();
            LogFlawGeneration();
        }
        private void YAxisOnlyLogTransform()
        {
            LogSignalGeneration();
            LinearFlawGeneration();
        }
        private void YAndXLogTransform()
        {
            LogSignalGeneration();
            LogFlawGeneration();
        }
        private void LinearXBoxcox()
        {
            BoxCoxSignalGeneration();
            LinearFlawGeneration();
        }
        private void LogXBoxcox()
        {
            BoxCoxSignalGeneration();
            LogFlawGeneration();
        }
        private void InverseXBoxcox()
        {
            BoxCoxSignalGeneration();
            InverseFlawGeneration();
        }
        private void LinearXInverseY()
        {
            InverseSignalGeneration();
            LinearFlawGeneration();
        }
        private void LogXInverseY()
        {
            InverseSignalGeneration();
            LogFlawGeneration();
        }
        private void InverseXLinearY()
        {
            LinearSignalGeneration();
            InverseFlawGeneration();
        }
        private void InverseXLogY()
        {
            LogSignalGeneration();
            InverseFlawGeneration();
        }
        private void InverseXInverseY()
        {
            InverseSignalGeneration();
            InverseFlawGeneration();
        }
        private void SetLambdaValueAndTransformInR()
        {
            //get the value of lambda
            this.myREngine.Evaluate("lambdaInput <-" + this.lambda.ToString());
            //tranform y-axis with lambda
            for(int i=0; i < this.newAHatAnalysis.Responses.Count; i++)
            {
                this.myREngine.Evaluate("y"+i.ToString()+"<-(y"+ i.ToString() + "^lambdaInput-1)/lambdaInput");
            }
        }
    }
    
}
