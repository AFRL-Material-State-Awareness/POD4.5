// <copyright file="NewSourceForm.cs" company="The University of Dayton">
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
using POD.Data;

namespace POD.Wizards
{
    public partial class NewSourceForm : Form
    {
        bool _checkedNameResult = true;
        List<string> _usedNames = new List<string>();

        public NewSourceForm(string myInitialName, List<string> mySources)
        {
            InitializeComponent();

            _usedNames = mySources;

            NameTextBox.Text = myInitialName;
            NameTextBox.Select();
            NameTextBox.SelectAll();

            CheckName(myInitialName);

            UpdateCheckImage();
        }

        private void CheckName(string myName)
        {
            _checkedNameResult = !_usedNames.Contains(myName);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void UpdateCheckImage()
        {
            if (_checkedNameResult == true)
                CheckResult.Image = imageList1.Images["Check.png"];
            else
                CheckResult.Image = imageList1.Images["X.png"];
        }

        private void NameBox_TextChanged(object sender, EventArgs e)
        {
            string name = NameTextBox.Text;

            CheckName(name);

            UpdateCheckImage();
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void NewName_Closing(object sender, FormClosingEventArgs e)
        {
            if (_checkedNameResult == false)
                e.Cancel = true;

            NameTextBox.SelectAll();
        }

        public string NewName
        {
            get
            {
                return NameTextBox.Text;
            }
        }
    }
}
