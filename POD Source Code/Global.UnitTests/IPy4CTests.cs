//using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using NUnit.Framework;
using System.Collections.Generic;
using POD;
using CSharpBackendWithR;

namespace Global.UnitTests
{
    [TestFixture]
    public class IPy4CTests
    {
        private IPy4C sampleIPy4C;
        private string sampleHitMiss= "SampleHitMiss";
        private string sampleSignalResponse= "SampleSignalResponse";
        [SetUp]
        public void Setup()
        {
            sampleIPy4C = new IPy4C();

            //add sample analysis to both dictionaries
            sampleIPy4C.HitMissAnalsysis(sampleHitMiss);
            sampleIPy4C.AHatAnalysis(sampleSignalResponse);
        }
        /// <summary>
        /// Tests for NotifyFinishAnalysis() function
        /// </summary> 
        [Test]
        public void NotifyFinishAnalysis_OnAnalysisFinishIsNull_NoInvocationThrown()
        {
            //Arrange
            sampleIPy4C.CurrentAnalysisName = "myAnalysis";
            POD.ErrorArgs errorargs = null;
            sampleIPy4C.OnAnalysisFinish += (sender, args) => { errorargs = args; };
            sampleIPy4C.OnAnalysisFinish = null;
            //Act
            sampleIPy4C.NotifyFinishAnalysis();
            //Assert
            Assert.That(errorargs, Is.EqualTo(null));
        }
        [Test]
        public void NotifyFinishAnalysis_OnAnalysisFinishNotNull_InvocationThrown()
        {
            //Arrange
            sampleIPy4C.CurrentAnalysisName = "myAnalysis";
            POD.ErrorArgs errorargs = null;
            sampleIPy4C.OnAnalysisFinish += (sender, args) => { errorargs = args; };
            //Act
            sampleIPy4C.NotifyFinishAnalysis();
            //Assert
            Assert.That(errorargs, Is.Not.EqualTo(null));
        }
        /// <summary>
        /// Tests for AddErrorText(string myError) function
        /// </summary> 
        [Test]
        public void AddErrorText_OnAnalysisErrorNull_NoInvocationThrown()
        {
            //Arrange
            sampleIPy4C.CurrentAnalysisName = "myAnalysis";
            POD.ErrorArgs errorargs = null;
            sampleIPy4C.OnAnalysisError += (sender, args) => { errorargs = args; };
            sampleIPy4C.OnAnalysisError = null;
            //Act
            sampleIPy4C.AddErrorText("myError");
            //Assert
            Assert.That(errorargs, Is.EqualTo(null));
        }
        [Test]
        public void AddErrorText_OnAnalysisErrorNotNull_InvocationThrown()
        {
            //Arrange
            sampleIPy4C.CurrentAnalysisName = "myAnalysis";
            POD.ErrorArgs errorargs = null;
            sampleIPy4C.OnAnalysisError += (sender, args) => { errorargs = args; };
            //Act
            sampleIPy4C.AddErrorText("myError");
            //Assert
            Assert.That(errorargs, Is.Not.EqualTo(null));
        }
        /// <summary>
        /// Tests for HitMissAnalysis() function
        /// </summary>
        [Test]
        public void HitMissAnalysis_DictionaryDoesNotcontainKey_DictionaryCountPlus1()
        {
            //Arrange
            var analysisName = "AnotherHitMissSample";
            //Act
            var result = sampleIPy4C.HitMissAnalsysis(analysisName);
            //Assert
            Assert.That(result is HMAnalysisObject, Is.True);
            //dictionary originally had 1 in the setup function
            Assert.That(sampleIPy4C.DictionarySizeHitMiss, Is.EqualTo(2));
        }
        [Test]
        public void HitMissAnalysis_DictionaryContainsKey_DictionaryRemainsTheSame()
        {
            //Arrange
            var analysisName = sampleHitMiss;
            //Act
            var result = sampleIPy4C.HitMissAnalsysis(analysisName);
            //Assert
            Assert.That(result is HMAnalysisObject, Is.True);
            //dictionary originally had 1 in the setup function
            Assert.That(sampleIPy4C.DictionarySizeHitMiss, Is.EqualTo(1));
        }
        /// <summary>
        /// Tests for AHatAnalysis() function
        /// </summary>
        [Test]
        public void AHatAnalysis_DictionaryDoesNotcontainKey_DictionaryCountPlus1()
        {
            //Arrange
            var analysisName = "AnotherAHatSample";
            //Act
            var result = sampleIPy4C.AHatAnalysis(analysisName);
            //Assert
            Assert.That(result is AHatAnalysisObject, Is.True);
            //dictionary originally had 1 in the setup function
            Assert.That(sampleIPy4C.DictionarySizeSignalResponse, Is.EqualTo(2));
        }
        [Test]
        public void AHatAnalysis_DictionaryContainsKey_DictionaryRemainsTheSame()
        {
            //Arrange
            var analysisName = sampleSignalResponse;
            //Act
            var result = sampleIPy4C.AHatAnalysis(analysisName);
            //Assert
            Assert.That(result is AHatAnalysisObject, Is.True);
            //dictionary originally had 1 in the setup function
            Assert.That(sampleIPy4C.DictionarySizeSignalResponse, Is.EqualTo(1));
        }
        /// <summary>
        /// Tests for GetMaxPrecision() function
        /// </summary>
        [Test]
        [TestCase (TransformTypeEnum.Linear)]
        [TestCase(TransformTypeEnum.Log)]
        [TestCase(TransformTypeEnum.Inverse)]
        [TestCase(TransformTypeEnum.Exponetial)]
        [TestCase(TransformTypeEnum.BoxCox)]
        [TestCase(TransformTypeEnum.None)]
        public void TransformEnumToInt_ValidTransform_ReturnsAnInteger(TransformTypeEnum transformTypeEnum)
        {
            //Arrange
            //Act
            var result = sampleIPy4C.TransformEnumToInt(transformTypeEnum);
            //Assert
            Assert.That(result, Is.Not.EqualTo(0));
        }
        [Test]
        public void TransformEnumToInt_ValidTransform_Default()
        {
            //Arrange
            TransformTypeEnum transformTypeEnum = TransformTypeEnum.None;
            //Act
            var result = sampleIPy4C.TransformEnumToInt(transformTypeEnum);
            //Assert
            Assert.That(result, Is.EqualTo(1));
        }
        [Test]
        public void TransformEnumToInt_LinearTransform_Returns1()
        {
            //Arrange
            TransformTypeEnum transformTypeEnum = TransformTypeEnum.Linear;
            //Act
            var result = sampleIPy4C.TransformEnumToInt(transformTypeEnum);
            //Assert
            Assert.That(result, Is.EqualTo(1));
        }
        [Test]
        public void TransformEnumToInt_LogTransform_Returns2()
        {
            //Arrange
            TransformTypeEnum transformTypeEnum = TransformTypeEnum.Log;
            //Act
            var result = sampleIPy4C.TransformEnumToInt(transformTypeEnum);
            //Assert
            Assert.That(result, Is.EqualTo(2));
        }
        [Test]
        public void TransformEnumToInt_InverseTransform_Returns3()
        {
            //Arrange
            TransformTypeEnum transformTypeEnum = TransformTypeEnum.Inverse;
            //Act
            var result = sampleIPy4C.TransformEnumToInt(transformTypeEnum);
            //Assert
            Assert.That(result, Is.EqualTo(3));
        }
        [Test]
        public void TransformEnumToInt_Exponential_Returns4()
        {
            //Arrange
            TransformTypeEnum transformTypeEnum = TransformTypeEnum.Exponetial;
            //Act
            var result = sampleIPy4C.TransformEnumToInt(transformTypeEnum);
            //Assert
            Assert.That(result, Is.EqualTo(4));
        }
        [Test]
        public void TransformEnumToInt_BoxcoxTransform_Returns5()
        {
            //Arrange
            TransformTypeEnum transformTypeEnum = TransformTypeEnum.BoxCox;
            //Act
            var result = sampleIPy4C.TransformEnumToInt(transformTypeEnum);
            //Assert
            Assert.That(result, Is.EqualTo(5));
        }

        /// <summary>
        /// Tests for GetPValueDecision() function
        /// </summary>  
        [Test]
        [TestCase(0.0)]
        [TestCase(.005)]
        [TestCase(.01)]
        [TestCase(.025)]
        [TestCase(.05)]
        [TestCase(.1)]
        [TestCase(.2)]
        public void GetPValueDecision_myValueIsValid_ReturnsDecisionStringNotEqualToUndefined(double value)
        {
            //Arrange

            //Act
            var result = sampleIPy4C.GetPValueDecision(value);
            //Assert
            Assert.That(result, Is.Not.EqualTo(""));
            Assert.That(result, Is.Not.EqualTo("Undefined").IgnoreCase);
        }
        [Test]
        [TestCase(-.1)]
        public void GetPValueDecision_myValueIsNotValid_ReturnsUndefined(double value)
        {
            //Arrange

            //Act
            var result = sampleIPy4C.GetPValueDecision(value);
            //Assert
            Assert.That(result, Is.EqualTo("Undefined").IgnoreCase);
        }
        [Test]
        [TestCase(0.0, "P <= .005")]
        [TestCase(.005, "P <= .005")]
        [TestCase(.006, ".005 < P <= 0.01")]
        [TestCase(.01, ".005 < P <= 0.01")]
        [TestCase(.02, "0.01 < P <= .025")]
        [TestCase(.025, "0.01 < P <= .025")]
        [TestCase(.026, ".025 < P <= 0.05")]
        [TestCase(.05, ".025 < P <= 0.05")]
        [TestCase(.06, "0.05 < P <= 0.1")]
        [TestCase(.1, "0.05 < P <= 0.1")]
        [TestCase (.2, "P > 0.1")]

        public void GetPValueDecision_PIsACertainRange_ReturnsCorrectPValueRangeAsString(double value, string expectedString)
        {
            //Act
            var result = sampleIPy4C.GetPValueDecision(value);
            //Assert
            Assert.That(result, Is.EqualTo(expectedString).IgnoreCase);
        }
        /// <summary>
        /// Tests for NthRoot(double A, double root, double checkLambdaDenominator=2.0) function
        /// </summary> 
        [Test]
        public void NthRoot_AIsPositiveAndRootIsPositive_ReturnsAPositiveRealNumber()
        {
            //Arrange
            var a = 16.0;
            var root = 2.0;
            //Act
            var result = IPy4C.NthRoot(a, root);
            //Assert
            Assert.That(result, Is.TypeOf<double>());
            Assert.That(result, Is.Not.EqualTo(Double.NaN));
            Assert.That(result, Is.EqualTo(4));
        }
        [Test]
        public void NthRoot_AIsNegativeButCheckLambdaDenominatorNotAddedAndRootIsEven_ReturnsDoubleNaN()
        {
            //Arrange
            var a = -16.0;
            var root = 2.0;
            //Act
            var result = IPy4C.NthRoot(a, root);
            //Assert
            Assert.That(result, Is.EqualTo(Double.NaN));
        }
        [Test]
        public void NthRoot_AIsNegativeButODDCheckLambdaDenominatorAddedAndRootIsEven_ReturnsANegativeRealNumber()
        {
            //Arrange
            var a = -16.0;
            var root = 2.0;
            //Act
            var result = IPy4C.NthRoot(a, root, 3.0);
            //Assert
            Assert.That(result, Is.Not.EqualTo(Double.NaN));
            Assert.That(result, Is.EqualTo(-4.0));
        }
        /// <summary>
        /// Tests for DecimalToFraction(double number, out long numerator, out long denominator) function
        /// Note: a mixed number is never passed into this function
        /// </summary> 
        [Test]
        public void DecimalToFraction_DecimalIsBetween0And1_ReturnsAProperFraction()
        {
            //Arrange
            long numerator;
            long denominator;
            double exampleNum = .5;

            IPy4C.DecimalToFraction(exampleNum, out numerator, out denominator);

            Assert.That(numerator, Is.EqualTo(1));
            Assert.That(denominator, Is.EqualTo(2));

        }
        [Test]
        public void DecimalToFraction_DecimalIsZero_Returns0Over1()
        {
            //Arrange
            long numerator;
            long denominator;
            double exampleNum = 0.0;

            IPy4C.DecimalToFraction(exampleNum, out numerator, out denominator);

            Assert.That(numerator, Is.EqualTo(0));
            Assert.That(denominator, Is.EqualTo(1));

        }
        [Test]
        [TestCase(-1.5)]
        [TestCase(-.5)]
        [TestCase(1.5)]
        public void DecimalToFraction_DecimalIsANegativeNumber_ReturnsAnImproperFraction(double testNum)
        {
            //Arrange
            long numerator;
            long denominator;
            double exampleNum = testNum;

            Assert.That(() => IPy4C.DecimalToFraction(exampleNum, out numerator, out denominator), Throws.TypeOf<Exception>());

        }
        /// <summary>
        /// Tests for GetMaxPrecision() function
        /// </summary> 
        [Test]
        public void GetMaxPrecision_ListIsEmpty_ReturnsPrecisionOf0()
        {
            //Arrange
            List<double> podItems = new List<double>();
            //Act
            int result = IPy4C.GetMaxPrecision(podItems);

            //Assert
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        [TestCase(1.0)]
        [TestCase(-1.0)]
        [TestCase(0.0)]
        public void GetMaxPrecision_ListHoldsAnInteger_ReturnsPrecision0(double testItem)
        {
            //Arrange
            List<double> podItems = new List<double>() { testItem };
            //Act
            int result = IPy4C.GetMaxPrecision(podItems);

            //Assert
            Assert.That(result, Is.EqualTo(0));
        }

       
        [Test]
        [TestCase(1.1)]
        [TestCase(.1)]
        [TestCase(-.1)]
        [TestCase(-1.1)]
        public void GetMaxPrecision_ListContainsADecimal_ReturnsPrecisionOf1(double testItem)
        {
            //Arrange
            List<double> podItems = new List<double>() { testItem };
            //Act
            int result = IPy4C.GetMaxPrecision(podItems);

            //Assert
            Assert.That(result, Is.EqualTo(1));
        }
        [Test]
        [TestCase(new double[] { .1, .12, .123 })]
        [TestCase(new double[] { .123, .12, .1 })]
        [TestCase(new double[] { .1, .123, .12 })]
        [TestCase(new double[] { -.1, .12, .123 })]
        [TestCase(new double[] { .123, -.12, .1 })]
        [TestCase(new double[] { .1, .123, -.12 })]
        [TestCase(new double[] { 1.1, 1.12, 1.123 })]
        [TestCase(new double[] { 1.1, -1.12, -1.123 })]
        public void GetMaxPrecision_ListContainsVaryingLenthDecimals_ReturnsAPrecisionOf3(double[] myArray)
        {
            //Arrange
            List<double> podItems = new List<double>();
            podItems.Add(myArray[0]);
            podItems.Add(myArray[1]);
            podItems.Add(myArray[2]);
            //Act
            int result = IPy4C.GetMaxPrecision(podItems);

            //Assert
            Assert.That(result, Is.EqualTo(3));
        }
        /// <summary>
        /// Unit tests for GetMaxPrecisionDict. The responses are a dictionary type
        /// in signal response, so this meethod is used to deal with multiple responses
        /// </summary>
        [Test]
        public void GetMaxPrecisionDict_OneDictionaryItem_Returns3()
        {
            //Arrange
            Dictionary<string, List<double>> podDict = new Dictionary<string, List<double>>();
            podDict.Add("test1,", new List<double> { .123, .12, .1 });

            //Act
            int result = IPy4C.GetMaxPrecisionDict(podDict);

            //Arrange
            Assert.That(result, Is.EqualTo(3));

        }
        [Test]
        public void GetMaxPrecisionDict_TwoDictionaryItems_Returns3()
        {
            //Arrange
            Dictionary<string, List<double>> podDict = new Dictionary<string, List<double>>();
            podDict.Add("test1,", new List<double> { .123, .12, .1 });
            podDict.Add("test2,", new List<double> { .1, .2, .3 });
            //Act
            int result = IPy4C.GetMaxPrecisionDict(podDict);

            //Arrange
            Assert.That(result, Is.EqualTo(3));

        }
    }
}
