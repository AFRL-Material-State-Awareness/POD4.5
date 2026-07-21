// <copyright file="ProjectProperties.cs" company="The University of Dayton">
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POD.ExcelData;

namespace POD
{
    [Serializable]
    public class ProjectProperties
    {
        Person _analyst;
        Person _customer;
        string _name = "";
        string _parent = "";
        string _notes = "";

        public ProjectProperties()
        {
            _analyst = new Person();
            _customer = new Person();

            _name = "Reset Name";
            _parent = "Reset Parent";

        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        

        public string Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }



        public Person Analyst
        {
            get { return _analyst; }
            set { _analyst = value; }
        }

        public Person Customer
        {
            get { return _customer; }
            set { _customer = value; }
        }

        public string Notes
        {
            get { return _notes; }
            set { _notes = value; }
        }

        internal void WriteToExcel(ExcelExport myWriter)
        {
            int row = 1;
            int column = 1;

            myWriter.SetCellValue(row++, column, "Parent Project");
            myWriter.SetCellValue(row++, column, "Project");
            myWriter.SetCellValue(row++, column, "Customer");
            myWriter.SetCellValue(row++, column, "Customer Company");
            myWriter.SetCellValue(row++, column, "Analyst");
            myWriter.SetCellValue(row++, column, "Analyst Company");
            myWriter.SetCellValue(row++, column, "Notes");
            

            row = 1;
            column = 2;

            myWriter.SetCellValue(row++, column, _parent);
            myWriter.SetCellValue(row++, column, _name);
            myWriter.SetCellValue(row++, column, _customer.Name);
            myWriter.SetCellValue(row++, column, _customer.Company);
            myWriter.SetCellValue(row++, column, _analyst.Name);
            myWriter.SetCellValue(row++, column, _analyst.Company);            

            myWriter.Workbook.AutoFitColumn(1, 2);

            myWriter.SetCellValue(row, column, _notes);
            myWriter.SetCellTextWrapped(row, column, true);

            if (Notes != null)
            {
                var lineCount = Notes.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList().Count;

                myWriter.MergeCells(row, column, row + lineCount, column + 7);
            }
        }
    }
}
