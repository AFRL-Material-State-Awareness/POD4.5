using NUnit.Framework;
using System;
using Moq;
using POD.Data;
using System.Collections.Generic;
using System.Data;
using POD;
using POD.ExcelData;
using SpreadsheetLight;

namespace Data.UnitTests
{
    [TestFixture]
    public class ExcelWriterControlTests
    {
        private Mock<IAnalysisData> _data;
        private Mock<IExcelExport> _excelWriter;
        private ExcelWriterControl _excelWriteControl;
        private DataTable _residualUncensoredTable;
        private DataTable _sampleTable;
        [SetUp]
        public void Setup()
        {
            _data = new Mock<IAnalysisData>();
            _excelWriter = new Mock<IExcelExport>();
            _excelWriteControl = new ExcelWriterControl(_excelWriter.Object, "AnalysisName", "WorkSheetName", true);
            _residualUncensoredTable = new DataTable();
            _sampleTable = new DataTable();
            GenerateSampleTable();
        }
        private void GenerateSampleTable()
        {
            _sampleTable.Columns.Add("Column1");
            _sampleTable.Columns.Add("Column2");
            _sampleTable.Columns.Add("Column3");
            _sampleTable.Rows.Add(1, 1, 1);
        }
        /// TEsts for the WriteResidualsToExcel(IAnalysisData data, DataTable residualCensoredTable) function
        [Test]
        public void WriteResidualsToExcel_DataTypeIsHat_CreatesAHatUncensoredNamesAndPassesAHigherEndCellValue()
        {
            //Arrange
            List<string> uncensoredNames = new List<string>(new string[] { "a", "ahat", _data.Object.FlawTransFormLabel, _data.Object.ResponseTransformLabel, "fit", "diff" });
            var spreadSheet = new SLDocument();
            _excelWriter.SetupGet(ew => ew.Workbook).Returns(spreadSheet);
            _data.SetupGet(d => d.DataType).Returns(AnalysisDataTypeEnum.AHat);
            _data.SetupGet(d => d.ResidualUncensoredTable).Returns(_sampleTable);
            //Act
            _excelWriteControl.WriteResidualsToExcel(_data.Object, _residualUncensoredTable);
            //Assert
            _data.Verify(d => d.ChangeTableColumnNames(_sampleTable, uncensoredNames));
            _excelWriter.Verify(ew => ew.InsertResidualChartIntoWorksheet(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), 5)); 
            GeneralAssertionsWriteResiduals();
        }
        [Test]
        public void WriteResidualsToExcel_DataTypeIsHitMiss_CreatesHitMissUncensoredNamesAndPassesALowerEndCellValue()
        {
            //Arrange
            List<string> uncensoredNames = new List<string>(new string[] { "a", _data.Object.FlawTransFormLabel, "hitrate", "fit", "diff" });
            var spreadSheet = new SLDocument();
            _excelWriter.SetupGet(ew => ew.Workbook).Returns(spreadSheet);
            _data.SetupGet(d => d.DataType).Returns(AnalysisDataTypeEnum.HitMiss);
            _data.SetupGet(d => d.ResidualUncensoredTable).Returns(_sampleTable);
            //Act
            _excelWriteControl.WriteResidualsToExcel(_data.Object, _residualUncensoredTable);
            //Assert
            _data.Verify(d => d.ChangeTableColumnNames(_sampleTable, uncensoredNames));
            _excelWriter.Verify(ew => ew.InsertResidualChartIntoWorksheet(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), 4));
            GeneralAssertionsWriteResiduals();
        }
        private void GeneralAssertionsWriteResiduals()
        {
            _excelWriter.Verify(ew => ew.SetCellValue(1, 1, "Analysis Name"));
            //int rowindex = 3;
            //int colindex = 1;
            //_excelWriter.Verify(ew => ew.WriteTableToExcel(_data.Object.FitResidualsTable, ref rowindex, ref colindex));
            _data.Verify(d => d.ChangeTableColumnNames(_sampleTable, It.IsAny<List<string>>()), Times.Exactly(2));
            _data.Verify(d => d.ChangeTableColumnNames(_residualUncensoredTable, It.IsAny<List<string>>()), Times.Exactly(2));
        }
        /// Tests for void WritePODToExcel(IAnalysisData data, int podEndIndex) function
        [Test]
        [TestCase(AnalysisDataTypeEnum.HitMiss)]
        [TestCase(AnalysisDataTypeEnum.AHat)]
        public void WritePODToExel_ValidDataAndIndexPassed_WritesPODTable(AnalysisDataTypeEnum datatype)
        {
            //Arrange
            List<string> podNames = new List<string>(new string[] { "a", "POD(a)", "95 confidence bound" });
            var spreadSheet = new SLDocument();
            _excelWriter.SetupGet(ew => ew.Workbook).Returns(spreadSheet);
            _data.SetupGet(d => d.DataType).Returns(datatype);
            _data.SetupGet(d => d.PodCurveTable).Returns(_sampleTable);
            //Act
            _excelWriteControl.WritePODToExcel(_data.Object, 1);
            //Assert
            _excelWriter.Verify(ew => ew.SetCellValue(1,1,"Analysis Name"), Times.Exactly(1));
            _data.Verify(d=>d.ChangeTableColumnNames(_sampleTable, podNames));
            _excelWriter.Verify(ew => ew.SetCellValue(2, 1, "POD DATA:"), Times.Exactly(1));
        }
        /// Tests for void WriteIterationsToExcel(IAnalysisData data, DataTable iterationsTable)
        [Test]
        [TestCase(AnalysisDataTypeEnum.HitMiss)]
        [TestCase(AnalysisDataTypeEnum.AHat)]
        public void WriteIterationsToExcel_ValidDataAndIterationTablePassedPassed_WritesPODTable(AnalysisDataTypeEnum datatype)
        {
            //Arrange
            var iterationNames = new List<string>(new string[] { "trial index", "iteration index", "mu", "sigma", "fnorm", "damping" });
            var spreadSheet = new SLDocument();
            _excelWriter.SetupGet(ew => ew.Workbook).Returns(spreadSheet);
            _data.SetupGet(d => d.DataType).Returns(datatype);
            //Act
            _excelWriteControl.WriteIterationsToExcel(_data.Object, _sampleTable);
            //Assert
            _excelWriter.Verify(ew => ew.SetCellValue(1, 1, "Analysis Name"), Times.Exactly(1));
            _data.Verify(d => d.ChangeTableColumnNames(_sampleTable, iterationNames));
            _excelWriter.Verify(ew => ew.SetCellValue(2, 1, "SOLVER DATA:"), Times.Exactly(1));
        }
        /// Tests for void WritePODThresholdToExcel(IAnalysisData data, DataTable thresholdTable)
        [Test]
        [TestCase(AnalysisDataTypeEnum.HitMiss)]
        [TestCase(AnalysisDataTypeEnum.AHat)]
        public void WritePODThresholdToExcel_ValidDataAndThresholdTablePassed_WritesTheThresholdToExcel(AnalysisDataTypeEnum datatype)
        {
            var thresholdNames = new List<string>(new string[] { "Threshold", "a90", "a90_95", "a50", });
            var spreadSheet = new SLDocument();
            _excelWriter.SetupGet(ew => ew.Workbook).Returns(spreadSheet);
            _data.SetupGet(d => d.DataType).Returns(datatype);
            _data.Setup(d => d.ChangeTableColumnNames(_sampleTable, thresholdNames)).Returns(thresholdNames);
            //Act
            _excelWriteControl.WritePODThresholdToExcel(_data.Object, _sampleTable);
            //Assert
            _excelWriter.Verify(ew => ew.SetCellValue(1, 1, "Analysis Name"), Times.Exactly(1));
            _data.Verify(d => d.ChangeTableColumnNames(_sampleTable, thresholdNames));
            _excelWriter.Verify(ew => ew.SetCellValue(2, 1, "POD THRESHOLD DATA:"), Times.Exactly(1));
        }
        /// Tests for void void WriteRemovedPointsToExcel(IAnalysisData data, DataTable podCurveTableAll, DataTable thresholdPlotTable, DataTable thresholdPlotTableAll)
        private DataTable _pODTableAll;
        private DataTable _thresholdTable;
        private DataTable _thresholdTableAll;
        [Test]
        public void WriteRemovedPointsToExcel_ValidDataAndPODThresholdTableAllPassedDataTypeHitMiss_WritesRemovedPointsButDoesNotWritePODOrThresholdTable()
        {
            CreateFakeTables();
            DataTable removedPoints = GenerateRemovedPointsDataTable(new DataTable());
            var spreadSheet = new SLDocument();
            _excelWriter.SetupGet(ew => ew.Workbook).Returns(spreadSheet);
            _data.SetupGet(d => d.DataType).Returns(AnalysisDataTypeEnum.HitMiss);
            _data.Setup(d => d.GenerateRemovedPointsTable()).Returns(removedPoints);
            SetupAvailableFlawsAndResponsesNamesAndUnits();
            //Act
            _excelWriteControl.WriteRemovedPointsToExcel(_data.Object, _pODTableAll, _thresholdTable, _thresholdTableAll);
            //Assert
            AssertRemovedPointsTableWritten(6);
            _excelWriter.Verify(ew => ew.SetCellValue(It.IsAny<int>(), It.IsAny<int>(), "POD AND THRESHOLD CURVES WITH NO POINTS REMOVED:"), Times.Never);
            _data.Verify(d => d.ChangeTableColumnNames(_pODTableAll, It.IsAny<List<string>>()), Times.Never);
            _data.Verify(d => d.ChangeTableColumnNames(_thresholdTableAll, It.IsAny<List<string>>()), Times.Never);
        }
        [Test]
        public void WriteRemovedPointsToExcel_ValidDataAndPODThresholdTableAllPassedDataTypeAHatWithRemovedPoints_WritesRemovedPointsAndWritesPODOrThresholdTableIndexOverwritten()
        {
            CreateFakeTables();
            DataTable removedPoints = GenerateRemovedPointsDataTable(new DataTable());
            var spreadSheet = new SLDocument();
            _excelWriter.SetupGet(ew => ew.Workbook).Returns(spreadSheet);
            _data.SetupGet(d => d.DataType).Returns(AnalysisDataTypeEnum.AHat);
            _data.Setup(d => d.GenerateRemovedPointsTable()).Returns(removedPoints);
            SetupAvailableFlawsAndResponsesNamesAndUnits();
            //Act
            _excelWriteControl.WriteRemovedPointsToExcel(_data.Object, _pODTableAll, _thresholdTable, _thresholdTableAll);
            //Assert
            AssertRemovedPointsTableWritten(24);
            AssertAHatTables(Times.AtLeastOnce);
        }
        [Test]
        public void WriteRemovedPointsToExcel_ValidDataAndPODThresholdTableAllPassedDataTypeAHatWithNORemovedPoints_WritesRemovedPointsAndWritesPODOrThresholdTableIndexNOTOverwritten()
        {
            CreateFakeTables();
            var spreadSheet = new SLDocument();
            _excelWriter.SetupGet(ew => ew.Workbook).Returns(spreadSheet);
            _data.SetupGet(d => d.DataType).Returns(AnalysisDataTypeEnum.AHat);
            _data.Setup(d => d.GenerateRemovedPointsTable()).Returns(new DataTable());
            SetupAvailableFlawsAndResponsesNamesAndUnits();
            //Act
            _excelWriteControl.WriteRemovedPointsToExcel(_data.Object, _pODTableAll, _thresholdTable, _thresholdTableAll);
            //Assert
            AssertRemovedPointsTableWritten(6);
            AssertAHatTables(Times.Never);

        }
        private void AssertAHatTables(Func<Times> amount)
        {
            _excelWriter.Verify(ew => ew.SetCellValue(It.IsAny<int>(), It.IsAny<int>(), "POD AND THRESHOLD CURVES WITH NO POINTS REMOVED:"), amount);
            _data.Verify(d => d.ChangeTableColumnNames(_pODTableAll, It.IsAny<List<string>>()), amount);
            _data.Verify(d => d.ChangeTableColumnNames(_thresholdTableAll, It.IsAny<List<string>>()), amount);
        }
        private void AssertRemovedPointsTableWritten(int removedPointsRow)
        {
            _data.Verify(d => d.GenerateRemovedPointsTable());
            _excelWriter.Verify(ew => ew.SetCellValue(1, 1, "Analysis Name"), Times.Exactly(1));
            _excelWriter.Verify(ew => ew.SetCellValue(2, 1, "Flaw"), Times.Exactly(1));
            _excelWriter.Verify(ew => ew.SetCellValue(3, 1, "Flaw Unit"), Times.Exactly(1));
            _excelWriter.Verify(ew => ew.SetCellValue(4, 1, "Responses"), Times.Exactly(1));
            _excelWriter.Verify(ew => ew.SetCellValue(4, 2, "AvailableResponse"), Times.Exactly(1));
            _excelWriter.Verify(ew => ew.SetCellValue(4, 3, "AnotherAvailableResponse"), Times.Exactly(1));
            _excelWriter.Verify(ew => ew.SetCellValue(5, 1, "Response Units"), Times.Exactly(1));
            _excelWriter.Verify(ew => ew.SetCellValue(5, 2, "cm"), Times.Exactly(1));
            _excelWriter.Verify(ew => ew.SetCellValue(5, 3, "mm"), Times.Exactly(1));
            _excelWriter.Verify(ew => ew.SetCellValue(removedPointsRow, 1, "REMOVED POINTS:"), Times.Exactly(1));
        }
        private void CreateFakeTables()
        {
            _pODTableAll = GeneratePODCurveTableAll(new DataTable());
            _thresholdTable = GenerateThresholdPlotTable(new DataTable());
            _thresholdTableAll = GenerateThresholdPlotTableAll(new DataTable());
        }
        private DataTable GenerateRemovedPointsDataTable(DataTable data)
        {
            data.Columns.Add("RemovedPoints");
            for (int i = 1; i < 6; i++)
                data.Rows.Add(i);
            return data;
        }
        private DataTable GeneratePODCurveTableAll(DataTable data)
        {
            data.Columns.Add("POD Curve All");
            for (int i = 1; i < 6; i++)
                data.Rows.Add(i*.1);
            return data;
        }
        private DataTable GenerateThresholdPlotTable(DataTable data)
        {
            data.Columns.Add("Threshold Table");
            for (int i = 1; i < 6; i++)
                data.Rows.Add(i * 6);
            return data;
        }
        private DataTable GenerateThresholdPlotTableAll(DataTable data)
        {
            data.Columns.Add("Threshold Table All");
            for (int i = 1; i < 6; i++)
                data.Rows.Add(i * 6);
            return data;
        }
        private void SetupAvailableFlawsAndResponsesNamesAndUnits()
        {
            _data.SetupGet(d => d.AvailableFlawNames).Returns(new List<string>() { "AvailableFlaw", "AnotherAvailableFlaw" });
            _data.SetupGet(d => d.AvailableFlawUnits).Returns(new List<string>() { "um", "pm" });
            _data.SetupGet(d => d.AvailableResponseNames).Returns(new List<string>() { "AvailableResponse", "AnotherAvailableResponse" });
            _data.SetupGet(d => d.AvailableResponseUnits).Returns(new List<string>() { "cm", "mm" });
        }
    }
}
