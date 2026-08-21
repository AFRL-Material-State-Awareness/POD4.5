// <copyright file="PODRatedNumericUpDown.cs" company="The University of Dayton">
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
    public partial class PODRatedNumericUpDown : PODImageNumericUpDown
    {
        TestRating _rating = TestRating.Undefined;
        private bool invert;

        public PODRatedNumericUpDown()
        {
            InitializeComponent();

            UpdateImage();

            this.invert = false;
        }

        public PODRatedNumericUpDown(bool invert = false)
        {
            InitializeComponent();
            
            UpdateImage();

            this.invert = invert;
        }
        private void GenerateInvertedImageList(ref List<TestRating> ratings)
        {
            ratings.RemoveAt(ratings.Count - 1);
            ratings.Reverse();
            ratings.Add(TestRating.Undefined);

        }

        protected override void UpdateImage()
        {
            Image image = null;
            var ratings = new List<TestRating>(new[]{TestRating.P1, TestRating.P05to1, TestRating.P025to05, TestRating.P01to025, TestRating.P005to01, TestRating.P005, TestRating.Undefined, TestRating.Hidden});
            if (this.invert)
                GenerateInvertedImageList(ref ratings);
            var index = ratings.IndexOf(_rating);

            if (index >= 0 && RatingImages.Images.Count > index)
                image = ColorManager.TransformImage(RatingImages.Images[index]);

            if (ImageBox != null && RatingImages.Images.Count > 0)
            {
                if (image != null)
                    ImageBox.Image = image;
                else
                    ImageBox.Image = ColorManager.TransformImage(RatingImages.Images[RatingImages.Images.Count - 1]);
            }

            FixBackgrounColor();
        }

        public TestRating Rating
        {
            get
            {
                return _rating;
            }

            set
            {
                _rating = value;

                UpdateImage();
            }
        }        
    }
}
