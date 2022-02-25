using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace POD.Data
{
    

    public enum ColType
    {
        Response,
        Flaw,
        Meta,
        ID
    }

    public class DataHelpers
    {
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

                        

                        while(dataTable.Columns.Contains(newName))
                        {
                            newName = cleanColumn + "_" + string.Format("{0:##}", tryIndex);
                            tryIndex++;
                        }

                        dataTable.Columns.Add(newName);
                    }

                    if (foundNumbers)
                    {
                        myDataRow.Insert(0, string.Format("{0:###0}", rowIndex));

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
