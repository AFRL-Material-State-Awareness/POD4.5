// <copyright file="Edit Analysis Dialog.cs" company="The University of Dayton">
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

namespace POD.Controls
{
    public partial class EditAnalysisDialog : Form
    {

        public EditAnalysisDialog()
        {
            InitializeComponent();
        }

        public EditAnalysisDialog(SimpleActionBar myAvailableBar, TableLayoutPanel myAvailablePanel,
                                  SimpleActionBar myAnalysisBar, TableLayoutPanel myAnalysisPanel)
        {

            InitializeComponent();

            createTableLayout.Controls.Clear();

            createTableLayout.Controls.Add(myAvailableBar, 0, 1);
            createTableLayout.Controls.Add(myAvailablePanel, 1, 1);
            createTableLayout.Controls.Add(myAnalysisBar, 2, 1);
            createTableLayout.Controls.Add(myAnalysisPanel, 3, 1);

            this.DialogResult = DialogResult.Cancel;
        }

        private void CloseDialog_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;

            

            Close();
        }

        public Button OkayButton
        {
            get
            {
                return UpdateBtn;
            }
        }

        private void From_Closed(object sender, FormClosedEventArgs e)
        {
            createTableLayout.Controls.Clear();
        }
    }
}
