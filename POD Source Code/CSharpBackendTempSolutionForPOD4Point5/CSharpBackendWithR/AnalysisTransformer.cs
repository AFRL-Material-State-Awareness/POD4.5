// <copyright file="DisplayTransformer.cs" company="The University of Dayton">
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
using System.Diagnostics;
using POD;

namespace CSharpBackendWithR
{
    /// <summary>
    /// Defines the contract for a service that handles display-related transformations of data,
    /// including inverse transforms for UI elements like axis labels and data tables.
    /// </summary>
    public interface IAnalysisTransformer
    {
        /// <summary>
        /// Transforms a value on the X-axis for display purposes.
        /// </summary>
        decimal TransformXValue(decimal value);
        double TransformXValue(double value);

        /// <summary>
        /// Transforms a value on the Y-axis for display purposes.
        /// </summary>
        decimal TransformYValue(decimal value);
        double TransformYValue(double value);

        /// <summary>
        /// Inversely transforms a value from its display-transformed state back to its original state for the X-axis.
        /// </summary>
        decimal InverseTransformXValue(decimal value);
        double InverseTransformXValue(double value);

        /// <summary>
        /// Inversely transforms a value from its display-transformed state back to its original state for the Y-axis.
        /// </summary>
        decimal InverseTransformYValue(decimal value);
        double InverseTransformYValue(double value);

        decimal TransformValue(AxisKind axis, decimal value);
        double TransformValue(AxisKind axis, double value);

        decimal InverseTransformValue(AxisKind axis, decimal value);
        double InverseTransformValue(AxisKind axis, double value);
        void UpdateParameters(BoxCoxAnalysisResult resultForTransformerY, TransformTypeEnum yAxisTransformIn, BoxCoxAnalysisResult resultForTransformerX, TransformTypeEnum xAxisTransformIn);
    }

    /// <summary>
    /// Provides a single source of truth for all post-analysis, display-related transformations.
    /// This class consolidates scattered transformation logic to ensure consistency across the UI.
    /// </summary>
    [Serializable]
    public class AnalysisTransformer : IAnalysisTransformer
    {
        private BoxCoxAnalysisResult _yAnalysisResult;
        private TransformTypeEnum _yTransformType;
        private BoxCoxAnalysisResult _xAnalysisResult;
        private TransformTypeEnum _xTransformType;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTransformer"/> class.
        /// </summary>
        public AnalysisTransformer(BoxCoxAnalysisResult yAnalysisResult, TransformTypeEnum yTransformType, BoxCoxAnalysisResult xAnalysisResult, TransformTypeEnum xTransformType)
        {
            _yAnalysisResult = yAnalysisResult ?? new BoxCoxAnalysisResult();
            _yTransformType = yTransformType;
            _xAnalysisResult = xAnalysisResult ?? new BoxCoxAnalysisResult();
            _xTransformType = xTransformType;
        }

        public void UpdateParameters(BoxCoxAnalysisResult resultForTransformerY, TransformTypeEnum yAxisTransformIn, BoxCoxAnalysisResult resultForTransformerX, TransformTypeEnum xAxisTransformIn)
        {
            _yAnalysisResult = resultForTransformerY;
            _yTransformType = yAxisTransformIn;
            _xAnalysisResult = resultForTransformerX;
            _xTransformType = xAxisTransformIn;
        }

        public double TransformXValue(double value) => Convert.ToDouble(TransformXValue(Convert.ToDecimal(value)));
        public decimal TransformXValue(decimal value) => PerformTransform(value, _xTransformType, _xAnalysisResult);

        public double TransformYValue(double value) => Convert.ToDouble(TransformYValue(Convert.ToDecimal(value)));
        public decimal TransformYValue(decimal value) => PerformTransform(value, _yTransformType, _yAnalysisResult);

        public double InverseTransformXValue(double value) => Convert.ToDouble(InverseTransformXValue(Convert.ToDecimal(value)));
        public decimal InverseTransformXValue(decimal value) => PerformInverseTransform(value, _xTransformType, _xAnalysisResult);

        public double InverseTransformYValue(double value) => Convert.ToDouble(InverseTransformYValue(Convert.ToDecimal(value)));
        public decimal InverseTransformYValue(decimal value) => PerformInverseTransform(value, _yTransformType, _yAnalysisResult);

        public decimal TransformValue(AxisKind axis, decimal value)
        {
            switch (axis)
            {
                case AxisKind.X:
                    return TransformXValue(value);
                default:
                    return TransformYValue(value);
            }
        }

        public double TransformValue(AxisKind axis, double value) => Convert.ToDouble(TransformValue(axis, Convert.ToDecimal(value)));


        public decimal InverseTransformValue(AxisKind axis, decimal value)
        {
            switch (axis)
            {
                case AxisKind.X:
                    return InverseTransformXValue(value);
                default:
                    return InverseTransformYValue(value);
            }
        }

        public double InverseTransformValue(AxisKind axis, double value) => Convert.ToDouble(InverseTransformValue(axis, Convert.ToDecimal(value)));

        private decimal PerformTransform(decimal value, TransformTypeEnum transformType, BoxCoxAnalysisResult analysisResult)
        {
            switch (transformType)
            {
                case TransformTypeEnum.Linear:
                    return value;

                case TransformTypeEnum.Log:
                    if (value <= 0m) return value;
                    try { return (decimal)Math.Log((double)value); }
                    catch (Exception exp)
                    {
                        PrintError(value, transformType, exp);
                        return value;
                    }

                case TransformTypeEnum.Inverse:
                    if (value == 0m) return value;
                    try { return 1.0m / value; }
                    catch (Exception exp)
                    {
                        PrintError(value, transformType, exp);
                        return value;
                    }

                case TransformTypeEnum.BoxCox:
                    if (value <= 0m) return value;
                    if (analysisResult.OriginalDataContainsZero) // Shifted Transform
                    {
                        double lambda = analysisResult.Lambda;
                        if (Math.Abs(lambda) < 1e-9) // Lambda is 0
                        {
                            if (value <= -1m) return value;
                            return (decimal)Math.Log((double)value + 1);
                        }

                        double k1 = analysisResult.K1;
                        if (Math.Abs(k1) < 1e-9) return value;
                        if (value <= -1m) return value;
                        try
                        {
                            return (decimal)((Math.Pow((double)value + 1, lambda) - 1) / k1);
                        }
                        catch (Exception exp)
                        {
                            PrintError(value, transformType, exp);
                            return value;
                        }
                    }
                    else // Standard Transform
                    {
                        double lambda = analysisResult.Lambda;
                        if (Math.Abs(lambda) < 1e-9) // Lambda is 0
                        {
                            if (value <= 0m) return value;
                            return (decimal)Math.Log((double)value);
                        }

                        if (value < 0m) return value;
                        try
                        {
                            return (decimal)((Math.Pow((double)value, lambda) - 1) / lambda);
                        }
                        catch (Exception exp)
                        {
                            PrintError(value, transformType, exp);
                            return value;
                        }
                    }

                default:
                    return value;
            }
        }

        private decimal PerformInverseTransform(decimal value, TransformTypeEnum transformType, BoxCoxAnalysisResult analysisResult)
        {
            switch (transformType)
            {
                case TransformTypeEnum.Linear:
                    return value;

                case TransformTypeEnum.Log:
                    if (value <= 0m) return value;
                    try { return (decimal)Math.Exp((double)value); }
                    catch (Exception exp)
                    {
                        PrintError(value, transformType, exp);
                        return value;
                    }

                case TransformTypeEnum.Inverse:
                    if (value == 0m) return value;
                    try { return 1.0m / value; }
                    catch (Exception exp)
                    {
                        PrintError(value, transformType, exp);
                        return value;
                    }

                case TransformTypeEnum.BoxCox:
                    if (value <= 0m) return value;
                    if (analysisResult.OriginalDataContainsZero) // Shifted Transform
                    {
                        double lambda = analysisResult.Lambda;
                        if (Math.Abs(lambda) < 1e-9) // Lambda is 0
                        {
                            try { return (decimal)Math.Exp((double)value) - 1; }
                            catch (Exception exp)
                            {
                                PrintError(value, transformType, exp);
                                return value;
                            }
                        }

                        double k1 = analysisResult.K1;
                        if (Math.Abs(k1) < 1e-9) return value;

                        double term = (double)value * k1 + 1;
                        if (term < 0) return value;
                        try
                        {
                            return (decimal)(Math.Pow(term, 1.0 / lambda) - 1);
                        }
                        catch (Exception exp)
                        {
                            PrintError(value, transformType, exp);
                            return value;
                        }
                    }
                    else // Standard Transform
                    {
                        double lambda = analysisResult.Lambda;
                        if (Math.Abs(lambda) < 1e-9) // Lambda is 0
                        {
                            try { return (decimal)Math.Exp((double)value); }
                            catch (Exception exp)
                            {
                                PrintError(value, transformType, exp);
                                return value;
                            }
                        }

                        double term = (double)value * lambda + 1;
                        if (term < 0) return value;
                        try
                        {
                            return (decimal)Math.Pow(term, 1.0 / lambda);
                        }
                        catch (Exception exp)
                        {
                            PrintError(value, transformType, exp);
                            return value;
                        }
                    }

                default:
                    return value;
            }
        }

        private static void PrintError(decimal value, TransformTypeEnum transformType, Exception exp)
        {
            Debug.WriteLine($"Value: {value}, Type: {transformType}, Error: {exp.Message}");
        }
    }
}
