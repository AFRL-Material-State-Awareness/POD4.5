using System;
using NUnit.Framework;
using POD.Controls;
using POD;
using System.Data;

namespace Controls.UnitTests
{
    [TestFixture]
    class PFModelBoxTests
    {
        //tests for the PFModelBox() Class

        /// <summary>
        /// Test for the Constructor PFModelBox()
        /// used to ensure that all elements passed in are of type ConfidenceIntervalTypeEnum
        /// </summary>
        [Test]
        public void PFModelBox_ValidItemsPassed_CreatesAListOfOnlyConfIntObjItems()
        {
            PFModelBox modelBox = new PFModelBox();
            //Assert
            foreach (Object i in modelBox.Items)
                Assert.That(i as PFModelObj, Is.Not.Null);

        }
        /// <summary>
        /// Tests for the SelectedModel field
        /// only test the setter since that is the part that contains logic
        /// </summary>
        [Test]
        [TestCase(HitMissRegressionType.LogisticRegression)]
        [TestCase(HitMissRegressionType.FirthLogisticRegression)]
        public void SelectedModel_SetAssignmentValid_SetsTheSelectedModelTypeToANewModel(HitMissRegressionType testModel)
        {
            //Arrange
            PFModelBox modelBox = new PFModelBox();
            //Act
            modelBox.SelectedModel = testModel;
            //Assert
            Assert.That(modelBox.SelectedModel, Is.EqualTo(testModel));

        }
        [Test]
        public void SelectedModel_SetAssignmentIsNull_SelectedModelRemainsNull()
        {
            //Arrange
            PFModelBox modelBox = new PFModelBox();
            //Act
            modelBox.SelectedModel = HitMissRegressionType.None;
            //Assert
            Assert.Throws<NullReferenceException>(() => { var value = modelBox.SelectedModel; });
        }
        [Test]
        public void SelectedModel_InvalidObjectAddedToitems_DoesNotThrowException()
        {
            //Arrange
            PFModelBox modelBox = new PFModelBox();
            modelBox.Items.Insert(0, new DataTable()); //Add a bad item to the beginning of the list
            //Act
            modelBox.SelectedModel = HitMissRegressionType.FirthLogisticRegression;
            //Assert
            Assert.That(modelBox.SelectedModel, Is.EqualTo(HitMissRegressionType.FirthLogisticRegression));
        }

        //tests for the ConfIntObj() Class

        /// <summary>
        /// Tests for the PFModelObj(HitMissRegressionType myType) constructor
        /// </summary>
        [Test]
        [TestCase(HitMissRegressionType.LogisticRegression, "Logistic Reg")]
        [TestCase(HitMissRegressionType.FirthLogisticRegression, "Firth Logistic Reg")]
        public void PFModelObj_ValidHitMissModel_AssignsAppropriateLabel(HitMissRegressionType testModel, string expectedLabel)
        {
            //Arrange
            //Act
            PFModelObj modelBox = new PFModelObj(testModel);
            //Assert
            Assert.That(modelBox.Label, Is.EqualTo(expectedLabel));
        }
        [Test]
        public void PFModelObj_InvalidModel_AssignsUndefinedLabel()
        {
            //Arrange
            //Act
            PFModelObj modelBox = new PFModelObj(HitMissRegressionType.None);
            //Assert
            Assert.That(modelBox.Label, Is.EqualTo("Undefined"));
        }
    }
}
