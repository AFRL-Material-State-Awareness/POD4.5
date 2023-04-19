//using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using NUnit.Framework;
using Moq;
using CSharpBackendWithR;
using POD.Analyze;
using POD;
using System.Data;
using POD.Data;
using POD.ExcelData;
using SpreadsheetLight;

namespace Analyze.UnitTests
{
    [TestFixture]
    public class AnalysisTests
    {
        private Mock<ITemporaryLambdaCalc> _tempLambdaCalc;
        private Mock<IREngineObject> _rEngine;
        private Mock<I_IPy4C> _python;
        private Mock<IExcelExport> _excelOutput;
        private Analysis _analysis;
        private DataTable _testDataTable;
        [SetUp]
        public void SetUp()
        {
            _tempLambdaCalc = new Mock<ITemporaryLambdaCalc>();
            _rEngine = new Mock<IREngineObject>();
            _python = new Mock<I_IPy4C>();
            _excelOutput = new Mock<IExcelExport>();
            _analysis = new Analysis();
            _analysis.Name = "SampleAnalysis";
            _analysis.WorksheetName = "myWorkSheet";
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
        // <summary>
        /// Tests for the RunAnalysis(bool quickAnalysis=false) function
        /// </summary>
        [Test]
        public void UpdateRTransforms_AnalysisTypeHitMissNoChange_ModelHitMissTheSame()
        {
            SetupIPy4CTransformsHitMiss(TransformTypeEnum.Linear, 1);
            SetPythonAndREngines();
            var originalModel = _analysis.Data.HMAnalysisObject.ModelType;
            //don't change the flaw transform to log
            //Act
            _analysis.UpdateRTransforms();
            //Assert
            Assert.That(_analysis.Data.HMAnalysisObject.ModelType, Is.EqualTo(originalModel));
            Assert.That(_analysis.Data.AHATAnalysisObject, Is.Null);
        }

        // <summary>
        /// Tests for the UpdateRTransforms() function
        /// </summary>
        [Test]
        [TestCase(TransformTypeEnum.Log , 2)]
        [TestCase(TransformTypeEnum.Inverse, 3)]
        public void UpdateRTransforms_AnalysisTypeHitMissChangesTransform_ReturnsModelUpdateForHitMiss(TransformTypeEnum transformChange, int expectedModelType)
        {
            SetupIPy4CTransformsHitMiss(transformChange, expectedModelType);
            SetPythonAndREngines();
            //change the flaw transform to log
            _analysis.InFlawTransform = transformChange;
            //Act
            _analysis.UpdateRTransforms();
            //Assert
            Assert.That(_analysis.Data.HMAnalysisObject.ModelType, Is.EqualTo(expectedModelType));
            Assert.That(_analysis.Data.AHATAnalysisObject, Is.Null);
        }
        [Test]
        public void UpdateRTransforms_AnalysisTypeAHatNoChange_TransformsAndModelSame()
        {
            SetupIPy4CTransformsHitAhat();
            SetPythonAndREngines();
            var originalModel = _analysis.Data.AHATAnalysisObject.ModelType;
            //dont change transforms
            //Act
            _analysis.UpdateRTransforms();
            //Assert
            Assert.That(_analysis.Data.AHATAnalysisObject.ModelType, Is.EqualTo(originalModel));
            Assert.That(_analysis.Data.HMAnalysisObject, Is.Null);
        }
        [Test]
        [TestCase(TransformTypeEnum.Linear, TransformTypeEnum.Linear, 1)]
        [TestCase(TransformTypeEnum.Log, TransformTypeEnum.Linear, 2)]
        [TestCase(TransformTypeEnum.Linear, TransformTypeEnum.Log, 3)]
        [TestCase(TransformTypeEnum.Log, TransformTypeEnum.Log, 4)]
        [TestCase(TransformTypeEnum.Linear, TransformTypeEnum.BoxCox, 5)]
        [TestCase(TransformTypeEnum.Log, TransformTypeEnum.BoxCox, 6)]
        [TestCase(TransformTypeEnum.Inverse, TransformTypeEnum.BoxCox, 7)]
        [TestCase(TransformTypeEnum.Linear, TransformTypeEnum.Inverse, 8)]
        [TestCase(TransformTypeEnum.Log, TransformTypeEnum.Inverse, 9)]
        [TestCase(TransformTypeEnum.Inverse, TransformTypeEnum.Linear, 10)]
        [TestCase(TransformTypeEnum.Inverse, TransformTypeEnum.Log, 11)]
        [TestCase(TransformTypeEnum.Inverse, TransformTypeEnum.Inverse, 12)]

        public void UpdateRTransforms_AnalysisTypeAHatChanges_TransformsAndModelSame(TransformTypeEnum transformChangeX, TransformTypeEnum transformChangeY, int expectedModelType)
        {
            SetupIPy4CTransformsHitAhat();
            SetPythonAndREngines();
            //change the flaw transform to log
            _analysis.InFlawTransform = transformChangeX;
            _analysis.InResponseTransform = transformChangeY;
            //Act
            _analysis.UpdateRTransforms();
            //Assert
            Assert.That(_analysis.Data.AHATAnalysisObject.ModelType, Is.EqualTo(expectedModelType));
            Assert.That(_analysis.Data.HMAnalysisObject, Is.Null);
        }
        private void SetupIPy4CTransformsHitMiss(TransformTypeEnum testTransformX, int expectedOutput)
        {
            _analysis.AnalysisDataType = AnalysisDataTypeEnum.HitMiss;
            _analysis.Data.DataType = AnalysisDataTypeEnum.HitMiss;
            _python.Setup(hitmiss => hitmiss.HitMissAnalsysis("SampleAnalysis")).Returns(new HMAnalysisObject("SampleAnalysis"));
            _python.Setup(modelType => modelType.TransformEnumToInt(testTransformX)).Returns(expectedOutput);
        }
        private void SetupIPy4CTransformsHitAhat()
        {
            _analysis.AnalysisDataType = AnalysisDataTypeEnum.AHat;
            _analysis.Data.DataType = AnalysisDataTypeEnum.AHat;
            _python.Setup(ahat => ahat.AHatAnalysis("SampleAnalysis")).Returns(new AHatAnalysisObject("SampleAnalysis"));
            //setup all possible transforms since AHat could be either
            _python.Setup(modelType => modelType.TransformEnumToInt(TransformTypeEnum.Linear)).Returns(1);
            _python.Setup(modelType => modelType.TransformEnumToInt(TransformTypeEnum.Log)).Returns(2);
            _python.Setup(modelType => modelType.TransformEnumToInt(TransformTypeEnum.Inverse)).Returns(3);
            _python.Setup(modelType => modelType.TransformEnumToInt(TransformTypeEnum.BoxCox)).Returns(5);
        }
        /// <summary>
        /// Tests for the SetDataSource(DataSource mySource) function
        /// </summary>

        //// Need to Mock Data (AnlysisData) to effectively test this method

        /// <summary>
        /// Tests for the SetDataSource(DataSource mySource, List<string> myFlaws, List<string> myMetaDatas,
        /// List<string> myResponses, List<string> mySpecIDs) function
        /// </summary>

        //// Need to Mock Data (AnlysisData) to effectively test this method

        /// <summary>
        /// Tests for the public void WriteToExcel(ExcelExport myWriter, bool myPartOfProject = true, DataTable table = null) function
        /// </summary>

        /// TODO: write tests for this method

        /// <summary>
        ///  WriteQuickAnalysis(ExcelExport myWriter, DataTable myInputTable, string myOperator, string mySpecimentSet, string mySpecUnits, double mySpecMin, double mySpecMax,
        ///  string myInstrument = "", string myInstUnits = "", double myInstMin = 0.0, double myInstMax = 1.0)
        /// </summary>
        [Test]
        public void WriteQuickAnalysis_WriteHitMissQuickAnalysis_WrittenToExcelWithNoInstrumentOrInstMinMax()
        {
            //Arrange
            _analysis.AnalysisDataType = AnalysisDataTypeEnum.HitMiss;
            _analysis.Data.DataType = AnalysisDataTypeEnum.HitMiss;
            SetPythonAndREngines();
            var spreadSheet = new SLDocument();
            _excelOutput.SetupGet(w => w.Workbook).Returns(spreadSheet);
            //Act
            _analysis.WriteQuickAnalysis(_excelOutput.Object, new DataTable(), "operator", "specimentSet", "specUnits", 0.0, 10.0, "instrument", "units");
            //Assert
            VerifySetCells(Times.Never);
            /// This test will still pass in the event any cells are added or changed
            VerifySetCellsCount(13, 2);
        }
        [Test]
        public void WriteQuickAnalysis_WriteAHatQuickAnalysis_WrittenToExcelWithNoInstrumentOrInstMinMax()
        {
            //Arrange          
            _analysis.AnalysisDataType = AnalysisDataTypeEnum.AHat;
            _analysis.Data.DataType = AnalysisDataTypeEnum.AHat;
            SetPythonAndREngines();
            var spreadSheet = new SLDocument();
            _excelOutput.SetupGet(w => w.Workbook).Returns(spreadSheet);
            //Act
            _analysis.WriteQuickAnalysis(_excelOutput.Object, new DataTable(), "operator", "specimentSet", "specUnits", 0.0, 10.0, "instrument", "units");
            //Assert
            VerifySetCells(Times.Once);
            /// This test will still pass in the event any cells are added or changed
            VerifySetCellsCount(18, 4);
        }
        private void VerifySetCells(Func<Times> shouldExecute)
        {
            _excelOutput.Verify(e => e.SetCellValue(1, 1, "Quick Analysis"), Times.Once);
            _excelOutput.Verify(e => e.SetCellValue(It.IsAny<int>(), 1, "Instrument"), shouldExecute);
            _excelOutput.Verify(e => e.SetCellValue(It.IsAny<int>(), 1, "RESPONSE:"), shouldExecute);
            _excelOutput.Verify(e => e.SetCellValue(It.IsAny<int>(), 2, "instrument"), shouldExecute);
            _excelOutput.Verify(e => e.RemoveDefaultSheet(), Times.Once);
        }
        private void VerifySetCellsCount(int countString, int countWithDouble)
        {
            _excelOutput.Verify(e => e.SetCellValue(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.AtLeast(countString));
            _excelOutput.Verify(e => e.SetCellValue(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.AtLeast(countWithDouble));
        }
    }
}
