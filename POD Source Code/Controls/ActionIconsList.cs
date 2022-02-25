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
    public partial class ActionIconsList : UserControl
    {
        public ActionIconsList()
        {
            InitializeComponent();
        }

        public ImageList List
        {
            get
            {
                return ActionIcons;
            }
        }
    }
}
