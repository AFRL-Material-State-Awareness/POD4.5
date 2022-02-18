using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POD.Analyze
{
    [Serializable]
    public class AnalysisList : List<Analysis>
    {
        public List<string> Names
        {
            get
            {
                var names = new List<string>();

                foreach(Analysis analysis in this)
                {
                    names.Add(analysis.Name);
                }

                return names;
            }
        }

        public bool UsingDataSource(string p)
        {
            foreach (Analysis analysis in this)
            {
                if (analysis.SourceName == p)
                    return true;
            }

            return false;
        }
    }

    
}
