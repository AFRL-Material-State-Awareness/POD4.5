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
using POD.Data;
using System.Drawing;

namespace Controls.UnitTests
{
    [TestFixture]
    public class PODTreeNodeTests
    {
        private PODListBoxItem _sampleListBox;
        private PODTreeNode _podTreeNodeNonCustomName;
        private PODTreeNode _podTreeNodeCustomName;
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
            _podTreeNodeNonCustomName = new PODTreeNode(_sampleListBox, "MyAnalysisName", false);
            _podTreeNodeCustomName = new PODTreeNode(_sampleListBox, "MyAnalysisName", true);
        }
        /// <summary>
        /// Tests for ResponseOriginal field
        /// </summary>
        [Test]
        public void ResponseOriginal_IsAnEmptyString_ReturnsResponseLabel()
        {
            //Arrange
            _podTreeNodeNonCustomName.ResponseOriginal = "";
            //Act
            var result = _podTreeNodeNonCustomName.ResponseOriginal;
            //Assert
            Assert.That(result, Is.EqualTo("MyResponseName"));
        }
        [Test]
        public void ResponseOriginal_IsNOTAnEmptyString_ReturnsResponseOriginalLabel()
        {
            //Act
            var result = _podTreeNodeNonCustomName.ResponseOriginal;
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
            _podTreeNodeNonCustomName.FlawOriginal = "";
            //Act
            var result = _podTreeNodeNonCustomName.FlawOriginal;
            //Assert
            Assert.That(result, Is.EqualTo("MyFlawName"));
        }
        [Test]
        public void FlawOriginal_IsNOTAnEmptyString_ReturnsFlawOriginalLabel()
        {
            //Act
            var result = _podTreeNodeNonCustomName.FlawOriginal;
            //Assert
            Assert.That(result, Is.EqualTo("MyOriginalFlawName"));
        }
        /// <summary>
        /// Tests for GetOriginalLabel() function
        /// </summary>
    }
}
