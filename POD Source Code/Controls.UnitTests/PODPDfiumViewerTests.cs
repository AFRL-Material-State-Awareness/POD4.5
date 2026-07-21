// <copyright file="PODPDfiumViewerTests.cs" company="The University of Dayton">
// Copyright © 2025 The University of Dayton. All rights reserved.
//
// Licensed under the GNU General Public License, Version 3.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.gnu.org/licenses/gpl-3.0.txt
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using POD.Controls;
using NUnit.Framework;
using Moq;
using System.Windows.Forms;
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
            _pdfLoader.Setup(pdf => pdf.LoadPDF(It.IsAny<PODPdfiumViewer>(), It.IsAny<IWin32Window>(), It.IsAny<string>())).Returns(_pdfDocument.Object);
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
            _pdfLoader.Verify(pdf => pdf.LoadPDF(It.IsAny<PODPdfiumViewer>(), It.IsAny<IWin32Window>(), It.IsAny<string>()), Times.Never);
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
            _pdfLoader.Verify(pdf => pdf.LoadPDF(It.IsAny<PODPdfiumViewer>(), It.IsAny<IWin32Window>(), It.IsAny<string>()));
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
