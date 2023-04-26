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
    public class SamplingTypeBoxTests
    {
        //tests for the PFModelBox() Class

        /// <summary>
        /// Test for the Constructor PFModelBox()
        /// used to ensure that all elements passed in are of type ConfidenceIntervalTypeEnum
        /// </summary>
        [Test]
        public void SamplingTypeBox_ValidItemsPassed_CreatesAListOfOnlySamplingMethodItems()
        {
            SamplingTypeBox modelBox = new SamplingTypeBox();
            //Assert
            foreach (Object i in modelBox.Items)
                Assert.That(i as SampleTypeObj, Is.Not.Null);
        }
        /// <summary>
        /// Tests for the SelectedModel field
        /// only test the setter since that is the part that contains logic
        /// </summary>
        [Test]
        [TestCase(SamplingTypeEnum.SimpleRandomSampling)]
        [TestCase(SamplingTypeEnum.RankedSetSampling)]
        public void SelectedSamplingType_SetAssignmentValid_SetsTheSelectedSamplingTypeToDifferentMethod(SamplingTypeEnum testConfInt)
        {
            //Arrange
            SamplingTypeBox modelBox = new SamplingTypeBox();
            //Act
            modelBox.SelectedSamplingType = testConfInt;
            //Assert
            Assert.That(modelBox.SelectedSamplingType, Is.EqualTo(testConfInt));

        }
        [Test]
        public void SelectedSamplingType_SetAssignmentIsNull_SelectedSamplingTypeRemainsNull()
        {
            //Arrange
            SamplingTypeBox modelBox = new SamplingTypeBox();
            //Act
            modelBox.SelectedSamplingType = SamplingTypeEnum.None;
            //Assert
            Assert.Throws<NullReferenceException>(() => { var value = modelBox.SelectedSamplingType; });
        }
        [Test]
        public void SelectedSamplingType_InvalidObjectAddedToitems_DoesNotThrowException()
        {
            //Arrange
            SamplingTypeBox modelBox = new SamplingTypeBox();
            modelBox.Items.Insert(0, new DataTable()); //Add a bad item to the beginning of the list
            //Act
            modelBox.SelectedSamplingType = SamplingTypeEnum.RankedSetSampling;
            //Assert
            Assert.That(modelBox.SelectedSamplingType, Is.EqualTo(SamplingTypeEnum.RankedSetSampling));
        }

        //tests for the ConfIntObj() Class

        /// <summary>
        /// Tests for the PFModelObj(HitMissRegressionType myType) constructor
        /// </summary>
        [Test]
        [TestCase(SamplingTypeEnum.SimpleRandomSampling, "Simple Random Sampling")]
        [TestCase(SamplingTypeEnum.RankedSetSampling, "Ranked Set Sampling")]
        public void PFModelObj_ValidHitMissModel_AssignsAppropriateLabel(SamplingTypeEnum testConfInt, string expectedLabel)
        {
            //Arrange
            //Act
            SampleTypeObj modelBox = new SampleTypeObj(testConfInt);
            //Assert
            Assert.That(modelBox.Label, Is.EqualTo(expectedLabel));
        }
        [Test]
        public void PFModelObj_InvalidModel_AssignsUndefinedLabel()
        {
            //Arrange
            //Act
            SampleTypeObj modelBox = new SampleTypeObj(SamplingTypeEnum.None);
            //Assert
            Assert.That(modelBox.Label, Is.EqualTo("Custom"));
        }
    }
}
