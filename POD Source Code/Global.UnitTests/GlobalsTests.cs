using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using POD;
using CSharpBackendWithR;
using System.Windows.Forms;
using Moq;
using System.Text.RegularExpressions;
using System.Drawing;

namespace Global.UnitTests
{
    [TestFixture]
    public class GlobalsTests
    {
        //private Mock<IContainerControl> _mockControl;
        private Control _control;
        [SetUp]
        public void Setup()
        {
            _control = new Control();
        }
        /// <summary>
        /// Tests for GetLabelIntervalBasedOnChartSize(Control chart, AxisKind kind) function
        /// </summary>
        /// <param name="axisKind"></param>
        [Test]
        [TestCase (AxisKind.X)]
        [TestCase(AxisKind.Y)]
        public void GetLabelIntervalBasedOnChartSize_ChartIsNull_ReturnsDefaultLabelCount(AxisKind axisKind)
        {
            var result=Globals.GetLabelIntervalBasedOnChartSize(null, axisKind);

            Assert.That(result, Is.EqualTo(Globals.DefaultLabelCount));
        }
        [Test]
        [TestCase(AxisKind.X)]
        [TestCase(AxisKind.Y)]
        public void GetLabelIntervalBasedOnChartSize_ChartIsNullAndAxisKindIsX_ReturnsInt32(AxisKind axisKind)
        {
            _control.Width = 500;
            _control.Height = 500;
            var result = Globals.GetLabelIntervalBasedOnChartSize(_control, axisKind);
            
            Assert.That(result, Is.GreaterThanOrEqualTo(1));
            Assert.That(result, Is.Not.EqualTo(10));
        }
        /// <summary>
        /// tests for StdWidth(Control control) function
        /// </summary>
        [Test]
        public void StdWidth_ControlIsNull_Returns70()
        {
            //Act
            var result = Globals.StdWidth(null);

            Assert.That(result, Is.EqualTo(70));

        }
        [Test]
        public void StdWidth_ControlIsNotNull_Returns70TimesDpiXOver96()
        {
            var testScale = _control.CreateGraphics();
            var scale = testScale.DpiX / 96.0;
            testScale.Dispose();
            //Act
            var result = Globals.StdWidth(_control);
            //Assert
            Assert.That(result, Is.EqualTo(70* scale));
        }
        /// <summary>
        /// tests for StdHeight(Control control) function
        /// </summary>
        [Test]
        public void Stdheight_ControlIsNotNull_ReturnsAnInt32()
        {
            //act
            var result = Globals.StdHeight(_control);
            //Assert
            Assert.That(result, Is.TypeOf<Int32>());
        }
        /// <summary>
        /// tests for CleanColumn(string myName) function
        /// </summary>
        [Test]
        [TestCase("ThisStringContainsNoInvalidCharacters")]
        [TestCase("This String Contains No Invalid Characters")]
        public void CleanColumnName_StringContainsNoInvalidCharacters_ReturnsTheSameColumnName(string testString)
        {
            var result = Globals.CleanColumnName(testString);

            Assert.That(result, Is.EqualTo(testString));
        }

        [Test]
        [TestCase("<This!String@Contains#No$Invalid%Characters>")]
        public void CleanColumnName_StringHasInvalidCharactersButDoesntTimeOut_ReturnsTheColumnNameWithoutIllegalsChars(string testString)
        {
            var result = Globals.CleanColumnName(testString);

            Assert.That(result, Is.EqualTo("ThisStringContainsNoInvalidCharacters"));
        }
        /*
        [Test]
        public void CleanColumnName_StringHasInvalidCharactersBuTimesOut_ReturnsAnEmptyString()
        {
            //Arrange
            string largeString = GenerateLargeRandomString(50000000);
            //Act
            var result = Globals.CleanColumnName(largeString);
            //Assert
            Assert.That(result, Is.EqualTo(String.Empty));
            Assert.Throws<RegexMatchTimeoutException>(() => Globals.CleanColumnName(largeString));

        }
        private string GenerateLargeRandomString(int stringLength)
        {
            Random rd = new Random();
            //const string allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@$?_-";
            const string allowedChars = @"!@$~()#\\/=><+/*%&|^'""[\]";
            char[] chars = new char[stringLength];

            for (int i = 0; i < stringLength; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }


            return new string(chars);
        }
        */
        ///<summary>
        /// tests for CreateCursorNoResize(Bitmap bmp, int xHotSpot, int yHotSpot)
        ///</summary>
        [Test]
        public void CreateCursorNoResize_ValidBitmapPassed_ReturnsACustomCursor()
        {
            //Arrange
            Cursor resultCursor = null;
            //Act
            using(Bitmap bmp=new Bitmap(10,10))
            {
                resultCursor = Globals.CreateCursorNoResize(bmp, 1, 1);
            }
            Assert.That(resultCursor, Is.Not.EqualTo(Cursors.Default));
        }
        [Test]
        public void CreateCursorNoResize_NullBitmapPassed_ReturnsDefaultCursor()
        {
            //Arrange
            Cursor resultCursor = null;
            
            //Act
            resultCursor = Globals.CreateCursorNoResize(null, 1, 1);;
            //Assert
            Assert.That(resultCursor, Is.EqualTo(Cursors.Default));
        }




    }
}
