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
                var reader = _streamReader.CreateStreamReader(myPath, Encoding.Unicode);
                string line;
                while ((line = reader.ReadLine()) != null)
                    AddLink(line);
            }

            AdjustPadding();

            ResumeLayout(false);
            PerformLayout();
        }

        

    }
    
}
