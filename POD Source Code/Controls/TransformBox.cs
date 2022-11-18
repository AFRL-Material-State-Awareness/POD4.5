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
    public partial class TransformBox : ComboBox
    {
        public TransformBox()
        {
            InitializeComponent();

            Items.Add(new TransformObj(TransformTypeEnum.Linear));
            Items.Add(new TransformObj(TransformTypeEnum.Log));
            //Items.Add(new TransformObj(TransformTypeEnum.Exponetial));
            Items.Add(new TransformObj(TransformTypeEnum.Inverse));
            //Items.Add(new TransformObj(TransformTypeEnum.BoxCox));
            DropDownStyle = ComboBoxStyle.DropDownList;

            

            //SelectedIndexChanged += TransformBox_SelectedIndexChanged;
        }

        //void TransformBox_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    MessageBox.Show("Changed Transform");
        //}

        public TransformTypeEnum SelectedTransform
        {
            get
            {
                if (SelectedItem != null)
                    return ((TransformObj)SelectedItem).TransformType;
                else
                    return TransformTypeEnum.Linear;
            }
            set
            {
                foreach(Object obj in Items)
                {
                    var type = obj as TransformObj;

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

    public class TransformObj
    {
        private TransformTypeEnum _transformType;
        private string _label;

        public TransformObj(TransformTypeEnum myType)
        {
            _transformType = myType;

            switch (TransformType)
            {
                //box cox label only used in choose transform panel. X does not have a box-cox transform in the
                //User interface
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
