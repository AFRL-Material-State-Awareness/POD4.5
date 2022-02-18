using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Resources;

namespace POD.Controls
{
    public partial class SimpleActionBar : UserControl
    {
        public PODToolTip ToolTip { get; set; }

        public SimpleActionBar()
        {
            InitializeComponent();

            int stdWidth = Globals.StdWidth(this);
            int stdHeight = Globals.StdWidth(this);

            Width = stdWidth;
            Height = stdHeight;
            MinimumSize = new Size(stdWidth, stdHeight);

            flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;

            Margin = new Padding(0, 0, 0, 0);
            Padding = new Padding(0, 0, 0, 0);

            ActionIcons = new ActionIconsList().List;
        }

        public void RemovePadding()
        {
            Margin = new Padding(0, 0, 0, 0);
            Padding = new Padding(0, 0, 0, 0);
        }

        public PODButton AddButton(string myLabel, EventHandler myClickHandler, string myToolTip)
        {
            PODButton button = new PODButton(myLabel);

            button.Name = myLabel.Replace("&", "");

            button.Click += myClickHandler;

            flowLayoutPanel1.Controls.Add(button);

            ToolTip.SetToolTip(button, myToolTip);

            AddIconToButton(button);

            return button;
        }

        public PODButton AddButton(string myLabel, EventHandler myClickHandler)
        {
            return AddButton(myLabel, myClickHandler, "");
        }

        private void AddIconToButton(PODButton button)
        {
            if (button != null)
            {
                var key = button.Name + ".png";

                if (ActionIcons.Images.ContainsKey(key) && button.Image == null)
                {
                    button.Image = ActionIcons.Images[key];

                    button.ImageAlign = ContentAlignment.MiddleCenter;
                    button.TextAlign = ContentAlignment.BottomCenter;
                    button.TextImageRelation = TextImageRelation.ImageAboveText;

                }
                else
                {
                    button.Image = ActionIcons.Images[0];

                    button.ImageAlign = ContentAlignment.MiddleCenter;
                    button.TextAlign = ContentAlignment.BottomCenter;
                    button.TextImageRelation = TextImageRelation.ImageAboveText;
                }
            }
        }

        public Label AddLabel(string myText)
        {
            var myLabel = new Label();

            myLabel.Text = myText;

            myLabel.Font = new Font(myLabel.Font, FontStyle.Underline);
            myLabel.Dock = DockStyle.Fill;
            myLabel.AutoSize = true;
            myLabel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            myLabel.MaximumSize = new System.Drawing.Size(Globals.StdWidth(this), 0);
            myLabel.TextAlign = ContentAlignment.MiddleCenter;

            flowLayoutPanel1.Controls.Add(myLabel);

            return myLabel;
        }

        
    }
}
