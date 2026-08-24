// <copyright file="ExcelWriterControl.cs" company="The University of Dayton">
// Copyright © 2025 The University of Dayton. All rights reserved.
//
// Licensed under the GNU General Public License, Version 3.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.gnu.org/licenses/gpl-3.0.txt
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

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

            _excelWriter.Workbook.AddWorksheet($"{_worksheetName} Residuals");

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

            string flawAxisTitle = $"{data.AvailableFlawNames[0]} ({data.AvailableFlawUnits[0]})";
            string responseAxisTitle = $"Residual ({data.AvailableResponseUnits[0]})";

            if (data.DataType == AnalysisDataTypeEnum.AHat)
                _excelWriter.InsertResidualChartIntoWorksheet(rowIndexChartStart, 3, rowIndex - 1, 5, "Residuals", flawAxisTitle, responseAxisTitle);
            else
                _excelWriter.InsertResidualChartIntoWorksheet(rowIndexChartStart, 2, rowIndex - 1, 4, "Residuals", flawAxisTitle, responseAxisTitle);

            colIndex = 1;
            rowIndex++;

            if (residualCensoredTable != null)
            {
                //change to excel appropriate names
                oldNames = data.ChangeTableColumnNames(residualCensoredTable, censoredNames);

                _excelWriter.SetCellValue(rowIndex, colIndex, "CENSORED RESIDUAL DATA:");
                _excelWriter.Workbook.MergeWorksheetCells(rowIndex, colIndex, rowIndex++, colIndex + 2);
                _excelWriter.WriteTableToExcel(residualCensoredTable, ref rowIndex, ref colIndex);

                //restore back to source code names
                data.ChangeTableColumnNames(residualCensoredTable, oldNames);
            }

            WriteAnalysisName(_excelWriter, _analysisName);
        }

        public void WritePODToExcel(IAnalysisData data, int podEndIndex)
        {
            var podNames = new List<string>(new string[] { "a", "POD(a)", "95 confidence bound" });
            var oldNames = new List<string>();

            _excelWriter.Workbook.AddWorksheet($"{_worksheetName} POD");

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

            string flawAxisTitle = $"{data.AvailableFlawNames[0]} ({data.AvailableFlawUnits[0]})";
            _excelWriter.InsertChartIntoWorksheet(3, 1, podEndIndex, 3, "POD Curve", PODChartLocations.Default, flawAxisTitle, "Probability of Detection (POD)");

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

            _excelWriter.Workbook.AddWorksheet($"{_worksheetName} {data.AdditionalWorksheet1Name}");

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

        public void WriteQQPlotToExcel(IAnalysisData data, DataTable qqPlotTable)
        {
            string sheetName = $"{_worksheetName} QQ Plot";
            _excelWriter.Workbook.AddWorksheet(sheetName);

            int rowIndex = 1;
            int colIndex = 1;

            _excelWriter.SetCellValue(rowIndex, colIndex, "Analysis Name");
            WriteTableOfContentsLink(_excelWriter, _worksheetName, _partOfProject, 1, 1);
            rowIndex++;

            // 1. Create a strict export table to guarantee exact columns and clean headers
            DataTable exportTable = new DataTable();
            exportTable.Columns.Add("Theoretical Quantiles", typeof(double));
            exportTable.Columns.Add("Sample Quantiles", typeof(double));
            exportTable.Columns.Add("Reference Line", typeof(double));
            exportTable.Columns.Add("Lower 95% Bound", typeof(double));
            exportTable.Columns.Add("Upper 95% Bound", typeof(double));

            // 2. Safely map the raw R data to our clean table
            foreach (DataRow row in qqPlotTable.Rows)
            {
                DataRow newRow = exportTable.NewRow();
                newRow[0] = row["theoretical_x"];
                newRow[1] = row["sample_y"];
                newRow[2] = row["reference_line"];
                newRow[3] = row["lower_ci_95"];
                newRow[4] = row["upper_ci_95"];
                exportTable.Rows.Add(newRow);
            }

            _excelWriter.SetCellValue(rowIndex, colIndex, "QQ PLOT DATA:");
            _excelWriter.Workbook.MergeWorksheetCells(rowIndex, colIndex, rowIndex++, colIndex + 4);

            int startRow = rowIndex;
            _excelWriter.WriteTableToExcel(exportTable, ref rowIndex, ref colIndex);

            // 3. Pass the explicit sheetName to the chart builder
            _excelWriter.InsertQQPlotChartIntoWorksheet(sheetName, startRow, 1, rowIndex - 1, 5, "Normal Q-Q Plot", "Theoretical (Gaussian) Quantiles", "Sample (Residual) Quantiles");

            WriteAnalysisName(_excelWriter, _analysisName);
        }

        public void WriteNormalityToExcel(IAnalysisData data, DataTable normalityTable, DataTable normalCurveTable)
        {
            string sheetName = $"{_worksheetName} Normality";
            _excelWriter.Workbook.AddWorksheet(sheetName);

            int rowIndex = 1;
            int colIndex = 1;

            _excelWriter.SetCellValue(rowIndex, colIndex, "Analysis Name");
            WriteTableOfContentsLink(_excelWriter, _worksheetName, _partOfProject, 1, 1);
            rowIndex++;

            _excelWriter.SetCellValue(rowIndex, colIndex, "NORMALITY DATA:");
            _excelWriter.Workbook.MergeWorksheetCells(rowIndex, colIndex, rowIndex, colIndex + 1);

            _excelWriter.SetCellValue(rowIndex, colIndex + 3, "NORMAL CURVE DATA:");
            _excelWriter.Workbook.MergeWorksheetCells(rowIndex, colIndex + 3, rowIndex++, colIndex + 4);

            int startRow = rowIndex;

            int histCol = 1;
            int histRow = startRow;
            _excelWriter.WriteTableToExcel(normalityTable, ref histRow, ref histCol);

            int curveCol = 4;
            int curveRow = startRow;
            if (normalCurveTable.Columns.Count > 1) { normalCurveTable.Columns.RemoveAt(0); }
            _excelWriter.WriteTableToExcel(normalCurveTable, ref curveRow, ref curveCol);

            string unitStr = (data.AvailableResponseUnits != null && data.AvailableResponseUnits.Count > 0) ? data.AvailableResponseUnits[0] : "units";

            // Pass explicit sheetName
            _excelWriter.InsertNormalityChartIntoWorksheet(sheetName, startRow, 1, histRow - 1, 2, "Normality", $"Residual ({unitStr})", "Frequency");

            WriteAnalysisName(_excelWriter, _analysisName);
        }

        public void WritePODThresholdToExcel(IAnalysisData data, DataTable thresholdTable)
        {
            var thresholdNames = new List<string>(new string[] { "Threshold", "a90", "a90_95", "a50", });
            var oldNames = new List<string>();

            _excelWriter.Workbook.AddWorksheet($"{_worksheetName} {data.AdditionalWorksheet1Name}");

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

            string flawAxisTitle = $"{data.AvailableFlawNames[0]} ({data.AvailableFlawUnits[0]})";
            _excelWriter.InsertChartIntoWorksheet(3, 1, rowIndex - 1, 3, "POD Threshold", PODChartLocations.Default, flawAxisTitle, "Probability of Detection (POD)");

            //restore back to source code names
            data.ChangeTableColumnNames(thresholdTable, oldNames);

            colIndex = 1;
            rowIndex++;

            WriteAnalysisName(_excelWriter, _analysisName);
        }

        public void WriteRemovedPointsToExcel(IAnalysisData data, DataTable podCurveTableAll, DataTable thresholdPlotTable, DataTable thresholdPlotTableAll)
        {
            DataTable removedPoints = data.GenerateRemovedPointsTable();

            _excelWriter.Workbook.AddWorksheet($"{_worksheetName} Removed Points");

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

                string flawAxisTitle = $"{data.AvailableFlawNames[0]} ({data.AvailableFlawUnits[0]})";

                _excelWriter.InsertChartIntoWorksheet(startRow, 1, rowIndex - 1, 3, "POD Curve (All Points)", PODChartLocations.Compare1, flawAxisTitle, "Probability of Detection (POD)");

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

                _excelWriter.InsertChartIntoWorksheet(startRow, 1, rowIndex - 1, 3, "Threshold Curve (All Points)", PODChartLocations.Compare2, flawAxisTitle, "Probability of Detection (POD)");

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
        void WriteNormalityToExcel(IAnalysisData data, DataTable normalityTable, DataTable normalCurveTable);
        void WriteQQPlotToExcel(IAnalysisData data, DataTable qqPlotTable);
        void WriteResidualsToExcel(IAnalysisData data, DataTable residualCensoredTable);
        void WritePODToExcel(IAnalysisData data, int podEndIndex);
        void WriteIterationsToExcel(IAnalysisData data, DataTable iterationsTable);
        void WritePODThresholdToExcel(IAnalysisData data, DataTable thresholdTable);
        void WriteRemovedPointsToExcel(IAnalysisData data, DataTable podCurveTableAll, DataTable thresholdPlotTable, DataTable thresholdPlotTableAll);
    }
}