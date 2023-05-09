using PdfiumViewer;
using System.Windows.Forms;

namespace POD.Controls
{
    public class PDFLoader :  IPDFLoader
    {
        public PDFLoader() { }

        public IPdfDocument LoadPDF(PODPdfiumViewer pdfiumClass, IWin32Window owner, string pdfFile)
        {
            PdfDocument mypdfDocument = PdfDocument.Load( owner, pdfFile);
            // Load PDF Document into WinForms Control
            pdfiumClass.Load(mypdfDocument);
            return mypdfDocument;
        }
    }
    public interface IPDFLoader
    {
        IPdfDocument LoadPDF(PODPdfiumViewer pdfiumClass, IWin32Window owner, string pdfFile);
    }
}
