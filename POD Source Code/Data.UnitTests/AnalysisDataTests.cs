using NUnit.Framework;
using System;
using Moq;
using POD.Data;
using System.Collections.Generic;
using System.Data;
using POD;
using CSharpBackendWithR;

namespace Data.UnitTests
{
    [TestFixture]
    public class AnalysisDataTests
    {
        private AnalysisData _data;
        private DataSource _source;
        private DataTable _table;
        [SetUp]
        public void Setup()
        {

            _data = new AnalysisData();
            _source = new DataSource("MyDataSource", "ID", "flawName.centimeters", "Response");
            _table = new DataTable();
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
            Assert.AreEqual(_data.CommentDictionary.Count, originalCount+1);
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
            _data.CommentDictionary.Add(columnIndex, new Dictionary<int, string>() { { rowIndex+1, "" } });
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
            Assert.DoesNotThrow(()=>_data.UpdateData());
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
        private Mock<I_IPy4C> _python;
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
        public void UpdateOaycutput_DataTypeIsHitMiss_HitMissTablesUpdated(RCalculationType calcType)
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

    }
}
