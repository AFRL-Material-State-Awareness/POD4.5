using System;
using System.Collections.Generic;
using System.Linq;
using POD.Controls;
using NUnit.Framework;
using Moq;
using System.Windows.Forms;
using POD;
using System.Reflection;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;
using POD.Data;
using System.Data;
using CSharpBackendWithR;

namespace Controls.UnitTests
{
    [TestFixture]
    public class RegressionAnalysisChartTests
    {
        private FakeRegressionAnalysisChart _regressionAnalysisChart;
        private Mock<IAnalysisData> _data;
        private bool _linesChangedFired;
        [SetUp]
        public void Setup()
        {
            _regressionAnalysisChart = new FakeRegressionAnalysisChart();
            SetUpAnalysisDataObject();
            _linesChangedFired = false;
        }
        private void SetUpAnalysisDataObject()
        {
            _data = new Mock<IAnalysisData>();
            _data.Setup(d => d.ActivatedFlaws).Returns(new DataTable());
            _data.Setup(d => d.ActivatedResponses).Returns(new DataTable());
            _data.Setup(d => d.ActivatedSpecimenIDs).Returns(new DataTable());
            _regressionAnalysisChart.Data= _data.Object;
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

        /// Tests for the void DrawEquation function
        [Test]
        public void DrawEquation_EquationIsNull_EquationAndBoxNotAddedToAnnotations()
        {
            //Arrange
            _regressionAnalysisChart.Equation = null;
            //Act
            _regressionAnalysisChart.DrawEquation();
            //Assert
            Assert.That(_regressionAnalysisChart.Annotations.Count, Is.EqualTo(2));
        }
        [Test]
        public void DrawEquation_EquationIsNotNull_EquationAndBoxNotAddedToAnnotations()
        {
            //Arrange
            _regressionAnalysisChart.Equation = new TextAnnotation();
            //Act
            _regressionAnalysisChart.DrawEquation();
            //Assert
            Assert.That(_regressionAnalysisChart.Annotations.Count, Is.Zero);
        }
        /// Tests for the void DrawAMaxBoundLine function
        [Test]
        public void DrawAMaxBoundLine_AMaxLineIsNull_AMaxLineAddedToAnnotations()
        {
            //Arrange
            _regressionAnalysisChart.AMaxLine = null;
            //Act
            _regressionAnalysisChart.DrawAMaxBoundLine();
            //Assert
            Assert.That(_regressionAnalysisChart.Annotations.Count, Is.EqualTo(1));
            _data.VerifyGet(d => d.ActivatedFlaws);
        }
        [Test]
        public void DrawAMaxBoundLine_AMaxLineIsNotNull_NotAddedToAnnotations()
        {
            //Arrange
            _regressionAnalysisChart.AMaxLine = new VerticalLineAnnotation();
            //Act
            _regressionAnalysisChart.DrawAMaxBoundLine();
            //Assert
            Assert.That(_regressionAnalysisChart.Annotations.Count, Is.Zero);
        }
        /// Tests for the void DrawAMinBoundLine function
        [Test]
        public void DrawAMinBoundLine_AMinLineIsNull_AMaxLineAddedToAnnotations()
        {
            //Arrange
            _regressionAnalysisChart.AMinLine = null;
            //Act
            _regressionAnalysisChart.DrawAMinBoundLine();
            //Assert
            Assert.That(_regressionAnalysisChart.Annotations.Count, Is.EqualTo(1));
            _data.VerifyGet(d => d.ActivatedFlaws);
        }
        [Test]
        public void DrawAMinBoundLine_AMinLineIsNotNull_NotAddedToAnnotations()
        {
            //Arrange
            _regressionAnalysisChart.AMinLine = new VerticalLineAnnotation();
            //Act
            _regressionAnalysisChart.DrawAMinBoundLine();
            //Assert
            Assert.That(_regressionAnalysisChart.Annotations.Count, Is.Zero);
        }
        /// Tests for the void DrawThresholdBoundLine function
        [Test]
        public void DrawThresholdBoundLine_ThresholdLineIsNull_ThresholdLineAddedToAnnotations()
        {
            //Arrange
            _regressionAnalysisChart.ThresholdLine = null;
            //Act
            _regressionAnalysisChart.DrawThresholdBoundLine();
            //Assert
            Assert.That(_regressionAnalysisChart.Annotations.Count, Is.EqualTo(1));
        }
        [Test]
        public void DrawThresholdBoundLine_ThresholdLineIsNotNull_NotAddedToAnnotations()
        {
            //Arrange
            _regressionAnalysisChart.ThresholdLine = new HorizontalLineAnnotation();
            //Act
            _regressionAnalysisChart.DrawThresholdBoundLine();
            //Assert
            Assert.That(_regressionAnalysisChart.Annotations.Count, Is.Zero);
        }
        [Test]
        public void DrawThresholdBoundLine_ThresholdFreezeIsTrue_AllowMovingFalseAndSetToFreezeValue()
        {
            //Arrange
            var freezeValue = 1.0;
            _regressionAnalysisChart.ThresholdLine = new HorizontalLineAnnotation();
            _regressionAnalysisChart.ThresholdLine.AllowMoving = true;
            _regressionAnalysisChart.FreezeThresholdLine(freezeValue);
            //Act
            _regressionAnalysisChart.DrawThresholdBoundLine();
            //Assert
            Assert.That(_regressionAnalysisChart.ThresholdLine.AllowMoving, Is.False);
            Assert.That(_regressionAnalysisChart.ThresholdLine.Y, Is.EqualTo(freezeValue));
        }
        [Test]
        public void DrawThresholdBoundLine_ThresholdFreezeIsFalse_AllowMovingTrueAndNotSetToFreezeValue()
        {
            //Arrange
            _regressionAnalysisChart.ThresholdLine = new HorizontalLineAnnotation();
            _regressionAnalysisChart.ThresholdLine.AllowMoving = true;
            //Act
            _regressionAnalysisChart.DrawThresholdBoundLine();
            //Assert
            Assert.That(_regressionAnalysisChart.ThresholdLine.AllowMoving, Is.True);
            Assert.That(_regressionAnalysisChart.ThresholdLine.Y, Is.EqualTo(double.NaN));
        }

        /// Tests for the bool FindValue(ControlLine line, ref double myValue) function
        [Test]
        [TestCase(ControlLine.AMax, 10.0)]
        [TestCase(ControlLine.AMin, 20.0)]
        [TestCase(ControlLine.Threshold, 30.0)]
        public void FindValue_ControlLineIsValidAndValueIsNotNaN_ReturnsTrueAndAssignsTheXYValue(ControlLine controlLine, double expectedValue)
        {
            //Arrange
            SetUpLineValues(10.0, 20.0, 30.0);
            double myValue = -1.0;
            //Act
            var result=_regressionAnalysisChart.FindValue(controlLine, ref myValue);
            //Assert
            Assert.That(result, Is.True);
            Assert.That(myValue, Is.EqualTo(expectedValue));
        }
        [Test]
        [TestCase(ControlLine.AMax, 1.0)]
        [TestCase(ControlLine.AMin, 2.0)]
        [TestCase(ControlLine.Threshold, 3.0)]
        public void FindValue_ControlLineIsValidAndValueIsNaN_ReturnsTrueAndTheXYAnchorValues(ControlLine controlLine, double expectedValue)
        {
            //Arrange
            SetUpLineValues(double.NaN, double.NaN, double.NaN);
            double myValue = -1.0;
            //Act

            var result=_regressionAnalysisChart.FindValue(controlLine, ref myValue);
            //Assert
            Assert.That(result, Is.True);
            Assert.That(myValue, Is.EqualTo(expectedValue));
        }
        [Test]
        public void FindValue_ControlLineIsInValid_ReturnsFalseAndAssignsNaN()
        {
            //Arrange
            double myValue = -1.0;
            //Act
            var result=_regressionAnalysisChart.FindValue(ControlLine.LeftCensor, ref myValue);
            //Assert
            Assert.That(result, Is.False);
            Assert.That(myValue, Is.EqualTo(double.NaN));
        }
        private void SetUpLineValues(double value1, double value2, double value3)
        {
            _regressionAnalysisChart.AMaxLine = new VerticalLineAnnotation() { X = value1, AnchorX = 1.0 };
            _regressionAnalysisChart.AMinLine = new VerticalLineAnnotation() { X = value2, AnchorX = 2.0 };
            _regressionAnalysisChart.ThresholdLine = new HorizontalLineAnnotation() { Y = value3, AnchorY = 3.0 };
        }
        /// Tests for OnLinesChanged(EventArgs e) function
        [Test]
        public void OnLinesChanged_LinesChangedIsNull_NoEventFired()
        {
            //_regressionAnalysisChart.LinesChanged += OnLinesChanged;
            //Act
            _regressionAnalysisChart.OnLinesChanged(EventArgs.Empty);
            //Assert
            Assert.That(_linesChangedFired, Is.False);
            _data.Verify(d => d.UpdateIncludedPointsBasedFlawRange(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<List<FixPoint>>()), Times.Never);
        }
        [Test]
        public void OnLinesChanged_LinesChangedIsNotNull_EventFiredAndDeterminePointsInThresholdCalled()
        {
            SetUpLineValues(10.0, 20.0, 30.0);
            _regressionAnalysisChart.LinesChanged += OnLinesChanged;
            //Act
            _regressionAnalysisChart.OnLinesChanged(EventArgs.Empty);
            //Assert
            Assert.That(_linesChangedFired, Is.True);
            _data.Verify(d => d.UpdateIncludedPointsBasedFlawRange(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<List<FixPoint>>()));
        }
        private void OnLinesChanged(object sender, EventArgs args)
        {
            _linesChangedFired = true;
        }
        /// Tests for void PickBestAxisRange(double myTransformedResponseMin, 
        /// double myTransformedResponseMax, double myTransformedFlawMin, double myTransformedFlawMax,
        /// IAxisObject xaxisT = null, IAxisObject yAxisT = null) function
        private double _flawMin = 1.0;
        private double _flawMax = 10.0;
        private double _responseMin = 20.0;
        private double _responseMax = 30.0;
        [Test]
        public void PickBestAxisRange_MaxNeverSmallerThanTransformMaxAndMinNeverLargerThanTransformMin_NoValuesReassigned()
        {
            //Arrange
            SetupAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, _responseMin, _responseMax);
            //Act
            _regressionAnalysisChart.PickBestAxisRange(_responseMin, _responseMax, _flawMin, _flawMax, xAxis.Object, yAxis.Object);
            //Assert
            xAxis.VerifySet(x => x.Min = It.IsAny<double>(), Times.Never);
            xAxis.VerifySet(x => x.Max = It.IsAny<double>(), Times.Never);
            yAxis.VerifySet(y => y.Min = It.IsAny<double>(), Times.Never);
            yAxis.VerifySet(y => y.Max = It.IsAny<double>(), Times.Never);
        }
        [Test]
        public void PickBestAxisRange_OriginalMaxSmallerThanTransformedResponseMax_ResponseMaxReassigned()
        {
            //Arrange
            SetupAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, _responseMin, _responseMax);
            //Act
            _regressionAnalysisChart.PickBestAxisRange(_responseMin, _responseMax + 1, _flawMin, _flawMax, xAxis.Object, yAxis.Object);
            //Assert
            xAxis.VerifySet(x => x.Min = It.IsAny<double>(), Times.Never);
            xAxis.VerifySet(x => x.Max = It.IsAny<double>(), Times.Never);
            yAxis.VerifySet(y => y.Min = It.IsAny<double>(), Times.Never);
            yAxis.VerifySet(y => y.Max = 31.0);

        }
        [Test]
        public void PickBestAxisRange_OriginalMinLargerThanTransformedResponseMin_ResponseMinReassigned()
        {
            //Arrange
            SetupAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, _responseMin, _responseMax);
            //Act
            _regressionAnalysisChart.PickBestAxisRange(_responseMin - 1, _responseMax, _flawMin, _flawMax, xAxis.Object, yAxis.Object);
            //Assert
            xAxis.VerifySet(x => x.Min = It.IsAny<double>(), Times.Never);
            xAxis.VerifySet(x => x.Max = It.IsAny<double>(), Times.Never);
            yAxis.VerifySet(y => y.Min = 19.0);
            yAxis.VerifySet(y => y.Max = It.IsAny<double>(), Times.Never);

        }
        [Test]
        public void PickBestAxisRange_OriginalMaxSmallerThanTransformedFlawMax_FlawMaxReassigned()
        {
            //Arrange
            SetupAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, _responseMin, _responseMax);
            //Act
            _regressionAnalysisChart.PickBestAxisRange(_responseMin, _responseMax, _flawMin, _flawMax + 1, xAxis.Object, yAxis.Object);
            //Assert
            xAxis.VerifySet(x => x.Min = It.IsAny<double>(), Times.Never);
            xAxis.VerifySet(x => x.Max = 11.0);
            yAxis.VerifySet(x => x.Min = It.IsAny<double>(), Times.Never);
            yAxis.VerifySet(x => x.Max = It.IsAny<double>(), Times.Never);

        }
        [Test]
        public void PickBestAxisRange_SmallestFlawIsGreaterThanSmallestTransformFlaw_FlawMinReassigned()
        {
            //Arrange
            SetupAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, _responseMin, _responseMax);
            //Act
            _regressionAnalysisChart.PickBestAxisRange(_responseMin, _responseMax, _flawMin -1, _flawMax , xAxis.Object, yAxis.Object);
            //Assert
            xAxis.VerifySet(x => x.Min = 0.0);
            xAxis.VerifySet(x => x.Max = It.IsAny<double>(), Times.Never);
            yAxis.VerifySet(y => y.Min = It.IsAny<double>(), Times.Never);
            yAxis.VerifySet(y => y.Max = It.IsAny<double>(), Times.Never);

        }
        [Test]
        public void PickBestAxisRange_OriginalMinLargerThanTransformedResponseMinAndOriginalMaxSmallerThanTransformResponseMax_ResponseMaxAndMinReassigned()
        {
            //Arrange
            SetupAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, _responseMin, _responseMax);
            //Act
            _regressionAnalysisChart.PickBestAxisRange(_responseMin - 1, _responseMax +1, _flawMin, _flawMax, xAxis.Object, yAxis.Object);
            //Assert
            xAxis.VerifySet(x => x.Min = It.IsAny<double>(), Times.Never);
            xAxis.VerifySet(x => x.Max = It.IsAny<double>(), Times.Never);
            yAxis.VerifySet(y => y.Min = 19.0);
            yAxis.VerifySet(y => y.Max = 31.0);

        }

        [Test]
        public void PickBestAxisRange_OriginalMaxSmallerThanTransformedFlawMaxAndOriginalMaxSmallerThanTransformResponseMax_ResponseMaxAndFlawMaxReassigned()
        {
            //Arrange
            SetupAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, _responseMin, _responseMax);
            //Act
            _regressionAnalysisChart.PickBestAxisRange(_responseMin, _responseMax + 1, _flawMin, _flawMax + 1, xAxis.Object, yAxis.Object);
            //Assert
            xAxis.VerifySet(x => x.Min = It.IsAny<double>(), Times.Never);
            xAxis.VerifySet(x => x.Max = 11.0);
            yAxis.VerifySet(y => y.Min = It.IsAny<double>(), Times.Never);
            yAxis.VerifySet(y => y.Max = 31.0);

        }
        [Test]
        public void PickBestAxisRange_OriginalMinLargerThanTransformedFlawMinAndOriginalMaxSmallerThanTransformResponseMax_ResponseMaxAndFlawMinReassigned()
        {
            //Arrange
            SetupAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, _responseMin, _responseMax);
            //Act
            _regressionAnalysisChart.PickBestAxisRange(_responseMin, _responseMax + 1, _flawMin - 1, _flawMax, xAxis.Object, yAxis.Object);
            //Assert
            xAxis.VerifySet(x => x.Min = 0.0);
            xAxis.VerifySet(x => x.Max = It.IsAny<double>(), Times.Never);
            yAxis.VerifySet(y => y.Min = It.IsAny<double>(), Times.Never);
            yAxis.VerifySet(y => y.Max = 31.0);

        }
        [Test]
        public void PickBestAxisRange_OriginalMaxSmallerThanTransformedFlawMaxAndOriginalMinLargerThanTransformResponseMin_FlawMaxAndResponseMinReassigned()
        {
            //Arrange
            SetupAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, _responseMin, _responseMax);
            //Act
            _regressionAnalysisChart.PickBestAxisRange(_responseMin -1, _responseMax, _flawMin, _flawMax + 1, xAxis.Object, yAxis.Object);
            //Assert
            xAxis.VerifySet(x => x.Min = It.IsAny<double>(), Times.Never);
            xAxis.VerifySet(x => x.Max = 11.0);
            yAxis.VerifySet(y => y.Min = 19.0);
            yAxis.VerifySet(y => y.Max = It.IsAny<double>(), Times.Never);

        }
        [Test]
        public void PickBestAxisRange_OriginalMinLargerThanTransformedFlawMinAndOriginalMinLargerThanTransformResponseMin_FlawMinAndResponseMinReassigned()
        {
            //Arrange
            SetupAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, _responseMin, _responseMax);
            //Act
            _regressionAnalysisChart.PickBestAxisRange(_responseMin - 1, _responseMax, _flawMin - 1, _flawMax, xAxis.Object, yAxis.Object);
            //Assert
            xAxis.VerifySet(x => x.Min = 0.0);
            xAxis.VerifySet(x => x.Max = It.IsAny<double>(), Times.Never);
            yAxis.VerifySet(y => y.Min = 19.0);
            yAxis.VerifySet(y => y.Max = It.IsAny<double>(), Times.Never);

        }
        [Test]
        public void PickBestAxisRange_OriginalMinLargerThanTransformedFlawMinAndOriginalMaxSmallerThanTransformFlawMax_FlawMinAndMaxReassigned()
        {
            //Arrange
            SetupAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, _responseMin, _responseMax);
            //Act
            _regressionAnalysisChart.PickBestAxisRange(_responseMin, _responseMax, _flawMin - 1, _flawMax + 1, xAxis.Object, yAxis.Object);
            //Assert
            xAxis.VerifySet(x => x.Min = 0.0);
            xAxis.VerifySet(x => x.Max = 11.0);
            yAxis.VerifySet(y => y.Min = It.IsAny<double>(), Times.Never);
            yAxis.VerifySet(y => y.Max = It.IsAny<double>(), Times.Never);

        }
        [Test]
        public void PickBestAxisRange_OrigMaxSmallerThanTransformedFlawMaxAndOrigMinLargerThanTransformResponseMinAndOrigMaxSmallerThanTransformResponseMax_FlawMaxAndResponseMinMaxReassigned()
        {
            //Arrange
            SetupAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, _responseMin, _responseMax);
            //Act
            _regressionAnalysisChart.PickBestAxisRange(_responseMin - 1, _responseMax + 1, _flawMin, _flawMax + 1, xAxis.Object, yAxis.Object);
            //Assert
            xAxis.VerifySet(x => x.Min = It.IsAny<double>(), Times.Never);
            xAxis.VerifySet(x => x.Max = 11.0);
            yAxis.VerifySet(y => y.Min = 19.0);
            yAxis.VerifySet(y => y.Max = 31.0);

        }
        [Test]
        public void PickBestAxisRange_OrigMinLargerThanTransformedFlawMinAndOrigMinLargerThanTransformResponseMinAndOrigMaxSmallerThanTransformResponseMax_FlawMinAndResponseMinMaxReassigned()
        {
            //Arrange
            SetupAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, _responseMin, _responseMax);
            //Act
            _regressionAnalysisChart.PickBestAxisRange(_responseMin - 1, _responseMax + 1, _flawMin - 1, _flawMax, xAxis.Object, yAxis.Object);
            //Assert
            xAxis.VerifySet(x => x.Min = 0.0);
            xAxis.VerifySet(x => x.Max = It.IsAny<double>(), Times.Never);
            yAxis.VerifySet(y => y.Min = 19.0);
            yAxis.VerifySet(y => y.Max = 31.0);

        }
        [Test]
        public void PickBestAxisRange_OrigMinLargerThanTransformedFlawMinAndOrigMaxSmallerThanTransformFlawMaxAndOrigMaxSmallerThanTransformResponseMax_FlawMinMaxAndResponseMaxReassigned()
        {
            //Arrange
            SetupAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, _responseMin, _responseMax);
            //Act
            _regressionAnalysisChart.PickBestAxisRange(_responseMin, _responseMax + 1, _flawMin - 1, _flawMax + 1, xAxis.Object, yAxis.Object);
            //Assert
            xAxis.VerifySet(x => x.Min = 0.0);
            xAxis.VerifySet(x => x.Max = 11.0);
            yAxis.VerifySet(y => y.Min = It.IsAny<double>(), Times.Never);
            yAxis.VerifySet(y => y.Max = 31.0);

        }
        [Test]
        public void PickBestAxisRange_OrigMinLargerThanTransformedFlawMinAndOrigMaxSmallerThanTransformFlawMaxAndOrigMinLargerThanTransformResponseMin_FlawMinMaxAndResponseMinReassigned()
        {
            //Arrange
            SetupAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, _responseMin, _responseMax);
            //Act
            _regressionAnalysisChart.PickBestAxisRange(_responseMin - 1, _responseMax, _flawMin - 1, _flawMax + 1, xAxis.Object, yAxis.Object);
            //Assert
            xAxis.VerifySet(x => x.Min = 0.0);
            xAxis.VerifySet(x => x.Max = 11.0);
            yAxis.VerifySet(y => y.Min = 19.0);
            yAxis.VerifySet(y => y.Max = It.IsAny<double>(), Times.Never);

        }
        [Test]
        public void PickBestAxisRange_MaxAlwaysSmallerThanTransformMaxAndMinAlwaysLargerThanTransformMin_AllReassigned()
        {
            //Arrange
            SetupAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, _responseMin, _responseMax);
            //Act
            _regressionAnalysisChart.PickBestAxisRange(_responseMin - 1, _responseMax + 1, _flawMin - 1, _flawMax + 1, xAxis.Object, yAxis.Object);
            //Assert
            xAxis.VerifySet(x => x.Min = 0.0);
            xAxis.VerifySet(x => x.Max = 11.0);
            yAxis.VerifySet(y => y.Min = 19.0);
            yAxis.VerifySet(y => y.Max = 31.0);

        }
        //Now that we've tested the 4 if statements we will now test the other logic that's within PickBestAxisRange(double myTransformedResponseMin, 
        // double myTransformedResponseMax, double myTransformedFlawMin, double myTransformedFlawMax,
        // IAxisObject xaxisT = null, IAxisObject yAxisT = null)
        [Test]
        [TestCase(.1, 1.1)]
        [TestCase(-.1, 1.2)]
        public void PickBestAxisRange_EitherYAxisMaxEquals1Point1OrMinEqualsNegativePoint1IsNotTrue_ValuesNotOverwrittenTo0And1Step25(double responseMin, double responseMax)
        {
            //Arrange
            _regressionAnalysisChart.GetBufferedCalled = false;
            SetupAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, responseMin, responseMax);
            //Act
            _regressionAnalysisChart.PickBestAxisRange(responseMin, responseMax, _flawMin, _flawMax, xAxis.Object, yAxis.Object);
            //Assert
            yAxis.VerifySet(y => y.Max = 1.0, Times.Never);
            yAxis.VerifySet(y => y.Min = 0.0, Times.Never);
            yAxis.VerifySet(y => y.Interval = .25, Times.Never);
            // Make sure that the GetBuffered range is called with specifically the YAxis object (XAxis always get called)
            Assert.That(_regressionAnalysisChart.GetBufferedCalled, Is.True);
        }
        [Test]
        public void PickBestAxisRange_BothYAxisMaxEquals1Point1AndMinEqualsNegativePoint1_ValuesOverwrittenTo0And1Step25()
        {
            //Arrange
            _regressionAnalysisChart.GetBufferedCalled = false;
            SetupAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, -.1, 1.1);
            //Act
            _regressionAnalysisChart.PickBestAxisRange(-.1, 1.1, _flawMin, _flawMax, xAxis.Object, yAxis.Object);
            //Assert
            yAxis.VerifySet(y => y.Max = 1.0);
            yAxis.VerifySet(y => y.Min = 0.0);
            yAxis.VerifySet(y => y.Interval = .25);
            // Make sure that the GetBuffered range is NOT called with specifically the YAxis object (XAxis always get called)
            Assert.That(_regressionAnalysisChart.GetBufferedCalled, Is.False);
        }
        [Test]
        public void PickBestAxisRange_AnalysisDataTypeAHat_RelabelAxesBetterCalledWithLambdaValue()
        {
            //Arrange
            SetupAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, -.1, 1.1);
            _data.SetupGet(d => d.DataType).Returns(AnalysisDataTypeEnum.AHat);
            //Act
            _regressionAnalysisChart.PickBestAxisRange(_responseMin, _responseMax, _flawMin, _flawMax, xAxis.Object, yAxis.Object);
            //Assert
            _data.VerifyGet(d => d.LambdaValue);
        }
        [Test]
        public void PickBestAxisRange_AnalysisDataTypeHitMiss_RelabelAxesBetterCalledWithoutLambdaValue()
        {
            //Arrange
            SetupAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, -.1, 1.1);
            _data.SetupGet(d => d.DataType).Returns(AnalysisDataTypeEnum.HitMiss);
            //Act
            _regressionAnalysisChart.PickBestAxisRange(_responseMin, _responseMax, _flawMin, _flawMax, xAxis.Object, yAxis.Object);
            //Assert
            _data.VerifyGet(d => d.LambdaValue, Times.Never);
        }
        // Tests for void PickBestAxisRange(IAxisObject xaxisT = null, IAxisObject yAxisT = null) --- no arguements version
        [Test]
        public void PickBestAxisRangeNoArgs_AnalysisDataTypeAHat_RelabelAxesBetterCalledWithLambdaValue()
        {
            //Arrange
            SetupAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, -.1, 1.1);
            _data.SetupGet(d => d.DataType).Returns(AnalysisDataTypeEnum.AHat);
            //Act
            _regressionAnalysisChart.PickBestAxisRange(xAxis.Object, yAxis.Object);
            //Assert
            _data.VerifyGet(d => d.LambdaValue);
        }
        [Test]
        public void PickBestAxisRangeNoArgs_AnalysisDataTypeHitMissAndXAxisMaxNotLessThanMin_RelabelAxesBetterCalledWithoutLambdaValueAndAxisMaxMinNotReassigned()
        {
            //Arrange
            SetupAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, -.1, 1.1);
            _data.SetupGet(d => d.DataType).Returns(AnalysisDataTypeEnum.HitMiss);
            //Act
            _regressionAnalysisChart.PickBestAxisRange(xAxis.Object, yAxis.Object);
            //Assert
            _data.VerifyGet(d => d.LambdaValue, Times.Never);
            _data.VerifyGet(d => d.HMAnalysisObject, Times.Exactly(0));
        }
        [Test]
        public void PickBestAxisRangeNoArgs_AnalysisDataTypeHitMissAndXAxisMaxLessThanMin_RelabelAxesBetterCalledWithoutLambdaValueAndAxisMaxMinReassigned()
        {
            //Arrange
            SetupAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, -.1, 1.1);
            _data.Setup(d => d.DataType).Returns(AnalysisDataTypeEnum.HitMiss);
            _regressionAnalysisChart.ChartAreas[0].AxisX.Maximum = 10.0;
            _regressionAnalysisChart.ChartAreas[0].AxisX.Minimum = 11.0;
            List<double> sampleFlaws = new List<double>() { 1.0, 2.0, 3.0 };
            _data.SetupGet(d => d.HMAnalysisObject).Returns(new HMAnalysisObject("HitMissName") { Flaws_All = sampleFlaws });
            //Act
            _regressionAnalysisChart.PickBestAxisRange(xAxis.Object, yAxis.Object);
            //Assert
            _data.VerifyGet(d => d.LambdaValue, Times.Never);
            _data.VerifyGet(d => d.HMAnalysisObject, Times.AtLeastOnce);
            Assert.That(_regressionAnalysisChart.ChartAreas[0].AxisX.Maximum, Is.GreaterThan(_regressionAnalysisChart.ChartAreas[0].AxisX.Minimum));
        }
        private void SetupAxesObjectsMinMax(out Mock<IAxisObject> xAxis, double minValueX, double maxValueX, out Mock<IAxisObject> yAxis, double minValueY, double maxValueY)
        {
            xAxis = new Mock<IAxisObject>();
            xAxis.SetupGet(x => x.Min).Returns(minValueX);
            xAxis.SetupGet(x => x.Max).Returns(maxValueX);
            yAxis = new Mock<IAxisObject>();
            yAxis.SetupGet(y => y.Min).Returns(minValueY);
            yAxis.SetupGet(y => y.Max).Returns(maxValueY);
        }
        /// Tests for the UpdateBestFitLine() function
        /*
        [Test]
        public void UpdateBestFitLine_()
        {
            //Arrange
            Series bestFitLine = new Series() { Name = PODRegressionLabels.BestFitLine };
            SetupDataTableSample(new DataTable());            
            _regressionAnalysisChart.Series.Add(bestFitLine);
            _regressionAnalysisChart.Show();
            _data.SetupGet(d => d.DataType).Returns(AnalysisDataTypeEnum.HitMiss);
            //Act
            _regressionAnalysisChart.UpdateBestFitLine();
            //Assert
            Assert.That(_regressionAnalysisChart.Series[PODRegressionLabels.BestFitLine].Points.FirstOrDefault(p=>p.XValue==2 && p.YValues[0] == 4), Is.True);
        }
        private void SetupDataTableSample(DataTable sampleDT)
        {
            sampleDT.Columns.Add(new DataColumn("flaw"));
            sampleDT.Columns.Add(new DataColumn("transformFlaw"));
            sampleDT.Columns.Add(new DataColumn("fit"));
            sampleDT.Columns.Add(new DataColumn("t_fit"));
            sampleDT.Rows.Add(new double[] { 1, 2, 3, 4 });
            _data.SetupGet(d => d.ResidualUncensoredTable).Returns(sampleDT);
        }
        */

        ///
        /// Tests for the UpdateLevelConfidenceLines(double myA50, double myA90, double myA90_95, double myFitM, double myFitB) function
        /// This function is used for AHat
        [Test]
        [TestCase(0)]
        public void UpdateLevelConfidenceLines_A9095IsZero_9095PointsNotAdded(int expectedPoints)
        {
            //Arrange
            SetupSeriesUpdateLevelConfidence();
            double a9095 = 0.0;
            //Act
            _regressionAnalysisChart.UpdateLevelConfidenceLines(myA50: .5, myA90: .75, myA90_95: a9095, 1.0, 0.0);
            //Assert
            AssertPointsAddedToCount(expectedPoints);
        }
        [Test]
        [TestCase(2)]
        public void UpdateLevelConfidenceLinesAHat_A9095IsNotZero_9095PointsAdded(int expectedPoints)
        {
            //Arrange
            SetupSeriesUpdateLevelConfidence();
            double a9095 = .9;
            //Act
            _regressionAnalysisChart.UpdateLevelConfidenceLines(.5, .75, myA90_95: a9095, 1.0, 0.0);
            //Assert
            AssertPointsAddedToCount(expectedPoints);
            AssertContainsPoint(PODRegressionLabels.a9095Line, a9095, .1);
            AssertContainsPoint(PODRegressionLabels.a9095Line, a9095, .75);
        }
        /// <summary>
        /// tests for UpdateLevelConfidenceLines(double myA50, double myA90, double myA90_95)
        /// This function body is called for hitmiss
        /// </summary>
        [Test]
        public void UpdateLevelConfidenceLines_ValidDoubleValuesPassed_AllPointsAdded()
        {
            //Arrange
            SetupSeriesUpdateLevelConfidence();
            //Act
            _regressionAnalysisChart.UpdateLevelConfidenceLines(myA50: 5.0, myA90: 9.0, myA90_95: 9.50);
            //Assert
            AssertPointsAddedToCount(2);
            //A50
            AssertContainsPoint(PODRegressionLabels.a50Line, 5.0, .1);
            AssertContainsPoint(PODRegressionLabels.a50Line, 5.0, .5);
            //A90
            AssertContainsPoint(PODRegressionLabels.a90Line, 9.0, .1);
            AssertContainsPoint(PODRegressionLabels.a90Line, 9.0, .9);
            //A9095
            AssertContainsPoint(PODRegressionLabels.a9095Line, 9.50, .1);
            AssertContainsPoint(PODRegressionLabels.a9095Line, 9.50, .9);
        }

        private void SetupSeriesUpdateLevelConfidence()
        {
            _regressionAnalysisChart.Series.Add(new Series(PODRegressionLabels.a50Line));
            _regressionAnalysisChart.Series.Add(new Series(PODRegressionLabels.a90Line));
            _regressionAnalysisChart.Series.Add(new Series(PODRegressionLabels.a9095Line));
            _regressionAnalysisChart.ChartAreas[0].AxisX.Minimum = 1.0;
            _regressionAnalysisChart.ChartAreas[0].AxisY.Minimum = .1;
        }
        private void AssertPointsAddedToCount(int expectedPoints)
        {
            Assert.That(_regressionAnalysisChart.Series[PODRegressionLabels.a9095Line].Points.Count, Is.EqualTo(expectedPoints));
            //Ensure that other points are being added as well
            Assert.That(_regressionAnalysisChart.Series[PODRegressionLabels.a50Line].Points.Count, Is.EqualTo(2));
            Assert.That(_regressionAnalysisChart.Series[PODRegressionLabels.a90Line].Points.Count, Is.EqualTo(2));
        }
        private void AssertContainsPoint(string label, double expectedX, double expectedY)
        {
            Assert.IsTrue(_regressionAnalysisChart.Series[label].Points.Any(point =>
            point.XValue == expectedX && point.YValues[0] == expectedY));
        }

        /// Tests for ForceIncludedPointsUpdate() function
        [Test]
        public void ForceIncludedPointsUpdate_TurnedOffPointsExists_AllTurnedOffPointsBecomeGray()
        {
            AddPointsToSeries();
            _data.SetupGet(d => d.TurnedOffPoints).Returns(new List<DataPointIndex>() { new DataPointIndex(1, 1, "MyReasonForOmission")});
            //Act
            _regressionAnalysisChart.ForceIncludedPointsUpdate();
            //Assert
            //var point = Series[3].Points[1];
            Assert.That(_regressionAnalysisChart.Series[3].Points[1].Color, Is.EqualTo(Color.Gray));
        }
        private void AddPointsToSeries()
        {
            List<string> mySeries = new List<string>() { PODRegressionLabels.a50Line, 
                                                        PODRegressionLabels.a90Line,
                                                        PODRegressionLabels.a9095Line,
                                                        PODRegressionLabels.BestFitLine };
            foreach(string series in mySeries)
            {
                _regressionAnalysisChart.Series.Add(new Series(series));
                for (int i = 0; i < 5; i++)
                    _regressionAnalysisChart.Series[series].Points.Add(new DataPoint(1.0 + i, .1 + i));
            }
        }
        /// Tests for ForceResizeAnnotations() function
        [Test]
        public void ForceResizeAnnotations_EquationIsNull_EquationTextNotAssignedAndResizeNotCalled()
        {
            //Arrange
            _regressionAnalysisChart.Equation = null;
            //Act and Assert
            Assert.DoesNotThrow(() => _regressionAnalysisChart.ForceResizeAnnotations());
        }
        [Test]
        public void ForceResizeAnnotations_EquationIsNOTNull_EquationTextNotAssignedAndResizeNotCalled()
        {
            //Arrange
            Mock<TextAnnotation> equation = new Mock<TextAnnotation>();
            _regressionAnalysisChart.Equation = equation.Object;
            //Act
            _regressionAnalysisChart.ForceResizeAnnotations();
            //Assert
            equation.Verify(e => e.ResizeToContent());
        }
        ///Tests for ForceRefillSortList() function
        [Test]
        public void ForceRefillSortList_CalledFromMainChart_ExecutesForceRefillSortListInAnalysisData()
        {
            //Act
            _regressionAnalysisChart.ForceRefillSortList();
            //Assert
            _data.Verify(d => d.ForceRefillSortList());
        }
    }
    public class FakeRegressionAnalysisChart : RegressionAnalysisChart
    {
        private bool _getBufferedCalled = false;
        public bool GetBufferedCalled {
            set { _getBufferedCalled = value; }
            get { return _getBufferedCalled; }
        }
        public TextAnnotation Equation
        {
            set { _equation = value; }
        }
        public VerticalLineAnnotation AMaxLine
        {
            set { _aMaxLine = value; }
        }
        public VerticalLineAnnotation AMinLine
        {
            set { _aMinLine = value; }
        }
        public HorizontalLineAnnotation ThresholdLine
        {
            set { _thresholdLine = value; }
            get { return this._thresholdLine; }
        }
        public IAnalysisData Data
        {
            set { _analysisData = value; }
        }
        protected override void GetBufferedRangeWrapper(Control chart, IAxisObject myAxis, double myMin, double myMax, AxisKind kind)
        {
            if (kind == AxisKind.Y)
                _getBufferedCalled = true;
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
