using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using POD.Controls;
using System.Diagnostics;

namespace POD.Docks
{
    public partial class RTFViewerDock : PodDock
    {
        protected string _rtfKind;
        public PDFDisplay PDFViewer = null;
        public LinkLayout LinkWindow { get { return LinkPanel; } }
        [NonSerialized]
        public EventHandler NeedViewer;
        [NonSerialized]
        public EventHandler SwitchBack = null;
        [NonSerialized]
        public SwitchHelpViewHandler SwitchHelpView = null;
        [NonSerialized]
        //static private Point SavedScrollPosition = new Point(0, 0);
        static Dictionary<string, Point> ScrollPostions = new System.Collections.Generic.Dictionary<string, Point>();
        string rtfFile = "";
        static int maxScroll = 0;

        public RTFViewerDock()
        {
            InitializeComponent();

            _rtfKind = GetType().Name;
            int dockIndex = _rtfKind.IndexOf("Dock");
            _rtfKind = _rtfKind.Remove(dockIndex);

            LinkPanel.BringToFront();

            LinkPanel.Paint += LinkPanel_Paint;

            LinkPanel.Parent.SizeChanged += LinkPanel_SizeChanged;

            //GotFocus += RTFViewerDock_GotFocus;
            //LostFocus += RTFViewerDock_LostFocus;
            //Activated += RTFViewerDock_Activated;
            LinkPanel.GotFocus += LinkPanel_GotFocus;
            panel1.Scroll += RTFViewerDock_Scroll;
        }

        void LinkPanel_SizeChanged(object sender, EventArgs e)
        {
            FitWidth();
        }

        public void FitWidth()
        {
            if (Parent != null && Parent.Width > 0)
                LinkPanel.MaximumSize = new Size(Parent.Width - SystemInformation.VerticalScrollBarWidth - 3, 0);
        }

        void LinkPanel_GotFocus(object sender, EventArgs e)
        {
            RestoreScollPosition(rtfFile);        
        }

        void RTFViewerDock_Scroll(object sender, ScrollEventArgs e)
        {
            StoreScrollPosition();
        }

        private void StoreScrollPosition()
        {
            if (panel1.AutoScrollPosition.Y < 0)
            {
                var SavedScrollPosition = panel1.AutoScrollPosition;
                SavedScrollPosition.Y = -(SavedScrollPosition.Y);

                if (ScrollPostions.ContainsKey(rtfFile))
                    ScrollPostions[rtfFile] = SavedScrollPosition;
                else
                    ScrollPostions.Add(rtfFile, SavedScrollPosition);

                maxScroll = LinkPanel.Height;
            }
        }

        void RTFViewerDock_Activated(object sender, EventArgs e)
        {
            //RestoreScollPosition(rtfFile);    
        }

        void RTFViewerDock_LostFocus(object sender, EventArgs e)
        {
            //SavedScrollPosition = AutoScrollPosition;
            //SavedScrollPosition.Y = -SavedScrollPosition.Y;
        }

        void RTFViewerDock_GotFocus(object sender, EventArgs e)
        {
            //RestoreScollPosition(rtfFile);    
        }

        void LinkPanel_Paint(object sender, PaintEventArgs e)
        {
            if(LinkPanel.MaximumSize.Width == 0)
            {
                if (Parent.Width > 0)
                    LinkPanel.MaximumSize = new Size(Parent.Width - SystemInformation.VerticalScrollBarWidth - 3, 0);
            }
        }

        public Panel ListPanel
        {
            get
            {
                return panel1;
            }
        }

        public FlowLayoutPanel Links
        {
            get
            {
                return LinkPanel;
            }
            set
            {
                Links = value;
            }
        }

        public void LoadRTF(string myPath)
        {
            //RestoreScollPosition(myPath); 

            BlendBox.CaptureOldStateImage(panel1);

            try
            {
                //Stopwatch watch = new Stopwatch();

                //watch.Start();

                LinkPanel.SuspendLayout();

                //StoreScrollPosition();

                rtfFile = myPath;

                LinkPanel.LoadFile(myPath);

                LinkPanel.ResumeLayout(false);
                LinkPanel.PerformLayout();

                if(Parent.Width > 0)
                    LinkPanel.MaximumSize = new Size(Parent.Width - SystemInformation.VerticalScrollBarWidth - 3, 0);

                //panel1.HorizontalScroll.Maximum = 0;
                //panel1.AutoScroll = false;
                //panel1.VerticalScroll.Visible = false;
                //panel1.AutoScroll = true;

                RestoreScollPosition(myPath); 

                //LinkPanel.AutoScroll = false;
                LinkPanel.AutoSize = true;
                //LinkPanel.AutoScroll = true;

                //watch.Stop();

                //MessageBox.Show((watch.ElapsedMilliseconds / 1000.0).ToString("F3"));

                ListPanel.BackColor = LinkPanel.BackColor;

                foreach(BookmarkLink link in LinkPanel.Links)
                {
                    link.Click += link_Click;
                    //link.Dock = DockStyle.None;
                    //link.Dock = DockStyle.Fill;
                    if (link.Padding.Left == 0)
                        link.Font = new Font(link.Font, FontStyle.Bold);

                }
            }
            catch(Exception exp)
            {
                MessageBox.Show(exp.Message, "POD v4 Error");
            }

            BlendBox.CaptureNewImage(panel1);
            BlendToNextImage();
        }

        private void RestoreScollPosition(string myPath)
        {
            if (ScrollPostions.ContainsKey(myPath))
            {
                //panel1.AutoScrollPosition = new Point(0, panel1.AutoScrollPosition.Y);

                var posi = ScrollPostions[myPath];

                panel1.AutoScrollPosition = posi;

                //while(-panel1.AutoScrollPosition.Y < posi.Y)
                //{
                //    Application.DoEvents();

                //    //posi = new Point(0, panel1.AutoScrollPosition.Y + posi.Y);
                //    //posi.Y--;
                //    //panel1.AutoScrollPosition = posi;// new Point(0, -1);
                //    panel1.VerticalScroll.Value = panel1.VerticalScroll.Maximum;
                //    panel1.PerformLayout();

                //}
            }
            else
                panel1.AutoScrollPosition = new Point(0, 0);
        }

        void link_Click(object sender, EventArgs e)
        {
            var link = sender as BookmarkLink;

            if (link != null)
            {
                GetViewer();

                PDFViewer.OpenBookmark(link.PathComponents);
            }
        }

        private void GetViewer()
        {
            if(NeedViewer != null)
            {
                NeedViewer.Invoke(this, null);
            }
        }

        public void SetRTF(string myText)
        {
            string path = Application.StartupPath + "\\" + _rtfKind + "\\" + myText;

            if (File.Exists(path))
            {
                LoadRTF(path);
            }
            else
            {
                LinkPanel.Message = "Could not find " + _rtfKind + " file! Make sure the corresponding is located at " + path + ".\n\nPlease double check spelling and ensure the related wizard or step class name hasn't been changed. If so the previous file's name will need to be updated.";
            }
        }

        public void Clear()
        {
            BlendBox.CaptureOldStateImage(LinkPanel);

            LinkPanel.Controls.Clear();

            BlendBox.CaptureNewImage(LinkPanel);

            BlendToNextImage();
        }

        public string HelpName { get; set; }

        private void RtnToAnlysBtn_Click(object sender, EventArgs e)
        {
            if (SwitchBack != null)
            {
                SwitchBack.Invoke(this, e);
            }
        }

        private void btnSmllMntr_Click(object sender, EventArgs e)
        {
            if (SwitchHelpView != null)
            {
                SwitchHelpView.Invoke(this, HelpView.Small);
            }
        }

        private void btnLrgMntr_Click(object sender, EventArgs e)
        {
            if (SwitchHelpView != null)
            {
                SwitchHelpView.Invoke(this, HelpView.Large);
            }
        }

        private void btnDualMntr_Click(object sender, EventArgs e)
        {
            if (SwitchHelpView != null)
            {
                SwitchHelpView.Invoke(this, HelpView.Dual);
            }
        }
    }
}
