using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POD.Analyze
{
    //the analysis class is inherited in order to easily integrate the class into the wizard dock pair list. Most of the inherited fields will
    //not be used
    public class RunAllAnalysis : Analysis
    {
        private string flawName;
        private AnalysisList allAnalyses;
        public RunAllAnalysis(AnalysisList allAnalysesInput, string flawNameInput="NONE")
        {
            this.flawName = flawNameInput;
            this.allAnalyses = allAnalysesInput;
        }
        //public new WizardEnum WizardType
        //{
            
        //}

    }
}
