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
namespace Controls.UnitTests
{
    
    [TestFixture]
    public class LinkLayoutTests
    {
        private LinkLayout _linkLayout;
        private Mock<IStreamReaderWrapper> _streamReader;
        private Mock<IFileExistsWrapper> _file;
        [SetUp]
        public void Setup()
        {
            _file = new Mock<IFileExistsWrapper>();
            _streamReader = new Mock<IStreamReaderWrapper>();
            _linkLayout = new LinkLayout(_file.Object, _streamReader.Object);
        }
        /// Tests For the LoadFile(string myPath)
        [Test]
        public void LoadFile_FilePathDoesNotExist_NobooksmarksAreAddedToTheControlsOrLabels()
        {
            //Arrange
            _file.Setup(f => f.Exists("ThisIsAnInvalidFilePath")).Returns(false);
            //Act
            _linkLayout.LoadFile("ThisIsAnInvalidFilePath");
            //Assert
            _streamReader.Verify(sr => sr.CreateStreamReader(It.IsAny<string>(), It.IsAny<Encoding>()), Times.Never);
            Assert.That(_linkLayout.Links, Is.Not.Null);
            Assert.That(_linkLayout.Links.Count, Is.Zero);
        }
        [Test]
        public void LoadFile_FilePathDoesExistsAndStreamReaderIsEmpty_NobooksmarksAreAddedToTheControlsOrLabels()
        {
            //Arrange
            _file.Setup(f => f.Exists("ThisIsAValidFilePath")).Returns(true);
            Stream stream = GenerateStreamFromString(String.Empty);
            _streamReader.Setup(sr => sr.CreateStreamReader("ThisIsAValidFilePath", Encoding.Unicode)).Returns(new StreamReader(stream));
            //Act
            _linkLayout.LoadFile("ThisIsAValidFilePath");
            //Assert
            _streamReader.Verify(sr => sr.CreateStreamReader(It.IsAny<string>(), It.IsAny<Encoding>()), Times.Once);
            Assert.That(_linkLayout.Links, Is.Not.Null);
            Assert.That(_linkLayout.Links.Count, Is.Zero);
        }
        [Test]
        public void LoadFile_FilePathExistsAndStreamReaderIsNotEmpty_Add3LinksToLabelsAndControls()
        {
            string lines = "Bookmark_1" + '\n' +
                "Bookmark_2" + '\n' +
                "Bookmark_3" + '\n';
            _file.Setup(f => f.Exists("ThisIsAValidFilePath")).Returns(true);
            Stream stream = GenerateStreamFromString(lines);
            _streamReader.Setup(sr => sr.CreateStreamReader("ThisIsAValidFilePath", Encoding.Unicode)).Returns(new StreamReader(stream));
            //Act
            _linkLayout.LoadFile("ThisIsAValidFilePath");
            //Assert
            _streamReader.Verify(sr => sr.CreateStreamReader(It.IsAny<string>(), It.IsAny<Encoding>()), Times.Once);
            Assert.That(_linkLayout.Links, Is.Not.Null);
            Assert.That(_linkLayout.Links.Count, Is.EqualTo(3));
            Assert.That(_linkLayout.Controls.Count, Is.EqualTo(3));
        }
        private Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
