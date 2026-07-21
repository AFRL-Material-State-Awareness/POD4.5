using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace POD.Controls
{
    public class NamedStripLine : StripLine
    {
        public string Name { get; set; }

        public NamedStripLine()
        {
        }

        public NamedStripLine(string stripName)
        {
            this.Name = stripName;
        }
    }
}
