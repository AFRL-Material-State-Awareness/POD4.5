// <copyright file="PFModelBox.cs" company="The University of Dayton">
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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POD.Controls
{
    public partial class PFModelBox : ComboBoxListEx
    {
        public PFModelBox()
        {
            InitializeComponent();

            Items.Add(new PFModelObj(HitMissRegressionType.LogisticRegression));
            Items.Add(new PFModelObj(HitMissRegressionType.FirthLogisticRegression));

            DropDownStyle = ComboBoxStyle.DropDownList;
        }

        public HitMissRegressionType SelectedModel
        {
            get { return ((PFModelObj) SelectedItem).ModelType; }
            set
            {
                foreach (Object obj in Items)
                {
                   var type = obj as PFModelObj;

                   if (type?.ModelType == value)
                        SelectedItem = obj;
                }
            }
        }
    }

    public class PFModelObj
    {
        private HitMissRegressionType _modelType = HitMissRegressionType.None;
        private string _label;

        public PFModelObj(HitMissRegressionType myType)
        {
            _modelType = myType;

            switch (ModelType)
            {
                case HitMissRegressionType.LogisticRegression:
                    _label = "Logistic Reg";
                    break;
                case HitMissRegressionType.FirthLogisticRegression:
                    _label = "Firth Logistic Reg";
                    break;
                default:
                    _label = "Undefined";
                    break;
            }
        }

        public string Label
        {
            get { return _label; }
        }

        public HitMissRegressionType ModelType
        {
            get { return _modelType; }
        }

        public override string ToString()
        {
            return Label;
        }
    }
}
