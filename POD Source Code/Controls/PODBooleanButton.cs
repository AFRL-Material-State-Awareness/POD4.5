// <copyright file="PODBooleanButton.cs" company="The University of Dayton">
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
    public partial class PODBooleanButton : PODButton
    {
        private string _trueText;
        private string _falseText;
        private bool _state;

        public PODBooleanButton(string myFalse, string myTrue, bool myState, string tooltip, PODToolTip myTip)
        {
            //UseMnemonic = true;

            myTip.ShowAlways = true;

            _falseText = myFalse;
            _trueText = myTrue;
            _state = myState;

            Name = myFalse.Replace("&", "");

            myTip.SetToolTip(this, tooltip);

            UpdateLabel();
        }

        public bool ButtonState
        {
            set 
            {
                _state = value;
                UpdateLabel(); 
            }
            get { return _state; }
        }

        public override void UpdateLabel()
        {
            if(_state == true)
            {
                Text = _trueText;
            }
            else
            {
                Text = _falseText;
            }
        }
    }
}
