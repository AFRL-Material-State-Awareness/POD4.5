// <copyright file="SideSplitter.cs" company="The University of Dayton">
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
using System.Runtime.InteropServices;

namespace POD.Controls
{
    public partial class SideSplitter : SplitContainer
    {
        public float SnapToSize;        

        public SideSplitter()
        {
            InitializeComponent();

            // Enable default double buffering processing (DoubleBuffered returns true)
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            Point centerPoint = new Point(SplitterRectangle.Left - 3 + SplitterRectangle.Width / 2, SplitterRectangle.Top - 1 + SplitterRectangle.Height / 2);

            e.Graphics.FillEllipse(SystemBrushes.ControlText, centerPoint.X, centerPoint.Y, 3, 3);
            if (Orientation == System.Windows.Forms.Orientation.Horizontal)
            {
                e.Graphics.FillEllipse(SystemBrushes.ControlText, centerPoint.X - 10, centerPoint.Y, 3, 3);
                e.Graphics.FillEllipse(SystemBrushes.ControlText, centerPoint.X + 10, centerPoint.Y, 3, 3);
            }
            else
            {
                e.Graphics.FillEllipse(SystemBrushes.ControlText, centerPoint.X, centerPoint.Y - 10, 3, 3);
                e.Graphics.FillEllipse(SystemBrushes.ControlText, centerPoint.X, centerPoint.Y + 10, 3, 3);
            }
        }
    }
}
