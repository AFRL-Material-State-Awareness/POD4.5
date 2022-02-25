using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POD.Controls
{
    public class PODToolTip : System.Windows.Forms.ToolTip
    {

        public PODToolTip()
            : base()
        {
            //Popup += ToolTip_Show;
        }

        public PODToolTip(System.ComponentModel.IContainer components)
            : base(components)
        {
            //Popup += ToolTip_Show;
        }

        //public new void SetToolTip(System.Windows.Forms.Control ctl, string caption)
        //{
        //    ctl.MouseEnter -= new System.EventHandler(toolTip_MouseEnter);
        //    base.SetToolTip(ctl, caption);
        //    if (caption != string.Empty)
        //        ctl.MouseEnter += new System.EventHandler(toolTip_MouseEnter);
        //}

        //private void toolTip_MouseEnter(object sender, EventArgs e)
        //{
        //    this.Active = false;
        //    this.Active = true;
        //    ShowAlways = true;
        //    //var caption = GetToolTip(sender as Control);

        //    //Show(caption, sender as Control);
        //    //var ctrl = sender as Control;
        //    //Hide(ctrl);

        //    //string tip = GetToolTip(ctrl);

        //    //Show(tip, ctrl, 0, 0, 5000);

        //        //Show(
        //        //    GetToolTip(ctrl), ctrl, ctrl.Width / 2, ctrl.Height / 2
        //        //);

        //}

        //private void ToolTip_Show(object sender, PopupEventArgs e)
        //{
        //    string tip = GetToolTip(e.AssociatedControl);

        //    if (tip.Length == 0)
        //        e.Cancel = true;

        //    ShowAlways = true;
        //}
    }
}
