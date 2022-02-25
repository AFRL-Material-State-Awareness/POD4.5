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
    public partial class PODInfoNumericUpDown : PODChartNumericUpDown
    {
        ChartPartType _type = ChartPartType.Undefined;

        public PODInfoNumericUpDown()
        {
            InitializeComponent();
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

            //if (Parent != null)
            //    ImageBox.BackColor = Parent.BackColor;

            ImageBox.Image = image;

            FixBackgrounColor();
        }

        public new ChartPartType PartType
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

        private void CopyAll_Click(object sender, EventArgs e)
        {

        }

        
        
    }
}
