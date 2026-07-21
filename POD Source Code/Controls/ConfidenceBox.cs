// <copyright file="ConfidenceBox.cs" company="The University of Dayton">
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
    public partial class ConfidenceBox : ComboBoxListEx
    {
        public ConfidenceBox()
        {
            InitializeComponent();
            Items.Add(new ConfIntObj(ConfidenceIntervalTypeEnum.StandardWald));
            Items.Add(new ConfIntObj(ConfidenceIntervalTypeEnum.ModifiedWald));
            Items.Add(new ConfIntObj(ConfidenceIntervalTypeEnum.LR));
            Items.Add(new ConfIntObj(ConfidenceIntervalTypeEnum.MLR));
            DropDownStyle = ComboBoxStyle.DropDownList;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }
        public ConfidenceIntervalTypeEnum SelectedConfInt
        {
            get
            {
                return ((ConfIntObj)SelectedItem).ConfIntervalType;
            }
            set
            {
                foreach (Object obj in Items)
                {
                    var type = obj as ConfIntObj;

                    if (type?.ConfIntervalType == value)
                        SelectedItem = obj;
                }
            }
        }

    }
    public class ConfIntObj
    {
        private ConfidenceIntervalTypeEnum _transformType;
        private string _label;

        public ConfIntObj(ConfidenceIntervalTypeEnum myType)
        {
            _transformType = myType;

            switch (ConfIntervalType)
            {
                case ConfidenceIntervalTypeEnum.StandardWald:
                    _label = "Std Wald";
                    break;
                case ConfidenceIntervalTypeEnum.ModifiedWald:
                    _label = "Mod Wald";
                    break;
                case ConfidenceIntervalTypeEnum.LR:
                    _label = "LR";
                    break;
                case ConfidenceIntervalTypeEnum.MLR:
                    _label = "MLR";
                    break;
                default:
                    _label = "Custom";
                    break;
            }
        }

        public string Label
        {
            get { return _label; }
        }

        public ConfidenceIntervalTypeEnum ConfIntervalType
        {
            get { return _transformType; }
        }

        //Needs to stay in here for the Label text to show up in the combo box
        public override string ToString()
        {
            return Label;
        }
    }
}
