using POD.ExcelData;
using System.Collections.Generic;
using System.Data;

namespace POD.Data
{
    public interface IDataSource
    {
        AnalysisDataTypeEnum AnalysisDataType { get; }
        List<int> DataColumnRange { get; }
        int DataCount { get; }
        int DefaultFlawSizeIndex { get; set; }
        int DefaultIDIndex { get; set; }
        List<int> FlawColumnRange { get; }
        List<string> FlawLabels { get; }
        List<PODListBoxItemWithProps> Flaws { get; }
        List<int> IDColumnRange { get; }
        List<string> IDLabels { get; }
        List<int> MetaDataColumnRange { get; }
        List<string> MetaDataLabels { get; }
        string Name { get; set; }
        DataTable Original { get; }
        List<string> ResponseLabels { get; }
        List<PODListBoxItemWithProps> Responses { get; }
        DataTableWithRanges Source { get; }
        string SourceName { get; set; }
        ImportTemplate Template { get; set; }

        List<ColumnInfo> ColumnInfos(ColType myType);
        List<DataView> GetAllGraphData();
        DataTable GetAllSpecimenData();
        DataTable GetData(List<string> names);
        DataTable GetData(string names);
        DataTable GetGraphData(int myYIndex);
        DataTable GetGraphData(int myXIndex, int myYIndex);
        DataTable GetGraphData(List<string> names);
        DataTable GetGraphData(string flawName, List<string> names);
        DataTable GetSpecimenData();
        List<double> Maximums(ColType myType);
        List<double> Minimums(ColType myType);
        List<string> Originals(ColType myType);
        List<double> PreviousMaximums(ColType myType);
        List<double> PreviousMinimums(ColType myType);
        List<double> PreviousThresholds(ColType myType);
        double SetMaximum(int myIndex, ColType myType, double myNewValue);
        double SetMinimum(int myIndex, ColType myType, double myNewValue);
        string SetName(ColType myType, int myIndex, string myNewName);
        double SetResponseThreshold(int myIndex, ColType myType, double myNewValue);
        string SetUnit(ColType myType, int myIndex, string myNewUnit);
        List<double> Thresholds(ColType myType);
        List<string> Units(ColType myType);
        void UpdateFromColumnInfo(ColType type, int myIndex, ColumnInfo info);
        void WriteDataToExcel(ExcelExport myWriter);
    }
}