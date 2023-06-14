using System.Collections.Generic;

namespace POD.Data
{
    public interface ISourceInfo
    {
        string NewName { get; set; }
        string OriginalName { get; set; }

        List<ColumnInfo> GetInfos(ColType myType);
        List<double> Maximums(ColType myType);
        List<double> Minimums(ColType myType);
        List<string> NewNames(ColType myType);
        List<string> Originals(ColType myType);
        double SetDoubleProperty(int index, ColType myType, InfoType myInfo, double newValue);
        string SetName(int index, ColType myType, string newValue);
        string SetStringProperty(int index, ColType myType, InfoType myInfo, string newValue);
        List<double> Thresholds(ColType myType);
        List<string> Units(ColType myType);
        void UpdateDataSource(DataSource mySource, ISetSourceColumnsFromInfosControl sourceInfosControlInput = null);

        void SortInfos(List<ColumnInfo> columnInfos);
    }
}