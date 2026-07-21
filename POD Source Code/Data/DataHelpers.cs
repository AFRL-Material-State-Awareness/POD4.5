// <copyright file="DataHelpers.cs" company="The University of Dayton">
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Diagnostics;

namespace POD.Data
{
    

    public enum ColType
    {
        Response,
        Flaw,
        Meta,
        ID
    }

    public static class DataHelpers
    {
        public static void DebugPrintChartValues(this DataTable instance)
        {
            // Using LINQ to get all column names
            var columnNames = instance.Columns.Cast<DataColumn>().Select(col => col.ColumnName);
            string colValues = string.Join(", ", columnNames);

            Debug.WriteLine(colValues);

            foreach (DataRow row in instance.Rows)
            {
                // Build a string of cell values for the current row
                string rowValues = string.Join(", ", row.ItemArray); 

                Debug.WriteLine(rowValues);
            }
        }

        public static void GetMinMaxOfColumn(this DataTable instance, int columnIndex, out double myMin, out double myMax)
        {
            if (columnIndex < 0 || columnIndex >= instance.Columns.Count)
            {
                myMin = 0;
                myMax = 0;

                throw new IndexOutOfRangeException($"Column Index {columnIndex} is outside of data table '{instance.TableName}' which has {instance.Columns.Count} column(s)");
            }

            GetMinMaxOfColumn(instance, instance.Columns[columnIndex], out myMin, out myMax);
        }

        public static void GetMinMaxOfColumn(this DataTable instance, string columnName, out double myMin, out double myMax)
        {
            if (!instance.Columns.Contains(columnName))
            {
                myMin = 0;
                myMax = 0;

                throw new ArgumentException($"Data table '{instance.TableName}' does not contain a column named '{columnName}'");
            }

            GetMinMaxOfColumn(instance, instance.Columns[columnName], out myMin, out myMax);
        }

        public static void GetMinMaxOfColumn(DataTable instance, DataColumn column, out double myMin, out double myMax)
        {
            if (column == null)
            {
                throw new ArgumentException($"DataColumn is null");
            }

            if (!IsNumericColumn(column))
            {
                throw new ArgumentException($"Data table {instance.TableName}[{column.ColumnName}] is of type {column.DataType} and not numeric.");
            }

            myMin = instance.AsEnumerable().Min(row => row.Field<double>(column));
            myMax = instance.AsEnumerable().Max(row => row.Field<double>(column));
        }

        public static bool IsNumericColumn(this DataColumn column)
        {
            if (column == null)
            {
                return false; // Or throw an exception, depending on your needs
            }

            Type dataType = column.DataType;

            return dataType == typeof(Int16) ||
                   dataType == typeof(Int32) ||
                   dataType == typeof(Int64) ||
                   dataType == typeof(UInt16) ||
                   dataType == typeof(UInt32) ||
                   dataType == typeof(UInt64) ||
                   dataType == typeof(Single) ||
                   dataType == typeof(Double) ||
                   dataType == typeof(Decimal);
        }

        public static DataTable GetTableFromRtfString(string rtfData)
        {
            int rowEnd = 0;
            int rowStart = 0;
            var dataTable = new DataTable();
            var firstRow = true;
            var rowIndex = 0;

            do
            {


                rowEnd = rtfData.IndexOf(@"\row", rowEnd, StringComparison.OrdinalIgnoreCase);
                if (rowEnd < 0)
                {
                    break;
                }
                if (rtfData[rowEnd - 1] == '\\')
                {
                    rowEnd++;
                    continue;
                }

                rowStart = rtfData.LastIndexOf(@"\trowd", rowEnd, StringComparison.OrdinalIgnoreCase);
                if (rowStart < 0)
                {
                    break;
                }
                if (rtfData[rowStart - 1] == '\\')
                {
                    rowEnd++;
                    continue;
                }

                string rowStr = rtfData.Substring(rowStart, rowEnd - rowStart);
                rowEnd++;

                var idxCell = 0;
                var idxCellMem = 0;
                var myDataRow = new List<string>();

                if (!firstRow)
                {
                    myDataRow.Add(string.Format("{0:####}", rowIndex));
                }

                do
                {
                    idxCell = rowStr.IndexOf(@"\Cell ", idxCell, StringComparison.OrdinalIgnoreCase);
                    if (idxCell < 0)
                    {
                        break;
                    }
                    if (rowStr[idxCell - 1] == '\\')
                    {
                        idxCell++;
                        continue;
                    }

                    myDataRow.Add(PurgeRtfCmds(rowStr.Substring(idxCellMem, idxCell - idxCellMem)));
                    idxCellMem = idxCell;
                    idxCell++;
                }
                while (idxCellMem > 0);

                if (firstRow)
                {
                    dataTable.Columns.Add("Auto Row ID");

                    var foundNumbers = false;

                    firstRow = false;
                    foreach (string colName in myDataRow)
                    {
                        var value = 0.0;
                        var tempName = colName;

                        if (Double.TryParse(colName, out value))
                        {
                            tempName = "TEMP";
                            foundNumbers = true;
                        }

                        var cleanColumn = Globals.CleanColumnName(tempName);

                        var newName = cleanColumn;
                        var tryIndex = 2;



                        while (dataTable.Columns.Contains(newName))
                        {
                            newName = $"{cleanColumn}_{tryIndex:##}";
                            tryIndex++;
                        }

                        dataTable.Columns.Add(newName);
                    }

                    if (foundNumbers)
                    {
                        myDataRow.Insert(0, $"{rowIndex:###0}");

                        dataTable.Rows.Add(myDataRow.ToArray());
                    }
                }
                else
                {
                    dataTable.Rows.Add(myDataRow.ToArray());
                }

                rowIndex++;
            }
            while ((rowStart > 0) && (rowEnd > 0));

            return dataTable;
        }

        private static string PurgeRtfCmds(string stringRtf)
        {
            int indexStart = 0;
            int indexEnd = 0;

            while (indexStart < stringRtf.Length)
            {
                indexStart = stringRtf.IndexOf('\\', indexStart);
                if (indexStart < 0)
                {
                    break;
                }
                if (stringRtf[indexStart + 1] == '\\')
                {
                    stringRtf = stringRtf.Remove(indexStart, 1);   //1 offset to erase space
                    indexStart++; //sckip "\"
                }
                else
                {
                    indexEnd = stringRtf.IndexOf(' ', indexStart);
                    if (indexEnd < 0)
                    {
                        if (stringRtf.Length > 0)
                        {
                            indexEnd = stringRtf.Length - 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                    stringRtf = stringRtf.Remove(indexStart, indexEnd - indexStart + 1);   //1 offset to erase space
                }
            }

            stringRtf = stringRtf.Trim(new char[] { ' ' });

            return stringRtf;
        }
    }
}
