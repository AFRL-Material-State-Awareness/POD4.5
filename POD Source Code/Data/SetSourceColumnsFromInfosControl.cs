using POD.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POD.Data
{
    public class SetSourceColumnsFromInfosControl : ISetSourceColumnsFromInfosControl
    {
        private ISourceInfo _source;
        public SetSourceColumnsFromInfosControl(ISourceInfo sourceInput)
        {
            _source = sourceInput;
        }
        public void SetSourceColumnsFromInfos(DataSource mySource, ColType type)
        {
            var newInfos = mySource.ColumnInfos(type);
            var oldInfos = _source.GetInfos(type);
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

            _source.SortInfos(oldInfos);
        }
    }

    public interface ISetSourceColumnsFromInfosControl
    {
        void SetSourceColumnsFromInfos(DataSource mySource, ColType type);
    }
}
