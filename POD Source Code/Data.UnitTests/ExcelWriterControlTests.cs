using NUnit.Framework;
using System;
using Moq;
using POD.Data;
using System.Collections.Generic;
using System.Data;
using POD;
using CSharpBackendWithR;
using POD.ExcelData;
using static POD.Data.SortPoint;
using System.Linq;
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
        [SetUp]
        public void Setup()
        {
            _data = new Mock<IAnalysisData>();
            _excelWriter = new Mock<IExcelExport>();
            _excelWriteControl = new ExcelWriterControl(_excelWriter.Object, "AnalysisName", "WorkSheetName", true);
            _residualUncensoredTable = new DataTable();
        }
        /// TEsts for the WriteResidualsToExcel(IAnalysisData data, DataTable residualCensoredTable) function
        [Test]
        public void WriteResidualsToExcel_DataType_CreatesAHatUncensoredNamesAndPassesAHigherEndCellValue()
        {
            //Arrange
            List<string> uncensoredNames = new List<string>(new string[] { "a", "ahat", _data.Object.FlawTransFormLabel, _data.Object.ResponseTransformLabel, "fit", "diff" });
            var spreadSheet = new SLDocument();
            _excelWriter.SetupGet(ew => ew.Workbook).Returns(spreadSheet);
            _data.SetupGet(d => d.DataType).Returns(AnalysisDataTypeEnum.AHat);
            _data.SetupGet(d => d.ResidualUncensoredTable).Returns((DataTable)null);
            //Act
            _excelWriteControl.WriteResidualsToExcel(_data.Object, _residualUncensoredTable);
            //Assert
            _data.Verify(d => d.ChangeTableColumnNames(It.IsAny<DataTable>(), uncensoredNames));
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
            _data.SetupGet(d => d.ResidualUncensoredTable).Returns((DataTable)null);
            //Act
            _excelWriteControl.WriteResidualsToExcel(_data.Object, _residualUncensoredTable);
            //Assert
            _data.Verify(d => d.ChangeTableColumnNames(It.IsAny<DataTable>(), uncensoredNames));
            _excelWriter.Verify(ew => ew.InsertResidualChartIntoWorksheet(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), 4));
            GeneralAssertionsWriteResiduals();
        }
        private void GeneralAssertionsWriteResiduals()
        {
            _excelWriter.Verify(ew => ew.SetCellValue(1, 1, "Analysis Name"));
            //int rowindex = 3;
            //int colindex = 1;
            //_excelWriter.Verify(ew => ew.WriteTableToExcel(_data.Object.FitResidualsTable, ref rowindex, ref colindex));
            _data.Verify(d => d.ChangeTableColumnNames(null, It.IsAny<List<string>>()));
            _data.Verify(d => d.ChangeTableColumnNames(_residualUncensoredTable, It.IsAny<List<string>>()));
        }

    }
}
