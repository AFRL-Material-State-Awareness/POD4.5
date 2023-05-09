using System;
using NUnit.Framework;
using POD.Controls;
using POD;
using System.Data;
namespace Controls.UnitTests
{
    [TestFixture]
    public class TransformBoxTests
    {
        //tests for the transformBox_x() Class

        /// <summary>
        /// Test for the Constructor transformBox_x()
        /// used to ensure that all elements passed in are of type TransformTypeEnum
        /// </summary>
        [Test]
        public void transformBox_x_ValidItemsPassed_CreatesAListOfOnlyConfIntObjItems()
        {
            TransformBoxX transformBox_x = new TransformBoxX();
            //Assert
            foreach (Object i in transformBox_x.Items)
                Assert.That(i as TransformObj, Is.Not.Null);

        }
        /// <summary>
        /// Tests for the SelectedTransform field
        /// only test the setter since that is the part that contains logic
        /// </summary>
        [Test]
        [TestCase(TransformTypeEnum.Linear)]
        [TestCase(TransformTypeEnum.Log)]
        [TestCase(TransformTypeEnum.Inverse)]
        public void SelectedTransform_SetAssignmentValid_SetsTheSelectedTransformToNewEnumeration(TransformTypeEnum testConfInt)
        {
            //Arrange
            TransformBoxX transformBox_x = new TransformBoxX();
            //Act
            transformBox_x.SelectedTransform = testConfInt;
            //Assert
            Assert.That(transformBox_x.SelectedTransform, Is.EqualTo(testConfInt));

        }
        [Test]
        public void SelectedTransform_SetAssignmentIsNull_SelectedTransformRemainsNull()
        {
            //Arrange
            TransformBoxX transformBox_x = new TransformBoxX();
            //Act
            transformBox_x.SelectedTransform = TransformTypeEnum.None;
            //Assert
            Assert.Throws<NullReferenceException>(() => { var value = transformBox_x.SelectedTransform; });
        }
        [Test]
        public void SelectedTransform_InvalidObjectAddedToitems_DoesNotThrowException()
        {
            //Arrange
            TransformBoxX transformBox_x = new TransformBoxX();
            transformBox_x.Items.Insert(0, new DataTable()); //Add a bad item to the beginning of the list
            //Act
            transformBox_x.SelectedTransform = TransformTypeEnum.Inverse;
            //Assert
            Assert.That(transformBox_x.SelectedTransform, Is.EqualTo(TransformTypeEnum.Inverse));
        }

        //tests for the ConfIntObj() Class

        /// <summary>
        /// Tests for the ConfIntObj(TransformTypeEnum myType) constructor
        /// </summary>
        [Test]
        [TestCase(TransformTypeEnum.BoxCox, "Box-Cox")]
        [TestCase(TransformTypeEnum.Log, "Log")]
        [TestCase(TransformTypeEnum.Inverse, "Inverse")]
        [TestCase(TransformTypeEnum.Linear, "Linear")]
        public void ConfIntObj_ValidConfIntType_AssignsAppropriateLabel(TransformTypeEnum testConfInt, string expectedLabel)
        {
            //Arrange
            //Act
            TransformObj transformBox_x = new TransformObj(testConfInt);
            //Assert
            Assert.That(transformBox_x.Label, Is.EqualTo(expectedLabel));
        }
        [Test]
        public void ConfIntObj_InvalidConfIntType_AssignsCustomLabel()
        {
            //Arrange
            //Act
            TransformObj transformBox_x = new TransformObj(TransformTypeEnum.None);
            //Assert
            Assert.That(transformBox_x.Label, Is.EqualTo("Custom"));
        }
    }
}
