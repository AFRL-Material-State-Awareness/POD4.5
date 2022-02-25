using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POD.Data;

namespace POD.Analyze
{
    [Serializable]
    public class HitMissAnalysis : Analysis
    {
        //private DataSource source;
        //private SourceInfo info;
        //private List<string> responses;

         public HitMissAnalysis(DataSource mySource) : base(mySource)
        {
            AnalysisDataType = AnalysisDataTypeEnum.HitMiss;
        }

         public HitMissAnalysis(DataSource source, SourceInfo info, string flawName, List<string> responses)
             : base(source, info, flawName, responses)
         {
             AnalysisDataType = AnalysisDataTypeEnum.HitMiss;
         }
    }
}
