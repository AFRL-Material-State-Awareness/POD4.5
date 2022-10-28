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
    public partial class ConfidenceBox : ComboBoxListEx
    {
        public ConfidenceBox()
        {
            InitializeComponent();
            Items.Add(new ConfIntObj(ConfidenceIntervalTypeEnum.StandardWald));
            Items.Add(new ConfIntObj(ConfidenceIntervalTypeEnum.ModifiedWald));
            Items.Add(new ConfIntObj(ConfidenceIntervalTypeEnum.LR));
            Items.Add(new ConfIntObj(ConfidenceIntervalTypeEnum.MLR));
            DropDownStyle = ComboBoxStyle.DropDownList;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }
        public ConfidenceIntervalTypeEnum SelectedConfInt
        {
            get
            {
                return ((ConfIntObj)SelectedItem).ConfIntervalType;
            }
            set
            {
                foreach (Object obj in Items)
                {
                    var type = obj as ConfIntObj;

                    if (type != null)
                    {
                        if (type.ConfIntervalType == value)
                        {
                            SelectedItem = obj;
                        }
                    }
                }
            }
        }

    }
    public class ConfIntObj
    {
        private ConfidenceIntervalTypeEnum _transformType;
        private string _label;

        public ConfIntObj(ConfidenceIntervalTypeEnum myType)
        {
            _transformType = myType;

            switch (ConfIntervalType)
            {
                case ConfidenceIntervalTypeEnum.StandardWald:
                    _label = "Std Wald";
                    break;
                case ConfidenceIntervalTypeEnum.ModifiedWald:
                    _label = "Mod Wald";
                    break;
                case ConfidenceIntervalTypeEnum.LR:
                    _label = "LR";
                    break;
                case ConfidenceIntervalTypeEnum.MLR:
                    _label = "MLR";
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

        public ConfidenceIntervalTypeEnum ConfIntervalType
        {
            get { return _transformType; }
        }

        public override string ToString()
        {
            return Label;
        }
    }
}
