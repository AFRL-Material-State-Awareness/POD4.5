using CSharpBackendWithR;
using Data;
using POD;
using POD.Data;
using POD.ExcelData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static POD.Data.SortPoint;

namespace POD.Data
{
    public interface IAnalysisData
    {
        double ResponseLeft { get; set; }
        double ResponseRight { get; set; }
        bool IsFrozen { get; set; }
        /// Propreties region
        string ActivatedFlawName { get; }

        string ActivatedOriginalFlawName { get; }

        List<string> ActivatedOriginalResponseNames { get; }

        string GetRemovedPointComment(int myColIndex, int myRowIndex);

        void SetRemovedPointComment(int myColIndex, int myRowIndex, string myComment);

        DataTable ActivatedFlaws { get; }
        List<string> ActivatedMetaDataNames { get; }
        DataTable ActivatedMetaDatas { get; }
        List<string> ActivatedResponseNames { get; }
        DataTable ActivatedResponses { get; }
        List<string> ActivatedSpecimenIDNames { get; }
        DataTable ActivatedSpecimenIDs { get; }
        DataTable ActivatedTransformedFlaws { get; }
        DataTable ActivatedTransformedResponses { get; }
        List<string> AvailableFlawNames { get; }
        List<string> AvailableMetaDataNames { get; }
        List<string> AvailableResponseNames { get; }
        AnalysisDataTypeEnum DataType { get; set; }
        DataTable FitResidualsTable { get; }
        TransformTypeEnum FlawTransform { get; set; }
        /// <summary>
        ///     Table holds the flaw size, POD and confidence bound
        /// </summary>
        DataTable PodCurveTable { get; }
        /// <summary>
        /// Holds the values to plot the ghost curve (will be the same as PODCurveTable if no flaws are excluded)
        /// </summary>
        DataTable PodCurveTable_All { get; }
        /// <summary>
        /// Table holds the frequency of ranges in order to plot the normality chart
        /// </summary>
        DataTable NormalityTable { get; }
        /// <summary>
        /// get the table to plot a normal curve overlay
        /// </summary>
        DataTable NormalityCurveTable { get; }
        DataTable OriginalData { get; }
        double LambdaValue { get; }
        /// <summary>
        ///     Get/set the response data transform type
        /// </summary>
        TransformTypeEnum ResponseTransform { get; set; }
        /// <summary>
        ///     Holds the a50, a90, a90/95, V11, V12, V22 (Vs represent the values for the varaince-covariance matrix)
        /// </summary>
        DataTable ThresholdPlotTable { get; }
        /// <summary>
        ///     Holds the a50, a90, a90/95, V11, V12, V22 (plots the ghost curve if applicable)
        /// </summary>
        DataTable ThresholdPlotTable_All { get; }
        /// <summary>
        /// Creates a duplicate list of turned off points to read. DataPointIndex has ColumnName and RowIndex.
        /// </summary>
        List<DataPointIndex> TurnedOffPoints { get; }
        /// <summary>
        ///     Make active a single flaw column.
        /// </summary>
        DataTable ActivateFlaw(String myName, bool runUpdate = true);
        /// <summary>
        ///     Make active multiple flaw columns.
        /// </summary>
        DataTable ActivateFlaws(List<string> myNames, bool runUpdate = true);
        DataTable ActivateResponses(List<string> myNames, bool runUpdate = true);
        DataTable ActivateSpecIDs(List<string> myNames);
        /// <summary>
        ///     Duplicate all of the analysis data.
        /// </summary>
        AnalysisData CreateDuplicate();
        /// <summary>
        ///     Copy only columns specified in the column name lists over to the analysis data. Then
        ///     activate the copied columns.
        /// </summary>
        /// <param name="mySource">the data source to copy data from</param>
        /// <param name="myFlaws">the name of the flaw columns to copy</param>
        /// <param name="myMetaDatas">the name of metadata columns to copy</param>
        /// <param name="myResponses">the name of response data columns to copy</param>
        /// <param name="mySpecIDs">the name of specimen id columns to copy</param>
        void SetSource(DataSource mySource, List<string> myFlaws, List<string> myMetaDatas,
            List<string> myResponses, List<string> mySpecIDs);
        /// <summary>
        ///     Copy all of the data available from the source to the analysis data then activate all available data.
        /// </summary>
        void SetSource(DataSource mySource);
        /// <summary>
        ///     Turn off a set of points all at once.
        /// </summary>
        void SetTurnedOffPoints(List<DataPointIndex> myList);
        /// <summary>
        ///     Turn ON all response data points previously turned OFF.
        /// </summary>
        void TurnAllPointsOn();
        /// <summary>
        ///     Turn off a point in the table response data.
        /// </summary>
        void TurnOffPoint(int myResponseColumnIndex, int myRowIndex);
        /// <summary>
        ///     Turn off a point in the table response data.
        /// </summary>
        void TurnOffPoints(int myRowIndex);
        /// <summary>
        ///     Turn on a point in the table response data.
        /// </summary>
        void TurnOnPoint(int myResponseColumnIndex, int myRowIndex);
        /// <summary>
        ///     Turns on all points at a given index (ie for a given flaw size)
        /// </summary>
        /// <param name="myRowIndex">index of row to turn off</param>
        void TurnOnPoints(int myRowIndex);
        void UpdateData(bool quickFlag = false);
        void SetPythonEngine(I_IPy4C myPy, string myAnalysisName);
        void UpdateOutput(RCalculationType myCalculationType,
            IUpdateOutputForAHatData updateOutputForAHatDataIn = null,
            IUpdateOutputForHitMissData updateOutputForHitMissDataIn = null);
        string AdditionalWorksheet1Name { get; }
        double UncensoredFlawRangeMin { get; }
        double FlawRangeMin { get; }
        double UncensoredFlawRangeMax { get; }
        double FlawRangeMax { get; }
        double InvertTransformedFlaw(double myValue);
        double InvertTransformedResponse(double myValue);
        int FlawCount { get; }
        int FlawCountUnique { get; }
        void GetXYBufferedRanges(Control chart, IAxisObject myXAxis, IAxisObject myYAxis, bool myGetTransformed);
        void GetYBufferedRange(Control chart, IAxisObject myAxis, bool myGetTransformed);
        void GetXBufferedRange(Control chart, IAxisObject myAxis, bool myGetTransformed);
        void GetUncensoredXBufferedRange(Control chart, AxisObject myAxis, bool myGetTransformed);
        AxisObject GetXBufferedRange(Control chart, bool myGetTransformed);
        AxisObject GetUncensoredXBufferedRange(Control chart, bool myGetTransformed);
        AxisObject GetYBufferedRange(Control chart, bool myGetTransformed);
        double DoNoTransform(double myValue);
        double TransformAValue(double myValue, int transform);
        double TransformBackAValue(double myValue, int transform);
        double TransformValueForXAxis(double myValue);
        double TransformValueForYAxis(double myValue);
        double InvertTransformValueForXAxis(double myValue);
        double InvertTransformValueForYAxis(double myValue);
        double SmallestFlaw { get; }
        double SmallestResponse { get; }
        string FlawTransFormLabel { get; }
        string ResponseTransformLabel { get; }
        void UpdateSourceFromInfos(SourceInfo sourceInfo, ITableUpdaterFromInfos tableUpdaterFromInfosIn = null);
        void GetUpdatedValue(ColType myType, string myExtColProperty, double currentValue, out double newValue);
        void GetNewValue(ColType myType, string myExtColProperty, out double newValue);
        void SetSource(DataSource source, string flawName, List<string> responses);
        bool FilterTransformedDataByRanges { get; set; }
        void CreateNewSortList();
        void UpdateIncludedPointsBasedFlawRange(double aboveX, double belowX, List<FixPoint> fixPoints,
            List<ISortPoint> sortByXIn = null);
        void ToggleResponse(double pointX, double pointY, string seriesName, int rowIndex, int colIndex, List<FixPoint> fixPoints);
        void ForceRefillSortListAndClearPoints();
        void ForceRefillSortList();
        void RefillSortList();
        void ToggleAllResponses(double pointX, List<FixPoint> fixPoints);
        DataTable QuickTable { get; }
        void AddData(string myID, double myFlaw, double myResponse, int index);
        void RecreateTables();
        AnalysisDataTypeEnum RecheckAnalysisType(AnalysisDataTypeEnum myForcedType);
        void GetRow(int rowIndex, out string myID, out double myFlaw, out double myResponse);
        void DeleteRow(int index);

        int RowCount { get; }
        bool EverythingCommented { get; }
        List<string> AvailableFlawUnits { get; }
        List<string> AvailableResponseUnits { get; }

        HMAnalysisObject HMAnalysisObject { get; set; }
        AHatAnalysisObject AHATAnalysisObject { get; set; }

        /// Will end up removing?
        int ResponsePartialBelowMinCount { get; }
        int ResponsePartialAboveMaxCount { get; }
        int ResponseCompleteBelowMinCount { get; }
        int ResponseCompleteAboveMaxCount { get; }
        int ResponseBetweenCount { get; }

        double MaxFlaw { get; set; }
        double MinFlaw { get; set; }
        double MaxSignal { get; set; }
        double MinSignal { get; set; }
        DataTable ResidualUncensoredTable { get; }
        Dictionary<int, Dictionary<int, string>> CommentDictionary { get; }
        DataTable ResidualPartialCensoredTable { get; }
        DataTable ResidualRawTable { get; }

        //Excel Properties
        void WriteToExcel(IExcelExport myWriter, string myAnalysisName, string myWorksheetName, bool myPartOfProject = true, IExcelWriterControl excelWriteControlIn = null);
        List<string> ChangeTableColumnNames(DataTable myTable, List<string> myNewNames);
        DataTable GenerateRemovedPointsTable();
        /// tables
    }
}
