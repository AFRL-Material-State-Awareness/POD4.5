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
    public partial class ColorMap : UserControl
    {
        NumericUpDown heightTest = new NumericUpDown();
        Font myFont = new Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Point);

        public ColorMap()
        {
            InitializeComponent();

            Font myFont = heightTest.Font;// new Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Point);
        }

        private void Paint_ColorMap(object sender, PaintEventArgs e)
        {
            
            
            var format = new StringFormat();
            format.Alignment = StringAlignment.Near;
            format.LineAlignment = StringAlignment.Center;

            e.Graphics.DrawString("PASS", myFont, Brushes.White, 0.0F, Height / 2.0F + 1.0F, format);

           var format2 = new StringFormat();
            format2.Alignment = StringAlignment.Far;
            format2.LineAlignment = StringAlignment.Center;

            e.Graphics.DrawString("FAIL", myFont, Brushes.White, Width, Height / 2.0F + 1.0F, format2);
            
        }

        private void Resize_ColorMap(object sender, EventArgs e)
        {
            Height = heightTest.Height;
            
            //if(Parent != null)
            //    Width = Parent.Width - ((Padding.Left + Padding.Right + Margin.Left + Margin.Right) +
            //            (Parent.Padding.Left + Parent.Padding.Right + Parent.Margin.Left + Parent.Margin.Right));
        }
    }
}
