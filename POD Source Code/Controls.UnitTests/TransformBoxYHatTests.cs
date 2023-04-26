using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using System.Windows.Forms.DataVisualization.Charting;
using POD.Controls;
using POD;
using System.Data;
namespace Controls.UnitTests
{
    [TestFixture]
    public class TransformBoxYHatTests
    {
        //tests for the TransformBoxYHat() Class

        /// <summary>
        /// Test for the Constructor transformBox_x()
        /// used to ensure that all elements passed in are of type TransformTypeEnum
        /// </summary>
        [Test]
        public void TransformBoxYHat_ValidItemsPassed_CreatesAListOfOnlyConfIntObjItems()
        {
            TransformBoxYHat transformBoxYHat = new TransformBoxYHat();
            //Assert
            foreach (Object i in transformBoxYHat.Items)
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
        [TestCase(TransformTypeEnum.BoxCox)]
        public void SelectedTransform_SetAssignmentValid_SetsTheSelectedTransformToNewEnumeration(TransformTypeEnum testConfInt)
        {
            //Arrange
            TransformBoxYHat transformBoxYHat = new TransformBoxYHat();
            //Act
            transformBoxYHat.SelectedTransform = testConfInt;
            //Assert
            Assert.That(transformBoxYHat.SelectedTransform, Is.EqualTo(testConfInt));

        }
        [Test]
        public void SelectedTransform_SetAssignmentIsNull_SelectedTransformRemainsNull()
        {
            //Arrange
            TransformBox transformBoxYHat = new TransformBox();
            //Act
            transformBoxYHat.SelectedTransform = TransformTypeEnum.None;
            //Assert
            Assert.Throws<NullReferenceException>(() => { var value = transformBoxYHat.SelectedTransform; });
        }
        [Test]
        public void SelectedTransform_InvalidObjectAddedToitems_DoesNotThrowException()
        {
            //Arrange
            TransformBox transformBoxYHat = new TransformBox();
            transformBoxYHat.Items.Insert(0, new DataTable()); //Add a bad item to the beginning of the list
            //Act
            transformBoxYHat.SelectedTransform = TransformTypeEnum.Inverse;
            //Assert
            Assert.That(transformBoxYHat.SelectedTransform, Is.EqualTo(TransformTypeEnum.Inverse));
        }
    }
}
