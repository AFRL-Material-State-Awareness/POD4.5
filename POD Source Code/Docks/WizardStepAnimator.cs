// <copyright file="WizardStepAnimator.cs" company="The University of Dayton">
// Copyright © 2025 The University of Dayton. All rights reserved.
//
// Licensed under the GNU General Public License, Version 3.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.gnu.org/licenses/gpl-3.0.txt
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

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
