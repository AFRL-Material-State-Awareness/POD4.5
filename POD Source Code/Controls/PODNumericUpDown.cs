using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POD.Controls
{
    //http://stackoverflow.com/questions/2230454/c-sharp-set-usercontrol-value-without-invoking-the-valuechanged-event

    public partial class PODNumericUpDown : NumericUpDown
    {
        //private bool bypassEvent = false;
        EventHandler eventHandler = null;

        public new event EventHandler ValueChanged
        {
            add
            {
                eventHandler += value;
                base.ValueChanged += value;
            }

            remove
            {
                eventHandler -= value;
                base.ValueChanged -= value;
            }
        }

        public new decimal Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                base.ValueChanged -= eventHandler;
                base.Value = value;
                base.ValueChanged += eventHandler;
            }
        }

        //protected override void OnKeyDown(KeyEventArgs e)
        //{
            //base.OnKeyDown(e);
        //}

        //protected override void OnTextBoxTextChanged(object source, EventArgs e)
        //{
            //MessageBox.Show(source.ToString());
        //}

       protected override void UpdateEditText()
       {
            //if (Value < .001m)
            //    this.Text = Value.ToString("E3");
            //else
            //    this.Text = Value.ToString("F3");
            // 

            //this.Text = Value.ToString("0.#########################");
           if (Value < .00001m)
               this.Text = Value.ToString("F6");
           else if (Value < .0001m)
               this.Text = Value.ToString("F5");
           else if (Value < .001m)
               this.Text = Value.ToString("F4");             
           else            
                base.UpdateEditText();
        }



        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if(ReadOnly)
            {
                var control = Controls[0];
                //control must be invisible or paint will not work
                control.Visible = false;

                var color = BackColor;
                var pen = new SolidBrush(color);


                var graphics = e.Graphics;

                graphics.FillRectangle(pen, new Rectangle(control.Location, control.Size));
            }

        }

        public void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
            }
        }

        private void PaintOverButtons(object sender, PaintEventArgs e)
        {
            //base.OnPaint(e);

            
        }

        public override void DownButton()
        {
            if (ReadOnly)
                return;
            base.DownButton();
        }

        public override void UpButton()
        {
            if (ReadOnly)
                return;
            base.UpButton();
        }

        
    }
}
