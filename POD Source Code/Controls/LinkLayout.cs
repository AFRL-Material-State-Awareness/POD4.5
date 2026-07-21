// <copyright file="LinkLayout.cs" company="The University of Dayton">
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
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace POD.Controls
{
    public class LinkLayout : PODFlowLayoutPanel
    {
        List<BookmarkLink> _labels;
        private IFileExistsWrapper _file;
        private IStreamReaderWrapper _streamReader;
        public List<BookmarkLink> Links
        {
            get { return _labels; }
            set { _labels = value; }
        }

        public string Message
        {
            set
            {
                Controls.Clear();

                var label = new Label();
                label.Text = value;

                Controls.Add(label);
            }
        }

        public LinkLayout(IFileExistsWrapper file = null, IStreamReaderWrapper stream = null)
        {
            _file = file ?? new FileExistsWrapper();
            _streamReader = stream ?? new StreamReaderWrapper();
            FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            BackColor = Color.White;
        }

        private BookmarkLink AddLink(string linkPath)
        {
            var bookmark = new BookmarkLink(linkPath);

            if (_labels == null)
                _labels = new List<BookmarkLink>();

            _labels.Add(bookmark);

            Controls.Add(bookmark);

            return bookmark;
        }
        private void AdjustPadding()
        {
            int minWidth = int.MaxValue;
            foreach (BookmarkLink link in _labels)
            {
                if (link.LeftPadding < minWidth)
                    minWidth = link.LeftPadding;
            }

            foreach (BookmarkLink link in _labels)
                link.LeftPadding -= minWidth;
        }

        public void LoadFile(string myPath)
        {
            if (_labels == null)
                _labels = new List<BookmarkLink>();
            
            SuspendLayout();

            Controls.Clear();
            _labels.Clear();

            

            if(_file.Exists(myPath))
            {
                using (var reader = _streamReader.CreateStreamReader(myPath, Encoding.Unicode))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                        AddLink(line);
                }
            }

            AdjustPadding();

            ResumeLayout(false);
            PerformLayout();
        }

        

    }
    
}
