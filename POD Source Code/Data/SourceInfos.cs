using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POD.Data
{
    [Serializable]
    public class SourceInfos : List<SourceInfo>
    {
        public bool ContainsOriginal(string myName)
        {
            return GetFromOriginalName(myName) != null;
        }

        public bool ContainsNew(string myName)
        {
            return GetFromNewName(myName) != null;
        }

        public SourceInfo GetFromOriginalName(string myName)
        {
            SourceInfo info = null;

            foreach(SourceInfo item in this)
            {
                if (item.OriginalName == myName)
                    info = item;
            }

            return info;
        }

        public SourceInfo GetFromNewName(string myName)
        {
            SourceInfo info = null;

            foreach (SourceInfo item in this)
            {
                if (item.NewName == myName)
                    info = item;
            }

            return info;
        }

        public void AddCopiedInfo(SourceInfo currentInfo, string newName)
        {
            SourceInfo newInfo = new SourceInfo(newName, newName, currentInfo);

            Add(newInfo);
        }
    }
}
