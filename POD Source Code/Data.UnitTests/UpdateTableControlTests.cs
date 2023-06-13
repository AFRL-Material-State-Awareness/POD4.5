using NUnit.Framework;
using System;
using Moq;
using POD.Data;
using System.Collections.Generic;
using System.Data;
using POD;
using CSharpBackendWithR;
using POD.ExcelData;
using static POD.Data.SortPoint;
using System.Linq;
using SpreadsheetLight;

namespace Data.UnitTests
{
    [TestFixture]
    public class UpdateTableControlTests
    {
        private UpdateTableControl _updateTabControl;
        private Mock<IAnalysisData> _data;
        [SetUp]
        public void Setup()
        {
            _data = new Mock<IAnalysisData>();
            _updateTabControl = new UpdateTableControl(_data.Object);
        }
        [Test]
        public void UpdateTable_InBoundsFlagPassed_CallsTurnOnPoint()
        {
            //Arrange
            //Act
            _updateTabControl.UpdateTable(1, 2, Flag.InBounds);
            //Assert
            _data.Verify(d => d.TurnOnPoint(2, 1));
        }
        [Test]
        public void UpdateTable_OutBoundsFlagPassed_CallsTurnOffPoint()
        {
            //Arrange
            //Act
            _updateTabControl.UpdateTable(1, 2, Flag.OutBounds);
            //Assert
            _data.Verify(d => d.TurnOffPoint(2, 1));
        }
        [Test]
        public void UpdateTable_InvalidFlagPassed_ThrowsAnArgumentOutOfRangeException()
        {
            //Arrange
            //Act
            //Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => _updateTabControl.UpdateTable(It.IsAny<int>(), It.IsAny<int>(), Flag.None));
            
        }
    }
}
