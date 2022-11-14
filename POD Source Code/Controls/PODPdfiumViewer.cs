using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Spire.PdfViewer.Forms;
using System.Windows.Forms;
using PdfiumViewer;
namespace POD.Controls
{
    public class PODPdfiumViewer : PdfRenderer
    {
        //Form _form;
        string _pdfFile;
        bool _loaded = false;
        PdfDocument _mypdfDoucment;
        //PdfRenderer _myPdfRender;
        public string PdfFileName
        {
            get { return _pdfFile; }
            set { _pdfFile = value; }
        }

        public PODPdfiumViewer()
        {
            //PdfLoaded += PODPdfViewer_PdfLoaded;
            //_myPdfRender = new PdfRenderer();


        }

        void PODPdfViewer_PdfLoaded(object sender, EventArgs args)
        {
            //_form = FindForm();

            //_form.VerticalScroll.Minimum = 0;
            //_form.VerticalScroll.Maximum = PageCount * 3;      


        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == System.Windows.Forms.Keys.PageUp)
            {
                //GoToPreviousPage();
                //Renderer.Page -= 1;
                Page -= 1;
                return true;
            }

            if (keyData == System.Windows.Forms.Keys.PageDown)
            {
                //GoToNextPage();
                //Renderer.Page += 1;
                Page += 1;
                return true;
            }

            if (keyData == Keys.Home)
            {
                //GoToFirstPage();
                //Renderer.Page = 1;
                Page = 1;
                return true;
            }

            if (keyData == Keys.End)
            {
                //GoToLastPage();
                //Renderer.Page = _mypdfDoucment.PageCount;
                Page = _mypdfDoucment.PageCount;
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
        /*
        public bool OpenPDF()
        {
            if (!_loaded)
            {
                LoadFromFile(_pdfFile);
                
                _loaded = true;

                return true;
            }

            return false;
        }
        */
        public bool OpenPDF()
        {
            if (!_loaded)
            {
                //LoadFromFile(_pdfFile);
                _mypdfDoucment = PdfDocument.Load(this, _pdfFile);
                // Load PDF Document into WinForms Control
                Load(_mypdfDoucment);
                _loaded = true;

                return true;
            }

            return false;
        }
    }

}

