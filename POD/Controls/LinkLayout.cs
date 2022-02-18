using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing;

namespace POD.Controls
{
    public class LinkLayout : PODFlowLayoutPanel
    {
        List<BookmarkLink> _labels;

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

        public LinkLayout()
        {
            FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            //AutoScroll = true;
            //WrapContents = true;
            BackColor = Color.White;
        }

        public BookmarkLink AddLink(string linkPath)
        {
            var bookmark = new BookmarkLink(linkPath);

            if (_labels == null)
                _labels = new List<BookmarkLink>();

            _labels.Add(bookmark);

            Controls.Add(bookmark);

            //bookmark.MouseEnter += bookmark_MouseEnter;

            return bookmark;
        }

        void bookmark_MouseEnter(object sender, EventArgs e)
        {
            if (Focused == false)
                Focus();
        }

        

        public void LoadFile(string myPath)
        {
            if (_labels == null)
                _labels = new List<BookmarkLink>();

            SuspendLayout();

            Controls.Clear();
            _labels.Clear();

            

            if(File.Exists(myPath))
            {
                var reader = new StreamReader(myPath, Encoding.Unicode);
                var line = "";

                while (line != null)
                {
                    line = reader.ReadLine();

                    if (line != null)
                    {
                        AddLink(line);
                    }
                }
            }

            int minWidth = 50000;

            foreach(BookmarkLink link in _labels)
            {
                if (link.LeftPadding < minWidth)
                    minWidth = link.LeftPadding;
            }

            foreach (BookmarkLink link in _labels)
            {
                link.LeftPadding -= minWidth;
            }

            ResumeLayout(false);
            PerformLayout();
        }

        

    }
}
