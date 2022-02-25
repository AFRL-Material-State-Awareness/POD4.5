using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POD.Analyze
{
    public delegate void AnalysisListHandler(object sender, AnalysisListArg e);

    public class AnalysisListArg : EventArgs
    {
        AnalysisList _list = null;

        public AnalysisList Analyses
        {
            get { return _list; }
            set { _list = value; }
        }

        public AnalysisListArg(ref AnalysisList list)
        {
            _list = list;
        }
    }
}
