using NUnit.Framework;
using System;
using Moq;
using POD.Data;
using System.Collections.Generic;
using System.Data;
using POD;
using CSharpBackendWithR;
using POD.ExcelData;

namespace Data.UnitTests
{
    [TestFixture]
    public class AnalysisDataTests
    {
        private AnalysisData _data;
        private DataSource _source;
        private DataTable _table;
        Mock<IExcelWriterControl> _excelWriterControl;
        Mock<IExcelExport> _excelExport;
        private Mock<I_IPy4C> _python;
        [SetUp]
        public void Setup()
        {

            _data = new AnalysisData();
            _source = new DataSource("MyDataSource", "ID", "flawName.centimeters", "Response");
            _table = new DataTable();
            _excelWriterControl = new Mock<IExcelWriterControl>();
            _excelExport = new Mock<IExcelExport>();
            GenerateSampleTable();

        }
        private void GenerateSampleTable()
        {
            List<int> list1 = new List<int>();
            List<int> list2 = new List<int>();
            List<int> list3 = new List<int>();
            _table.Columns.Add("Column1");
            _table.Columns.Add("Column2");
            _table.Columns.Add("Column3");
        }
        /// <summary>
        /// tests ActivatedFlawName Getter
        /// </summary>
        [Test]
        public void ActivatedFlawName_ActivatedFlawCountIsZero_ReturnsEmptyString()
        {
            //Arrange
            //Act
            var result = _data.ActivatedFlawName;
            //Assert
            Assert.AreEqual(result, string.Empty);
        }
        [Test]
        public void ActivatedFlawName_ActivatedFlawCountGreaterThanZero_ReturnsActivatedFlawName()
        {
            //Arrange
            _data.SetSource(_source);
            //Act
            var result = _data.ActivatedFlawName;
            //Assert
            Assert.AreEqual(result, "flawName.centimeters");
        }
        /// <summary>
        /// tests ActivatedOriginalFlawName Getter
        /// </summary>
        [Test]
        public void ActivatedOriginalFlawName_NamesCountIsZero_ReturnsEmptyString()
        {
            //Arrange
            //Act
            var result = _data.ActivatedOriginalFlawName;
            //Assert
            Assert.AreEqual(result, string.Empty);
        }
        [Test]
        public void ActivatedOriginalFlawName_NamesCountGreaterThanZero_ReturnsActivatedFlawName()
        {
            //Arrange
            _data.SetSource(_source);
            _data.ActivateFlaw("flawName.centimeters");
            //_data.ActivateFlaws(new List<string>());
            //Act
            var result = _data.ActivatedOriginalFlawName;
            //Assert
            Assert.AreEqual(result, "flawName.centimeters");
        }
        /// Tests for GetRemovedPointComment(int myColIndex, int myRowIndex) function
        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void GetRemovedPointComment_ColumnIndexIsNotThere_AddsToTheDictionaryAndReturnsEmptyString(int columnIndex)
        {
            //Arrange
            var _ = _data.CommentDictionary;
            var originalCount = _data.CommentDictionary.Count;
            //Act 
            var result = _data.GetRemovedPointComment(columnIndex, 1);
            //Assert
            Assert.AreEqual(result, string.Empty);
            Assert.IsTrue(_data.CommentDictionary.ContainsKey(columnIndex));
            Assert.AreEqual(_data.CommentDictionary.Count, originalCount + 1);
        }
        /// Tests for GetRemovedPointComment(int myColIndex, int myRowIndex) function
        [Test]
        [TestCase(1, 4)]
        [TestCase(2, 5)]
        [TestCase(3, 6)]
        public void GetRemovedPointComment_ColumnIndexIsThereButRowIsNot_AddsToTheDictionaryAndReturnsEmptyString(int columnIndex, int rowIndex)
        {
            //Arrange
            var _ = _data.CommentDictionary;
            //Alter the row index so that the dictionary contains the column, but not the row
            _data.CommentDictionary.Add(columnIndex, new Dictionary<int, string>() { { rowIndex + 1, "" } });
            var originalCount = _data.CommentDictionary.Count;
            var originalColIndexCount = _data.CommentDictionary[columnIndex].Count;
            //Act 
            var result = _data.GetRemovedPointComment(columnIndex, rowIndex);
            //Assert
            Assert.AreEqual(result, string.Empty);
            Assert.IsTrue(_data.CommentDictionary.ContainsKey(columnIndex));
            Assert.IsTrue(_data.CommentDictionary[columnIndex].ContainsKey(rowIndex));
            // Dictionary size stays the same, but the col index dictionary gets bigger
            Assert.AreEqual(_data.CommentDictionary.Count, originalCount);
            Assert.AreEqual(_data.CommentDictionary[columnIndex].Count, originalColIndexCount + 1);
        }
        [Test]
        [TestCase(1, 4)]
        [TestCase(2, 5)]
        [TestCase(3, 6)]
        public void GetRemovedPointComment_ColumnAndRowIndexAreBothThere_ReturnsTheRemovedPointComment(int columnIndex, int rowIndex)
        {
            //Arrange
            var _ = _data.CommentDictionary;
            //Alter the row index so that the dictionary contains the column, but not the row
            _data.CommentDictionary.Add(columnIndex, new Dictionary<int, string>() { { rowIndex, "MyRemovedPointComment" } });
            var originalCount = _data.CommentDictionary.Count;
            var originalColIndexCount = _data.CommentDictionary[columnIndex].Count;
            //Act 
            var result = _data.GetRemovedPointComment(columnIndex, rowIndex);
            Assert.AreEqual(_data.CommentDictionary.Count, originalCount);
            Assert.AreEqual(_data.CommentDictionary[columnIndex].Count, originalColIndexCount);
        }
        /// Tests for SetRemovedPointComment(int myColIndex, int myRowIndex, string myComment) function
        [Test]
        public void SetRemovedPointComment_ValidCommentPassed_AssignsTheValueInTheDictionary()
        {
            //Arrange
            var _ = _data.CommentDictionary;
            _data.CommentDictionary.Add(1, new Dictionary<int, string>() { { 1, "MyRemovedPointComment" } });
            //Act
            _data.SetRemovedPointComment(1, 1, "MyNewRemovePointComment");
            //Assert
            Assert.AreEqual(_data.CommentDictionary[1][1], "MyNewRemovePointComment");
        }
        /*
         * TODO: figure out why the duplicate table tests are being passed by reference
         * It's possible this function may not even be necessary
        /// Tests For DuplicateTable(DataTable fromTable, DataTable toTable)
        [Test]
        public void DuplicateTable_FromTableIsNull_ToTableBecomesNull()
        {
            //Arrange
            DataTable fromTable = null;
            DataTable toTable = new DataTable();
            //Act
            AnalysisData.DuplicateTable(fromTable, toTable);
            //Assert
            Assert.IsNull(toTable);
        }
        [Test]
        public void DuplicateTable_FromTableIsNotNullAndToTableIsNull_TableDuplicated()
        {
            //Arrange
            DataTable fromTable = new DataTable();
            DataTable toTable = null;
            //Act
            AnalysisData.DuplicateTable(fromTable, toTable);
            //Assert
            Assert.IsNotNull(toTable);
        }
        [Test]
        public void DuplicateTable_fromTableAndToTableBothNotNull_TableDuplicated()
        {
            //Arrange
            DataTable fromTable = _table;
            DataTable toTable = new DataTable();
            //Act
            AnalysisData.DuplicateTable(fromTable, toTable);
            //Assert
            Assert.AreEqual(fromTable, toTable);
            Assert.IsFalse(ReferenceEquals(fromTable, toTable));
        }
        */
        ///Tests for the SetSource(DataSource mySource, List<string> myFlaws, List<string> myMetaDatas,
        ///List<string> myResponses, List<string> mySpecIDs)
        /// function
        [Test]
        public void SetSource_OriginalRowsCountIsZero_ColumnsAddedToQuickTable()
        {
            //Arrange
            SetUpFakeData(out List<string> myFlaws, out List<string> myMetaDatas, out List<string> myResponses, out List<string> mySpecIDs);
            //Act
            _data.SetSource(_source, myFlaws, myMetaDatas, myResponses, mySpecIDs);
            //Assert
            Assert.AreEqual(_data.QuickTable.Columns.Count, 3);
        }
        [Test]
        public void SetSource_OriginalRowsCountIsNotZeroZero_ColumnsNotToQuickTable()
        {
            //Arrange
            _source.Original.Rows.Add(new List<string> { "1" });
            SetUpFakeData(out List<string> myFlaws, out List<string> myMetaDatas, out List<string> myResponses, out List<string> mySpecIDs);
            //Act
            _data.SetSource(_source, myFlaws, myMetaDatas, myResponses, mySpecIDs);
            //Assert
            Assert.That(_data.QuickTable.Columns.Count, Is.Zero);
        }
        [Test]
        public void SetSource_DataTypeIsNothitMiss_FlawTransformSetToLinear()
        {
            //Arrange
            var ahatTable = CreateSampleDataTable();
            for (int i = 0; i < 10; i++)
                ahatTable.Rows.Add(i, i * .25, i + .1);
            DataSource sourceWithActualData = SetupSampleDataSource(ahatTable);
            SetUpFakeData(out List<string> myFlaws, out List<string> myMetaDatas, out List<string> myResponses, out List<string> mySpecIDs);
            //Act
            _data.SetSource(sourceWithActualData, myFlaws, myMetaDatas, myResponses, mySpecIDs);
            //Assert
            Assert.That(_data.FlawTransform, Is.EqualTo(TransformTypeEnum.Linear));
        }
        [Test]
        public void SetSource_DataTypeIshitMiss_FlawTransformSetToLog()
        {
            //Arrange
            var hitmissTable = CreateSampleDataTable();
            for (int i = 0; i < 10; i++)
                hitmissTable.Rows.Add(i, i * .25, i % 2);
            DataSource sourceWithActualData = SetupSampleDataSource(hitmissTable);
            SetUpFakeData(out List<string> myFlaws, out List<string> myMetaDatas, out List<string> myResponses, out List<string> mySpecIDs);
            //Act
            _data.SetSource(sourceWithActualData, myFlaws, myMetaDatas, myResponses, mySpecIDs);
            //Assert
            Assert.That(_data.FlawTransform, Is.EqualTo(TransformTypeEnum.Log));
        }
        private DataTable CreateSampleDataTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("ID");
            table.Columns.Add("flawName.centimeters");
            table.Columns.Add("Response");
            return table;
        }
        private void SetUpFakeData(out List<string> myFlaws, out List<string> myMetaDatas,
            out List<string> myResponses, out List<string> mySpecIDs)
        {
            myFlaws = new List<string> { "flawName.centimeters" };
            myMetaDatas = new List<string>();
            myResponses = new List<string> { "Response" };
            mySpecIDs = new List<string> { "ID" };
        }
        private DataSource SetupSampleDataSource(DataTable table)
        {
            return new DataSource(table,
                new TableRange(RangeNames.SpecID) { Range = new List<int> { 0 }, Count = 1, StartIndex = 0, MaxIndex = 0 },
                new TableRange(RangeNames.MetaData),
                new TableRange(RangeNames.FlawSize) { Range = new List<int> { 1 }, Count = 1, StartIndex = 1, MaxIndex = 1 },
                 new TableRange(RangeNames.Response) { Range = new List<int> { 2 }, Count = 1, StartIndex = 2, MaxIndex = 2 });
        }
        /*
        /// <summary>
        /// Tests for the TurnAllPointsOn() function
        /// </summary>
        [Test]
        public void TurnAllPointsOn_TurnedOffPointsExistInTheData_AllPointsAreTurnedOnAndListBecomesEmpty()
        {
            //Arrange
            _data.TurnedOffPoints.Add(new DataPointIndex(1,1,"first"));
            _data.TurnedOffPoints.Add(new DataPointIndex(1, 2, "second"));
            _data.TurnedOffPoints.Add(new DataPointIndex(1, 3, "third"));
            //Act
            _data.TurnAllPointsOn();
            //Assert
            Assert.That(_data.TurnedOffPoints.Count, Is.Zero);
        }
        */
        /// <summary>
        /// Tests for the TurnOffPoint() function
        /// </summary>
        /// can't really test this one either 

        /// Tests for UpdateData(bool quickFlag = false)
        [Test]
        public void UpdateData_PythonIsNull_DataNotUpdated()
        {
            //Arrange
            _data.SetPythonEngine(null, "Name");
            SetupHitMissAnalysisObject(new List<double> { .1, .2, .3 },
                new Dictionary<string, List<double>> { { "Responses", new List<double>() { 0.0, 0.0, 1.0 } } });
            SetupAHatAnalysisObject(new List<double> { .1, .2, .3 },
                new Dictionary<string, List<double>> { { "Responses", new List<double>() { 1.0, 2.0, 3.0 } } });
            //Act
            _data.UpdateData();
            //Assert
            AssertFlawResponseDataNotUpdated();
        }
        [Test]
        public void UpdateData_PythonNotNullAndBothAnalysisObjectsAreNull_DataNotUpdated()
        {
            //Arrange
            _data.SetPythonEngine(new Mock<I_IPy4C>().Object, "Name");
            //Act
            Assert.DoesNotThrow(() => _data.UpdateData());
            _data.UpdateData();
            //Assert
            Assert.That(_data.HMAnalysisObject, Is.Null);
            Assert.That(_data.AHATAnalysisObject, Is.Null);
        }
        [Test]
        [TestCase(AnalysisDataTypeEnum.None)]
        [TestCase(AnalysisDataTypeEnum.Undefined)]
        public void UpdateData_PythonNotNullAndAHatAnalysisNotNullAndDataTypeIsInvalid_OnlyDataColumnNamesUpdated(AnalysisDataTypeEnum dataType)
        {
            //Arrange
            SetupActivationDataSignalResponse();
            SetupAHatAnalysisObject(new List<double>() { 1.0 },
                new Dictionary<string, List<double>>() { { "FakeResponse", new List<double>() { 1.0, 2.0, 3.0 } } });
            _data.DataType = dataType;
            //Act
            _data.UpdateData();
            //Assert
            Assert.That(_data.AHATAnalysisObject.SignalResponseName, Is.Not.EqualTo("Response"));
            Assert.That(_data.AHATAnalysisObject.Flaws.Count, Is.Zero);
            Assert.That(_data.AHATAnalysisObject.Flaws_All.Count, Is.EqualTo(1));
            Assert.That(_data.AHATAnalysisObject.Responses.Count, Is.Zero);
            Assert.That(_data.AHATAnalysisObject.Responses_all.Count, Is.EqualTo(1));
        }
        [Test]
        [TestCase(AnalysisDataTypeEnum.None)]
        [TestCase(AnalysisDataTypeEnum.Undefined)]
        public void UpdateData_PythonNotNullAndHitMissAnalysisNotNullAndDataTypeIsInvalid_OnlyDataColumnNamesUpdated(AnalysisDataTypeEnum dataType)
        {
            //Arrange
            SetupActivationDataHitMiss();
            SetupHitMissAnalysisObject(new List<double>() { 1.0 },
                new Dictionary<string, List<double>>() { { "FakeResponse", new List<double>() { 0.0, 0.0, 1.0 } } });
            _data.DataType = dataType;
            //Act
            _data.UpdateData();
            //Assert
            Assert.That(_data.HMAnalysisObject.HitMiss_name, Is.Not.EqualTo("Response"));
            Assert.That(_data.HMAnalysisObject.Flaws.Count, Is.Zero);
            Assert.That(_data.HMAnalysisObject.Flaws_All.Count, Is.EqualTo(1));
            Assert.That(_data.HMAnalysisObject.Responses.Count, Is.Zero);
            Assert.That(_data.HMAnalysisObject.Responses_all.Count, Is.EqualTo(1));
        }
        [Test]
        public void UpdateData_AHatAnalysisNotNullButFlawsAllAndResponsesAllNotZeroAndQuickFlagFalse_OnlyDataColumnNamesUpdated()
        {
            //Arrange
            SetupActivationDataSignalResponse();
            SetupAHatAnalysisObject(new List<double>() { 1.0 },
                new Dictionary<string, List<double>>() { { "FakeResponse", new List<double>() { 1.0, 2.0, 3.0 } } });
            //Act
            _data.UpdateData();
            //Assert
            Assert.That(_data.AHATAnalysisObject.SignalResponseName, Is.EqualTo("Response"));
            Assert.That(_data.AHATAnalysisObject.Flaws.Count, Is.GreaterThan(1));
            Assert.That(_data.AHATAnalysisObject.Flaws_All.Count, Is.EqualTo(1));
            Assert.AreNotEqual(_data.AHATAnalysisObject.Responses, _data.AHATAnalysisObject.Responses_all);
        }
        [Test]
        public void UpdateData_AHatAnalysisNotNullButFlawsAllAndResponsesAllNotZeroAndQuickFlagTrue_DataColumnNamesFlawsAndResponsesUpdated()
        {
            //Arrange
            SetupActivationDataSignalResponse();
            SetupAHatAnalysisObject(new List<double>() { 1.0 },
                new Dictionary<string, List<double>>() { { "FakeResponse", new List<double>() { 1.0, 2.0, 3.0 } } });
            //Act
            _data.UpdateData(true);
            //Assert
            Assert.That(_data.AHATAnalysisObject.SignalResponseName, Is.EqualTo("Response"));
            Assert.AreEqual(_data.AHATAnalysisObject.Flaws, _data.AHATAnalysisObject.Flaws_All);
            Assert.AreEqual(_data.AHATAnalysisObject.Responses, _data.AHATAnalysisObject.Responses_all);
        }
        [Test]
        public void UpdateData_HMAnalysisNotNullButFlawsAllAndResponsesAllNotZeroAndQuickFlagFalse_OnlyDataColumnNamesUpdated()
        {
            //Arrange
            SetupActivationDataHitMiss();
            SetupHitMissAnalysisObject(new List<double>() { 1.0 },
                new Dictionary<string, List<double>>() { { "FakeResponse", new List<double>() { 0.0, 0.0, 1.0 } } });
            //Act
            _data.UpdateData();
            //Assert
            Assert.That(_data.HMAnalysisObject.HitMiss_name, Is.EqualTo("Response"));
            Assert.That(_data.HMAnalysisObject.Flaws.Count, Is.GreaterThan(1));
            Assert.That(_data.HMAnalysisObject.Flaws_All.Count, Is.EqualTo(1));
            Assert.AreNotEqual(_data.HMAnalysisObject.Responses, _data.HMAnalysisObject.Responses_all);
        }
        [Test]
        public void UpdateData_HMAnalysisNotNullButFlawsAllAndResponsesAllNotZeroAndQuickFlagTrue_DataColumnNamesFlawsAndResponsesUpdated()
        {
            //Arrange
            SetupActivationDataHitMiss();
            SetupHitMissAnalysisObject(new List<double>() { 1.0 },
                new Dictionary<string, List<double>>() { { "FakeResponse", new List<double>() { 0.0, 0.0, 1.0 } } });
            //Act
            _data.UpdateData(true);
            //Assert
            Assert.That(_data.HMAnalysisObject.HitMiss_name, Is.EqualTo("Response"));
            Assert.AreEqual(_data.HMAnalysisObject.Flaws, _data.HMAnalysisObject.Flaws_All);
            Assert.AreEqual(_data.HMAnalysisObject.Responses, _data.HMAnalysisObject.Responses_all);
        }
        [Test]
        public void UpdateData_AHatAnalysisNotNullAndFlawsAllAreZeroAndResponsesAreNOTZero_DataColumnNamesAndFlawsUpdated()
        {
            //Arrange
            SetupActivationDataSignalResponse();
            SetupAHatAnalysisObject(new List<double>(),
                new Dictionary<string, List<double>>() { { "FakeResponse", new List<double>() { 1.0, 2.0, 3.0 } } });
            //Act
            _data.UpdateData();
            //Assert
            Assert.That(_data.AHATAnalysisObject.SignalResponseName, Is.EqualTo("Response"));
            Assert.AreEqual(_data.AHATAnalysisObject.Flaws, _data.AHATAnalysisObject.Flaws_All);
            Assert.AreNotEqual(_data.AHATAnalysisObject.Responses, _data.AHATAnalysisObject.Responses_all);
        }
        [Test]
        public void UpdateData_AHatAnalysisNotNullAndFlawsAllAreNotZeroAndResponsesAreZero_DataColumnNamesAndResponsesUpdated()
        {
            //Arrange
            SetupActivationDataSignalResponse();
            SetupAHatAnalysisObject(new List<double>() { -1.0 }, new Dictionary<string, List<double>>());
            //Act
            _data.UpdateData();
            //Assert
            Assert.That(_data.AHATAnalysisObject.SignalResponseName, Is.EqualTo("Response"));
            Assert.AreNotEqual(_data.AHATAnalysisObject.Flaws, _data.AHATAnalysisObject.Flaws_All);
            Assert.AreEqual(_data.AHATAnalysisObject.Responses, _data.AHATAnalysisObject.Responses_all);
        }
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void UpdateData_AHatAnalysisNotNullAndFlawsAllAreZeroANDResponsesAreZero_DataColumnNamesAndResponsesUpdated(bool quickFlag)
        {
            //Arrange
            SetupActivationDataSignalResponse();
            SetupAHatAnalysisObject(new List<double>(), new Dictionary<string, List<double>>());
            //Act
            _data.UpdateData(quickFlag);
            //Assert
            Assert.That(_data.AHATAnalysisObject.SignalResponseName, Is.EqualTo("Response"));
            Assert.AreEqual(_data.AHATAnalysisObject.Flaws, _data.AHATAnalysisObject.Flaws_All);
            Assert.AreEqual(_data.AHATAnalysisObject.Responses, _data.AHATAnalysisObject.Responses_all);
        }
        [Test]
        public void UpdateData_HitMissAnalysisNotNullAndFlawsAllZeroResponsesAreNOTZero_DataColumnNamesAndFlawsUpdated()
        {
            //Arrange
            SetupActivationDataHitMiss();
            SetupHitMissAnalysisObject(new List<double>(), new Dictionary<string, List<double>>() { { "FakeResponse", new List<double>() } });
            //Act
            _data.UpdateData();
            //Assert
            Assert.That(_data.HMAnalysisObject.HitMiss_name, Is.EqualTo("Response"));
            Assert.AreEqual(_data.HMAnalysisObject.Flaws, _data.HMAnalysisObject.Flaws_All);
            Assert.AreNotEqual(_data.HMAnalysisObject.Responses, _data.HMAnalysisObject.Responses_all);
        }
        [Test]
        public void UpdateData_HitMissAnalysisNotNullAndFlawsAllNOTZeroResponsesAreZero_DataColumnNamesAndResponsesUpdated()
        {
            //Arrange
            SetupActivationDataHitMiss();
            SetupHitMissAnalysisObject(new List<double>() { -1.0 }, new Dictionary<string, List<double>>());
            //Act
            _data.UpdateData();
            //Assert
            Assert.That(_data.HMAnalysisObject.HitMiss_name, Is.EqualTo("Response"));
            Assert.AreNotEqual(_data.HMAnalysisObject.Flaws, _data.HMAnalysisObject.Flaws_All);
            Assert.AreEqual(_data.HMAnalysisObject.Responses, _data.HMAnalysisObject.Responses_all);
        }
        // Quick flag doesn't matter in this case
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void UpdateData_HitMissAnalysisNotNullAndFlawsAllAreZeroAndResponsesAreZero_DataColumnNamesAndFlawsResponsesUpdated(bool quickFlag)
        {
            //Arrange
            SetupActivationDataHitMiss();
            SetupHitMissAnalysisObject(new List<double>(), new Dictionary<string, List<double>>());
            //Act
            _data.UpdateData(quickFlag);
            //Assert
            Assert.That(_data.HMAnalysisObject.HitMiss_name, Is.EqualTo("Response"));
            Assert.AreEqual(_data.HMAnalysisObject.Flaws, _data.HMAnalysisObject.Flaws_All);
            Assert.AreEqual(_data.HMAnalysisObject.Responses, _data.HMAnalysisObject.Responses_all);
        }
        private void SetupAHatAnalysisObject(List<double> flawsAll, Dictionary<string, List<double>> responsesAll)
        {
            _data.AHATAnalysisObject = new AHatAnalysisObject("AHat Analysis")
            {
                Flaws_All = flawsAll,
                Responses_all = responsesAll
            };
        }
        private void SetupHitMissAnalysisObject(List<double> flawsAll, Dictionary<string, List<double>> responsesAll)
        {
            _data.HMAnalysisObject = new HMAnalysisObject("HitMissAnalysis")
            {
                Flaws_All = flawsAll,
                Responses_all = responsesAll
            };
        }
        private void SetupActivationDataHitMiss()
        {
            var hitmissTable = CreateSampleDataTable();
            for (int i = 0; i < 10; i++)
                hitmissTable.Rows.Add(i, i * .25, i % 2);
            DataSource sourceWithActualData = SetupSampleDataSource(hitmissTable);
            SetUpFakeData(out List<string> myFlaws, out List<string> myMetaDatas, out List<string> myResponses, out List<string> mySpecIDs);
            _data.SetSource(sourceWithActualData, myFlaws, myMetaDatas, myResponses, mySpecIDs);
            _data.SetPythonEngine(new Mock<I_IPy4C>().Object, "Name");
        }
        private void SetupActivationDataSignalResponse()
        {
            var ahatTable = CreateSampleDataTable();
            for (int i = 0; i < 10; i++)
                ahatTable.Rows.Add(i, i * .25, i + .1);
            DataSource sourceWithActualData = SetupSampleDataSource(ahatTable);
            SetUpFakeData(out List<string> myFlaws, out List<string> myMetaDatas, out List<string> myResponses, out List<string> mySpecIDs);
            _data.SetSource(sourceWithActualData, myFlaws, myMetaDatas, myResponses, mySpecIDs);
            _data.SetPythonEngine(new Mock<I_IPy4C>().Object, "Name");
        }
        private void AssertFlawResponseDataNotUpdated()
        {
            Assert.That(_data.HMAnalysisObject.Flaws.Count, Is.Zero);
            Assert.That(_data.AHATAnalysisObject.Flaws.Count, Is.Zero);
            Assert.IsFalse(_data.HMAnalysisObject.Responses.ContainsKey("Responses"));
            Assert.IsFalse(_data.AHATAnalysisObject.Responses.ContainsKey("Responses"));
        }
        /// Tests for SetPythonEngine(I_IPy4C myPy, string myAnalysisName)
        
        [Test]
        public void SetPythonEngine_IP4yCArgumentIsNull_NoAnalysisObjectCreated()
        {
            //Arrange
            //Act
            _data.SetPythonEngine(null, string.Empty);
            //Assert
            Assert.That(_data.HMAnalysisObject, Is.Null);
            Assert.That(_data.AHATAnalysisObject, Is.Null);
        }
        [Test]
        [TestCase(AnalysisDataTypeEnum.AHat)]
        [TestCase(AnalysisDataTypeEnum.HitMiss)]
        [TestCase(AnalysisDataTypeEnum.None)]
        [TestCase(AnalysisDataTypeEnum.Undefined)]
        public void SetPythonEngine_IP4yCNotNullAndHMAndAHATObjectsNotNull_NoAnalysisObjectCreated(AnalysisDataTypeEnum analysisDataType)
        {
            //Arrange
            SetupIP4yC();
            _data.HMAnalysisObject = new HMAnalysisObject("myName");
            _data.AHATAnalysisObject = new AHatAnalysisObject("myName");
            _data.DataType = analysisDataType;
            //Act
            _data.SetPythonEngine(_python.Object, string.Empty);
            //Assert
            _python.Verify(p => p.HitMissAnalsysis(It.IsAny<string>()), Times.Never);
            _python.Verify(p => p.AHatAnalysis(It.IsAny<string>()), Times.Never);
        }
        [Test]
        [TestCase(AnalysisDataTypeEnum.None)]
        [TestCase(AnalysisDataTypeEnum.Undefined)]
        public void SetPythonEngine_IP4yCNotNullAndInvalidAnalysisDataTypePresent_NoAnalysisObjectCreated(AnalysisDataTypeEnum analysisDataType)
        {
            //Arrange
            SetupIP4yC();
            _data.DataType = analysisDataType;
            //Act
            _data.SetPythonEngine(_python.Object, string.Empty);
            //Assert
            _python.Verify(p => p.HitMissAnalsysis(It.IsAny<string>()), Times.Never);
            _python.Verify(p => p.AHatAnalysis(It.IsAny<string>()), Times.Never);
        }
        [Test]
        public void SetPythonEngine_IP4yCNotNullAndAnalysisDataTypeHitMissButHMAnalysisObjectIsNotNull_NoAnalysisObjectCreated()
        {
            //Arrange
            SetupIP4yC();
            _data.DataType = AnalysisDataTypeEnum.HitMiss;
            _data.HMAnalysisObject = new HMAnalysisObject("myName");
            //Act
            _data.SetPythonEngine(_python.Object, string.Empty);
            //Assert
            _python.Verify(p => p.HitMissAnalsysis(It.IsAny<string>()), Times.Never);
            _python.Verify(p => p.AHatAnalysis(It.IsAny<string>()), Times.Never);
        }
        [Test]
        public void SetPythonEngine_IP4yCNotNullAndAnalysisDataTypeAHatButAHatAnalysisObjectIsNotNull_NoAnalysisObjectCreated()
        {
            //Arrange
            SetupIP4yC();
            _data.DataType = AnalysisDataTypeEnum.AHat;
            _data.AHATAnalysisObject = new AHatAnalysisObject("myName");
            //Act
            _data.SetPythonEngine(_python.Object, string.Empty);
            //Assert
            _python.Verify(p => p.HitMissAnalsysis(It.IsAny<string>()), Times.Never);
            _python.Verify(p => p.AHatAnalysis(It.IsAny<string>()), Times.Never);
        }
        [Test]
        public void SetPythonEngine_IP4yCNotNullAndAnalysisDataTypeHitMiss_HMAnalysisCreated()
        {
            //Arrange
            SetupIP4yC();
            _data.DataType = AnalysisDataTypeEnum.HitMiss;
            //Act
            _data.SetPythonEngine(_python.Object, string.Empty);
            //Assert
            _python.Verify(p => p.HitMissAnalsysis(It.IsAny<string>()), Times.Once);
            _python.Verify(p => p.AHatAnalysis(It.IsAny<string>()), Times.Never);
            Assert.That(_data.HMAnalysisObject, Is.Not.Null);
        }
        [Test]
        public void SetPythonEngine_IP4yCNotNullAndAnalysisDataTypeAHat_AHatAnalysisCreated()
        {
            //Arrange
            SetupIP4yC();
            _data.DataType = AnalysisDataTypeEnum.AHat;
            //Act
            _data.SetPythonEngine(_python.Object, string.Empty);
            //Assert
            _python.Verify(p => p.HitMissAnalsysis(It.IsAny<string>()), Times.Never);
            _python.Verify(p => p.AHatAnalysis(It.IsAny<string>()), Times.Once);
            Assert.That(_data.AHATAnalysisObject, Is.Not.Null);
        }
        private void SetupIP4yC()
        {
            _python = new Mock<I_IPy4C>();
            _python.Setup(p => p.HitMissAnalsysis(It.IsAny<string>())).Returns(new HMAnalysisObject("myName"));
            _python.Setup(p => p.AHatAnalysis(It.IsAny<string>())).Returns(new AHatAnalysisObject("myName"));
        }


        /// Tests For the function : UpdateOutput(RCalculationType myCalculationType,
        /// IUpdateOutputForAHatData updateOutputForAHatDataIn = null,
        /// IUpdateOutputForHitMissData updateOutputForHitMissDataIn=null)
        DataTable _fitResidualsTable;
        DataTable _residualUncensoredTable;
        DataTable _residualRawTable;
        DataTable _residualCensoredTable;
        DataTable _residualFullCensoredTable;
        DataTable _residualPartialCensoredTable;
        DataTable _podCurveTable;
        DataTable _podCurveTableAll;
        DataTable _thresholdPlotTable;
        DataTable _thresholdPlotTable_All;
        DataTable _normalityTable;
        DataTable _normalityCurveTable;
        [Test]
        public void UpdateOutput_DataTypeIsAHatAndRCalculationTypeIsFull_AllTablesUpdated()
        {
            //Arrange
            Mock<IUpdateOutputForAHatData> updateoutputForAHatData = new Mock<IUpdateOutputForAHatData>();
            _data.DataType = AnalysisDataTypeEnum.AHat;
            SetupTableVariables();
            //Act
            _data.UpdateOutput(RCalculationType.Full, updateoutputForAHatData.Object);
            //Assert
            updateoutputForAHatData.Verify(upahat => upahat.UpdatePODCurveTable(ref _podCurveTable), Times.Once);
            updateoutputForAHatData.Verify(upahat => upahat.UpdatePODCurveAllTable(ref _podCurveTableAll), Times.Once);
            VerifyAllButPODCurveTables(updateoutputForAHatData, Times.Once);
        }
        [Test]
        public void UpdateOutput_DataTypeIsAHatAndRCalculationTypeIsThresholdChange_OnlyPODTablesUpdated()
        {
            //Arrange
            Mock<IUpdateOutputForAHatData> updateoutputForAHatData = new Mock<IUpdateOutputForAHatData>();
            _data.DataType = AnalysisDataTypeEnum.AHat;
            SetupTableVariables();
            //Act
            _data.UpdateOutput(RCalculationType.ThresholdChange, updateoutputForAHatData.Object);
            //Assert
            updateoutputForAHatData.Verify(upahat => upahat.UpdatePODCurveTable(ref _podCurveTable), Times.Once);
            updateoutputForAHatData.Verify(upahat => upahat.UpdatePODCurveAllTable(ref _podCurveTableAll), Times.Once);
            //Make sure these aren't executed
            VerifyAllButPODCurveTables(updateoutputForAHatData, Times.Never);
        }
        private void SetupTableVariables()
        {
            _fitResidualsTable = _data.FitResidualsTable;
            _residualUncensoredTable = _data.ResidualUncensoredTable;
            _residualRawTable = _data.ResidualRawTable;
            _residualCensoredTable = _data.ResidualCensoredTable;
            _residualFullCensoredTable = _data.ResidualFullCensoredTable;
            _residualPartialCensoredTable = _data.ResidualPartialCensoredTable;
            _podCurveTable = _data.PodCurveTable;
            _podCurveTableAll = _data.PodCurveTable_All;
            _thresholdPlotTable = _data.ThresholdPlotTable;
            _thresholdPlotTable_All = _data.ThresholdPlotTable_All;
            _normalityTable = _data.NormalityTable;
            _normalityCurveTable = _data.NormalityCurveTable;
        }
        private void VerifyAllButPODCurveTables(Mock<IUpdateOutputForAHatData> updateoutputForAHatData, Func<Times> times)
        {
            updateoutputForAHatData.Verify(upahat => upahat.UpdateFitResidualsTable(ref _fitResidualsTable), times);
            updateoutputForAHatData.Verify(upahat => upahat.UpdateResidualUncensoredTable(ref _residualUncensoredTable), times);
            updateoutputForAHatData.Verify(upahat => upahat.UpdateResidualRawTable(ref _residualRawTable), times);
            updateoutputForAHatData.Verify(upahat => upahat.UpdateResidualCensoredTable(ref _residualCensoredTable, _residualRawTable), times);
            updateoutputForAHatData.Verify(upahat => upahat.UpdateResidualFullCensoredTable(ref _residualFullCensoredTable, _residualRawTable), times);
            updateoutputForAHatData.Verify(upahat => upahat.UpdateResidualPartialCensoredTable(ref _residualPartialCensoredTable), times);
            updateoutputForAHatData.Verify(upahat => upahat.UpdateThresholdCurveTable(ref _thresholdPlotTable), times);
            updateoutputForAHatData.Verify(upahat => upahat.UpdateThresholdCurveTableAll(ref _thresholdPlotTable_All), times);
            updateoutputForAHatData.Verify(upahat => upahat.UpdateNormalityTable(ref _normalityTable, ref _normalityCurveTable), times);
        }
        //Calculation type should have no effect
        [Test]
        [TestCase(RCalculationType.Full)]
        [TestCase(RCalculationType.None)]
        [TestCase(RCalculationType.ThresholdChange)]
        public void UpdateOutput_DataTypeIsHitMiss_HitMissTablesUpdated(RCalculationType calcType)
        {
            //Arrange
            Mock<IUpdateOutputForHitMissData> updateoutputForHitMissData = new Mock<IUpdateOutputForHitMissData>();
            _data.DataType = AnalysisDataTypeEnum.HitMiss;
            var originalData = _data.OriginalData;
            var flawCount = _data.FlawCount;
            var podCurveTable = _data.PodCurveTable;
            var residualUncensoredTable = _data.ResidualUncensoredTable;
            var residualPartialCensoredTable = _data.ResidualPartialCensoredTable;
            var iterationsTable = _data.IterationsTable;
            //Act
            _data.UpdateOutput(calcType, null, updateoutputForHitMissData.Object);
            //Assert
            updateoutputForHitMissData.Verify(upahitmiss => upahitmiss.UpdateOriginalData(ref originalData));
            updateoutputForHitMissData.Verify(upahitmiss => upahitmiss.UpdateTotalFlawCount(ref flawCount));
            updateoutputForHitMissData.Verify(upahitmiss => upahitmiss.UpdatePODCurveTable(ref podCurveTable));
            updateoutputForHitMissData.Verify(upahitmiss => upahitmiss.UpdateResidualUncensoredTable(ref residualUncensoredTable));
            updateoutputForHitMissData.Verify(upahitmiss => upahitmiss.UpdateResidualPartialUncensoredTable(ref residualPartialCensoredTable));
            updateoutputForHitMissData.Verify(upahitmiss => upahitmiss.UpdateIterationsTable(ref iterationsTable));
        }
        /// tests for the WriteToExcel(ExcelExport myWriter, string myAnalysisName, string myWorksheetName, bool myPartOfProject = true,
        /// IExcelWriterControl excelWriteControlIn = null) function
        [Test]
        public void WriteToExcel_AnalysisTypeHitMiss_ExecutesTheWriteIterationsToExcelTable()
        {
            //Arrange
            _data.DataType = AnalysisDataTypeEnum.HitMiss;
            //Act
            _data.WriteToExcel(_excelExport.Object, "AnalysisName", "WorksheetName", true, _excelWriterControl.Object);
            //Assert

            _excelWriterControl.Verify(ewc => ewc.WriteIterationsToExcel(_data, It.IsAny<DataTable>()), Times.Exactly(1));
            _excelWriterControl.Verify(ewc => ewc.WritePODThresholdToExcel(_data, It.IsAny<DataTable>()), Times.Never);

        }
        [Test]
        public void WriteToExcel_AnalysisTypeAHat_ExecutesTheWritePODThresholdToExcelTable()
        {
            //Arrange
            _data.DataType = AnalysisDataTypeEnum.AHat;
            //Act
            _data.WriteToExcel(_excelExport.Object, "AnalysisName", "WorksheetName", true, _excelWriterControl.Object);
            //Assert

            _excelWriterControl.Verify(ewc => ewc.WriteIterationsToExcel(_data, It.IsAny<DataTable>()), Times.Never);
            _excelWriterControl.Verify(ewc => ewc.WritePODThresholdToExcel(_data, It.IsAny<DataTable>()), Times.Exactly(1));
            AssertGeneralWriteToExcelTables();
        }
        private void AssertGeneralWriteToExcelTables()
        {
            _excelWriterControl.Verify(ewc => ewc.WriteResidualsToExcel(_data, It.IsAny<DataTable>()), Times.Exactly(1));
            _excelWriterControl.Verify(ewc => ewc.WritePODToExcel(_data, It.IsAny<int>()), Times.Exactly(1));
            _excelWriterControl.Verify(ewc => ewc.WriteRemovedPointsToExcel(_data, It.IsAny<DataTable>(), It.IsAny<DataTable>(),
            It.IsAny<DataTable>()), Times.Exactly(1));
        }
        /// Tests for the AdditionalWorksheet1Name getter
        [Test]
        [TestCase(AnalysisDataTypeEnum.None)]
        [TestCase(AnalysisDataTypeEnum.Undefined)]
        public void AdditionalWorksheet1Name_DataTypeIsInvalid_ReturnsNotApplicable(AnalysisDataTypeEnum datatype)
        {
            //Arrange
            _data.DataType = datatype;
            //Act
            var result = _data.AdditionalWorksheet1Name;
            //Assert
            Assert.That(result, Is.EqualTo(Globals.NotApplicable));
        }
        [Test]
        [TestCase(AnalysisDataTypeEnum.AHat, "Threshold")]
        [TestCase(AnalysisDataTypeEnum.HitMiss, "Solver")]
        public void AdditionalWorksheet1Name_DataTypeIsValid_ReturnsAppropriateString(AnalysisDataTypeEnum datatype, string expectedName)
        {
            //Arrange
            _data.DataType = datatype;
            //Act
            var result = _data.AdditionalWorksheet1Name;
            //Assert
            Assert.That(result, Is.EqualTo(expectedName));
        }
        /// Tests for the UncensoredFlawRangeMin getter
        [Test]
        [TestCase(TransformTypeEnum.Linear , 1.0)]
        [TestCase(TransformTypeEnum.Log, 4.0)]
        [TestCase(TransformTypeEnum.Inverse, 7.0)]
        public void UncensoredFlawRangeMin_DataTypeIsHitMiss_ReturnsMinBasedOnTransformType(TransformTypeEnum transform, double min)
        {
            //Arrange
            _data.FlawTransform = transform;
            HMAnalysisObject hmAnalysisObject = SetupFlawsAtAllTransforms(new HMAnalysisObject("HitMissName"));
            _data.DataType = AnalysisDataTypeEnum.HitMiss;
            _data.HMAnalysisObject = hmAnalysisObject;
            //Act
            var result = _data.UncensoredFlawRangeMin;
            //Assert
            Assert.That(result, Is.EqualTo(min));

        }
        [Test]
        [TestCase(TransformTypeEnum.Linear, 1.0)]
        [TestCase(TransformTypeEnum.Log, 4.0)]
        public void UncensoredFlawRangeMin_DataTypeIsAHat_ReturnsMinBasedOnTransformType(TransformTypeEnum transform, double min)
        {
            //Arrange
            _data.FlawTransform = transform;
            AHatAnalysisObject ahatAnalysisObject = SetupFlawsAtAllTransformsAHAT(new AHatAnalysisObject("AHatName"));
            _data.DataType = AnalysisDataTypeEnum.AHat;
            _data.AHATAnalysisObject = ahatAnalysisObject;
            //Act
            var result = _data.UncensoredFlawRangeMin;
            //Assert
            Assert.That(result, Is.EqualTo(min));

        }
        private HMAnalysisObject SetupFlawsAtAllTransforms(HMAnalysisObject hmAnalysisObject)
        {
            hmAnalysisObject.Flaws_All = new List<double>() { 1.0, 2.0, 3.0 };
            hmAnalysisObject.LogFlaws_All = new List<double>() { 4.0, 5.0, 6.0 };
            hmAnalysisObject.InverseFlaws_All = new List<double>() { 7.0, 8.0, 9.0 };
            return hmAnalysisObject;
        }
        private AHatAnalysisObject SetupFlawsAtAllTransformsAHAT(AHatAnalysisObject ahatAnalysisObject)
        {
            ahatAnalysisObject.Flaws_All = new List<double>() { 1.0, 2.0, 3.0 };
            ahatAnalysisObject.LogFlaws_All = new List<double>() { 4.0, 5.0, 6.0 };
            ahatAnalysisObject.InverseFlaws_All = new List<double>() { 7.0, 8.0, 9.0 };
            return ahatAnalysisObject;
        }
        /// Tests for the FlawRangeMin getter
        [Test]
        [TestCase(AnalysisDataTypeEnum.None)]
        [TestCase(AnalysisDataTypeEnum.Undefined)]
        [TestCase(AnalysisDataTypeEnum.HitMiss)]
        [TestCase(AnalysisDataTypeEnum.AHat)]
        public void FlawRangeMin_DataTypePassedButFlawsForbothHitMissAndAHatAreEmpty_ReturnsNaN(AnalysisDataTypeEnum dataType)
        {
            //Arrange
            _data.DataType = dataType;
            SetupAHatAndHMAnalysisObjects();
            //Act
            var result = _data.FlawRangeMin;
            //Assert
            Assert.That(result, Is.EqualTo(double.NaN));
        }
        [Test]
        public void FlawRangeMin_DataTypeIsHitMissAndFlawsCountIsGreaterThan0_ReturnsMinFlaw()
        {
            //Arrange
            _data.DataType = AnalysisDataTypeEnum.HitMiss;
            SetupAHatAndHMAnalysisObjects();
            _data.HMAnalysisObject.Flaws = new List<double>() { 1.0, 2.0, 3.0 };
            //Act
            var result = _data.FlawRangeMin;
            //Assert
            Assert.That(result, Is.EqualTo(1.0));
        }
        [Test]
        public void FlawRangeMin_DataTypeIsAHatAndFlawsCountIsGreaterThan0_ReturnsMinFlaw()
        {
            //Arrange
            _data.DataType = AnalysisDataTypeEnum.AHat;
            SetupAHatAndHMAnalysisObjects();
            _data.AHATAnalysisObject.Flaws = new List<double>() { 1.0, 2.0, 3.0 };
            //Act
            var result = _data.FlawRangeMin;
            //Assert
            Assert.That(result, Is.EqualTo(1.0));
        }
        private void SetupAHatAndHMAnalysisObjects()
        {
            _data.HMAnalysisObject = new HMAnalysisObject("HitMissName");
            _data.AHATAnalysisObject = new AHatAnalysisObject("AHatName");
        }
        /// Tests for the UncensoredFlawRangeMax getter
        [Test]
        [TestCase(TransformTypeEnum.Linear, 3.0)]
        [TestCase(TransformTypeEnum.Log, 6.0)]
        [TestCase(TransformTypeEnum.Inverse, 9.0)]
        public void UncensoredFlawRangeMax_DataTypeIsHitMiss_ReturnsMaxBasedOnTransformType(TransformTypeEnum transform, double min)
        {
            //Arrange
            _data.FlawTransform = transform;
            HMAnalysisObject hmAnalysisObject = SetupFlawsAtAllTransforms(new HMAnalysisObject("HitMissName"));
            _data.DataType = AnalysisDataTypeEnum.HitMiss;
            _data.HMAnalysisObject = hmAnalysisObject;
            //Act
            var result = _data.UncensoredFlawRangeMax;
            //Assert
            Assert.That(result, Is.EqualTo(min));

        }
        [Test]
        [TestCase(TransformTypeEnum.Linear, 3.0)]
        [TestCase(TransformTypeEnum.Log, 6.0)]
        public void UncensoredFlawRangeMax_DataTypeIsAHat_ReturnsMaxBasedOnTransformType(TransformTypeEnum transform, double min)
        {
            //Arrange
            _data.FlawTransform = transform;
            AHatAnalysisObject ahatAnalysisObject = SetupFlawsAtAllTransformsAHAT(new AHatAnalysisObject("AHatName"));
            _data.DataType = AnalysisDataTypeEnum.AHat;
            _data.AHATAnalysisObject = ahatAnalysisObject;
            //Act
            var result = _data.UncensoredFlawRangeMax;
            //Assert
            Assert.That(result, Is.EqualTo(min));

        }
        /// Tests for InvertTransformedFlaw(double myValue)
        [Test]
        [TestCase(TransformTypeEnum.Linear)]
        [TestCase(TransformTypeEnum.Log)]
        [TestCase(TransformTypeEnum.Exponetial)]
        [TestCase(TransformTypeEnum.Inverse)]
        [TestCase(TransformTypeEnum.BoxCox)]
        [TestCase(TransformTypeEnum.None)]
        public void InvertTransformedFlaw_BothAHatandHMAnalysisObjectIsNull_ReturnsTheSameValue(TransformTypeEnum transform)
        {
            //Arrange
            SetupPythonMock();
            _data.FlawTransform = transform;
            //Act
            var result = _data.InvertTransformedFlaw(1.0);
            //Assert
            Assert.That(result, Is.EqualTo(1));
            _python.Verify(p => p.TransformEnumToInt(It.IsAny<TransformTypeEnum>()), Times.Never);
        }
        /// The log transform is in a separate test since it requires Math.Exp
        [Test]
        [TestCase(1, 2.0)]
        [TestCase(3, 1.0/2.0)]
        [TestCase(4, 2.0)]
        [TestCase(5, 3.0)]
        [TestCase(6, 2.0)]
        public void InvertTransformedFlaw_AHatAnalysisObjectNotNull_ReturnsTransformedValue(int inputTransform, double expectedTransformValue)
        {
            //Arrange
            SetupPythonMock();
            _python.Setup(p => p.TransformEnumToInt(It.IsAny<TransformTypeEnum>())).Returns(inputTransform);
            _data.AHATAnalysisObject = new AHatAnalysisObject("AnalysisName") { Lambda = 1.0 };
            //Act
            var result = _data.InvertTransformedFlaw(2.0);
            //Assert
            _python.Verify(p => p.TransformEnumToInt(It.IsAny<TransformTypeEnum>()), Times.Once);
            Assert.That(result, Is.EqualTo(expectedTransformValue));
        }
        [Test]
        public void InvertTransformedFlaw_AHatAnalysisObjectNotNullAndTransformIsLog_ReturnsTransformedValue()
        {
            //Arrange
            SetupPythonMock();
            _python.Setup(p => p.TransformEnumToInt(It.IsAny<TransformTypeEnum>())).Returns(2);
            _data.AHATAnalysisObject = new AHatAnalysisObject("AnalysisName");
            //Act
            var result = _data.InvertTransformedFlaw(2.0);
            //Assert
            _python.Verify(p => p.TransformEnumToInt(It.IsAny<TransformTypeEnum>()), Times.Once);
            Assert.That(result, Is.EqualTo(Math.Exp(2)));
        }
        // BoxCox is never passed in for hitmiss (case = 5)
        [Test]
        [TestCase(1, 2.0)]
        [TestCase(3, 1.0 / 2.0)]
        [TestCase(4, 2.0)]
        [TestCase(6, 2.0)]
        public void InvertTransformedFlaw_HMAnalysisObjectNotNull_ReturnsTransformedValue(int inputTransform, double expectedTransformValue)
        {
            //Arrange
            SetupPythonMock();
            _python.Setup(p => p.TransformEnumToInt(It.IsAny<TransformTypeEnum>())).Returns(inputTransform);
            _data.HMAnalysisObject = new HMAnalysisObject("AnalysisName");
            _data.DataType = AnalysisDataTypeEnum.HitMiss;
            //Act
            var result = _data.InvertTransformedFlaw(2.0);
            //Assert
            _python.Verify(p => p.TransformEnumToInt(It.IsAny<TransformTypeEnum>()), Times.Once);
            Assert.That(result, Is.EqualTo(expectedTransformValue));
        }

        private void SetupPythonMock()
        {
            _python = new Mock<I_IPy4C>();
            _data.SetPythonEngine(_python.Object, "AnalysisName");
        }
        /// tests for InvertTransformedResponse(double myValue)
        /// The log transform is in a separate test since it requires Math.Exp
        [Test]
        [TestCase(1, 2.0)]
        [TestCase(3, 1.0/2.0)]
        [TestCase(4, 2.0)]
        [TestCase(5, 3.0)]
        [TestCase(6, 2.0)]
        public void InvertTransformedResponse_AHatAnalysisObjectNotNull_ReturnsTransformedValue(int inputTransform, double expectedTransformValue)
        {
            //Arrange
            SetupPythonMock();
            _python.Setup(p => p.TransformEnumToInt(It.IsAny<TransformTypeEnum>())).Returns(inputTransform);
            _data.AHATAnalysisObject = new AHatAnalysisObject("AnalysisName") { Lambda = 1.0 };
            //Act
            var result = _data.InvertTransformedResponse(2.0);
            //Assert
            _python.Verify(p => p.TransformEnumToInt(It.IsAny<TransformTypeEnum>()), Times.Once);
            Assert.That(result, Is.EqualTo(expectedTransformValue));
        }
        [Test]
        public void InvertTransformedResponse_AHatAnalysisObjectNotNullAndTransformIsLog_ReturnsTransformedValue()
        {
            //Arrange
            SetupPythonMock();
            _python.Setup(p => p.TransformEnumToInt(It.IsAny<TransformTypeEnum>())).Returns(2);
            _data.AHATAnalysisObject = new AHatAnalysisObject("AnalysisName");
            //Act
            var result = _data.InvertTransformedResponse(2.0);
            //Assert
            _python.Verify(p => p.TransformEnumToInt(It.IsAny<TransformTypeEnum>()), Times.Once);
            Assert.That(result, Is.EqualTo(Math.Exp(2)));
        }
        [Test]
        [TestCase(1)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        public void InvertTransformedResponse_HMAnalysisObjectNotNull_ReturnsTransformedValue(int inputTransform)
        {
            //Arrange
            SetupPythonMock();
            _python.Setup(p => p.TransformEnumToInt(It.IsAny<TransformTypeEnum>())).Returns(inputTransform);
            _data.HMAnalysisObject = new HMAnalysisObject("AnalysisName");
            _data.DataType = AnalysisDataTypeEnum.HitMiss;
            //Act
            var result = _data.InvertTransformedResponse(2.0);
            //Assert
            _python.Verify(p => p.TransformEnumToInt(It.IsAny<TransformTypeEnum>()), Times.Never);
            Assert.That(result, Is.EqualTo(2.0));
        }
        /// Tests for FlawCount getter
        [Test]
        [TestCase(AnalysisDataTypeEnum.None)]
        [TestCase(AnalysisDataTypeEnum.Undefined)]
        public void FlawCount_DataTypeInvalid_Returns0(AnalysisDataTypeEnum datatype)
        {
            // Arrange
            _data.DataType = datatype;
            //Act
            var result = _data.FlawCount;
            //Assert
            Assert.That(result, Is.Zero);
        }
        [Test]
        public void FlawCount_DataTypeIsHitMiss_ReturnsTotalFlawCount()
        {
            // Arrange
            HMAnalysisObject hmAnalysisObject = new HMAnalysisObject("AnalysisName") { Flaws = new List<double>() { 1.0, 2.0, 3.0 } };
            UpdateOutputForHitMissData updateOutput = new UpdateOutputForHitMissData(hmAnalysisObject, new Mock<IMessageBoxWrap>().Object);
            _data.HMAnalysisObject = hmAnalysisObject;
            _data.DataType = AnalysisDataTypeEnum.HitMiss;
            _data.UpdateOutput(RCalculationType.Full, null, updateOutput);
            // Act
            var result=_data.FlawCount;
            // Assert
            Assert.That(result, Is.EqualTo(3));
        }
        [Test]
        public void FlawCount_DataTypeIsAHatAndTablesAreNotNull_ReturnsSumOfResidualTables()
        {
            // Arrange
            SetupResidTable();
            AHatAnalysisObject ahatAnalysisObject = CreateFakeAHatObject(_table, _table.Copy());
            SetupUpdateOutputDataAHAT(ahatAnalysisObject);
            // Act
            var result = _data.FlawCount;
            // Assert
            Assert.That(result, Is.EqualTo(10));
        }
        [Test]
        public void FlawCount_DataTypeIsAHatAndBothTablesAreNull_ReturnsSumOfResidualTables()
        {
            // Arrange
            SetupResidTable();
            AHatAnalysisObject ahatAnalysisObject = CreateFakeAHatObject(null, null);
            _data.AHATAnalysisObject = ahatAnalysisObject;
            _data.DataType = AnalysisDataTypeEnum.AHat;
            // Act
            var result = _data.FlawCount;
            // Assert
            Assert.That(result, Is.Zero);
        }
        [Test]
        public void FlawCount_DataTypeIsAHatAndUncensoredTableIsNull_ReturnsSumOfResidualTables()
        {
            // Arrange
            SetupResidTable();
            AHatAnalysisObject ahatAnalysisObject = CreateFakeAHatObject(_table, null);
            SetupUpdateOutputDataAHAT(ahatAnalysisObject);
            // Act
            var result = _data.FlawCount;
            // Assert
            Assert.That(result, Is.Zero);
        }
        [Test]
        public void FlawCount_DataTypeIsAHatAndCensoredTableIsNull_ReturnsSumOfResidualTables()
        {
            // Arrange
            SetupResidTable();
            AHatAnalysisObject ahatAnalysisObject = CreateFakeAHatObject(null, _table);
            SetupUpdateOutputDataAHAT(ahatAnalysisObject);
            // Act
            var result = _data.FlawCount;
            // Assert
            Assert.That(result, Is.Zero);
        }
        private AHatAnalysisObject CreateFakeAHatObject(DataTable table1, DataTable table2)
        {
            AHatAnalysisObject ahatAnalysisObject = new AHatAnalysisObject("AnalysisName")
            {
                Flaws = new List<double>() { 1.0, 2.0, 3.0 },
                AHatResultsResidUncensored = table1,
                AHatResultsResid = table2,
                //Censor one of the points to ensure that all the flaws are still being added up
                FlawsCensored = new List<double>() { 2.0 }
            };
            return ahatAnalysisObject;
        }
        private void SetupUpdateOutputDataAHAT(AHatAnalysisObject ahatAnalysisObject)
        {
            UpdateOutputForAHatData updateOutput = new UpdateOutputForAHatData(ahatAnalysisObject, new Mock<IMessageBoxWrap>().Object);
            _data.AHATAnalysisObject = ahatAnalysisObject;
            _data.DataType = AnalysisDataTypeEnum.AHat;
            _data.UpdateOutput(RCalculationType.Full, updateOutput);
        }
        private void SetupResidTable()
        {
            _table.Columns.Add("Column4");
            _table.Columns["Column1"].ColumnName = "flaw";
            _table.Columns["Column2"].ColumnName = "y";
            for (int i = 1; i < 11; i++)
                _table.Rows.Add(i, i * .25, i + 1, i + 2);
        }

        /// Tests for FlawCountUnique getter
        /// This getter is only really used for hitmiss
        [Test]
        [TestCase(AnalysisDataTypeEnum.None)]
        [TestCase(AnalysisDataTypeEnum.Undefined)]
        public void FlawCountUnique_DataTypeNotValid_Returns0(AnalysisDataTypeEnum datatype)
        {
            //Arrange
            _data.DataType = datatype;
            //Act
            var result = _data.FlawCountUnique;
            //Assert
            Assert.That(result, Is.Zero);
        }
        [Test]
        public void FlawCountUnique_DataTypeAHat_Returns0()
        {
            //Arrange
            SetupResidTable();
            AHatAnalysisObject ahatAnalysisObject = CreateFakeAHatObject(null, null);
            UpdateOutputForAHatData updateOutput = new UpdateOutputForAHatData(ahatAnalysisObject, new Mock<IMessageBoxWrap>().Object);
            _data.AHATAnalysisObject = ahatAnalysisObject;
            _data.DataType = AnalysisDataTypeEnum.AHat;
            _data.UpdateOutput(RCalculationType.Full, updateOutput);
            //Act
            var result = _data.FlawCountUnique;
            //Assert
            Assert.That(result, Is.Zero);
        }
        [Test]
        public void FlawCountUnique_DataTypeIsHitMissAndResidualTableIsNull_Returns0()
        {
            //Arrange
            SetupResidTable();
            HMAnalysisObject hmAnalysisObject = new HMAnalysisObject("AnalysisName") { 
                Flaws = new List<double>() { 1.0, 2.0, 3.0 },
                ResidualTable = null
            };
            UpdateOutputForHitMissData updateOutput = new UpdateOutputForHitMissData(hmAnalysisObject, new Mock<IMessageBoxWrap>().Object);
            _data.HMAnalysisObject = hmAnalysisObject;
            _data.DataType = AnalysisDataTypeEnum.HitMiss;
            _data.UpdateOutput(RCalculationType.Full, null, updateOutput);
            //Act
            var result = _data.FlawCountUnique;
            //Assert
            Assert.That(result, Is.Zero);
        }
        [Test]
        public void FlawCountUnique_DataTypeIsHitMissAndResidualTableIsNotNull_ReturnsCountOfRows()
        {
            //Arrange
            SetupResidTable();
            HMAnalysisObject hmAnalysisObject = new HMAnalysisObject("AnalysisName") { 
                Flaws = new List<double>() { 1.0, 2.0, 3.0 },
                ResidualTable = _table,  
            };
            UpdateOutputForHitMissData updateOutput = new UpdateOutputForHitMissData(hmAnalysisObject, new Mock<IMessageBoxWrap>().Object);
            _data.HMAnalysisObject = hmAnalysisObject;
            _data.DataType = AnalysisDataTypeEnum.HitMiss;
            _data.UpdateOutput(RCalculationType.Full, null, updateOutput);
            //Act
            var result = _data.FlawCountUnique;
            //Assert
            Assert.That(result, Is.EqualTo(10));
        }

        /// Skipping buffered ranges and minmax functions for now
        /// 

        /// Tests for TransformAValue(double myValue, int transform) function
        [Test]
        [TestCase(1, 2.0)]
        [TestCase(3, 0.5)]
        [TestCase(4, 2.0)]
        [TestCase(6, 2.0)]
        public void TransformAValue_LinearOrInverseTransformPassed_ReturnsTransformedValue(int transform, double expectedValue)
        {
            //Arrange
            //Act
            var result = _data.TransformAValue(2.0, transform);
            //Assert
            Assert.That(result, Is.EqualTo(expectedValue));
        }
        [Test]
        public void TransformAValue_LogTransformPassed_ReturnsTransformedValue()
        {
            //Arrange
            //Act
            var result = _data.TransformAValue(Math.E, 2);
            //Assert
            Assert.That(result, Is.EqualTo(1));
        }
        [Test]
        public void TransformAValue_BoxCoxTransformPassed_ReturnsTransformedValue()
        {
            //Arrange
            _data.AHATAnalysisObject = new AHatAnalysisObject("AnalysisName") { Lambda = 2.0 };
            //Act
            var result = _data.TransformAValue(2.0, 5);
            //Assert
            Assert.That(result, Is.EqualTo(1.5));
        }

        /// Tests for TransformBackAValue(double myValue, int transform) function
        [Test]
        [TestCase(1, 1.0 / 2.0, 1.0 / 2.0)]
        [TestCase(3, 1.0 / 2.0, 2.0)]
        public void TransformBackAValue_LinearOrInverseTransformPassed_ReturnsTransformedBackValue(int transform, double transformedValue, double expectedBackValue)
        {
            //Arrange
            //Act
            var result = _data.TransformBackAValue(transformedValue, transform);
            //Assert
            Assert.That(result, Is.EqualTo(expectedBackValue));
        }
        [Test]
        public void TransformBackAValue_TransformTypeIsLog_ReturnsTransformedBackValue()
        {
            //Arrange

            //Act
            var result = _data.TransformBackAValue(2.0, 2);
            //Assert
            Assert.That(result, Is.EqualTo(Math.Exp(2.0)));
        }
        [Test]
        public void TransformBackAValue_TransformIsBoxCox_ReturnsTransformedBackValue()
        {
            //Arrange
            Mock<ITransformBackLambdaControl> transformBackLambdaControl = new Mock<ITransformBackLambdaControl>();
            transformBackLambdaControl.Setup(tblc => tblc.TransformBackLambda(It.IsAny<double>())).Returns(-1.0);
            _data.TransformBackLambda = transformBackLambdaControl.Object;
            //Act
            var result = _data.TransformBackAValue(2.0, 5);
            //Assert
            Assert.That(result, Is.EqualTo(-1.0));
        }
        [Test]
        [TestCase(4, .5)]
        [TestCase(4, 1.0)]
        [TestCase(6, .5)]
        [TestCase(6, 1.0)]
        public void TransformBackAValue_InvalidTransformPassed_ReturnsTheSameValue(int transform, double inputValue)
        {
            //Act
            var result = _data.TransformBackAValue(inputValue, transform);
            //Assert
            Assert.That(result, Is.EqualTo(inputValue));
        }

        /// Tests for TransformValueForXAxis
        [Test]
        [TestCase(TransformTypeEnum.Log, 0.0)]
        [TestCase(TransformTypeEnum.Log, -1.0)]
        [TestCase(TransformTypeEnum.Inverse, 0.0)]
        [TestCase(TransformTypeEnum.Inverse, -1.0)]
        public void TransformValueForXAxis_ValueIsZeroOrLessAndTransformIsLogOrInverse_Returns0(TransformTypeEnum transform, double inputValue)
        {
            //Arrange
            _data.FlawTransform = transform;
            //Act
            var result=_data.TransformValueForXAxis(inputValue);
            //Assert
            Assert.That(result, Is.Zero);
        }
        [Test]
        [TestCase(TransformTypeEnum.Linear, 0.0)]
        [TestCase(TransformTypeEnum.Linear, -1.0)]
        [TestCase(TransformTypeEnum.Exponetial, 0.0)]
        [TestCase(TransformTypeEnum.Exponetial, -1.0)]
        [TestCase(TransformTypeEnum.BoxCox, 0.0)]
        [TestCase(TransformTypeEnum.BoxCox, -1.0)]
        [TestCase(TransformTypeEnum.Custom, 0.0)]
        [TestCase(TransformTypeEnum.Custom, -1.0)]
        [TestCase(TransformTypeEnum.None, 0.0)]
        [TestCase(TransformTypeEnum.None, -1.0)]
        [TestCase(TransformTypeEnum.Log, 1.0)]
        [TestCase(TransformTypeEnum.Log, 1.0)]
        [TestCase(TransformTypeEnum.Inverse, 1.0)]
        [TestCase(TransformTypeEnum.Inverse, 1.0)]
        public void TransformValueForXAxis_MyValueIsGreaterThan0OrTransformIsNotLogOrInverse_ReturnsTransformValue(TransformTypeEnum transform, double inputValue)
        {
            //Arrange
            SetupPythonMock();
            _python.Setup(p => p.TransformEnumToInt(It.IsAny<TransformTypeEnum>())).Returns(1); //Effectively makes the transform linear
            _data.FlawTransform=transform;
            //Act
            var result=_data.TransformValueForXAxis(inputValue);
            //Assert
            Assert.That(result, Is.EqualTo(inputValue));
            _python.Verify(p => p.TransformEnumToInt(transform));
        }
        [Test]
        [TestCase(TransformTypeEnum.Log, 0.0)]
        [TestCase(TransformTypeEnum.Log, -1.0)]
        [TestCase(TransformTypeEnum.Inverse, 0.0)]
        [TestCase(TransformTypeEnum.Inverse, -1.0)]
        /// Tests for TransformValueForYAxis
        public void TransformValueForYAxis_ValueIsZeroOrLessAndTransformIsLogOrInverse_Returns0(TransformTypeEnum transform, double inputValue)
        {
            //Arrange
            _data.ResponseTransform = transform;
            //Act
            var result = _data.TransformValueForYAxis(inputValue);
            //Assert
            Assert.That(result, Is.Zero);
        }
        [Test]
        [TestCase(TransformTypeEnum.Linear, 0.0)]
        [TestCase(TransformTypeEnum.Linear, -1.0)]
        [TestCase(TransformTypeEnum.Exponetial, 0.0)]
        [TestCase(TransformTypeEnum.Exponetial, -1.0)]
        [TestCase(TransformTypeEnum.BoxCox, 0.0)]
        [TestCase(TransformTypeEnum.BoxCox, -1.0)]
        [TestCase(TransformTypeEnum.Custom, 0.0)]
        [TestCase(TransformTypeEnum.Custom, -1.0)]
        [TestCase(TransformTypeEnum.None, 0.0)]
        [TestCase(TransformTypeEnum.None, -1.0)]
        [TestCase(TransformTypeEnum.Log, 1.0)]
        [TestCase(TransformTypeEnum.Inverse, 1.0)]
        public void TransformValueForYAxis_MyValueIsGreaterThan0OrTransformIsNotLogOrInverse_ReturnsTransformValue(TransformTypeEnum transform, double inputValue)
        {
            //Arrange
            SetupPythonMock();
            _python.Setup(p => p.TransformEnumToInt(It.IsAny<TransformTypeEnum>())).Returns(1); //Effectively makes the transform linear
            _data.ResponseTransform = transform;
            //Act
            var result = _data.TransformValueForYAxis(inputValue);
            //Assert
            Assert.That(result, Is.EqualTo(inputValue));
            _python.Verify(p => p.TransformEnumToInt(transform));
        }
        /// Tests for InvertTransformValueForXAxis
        [TestCase(TransformTypeEnum.Inverse, 0.0)]
        [Test]
        public void InvertTransformValueForXAxis_ValueIsZeroOrLessAndTransformIsLogOrInverse_Returns0(TransformTypeEnum transform, double inputValue)
        {
            //Arrange
            _data.FlawTransform = transform;
            //Act
            var result = _data.InvertTransformValueForXAxis(inputValue);
            //Assert
            Assert.That(result, Is.Zero);
        }
        [Test]
        [TestCase(TransformTypeEnum.Linear, 0.0)]
        [TestCase(TransformTypeEnum.Linear, -1.0)]
        [TestCase(TransformTypeEnum.Log, 0.0)]
        [TestCase(TransformTypeEnum.Log, -1.0)]
        [TestCase(TransformTypeEnum.Exponetial, 0.0)]
        [TestCase(TransformTypeEnum.Exponetial, -1.0)]
        [TestCase(TransformTypeEnum.BoxCox, 0.0)]
        [TestCase(TransformTypeEnum.BoxCox, -1.0)]
        [TestCase(TransformTypeEnum.Custom, 0.0)]
        [TestCase(TransformTypeEnum.Custom, -1.0)]
        [TestCase(TransformTypeEnum.None, 0.0)]
        [TestCase(TransformTypeEnum.Inverse, -1.0)]
        [TestCase(TransformTypeEnum.Inverse, 1.0)]
        public void InvertTransformValueForXAxis_MyValueIsGreaterThan0OrTransformIsNotLogOrInverse_ReturnsTransformBackValue(TransformTypeEnum transform, double inputValue)
        {
            //Arrange
            SetupPythonMock();
            _python.Setup(p => p.TransformEnumToInt(It.IsAny<TransformTypeEnum>())).Returns(1); //Effectively makes the transform linear
            _data.FlawTransform = transform;
            //Act
            var result = _data.InvertTransformValueForXAxis(inputValue);
            //Assert
            Assert.That(result, Is.EqualTo(inputValue));
            _python.Verify(p => p.TransformEnumToInt(transform));
        }
        /// Tests for InvertTransformValueForXAxis
        [Test]
        [TestCase(TransformTypeEnum.Inverse, 0.0)]
        public void InvertTransformValueForYAxis_ValueIsZeroOrLessAndTransformIsLogOrInverse_Returns0(TransformTypeEnum transform, double inputValue)
        {
            //Arrange
            _data.ResponseTransform = transform;
            //Act
            var result = _data.InvertTransformValueForYAxis(inputValue);
            //Assert
            Assert.That(result, Is.Zero);
        }
        [Test]
        [TestCase(TransformTypeEnum.Linear, 0.0)]
        [TestCase(TransformTypeEnum.Linear, -1.0)]
        [TestCase(TransformTypeEnum.Log, 0.0)]
        [TestCase(TransformTypeEnum.Log, -1.0)]
        [TestCase(TransformTypeEnum.Exponetial, 0.0)]
        [TestCase(TransformTypeEnum.Exponetial, -1.0)]
        [TestCase(TransformTypeEnum.BoxCox, 0.0)]
        [TestCase(TransformTypeEnum.BoxCox, -1.0)]
        [TestCase(TransformTypeEnum.Custom, 0.0)]
        [TestCase(TransformTypeEnum.Custom, -1.0)]
        [TestCase(TransformTypeEnum.None, 0.0)]
        [TestCase(TransformTypeEnum.Inverse, -1.0)]
        [TestCase(TransformTypeEnum.Inverse, 1.0)]
        public void InvertTransformValueForYAxis_MyValueIsGreaterThan0OrTransformIsNotLogOrInverse_ReturnsTransformBackValue(TransformTypeEnum transform, double inputValue)
        {
            //Arrange
            SetupPythonMock();
            _python.Setup(p => p.TransformEnumToInt(It.IsAny<TransformTypeEnum>())).Returns(1); //Effectively makes the transform linear
            _data.ResponseTransform = transform;
            //Act
            var result = _data.InvertTransformValueForYAxis(inputValue);
            //Assert
            Assert.That(result, Is.EqualTo(inputValue));
            _python.Verify(p => p.TransformEnumToInt(transform));
        }
    }
}
