using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDotNet;
using System.Threading;
using System.Collections;

namespace POD
{
    public static class REngineExtensions
    {
        private static readonly object _syncRoot = new object();

        public static SymbolicExpression SafeEvaluate(this REngine engine, string statement)
        {
            // This lock is the "Hardening" layer
            lock (_syncRoot)
            {
                return engine.Evaluate(statement);
            }
        }

        // Use this for your dataframe creation
        public static DataFrame SafeCreateDataFrame(this REngine engine, IEnumerable[] columns, string[] columnNames)
        {
            lock (_syncRoot)
            {
                // We lock the engine while it allocates the internal R list and sets attributes
                return engine.CreateDataFrame(columns, columnNames: columnNames);
            }
        }

        // Hardened Vector Creation
        public static NumericVector SafeCreateNumericVector(this REngine engine, IEnumerable<double> values)
        {
            lock (_syncRoot)
            {
                // Allocation happens inside the lock to protect the R heap
                return engine.CreateNumericVector(values);
            }
        }

        // Hardened Vector Creation
        public static IntegerVector SafeCreateIntegerVector(this REngine engine, IEnumerable<int> values)
        {
            lock (_syncRoot)
            {
                return engine.CreateIntegerVector(values);
            }
        }

        // You can also harden other common R calls here
        public static void SafeSetSymbol(this REngine engine, string name, SymbolicExpression expression)
        {
            lock (_syncRoot)
            {
                engine.SetSymbol(name, expression);
            }
        }

        public static T SafeEvaluateAndFetch<T>(this REngine engine, string expression, Func<SymbolicExpression, T> extractor)
        {
            lock (_syncRoot)
            {
                // 1. Perform the evaluation while locked
                var rObject = engine.Evaluate(expression);

                // 2. Extract the data into C# types while STILL locked
                return extractor(rObject);
            }
        }

        public static T SafeBatch<T>(this REngine engine, Func<REngine, T> action)
        {
            lock (_syncRoot) // This is the private lock inside your extension class
            {
                return action(engine);
            }
        }
    }
}
