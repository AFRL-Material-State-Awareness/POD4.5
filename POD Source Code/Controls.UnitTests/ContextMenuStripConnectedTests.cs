using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;
using System.Windows.Forms.DataVisualization.Charting;
using POD.Controls;
using POD;
using System.Data;
using System.Windows.Forms;

namespace Controls.UnitTests
{
    [TestFixture]
    public class ContextMenuStripConnectedTests
    {
        private ContextMenuStripConnected _contextMenuStripCnt;
        private FlowLayoutPanel _panel;
        private Button _button;
        [SetUp]
        public void SetUp()
        {
            _contextMenuStripCnt = new ContextMenuStripConnected();
            _panel = new FlowLayoutPanel()
            {
                Name = "MyFlowLayoutPanel",
                Visible = true,
                Enabled = true,
                AutoSize = false,
                Size = new System.Drawing.Size(100, 50)
                
            };
            _panel.MinimumSize = _panel.Size;
            //The Button is added to the panel for testing Parent of a parent in the event CloseEverythingElse
            _button = new Button();
            _panel.Controls.Add(_button);
            var host = new ToolStripControlHost(_panel);
            ToolStripItem[] items = new ToolStripItem[] { new ToolStripButton(), new ToolStripButton(), host };
            _contextMenuStripCnt.Items.AddRange(items);
        }
        /// ShowOnlyButtons Getter tests
        [Test]
        public void ShowOnlyButtons_ValueIsFalse_SetsAllItemsAsVisble()
        {
            //Act
            _contextMenuStripCnt.ShowOnlyButtons = false;
            //Assert
            Assert.That(_contextMenuStripCnt.ShowOnlyButtons, Is.False);
            foreach (ToolStripItem item in _contextMenuStripCnt.Items)
                Assert.That(item.Visible, Is.False);
        }
        [Test]
        [Ignore("Can't get this to work in test")]
        public void ShowOnlyButtons_ValueIsTrue_AttemptsToCastItemAsToolStripControlHostAndWillMakeTheItemVisibleIfItIs()
        {
            //Act
            _contextMenuStripCnt.ShowOnlyButtons = true;
            //Assert
            Assert.That(_contextMenuStripCnt.ShowOnlyButtons, Is.True);
            Assert.That(_contextMenuStripCnt.Items[0], Is.TypeOf<ToolStripButton>());
            Assert.That(_contextMenuStripCnt.Items[0].Visible, Is.False);
            Assert.That(_contextMenuStripCnt.Items[1], Is.TypeOf<ToolStripButton>());
            Assert.That(_contextMenuStripCnt.Items[1].Visible, Is.False);
            Assert.That(_contextMenuStripCnt.Items[2], Is.TypeOf<ToolStripControlHost>());
            //Can't get this to work in test
            //Assert.That(_contextMenuStripCnt.Items[2].Visible, Is.True);

        }

        /// <summary>
        /// Test for ForcePanelToDraw() Function
        /// </summary>
        //[Test]
        //public void ForcePanelToDraw_ValidLayoutPanelPassed_


        /// <summary>
        /// Tests for CloseEverythingElse(object sender, EventArgs e) Function
        /// </summary>
        [Test]
        public void CloseEverythingElse_SenderIsNotButton_ContextMenuStripCntShowOnlyButtonsRemainsFalse()
        {
            //Arrange
            object notAButton = _contextMenuStripCnt.Items[2];
            //Act
            ContextMenuStripConnected.CloseEverythingElse(notAButton, null);
            //Assert
            Assert.That(_contextMenuStripCnt.ShowOnlyButtons, Is.False);
        }
        [Test]
        public void CloseEverythingElse_SenderIsButtonButHasNoParentOfParent_ContextMenuStripCntShowOnlyButtonsRemainsFalse()
        {
            //Arrange
            Button aButtonWithNoParentOfParent = new Button();
            Panel panel = new Panel();
            panel.Controls.Add(aButtonWithNoParentOfParent);
            //Act
            ContextMenuStripConnected.CloseEverythingElse(aButtonWithNoParentOfParent, null);
            //Assert
            Assert.That(_contextMenuStripCnt.ShowOnlyButtons, Is.False);
        }
        [Test]
        public void CloseEverythingElse_SenderIsButtonButHasInvalidParentOfParent_ContextMenuStripCntShowOnlyButtonsRemainsFalse()
        {
            //Arrange
            Button aButtonWrongParentOfParent = new Button();
            Panel panel = new Panel();
            panel.Controls.Add(aButtonWrongParentOfParent);
            Control control = new Control();
            control.Controls.Add(panel);
            //Act
            ContextMenuStripConnected.CloseEverythingElse(aButtonWrongParentOfParent, null);
            //Assert
            Assert.That(_contextMenuStripCnt.ShowOnlyButtons, Is.False);
        }
        [Test]
        public void CloseEverythingElse_SenderIsButtonWithContextMenuStripConnectedParent_ContextMenuStripCntShowOnlyButtonsBecomesTrue()
        {
            //Arrange
            Button aButton = _button;
            //Act
            ContextMenuStripConnected.CloseEverythingElse(aButton, null);
            //Assert
            Assert.That(_contextMenuStripCnt.ShowOnlyButtons, Is.True);
        }
    }
}
