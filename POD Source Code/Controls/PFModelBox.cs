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
    public partial class PFModelBox : ComboBox
    {
        public PFModelBox()
        {
            InitializeComponent();

            Items.Add(new PFModelObj(PFModelEnum.Normal));
            Items.Add(new PFModelObj(PFModelEnum.Odds));

            DropDownStyle = ComboBoxStyle.DropDownList;
        }

        public PFModelEnum SelectedModel
        {
            get { return ((PFModelObj) SelectedItem).ModelType; }
            set
            {
                foreach (Object obj in Items)
                {
                    var type = obj as PFModelObj;

                    if (type != null)
                    {
                        if (type.ModelType == value)
                        {
                            SelectedItem = obj;
                        }
                    }
                }
            }
        }
    }

    public class PFModelObj
    {
        private PFModelEnum _modelType;
        private string _label;

        public PFModelObj(PFModelEnum myType)
        {
            _modelType = myType;

            switch (ModelType)
            {
                case PFModelEnum.Normal:
                    _label = "Logistic Reg";
                    break;
                case PFModelEnum.Odds:
                    _label = "Firth Logistic Reg";
                    break;
                default:
                    _label = "Undefined";
                    break;
            }
        }

        public string Label
        {
            get { return _label; }
        }

        public PFModelEnum ModelType
        {
            get { return _modelType; }
        }

        public override string ToString()
        {
            return Label;
        }
    }
}
