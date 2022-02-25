using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace POD
{
    public class GetProjectInfoArgs
    {
        public string ProjectFileName;
        public string SourceName;
        public DataTable SourceTable;

        public GetProjectInfoArgs(string mySource)
        {
            ProjectFileName = "";
            SourceTable = null;
            SourceName = mySource;
        }
    }
}
