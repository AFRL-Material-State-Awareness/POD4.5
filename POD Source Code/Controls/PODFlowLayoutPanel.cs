// <copyright file="PODFlowLayoutPanel.cs" company="The University of Dayton">
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

namespace POD.Controls
{
    public class PODFlowLayoutPanel : FlowLayoutPanel
    {
        //Control _active;

        public PODFlowLayoutPanel()
        {
            ControlAdded += PODFlowLayoutPanel_ControlAdded;
        }

        void PODFlowLayoutPanel_ControlAdded(object sender, ControlEventArgs e)
        {
            e.Control.Click += Control_Click;
        }

        void Control_Click(object sender, EventArgs e)
        {
            Focus();
        }

        

        protected override void OnMouseEnter(EventArgs e)
        {
            //var form = FindForm();

            //_active = form.ActiveControl;

            //Focus();

            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            //if(_active != null)
            //    _active.Focus();

            //base.OnMouseLeave(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            Focus();

            base.OnMouseClick(e);
        }
    }
}
