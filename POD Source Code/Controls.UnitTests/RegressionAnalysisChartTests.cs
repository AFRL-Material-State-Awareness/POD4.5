using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POD.Controls;
using NUnit.Framework;
using Moq;
using System.Windows.Forms;
using POD;
using System.Reflection;
using System.Drawing;
using NUnit.Framework.Constraints;

namespace Controls.UnitTests
{
    [TestFixture]
    public class RegressionAnalysisChartTests
    {
        private RegressionAnalysisChart _regressionAnalysisChart;
        [SetUp]
        public void Setup()
        {
            _regressionAnalysisChart = new RegressionAnalysisChart();
        }
        /// Tests for the List<ToolStripItem> BuildMenuItems(double x, double y) function
        [Test]
        public void BuildMenuItems_ContetMenuImageListIsNull_NoImageAddedToSetAMaxAndMin()
        {
            //Arrange
            RegressionAnalysisChart.ContextMenuImageList = null;
            //Act
            var result = _regressionAnalysisChart.BuildMenuItems(1.0, 1.0);
            //Assert
            BuildMenuItemsAssertionsNullCheck(result);
        }
        [Test]
        [TestCase(1)]
        [TestCase(5)]
        public void BuildMenuItems_ContetMenuImageListNotNullAndImageCountLessthanOrEqualTo4_NoImageAddedToSetAMaxAndMin(int imageCount)
        {
            ImageList images = new ImageList();
            //Arrange
            for (int i = 0; i < imageCount; i++)
                images.Images.Add(CreateSampleIamge());
            RegressionAnalysisChart.ContextMenuImageList = images;
            //Act
            var result = _regressionAnalysisChart.BuildMenuItems(1.0, 1.0);
            //Assert
            Assert.That(RegressionAnalysisChart.ContextMenuImageList, Is.Not.Null);
            BuildMenuItemsAssertionsNullCheck(result);
            // Dispose of bitmaps
            for (int i = 0; i < imageCount; i++)
                images.Images[i].Dispose();
        }
        [Test]
        [TestCase(6)]
        public void BuildMenuItems_ContetMenuImageListNotNullAndImageCountGreaterThan4_ImageAddedToSetAMaxAndMin(int imageCount)
        {
            ImageList images = new ImageList();
            //Arrange
            for (int i = 0; i < imageCount; i++)
                images.Images.Add(CreateSampleIamge());
            RegressionAnalysisChart.ContextMenuImageList = images;
            //Act
            var result = _regressionAnalysisChart.BuildMenuItems(1.0, 1.0);
            //Assert
            Assert.That(RegressionAnalysisChart.ContextMenuImageList, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Image, Is.Not.Null);
            Assert.That(result[1].Image, Is.Not.Null);
            // Dispose of bitmaps
            for (int i = 0; i < imageCount; i++)
                images.Images[i].Dispose();
        }
        private void BuildMenuItemsAssertionsNullCheck(List<ToolStripItem> result)
        {
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Image, Is.Null);
            Assert.That(result[1].Image, Is.Null);
        }
        private Image CreateSampleIamge()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Controls.UnitTests.Resources.ShowNormalitySample.png"))
                return new Bitmap(stream);
        }
    }
}
