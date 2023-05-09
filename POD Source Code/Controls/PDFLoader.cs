using PdfiumViewer;
using System.Windows.Forms;

namespace POD.Controls
{
    public class PDFLoader : PdfRenderer, IPDFLoader
    {
        public PDFLoader() { }

        public IPdfDocument LoadPDF(IWin32Window owner, string pdfFile)
        {
            PdfDocument mypdfDoucment = PdfDocument.Load(owner, pdfFile);
            // Load PDF Document into WinForms Control
            Load(mypdfDoucment);
            return mypdfDoucment;
        }
    }
    public interface IPDFLoader
    {
        IPdfDocument LoadPDF(IWin32Window owner, string pdfFile);
    }
}
