// <copyright file="PODNumericUpDown.cs" company="The University of Dayton">
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
    //http://stackoverflow.com/questions/2230454/c-sharp-set-usercontrol-value-without-invoking-the-valuechanged-event

    public partial class PODNumericUpDown : NumericUpDown
    {
        EventHandler eventHandler = null;

        public new event EventHandler ValueChanged
        {
            add
            {
                eventHandler += value;
                base.ValueChanged += value;
            }

            remove
            {
                eventHandler -= value;
                base.ValueChanged -= value;
            }
        }

        public new decimal Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                base.ValueChanged -= eventHandler;
                base.Value = value;
                base.ValueChanged += eventHandler;
            }
        }

       // Analysis outputs that could not be calculated arrive as this sentinel (a NaN result
       // swapped to a finite, displayable/exportable value upstream). Read-only boxes show it as "N/A".
       public const decimal NotCalculableSentinel = -9999999999m;

       public static string FormatOutput(double value)
       {
           return Math.Round(value, 3) == 0.0 ? value.ToString("0.000###") : value.ToString("0.000");
       }

       protected override void UpdateEditText()
       {
           if (ReadOnly)
               this.Text = Value == NotCalculableSentinel ? "N/A" : FormatOutput((double)Value);
           else
               this.Text = Value.ToString("0.000###");
        }



        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if(ReadOnly)
            {
                var control = Controls[0];
                //control must be invisible or paint will not work
                control.Visible = false;

                var color = BackColor;
                var pen = new SolidBrush(color);


                var graphics = e.Graphics;

                graphics.FillRectangle(pen, new Rectangle(control.Location, control.Size));
            }

        }

        public void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
            }
        }

        private void PaintOverButtons(object sender, PaintEventArgs e)
        {
        }

        public override void DownButton()
        {
            if (ReadOnly)
                return;
            base.DownButton();
        }

        public override void UpButton()
        {
            if (ReadOnly)
                return;
            base.UpButton();
        }

        
    }
}
