using NUnit.Framework;
using Moq;
using POD.Data;
using System.Data;
using POD;


namespace Data.UnitTests
{
    [TestFixture]
    public class GetPreviousValueControlTests
    {
        private GetPreviousValueControl _prevValueControl;
        private Mock<IColumnInfo> _columnInfo;
        private DataColumn _column;
        [SetUp]
        public void Setup()
        {
            _prevValueControl = new GetPreviousValueControl();
            _columnInfo = new Mock<IColumnInfo>();
            _column = new DataColumn();
        }
        /// <summary>
        ///  Tests for the GetPreviousValue(DataColumn column, string colType, ColumnInfo info, InfoType infoType, double defaultValue) function
        /// </summary>
        [Test]
        public void GetPreviousValue_ColumnTypeExtInvalidAndColumnIsNullAndPrevValueEqualsDefualtValue_CallsGetDoubleValueAndReturns1()
        {
            //Arrange
            _columnInfo.Setup(ci => ci.GetDoubleValue(It.IsAny<InfoType>())).Returns(1.0);
            //Act
            var result = _prevValueControl.GetPreviousValue(null, "", _columnInfo.Object, InfoType.OriginalName, 0.0);
            //Assert
            _columnInfo.Verify(ci => ci.GetDoubleValue(It.IsAny<InfoType>()), Times.Once);
            Assert.That(result, Is.EqualTo(1));
        }
        [Test]
        public void GetPreviousValue_ColumnTypeExtInvalidandColumnIsNullAndPrevValueNotEqualToDefaultValue_NoCallToGetDoubleValueAndReturns0()
        {
            //Arrange
            //Act
            var result = _prevValueControl.GetPreviousValue(null, "", _columnInfo.Object, InfoType.OriginalName, 1.0);
            //Assert
            _columnInfo.Verify(ci => ci.GetDoubleValue(It.IsAny<InfoType>()), Times.Never);
            Assert.That(result, Is.Zero);
        }
        [Test]
        public void GetPreviousValue_ColumnTypeExtInvalidAndColumnIsNotNullAndContainsKey_ExtendedPropertyKeyNotReassignedAndReturns0()
        {
            //Arrange
            _column.ExtendedProperties[""] = 1.0;
            //_column.ExtendedProperties["AMatchingKey"] = 10.0;
            //Act
            var result = _prevValueControl.GetPreviousValue(_column, "AMatchingKey", _columnInfo.Object, InfoType.OriginalName, 5.0);
            //Assert
            _columnInfo.Verify(ci => ci.GetDoubleValue(It.IsAny<InfoType>()), Times.Never);
            Assert.That(result, Is.Zero);
            Assert.That(_column.ExtendedProperties.ContainsKey(""), Is.True);
            Assert.That(_column.ExtendedProperties[""], Is.EqualTo(1.0));
        }
        [Test]
        public void GetPreviousValue_ColumnTypeIsValidForGetDefaultValueAndColumnIsNotNullAndDoesContainKey_ExtendedPropertyKeyAssignedAndReturnsGetDoubleValue()
        {
            //Arrange
            _column.ExtendedProperties["ANonMatchingKey"] = 10.0;
            _columnInfo.Setup(ci => ci.GetDoubleValue(It.IsAny<InfoType>())).Returns(99.0);
            //Act
            var result = _prevValueControl.GetPreviousValue(_column, ExtColProperty.Unit, _columnInfo.Object, InfoType.OriginalName, 1.0);
            //Assert
            _columnInfo.Verify(ci => ci.GetDoubleValue(It.IsAny<InfoType>()), Times.Once);
            Assert.That(result, Is.EqualTo(99.0));
            Assert.That(_column.ExtendedProperties.ContainsKey(ExtColProperty.Unit), Is.True);
            Assert.That(_column.ExtendedProperties[""], Is.EqualTo(1.0));
        }
        [Test]
        [TestCase(ExtColProperty.Min, 10.0)]
        [TestCase(ExtColProperty.Max, 100.0)]
        [TestCase(ExtColProperty.Thresh, 50.0)]
        public void GetPreviousValue_ColumnTypeExtValidAndColumnIsNull_ReturnsAppropriateDefaultValue(string extProperty, double expectedResult)
        {
            //Arrange
            SetupDefaults();
            //Act
            var result = _prevValueControl.GetPreviousValue(null, extProperty, _columnInfo.Object, InfoType.OriginalName, 1.0);
            //Assert
            _columnInfo.Verify(ci => ci.GetDoubleValue(It.IsAny<InfoType>()), Times.Never);
            Assert.That(result, Is.EqualTo(expectedResult));
        }
        [Test]
        [TestCase(ExtColProperty.Min, 10.0)]
        [TestCase(ExtColProperty.Max, 100.0)]
        [TestCase(ExtColProperty.Thresh, 50.0)]
        public void GetPreviousValue_ColumnTypeExtValidAndColumnIsNotNullAndDoesNotContainKey_ReturnsGetDoubleValue(string extProperty, double expectedResult)
        {
            //Arrange
            SetupDefaults();
            _columnInfo.Setup(ci => ci.GetDoubleValue(It.IsAny<InfoType>())).Returns(1.0);
            //Act
            var result = _prevValueControl.GetPreviousValue(_column, extProperty, _columnInfo.Object, InfoType.OriginalName, expectedResult);
            //Assert
            _columnInfo.Verify(ci => ci.GetDoubleValue(It.IsAny<InfoType>()), Times.Once);
            Assert.That(result, Is.EqualTo(1.0));
        }
        [Test]
        [TestCase(ExtColProperty.Min, ExtColProperty.MinPrev, 10.0)]
        [TestCase(ExtColProperty.Max, ExtColProperty.MaxPrev, 100.0)]
        [TestCase(ExtColProperty.Thresh, ExtColProperty.ThreshPrev, 50.0)]
        public void GetPreviousValue_ColumnTypeExtValidAndColumnIsNotNullAndContainsKeyPrev_ReturnsCurrentValueAndGetDoubleValueNotCalled(string extProperty, string extPrevProperty, double expectedResult)
        {
            //Arrange
            SetupDefaults();
            _columnInfo.Setup(ci => ci.GetDoubleValue(It.IsAny<InfoType>())).Returns(1.0);
            _column.ExtendedProperties[extPrevProperty] = 2.0;
            //Act
            var result = _prevValueControl.GetPreviousValue(_column, extProperty, _columnInfo.Object, InfoType.OriginalName, 3.0);
            //Assert
            _columnInfo.Verify(ci => ci.GetDoubleValue(It.IsAny<InfoType>()), Times.Never);
            Assert.That(result, Is.EqualTo(expectedResult));
        }
        [Test]
        [TestCase(ExtColProperty.Min, ExtColProperty.MinPrev)]
        [TestCase(ExtColProperty.Max, ExtColProperty.MaxPrev)]
        [TestCase(ExtColProperty.Thresh, ExtColProperty.ThreshPrev)]
        public void GetPreviousValue_ColumnTypeExtValidAndColumnIsNotNullAndContainsKeyDefaultAndPrev_ReturnsValuesAlreadyAssignedAndGetDoubleValueNotCalled(string extProperty, 
            string extPrevProperty)
        {
            //Arrange
            SetupDefaults();
            _columnInfo.Setup(ci => ci.GetDoubleValue(It.IsAny<InfoType>())).Returns(1.0);
            _column.ExtendedProperties[extProperty] = 1.5;
            _column.ExtendedProperties[extPrevProperty] = 2.0;
            //Act
            var result = _prevValueControl.GetPreviousValue(_column, extProperty, _columnInfo.Object, InfoType.OriginalName, 3.0);
            //Assert
            _columnInfo.Verify(ci => ci.GetDoubleValue(It.IsAny<InfoType>()), Times.Never);
            Assert.That(result, Is.EqualTo(1.5));
        }
        private void SetupDefaults()
        {
            ExtColProperty.MaxDefault = 100.0;
            ExtColProperty.MinDefault = 10.0;
            ExtColProperty.ThreshDefault = 50.0;
        }
    }
}
