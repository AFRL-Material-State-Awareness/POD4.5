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

namespace Controls.UnitTests
{
    [TestFixture]
    public class QuickDataGridViewTests
    {
        private QuickDataGridView _quickDGView;
        private Mock<DataPointChart> _chart;
        private bool _runAnalysisFired;
        [SetUp]
        public void Setup()
        {
            _quickDGView = new QuickDataGridView();
            _quickDGView.AutoGenerateColumns = false;
            // There needs to be three cells per row, so this for loop will generate three columns
            for(int i =0; i < 3; i++)
            {
                DataGridViewColumn datagidcolumn = new DataGridViewColumn()
                {
                    DataPropertyName = "myDataProperty",
                    HeaderText = "myHeaderText",
                    Name = "SampleColumn",
                    CellTemplate = new DataGridViewTextBoxCell()
                };
                _quickDGView.Columns.Add(datagidcolumn);
            } 
            _chart = new Mock<DataPointChart>();
            _runAnalysisFired = false;

        }
        /// <summary>
        /// Tests for bool CheckForValidDataGrid(DataPointChart chart, string analysisName)
        /// </summary>
        [Test]
        public void CheckForValidDataGrid_ResultIsLessthan8_ReturnsFalse()
        {
            //Arrange
            for (int i = 0; i < 7; i++)
                _quickDGView.Rows.Add(new DataGridViewRow());
            //Act
            var result = _quickDGView.CheckForValidDataGrid(_chart.Object, "My Analysis Name");
            //Assert
            Assert.That(result, Is.False);
            _chart.Verify(c => c.ForceResizeAnnotations());
        }
        [Test]
        public void CheckForValidDataGrid_ResultIsGreaterThanOrEqualTo8_ReturnsTrue()
        {
            //Arrange
            for (int i = 0; i < 8; i++)
                _quickDGView.Rows.Add(new DataGridViewRow());
            //Act
            var result = _quickDGView.CheckForValidDataGrid(_chart.Object, "My Analysis Name");
            //Assert
            Assert.That(result, Is.True);
            _chart.Verify(c => c.ForceResizeAnnotations(), Times.Never);
        }
        [Test]
        [TestCase("My Analysis Name")]
        [TestCase(null)]
        public void CheckForValidDataGrid_ResultIsLessThan8AndEitherChartOrAnalysisNameIsNull_ReturnsFalseWithForceResizeAnnotationsNotCalled(string analysisName)
        {
            //Arrange
            for (int i = 0; i < 7; i++)
                _quickDGView.Rows.Add(new DataGridViewRow());
            //Act
            var result = _quickDGView.CheckForValidDataGrid(null, analysisName);
            //Assert
            Assert.That(result, Is.False);
            _chart.Verify(c => c.ForceResizeAnnotations(), Times.Never);
        }
        [Test]
        public void CheckForValidDataGrid_ResultIsLessThan8AndChartIsNotNullWhileAnalysisNameIsNull_ReturnsFalseWithForceResizeAnnotationsNotCalled()
        {
            //Arrange
            for (int i = 0; i < 7; i++)
                _quickDGView.Rows.Add(new DataGridViewRow());
            //Act
            var result = _quickDGView.CheckForValidDataGrid(_chart.Object, null);
            //Assert
            Assert.That(result, Is.False);
            _chart.Verify(c => c.ForceResizeAnnotations(), Times.Never);
        }

        /// Test for the void PasteFromClipboard(IPasteFromClipBoardWrapper clipboardPaster=null) function
        [Test]
        public void PasteFromClipboard_ValidDataPastedFromClipBoard_CallsPasteToClipBoardDependencyAndRaisesRunAnalysis()
        {
            //Arrange
            Mock<IPasteFromClipBoardWrapper> pasteFromClipboard = new Mock<IPasteFromClipBoardWrapper>();
            pasteFromClipboard.Setup(cb => cb.GetClipBoardContents(It.IsAny<TextDataFormat>())).Returns("ClipBoardContents");
            _quickDGView.RunAnalysis = null;
            _quickDGView.RunAnalysis += RunAnalysis;
            //Act
            _quickDGView.PasteFromClipboard(pasteFromClipboard.Object);
            //Assert
            pasteFromClipboard.Verify(cb => cb.GetClipBoardContents(It.IsAny<TextDataFormat>()));
            Assert.That(_runAnalysisFired, Is.True);
        }
        private void RunAnalysis(object sender, EventArgs args)
        {
            _runAnalysisFired = true;
        }
    }
}
