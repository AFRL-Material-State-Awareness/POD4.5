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
    public partial class PFModelBox : ComboBoxListEx
    {
        public PFModelBox()
        {
            InitializeComponent();

            Items.Add(new PFModelObj(HitMissRegressionType.LogisticRegression));
            Items.Add(new PFModelObj(HitMissRegressionType.FirthLogisticRegression));

            DropDownStyle = ComboBoxStyle.DropDownList;
        }

        public HitMissRegressionType SelectedModel
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
        private HitMissRegressionType _modelType;
        private string _label;

        public PFModelObj(HitMissRegressionType myType)
        {
            _modelType = myType;

            switch (ModelType)
            {
                case HitMissRegressionType.LogisticRegression:
                    _label = "Logistic Reg";
                    break;
                case HitMissRegressionType.FirthLogisticRegression:
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

        public HitMissRegressionType ModelType
        {
            get { return _modelType; }
        }

        public override string ToString()
        {
            return Label;
        }
    }
}
