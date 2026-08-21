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
    public interface IDataTransformer
    {
        TransformTypeEnum FlawTransform { get; }
        TransformTypeEnum ResponseTransform { get; }

        /// <summary>
        /// Transforms a value on the X-axis for display purposes.
        /// </summary>
        decimal TransformXAxisValue(decimal value);
        double TransformXAxisValue(double value);

        decimal TransformXDataValue(decimal value);
        double TransformXDataValue(double value);

        /// <summary>
        /// Transforms a value on the Y-axis for display purposes.
        /// </summary>
        decimal TransformYAxisValue(decimal value);
        double TransformYAxisValue(double value);

        decimal TransformYDataValue(decimal value);
        double TransformYDataValue(double value);

        /// <summary>
        /// Inversely transforms a value from its display-transformed state back to its original state for the X-axis.
        /// </summary>
        decimal InverseTransformXAxisValue(decimal value);
        double InverseTransformXAxisValue(double value);

        decimal InverseTransformXDataValue(decimal value);
        double InverseTransformXDataValue(double value);

        /// <summary>
        /// Inversely transforms a value from its display-transformed state back to its original state for the Y-axis.
        /// </summary>
        decimal InverseTransformYAxisValue(decimal value);
        double InverseTransformYAxisValue(double value);

        decimal InverseTransformYDataValue(decimal value);
        double InverseTransformYDataValue(double value);

        decimal TransformAxisValue(AxisKind axis, decimal value, TransformTypeEnum forcedTransform);
        double TransformAxisValue(AxisKind axis, double value, TransformTypeEnum forcedTransform);

        decimal TransformAxisValue(AxisKind axis, decimal value);
        double TransformAxisValue(AxisKind axis, double value);

        decimal TransformDataValue(AxisKind axis, decimal value);
        double TransformDataValue(AxisKind axis, double value);

        decimal InverseTransformAxisValue(AxisKind axis, decimal value, TransformTypeEnum forcedTransform);
        double InverseTransformAxisValue(AxisKind axis, double value, TransformTypeEnum forcedTransform);

        decimal InverseTransformAxisValue(AxisKind axis, decimal value);
        double InverseTransformAxisValue(AxisKind axis, double value);

        decimal InverseTransformDataValue(AxisKind axis, decimal value);
        double InverseTransformDataValue(AxisKind axis, double value);


        void UpdateAxisTransformParameters(BoxCoxAnalysisResult resultForTransformerY, TransformTypeEnum yAxisTransformIn, double yMinData, double yMaxData, BoxCoxAnalysisResult resultForTransformerX, TransformTypeEnum xAxisTransformIn, double xMinData, double xMaxData);
        IDataTransformer Duplicate();
        void CopyTo(IDataTransformer copyTo);
        void ForceAxisTransforms(TransformTypeEnum yAxis, TransformTypeEnum xAxis);

        bool IsSymLogActiveX();
        bool IsSymLogActiveY();
    }

    /// <summary>
    /// Provides a single source of truth for all post-analysis, display-related transformations.
    /// This class consolidates scattered transformation logic to ensure consistency across the UI.
    /// </summary>
    [Serializable]
    public class DataTransformer : IDataTransformer
    {
        private BoxCoxAnalysisResult _yAnalysisResult;
        private TransformTypeEnum _yTransformType;
        private BoxCoxAnalysisResult _xAnalysisResult;
        private TransformTypeEnum _xTransformType;
        private double _xMinDataValue = double.MinValue;
        private double _xMaxDataValue = double.MaxValue;
        private double _yMinDataValue = double.MinValue;
        private double _yMaxDataValue = double.MaxValue;
        private bool _isSymlogActiveForY = false;
        private bool _isSymlogActiveForX = false;

        public DataTransformer()
        {
            _yAnalysisResult = new BoxCoxAnalysisResult();
            _yTransformType = TransformTypeEnum.Linear;
            _xAnalysisResult = new BoxCoxAnalysisResult();
            _xTransformType = TransformTypeEnum.Linear;
            _yMinDataValue = 0.0;
            _yMaxDataValue = 1.0;
            _xMinDataValue = 0.0;
            _xMaxDataValue = 1.0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTransformer"/> class.
        /// </summary>
        public DataTransformer(BoxCoxAnalysisResult yAnalysisResult, TransformTypeEnum yTransformType, double yMinData, double yMaxData, BoxCoxAnalysisResult xAnalysisResult, TransformTypeEnum xTransformType, double xMinData, double xMaxData)
        {
            _yAnalysisResult = yAnalysisResult ?? new BoxCoxAnalysisResult();
            _yTransformType = yTransformType;
            _xAnalysisResult = xAnalysisResult ?? new BoxCoxAnalysisResult();
            _xTransformType = xTransformType;
            _yMinDataValue = yMinData;
            _yMaxDataValue = yMaxData;
            _xMinDataValue = xMinData;
            _xMaxDataValue = xMaxData;
        }

        public void UpdateAxisTransformParameters(BoxCoxAnalysisResult resultForTransformerY, TransformTypeEnum yAxisTransformIn, double yMinData, double yMaxData, BoxCoxAnalysisResult resultForTransformerX, TransformTypeEnum xAxisTransformIn, double xMinData, double xMaxData)
        {
            _yAnalysisResult = resultForTransformerY;
            if (yAxisTransformIn != TransformTypeEnum.None)
                _yTransformType = yAxisTransformIn;
            _xAnalysisResult = resultForTransformerX;
            if (xAxisTransformIn != TransformTypeEnum.None)
                _xTransformType = xAxisTransformIn;
            _yMinDataValue = yMinData;
            _yMaxDataValue = yMaxData;
            _xMinDataValue = xMinData;
            _xMaxDataValue = xMaxData;
        }

        public TransformTypeEnum FlawTransform => _xTransformType;
        public TransformTypeEnum ResponseTransform => _yTransformType;

        public bool IsSymLogActiveX() => _isSymlogActiveForX;
        public bool IsSymLogActiveY() => _isSymlogActiveForY;

        public double TransformXAxisValue(double value) => Convert.ToDouble(TransformXAxisValue(Convert.ToDecimal(value)));
        public decimal TransformXAxisValue(decimal value) => PerformAxisTransform(value, _xTransformType, _xAnalysisResult, _xMinDataValue, _xMaxDataValue, IsSymLogActiveX());

        public double TransformXDataValue(double value) => Convert.ToDouble(TransformXDataValue(Convert.ToDecimal(value)));
        public decimal TransformXDataValue(decimal value) => PerformDataTransform(value, _xTransformType, _xAnalysisResult);

        public double TransformYAxisValue(double value) => Convert.ToDouble(TransformYAxisValue(Convert.ToDecimal(value)));
        public decimal TransformYAxisValue(decimal value) => PerformAxisTransform(value, _yTransformType, _yAnalysisResult, _yMinDataValue, _yMaxDataValue, IsSymLogActiveY());

        public double TransformYDataValue(double value) => Convert.ToDouble(TransformYDataValue(Convert.ToDecimal(value)));
        public decimal TransformYDataValue(decimal value) => PerformDataTransform(value, _yTransformType, _yAnalysisResult);

        public double InverseTransformXAxisValue(double value) => Convert.ToDouble(InverseTransformXAxisValue(Convert.ToDecimal(value)));
        public decimal InverseTransformXAxisValue(decimal value) => PerformInverseAxisTransform(value, _xTransformType, _xAnalysisResult, _xMinDataValue, _xMaxDataValue, IsSymLogActiveX());

        public double InverseTransformXDataValue(double value) => Convert.ToDouble(InverseTransformXDataValue(Convert.ToDecimal(value)));
        public decimal InverseTransformXDataValue(decimal value) => PerformInverseDataTransform(value, _xTransformType, _xAnalysisResult);

        public double InverseTransformYAxisValue(double value) => Convert.ToDouble(InverseTransformYAxisValue(Convert.ToDecimal(value)));
        public decimal InverseTransformYAxisValue(decimal value) => PerformInverseAxisTransform(value, _yTransformType, _yAnalysisResult, _yMinDataValue, _yMaxDataValue, IsSymLogActiveY());

        public double InverseTransformYDataValue(double value) => Convert.ToDouble(InverseTransformYDataValue(Convert.ToDecimal(value)));
        public decimal InverseTransformYDataValue(decimal value) => PerformInverseDataTransform(value, _yTransformType, _yAnalysisResult);

        public decimal TransformAxisValue(AxisKind axis, decimal value, TransformTypeEnum forcedTransform)
        {
            switch (axis)
            {
                case AxisKind.X:
                    TransformTypeEnum previousX = _xTransformType;
                    _xTransformType = forcedTransform;

                    var resultX = TransformXAxisValue(value);

                    _xTransformType = previousX;

                    return resultX;
                default:
                    TransformTypeEnum previousY = _yTransformType;
                    _yTransformType = forcedTransform;

                    var resultY = TransformYAxisValue(value);

                    _yTransformType = previousY;

                    return resultY;
            }
        }

        public double TransformAxisValue(AxisKind axis, double value, TransformTypeEnum forcedTransform)
        {
            switch (axis)
            {
                case AxisKind.X:
                    TransformTypeEnum previousX = _xTransformType;
                    _xTransformType = forcedTransform;

                    var resultX = TransformXAxisValue(value);

                    _xTransformType = previousX;

                    return resultX;
                default:
                    TransformTypeEnum previousY = _yTransformType;
                    _yTransformType = forcedTransform;

                    var resultY = TransformYAxisValue(value);

                    _yTransformType = previousY;

                    return resultY;
            }
        }

        public decimal TransformAxisValue(AxisKind axis, decimal value)
        {
            switch(axis)
            {
                case AxisKind.X:
                    return TransformXAxisValue(value);
                default:
                    return TransformYAxisValue(value);
            }
        }

        public decimal TransformDataValue(AxisKind axis, decimal value)
        {
            switch (axis)
            {
                case AxisKind.X:
                    return TransformXDataValue(value);
                default:
                    return TransformYDataValue(value);
            }
        }

        public double TransformAxisValue(AxisKind axis, double value) => Convert.ToDouble(TransformAxisValue(axis, Convert.ToDecimal(value)));

        public decimal InverseTransformAxisValue(AxisKind axis, decimal value, TransformTypeEnum forcedTransform)
        {
            switch (axis)
            {
                case AxisKind.X:
                    TransformTypeEnum previousX = _xTransformType;
                    _xTransformType = forcedTransform;

                    var resultX = InverseTransformXAxisValue(value);

                    _xTransformType = previousX;

                    return resultX;
                default:
                    TransformTypeEnum previousY = _yTransformType;
                    _yTransformType = forcedTransform;

                    var resultY = InverseTransformYAxisValue(value);

                    _yTransformType = previousY;

                    return resultY;
            }
        }

        public double InverseTransformAxisValue(AxisKind axis, double value, TransformTypeEnum forcedTransform)
        {
            switch (axis)
            {
                case AxisKind.X:
                    TransformTypeEnum previousX = _xTransformType;
                    _xTransformType = forcedTransform;

                    var resultX = InverseTransformXAxisValue(value);

                    _xTransformType = previousX;

                    return resultX;
                default:
                    TransformTypeEnum previousY = _yTransformType;
                    _yTransformType = forcedTransform;

                    var resultY = InverseTransformYAxisValue(value);

                    _yTransformType = previousY;

                    return resultY;
            }
        }

        public decimal InverseTransformAxisValue(AxisKind axis, decimal value)
        {
            switch (axis)
            {
                case AxisKind.X:
                    return InverseTransformXAxisValue(value);
                default:
                    return InverseTransformYAxisValue(value);
            }
        }

        public double TransformDataValue(AxisKind axis, double value) => Convert.ToDouble(TransformDataValue(axis, Convert.ToDecimal(value)));


        public decimal InverseTransformDataValue(AxisKind axis, decimal value)
        {
            switch (axis)
            {
                case AxisKind.X:
                    return InverseTransformXDataValue(value);
                default:
                    return InverseTransformYDataValue(value);
            }
        }

        public double InverseTransformAxisValue(AxisKind axis, double value) => Convert.ToDouble(InverseTransformAxisValue(axis, Convert.ToDecimal(value)));

        public double InverseTransformDataValue(AxisKind axis, double value) => Convert.ToDouble(InverseTransformAxisValue(axis, Convert.ToDecimal(value)));

        private decimal PerformAxisTransform(decimal value, TransformTypeEnum transformType, BoxCoxAnalysisResult analysisResult, double smallestValue, double largestValue, bool symlogActive)
        {
            switch (transformType)
            {
                case TransformTypeEnum.Linear:
                    return value;

                case TransformTypeEnum.Log:
                    //if (value <= 0m) return value;
                    //if (value <= 0.0m)
                    //{
                    //    return (decimal)Math.Log((double)(smallestValue + 1E-9) / 2.0);
                    //}

                    //if (value == 0.0m)
                    //{
                    //    value = (decimal)smallestValue;
                    //}

                    try
                    {
                        if (Compute.ApproximatelyMoreThan((double)value, Compute.Epsilon))
                        {
                            var result = (decimal)Math.Log((double)value);

                            return result;
                        }
                        else
                        {
                            decimal result = (decimal)(((double)value / Compute.ZeroTransformEpsilon) + Math.Log(Compute.ZeroTransformEpsilon) - 1.0);
                            return result;
                        }
                    }
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

        private decimal PerformDataTransform(decimal value, TransformTypeEnum transformType, BoxCoxAnalysisResult analysisResult)
        {
            switch (transformType)
            {
                case TransformTypeEnum.Linear:
                    return value;

                case TransformTypeEnum.Log:
                    //if (value <= 0m) return value;
                    //if (value <= 0.0m)
                    //{
                    //    return (decimal)Math.Log((double)(smallestValue + 1E-9) / 2.0);
                    //}

                    //if (value == 0.0m)
                    //{
                    //    value = (decimal)smallestValue;
                    //}

                    try
                    {
                        //decimal result = (decimal)Math.Log((double)value);
                        //return result;

                        if (Compute.ApproximatelyMoreThan((double)value, Compute.Epsilon))
                        {
                            var result = (decimal)Math.Log((double)value);

                            return result;
                        }
                        else
                        {
                            decimal result = (decimal)(((double)value / Compute.ZeroTransformEpsilon) + Math.Log(Compute.ZeroTransformEpsilon) - 1.0);
                            return result;
                        }
                    }
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

        private decimal PerformInverseDataTransform(decimal value, TransformTypeEnum transformType, BoxCoxAnalysisResult analysisResult)
        {
            switch (transformType)
            {
                case TransformTypeEnum.Linear:
                    return value;

                case TransformTypeEnum.Log:
                    if (value >= 66.0m)
                    {
                        return value;
                    }

                    try
                    {
                        if (Compute.ApproximatelyMoreThan((double)value, Math.Log(Compute.Epsilon)))
                        {
                            decimal result = (decimal)Math.Exp((double)value);

                            return result;
                        }
                        else
                        {
                            decimal result = (decimal)(((double)value - Math.Log(Compute.ZeroTransformEpsilon) + 1) * Compute.ZeroTransformEpsilon);

                            return result;
                        }

                    }
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

        private decimal PerformInverseAxisTransform(decimal value, TransformTypeEnum transformType, BoxCoxAnalysisResult analysisResult, double smallestValue, double largestValue, bool symlogActive)
        {
            switch (transformType)
            {
                case TransformTypeEnum.Linear:
                    return value;

                case TransformTypeEnum.Log:
                    if (value >= 66.0m)
                    {
                        return value;
                    }

                    try 
                    {

                        if (Compute.ApproximatelyMoreThan((double)value, Math.Log(Compute.Epsilon)))
                        {
                            decimal result = (decimal)Math.Exp((double)value);

                            return result;
                        }
                        else
                        {
                            decimal result = (decimal)(((double)value - Math.Log(Compute.ZeroTransformEpsilon) + 1) * Compute.ZeroTransformEpsilon);
                            return result;
                        }
                        
                    }
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

        public void ForceAxisTransforms(TransformTypeEnum yAxis, TransformTypeEnum xAxis)
        {
            this._yTransformType = yAxis;
            this._xTransformType = xAxis;
        }

        public IDataTransformer Duplicate()
        {
            return new DataTransformer(_yAnalysisResult, _yTransformType, _yMinDataValue, _yMaxDataValue, _xAnalysisResult, _xTransformType, _xMinDataValue, _xMaxDataValue);
        }

        private static void PrintError(decimal value, TransformTypeEnum transformType, Exception exp)
        {
            //Debug.WriteLine($"Value: {value}, Type: {transformType}, Error: {exp.Message}");
        }

        public void CopyTo(IDataTransformer copyTo)
        {
            var copyToTransformer = (DataTransformer)copyTo;
            copyToTransformer._yAnalysisResult = _yAnalysisResult;
            copyToTransformer._yTransformType = _yTransformType;
            copyToTransformer._xAnalysisResult = _xAnalysisResult;
            copyToTransformer._xTransformType = _xTransformType;
            copyToTransformer._yMinDataValue = _yMinDataValue;
            copyToTransformer._yMaxDataValue = _yMaxDataValue;
            copyToTransformer._xMinDataValue = _xMinDataValue;
            copyToTransformer._xMaxDataValue = _xMaxDataValue;
        }
    }
}
