using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POD.Analyze;

namespace POD.Wizards
{
    public class ProjectFinishArgs : FinishArgs
    {
        public AnalysisList Analyses;
        public AnalysisList Removed;

        public ProjectFinishArgs()
        {
            Analyses = new AnalysisList();
            Removed = new AnalysisList();
        }

        internal void UpdateAnalyses(AnalysisList analysesKeep, AnalysisList analysesRemoved)
        {
            Analyses = analysesKeep;
            Removed = analysesRemoved;

        }

        
    }
}
