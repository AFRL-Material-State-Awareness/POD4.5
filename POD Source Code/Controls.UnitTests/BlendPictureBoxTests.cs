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
        /*
         * This test case isn't relavant since there is check of Width and Height greater than 0 before RtbToBitmap is called
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
        */
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

        /// Tests for CaptureOldStateImage(Control myControl) function
        [Test]
        public void CaptureOldStateImage_ControlIsDisposed_BackgroundImageIsNull()
        {
            //Arrange
            Control control = new Control();
            control.Dispose();
            //Act
            _blendPicBox.CaptureOldStateImage(control);
            //Assert
            Assert.That(_blendPicBox.BackgroundImage, Is.Null);
        }
        [Test]
        public void CaptureOldStateImage_ControlTypeIsRTF_AssignsRTBAsBitmap()
        {
            //Arrange
            RichTextBox control = new RichTextBox() { Width = 100, Height = 100 };
            //Act
            _blendPicBox.CaptureOldStateImage(control);
            //Assert
            Assert.That(_blendPicBox.BackgroundImage, Is.Not.Null);
            Assert.AreEqual(_blendPicBox.BackgroundImage.Width, 100);
            Assert.AreEqual(_blendPicBox.BackgroundImage.Height, 100);
        }
        [Test]
        [TestCase(0, 0)]
        [TestCase(-1, -1)]
        public void CaptureOldStateImage_ControlTypeIsNOTRTFORWidthHeightAre0OrLess_AssignsA50By50Bitmap(int width, int height)
        {
            //Arrange
            Control control = new Control() { Width = width, Height = height };
            //Act
            _blendPicBox.CaptureOldStateImage(control);
            //Assert
            Assert.That(_blendPicBox.BackgroundImage, Is.Not.Null);
            Assert.AreEqual(_blendPicBox.BackgroundImage.Width, 50);
            Assert.AreEqual(_blendPicBox.BackgroundImage.Height, 50);
        }
        [Test]
        [TestCase(100, 100)]
        [TestCase(100, 1)]
        [TestCase(1, 100)]
        public void CaptureOldStateImage_ControlTypeIsNOTRTFORWidthHeightAreGreaterThan0_AssignsABitMapOfWidthHeightToBackgroundImage(int width, int height)
        {
            //Arrange
            Control control = new Control() { Width = width, Height = height };
            //Act
            _blendPicBox.CaptureOldStateImage(control);
            //Assert
            Assert.That(_blendPicBox.BackgroundImage, Is.Not.Null);
            Assert.AreEqual(_blendPicBox.BackgroundImage.Width, width);
            Assert.AreEqual(_blendPicBox.BackgroundImage.Height, height);
        }
        [Test]
        [TestCase(0, 0)]
        [TestCase(0, 1)]
        [TestCase(1, 0)]
        [TestCase(-1, -1)]
        [TestCase(0, -1)]
        [TestCase(-1, 0)]
        public void CaptureNewImage_WidthORHeightAre0OrLess_AssignsA50By50Bitmap(int width, int height)
        {
            //Arrange
            Control control = new Control() { Width = width, Height = height };
            //Act
            _blendPicBox.CaptureNewImage(control);
            //Assert
            Assert.AreEqual(_blendPicBox.Image.Width, 50);
            Assert.AreEqual(_blendPicBox.Image.Height, 50);
        }
        [Test]
        [TestCase(100,100)]
        [TestCase(1, 100)]
        [TestCase(100, 1)]
        public void CaptureNewImage_WidthORHeightAreGreaterThan0_AssignsABitmapOfWidthAndHeightToImage(int width, int height)
        {
            //Arrange
            Control control = new Control() { Width = width, Height = height };
            //Act
            _blendPicBox.CaptureNewImage(control);
            //Assert
            Assert.AreEqual(_blendPicBox.Image.Width, width);
            Assert.AreEqual(_blendPicBox.Image.Height, height);
        }
    }
}
