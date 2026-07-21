// <copyright file="LambdaNumericUpDown.cs" company="The University of Dayton">
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
using System.Windows.Forms;
namespace POD.Wizards.Steps.AHatVsANormalSteps
{
    //source: Stack overflow
    //Question: https://stackoverflow.com/questions/63191222/numericupdown-custom-increment-using-default-buttons
    //Author : Jimi -- https://stackoverflow.com/users/7444103/jimi
    public class LambdaNumericUpDown : NumericUpDown
    {
        private decimal customIncrement = 0.1m;
        public LambdaNumericUpDown()
        {

        }

        public override void UpButton() =>
        Value = Math.Min(Value + (customIncrement - (Value % customIncrement)), Maximum);

        public override void DownButton() =>
            Value = Math.Max(Value - (Value % customIncrement == 0 ? customIncrement : Value % customIncrement), Minimum);

        protected override void OnValueChanged(EventArgs e)
        {
            base.OnValueChanged(e);
            Value = Math.Max(Math.Min(Value, Maximum), Minimum);
        }
    }
}
