// <copyright file="TemporaryLambdaCalc.cs" company="The University of Dayton">
// Copyright © 2025 The University of Dayton. All rights reserved.
//
// Licensed under the GNU General Public License, Version 3.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.gnu.org/licenses/gpl-3.0.txt
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDotNet;
using POD;

namespace CSharpBackendWithR
{
    /// <summary>
    /// auto-calculates a default lambda if the user selected a box-cox transformation in the choose transform panel or the full regression panel
    /// </summary>
    public class TemporaryLambdaCalc : ITemporaryLambdaCalc
    {
        private List<double> cracks;
        private List<double> signalResponse;
        private IREngineObject myREngineObject;
        private REngine myREngine;
        public TemporaryLambdaCalc(List<double> cracksInput, List<double> signalResponseInput, IREngineObject myREngineInput)
        {
            this.cracks = cracksInput;
            this.signalResponse = signalResponseInput;
            this.myREngineObject = myREngineInput;
            this.myREngine = myREngineObject.RDotNetEngine;
        }
        public double CalcTempLambda()
        {
            double tempLambda = 0.0;
            double minCracks = POD.Compute.FixMin(cracks.Min());
            double minSignals = POD.Compute.FixMin(signalResponse.Min());

            // Create numeric vectors in R, which is safer than string concatenation.
            NumericVector xVec = myREngine.SafeCreateNumericVector(cracks.Select(c => c + minCracks));
            myREngine.SafeSetSymbol("x", xVec);

            NumericVector yVec = myREngine.SafeCreateNumericVector(signalResponse.Select(s => s + minSignals));
            myREngine.SafeSetSymbol("y", yVec);

            //box-cox tranform has been selected. Find the optimal value for lambda!
            this.myREngine.SafeEvaluate("bc<-boxcox(y~x, plotit=FALSE)");
            //get the value of lambda
            tempLambda=Convert.ToDouble(this.myREngine.SafeEvaluate("bc$x[which.max(bc$y)]").AsNumeric()[0]);
            //remove parameters to avoid interference
            this.myREngine.SafeEvaluate("rm(x)");
            this.myREngine.SafeEvaluate("rm(y)");
            this.myREngine.SafeEvaluate("rm(bc)");

            // A lambda of 0 corresponds to a log transform. If the optimal lambda is 0,
            // we use a very close value instead to avoid issues with the Box-Cox formula.
            return tempLambda == 0.0 ? 0.1 : tempLambda;
        }
    }

    public interface ITemporaryLambdaCalc
    {
        double CalcTempLambda();
    }
}
