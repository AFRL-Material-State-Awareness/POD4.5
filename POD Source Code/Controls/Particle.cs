// <copyright file="Particle.cs" company="The University of Dayton">
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
using System.Diagnostics;
using System.Drawing;

namespace POD.Controls
{
    public class Particle
    {
        public PointF EndLocation { get; set; }
        public PointF StartLocation { get; set; }
        public PointF Location { get; set; }

        public Color Color { get; set; }
        public double Speed { get; set; }
        public float FireworkSpeed { get; set; }
        public double Angle { get; set; }
        public int Fade { get; set; }
        public int Weight { get; set; }
        public int Size { get; set; }
        public bool Exploded { get; set; }
        public bool Hide { get; set; } = false;

        // Added property to flag if this particle should strobe
        public bool IsTwinkling { get; set; } = false;

        protected int frame = 0;
        protected int middle = 0;

        // Added a static random instance for the twinkle probability
        private static Random rand = new Random();

        public Particle(PointF startLocation, PointF endLocation, Color color, double speed, double angle, int fade, int weight, int size, double fireworkSpeed)
        {
            StartLocation = startLocation;
            EndLocation = endLocation;
            Location = StartLocation;
            Color = color;
            Speed = speed;
            FireworkSpeed = Convert.ToSingle(fireworkSpeed);
            Angle = angle;
            Fade = fade;
            Weight = weight;
            Size = size;
            middle = Convert.ToInt32(Size / 2);

            this.Exploded = false;
        }

        public void Draw(Graphics g)
        {

            if (this.Exploded)
            {
                int newA = Color.A - this.Fade;

                if (newA < 0)
                {
                    newA = 0;
                }

                if (newA > 255)
                {
                    newA = 255;
                }

                this.Color = Color.FromArgb(newA, Color.R, Color.G, Color.B);
            }

            // TWINKLE LOGIC: Only twinkle AFTER the initial burst when gravity starts pulling it down
            if (this.Exploded && this.IsTwinkling && this.frame > this.FireworkSpeed && rand.Next(0, 2) == 0)
            {
                return; // Skips the fill ellipse draw below, causing a flicker
            }

            // Draw the particle as a small filled circle
            using (var brush = new SolidBrush(Color))
            {
                g.FillEllipse(brush, Location.X - middle, Location.Y - middle, Size, Size);
            }
        }

        public virtual bool Update()
        {
            if (this.Exploded)
            {
                // AIR RESISTANCE: Slows the outward burst down gracefully over time
                this.Speed *= 0.96;

                double gravFrame = this.frame - this.FireworkSpeed;

                if (gravFrame < 0)
                {
                    gravFrame = 0;
                }

                var speed = Convert.ToDouble(gravFrame / 60.0);

                // Update the particle's location based on its speed and angle
                Location = new PointF(
                    Location.X + (float)(Speed * Math.Cos(Angle)),
                    Location.Y + (float)(Speed * Math.Sin(Angle)) + Convert.ToSingle(Weight * Math.Sin(speed)));

                frame++;
            }

            return this.Exploded;
        }

        public bool IsOutOfBounds(Rectangle bounds)
        {
            // Check if the particle is outside the form's bounds
            return Location.X < 0 || Location.X > bounds.Width || Location.Y < 0 || Location.Y > bounds.Height || Color.A == 0 || Hide;
        }
    }
}