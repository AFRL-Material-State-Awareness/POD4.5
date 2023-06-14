using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace POD.Data
{
    public enum InfoType
    {
        NewName,
        OriginalName,
        Unit,
        Min,
        Max, 
        Threshold
    }


    [Serializable]
    public class SourceInfo : ISourceInfo
    {
        string _originalName;
        string _newName;

        List<ColumnInfo> _flaws;
        List<ColumnInfo> _responses;

        public SourceInfo(string myOriginalName, string myNewName, IDataSource mySource)
        {
            OriginalName = myOriginalName;
            NewName = myNewName;

            if (mySource != null)
            {
                _flaws = mySource.ColumnInfos(ColType.Flaw);
                _responses = mySource.ColumnInfos(ColType.Response);
            }
            else
            {
                _flaws = new List<ColumnInfo>();
                _responses = new List<ColumnInfo>();
            }
        }

        public SourceInfo(string originalName, string newName, SourceInfo currentInfo)
        {
            _originalName = originalName;
            _newName = newName;

            _flaws = new List<ColumnInfo>();
            _responses = new List<ColumnInfo>();

            foreach (ColumnInfo column in currentInfo._flaws)
            {
                _flaws.Add(column.Copy());
            }

            foreach (ColumnInfo column in currentInfo._responses)
            {
                _responses.Add(column.Copy());
            }
        }

        /// <summary>
        /// Return either flaws or responses column info.
        /// </summary>
        /// <param name="myType">column type</param>
        /// <returns>list fo associated column info objects</returns>
        public List<ColumnInfo> GetInfos(ColType myType)
        {
            if (myType == ColType.Flaw)
                return _flaws;
            else if (myType == ColType.Response)
                return _responses;
            else
                throw new ArgumentOutOfRangeException("Not a valid column Type argument!");
        }

        public List<string> NewNames(ColType myType)
        {
            return GetStringValues(myType, InfoType.NewName);
        }

        public List<string> Originals(ColType myType)
        {
            return GetStringValues(myType, InfoType.OriginalName);
        }

        public List<string> Units(ColType myType)
        {
            return GetStringValues(myType, InfoType.Unit);
        }

        public List<double> Maximums(ColType myType)
        {
            return GetDoubleValues(myType, InfoType.Max);
        }

        public List<double> Minimums(ColType myType)
        {
            return GetDoubleValues(myType, InfoType.Min);
        }

        public List<double> Thresholds(ColType myType)
        {
            return GetDoubleValues(myType, InfoType.Threshold);
        }

        public string OriginalName
        {
            get { return _originalName; }
            set { _originalName = value; }
        }

        public string NewName
        {
            get { return _newName; }
            set { _newName = value; }
        }

        private List<string> GetStringValues(ColType myType, InfoType myDataType)
        {
            var list = GetInfos(myType);
            var strings = new List<string>();

            foreach (ColumnInfo info in list)
            {
                strings.Add(info.GetStringValue(myDataType));
            }

            return strings;
        }

        private List<double> GetDoubleValues(ColType myType, InfoType myDataType)
        {
            var list = GetInfos(myType);
            var values = new List<double>();

            foreach (ColumnInfo info in list)
            {
                values.Add(info.GetDoubleValue(myDataType));
            }

            return values;
        }

        /// <summary>
        /// Update data source with latest source info settings
        /// </summary>
        /// <param name="mySource"></param>
        public void UpdateDataSource(DataSource mySource, 
            ISetSourceColumnsFromInfosControl sourceInfosControlInput = null)
        {
            ISetSourceColumnsFromInfosControl sourceinfosControl = sourceInfosControlInput ?? new SetSourceColumnsFromInfosControl(this);
            if (mySource == null)
                return;

            if (mySource.SourceName == _originalName)
            {
                mySource.SourceName = _newName;
            }

            if (mySource.SourceName == _newName)
            {
                sourceinfosControl.SetSourceColumnsFromInfos(mySource, ColType.Flaw);
                sourceinfosControl.SetSourceColumnsFromInfos(mySource, ColType.Response);
            }
        }

        /// <summary>
        /// Update the source's columns with info for source info. Original name is never overwritten
        /// since it is a key for deciding where to copy.
        /// </summary>
        /// <param name="mySource"></param>
        /// <param name="type"></param>
        private void SetSourceColumnsFromInfos(DataSource mySource, ColType type)
        {
            var newInfos = mySource.ColumnInfos(type);
            var oldInfos = GetInfos(type);
            var deleteList = new List<ColumnInfo>();


            for (int i = 0; i < newInfos.Count; i++)
            {
                bool found = false;

                foreach (ColumnInfo info in oldInfos)
                {
                    if (info.OriginalName == newInfos[i].OriginalName)
                    {
                        mySource.UpdateFromColumnInfo(type, i, info);
                        found = true;
                        break;
                    }
                }

                if (found == false)
                {
                    oldInfos.Add(newInfos[i]);
                }
            }

            for (int i = 0; i < oldInfos.Count; i++)
            {
                bool found = false;

                foreach (ColumnInfo newInfo in newInfos)
                {
                    if (newInfo.OriginalName == oldInfos[i].OriginalName)
                    {
                        found = true;
                        break;
                    }
                }

                if (found == false)
                {
                    deleteList.Add(oldInfos[i]);
                }
            }

            while (deleteList.Count > 0)
            {
                oldInfos.Remove(deleteList[0]);
                deleteList.RemoveAt(0);
            }

            SortInfos(oldInfos);
        }

        public void SortInfos(List<ColumnInfo> columnInfos)
        {
            var newList = new List<ColumnInfo>();

            foreach (ColumnInfo item in columnInfos)
            {
                var insertIndex = 0;

                for (int i = 0; i < newList.Count; i++)
                {
                    if (item.Index > newList[i].Index)
                        insertIndex++;

                }

                newList.Insert(insertIndex, item);
            }

            columnInfos.Clear();

            foreach (ColumnInfo item in newList)
            {
                columnInfos.Add(item);
            }
        }





        public string SetName(int index, ColType myType, string newValue)
        {
            return SetName(GetInfos(myType), NewNames(myType), index, newValue);
        }

        private string SetName(List<ColumnInfo> list, List<string> names, int index, string newValue)
        {
            var oldName = names[index].Trim();

            newValue = Globals.CleanColumnName(newValue).Trim();

            if (oldName != newValue)
            {
                //don't want to check for the name we are changing
                names.RemoveAt(index);

                while (names.Contains(newValue))
                    newValue = newValue + " Copy";

                list[index].NewName = newValue;
            }

            return newValue;
        }

        public string SetStringProperty(int index, ColType myType, InfoType myInfo, string newValue)
        {
            newValue = GetInfos(myType)[index].SetStringValue(myInfo, newValue);

            return newValue;
        }

        public double SetDoubleProperty(int index, ColType myType, InfoType myInfo, double newValue)
        {
            newValue = GetInfos(myType)[index].SetDoubleValue(myInfo, newValue);

            return newValue;
        }
    }

    [Serializable]
    public class ColumnInfo : IColumnInfo
    {
        string _originalName;

        public string OriginalName
        {
            get 
            {
                if (_originalName == null)
                    _originalName = "";

                return _originalName; 
            }
            set { _originalName = value; }
        }
        string _newName;

        public string NewName
        {
            get 
            {
                if (_newName == null)
                    _newName = "";
                
                return _newName;
            }
            set { _newName = value; }
        }
        string _unit;

        public string Unit
        {
            get
            {
                if (_unit == null)
                    _unit = "";

                return _unit; 
            }
            set { _unit = value; }
        }
        int _index;

        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        double _minimum;

        public double Min
        {
            get { return _minimum; }
            set { _minimum = value; }
        }
        double _maximum;

        public double Max
        {
            get { return _maximum; }
            set { _maximum = value; }
        }
        double _threshold;

        public ColumnInfo(string oldName, string newName, string unit, double min, double max, int index)
        {
            OriginalName = oldName;
            NewName = newName;
            Unit = unit;
            Max = max;
            Min = min;
            Threshold = 0.0;
            Index = index;
        }

        public ColumnInfo(string oldName, string newName, string unit, double min, double max, double threshold, int index)
        {
            OriginalName = oldName;
            NewName = newName;
            Unit = unit;
            Max = max;
            Min = min;
            Threshold = threshold;
            Index = index;
        }

        public double Threshold
        {
            get { return _threshold; }
            set { _threshold = value; }
        }

        public string GetStringValue(InfoType myDataType)
        {
            if (myDataType == InfoType.Unit)
                return Unit;
            else if (myDataType == InfoType.OriginalName)
                return OriginalName;
            else if (myDataType == InfoType.NewName)
                return NewName;
            else
                return "";
        }

        public string SetStringValue(InfoType myDataType, string myValue)
        {
            var val = myValue.Trim();

            if (myDataType == InfoType.Unit)
                Unit = val;
            else if (myDataType == InfoType.OriginalName)
                OriginalName = val;
            else if (myDataType == InfoType.NewName)
                NewName = val;

            return val;
        }

        public double GetDoubleValue(InfoType myDataType)
        {
            if (myDataType == InfoType.Max)
                return Max;
            else if (myDataType == InfoType.Min)
                return Min;
            else if (myDataType == InfoType.Threshold)
                return Threshold;
            else
                return 0.0;
        }

        public double SetDoubleValue(InfoType myDataType, double myValue)
        {
            if (myDataType == InfoType.Max)
                Max = myValue;
            else if (myDataType == InfoType.Min)
                Min = myValue;
            else if (myDataType == InfoType.Threshold)
                Threshold = myValue;

            return myValue;
        }

        internal ColumnInfo Copy()
        {
            return new ColumnInfo(_originalName, _newName, _unit, _minimum, _maximum, _threshold, _index);
        }
    }
    public interface IColumnInfo
    {
        double GetDoubleValue(InfoType myDataType);
    }

    
}
