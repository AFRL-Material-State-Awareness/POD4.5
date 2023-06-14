using NUnit.Framework;
using System;
using Moq;
using POD.Data;
using CSharpBackendWithR;
namespace Data.UnitTests
{
    [TestFixture]
    public class TransformBackLambdaControlTests
    {
        private TransformBackLambdaControl _transformBackLambda;
        private AHatAnalysisObject _ahatAnalysisObject;
        [SetUp]
        public void Setup()
        {
            
            _ahatAnalysisObject = new AHatAnalysisObject("MyAnalysisAHat");
        }
        /// Tests for TransfomBackLambda (double value) function
        [Test]
        [TestCase(1.0)]
        [TestCase(2.0)]
        public void TransformBackLambda_ValueIsGreaterThanOrEqualToNegative1OverLambdaAndLambdaIsNegative_ReturnsSignalMaxTimes10(double input)
        {
            //Arrange 
            SetupAHatMetrics(-1);
            //Act
            var result = _transformBackLambda.TransformBackLambda(input);
            //Assert
            Assert.That(result, Is.EqualTo(100));
        }
        [Test]
        public void TransformBackLambda_MyValueIsLessThanNegative1OverLambdaAndLambda_ReturnsValidTransformBack()
        {
            //Arrange
            SetupAHatMetrics(2.0);
            //Act
            var result = _transformBackLambda.TransformBackLambda(1.5);
            //Assert
            Assert.That(result, Is.EqualTo(2));
        }
        [Test]
        [TestCase(1.5, 2.0)]
        [TestCase(-5, -3)]
        public void TransformBackLambda_LambdaIsGreaterThanZero_ReturnsValidTransformBack(double input, double expectedTransformBack)
        {
            //Arrange
            SetupAHatMetrics(2.0);
            //Act
            var result = _transformBackLambda.TransformBackLambda(input);
            //Assert
            Assert.That(result, Is.EqualTo(expectedTransformBack));
        }
        [Test]
        public void TransformBackLambda_LambdaIsLessThanZeroAndReturnsValidValue_ReturnsValidTransformBack()
        {
            //Arrange
            SetupAHatMetrics(3.0);
            //Act
            var result = _transformBackLambda.TransformBackLambda(-3.0);
            //Assert
            Assert.That(result, Is.EqualTo(-2.0));
        }
        [Test]
        public void TransformBackLambda_LambdaIsLessThanZeroAndReturnsNaNValue_ReturnsAdjustedTransformBack()
        {
            //Arrange
            SetupAHatMetrics(2.5);
            //Act
            var result = _transformBackLambda.TransformBackLambda(-2.0);
            //Assert
            Assert.That(result, Is.Not.EqualTo(double.NaN));
        }
        private void SetupAHatMetrics(double lambdaValue)
        {
            _ahatAnalysisObject.Lambda = lambdaValue;
            _ahatAnalysisObject.Signalmax = 10;
            _transformBackLambda = new TransformBackLambdaControl(_ahatAnalysisObject);
        }
    }
}
