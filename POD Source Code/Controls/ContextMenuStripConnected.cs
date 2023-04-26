using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

namespace POD.Controls
{
    public class ContextMenuStripConnected : ContextMenuStrip
    {
        private bool _showOnlyButtons = false;
        public bool ShowOnlyButtons
        {
            get
            {
                return _showOnlyButtons;
            }
            set
            {
                if (!value)
                {
                    foreach (ToolStripItem item in Items)
                    {
                        item.Visible = true;
                    }
                }
                else
                {
                    foreach (ToolStripItem item in Items)
                    {
                        var host = item as ToolStripControlHost;

                        if (host != null)
                            item.Visible = true;
                        else
                            item.Visible = false;
                    }
                }

                _showOnlyButtons = value;
            }
        }
        public bool IsTextboxMenu { get; set; }

        public ContextMenuStripConnected()
        {
            Initialize();

        }

        private void Initialize()
        {
            
        }

        public static void ForcePanelToDraw(FlowLayoutPanel panel)
        {
            //var bitmap = new Bitmap(panel.Bounds.Width, panel.Bounds.Height);
            using(var bitmap = new Bitmap(panel.Bounds.Width, panel.Bounds.Height))
                panel.DrawToBitmap(bitmap, panel.Bounds);
        }

        public static FlowLayoutPanel MakeNewMenuFlowLayoutPanel(string name)
        {
            var actionPanel = new FlowLayoutPanel();
            actionPanel.Name = name;
            actionPanel.FlowDirection = FlowDirection.LeftToRight;
            actionPanel.Padding = new Padding(0, 0, 0, 0);
            actionPanel.Margin = new Padding(0, 0, 0, 0);
            actionPanel.BackColor = Color.Transparent;
            actionPanel.Dock = DockStyle.Fill;
            actionPanel.MaximumSize = new Size(120, 120);
            return actionPanel;
        }

        public static void CloseEverythingElse(object sender, EventArgs e)
        {
            if (sender is Button button)
                if (button.Parent.Parent is ContextMenuStripConnected menu)
                    menu.ShowOnlyButtons = true;
        }

        public ContextMenuStripConnected(System.ComponentModel.IContainer container) : base(container)
        {
            Initialize();

            ShowImageMargin = false;
        }

        public static void AddButtonToMenu(FlowLayoutPanel availablePanel, ButtonHolder item, PODToolTip tooltip)
        {
            var actButton = new Button();
            actButton.TextImageRelation = TextImageRelation.Overlay;

            actButton.Text = "";// button.Button.Text;
            actButton.Name = availablePanel.Name + "_" + item.Name;

            if (item.Image != null)
            {
                actButton.BackgroundImage = item.Image;
                actButton.BackgroundImageLayout = ImageLayout.Center;
                actButton.Width = item.Image.Width + 10;
                actButton.Height = item.Image.Height + 10;
            }

            actButton.Padding = new Padding(0, 0, 0, 0);
            actButton.Margin = new Padding(0, 0, 0, 0);
            actButton.Enabled = true;
            actButton.Click += item.Event;
            actButton.Click += ContextMenuStripConnected.CloseEverythingElse;

            tooltip.SetToolTip(actButton, item.ToolTip);

            availablePanel.Controls.Add(actButton);
        }
    }
}
