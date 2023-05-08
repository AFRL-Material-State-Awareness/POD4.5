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

            ImageBox.Image = image;

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
