using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class AddRowToTableControl : IAddRowToTableControl
    {
        public void AddStringRowToTable(string myID, int index, DataTable table)
        {
            if (table.Rows.Count <= index)
            {
                var row = table.NewRow();

                row[0] = myID;

                table.Rows.Add(row);
            }
            else
                table.Rows[index][0] = myID;
        }

        public void AddDoubleRowToTable(double myValues, int index, DataTable table)
        {
            if (table.Rows.Count <= index)
            {
                var row = table.NewRow();

                row[0] = myValues;

                table.Rows.Add(row);
            }
            else
                table.Rows[index][0] = myValues;
        }
    }
    public interface IAddRowToTableControl
    {
        void AddStringRowToTable(string myID, int index, DataTable table);
        void AddDoubleRowToTable(double myValues, int index, DataTable table);
    }
}
