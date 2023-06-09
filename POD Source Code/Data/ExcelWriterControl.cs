using POD;
using POD.Data;
using POD.ExcelData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class ExcelWriterControl : IExcelWriterControl
    {
        private IExcelExport _excelWriter;
        private string _analysisName;
        private string _worksheetName;
        private bool _partOfProject;
        public ExcelWriterControl(IExcelExport myWriter, string myAnalysisName, string myWorksheetName, bool myPartOfProject)
        {
            _excelWriter = myWriter;
            _analysisName = myAnalysisName;
            _worksheetName = myWorksheetName;
            _partOfProject = myPartOfProject;
        }
        public void WriteResidualsToExcel(IAnalysisData data, DataTable residualCensoredTable)
        {
            List<string> uncensoredNames = null;

            var fitResidualNames = new List<string>(new string[] { "a", "fit" });
            if (data.DataType == AnalysisDataTypeEnum.AHat)
                uncensoredNames = new List<string>(new string[] { "a", "ahat", data.FlawTransFormLabel, data.ResponseTransformLabel, "fit", "diff" });
            else
                uncensoredNames = new List<string>(new string[] { "a", data.FlawTransFormLabel, "hitrate", "fit", "diff" });
            var censoredNames = new List<string>(new string[] { "a", "ahat", data.FlawTransFormLabel, data.ResponseTransformLabel, "fit", "diff" });
            var oldNames = new List<string>();

            _excelWriter.Workbook.AddWorksheet(_worksheetName + " Residuals");

            int rowIndex = 1;
            int colIndex = 1;

            _excelWriter.SetCellValue(rowIndex, colIndex, "Analysis Name");
            WriteTableOfContentsLink(_excelWriter, _worksheetName, _partOfProject, 1, 1);
            rowIndex++;

            //change to excel appropriate names
            oldNames = data.ChangeTableColumnNames(data.FitResidualsTable, fitResidualNames);

            _excelWriter.SetCellValue(rowIndex, colIndex, "FIT PLOT DATA:");
            _excelWriter.Workbook.MergeWorksheetCells(rowIndex, colIndex, rowIndex++, colIndex + 2);
            _excelWriter.WriteTableToExcel(data.FitResidualsTable, ref rowIndex, ref colIndex);

            //restore back to source code names
            data.ChangeTableColumnNames(data.FitResidualsTable, oldNames);

            colIndex = 1;
            rowIndex++;

            //change to excel appropriate names
            oldNames = data.ChangeTableColumnNames(data.ResidualUncensoredTable, uncensoredNames);

            _excelWriter.SetCellValue(rowIndex, colIndex, "UNCENSORED RESIDUAL DATA:");
            _excelWriter.Workbook.MergeWorksheetCells(rowIndex, colIndex, rowIndex++, colIndex + 2);
            var rowIndexChartStart = rowIndex;
            _excelWriter.WriteTableToExcel(data.ResidualUncensoredTable, ref rowIndex, ref colIndex);

            //restore back to source code names
            data.ChangeTableColumnNames(data.ResidualUncensoredTable, oldNames);

            if (data.DataType == AnalysisDataTypeEnum.AHat)
                _excelWriter.InsertResidualChartIntoWorksheet(rowIndexChartStart, 3, rowIndex - 1, 5);
            else
                _excelWriter.InsertResidualChartIntoWorksheet(rowIndexChartStart, 2, rowIndex - 1, 4);

            colIndex = 1;
            rowIndex++;

            //change to excel appropriate names
            oldNames = data.ChangeTableColumnNames(residualCensoredTable, censoredNames);

            _excelWriter.SetCellValue(rowIndex, colIndex, "CENSORED RESIDUAL DATA:");
            _excelWriter.Workbook.MergeWorksheetCells(rowIndex, colIndex, rowIndex++, colIndex + 2);
            _excelWriter.WriteTableToExcel(residualCensoredTable, ref rowIndex, ref colIndex);

            //change to excel appropriate names
            oldNames = data.ChangeTableColumnNames(residualCensoredTable, oldNames);

            WriteAnalysisName(_excelWriter, _analysisName);
        }
        
        public void WritePODToExcel(IAnalysisData data, int podEndIndex)
        {
            //var podNames = new List<string>(new string[] { "a","ln(a)", "POD(a)", "95 confidence bound" });
            var podNames = new List<string>(new string[] { "a", "POD(a)", "95 confidence bound" });
            var oldNames = new List<string>();

            _excelWriter.Workbook.AddWorksheet(_worksheetName + " POD");

            int rowIndex = 1;
            int colIndex = 1;

            _excelWriter.SetCellValue(rowIndex, colIndex, "Analysis Name");
            WriteTableOfContentsLink(_excelWriter, _worksheetName, _partOfProject, 1, 1);
            rowIndex++;

            //change to excel appropriate names
            oldNames = data.ChangeTableColumnNames(data.PodCurveTable, podNames);

            _excelWriter.SetCellValue(rowIndex, colIndex, "POD DATA:");
            _excelWriter.Workbook.MergeWorksheetCells(rowIndex, colIndex, rowIndex++, colIndex + 2);
            _excelWriter.WriteTableToExcel(data.PodCurveTable, ref rowIndex, ref colIndex);

            podEndIndex = rowIndex - 1;
            _excelWriter.InsertChartIntoWorksheet(3, 1, podEndIndex, 3);

            //restore back to source code names
            data.ChangeTableColumnNames(data.PodCurveTable, oldNames);

            colIndex = 1;
            rowIndex++;

            WriteAnalysisName(_excelWriter, _analysisName);
        }
        public void WriteIterationsToExcel(IAnalysisData data, DataTable iterationsTable)
        {
            var iterationNames = new List<string>(new string[] { "trial index", "iteration index", "mu", "sigma", "fnorm", "damping" });
            var oldNames = new List<string>();

            _excelWriter.Workbook.AddWorksheet(_worksheetName + " " +data.AdditionalWorksheet1Name);

            int rowIndex = 1;
            int colIndex = 1;

            _excelWriter.SetCellValue(rowIndex, colIndex, "Analysis Name");
            WriteTableOfContentsLink(_excelWriter, _worksheetName, _partOfProject, 1, 1);
            rowIndex++;

            //change to excel appropriate names
            oldNames = data.ChangeTableColumnNames(iterationsTable, iterationNames);

            _excelWriter.SetCellValue(rowIndex, colIndex, "SOLVER DATA:");
            _excelWriter.Workbook.MergeWorksheetCells(rowIndex, colIndex, rowIndex++, colIndex + 2);
            _excelWriter.WriteTableToExcel(iterationsTable, ref rowIndex, ref colIndex);

            //restore back to source code names
            data.ChangeTableColumnNames(iterationsTable, oldNames);

            colIndex = 1;
            rowIndex++;

            WriteAnalysisName(_excelWriter, _analysisName);
        }
        public void WritePODThresholdToExcel(IAnalysisData data, DataTable thresholdTable)
        {
            var thresholdNames = new List<string>(new string[] { "Threshold", "a90", "a90_95", "a50", });
            var oldNames = new List<string>();

            _excelWriter.Workbook.AddWorksheet(_worksheetName + " " + data.AdditionalWorksheet1Name);

            int rowIndex = 1;
            int colIndex = 1;

            _excelWriter.SetCellValue(rowIndex, colIndex, "Analysis Name");
            WriteTableOfContentsLink(_excelWriter, _worksheetName, _partOfProject, 1, 1);
            rowIndex++;

            //change to excel appropriate names
            oldNames = data.ChangeTableColumnNames(thresholdTable, thresholdNames);

            _excelWriter.SetCellValue(rowIndex, colIndex, "POD THRESHOLD DATA:");
            _excelWriter.Workbook.MergeWorksheetCells(rowIndex, colIndex, rowIndex++, colIndex + 2);
            _excelWriter.WriteTableToExcel(thresholdTable, ref rowIndex, ref colIndex);

            _excelWriter.InsertChartIntoWorksheet(3, 1, rowIndex - 1, 3);

            //restore back to source code names
            data.ChangeTableColumnNames(thresholdTable, oldNames);

            colIndex = 1;
            rowIndex++;

            WriteAnalysisName(_excelWriter, _analysisName);
        }
        public void WriteRemovedPointsToExcel(IAnalysisData data, DataTable podCurveTableAll, DataTable thresholdPlotTable, DataTable thresholdPlotTableAll)
        {
            DataTable removedPoints = data.GenerateRemovedPointsTable();

            _excelWriter.Workbook.AddWorksheet(_worksheetName + " Removed Points");

            int rowIndex = 1;
            int colIndex = 1;

            _excelWriter.SetCellValue(rowIndex++, colIndex, "Analysis Name");
            //write name at end after column fitting has been doen
            _excelWriter.SetCellValue(rowIndex, colIndex, "Flaw");
            _excelWriter.SetCellValue(rowIndex++, colIndex + 1, data.AvailableFlawNames[0]);
            _excelWriter.SetCellValue(rowIndex, colIndex, "Flaw Unit");
            _excelWriter.SetCellValue(rowIndex++, colIndex + 1, data.AvailableFlawUnits[0]);
            _excelWriter.SetCellValue(rowIndex, colIndex, "Responses");

            var addTo = 1;
            foreach (string name in data.AvailableResponseNames)
            {
                _excelWriter.SetCellValue(rowIndex, colIndex + addTo, name);
                addTo++;
            }

            rowIndex++;
            _excelWriter.SetCellValue(rowIndex, colIndex, "Response Units");

            addTo = 1;
            foreach (string unit in data.AvailableResponseUnits)
            {
                _excelWriter.SetCellValue(rowIndex, colIndex + addTo, unit);
                addTo++;
            }

            rowIndex++;



            if (data.DataType == AnalysisDataTypeEnum.AHat && removedPoints.Rows.Count > 0)
                rowIndex = 24;

            _excelWriter.SetCellValue(rowIndex, colIndex, "REMOVED POINTS:");
            _excelWriter.Workbook.MergeWorksheetCells(rowIndex, colIndex, rowIndex++, colIndex + 2);
            _excelWriter.WriteTableToExcel(removedPoints, ref rowIndex, ref colIndex, false);

            //done after fitting

            WriteTableOfContentsLink(_excelWriter, _worksheetName, _partOfProject, 1, 1);

            colIndex = 1;
            rowIndex++;

            if (data.DataType == AnalysisDataTypeEnum.AHat && removedPoints.Rows.Count > 0)
            {

                _excelWriter.SetCellValue(rowIndex, colIndex, "POD AND THRESHOLD CURVES WITH NO POINTS REMOVED:");
                _excelWriter.Workbook.MergeWorksheetCells(rowIndex++, colIndex, rowIndex++, colIndex + 3);

                var podNames = new List<string>(new string[] { "a", "POD(a)", "95 confidence bound" });
                var oldNames = new List<string>();

                //change to excel appropriate names
                oldNames = data.ChangeTableColumnNames(podCurveTableAll, podNames);

                _excelWriter.SetCellValue(rowIndex, colIndex, "POD DATA:");
                _excelWriter.Workbook.MergeWorksheetCells(rowIndex, colIndex, rowIndex++, colIndex + 2);
                var startRow = rowIndex;
                _excelWriter.WriteTableToExcel(podCurveTableAll, ref rowIndex, ref colIndex, true);

                _excelWriter.InsertChartIntoWorksheet(startRow, 1, rowIndex - 1, 3, PODChartLocations.Compare1);

                //restore back to source code names
                data.ChangeTableColumnNames(podCurveTableAll, oldNames);

                colIndex = 1;
                rowIndex++;

                var thresholdNames = new List<string>(new string[] { "Threshold", "a90", "a90_95", "a50", });
                oldNames = new List<string>();

                //change to excel appropriate names
                oldNames = data.ChangeTableColumnNames(thresholdPlotTable, thresholdNames);

                _excelWriter.SetCellValue(rowIndex, colIndex, "POD THRESHOLD DATA:");
                _excelWriter.Workbook.MergeWorksheetCells(rowIndex, colIndex, rowIndex++, colIndex + 2);
                startRow = rowIndex;
                _excelWriter.WriteTableToExcel(thresholdPlotTableAll, ref rowIndex, ref colIndex, true);

                _excelWriter.InsertChartIntoWorksheet(startRow, 1, rowIndex - 1, 3, PODChartLocations.Compare2);

                //restore back to source code names
                data.ChangeTableColumnNames(thresholdPlotTableAll, oldNames);

                colIndex = 1;
                rowIndex++;
            }

            WriteAnalysisName(_excelWriter, _analysisName);
        }
        private void WriteAnalysisName(IExcelExport myWriter, string myAnalysisName)
        {
            myWriter.SetCellValue(1, 2, myAnalysisName);
        }
        private void WriteTableOfContentsLink(IExcelExport myWriter, string _worksheetName, bool myPartOfProject, int rowIndex, int colIndex)
        {
            if (myPartOfProject)
            {
                //myWriter.Workbook.MergeWorksheetCells(rowIndex, colIndex + 1, rowIndex, colIndex + 2);
                myWriter.InsertReturnToTableOfContents(rowIndex, colIndex, _worksheetName);

            }
        }

    }
    public interface IExcelWriterControl
    {
        void WriteResidualsToExcel(IAnalysisData data, DataTable residualCensoredTable);
        void WritePODToExcel(IAnalysisData data, int podEndIndex);
        void WriteIterationsToExcel(IAnalysisData data, DataTable iterationsTable);
        void WritePODThresholdToExcel(IAnalysisData data, DataTable thresholdTable);
        void WriteRemovedPointsToExcel(IAnalysisData data, DataTable podCurveTableAll, DataTable thresholdPlotTable, DataTable thresholdPlotTableAll);
    }
}
