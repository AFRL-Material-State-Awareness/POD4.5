// <copyright file="TransformBoxYHatTests.cs" company="The University of Dayton">
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
            TransformBoxX transformBoxYHat = new TransformBoxX();
            //Act
            transformBoxYHat.SelectedTransform = TransformTypeEnum.None;
            //Assert
            Assert.Throws<NullReferenceException>(() => { var value = transformBoxYHat.SelectedTransform; });
        }
        [Test]
        public void SelectedTransform_InvalidObjectAddedToitems_DoesNotThrowException()
        {
            //Arrange
            TransformBoxX transformBoxYHat = new TransformBoxX();
            transformBoxYHat.Items.Insert(0, new DataTable()); //Add a bad item to the beginning of the list
            //Act
            transformBoxYHat.SelectedTransform = TransformTypeEnum.Inverse;
            //Assert
            Assert.That(transformBoxYHat.SelectedTransform, Is.EqualTo(TransformTypeEnum.Inverse));
        }
    }
}
