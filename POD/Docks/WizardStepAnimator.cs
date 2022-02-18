using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POD.Wizards;
using Transitions;
using System.Drawing;
using System.Windows.Forms;

namespace POD.Docks
{
    public delegate void DrawBitmapFunction(Bitmap image, Rectangle rectangle);

    public class WizardStepAnimator
    {
        PictureBox MoveBox;
        PictureBox NextBox;
        Bitmap MoveBitmap;
        Bitmap NextBitmap;
        int ControlHeight;
        int ControlWidth;
        int ControlEndLeft;
        int ControlEndTop;
        public Transition T;
        SwapStepsEventArgs Args;

        public WizardStepAnimator(Transition myT, SwapStepsEventArgs myArgs)
        {
            T = myT;
            Args = myArgs;
        }

        internal void SetupSizes(PictureBox myMoveBox, PictureBox myNextBox, int myWidth, int myHeight)
        {
            ControlWidth = myWidth;
            ControlHeight = myHeight;

            MoveBitmap = new Bitmap(ControlWidth, ControlHeight);
            NextBitmap = new Bitmap(ControlWidth, ControlHeight);

            MoveBox = myMoveBox;
            NextBox = myNextBox;

            
        }

        public void SetupMoveLocations(DrawBitmapFunction myDrawMove, DrawBitmapFunction myDrawNext,
                                       int myStartTop, int myStartLeft, int myEndTop, int myEndLeft)
        {
            if (Args.Direction == 0)
            {
                myDrawMove(MoveBitmap, new Rectangle(0, 0, ControlWidth, ControlHeight));
                MoveBox.Image = MoveBitmap;
                MoveBox.Height = ControlHeight;
                MoveBox.Width = ControlWidth;
                MoveBox.Top = myStartTop;
                MoveBox.Left = myStartLeft;

                ControlEndTop = myEndTop;
                ControlEndLeft = myEndLeft;
            }
            else
            {
                myDrawNext(MoveBitmap, new Rectangle(0, 0, ControlWidth, ControlHeight));
                MoveBox.Image = MoveBitmap;
                MoveBox.Height = ControlHeight;
                MoveBox.Width = ControlWidth;
                MoveBox.Top = myEndTop;
                MoveBox.Left = myEndLeft;

                ControlEndTop = myStartTop;
                ControlEndLeft = myStartLeft;
                
            }

            MoveBox.Show();
            MoveBox.BringToFront();
        }

        public void SetupNextLocations(DrawBitmapFunction myDrawNext, int myNextTop, int myNextLeft, int myDirection)
        {
            if (myDirection == 1)
            {
                myDrawNext(NextBitmap, new Rectangle(0, 0, ControlWidth, ControlHeight));
                NextBox.Image = NextBitmap;

                NextBox.Height = ControlHeight;
                NextBox.Width = ControlWidth;
                NextBox.Top = myNextTop;
                NextBox.Left = myNextLeft;

                NextBox.BringToFront();
                MoveBox.BringToFront();
                NextBox.Show();
            }
            else
            {
                NextBox.Hide();
            }
        }

        internal void AddAnimations()
        {
            T.add(MoveBox, "Left", ControlEndLeft);
            T.add(MoveBox, "Top", ControlEndTop);
        }

        internal void ChangeEndLocation(int myEndTop, int myEndLeft)
        {
            ControlEndTop = myEndTop;
            ControlEndLeft = myEndLeft;
        }
    }
}
