using NUnit.Framework;
using System;
using Moq;
using POD.Data;
using System.Collections.Generic;

namespace Data.UnitTests
{
    [TestFixture]
    public class AnalysisDataTests
    {
        private AnalysisData _data;
        [SetUp]
        public void Setup()
        {
            _data = new AnalysisData();
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
            DataSource source = new DataSource("MyDataSource", "ID", "flawName.centimeters", "Response");
            _data.SetSource(source);
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
            DataSource source = new DataSource("MyDataSource", "ID", "flawName.centimeters", "Response");
            _data.SetSource(source);
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
    }
}
