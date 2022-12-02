using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POD.Controls
{
    public partial class BothTransformBoxes : UserControl
    {
        public BothTransformBoxes()
        {
            InitializeComponent();
            InitializeTransforms();
        }
        private void InitializeTransforms()
        {
            transformBox1.SelectedTransform = TransformTypeEnum.Linear;
            transformBoxYHat1.SelectedTransform = TransformTypeEnum.Linear;
        }
        public TransformTypeEnum xTransformSelected => transformBox1.SelectedTransform;
        public TransformTypeEnum yTransformSelected => transformBoxYHat1.SelectedTransform;
        public TransformBox xTransformBox => transformBox1;
        public TransformBoxYHat yTransformBox => transformBoxYHat1;
        public bool YTransformVisible
        {
            set {
                transformBoxYHat1.Visible = value;
                yTransform.Visible =value; 
            }
        }
    }
}
