using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POD
{
    [Serializable]
    public class Report
    {
        public Report CreateDuplicate()
        {
            return new Report();
        }
    }
}
