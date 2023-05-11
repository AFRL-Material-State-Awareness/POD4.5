﻿using System;
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
using System.Windows.Forms.DataVisualization.Charting;
using POD.Data;
using System.Data;

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
            SetupXAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, _responseMin, _responseMax);
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
            SetupXAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, _responseMin, _responseMax);
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
            SetupXAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, _responseMin, _responseMax);
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
            SetupXAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, _responseMin, _responseMax);
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
            SetupXAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, _responseMin, _responseMax);
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
            SetupXAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, _responseMin, _responseMax);
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
            SetupXAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, _responseMin, _responseMax);
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
            SetupXAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, _responseMin, _responseMax);
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
            SetupXAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, _responseMin, _responseMax);
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
            SetupXAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, _responseMin, _responseMax);
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
            SetupXAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, _responseMin, _responseMax);
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
            SetupXAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, _responseMin, _responseMax);
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
            SetupXAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, _responseMin, _responseMax);
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
            SetupXAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, _responseMin, _responseMax);
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
            SetupXAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, _responseMin, _responseMax);
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
            SetupXAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, _responseMin, _responseMax);
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
            SetupXAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, responseMin, responseMax);
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
            SetupXAxesObjectsMinMax(out Mock<IAxisObject> xAxis, _flawMin, _flawMax, out Mock<IAxisObject> yAxis, -.1, 1.1);
            //Act
            _regressionAnalysisChart.PickBestAxisRange(-.1, 1.1, _flawMin, _flawMax, xAxis.Object, yAxis.Object);
            //Assert
            yAxis.VerifySet(y => y.Max = 1.0);
            yAxis.VerifySet(y => y.Min = 0.0);
            yAxis.VerifySet(y => y.Interval = .25);
            // Make sure that the GetBuffered range is NOT called with specifically the YAxis object (XAxis always get called)
            Assert.That(_regressionAnalysisChart.GetBufferedCalled, Is.False);
        }
        private void SetupXAxesObjectsMinMax(out Mock<IAxisObject> xAxis, double minValueX, double maxValueX, out Mock<IAxisObject> yAxis, double minValueY, double maxValueY)
        {
            xAxis = new Mock<IAxisObject>();
            xAxis.SetupGet(x => x.Min).Returns(minValueX);
            xAxis.SetupGet(x => x.Max).Returns(maxValueX);
            yAxis = new Mock<IAxisObject>();
            yAxis.SetupGet(y => y.Min).Returns(minValueY);
            yAxis.SetupGet(y => y.Max).Returns(maxValueY);
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