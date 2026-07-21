// <copyright file="PODImageNumericUpDown.cs" company="The University of Dayton">
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
    public partial class PODImageNumericUpDown : UserControl, ISupportInitialize
    {

        public PODImageNumericUpDown()
        {
            InitializeComponent();

            UpdateImage();
            NumericUpDown.KeyDown += NumericUpDown.TextBox_KeyDown;
        }

        public bool ReadOnly
        {
            get
            {
                return ValueNumeric.ReadOnly;
            }

            set
            {
                ValueNumeric.ReadOnly = value;
            }
        }

        virtual protected void UpdateImage()
        {
        }

        protected void FixBackgrounColor()
        {
            ImageBox.BackColor = SystemColors.Control;
            tableLayoutPanel1.BackColor = SystemColors.Control;

            if (ValueNumeric.ReadOnly)
                ValueNumeric.BackColor = SystemColors.Control;
            else
                ValueNumeric.BackColor = SystemColors.Window;

            BackColor = SystemColors.Control;

            if (Parent != null)
            {
                Parent.BackColor = SystemColors.Control;
            }

            if (ImageBox.Image != null)
            {
                var top = (ImageBox.Height - ImageBox.Image.Height) / 2 + 1;

                if (top < 0 || ImageBox.Height == ImageBox.Image.Height)
                    top = 0;

                ImageBox.Margin = new Padding(2, top, 0, 0);
            }
            else
            {
                ImageBox.Margin = new Padding(2, 0, 0, 0);
            }

            //ValueNumeric.Dock = DockStyle.Fill;
            //ImageBox.Location = new Point(0, 0);
            //ImageBox.Margin = new Padding(2, 0, 0, 0);
            ImageBox.SizeMode = PictureBoxSizeMode.AutoSize;
        }
        
        protected override void OnSizeChanged(EventArgs e)
        {


            /*if (this.Size.Height != ValueNumeric.Height)
            {
                this.Size = new Size(this.Size.Width, ValueNumeric.Height+4);
                this.tableLayoutPanel1.Size = new Size(this.Size.Width, ValueNumeric.Height+4);
            }

            ImageBox.Height = Size.Height;
            ImageBox.Width = Size.Height;

            this.Dock = DockStyle.Fill;

            /*if (this.Parent != null && this.Width < this.Parent.Width)
            {
                var table = this.Parent as TableLayoutPanel;

                if(table != null)
                {
                    this.Width = table.GetColumnWidths()[1];
                }
            }*/
                

            base.OnSizeChanged(e);
        }

        public Decimal Maximum
        {
            get
            {
                return ValueNumeric.Maximum;
            }

            set
            {
                ValueNumeric.Maximum = value;
            }
        }

        public Decimal Minimum
        {
            get
            {
                return ValueNumeric.Minimum;
            }

            set
            {
                ValueNumeric.Minimum = value;
            }
        }

        public decimal Value
        {
            get
            {
                return ValueNumeric.Value;
            }

            set
            {
                ValueNumeric.Value = value;
            }
        }

        public int DecimalPlaces
        {
            get
            {
                return ValueNumeric.DecimalPlaces;
            }

            set
            {
                ValueNumeric.DecimalPlaces = value;
            }
        }

        public decimal Increment
        {
            get
            {
                return ValueNumeric.Increment;
            }

            set
            {
                ValueNumeric.Increment = value;
            }
        }

        public bool InterceptArrowKeys
        {
            get
            {
                return ValueNumeric.InterceptArrowKeys;
            }

            set
            {
                ValueNumeric.InterceptArrowKeys = value;
            }
        }

        public PODNumericUpDown NumericUpDown
        {
            get
            {
                return ValueNumeric;
            }
        }

        // ISupportInitialize members
        public void BeginInit()
        {
        }

        public void EndInit()
        {
        }

        public string TooltipForNumeric
        {
            get
            {
                return toolTip1.GetToolTip(NumericUpDown);
            }

            set
            {
                toolTip1.SetToolTip(NumericUpDown, value);
            }
        }

    }
}
