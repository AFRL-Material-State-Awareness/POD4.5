using System;
using System.Collections.Generic;
using System.Linq;
using POD.Controls;
using NUnit.Framework;
using Moq;
using System.Windows.Forms;
using POD;
using System.Reflection;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;
using POD.Data;
using System.Data;
using CSharpBackendWithR;

namespace Controls.UnitTests
{
    [TestFixture]
    public class BlendPictureBoxTests
    {
        private BlendPictureBox _blendPicBox;
        private Mock<IRichTextBoxWrapper> _rtb;
        [SetUp]
        public void Setup()
        {
            _blendPicBox = new BlendPictureBox();
            _rtb = new Mock<IRichTextBoxWrapper>();
        }
        [Test]
        [TestCase(0, 0)]
        [TestCase(0, 1)]
        [TestCase(1, 0)]
        public void RtbToBitmap_rtbWidthAOrHeight0_ReturnsNew50By50Bitmap(int inputwidth, int inputheight)
        {
            //Arrange
            _rtb.Setup(r => r.Width).Returns(inputwidth);
            _rtb.Setup(r => r.Height).Returns(inputheight);
            //Act
            var result = BlendPictureBox.RtbToBitmap(_rtb.Object);
            //Assert
            AssertBitMap(result, 50, 50);
        }
        [Test]
        [TestCase(100, 100)]
        [TestCase(1, 100)]
        [TestCase(100, 1)]
        public void RtbToBitmap_rtbIsDisposed_ReturnsNewBitMapWithRTBDimensions(int inputwidth, int inputheight)
        {
            //Arrange
            _rtb.Setup(r => r.Width).Returns(inputwidth);
            _rtb.Setup(r => r.Height).Returns(inputheight);
            _rtb.Setup(r => r.IsDisposed).Returns(true);
            //Act
            var result = BlendPictureBox.RtbToBitmap(_rtb.Object);
            //Assert
            AssertBitMap(result, inputwidth, inputheight);
            _rtb.Verify(r => r.Update(), Times.Never);
            _rtb.Verify(r => r.PointToScreen(Point.Empty), Times.Never);
        }
        [Test]
        [TestCase(100, 100)]
        [TestCase(1, 100)]
        [TestCase(100, 1)]
        public void RtbToBitmap_rtbValid_ReturnsNewBitmapAndUpdatesGraphics(int inputwidth, int inputheight)
        {
            //Arrange
            _rtb.Setup(r => r.Width).Returns(inputwidth);
            _rtb.Setup(r => r.Height).Returns(inputheight);
            _rtb.Setup(r => r.IsDisposed).Returns(false);
            //Act
            var result = BlendPictureBox.RtbToBitmap(_rtb.Object);
            //Assert
            AssertBitMap(result, inputwidth, inputheight);
            _rtb.Verify(r => r.Update());
            _rtb.Verify(r => r.PointToScreen(Point.Empty));
            
        }
        private void AssertBitMap(Bitmap result, int width, int height)
        {
            Assert.IsTrue(result is Bitmap);
            Assert.AreEqual(result.Width, width);
            Assert.AreEqual(result.Height, height);
        }
    }
}
