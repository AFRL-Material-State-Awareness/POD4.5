// <copyright file="SamplingTypeBox.cs" company="The University of Dayton">
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
    public partial class SamplingTypeBox : ComboBoxListEx
    {
        public SamplingTypeBox()
        {
            InitializeComponent();
            Items.Add(new SampleTypeObj(SamplingTypeEnum.SimpleRandomSampling));
            Items.Add(new SampleTypeObj(SamplingTypeEnum.RankedSetSampling));
            DropDownStyle = ComboBoxStyle.DropDownList;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }
        public SamplingTypeEnum SelectedSamplingType
        {
            get
            {
                return ((SampleTypeObj)SelectedItem).SamplingType;
            }
            set
            {
                foreach (Object obj in Items)
                {
                    var type = obj as SampleTypeObj;

                    if (type?.SamplingType == value)
                        SelectedItem = obj;
                }
            }
        }

    }
    public class SampleTypeObj
    {
        private SamplingTypeEnum _samplingType;
        private string _label;

        public SampleTypeObj(SamplingTypeEnum myType)
        {
            _samplingType = myType;

            switch (SamplingType)
            {
                case SamplingTypeEnum.SimpleRandomSampling:
                    _label = "Simple Random Sampling";
                    break;
                case SamplingTypeEnum.RankedSetSampling:
                    _label = "Ranked Set Sampling";
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

        public SamplingTypeEnum SamplingType
        {
            get { return _samplingType; }
        }

        //Needs to stay in here for the Label text to show up in the combo box
        public override string ToString()
        {
            return Label;
        }
    }
}

