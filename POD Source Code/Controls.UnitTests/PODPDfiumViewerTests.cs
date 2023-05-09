using POD.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using System.Windows.Forms.DataVisualization.Charting;
using POD;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Reflection;
using POD.Data;
using PdfiumViewer;

namespace Controls.UnitTests
{
    [TestFixture]
    public class PODPDfiumViewerTests
    {
        private Mock<IPDFLoader> _pdfLoader;
        [SetUp]
        public void Setup()
        {
            _pdfLoader = new Mock<IPDFLoader>();
            _pdfLoader.Setup(pdf => pdf.LoadPDF(It.IsAny<IWin32Window>(), It.IsAny<string>())).Returns(new Mock<IPdfDocument>().Object);
        }
        /// <summary>
        /// Tests for the OpenPDF() function
        /// </summary>
        [Test]
        public void OpenPDF_LoadedIsTrue_ReturnsFalseAndLoadPDFNotCalled()
        {
            //Arrange
            PODPdfiumViewer pdfiumViewer = new PODPdfiumViewer(_pdfLoader.Object, true);
            //Act
            var result= pdfiumViewer.OpenPDF();
            //Assert
            _pdfLoader.Verify(pdf => pdf.LoadPDF(It.IsAny<IWin32Window>(), It.IsAny<string>()), Times.Never);
            Assert.That(result, Is.False);
        }
        [Test]
        public void OpenPDF_LoadedIsFalse_ReturnsTrueAndCallsLoadPDF()
        {
            //Arrange
            PODPdfiumViewer pdfiumViewer = new PODPdfiumViewer(_pdfLoader.Object);
            //Act
            var result = pdfiumViewer.OpenPDF();
            //Assert
            _pdfLoader.Verify(pdf => pdf.LoadPDF(It.IsAny<IWin32Window>(), It.IsAny<string>()));
            Assert.That(result, Is.True);
        }

    }
}
