using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace POD.Controls
{
    public partial class SideSplitter : SplitContainer
    {
        public float SnapToSize;        

        public SideSplitter()
        {
            InitializeComponent();

            // Enable default double buffering processing (DoubleBuffered returns true)
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            //base.OnPaint(e);
            //ControlPaint.DrawGrabHandle(e.Graphics, SplitterRectangle, false, Enabled);

            //base.OnPaint(e);

            //e.Graphics.FillRectangle(SystemBrushes.ControlDark, 0.0F, 0.0F, SnapToSize, SnapToSize);

            Point centerPoint = new Point(SplitterRectangle.Left - 3 + SplitterRectangle.Width / 2, SplitterRectangle.Top - 1 + SplitterRectangle.Height / 2);

            e.Graphics.FillEllipse(SystemBrushes.ControlText, centerPoint.X, centerPoint.Y, 3, 3);
            if (Orientation == System.Windows.Forms.Orientation.Horizontal)
            {
                e.Graphics.FillEllipse(SystemBrushes.ControlText, centerPoint.X - 10, centerPoint.Y, 3, 3);
                e.Graphics.FillEllipse(SystemBrushes.ControlText, centerPoint.X + 10, centerPoint.Y, 3, 3);
            }
            else
            {
                e.Graphics.FillEllipse(SystemBrushes.ControlText, centerPoint.X, centerPoint.Y - 10, 3, 3);
                e.Graphics.FillEllipse(SystemBrushes.ControlText, centerPoint.X, centerPoint.Y + 10, 3, 3);
            }
        }
    }
}
