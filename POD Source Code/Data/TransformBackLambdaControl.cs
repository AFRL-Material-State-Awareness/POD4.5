// <copyright file="TransformBackLambdaControl.cs" company="The University of Dayton">
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

using CSharpBackendWithR;
using POD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class TransformBackLambdaControl : ITransformBackLambdaControl
    {
        private AHatAnalysisObject _aHatAnalysisObject;
        public TransformBackLambdaControl(AHatAnalysisObject ahatAnalysisObjectInput)
        {
            _aHatAnalysisObject = ahatAnalysisObjectInput;
        }
        public double TransformBackLambda(double myValue)
        {
            double transformValue;
            if (myValue >= -(1 / _aHatAnalysisObject.Lambda) && _aHatAnalysisObject.Lambda < 0)
            {
                return _aHatAnalysisObject.Signalmax * 10;
            }
            //convert lambda to an improper fraction to handle negtive values with the nth root
            long num, den;
            double whole = Math.Floor(_aHatAnalysisObject.Lambda);
            double decimalVal = _aHatAnalysisObject.Lambda - whole;
            IPy4C.DecimalToFraction(decimalVal, out num, out den);
            transformValue = IPy4C.NthRoot(myValue * _aHatAnalysisObject.Lambda + 1, _aHatAnalysisObject.Lambda, den);
            if (double.IsNaN(transformValue))
            {
                //add a very small '.01' number to the denominator to return a valid value to scale      
                double approxLambda = whole + Convert.ToDouble(num) / (Convert.ToDouble(den) + .000000000001);
                transformValue = IPy4C.NthRoot(myValue * approxLambda + 1, approxLambda, 1.0);
            }
            return transformValue;
        }
    }
    public interface ITransformBackLambdaControl
    {
        double TransformBackLambda(double myValue);
    }
}
