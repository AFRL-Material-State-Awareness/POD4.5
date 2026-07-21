// <copyright file="PODButton.cs" company="The University of Dayton">
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
using System.Drawing;


namespace POD.Controls
{
    public class PODButton : Button
    {
        PODToolTip _tip = null;

        public PODToolTip PODToolTip
        {
            get { return _tip; }
            set
            {
                _tip = value;

                if(_tip != null && TipForControl != null)
                    _tip.SetToolTip(this, TipForControl);
            }
        }
        string _tooltip = "";

        public string TipForControl
        {
            get { return _tooltip; }
            set { _tooltip = value; }
        }

        public PODButton()
        {

            //UseMnemonic = true;
            Height = StdHeight;
            Width = StdWidth;

            Padding = new Padding(0);
            Margin = new Padding(0);
        }

        public PODButton(string myLabel)
        {
            //UseMnemonic = true;

            Height = StdHeight;
            Width = StdWidth;

            Text = myLabel;

            Name = myLabel.Replace("&", "");

            Padding = new Padding(0);
            Margin = new Padding(0);
        }

        public PODButton(string myLabel, string tooltip, PODToolTip myTip)
        {
            //UseMnemonic = true;

            _tip = myTip;
            _tooltip = tooltip;

            myTip.ShowAlways = true;

            Height = StdHeight;
            Width = StdWidth;

            Text = myLabel;

            Name = myLabel.Replace("&", "");

            myTip.SetToolTip(this, _tooltip);

            Padding = new Padding(0);
            Margin = new Padding(0);
        }

        public PODButton(PODToolTip tip, string tooltip)
        {
            _tip = tip;
            _tooltip = tooltip;

            _tip.SetToolTip(this, _tooltip);
        }

        public int StdWidth
        {
            get
            {
                return Globals.StdWidth(this);
            }
        }

        public int StdHeight
        {
            get 
            {
                return Globals.StdHeight(this);
            }
        }

        public virtual void UpdateLabel()
        {
            
        }
    }

    public class ButtonHolder
    {
        public string Name { get; set; }
        public Image Image { get; set; }
        public EventHandler Event { get; set; }
        public string ToolTip { get; set; }

        public ButtonHolder(string name, Image image, EventHandler handler, string toolTip = "")
        {
            Name = name;
            Image = image;
            Event = handler;
            ToolTip = toolTip;
        }
    }
}
