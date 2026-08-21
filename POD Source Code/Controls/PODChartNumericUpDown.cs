// <copyright file="PODChartNumericUpDown.cs" company="The University of Dayton">
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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POD.Controls
{
    public partial class PODChartNumericUpDown : PODImageNumericUpDown
    {
        ChartPartType _type = ChartPartType.Undefined;
        public static Control LastActiveNumeric = null;

        public PODChartNumericUpDown()
        {
            InitializeComponent();

            this.NumericUpDown.MouseDown += PODChartNumericUpDown_MouseDown;
        }

        void PODChartNumericUpDown_MouseDown(object sender, MouseEventArgs e)
        {
            var numeric = sender as PODNumericUpDown;

            if (numeric != null && numeric != LastActiveNumeric)
            {
                numeric.Select(0, numeric.Text.Length);
                LastActiveNumeric = numeric;
            }
        }

        protected override void UpdateImage()
        {
            Image image = null;
            var ratings = new List<ChartPartType>(new[] { ChartPartType.LinearFit, ChartPartType.POD, ChartPartType.CensorLeft, ChartPartType.CensorRight, 
                                                          ChartPartType.CrackMin, ChartPartType.CrackMax, ChartPartType.A50, ChartPartType.A90, ChartPartType.A9095,
                                                          ChartPartType.Decision});
            var index = ratings.IndexOf(_type);

            if (index >= 0)
                image = RatingImages.Images[index];

            ImageBox.Image = ColorManager.TransformImage(image);

            FixBackgrounColor();
        }

        public ChartPartType PartType
        {
            get
            {
                return _type;
            }

            set
            {
                _type = value;

                UpdateImage();
            }
        }

        
    }
}
