using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POD.Data
{
    /// <summary>
    /// Holds a set of related Table Ranges. One specifying how the table is going to be copied from a table.
    /// And the other specifying how it will copied into the new table.
    /// </summary>
    [Serializable]
    public class TableRangePair
    {
        #region Fields
        /// <summary>
        /// Specifies how data will be copied FROM the table to be imported.
        /// </summary>
        public TableRange From;
        /// <summary>
        /// Specifies where the data will be copied TO the new table.
        /// </summary>
        public TableRange To;
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new Table Range Pair.
        /// </summary>
        /// <param name="myFrom"></param>
        /// <param name="myTo"></param>
        public TableRangePair(TableRange myFrom, TableRange myTo)
        {
            From = myFrom;
            To = myTo;
        }
        #endregion

        #region Properties
        #endregion

        #region Methods
        #endregion

        #region Event Handling
        #endregion

    }
}
