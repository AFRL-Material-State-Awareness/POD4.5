using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using static POD.Data.SortPoint;
using POD.Data;

namespace Data
{
    public class SortPointListWrapper : ISortPointListWrapper
    {
        private List<ISortPoint> _sortPointList;
        public SortPointListWrapper(List<ISortPoint> sortPointInput)
        {
            _sortPointList = sortPointInput;
        }
        public int GetCount => _sortPointList.Count;
        public bool HasAnyPoints() => _sortPointList.Any();
        public List<ISortPoint> SortPointList =>_sortPointList;
        public int BinarySearch(SortPoint item)
        {
            return _sortPointList.BinarySearch(item);
        }
        //public int SeriesPtIndex { get { return _sortPointList.SeriesPtIndex; } }
    }
    public interface ISortPointListWrapper
    {
        int GetCount { get; }
        bool HasAnyPoints();
        List<ISortPoint> SortPointList { get; }
        int BinarySearch(SortPoint item);
        //int SeriesPtIndex { get; }
    }
}
