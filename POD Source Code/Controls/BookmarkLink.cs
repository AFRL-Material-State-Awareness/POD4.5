// <copyright file="BookmarkLink.cs" company="The University of Dayton">
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
using System.Text.RegularExpressions;

namespace POD.Controls
{
    public class BookmarkLink : Label
    {
        string _path = "";
        List<string> _componets = new List<string>();
        int levelIncrement = 20;

        public int LeftPadding
        {
            get { return Padding.Left; }
            set
            {
                Padding = new Padding(value, Padding.Top, Padding.Right, Padding.Bottom); 
            }
        }
        public bool Valid { get; set; }

        public List<string> PathComponents
        {
            get
            {
                var list = _componets.ToList();

                for (int i = 0; i < list.Count; i++)
                    list[i] = list[i].Trim();

                return list; 
            }
        }

        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;

                ProcessPath();
            }
        }

        public BookmarkLink()
        {
            Path = "";
            //AutoSize = true;
            //Dock = DockStyle.Left;
        }

        public BookmarkLink(string path)
        {
            Path = path;
            AutoSize = true;
            Dock = DockStyle.Left;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            ForeColor = Color.Blue;
            if(Font.Bold)
                Font = new Font(Font, FontStyle.Underline | FontStyle.Bold);
            else
                Font = new Font(Font, FontStyle.Underline);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            ForeColor = Color.Black;
            if (Font.Bold)
                Font = new Font(Font, FontStyle.Bold);
            else
                Font = new Font(Font, FontStyle.Regular);
        }

        protected void ProcessPath()
        {
            var splitters = new char[] {'|'};
            var items = Path.Split(splitters, StringSplitOptions.RemoveEmptyEntries);

            _componets = items.ToList();

            if (_componets.Count > 0)
            {
                var leftPadding = -levelIncrement;

                foreach (string level in _componets)
                {
                    leftPadding += levelIncrement;
                }

                Padding = new Padding(leftPadding, 2, 0, 5);

                var labelText = _componets.Last();

                labelText = Regex.Replace(labelText, @"[^\u0000-\u007F]", string.Empty);

                Text = labelText;
                Valid = true;
            }
            else
            {
                Text = "Undefined";
                Valid = false;
            }
        }
    }
}
