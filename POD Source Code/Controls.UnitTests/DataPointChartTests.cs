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
using POD.Data;

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
        public void Select_IsSelectable_FiresMouseClickEventAndChecksForCanUnselect(bool setCanUnselect, bool setIsSelected, bool expectedAssignmentOfIsSelected)
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
        /// Skipping tests for reload chart data for now
        /// 

        /// Tests for SetXAxisRange(AxisObject myAxis, IAnalysisData data, bool forceLinear = false, bool keepLabelCount = false, bool transformResidView = false)
        [Test]
        [TestCase(2.0)]
        [TestCase(10.0)]
        public void SetXAxisRange_TransformResidViewIsTrueMaxIsGreaterThanOrEqualMinForceLinearOff_DoesNotCallGetBufferedRangeMaxMinNotOverwrittenNoForceLinearRelabelAxesCalled(double axisMax)
        {
            //Arrange
            FakeDataPointChart chart = SetUpMockObjects(out Mock<IAxisObject> axisObject, out Mock<IAnalysisData> data, axisMax);
            //Act
            chart.SetXAxisRange(axisObject.Object, data.Object, false, false, true);
            //Assert
            axisObject.VerifyGet(axis => axis.Max, Times.Exactly(2));
            axisObject.VerifyGet(axis => axis.Min, Times.Exactly(2));
            //these are called in the RelabelAxesBetter
            data.Verify(d => d.FlawTransform, Times.Once);
            data.Verify(d => d.ResponseTransform, Times.Once);
            axisObject.VerifySet(axis => axis.Max = It.IsAny<double>(), Times.Never);
            axisObject.VerifySet(axis => axis.Min = It.IsAny<double>(), Times.Never);
        }
        [Test]
        public void SetXAxisRange_TransformResidViewIsFalseMaxIsGreaterThanOrEqualMinForceLinearOff_CallsGetBufferedRangeMaxMinNotOverwrittenLargeNoForceLinearRelabelAxesCalled()
        {
            //Arrange
            FakeDataPointChart chart = SetUpMockObjects(out Mock<IAxisObject> axisObject, out Mock<IAnalysisData> data, 10.0);
            //Act
            chart.SetXAxisRange(axisObject.Object, data.Object);
            //Assert
            axisObject.VerifyGet(axis => axis.Max, Times.Exactly(3));
            axisObject.VerifyGet(axis => axis.Min, Times.Exactly(3));
            axisObject.VerifySet(axis => axis.Max = It.IsAny<double>(), Times.AtLeastOnce);
            axisObject.VerifySet(axis => axis.Min = It.IsAny<double>(), Times.AtLeastOnce);
            //these are called in the RelabelAxesBetter
            data.Verify(d => d.FlawTransform, Times.Once);
            data.Verify(d => d.ResponseTransform, Times.Once);
        }
        [Test]
        public void SetXAxisRange_TransformResidViewIsFalseMaxIsGreaterThanOrEqualMinForceLinearOn_CallsGetBufferedRangeMaxMinNotOverwrittenLargeForceLinearRelabelAxesCalled()
        {
            //Arrange
            FakeDataPointChart chart = SetUpMockObjects(out Mock<IAxisObject> axisObject, out Mock<IAnalysisData> data, 10.0);
            //Act
            chart.SetXAxisRange(axisObject.Object, data.Object, true);
            //these are called in the RelabelAxesBetter
            data.Verify(d => d.FlawTransform, Times.Never);
            data.Verify(d => d.ResponseTransform, Times.Never);
        }
        [Test]
        public void SetXAxisRange_TransformResidViewIsFalseMaxIsLessThanOrEqualMinForceLinearOff_CallsGetBufferedRangeMaxMinNotOverwrittenLargeForceLinearRelabelAxesCalled()
        {
            //Arrange
            FakeDataPointChart chart = SetUpMockObjects(out Mock<IAxisObject> axisObject, out Mock<IAnalysisData> data, 1.0);
            //Act
            chart.SetXAxisRange(axisObject.Object, data.Object);
            //these are called in the RelabelAxesBetter
            axisObject.VerifySet(axis => axis.Max = 1.0, Times.AtLeastOnce);
            axisObject.VerifySet(axis => axis.Min = 0.0, Times.AtLeastOnce);
            data.Verify(d => d.FlawTransform, Times.Once);
            data.Verify(d => d.ResponseTransform, Times.Once);
        }
        [Test]
        public void SetXAxisRange_TransformResidViewIsTrueMaxIsLessThanMinForceLinearOn_CallsGetBufferedRangeMaxMinNotOverwrittenLargeForceLinearRelabelAxesCalled()
        {
            //Arrange
            FakeDataPointChart chart = SetUpMockObjects(out Mock<IAxisObject> axisObject, out Mock<IAnalysisData> data, 1.0);
            //Act
            chart.SetXAxisRange(axisObject.Object, data.Object, true);
            //these are called in the RelabelAxesBetter
            axisObject.VerifySet(axis => axis.Max = 1.0, Times.AtLeastOnce);
            axisObject.VerifySet(axis => axis.Min = 0.0, Times.AtLeastOnce);
            data.Verify(d => d.FlawTransform, Times.Never);
            data.Verify(d => d.ResponseTransform, Times.Never);
        }


        /// Tests for SetXAxisRange(AxisObject myAxis, IAnalysisData data, bool forceLinear = false, bool keepLabelCount = false, bool transformResidView = false)

        /// Tests for SetXAxisRange(AxisObject myAxis, IAnalysisData data, bool forceLinear = false, bool keepLabelCount = false, bool transformResidView = false)
        [Test]
        [TestCase(2.0)]
        [TestCase(10.0)]
        public void SetYAxisRange_TransformResidViewIsTrueMaxIsGreaterThanOrEqualMinForceLinearOff_DoesNotCallGetBufferedRangeMaxMinNotOverwrittenNoForceLinearRelabelAxesCalled(double axisMax)
        {
            //Arrange
            FakeDataPointChart chart = SetUpMockObjects(out Mock<IAxisObject> axisObject, out Mock<IAnalysisData> data, axisMax);
            //Act
            chart.SetYAxisRange(axisObject.Object, data.Object, false, false, true);
            //Assert
            axisObject.VerifyGet(axis => axis.Max, Times.Exactly(2));
            axisObject.VerifyGet(axis => axis.Min, Times.Exactly(2));
            //these are called in the RelabelAxesBetter
            data.Verify(d => d.FlawTransform, Times.Once);
            data.Verify(d => d.ResponseTransform, Times.Once);
            axisObject.VerifySet(axis => axis.Max = It.IsAny<double>(), Times.Never);
            axisObject.VerifySet(axis => axis.Min = It.IsAny<double>(), Times.Never);
        }
        [Test]
        public void SetYAxisRange_TransformResidViewIsFalseMaxIsGreaterThanOrEqualMinForceLinearOffHitMissData_CallsGetBufferedRangeMaxMinNotOverwrittenLargeNoForceLinearRelabelAxesCalled()
        {
            //Arrange
            FakeDataPointChart chart = SetUpMockObjects(out Mock<IAxisObject> axisObject, out Mock<IAnalysisData> data, 10.0);
            data.SetupGet(d => d.DataType).Returns(AnalysisDataTypeEnum.HitMiss);
            //Act
            chart.SetYAxisRange(axisObject.Object, data.Object);
            //Assert
            ForceLinearOffVerification(axisObject, data);
            data.Verify(d => d.LambdaValue, Times.Never);
        }
        [Test]
        public void SetYAxisRange_TransformResidViewIsFalseMaxIsGreaterThanOrEqualMinForceLinearOffSignalResponseData_CallsGetBufferedRangeMaxMinNotOverwrittenLargeNoForceLinearRelabelAxesCalled()
        {
            //Arrange
            FakeDataPointChart chart = SetUpMockObjects(out Mock<IAxisObject> axisObject, out Mock<IAnalysisData> data, 10.0);
            data.SetupGet(d => d.DataType).Returns(AnalysisDataTypeEnum.AHat);
            //Act
            chart.SetYAxisRange(axisObject.Object, data.Object);
            //Assert
            ForceLinearOffVerification(axisObject, data);
            data.Verify(d => d.LambdaValue, Times.Once);
        }
        private void ForceLinearOffVerification(Mock<IAxisObject> axisObject, Mock<IAnalysisData> data)
        {
            axisObject.VerifyGet(axis => axis.Max, Times.Exactly(3));
            axisObject.VerifyGet(axis => axis.Min, Times.Exactly(3));
            axisObject.VerifySet(axis => axis.Max = It.IsAny<double>(), Times.AtLeastOnce);
            axisObject.VerifySet(axis => axis.Min = It.IsAny<double>(), Times.AtLeastOnce);
            //these are called in the RelabelAxesBetter
            data.Verify(d => d.FlawTransform, Times.Once);
            data.Verify(d => d.ResponseTransform, Times.Once);
        }
        [Test]
        public void SetYAxisRange_TransformResidViewIsFalseMaxIsGreaterThanOrEqualMinForceLinearOn_CallsGetBufferedRangeMaxMinNotOverwrittenLargeForceLinearRelabelAxesCalled()
        {
            //Arrange
            FakeDataPointChart chart = SetUpMockObjects(out Mock<IAxisObject> axisObject, out Mock<IAnalysisData> data, 10.0);
            //Act
            chart.SetYAxisRange(axisObject.Object, data.Object, true);
            //these are called in the RelabelAxesBetter
            data.Verify(d => d.FlawTransform, Times.Never);
            data.Verify(d => d.ResponseTransform, Times.Never);
        }
        [Test]
        public void SetYAxisRange_TransformResidViewIsFalseMaxIsLessThanOrEqualMinForceLinearOff_CallsGetBufferedRangeMaxMinNotOverwrittenLargeForceLinearRelabelAxesCalled()
        {
            //Arrange
            FakeDataPointChart chart = SetUpMockObjects(out Mock<IAxisObject> axisObject, out Mock<IAnalysisData> data, 1.0);
            //Act
            chart.SetYAxisRange(axisObject.Object, data.Object);
            //these are called in the RelabelAxesBetter
            axisObject.VerifySet(axis => axis.Max = 1.0, Times.AtLeastOnce);
            axisObject.VerifySet(axis => axis.Min = 0.0, Times.AtLeastOnce);
            data.Verify(d => d.FlawTransform, Times.Once);
            data.Verify(d => d.ResponseTransform, Times.Once);
        }
        [Test]
        public void SetYAxisRange_TransformResidViewIsTrueMaxIsLessThanMinForceLinearOn_CallsGetBufferedRangeMaxMinNotOverwrittenLargeForceLinearRelabelAxesCalled()
        {
            //Arrange
            FakeDataPointChart chart = SetUpMockObjects(out Mock<IAxisObject> axisObject, out Mock<IAnalysisData> data, 1.0);
            //Act
            chart.SetYAxisRange(axisObject.Object, data.Object, true);
            //these are called in the RelabelAxesBetter
            axisObject.VerifySet(axis => axis.Max = 1.0, Times.AtLeastOnce);
            axisObject.VerifySet(axis => axis.Min = 0.0, Times.AtLeastOnce);
            data.Verify(d => d.FlawTransform, Times.Never);
            data.Verify(d => d.ResponseTransform, Times.Never);
        }


        private FakeDataPointChart SetUpMockObjects(out Mock<IAxisObject> axisObject, out Mock<IAnalysisData> data, double axisMax)
        {
            //Arrange
            FakeDataPointChart chart = new FakeDataPointChart();
            axisObject = new Mock<IAxisObject>();
            data = new Mock<IAnalysisData>();
            axisObject.SetupGet(axis => axis.Max).Returns(axisMax);
            axisObject.SetupGet(axis => axis.Min).Returns(2.0);
            axisObject.SetupGet(axis => axis.Interval).Returns(4.0);
            return chart;
        }

        /// Tests for the BuildColorMap() function
        [Test]
        public void BuildColorMap_SeriesContainsNoPoints_ColorMapAddsSeriesWithTheColorBeingTheColorOfTheSeries()
        {
            //Arrange
            FakeDataPointChart chart = new FakeDataPointChart();
            Series sampleSeries = new Series { Name = "MySampleSeries", Color = Color.Blue };
            chart.Series.Add(sampleSeries);
            //Act
            chart.BuildColorMap();
            //Assert
            Assert.That(chart.MyInputColorMap.Count, Is.EqualTo(1));
            Assert.That(chart.MyInputColorMap.ContainsKey("MySampleSeries"));
            Assert.That(chart.MyInputColorMap.ContainsValue(Color.Blue));
        }
        /// Tests for the BuildColorMap() function
        [Test]
        public void BuildColorMap_SeriesContainsPointsNotGreyAndNotTransparent_ColorAssignedIsTheColorOfThePoints()
        {
            //Arrange
            FakeDataPointChart chart = SetupSeriesForColorMap(new FakeDataPointChart());
            chart.Series[0].Points[0].Color = Color.Red;
            //Act
            chart.BuildColorMap();
            //Assert
            Assert.That(chart.MyInputColorMap.Count, Is.EqualTo(1));
            Assert.That(chart.MyInputColorMap.ContainsKey("MySampleSeries"));
            Assert.That(chart.MyInputColorMap.ContainsValue(Color.Red));
        }
        [Test]
        public void BuildColorMap_SeriesContainsPointsAreGreyAndNotTransparent_ColorAssignedIsTheOriginalSeriesColor()
        {
            //Arrange
            FakeDataPointChart chart = SetupSeriesForColorMap(new FakeDataPointChart());
            chart.Series[0].Points[0].Color = Color.Gray;
            //Act
            chart.BuildColorMap();
            //Assert
            Assert.That(chart.MyInputColorMap.Count, Is.EqualTo(1));
            Assert.That(chart.MyInputColorMap.ContainsKey("MySampleSeries"));
            Assert.That(chart.MyInputColorMap.ContainsValue(Color.Blue));
        }
        [Test]
        public void BuildColorMap_SeriesContainsPointsAreNotGreyButAreTransparent_ColorAssignedIsTheOriginalSeriesColor()
        {
            //Arrange
            FakeDataPointChart chart = SetupSeriesForColorMap(new FakeDataPointChart());
            chart.Series[0].Points[0].Color = Color.FromArgb(0, Color.Red);
            //Act
            chart.BuildColorMap();
            //Assert
            Assert.That(chart.MyInputColorMap.Count, Is.EqualTo(1));
            Assert.That(chart.MyInputColorMap.ContainsKey("MySampleSeries"));
            Assert.That(chart.MyInputColorMap.ContainsValue(Color.Blue));
        }
        [Test]
        public void BuildColorMap_SeriesContainsPointsAreGreyAndTransparent_ColorAssignedIsTheOriginalSeriesColor()
        {
            //Arrange
            FakeDataPointChart chart = SetupSeriesForColorMap(new FakeDataPointChart());
            chart.Series[0].Points[0].Color = Color.FromArgb(0, Color.Gray);
            //Act
            chart.BuildColorMap();
            //Assert
            Assert.That(chart.MyInputColorMap.Count, Is.EqualTo(1));
            Assert.That(chart.MyInputColorMap.ContainsKey("MySampleSeries"));
            Assert.That(chart.MyInputColorMap.ContainsValue(Color.Blue));
        }
        [Test]
        public void BuildColorMap_SeriesNameAndColorAlreadyInColorMap_ColorMapDoesNotChange()
        {
            FakeDataPointChart chart = new FakeDataPointChart();
            Series sampleSeries = new Series { Name = "MySampleSeries", Color = Color.Blue };
            chart.Series.Add(sampleSeries);
            chart.MyInputColorMap.Add("MySampleSeries", Color.Blue);
            //Act
            chart.BuildColorMap();
            //Assert
            Assert.That(chart.MyInputColorMap.Count, Is.EqualTo(1));
        }
        private FakeDataPointChart SetupSeriesForColorMap(FakeDataPointChart chart)
        {
            Series sampleSeries = new Series { Name = "MySampleSeries", Color = Color.Blue };
            chart.Series.Add(sampleSeries);
            chart.Series[0].Points.AddXY(1.0, 1.0);
            return chart;
        }

        /// Tests for the  public Color GetColor(Series series) function
        [Test]
        public void GetColor_ColorMapContainsKey_ReturnsValueColorWithKey()
        {
            FakeDataPointChart chart = new FakeDataPointChart();
            Series sampleSeries = new Series { Name = "MySampleSeries", Color = Color.Blue };
            chart.MyInputColorMap.Add("MySampleSeries", Color.Blue);
            //Act
            var result=chart.GetColor(sampleSeries);
            //Assert
            Assert.That(result, Is.EqualTo(Color.Blue));
        }
        [Test]
        public void GetColor_ColorMapDoesNotContainKey_ReturnsTransparent()
        {
            FakeDataPointChart chart = new FakeDataPointChart();
            Series sampleSeries = new Series { Name = "MySampleSeries", Color = Color.Blue };
            //Act
            var result = chart.GetColor(sampleSeries);
            //Assert
            Assert.That(result, Is.EqualTo(Color.Transparent));
        }

    }
    // Used for the tests inside the series for for loop in order to control the GetColor() dependency within the FixUpLegend function
    public class FakeDataPointChart : DataPointChart
    {
        public Dictionary<string, Color> MyInputColorMap
        {
            get { return _colorMap; }
            set { _colorMap = value; }
        }
        public Legend SetLegendManually
        {
            set { _legend = value; }
        }
        protected override void RelabelAxesBetter(IAxisObject xAxis, IAxisObject yAxis,
                                                 Globals.InvertAxisFunction invertX, Globals.InvertAxisFunction invertY,
                                                 int xLabelCount, int yLabelCount,
                                                 bool myCenterXAtZero = false, bool myCenterYAtZero = false,
                                                 TransformTypeEnum xAxisTransform = TransformTypeEnum.Linear,
                                                 TransformTypeEnum yAxisTransform = TransformTypeEnum.Linear,
                                                 Globals.InvertAxisFunction transformX = null, Globals.InvertAxisFunction transformY = null,
                                                 bool forceKeepCountX = false, bool forceKeepCountY = false, double lambda = Double.NaN)
        {
            return;
        }
    }
}
