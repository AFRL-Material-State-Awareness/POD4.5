using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace POD.Analyze
{
    //the analysis class is inherited in order to easily integrate the class into the wizard dock pair list. Most of the inherited fields will
    //not be used
    public class RunAllAnalysis : Analysis
    {
        private string flawName;
        private AnalysisList allAnalyses;
        private List<DataTable> PODTables;
        public RunAllAnalysis(AnalysisList allAnalysesInput, string flawNameInput="NONE")
        {
            this.flawName = flawNameInput;
            this.allAnalyses = allAnalysesInput;
        }
        public void RunAllAnalyses()
        {
            UpdateTransformsAll();
            foreach (Analysis analysis in this.allAnalyses)
            {
                analysis.RunAnalysis();
                PODTables.Add(analysis.Data.PodCurveTable);
            }
        }
        public void UpdateTransformsAll()
        {
            //This will need to iterate through all the tab with analysis objects to add the 
            //appropriate transforms and/or model types
        }

    }
}
