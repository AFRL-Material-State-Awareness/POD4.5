using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;


namespace POD.Controls
{
    public class PODButton : Button
    {
        PODToolTip _tip = null;

        public PODToolTip PODToolTip
        {
            get { return _tip; }
            set
            {
                _tip = value;

                if(_tip != null && TipForControl != null)
                    _tip.SetToolTip(this, TipForControl);
            }
        }
        string _tooltip = "";

        public string TipForControl
        {
            get { return _tooltip; }
            set { _tooltip = value; }
        }

        public PODButton()
        {

            //UseMnemonic = true;
            Height = StdHeight;
            Width = StdWidth;

            Padding = new Padding(0);
            Margin = new Padding(0);
        }

        /*protected override void OnMouseEnter(EventArgs e)
        {
            if (_tip != null)
                _tip.Show(_tooltip, this);

            base.OnMouseEnter(e);
        }*/

        public PODButton(string myLabel)
        {
            //UseMnemonic = true;

            Height = StdHeight;
            Width = StdWidth;

            Text = myLabel;

            Name = myLabel.Replace("&", "");

            Padding = new Padding(0);
            Margin = new Padding(0);
        }

        public PODButton(string myLabel, string tooltip, PODToolTip myTip)
        {
            //UseMnemonic = true;

            _tip = myTip;
            _tooltip = tooltip;

            myTip.ShowAlways = true;

            Height = StdHeight;
            Width = StdWidth;

            Text = myLabel;

            Name = myLabel.Replace("&", "");

            myTip.SetToolTip(this, _tooltip);

            Padding = new Padding(0);
            Margin = new Padding(0);
        }

        public PODButton(PODToolTip tip, string tooltip)
        {
            _tip = tip;
            _tooltip = tooltip;

            _tip.SetToolTip(this, _tooltip);
        }

        public int StdWidth
        {
            get
            {
                return Globals.StdWidth(this);
            }
        }

        public int StdHeight
        {
            get 
            {
                return Globals.StdHeight(this);
            }
        }

        public virtual void UpdateLabel()
        {
            
        }
    }

    public class ButtonHolder
    {
        public string Name { get; set; }
        public Image Image { get; set; }
        public EventHandler Event { get; set; }
        public string ToolTip { get; set; }

        public ButtonHolder(string name, Image image, EventHandler handler, string toolTip = "")
        {
            Name = name;
            Image = image;
            Event = handler;
            ToolTip = toolTip;
        }
    }
}
