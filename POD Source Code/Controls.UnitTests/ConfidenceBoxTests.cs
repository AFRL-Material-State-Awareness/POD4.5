// <copyright file="ConfidenceBoxTests.cs" company="The University of Dayton">
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
