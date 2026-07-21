// <copyright file="UpdateTableControlTests.cs" company="The University of Dayton">
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

using NUnit.Framework;
using System;
using Moq;
using POD.Data;
namespace Data.UnitTests
{
    [TestFixture]
    public class UpdateTableControlTests
    {
        private UpdateTableControl _updateTabControl;
        private Mock<IAnalysisData> _data;
        [SetUp]
        public void Setup()
        {
            _data = new Mock<IAnalysisData>();
            _updateTabControl = new UpdateTableControl(_data.Object);
        }
        [Test]
        public void UpdateTable_InBoundsFlagPassed_CallsTurnOnPoint()
        {
            //Arrange
            //Act
            _updateTabControl.UpdateTable(1, 2, Flag.InBounds);
            //Assert
            _data.Verify(d => d.TurnOnPoint(2, 1));
        }
        [Test]
        public void UpdateTable_OutBoundsFlagPassed_CallsTurnOffPoint()
        {
            //Arrange
            //Act
            _updateTabControl.UpdateTable(1, 2, Flag.OutBounds);
            //Assert
            _data.Verify(d => d.TurnOffPoint(2, 1));
        }
        [Test]
        public void UpdateTable_InvalidFlagPassed_ThrowsAnArgumentOutOfRangeException()
        {
            //Arrange
            //Act
            //Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => _updateTabControl.UpdateTable(It.IsAny<int>(), It.IsAny<int>(), Flag.None));
            
        }
    }
}
