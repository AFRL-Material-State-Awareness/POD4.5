using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Linq;
using POD.ExcelData;
using System.Windows.Forms;
//Rengine
using CSharpBackendWithR;
using Data;

namespace POD.Data
{
    /// <summary>
    ///     This holds all of the input data tables for an analysis. This includes transformed data as well.
    ///     It is also in charge of talking to the Python code/libraries for performing the transforms.
    /// </summary>
    [Serializable]
    public class AnalysisData : IAnalysisData
    {
        #region Fields

        [NonSerialized]
        public List<SortPoint> sortByX = new List<SortPoint>();
        private int _prevAbove;
        private int _prevBelow;
        private bool _prevBelowDoesNotInclude;
        public Dictionary<int, Dictionary<int, string>> _commentDictionary;

        public Dictionary<int, Dictionary<int, string>> CommentDictionary
        {
            get
            {
                if (_commentDictionary == null)
                    _commentDictionary = new Dictionary<int, Dictionary<int, string>>();

                return _commentDictionary;
            }
        }
        
        /// <summary>
        ///     a data table containing data from activated flaw size columns
        /// </summary>
        private DataTable _activatedFlawTable;

        /// <summary>
        ///     names of the activated flaw size columns
        /// </summary>
        private List<string> _activatedFlaws;

        /// <summary>
        ///     a data table containing data from activated meta data columns
        /// </summary>
        private DataTable _activatedMetaDataTable;

        /// <summary>
        ///     names of activated metadata columns
        /// </summary>
        private List<string> _activatedMetaDatas;

        /// <summary>
        ///     a data table containing data from activated response data columns
        /// </summary>
        private DataTable _activatedResponseTable;
        private DataTable _calculatedResponseTable;

        /// <summary>
        ///     names of activated response data columns
        /// </summary>
        private List<string> _activatedResponses;

        /// <summary>
        ///     a data table containing data from activated specimen ID columns
        /// </summary>
        private DataTable _activatedSpecIDTable;

        /// <summary>
        ///     names of activated specimen ID columns
        /// </summary>
        private List<string> _activatedSpecIDs;

        /// <summary>
        ///     transformed version of activated flaw size table
        /// </summary>
        private DataTable _activatedTransformedFlawTable;

        /// <summary>
        ///     transformed version of activated response data table
        /// </summary>
        private DataTable _activatedTransformedResponseTable;

        /// <summary>
        ///     available flaw size columns that can be activated
        /// </summary>
        private List<string> _availableFlaws;

        /// <summary>
        ///     table full of all available flaw size data. Key is column name.
        ///     Value is list of values found at that column.
        /// </summary>
        private DataTable _availableFlawsTable;

        /// <summary>
        ///     available metadata columns that can be activated
        /// </summary>
        private List<string> _availableMetaDatas;

        /// <summary>
        ///     table full of all available metadata. Key is column name.
        ///     Value is list of values found at that column.
        /// </summary>
        private DataTable _availableMetaDatasTable;

        /// <summary>
        ///     available response data columns that can be activated
        /// </summary>
        private List<string> _availableResponses;

        /// <summary>
        ///     table full of all available response data. Key is column name.
        ///     Value is list of values found at that column.
        /// </summary>
        private DataTable _availableResponsesTable;

        /// <summary>
        ///     available specimen ID columns that can be activated
        /// </summary>
        private List<string> _availableSpecIDs;

        /// <summary>
        ///     table full of all available specimen IDs. Key is column name.
        ///     Value is list of values found at that column.
        /// </summary>
        private DataTable _availableSpecIDsTable;

        /// <summary>
        ///     equation used to apply custom transform to flaw size table
        /// </summary>
        private string _customFlawTransformEquation;

        /// <summary>
        ///     equation used to invert custom transform to flaw size table
        /// </summary>
        private string _customFlawTransformInverseEquation;

        /// <summary>
        ///     equation used to apply custom transform to response data table
        /// </summary>
        private string _customResponseTransformEquation;

        /// <summary>
        ///     equation used to invert custom transform to response data table
        /// </summary>
        private string _customResponseTransformInverseEquation;

        /// <summary>
        ///     type of analysis that should be performed on the data
        /// </summary>
        private AnalysisDataTypeEnum _dataType;

        /// <summary>
        ///     Table holds the fitted value for each flaw size and the difference between fitted and input data for each point.
        /// </summary>
        private DataTable _fitResidualsTable;

        /// <summary>
        ///     the type of transform to apply on the flaw size table
        /// </summary>
        private TransformTypeEnum _flawTransform;

        /// <summary>
        ///     Table holds the flaw size, POD and confidence bound
        /// </summary>
        private DataTable _podCurveTable;
        private DataTable _podCurveTable_All;
        /// <summary>
        ///     Table holds the flaw range and number of flaws in that rnage
        /// </summary>
        private DataTable _normalityTable;
        /// <summary>
        /// Table holds points to plot a normal curve over the histogram
        /// </summary>
        private DataTable _normalityCurveTable;

        /// <summary>
        /// used to store and plot the original data (mainly used when modified wald, lr, or mlr is used
        /// </summary>
        private DataTable _originalData;
        /// <summary>
        ///     IronPython engine used to call Python code/libraries.
        /// </summary>
        [NonSerialized]
        private I_IPy4C _python;

        private HMAnalysisObject _hmAnalysisObject;
        private AHatAnalysisObject _aHatAnalysisObject;

        /// <summary>
        ///     the type of transform to apply on the response data table
        /// </summary>
        private TransformTypeEnum _responseTransform;

        /// <summary>
        ///     Holds the a50, a90, a90/95, V11, V12, V22 (not sure what Vs are yet)
        /// </summary>
        private DataTable _thresholdPlotTable;

        /// <summary>
        ///     Holds the a50, a90, a90/95, V11, V12, V22 (not sure what Vs are yet)
        /// </summary>
        private DataTable _thresholdPlotTable_All;

        /// <summary>
        ///     a list of response data points that are turned off (ie value in activated data table = Double.NaN)
        /// </summary>
        private List<DataPointIndex> _turnedOffPoints;
        private bool _updatePythonData = true;

        private DataTable _residualUncensoredTable;

        public DataTable ResidualUncensoredTable
        {
            get { return _residualUncensoredTable; }
            //set { _residualUncensoredTable = value; }
        }

        private DataTable _residualRawTable;

        public DataTable ResidualRawTable
        {
            get { return _residualRawTable; }
            //set { _residualUncensoredTable = value; }
        }

        private DataTable _residualCensoredTable;

        private DataTable _iterationsTable;

        public DataTable IterationsTable
        {
            get { return _iterationsTable; }
        }

        private DataTable _residualsTable;

        public DataTable ResidualsTable
        {
            get { return _residualsTable; }
        }

        public DataTable ResidualCensoredTable
        {
            get { return _residualCensoredTable; }
        }

        private DataTable _residualPartialCensoredTable;

        public DataTable ResidualPartialCensoredTable
        {
            get { return _residualPartialCensoredTable; }
        }

        private DataTable _residualFullCensoredTable;

        public DataTable ResidualFullCensoredTable
        {
            get { return _residualFullCensoredTable; }
        }

        public double ResponseLeft { get; set; }
        public double ResponseRight { get; set; }
        private double _smallestFlawSize;
        private double _smallestResponse;
        public bool IsFrozen { get; set; }

        private double _minSignal = 0.0;

        public double MinSignal
        {
            get { return _minSignal; }
            set { _minSignal = value; }
        }

        private double _minFlaw = 0.0;
        private bool _filterByRanges;

        public double MinFlaw
        {
            get { return _minFlaw; }
            set { _minFlaw = value; }
        }

        private double _maxSignal = 0.0;

        public double MaxSignal
        {
            get { return _maxSignal; }
            set { _maxSignal = value; }
        }

        private double _maxFlaw = 0.0;

        public double MaxFlaw
        {
            get { return _maxFlaw; }
            set { _maxFlaw = value; }
        }


        #endregion

        #region Constructors

        /// <summary>
        ///     create a new analysis data with no associated data source.
        /// </summary>
        public AnalysisData()
        {
            Initialize();

            sortByX = new List<SortPoint>();
        }

        /// <summary>
        ///     a create a new analysis data with all possible data from data source copied over
        /// </summary>
        /// <param name="mySource">the data source to copy to the analysis data</param>
        public AnalysisData(DataSource mySource)
        {
            Initialize();

            SetSource(mySource);

            sortByX = new List<SortPoint>();

           // UpdateData();
        }

        /// <summary>
        ///     Create a new analysis data with only selected columns of data copied over
        /// </summary>
        /// <param name="mySource">the data source to copy to the analysis data</param>
        /// <param name="myFlaws">name of flaw size columns to copy</param>
        /// <param name="myMetaDatas">name of metadata columns to copy</param>
        /// <param name="myResponses">name of response data columns to copy</param>
        /// <param name="mySpecimenIDs">name of specimen ID column to copy</param>
        public AnalysisData(DataSource mySource, List<string> myFlaws, List<string> myMetaDatas,
            List<string> myResponses, List<string> mySpecimenIDs)
        {
            Initialize();

            SetSource(mySource, myFlaws, myMetaDatas, myResponses, mySpecimenIDs);

            //UpdateData();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Get a copy of the activated flaw size column names
        /// </summary>
        public string ActivatedFlawName
        {
            get
            {
                if (_activatedFlaws.Count > 0)
                    return _activatedFlaws[0];
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// Get a list of the original names of the flaw columns
        /// </summary>
        public string ActivatedOriginalFlawName
        {
            get
            {
                var names = GetOriginalNamesFromTable(_activatedFlawTable);

                if (names.Count > 0)
                    return names[0];
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// Get a list of the original names of the response columns
        /// </summary>
        public List<string> ActivatedOriginalResponseNames
        {
            get
            {
                return GetOriginalNamesFromTable(_activatedResponseTable);
            }
        }

        private static List<string> GetOriginalNamesFromTable(DataTable table)
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

        public string GetRemovedPointComment(int myColIndex, int myRowIndex)
        {
            //if column entry is there
            if(CommentDictionary.ContainsKey(myColIndex))
            {
                //if row entry is there
                if(CommentDictionary[myColIndex].ContainsKey(myRowIndex))
                    return CommentDictionary[myColIndex][myRowIndex];
                //add row entry
                else
                    CommentDictionary[myColIndex].Add(myRowIndex, "");
            }
            else
            {
                //add column and row entry
                var rowDictionary = new Dictionary<int, string>();
                CommentDictionary.Add(myColIndex, rowDictionary);
                rowDictionary.Add(myRowIndex, "");
            }
            return "";
        }

        public void SetRemovedPointComment(int myColIndex, int myRowIndex, string myComment)
        {
            //force spot in the dictionary
            GetRemovedPointComment(myColIndex, myRowIndex);
            //set the value
            CommentDictionary[myColIndex][myRowIndex] = myComment;
        }

        /// <summary>
        ///     Get the activated columns flaw size data datable
        /// </summary>
        public DataTable ActivatedFlaws
        {
            get
            {
                TransformData(_activatedFlawTable, ref _activatedTransformedFlawTable, _flawTransform,
                                  _customFlawTransformEquation);

                return _activatedTransformedFlawTable; 
            }
        }

        /// <summary>
        ///     Get a copy of the activated metadata column names
        /// </summary>
        public List<string> ActivatedMetaDataNames
        {
            get { return new List<string>(_activatedMetaDatas); }
        }

        /// <summary>
        ///     Get the activated columns metadata datable
        /// </summary>
        public DataTable ActivatedMetaDatas
        {
            get { return _activatedMetaDataTable; }
        }

        /// <summary>
        ///     Get a copy of the activated response data column names
        /// </summary>
        public List<string> ActivatedResponseNames
        {
            get { return new List<string>(_activatedResponses); }
        }


        /// <summary>
        ///     Get the activated columns response data datable
        /// </summary>
        public DataTable ActivatedResponses
        {
            get
            {
                TransformData(_activatedResponseTable, ref _activatedTransformedResponseTable, _responseTransform,
                              _customResponseTransformEquation);


                return _activatedTransformedResponseTable; 
            }
        }

        /// <summary>
        ///     Get a copy of the activated specimen ID column names
        /// </summary>
        public List<string> ActivatedSpecimenIDNames
        {
            get { return new List<string>(_activatedSpecIDs); }
        }

        /// <summary>
        ///     Get the activated columns specimen ID datable
        /// </summary>
        public DataTable ActivatedSpecimenIDs
        {
            get { return _activatedSpecIDTable; }
        }

        /// <summary>
        ///     Get the activated columns transformed flaw size datable
        /// </summary>
        public DataTable ActivatedTransformedFlaws
        {
            get { return _activatedTransformedFlawTable; }
        }

        /// <summary>
        ///     Get the activated columns transformed response data datable
        /// </summary>
        public DataTable ActivatedTransformedResponses
        {
            get { return _activatedTransformedResponseTable; }
        }

        /// <summary>
        ///     Get a copy of the available flaw size column names
        /// </summary>
        public List<string> AvailableFlawNames
        {
            get { return new List<string>(_availableFlaws); }
        }

        /// <summary>
        ///     Get a copy of the available metadata column names
        /// </summary>
        public List<string> AvailableMetaDataNames
        {
            get { return new List<string>(_availableMetaDatas); }
        }

        /// <summary>
        ///     Get a copy of the available response data column names
        /// </summary>
        public List<string> AvailableResponseNames
        {
            get { return new List<string>(_availableResponses); }
        }


        /// <summary>
        ///     Get the type of analysis that will be performed on the data
        /// </summary>
        public AnalysisDataTypeEnum DataType
        {
            set { _dataType = value; }
            get { return _dataType; }
        }

        /// <summary>
        ///     Table holds the fitted value for each flaw size and the difference between fitted and input data for each point.
        /// </summary>
        public DataTable FitResidualsTable
        {
            get { return _fitResidualsTable; }
        }

        /// <summary>
        ///     Get/set the flaw size transform type
        /// </summary>
        public TransformTypeEnum FlawTransform
        {
            get { return _flawTransform; }
            set
            {
                _flawTransform = value;

                TransformData(_activatedFlawTable, ref _activatedTransformedFlawTable,
                    _flawTransform, _customFlawTransformEquation);                

                RefreshTurnedOffPoints();
            }
        }

        /// <summary>
        ///     Table holds the flaw size, POD and confidence bound
        /// </summary>
        public DataTable PodCurveTable
        {
            get { return _podCurveTable; }
        }

        public DataTable PodCurveTable_All
        {
            get { return _podCurveTable_All; }
        }

        /// <summary>
        /// Table holds the frequency of ranges in order to plot the normality chart
        /// </summary>
        public DataTable NormalityTable
        {
            get { return _normalityTable; }
        }

        /// <summary>
        /// get the table to plot a normal curve overlay
        /// </summary>
        public DataTable NormalityCurveTable
        {
            get { return _normalityCurveTable; }
        }

        public DataTable OriginalData
        {
            get { return _originalData; }
        }
        public double LambdaValue
        {
            get { return _aHatAnalysisObject.Lambda; }
        }
        /// <summary>
        ///     Get/set the response data transform type
        /// </summary>
        public TransformTypeEnum ResponseTransform
        {
            get { return _responseTransform; }
            set
            {
                _responseTransform = value;

                TransformData(_activatedResponseTable, ref _activatedTransformedResponseTable,
                    _responseTransform, _customResponseTransformEquation);                

                RefreshTurnedOffPoints();
            }
        }

        /// <summary>
        ///     Holds the a50, a90, a90/95, V11, V12, V22 (Vs represent the values for the varaince-covariance matrix)
        /// </summary>
        public DataTable ThresholdPlotTable
        {
            get { return _thresholdPlotTable; }
        }

        /// <summary>
        ///     Holds the a50, a90, a90/95, V11, V12, V22 (plots the ghost curve if applicable)
        /// </summary>
        public DataTable ThresholdPlotTable_All
        {
            get { return _thresholdPlotTable_All; }
        }

        /// <summary>
        /// Creates a duplicate list of turned off points to read. DataPointIndex has ColumnName and RowIndex.
        /// </summary>
        public List<DataPointIndex> TurnedOffPoints
        {
            get
            {
                List<DataPointIndex> list = new List<DataPointIndex>(_turnedOffPoints);

                return list;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Make active a single flaw column.
        /// </summary>
        /// <param name="myName">the name of the flaw column</param>
        /// <returns>a data table containing the flaw data</returns>
        public DataTable ActivateFlaw(String myName, bool runUpdate = true)
        {
            ActivateValue(ref _activatedFlaws, myName, ref _activatedFlawTable, _availableFlawsTable);
            TransformData(_activatedFlawTable, ref _activatedTransformedFlawTable, _flawTransform,
                _customFlawTransformEquation);

            if(runUpdate)
                UpdateData();

            return _activatedFlawTable;
        }

        /// <summary>
        ///     Make active multiple flaw columns.
        /// </summary>
        /// <param name="myNames">name of the columns</param>
        /// <returns>a data table containing the flaw data</returns>
        public DataTable ActivateFlaws(List<string> myNames, bool runUpdate = true)
        {
            ActivateValues(ref _activatedFlaws, myNames, ref _activatedFlawTable, _availableFlawsTable);
            TransformData(_activatedFlawTable, ref _activatedTransformedFlawTable, _flawTransform,
                _customFlawTransformEquation);

            if(runUpdate)
                UpdateData();

            return _activatedFlawTable;
        }


        /// <summary>
        ///     Make active multiple metadata columns.
        /// </summary>
        /// <param name="myNames">name of the columns</param>
        /// <returns>a data table containing the metadata data</returns>
        private DataTable ActivateMetaDatas(List<string> myNames)
        {
            return ActivateValues(ref _activatedMetaDatas, myNames, ref _activatedMetaDataTable,
                _availableMetaDatasTable);
        }


        /// <summary>
        ///     Make active multiple response data columns.
        /// </summary>
        /// <param name="myNames">name of the columns</param>
        /// <returns>a data table containing the response data</returns>
        public DataTable ActivateResponses(List<string> myNames, bool runUpdate = true)
        {
            ActivateValues(ref _activatedResponses, myNames, ref _activatedResponseTable, ref _calculatedResponseTable, _availableResponsesTable);
            TransformData(_activatedResponseTable, ref _activatedTransformedResponseTable, _responseTransform,
                _customResponseTransformEquation);

            if (runUpdate)
            {
                RefreshTurnedOffPoints();
                UpdateData();
            }

            return _activatedResponseTable;
        }
        /// <summary>
        ///     Make active multiple specimen ID data columns.
        /// </summary>
        /// <param name="myNames">name of the columns</param>
        /// <returns>a data table containing the specimen ID data</returns>
        public DataTable ActivateSpecIDs(List<string> myNames)
        {
            return ActivateValues(ref _activatedSpecIDs, myNames, ref _activatedSpecIDTable, _availableSpecIDsTable);
        }

        /// <summary>
        ///     Activates a column for a kind of data of type T.
        /// </summary>
        /// <param name="myList">the current list of active columns</param>
        /// <param name="myName">the name of the column to activate</param>
        /// <param name="myTable">the data table that will hold the actived column data</param>
        /// <param name="myData">the table to pull the data from</param>
        /// <returns>the data table holding activated column's data</returns>
        private static DataTable ActivateValue(ref List<string> myList, string myName, ref DataTable myTable, ref DataTable myCalcTable,
            DataTable myData)
        {
            var names = new List<string>();

            names.Add(myName);

            return ActivateValues(ref myList, names, ref myTable, ref myCalcTable, myData);
        }

        private static DataTable ActivateValue(ref List<string> myList, string myName, ref DataTable myTable, DataTable myData)
        {
            var names = new List<string>();

            names.Add(myName);

            return ActivateValues(ref myList, names, ref myTable, myData);
        }

        /// <summary>
        ///     Activates a set of columns for a kind of data of type T.
        /// </summary>
        /// <param name="myList">the current list of active columns</param>
        /// <param name="myNames">the name of the columns to activate</param>
        /// <param name="myTable">the data table that will hold the actived columns' data</param>
        /// <param name="myData">the table to pull the data from</param>
        /// <returns>the data table holding activated columns' data</returns>
        private static DataTable ActivateValues(ref List<string> myList, List<string> myNames, ref DataTable myTable, ref DataTable myCalcTable,
                                                DataTable myData)
        {
            myCalcTable = ActivateValues(ref myList, myNames, ref myTable, myData).DefaultView.ToTable(false, myNames.ToArray());
            
            return myTable;
        }

        private static DataTable ActivateValues(ref List<string> myList, List<string> myNames, ref DataTable myTable, DataTable myData)
        {
            myTable = myData.DefaultView.ToTable(false, myNames.ToArray());
            myList = new List<string>(myNames);

            return myTable;
        }

        /// <summary>
        ///     For a given kind of data of type T, build the data dictionaries from the DataSource object.
        /// </summary>
        /// <param name="mySource">the given DataSource object</param>
        /// <param name="myValues">data table for values of type T</param>
        /// <param name="myColumnNames">names of columns in the DataSource that hold the data</param>
        private static void BuildAvailableData(DataSource mySource, ref DataTable myValues, List<string> myColumnNames)
        {
            myValues = mySource.GetData(myColumnNames);
        }

        /// <summary>
        ///     Duplicate all of the analysis data.
        /// </summary>
        /// <returns>duplicated analysis data</returns>
        public AnalysisData CreateDuplicate()
        {
            var data = (AnalysisData) MemberwiseClone();

            data._python = null;// _python.CreateDuplicate();

            //data._podDoc = null;

            data._hmAnalysisObject = null;
            data._aHatAnalysisObject = null;

            var fromTables = new List<DataTable>
            {

                _activatedFlawTable,
                _activatedMetaDataTable,
                _activatedResponseTable,
                _activatedSpecIDTable,
                _activatedTransformedFlawTable,
                _activatedTransformedResponseTable,
                _availableFlawsTable,
                _availableMetaDatasTable,
                _availableResponsesTable,
                _availableSpecIDsTable,
                _calculatedResponseTable,
                _fitResidualsTable,
                _iterationsTable,
                _podCurveTable,
                _podCurveTable_All,
                _quickTable,
                _residualCensoredTable,
                _residualFullCensoredTable,
                _residualPartialCensoredTable,
                _residualRawTable,
                _residualsTable,
                _residualUncensoredTable,
                _thresholdPlotTable,
                _thresholdPlotTable_All,
                _originalData
            };

            

            data._activatedFlawTable = new DataTable();
            data._activatedMetaDataTable = new DataTable();
            data._activatedResponseTable = new DataTable();
            data._activatedSpecIDTable = new DataTable();
            data._activatedTransformedFlawTable = new DataTable();
            data._activatedTransformedResponseTable = new DataTable();
            data._availableFlawsTable = new DataTable();
            data._availableMetaDatasTable = new DataTable();
            data._availableResponsesTable = new DataTable();
            data._availableSpecIDsTable = new DataTable();
            data._calculatedResponseTable = new DataTable();
            data._fitResidualsTable = new DataTable();
            data._iterationsTable = new DataTable();
            data._podCurveTable = new DataTable();
            data._podCurveTable_All = new DataTable();
            data._quickTable = new DataTable();
            data._residualCensoredTable = new DataTable();
            data._residualFullCensoredTable = new DataTable();
            data._residualPartialCensoredTable = new DataTable();
            data._residualRawTable = new DataTable();
            data._residualsTable = new DataTable();
            data._residualUncensoredTable = new DataTable();
            data._thresholdPlotTable = new DataTable();
            data._thresholdPlotTable_All = new DataTable();
            data._originalData = new DataTable();
            var toTables = new List<DataTable>
            {
                data._activatedFlawTable,
                data._activatedMetaDataTable,
                data._activatedResponseTable,
                data._activatedSpecIDTable,
                data._activatedTransformedFlawTable,
                data._activatedTransformedResponseTable,
                data._availableFlawsTable,
                data._availableMetaDatasTable,
                data._availableResponsesTable,
                data._availableSpecIDsTable,
                data._calculatedResponseTable,
                data._fitResidualsTable,
                data._iterationsTable,
                data._podCurveTable,
                data._podCurveTable_All,
                data._quickTable,
                data._residualCensoredTable,
                data._residualFullCensoredTable,
                data._residualPartialCensoredTable,
                data._residualRawTable,
                data._residualsTable,
                data._residualUncensoredTable,
                data._thresholdPlotTable,
                data._thresholdPlotTable_All,
                data._originalData
            };

            Debug.Assert(fromTables.Count == toTables.Count);

            //copy each column then all data
            for (int i = 0; i < fromTables.Count; i++)
            {
                if (fromTables[i] != null)
                {
                    var toTable = toTables[i];
                    var fromTable = fromTables[i];

                    DuplicateTable(fromTable, toTable);
                }
            }

            var fromStringLists = new List<List<string>>
            {
                _activatedFlaws,
                _activatedMetaDatas,
                _activatedResponses,
                _activatedSpecIDs,
                _availableFlaws,
                _availableMetaDatas,
                _availableResponses,
                _availableSpecIDs
            };

            data._activatedFlaws = new List<string>();
            data._activatedMetaDatas = new List<string>();
            data._activatedResponses = new List<string>();
            data._activatedSpecIDs = new List<string>();
            data._availableFlaws = new List<string>();
            data._availableMetaDatas = new List<string>();
            data._availableResponses = new List<string>();
            data._availableSpecIDs = new List<string>();

            var toStringLists = new List<List<string>>
            {
                data._activatedFlaws,
                data._activatedMetaDatas,
                data._activatedResponses,
                data._activatedSpecIDs,
                data._availableFlaws,
                data._availableMetaDatas,
                data._availableResponses,
                data._availableSpecIDs
            };

            Debug.Assert(fromStringLists.Count == toStringLists.Count);

            //duplicate each string
            for (int i = 0; i < fromStringLists.Count; i++)
            {
                toStringLists[i].AddRange(fromStringLists[i]);
            }

            data._turnedOffPoints = new List<DataPointIndex>();

            //duplicate each turned off point
            foreach (DataPointIndex point in _turnedOffPoints)
            {
                var comment = GetRemovedPointComment(point.ColumnIndex, point.RowIndex);

                data._turnedOffPoints.Add(new DataPointIndex(point.ColumnIndex, point.RowIndex, comment));
            }

            data._commentDictionary = new Dictionary<int, Dictionary<int, string>>();

            foreach(KeyValuePair<int, Dictionary<int, string>> entry in _commentDictionary)
            {
                data._commentDictionary.Add(entry.Key, CloneDictionaryCloningValues<int, string>(entry.Value));
            }

            return data;
        }

        public static void DuplicateTable(DataTable fromTable, DataTable toTable)
        {
            if (toTable == null && fromTable != null)
                toTable = new DataTable();

            if (fromTable == null)
                toTable = null;

            if (fromTable != null && toTable != null)
            {
                toTable.TableName = fromTable.TableName;
                toTable.Columns.Clear();
                toTable.Rows.Clear();

                foreach (DataColumn col in fromTable.Columns)
                {
                    var newCol = new DataColumn(col.ColumnName, col.DataType);

                    foreach (string key in col.ExtendedProperties.Keys)
                    {
                        newCol.ExtendedProperties.Add(key, col.ExtendedProperties[key]);
                    }

                    toTable.Columns.Add(newCol);
                }
                toTable.Load(fromTable.CreateDataReader());
            }
        }

        //Taken From: http://stackoverflow.com/questions/139592/what-is-the-best-way-to-clone-deep-copy-a-net-generic-dictionarystring-t
        //Code by Jon Skeet
        public static Dictionary<TKey, TValue> CloneDictionaryCloningValues<TKey, TValue> (Dictionary<TKey, TValue> original) where TValue : ICloneable
        {
            Dictionary<TKey, TValue> ret = new Dictionary<TKey, TValue>(original.Count,
                                                                    original.Comparer);
            foreach (KeyValuePair<TKey, TValue> entry in original)
            {
                ret.Add(entry.Key, (TValue)entry.Value.Clone());
            }
            return ret;
        }

        /// <summary>
        ///     Initialize data structures and set default values.
        /// </summary>
        private void Initialize()
        {
            _availableFlawsTable = new DataTable();
            _availableResponsesTable = new DataTable();
            _availableMetaDatasTable = new DataTable();
            _availableSpecIDsTable = new DataTable();

            _activatedFlaws = new List<string>();
            _activatedMetaDatas = new List<string>();
            _activatedResponses = new List<string>();
            _activatedSpecIDs = new List<string>();

            _activatedFlawTable = new DataTable();
            _activatedMetaDataTable = new DataTable();
            _activatedResponseTable = new DataTable();
            _calculatedResponseTable = new DataTable();
            _activatedSpecIDTable = new DataTable();
            _activatedTransformedFlawTable = new DataTable();
            _activatedTransformedResponseTable = new DataTable();
            _podCurveTable = new DataTable();
            _thresholdPlotTable = new DataTable();

            _turnedOffPoints = new List<DataPointIndex>();

            _flawTransform = TransformTypeEnum.Linear;
            _responseTransform = TransformTypeEnum.Linear;

            _updatePythonData = true;

            TransformBackLambda = null;
        }

        /// <summary>
        ///     Reapply all turned off points to response data tables.
        /// </summary>
        private void RefreshTurnedOffPoints()
        {
            //update the response table with points that are turned off
            foreach (DataPointIndex point in _turnedOffPoints)
            {
                TurnPointInTables(point.ColumnIndex, point.RowIndex, false);
            }
        }

        /// <summary>
        ///     Copy only columns specified in the column name lists over to the analysis data. Then
        ///     activate the copied columns.
        /// </summary>
        /// <param name="mySource">the data source to copy data from</param>
        /// <param name="myFlaws">the name of the flaw columns to copy</param>
        /// <param name="myMetaDatas">the name of metadata columns to copy</param>
        /// <param name="myResponses">the name of response data columns to copy</param>
        /// <param name="mySpecIDs">the name of specimen id columns to copy</param>
        public void SetSource(DataSource mySource, List<string> myFlaws, List<string> myMetaDatas,
            List<string> myResponses, List<string> mySpecIDs)
        {
             _availableResponses = myResponses;
            _availableFlaws = myFlaws;
            _availableMetaDatas = myMetaDatas;
            _availableSpecIDs = mySpecIDs;

            _updatePythonData = false;

            BuildAvailableData(mySource, ref _availableResponsesTable, _availableResponses);
            BuildAvailableData(mySource, ref _availableFlawsTable, _availableFlaws);
            BuildAvailableData(mySource, ref _availableMetaDatasTable, _availableMetaDatas);
            BuildAvailableData(mySource, ref _availableSpecIDsTable, _availableSpecIDs);

            //if quick analysis data
            if(mySource.Original.Rows.Count == 0)
            {
                _quickTable.Columns.Clear();

                foreach(DataColumn col in mySource.Original.Columns)
                {
                    _quickTable.Columns.Add(col.ColumnName, col.DataType);
                }
            }

            ActivateFlaws(myFlaws);
            ActivateMetaDatas(myMetaDatas);
            ActivateResponses(myResponses);
            ActivateSpecIDs(mySpecIDs);

            //Variable used to store analysis type (hit/miss, ahat, etc)
             _dataType = mySource.AnalysisDataType;

            _flawTransform = TransformTypeEnum.Linear;
            _responseTransform = TransformTypeEnum.Linear;

            if(_dataType == AnalysisDataTypeEnum.HitMiss)
                _flawTransform = TransformTypeEnum.Log;

            TransformData(_activatedResponseTable, ref _activatedTransformedResponseTable, _responseTransform,
                _customResponseTransformEquation);
            TransformData(_activatedFlawTable, ref _activatedTransformedFlawTable, _flawTransform,
                _customFlawTransformEquation);

            _updatePythonData = true;

            RefreshTurnedOffPoints();
            UpdateData();

            CalculateMinFlaw();
            CalculateMinResponse();
        }
        //Method is used to calculate the minimum flaw size of the dataset
        private void CalculateMinFlaw()
        {
            double minFlaw = double.MaxValue;
            var flaws = _availableFlawsTable;
            //printDT(_availableFlawsTable);
            //used for quick analysis
            if (flaws.Rows.Count == 0)
                minFlaw = 0.0;

            foreach (DataRow dr in flaws.Rows)
            {
                var flaw = dr.Field<double>(0);
                minFlaw = Math.Min(minFlaw, flaw);
            }

            _smallestFlawSize = minFlaw;
        }

        private void CalculateMinResponse()
        {
            double minResponse = double.MaxValue;
            var responses = _availableResponsesTable;

            //used for quick analysis
            if (responses.Rows.Count == 0)
                minResponse = 0.0;

            foreach (DataRow dr in responses.Rows)
            {
                foreach (DataColumn col in responses.Columns)
                {
                    double response = dr.Field<double>(col);
                    
                    minResponse = Math.Min(minResponse, response);
                }
            }

            _smallestResponse = minResponse;
        }

        /// <summary>
        ///     Copy all of the data available from the source to the analysis data then activate all available data.
        /// </summary>
        /// <param name="mySource">the data source to copy</param>
        public void SetSource(DataSource mySource)
        {
            SetSource(mySource, mySource.FlawLabels, mySource.MetaDataLabels, mySource.ResponseLabels, mySource.IDLabels);
        }

        /// <summary>
        ///     Turn off a set of points all at once.
        /// </summary>
        /// <param name="myList">list of points to turn off</param>
        public void SetTurnedOffPoints(List<DataPointIndex> myList)
        {
            _turnedOffPoints = new List<DataPointIndex>(myList);
        }

        /// <summary>
        ///     Transforms data found in the source table and stores it in the transform table.
        /// </summary>
        /// <param name="mySourceTable">the data to transform</param>
        /// <param name="myTransformTable">table that holds transformed data</param>
        /// <param name="myTransformType">what function should be used to transform the data</param>
        /// <param name="myCustomEquation">equation to use if it is a custom transform</param>
        private void TransformData(DataTable mySourceTable, ref DataTable myTransformTable,
                                   TransformTypeEnum myTransformType, string myCustomEquation)
        {
            if ((_hmAnalysisObject != null || _aHatAnalysisObject != null) && _python != null)
            {
                myTransformTable = mySourceTable.DefaultView.ToTable();

                var minValue = 0.0;
                var safeValue = 1.0;
                var minValueTransformed = 0.0;

                foreach (DataColumn column in mySourceTable.Columns)
                {
                    //get the data to transform
                    var values = new List<double>();
                    foreach (DataRow row in mySourceTable.Rows)
                    {
                        var value = (double)row[column.ColumnName];

                        values.Add(value);
                    }

                    var minChanged = new List<int>();
                    //var maxChanged = new List<int>();

                    //filter or keep values to transform safe
                    if(myTransformType == TransformTypeEnum.Inverse || myTransformType == TransformTypeEnum.Log || myTransformType == TransformTypeEnum.BoxCox)// || FilterTransformedDataByRanges)
                    {
                        for(int i = 0; i < values.Count; i++)
                        {
                            if(values[i] <= minValue)
                            {
                                minChanged.Add(i);
                                values[i] = safeValue;
                            }
                        }
                    }

                    for(int i=0; i < values.Count(); i++)
                    {
                        values[i]=TransformAValue(values[i], _python.TransformEnumToInt(myTransformType));
                    }
                    //copy transformed data back to the other table
                    int index = 0;
                    foreach (double value in values)
                    {
                        myTransformTable.Rows[index][column.ColumnName] = value;
                        index++;
                    }
                    
                    foreach(var changedIndex in minChanged)
                    {
                        myTransformTable.Rows[changedIndex][column.ColumnName] = minValueTransformed;
                    }
                }
            }
        }

        /// <summary>
        ///     Turn ON all response data points previously turned OFF.
        /// </summary>
        public void TurnAllPointsOn()
        {
            foreach (DataPointIndex index in _turnedOffPoints)
            {
                TurnPoint(index.ColumnIndex, index.RowIndex, true);
            }
        }

        /// <summary>
        ///     Turn off a point in the table response data.
        /// </summary>
        /// <param name="myResponseColumnIndex">column index relative to response data table</param>
        /// <param name="myRowIndex">row index of data point</param>
        public void TurnOffPoint(int myResponseColumnIndex, int myRowIndex)
        {
            TurnPoint(myResponseColumnIndex, myRowIndex, false);
        }

        /// <summary>
        ///     Turn off a point in the table response data.
        /// </summary>
        /// <param name="myResponseColumn">the name of the response data column</param>
        /// <param name="myRowIndex">row index of data point</param>
        //public void TurnOffPoint(string myResponseColumn, int myRowIndex)
        //{
        //    TurnPoint(myResponseColumn, myRowIndex, false);
        //}

        /// <summary>
        ///     Turns off all points at a given index (ie for a given flaw size)
        /// </summary>
        /// <param name="myRowIndex">index of row to turn off</param>
        public void TurnOffPoints(int myRowIndex)
        {
            TurnPoints(myRowIndex, false);
        }

        /// <summary>
        ///     Turn on a point in the table response data.
        /// </summary>
        /// <param name="myResponseColumnIndex">column index relative to response data table</param>
        /// <param name="myRowIndex">row index of data point</param>
        public void TurnOnPoint(int myResponseColumnIndex, int myRowIndex)
        {
            TurnPoint(myResponseColumnIndex, myRowIndex, true);
        }

        /// <summary>
        ///     Turn on a point in the table response data.
        /// </summary>
        /// <param name="myResponseColumn">the name of the response data column</param>
        /// <param name="myRowIndex">row index of data point</param>
        //public void TurnOnPoint(string myResponseColumn, int myRowIndex)
        //{
        //    TurnPoint(myResponseColumn, myRowIndex, true);
        //}

        /// <summary>
        ///     Turns on all points at a given index (ie for a given flaw size)
        /// </summary>
        /// <param name="myRowIndex">index of row to turn off</param>
        public void TurnOnPoints(int myRowIndex)
        {
            TurnPoints(myRowIndex, true);
        }

        /// <summary>
        ///     Turn a response data point either ON or OFF.
        /// </summary>
        /// <param name="myResponseColumn">the name of the response data column</param>
        /// <param name="myRowIndex">index of row to turn ON/OFF</param>
        /// <param name="myTurnOn">should the data point be turned on?</param>
        private void TurnPoint(int myColumnIndex, int myRowIndex, bool myTurnOn)
        {
            var comment = GetRemovedPointComment(myColumnIndex, myRowIndex);

            var dataPoint = new DataPointIndex(myColumnIndex, myRowIndex, comment);

            //if (myTurnOn == false)
            if (myTurnOn == true)
            {
                if (_turnedOffPoints.Contains(dataPoint))
                {
                    _turnedOffPoints.Remove(dataPoint);
                }
            }
            else
            {
                if (_turnedOffPoints.Contains(dataPoint) == false)
                {
                    _turnedOffPoints.Add(dataPoint);
                }
            }

            TurnPointInTables(dataPoint.ColumnIndex, dataPoint.RowIndex, myTurnOn);
        }

        /// <summary>
        ///     Turn a response data points in the table either ON or OFF. Called whenever the activated response
        ///     data columns are changed or new points are turned on or off. Double.NaN indicates a data point is turned off
        ///     in the response data table.
        /// </summary>
        /// <param name="myResponseColumnName">the name of the response data column</param>
        /// <param name="myRowIndex">index of row to turn ON/OFF</param>
        /// <param name="myTurnOn">should the data point be turned on?</param>
        private void TurnPointInTables(int myColumnIndex, int myRowIndex, bool myTurnOn)
        {
            //only bother with activated columns
            if (myColumnIndex >= 0 && myColumnIndex < _calculatedResponseTable.Columns.Count)
            {
                if (myTurnOn == false)
                {
                    _calculatedResponseTable.Rows[myRowIndex][myColumnIndex] = double.NaN;
                }
                else
                {
                    double value = Convert.ToDouble(_activatedResponseTable.Rows[myRowIndex][myColumnIndex]);

                    _calculatedResponseTable.Rows[myRowIndex][myColumnIndex] = value;
                }
            }
        }

        /// <summary>
        ///     Turn off response data points that are found at a given flaw size.
        /// </summary>
        /// <param name="myRowIndex">the row index where the flaw size is</param>
        /// <param name="myTurnOn">should the data point be turned on?</param>
        private void TurnPoints(int myRowIndex, bool myTurnOn)
        {
            for (int i = 0; i < ActivatedResponseNames.Count; i++ )
            {
                TurnPoint(i, myRowIndex, myTurnOn);
            }
        }
        //pass datatables from c# into the 
        public void UpdateData(bool quickFlag = false)
        {
            //only update data when appropriate
            if (_updatePythonData == true && _python != null && (_hmAnalysisObject!=null || _aHatAnalysisObject!=null))
            {
                //create list to store the flaws
                List<double> flaws = new List<double>();
                //create dictionary to store responses
                Dictionary<string, List<double>> responses = new Dictionary<string, List<double>>();
                //Create a dictionary to store ALL responses in the event the user censors data
                Dictionary<string, List<double>> allResponses = new Dictionary<string, List<double>>();
                //store the flaws in the list
                foreach (DataRow row in _activatedFlawTable.Rows)
                    flaws.Add((double)row[0]);
                //for each loop is used for more than one response column (such as multiple inspectors)
                foreach (DataColumn col in _calculatedResponseTable.Columns)
                {
                    List<double> list = new List<double>();

                    foreach (DataRow row in _calculatedResponseTable.Rows)
                        list.Add((double)row[col]);

                    responses.Add(col.ColumnName, list);
                }

                foreach (DataColumn col in _activatedResponseTable.Columns)
                {
                    List<double> list = new List<double>();

                    foreach (DataRow row in _activatedResponseTable.Rows)
                        list.Add((double)row[col]);

                    allResponses.Add(col.ColumnName, list);
                    if (_dataType == AnalysisDataTypeEnum.HitMiss)
                        _hmAnalysisObject.HitMiss_name = col.ColumnName;
                    else if (_dataType == AnalysisDataTypeEnum.AHat)
                        _aHatAnalysisObject.SignalResponseName = col.ColumnName;

                }
                if(_dataType == AnalysisDataTypeEnum.HitMiss)
                {
                    if (_hmAnalysisObject.Flaws_All.Count()==0 || quickFlag)
                    {
                        _hmAnalysisObject.Flaws_All = flaws;
                        //set the dataset size
                        _hmAnalysisObject.Count = _hmAnalysisObject.Flaws_All.Count();
                        List<double> logOfFlaws = new List<double>();
                        List<double> inverseOfFlaws = new List<double>();
                        for (int i=0; i<flaws.Count(); i++)
                        {
                            logOfFlaws.Add(Math.Log(flaws[i]));
                            inverseOfFlaws.Add(1.0 / flaws[i]);
                        }
                        _hmAnalysisObject.LogFlaws_All = logOfFlaws;                            
                        _hmAnalysisObject.InverseFlaws_All = inverseOfFlaws;
                    }
                    if (_hmAnalysisObject.Responses_all.Count() == 0 || quickFlag)
                        _hmAnalysisObject.Responses_all = allResponses;

                    _hmAnalysisObject.Flaws = flaws;
                    _hmAnalysisObject.Responses = responses;
                      
                }
                else if (_dataType == AnalysisDataTypeEnum.AHat)
                {
                    if (_aHatAnalysisObject.Flaws_All.Count() == 0 || quickFlag)
                    {
                        _aHatAnalysisObject.Flaws_All = flaws;
                        List<double> logOfFlaws = new List<double>();
                        for (int i = 0; i < flaws.Count(); i++)
                            logOfFlaws.Add(Math.Log(flaws[i]));
                        _aHatAnalysisObject.LogFlaws_All = logOfFlaws;
                    }
                    if (_aHatAnalysisObject.Responses_all.Count() == 0 || quickFlag)
                        _aHatAnalysisObject.Responses_all = allResponses;
                    //used for the ahat analysis obejct for RDotnet
                    _aHatAnalysisObject.Flaws = flaws;
                    _aHatAnalysisObject.Responses = responses;
                }
                
            }
        }

        #endregion

        #region Event Handling
        [OnDeserializing]
        private void SetUpdatePythonDataDefault(StreamingContext sc)
        {
            _updatePythonData = true;
        }
        #endregion

        public void SetPythonEngine(I_IPy4C myPy, string myAnalysisName)
        {
            if (myPy == null)
                return;
            _python = myPy;
            if (_hmAnalysisObject == null && _dataType == AnalysisDataTypeEnum.HitMiss)
                _hmAnalysisObject = _python.HitMissAnalsysis(myAnalysisName);
            else if (_aHatAnalysisObject == null && _dataType == AnalysisDataTypeEnum.AHat)
                _aHatAnalysisObject = _python.AHatAnalysis(myAnalysisName);
        }

        public void UpdateOutput(RCalculationType myCalculationType,
            IUpdateOutputForAHatData updateOutputForAHatDataIn=null,
            IUpdateOutputForHitMissData updateOutputForHitMissDataIn=null)
        {
            
            if (_dataType == AnalysisDataTypeEnum.AHat)
            {
                IUpdateOutputForAHatData updateOutputForAHatData = updateOutputForAHatDataIn ??
                    new UpdateOutputForAHatData(_aHatAnalysisObject);
                updateOutputForAHatData.UpdatePODCurveTable(ref _podCurveTable);
                updateOutputForAHatData.UpdatePODCurveAllTable(ref _podCurveTable_All);
                if (myCalculationType == RCalculationType.ThresholdChange)
                    return;
                updateOutputForAHatData.UpdateFitResidualsTable(ref _fitResidualsTable);
                updateOutputForAHatData.UpdateResidualUncensoredTable(ref _residualUncensoredTable);
                updateOutputForAHatData.UpdateResidualRawTable(ref _residualRawTable);
                updateOutputForAHatData.UpdateResidualCensoredTable(ref _residualCensoredTable, _residualRawTable);
                updateOutputForAHatData.UpdateResidualFullCensoredTable(ref _residualFullCensoredTable, _residualRawTable);
                updateOutputForAHatData.UpdateResidualPartialCensoredTable(ref _residualPartialCensoredTable);
                updateOutputForAHatData.UpdateThresholdCurveTable(ref _thresholdPlotTable);
                updateOutputForAHatData.UpdateThresholdCurveTableAll(ref _thresholdPlotTable_All);
                updateOutputForAHatData.UpdateNormalityTable(ref _normalityTable, ref _normalityCurveTable);
            }
            else
            {
                IUpdateOutputForHitMissData updateOutputForHitMissData = updateOutputForHitMissDataIn ??
                new UpdateOutputForHitMissData(_hmAnalysisObject);
                updateOutputForHitMissData.UpdateOriginalData(ref _originalData);
                updateOutputForHitMissData.UpdateTotalFlawCount(ref _totalFlawCount);
                updateOutputForHitMissData.UpdatePODCurveTable(ref _podCurveTable);
                updateOutputForHitMissData.UpdateResidualUncensoredTable(ref _residualUncensoredTable);
                updateOutputForHitMissData.UpdateResidualPartialUncensoredTable(ref _residualPartialCensoredTable);
                updateOutputForHitMissData.UpdateIterationsTable(ref _iterationsTable);
            }
        }
        public List<string> ChangeTableColumnNames(DataTable myTable, List<string> myNewNames)
        {
            var oldNames = new List<string>();

            if (myTable != null)
            {         
                for (int i = 0; i < myNewNames.Count; i++)
                {
                    if (i < myTable.Columns.Count)
                    {
                        oldNames.Add(myTable.Columns[i].ColumnName);
                        myTable.Columns[i].ColumnName = myNewNames[i];
                    }
                }
            }

            return oldNames;
        }
        // Perhaps add an assertion to make sure that myWriter is an object and not an interface
        // when the application is running?
        public void WriteToExcel(IExcelExport myWriter, string myAnalysisName, string myWorksheetName, bool myPartOfProject = true,
            IExcelWriterControl excelWriteControlIn=null)
        {
            IExcelWriterControl excelWriteControl = excelWriteControlIn ??
                new ExcelWriterControl(myWriter, myAnalysisName, myWorksheetName, myPartOfProject);
            excelWriteControl.WriteResidualsToExcel(this, _residualCensoredTable);
            excelWriteControl.WritePODToExcel(this, _podEndIndex);
            if(_dataType == AnalysisDataTypeEnum.HitMiss)
                excelWriteControl.WriteIterationsToExcel(this, _iterationsTable);
            else
                excelWriteControl.WritePODThresholdToExcel(this, _thresholdPlotTable);
            excelWriteControl.WriteRemovedPointsToExcel(this, _podCurveTable_All,
                _thresholdPlotTable, _thresholdPlotTable_All);

        }

        public string AdditionalWorksheet1Name
        {
            get
            {
                var worksheetName = Globals.NotApplicable;

                if (_dataType == AnalysisDataTypeEnum.AHat)
                    worksheetName = "Threshold";
                else if (_dataType == AnalysisDataTypeEnum.HitMiss)
                    worksheetName = "Solver";

                return worksheetName;
            }
        }
        public DataTable GenerateRemovedPointsTable()
        {
            DataTable table = new DataTable();

            table.Columns.Add("Index", typeof(string));
            table.Columns.Add("Flaw Name", typeof(string));
            table.Columns.Add("Response Name", typeof(string));
            table.Columns.Add("ID", typeof(string));
            table.Columns.Add("Flaw", typeof(Double));
            table.Columns.Add("Response", typeof(Double));
            table.Columns.Add("Comment", typeof(string));

            foreach(DataPointIndex index in _turnedOffPoints)
            {
                DataRow row = table.NewRow();

                row["Flaw"] = _activatedFlawTable.Rows[index.RowIndex][0];
                row["Flaw Name"] = _activatedFlaws[0];
                row["Response"] = _activatedResponseTable.Rows[index.RowIndex][index.ColumnIndex];
                row["Response Name"] = _activatedResponses[index.ColumnIndex];
                row["ID"] = _activatedSpecIDTable.Rows[index.RowIndex][0].ToString();
                row["Index"] = index.RowIndex;

                var comment = GetRemovedPointComment(index.ColumnIndex, index.RowIndex).Trim();

                if (comment.Length == 0)
                    comment = "ANALYST DID NOT COMMENT ON WHY POINT WAS REMOVED FROM THE ANALYSIS!";

                row["Comment"] = comment;

                table.Rows.Add(row);
            }

            table.DefaultView.Sort = "flaw, response, index" + " " + "ASC";
            table = table.DefaultView.ToTable();



            return table;
        }

        /// <summary>
        /// Returns the transformed min uncensored flaw size
        /// </summary>
        public double UncensoredFlawRangeMin
        {
            get
            {
                if (_dataType == AnalysisDataTypeEnum.HitMiss)
                    switch (_flawTransform)
                    {
                        case TransformTypeEnum.Log:
                            return _hmAnalysisObject.LogFlaws_All.Min();
                        case TransformTypeEnum.Inverse:
                            return _hmAnalysisObject.InverseFlaws_All.Min();
                        default:
                            return _hmAnalysisObject.Flaws_All.Min();
                    }
                else
                    switch (_flawTransform)
                    {
                        case TransformTypeEnum.Log:
                            return _aHatAnalysisObject.LogFlaws_All.Min();
                        default:
                            return _aHatAnalysisObject.Flaws_All.Min();
                    }
            }
        }

        /// <summary>
        /// Returns the transformed min flaw size with at least 1 response associated with it
        /// </summary>
        public double FlawRangeMin
        {
            get
            {
                if (_dataType == AnalysisDataTypeEnum.HitMiss && _hmAnalysisObject?.Flaws.Count > 0)
                    return _hmAnalysisObject.Flaws.Min();
                else if (_dataType == AnalysisDataTypeEnum.AHat && _aHatAnalysisObject?.Flaws.Count > 0)
                    return _aHatAnalysisObject.Flaws.Min();
                return double.NaN;
            }
        }

        /// <summary>
        /// Returns the transformed max uncensored flaw size
        /// </summary>
        public double UncensoredFlawRangeMax
        {
            get
            {

                if (_dataType == AnalysisDataTypeEnum.HitMiss)
                    switch (_flawTransform)
                    {
                        case TransformTypeEnum.Log:
                            return _hmAnalysisObject.LogFlaws_All.Max();
                        case TransformTypeEnum.Inverse:
                            return _hmAnalysisObject.InverseFlaws_All.Max();
                        default:
                            return _hmAnalysisObject.Flaws_All.Max();
                    }
                else
                    switch (_flawTransform)
                    {
                        case TransformTypeEnum.Log:
                            return _aHatAnalysisObject.LogFlaws_All.Max();
                        default:
                            return _aHatAnalysisObject.Flaws_All.Max();
                    }
            }
        }

        /// <summary>
        /// Returns the transformed max flaw size with at least 1 response associated with it
        /// </summary>
        public double FlawRangeMax
        {
            get
            {
                if (_dataType == AnalysisDataTypeEnum.HitMiss && _hmAnalysisObject?.Flaws.Count > 0)
                    return _hmAnalysisObject.Flaws.Max();
                else if (_dataType == AnalysisDataTypeEnum.AHat && _aHatAnalysisObject?.Flaws.Count > 0)
                    return _aHatAnalysisObject.Flaws.Max();        
                return double.NaN;
            }
        }

        /// <summary>
        /// Invert a transformed flaw value.
        /// </summary>
        /// <param name="myValue"></param>
        /// <returns></returns>
        public double InvertTransformedFlaw(double myValue)
        {
            //FlawTransform
            if (_hmAnalysisObject!=null || _aHatAnalysisObject != null)
                return TransformBackAValue(myValue, _python.TransformEnumToInt(_flawTransform));
            else
                return myValue;
        }

        /// <summary>
        /// Invert a transformed response value.
        /// </summary>
        /// <param name="myValue"></param>
        /// <returns></returns>
        public double InvertTransformedResponse(double myValue)
        {
            if (_hmAnalysisObject != null || _aHatAnalysisObject != null)
                if(_dataType == AnalysisDataTypeEnum.AHat)
                    return TransformBackAValue(myValue, _python.TransformEnumToInt(_responseTransform));
            return myValue;
        }

        /// <summary>
        /// Get the number of flaws used in the analysis (censored or uncensored)
        /// </summary>
        public int FlawCount
        {
            get
            {
                switch (_dataType)
                {
                    case AnalysisDataTypeEnum.AHat when _residualUncensoredTable != null && _residualCensoredTable != null:
                        return _residualUncensoredTable.Rows.Count + _residualCensoredTable.Rows.Count;
                    case AnalysisDataTypeEnum.HitMiss:
                        return _totalFlawCount;
                    default:
                        return 0;
                }
            }
        }

        /// <summary>
        /// Get the number of flaws used in the analysis (censored or uncensored), AHat vs A might need to be implmented here
        /// </summary>
        public int FlawCountUnique
        {
            get
            {
                if (_dataType == AnalysisDataTypeEnum.HitMiss)
                    return _residualUncensoredTable?.Rows.Count ?? 0;
                return 0;
            }
        }
        //**********IMPORTANT NOTE
        // lines 2486 to 2524 are used for writing to excel only(will need to implement this in r)
        //does not seem to return anything special in python code either
        public int ResponsePartialBelowMinCount
        {
            get
            {
                //return _podDoc.GetFlawCountPartialBelowResponseMin();
                return -1;
            }
        }

        public int ResponsePartialAboveMaxCount
        {
            get
            {
                //return _podDoc.GetFlawCountPartialAboveResponseMax();
                return -1;
            }
        }

        public int ResponseCompleteBelowMinCount
        {
            get
            {
                //return _podDoc.GetFlawCountFullBelowResponseMin();
                return -1;
            }
        }

        public int ResponseCompleteAboveMaxCount
        {
            get
            {
                //return _podDoc.GetFlawCountFullAboveResponseMax();
                return -1;
            }
        }

        public int ResponseBetweenCount
        {
            get
            {
                //return _podDoc.GetFlawCountUncensored();
                return -1;
            }
        }

        public void GetXYBufferedRanges(Control chart, IAxisObject myXAxis, IAxisObject myYAxis, bool myGetTransformed)
        {
            GetXBufferedRange(chart, myXAxis, myGetTransformed);
            GetYBufferedRange(chart, myYAxis, myGetTransformed);
        }

        public static void GetMinMaxOfTable(DataTable myTable, ref double myMin, ref double myMax)
        {
            myMax = Compute.InitMaxValue;
            myMin = Compute.InitMinValue;

            if(myTable.Rows.Count > 0)
            {
                foreach (DataColumn col in myTable.Columns)
                {
                    Compute.MinMax(myTable, col, ref myMin, ref myMax);
                }
            }

            Compute.SanityCheck(ref myMin, ref myMax);
        }

        public static void GetBufferedRange(Control chart, IAxisObject myAxis, DataTable myTable, AxisKind kind)
        {
            double myMin = Double.MaxValue;
            double myMax = Double.MinValue;

            GetMinMaxOfTable(myTable, ref myMin, ref myMax);

            GetBufferedRange(chart, myAxis, myMin, myMax, kind);
        }

        public static void GetBufferedRange(Control chart, IAxisObject myAxis, double myMin, double myMax, AxisKind kind)
        {
            if (myMin != myMax)
            {
                double range = Math.Abs(myMax - myMin);
                double logRange = Math.Floor(Math.Log10(range)) - 1;
                //double logRange = Math.Floor(Math.Log(range)) - 1;
                double buffer = myAxis.BufferPercentage / 100.0 * range;
                double max = myMax + buffer;
                double min = myMin - buffer;
                double correctedRange = Math.Pow(10.0, logRange);
                //double correctedRange = Math.Exp(logRange);

                max = Math.Ceiling(max / correctedRange) * correctedRange;
                min = Math.Floor(min / correctedRange) * correctedRange;
                //max =Math.Round(max, 2);
                //min = Math.Round(min, 2);

                range = Math.Abs(max - min);

                myAxis.Interval = correctedRange;
                double multiplier = 1.0;

                var count = Globals.GetLabelIntervalBasedOnChartSize(chart, kind);

                while (range / myAxis.Interval > count)
                {
                    myAxis.Interval = correctedRange * multiplier;

                    multiplier += .5;
                    //multiplier += .95;
                }

                myAxis.Max = max;
                myAxis.Min = min;
            }
            else
            {
                myAxis.Max = ++myMax;
                myAxis.Min = --myMin;
                
                myAxis.Interval = .25;
            }
        }

        public void GetYBufferedRange(Control chart, IAxisObject myAxis, bool myGetTransformed)
        {
            DataTable table;

            if (myGetTransformed)
                table = ActivatedResponses;
            else
                table = _activatedResponseTable;

            GetBufferedRange(chart, myAxis, table, AxisKind.Y);
        }

        public void GetXBufferedRange(Control chart, IAxisObject myAxis, bool myGetTransformed)
        {
            DataTable table;

            if (myGetTransformed)
                table = ActivatedFlaws;
            else
                table = _activatedFlawTable;

            GetBufferedRange(chart, myAxis, table, AxisKind.X);
        }

        public void GetUncensoredXBufferedRange(Control chart, AxisObject myAxis, bool myGetTransformed)
        {
            DataTable table;
            var isLinear = !myGetTransformed || _flawTransform == TransformTypeEnum.Linear || _flawTransform == TransformTypeEnum.Inverse;
            
            if (_dataType == AnalysisDataTypeEnum.HitMiss)
            {
                _hmAnalysisObject.ModelType = _python.TransformEnumToInt(_flawTransform);
            }
            else if (_dataType == AnalysisDataTypeEnum.AHat)
            {
                _aHatAnalysisObject.A_transform= _python.TransformEnumToInt(_flawTransform);
            }

            if (_dataType == AnalysisDataTypeEnum.AHat)
            {
                _aHatAnalysisObject.Ahat_transform = _python.TransformEnumToInt(_flawTransform);
                AHatModelUpdate();
            }

            
            AxisObject maxAxis = new AxisObject();
            GetXBufferedRange(chart, maxAxis, false);

            if (myGetTransformed)
                table = ActivatedFlaws;
            else
                table = _activatedFlawTable;

            double myMin = Double.MaxValue;
            double myMax = Double.MinValue;            

            double uncensoredMin = double.MaxValue;
            double uncensoredMax = double.MinValue;
            double partialMin = double.MaxValue;
            double partialMax = double.MinValue;
            double podMin = double.MaxValue;
            double podMax = double.MinValue;
            double podMinThrowAway = double.MaxValue;
            double podMaxThrowAway = double.MinValue;
            double xMax = maxAxis.Max; //this.MaxFlaw;
            double xMin = maxAxis.Min;//this.MinFlaw;

            if (ResidualPartialCensoredTable != null)
            {
                var partialCol = ResidualPartialCensoredTable.Columns["flaw"];
                Compute.MinMax(ResidualPartialCensoredTable, partialCol, ref partialMin, ref partialMax);
            }

            var uncensoredCol = ResidualUncensoredTable.Columns["flaw"];
            var podCol = PodCurveTable.Columns["flaw"];
            var podPODCol = PodCurveTable.Columns["pod"];
            var podPOD95Col = PodCurveTable.Columns["confidence"];
        
            if(podCol == null)
                podCol = PodCurveTable.Columns["a"];

            if (podCol == null)
                podCol = PodCurveTable.Columns[0];

            if (podPODCol == null)
                podPODCol = PodCurveTable.Columns["POD(a)"];

            if (podPODCol == null)
                podPODCol = PodCurveTable.Columns[1];

            if (podPOD95Col == null)
                podPOD95Col = PodCurveTable.Columns["95 confidence bound"];

            if (podPOD95Col == null)
                podPOD95Col = PodCurveTable.Columns["confidence pod"];

            if (podPOD95Col == null)
                podPOD95Col = PodCurveTable.Columns[2];


            Compute.MinMax(ResidualUncensoredTable, uncensoredCol, ref uncensoredMin, ref uncensoredMax);
            Compute.MinMax(PodCurveTable, podCol, ref podMin, ref podMaxThrowAway, .5, 1.0, podPODCol);
            Compute.MinMax(PodCurveTable, podCol, ref podMinThrowAway, ref podMax, 0.0, .9, podPOD95Col);



            var mins = new List<double>(new double[] { podMin, uncensoredMin, partialMin });
            var maxes = new List<double>(new double[] { podMax, uncensoredMax, partialMax });

            myMin = mins.Min();//InvertTransformedFlaw(UncensoredFlawRangeMin);
            myMax = maxes.Max();//InvertTransformedFlaw(UncensoredFlawRangeMax);

            if (myMin < xMin)
                myMin = xMin;

            if (myMax > xMax)
                myMax = xMax;

            GetBufferedRange(null, myAxis, myMin, myMax, AxisKind.X);

            
        }

        public AxisObject GetXBufferedRange(Control chart, bool myGetTransformed)
        {
            AxisObject axis = new AxisObject();

            GetXBufferedRange(chart, axis, myGetTransformed);

            return axis;
        }
        private void AHatModelUpdate()
        {
            //linear- linear
            if (_aHatAnalysisObject.A_transform == 1 && _aHatAnalysisObject.Ahat_transform == 1)
            {
                _aHatAnalysisObject.ModelType = 1;
            }
            //log - linear
            else if (_aHatAnalysisObject.A_transform == 2 && _aHatAnalysisObject.Ahat_transform == 1)
            {
                _aHatAnalysisObject.ModelType = 2;
            }
            //linear- log
            else if (_aHatAnalysisObject.A_transform == 1 && _aHatAnalysisObject.Ahat_transform == 2)
            {
                _aHatAnalysisObject.ModelType = 3;
            }
            // log - log
            else if (_aHatAnalysisObject.A_transform == 2 && _aHatAnalysisObject.Ahat_transform == 2)
            {
                _aHatAnalysisObject.ModelType = 4;
            }
            // linear - box-cox
            else if (_aHatAnalysisObject.A_transform == 1 && _aHatAnalysisObject.Ahat_transform == 5)
            {
                _aHatAnalysisObject.ModelType = 5;
            }
            // log - boxcox
            else if (_aHatAnalysisObject.A_transform == 2 && _aHatAnalysisObject.Ahat_transform == 5)
            {
                _aHatAnalysisObject.ModelType = 6;
            }
            // inverse - boxcox
            else if (_aHatAnalysisObject.A_transform == 3 && _aHatAnalysisObject.Ahat_transform == 5)
            {
                _aHatAnalysisObject.ModelType = 7;
            }
            // linear - inverse
            else if (_aHatAnalysisObject.A_transform == 1 && _aHatAnalysisObject.Ahat_transform == 3)
            {
                _aHatAnalysisObject.ModelType = 8;
            }
            // log - inverse
            else if (_aHatAnalysisObject.A_transform == 2 && _aHatAnalysisObject.Ahat_transform == 3)
            {
                _aHatAnalysisObject.ModelType = 9;
            }
            // inverse - linear
            else if (_aHatAnalysisObject.A_transform == 3 && _aHatAnalysisObject.Ahat_transform == 1)
            {
                _aHatAnalysisObject.ModelType = 10;
            }
            // inverse - log
            else if (_aHatAnalysisObject.A_transform == 3 && _aHatAnalysisObject.Ahat_transform == 2)
            {
                _aHatAnalysisObject.ModelType = 11;
            }
            // inverse x - inverse y
            else if (_aHatAnalysisObject.A_transform == 3 && _aHatAnalysisObject.Ahat_transform == 3)
            {
                _aHatAnalysisObject.ModelType = 12;
            }
        }
        public AxisObject GetUncensoredXBufferedRange(Control chart, bool myGetTransformed)
        {
            AxisObject axis = new AxisObject();

            GetUncensoredXBufferedRange(chart, axis, myGetTransformed);

            return axis;
        }

        public AxisObject GetYBufferedRange(Control chart, bool myGetTransformed)
        {
            AxisObject axis = new AxisObject();

            GetYBufferedRange(chart, axis, myGetTransformed);

            return axis;
        }

        public double DoNoTransform(double myValue)
        {
            return myValue;
        }
        public double TransformAValue(double myValue, int transform)
        {
            switch (transform)
            {
                case 2: //log transform
                    return Math.Log(myValue);
                case 3: //inverse transform
                    return 1.0 / myValue;
                case 5: //BoxCox transform
                    return (Math.Pow(myValue, _aHatAnalysisObject.Lambda) - 1) / _aHatAnalysisObject.Lambda;
                default: //linear or unknown transform
                    return myValue;
            }
        }
        public ITransformBackLambdaControl TransformBackLambda { get; set; }
        public double TransformBackAValue(double myValue, int transform)
        {
            ITransformBackLambdaControl transformBackLambdaControl = TransformBackLambda ?? new TransformBackLambdaControl(_aHatAnalysisObject);
            switch (transform)
            {
                case 2:
                    return Math.Exp(myValue);
                case 3:
                    return 1.0 / myValue;
                case 5:
                   return transformBackLambdaControl.TransformBackLambda(myValue);
                default:
                    return myValue;
            }
        }
        public double TransformValueForXAxis(double myValue)
        {
            if (myValue <= 0.0 && (_flawTransform == TransformTypeEnum.Log || _flawTransform == TransformTypeEnum.Inverse))
                return 0.0;

            return TransformAValue(myValue, _python.TransformEnumToInt(_flawTransform));
        }
        
        public double TransformValueForYAxis(double myValue)
        {
            if (myValue <= 0.0 && (_responseTransform == TransformTypeEnum.Log || _responseTransform == TransformTypeEnum.Inverse))
                return 0.0;

            return TransformAValue(myValue, _python.TransformEnumToInt(_responseTransform));
        }

        public double InvertTransformValueForXAxis(double myValue)
        {
            if (myValue == 0.0 && _flawTransform == TransformTypeEnum.Inverse)
                return 0.0;

            return TransformBackAValue(myValue, _python.TransformEnumToInt(_flawTransform));

        }

        public double InvertTransformValueForYAxis(double myValue)
        {
            if (myValue == 0.0 && _responseTransform == TransformTypeEnum.Inverse)
                return 0.0;
            return TransformBackAValue(myValue, _python.TransformEnumToInt(_responseTransform));
        }
        public double SmallestFlaw
        {
            get
            {
                return _smallestFlawSize;
            }
        }

        public double SmallestResponse
        {
            get
            {
                return _smallestResponse;
            }
        }

        public string FlawTransFormLabel
        {
            get
            {
                switch (_flawTransform)
                {
                    case TransformTypeEnum.Log:
                        return "ln(a)";
                    case TransformTypeEnum.Linear:
                        return "{{a}}";
                    case TransformTypeEnum.Exponetial:
                        return "e^a";
                    case TransformTypeEnum.Inverse:
                        return "1/a";
                    default:
                        return "Custom";
                }
            }
        }

        public string ResponseTransformLabel
        {
            get
            {
                switch (_responseTransform)
                {
                    case TransformTypeEnum.Log:
                        return "ln(ahat)";
                    case TransformTypeEnum.Linear:
                        return "{{ahat}}";
                    case TransformTypeEnum.Exponetial:
                        return "e^ahat";
                    case TransformTypeEnum.Inverse:
                        return "1/ahat";
                    case TransformTypeEnum.BoxCox:
                        return "[(ahat)^(lambda)-1]/lambda";
                    default:
                        return "Custom";
                }
            }
        }



        public void UpdateSourceFromInfos(SourceInfo sourceInfo, ITableUpdaterFromInfos tableUpdaterFromInfosIn = null)
        {
            //UpdateTableFromInfos(sourceInfo, ColType.Flaw, _availableFlawsTable, _activatedFlawTable, _availableFlaws, _activatedFlaws);
            //UpdateTableFromInfos(sourceInfo, ColType.Response, _availableResponsesTable, _activatedResponseTable, _availableResponses, _activatedResponses);
            ITableUpdaterFromInfos tableUpdaterFromInfos = tableUpdaterFromInfosIn ?? new TableUpdaterFromInfos();
            tableUpdaterFromInfos.UpdateTableFromInfos(sourceInfo, ColType.Flaw, _availableFlawsTable, _activatedFlawTable, _availableFlaws, _activatedFlaws);
            tableUpdaterFromInfos.UpdateTableFromInfos(sourceInfo, ColType.Response, _availableResponsesTable, _activatedResponseTable, _availableResponses, _activatedResponses);
        }
        /// These setters are for Unit Testing only, They cannot be injected as function arguments because 
        /// then certain tests in Analysis.cs will throw a compilation error
        public IUpdaterExcelPropertyValue UpdaterExcelProp { set; private get; }
        public IDataTableWrapper AvailableFlawsTable { set; private get; }
        public IDataTableWrapper AvailableResponsesTable { set; private get; }
        /// <remarks>
        /// DataTables and Excel property value injected for unit testing purposes
        /// </remarks>
        public void GetUpdatedValue(ColType myType, string myExtColProperty, double currentValue, out double newValue)
        {
            IUpdaterExcelPropertyValue updaterExcelProp = UpdaterExcelProp ?? new UpdaterExcelPropertyValue();
            IDataTableWrapper availableFlawsTable = AvailableFlawsTable ?? new DataTableWrapper(_availableFlawsTable);
            IDataTableWrapper availableResponsesTable = AvailableResponsesTable ?? new DataTableWrapper(_availableResponsesTable);

            DataColumnCollection columns = null;
            var values = new List<double>();

            if (myType == ColType.Flaw)
                columns = availableFlawsTable.Columns;
            else if (myType == ColType.Response)
                columns = availableResponsesTable.Columns;
            else
            {
                newValue = currentValue;
                throw new ArgumentException("Column Type: " + myType.ToString() + " is not valid for GetUpdatedValue");
            }

            foreach (DataColumn column in columns)
            {
                values.Add(updaterExcelProp.GetUpdatedValue(myExtColProperty, currentValue, column));
            }
            if (myExtColProperty == ExtColProperty.Min)
            {
                newValue = values.Min();
            }
            else if (myExtColProperty == ExtColProperty.Max)
            {
                newValue = values.Max();
            }
            else if (myExtColProperty == ExtColProperty.Thresh)
            {
                newValue = values.Min();
            }
            else
            {
                newValue = currentValue;

                throw new Exception("ExtColProprty: " + myExtColProperty + " is not valid.");
            }
            /*
            switch (myExtColProperty)
            {
                case ExtColProperty.Max:
                    newValue = values.Max();
                    break;
                case ExtColProperty.Min:
                case ExtColProperty.Thresh:
                    newValue = values.Min();
                    break;
                default:
                    newValue = currentValue;
                    throw new Exception("ExtColProprty: " + myExtColProperty + " is not valid.");
            }*/
        }

        public void GetNewValue(ColType myType, string myExtColProperty, out double newValue)
        {
            DataColumnCollection columns = null;
            var values = new List<double>();

            if (myType == ColType.Flaw)
                columns = _availableFlawsTable.Columns;
            else if (myType == ColType.Response)
                columns = _availableResponsesTable.Columns;

            foreach (DataColumn column in columns)
            {
                values.Add(GetNewValue(myExtColProperty, column));
            }

            if (myExtColProperty == ExtColProperty.Min)
            {
                newValue = values.Min();
            }
            else if (myExtColProperty == ExtColProperty.Max)
            {
                newValue = values.Max();
            }
            else if (myExtColProperty == ExtColProperty.Thresh)
            {
                newValue = values.Min();
            }
            else
            {
                throw new Exception("ExtColProprty: " + myExtColProperty + " is not valid.");
            }
        }

        private double GetNewValue(string myExtColProperty, DataColumn column)
        {
            double newValue;
            double newTableValue = 0.0;


            if (!Double.TryParse(column.ExtendedProperties[myExtColProperty].ToString(), out newTableValue))
                newTableValue = 0.0;

            newValue = newTableValue;

            return newValue;
        }

        private static double GetUpdatedValue(string myExtColProperty, double currentValue, DataColumn column)
        {
            double newValue;
            var prevString = "";

            if (myExtColProperty == ExtColProperty.Min)
                prevString = ExtColProperty.MinPrev;
            else if (myExtColProperty == ExtColProperty.Max)
                prevString = ExtColProperty.MaxPrev;
            else if (myExtColProperty == ExtColProperty.Thresh)
                prevString = ExtColProperty.ThreshPrev;

            if (prevString == "")
            {
                throw new Exception("ExtColProprty: " + myExtColProperty + " is not valid.");
            }

            double prevValue = 0.0;
            double newTableValue = 0.0;


            if (!Double.TryParse(GetExtendedProperty(column, prevString), out prevValue))
                prevValue = 0.0;

            if (!Double.TryParse(GetExtendedProperty(column, myExtColProperty), out newTableValue))
                newTableValue = 0.0;

            if (currentValue == prevValue)
                newValue = newTableValue;
            else
                newValue = currentValue;
            return newValue;
        }

        private static string GetExtendedProperty(DataColumn column, string colType)
        {
            string value = "";

            if (column == null)
            {
                value = ExtColProperty.GetDefaultValue(colType);
                return value;
            }

            if (column.ExtendedProperties.ContainsKey(colType))
                value = column.ExtendedProperties[colType].ToString();
            else
            {
                value = ExtColProperty.GetDefaultValue(colType);
                column.ExtendedProperties[colType] = value;
            }

            return value;
        }

        

        public void SetSource(DataSource source, string flawName, List<string> responses)
        {
            SetSource(source, new List<string>(new string[] {flawName}) , source.MetaDataLabels, responses, source.IDLabels);
        }



        public DataTable TransformedInput
        {
            get
            {
                var flaws = ActivatedTransformedFlaws;
                var responses = ActivatedTransformedResponses;
                var column = new DataColumn(ActivatedFlawName, typeof(double));

                responses.Columns.Add(column);

                column.SetOrdinal(0);

                for (int index = 0; index < flaws.Rows.Count; index++ )
                {
                    responses.Rows[index][0] = flaws.Rows[index][0];
                }

                responses.AcceptChanges();

                return responses;
            }
        }

        public bool FilterTransformedDataByRanges
        {
            get
            {
                return _filterByRanges;
            }
            set
            {
                _filterByRanges = value;
            }
        }

        public bool IsResponseTable(DataTable mySourceTable)
        {
            return _activatedResponseTable == mySourceTable;
        }

        public bool IsFlawTable(DataTable mySourceTable)
        {
            return _activatedFlawTable == mySourceTable;
        }

        public void CreateNewSortList()
        {
            sortByX = new List<SortPoint>();
        }

        public void UpdateTable(int rowIndex, int colIndex, Flag bounds)
        {
            switch (bounds)
            {
                case Flag.InBounds:
                    TurnOnPoint(colIndex, rowIndex);
                    break;
                case Flag.OutBounds:
                    TurnOffPoint(colIndex, rowIndex);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("bounds must be either InBounds or OutBounds");
            }
        }

        public void UpdateIncludedPointsBasedFlawRange(double aboveX, double belowX, List<FixPoint> fixPoints)
        {
            if (sortByX.Any())
            {
                //keep track if they actually found the values in the data points
                //var aboveDoesNotInclude = false;
                var belowDoesNotInclude = false;



                int xAboveIndex = sortByX.BinarySearch(new SortPoint { XValue = aboveX });
                int xBelowIndex = sortByX.BinarySearch(new SortPoint { XValue = belowX });

                if (xAboveIndex < 0)
                {
                    xAboveIndex = ~xAboveIndex;
                    //aboveDoesNotInclude = true;
                }

                if (xBelowIndex < 0)
                {
                    xBelowIndex = ~xBelowIndex;
                    belowDoesNotInclude = true;
                }

                // remove points from out of bounds
                // else add points to out of bounds
                if (xAboveIndex > _prevAbove)
                {
                    for (int i = _prevAbove; i < xAboveIndex; i++)
                    {
                        fixPoints.Add(new FixPoint(sortByX[i].SeriesPtIndex, sortByX[i].SeriesIndex, Flag.InBounds));
                        UpdateTable(sortByX[i].RowIndex, sortByX[i].ColIndex, Flag.InBounds);
                    }
                }
                else if (xAboveIndex < _prevAbove)
                {
                    int indexL = xAboveIndex;

                    if (indexL >= sortByX.Count)
                    {
                        indexL = sortByX.Count - 1;
                    }

                    int indexR = _prevAbove;

                    if (indexR >= sortByX.Count)
                    {
                        indexR = sortByX.Count - 1;
                    }

                    for (int i = indexR; i >= indexL; i--)
                    {
                        fixPoints.Add(new FixPoint(sortByX[i].SeriesPtIndex, sortByX[i].SeriesIndex, Flag.OutBounds));
                        UpdateTable(sortByX[i].RowIndex, sortByX[i].ColIndex, Flag.OutBounds);
                    }
                }


                // Repeat for below line
                if (xBelowIndex < _prevBelow)
                {
                    int indexL = xBelowIndex;

                    if (indexL >= sortByX.Count)
                    {
                        indexL = sortByX.Count - 1;
                    }

                    //if max line went below min line then shift index over
                    //so last out of bounds point isn't added back in
                    if (_prevBelow == xAboveIndex)
                        _prevBelow--;

                    int indexR = _prevBelow;

                    if (_prevBelowDoesNotInclude)
                        indexR--;

                    if (indexR >= sortByX.Count)
                    {
                        indexR = sortByX.Count - 1;
                    }

                    for (int i = indexR; i >= indexL; i--)
                    {
                        if (i >= 0)
                        {
                            fixPoints.Add(new FixPoint(sortByX[i].SeriesPtIndex, sortByX[i].SeriesIndex, Flag.InBounds));
                            UpdateTable(sortByX[i].RowIndex, sortByX[i].ColIndex, Flag.InBounds);
                        }
                    }
                }
                else if (xBelowIndex > _prevBelow)
                {
                    for (int i = _prevBelow; i < xBelowIndex; i++)
                    {
                        fixPoints.Add(new FixPoint(sortByX[i].SeriesPtIndex, sortByX[i].SeriesIndex, Flag.OutBounds));
                        UpdateTable(sortByX[i].RowIndex, sortByX[i].ColIndex, Flag.OutBounds);
                    }
                }


                // update the prevIndex values for next check
                _prevAbove = xAboveIndex;
                _prevBelow = xBelowIndex;
                _prevBelowDoesNotInclude = belowDoesNotInclude;


            }
        }

        public void ToggleResponse(double pointX, double pointY, string seriesName, int rowIndex, int colIndex, List<FixPoint> fixPoints)
        {
            int index = sortByX.BinarySearch(new SortPoint { XValue = pointX, YValue = pointY, SeriesName = seriesName, RowIndex = rowIndex, ColIndex = colIndex });
            bool skipIndex = false;

            if (index < 0)
            {
                index = ~index;
                if (index > sortByX.Count)
                {
                    skipIndex = true;
                }
            }

            if(!skipIndex)
            {
                SortPoint foundPoint = sortByX[index];

                var comment = GetRemovedPointComment(foundPoint.ColIndex, foundPoint.RowIndex);
                var dpIndex = new DataPointIndex(foundPoint.ColIndex, foundPoint.RowIndex, comment);

                if (TurnedOffPoints.Contains(dpIndex))
                {
                    UpdateTable(foundPoint.RowIndex, foundPoint.ColIndex, Flag.InBounds);
                    fixPoints.Add(new FixPoint(foundPoint.SeriesPtIndex, foundPoint.SeriesIndex, Flag.InBounds));
                }
                else
                {
                    UpdateTable(foundPoint.RowIndex, foundPoint.ColIndex, Flag.OutBounds);
                    fixPoints.Add(new FixPoint(foundPoint.SeriesPtIndex, foundPoint.SeriesIndex, Flag.OutBounds));
                }
            }
        }

        public void ForceRefillSortListAndClearPoints()
        {
            FixMissingSortList();

            _turnedOffPoints.Clear();
            _prevAbove = 0;
            _prevBelow = 0;
            _prevBelowDoesNotInclude = false;

            sortByX.Clear();

            RefillSortList();


        }

        public void ForceRefillSortList()
        {
            FixMissingSortList();

            sortByX.Clear();

            RefillSortList();

            
        }

        private void FixMissingSortList()
        {
            if (sortByX == null)
                sortByX = new List<SortPoint>();
        }

        public void RefillSortList()
        {
            FixMissingSortList();

            if (!sortByX.Any())
            {
                //first Xth series will always be data series
                //everything after that is additional helper series
                for (int i = 0; i < ActivatedResponses.Columns.Count; i++)
                {
                    FillLists(ActivatedFlaws, ActivatedResponses, i);
                }
            }
        }

        private void FillLists(DataTable flawTable, DataTable responseTable, int index)
        {
            FillFromTableSeries(sortByX, flawTable, responseTable, index);
            sortByX.Sort();
        }

        private void FillFromTableSeries(List<SortPoint> sortPoints, DataTable flawTable,
                                         DataTable responseTable, int index)
        {
            string seriesName = responseTable.Columns[index].ColumnName;
            int seriesIndex = index + 3;
            //Series series = null;

            for (int i = 0; i < flawTable.Rows.Count; i++)
            {
                DataRow row = flawTable.Rows[i];

                var sp = new SortPoint
                {
                    ColIndex = responseTable.Columns.IndexOf(seriesName),
                    RowIndex = i,
                    SeriesName = seriesName,
                    SeriesIndex = seriesIndex,
                    SeriesPtIndex = i,
                    XValue = Convert.ToDouble(row.ItemArray[0]),
                    YValue =
                        Convert.ToDouble(
                            responseTable.Rows[i].ItemArray[
                                responseTable.Columns.IndexOf(seriesName)])
                };
                sortPoints.Add(sp);
            }
        }

        public void ToggleAllResponses(double pointX, List<FixPoint> fixPoints)
        {
            List<SortPoint> foundPoints = sortByX.Where(p => p.XValue == pointX).ToList();
            var found = false;
            List<int> rowIndex = foundPoints.Select(p => p.RowIndex).Distinct().ToList();

            if (rowIndex.Any())
            {
                found = true;
            }

            if (found)
            {
                bool turnedOff = TurnedOffPoints.Where(p => rowIndex.Contains(p.RowIndex)).ToList().Any();

                foreach (int index in rowIndex)
                {
                    if (turnedOff)
                    {
                        TurnOnPoints(index);
                    }
                    else
                    {
                        TurnOffPoints(index);
                    }
                }

                Flag state = turnedOff ? Flag.InBounds : Flag.OutBounds;

                foreach (SortPoint point in foundPoints)
                {
                    fixPoints.Add(new FixPoint(point.SeriesPtIndex, point.SeriesIndex, state));
                }
            }
        }

        private DataTable _quickTable = new DataTable();
        private int _podEndIndex;
        private int _totalFlawCount;

        public DataTable QuickTable
        {
            get
            {
                return _quickTable;
            }
        }

        public void AddData(string myID, double myFlaw, double myResponse, int index)
        {
            AddStringRowToTable(myID, index, _availableSpecIDsTable);
            AddDoubleRowToTable(myFlaw, index, _availableFlawsTable);
            AddDoubleRowToTable(myResponse, index, _availableResponsesTable);
        }

        private static void AddStringRowToTable(string myID, int index, DataTable table)
        {
            if (table.Rows.Count <= index)
            {
                var row = table.NewRow();

                row[0] = myID;

                table.Rows.Add(row);
            }
            else
            {
                table.Rows[index][0] = myID;
            }
        }

        private static void AddDoubleRowToTable(double myValues, int index, DataTable table)
        {
            if (table.Rows.Count <= index)
            {
                var row = table.NewRow();

                row[0] = myValues;

                table.Rows.Add(row);
            }
            else
            {
                table.Rows[index][0] = myValues;
            }
        }

        public void RecreateTables()
        {
            
            ActivateMetaDatas(ActivatedMetaDataNames);
            ActivateSpecIDs(ActivatedSpecimenIDNames);
            ActivateFlaw(ActivatedFlawName, false);
            ActivateResponses(ActivatedResponseNames, true);
            
        }

        public AnalysisDataTypeEnum RecheckAnalysisType(AnalysisDataTypeEnum myForcedType)
        {
            if (ActivatedResponses.Rows.Count > 0 && myForcedType != AnalysisDataTypeEnum.AHat && myForcedType != AnalysisDataTypeEnum.HitMiss)
                _dataType = DataSource.DecideAnalysisType(ActivatedResponses);
            else
                _dataType = myForcedType;

            return _dataType;
        }

        public void GetRow(int rowIndex, out string myID, out double myFlaw, out double myResponse)
        {
            if (rowIndex < _availableSpecIDsTable.Rows.Count)
            {
                myID = _availableSpecIDsTable.Rows[rowIndex][0].ToString();

                myFlaw = 0.0;

                if (!double.TryParse(_availableFlawsTable.Rows[rowIndex][0].ToString(), out myFlaw))
                    myFlaw = double.NaN;

                myResponse = 0.0;

                if (!double.TryParse(_availableResponsesTable.Rows[rowIndex][0].ToString(), out myResponse))
                    myResponse = double.NaN;
            }
            else
            {
                myID = "";
                myFlaw = 0.0;
                myResponse = 0.0;
            }
        }

        public void DeleteRow(int index)
        {
            _availableSpecIDsTable.Rows.RemoveAt(index);
            _availableFlawsTable.Rows.RemoveAt(index);
            _availableResponsesTable.Rows.RemoveAt(index);
        }

        public int RowCount
        {
            get
            {
                return _availableSpecIDsTable.Rows.Count;
            }
        }

        public bool EverythingCommented
        {
            get
            {
                foreach(DataPointIndex point in TurnedOffPoints)
                {
                    var comment = CommentDictionary[point.ColumnIndex][point.RowIndex];

                    if (comment == null || comment.Trim().Length == 0)
                        return false;
                }

                return true;
            }
        }

        public List<string> AvailableFlawUnits
        {
            get
            {
                return GetListPropertyValuesFromTable(_availableFlawsTable, ExtColProperty.Unit, AvailableFlawNames);
            }
        }

        public List<string> AvailableResponseUnits
        {
            get
            {
                return GetListPropertyValuesFromTable(_availableResponsesTable, ExtColProperty.Unit, AvailableResponseNames);
            }
        }

        private static List<string> GetListPropertyValuesFromTable(DataTable table, string property, List<string> names)
        {
            var values = new List<string>();

            foreach (String name in names)
            {
                var col = table.Columns[name];

                if (col == null)
                {
                    col = table.Columns[names.IndexOf(name)];
                    col.ColumnName = name;
                }

                var propValue = GetExtendedProperty(col, property);

                values.Add(propValue);
            }

            return values;
        }
        public HMAnalysisObject HMAnalysisObject
        {
            set { _hmAnalysisObject = value; }
            get { return _hmAnalysisObject; }
        }
        public AHatAnalysisObject AHATAnalysisObject
        {
            set { _aHatAnalysisObject = value; }
            get { return _aHatAnalysisObject; }
        }
    }
    
    
}

