﻿using System;
using NUnit.Framework;
using Moq;
using POD;
using System.Linq;
using System.Data;
namespace Data.UnitTests
{
    [TestFixture]
    public class UpdaterExcelPropertyValueTests
    {
        private UpdaterExcelPropertyValue _updateExcelPropVal;
        private Mock<IGetExtendedPropertyControl> _getExtendedProp;
        [SetUp]
        public void Setup()
        {
            _getExtendedProp = new Mock<IGetExtendedPropertyControl>();
            _updateExcelPropVal = new UpdaterExcelPropertyValue(_getExtendedProp.Object);

        }
        /// tests for GetUpdatedValue(string myExtColProperty, double currentValue, DataColumn column)
        [Test]
        public void GetUpdatedValue_InvalidExtColProperty_ThrowsException()
        {
            //Arrange
            //Act
            //Assert
            Assert.Throws<ArgumentException>(()=>_updateExcelPropVal.GetUpdatedValue("InvalidExtProp", 1.0, new DataColumn()));
        }
        [Test]
        [TestCase(ExtColProperty.MinPrev, ExtColProperty.Min, "", "", 1.0)]
        [TestCase(ExtColProperty.MaxPrev, ExtColProperty.Max, "", "", 1.0)]
        [TestCase(ExtColProperty.ThreshPrev, ExtColProperty.Thresh, "", "", 1.0)]
        [TestCase(ExtColProperty.MinPrev, ExtColProperty.Min, "1.0", "", 0.0)]
        [TestCase(ExtColProperty.MinPrev, ExtColProperty.Min, "", "2.0", 1.0)]
        [TestCase(ExtColProperty.MinPrev, ExtColProperty.Min, "1.0", "2.0", 2.0)]
        [TestCase(ExtColProperty.MaxPrev, ExtColProperty.Max, "1.0", "", 0.0)]
        [TestCase(ExtColProperty.MaxPrev, ExtColProperty.Max, "", "2.0", 1.0)]
        [TestCase(ExtColProperty.MaxPrev, ExtColProperty.Max, "1.0", "2.0", 2.0)]
        [TestCase(ExtColProperty.ThreshPrev, ExtColProperty.Thresh, "1.0", "", 0.0)]
        [TestCase(ExtColProperty.ThreshPrev, ExtColProperty.Thresh, "", "2.0", 1.0)]
        [TestCase(ExtColProperty.ThreshPrev, ExtColProperty.Thresh, "1.0", "2.0", 2.0)]
        public void GetUpdatedValue_ValidExtPropertyPassed_TryParsesDoublesAndReturnsAccordingly(string prevString,
            string extColumnProp, string returnPrevSring, string returnExtColProp, double expectedResult)
        {
            //Arrange
            DataColumn dataColumn = new DataColumn();
            _getExtendedProp.Setup(gep => gep.GetExtendedProperty(dataColumn, prevString)).Returns(returnPrevSring);
            _getExtendedProp.Setup(gep => gep.GetExtendedProperty(dataColumn, extColumnProp)).Returns(returnExtColProp);
            //Act
            var result = _updateExcelPropVal.GetUpdatedValue(extColumnProp, 1.0, dataColumn);
            //Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }
        /// Tests for GetNewValue(string myExtColProperty, DataColumn column) function
        [Test]
        [TestCase(ExtColProperty.Min)]
        [TestCase(ExtColProperty.Max)]
        [TestCase(ExtColProperty.Thresh)]
        [TestCase(ExtColProperty.Unit)]
        public void GetNewValue_ExtendedPropertiesAttributeIsNull_Returns0(string extColProperty)
        {
            //Arrange
            DataColumn dataColumn = new DataColumn();
            //Act
            var result = _updateExcelPropVal.GetNewValue(extColProperty, dataColumn);
            //Assert
            Assert.That(result, Is.Zero);
        }
        [Test]
        [TestCase(ExtColProperty.Min)]
        [TestCase(ExtColProperty.Max)]
        [TestCase(ExtColProperty.Thresh)]
        [TestCase(ExtColProperty.Unit)]
        public void GetNewValue_ExtendedPropertiesContainsANonParseDoubleString_Returns0(string extColProperty)
        {
            //Arrange
            DataColumn dataColumn = new DataColumn();
            dataColumn.ExtendedProperties[ExtColProperty.Max] = "";
            //Act
            var result = _updateExcelPropVal.GetNewValue(extColProperty, dataColumn);
            //Assert
            Assert.That(result, Is.Zero);
        }
        [Test]
        [TestCase(ExtColProperty.Min)]
        [TestCase(ExtColProperty.Max)]
        [TestCase(ExtColProperty.Thresh)]
        [TestCase(ExtColProperty.Unit)]
        public void GetNewValue_ExtendedPropertiesContainsAParseDoubleString_ReturnsParsedDouble(string extColProperty)
        {
            //Arrange
            DataColumn dataColumn = new DataColumn();
            dataColumn.ExtendedProperties[extColProperty] = "1.0";
            //Act
            var result = _updateExcelPropVal.GetNewValue(extColProperty, dataColumn);
            //Assert
            Assert.That(result, Is.EqualTo(1.0));
        }
        [Test]
        [TestCase(ExtColProperty.Min)]
        [TestCase(ExtColProperty.Max)]
        [TestCase(ExtColProperty.Thresh)]
        [TestCase(ExtColProperty.Unit)]
        public void GetNewValue_ExtendedPropertiesContainsAParseDoubleNONString_ReturnsParsedDouble(string extColProperty)
        {
            //Arrange
            DataColumn dataColumn = new DataColumn();
            dataColumn.ExtendedProperties[extColProperty] = 1.0;
            //Act
            var result = _updateExcelPropVal.GetNewValue(extColProperty, dataColumn);
            //Assert
            Assert.That(result, Is.EqualTo(1.0));
        }

    }
}
