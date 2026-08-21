// <copyright file="ColorMap.cs" company="The University of Dayton">
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
    public partial class ColorMap : UserControl
    {
        NumericUpDown heightTest = new NumericUpDown();
        Font myFont = new Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Point);

        public ColorMap()
        {
            InitializeComponent();

            Font myFont = heightTest.Font;// new Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Point);

            pictureBox1.Image = ColorManager.TransformImage(pictureBox1.Image);
        }

        private void Paint_ColorMap(object sender, PaintEventArgs e)
        {
            
            
            var format = new StringFormat();
            format.Alignment = StringAlignment.Near;
            format.LineAlignment = StringAlignment.Center;

            e.Graphics.DrawString("PASS", myFont, Brushes.White, 0.0F, Height / 2.0F + 1.0F, format);

           var format2 = new StringFormat();
            format2.Alignment = StringAlignment.Far;
            format2.LineAlignment = StringAlignment.Center;

            e.Graphics.DrawString("FAIL", myFont, Brushes.White, Width, Height / 2.0F + 1.0F, format2);
            
        }

        private void Resize_ColorMap(object sender, EventArgs e)
        {
            Height = heightTest.Height;
            
            //if(Parent != null)
            //    Width = Parent.Width - ((Padding.Left + Padding.Right + Margin.Left + Margin.Right) +
            //            (Parent.Padding.Left + Parent.Padding.Right + Parent.Margin.Left + Parent.Margin.Right));
        }
    }
}
