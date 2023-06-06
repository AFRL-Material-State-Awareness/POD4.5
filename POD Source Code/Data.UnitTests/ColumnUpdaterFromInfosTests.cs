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

namespace Data.UnitTests
{
    [TestFixture]
    public class ColumnUpdaterFromInfosTests
    {
        private Mock<IGetPreviousValueControl> _previousValueControl;
        private ColumnUpdaterFromInfos _columnUpdaterFromInfos;
        ColumnInfo _myColumnInfo;
        [SetUp]
        public void Setup()
        {
            _previousValueControl = new Mock<IGetPreviousValueControl>();
            _columnUpdaterFromInfos = new ColumnUpdaterFromInfos(_previousValueControl.Object);
            _myColumnInfo = new ColumnInfo("Oldname", "NewName", "cm", 0, 1, 0) { Threshold = .5 };
            // Mock returns prev value of min=10, max=100, and threshold = 50
            _previousValueControl.Setup(pvc => pvc.GetPreviousValue(It.IsAny<DataColumn>(), It.IsAny<string>(),
                _myColumnInfo, InfoType.Min, It.IsAny<double>())).Returns(10);
            _previousValueControl.Setup(pvc => pvc.GetPreviousValue(It.IsAny<DataColumn>(), It.IsAny<string>(),
                _myColumnInfo, InfoType.Max, It.IsAny<double>())).Returns(100);
            _previousValueControl.Setup(pvc => pvc.GetPreviousValue(It.IsAny<DataColumn>(), It.IsAny<string>(),
                _myColumnInfo, InfoType.Threshold, It.IsAny<double>())).Returns(50);
        }
        /// test For void UpdateColumnFromInfo(DataColumn column, ColumnInfo info) function
        [Test]
        public void UpdateColumnFromInfo_ValidColumnAndInfoPased_AddsExcelProperties()
        {
            //Arrange
            DataColumn myColumn = new DataColumn();
            //Act
            _columnUpdaterFromInfos.UpdateColumnFromInfo(myColumn, _myColumnInfo);
            //Assert
            _previousValueControl.Verify(pvc => pvc.GetPreviousValue(It.IsAny<DataColumn>(), It.IsAny<string>(),
                _myColumnInfo, It.IsAny<InfoType>(), It.IsAny<double>()), Times.Exactly(3));
            Assert.That(myColumn.ExtendedProperties[ExtColProperty.Min], Is.EqualTo("0"));
            Assert.That(myColumn.ExtendedProperties[ExtColProperty.Max], Is.EqualTo("1"));
            Assert.That(myColumn.ExtendedProperties[ExtColProperty.Thresh], Is.EqualTo("0.5"));
            Assert.That(myColumn.ExtendedProperties[ExtColProperty.MinPrev], Is.EqualTo("10"));
            Assert.That(myColumn.ExtendedProperties[ExtColProperty.MaxPrev], Is.EqualTo("100"));
            Assert.That(myColumn.ExtendedProperties[ExtColProperty.ThreshPrev], Is.EqualTo("50"));
            Assert.That(myColumn.ExtendedProperties[ExtColProperty.Unit], Is.EqualTo("cm"));
        }
    }
}
