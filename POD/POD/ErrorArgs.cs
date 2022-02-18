using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POD
{
    public class ErrorArgs : EventArgs
    {
        public string Error;
        public int Progress;
        public string AnalysisName;

        public ErrorArgs(string myAnalysisName, string myError)
        {
            UpdateValues(myAnalysisName, myError, 0);
        }

        public ErrorArgs(string myAnalysisName, int myProgress)
        {
            UpdateValues(myAnalysisName, "", myProgress);
        } 

        private void UpdateValues(string myAnalysisName, string myError, int myProgress)
        {
            AnalysisName = myAnalysisName;
            Error = myError;
            Progress = myProgress;
        }
    }
}
