// <copyright file="SamplingTypeBoxTests.cs" company="The University of Dayton">
// Copyright © 2025 The University of Dayton. All rights reserved.
//
// Licensed under the GNU General Public License, Version 3.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.gnu.org/licenses/gpl-3.0.txt
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System;
using NUnit.Framework;
using POD.Controls;
using POD;
using System.Data;
namespace Controls.UnitTests
{
    [TestFixture]
    public class SamplingTypeBoxTests
    {
        //tests for the PFsamplingBox() Class

        /// <summary>
        /// Test for the Constructor PFsamplingBox()
        /// used to ensure that all elements passed in are of type ConfidenceIntervalTypeEnum
        /// </summary>
        [Test]
        public void SamplingTypeBox_ValidItemsPassed_CreatesAListOfOnlySamplingMethodItems()
        {
            SamplingTypeBox samplingBox = new SamplingTypeBox();
            //Assert
            foreach (Object i in samplingBox.Items)
                Assert.That(i as SampleTypeObj, Is.Not.Null);
        }
        /// <summary>
        /// Tests for the SelectedModel field
        /// only test the setter since that is the part that contains logic
        /// </summary>
        [Test]
        [TestCase(SamplingTypeEnum.SimpleRandomSampling)]
        [TestCase(SamplingTypeEnum.RankedSetSampling)]
        public void SelectedSamplingType_SetAssignmentValid_SetsTheSelectedSamplingTypeToDifferentMethod(SamplingTypeEnum testSampleType)
        {
            //Arrange
            SamplingTypeBox samplingBox = new SamplingTypeBox();
            //Act
            samplingBox.SelectedSamplingType = testSampleType;
            //Assert
            Assert.That(samplingBox.SelectedSamplingType, Is.EqualTo(testSampleType));

        }
        [Test]
        public void SelectedSamplingType_SetAssignmentIsNull_SelectedSamplingTypeRemainsNull()
        {
            //Arrange
            SamplingTypeBox samplingBox = new SamplingTypeBox();
            //Act
            samplingBox.SelectedSamplingType = SamplingTypeEnum.None;
            //Assert
            Assert.Throws<NullReferenceException>(() => { var value = samplingBox.SelectedSamplingType; });
        }
        [Test]
        public void SelectedSamplingType_InvalidObjectAddedToitems_DoesNotThrowException()
        {
            //Arrange
            SamplingTypeBox samplingBox = new SamplingTypeBox();
            samplingBox.Items.Insert(0, new DataTable()); //Add a bad item to the beginning of the list
            //Act
            samplingBox.SelectedSamplingType = SamplingTypeEnum.RankedSetSampling;
            //Assert
            Assert.That(samplingBox.SelectedSamplingType, Is.EqualTo(SamplingTypeEnum.RankedSetSampling));
        }

        //tests for the ConfIntObj() Class

        /// <summary>
        /// Tests for the PFModelObj(HitMissRegressionType myType) constructor
        /// </summary>
        [Test]
        [TestCase(SamplingTypeEnum.SimpleRandomSampling, "Simple Random Sampling")]
        [TestCase(SamplingTypeEnum.RankedSetSampling, "Ranked Set Sampling")]
        public void PFModelObj_ValidHitMissModel_AssignsAppropriateLabel(SamplingTypeEnum testSampleType, string expectedLabel)
        {
            //Arrange
            //Act
            SampleTypeObj samplingBox = new SampleTypeObj(testSampleType);
            //Assert
            Assert.That(samplingBox.Label, Is.EqualTo(expectedLabel));
        }
        [Test]
        public void PFModelObj_InvalidModel_AssignsUndefinedLabel()
        {
            //Arrange
            //Act
            SampleTypeObj samplingBox = new SampleTypeObj(SamplingTypeEnum.None);
            //Assert
            Assert.That(samplingBox.Label, Is.EqualTo("Custom"));
        }
    }
}
