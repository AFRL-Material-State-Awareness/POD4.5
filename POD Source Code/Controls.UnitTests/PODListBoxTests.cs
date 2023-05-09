using POD.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using System.Windows.Forms.DataVisualization.Charting;
using POD;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Reflection;
using POD.Data;

namespace Controls.UnitTests
{
    [TestFixture]
    public class PODListBoxTests
    {
        ///tests SingleSelectedItemWithProps() Function
        [Test]
        public void AddInitialColumn_ColumnsIsNotNull_ClearsTheColumnsListAndAddsAColumnCalledItems()
        {
            //Arrange
            PODListBox podListBox = new PODListBox();
            podListBox.Columns.Add(new DataGridViewTextBoxColumn());
            //Act
            podListBox.AddInitialColumn();
            //Assert
            Assert.That(podListBox.Columns.Count, Is.EqualTo(1));
            Assert.That(podListBox.Columns[0].Name, Is.EqualTo("Items"));
            Assert.That(podListBox.Columns[0].ValueType, Is.EqualTo(typeof(PODListBoxItem)));
        }

        //Tests for the SingleSelectedItem getter property
        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public void SingleSelectedItem_SelectedRowsIsZero_ReturnsNull(int rowCount)
        {
            //Arrange
            PODListBox podListBox = new PODListBox();
            for(int i =0; i < rowCount; i++)
                podListBox.Rows.Add(new object[] { new PODListBoxItem() });
            //Act
            var result = podListBox.SingleSelectedItem;
            //Assert
            Assert.That(result, Is.Null);
        }
        [Test]
        public void SingleSelectedItem_SelectedRowsIsNotZeroAndNotAPODListBoxItem_ReturnsNull()
        {
            //Arrange
            PODListBox podListBox = new PODListBox();
            podListBox.Rows.Add(new object[] { "This is not a PODListBoxItem" });
            podListBox.SetSelected(0, true);
            //Act
            var result = podListBox.SingleSelectedItem;
            //Assert
            Assert.That(result, Is.Null);
        }
        [Test]
        public void SingleSelectedItem_SelectedRowsIsNotZeroAndAPODListBoxItem_ReturnsThePODListBoxItem()
        {
            //Arrange
            PODListBox podListBox = new PODListBox();
            podListBox.Rows.Add(new object[] { new PODListBoxItem() });
            podListBox.SetSelected(0, true);
            //Act
            var result = podListBox.SingleSelectedItem;
            //Assert
            Assert.That(result is PODListBoxItem);
        }

        ///Tests for the SingleSelectedItemWithProps getter property
        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public void SingleSelectedItemWithProps_SelectedRowsIsZero_ReturnsNull(int rowCount)
        {
            //Arrange
            PODListBox podListBox = new PODListBox();
            for (int i = 0; i < rowCount; i++)
                podListBox.Rows.Add(new object[] { new PODListBoxItemWithProps() });
            //Act
            var result = podListBox.SingleSelectedItemWithProps;
            //Assert
            Assert.That(result, Is.Null);
        }
        [Test]
        public void SingleSelectedItemWithProps_SelectedRowsIsNotZeroAndNotAPODListBoxItemWithProps_ReturnsNull()
        {
            //Arrange
            PODListBox podListBox = new PODListBox();
            podListBox.Rows.Add(new object[] { "This is not a PODListBoxItemWithProps" });
            podListBox.SetSelected(0, true);
            //Act
            var result = podListBox.SingleSelectedItemWithProps;
            //Assert
            Assert.That(result, Is.Null);
        }
        [Test]
        public void SingleSelectedItemWithProps_SelectedRowsIsNotZeroAndAPODListBoxItemWithProps_ReturnsThePODListBoxItem()
        {
            //Arrange
            PODListBox podListBox = new PODListBox();
            podListBox.Rows.Add(new object[] { new PODListBoxItemWithProps() });
            podListBox.SetSelected(0, true);
            //Act
            var result = podListBox.SingleSelectedItemWithProps;
            //Assert
            Assert.That(result is PODListBoxItem);
        }

        ///Tests for the SingleSelectedIndex getter and setter property
        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public void SingleSelectedIndex_SelectedRowsIsZero_ReturnsNegative1(int rowCount)
        {
            //Arrange
            PODListBox podListBox = new PODListBox();
            for (int i = 0; i < rowCount; i++)
                podListBox.Rows.Add(new object[] { 1 });
            //Act
            var result = podListBox.SingleSelectedIndex;
            //Assert
            Assert.That(result, Is.EqualTo(-1));
        }
        [Test]
        public void SingleSelectedIndex_SelectedRowsIsNotZeroAndAPODListBoxItemWithProps_ReturnsAValueThatIs0OrGreater()
        {
            //Arrange
            PODListBox podListBox = new PODListBox();
            podListBox.Rows.Add(new object[] { 1 });
            podListBox.SetSelected(0, true);
            //Act
            var result = podListBox.SingleSelectedIndex;
            //Assert
            Assert.That(result, Is.GreaterThan(-1));
        }
        [Test]
        [TestCase(-1)]
        [TestCase(11)]
        public void SingleSelectedIndex_ValueNotInRowCount_NoRowsBecomeSelected(int testIndex)
        {
            //Arrange
            PODListBox podListBox = new PODListBox();
            for (int i = 0; i < 10; i++)
                podListBox.Rows.Add(new object[] { i+1 });
            //Act
            podListBox.SingleSelectedIndex = testIndex;
            //Assert
            foreach(DataGridViewRow row in podListBox.Rows)
            {
                Assert.That(row.Selected, Is.False);
            }
        }
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void SingleSelectedIndex_MultiSelectIsFalse_OnlyOneIndexIsSelected(bool multiSelectFlag)
        {
            //Arrange
            PODListBox podListBox = new PODListBox();
            for (int i = 0; i < 3; i++)
                podListBox.Rows.Add(new object[] { i + 1 });
            podListBox.Rows[0].Selected = true;
            podListBox.Rows[1].Selected = true;
            podListBox.MultiSelect = multiSelectFlag;
            //Act
            podListBox.SingleSelectedIndex = 2;
            //Assert
            Assert.That(podListBox.Rows[0].Selected, Is.False);
            Assert.That(podListBox.Rows[1].Selected, Is.False);
            Assert.That(podListBox.Rows[2].Selected, Is.True);        
        }

    }
}
