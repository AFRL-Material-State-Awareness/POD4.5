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
        private Mock<IPdfDocument> _pdfDocument;
        [SetUp]
        public void Setup()
        {
            _pdfLoader = new Mock<IPDFLoader>();
            _pdfDocument = new Mock<IPdfDocument>();
            _pdfDocument.SetupGet(pdfDoc => pdfDoc.PageCount).Returns(1);
            _pdfDocument.SetupGet(bmk => bmk.Bookmarks).Returns(new PdfBookmarkCollection() { new PdfBookmark() });
            _pdfLoader.Setup(pdf => pdf.LoadPDF(It.IsAny<IWin32Window>(), It.IsAny<string>())).Returns(_pdfDocument.Object);
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
        // tests for PageCount getter
        [Test]
        public void PageCount_MyPDFDocumentIsEmpty_Returns0()
        {
            //Arrange
            PODPdfiumViewer pdfiumViewer = new PODPdfiumViewer(_pdfLoader.Object, true);
            //Act
            var result = pdfiumViewer.PageCount;
            //Assert
            Assert.That(result, Is.Zero);
            _pdfDocument.VerifyGet(pdfDoc => pdfDoc.PageCount, Times.Never);
        }
        
        [Test]
        public void PageCount_ContainsMyPDFDocument_ReturnsPageNumberOfMyPDFDoc()
        {
            //Arrange
            PODPdfiumViewer pdfiumViewer = new PODPdfiumViewer(_pdfLoader.Object, true, _pdfDocument.Object);
            //Act
            var result = pdfiumViewer.PageCount;
            //Assert
            Assert.That(result, Is.EqualTo(1));
            _pdfDocument.VerifyGet(pdfDoc => pdfDoc.PageCount);
        }
        // tests for Bookmarks getter
        [Test]
        public void Bookmarks_MyPDFDocumentIsEmpty_ReturnsNull()
        {
            //Arrange
            PODPdfiumViewer pdfiumViewer = new PODPdfiumViewer(_pdfLoader.Object, true);
            //Act
            var result = pdfiumViewer.Bookmarks;
            //Assert
            Assert.That(result, Is.Null);
            _pdfDocument.VerifyGet(pdfDoc => pdfDoc.PageCount, Times.Never);
        }
        [Test]
        public void Bookmarks_MyPDFDocumentIsEmpty_ReturnsBookmarks()
        {
            //Arrange
            PODPdfiumViewer pdfiumViewer = new PODPdfiumViewer(_pdfLoader.Object, true, _pdfDocument.Object);
            //Act
            var result = pdfiumViewer.Bookmarks;
            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
            _pdfDocument.VerifyGet(pdfDoc => pdfDoc.Bookmarks);
        }

    }
}
