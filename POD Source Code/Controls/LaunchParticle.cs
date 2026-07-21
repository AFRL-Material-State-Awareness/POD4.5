// <copyright file="LaunchParticle.cs" company="The University of Dayton">
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
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POD.Controls
{
    public class LaunchParticle : Particle
    {
        public PointF Distance { get; set; }
        public float ChangeX { get; private set; }
        public float ChangeY { get; private set; }

        public LaunchParticle(PointF startLocation, PointF endLocation, Color color, double speed, double angle, int fade, int weight, int size, double fireworkSpeed)
            : base(startLocation, endLocation, color, speed, angle, fade, weight, size, fireworkSpeed)
        {          
            this.Distance = new PointF(this.EndLocation.X - this.StartLocation.X, this.EndLocation.Y - this.StartLocation.Y);

            //this.FireworkSpeed = 100;

            this.ChangeX = Distance.X / FireworkSpeed;
            this.ChangeY = Distance.Y / FireworkSpeed;
        }

        public override bool Update()
        {
            if (frame < FireworkSpeed)
            {
                this.Location = new PointF(this.Location.X + this.ChangeX, this.Location.Y + ChangeY);
                this.Exploded = false;
            }
            else if (!Exploded)
            {
                this.Location = this.EndLocation;
                this.Exploded = true;
                this.Color = Color.FromArgb(0, Color.Black);
            }

            this.frame++;

            return this.Exploded;
        }
    }
}
