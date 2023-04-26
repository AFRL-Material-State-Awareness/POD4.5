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

            Items.Add(new TransformObjYHat(TransformTypeEnum.Linear));
            Items.Add(new TransformObjYHat(TransformTypeEnum.Log));
            Items.Add(new TransformObjYHat(TransformTypeEnum.Inverse));
            Items.Add(new TransformObjYHat(TransformTypeEnum.BoxCox));
            DropDownStyle = ComboBoxStyle.DropDownList;

            

            //SelectedIndexChanged += TransformBox_SelectedIndexChanged;
        }

        public TransformTypeEnum SelectedTransform
        {
            get
            {
                return ((TransformObjYHat) SelectedItem).TransformType; 
            }
            set
            {
                foreach(Object obj in Items)
                {
                    var type = obj as TransformObjYHat;

                    if(type != null)
                    {
                        if(type.TransformType == value)
                        {
                            SelectedItem = obj;
                        }
                    }
                }
            }
        }
    }

    public class TransformObjYHat
    {
        private TransformTypeEnum _transformType;
        private string _label;

        public TransformObjYHat(TransformTypeEnum myType)
        {
            _transformType = myType;

            switch (TransformType)
            {
                case TransformTypeEnum.BoxCox:
                    _label = "Box-Cox";
                    break;
                case TransformTypeEnum.Log:
                    _label = "Log";
                    break;
                case TransformTypeEnum.Exponetial:
                    _label = "Exponetional";
                    break;
                case TransformTypeEnum.Inverse:
                    _label = "Inverse";
                    break;
                case TransformTypeEnum.Linear:
                    _label = "Linear";
                    break;
                default:
                    _label = "Custom";
                    break;
            }
        }

        public string Label
        {
            get { return _label; }
        }

        public TransformTypeEnum TransformType
        {
            get { return _transformType; }
        }

        public override string ToString()
        {
            return Label;
        }
    }
}
