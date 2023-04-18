using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDotNet;
namespace CSharpBackendWithR
{
    /// <summary>
    /// auto-calculates a default lambda if the user selected a box-cox transformation in the choose transform panel or the full regression panel
    /// </summary>
    public class TemporaryLambdaCalc : ITemporaryLambdaCalc
    {
        private List<double> cracks;
        private List<double> signalResponse;
        private REngineObject myREngineObject;
        private REngine myREngine;
        public TemporaryLambdaCalc(List<double> cracksInput, List<double> signalResponseInput, REngineObject myREngineInput)
        {
            this.cracks = cracksInput;
            this.signalResponse = signalResponseInput;
            this.myREngineObject = myREngineInput;
            this.myREngine = myREngineObject.RDotNetEngine;
        }
        public double CalcTempLambda()
        {
            double tempLambda = 0.0;

            //initialize the matrices used to create the input dataframe
            this.myREngine.Evaluate("x<-c(" + cracks[0].ToString() + ")");
            this.myREngine.Evaluate("y<-c(" + signalResponse[0].ToString() + ")");
            //acumulate r matrices in order to create the dataframe
            for (int i = 1; i < cracks.Count; i++)
            {
                this.myREngine.Evaluate("x<-c(x," + cracks[i].ToString() + ")");
                this.myREngine.Evaluate("y<-c(y, " + signalResponse[i].ToString() + ")");
            }
            //box-cox tranform has been selected. Find the optimal value for lambda!
            this.myREngine.Evaluate("bc<-boxcox(y~x, plotit=FALSE)");
            //get the value of lambda
            tempLambda=Convert.ToDouble(this.myREngine.Evaluate("bc$x[which.max(bc$y)]").AsNumeric()[0]);
            //remove parameters to avoid interference
            this.myREngine.Evaluate("rm(x)");
            this.myREngine.Evaluate("rm(y)");
            this.myREngine.Evaluate("rm(bc)");
            if (No_0_Lambda(tempLambda))
                return tempLambda;
            else
                return tempLambda += .1;
        }
        private bool No_0_Lambda (double lambda)
        {
            if (lambda != 0.0) return true;
            else return false;
        }
    }

    public interface ITemporaryLambdaCalc
    {
        double CalcTempLambda();
    }
}
