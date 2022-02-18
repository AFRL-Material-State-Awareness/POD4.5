using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POD.Analyze;

namespace POD
{
    public class PODFile : SkillTypeHolder
    {
        Project _project;
        Analysis _analysis;
        AnalysisList _analyses;

        public PODFile()
        {
            _analyses = new AnalysisList();
        }

        public Project Project
        {
            get { return _project; }
            set { _project = value; }
        }

        public Analysis Analysis
        {
            get { return _analysis; }
            set { _analysis = value; }
        }

        public void AddAnalysis(Analysis myAnalysis)
        {
            _analyses.Add(myAnalysis);

            if(_analysis == null)
                _analysis = myAnalysis;
        }

        public Analysis GetAnalysis(int myIndex)
        {
            if(myIndex >= 0 && myIndex < _analyses.Count)
                return _analyses[myIndex];

            return null;
        }

        public Analysis GetAnalysis(string myName)
        {
            for(int i = 0; i < _analyses.Count; i++)
            {
                if (myName == _analyses[i].Name)
                    return _analyses[i];
            }

            return null;
        }

        public int Count
        {
            get
            {
                return _analyses.Count;
            }
        }
    }
}
