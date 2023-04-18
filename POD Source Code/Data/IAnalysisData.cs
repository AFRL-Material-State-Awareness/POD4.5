using CSharpBackendWithR;
using POD;
using POD.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POD.Data
{
    public interface IAnalysisData
    {
        /// <summary>
        ///     Make active a single flaw column.
        /// </summary>
        DataTable ActivateFlaw(String myName, bool runUpdate = true);
        /// <summary>
        ///     Make active multiple flaw columns.
        /// </summary>
        DataTable ActivateFlaws(List<string> myNames, bool runUpdate = true)
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
        void SetREngine(IREngineObject myREngine, string myAnalysisName);
        void UpdateOutput(RCalculationType myCalculationType = RCalculationType.Full);
        string AdditionalWorksheet1Name { get; }
        double UncensoredFlawRangeMin { get; }
        double FlawRangeMin { get; }
        double UncensoredFlawRangeMax { get; }
        double FlawRangeMax { get; }
        double InvertTransformedFlaw(double myValue);
        double InvertTransformedResponse(double myValue);
        int FlawCount { get; }
        int FlawCountUnique { get; }
        void GetXYBufferedRanges(Control chart, AxisObject myXAxis, AxisObject myYAxis, bool myGetTransformed);
        void GetYBufferedRange(Control chart, AxisObject myAxis, bool myGetTransformed);
        void GetXBufferedRange(Control chart, AxisObject myAxis, bool myGetTransformed);
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
        void UpdateHitMissModel(int modelType);
        double SmallestFlaw { get; }
        double SmallestResponse { get; }
        string FlawTransFormLabel { get; }
        string ResponseTransformLabel { get; }
        void UpdateSourceFromInfos(SourceInfo sourceInfo);
        void GetUpdatedValue(ColType myType, string myExtColProperty, double currentValue, out double newValue);
        void GetNewValue(ColType myType, string myExtColProperty, out double newValue);
        void SetSource(DataSource source, string flawName, List<string> responses);
        DataTable TransformedInput { get; }
        bool FilterTransformedDataByRanges { get; set; }
        bool IsResponseTable(DataTable mySourceTable);
        bool IsFlawTable(DataTable mySourceTable);
        void CreateNewSortList();
        void UpdateTable(int rowIndex, int colIndex, Flag bounds);
        void UpdateIncludedPointsBasedFlawRange(double aboveX, double belowX, List<FixPoint> fixPoints);
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

        HMAnalysisObject HMAnalysisObject { get; }
        AHatAnalysisObject AHATAnalysisObject { get; }
        

    }
}
