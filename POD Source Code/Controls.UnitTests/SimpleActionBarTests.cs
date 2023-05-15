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
    public class SimpleActionBarTests
    {
        private SimpleActionBar _simpleActionBar;
        private bool _buttonClicked;
        [SetUp]
        public void Setup()
        {
            _simpleActionBar = new SimpleActionBar();
            _buttonClicked = false;
            // This is used to ensure that the amount of controls is 1 when the SimpleActionBar is initialized
            Assert.That(_simpleActionBar.Controls.Count, Is.EqualTo(1));
        }
        /// <summary>
        /// Tests for the RemovePadding() function
        /// </summary>
        [Test]
        public void RemovePadding_MarginAndPaddingAreSet_AllMarginAndPaddingSetTo0()
        {
            //Arrange
            _simpleActionBar.Margin = new Padding(1, 1, 1, 1);
            _simpleActionBar.Padding = new Padding(1, 1, 1, 1);
            //Act
            _simpleActionBar.RemovePadding();
            //Arrange
            Assert.That(Is.Equals(_simpleActionBar.Margin, new Padding(0, 0, 0, 0)));
            Assert.That(Is.Equals(_simpleActionBar.Padding, new Padding(0, 0, 0, 0)));
        }

        /// Tests for the AddButton(string myLabel, EventHandler myClickHandler, string myToolTip) function
        [Test]
        public void AddButton_ValidLabelHandlerAndToolTipPassed_ReturnsAButtonWithNameLabelExcludingAmpAndToolTip()
        {
            //Arrange
            _simpleActionBar.ToolTip = new PODToolTip();
            string label = "MyButton&";
            EventHandler MyClickEvent = ClickEvent;
            string toolTip = "Button tooltip";
            //Act
            var result = _simpleActionBar.AddButton(label, MyClickEvent, toolTip);
            result.PerformClick();
            //Assert
            Assert.That(result is PODButton);
            Assert.That(result.Name, Is.EqualTo("MyButton"));
            Assert.That(_simpleActionBar.ToolTip.GetToolTip(result), Is.EqualTo(toolTip));
            Assert.IsTrue(_simpleActionBar.Controls[0].Controls.Contains(result));
            Assert.IsTrue(_buttonClicked);
        }
        /// <summary>
        /// Tests for the AddLabel(string myLabel) function
        /// </summary>
        [Test]
        public void AddLabel_ValueStringPassed_ReturnsANewLabelObject()
        {
            //Arrange
            string label = "MyLabel";
            //Act
            var result = _simpleActionBar.AddLabel(label);
            //Assert
            Assert.That(result is Label);
            Assert.That(result.Text, Is.EqualTo(label));
            Assert.IsTrue(_simpleActionBar.Controls[0].Controls.Contains(result));
        }
        public void ClickEvent(object sender, EventArgs args)
        {
            _buttonClicked = true;
        }
    }
}
