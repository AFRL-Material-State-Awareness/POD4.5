using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POD.Controls
{
    public class PODFlowLayoutPanel : FlowLayoutPanel
    {
        //Control _active;

        public PODFlowLayoutPanel()
        {
            ControlAdded += PODFlowLayoutPanel_ControlAdded;
        }

        void PODFlowLayoutPanel_ControlAdded(object sender, ControlEventArgs e)
        {
            e.Control.Click += Control_Click;
        }

        void Control_Click(object sender, EventArgs e)
        {
            Focus();
        }

        

        protected override void OnMouseEnter(EventArgs e)
        {
            //var form = FindForm();

            //_active = form.ActiveControl;

            //Focus();

            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            //if(_active != null)
            //    _active.Focus();

            //base.OnMouseLeave(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            Focus();

            base.OnMouseClick(e);
        }
    }
}
