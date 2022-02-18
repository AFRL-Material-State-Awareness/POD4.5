using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using POD.ExcelData;

namespace POD.Data
{
    [Serializable]
    public class DataTableWithRanges
    {
        /// <summary>
        /// Holds the range of columns that hold Flaw Size data in the processed table.
        /// </summary>
        private TableRange _flawSizeRange;

        /// <summary>
        /// Holds the range of columns that hold meta-data in the processed table.
        /// </summary>
        private TableRange _metaDataRange;

        /// <summary>
        /// The data table holding the data that the imported data is stored into.
        /// </summary>
        private DataTable _table;

        /// <summary>
        /// Holds the range of columns that hold Response data in the processed table.
        /// </summary>
        private TableRange _responseRange;

        /// <summary>
        /// Holds the range of columns that hold Specimen ID data in the processed table.
        /// </summary>
        private TableRange _specimenIDRange;

        /// <summary>
        /// The last column index of the table.
        /// </summary>
        private int _lastIndex;

        public int LastIndex
        {
            get { return _lastIndex; }
            set { _lastIndex = value; }
        }

        public DataTableWithRanges(DataTable myTable, TableRange mySpecimenIDRange, TableRange myMetaDataRange, TableRange myFlawSizeRange, TableRange myResponseDataRange)
        {
            _table = myTable;
            _specimenIDRange = mySpecimenIDRange;
            _metaDataRange = myMetaDataRange;
            _flawSizeRange = myFlawSizeRange;
            _responseRange = myResponseDataRange;
        }

        internal Dictionary<string, TableRangePair> CreatePairs(DataTableWithRanges myFromTable)
        {
            TableRangePair specIDPair = new TableRangePair(myFromTable._specimenIDRange, _specimenIDRange);
            TableRangePair metaDataPair = new TableRangePair(myFromTable._metaDataRange, _metaDataRange);
            TableRangePair flawSizePair = new TableRangePair(myFromTable._flawSizeRange, _flawSizeRange);
            TableRangePair responsePair = new TableRangePair(myFromTable._responseRange, _responseRange);

            var pairs = new Dictionary<string, TableRangePair>();

            pairs.Add(_specimenIDRange.Name, specIDPair);
            pairs.Add(_metaDataRange.Name, metaDataPair);
            pairs.Add(_flawSizeRange.Name, flawSizePair);
            pairs.Add(_responseRange.Name, responsePair);

            return pairs;
        }

        public DataTable Table
        {
            get
            {
                return _table;
            }

        }

        internal void ResetLastIndex()
        {
            _lastIndex = 0;
        }

        public TableRange SpecimenIDRange
        {
            get
            {
                return _specimenIDRange;
            }
        }

        public TableRange MetaDataRange
        {
            get
            {
                return _metaDataRange;
            }
        }

        public TableRange GetRange(ColType myType)
        {
            switch(myType)
            {
                case ColType.Flaw:
                    return FlawSizeRange;
                case ColType.ID:
                    return SpecimenIDRange;
                case ColType.Meta:
                    return MetaDataRange;
                case ColType.Response:
                    return ResponseRange;
                default:
                    throw new Exception("Column Type " + myType.ToString() + " not supported.");                        
            }
        }

        public TableRange FlawSizeRange
        {
            get
            {
                return _flawSizeRange;
            }
        }

        public TableRange ResponseRange
        {
            get
            {
                return _responseRange;
            }
        }
    }
}
