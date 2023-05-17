using POD.Controls;
using System.Collections.Generic;
using NUnit.Framework;
using Moq;
using System.Windows.Forms;
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
        public void SingleSelectedIndex_MultiSelectIsTrueOrFalse_OnlyOneIndexIsSelected(bool multiSelectFlag)
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

        /// Skipping SuspendDrawing() tests for now
        /// Skipping ResumeDrawing() tests for now
        
        /// tests for string CreateAutoName(List<PODListBoxItem> listBoxItems) function
        [Test]
        public void CreateAutoName_ListPODListBoxItemIsEmpty_ReturnsEmptyString()
        {
            //Arrange
            List<PODListBoxItem> listOfPODBoxItems = new List<PODListBoxItem>();
            //Act
            var result=PODListBox.CreateAutoName(listOfPODBoxItems);
            //Arrange
            Assert.That(result, Is.EqualTo(string.Empty));
        }
        //input strings in various formats
        [Test]
        [TestCase("myName.x.y")]
        [TestCase("my Name.x.y")]
        [TestCase("myName.x.responseName")]
        [TestCase("my Name.x.responseName")]
        [TestCase("myName.x.response Name")]
        [TestCase("my Name.x.response Name")]
        public void CreateAutoName_ListPODListBoxItemContains1Name_ReturnsTheName(string myName)
        {
            //Arrange
            PODListBoxItem podListBoxItem = new PODListBoxItem();
            podListBoxItem.DataSourceName = myName.Split('.')[0];
            podListBoxItem.FlawColumnName = myName.Split('.')[1];
            podListBoxItem.ResponseColumnName = myName.Split('.')[myName.Split('.').Length-1];
            List<PODListBoxItem> listOfPODBoxItems = new List<PODListBoxItem>() { podListBoxItem };
            //Act
            var result = PODListBox.CreateAutoName(listOfPODBoxItems);
            //Assert
            Assert.That(result, Is.EqualTo(myName));
        }
        [Test]
        public void CreateAutoName_ListPODListBoxItemContainsNumerousWithNoOverlappingStrings_ReturnsConcatinatedResponsesStringWithParentheses()
        {
            //Arrange
            PODListBoxItem podListBoxItem1 = PopulatePODListBoxItem(new PODListBoxItem(), "myName.flawNames.responseNames");
            PODListBoxItem podListBoxItem2 = PopulatePODListBoxItem(new PODListBoxItem(), "HitMiss.x.y");
            PODListBoxItem podListBoxItem3 = PopulatePODListBoxItem(new PODListBoxItem(), "SignalResponse.no.substrings");

            List<PODListBoxItem> listOfPODBoxItems = new List<PODListBoxItem>() { podListBoxItem1, podListBoxItem2, podListBoxItem3 };
            //Act
            var result = PODListBox.CreateAutoName(listOfPODBoxItems);
            //Assert
            Assert.That(result, Is.EqualTo("myName" + "." + "flawNames" + "." + "(" + "responseNames" + ", " + "y" + ", " + "substrings" + ")"));
        }
        [Test]
        public void reateAutoName_ListPODListBoxItemContainsNumerousThatHaveNonStartingOverlappingStrings_ReturnsConcatinatedResponsesStringWithParentheses()
        {
            //Arrange
            PODListBoxItem podListBoxItem1 = PopulatePODListBoxItem(new PODListBoxItem(), "myName.flawNames.xyzhelloworldabc");
            PODListBoxItem podListBoxItem2 = PopulatePODListBoxItem(new PODListBoxItem(), "myName.flawNames.whyhelloworldmad");
            PODListBoxItem podListBoxItem3 = PopulatePODListBoxItem(new PODListBoxItem(), "myName.flawNames.lmnhelloworldopq");

            List<PODListBoxItem> listOfPODBoxItems = new List<PODListBoxItem>() { podListBoxItem1, podListBoxItem2, podListBoxItem3 };
            //Act
            var result = PODListBox.CreateAutoName(listOfPODBoxItems);
            //Assert
            Assert.That(result, Is.EqualTo("myName" + "." + "flawNames" + "." + "(" + "xyzhelloworldabc" + ", " + "whyhelloworldmad" + ", " + "lmnhelloworldopq" + ")"));
        }
        [Test]
        public void CreateAutoName_ListPODListBoxItemContainsNumerousWithStartingOverlappingStrings_ReturnsResponsesInParanthesisWithTheOverlapString()
        {
            //Arrange
            PODListBoxItem podListBoxItem1 = PopulatePODListBoxItem(new PODListBoxItem(), "myName.flawNames.helloworldabc");
            PODListBoxItem podListBoxItem2 = PopulatePODListBoxItem(new PODListBoxItem(), "myName.flawNames.helloworlddef");
            PODListBoxItem podListBoxItem3 = PopulatePODListBoxItem(new PODListBoxItem(), "myName.flawNames.helloworldghi");
            List<PODListBoxItem> listOfPODBoxItems = new List<PODListBoxItem>() { podListBoxItem1, podListBoxItem2, podListBoxItem3 };
            //Act
            var result = PODListBox.CreateAutoName(listOfPODBoxItems);
            //Assert
            string commonSubstring = "helloworld";
            Assert.That(result, Is.EqualTo("myName" + "." + "flawNames" + "." + commonSubstring+ "(" + "abc" + ", " + "def" + ", " + "ghi" + ")"));
        }
        private PODListBoxItem PopulatePODListBoxItem(PODListBoxItem podListBoxItem,string myName)
        {
            podListBoxItem.DataSourceName = myName.Split('.')[0];
            podListBoxItem.FlawColumnName = myName.Split('.')[1];
            podListBoxItem.ResponseColumnName = myName.Split('.')[myName.Split('.').Length - 1];
            return podListBoxItem;
        }
        /// <summary>
        /// Tests for FitAllRows(int myMaxCount) function
        
        /// </summary>
        [Test]
        public void FitAllRows_NoRowsInListBox_HeightDefaultReturned()
        {
            //Arrange
            PODListBox podListBox = new PODListBox();
            //Act
            podListBox.FitAllRows(It.IsAny<int>());
            //Assert
            /// Default Height is 150
            Assert.That(podListBox.Height, Is.EqualTo(150));
        }
        [Test]
        [TestCase(0, 3)]
        [TestCase(1, 25)]
        [TestCase(2, 47)]
        [TestCase(3, 69)]
        public void FitAllRows_RowsAreInListBoxAndMaxCountIsLessthanOrEqualToRowCount_NewHeightReturned(int maxCount, int exectedHeight)
        {
            //Arrange
            PODListBox podListBox = AddPODListBoxItems(new PODListBox());
            //Act
            podListBox.FitAllRows(maxCount);
            //Assert
            Assert.That(podListBox.Height, Is.EqualTo(exectedHeight));
        }
        [Test]
        [TestCase(4, 69)]
        [TestCase(5, 69)]
        public void FitAllRows_RowsAreInListBoxAndMaxCountIsGreaterThanRowCount_NewHeightReturnsTheSameValue(int maxCount, int exectedHeight)
        {
            //Arrange
            PODListBox podListBox = AddPODListBoxItems(new PODListBox());
            //Act
            podListBox.FitAllRows(maxCount);
            //Assert
            Assert.That(podListBox.Height, Is.EqualTo(exectedHeight));
        }
        private PODListBox AddPODListBoxItems(PODListBox podListBox)
        {
            podListBox.Rows.Add(new DataGridViewRow());
            podListBox.Rows.Add(new DataGridViewRow());
            podListBox.Rows.Add(new DataGridViewRow());
            return podListBox;
        }

    }
}
