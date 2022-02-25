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
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();

            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        }

        private void Link_Clicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("mailto:pod@udri.udayton.edu");
            }
            catch
            {

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("MIT License for DockPanelSuite"  + System.Environment.NewLine + 
                "Copyright (c) 2007 Weifen Luo (email: weifenluo@yahoo.com)" + System.Environment.NewLine  + System.Environment.NewLine +

                "MIT License for Spreadsheet Light" + System.Environment.NewLine + 
                "Copyright (c) 2011 Vincent Tan Wai Lip" + System.Environment.NewLine + System.Environment.NewLine +  

                "Permission is hereby granted, free of charge, to any person obtaining a copy of this " + 
                "software and associated documentation files (the \"Software\"), to deal in the Software " + 
                "without restriction, including without limitation the rights to use, copy, modify, merge, "  + 
                "publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons "  +
                "to whom the Software is furnished to do so, subject to the following conditions:" + System.Environment.NewLine + System.Environment.NewLine +                
                "The above copyright notice and this permission notice shall be included in all copies or "  +
                "substantial portions of the Software." + System.Environment.NewLine + System.Environment.NewLine +      
                "THE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, "  + 
                "INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR "  + 
                "PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE "  + 
                "FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR "  + 
                "OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR "  + 
                "OTHER DEALINGS IN THE SOFTWARE.", "License Information");
        }
    }
}
