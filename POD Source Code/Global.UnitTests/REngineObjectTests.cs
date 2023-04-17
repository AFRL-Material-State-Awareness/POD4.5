using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Data;
using NUnit.Framework;
using POD;
using CSharpBackendWithR;
using RDotNet;
using System.Collections;

namespace Global.UnitTests
{
    [TestFixture]
    public class REngineObjectTests
    {
        private REngineObject _engineObject;
        private DataFrame _testDataFrame;
        [SetUp]
        public void SetUp()
        {
            _engineObject = new REngineObject();
            
        }
        [Test]
        public void rDataFrameToDataTable_EmptyDataFrame_ReturnsAnEmptyDatatable()
        {
            IEnumerable[] columns = CreateEmptyDfColumns();
            _testDataFrame = _engineObject.RDotNetEngine.CreateDataFrame(columns);

            DataTable result = _engineObject.rDataFrameToDataTable(_testDataFrame);

            Assert.That(result.Rows.Count, Is.EqualTo(0));
            Assert.That(result.Columns.Count, Is.EqualTo(0));

        }
        [Test]
        public void rDataFrameToDataTable_NotEmptyDataFrame_ReturnsDataInTheFormOfADataTable()
        {
            IEnumerable[] columns = CreateTestDfColumns();
            _testDataFrame = _engineObject.RDotNetEngine.CreateDataFrame(columns);

            DataTable result = _engineObject.rDataFrameToDataTable(_testDataFrame);

            var firstRow = result.Rows[0].ItemArray;
            var secondRow = result.Rows[1].ItemArray;
            //check the first two rows for validation
            //Approximation is used in the event the floating point numbers are acting weird
            Assert.That(NearlyEqual((double)firstRow[0], -1.0, .0000000000000001));
            Assert.That(NearlyEqual((double)firstRow[1], 1.0, .0000000000000001));
            Assert.That(NearlyEqual((double)firstRow[2], 1.1, .0000000000000001));

            Assert.That(NearlyEqual((double)secondRow[0], -2.0, .0000000000000001));
            Assert.That(NearlyEqual((double)secondRow[1], 2.0, .0000000000000001));
            Assert.That(NearlyEqual((double)secondRow[2], 2.1, .0000000000000001));

        }
        [Test]
        public void rDataFrameToDataTable_DataFrameWithColumnThatIsntADouble_ThrowsException()
        {
            IEnumerable[] columns = CreateTestDfColumnsInvalid();
            _testDataFrame = _engineObject.RDotNetEngine.CreateDataFrame(columns);

            Assert.That(() => _engineObject.rDataFrameToDataTable(_testDataFrame), Throws.TypeOf<ArgumentException>());
        }
        private IEnumerable[] CreateEmptyDfColumns()
        {
            IEnumerable[] columns = new IEnumerable[0];
            return columns;
        }
        /// <summary>
        /// Helper functions for testing the Dataframe to DataTable conversion function
        /// </summary>
        /// <returns></returns>
        private IEnumerable[] CreateTestDfColumns()
        {
            IEnumerable[] columns = new IEnumerable[3];
            columns[0] = new double[] { -1.0, -2.0, -3.0, -4.0, -5.0, -6.0, -7.0, -8.0, -9.0, -10.0 };
            columns[1] = new double[] { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0 };
            columns[2] = new double[] { 1.1, 2.1, 3.1, 4.1, 5.1, 6.1, 7.1, 8.1, 9.1, 10.1 };
            return columns;
        }
        private IEnumerable[] CreateTestDfColumnsInvalid()
        {
            IEnumerable[] columns = new IEnumerable[3];
            columns[0] = new string[] { "a", "a", "a", "a", "a", "b", "b", "b", "b", "b" };
            columns[1] = new double[] { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0 };
            columns[2] = new double[] { 1.1, 2.1, 3.1, 4.1, 5.1, 6.1, 7.1, 8.1, 9.1, 10.1 };
            return columns;
        }
        private static bool NearlyEqual(double a, double b, double epsilon)
        {
            const double MinNormal = 2.2250738585072014E-308d;
            double absA = Math.Abs(a);
            double absB = Math.Abs(b);
            double diff = Math.Abs(a - b);

            if (a.Equals(b))
            { // shortcut, handles infinities
                return true;
            }
            else if (a == 0 || b == 0 || absA + absB < MinNormal)
            {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < (epsilon * MinNormal);
            }
            else
            { // use relative error
                return diff / (absA + absB) < epsilon;
            }
        }

    }
}
