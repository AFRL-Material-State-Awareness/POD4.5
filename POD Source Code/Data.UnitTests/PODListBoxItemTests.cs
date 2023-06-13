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
        private PODListBoxItemWithProps _podlistboxItemWithProps;
        [SetUp]
        public void Setup()
        {
            _podListBoxItem = new PODListBoxItem();
        }
        /// <summary>
        /// Tests for the ToString method
        /// </summary>
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
            _podListBoxItem.ResponseColumnName = "MyColumnName";
            //Act
            var result = _podListBoxItem.ToString();
            //Assert
            Assert.That(result, Is.EqualTo("MyColumnName"));
        }
        [Test]
        public void ToString_DataSourceNameIsEmptyStringAndFlawResponseColumnNameNotEmptyString_ReturnsFlawDOTResponseName()
        {
            //Arrange
            _podListBoxItem.DataSourceName = string.Empty;
            _podListBoxItem.FlawColumnName = "MyFlawColumnName";
            _podListBoxItem.ResponseColumnName = "MyColumnName";
            //Act
            var result = _podListBoxItem.ToString();
            //Assert
            Assert.That(result, Is.EqualTo("MyFlawColumnName.MyColumnName"));
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
            _podListBoxItem.ResponseColumnName = "MyColumnName";
            //Act
            var result = _podListBoxItem.ToString();
            //Assert
            Assert.That(result, Is.EqualTo("MyDataSource.MyColumnName"));
        }
        [Test]
        public void ToString_DataSourceNameIsNOTEmptyStringAndFlawNameNotEmptyStringColumnNotEmptyString_ReturnsDataSourceNameDotFlawName()
        {
            //Arrange
            _podListBoxItem.DataSourceName = "MyDataSource";
            _podListBoxItem.FlawColumnName = "MyFlawColumnName";
            _podListBoxItem.ResponseColumnName = "MyColumnName";
            //Act
            var result = _podListBoxItem.ToString();
            //Assert
            Assert.That(result, Is.EqualTo("MyDataSource.MyFlawColumnName.MyColumnName"));
        }
        /// Tests for the _podlistboxItemWithProps constructor
        [Test]
        public void PODListBoxItemWithProps_BoxtypeResponse_AssignedToResponseColumnNameAndLeavesFlawNamesEmpty()
        {
            //Arrange
            //Act
            _podlistboxItemWithProps = new PODListBoxItemWithProps(System.Drawing.Color.Black, ColType.Response, "MyColumnName", "OriginalResponseColumnName",
                "DataSource", "cm", 0.0, 1.0, 0.5);
            //Assert
            Assert.That(_podlistboxItemWithProps.BoxType, Is.EqualTo(ColType.Response));
            Assert.That(_podlistboxItemWithProps.FlawColumnName, Is.EqualTo(""));
            Assert.That(_podlistboxItemWithProps.FlawOriginalName, Is.EqualTo(""));
            Assert.That(_podlistboxItemWithProps.ResponseColumnName, Is.EqualTo("MyColumnName"));
            Assert.That(_podlistboxItemWithProps.ResponseOriginalName, Is.EqualTo("OriginalResponseColumnName"));
            Assert.That(_podlistboxItemWithProps.Threshold, Is.EqualTo(0.5));

            GenerateAssertionsForPODListItemWithPropsConstructor();
        }
        [Test]
        public void PODListBoxItemWithProps_BoxtypeFlaw_AssignedToFlawColumnNameAndLeavesFlawNamesEmpty()
        {
            //Arrange
            //Act
            _podlistboxItemWithProps = new PODListBoxItemWithProps(System.Drawing.Color.Black, ColType.Flaw, "MyColumnName", "OriginalResponseColumnName",
                "DataSource", "cm", 0.0, 1.0, 0.5);
            //Assert
            Assert.That(_podlistboxItemWithProps.BoxType, Is.EqualTo(ColType.Flaw));
            Assert.That(_podlistboxItemWithProps.FlawColumnName, Is.EqualTo("MyColumnName"));
            Assert.That(_podlistboxItemWithProps.FlawOriginalName, Is.EqualTo("OriginalResponseColumnName"));
            Assert.That(_podlistboxItemWithProps.ResponseColumnName, Is.EqualTo(""));
            Assert.That(_podlistboxItemWithProps.ResponseOriginalName, Is.EqualTo(""));
            Assert.That(_podlistboxItemWithProps.Threshold, Is.EqualTo(0.0));

            GenerateAssertionsForPODListItemWithPropsConstructor();
        }
        private void GenerateAssertionsForPODListItemWithPropsConstructor()
        {
            Assert.That(_podlistboxItemWithProps.RowColor, Is.EqualTo(System.Drawing.Color.Black));
            
            Assert.That(_podlistboxItemWithProps.DataSourceName, Is.EqualTo("DataSource"));
            Assert.That(_podlistboxItemWithProps.DataSourceOriginalName, Is.EqualTo("DataSource"));
            Assert.That(_podlistboxItemWithProps.Unit, Is.EqualTo("cm"));
            Assert.That(_podlistboxItemWithProps.Min, Is.EqualTo(0.0));
            Assert.That(_podlistboxItemWithProps.Max, Is.EqualTo(1.0));
        }
        /// tests for the GetColumnNameFunction
        [Test]
        [TestCase(ColType.Flaw, "FlawColumnName")]
        [TestCase(ColType.Response, "ResponseColumnName")]
        public void GetColumnName_BoxTypeValid_ReturnsTheCorrespondingName(ColType coltype, string expectedName)
        {
            //Arrange
            _podlistboxItemWithProps = new PODListBoxItemWithProps() { BoxType = coltype, FlawColumnName = "FlawColumnName", ResponseColumnName = "ResponseColumnName" };
            //Act
            var result = _podlistboxItemWithProps.GetColumnName();
            //Arrange
            Assert.That(result, Is.EqualTo(expectedName));
        }
        /// Tests for the toString Method
        [Test]
        public void ToString_HideExtendedTextIsFalse_ReturnsTheBaseToString()
        {
            //Arrange
            _podlistboxItemWithProps = new PODListBoxItemWithProps(System.Drawing.Color.Black, ColType.Flaw, "MyColumnName", "OriginalResponseColumnName",
                "DataSource", "cm", 0.0, 1.0, 0.5);
            _podlistboxItemWithProps.HideExtendedText = true;
            //Act
            var result = _podlistboxItemWithProps.ToString();
            //Assert
            Assert.That(result, Is.EqualTo("DataSource.MyColumnName"));
        }
        [Test]
        public void ToString_HideExtendedTextIsTrueAndLengthOfUnitIs0AndBoxTypeIsNotResponse_OverwritesTextToTextPlusFlawLabelMinMax()
        {
            //Arrange
            _podlistboxItemWithProps = new PODListBoxItemWithProps(System.Drawing.Color.Black, ColType.Flaw, "MyColumnName", "OriginalResponseColumnName",
                "DataSource", "", 0.0, 1.0, 0.5);
            _podlistboxItemWithProps.HideExtendedText = false;
            //Act
            var result = _podlistboxItemWithProps.ToString();
            //Assert
            Assert.That(result, Is.EqualTo("DataSource.MyColumnName [0,1]"));
        }
        [Test]
        public void ToString_HideExtendedTextIsTrueAndLengthOfUnitIsGreaterThan0AndBoxTypeIsNotResponse_OverwritesTextToTextPlusUnitPlusFlawLabelMinMax()
        {
            //Arrange
            _podlistboxItemWithProps = new PODListBoxItemWithProps(System.Drawing.Color.Black, ColType.Flaw, "MyColumnName", "OriginalResponseColumnName",
                "DataSource", "cm", 0.0, 1.0, 0.5);
            _podlistboxItemWithProps.HideExtendedText = false;
            //Act
            var result = _podlistboxItemWithProps.ToString();
            //Assert
            Assert.That(result, Is.EqualTo("DataSource.MyColumnName (cm), [0,1]"));
        }
        [Test]
        public void ToString_HideExtendedTextIsTrueAndLengthOfUnitIsGreaterThan0AndBoxTypeIsResponse_OverwritesTextToTextPlusUnitPlusFlawLabelMinMaxPlusThreshold()
        {
            //Arrange
            _podlistboxItemWithProps = new PODListBoxItemWithProps(System.Drawing.Color.Black, ColType.Response, "MyColumnName", "OriginalResponseColumnName",
                "DataSource", "cm", 0.0, 1.0, 0.5);
            _podlistboxItemWithProps.HideExtendedText = false;
            //Act
            var result = _podlistboxItemWithProps.ToString();
            //Assert
            Assert.That(result, Is.EqualTo("DataSource.MyColumnName (cm), [0,1], 0.5"));
        }
        /// Tests for the SetStringValue(InfoType myDataType, string myValue) function
        [Test]
        public void SetStringValue_InvalidValuePassedFlaw_ReturnsMyValuedTrimmed()
        {
            //Arrange
            _podlistboxItemWithProps = new PODListBoxItemWithProps(System.Drawing.Color.Black, ColType.Flaw, "MyColumnName", "OriginalResponseColumnName",
                "DataSource", "cm", 0.0, 1.0, 0.5);
            //Act
            var result =_podlistboxItemWithProps.SetStringValue(InfoType.OriginalName, "  MyNewName  ");
            //Assert
            Assert.That(result, Is.EqualTo("MyNewName"));
            Assert.That(_podlistboxItemWithProps.Unit, Is.EqualTo("cm"));
            Assert.That(_podlistboxItemWithProps.FlawColumnName, Is.EqualTo("MyColumnName"));

        }
        [Test]
        public void SetStringValue_InvalidValuePassedResponse_ReturnsMyValuedTrimmed()
        {
            //Arrange
            _podlistboxItemWithProps = new PODListBoxItemWithProps(System.Drawing.Color.Black, ColType.Response, "MyColumnName", "OriginalResponseColumnName",
                "DataSource", "cm", 0.0, 1.0, 0.5);
            //Act
            var result = _podlistboxItemWithProps.SetStringValue(InfoType.OriginalName, "  MyNewName  ");
            //Assert
            Assert.That(result, Is.EqualTo("MyNewName"));
            Assert.That(_podlistboxItemWithProps.Unit, Is.EqualTo("cm"));
            Assert.That(_podlistboxItemWithProps.ResponseColumnName, Is.EqualTo("MyColumnName"));
        }
        [Test]
        public void SetStringValue_UnitInfoTypePassed_ReturnsMyValuedTrimmedAndAssignsUnit()
        {
            //Arrange
            _podlistboxItemWithProps = new PODListBoxItemWithProps(System.Drawing.Color.Black, ColType.Response, "MyColumnName", "OriginalResponseColumnName",
                "DataSource", "cm", 0.0, 1.0, 0.5);
            //Act
            var result = _podlistboxItemWithProps.SetStringValue(InfoType.Unit, "  MyNewUnit  ");
            //Assert
            Assert.That(result, Is.EqualTo("MyNewUnit"));
            Assert.That(_podlistboxItemWithProps.Unit, Is.EqualTo("MyNewUnit"));
            Assert.That(_podlistboxItemWithProps.ResponseColumnName, Is.EqualTo("MyColumnName"));
        }
        [Test]
        public void SetStringValue_InfoTypeNewNameAndColumnnamePassedFlaw_ReturnsMyValuedTrimmedAndAssignsColumnNameAccordingly()
        {
            //Arrange
            _podlistboxItemWithProps = new PODListBoxItemWithProps(System.Drawing.Color.Black, ColType.Flaw, "MyColumnName", "OriginalResponseColumnName",
                "DataSource", "cm", 0.0, 1.0, 0.5);
            //Act
            var result = _podlistboxItemWithProps.SetStringValue(InfoType.NewName, "  MynewColumnName  ");
            //Assert
            Assert.That(result, Is.EqualTo("MynewColumnName"));
            Assert.That(_podlistboxItemWithProps.Unit, Is.EqualTo("cm"));
            Assert.That(_podlistboxItemWithProps.FlawColumnName, Is.EqualTo("MynewColumnName"));
        }
        [Test]
        public void SetStringValue_InfoTypeNewNameAndColumnnamePassedResponse_ReturnsMyValuedTrimmedAndAssignsColumnNameAccordingly()
        {
            //Arrange
            _podlistboxItemWithProps = new PODListBoxItemWithProps(System.Drawing.Color.Black, ColType.Response, "MyColumnName", "OriginalResponseColumnName",
                "DataSource", "cm", 0.0, 1.0, 0.5);
            //Act
            var result = _podlistboxItemWithProps.SetStringValue(InfoType.NewName, "  MynewColumnName  ");
            //Assert
            Assert.That(result, Is.EqualTo("MynewColumnName"));
            Assert.That(_podlistboxItemWithProps.Unit, Is.EqualTo("cm"));
            Assert.That(_podlistboxItemWithProps.ResponseColumnName, Is.EqualTo("MynewColumnName"));
        }
        /// Tests for the SetDoubleValue(InfoType myDataType, double myValue) function
        /*
        [Test]
        public void SetDoubleValue_DataTypeIsInvalid_returnsMyValuesWithNoAssignments()
        {
            //Arrange
            _podlistboxItemWithProps = new PODListBoxItemWithProps(System.Drawing.Color.Black, ColType.Response, "MyColumnName", "OriginalResponseColumnName",
                "DataSource", "cm", 0.0, 1.0, 0.5);
        }
        */

    }
}
