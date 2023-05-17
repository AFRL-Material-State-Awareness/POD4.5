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
            var ratings = new List<TestRating>(new[]{TestRating.P1, TestRating.P05to1, TestRating.P025to05, TestRating.P01to025, TestRating.P005to01, TestRating.P005, TestRating.Undefined});
            if (this.invert)
                GenerateInvertedImageList(ref ratings);
            var index = ratings.IndexOf(_rating);

            if (index >= 0 && RatingImages.Images.Count > index)
                image = RatingImages.Images[index];

            if (ImageBox != null && RatingImages.Images.Count > 0)
            {
                if (image != null)
                    ImageBox.Image = image;
                else
                    ImageBox.Image = RatingImages.Images[RatingImages.Images.Count - 1];
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
