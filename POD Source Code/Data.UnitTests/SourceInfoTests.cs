using NUnit.Framework;
using System;
using Moq;
using POD.Data;
using System.Collections.Generic;

namespace Data.UnitTests
{
    [TestFixture]
    public class SourceInfoTests
    {
        private SourceInfo _sourceInfo;
        private Mock<IDataSource> _dataSource;
        private List<ColumnInfo> _flawInfo;
        private List<ColumnInfo> _responseInfo;
        [SetUp]
        public void Setup()
        {

            _dataSource = new Mock<IDataSource>();
            _flawInfo = new List<ColumnInfo>() { new ColumnInfo("", "flaw", "", 0.0, 1.0, 0) };
            _responseInfo = new List<ColumnInfo>() { new ColumnInfo("", "Response", "", 1.0, 10.0, 1) };
            _dataSource.Setup(ds => ds.ColumnInfos(ColType.Flaw)).Returns(_flawInfo);
            _dataSource.Setup(ds => ds.ColumnInfos(ColType.Response)).Returns(_responseInfo);
            
        }
        /// <summary>
        /// Tests for the SourceInfo(string myOriginalName, string myNewName, IDataSource mySource) contructor
        /// </summary>
        [Test]
        public void SourceInfo_MySourceIsNull_EmptyListsCreatedAndColumnInfosNotCalled()
        {
            //Arrange
            //_dataSource = null;
            //Act
            SourceInfo sourceInfo = new SourceInfo("MyOriginalName", "MyNewName", (DataSource)null);
            //Asssert
            _dataSource.Verify(ds => ds.ColumnInfos(ColType.Flaw), Times.Never);
            _dataSource.Verify(ds => ds.ColumnInfos(ColType.Response), Times.Never);
        }
        [Test]
        public void SourceInfo_MySourceIsNOTNull_ListsAssignedAndColumnInfosNotCalled()
        {
            //Arrange
            //Act
            SourceInfo sourceInfo = new SourceInfo("MyOriginalName", "MyNewName", _dataSource.Object);
            //Asssert
            _dataSource.Verify(ds => ds.ColumnInfos(ColType.Flaw), Times.Exactly(1));
            _dataSource.Verify(ds => ds.ColumnInfos(ColType.Response), Times.Exactly(1));
        }
        /// Tests for the GetInfos(ColType myType) function
        [Test]
        public void GetInfos_ColTypeIsFlaw_ReturnsFlawsColumnInfo()
        {
            //Arrange
            _sourceInfo = new SourceInfo("MyOriginalName", "MyNewName", _dataSource.Object);
            //Act
            var result = _sourceInfo.GetInfos(ColType.Flaw);
            //Assert
            Assert.That(result, Is.EqualTo(_flawInfo));
        }
        [Test]
        public void GetInfos_ColTypeIsResponse_ReturnsResponseColumnInfo()
        {
            //Arrange
            _sourceInfo = new SourceInfo("MyOriginalName", "MyNewName", _dataSource.Object);
            //Act
            var result = _sourceInfo.GetInfos(ColType.Response);
            //Assert
            Assert.That(result, Is.EqualTo(_responseInfo));
        }
    }
}
