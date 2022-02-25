using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POD.Data
{
    [Serializable]
    public class DataPointIndex : IEquatable<DataPointIndex>

    {
        public int RowIndex;
        //public string ColumnName;
        public int ColumnIndex;
        public string Reason;

        public DataPointIndex(int myColumnIndex, int myRowIndex, string reason)
        {
            //ColumnName = ""; //myColumnName;
            RowIndex = myRowIndex;
            ColumnIndex = myColumnIndex;
            Reason = reason;
        }

        public bool Equals(DataPointIndex point)
        {          
            return RowIndex == point.RowIndex && ColumnIndex == point.ColumnIndex;
        }
    }
}
