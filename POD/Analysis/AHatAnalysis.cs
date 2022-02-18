using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POD.Data;

namespace POD.Analyze
{
    [Serializable]
    public class AHatAnalysis : Analysis
    {
        public AHatAnalysis(DataSource mySource) : base(mySource)
        {
            AnalysisDataType = AnalysisDataTypeEnum.AHat;
            
        }

        public AHatAnalysis(DataSource source, SourceInfo info, string flawName, List<string> responses) : base(source, info, flawName, responses)
        {
            AnalysisDataType = AnalysisDataTypeEnum.AHat;
        }

    }
}
