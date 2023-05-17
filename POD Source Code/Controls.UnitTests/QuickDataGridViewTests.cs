using System;
using System.Collections.Generic;
using System.Linq;
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
            // Note that adding these columns will set the datagridview row count to 1 by default
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
            //Set a ToolTip to prevent any null reference exception
            _quickDGView.ToolTip = new ToolTip();
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

        /// skipping tests for GridCheckForProblems()  for now
        /// need to ask Tom about this
        /// 

        /// Tests for ValidResponsRowCount getter
        [Test]
        public void ValidResponsRowCount_RowCountIsLessThanOrEqualTo1_Returns0()
        {
            //Act
            var result=_quickDGView.ValidResponsRowCount;
            //Assert
            Assert.That(result, Is.Zero);
        }
        [Test]
        [TestCase(RowStatus.Valid, CellStatus.Valid, 2)]
        [TestCase(RowStatus.Invalid, CellStatus.Valid, 1)]
        [TestCase(RowStatus.Valid, CellStatus.Invalid, 1)]
        [TestCase(RowStatus.Invalid, CellStatus.Invalid, 1)]
        public void ValidResponseRowCount_RowCountIsGreaterThan1AndStatusIsCellTagAndRowStatusIsRowTag_ReturnsARowCountOf1(RowStatus rowStatus,
                                                                                                                           CellStatus cellStatus,
                                                                                                                           int expectedRowCount)
        {
            //Arrange
            //Row will always be added to count     
            _quickDGView.Rows[0].Tag = new RowTag(RowStatus.Valid);
            _quickDGView.Rows[0].Cells[2].Tag = new CellTag(CellStatus.Valid);
            //Row may or may not be added to count
            DataGridViewRow myRow = new DataGridViewRow();
            _quickDGView.Rows.Add(myRow);
            _quickDGView.Rows[1].Tag= new RowTag(rowStatus);
            _quickDGView.Rows[1].Cells[2].Tag = new CellTag(cellStatus);
            //Act
            var result = _quickDGView.ValidResponsRowCount;
            //Assert
            Assert.That(result, Is.EqualTo(expectedRowCount));
        }
        [Test]
        public void ValidResponseRowCount_RowCountIsGreaterThan1AndStatusIsNotCellTagAndRowStatusIsNotRowTag_ReturnsARowCountOf0()
        {
            //Arrange
            //Flipped the assignments in order for them both to be invalid tags
            _quickDGView.Rows[0].Tag = new CellTag(CellStatus.Valid);
            _quickDGView.Rows[0].Cells[2].Tag = new RowTag(RowStatus.Valid);
            //Act
            var result = _quickDGView.ValidResponsRowCount;
            //Assert
            Assert.That(result, Is.EqualTo(0));
        }
        /// Tests for DeleteSelectedRow() function
        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void DeleteSelectedRow_NoUserAddedRows_RowIsNeverRemovedEvenIfSelected(bool rowSelected)
        {
            //Arrange
            _quickDGView.Rows[0].Selected = rowSelected;
            //Act
            _quickDGView.DeleteSelectedRow();
            //Assert
            Assert.That(_quickDGView.Rows.Count, Is.EqualTo(1));
        }
        [Test]
        [TestCase(1, false, 2)]
        [TestCase(1, true, 2)]
        [TestCase(0, false, 2)]
        [TestCase(0, true, 1)]
        public void DeleteSelectedRow_UserAddedRows_RowIsRemovedIfItIsSelectedAndNotTheLastRow(int rowSelectedIndex, bool rowSelected, int expectedRows)
        {
            //Arrange
            _quickDGView.Rows.Add(new DataGridViewRow());
            _quickDGView.Rows[rowSelectedIndex].Selected = rowSelected;
            //Act
            _quickDGView.DeleteSelectedRow();
            //Assert
            Assert.That(_quickDGView.Rows.Count, Is.EqualTo(expectedRows));
        }
        /// Tests for AddRow() function
        [Test]
        public void AddRow_NoSelectedCellsAndLastDataRowIsNull_InsertsRowAtIndexZeroAndSetsCurrentCellToFirstCellInTheFirstRow()
        {
            //Arrange
            //Act
            _quickDGView.AddRow();
            //Assert
            Assert.That(_quickDGView.Rows.Count, Is.GreaterThan(1));
            Assert.That(_quickDGView.CurrentCell.RowIndex, Is.Zero);
            Assert.That(_quickDGView.CurrentCell.ColumnIndex, Is.Zero);
            Assert.That(_quickDGView.CurrentCell.Selected, Is.True);
        }
        /// Tests for AddRow() function
        [Test]
        public void AddRow_NoSelectedCellsAndLastDataRowIsNOTNull_InsertsRowAtLastColumnIndexAndSetsCurrentCellToTheRowAndColumnIndexOfLastDataRow()
        {
            //Arrange
            _quickDGView.Rows.Add(new DataGridViewRow());
            //Act
            _quickDGView.AddRow();
            //Assert
            Assert.That(_quickDGView.Rows.Count, Is.GreaterThan(2));
            Assert.That(_quickDGView.CurrentCell.RowIndex, Is.EqualTo(1));
            Assert.That(_quickDGView.CurrentCell.ColumnIndex, Is.Zero);
            Assert.That(_quickDGView.CurrentCell.Selected, Is.True);
        }
        [Test]
        public void AddRow_SelectedCellsAndIsAddByUserRowTrueWithLastDataRowNull_InsertsRowInFrontOfTheIndexOfTheFirstSelectedCell()
        {
            //Arrange
            _quickDGView.Rows[0].Cells[0].Selected = true;
            //Act
            _quickDGView.AddRow();
            //Assert
            Assert.That(_quickDGView.Rows.Count, Is.GreaterThan(1));
            Assert.That(_quickDGView.CurrentCell.RowIndex, Is.Zero);
            Assert.That(_quickDGView.CurrentCell.ColumnIndex, Is.Zero);
            Assert.That(_quickDGView.CurrentCell.Selected, Is.True);
        }
        [Test]
        public void AddRow_SelectedCellsAndIsAddByUserRowTrueWithLastDataRowNOTNull_InsertsRowInFrontOfTheIndexOfTheFirstSelectedCell()
        {
            //Arrange
            _quickDGView.Rows.Add(new DataGridViewRow());
            //IsAddByUserRow(cell.RowIndex) will be true
            _quickDGView.Rows[1].Cells[1].Selected = true;
            //Act
            _quickDGView.AddRow();
            //Assert
            Assert.That(_quickDGView.Rows.Count, Is.GreaterThan(2));
            Assert.That(_quickDGView.CurrentCell.RowIndex, Is.EqualTo(1));
            Assert.That(_quickDGView.CurrentCell.ColumnIndex, Is.EqualTo(1));
            Assert.That(_quickDGView.CurrentCell.Selected, Is.True);
        }
        [Test]
        public void AddRow_SelectedCellsAndIsAddByUserRowIsFalse_InsertsRowBelowTheIndexOfTheFirstSelectedCell()
        {
            //Arrange
            _quickDGView.Rows.Add(new DataGridViewRow());
            //IsAddByUserRow(cell.RowIndex) will be false because The selected cell is in the first cell of two rows and LastDataRow is not null
            _quickDGView.Rows[0].Cells[0].Selected = true;
            //Act
            _quickDGView.AddRow();
            //Assert
            Assert.That(_quickDGView.Rows.Count, Is.GreaterThan(2));
            Assert.That(_quickDGView.CurrentCell.RowIndex, Is.EqualTo(0));
            Assert.That(_quickDGView.CurrentCell.ColumnIndex, Is.EqualTo(0));
            Assert.That(_quickDGView.CurrentCell.Selected, Is.True);
        }
    }
}
