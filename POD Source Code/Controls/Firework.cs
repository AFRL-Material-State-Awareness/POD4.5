// <copyright file="Firework.cs" company="The University of Dayton">
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
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace POD.Controls
{
    public enum FireworkShape
    {
        Peony,
        Chrysanthemum,
        Dahlia,
        Willow,
        Palm,
        TwinklingWillow
    }

    public class Firework
    {
        private Random random = new Random();
        private List<Particle> particles = new List<Particle>();
        private Control control = null;
        private Form form = null;
        private static int count = 0;
        private int fireworkColorR;
        private int fireworkColorG;
        private int fireworkColorB;

        public static bool Started => Firework.count == 1;
        public static bool Finished => Firework.count == 0;

        public Particle ShootParticle { get; private set; }
        public static bool HideAll { get; set; } = false;

        public Firework()
        {

        }

        private double GetRandomNumber(double minValue, double maxValue)
        {
            // Generate a random number between 0 and 1
            double randomValue = random.NextDouble();

            // Scale the number to the desired range
            return minValue + (randomValue * (maxValue - minValue));
        }

        public void CreateFirework(Point startLocation, Point endlocation, Form form, Control control)
        {
            this.control = control;
            this.form = form;
            Firework.count++;

            control.Paint -= Form_Paint;
            control.Paint += Form_Paint;

            // Change the minimum multiplier from 0.1 to 0.75 so you never get a tiny dud
            var speedMultiplier = GetRandomNumber(0.75, 1.5);

            this.GetFireworkColor();

            var fireworkSpeed = random.Next(20, 150);

            // Boost the base count from 50 to 100 for a much fuller visual
            int count = Convert.ToInt32(100 * speedMultiplier);

            // Pick a random shape for this specific firework
            Array shapes = Enum.GetValues(typeof(FireworkShape));
            FireworkShape shape = (FireworkShape)shapes.GetValue(random.Next(shapes.Length));

            // Adjust counts based on specific shape characteristics
            if (shape == FireworkShape.Dahlia) count /= 2;
            if (shape == FireworkShape.Willow) count = (int)(count * 1.5);
            if (shape == FireworkShape.TwinklingWillow) count = (int)(count * 1.25); // Dense, falling cascade

            // Pre-calculate branches for the Palm shape
            List<double> palmAngles = new List<double>();
            if (shape == FireworkShape.Palm)
            {
                int fronds = random.Next(5, 9);
                for (int i = 0; i < fronds; i++)
                {
                    palmAngles.Add(GetAngle());
                }
            }

            // Generate particles mapped to the specific shape physics
            for (int i = 0; i < count; i++)
            {
                Color color = GetColor();
                double speed = 0;
                double angle = 0;

                // Default particle arguments: Fade, Weight, Size
                // FAST FADE: Subtracting 5 to 20 alpha per frame ensures normal fireworks 
                // burn out quickly before they start falling.
                int pArg1 = random.Next(5, 20);
                int pArg2 = random.Next(0, 60);
                int pArg3 = random.Next(2, 6);

                switch (shape)
                {
                    case FireworkShape.Peony:
                        angle = GetAngle();
                        speed = GetSpeed();
                        break;

                    case FireworkShape.Chrysanthemum:
                        angle = GetAngle();
                        speed = 7.0 + (random.NextDouble() * 1.0);
                        break;

                    case FireworkShape.Dahlia:
                        angle = GetAngle();
                        speed = 9.0 + (random.NextDouble() * 5.0);
                        break;

                    case FireworkShape.Willow:
                        angle = GetAngle();
                        speed = 1.5 + (random.NextDouble() * 2.5);
                        break;

                    case FireworkShape.TwinklingWillow:
                        angle = GetAngle();
                        speed = 4.0 + (random.NextDouble() * 4.0);

                        // FADE: Lowered to 2-5. Extends the minimum lifespan so you get a proper, sustained twinkle.
                        pArg1 = random.Next(2, 5);

                        // WEIGHT: 4-10. Keeps the fall gentle and floaty so they don't hit the bottom of the screen.
                        pArg2 = random.Next(4, 11);
                        break;

                    case FireworkShape.Palm:
                        double baseAngle = palmAngles[random.Next(palmAngles.Count)];
                        angle = baseAngle + (random.NextDouble() * 0.1 - 0.05);
                        speed = 2.0 + (random.NextDouble() * 8.0);
                        break;
                }

                var newParticle = new Particle(startLocation, endlocation, color, speed * speedMultiplier, angle, pArg1, pArg2, pArg3, fireworkSpeed);

                // Turn on the strobe effect specifically for the Twinkling Willow shape
                if (shape == FireworkShape.TwinklingWillow)
                {
                    newParticle.IsTwinkling = true;
                }

                particles.Add(newParticle);
            }

            this.ShootParticle = new LaunchParticle(startLocation, endlocation, GetColor(false), 1, GetAngle(), random.Next(5, 20), random.Next(0, 60), 3, fireworkSpeed);

            // Start the animation
            Task.Run(() => AnimateFirework());
        }

        private void GetFireworkColor()
        {
            this.fireworkColorR = random.Next(64, 256);
            this.fireworkColorG = random.Next(64, 256);
            this.fireworkColorB = random.Next(64, 256);

            var list = new List<int> { this.fireworkColorR, this.fireworkColorG, this.fireworkColorB };

            var min = list.Min();
            var index = list.IndexOf(min);

            list[index] = 0;

            var max = list.Max();
            index = list.IndexOf(max);

            list[index] = 255;

            this.fireworkColorR = list[0];
            this.fireworkColorG = list[1];
            this.fireworkColorB = list[2];
        }

        private double GetAngle()
        {
            return random.NextDouble() * 2 * Math.PI;
        }

        private double GetSpeed()
        {
            return random.NextDouble() * 8 + 2;
        }

        private int GetOffsetColor(int baseColor)
        {
            var offset = random.Next(-64, 128) + baseColor;

            if (offset > 255)
            {
                offset = 255;
            }

            if (offset < 0)
            {
                offset = 0;
            }

            return offset;
        }

        private Color GetColor(bool canBeWhite = true)
        {
            if (canBeWhite && random.NextDouble() > .75)
            {
                return Color.White;
            }

            var r = this.GetOffsetColor(this.fireworkColorR);
            var g = this.GetOffsetColor(this.fireworkColorG);
            var b = this.GetOffsetColor(this.fireworkColorB);

            return Color.FromArgb(r, g, b);
        }

        private void Form_Paint(object sender, PaintEventArgs e)
        {
            var particle_copy = particles.ToList();

            if (!this.ShootParticle.Exploded)
            {
                this.ShootParticle.Draw(e.Graphics);
            }
            else
            {
                foreach (var particle in particle_copy)
                {
                    particle.Draw(e.Graphics);
                }
            }
        }

        private void AnimateFirework()
        {
            bool exploded = false;

            // Animate the firework particles
            while (particles.Count > 0)
            {
                bool hide = !control.Visible || Firework.HideAll;

                if (!exploded)
                {
                    this.ShootParticle.Hide = hide;
                    exploded = this.ShootParticle.Update();

                    if (exploded)
                    {
                        // Update particle positions
                        for (int i = particles.Count - 1; i >= 0; i--)
                        {
                            particles[i].Exploded = exploded;
                            particles[i].Location = this.ShootParticle.EndLocation;
                        }

                        this.ShootParticle.Hide = true;
                    }
                }
                else
                {
                    // Update particle positions
                    for (int i = particles.Count - 1; i >= 0; i--)
                    {
                        particles[i].Hide = hide;
                        particles[i].Update();
                        if (particles[i].IsOutOfBounds(control.ClientRectangle))
                        {
                            particles.RemoveAt(i);
                        }
                    }
                }

                Thread.Sleep(4);
            }

            Firework.count--;
        }
    }
}