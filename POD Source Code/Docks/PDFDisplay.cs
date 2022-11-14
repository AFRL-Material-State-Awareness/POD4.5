using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Spire.PdfViewer.Forms;
using PdfiumViewer;
namespace POD.Docks
{
    public delegate void SwitchHelpViewHandler(object sender, HelpView view);

    public partial class PDFDisplay : PodDock
    {
        private int _zoom = 100;
        private bool _isZoomDynamic = false;
        PDFDisplay _sibling;
        static bool _synced = false;
        [NonSerialized]
        public SwitchHelpViewHandler SwitchHelpView = null;
        [NonSerialized]
        public EventHandler SwitchBack = null;
        //private Controls.PODPdfiumViewer pdfiumViewer;
        public PDFDisplay(string name)
        {
            InitializeComponent();

            Label = name;
            TopMost = true;
            Text = name;
            AutoScroll = true;
            HideOnClose = true;
            Load += PDFDisplay_Load;

            PdfiumViwerInitialize();
        }
        void PdfiumViwerInitialize()
        {
            this.Controls.SetChildIndex(this.pdfiumViewer, 0);
            /*
            pdfiumViewer = new Controls.PODPdfiumViewer();
            //pdfiumViewer.Width = 200;
            //pdfiumViewer.Height = 200;
            pdfiumViewer.Location = new Point(0, 0);
            pdfiumViewer.BackColor = System.Drawing.Color.Blue;
            this.pdfiumViewer.Enabled = true;
            this.pdfiumViewer.Name = "podPdfiumViewer";
            this.pdfiumViewer.PdfFileName = null;
            this.pdfiumViewer.Size = new System.Drawing.Size(250, 250);
            this.pdfiumViewer.TabIndex = 0;
            this.pdfiumViewer.Text = "podPdfViewer1";
            //this.pdfiumViewer.Threshold = 60;
            //this.toolTip1.SetToolTip(this.podPdfViewer1, "Double click to switch back analysis/project.");
            //this.podPdfViewer1.ZoomMode = Spire.PdfViewer.Forms.ZoomMode.FitWidth;
            this.pdfiumViewer.ZoomMode = PdfViewerZoomMode.FitWidth;
            this.pdfiumViewer.Click += new System.EventHandler(this.podPdfViewer1_Click);
            this.pdfiumViewer.DoubleClick += new System.EventHandler(this.pdf_DoubleClick);
            this.Controls.Add(this.pdfiumViewer);
            */
        }
        void PDFDisplay_Load(object sender, EventArgs e)
        {
          
            //pdfDocumentViewer mouseWheel event

            //podPdfViewer1.MouseWheel += new MouseEventHandler(this.podPdfViewer1_MouseWheel);
            //podPdfViewer1.LostFocus += new EventHandler(this.podPdfViewer1_LostFocus);

            pdfiumViewer.MouseWheel += new MouseEventHandler(this.podPdfViewer1_MouseWheel);
            pdfiumViewer.LostFocus += new EventHandler(this.podPdfViewer1_LostFocus);
            //pdfiumViewer.Scroll += PdfiumViewer_Scroll;
            this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
            this.btnZoonIn.Click += new System.EventHandler(this.btnZoonIn_Click);
            this.btnDynamic.Click += new System.EventHandler(this.btnDynamic_Click);
            this.btnActural.Click += new System.EventHandler(this.btnActural_Click);
            this.btnFitPage.Click += new System.EventHandler(this.btnFitPage_Click);
            this.btnFitWidth.Click += new System.EventHandler(this.btnFitWidth_Click);
            this.btnDualMntr.Click += new System.EventHandler(this.btnDualMntr_Click);
            this.btnLrgMntr.Click += new System.EventHandler(this.btnLrgMntr_Click);
            this.btnSmllMntr.Click += new System.EventHandler(this.btnSmllMntr_Click);

        }

        private void PdfiumViewer_Scroll(object sender, ScrollEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void btnSmllMntr_Click(object sender, EventArgs e)
        {
            if(SwitchHelpView != null)
            {
                SwitchHelpView.Invoke(this, HelpView.Small);
                //btnFitPage.PerformClick();
            }
        }

        private void btnLrgMntr_Click(object sender, EventArgs e)
        {
            if(SwitchHelpView != null)
            {
                SwitchHelpView.Invoke(this, HelpView.Large);
                //btnActural.PerformClick();
            }
        }

        private void btnDualMntr_Click(object sender, EventArgs e)
        {
            if(SwitchHelpView != null)
            {
                SwitchHelpView.Invoke(this, HelpView.Dual);
                //btnFitPage.PerformClick();
            }
        }

        public PDFDisplay Sibling
        {
            get { return _sibling; }
            set { _sibling = value; }
        }

        public string PdfFilename
        {
            set
            {
                //podPdfViewer1.PdfFileName = value;

                pdfiumViewer.PdfFileName = value;
            }
        }

        internal void OpenBookmark(List<string> path)
        {
            //if(podPdfViewer1.OpenPDF())
            //    btnFitWidth.PerformClick();

            if (pdfiumViewer.OpenPDF())
                btnFitWidth.PerformClick();

            /*
            var bookmarks = podPdfViewer1.GetBookmarkContainer();
            
            
            
            int index = 0;

            foreach (PdfDocumentBookmark bookmark in bookmarks.Childs)
            {
                if (bookmark.Title.Trim() == path[index])
                {
                    if (index < path.Count - 1)
                    {
                        var finalBookamrk = ProcessBookmark(bookmark, path, ++index);

                        //finalBookamrk.Destination.Locaton = new PointF(1440.3F, 660.0F);

                        podPdfViewer1.GoToBookmark(finalBookamrk);
                    }
                    else
                    {
                        podPdfViewer1.GoToBookmark(bookmark);
                    }
                    break;
                }
            }
            */
            var bookmarks = pdfiumViewer.Bookmarks;
            int index = 1;
            foreach (PdfBookmark bookmark in bookmarks)
            {
                
                string currCheck=bookmark.Title.Substring(3);
                //foreach(PdfBookmark subBookMark in bookmark.Children)
                //{
                //    Console.WriteLine(subBookMark);
                //}
                if (bookmark.Title.Trim() == path[index] || path[index].Contains(currCheck))
                {
                    if (index < path.Count - 1)
                    {
                        var finalBookamrk = ProcessBookmark(bookmark, path, ++index);

                        //finalBookamrk.Destination.Locaton = new PointF(1440.3F, 660.0F);

                        //podPdfViewer1.GoToBookmark(finalBookamrk);
                        pdfiumViewer.Page = finalBookamrk.PageIndex;
                        //finalBookamrk.
                    }
                    else
                    {
                        //podPdfViewer1.GoToBookmark(bookmark);
                        pdfiumViewer.Page = bookmark.PageIndex;
                    }
                    break;
                }
            }
        }

        public void OpenSimplePDF()
        {
            //podPdfViewer1.OpenPDF();
            pdfiumViewer.OpenPDF();
        }

        private PdfDocumentBookmark ProcessBookmark(PdfDocumentBookmark bookmark, List<string> path, int index)
        {
            foreach (PdfDocumentBookmark childMark in bookmark.Children)
            {
                if (childMark.Title.Trim() == path[index] || childMark.Title.Trim() == ("ColorFound" + path[index]))
                {
                    if (index != path.Count - 1)
                    {
                        //if (childMark.Title.StartsWith("ColorFound"))
                        //    path[index] = "ColorFound" + path[index];

                        return ProcessBookmark(childMark, path, ++index);
                    }
                    else
                        return childMark;
                }
            }

            return bookmark;
        }

        private PdfBookmark ProcessBookmark(PdfBookmark bookmark, List<string> path, int index)
        {
            foreach (PdfBookmark childMark in bookmark.Children)
            {
                if (childMark.Title.Trim() == path[index] || childMark.Title.Trim() == ("ColorFound" + path[index]))
                {
                    if (index != path.Count - 1)
                    {
                        //if (childMark.Title.StartsWith("ColorFound"))
                        //    path[index] = "ColorFound" + path[index];

                        return ProcessBookmark(childMark, path, ++index);
                    }
                    else
                        return childMark;
                }
            }

            return bookmark;
        }

        private void podPdfViewer1_Click(object sender, EventArgs e)
        {

        }
        
        private void ClickForSibling(ToolStripItem control)
        {
            if(control != null && Sibling != null && _synced == false)
            {
                var button = control as ToolStripButton;

                if (button != null)
                {
                    _synced = true;
                    ClickMatchingButton(button);
                    
                    return;
                }
            }
        }

        private void ClickMatchingButton(ToolStripButton button)
        {
            foreach(ToolStripItem item in Sibling.toolStrip1.Items)
            {
                var toolButton = item as ToolStripButton;

                if(toolButton != null)
                {
                    if (toolButton.Name == button.Name)
                        toolButton.PerformClick();
                }
            }
        }

        private void podPdfViewer1_LostFocus(Object sender, EventArgs args)
        {
            this._isZoomDynamic = false;
            this._zoom = 100;
        }

        private void podPdfViewer1_MouseWheel(Object sender, MouseEventArgs args)
        {
            if (this._isZoomDynamic)
            {
                int wheelValue = (Int32)args.Delta / 24;


                this._zoom += wheelValue;

                if (this._zoom < 0)
                    this._zoom = 0;
                //this.podPdfViewer1.ZoomTo(this._zoom);
                pdfiumViewer.Zoom=this._zoom;
            }
            //else
            //{
            //    int wheelValue = -(Int32)args.Delta / 12;
            //    this._zoom += wheelValue;
            //    if (this._zoom < 0)
            //        this._zoom = 0;
            //   // MessageBox.Show(this._zoom.ToString());
            //    this.podPdfViewer1.ZoomTo(this._zoom);
            //}



        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            /*
            if (this.podPdfViewer1.PageCount > 0)
            {
                int delta = 10;
                this._zoom += delta;
                this.podPdfViewer1.ZoomTo(this._zoom);
            }
            */
            if (this.pdfiumViewer.PageCount > 0)
            {
                int delta = 10;
                this._zoom += delta;
                //this.podPdfViewer1.ZoomTo(this._zoom);
                //pdfiumViewer.Zoom = this._zoom;
                pdfiumViewer.ZoomIn();
            }

            ClickForSibling(sender as ToolStripItem);
            _synced = false;
        }

        private void btnZoonIn_Click(object sender, EventArgs e)
        {
            /*
            if (this.podPdfViewer1.PageCount > 0)
            {
                int delta = 5;
                this._zoom -= delta;
                if (this._zoom < 0)
                    this._zoom = 0;
                this.podPdfViewer1.ZoomTo(this._zoom);
                pdfiumViewer.Zoom = this._zoom;
            }
            */
            if (this.pdfiumViewer.PageCount > 0)
            {
                int delta = 5;
                this._zoom -= delta;
                if (this._zoom < 0)
                    this._zoom = 0;
                //this.podPdfViewer1.ZoomTo(this._zoom);
                pdfiumViewer.ZoomOut();
            }

            ClickForSibling(sender as ToolStripItem);
            _synced = false;
        }

        private void btnActural_Click(object sender, EventArgs e)
        {
            double fullZoom = 1.0;
            this._isZoomDynamic = false;
            this.btnDynamic.Text = "Zoom Dynamic";
            /*
            if (this.podPdfViewer1.PageCount > 0)
            {
                this.podPdfViewer1.ZoomTo(100);
            }
            */
            if (this.pdfiumViewer.PageCount > 0)
            {
                pdfiumViewer.Zoom = fullZoom;
            }

            ClickForSibling(sender as ToolStripItem);
            _synced = false;
        }



        private void btnFitPage_Click(object sender, EventArgs e)
        {
            /*
            if (this.podPdfViewer1.PageCount > 0)
            {
                this.podPdfViewer1.ZoomTo(ZoomMode.FitPage);
            }
            */
            if (this.pdfiumViewer.PageCount > 0)
            {
                this.pdfiumViewer.ZoomMode = PdfViewerZoomMode.FitBest;
                //this.pdfiumViewer.zoo
            }
            ClickForSibling(sender as ToolStripItem);
            _synced = false;
        }

        private void btnFitWidth_Click(object sender, EventArgs e)
        {
            /*
            if (this.podPdfViewer1.PageCount > 0)
            {
                this.podPdfViewer1.ZoomTo(ZoomMode.FitWidth);
            }
            */
            if (this.pdfiumViewer.PageCount > 0)
            {
                this.pdfiumViewer.ZoomMode = PdfViewerZoomMode.FitWidth;
            }
            ClickForSibling(sender as ToolStripItem);
            _synced = false;
        }

        private void btnDynamic_Click(object sender, EventArgs e)
        {
            this._isZoomDynamic = !this._isZoomDynamic;
            if (this._isZoomDynamic)
            {

                this.btnDynamic.Text = "Cancel dynamic zoom";
                this.btnDynamic.ToolTipText = "Cancel dynamic zoom";
            }

            else
            {
                this.btnDynamic.Text = "Zoom dynamic";
                this.btnDynamic.ToolTipText = "Zoom dynamic";
            }

            ClickForSibling(sender as ToolStripItem);
            _synced = false;
        }

        public void ActualWidth()
        {
            btnActural.PerformClick();
        }

        public void FitWidth()
        {
            btnFitWidth.PerformClick();
        }

        private void pdf_DoubleClick(object sender, EventArgs e)
        {
            if(SwitchBack != null)
            {
                SwitchBack.Invoke(this, e);
            }
        }

        private void RtnToAnlysBtn_Click(object sender, EventArgs e)
        {
            if (SwitchBack != null)
            {
                SwitchBack.Invoke(this, e);
            }
        }

    }
}
