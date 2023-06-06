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
    public class AddRowToDataTableControlTests
    {
        private AddRowToTableControl _addRowToTableCon;
        private DataTable _datatableString;
        private DataTable _datatableDouble;
        [SetUp]
        public void Setup()
        {
            _addRowToTableCon = new AddRowToTableControl();
            _datatableString = new DataTable();
            _datatableString.Columns.Add("Column1");
            _datatableString.Rows.Add("MyString");
            _datatableDouble = new DataTable();
            _datatableDouble.Columns.Add(new DataColumn("Column1") { DataType = typeof(double)});
            _datatableDouble.Rows.Add(1.2);
        }
        ///  Tests for the AddStringRowToTable(string myID, int index, DataTable table) function
        [Test]
        public void AddStringRowToTable_TableRowsCountGreaterThanIndex_DoesNotAddARowAndAssignsTheIDString()
        {
            //Act
            _addRowToTableCon.AddStringRowToTable("MyIDString", 0, _datatableString);
            //Assert
            Assert.That(_datatableString.Rows.Count, Is.EqualTo(1));
            Assert.That(_datatableString.Rows[0][0], Is.EqualTo("MyIDString"));
        }
        [Test]
        public void AddStringRowToTable_TableRowsCountGreaterThanIndex_AddsTheRowAndAssignsTheIDString()
        {
            //Act
            _addRowToTableCon.AddStringRowToTable("MyIDString", 1, _datatableString);
            //Assert
            Assert.That(_datatableString.Rows.Count, Is.EqualTo(2));
            Assert.That(_datatableString.Rows[1][0], Is.EqualTo("MyIDString"));
        }
        ///  Tests for the AddDoubleRowToTable(double myValues, int index, DataTable table) function
        [Test]
        public void AddDoubleRowToTable_TableRowsCountGreaterThanIndex_DoesNotAddARowAndAssignsTheIDString()
        {
            //Act
            _addRowToTableCon.AddDoubleRowToTable(1.2, 0, _datatableDouble);
            //Assert
            Assert.That(_datatableDouble.Rows.Count, Is.EqualTo(1));
            Assert.That(_datatableDouble.Rows[0][0], Is.EqualTo(1.2));
        }
        [Test]
        public void AddDoubleRowToTable_TableRowsCountGreaterThanIndex_AddsTheRowAndAssignsTheIDString()
        {
            //Act
            _addRowToTableCon.AddDoubleRowToTable(1.2, 1, _datatableDouble);
            //Assert
            Assert.That(_datatableDouble.Rows.Count, Is.EqualTo(2));
            Assert.That(_datatableDouble.Rows[1][0], Is.EqualTo(1.2));
        }
    }
}
