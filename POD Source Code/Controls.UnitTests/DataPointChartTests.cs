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
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Reflection;
namespace Controls.UnitTests
{
    [TestFixture]
    public class DataPointChartTests
    {
        
        private DataPointChart _chart;
        [SetUp]
        public void SetUp()
        {
            _chart = new DataPointChart();
        }
        /// Tests for the DataPointChart() Constructor
        [Test]
        public void DataPointChart_DataPointChartConstructorInializedForTheFirstTime_CreatesImageLists()
        {
            //Assert
            Assert.That(DataPointChart.ContextMenuImageList, Is.Not.Null);
            Assert.That(DataPointChart.ContextMenuImageList.Images.Count, Is.Not.Zero);
        }
        [Test]
        public void DataPointChart_DataPointChartConstructorIsAlreadyInitialized_TheImageListIsNotOverwritten()
        {
            DataPointChart.ContextMenuImageList = null;
            ImageList imageList = new ImageList();
            imageList.Images.Add(CreateSampleIamge());
            DataPointChart.ContextMenuImageList = imageList;
            //Act
            DataPointChart anotherChart = new DataPointChart();
            //Assert
            Assert.That(DataPointChart.ContextMenuImageList, Is.Not.Null);
            Assert.That(DataPointChart.ContextMenuImageList.Images.Count, Is.EqualTo(1));
        }
        private Image CreateSampleIamge()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Controls.UnitTests.Resources.ShowNormalitySample.png"))
                return new Bitmap(stream);
        }
        /// <summary>
        /// Tests for the void FixUpLegend(bool showlegend) function
        /// </summary>
        [Test]
        public void FixUpLegend_ShowLegendIsFalse_HideLegendCalled()
        {
            //Arrange
            Mock<FakeDataPointChart> chart = new Mock<FakeDataPointChart>();
            //Act
            chart.Object.FixUpLegend(false);
            //Assert That
            chart.Verify(c => c.HideLegend(), Times.Once);
        }
        // Series count is empty for this test
        [Test]
        public void FixUpLegend_ShowLegendIsTrueAndLegendsCountIsZero_AssignslegendAndAddsItToTheListAndCreatesCustomItems()
        {
            //Arrange
            Mock<FakeDataPointChart> chart = new Mock<FakeDataPointChart>();
            //Act
            chart.Object.FixUpLegend(true);
            //Assert That
            AssertionsForFixUpLegendIsTrue(chart);
        }
        [Test]
        public void FixUpLegend_ShowLegendIsTrueAndLegendsCountIsGreaterThan0_PullFromTheFirstIndexOfTheLegendsList()
        {
            //Arrange
            Mock<FakeDataPointChart> chart = new Mock<FakeDataPointChart>();
            //Populate the legends object with two dummy legends.
            //After executing FixUpLegend(true) the legend count should remain the same
            chart.Object.Legends.Add(new Legend());
            chart.Object.Legends.Add(new Legend());
            //Act
            chart.Object.FixUpLegend(true);
            //Assert That
            Assert.That(chart.Object.Legends.Count, Is.EqualTo(2));
            chart.Verify(c => c.ShowLegend(), Times.Once);
            chart.Verify(c => c.Refresh(), Times.Once);
        }
        // Series count is empty for this test
        [Test]
        public void FixUpLegend_SeriesCountContainsItemsWhoseNameIsNotSelected_CreatesCustomLegendItemsForThatSeries()
        {
            //Arrange
            Mock<FakeDataPointChart> chart = SetupSeries( new Mock<FakeDataPointChart>());
            //Act
            chart.Object.FixUpLegend(true);
            //Assert That
            AssertionsForFixUpLegendIsTrue(chart);               
        }
        [Test]
        public void FixUpLegend_SeriesCountContainsItemsWhoseNameIsNotSelectedAndContainsPointsWhereGetColorIsNotTransparent_DoesNotOverwriteItemMarkerColor()
        {
            //Arrange
            Mock<FakeDataPointChart> chart = SetupSeries(new Mock<FakeDataPointChart>());
            Dictionary<string, Color> colorMap = new Dictionary<string, Color>();
            colorMap.Add("Black", Color.Black);
            chart.Object.MyInputColorMap = colorMap;
            //This should not overwrite item.markercolor since the series has a color assigned
            chart.Object.Series[0].Points.AddXY(1.0, 1.0);
            chart.Object.Series[0].Points[0].Color = Color.Red;
            //Act
            chart.Object.FixUpLegend(true);
            //Assert That
            AssertionsForFixUpLegendIsTrue(chart);
            AssertLegendItems(chart);

            var legend = chart.Object.Legends[0];
            Assert.That(legend.CustomItems[0].MarkerColor, Is.EqualTo(Color.Black));
        }
        [Test]
        public void FixUpLegend_SeriesCountContainsItemsWhoseNameIsNotSelectedAndContainsPointsWhereGetColorIsTransparent_OverwritesItemMarkerColor()
        {
            //Arrange
            Mock<FakeDataPointChart> chart = SetupSeries(new Mock<FakeDataPointChart>());
            //This should overwrite item.markercolor since color is transparent and the series contains at least one point
            chart.Object.Series[0].Points.AddXY(1.0, 1.0);
            chart.Object.Series[0].Points[0].Color = Color.Red;
            //Act
            chart.Object.FixUpLegend(true);
            //Assert That
            AssertionsForFixUpLegendIsTrue(chart);
            AssertLegendItems(chart);
            var legend = chart.Object.Legends[0];
            Assert.That(legend.CustomItems[0].MarkerColor, Is.EqualTo(Color.Red));
        }
        private Mock<FakeDataPointChart> SetupSeries(Mock<FakeDataPointChart> chart)
        {
            var series1 = new Series() { Name = "Black" };
            var series2 = new Series() { Name = "Blue" };
            var series3 = new Series() { Name = "Selected" };
            chart.Object.Series.Add(series1);
            chart.Object.Series.Add(series2);
            chart.Object.Series.Add(series3);
            return chart;
        }
        private void AssertionsForFixUpLegendIsTrue(Mock<FakeDataPointChart> chart)
        {
            Assert.That(chart.Object.Legends.Count, Is.EqualTo(1));
            chart.Verify(c => c.ShowLegend(), Times.Once);
            chart.Verify(c => c.Refresh(), Times.Once);
            
        }
        private void AssertLegendItems(Mock<FakeDataPointChart> chart)
        {
            var legend = chart.Object.Legends[0];
            Assert.That(legend.CustomItems.Count, Is.EqualTo(2));
            foreach (LegendItem item in legend.CustomItems)
            {
                Assert.That(item.ImageStyle, Is.EqualTo(LegendImageStyle.Marker));
                Assert.That(item.Cells.Count, Is.GreaterThanOrEqualTo(2));
            }
        }
        // Used for the tests inside the series for for loop in order to control the GetColor() dependency within the FixUpLegend function
        public class FakeDataPointChart : DataPointChart
        {
            public Dictionary<string, Color> MyInputColorMap
            {
                set { _colorMap = value; }
            }

        }
    }
}
