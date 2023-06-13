using POD;
using POD.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class TableUpdaterFromInfos : ITableUpdaterFromInfos
    {
        private IColumnUpdaterFromInfos _columnUpdaterFromInfos;
        public TableUpdaterFromInfos(IColumnUpdaterFromInfos columnUpdaterFromInfos = null)
        {
            _columnUpdaterFromInfos = columnUpdaterFromInfos ?? new ColumnUpdaterFromInfos();
        }
        public void UpdateTableFromInfos(SourceInfo sourceInfo, ColType type, DataTable table, DataTable activeTable, List<string> availableNames, List<string> activatedNames)
        {
            //fix the available columns
            var infos = sourceInfo.GetInfos(type);
            var originals = GetOriginalNamesFromTable(table);
            var columns = table.Columns;
            var activeColumns = activeTable.Columns;

            foreach (DataColumn col in columns)
            {
                foreach (var info in infos)
                {
                    if (info.OriginalName == originals[col.Ordinal])
                    {
                        _columnUpdaterFromInfos.UpdateColumnFromInfo(col, info);

                        var index = availableNames.IndexOf(info.NewName);

                        availableNames[col.Ordinal] = info.NewName;
                        if (col.Ordinal < activatedNames.Count)
                            activatedNames[col.Ordinal] = info.NewName;
                        break;
                    }
                }
            }

            foreach (DataColumn col in activeColumns)
            {
                foreach (var info in infos)
                {
                    if (info.OriginalName == originals[col.Ordinal])
                    {
                        _columnUpdaterFromInfos.UpdateColumnFromInfo(col, info);
                        break;
                    }
                }
            }
        }
        private List<string> GetOriginalNamesFromTable(DataTable table)
        {
            var names = new List<string>();

            foreach (DataColumn column in table.Columns)
            {
                if (column.ExtendedProperties.ContainsKey(ExtColProperty.Original))
                    names.Add(column.ExtendedProperties[ExtColProperty.Original].ToString());
                else
                {
                    names.Add(column.ColumnName);
                    //if it is missing then add it
                    column.ExtendedProperties[ExtColProperty.Original] = column.ColumnName;
                }
            }
            return names;
        }

    }
    public interface ITableUpdaterFromInfos
    {
        void UpdateTableFromInfos(SourceInfo sourceInfo, ColType type, DataTable table, DataTable activeTable, List<string> availableNames, List<string> activatedNames);
    }

}
