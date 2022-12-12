using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POD
{
    public partial class LicensingInfo : Form
    {
        public LicensingInfo()
        {
            InitializeComponent();
            //AlignTextBox();
        }
        public void AlignTextBox()
        {
            LicenseBodyRTF.SelectAll();
            LicenseBodyRTF.SelectionAlignment = HorizontalAlignment.Center;
            LicenseBodyRTF.DeselectAll();
        }
    }
}
