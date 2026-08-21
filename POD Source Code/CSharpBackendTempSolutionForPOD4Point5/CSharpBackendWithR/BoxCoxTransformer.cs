using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using POD;
using RDotNet;

namespace CSharpBackendWithR
{
    /// <summary>
    /// A custom exception thrown when the inverse transformation does not revert to the original data,
    /// indicating a potential issue in the transformation logic.
    /// </summary>
    public class TransformationVerificationException : Exception
    {
        public TransformationVerificationException(string message) : base(message) { }
    }

    /// <summary>
    /// A simple container holding a snapshot of data before and after a transformation,
    /// primarily used for debugging and verification.
    /// </summary>
    [Serializable]
    public class TransformationDataSnapshot
    {
        /// <summary>
        /// Stores the original, raw data with multiple response columns before any averaging.
        /// This property will be null if the transformation was called with a single column.
        /// </summary>
        public DataTable RawMultiColumnData { get; set; }

        /// <summary>
        /// Stores the single vector of data that was fed into the transformation algorithm.
        /// (This would be the averaged data in the multi-column case).
        /// </summary>
        public List<double> DataBeforeTransform { get; set; }

        /// <summary>
        /// Stores the final data after the forward transformation has been applied.
        /// </summary>
        public List<double> DataAfterTransform { get; set; }
    }

    /// <summary>
    /// A comprehensive and flexible container for the results of a Box-Cox analysis operation.
    /// It holds the statistical parameters and a snapshot of the data for debugging and verification.
    /// </summary>
    [Serializable]
    public class BoxCoxAnalysisResult
    {
        public double Lambda { get; set; }
        public Dictionary<string, object> AdditionalParameters { get; private set; }
        public TransformationDataSnapshot Data { get; set; }
        public bool OriginalDataContainsZero { get; set; }

        public BoxCoxAnalysisResult()
        {
            AdditionalParameters = new Dictionary<string, object>();
            Data = new TransformationDataSnapshot();
        }

        public double K1 => GetParameter<double>("K1");
        public double K2 => GetParameter<double>("K2");
        public double K_2_1 => GetParameter<double>("K_2.1");

        private T GetParameter<T>(string key)
        {
            if (AdditionalParameters.TryGetValue(key, out object value) && value is T typedValue)
            {
                return typedValue;
            }
            throw new InvalidOperationException($"The '{key}' parameter is not available or is not a valid {typeof(T).Name}.");
        }
    }

    /// <summary>
    /// Orchestrates the Box-Cox transformation process, acting as the primary interface for the application.
    /// This class calls an external engine to determine statistical parameters and then performs and verifies
    /// the transformations in C#.
    /// </summary>
    public interface IBoxCoxTransformer
    {
        void RecalculateK1(BoxCoxAnalysisResult result, double newLambda);
        double ForwardTransform(double value, BoxCoxAnalysisResult parameters);
        double InverseTransform(double transformedValue, BoxCoxAnalysisResult parameters);
        BoxCoxAnalysisResult ExecuteAndVerifyTransformation(DataTable wholeTable, DataTable nonCensoredTable, List<string> responseColumnNames);
    }

    [Serializable]
    public class BoxCoxTransformer : IBoxCoxTransformer
    {
        private const double VerificationTolerance = 1e-6;
        [NonSerialized]
        private readonly REngine _rEngine;

        public BoxCoxTransformer(IREngineObject rEngine)
        {
            _rEngine = rEngine.RDotNetEngine ?? throw new ArgumentNullException(nameof(rEngine));
        }

        public void RecalculateK1(BoxCoxAnalysisResult result, double newLambda)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            result.Lambda = newLambda;
            if (result.OriginalDataContainsZero)
            {
                double k2 = result.K2;
                if (k2 == 0) throw new InvalidOperationException("K2 cannot be zero when recalculating K1 for a shifted transform.");
                double k1 = newLambda * Math.Pow(k2, newLambda - 1);
                result.AdditionalParameters["K1"] = k1;
            }
        }

        public BoxCoxAnalysisResult ExecuteAndVerifyTransformation(DataTable wholeTable, DataTable nonCensoredTable, List<string> responseColumnNames)
        {
            //TODO: figure out if we can use null or empty result for X-Axis, TRB
            if(wholeTable == null || nonCensoredTable == null || responseColumnNames == null)
            {
                return new BoxCoxAnalysisResult();
            }


            // --- Part 1: Calculate parameters using NON-CENSORED data ---

            // Determine if the original, whole dataset contains zeros.
            bool originalDataContainsZero = wholeTable.AsEnumerable()
                .SelectMany(row => responseColumnNames.Select(col => Convert.ToDouble(row[col])))
                .Any(val => Compute.ApproximatelyLessThanOrEqual(val, 0.0));

            // 1a. Average the response columns from the NON-CENSORED table for parameter calculation
            var averagedNonCensoredData = new List<double>();
            var nonCensoredRowCount = nonCensoredTable.Rows.Count;
            for (int i = 0; i < nonCensoredRowCount; i++)
            {
                double sum = 0;
                foreach (var colName in responseColumnNames)
                {
                    sum += Convert.ToDouble(nonCensoredTable.Rows[i][colName]);
                }
                averagedNonCensoredData.Add(sum / responseColumnNames.Count);
            }

            // 2a. Create a new DataTable for CalculateParameters from the NON-CENSORED table
            var preparedTable = new DataTable();
            preparedTable.Columns.Add("Ahat", typeof(double));
            preparedTable.Columns.Add("Size", typeof(double));
            var sizeData = nonCensoredTable.AsEnumerable().Select(r => r.Field<double>("Size")).ToList();
            for (int i = 0; i < nonCensoredRowCount; i++)
            {
                preparedTable.Rows.Add(averagedNonCensoredData[i], sizeData[i]);
            }

            // 3a. Call CalculateParameters with the prepared non-censored table
            var analysisResult = CalculateParameters(preparedTable, "Ahat");

            // 4a. Overwrite the flag on the result object with the one derived from the whole table.
            analysisResult.OriginalDataContainsZero = originalDataContainsZero;


            // --- Part 2: Verify transformation on the ENTIRE dataset ---

            // 1b. Average the response columns from the WHOLE table for verification
            var averagedWholeData = new List<double>();
            var wholeRowCount = wholeTable.Rows.Count;
            for (int i = 0; i < wholeRowCount; i++)
            {
                double sum = 0;
                foreach (var colName in responseColumnNames)
                {
                    sum += Convert.ToDouble(wholeTable.Rows[i][colName]);
                }
                averagedWholeData.Add(sum / responseColumnNames.Count);
            }
            
            // 2b. Perform transformation and verification on the averaged WHOLE data
            var transformedData = averagedWholeData.Select(value => ForwardTransform(value, analysisResult)).ToList();
            var revertedData = transformedData.Select(value => InverseTransform(value, analysisResult)).ToList();
            for (int i = 0; i < averagedWholeData.Count; i++)
            {
                if (double.IsNaN(revertedData[i]) || Math.Abs(averagedWholeData[i] - revertedData[i]) > VerificationTolerance)
                {
                    string errorMessage = $"Box-Cox integrity check failed on WHOLE dataset at index {i}. Original: {averagedWholeData[i]}, Reverted: {revertedData[i]}.";
                    throw new TransformationVerificationException(errorMessage);
                }
            }

            // --- Part 3: Populate snapshot and return ---
            // The snapshot should contain the data that was actually used to FIT the model (non-censored).
            analysisResult.Data.RawMultiColumnData = nonCensoredTable;
            analysisResult.Data.DataBeforeTransform = averagedNonCensoredData;
            analysisResult.Data.DataAfterTransform = averagedNonCensoredData.Select(value => ForwardTransform(value, analysisResult)).ToList();

            return analysisResult;
        }

        private BoxCoxAnalysisResult CalculateParameters(DataTable nonCensoredData, string columnName)
        {
            // Capture the lock for the ENTIRE sequence so no other thread can 
            // mess with 'df_nc' or R memory until we have our result.
            return _rEngine.SafeBatch(engine =>
            {
                var ahatValues = nonCensoredData.AsEnumerable().Select(r => r.Field<double>("Ahat")).ToList();
                var sizeValues = nonCensoredData.AsEnumerable().Select(r => r.Field<double>("Size")).ToList();
                NumericVector ahatVector = _rEngine.SafeCreateNumericVector(ahatValues);
                NumericVector sizeVector = _rEngine.SafeCreateNumericVector(sizeValues);
                var df = _rEngine.SafeCreateDataFrame(new[] { ahatVector, sizeVector }, columnNames: new[] { "Ahat", "Size" });
                _rEngine.SafeSetSymbol("df_nc", df);
                //GenericVector rResult = _rEngine.SafeEvaluate("GetBoxCoxParameters(df_nc)").AsList();
                //var result = new BoxCoxAnalysisResult();
                //result.Lambda = rResult["lambda"].AsNumeric().First();
                //result.AdditionalParameters["K1"] = rResult["K1"].AsNumeric().First();
                //result.AdditionalParameters["K2"] = rResult["K2"].AsNumeric().First();
                //result.AdditionalParameters["K_2.1"] = rResult["K_2.1"].AsNumeric().First();
                // This check is no longer needed here as it's done on the whole table in the calling method.
                // result.OriginalDataContainsZero = ahatValues.Any(v => v <= 0);
                //return result;

                var result = _rEngine.SafeEvaluateAndFetch("GetBoxCoxParameters(df_nc)", rObject =>
                {
                // Inside here, we are protected by the lock
                var rList = rObject.AsList();

                // Create a simple C# DTO or Dictionary to hold the raw numbers
                var bcResult = new BoxCoxAnalysisResult();

                    bcResult.Lambda = rList["lambda"].AsNumeric().First();
                    bcResult.AdditionalParameters["K1"] = rList["K1"].AsNumeric().First();
                    bcResult.AdditionalParameters["K2"] = rList["K2"].AsNumeric().First();
                    bcResult.AdditionalParameters["K_2.1"] = rList["K_2.1"].AsNumeric().First();

                    return bcResult;
                });

                return result;
            });
        }

        public double ForwardTransform(double value, BoxCoxAnalysisResult parameters)
        {
            if (parameters.OriginalDataContainsZero)
            {
                if (parameters.Lambda == 0) return Math.Log(value + 1);
                if (parameters.K1 == 0) throw new InvalidOperationException("K1 cannot be zero for shifted Box-Cox transformation.");
                double trans = (Math.Pow(value + 1, parameters.Lambda) - 1) / parameters.K1;
                return trans;
            }
            else
            {
                if (parameters.Lambda == 0) return Math.Log(value);
                return (Math.Pow(value, parameters.Lambda) - 1) / parameters.Lambda;
            }
        }

        public double InverseTransform(double transformedValue, BoxCoxAnalysisResult parameters)
        {
            if (parameters.OriginalDataContainsZero)
            {
                if (parameters.Lambda == 0) return Math.Exp(transformedValue) - 1;
                if (parameters.K1 == 0) throw new InvalidOperationException("K1 cannot be zero for shifted Box-Cox transformation.");
                return Math.Pow(transformedValue * parameters.K1 + 1, 1 / parameters.Lambda) - 1;
            }
            else
            {
                if (parameters.Lambda == 0) return Math.Exp(transformedValue);
                return Math.Pow(transformedValue * parameters.Lambda + 1, 1 / parameters.Lambda);
            }
        }
    }
}
