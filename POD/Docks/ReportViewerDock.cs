﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace POD.Docks
{
    public partial class ReportDock : PodDock
    {
        public ReportDock()
        {
            InitializeComponent();

            Label = Globals.ReportDockLabel;
            Text = Label;
            

            //richTextBox1.LoadFile("Example Report.rtf");
        }
    }
}
