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
using System.Data;

namespace Global.UnitTests
{
    [TestFixture]
    public class GlobalsTests
    {
        //private Mock<IContainerControl> _mockControl;
        private Control _control;
        private string latinString;
        private string latinStringWLineBreak;
        // used for the Compute Class
        private DataTable _sampleTable;
        [SetUp]
        public void Setup()
        {
            _control = new Control();

            latinString = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod" +
                         "tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam," +
                          "quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo" +
                          "consequat.Duis aute irure dolor in reprehenderit in voluptate velit esse" +
                          "cillum dolore eu fugiat nulla pariatur.Excepteur sint occaecat cupidatat non" +
                          "proident, sunt in culpa qui";
            latinStringWLineBreak = latinString + '\n';

            SetUpDataTable();
            SetUpExtColProperty();
        }
        private void SetUpDataTable()
        {
            _sampleTable = new DataTable();
            _sampleTable.Columns.Add("Column_1", typeof(double));
            _sampleTable.Columns.Add("Column_2", typeof(double));
            _sampleTable.Columns.Add("BadColumn", typeof(string));
            var column1 = new List<double> { 1.0, 3.0, 5.0, 4.0, 2.0 };
            var column2 = new List<double> { 3.0, 6.0, 9.0, 12.0, 10.0 };
            var badColumn = new List<string> { "a", "b", "c", "d", "e" };
            for (int i = 0; i < column1.Count; i++)
                _sampleTable.Rows.Add(column1[i], column2[i], badColumn[i]);

        }
        private void SetUpExtColProperty()
        {
            /// these are used to test the methods in the ExtColPropertyClass
            ExtColProperty.UnitDefault = "myDefaultUnit";
            ExtColProperty.MaxDefault = 10.0;
            ExtColProperty.MinDefault = 1.0;
            ExtColProperty.ThreshDefault = 5.0;
        }
        /// <summary>
        /// Tests for GetLabelIntervalBasedOnChartSize(Control chart, AxisKind kind) function
        /// </summary>
        /// <param name="axisKind"></param>
        [Test]
        [TestCase(AxisKind.X)]
        [TestCase(AxisKind.Y)]
        public void GetLabelIntervalBasedOnChartSize_ChartIsNull_ReturnsDefaultLabelCount(AxisKind axisKind)
        {
            var result = Globals.GetLabelIntervalBasedOnChartSize(null, axisKind);

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
            Assert.That(result, Is.EqualTo(70 * scale));
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
            using (Bitmap bmp = new Bitmap(10, 10))
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
            Mock<IMessageBoxWrap> messageBox = new Mock<IMessageBoxWrap>();
            Globals.MessageBoxWrapper = messageBox.Object;
            //Act
            resultCursor = Globals.CreateCursorNoResize(null, 1, 1);
            //Assert
            messageBox.Verify(mb => mb.Show(It.IsAny<string>()));
            Assert.That(resultCursor, Is.EqualTo(Cursors.Default));
        }
        ///<summary>
        /// DeleteMRUList(string value, string fileName) is too difficult to unit test
        ///</summary>

        ///<summary>
        /// UpdateMRUListMultiLine(string value, string fileName, int maxLines = 8) is too difficult to unit test
        ///</summary>

        ///<summary>
        ///  UpdateMRUList(string value, string fileName, bool useSplitted = false, string splitCharacter = "|", int maxLines = 8) is too difficult to unit test
        ///</summary>

        ///<summary>
        /// GetMRUListMultiLine(string fileName) is too difficult to unit test
        ///</summary>

        ///<summary>
        /// GetMRUList(string fileName) is too difficult to unit test
        ///</summary>

        ///<summary>
        /// GetMRUListWithoutEmpties(string fileName, bool useSplitted = false, string splitCharacter = "|") is too difficult to unit test
        ///</summary>

        ///<summary>
        /// CleanUpRandomImageFiles() is too difficult to unit test
        ///</summary>

        ///<summary>
        /// Tests for SplitIntoLines(string p) function
        ///</summary>
        [Test]
        public void SplitIntoLines_StringWithNoSpaces_ReturnsTheSameString()
        {
            var myString = "ThisIsAStringWithNoSpaces";

            string result = Globals.SplitIntoLines(myString);
            //Assert
            Assert.That(result, Is.EqualTo(myString));
        }
        [Test]
        public void SplitIntoLines_StringWithSpaces_ReturnsTheSameString()
        {
            var myString = "This Is A String With Spaces";

            string result = Globals.SplitIntoLines(myString);
            //Assert
            Assert.That(result, Is.EqualTo(myString));
        }
        [Test]
        public void SplitIntoLines_EmptyString_ReturnsEmptyString()
        {
            //arrange
            var myString = "";
            //Act
            string result = Globals.SplitIntoLines(myString);
            //Assert
            Assert.That(result, Is.EqualTo(String.Empty));
        }
        [Test]
        public void SplitIntoLines_StringHasWordsAndLongerThan40Chars_ReturnsTheStringWithLineBreaksWithoutEndingWithALineBreak()
        {
            //arrange
            var myString = latinString;
            //Act
            string result = Globals.SplitIntoLines(myString);
            //Assert
            AssertMultiLineString(result, myString);
        }
        [Test]
        public void SplitIntoLines_StringHasWordsAndLongerThan40CharsAndEndsInLineBreak_ReturnsTheStringWithLineBreaksWithoutEndingWithALineBreak()
        {
            //arrange
            var myString = latinStringWLineBreak;
            //Act
            string result = Globals.SplitIntoLines(myString);
            //Assert
            AssertMultiLineString(result, myString);
        }
        private void AssertMultiLineString(string result, string origString)
        {
            var words = origString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            //make sure the last word is in the string (to ensure the for loop did not terminate early)
            Assert.That(result.Contains(words[words.Length-1].Trim()));
            //because new lines are being made, the old and new string should never be equal
            Assert.That(result, Is.Not.EqualTo(origString));
            Assert.That(result.Contains('\n'));
            Assert.That(result.Contains(" \n") == false);
            Assert.That(result.Contains("\n ") == false);
            Assert.That(result.StartsWith("\n") == false);
            Assert.That(result.EndsWith("\n") == false);
            Assert.That(result.EndsWith(" ") == false);
        }

        ///<summary>
        /// MeasureDisplayStringWidth(Graphics graphics, string text, Font font) contains many dependences.
        /// would need to inject multple dependencies in order to test this method
        ///</summary>

        ///<summary>
        /// Tests for TestRating ValueFromLabel(string myRating) function
        ///</summary>
        [Test]
        public void ValueFromLabel_RatingNotValid_ReturnsUndefined()
        {
            //Arrange
            string myRating = "NotAValidRating";
            //Act
            var result = TestRatingLabels.ValueFromLabel(myRating);
            //Assert
            Assert.That(result, Is.EqualTo(TestRating.Undefined));
        }

        [Test]
        [TestCase("P <= .005")]
        [TestCase(".005 < P <= 0.01")]
        [TestCase("0.01 < P <= .025")]
        [TestCase(".025 < P <= 0.05")]
        [TestCase("0.05 < P <= 0.1")]
        [TestCase("P > 0.1")]
        public void ValueFromLabel_ValidRating_ReturnsAValidRatingEnum(string myRatingString)
        {
            ///Act
            var result = TestRatingLabels.ValueFromLabel(myRatingString);
            //Assert
            Assert.That(result, Is.TypeOf<TestRating>());
            Assert.That(result, Is.Not.EqualTo(TestRating.Undefined));
        }

        ///<summary>
        /// Tests for the compute class
        ///</summary>

        ///<summary>
        /// test for MinMax(DataTable myTable, DataColumn column, ref double myMin, ref double myMax) function
        /// </summary>
        [Test]
        public void MinMaxOneColumn_PassInNonEmptyDatableWithValidColumn_ReturnsTheMaxForAGivenColumn()
        {
            //arrange
            var testColumn1 = _sampleTable.Columns["Column_1"];
            var refMin = double.MaxValue;
            var refMax = double.MinValue;
            //Act
            Compute.MinMax(_sampleTable, testColumn1, ref refMin, ref refMax);
            //Assert
            Assert.That(refMin, Is.Not.EqualTo(double.MaxValue));
            Assert.That(refMax, Is.Not.EqualTo(double.MinValue));

        }
        [Test]
        public void MinMaxOneColumn_PassInNonEmptyDatableWithInvalidColumn_MaxAndMinDontChange()
        {
            //arrange
            var testColumn1 = _sampleTable.Columns["BadColumn"];
            var refMin = double.MaxValue;
            var refMax = double.MinValue;
            //Act
            Compute.MinMax(_sampleTable, testColumn1, ref refMin, ref refMax);
            //Assert
            Assert.That(refMin, Is.EqualTo(double.MaxValue));
            Assert.That(refMax, Is.EqualTo(double.MinValue));

        }
        [Test]
        public void MinMaxOneColumn_PassInEmptyDatable_MaxAndMinDontChange()
        {
            //arrange
            var testColumn1 = _sampleTable.Columns["Column_1"];
            var refMin = double.MaxValue;
            var refMax = double.MinValue;
            //Act
            Compute.MinMax(new DataTable(), testColumn1, ref refMin, ref refMax);
            //Assert
            Assert.That(refMin, Is.EqualTo(double.MaxValue));
            Assert.That(refMax, Is.EqualTo(double.MinValue));

        }
        [Test]
        public void MinMaxMultiColumn_PassInNonEmptyDatableWithTwoXValuesInBound_ReturnsTheMaxAndMinBetweenSpecifiedXRange()
        {
            //arrange
            var testColumn1 = _sampleTable.Columns["Column_1"];
            var testColumn2 = _sampleTable.Columns["Column_2"];
            var refMin = double.MaxValue;
            var refMax = double.MinValue;
            //Act
            Compute.MinMax(_sampleTable, testColumn1, ref refMin, ref refMax, 1.0, 6.0, testColumn2);
            //Assert
            Assert.That(refMin, Is.Not.EqualTo(double.MaxValue));
            Assert.That(refMax, Is.Not.EqualTo(double.MinValue));
        }
        [Test]
        public void MinMaxMultiColumn_PassInEmptyDatable_MaxAndMinDontChange()
        {
            //arrange
            var testColumn1 = _sampleTable.Columns["Column_1"];
            var testColumn2 = _sampleTable.Columns["Column_2"];
            var refMin = double.MaxValue;
            var refMax = double.MinValue;
            //Act
            Compute.MinMax(new DataTable(), testColumn1, ref refMin, ref refMax, 1.0, 6.0, testColumn2);
            //Assert
            Assert.That(refMin, Is.EqualTo(double.MaxValue));
            Assert.That(refMax, Is.EqualTo(double.MinValue));
        }

        ///<summary>
        /// test forSanityCheck(ref double myMin, ref double myMax) function
        /// </summary>
        [Test]
        public void SanityCheck_BothMaxAndMinAreOverwritten_ReturnsMinAndMax()
        {
            //Arrange
            var refMin = 10.0;
            var refMax= 100.0;
            //Act
            Compute.SanityCheck(ref refMin, ref refMax);
            //Assert
            Assert.That(refMin, Is.EqualTo(10.0));
            Assert.That(refMax, Is.EqualTo(100.0));
        }
        [Test]
        public void SanityCheck_MaxNotOverwritten_ReturnsMinValueAnd1()
        {
            //Arrange
            var refMin = 10.0;
            var refMax = double.MinValue;
            //Act
            Compute.SanityCheck(ref refMin, ref refMax);
            //Assert
            Assert.That(refMin, Is.EqualTo(10.0));
            Assert.That(refMax, Is.Not.EqualTo(double.MinValue));
        }
        [Test]
        public void SanityCheck_MinAndMaxNotOverwritten_ReturnsMinValueAnd1()
        {
            //Arrange
            var refMin = double.MaxValue;
            var refMax = double.MinValue;
            //Act
            Compute.SanityCheck(ref refMin, ref refMax);
            //Assert
            Assert.That(refMin, Is.Not.EqualTo(double.MaxValue));
            Assert.That(refMax, Is.Not.EqualTo(double.MinValue));
        }

        ///<summary>
        /// test OriginalDefault(DataColumn column) function
        /// </summary>
        [Test]
        public void OriginalDefault_DataColumnWithName_ReturnsDataColumnNameAsString()
        {
            //Arrange
            DataColumn testColumn = _sampleTable.Columns[0];
            //Act
            string result = ExtColProperty.OriginalDefault(testColumn);
            //Assert
            Assert.That(result, Is.EqualTo( "Column_1"));
        }
        [Test]
        public void OriginalDefault_DataColumnWitNoname_ReturnsAnEmptyString()
        {
            //Act
            string result = ExtColProperty.OriginalDefault(new DataColumn());
            //Assert
            Assert.That(result, Is.EqualTo(String.Empty));
        }

        ///<summary>
        /// test GetDefaultValue(string colType) function
        /// </summary>
        [Test]
        public void GetDefaultValue_ColumnTypeIsInvalid_ReturnsEmptyString()
        {
            //Arrange
           
            var colType = "Other Column";
            //Act
            var result = ExtColProperty.GetDefaultValue(colType);
            //Assert
            Assert.That(result, Is.EqualTo(String.Empty));
        }
        [Test]
        [TestCase("Unit", "myDefaultUnit")]
        [TestCase("Maximum", "10")]
        [TestCase("Minimum", "1")]
        [TestCase("Threshold", "5")]
        public void GetDefaultValue_ColumnTypeIsValid_ReturnsDefaultAsString(string colTypeInput, string expectedDefault)
        {
            //Arrange
            var colType = colTypeInput;
            //Act
            var result = ExtColProperty.GetDefaultValue(colType);
            //Assert
            Assert.That(result, Is.EqualTo(expectedDefault));
        }
    }

}
