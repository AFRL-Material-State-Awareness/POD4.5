// <copyright file="PODPanel.cs" company="The University of Dayton">
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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POD.Controls
{
    public class PODPanel : Panel
    {
        private Point launchPoint = new Point(0, 0);
        private Random random = new Random();
        public string Message = "Please while wait while analysis is performed...";

        public Color FadeColor = Color.Black;

        public int Fade
        {
            get
            {
                return fade;
            }
            set
            {
                fade = value;

                fade = fade > 255 ? 255 : fade;
                fade = fade < 0 ? 0 : fade;
            }
        }

        public PODPanel()
        {
            this.DoubleBuffered = true;

            timer.Elapsed += (sender, e) => InvalidateVisible();
            timer.AutoReset = true; // Ensures the timer repeats
            
        }

        private void InvalidateVisible()
        {
            if (_backgroundControl != null)
            {
                this.Invalidate();
            }
        }

        public void GenerateLaunch()
        {
            var whichSide = Math.Floor(random.NextDouble() * 4.0);
            var skew = random.NextDouble();
            var halfHeight = this.Height * .5;
            var x = 0;
            var y = 0;

            switch(whichSide)
            {
                case 0: //bottom
                case 1:
                    x = Convert.ToInt32(this.Width * skew);
                    y = Height;
                    break;
                case 2: //left
                    x = 0;
                    y = Convert.ToInt32(halfHeight + skew * .5); ;
                    break;
                case 3: //right
                    x = this.Width;
                    y = Convert.ToInt32(halfHeight + skew * .5); ;
                    break;
            }


            launchPoint = new Point(x, y);
        }

        private Control _backgroundControl;
        private int fade = 225;

        public Control BackgroundControl
        {
            get { return _backgroundControl; }
            set
            {
                _backgroundControl = value;
                Invalidate(); // Redraw the panel when the control changes
            }
        }

        private System.Timers.Timer timer = new System.Timers.Timer(1000);

        public Point Launch => launchPoint;

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Let the base class handle the default background painting
            base.OnPaintBackground(e);

            if (_backgroundControl != null)
            {
                // Draw the control to a bitmap
                using (Bitmap bmp = new Bitmap(_backgroundControl.Width, _backgroundControl.Height))
                {
                    _backgroundControl.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));

                    // Draw the bitmap onto the panel's background
                    e.Graphics.DrawImage(bmp, 0, 0);

                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(this.Fade, this.FadeColor)), this.ClientRectangle);

                    ElapsedTime = Convert.ToInt32((DateTime.Now - this.StartTime).TotalSeconds);

                    var messageSize = e.Graphics.MeasureString(this.Message, new Font(Font.FontFamily, 15.0f));
                    var elapsedSize = e.Graphics.MeasureString(FormatElapsedTime(ElapsedTime), new Font(Font.FontFamily, 15.0f));

                    e.Graphics.DrawString(this.Message, new Font(Font.FontFamily, 15.0f), Brushes.White, new PointF(this.ClientRectangle.Width / 2.0f - messageSize.Width / 2.0f, this.ClientRectangle.Height / 2.0f + messageSize.Height / 2.0f));
                    e.Graphics.DrawString(FormatElapsedTime(ElapsedTime), new Font(Font.FontFamily, 15.0f), Brushes.White, new PointF(this.ClientRectangle.Width / 2.0f - elapsedSize.Width / 2.0f, this.ClientRectangle.Height / 2.0f + elapsedSize.Height * 1.5f));
                }
            }
        }

        public void SetMouse(bool turnOn)
        {
            this.MouseClick -= MainChart_MouseClick;

            if (turnOn)
            {
                this.MouseClick += MainChart_MouseClick;
            }
        }

        private void MainChart_MouseClick(object sender, MouseEventArgs e)
        {
            // Create firework particles at the clicked location
            Firework firework = new Firework();

            firework.CreateFirework(this.Launch, e.Location, this.FindForm(), this);

            if (Firework.Started)
            {
                Task.Run(() => Animate(firework));
            }
        }

        private void Animate(Firework firework)
        {
            while (!Firework.Finished)
            {
                this.Invalidate();
                Thread.Sleep(16);
            }
        }

        


        public void ShowDisplayPanel()
        {
            Firework.HideAll = false;
            this.SetMouse(true);
            this.Visible = true;
            this.BringToFront();
            this.GenerateLaunch();
            this.StartTime = DateTime.Now;
            timer.Enabled = true; // Start the timer
            timer.Start();
        }

        public void HideDisplayPanel()
        {
            Firework.HideAll = true;
            this.Visible = false;
            this.SendToBack();
            timer.Stop();
            timer.Enabled = false;
        }

        public int ElapsedTime { get; private set; }
        public DateTime StartTime { get; private set; }

        public static string FormatElapsedTime(int totalSeconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(totalSeconds);

            return string.Format("Elapsed Time: {0:D2}h:{1:D2}m:{2:D2}s",
                                 time.Hours,
                                 time.Minutes,
                                 time.Seconds);
        }
    }
}
