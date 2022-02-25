using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace POD.Controls
{
    public class DataChartTableArg
    {
        public int ChartIndex;
        public DataTable Table;

        public DataChartTableArg(int myIndex, DataTable myTable)
        {
            ChartIndex = myIndex;
            Table = myTable;
        }
    }
}
