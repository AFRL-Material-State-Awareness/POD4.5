using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using POD.ExcelData;
using System.Drawing;

namespace POD.Data
{
    /// <summary>
    /// This holds all of the data that will be available to the analyses from one source.
    /// A project can hold more than one source but an anlaysis can only feature data from one source. 
    /// </summary>
    [Serializable]
    public class DataSource : IDataSource
    {
        #region Fields
        /// <summary>
        /// Index of default flaw size column relative to flaw size table range.
        /// </summary>
        private int _defaultFlawSizeIndex;

        /// <summary>
        /// Index of default specimen ID column relative to specimen ID table range.
        /// </summary>
        private int _defaultIDIndex;

        /// <summary>
        /// The data table holding the data with the noise information. Will have lots of NaN values.
        /// </summary>
        private DataTable _noiseTable;

        /// <summary>
        /// Template used to define the Table Ranges being passed in.
        /// </summary>
        private ImportTemplate _template;

        private AnalysisDataTypeEnum _analysisDataType;

        DataTableWithRanges _processed;
        DataTableWithRanges _original;



        /// <summary>
        /// name of the data source
        /// </summary>
        private string _name;
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new data source from data table with a set of defined table ranges.
        /// </summary>
        /// <param name="myTable">the data table that holds all of the data</param>
        /// <param name="mySpecimenIDRange">range of columns that hold Specimen ID data</param>
        /// <param name="myMetaDataRange">range of columns that hold meta-data</param>
        /// <param name="myFlawSizeRange">range of columns that hold flaw size data</param>
        /// <param name="myResponseDataRange">range of columns that hold response data</param>
        public DataSource(DataTable myTable, TableRange mySpecimenIDRange, TableRange myMetaDataRange,
                          TableRange myFlawSizeRange, TableRange myResponseDataRange)
        {
            _name = myTable.TableName;

            var processedTable = new DataTable();
            _noiseTable = new DataTable();
            _original = new DataTableWithRanges(myTable, mySpecimenIDRange, myMetaDataRange,
                                                     myFlawSizeRange, myResponseDataRange);

            var specimenIDRange = new TableRange(RangeNames.SpecID);
            var metaDataRange = new TableRange(RangeNames.MetaData);
            var flawSizeRange = new TableRange(RangeNames.FlawSize);
            var responseRange = new TableRange(RangeNames.Response);

            _processed = new DataTableWithRanges(processedTable, specimenIDRange, metaDataRange,
                                                      flawSizeRange, responseRange);

            Dictionary<string, TableRangePair> pairs = _processed.CreatePairs(_original);

            _processed.ResetLastIndex();
            ProcessColumnRange(myTable, pairs[RangeNames.SpecID], typeof(string));
            ProcessColumnRange(myTable, pairs[RangeNames.MetaData], typeof(string));
            ProcessColumnRange(myTable, pairs[RangeNames.FlawSize], typeof(double));
            ProcessColumnRange(myTable, pairs[RangeNames.Response], typeof(double));
            _processed.ResetLastIndex();

            ProcessRows(myTable, pairs.Values.ToList());

            //default to the first column in each range
            _defaultFlawSizeIndex = 0;
            _defaultIDIndex = 0;

            _analysisDataType = DecideAnalysisType(GetData(ResponseLabels));

            CleanUpZeroSizedFlaws();
        }

        public DataSource(string mySourceName, string myIdName, string myFlawName, string myResponseName)
        {
            _name = mySourceName;
            SourceName = mySourceName;

            var myTable = new DataTable();

            myTable.Columns.Add(new DataColumn(myIdName, typeof(string)));
            myTable.Columns.Add(new DataColumn(myFlawName, typeof(double)));
            myTable.Columns.Add(new DataColumn(myResponseName, typeof(double)));

            var mySpecimenIDRange = new TableRange(RangeNames.SpecID);
            var myMetaDataRange = new TableRange(RangeNames.MetaData);
            var myFlawSizeRange = new TableRange(RangeNames.FlawSize);
            var myResponseDataRange = new TableRange(RangeNames.Response);

            mySpecimenIDRange.Count = 1;
            mySpecimenIDRange.DataType = typeof(string);
            mySpecimenIDRange.StartIndex = 0;
            mySpecimenIDRange.MaxIndex = 0;
            mySpecimenIDRange.Range = new List<int>();
            mySpecimenIDRange.Range.Add(0);

            myFlawSizeRange.Count = 1;
            myFlawSizeRange.DataType = typeof(double);
            myFlawSizeRange.StartIndex = 1;
            myFlawSizeRange.MaxIndex = 1;
            myFlawSizeRange.Range = new List<int>();
            myFlawSizeRange.Range.Add(1);

            myResponseDataRange.Count = 1;
            myResponseDataRange.DataType = typeof(double);
            myResponseDataRange.StartIndex = 2;
            myResponseDataRange.MaxIndex = 2;
            myResponseDataRange.Range = new List<int>();
            myResponseDataRange.Range.Add(2);

            var processedTable = new DataTable();
            _noiseTable = new DataTable();
            _original = new DataTableWithRanges(myTable, mySpecimenIDRange, myMetaDataRange,
                                                     myFlawSizeRange, myResponseDataRange);

            var specimenIDRange = new TableRange(RangeNames.SpecID);
            var metaDataRange = new TableRange(RangeNames.MetaData);
            var flawSizeRange = new TableRange(RangeNames.FlawSize);
            var responseRange = new TableRange(RangeNames.Response);

            _processed = new DataTableWithRanges(processedTable, specimenIDRange, metaDataRange,
                                                      flawSizeRange, responseRange);

            Dictionary<string, TableRangePair> pairs = _processed.CreatePairs(_original);

            _processed.ResetLastIndex();
            ProcessColumnRange(myTable, pairs[RangeNames.SpecID], typeof(string));
            ProcessColumnRange(myTable, pairs[RangeNames.MetaData], typeof(string));
            ProcessColumnRange(myTable, pairs[RangeNames.FlawSize], typeof(double));
            ProcessColumnRange(myTable, pairs[RangeNames.Response], typeof(double));
            _processed.ResetLastIndex();

            ProcessRows(myTable, pairs.Values.ToList());

            //default to the first column in each range
            _defaultFlawSizeIndex = 0;
            _defaultIDIndex = 0;

            _analysisDataType = DecideAnalysisType(GetData(ResponseLabels));

            CleanUpZeroSizedFlaws();

        }

        private void CleanUpZeroSizedFlaws()
        {
            int index = _processed.FlawSizeRange.StartIndex;

            //if you can check the flaw size range
            if (_processed.Table.Columns.Count > index)
            {
                for (int i = 0; i < _processed.Table.Rows.Count; i++)
                {
                    DataRow row = _processed.Table.Rows[i];

                    double flaw = Convert.ToDouble(row[index]);

                    if (flaw == 0.0)
                    {
                        DataRow newRow = _noiseTable.Rows.Add();

                        foreach (DataColumn col in _processed.Table.Columns)
                        {
                            newRow[col.ColumnName] = row[col.ColumnName];
                        }

                        _processed.Table.Rows.Remove(row);

                        i--;
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Get a list of column indicies for the ID columns for the original data table.
        /// </summary>
        public List<int> IDColumnRange
        {
            get
            {
                return _original.SpecimenIDRange.Range;
            }
        }

        /// <summary>
        /// Get a list of column indicies for the meta data columns for the original data table.
        /// </summary>
        public List<int> MetaDataColumnRange
        {
            get
            {
                return _original.MetaDataRange.Range;
            }
        }

        /// <summary>
        /// Get a list of column indicies for the flaw columns for the original data table.
        /// </summary>
        public List<int> FlawColumnRange
        {
            get
            {
                return _original.FlawSizeRange.Range;
            }
        }

        /// <summary>
        /// Get a list of column indicies for the data columns for the original data table.
        /// </summary>
        public List<int> DataColumnRange
        {
            get
            {
                return _original.ResponseRange.Range;
            }
        }



        #region Properties
        /// <summary>
        /// Get the analysis type based on the values of the data table.
        /// </summary>
        public AnalysisDataTypeEnum AnalysisDataType
        {
            get
            {
                return _analysisDataType;
            }
        }
        /// <summary>
        /// Get the number of Response Data columns.
        /// </summary>
        public int DataCount
        {
            get
            {
                return _processed.ResponseRange.Count;
            }
        }

        /// <summary>
        /// Get the Response Data labels.
        /// </summary>
        public List<string> ResponseLabels
        {
            get
            {
                return GetLabels(_processed.ResponseRange);
            }
        }

        /// <summary>
        /// Get the Response original labels.
        /// </summary>
        public List<string> Originals(ColType myType)
        {
            return GetStringProperty(GetRange(myType), ExtColProperty.Original);
        }

        private TableRange GetRange(ColType myType)
        {
            switch (myType)
            {
                case ColType.Flaw:
                    return _processed.FlawSizeRange;
                case ColType.Response:
                    return _processed.ResponseRange;
                case ColType.Meta:
                    return _processed.MetaDataRange;
                case ColType.ID:
                    return _processed.SpecimenIDRange;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Get the Response Data units.
        /// </summary>
        public List<string> Units(ColType myType)
        {
            return GetStringProperty(GetRange(myType), ExtColProperty.Unit);
        }

        /// <summary>
        /// Get the Response Data units.
        /// </summary>
        public List<double> Maximums(ColType myType)
        {
            return GetDoubleProperty(GetRange(myType), ExtColProperty.Max);
        }

        /// <summary>
        /// Get the Response Data units.
        /// </summary>
        public List<double> PreviousMaximums(ColType myType)
        {
            return GetDoubleProperty(GetRange(myType), ExtColProperty.MaxPrev);
        }

        /// <summary>
        /// Get the Response Data units.
        /// </summary>
        public List<double> Minimums(ColType myType)
        {
            return GetDoubleProperty(GetRange(myType), ExtColProperty.Min);
        }

        /// <summary>
        /// Get the Response Data units.
        /// </summary>
        public List<double> PreviousMinimums(ColType myType)
        {
            return GetDoubleProperty(GetRange(myType), ExtColProperty.MinPrev);
        }

        /// <summary>
        /// Get the Response Data units.
        /// </summary>
        public List<double> Thresholds(ColType myType)
        {
            return GetDoubleProperty(GetRange(myType), ExtColProperty.Thresh);
        }

        /// <summary>
        /// Get the Response Data units.
        /// </summary>
        public List<double> PreviousThresholds(ColType myType)
        {
            return GetDoubleProperty(GetRange(myType), ExtColProperty.ThreshPrev);
        }

        /// <summary>
        /// Get list of column infos based on current columns in the processed table.
        /// </summary>
        /// <param name="myType"></param>
        /// <returns></returns>
        public List<ColumnInfo> ColumnInfos(ColType myType)
        {

            var list = new List<ColumnInfo>();

            var names = GetLabels(GetRange(myType));
            var originals = Originals(myType);
            var units = Units(myType);
            var mins = Minimums(myType);
            var maxes = Maximums(myType);
            var threshs = Thresholds(myType);

            for (int i = 0; i < names.Count; i++)
            {
                list.Add(new ColumnInfo(originals[i], names[i], units[i], mins[i], maxes[i], threshs[i], i));
            }

            return list;
        }


        /// <summary>
        /// Get/set the default Flaw Size Data column index. Relative to the Flaw Size Range. This is the x-axis data for a plot.
        /// </summary>
        public int DefaultFlawSizeIndex
        {
            get { return _defaultFlawSizeIndex; }
            set { _defaultFlawSizeIndex = value; }
        }

        /// <summary>
        /// Get/set the default Specimen ID column index. Relative to the Specimen ID Range.
        /// </summary>
        public int DefaultIDIndex
        {
            get { return _defaultIDIndex; }
            set { _defaultIDIndex = value; }
        }

        /// <summary>
        /// Get the Flaw labels.
        /// </summary>
        public List<string> FlawLabels
        {
            get
            {
                return GetLabels(_processed.FlawSizeRange);
            }
        }

        /// <summary>
        /// Get the Specimen ID labels.
        /// </summary>
        public List<string> IDLabels
        {
            get
            {
                return GetLabels(_processed.SpecimenIDRange);
            }
        }



        /// <summary>
        /// Get the meta-data labels.
        /// </summary>
        public List<string> MetaDataLabels
        {
            get
            {
                return GetLabels(_processed.MetaDataRange);
            }
        }

        /// <summary>
        /// Name of the data source.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        /// <summary>
        /// Get/set the formatting template used to create the Table Ranges.
        /// </summary>
        public ImportTemplate Template
        {
            get { return _template; }
            set { _template = value; }
        }

        /// <summary>
        /// The data table that stores all of the data. This is the table after it's been processed and
        /// not the original data table. Only what was specified by the Table Ranges is included here.
        /// </summary>
        public DataTableWithRanges Source
        {
            get
            {
                return _processed;
            }
        }

        public string SourceName { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Copies values from a Data Row in to an Object array based Table Range parameters.
        /// </summary>
        /// <param name="myObjArray">array to copy values to</param>
        /// <param name="myRow">data row to copy values from</param>
        /// <param name="myPair">table range pair that defines how to get the data and where to put it</param>
        /// <returns>Returns if the object array had values that should've been converted to double but couldn't be</returns>
        private bool CopyValuesFromRange(object[] myObjArray, DataRow myRow, TableRangePair myPair)
        {
            int next = 0;
            double value;
            bool hasInvalidFlaw = false;
            var colIndex = 0;
            var valueStr = "";

            if (myPair.To.DataType != typeof(double))
            {
                foreach (int index in myPair.From.Range)
                {
                    myObjArray[myPair.To.StartIndex + next] = Convert.ChangeType(myRow[index], myPair.From.DataType);
                    next++;
                }
            }
            else
            {
                foreach (int index in myPair.From.Range)
                {
                    colIndex = myPair.To.StartIndex + next;
                    valueStr = myRow[index].ToString();

                    if (Double.TryParse(valueStr, out value) == true)
                    {
                        myObjArray[colIndex] = value;

                        if (myPair.To.Name == RangeNames.FlawSize)
                        {
                            //can't have flaw sizes equal to 0.0 in an analysis
                            if (value <= 0.0)
                                hasInvalidFlaw = true;
                        }
                    }
                    else if (myPair.To.Name == RangeNames.FlawSize)
                    {
                        myObjArray[colIndex] = double.NaN;
                        hasInvalidFlaw = true;
                    }
                    //FLAW SIZE MUST ALWAYS BE PROCESSED FIRST
                    else if (myPair.To.Name == RangeNames.Response)
                    {

                        if (hasInvalidFlaw == false)
                        {
                            myObjArray[colIndex] = Double.NaN;
                        }
                        else
                        {
                            myObjArray[colIndex] = valueStr;
                        }

                    }

                    next++;
                }
            }

            return hasInvalidFlaw;
        }



        /// <summary>
        /// Figures it the data table is for hit/miss or aHat vs a.
        /// </summary>
        /// <returns>which type it is</returns>
        static public AnalysisDataTypeEnum DecideAnalysisType(DataTable table)
        {
            double value;
            bool foundAHat = false;


            foreach (DataColumn column in table.Columns)
            {
                DataTable distinct = table.DefaultView.ToTable(true, column.ColumnName);

                if (distinct.Rows.Count < 4)
                {
                    foreach (DataRow row in distinct.Rows)
                    {
                        value = Convert.ToDouble(row[0]);

                        if (value != 1.0 && value != 0.0 && !Double.IsNaN(value))
                        {
                            foundAHat = true;

                            //once it is recognized as aHat vs a it can't be hit/miss
                            break;
                        }
                    }
                }
                else
                {
                    foundAHat = true;
                    break;
                }
            }

            if (foundAHat == false)
            {
                return AnalysisDataTypeEnum.HitMiss;
            }
            else
            {

                return AnalysisDataTypeEnum.AHat;
            }

        }

        /// <summary>
        /// Get all columns of data found in the SPecimen ID range.
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllSpecimenData()
        {
            DataTable table = new DataTable();

            if (_processed.Table.Columns.Count > 0)
            {
                table = _processed.Table.DefaultView.ToTable(false, IDLabels.ToArray());
            }

            return table;
        }

        /// <summary>
        /// Get column data of any column speicified in the list.
        /// </summary>
        /// <param name="name">name of column</param>
        /// <returns>a data table with the data from the specified column names</returns>
        public DataTable GetData(string names)
        {
            var list = new List<String>();

            list.Add(names);

            return GetData(list);
        }

        /// <summary>
        /// Get column data of any column speicified in the list.
        /// </summary>
        /// <param name="names">list of names of columns</param>
        /// <returns>a data table with the data from the specified column names</returns>
        public DataTable GetData(List<string> names)
        {
            DataTable table = new DataTable();

            table = _processed.Table.DefaultView.ToTable(false, names.ToArray());

            return table;
        }

        /// <summary>
        /// Get a table with all the data needed to plot a specified Response column.
        /// </summary>
        /// <param name="myYIndex">the range relative index of the Response column</param>
        /// <returns>a two column table [x-axis default Flaw Size data, y-axis Response data]</returns>
        public DataTable GetGraphData(int myYIndex)
        {
            return GetGraphData(_defaultFlawSizeIndex, myYIndex);
        }

        /// <summary>
        /// Get a table with all the data needed to plot a specified Response column.
        /// </summary>
        /// <param name="myXIndex">the range relative index of the Flaw Size column</param>
        /// <param name="myYIndex">the range relative index of the Response column</param>
        /// <returns></returns>
        public DataTable GetGraphData(int myXIndex, int myYIndex)
        {
            DataTable table = new DataTable();

            table = _processed.Table.DefaultView.ToTable(false, _processed.Table.Columns[_processed.FlawSizeRange.StartIndex + myXIndex].ColumnName,
                                                              _processed.Table.Columns[_processed.ResponseRange.StartIndex + myYIndex].ColumnName);

            return table;
        }

        /// <summary>
        /// Get a table with all the data needed to plot multiple specified Response column.
        /// </summary>
        /// <param name="names">names of the Response columns</param>
        /// <returns></returns>
        public DataTable GetGraphData(string flawName, List<string> names)
        {
            List<string> allNames = names.ToList();
            allNames.Insert(0, flawName);
            DataTable table = new DataTable();

            table = _processed.Table.DefaultView.ToTable(false, allNames.ToArray());

            return table;
        }

        /// <summary>
        /// Get a table with all the data needed to plot multiple specified Response column.
        /// </summary>
        /// <param name="names">names of the Response columns</param>
        /// <returns></returns>
        public DataTable GetGraphData(List<string> names)
        {
            List<string> allNames = names.ToList();
            allNames.Insert(0, _processed.Table.Columns[_processed.FlawSizeRange.StartIndex + _defaultFlawSizeIndex].ColumnName);
            DataTable table = new DataTable();

            table = _processed.Table.DefaultView.ToTable(false, allNames.ToArray());

            return table;
        }

        /// <summary>
        /// Get the names of columns found in a specified range.
        /// </summary>
        /// <param name="myRange">the specified range</param>
        /// <returns></returns>
        private List<string> GetLabels(TableRange myRange)
        {
            List<string> labels = new List<string>();

            foreach (int index in myRange.Range)
            {
                labels.Add(_processed.Table.Columns[index].ColumnName);
            }

            return labels;
        }

        /// <summary>
        /// Get the names of columns found in a specified range.
        /// </summary>
        /// <param name="myRange">the specified range</param>
        /// <returns></returns>
        private List<string> GetStringProperty(TableRange myRange, string myLabel)
        {
            List<string> labels = new List<string>();

            foreach (int index in myRange.Range)
            {
                labels.Add(_processed.Table.Columns[index].ExtendedProperties[myLabel].ToString());
            }

            return labels;
        }


        /// <summary>
        /// Get the names of columns found in a specified range.
        /// </summary>
        /// <param name="myRange">the specified range</param>
        /// <returns></returns>
        private List<double> GetDoubleProperty(TableRange myRange, string myLabel)
        {
            var values = new List<double>();

            foreach (int index in myRange.Range)
            {
                values.Add(Convert.ToDouble(_processed.Table.Columns[index].ExtendedProperties[myLabel].ToString()));
            }

            return values;
        }

        /// <summary>
        /// Get a table of all of the Specimen ID columns data.
        /// </summary>
        /// <returns>a data table with all Specimen ID columns data</returns>
        public DataTable GetSpecimenData()
        {
            DataTable table = new DataTable();

            if (_processed.Table.Columns.Count > 0)
            {
                table = _processed.Table.DefaultView.ToTable(false, IDLabels.ToArray());//_processed.Table.Columns[_processed.SpecimenIDRange.StartIndex + _defaultIDIndex].ColumnName);
            }

            if (table.Columns.Count > 1)
            {
                DataTable concat = new DataTable();

                concat.Columns.Add(new DataColumn("ID", typeof(String)));

                foreach (DataRow row in table.Rows)
                {
                    var newRow = concat.NewRow();

                    foreach (DataColumn column in table.Columns)
                    {
                        newRow[0] += row[column.ColumnName].ToString() + " - ";
                    }

                    var name = newRow[0].ToString();

                    newRow[0] = name.Remove(name.Length - 3);

                    concat.Rows.Add(newRow);
                }

                return concat;
            }

            return table;
        }

        /// <summary>
        /// Get all the column information needed to the To Range based on the From Range parameters and the Data Table.
        /// </summary>
        /// <param name="myTable">the table that will be imported</param>
        /// <param name="myPair">holds tot he To and From Ranges</param>
        /// <param name="myType">the data type to import the data as for the To Range</param>
        private void ProcessColumnRange(DataTable myTable, TableRangePair myPair, Type myType)
        {
            myPair.From.DataType = myType;

            if (myPair.From.IsCountUnknown)
            {
                myPair.From.MaxIndex = myTable.Columns.Count - 1;
            }

            myPair.To.StartIndex = _processed.LastIndex;

            foreach (int index in myPair.From.Range)
            {
                var col = _processed.Table.Columns.Add(myTable.Columns[index].ColumnName, myType);
                col.ExtendedProperties.Add(ExtColProperty.Unit, ExtColProperty.UnitDefault);
                col.ExtendedProperties.Add(ExtColProperty.Max, ExtColProperty.MaxDefault);
                col.ExtendedProperties.Add(ExtColProperty.Min, ExtColProperty.MinDefault);
                col.ExtendedProperties.Add(ExtColProperty.Thresh, ExtColProperty.ThreshDefault);
                col.ExtendedProperties.Add(ExtColProperty.MaxPrev, ExtColProperty.MaxDefault);
                col.ExtendedProperties.Add(ExtColProperty.MinPrev, ExtColProperty.MinDefault);
                col.ExtendedProperties.Add(ExtColProperty.ThreshPrev, ExtColProperty.ThreshDefault);
                col.ExtendedProperties.Add(ExtColProperty.Original, ExtColProperty.OriginalDefault(col));
                var noise = _noiseTable.Columns.Add(myTable.Columns[index].ColumnName, myType);
                noise.ExtendedProperties.Add(ExtColProperty.Unit, ExtColProperty.UnitDefault);
                noise.ExtendedProperties.Add(ExtColProperty.Max, ExtColProperty.MaxDefault);
                noise.ExtendedProperties.Add(ExtColProperty.Min, ExtColProperty.MinDefault);
                noise.ExtendedProperties.Add(ExtColProperty.Thresh, ExtColProperty.ThreshDefault);
                noise.ExtendedProperties.Add(ExtColProperty.MaxPrev, ExtColProperty.MaxDefault);
                noise.ExtendedProperties.Add(ExtColProperty.MinPrev, ExtColProperty.MinDefault);
                noise.ExtendedProperties.Add(ExtColProperty.ThreshPrev, ExtColProperty.ThreshDefault);
                noise.ExtendedProperties.Add(ExtColProperty.Original, ExtColProperty.OriginalDefault(col));
            }

            //update count since we know the range now
            myPair.From.Count = myPair.From.Range.Count;
            myPair.To.Count = myPair.From.Range.Count;
            myPair.To.MaxIndex = myPair.To.StartIndex + (myPair.To.Count - 1);

            List<int> toRange = new List<int>();
            for (int i = myPair.To.StartIndex; i <= myPair.To.MaxIndex; i++)
            {
                toRange.Add(i);
            }
            myPair.To.Range = toRange;

            myPair.To.DataType = myType;
            myPair.To.Increment = 1;

            _processed.LastIndex += myPair.From.Count;
        }

        /// <summary>
        /// Copy and Process all the data rows in the table that's being imported into the Source Table.
        /// </summary>
        /// <param name="myTable">the data table to copy data rows from</param>
        /// <param name="myRanges">the To and From Ranges that specify where to get data from and where to put it</param>
        private void ProcessRows(DataTable myTable, List<TableRangePair> myRanges)
        {
            bool hasNaN = false; //determine if row had NaN values

            for (int i = 0; i < myTable.Rows.Count; i++)
            {
                Object[] objs = new Object[_processed.Table.Columns.Count];

                DataRow row = myTable.Rows[i];

                hasNaN = false;

                foreach (TableRangePair pair in myRanges)
                {
                    hasNaN |= CopyValuesFromRange(objs, row, pair);
                }

                if (hasNaN == false)
                    _processed.Table.Rows.Add(objs);
                else
                    _noiseTable.Rows.Add(objs);
            }
        }
        #endregion

        #region Event Handling
        #endregion


        public void WriteDataToExcel(ExcelExport myWriter)
        {
            myWriter.Workbook.AddWorksheet(SourceName);

            int colIndex = 1;

            List<TableRange> ranges = new List<TableRange>();

            ranges.Add(_processed.SpecimenIDRange);
            ranges.Add(_processed.MetaDataRange);
            ranges.Add(_processed.FlawSizeRange);
            ranges.Add(_processed.ResponseRange);

            foreach (TableRange range in ranges)
            {
                WriteRange(myWriter, range, colIndex);

                colIndex = colIndex + range.Count;
            }


        }

        private void WriteRange(ExcelExport myWriter, TableRange myRange, int myColumnIndex)
        {
            List<string> names = GetLabels(myRange);

            //if range has any columns to write
            if (names.Count > 0)
            {
                DataTable table = _processed.Table.DefaultView.ToTable(false, names.ToArray());

                int colInc = 0;

                foreach (DataColumn col in table.Columns)
                {
                    int rowIndex = 2;
                    int colIndex = myColumnIndex + colInc;

                    myWriter.SetCellValue(1, colIndex, col.ColumnName);

                    //write double or string values to Excel
                    if (myRange.DataType == typeof(Double))
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            myWriter.SetCellValue(rowIndex++, colIndex, Convert.ToDouble(row[colInc]));
                        }
                    }
                    else
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            myWriter.SetCellValue(rowIndex++, colIndex, row[colInc].ToString());
                        }
                    }

                    colInc++;
                }

                myWriter.Workbook.AutoFitColumn(1, myColumnIndex + colInc - 1);
            }


        }

        public DataTable Original
        {
            get
            {
                if (_original != null)
                    return _original.Table;
                else
                    return null;
            }
        }

        internal void WritePropertiesToExcel(ExcelExport myWriter)
        {
            myWriter.Workbook.AddWorksheet(SourceName + " Properties");

            int rowIndex = 1;

            List<TableRange> ranges = new List<TableRange>();

            ranges.Add(_processed.SpecimenIDRange);
            ranges.Add(_processed.MetaDataRange);
            ranges.Add(_processed.FlawSizeRange);
            ranges.Add(_processed.ResponseRange);

            foreach (TableRange range in ranges)
            {
                range.WriteToExcel(myWriter, ref rowIndex);
            }
        }

        /// <summary>
        /// Get a list of dataviews for every possible flaw, response combination. ({f1,r1}, {f1,r2},...,{f2,r1},...)
        /// </summary>
        /// <returns></returns>
        public List<DataView> GetAllGraphData()
        {
            var views = new List<DataView>();

            var temp = _defaultFlawSizeIndex;

            for (int i = 0; i < FlawLabels.Count; i++)
            {
                _defaultFlawSizeIndex = i;

                for (int j = 0; j < DataCount; j++)
                {
                    views.Add(GetGraphData(j).AsDataView());
                }
            }

            _defaultFlawSizeIndex = temp;

            return views;
        }

        public List<PODListBoxItemWithProps> Flaws
        {
            get
            {
                var list = new List<PODListBoxItemWithProps>();

                foreach (int index in _processed.FlawSizeRange.Range)
                {
                    var name = _processed.Table.Columns[index].ColumnName;
                    var originalName = _processed.Table.Columns[index].ExtendedProperties[ExtColProperty.Original].ToString();
                    var units = _processed.Table.Columns[index].ExtendedProperties[ExtColProperty.Unit].ToString();
                    var min = Convert.ToDouble(_processed.Table.Columns[index].ExtendedProperties[ExtColProperty.Min].ToString());
                    var max = Convert.ToDouble(_processed.Table.Columns[index].ExtendedProperties[ExtColProperty.Max].ToString());


                    var item = new PODListBoxItemWithProps(Color.Black, ColType.Flaw, name, originalName, SourceName, units, min, max, 0.0);

                    list.Add(item);
                }

                return list;
            }
        }

        public List<PODListBoxItemWithProps> Responses
        {
            get
            {
                var list = new List<PODListBoxItemWithProps>();

                foreach (int index in _processed.ResponseRange.Range)
                {
                    var name = _processed.Table.Columns[index].ColumnName;
                    var originalName = _processed.Table.Columns[index].ExtendedProperties[ExtColProperty.Original].ToString();
                    var units = _processed.Table.Columns[index].ExtendedProperties[ExtColProperty.Unit].ToString();
                    var min = Convert.ToDouble(_processed.Table.Columns[index].ExtendedProperties[ExtColProperty.Min].ToString());
                    var max = Convert.ToDouble(_processed.Table.Columns[index].ExtendedProperties[ExtColProperty.Max].ToString());
                    var threshold = Convert.ToDouble(_processed.Table.Columns[index].ExtendedProperties[ExtColProperty.Thresh].ToString());

                    var item = new PODListBoxItemWithProps(Color.Black, ColType.Flaw, name, originalName, SourceName, units, min, max, threshold);

                    list.Add(item);
                }

                return list;
            }
        }

        public string SetName(ColType myType, int myIndex, string myNewName)
        {
            var range = _processed.GetRange(myType);

            return SetNewName(GetLabels(range), range, myIndex, myNewName);
        }

        public string SetUnit(ColType myType, int myIndex, string myNewUnit)
        {
            var range = _processed.GetRange(myType);

            return SetNewStringProperty(range, ExtColProperty.Unit, myIndex, myNewUnit);
        }

        private string SetNewStringProperty(TableRange myRange, string myProperty, int myIndex, string myNewUnit)
        {
            if (myNewUnit == null)
                myNewUnit = "";

            myNewUnit = myNewUnit.Trim();

            _processed.Table.Columns[myIndex + myRange.StartIndex].ExtendedProperties[myProperty] = myNewUnit;

            return myNewUnit;
        }

        private string SetNewName(List<string> currentNames, TableRange myRange, int myIndex, string myNewName)
        {
            var oldName = currentNames[myIndex];


            if (oldName != myNewName)
            {

                //don't want to check for the name we are changing
                currentNames.RemoveAt(myIndex);

                while (currentNames.Contains(myNewName))
                    myNewName = myNewName + " Copy";

                _processed.Table.Columns[myIndex + myRange.StartIndex].ColumnName = myNewName;
            }

            return myNewName;
        }

        public double SetMinimum(int myIndex, ColType myType, double myNewValue)
        {
            return SetNewDoubleProperty(_processed.GetRange(myType), ExtColProperty.Min, myIndex, myNewValue);
        }

        public double SetMaximum(int myIndex, ColType myType, double myNewValue)
        {
            return SetNewDoubleProperty(_processed.GetRange(myType), ExtColProperty.Max, myIndex, myNewValue);
        }

        public double SetResponseThreshold(int myIndex, ColType myType, double myNewValue)
        {
            return SetNewDoubleProperty(_processed.GetRange(myType), ExtColProperty.Thresh, myIndex, myNewValue);
        }

        private double SetNewDoubleProperty(TableRange myRange, string myProperty, int myIndex, double myNewValue)
        {
            var value = myNewValue;

            _processed.Table.Columns[myIndex + myRange.StartIndex].ExtendedProperties[myProperty] = myNewValue;

            return value;
        }

        /// <summary>
        /// Update a source's column from a column info object.
        /// </summary>
        /// <param name="type">type of column range to update</param>
        /// <param name="myIndex">index relative to the range type</param>
        /// <param name="info">info to get values from</param>
        public void UpdateFromColumnInfo(ColType type, int myIndex, ColumnInfo info)
        {
            var range = GetRange(type);

            SetNewName(GetLabels(range), range, myIndex, info.NewName);
            SetNewDoubleProperty(range, ExtColProperty.Min, myIndex, info.Min);
            SetNewDoubleProperty(range, ExtColProperty.Max, myIndex, info.Max);
            SetNewDoubleProperty(range, ExtColProperty.Thresh, myIndex, info.Threshold);
            SetNewStringProperty(range, ExtColProperty.Unit, myIndex, info.Unit);

            //keep track of current location of the info in the table
            info.Index = myIndex;
        }
    }
}
