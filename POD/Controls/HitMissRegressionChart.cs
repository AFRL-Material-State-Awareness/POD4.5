using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POD.Controls
{
    public partial class HitMissRegressionChart : RegressionAnalysisChart
    {
        public HitMissRegressionChart()
        {
            InitializeComponent();

            IsSquare = false;
            CanUnselect = false;
            Selectable = false;
        }
    }
}
