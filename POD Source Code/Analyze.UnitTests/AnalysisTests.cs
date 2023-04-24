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
        private Mock<IAnalysisData> _data;
        private Mock<ITemporaryLambdaCalc> _tempLambdaCalc;
        private Mock<IREngineObject> _rEngine;
        private Mock<I_IPy4C> _python;
        private Mock<IExcelExport> _excelOutput;
        private Analysis _analysis;
        private DataTable _testDataTable;
        [SetUp]
        public void SetUp()
        {
            _data = new Mock<IAnalysisData>();
            _tempLambdaCalc = new Mock<ITemporaryLambdaCalc>();
            _rEngine = new Mock<IREngineObject>();
            _python = new Mock<I_IPy4C>();
            _excelOutput = new Mock<IExcelExport>();
            _analysis = new Analysis(_data.Object);
            _analysis.Name = "SampleAnalysis";
            _analysis.WorksheetName = "myWorkSheet";
        }
        private Analysis AnalysisWithNoDataMock()
        {
            var analysis = new Analysis();
            analysis.Name = "SampleAnalysis";
            analysis.WorksheetName = "myWorkSheet";
            return analysis;
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
            //_data.SetupSet(x => x.AHATAnalysisObject).Callback(value => dummyObject = value);
            _data.SetupGet(ahat => ahat.AHATAnalysisObject).Returns(new AHatAnalysisObject("SampleAnalysis"));
            _python.Setup(ahat => ahat.AHatAnalysis("SampleAnalysis")).Returns(new AHatAnalysisObject("SampleAnalysis"));
            SetPythonAndREngines();
            _tempLambdaCalc.Setup(l => l.CalcTempLambda()).Returns(1.0);           
            //Act
            _analysis.SetUpLambda(_tempLambdaCalc.Object);
            //Assert
            Assert.That(_analysis.InLambdaValue, Is.EqualTo(1.0));
            Assert.That(_analysis.Data.AHATAnalysisObject.Lambda, Is.EqualTo(1.0));
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
            Analysis analysis = new Analysis(_data.Object);
            analysis.HasBeenInitialized = false;
            analysis.SetPythonEngine(null);
            //Act
            analysis.CalculateInitialValuesWithNewData();
            //Assert
            _data.Verify(sp=>sp.SetPythonEngine((I_IPy4C)null, (String)null), Times.Once);
            Assert.That(analysis.Python, Is.Null);
            Assert.That(analysis.HasBeenInitialized, Is.False);
        }
        [Test]
        public void CalculateInitialValuesWithNewData_PythonIsNullAndHasBeenInitizlizedTrue_HasBeenInitializedIsStillTrueAndPythonNull()
        {
            //Arrange
            Analysis analysis = new Analysis(_data.Object);
            analysis.HasBeenInitialized = true;
            analysis.SetPythonEngine(null);
            //Act
            analysis.CalculateInitialValuesWithNewData();
            //Assert
            _data.Verify(sp => sp.SetPythonEngine((I_IPy4C)null, (String)null), Times.Once);
            Assert.That(analysis.HasBeenInitialized, Is.True);
            Assert.That(analysis.Python, Is.Null);
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
            _data.Verify(sp => sp.SetPythonEngine(_python.Object, "SampleAnalysis"), Times.Once);
            Assert.That(_analysis.HasBeenInitialized, Is.True);
            Assert.That(_analysis.Python, Is.Not.Null);
            Assert.That(_analysis.Data.HMAnalysisObject == null);
        }
        [Test]
        public void CalculateInitialValuesWithNewData_PythonNotNullAndHasBeenInitizlizedFalseNotHtMiss_ForceUpdateFunctionFired()
        {
            //Arrange           
            _analysis.HasBeenInitialized = false;
            AssignActivatedFlawsAndResponses();
            SetPythonAndREngines();
            //Act
            _analysis.CalculateInitialValuesWithNewData();
            //Assert
            Assert.That(_analysis.HasBeenInitialized, Is.True);
            _python.Verify(te => te.TransformEnumToInt(It.IsAny<TransformTypeEnum>()), Times.Never);
            // These values are set when ForceUpdateFunctionIsFired
            AssertForceUpdateFunctionIsFired();

            Assert.That(_analysis.AnalysisDataType, Is.EqualTo(AnalysisDataTypeEnum.AHat));
            Assert.That(_analysis.InFlawTransform, Is.EqualTo(TransformTypeEnum.Linear));
        }
        [Test]
        public void CalculateInitialValuesWithNewData_PythonNotNullAndHasBeenInitizlizedFalseIsHtMiss_ForceUpdateFunctionFiredAndHMModelSetToLog()
        {
            //Arrange
            //Analysis analysis=AnalysisWithNoDataMock();
            _analysis.AnalysisDataType = AnalysisDataTypeEnum.HitMiss;
            _analysis.HasBeenInitialized = false;
            AssignActivatedFlawsAndResponses();
            _data.SetupGet(hitmiss => hitmiss.HMAnalysisObject).Returns(new HMAnalysisObject("SampleAnalysis"));
            _data.SetupGet(dt => dt.DataType).Returns(AnalysisDataTypeEnum.HitMiss);
            _python.Setup(p => p.TransformEnumToInt(TransformTypeEnum.Log)).Returns(2);
            _python.Setup(hitmiss => hitmiss.HitMissAnalsysis("SampleAnalysis")).Returns(new HMAnalysisObject("SampleAnalysis"));
            SetPythonAndREngines();
            //Act
            _analysis.CalculateInitialValuesWithNewData();
            //Assert
            Assert.That(_analysis.HasBeenInitialized, Is.True);
            _python.Verify(te => te.TransformEnumToInt(It.IsAny<TransformTypeEnum>()), Times.Once);
            AssertForceUpdateFunctionIsFired();
            Assert.That(_analysis.AnalysisDataType, Is.EqualTo(AnalysisDataTypeEnum.HitMiss));
            Assert.That(_analysis.InFlawTransform, Is.EqualTo(TransformTypeEnum.Log));
            Assert.That(_analysis.Data.FlawTransform, Is.EqualTo(TransformTypeEnum.Log));
        }
        // These values are set when ForceUpdateFunctionIsFired

        private void AssertForceUpdateFunctionIsFired()
        {
            Assert.That(_analysis.InResponseDecisionMin, Is.EqualTo(.05));
            Assert.That(_analysis.InResponseDecisionMax, Is.EqualTo(.05));
            Assert.That(_analysis.InResponseDecisionIncCount, Is.EqualTo(30));
        }
        private void AssignActivatedFlawsAndResponses()
        {
            _data.SetupGet(ar => ar.ActivatedResponses).Returns(new DataTable());
            _data.SetupGet(ar => ar.ActivatedFlaws).Returns(new DataTable());
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
            //_data.Setup(cd => cd.CreateDuplicate()).Returns(_data.Object);
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
            var analysis =SetupIPy4CTransformsHitMiss(TransformTypeEnum.Linear, 1);
            
            var originalModel = analysis.Data.HMAnalysisObject.ModelType;
            //don't change the flaw transform to log
            //Act
            analysis.UpdateRTransforms();
            //Assert
            Assert.That(analysis.Data.HMAnalysisObject.ModelType, Is.EqualTo(originalModel));
            Assert.That(analysis.Data.AHATAnalysisObject, Is.Null);
        }

        // <summary>
        /// Tests for the UpdateRTransforms() function
        /// </summary>
        [Test]
        [TestCase(TransformTypeEnum.Log , 2)]
        [TestCase(TransformTypeEnum.Inverse, 3)]
        public void UpdateRTransforms_AnalysisTypeHitMissChangesTransform_ReturnsModelUpdateForHitMiss(TransformTypeEnum transformChange, int expectedModelType)
        {
            var analysis = SetupIPy4CTransformsHitMiss(transformChange, expectedModelType);
            //change the flaw transform to log
            analysis.InFlawTransform = transformChange;
            //Act
            analysis.UpdateRTransforms();
            //Assert
            Assert.That(analysis.Data.HMAnalysisObject.ModelType, Is.EqualTo(expectedModelType));
            Assert.That(analysis.Data.AHATAnalysisObject, Is.Null);
        }
        [Test]
        public void UpdateRTransforms_AnalysisTypeAHatNoChange_TransformsAndModelSame()
        {
            var analysis = SetupIPy4CTransformsAhat();
            SetPythonAndREngines();
            var originalModel = analysis.Data.AHATAnalysisObject.ModelType;
            //dont change transforms
            //Act
            analysis.UpdateRTransforms();
            //Assert
            Assert.That(analysis.Data.AHATAnalysisObject.ModelType, Is.EqualTo(originalModel));
            Assert.That(analysis.Data.HMAnalysisObject, Is.Null);
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
            var analysis = SetupIPy4CTransformsAhat();
            SetPythonAndREngines();
            //change the flaw transform to log
            analysis.InFlawTransform = transformChangeX;
            analysis.InResponseTransform = transformChangeY;
            //Act
            analysis.UpdateRTransforms();
            //Assert
            Assert.That(analysis.Data.AHATAnalysisObject.ModelType, Is.EqualTo(expectedModelType));
            Assert.That(analysis.Data.HMAnalysisObject, Is.Null);
        }
        private Analysis SetupIPy4CTransformsHitMiss(TransformTypeEnum testTransformX, int expectedOutput)
        {
            var analysis = AnalysisWithNoDataMock();
            analysis.AnalysisDataType = AnalysisDataTypeEnum.HitMiss;
            analysis.Data.DataType = AnalysisDataTypeEnum.HitMiss;
            _python.Setup(hitmiss => hitmiss.HitMissAnalsysis("SampleAnalysis")).Returns(new HMAnalysisObject("SampleAnalysis"));
            _python.Setup(modelType => modelType.TransformEnumToInt(testTransformX)).Returns(expectedOutput);
            analysis.SetPythonEngine(_python.Object);
            analysis.SetREngine(_rEngine.Object);
            return analysis;
        }
        private Analysis SetupIPy4CTransformsAhat()
        {
            var analysis = AnalysisWithNoDataMock();
            analysis.AnalysisDataType = AnalysisDataTypeEnum.AHat;
            analysis.Data.DataType = AnalysisDataTypeEnum.AHat;
            _python.Setup(ahat => ahat.AHatAnalysis("SampleAnalysis")).Returns(new AHatAnalysisObject("SampleAnalysis"));
            //setup all possible transforms since AHat could be either
            _python.Setup(modelType => modelType.TransformEnumToInt(TransformTypeEnum.Linear)).Returns(1);
            _python.Setup(modelType => modelType.TransformEnumToInt(TransformTypeEnum.Log)).Returns(2);
            _python.Setup(modelType => modelType.TransformEnumToInt(TransformTypeEnum.Inverse)).Returns(3);
            _python.Setup(modelType => modelType.TransformEnumToInt(TransformTypeEnum.BoxCox)).Returns(5);
            analysis.SetPythonEngine(_python.Object);
            analysis.SetREngine(_rEngine.Object);
            return analysis;
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
        /*
        [Test]
        public void WriteToExcel_PartOfProjectAndTableIsNull_WorSheetNameNotOverwrittenAndRemoveDefaultSheetNotCalled()
        {
            //Arrange
            SetUpExcelWriting();
            //Act
            _analysis.WriteToExcel(_excelOutput.Object, true);

            Assert.That(_analysis.WorksheetName, Is.EqualTo("myWorkSheet"));
            _excelOutput.Verify(e => e.RemoveDefaultSheet(), Times.Never);
        }
        */


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
            SetUpExcelWriting();
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
        private void SetUpExcelWriting()
        {
            SetPythonAndREngines();
            var spreadSheet = new SLDocument();
            _excelOutput.SetupGet(w => w.Workbook).Returns(spreadSheet);
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

        /// <summary>
        /// Test for the public double TransformValueForXAxis(double myValue) function
        /// ensure a double is always returned
        /// </summary>
        [Test]
        public void TransformValueForXAxis_AnyDoublePassed_ReturnsAValidDouble()
        {
            //Arrange
            //_python.Setup(p => p.TransformEnumToInt(transformType)).Returns();
            SetPythonAndREngines();
            var myValue = It.IsAny<double>();

            //Act
            var result = _analysis.TransformValueForXAxis(myValue);

            //Assert
            Assert.That(result, Is.Not.EqualTo(double.NaN));
        }

        /// <summary>
        /// Test for the public decimal TransformValueForXAxis(decimal myValue) function
        /// </summary>
        //The possible transforms that can go into this function is Linear, Log, and inverse
        [Test]
        [TestCase(TransformTypeEnum.Linear, 1, 2.0, 2.0)]
        [TestCase(TransformTypeEnum.Linear, 1, 0.0, 0.0)]
        [TestCase(TransformTypeEnum.Linear, 1, -2.0, -2.0)]
        [TestCase(TransformTypeEnum.Inverse, 3, 2.0, .5)]
        [TestCase(TransformTypeEnum.Inverse, 3, -2.0, -.5)]
        public void TransformValueForXAxis_NonLogtransformPassed_ReturnsValidtransform(TransformTypeEnum transform, int enumTransform, decimal myValue,  decimal expectedResult)
        {
            //Arrange
            SetPythonAndREngines();
            _analysis.InFlawTransform = transform;
            _python.Setup(e => e.TransformEnumToInt(transform)).Returns(enumTransform);
            //Act
            var result = _analysis.TransformValueForXAxis(myValue);
            Assert.That(result, Is.EqualTo(expectedResult));
            //Assert.Throws<OverflowException>(()=>_analysis.TransformValueForXAxis(-1.0M));
        }
        [Test]
        public void TransformValueForXAxis_0PassedInForInverseTransform_ThrowsOverflowExcpetionAndReturnsTheSameValue()
        {
            //Arrange
            SetPythonAndREngines();
            _analysis.InFlawTransform = TransformTypeEnum.Inverse;
            _python.Setup(e => e.TransformEnumToInt(TransformTypeEnum.Inverse)).Returns(3);
            //Act
            var result = _analysis.TransformValueForXAxis(0.0M);
            //Assert
            Assert.That(result, Is.EqualTo(0.0M));
            //Assert.That(() => _analysis.TransformValueForXAxis(0.0M), Throws.Exception.TypeOf<OverflowException>());
        }
        [Test]
        [TestCase(Math.E, 1.0)]
        [TestCase(.1, -2.303)]
        public void TransformValueForXAxis_LogTransformPassedAndValueIsPositive_ReturnsValidLogTransform(decimal inputValue, decimal expectedValue)
        {
            //Arrange
            SetPythonAndREngines();
            _analysis.InFlawTransform = TransformTypeEnum.Log;
            _python.Setup(e => e.TransformEnumToInt(TransformTypeEnum.Log)).Returns(2);
            //Act
            var result = _analysis.TransformValueForXAxis(inputValue);
            //Assert
            Assert.That(Math.Round(result, 3), Is.EqualTo(expectedValue));

        }
        [Test]
        [TestCase(0.0)]
        [TestCase(-1.0)]
        public void TransformValueForXAxis_LogTransformPassedAndValueIsNegativeOr0AndSmallestFlawIsNot0_ReturnsValidLogTransform(decimal inputValue)
        {
            //Arrange
            SetPythonAndREngines();
            _analysis.InFlawTransform = TransformTypeEnum.Log;
            _python.Setup(e => e.TransformEnumToInt(TransformTypeEnum.Log)).Returns(2);
            _data.SetupGet(sf => sf.SmallestFlaw).Returns(.1);
            //Act
            var result = _analysis.TransformValueForXAxis(inputValue);
            //Assert
            Assert.That(result, Is.EqualTo(Convert.ToDecimal(Math.Log(.1/2.0))));

        }
        [Test]
        [TestCase(0.0)]
        [TestCase(-1.0)]
        public void TransformValueForXAxis_LogTransformPassedAndValueIsNegativeOr0AndSmallestFlawIs0_Returns0(decimal inputValue)
        {
            //Arrange
            SetPythonAndREngines();
            _analysis.InFlawTransform = TransformTypeEnum.Log;
            _python.Setup(e => e.TransformEnumToInt(TransformTypeEnum.Log)).Returns(2);
            _data.SetupGet(sf => sf.SmallestFlaw).Returns(0.0);
            //Act
            var result = _analysis.TransformValueForXAxis(inputValue);
            //Assert
            Assert.That(result, Is.EqualTo(0.0M));
            //Assert.That(() => _analysis.TransformValueForXAxis(inputValue), Throws.Exception.TypeOf<OverflowException>());
        }

        /// <summary>
        /// Test for the public double TransformValueForYAxis(double myValue) function
        /// ensure a double is always returned
        /// </summary>
        [Test]
        public void TransformValueForYAxis_AnyDoublePassed_ReturnsAValidDouble()
        {
            //Arrange
            //_python.Setup(p => p.TransformEnumToInt(transformType)).Returns();
            SetPythonAndREngines();
            var myValue = It.IsAny<double>();

            //Act
            var result = _analysis.TransformValueForYAxis(myValue);

            //Assert
            Assert.That(result, Is.Not.EqualTo(double.NaN));
        }

        /// <summary>
        /// Test for the public decimal TransformValueForYAxis(decimal myValue) function
        /// </summary>
        //The possible transforms that can go into this function is Linear, Log, BoxCox, and inverse
        [Test]
        [TestCase(TransformTypeEnum.Linear, 1, 2.0, 2.0)]
        [TestCase(TransformTypeEnum.Linear, 1, 0.0, 0.0)]
        [TestCase(TransformTypeEnum.Linear, 1, -2.0, -2.0)]
        [TestCase(TransformTypeEnum.Inverse, 3, 2.0, .5)]
        [TestCase(TransformTypeEnum.Inverse, 3, -2.0, -.5)]
        public void TransformValueForYAxis_NonLogOrBoxCoxtransformPassed_ReturnsValidtransform(TransformTypeEnum transform, int enumTransform, decimal myValue, decimal expectedResult)
        {
            //Arrange
            SetPythonAndREngines();
            _analysis.InResponseTransform = transform;
            _python.Setup(e => e.TransformEnumToInt(transform)).Returns(enumTransform);
            //Act
            var result = _analysis.TransformValueForYAxis(myValue);
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void TransformValueForYAxis_0PassedInForInverseTransform_ThrowsOverflowExcpetionAndReturnsTheSameValue()
        {
            //Arrange
            SetPythonAndREngines();
            _analysis.InResponseTransform = TransformTypeEnum.Inverse;
            _python.Setup(e => e.TransformEnumToInt(TransformTypeEnum.Inverse)).Returns(3);
            //Act
            var result = _analysis.TransformValueForYAxis(0.0M);
            //Assert
            Assert.That(result, Is.EqualTo(0.0M));
            //Assert.That(() => _analysis.TransformValueForXAxis(0.0M), Throws.Exception.TypeOf<OverflowException>());
        }
        [Test]
        [TestCase(Math.E, 1.0)]
        [TestCase(.1, -2.303)]
        public void TransformValueForYAxis_LogTransformPassedAndValueIsPositive_ReturnsValidLogTransform(decimal inputValue, decimal expectedValue)
        {
            //Arrange
            SetPythonAndREngines();
            _analysis.InResponseTransform = TransformTypeEnum.Log;
            _python.Setup(e => e.TransformEnumToInt(TransformTypeEnum.Log)).Returns(2);
            //Act
            var result = _analysis.TransformValueForYAxis(inputValue);
            //Assert
            Assert.That(Math.Round(result, 3), Is.EqualTo(expectedValue));

        }
        [Test]
        [TestCase(0.0)]
        [TestCase(-1.0)]
        public void TransformValueForYAxis_LogTransformPassedAndValueIsNegativeOr0AndSmallestFlawIsNot0_ReturnsValidLogTransform(decimal inputValue)
        {
            //Arrange
            SetPythonAndREngines();
            _analysis.InResponseTransform = TransformTypeEnum.Log;
            _python.Setup(e => e.TransformEnumToInt(TransformTypeEnum.Log)).Returns(2);
            _data.SetupGet(sf => sf.SmallestResponse).Returns(.1);
            //Act
            var result = _analysis.TransformValueForYAxis(inputValue);
            //Assert
            Assert.That(result, Is.EqualTo(Convert.ToDecimal(Math.Log(.1 / 2.0))));

        }
        [Test]
        [TestCase(0.0)]
        [TestCase(-1.0)]
        public void TransformValueForYAxis_LogTransformPassedAndValueIsNegativeOr0AndSmallestFlawIs0_Returns0(decimal inputValue)
        {
            //Arrange
            SetPythonAndREngines();
            _analysis.InResponseTransform = TransformTypeEnum.Log;
            _python.Setup(e => e.TransformEnumToInt(TransformTypeEnum.Log)).Returns(2);
            _data.SetupGet(sf => sf.SmallestResponse).Returns(0.0);
            //Act
            var result = _analysis.TransformValueForYAxis(inputValue);
            //Assert
            Assert.That(result, Is.EqualTo(0.0M));
            //Assert.That(() => _analysis.TransformValueForXAxis(inputValue), Throws.Exception.TypeOf<OverflowException>());
        }
        //Testing for a postive and negative value for lambda
        [Test]
        [TestCase(.5, 16.0, 6.0)]
        [TestCase(-.5, 16.0, 1.5)]
        public void TransformValueForYAxis_BoxCoxTransformPassedAndValueIsPositive_ReturnsValidBoxCoxTransform(double lambdaValue, decimal myValue, decimal ExpectedOutput)
        {
            //Arrange
            SetPythonAndREngines();
            _analysis.InResponseTransform = TransformTypeEnum.BoxCox;
            _python.Setup(e => e.TransformEnumToInt(TransformTypeEnum.BoxCox)).Returns(5);
            _data.SetupGet(sf => sf.AHATAnalysisObject).Returns(new AHatAnalysisObject("SampleAnalysis"));
            _analysis.InLambdaValue = lambdaValue;
            //Act
            var result = _analysis.TransformValueForYAxis(myValue);
            //Assert
            Assert.That(result, Is.EqualTo(ExpectedOutput));
        }
        [Test]
        [TestCase(.5, -16.0, -1.0)]
        [TestCase(-.5, -16.0, 0.0)]
        public void TransformValueForYAxis_BoxCoxTransformPassedAndValueIsPositive_ReturnsNegative1ForPositiveLambdaAndZeroForNegativeLambdas(double lambdaValue, decimal myValue, decimal ExpectedOutput)
        {
            //Arrange
            SetPythonAndREngines();
            _analysis.InResponseTransform = TransformTypeEnum.BoxCox;
            _python.Setup(e => e.TransformEnumToInt(TransformTypeEnum.BoxCox)).Returns(5);
            _data.SetupGet(sf => sf.AHATAnalysisObject).Returns(new AHatAnalysisObject("SampleAnalysis"));
            _analysis.InLambdaValue = lambdaValue;
            //Act
            var result = _analysis.TransformValueForYAxis(myValue);
            //Assert
            Assert.That(result, Is.EqualTo(ExpectedOutput));
        }


        /// <summary>
        /// Test for the public double InvertTransformValueForXAxis(double myValue) function
        /// ensure a double is always returned
        /// </summary>
        [Test]
        public void InvertTransformValueForXAxis_AnyDoublePassed_ReturnsAValidDouble()
        {
            //Arrange
            SetPythonAndREngines();
            var myValue = It.IsAny<double>();

            //Act
            var result = _analysis.InvertTransformValueForXAxis(myValue);

            //Assert
            Assert.That(result, Is.Not.EqualTo(double.NaN));
        }

        /// <summary>
        /// Test for the public double TransformValueForYAxis(decimal myValue) function
        /// </summary>
        [Test]
        [TestCase(2, 0.0, 1.0)]
        public void InvertTransformValueForXAxis_ValidValuePassedAndPythonNotNull_ReturnsValidtransform(int enumTransform, decimal myValue, decimal expectedResult)
        {
            //Arrange
            SetPythonAndREngines();
            _analysis.InFlawTransform = TransformTypeEnum.Log;
            _python.Setup(e => e.TransformEnumToInt(TransformTypeEnum.Log)).Returns(enumTransform);
            _data.Setup(transB => transB.TransformBackAValue(Convert.ToDouble(myValue), enumTransform)).Returns(Convert.ToDouble(expectedResult));
            //Act
            var result = _analysis.InvertTransformValueForXAxis(myValue);
            //Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }
        [Test]
        [TestCase(-1.0)]
        [TestCase(0.0)]
        [TestCase(1.0)]
        public void InvertTransformValueForXAxis_ValidValuePassedPythonIsNull_ReturnsTheSameValue(decimal myValue)
        {
            _analysis.InFlawTransform = TransformTypeEnum.Log;
            _python.Setup(e => e.TransformEnumToInt(TransformTypeEnum.Log)).Returns(2);
            _data.Setup(transB => transB.TransformBackAValue(Convert.ToDouble(myValue), 2)).Returns(Convert.ToDouble(2.0));
            //Act
            var result = _analysis.InvertTransformValueForXAxis(myValue);
            //Assert
            Assert.That(result, Is.EqualTo(myValue));
        }
        [Test]
        [TestCase(1.0)]
        public void InvertTransformValueForXAxis_ThrowsException_ReturnsTheSameValue(decimal myValue)
        {
            SetupTransformBackFlaws();
            _data.Setup(transB => transB.TransformBackAValue(Convert.ToDouble(myValue), 2)).Throws<Exception>();
            //Act
            var result = _analysis.InvertTransformValueForXAxis(myValue);
            //Assert
            Assert.That(result, Is.EqualTo(myValue));
        }
        private void SetupTransformBackFlaws()
        {
            SetPythonAndREngines();
            _analysis.InFlawTransform = TransformTypeEnum.Log;
            _python.Setup(e => e.TransformEnumToInt(TransformTypeEnum.Log)).Returns(2);
        }

        /// <summary>
        /// Test for the public decimal TransformValueForYAxis(decimal myValue) function
        /// </summary>
        [Test]
        [TestCase(2, 0.0, 1.0)]
        public void InvertTransformValueForYAxis_ValidValuePassedAndPythonNotNull_ReturnsValidtransform(int enumTransform, decimal myValue, decimal expectedResult)
        {
            //Arrange
            SetPythonAndREngines();
            _analysis.InResponseTransform = TransformTypeEnum.Log;
            _python.Setup(e => e.TransformEnumToInt(TransformTypeEnum.Log)).Returns(enumTransform);
            _data.Setup(transB => transB.TransformBackAValue(Convert.ToDouble(myValue), enumTransform)).Returns(Convert.ToDouble(expectedResult));
            //_python.Setup(e => e).Returns(new IPy4C());
            //Act
            var result = _analysis.InvertTransformValueForYAxis(myValue);
            //Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }
        [Test]
        [TestCase(-1.0)]
        [TestCase(0.0)]
        [TestCase(1.0)]
        public void InvertTransformValueForYAxis_ValidValuePassedPythonIsNull_ReturnsTheSameValue(decimal myValue)
        {
            _analysis.InResponseTransform = TransformTypeEnum.Log;
            _python.Setup(e => e.TransformEnumToInt(TransformTypeEnum.Log)).Returns(2);
            _data.Setup(transB => transB.TransformBackAValue(Convert.ToDouble(myValue), 2)).Returns(Convert.ToDouble(2.0));
            //Act
            var result = _analysis.InvertTransformValueForYAxis(myValue);
            //Assert
            Assert.That(result, Is.EqualTo(myValue));
        }
        [Test]
        [TestCase(1.0)]
        public void InvertTransformValueForYAxis_ThrowsException_ReturnsTheSameValue(decimal myValue)
        {
            SetupTransformBackResponse();
            _data.Setup(transB => transB.TransformBackAValue(Convert.ToDouble(myValue), 2)).Throws<Exception>();
            //Act
            var result = _analysis.InvertTransformValueForYAxis(myValue);
            //Assert
            Assert.That(result, Is.EqualTo(myValue));
        }
        private void SetupTransformBackResponse()
        {
            SetPythonAndREngines();
            _analysis.InResponseTransform = TransformTypeEnum.Log;
            _python.Setup(e => e.TransformEnumToInt(TransformTypeEnum.Log)).Returns(2);
        }

        /// <summary>
        /// Test for the public void UpdateRangesFromData() function
        /// </summary>
        [Test]
        public void UpdateRangesFromData_HasBeenInitializedTrue_GetUpdatedValueFunctionFired()
        {
            //Arrange
            _analysis.HasBeenInitialized = true;
            //Act
            _analysis.UpdateRangesFromData();
            //Assert
            var tempValue = 0.0;
            _data.Verify(guv => guv.GetUpdatedValue(ColType.Flaw, ExtColProperty.Max, It.IsAny<double>(), out tempValue), Times.Once);
            _data.Verify(guv => guv.GetUpdatedValue(ColType.Flaw, ExtColProperty.Min, It.IsAny<double>(), out tempValue), Times.Once);
            _data.Verify(guv => guv.GetUpdatedValue(ColType.Response, ExtColProperty.Max, It.IsAny<double>(), out tempValue), Times.Once);
            _data.Verify(guv => guv.GetUpdatedValue(ColType.Response, ExtColProperty.Min, It.IsAny<double>(), out tempValue), Times.Once);
            _data.Verify(guv => guv.GetUpdatedValue(ColType.Response, ExtColProperty.Thresh, It.IsAny<double>(), out tempValue), Times.Once);
        }
        [Test]
        public void UpdateRangesFromData_HasBeenInitializedFalse_GetNewValueFunctionFired()
        {
            //Arrange
            _analysis.HasBeenInitialized = false;
            //Act
            _analysis.UpdateRangesFromData();
            //Assert
            var tempValue = 0.0;
            _data.Verify(gnv => gnv.GetNewValue(ColType.Flaw, ExtColProperty.Max, out tempValue), Times.Once);
            _data.Verify(gnv => gnv.GetNewValue(ColType.Flaw, ExtColProperty.Min, out tempValue), Times.Once);
            _data.Verify(gnv => gnv.GetNewValue(ColType.Response, ExtColProperty.Max, out tempValue), Times.Once);
            _data.Verify(gnv => gnv.GetNewValue(ColType.Response, ExtColProperty.Min, out tempValue), Times.Once);
            _data.Verify(gnv => gnv.GetNewValue(ColType.Response, ExtColProperty.Thresh, out tempValue), Times.Once);
        }

    }
}
