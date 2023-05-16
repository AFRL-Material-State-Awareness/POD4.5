using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
using System.IO;


namespace POD.Controls
{
    public partial class BlendPictureBox : PictureBox
    {
        private float _transparency;
        private ColorMatrix _matrix;
        private ImageAttributes _imageAttributes;
        private Brush _whiteBrush;
        private int _vX;


        public float Transparency
        {
            get { return _transparency; }
            set
            { 
                _transparency = value;

                _matrix.Matrix33 = _transparency / 100.0F; //opacity 0 = completely transparent, 1 = completely opaque
                _imageAttributes.SetColorMatrix(_matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                Invalidate();
            }
        }

        public BlendPictureBox()
        {
            InitializeComponent();

            Initialize();
        }

        public BlendPictureBox(Image myNewPicture, Image myOldPicture)
        {
            InitializeComponent();

            BackgroundImage = myOldPicture;
            

            Image = myNewPicture;

            Initialize();
        }

        public static Bitmap RtbToBitmap(IRichTextBoxWrapper rtb)
        {
            if (rtb.Width == 0 || rtb.Height == 0)
                return new Bitmap(50, 50);

            Bitmap bmp = new Bitmap(rtb.Width, rtb.Height);

            if (rtb.IsDisposed == false)
            {
                rtb.Update();  // Ensure RTB fully painted

                using (Graphics gr = Graphics.FromImage(bmp))
                    gr.CopyFromScreen(rtb.PointToScreen(Point.Empty), Point.Empty, rtb.Size);
            }
            return bmp;
        }

        public void CaptureOldStateImage(Control myControl)
        {
            if (!myControl.IsDisposed)
            {
                Bitmap image;

                if (myControl.GetType().Name == "RichTextBox")
                {
                    image = RtbToBitmap(new RichTextBoxWrapper((RichTextBox)myControl));
                }
                else
                {
                    if (myControl.Width <= 0 && myControl.Height <= 0)
                    {
                        image = new Bitmap(50, 50);
                    }
                    else
                    {


                        image = new Bitmap(myControl.Width, myControl.Height);

                        myControl.DrawToBitmap(image, new Rectangle(0, 0, myControl.Width, myControl.Height));
                    }
                }

                BackgroundImage = image;
                Transparency = 0.0F;

                Show();
                BringToFront();
            }
        }

        public void CaptureNewImage(Control myControl)
        {
            Bitmap image;

            if (myControl.Width <= 0 && myControl.Height <= 0)
            {
                image = new Bitmap(50, 50);
            }
            else
            {
                image = new Bitmap(myControl.Width, myControl.Height);

                if (myControl.IsDisposed == false)
                    myControl.DrawToBitmap(image, new Rectangle(0, 0, myControl.Width, myControl.Height));
            }
            
            Image = image;
        }

        protected void Initialize()
        {
            _matrix = new ColorMatrix();   
            _imageAttributes = new ImageAttributes();        

            //start out only showing the background image
            Transparency = 0.0F;

            BackgroundImageLayout = ImageLayout.None;

            _whiteBrush = new SolidBrush(Color.FromArgb(255, 255, 255));

            PossibleLines = new List<int>();
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            
            int alpha = Convert.ToInt32(255 * (_transparency / 100.0F));

            if(Width != 0 && Height != 0)
            {
                if (BackgroundImage != null)
                    e.Graphics.DrawImage(BackgroundImage, new Rectangle(0, 0, Width, Height));
            
                Brush whiteBrush2 = new SolidBrush(Color.FromArgb(alpha, 255, 255, 255));
                e.Graphics.FillRectangle(whiteBrush2, new Rectangle(0, 0, Width, Height));

                //fading in new control
                if (Image != null)
                {
                    e.Graphics.DrawImage(Image, new Rectangle(0, 0, Width, Height),
                                         0, 0, Width, Height, GraphicsUnit.Pixel, _imageAttributes);
                }

                if (DrawVerticalLine == true)
                {
                    Font boldFont = new System.Drawing.Font(Font.FontFamily, Font.Size * 2.0F, FontStyle.Bold);
                    //StringFormat drawFormat = new StringFormat();
                    // Create a StringFormat object with the each line of text, and the block 
                    // of text centered on the page.
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Center;
                    stringFormat.LineAlignment = StringAlignment.Center;

                    int value = 0;

                    for (int j = 0; j < PossibleLines.Count; j++)
                    {
                        

                        int height = _vX - 6;
                        int finalHeight = height + value + 2;

                        if (finalHeight > Height - 16)
                            height = Height - value - 8;

                        int width = _vX - 6;

                        Rectangle rect = new Rectangle(2, value + 2, width+1, height);
                        e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, 144, 144, 192)), rect);
                        e.Graphics.DrawRectangle(new Pen(Color.Black), rect);

                        e.Graphics.DrawString((j + 1).ToString(), boldFont, new SolidBrush(Color.White), rect, stringFormat);

                        value += _vX;

                        //if (_vX == PossibleLines[j])
                        //    break;
                    }

                    if (PossibleLines != null && PossibleLines.Count > 0)
                    {
                        int maxSize = PossibleLines.Max();

                        for (int i = 0; i < PossibleLines.Count; i++)
                        {
                            value = PossibleLines[i];

                            if (value == _vX)
                            {
                                e.Graphics.DrawLine(new Pen(Color.Black, 1.0F), _vX + 4, 0.0F, _vX + 4, Height);

                                int width = Width - maxSize;
                                int height = Height;

                                e.Graphics.DrawString(GetLineLabel(i), boldFont, new SolidBrush(Color.Black), new Rectangle(maxSize, 0, width, height), stringFormat);
                            }

                            e.Graphics.FillRectangle(new SolidBrush(Color.Black), value - 5, 0, 12, 10);
                            //e.Graphics.FillEllipse(new SolidBrush(Color.Black), value - 5, 0, 11, 11);
                            e.Graphics.FillRectangle(new SolidBrush(Color.Black), value - 5, Height - 10, 12, 10);
                            //e.Graphics.FillEllipse(new SolidBrush(Color.Black), value - 5, Height - 12, 11, 11);
                        }
                    }

                }
            }
        }


        public bool DrawVerticalLine { get; set; }

        public int VerticalLineX
        {
            get
            {
                return _vX;
            }
            set
            {
                _vX = value;
                Invalidate();
            }
        }

        public List<int> PossibleLines { get; set; }

        public string GetLineLabel(int myIndex)
        {

            if (myIndex == PossibleLines.Count - 1)
                return "Show 1 Chart\non the Left Side";
            else
                return "Show " + (PossibleLines.Count - myIndex).ToString() + " Charts\non the Left Side";
        }

        int _mouseX;

        public int MouseX
        {
            get
            {
                return _mouseX;
            }

            set
            {
                _mouseX = value;
            }
        }
    }
}
