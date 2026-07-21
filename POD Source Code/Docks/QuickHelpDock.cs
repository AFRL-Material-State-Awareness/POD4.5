// <copyright file="QuickHelpDock.cs" company="The University of Dayton">
// Copyright © 2025 The University of Dayton. All rights reserved.
//
// Licensed under the GNU General Public License, Version 3.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.gnu.org/licenses/gpl-3.0.txt
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using Transitions;
using System.IO;

namespace POD.Docks
{
    public partial class QuickHelpDock : RTFViewerDock
    {
        public QuickHelpDock() : base()
        {
            InitializeComponent();

            Label = Globals.QuickDockLabel;
            Text = Label;
            HelpName = "Quick Help Viewer";
        }

        

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void RichTextBox_MouseClick(object sender, MouseEventArgs e)
        {
            
        }     
   
       
    }
}
