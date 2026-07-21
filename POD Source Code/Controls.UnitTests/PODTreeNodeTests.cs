// <copyright file="PODTreeNodeTests.cs" company="The University of Dayton">
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
using POD.Controls;
using POD.Data;
using System.Drawing;

namespace Controls.UnitTests
{
    [TestFixture]
    public class PODTreeNodeTests
    {
        private PODListBoxItem _sampleListBox;
        private PODTreeNode _podTreeNode;
        //private PODTreeNode _podTreeNodeCustomName;
        [SetUp]
        public void Setup()
        {
            _sampleListBox = new PODListBoxItem()
            {
                RowColor = Color.Blue,
                ResponseColumnName = "MyResponseName",
                DataSourceName = "MyDataSourceName",
                FlawColumnName = "MyFlawName",
                ResponseOriginalName = "MyOriginalResponseName",
                FlawOriginalName = "MyOriginalFlawName",
                DataSourceOriginalName = "MyOriginalDataSourceName"
            };
            //Testing podTreeNode will not have a custom name for most of the tests
            _podTreeNode = new PODTreeNode(_sampleListBox, "MyAnalysisName", false);
            //_podTreeNodeCustomName = new PODTreeNode(_sampleListBox, "MyAnalysisName", true);
        }
        /// <summary>
        /// Tests for ResponseOriginal field
        /// </summary>
        [Test]
        public void ResponseOriginal_IsAnEmptyString_ReturnsResponseLabel()
        {
            //Arrange
            _podTreeNode.ResponseOriginal = "";
            //Act
            var result = _podTreeNode.ResponseOriginal;
            //Assert
            Assert.That(result, Is.EqualTo("MyResponseName"));
        }
        [Test]
        public void ResponseOriginal_IsNOTAnEmptyString_ReturnsResponseOriginalLabel()
        {
            //Act
            var result = _podTreeNode.ResponseOriginal;
            //Assert
            Assert.That(result, Is.EqualTo("MyOriginalResponseName"));
        }
        /// <summary>
        /// Tests for  FlawOriginal field
        /// </summary>
        [Test]
        public void FlawOriginal_IsAnEmptyString_ReturnsFlawLabel()
        {
            //Arrange
            _podTreeNode.FlawOriginal = "";
            //Act
            var result = _podTreeNode.FlawOriginal;
            //Assert
            Assert.That(result, Is.EqualTo("MyFlawName"));
        }
        [Test]
        public void FlawOriginal_IsNOTAnEmptyString_ReturnsFlawOriginalLabel()
        {
            //Act
            var result = _podTreeNode.FlawOriginal;
            //Assert
            Assert.That(result, Is.EqualTo("MyOriginalFlawName"));
        }
        /// <summary>
        /// Tests for GetOriginalLabel() function
        /// </summary>
        [Test]
        public void GetOriginalLabel_ColTypeResponse_ReturnsResponseOriginalLabel()
        {
            //Act
            var result = _podTreeNode.GetOriginalLabel(ColType.Response);
            //Assert
            Assert.That(result, Is.EqualTo("MyOriginalResponseName"));
        }
        [Test]
        public void GetOriginalLabel_ColTypeFlaw_ReturnsFlawOriginalLabel()
        {
            //Act
            var result = _podTreeNode.GetOriginalLabel(ColType.Flaw);
            //Assert
            Assert.That(result, Is.EqualTo("MyOriginalFlawName"));
        }
        [Test]
        [TestCase(ColType.ID)]
        [TestCase(ColType.Meta)]
        public void GetOriginalLabel_ColTypeNotFlawOrResponse_ReturnsAnEmptyString(ColType coltype)
        {
            //Act
            var result = _podTreeNode.GetOriginalLabel(coltype);
            //Assert
            Assert.That(result, Is.EqualTo(""));
        }
        /// <summary>
        /// Tests for void Label(ColType myType, string myName) function
        /// </summary>
        [Test]
        public void Label_ColumnTypeFlaw_OverwritesFlawLabel()
        {
            //Act
            _podTreeNode.Label(ColType.Flaw, "MyOverwrittenFlawName");
            //Assert
            Assert.That(_podTreeNode.FlawLabel, Is.EqualTo("MyOverwrittenFlawName"));
        }
        [Test]
        public void Label_ColumnTypeResponse_OverwritesResponseLabel()
        {
            //Act
            _podTreeNode.Label(ColType.Response, "MyOverwrittenResponseName");
            //Assert
            Assert.That(_podTreeNode.ResponseLabel, Is.EqualTo("MyOverwrittenResponseName"));
        }
        /// <summary>
        /// Tests for PODTreeNode(PODListBoxItem listItem, string name, bool hasCustomName) : base(name) constructor
        /// </summary>
        [Test]
        public void PODTreeNode_HasCustomName_CustomNameIsTrueAndAnalysisAutoNameIsEmptyString()
        {
            //Act
            PODTreeNode customNameTreeNode = new PODTreeNode(_sampleListBox, "MyCustomAnalysisName", true);
            //Assert
            Assert.That(customNameTreeNode.HasCustomName, Is.True);
            Assert.That(customNameTreeNode.CustomAnalysisName, Is.EqualTo("MyCustomAnalysisName"));
            Assert.That(customNameTreeNode.AnalysisAutoName, Is.EqualTo(""));
        }
        [Test]
        public void PODTreeNode_DoesNotHaveCustomName_AnalysisAutoNameIsInputNameAndCustomNameIsFalse()
        {
            //Act
            PODTreeNode NotCustomNameTreeNode = new PODTreeNode(_sampleListBox, "MyAnalysisName", false);
            //Assert
            Assert.That(NotCustomNameTreeNode.HasCustomName, Is.False);
            Assert.That(NotCustomNameTreeNode.AnalysisAutoName, Is.EqualTo("MyAnalysisName"));
            Assert.That(NotCustomNameTreeNode.CustomAnalysisName, Is.EqualTo(""));
        }
    }
}
