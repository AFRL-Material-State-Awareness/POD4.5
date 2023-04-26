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
    public partial class TransformBoxYHat : ComboBox
    {
        public TransformBoxYHat()
        {
            InitializeComponent();

            Items.Add(new TransformObj(TransformTypeEnum.Linear));
            Items.Add(new TransformObj(TransformTypeEnum.Log));
            Items.Add(new TransformObj(TransformTypeEnum.Inverse));
            Items.Add(new TransformObj(TransformTypeEnum.BoxCox));
            DropDownStyle = ComboBoxStyle.DropDownList;
        
        }

        public TransformTypeEnum SelectedTransform
        {
            get
            {
                return ((TransformObj) SelectedItem).TransformType; 
            }
            set
            {
                foreach(Object obj in Items)
                {
                    var type = obj as TransformObj;

                    if (type?.TransformType == value)
                        SelectedItem = obj;
                }
            }
        }
    }
}
