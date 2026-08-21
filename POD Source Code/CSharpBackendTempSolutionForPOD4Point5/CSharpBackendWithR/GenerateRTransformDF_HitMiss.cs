﻿// <copyright file="GenerateRTransformDF_HitMiss.cs" company="The University of Dayton">
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
using POD;
using RDotNet;
namespace CSharpBackendWithR
{
    class GenerateRTransformDF_HitMiss
    {
        private IREngineObject myREngineObject;
        private REngine myREngine;
        private HMAnalysisObject newHMAnalysis;
        private List<double> cracks;
        private readonly IDataTransformer _dataTransformer;

        public GenerateRTransformDF_HitMiss(IREngineObject myREngineObjectInput, HMAnalysisObject newHMnalysisInput, IDataTransformer dataTransformer)
        {
            this.myREngineObject = myREngineObjectInput;
            this.myREngine = this.myREngineObject.RDotNetEngine;
            this.newHMAnalysis = newHMnalysisInput;
            this.cracks = newHMAnalysis.Flaws;

            // The transformer is needed to correctly handle data transformations. It is initialized here
            // using the ModelType from the HMAnalysisObject, which corresponds to the flaw transform.
            // The Y-axis (response) in Hit/Miss is always linear (0 or 1) and doesn't get transformed.
            //_dataTransformer = new DataTransformer(null, TransformTypeEnum.Linear, 0, 1, null, (TransformTypeEnum)newHMAnalysis.ModelType, 0, 1);
            _dataTransformer = dataTransformer;
        }

        public void GenerateTransformDataframe()
        {
            // 1. Get flaw and response data from the analysis object.
            List<double> flawData = this.newHMAnalysis.Flaws;
            List<double> responseData = this.newHMAnalysis.Responses[this.newHMAnalysis.HitMiss_name];

            // 2. Create and transfer the 'Index' vector.
            List<int> indices = Enumerable.Range(1, flawData.Count).ToList();
            IntegerVector indexVector = this.myREngine.SafeCreateIntegerVector(indices);
            this.myREngine.SafeSetSymbol("Index", indexVector);

            // 3. Create and transfer the 'y' (response) vector.
            NumericVector yVector = this.myREngine.SafeCreateNumericVector(responseData);
            this.myREngine.SafeSetSymbol("y", yVector);

            // 4. Transform flaw data in C# based on the model type.
            List<double> transformedFlaws;
            switch ((RModelTypeEnum)newHMAnalysis.ModelType)
            {
                case RModelTypeEnum.Linear:
                    this.myREngine.SafeEvaluate("isLog=FALSE");
                    break;

                case RModelTypeEnum.Log:
                    this.myREngine.SafeEvaluate("isLog=TRUE");                    
                    break;

                case RModelTypeEnum.Inverse:
                    this.myREngine.SafeEvaluate("isLog=FALSE");
                    break;

                default:
                    throw new NotSupportedException($"Model type '{newHMAnalysis.ModelType}' is not supported for Hit/Miss analysis.");
            }

            transformedFlaws = flawData.Select(f => _dataTransformer.TransformXDataValue(f)).ToList();

            // 5. Create and transfer the transformed 'x' (flaw) vector.
            NumericVector xVector = this.myREngine.SafeCreateNumericVector(transformedFlaws);
            this.myREngine.SafeSetSymbol("x", xVector);

            // 6. Set other necessary R variables.
            // Set lambda as 0 by default - for boxcox transformations only (not used in H/M but good for consistency)
            this.myREngine.SafeEvaluate("lambdaInput <- 0");
            // Needed for the R code
            this.myREngine.SafeEvaluate($"normSampleSize <- {newHMAnalysis.A_x_n}");
        }
    }
}
