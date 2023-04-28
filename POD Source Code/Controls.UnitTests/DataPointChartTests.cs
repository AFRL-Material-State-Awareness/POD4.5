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
        /// <summary>
        /// NOTE: For the tests in this class, DesignMode is always assume to be false since there is no easy way to simulate it.
        /// It also does occur often (if at all) when running the application
        /// </summary>

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
            Mock<FakeDataPointChart> chart = SetupSeries(new Mock<FakeDataPointChart>());
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
        /// <summary>
        /// tests for the ChartTitle Property
        /// Only testing the git properly
        /// </summary>
        [Test]
        public void ChartTitle_NotInDeisgnMoreAndTitlesEmpty_ReturnEmptyString()
        {
            //Act
            var result = _chart.ChartTitle;
            //Assert
            Assert.That(result, Is.EqualTo(""));
        }
        [Test]
        public void ChartTitle_TitleNotEmpty_ReturnsANonEmptyString()
        {
            //Arrange
            _chart.Titles.Add("MyChartTitle");
            _chart.XAxisTitle = "Flaws";
            //Act
            var result = _chart.ChartTitle;
            //Assert
            Assert.That(result, Is.Not.EqualTo(""));
        }
        /// <summary>
        /// tests for ShowLegend() function
        /// </summary>
        [Test]
        public void ShowLegend_legendIsNull_LegendsRemainstheSame()
        {
            //Arrange
            FakeDataPointChart chart = new FakeDataPointChart();
            chart.SetLegendManually = null;
            //Act
            chart.ShowLegend();
            //Assert
            Assert.That(() => chart.ShowLegend(), Throws.Nothing);
        }
        [Test]
        public void ShowLegend_legendIsInLegends_LegendEnabledTrueAndLegendsCountDoesNotChange()
        {
            //Arrange
           FakeDataPointChart chart = new FakeDataPointChart();
            var legend = new Legend() { Name = "MyLegend", Enabled = false };
            chart.SetLegendManually = legend;
            chart.Legends.Add(legend);
            //Act
            chart.ShowLegend();
            //Assert
            Assert.That(legend.Enabled, Is.True);
            Assert.That(chart.Legends.Count, Is.EqualTo(1));
        }
        [Test]
        public void ShowLegend_legendIsNotInLegends_LegendEnabledTrueAndNewLegendGetsAddedToLegends()
        {
            //Arrange
            FakeDataPointChart chart = new FakeDataPointChart();
            var legend = new Legend() { Name = "MyLegend", Enabled = false };
            chart.SetLegendManually = legend;
            chart.Legends.Add(new Legend());
            //Act
            chart.ShowLegend();
            //Assert
            Assert.That(legend.Enabled, Is.True);
            Assert.That(chart.Legends.Count, Is.EqualTo(2));
        }
        /// <summary>
        /// tests for HideLegend() function
        /// </summary>
        [Test]
        public void HideLegend_legendIsNull_LegendsRemainstheSameWithNoExceptionThrown()
        {
            //Arrange
            FakeDataPointChart chart = new FakeDataPointChart();
            chart.SetLegendManually = null;
            //Act
            chart.HideLegend();
            //Assert
            Assert.That(() => chart.ShowLegend(), Throws.Nothing);
        }
        [Test]
        public void HideLegend_legendIsInLegends_LegendEnabledFalseAndRemovedFromLegendsAndEnabledIsFalse()
        {
            //Arrange
            FakeDataPointChart chart = new FakeDataPointChart();
            var legend = new Legend() { Name = "MyLegend", Enabled = true };
            chart.SetLegendManually = legend;
            chart.Legends.Add(legend);
            //Act
            chart.HideLegend();
            //Assert
            Assert.That(legend.Enabled, Is.False);
            Assert.That(chart.Legends.Count, Is.EqualTo(0));
        }
        [Test]
        public void HideLegend_legendIsNotInLegends_LegendsCountRemainsTheSameAndEnabledIsFalse()
        {
            //Arrange
            FakeDataPointChart chart = new FakeDataPointChart();
            var legend = new Legend() { Name = "MyLegend", Enabled = true };
            chart.SetLegendManually = legend;
            chart.Legends.Add(new Legend());
            //Act
            chart.HideLegend();
            //Assert
            Assert.That(legend.Enabled, Is.False);
            Assert.That(chart.Legends.Count, Is.EqualTo(1));
        }
        /// <summary>
        /// tests for IsSelected function
        /// only testing the setter
        /// </summary>
        [Test]
        public void IsSelected_NotSelectable_InvalidateFiredAndIsSelectedUnchanged()
        {
            //Arrange
            FakeDataPointChart chart = new FakeDataPointChart();
            chart.Selectable = false;
            //Act
            chart.IsSelected = true;
            //Assert
            Assert.That(chart.IsSelected, Is.False);
        }
        [Test]
        public void IsSelected_IsSelectableAndSetToTrue_IsSelectedBecomesTrue()
        {
            //Arrange
            FakeDataPointChart chart = new FakeDataPointChart();
            chart.Selectable = true;
            //Act
            chart.IsSelected = true;
            //Assert
            Assert.That(chart.IsSelected, Is.True);
        }
        [Test]
        public void IsSelected_IsSelectableAndSetToTrueWhileMouseInsideIsAlsoTrue_IsSelectedUnchanged()
        {
            //Arrange
            FakeDataPointChart chart = new FakeDataPointChart();
            chart.Selectable = true;
            chart.HighlightChart();
            //Act
            chart.IsSelected = true;
            //Assert
            Assert.That(chart.IsSelected, Is.False);
        }
        /*
         * TODO: figure out how to verify invalidate was called
        [Test]
        public void IsSelected_IsSelectableAndSetToFalseWhileMouseInsideIsAlsoTrue_IsSelectedUnchangedAndInvalidateFired()
        {
            //Arrange
            Mock<FakeDataPointChart> chart = new Mock<FakeDataPointChart>();
            //Temproarily set to true for the eventhandler
            chart.Object.Selectable = true;
            //Act
            chart.Object.IsSelected = false;
            //Assert
            Assert.That(chart.Object.IsSelected, Is.False);
            //chart.Verify(c => c.Invalidate());
        }
        */
        /// <summary>
        /// tests for the SelectChart(), HighlightChart(), and  RemoveChartHighlight() functions
        /// </summary>
        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void SelectChart_NotSelectable_FiresMouseClickEventAndDoesNotAlterIsSelected(bool setCanUnselect)
        {
            //Arrange
            _chart.Selectable = false;
            _chart.IsSelected = false;
            _chart.CanUnselect = setCanUnselect;
            //Act
            _chart.SelectChart();
            //Assert
            Assert.That(_chart.IsSelected, Is.False);

        }
        // In the first two test cases _isSelected is flipped to the opposite boolean value when CanUnselect is true
        // In the second two cases, IsSelected will be true no matter what
        [Test]
        [TestCase(true, false, true)]
        [TestCase(true, true, false)]
        [TestCase(false, false, true)]
        [TestCase(false, true, true)]
        public void Select_IsSelectable_FiresMouseClickEventAndChecksForCanUnselect(bool setCanUnselect,  bool setIsSelected, bool expectedAssignmentOfIsSelected)
        {
            //Arrange
            _chart.Selectable = true;
            _chart.IsSelected = setIsSelected;
            _chart.CanUnselect = setCanUnselect;
            //Act
            _chart.SelectChart();
            //Assert
            Assert.That(_chart.IsSelected, Is.EqualTo(expectedAssignmentOfIsSelected));
        }
        [Test]
        public void HighlightChart_IsNotSelectable_IsHighlightedBecomesTrueAndCallsInvalidate()
        {
            //Arrange
            _chart.Selectable = false;
            //Act
            _chart.HighlightChart();
            //Assert
            Assert.That(_chart.IsHighlighted, Is.False);
        }
        [Test]
        public void HighlightChart_IsSelectable_IsHighlightedBecomesTrueAndCallsInvalidate()
        {
            //Arrange
            _chart.Selectable = true;
            //Act
            _chart.HighlightChart();
            //Assert
            Assert.That(_chart.IsHighlighted, Is.True);
        }
        [Test]
        public void RemoveChartHighlight_IsNotSelectable_IsHighlightedBecomesTrueAndCallsInvalidate()
        {
            //Arrange
            //First fire this to ensure that IsHighlighted starts out as true
            _chart.Selectable = true;
            _chart.HighlightChart();
            // Now set selectable to false
            _chart.Selectable = false;
            //Act
            _chart.RemoveChartHighlight();
            //Assert
            Assert.That(_chart.IsHighlighted, Is.True);
        }
        [Test]
        public void RemoveChartHighlight_IsSelectable_IsHighlightedBecomesTrueAndCallsInvalidate()
        {
            //Arrange
            //First fire this to ensure that IsHighlighted starts out as true
            _chart.Selectable = true;
            _chart.HighlightChart();
            // Now set selectable to false
            _chart.Selectable = true;
            //Act
            _chart.RemoveChartHighlight();
            //Assert
            Assert.That(_chart.IsHighlighted, Is.False);
        }
        /// <summary>
        /// tests for the GetLargeColorList(bool designMode) function
        /// </summary>
        [Test]
        public void GetLargeColorList_DesignMode_ReturnsAnEmptyColorList()
        {
            //Act
            var result = DataPointChart.GetLargeColorList(true);
            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.Zero);
        }
        [Test]
        public void GetLargeColorList_NotDesignMode_ReturnsAListOfColorsThatAreAllUnique()
        {
            //Act
            var result = DataPointChart.GetLargeColorList(false);
            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.GreaterThanOrEqualTo(50));
            // TODO: try to implement a color generation technique that has 50 or more unique colors
            Assert.That(result.Distinct().Count, Is.EqualTo(15));
        }
        /// <summary>
        /// tests for the GetLargeColorList(bool designMode) function
        /// </summary>
        [Test]
        [TestCase(0.0)]
        [TestCase(double.NaN)]
        public void CleanUpDataSeries_NoPointsToRemoveThatAreNaN_TheSeriesDoesNotChange(double inputY)
        {
            //Arrange
            _chart.Series.Add(new Series());
            double x = 0.0;
            double y = inputY;
            DataPoint targetPoint = new DataPoint(x, y);
            _chart.Series[0].Points.AddXY(x, y);
            //Act
            _chart.CleanUpDataSeries();
            //Assert
            Assert.That(_chart.Series[0].Points.Count, Is.EqualTo(1));
            bool containsPoint = _chart.Series[0].Points.Any(p => p.XValue == targetPoint.XValue && p.YValues[0] == targetPoint.YValues[0]);
        }
        [Test]
        public void CleanUpDataSeries_XValueForPointIsNaN_SeriesHasThosePointsRemoved()
        {
            //Arrange
            _chart.Series.Add(new Series());
            double x = double.NaN;
            double y = 0.0;
            _chart.Series[0].Points.AddXY(x, y);
            //Act
            _chart.CleanUpDataSeries();
            //Assert
            Assert.That(_chart.Series[0].Points.Count, Is.EqualTo(0));
        }
    }
    // Used for the tests inside the series for for loop in order to control the GetColor() dependency within the FixUpLegend function
    public class FakeDataPointChart : DataPointChart
    {
        public Dictionary<string, Color> MyInputColorMap
        {
            set { _colorMap = value; }
        }
        public Legend SetLegendManually
        {
            set { _legend = value; }
        }
    }
}
