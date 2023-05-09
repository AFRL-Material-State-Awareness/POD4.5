using PdfiumViewer;
using System.Windows.Forms;

namespace POD.Controls
{
    public class PDFLoader : PdfRenderer, IPDFLoader
    {
        public PDFLoader() { }

        public IPdfDocument LoadPDF(IWin32Window owner, string pdfFile)
        {
            PdfDocument mypdfDocument = PdfDocument.Load(owner, pdfFile);
            // Load PDF Document into WinForms Control
            Load(mypdfDocument);
            return mypdfDocument;
        }
    }
    public interface IPDFLoader
    {
        IPdfDocument LoadPDF(IWin32Window owner, string pdfFile);
    }
}
