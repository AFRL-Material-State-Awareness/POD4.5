// <copyright file="SimpleActionBar.cs" company="The University of Dayton">
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
using System.Resources;

namespace POD.Controls
{
    public partial class SimpleActionBar : UserControl
    {
        public PODToolTip ToolTip { get; set; }

        public SimpleActionBar()
        {
            InitializeComponent();

            int stdWidth = Globals.StdWidth(this);
            int stdHeight = Globals.StdWidth(this);

            Width = stdWidth;
            Height = stdHeight;
            MinimumSize = new Size(stdWidth, stdHeight);

            flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;

            Margin = new Padding(0, 0, 0, 0);
            Padding = new Padding(0, 0, 0, 0);

            ActionIcons = new ActionIconsList().List;
        }

        public void RemovePadding()
        {
            Margin = new Padding(0, 0, 0, 0);
            Padding = new Padding(0, 0, 0, 0);
        }

        public PODButton AddButton(string myLabel, EventHandler myClickHandler, string myToolTip)
        {
            PODButton button = new PODButton(myLabel);

            button.Name = myLabel.Replace("&", "");

            button.Click += myClickHandler;

            flowLayoutPanel1.Controls.Add(button);

            ToolTip.SetToolTip(button, myToolTip);

            AddIconToButton(button);

            return button;
        }

        public PODButton AddButton(string myLabel, EventHandler myClickHandler)
        {
            return AddButton(myLabel, myClickHandler, "");
        }

        private void AddIconToButton(PODButton button)
        {
            if (button == null)
                return;
           var key = $"{button.Name}.png";

            if (ActionIcons.Images.ContainsKey(key) && button.Image == null)
                button.Image = ActionIcons.Images[key];
            else
                button.Image = ActionIcons.Images[0];
            button.ImageAlign = ContentAlignment.TopCenter;
            button.TextAlign = ContentAlignment.BottomCenter;
            button.TextImageRelation = TextImageRelation.ImageAboveText;
        }

        public Label AddLabel(string myText)
        {
            var myLabel = new Label();

            myLabel.Text = myText;

            myLabel.Font = new Font(myLabel.Font, FontStyle.Underline);
            myLabel.Dock = DockStyle.Fill;
            myLabel.AutoSize = true;
            myLabel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            myLabel.MaximumSize = new System.Drawing.Size(Globals.StdWidth(this), 0);
            myLabel.TextAlign = ContentAlignment.MiddleCenter;

            flowLayoutPanel1.Controls.Add(myLabel);

            return myLabel;
        }

        
    }
}
