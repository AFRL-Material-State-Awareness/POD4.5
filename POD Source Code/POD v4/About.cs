// <copyright file="About.cs" company="The University of Dayton">
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
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
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

            VersionHelper.UpdatePODVersionHeader(this);

            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            this.UpdateBuildMonth();
            this.UpdateCopyright();
        }

        private void UpdateBuildMonth()
        {
            DateTime lastWritten = VersionHelper.GetExecutableLastModifiedDate();
            monthYearLabel.Text =  lastWritten.ToString("MMMM yyyy", CultureInfo.InvariantCulture);
        }

        private void UpdateCopyright()
        {
            DateTime lastWritten = VersionHelper.GetExecutableLastModifiedDate();
            string year = lastWritten.ToString("yyyy", CultureInfo.InvariantCulture);
            copyrightLabel.Text = $"\u00A9 {year} UDRI";
        }

        private void Link_Clicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("mailto:Christine.Henry@us.af.mil");
            }
            catch
            {

            }
        }
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("mailto:christine.henry@afrl.af.mil");
            }
            catch
            {

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string licenseText = @"GNU General Public License Version 3 for the POD Pro v4.5 program as a whole: 
Probability of Detection Professional Version 4.5 (POD Pro v4.5)
Copyright (C) 2024  University of Dayton Research Institute (UDRI)

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>

MIT License for DockPanelSuite
Copyright (c) 2007 Weifen Luo (email: weifenluo@yahoo.com)

MIT License for Spreadsheet Light
Copyright (c) 2011 Vincent Tan Wai Lip

Permission is hereby granted, free of charge, to any person obtaining a copy of this 
software and associated documentation files (the ""Software""), to deal in the Software 
without restriction, including without limitation the rights to use, copy, modify, merge, 
publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons 
to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or 
substantial portions of the Software.

THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
OTHER DEALINGS IN THE SOFTWARE.";

            MessageBox.Show(licenseText, "License Information");
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void ChangeLogButton_Click(object sender, EventArgs e)
        {
            try
            {
                string executablePath = AppDomain.CurrentDomain.BaseDirectory;
                string logFile = Path.Combine(executablePath, "Change Log.md");

                if (File.Exists(logFile))
                {
                    Process.Start(logFile);
                }
            }
            catch (Exception exp)
            {
                Debug.WriteLine($"An error occurred opening change log: {exp.Message}");
            }
        }
    }
}
