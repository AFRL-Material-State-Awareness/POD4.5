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
    public class PODListBoxItemTests
    {
        private PODListBoxItem _podListBoxItem;
        [SetUp]
        public void Setup()
        {
            _podListBoxItem = new PODListBoxItem();
        }
        [Test]
        public void ToString_DataSourceNameIsEmptyStringAndFlawResponseColumnNameIsEmptyString_ReturnsDataSourceName()
        {
            //Arrange
            _podListBoxItem.DataSourceName = string.Empty;
            _podListBoxItem.FlawColumnName = string.Empty;
            _podListBoxItem.ResponseColumnName = string.Empty;
            //Act
            var result = _podListBoxItem.ToString();
            //Assert
            Assert.That(result, Is.EqualTo(""));
        }
        [Test]
        public void ToString_DataSourceNameIsEmptyStringAndFlawColumnNameNotEmptyString_ReturnsFlawName()
        {
            //Arrange
            _podListBoxItem.DataSourceName = string.Empty;
            _podListBoxItem.FlawColumnName = "MyFlawColumnName";
            _podListBoxItem.ResponseColumnName = string.Empty;
            //Act
            var result = _podListBoxItem.ToString();
            //Assert
            Assert.That(result, Is.EqualTo("MyFlawColumnName"));
        }
        [Test]
        public void ToString_DataSourceNameIsEmptyStringAndResponseColumnNameNotEmptyString_ReturnsResponseName()
        {
            //Arrange
            _podListBoxItem.DataSourceName = string.Empty;
            _podListBoxItem.FlawColumnName = string.Empty;
            _podListBoxItem.ResponseColumnName = "MyResponseColumnName";
            //Act
            var result = _podListBoxItem.ToString();
            //Assert
            Assert.That(result, Is.EqualTo("MyResponseColumnName"));
        }
        [Test]
        public void ToString_DataSourceNameIsEmptyStringAndFlawResponseColumnNameNotEmptyString_ReturnsFlawDOTResponseName()
        {
            //Arrange
            _podListBoxItem.DataSourceName = string.Empty;
            _podListBoxItem.FlawColumnName = "MyFlawColumnName";
            _podListBoxItem.ResponseColumnName = "MyResponseColumnName";
            //Act
            var result = _podListBoxItem.ToString();
            //Assert
            Assert.That(result, Is.EqualTo("MyFlawColumnName.MyResponseColumnName"));
        }
        [Test]
        public void ToString_DataSourceNameIsNOTEmptyStringAndFlawResponseColumnNameIsEmptyString_ReturnsDataSourceName()
        {
            //Arrange
            _podListBoxItem.DataSourceName = "MyDataSource";
            _podListBoxItem.FlawColumnName = string.Empty;
            _podListBoxItem.ResponseColumnName = string.Empty;
            //Act
            var result = _podListBoxItem.ToString();
            //Assert
            Assert.That(result, Is.EqualTo("MyDataSource"));
        }
        [Test]
        public void ToString_DataSourceNameIsNOTEmptyStringAndFlawNameNotEmptyStringColumnISEmptyString_ReturnsDataSourceNameDotFlawName()
        {
            //Arrange
            _podListBoxItem.DataSourceName = "MyDataSource";
            _podListBoxItem.FlawColumnName = "MyFlawColumnName";
            _podListBoxItem.ResponseColumnName = string.Empty;
            //Act
            var result = _podListBoxItem.ToString();
            //Assert
            Assert.That(result, Is.EqualTo("MyDataSource.MyFlawColumnName"));
        }
        [Test]
        public void ToString_DataSourceNameIsNOTEmptyStringAndFlawNameISEmptyStringColumnNotEmptyString_ReturnsDataSourceNameDotFlawName()
        {
            //Arrange
            _podListBoxItem.DataSourceName = "MyDataSource";
            _podListBoxItem.FlawColumnName = string.Empty;
            _podListBoxItem.ResponseColumnName = "MyResponseColumnName";
            //Act
            var result = _podListBoxItem.ToString();
            //Assert
            Assert.That(result, Is.EqualTo("MyDataSource.MyResponseColumnName"));
        }
        [Test]
        public void ToString_DataSourceNameIsNOTEmptyStringAndFlawNameNotEmptyStringColumnNotEmptyString_ReturnsDataSourceNameDotFlawName()
        {
            //Arrange
            _podListBoxItem.DataSourceName = "MyDataSource";
            _podListBoxItem.FlawColumnName = "MyFlawColumnName";
            _podListBoxItem.ResponseColumnName = "MyResponseColumnName";
            //Act
            var result = _podListBoxItem.ToString();
            //Assert
            Assert.That(result, Is.EqualTo("MyDataSource.MyFlawColumnName.MyResponseColumnName"));
        }


    }
}
