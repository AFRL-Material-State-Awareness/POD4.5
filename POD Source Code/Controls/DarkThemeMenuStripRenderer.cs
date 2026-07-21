// <copyright file="DarkThemeMenuStripRenderer.cs" company="The University of Dayton">
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
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace POD
{
    public class DarkThemeMenuStripRenderer : ToolStripProfessionalRenderer
    {
        // Colors for the dark theme
        private Color backgroundColor = Color.FromArgb(45, 45, 48); // Dark background
        private Color itemTextColor = Color.White;
        private Color highlightedItemColor = Color.FromArgb(70, 70, 70); // Subtle highlight
        private Color separatorColor = Color.FromArgb(0, 0, 0);
        private Color arrowColor = Color.White;

        // Override rendering methods to apply the dark theme

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            // Background
            Rectangle rect = new Rectangle(Point.Empty, e.Item.Size);
            e.Graphics.FillRectangle(new SolidBrush(backgroundColor), rect);

            // Highlight
            if (e.Item.Selected)
            {
                e.Graphics.FillRectangle(new SolidBrush(highlightedItemColor), rect);
            }

            // Text
            e.Item.ForeColor = itemTextColor;
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            Rectangle rect = new Rectangle(Point.Empty, e.ToolStrip.Size);
            e.Graphics.FillRectangle(new SolidBrush(backgroundColor), rect);
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            // Define the pen for drawing the border
            using (Pen borderPen = new Pen(backgroundColor, 3)) // Adjust thickness if needed
            {
                // Draw the border around the ToolStrip
                e.Graphics.DrawRectangle(borderPen, 0, 0, e.ToolStrip.Width - 2, e.ToolStrip.Height - 2);
            }

            using (Pen borderPen = new Pen(Color.Black, 1)) // Adjust thickness if needed
            {
                // Draw the border around the ToolStrip
                e.Graphics.DrawRectangle(borderPen, 0, 0, e.ToolStrip.Width - 1, e.ToolStrip.Height - 1);
            }
        }

        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            e.ArrowColor = arrowColor;
            base.OnRenderArrow(e);
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            if ((e.Item as ToolStripSeparator) == null)
            {
                base.OnRenderSeparator(e);
                return;
            }

            // Clear any existing drawing in the separator's area (optional, but might be needed)
            e.Graphics.FillRectangle(new SolidBrush(backgroundColor), e.Item.Bounds);

            // Draw the black separator using a Pen
            using (Pen separatorPen = new Pen(Color.Black, 1)) // Adjust thickness if needed
            {
                int x, y, length;
                if (e.ToolStrip.Orientation == Orientation.Horizontal)
                {
                    x = e.Item.Bounds.X;
                    y = e.Item.Bounds.Y + e.Item.Bounds.Height / 2;
                    length = e.Item.Bounds.Width;
                }
                else
                {
                    x = e.Item.Bounds.X + e.Item.Bounds.Width / 2;
                    y = e.Item.Bounds.Y;
                    length = e.Item.Bounds.Height;
                }

                e.Graphics.DrawLine(separatorPen, x, y, x + length, y);
            }
        }
    }
}
