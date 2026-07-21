// <copyright file="TransformBox.cs" company="The University of Dayton">
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
    public partial class TransformBoxX : TransformBoxControl
    {
        public TransformBoxX()
        {
            InitializeComponent();

            Items.Add(new TransformObj(TransformTypeEnum.Linear));
            Items.Add(new TransformObj(TransformTypeEnum.Log));
            //Items.Add(new TransformObj(TransformTypeEnum.Inverse));
            DropDownStyle = ComboBoxStyle.DropDownList;  
        }
    }

    public class TransformObj
    {
        private TransformTypeEnum _transformType;
        private string _label;

        public TransformObj(TransformTypeEnum myType)
        {
            _transformType = myType;

            switch (TransformType)
            {
                //box cox label only used in choose transform panel. X does not have a box-cox transform in the
                //User interface
                case TransformTypeEnum.BoxCox:
                    _label = "Box-Cox";
                    break;
                case TransformTypeEnum.Log:
                    _label = "Log";
                    break;
                case TransformTypeEnum.Exponetial:
                    _label = "Exponetional";
                    break;
                case TransformTypeEnum.Inverse:
                    _label = "Inverse";
                    break;
                case TransformTypeEnum.Linear:
                    _label = "Linear";
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

        public TransformTypeEnum TransformType
        {
            get { return _transformType; }
        }

        public override string ToString()
        {
            return Label;
        }
    }
}
