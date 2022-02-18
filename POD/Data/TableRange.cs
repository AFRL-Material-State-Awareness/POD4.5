using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using POD.ExcelData;

namespace POD.Data
{
    /// <summary>
    /// Holds a range of column indicies for a table.
    /// </summary>
    [Serializable]
    public class TableRange
    {
        ///TODO: Add ability to define an arbitrary set of non-consecutive column indicies rather than one generated from parameters

        #region Fields
        /// <summary>
        /// The first column's index for the range relative to the source table.
        /// </summary>
        private int _startIndex;
        /// <summary>
        /// The number of indices in a range.
        /// </summary>
        private int _count;
        /// <summary>
        /// A list of inidices that are found in the range.
        /// </summary>
        private List<int> _list;
        /// <summary>
        /// The data type for the columns found in the range. One type per range.
        /// </summary>
        private Type _dataType;
        /// <summary>
        /// When creating list, this specifies how index should be incremented. (ie every ith column)
        /// </summary>
        private int _increment;
        /// <summary>
        /// If the Count is unknown, this is the max index value that can be generated for the list. 
        /// </summary>
        private int _maxIndex;
        /// <summary>
        /// Name of the kind of data specified by the table range. Should be a unique name.
        /// </summary>
        private string _name;  
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new Table Range with no indicies.
        /// </summary>
        public TableRange()
        {
            InitializeEmptyRange();

            _name = "";
        }        
        /// <summary>
        /// Create a new Table Range with no indicies.
        /// </summary>
        /// <param name="myName"></param>
        public TableRange(string myName)
        {
            InitializeEmptyRange();

            _name = myName;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Get/set the number of indicies contained in the range.
        /// </summary>
        public int Count
        {
            get
            {
                return _count;
            }
            set
            {
                _count = value;

                //UpdateList();
            }

        }
        /// <summary>
        /// The data type of the columns in the range.
        /// </summary>
        public Type DataType
        {
            get { return _dataType; }
            set { _dataType = value; }
        }        
        /// <summary>
        /// How to increment the index when generating a list of indicies.
        /// </summary>
        public int Increment
        {
            get
            {
                return _increment;
            }
            set
            {
                if (value < 1)
                {
                    _increment = 1;
                }
                else
                {
                    _increment = value;
                }

                //UpdateList();
            }
        }
        /// <summary>
        /// Is the number of indicies unknown at this time?
        /// </summary>
        public bool IsCountUnknown
        {
            get
            {
                return _count == -1;
            }
        }
        /// <summary>
        /// Is the max possible index unknown at this time?
        /// </summary>
        public bool IsMaxIndexUnknown
        {
            get
            {
                return _maxIndex == -1;
            }
        }
        /// <summary>
        /// Is the max index of incidies that can be generated known?
        /// </summary>
        public int MaxIndex
        {
            get
            {
                return _maxIndex;
            }
            set
            {
                _maxIndex = value;
                //UpdateList();
            }
        }
        /// <summary>
        /// Name of the kind of data specified by the table range. Should be a unique name.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value; 
            }
        }
        /// <summary>
        /// A list of inidicies found in the range.
        /// </summary>
        public List<int> Range
        {
            get
            {
                return _list;
            }
            set
            {
                _list = value;
            }
        }
        /// <summary>
        /// The index of the first column in the range relative to the source table.
        /// </summary>
        public int StartIndex
        {
            get
            {
                return _startIndex;
            }
            set
            {
                _startIndex = value;

                //UpdateList();
            }

        }
        #endregion

        #region Methods
        /*/// <summary>
        /// Let the Table Range know Max Index or Count needs to be defined before it can generate a Range list.
        /// </summary>
        public void CountTillEndOfTable()
        {
            Count = -1;
            MaxIndex = -1;
        }*/
        /// <summary>
        /// Create an empty list with increment of 1 and count, max and start of -1.
        /// </summary>
        private void InitializeEmptyRange()
        {
            _list = new List<int>();
            _increment = 1;
            _count = -1;
            _maxIndex = -1;
            _startIndex = 0;
        }
        /*/// <summary>
        /// Generate a new Range list based on the current settings.
        /// </summary>
        private void UpdateList()
        {
            int index = 0;

            _list.Clear();
            
            //can't generate an index list if length is undefined
            if (IsCountUnknown && IsMaxIndexUnknown)
                return;

            //if Count unknown but Max Index is then go until Max Index is reached
            //else add indicies based on Count
            if (IsCountUnknown && !IsMaxIndexUnknown)
            {
                int nextIndex = -1;
                int i = 0;

                while(nextIndex <= MaxIndex)
                {
                    index = (i * Increment) + StartIndex;
                    _list.Add(index);
                    i++;
                    nextIndex = (i * Increment) + StartIndex;
                }
            }
            else
            {
                for (int i = 0; i < Count; i++)
                {
                    index = (i * Increment) + StartIndex;
                    _list.Add(index);
                }
            }

            _count = _list.Count;
            
        }*/
        #endregion

        #region Event Handling
        #endregion

        internal void WriteToExcel(ExcelExport myWriter, ref int myRowIndex)
        {
            int startRowIndex = myRowIndex;
            int colIndex = 1;

            myWriter.SetCellValue(myRowIndex++, colIndex, _name + " StartIndex");
            myWriter.SetCellValue(myRowIndex++, colIndex, _name + " Count");
            myWriter.SetCellValue(myRowIndex++, colIndex, _name + " Data Type");
            myWriter.SetCellValue(myRowIndex++, colIndex, _name + " Increment");
            myWriter.SetCellValue(myRowIndex++, colIndex, _name + " Max Index");

            myRowIndex = startRowIndex;
            colIndex++;

            myWriter.SetCellValue(myRowIndex++, colIndex, _startIndex);
            myWriter.SetCellValue(myRowIndex++, colIndex, _count);
            myWriter.SetCellValue(myRowIndex++, colIndex, _dataType.ToString());
            myWriter.SetCellValue(myRowIndex++, colIndex, _increment);
            myWriter.SetCellValue(myRowIndex++, colIndex, _maxIndex);

            myWriter.Workbook.AutoFitColumn(1, 2);
        }
    }
}
