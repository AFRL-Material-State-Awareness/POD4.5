using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using SpreadsheetLight;
using SpreadsheetLight.Charts;
using System.Windows.Forms;
using System.Data;
using DocumentFormat;
//used to check valid file
using System.IO;

namespace POD.ExcelData
{
    public enum PODChartLocations
    {
        Default,
        Compare1,
        Compare2,
        Compare3,
        Compare4
    }

    public class ExcelExport
    {
        SLDocument _workbook;

        public SLDocument Workbook
        {
            get { return _workbook; }
        }

        public ExcelExport()
        {
            _workbook = new SLDocument();
        }

        public void WriteTest()
        {
            _workbook.SetCellValueNumeric(4, 2, "3.14159");
        }

        public string AskUserToSave(string myDefaultFileName, out bool shouldSave)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            
            dialog.Filter = "Excel File (*.xlsx)|*.xlsx";
            dialog.FileName = myDefaultFileName;

            DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string name = dialog.FileName;
                //get file info and check to make sure the file isn't in use by anotherProcess
                FileInfo excelFileInfo = new FileInfo(name);
                if (IsFileLocked(excelFileInfo))
                {
                    shouldSave = false;
                    return null;
                }
                shouldSave = true;

                return name;
            }
            else
            {
                shouldSave = false;

                return "";
            }
        }
        /// <summary>
        /// source: https://stackoverflow.com/questions/876473/is-there-a-way-to-check-if-a-file-is-in-use
        /// author: ChrisW , edited by Collin Dauphinee
        /// link to author: https://stackoverflow.com/users/79271/chrisw
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        protected virtual bool IsFileLocked(FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (FileNotFoundException)
            {
                //if the file is not found, we know we are not overwriting so return false
                return false;
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            

            //file is not locked
            return false;
        }
        public void SaveToFileWithDefaultName(string myDefaultFileName)
        {
            SaveFileDialog dialog = new SaveFileDialog();

            dialog.Filter = "Excel File (*.xlsx)|*.xlsx";
            dialog.FileName = myDefaultFileName;

            DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string name = dialog.FileName;

                SaveToFile(name);
            }
        }

        public void SaveToFile()
        {
            SaveFileDialog dialog = new SaveFileDialog();

            dialog.Filter = "Excel File (*.xlsx)|*.xlsx";

            DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string name = dialog.FileName;

                SaveToFile(name);
            }
        }

        public void SaveToFile(string myFileName)
        {
            try
            {
                _workbook.SaveAs(myFileName);
                _workbook = new SLDocument();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void RemoveDefaultSheet()
        {
            _workbook.DeleteWorksheet("Sheet1");
        }

        public void InsertChartIntoWorksheet(string sheetName, int startRow, int startCol, int endRow, int endCol, PODChartLocations location = PODChartLocations.Default)
        {
            SLCreateChartOptions options = new SLCreateChartOptions();
            SLChart chart = _workbook.CreateChart(startRow, startCol, endRow, endCol, options);

            CreateScatterChartForWorksheet(startRow, startCol, endRow, endCol, ref chart, location);

            var currentSheet = _workbook.GetCurrentWorksheetName();

            _workbook.SelectWorksheet(sheetName);

            Workbook.InsertChart(chart);

            _workbook.SelectWorksheet(currentSheet);
        }

        public void InsertChartIntoWorksheet(int startRow, int startCol, int endRow, int endCol, PODChartLocations location = PODChartLocations.Default)
        {
            InsertChartIntoWorksheet(_workbook.GetCurrentWorksheetName(), startRow, startCol, endRow, endCol, location);
        }

        private void CreateScatterChartForWorksheet(int startRow, int startCol, int endRow, int endCol, ref SLChart chart, PODChartLocations location = PODChartLocations.Default)
        {
            SLCreateChartOptions options = new SLCreateChartOptions();
            double top = 0.0;
            double left = 0.0;
            double bottom = 0.0;
            double right = 0.0;


            if (location == PODChartLocations.Default)
            {
                top = 3.0;
                left = 4.5;
                bottom = 35.0;
                right = 15.5;
            }
            else
            {
                top = 5.5;
                bottom = 22.5;
            }

            if(location == PODChartLocations.Compare1)
            {
                
                left = 0.5;
                right = 2.9;
            }
            else if(location == PODChartLocations.Compare2)
            {
                left = 3.0;
                right = 6.1;
            }
            else if (location == PODChartLocations.Compare3)
            {
                left = 9.75;
                right = 15.0;
            }
            else if (location == PODChartLocations.Compare4)
            {
                left = 15.25;
                right = 20.5;
            }

            chart.SetChartType(SLScatterChartType.ScatterWithSmoothLines);
            chart.SetChartPosition(top, left, bottom, right);            
        }

        public void InsertResidualChartIntoWorksheet(int startRow, int startCol, int endRow, int endCol)
        {
            SLCreateChartOptions options = new SLCreateChartOptions();
            SLChart chart = _workbook.CreateChart(startRow, startCol, endRow, endCol, options);

            CreateScatterChartForWorksheet(startRow, startCol, endRow, endCol, ref chart);

            var dataOptions = chart.GetDataSeriesOptions(1);

            dataOptions.Marker.Symbol = DocumentFormat.OpenXml.Drawing.Charts.MarkerStyleValues.Circle;
            dataOptions.Marker.Size = 7;
            dataOptions.Line.SetNoLine();

            chart.SetDataSeriesOptions(1, dataOptions);

            Workbook.InsertChart(chart);
        }

        public void SetCellValue(int rowIndex, int colIndex, string myValue)
        {
            Workbook.SetCellValue(rowIndex, colIndex, myValue);
        }

        public void SetCellValue(int rowIndex, int colIndex, double myValue)
        {
            Workbook.SetCellValue(rowIndex, colIndex, myValue);
        }

        public void SetCellValue(int rowIndex, int colIndex, DateTime myValue)
        {
            Workbook.SetCellValue(rowIndex, colIndex, myValue.ToShortDateString() + " " + myValue.ToShortTimeString());
        }

        public void WriteTableToExcel(System.Data.DataTable myTable, ref int myRowIndex, ref int myColIndex, bool fitWidthOnLastColumn = true)
        {
            if (myTable != null)
            {
                int colInc = 0;
                int rowIndex = myRowIndex + 1;
                int colIndex = myColIndex + colInc;

                foreach (DataColumn col in myTable.Columns)
                {
                    rowIndex = myRowIndex;
                    colIndex = myColIndex + colInc;

                    var colName = col.ColumnName;

                    //get rid of markers that only there to make sure column names are different
                    if(colName.StartsWith("{{") && colName.EndsWith("}}"))
                    {
                        colName = colName.TrimStart(new char[] {'{'});
                        colName = colName.TrimEnd(new char[] {'}'});
                    }

                    Workbook.SetCellValue(rowIndex++, colIndex, colName);

                    if (col.DataType == typeof(double))
                    {
                        foreach (DataRow row in myTable.Rows)
                        {
                            if (row[colInc] != DBNull.Value)
                            {
                                double value = Convert.ToDouble(row[colInc]);

                                if (Double.IsNaN(value) == false)
                                    Workbook.SetCellValue(rowIndex++, colIndex, value);
                                else
                                    Workbook.SetCellValue(rowIndex++, colIndex, "#NUM!");
                            }
                        }
                    }
                    else
                    {
                        foreach (DataRow row in myTable.Rows)
                        {
                            Workbook.SetCellValue(rowIndex++, colIndex, row[colInc].ToString());
                        }
                    }

                    colInc++;
                }

                var lastColumnIndex = myColIndex + colInc - 1;

                if (!fitWidthOnLastColumn)
                {
                    lastColumnIndex--;
                }

                if(lastColumnIndex >= 0)
                    Workbook.AutoFitColumn(myColIndex, lastColumnIndex);

                //put reference at one cell over from bottom right corner of the table
                myColIndex = myColIndex + colInc;
                myRowIndex = rowIndex;
            }
        }

        public void InsertReturnToTableOfContents(int myRowIndex, int myColIndex, string myAnalysisIndex)
        {
            int index = Convert.ToInt32(myAnalysisIndex) + 1;

            Workbook.InsertHyperlink(myRowIndex, myColIndex, SLHyperlinkTypeValues.InternalDocumentLink, "'Analysis Table of Contents'!A" + index, "Analysis Name", "Return to Analysis Table of Contents.");

            //RightJustifyCell(myRowIndex, myColIndex);
        }

        public void InsertAnalysisWorksheetLink(int myRowIndex, int myColIndex, string myAnalysisWorksheetName, string myPageName)
        {
            Workbook.InsertHyperlink(myRowIndex, myColIndex, SLHyperlinkTypeValues.InternalDocumentLink,
                                     "'" + myAnalysisWorksheetName + " " + myPageName + "'!A1",
                                     myPageName, "View Analysis " + myPageName + " Worksheet.");
        }



        public void SetCellTextWrapped(int row, int column, bool p)
        {
            var style = Workbook.GetCellStyle(row, column);

            style.SetWrapText(p);

            Workbook.SetCellStyle(row, column, style);
        }

        public void MergeCells(int rowStart,int columnStart, int rowEnd,int columnEnd)
        {
            Workbook.MergeWorksheetCells(rowStart, columnStart, rowEnd, columnEnd);
        }

        public void SetRowSize(int rowIndex, double multFactor)
        {
            var style = Workbook.GetRowHeight(rowIndex);

            Workbook.SetRowHeight(rowIndex, style * multFactor);
        }

        public void RightJustifyCell(int rowIndex, int colIndex)
        {
            var style =Workbook.GetCellStyle(rowIndex, colIndex);

            style.Alignment.Horizontal = HorizontalAlignmentValues.Right;

            Workbook.SetCellStyle(rowIndex, colIndex, style);
        }
    }
}
