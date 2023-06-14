using NUnit.Framework;
using System;
using Moq;
using POD.Data;
using CSharpBackendWithR;
namespace Data.UnitTests
{
    [TestFixture]
    public class TransformBackLambdaControlTest
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
            _ahatAnalysisObject.Lambda = -1;
            _ahatAnalysisObject.Signalmax = 10;
            _transformBackLambda = new TransformBackLambdaControl(_ahatAnalysisObject);
            //Act
            var result=_transformBackLambda.TransformBackLambda(input);
            //Assert
            Assert.That(result, Is.EqualTo(100));
        }
    }
}
