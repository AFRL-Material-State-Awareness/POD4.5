﻿// <copyright file="GenerateRTransformDF_AHAT.cs" company="The University of Dayton">
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDotNet;
using POD;

namespace CSharpBackendWithR
{
    class GenerateRTransformDF_AHAT
    {
        private IREngineObject myREngineObject;
        private REngine myREngine;
        private AHatAnalysisObject newAHatAnalysis;
        private double lambda;

        private bool usingAllPoints;
        private readonly IDataTransformer _dataTransformer;

        public bool UsingAllPoints => newAHatAnalysis.ExcludedFlaws.Count == 0;

        public GenerateRTransformDF_AHAT(IREngineObject myREngineObjectInput, AHatAnalysisObject newAHatAnalysisInput, bool usingAllPoints, IDataTransformer dataTransformer)
        {
            this.myREngineObject = myREngineObjectInput;
            this.myREngine = this.myREngineObject.RDotNetEngine;
            this.newAHatAnalysis = newAHatAnalysisInput;
            this.lambda = newAHatAnalysisInput.Lambda;

            this.usingAllPoints = usingAllPoints;
            this._dataTransformer = dataTransformer;
        }

        public void GenerateTransformDataframe()
        {
            // 1. Get the correct data source (all points for ghost curve, or active points for analysis)
            List<double> flawData = this.usingAllPoints ? this.newAHatAnalysis.Flaws_All : this.newAHatAnalysis.Flaws;
            Dictionary<string, List<double>> responseDataSource = this.usingAllPoints ? this.newAHatAnalysis.Responses_all : this.newAHatAnalysis.Responses;

            // 2. Transform flaw data in C# and transfer to R
            List<double> transformedFlaws = flawData.Select(f => _dataTransformer.TransformXDataValue(f)).ToList();
            NumericVector xVector = this.myREngine.SafeCreateNumericVector(transformedFlaws);
            this.myREngine.SafeSetSymbol("x", xVector);

            // Also create and set the 'Index' vector, which is just a sequence from 1 to N.
            List<int> indices = Enumerable.Range(1, flawData.Count).ToList();
            IntegerVector indexVector = this.myREngine.SafeCreateIntegerVector(indices);
            this.myREngine.SafeSetSymbol("Index", indexVector);

            // 3. Transform each response column in C# and transfer to R
            int responseIndex = 0;
            foreach (var responseColumn in responseDataSource)
            {
                string rVectorName = $"y{responseIndex}";
                List<double> responseData = responseColumn.Value;

                List<double> transformedResponses = responseData.Select(r => _dataTransformer.TransformYDataValue(r)).ToList();

                NumericVector yVector = this.myREngine.SafeCreateNumericVector(transformedResponses);
                this.myREngine.SafeSetSymbol(rVectorName, yVector);

                responseIndex++;
            }

            // 4. Set the lambda value in R for potential use in Box-Cox related calculations.
            // Although the main transform is done in C#, R scripts might still need this value.
            var lambdaValue = this.lambda;
            if (Math.Abs(lambdaValue - .500) < .001)
            {
                lambdaValue = .501;
            }
            this.myREngine.SafeEvaluate($"lambdaInput <- {lambdaValue}");
        }
    }
    
}
