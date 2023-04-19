//using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using NUnit.Framework;
using Moq;
using CSharpBackendWithR;
using POD.Analyze;
using POD;
using System.Data;
using POD.Data;

namespace Analyze.UnitTests
{
    [TestFixture]
    public class AnalysisTests
    {
        private Mock<ITemporaryLambdaCalc> _tempLambdaCalc;
        private Mock<IREngineObject> _rEngine;
        private Mock<I_IPy4C> _python;
        private Analysis _analysis;
        private DataTable _testDataTable;
        [SetUp]
        public void SetUp()
        {
            _tempLambdaCalc = new Mock<ITemporaryLambdaCalc>();
            _rEngine = new Mock<IREngineObject>();
            _python = new Mock<I_IPy4C>();
            _analysis = new Analysis();
            _analysis.Name = "SampleAnalysis";
        }
        private void SetPythonAndREngines()
        {
            _analysis.SetPythonEngine(_python.Object);
            _analysis.SetREngine(_rEngine.Object);
        }
        private void DataTableSampleSetupLinear()
        {
            _testDataTable = new DataTable();
            _testDataTable.Columns.Add("Test_Column_1");
            _testDataTable.Columns[0].DataType = typeof(double);
            for (int i =0; i< 11; i++)
            {
                _testDataTable.Rows.Add((double)i);
            }
        }
        /// <summary>
        /// Tests for the SetUpLambda() function
        /// </summary>
        [Test]
        public void SetUpLambda_ValidLambdaCalculated_AssignedLambdaToInLambdaValueField()
        {
            //Arrange
            _analysis.AnalysisDataType = AnalysisDataTypeEnum.AHat;
            _python.Setup(ahat => ahat.AHatAnalysis("SampleAnalysis")).Returns(new AHatAnalysisObject("SampleAnalysis"));
            SetPythonAndREngines();
            _tempLambdaCalc.Setup(l => l.CalcTempLambda()).Returns(1.0);           
            //Act
            _analysis.SetUpLambda(_tempLambdaCalc.Object);
            //Assert
            Assert.That(_analysis.InLambdaValue, Is.EqualTo(1.0));
        }

        /// <summary>
        /// Tests UpdateProgress(Object sender, int myProgressOutOF100) check if needed first
        /// </summary>

        /// <summary>
        /// Tests UpdateStatus(Object sender, string myCurrentStatus) check if needed first
        /// </summary>

        /// <summary>
        /// Tests AddError(Object sender, string myNewError) check if needed first
        /// </summary>

        /// <summary>
        /// Tests ClearErrors(Object sender) check if needed first
        /// </summary>

        /// <summary>
        /// cannot test ForceUpdateInputsFromData(bool recheckAnalysisType = false, AnalysisDataTypeEnum forcedType = AnalysisDataTypeEnum.Undefined) at this time
        /// </summary>

        /// <summary>
        /// Tests for the CalculateInitialValuesWithNewData() function
        /// </summary>
        [Test]
        public void CalculateInitialValuesWithNewData_PythonIsNullAndHasBeenInitizlizedFalse_HasBeenInitializedStillFalse()
        {
            //Arrange
            Analysis analysis = new Analysis();
            analysis.HasBeenInitialized = false;
            analysis.SetPythonEngine(null);
            //Act
            analysis.CalculateInitialValuesWithNewData();
            //Assert
            Assert.That(analysis.HasBeenInitialized, Is.False);
            Assert.That(analysis.Data.HMAnalysisObject == null);
        }
        [Test]
        public void CalculateInitialValuesWithNewData_PythonIsNullAndHasBeenInitizlizedTrue_HasBeenInitializedIsStillTrueAndPythonNull()
        {
            //Arrange
            Analysis analysis = new Analysis();
            analysis.HasBeenInitialized = true;
            analysis.SetPythonEngine(null);
            //Act
            analysis.CalculateInitialValuesWithNewData();
            //Assert
            Assert.That(analysis.HasBeenInitialized, Is.True);
            Assert.That(analysis.Python, Is.Null);
            Assert.That(analysis.Data.HMAnalysisObject == null);
        }
        [Test]
        public void CalculateInitialValuesWithNewData_PythonIsNotNullAndHasBeenInitizlizedTrue_HasBeenInitializedIsStillTrueAndPythonNotNull()
        {
            //Arrange           
            _analysis.HasBeenInitialized = true;
            SetPythonAndREngines();
            //Act
            _analysis.CalculateInitialValuesWithNewData();
            //Assert
            Assert.That(_analysis.HasBeenInitialized, Is.True);
            Assert.That(_analysis.Python, Is.Not.Null);
            Assert.That(_analysis.Data.HMAnalysisObject == null);
        }
        [Test]
        public void CalculateInitialValuesWithNewData_PythonNotNullAndHasBeenInitizlizedFalseNotHtMiss_ForceUpdateFunctionFired()
        {
            //Arrange           
            _analysis.HasBeenInitialized = false;
            SetPythonAndREngines();
            //Act
            _analysis.CalculateInitialValuesWithNewData();
            //Assert
            Assert.That(_analysis.HasBeenInitialized, Is.True);
            // These values are set when ForceUpdateFunctionIsFired
            Assert.That(_analysis.InResponseDecisionMin, Is.EqualTo(.05));
            Assert.That(_analysis.InResponseDecisionMax, Is.EqualTo(.05));
            Assert.That(_analysis.InResponseDecisionIncCount, Is.EqualTo(30));
            //
            Assert.That(_analysis.AnalysisDataType, Is.EqualTo(AnalysisDataTypeEnum.AHat));
            Assert.That(_analysis.InFlawTransform, Is.EqualTo(TransformTypeEnum.Linear));
        }
        [Test]
        public void CalculateInitialValuesWithNewData_PythonNotNullAndHasBeenInitizlizedFalseNotHtMiss_ForceUpdateFunctionFiredAndHMModelSetToLog()
        {
            //Arrange           
            _analysis.AnalysisDataType = AnalysisDataTypeEnum.HitMiss;
            _analysis.Data.DataType = AnalysisDataTypeEnum.HitMiss;
            _analysis.HasBeenInitialized = false;
            _python.Setup(p => p.TransformEnumToInt(TransformTypeEnum.Log)).Returns(2);
            _python.Setup(hitmiss => hitmiss.HitMissAnalsysis("SampleAnalysis")).Returns(new HMAnalysisObject("SampleAnalysis"));
            SetPythonAndREngines();
            //Act
            _analysis.CalculateInitialValuesWithNewData();
            //Assert
            Assert.That(_analysis.HasBeenInitialized, Is.True);
            // These values are set when ForceUpdateFunctionIsFired
            Assert.That(_analysis.InResponseDecisionMin, Is.EqualTo(.05));
            Assert.That(_analysis.InResponseDecisionMax, Is.EqualTo(.05));
            Assert.That(_analysis.InResponseDecisionIncCount, Is.EqualTo(30));
            //***
            Assert.That(_analysis.AnalysisDataType, Is.EqualTo(AnalysisDataTypeEnum.HitMiss));
            Assert.That(_analysis.InFlawTransform, Is.EqualTo(TransformTypeEnum.Log));
            Assert.That(_analysis.Data.FlawTransform, Is.EqualTo(TransformTypeEnum.Log));
        }

        /// <summary>
        /// Tests for the GetBufferedMinMax(DataTable myTable, out double myMin, out double myMax) function
        /// Note: this function does not accept negative flaw values
        /// </summary>
        [Test]
        public void GetBufferedMinMax_EmptyDataTable_ReturnsNegativePt1MinAnd1Pt1Max()
        {
            //Arrange
            DataTable emptyDataTable = new DataTable();
            double myMin = Double.NaN;
            double myMax = Double.NaN;
            //Act
            Analysis.GetBufferedMinMax(emptyDataTable, out myMin, out myMax);
            //Assert
            Assert.That(myMin, Is.EqualTo(-.1));
            Assert.That(myMax, Is.EqualTo(1.1));

        }
        [Test]
        public void GetBufferedMinMax_NonEmptyDataTableLinear_ReturnsBufferedMinAndMaxOfDataTable()
        {
            //Arrange
            DataTableSampleSetupLinear();
            double myMin = Double.NaN;
            double myMax = Double.NaN;
            //Act
            Analysis.GetBufferedMinMax(_testDataTable, out myMin, out myMax);
            //Assert
            Assert.That(myMin, Is.EqualTo(-1));
            Assert.That(myMax, Is.EqualTo(11));

        }
        /*
        [Test]
        public void GetBufferedMinMax_NonEmptyDataTableLog_ReturnsBufferedMinAndMaxOfDataTable()
        {
            DataTableSampleSetupLog();
            //Arrange
            double myMin = Double.NaN;
            double myMax = Double.NaN;
            //Act
            Analysis.GetBufferedMinMax(_testDataTable, out myMin, out myMax);
            //Assert
            Assert.That(myMin, Is.EqualTo(-1));
            Assert.That(myMax, Is.EqualTo(11));

        }
        */

        /// <summary>
        /// Tests for the CreateDuplicate() function
        /// </summary>
        [Test]
        public void CreateDuplicate_AnalysisObjectInitialized_ReturnsClonedAnalysisObject()
        {
            //Arrange
            DataSource source = new DataSource("DataSource", "ID", "Flaw", "Response");
            _analysis.SetDataSource(source);
            var placeholder=_analysis.Data.CommentDictionary;
            SetPythonAndREngines();
            //Act
            Analysis clone = _analysis.CreateDuplicate();
            //Assert
            Assert.That(_analysis != clone);
            Assert.That(clone.Python, Is.Null);
            Assert.That(_analysis.Python, Is.Not.Null);
        }

        /// <summary>
        /// Tests for the RunAnalysis(bool quickAnalysis=false) function
        /// </summary>
        [Test]
        [Ignore("Need to figure out how to mock a background worker")]
        public void RunAnalysis_NotQuickAnalysis_ReturnsTrue()
        {
            

        }
    }
}
