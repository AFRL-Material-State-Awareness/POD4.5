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
        [SetUp]
        public void Setup()
        {
            _regressionAnalysisChart = new FakeRegressionAnalysisChart();
            SetUpAnalysisDataObject();
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
    }
    public class FakeRegressionAnalysisChart : RegressionAnalysisChart
    {
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
        private double GetInitialAMaxLocationFromData()
        {
            return 0.0;
        }
    }
}
