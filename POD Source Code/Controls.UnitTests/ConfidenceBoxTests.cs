using System;
using NUnit.Framework;
using Moq;
using System.Windows.Forms.DataVisualization.Charting;
using POD.Controls;
using POD;
using System.Data;

namespace Controls.UnitTests
{
    [TestFixture]
    public class ConfidenceBoxAndConfIntObjTests
    {
        //tets for the ConfidenceBox() Class

        /// <summary>
        /// Test for the Constructor ConfidenceBox()
        /// used to ensure that all elements passed in are of type ConfidenceIntervalTypeEnum
        /// </summary>
        [Test]
        public void ConfidenceBox_ValidItemsPassed_CreatesAListOfOnlyConfIntObjItems()
        {
            ConfidenceBox confidenceBox = new ConfidenceBox();
            //Assert
            foreach(Object i  in confidenceBox.Items)
                Assert.That(i as ConfIntObj, Is.Not.Null);

        }
        /// <summary>
        /// Tests for the SelectedConfInt field
        /// only test the setter since that is the part that contains logic
        /// </summary>
        [Test]
        [TestCase(ConfidenceIntervalTypeEnum.StandardWald)]
        [TestCase(ConfidenceIntervalTypeEnum.ModifiedWald)]
        [TestCase(ConfidenceIntervalTypeEnum.LR)]
        [TestCase(ConfidenceIntervalTypeEnum.MLR)]
        public void SelectedConfInt_SetAssignmentValid_SetsTheSelectedConfIntToNewEnumeration(ConfidenceIntervalTypeEnum testConfInt)
        {
            //Arrange
            ConfidenceBox confidenceBox = new ConfidenceBox();
            //Act
            confidenceBox.SelectedConfInt = testConfInt;
            //Assert
            Assert.That(confidenceBox.SelectedConfInt, Is.EqualTo(testConfInt));

        }
        [Test]
        public void SelectedConfInt_SetAssignmentIsNull_SelectedConfIntRemainsNull()
        {
            //Arrange
            ConfidenceBox confidenceBox = new ConfidenceBox();
            //Act
            confidenceBox.SelectedConfInt = ConfidenceIntervalTypeEnum.None;
            //Assert
            Assert.Throws<NullReferenceException>(() => { var value = confidenceBox.SelectedConfInt; });
        }
        [Test]
        public void SelectedConfInt_InvalidObjectAddedToitems_DoesNotThrowException()
        {
            //Arrange
            ConfidenceBox confidenceBox = new ConfidenceBox();
            confidenceBox.Items.Insert(0, new DataTable()); //Add a bad item to the beginning of the list
            //Act
            confidenceBox.SelectedConfInt = ConfidenceIntervalTypeEnum.MLR;
            //Assert
            Assert.That(confidenceBox.SelectedConfInt, Is.EqualTo(ConfidenceIntervalTypeEnum.MLR));
        }

        //tests for the ConfIntObj() Class

        /// <summary>
        /// Tests for the ConfIntObj(ConfidenceIntervalTypeEnum myType) constructor
        /// </summary>
        [Test]
        [TestCase(ConfidenceIntervalTypeEnum.StandardWald, "Std Wald")]
        [TestCase(ConfidenceIntervalTypeEnum.ModifiedWald, "Mod Wald")]
        [TestCase(ConfidenceIntervalTypeEnum.LR, "LR")]
        [TestCase(ConfidenceIntervalTypeEnum.MLR, "MLR")]
        public void ConfIntObj_ValidConfIntType_AssignsAppropriateLabel(ConfidenceIntervalTypeEnum testConfInt, string expectedLabel)
        {
            //Arrange
            //Act
            ConfIntObj confidenceBox = new ConfIntObj(testConfInt);
            //Assert
            Assert.That(confidenceBox.Label, Is.EqualTo(expectedLabel));
        }
        [Test]
        public void ConfIntObj_InvalidConfIntType_AssignsCustomLabel()
        {
            //Arrange
            //Act
            ConfIntObj confidenceBox = new ConfIntObj(ConfidenceIntervalTypeEnum.None);
            //Assert
            Assert.That(confidenceBox.Label, Is.EqualTo("Custom"));
        }
    }
}
