using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POD
{
    [Serializable]
    public class SkillTypeHolder
    {
        SkillLevelEnum _skillLevel;

        public SkillLevelEnum SkillLevel
        {
            get { return _skillLevel; }
            set { _skillLevel = value; }
        }

        AnalysisTypeEnum _analysisType;

        public AnalysisTypeEnum AnalysisType
        {
            get { return _analysisType; }
            set { _analysisType = value; }
        }
    }
}
