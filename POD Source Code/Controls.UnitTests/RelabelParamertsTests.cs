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
using POD.Data;
using System.Drawing;

namespace Controls.UnitTests
{
    [TestFixture]
    public class RelabelParamertsTests
    {
        private AxisObject _testAxisObject;
        private Mock<IAnalysisData> _data;
        [SetUp]
        public void Setup()
        {
            _testAxisObject = new AxisObject()
            {
                Max = 10.0,
                Min = 1.0,
                IntervalOffset = .1,
                Interval = .5,
                BufferPercentage = 15.0
            };
            _data = new Mock<IAnalysisData>();
        }
        /// <summary>
        /// Tests for the RelabelParameters() constructor
        /// </summary>
        [Test]
        public void RelabelParameters_AxisIsNotNull_AxisPropertyIsCloneOfOriginalAxis()
        {
            //Act
            RelabelParameters relabelParameters = new RelabelParameters(_testAxisObject, _data.Object.InvertTransformedFlaw, 10, false, TransformTypeEnum.Linear, _data.Object.TransformValueForXAxis);
            //Assert
            AreEqualByJson(_testAxisObject, relabelParameters.Axis);
        }
        [Test]
        public void RelabelParameters_AxisIsNull_AxisPropertyIsAlsoNull()
        {
            //Act
            RelabelParameters relabelParameters = new RelabelParameters(null, _data.Object.InvertTransformedFlaw, 10, false, TransformTypeEnum.Linear, _data.Object.TransformValueForXAxis);
            //Assert
            Assert.That(relabelParameters.Axis, Is.Null);
        }
        // This function checks for equality between two Axes objects through the use of Javascript json serialization
        private static void AreEqualByJson(object expected, object actual)
        {
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var expectedJson = serializer.Serialize(expected);
            var actualJson = serializer.Serialize(actual);
            Assert.AreEqual(expectedJson, actualJson);
        }
    }
}
