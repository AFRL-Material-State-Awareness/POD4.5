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
        private List<SortPoint> _sortPointList;
        public SortPointListWrapper(List<SortPoint> sortPointInput)
        {
            _sortPointList = sortPointInput;
        }
        public int GetCountOfList() => _sortPointList.Count;
        public bool HasAnyPoints() => _sortPointList.Any();
        public List<SortPoint> SortPointList =>_sortPointList;
        public int BinarySearch(SortPoint item)
        {
            return _sortPointList.BinarySearch(item);
        }
        //public int SeriesPtIndex { get { return _sortPointList.SeriesPtIndex; } }
    }
    public interface ISortPointListWrapper
    {
        int GetCountOfList();
        bool HasAnyPoints();
        List<SortPoint> SortPointList { get; }
        int BinarySearch(SortPoint item);
        //int SeriesPtIndex { get; }
    }
}
