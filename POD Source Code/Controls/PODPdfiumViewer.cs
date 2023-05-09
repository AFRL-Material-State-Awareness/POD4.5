using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PdfiumViewer;
namespace POD.Controls
{
    public class PODPdfiumViewer : PdfRenderer
    {
        IPDFLoader _pdfLoader;
        string _pdfFile;
        bool _loaded;
        IPdfDocument _mypdfDocument;
        public string PdfFileName
        {
            get { return _pdfFile; }
            set { _pdfFile = value; }
        }
        // Inject dependencies in contructor for testing purposes (do not use args in code)
        public PODPdfiumViewer(IPDFLoader pdfLoader = null, bool loaded = false,
            IPdfDocument pdfDoc = null)
        {
            _pdfLoader = pdfLoader ?? new PDFLoader();
            _loaded = loaded;
            _mypdfDocument = pdfDoc;
        }

        void PODPdfViewer_PdfLoaded(object sender, EventArgs args)
        {
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == System.Windows.Forms.Keys.PageUp)
            {
                Page -= 1;
                return true;
            }

            if (keyData == System.Windows.Forms.Keys.PageDown)
            {
                Page += 1;
                return true;
            }

            if (keyData == Keys.Home)
            {
                Page = 1;
                return true;
            }

            if (keyData == Keys.End)
            {
                Page = _mypdfDocument.PageCount;
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        Control _active;

        protected override void OnMouseEnter(EventArgs e)
        {
            var form = FindForm();

            _active = form.ActiveControl;

            Focus();

            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (_active != null)
                _active.Focus();

            base.OnMouseLeave(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            Focus();

            base.OnMouseClick(e);
        }

        public bool OpenPDF()
        {
            if (!_loaded)
            {
                _mypdfDocument=_pdfLoader.LoadPDF(this, _pdfFile);

                _loaded = true;

                return true;
            }
            return false;
        }
        public int PageCount => _mypdfDocument?.PageCount ?? 0;

        public PdfBookmarkCollection Bookmarks => _mypdfDocument?.Bookmarks;
    }

}

