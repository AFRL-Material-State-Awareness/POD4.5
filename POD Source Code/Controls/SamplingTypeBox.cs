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
    public partial class SamplingTypeBox : ComboBox
    {
        public SamplingTypeBox()
        {
            InitializeComponent();
            Items.Add(new SampleTypeObj(SamplingTypeEnum.SimpleRandomSampling));
            Items.Add(new SampleTypeObj(SamplingTypeEnum.RankedSetSampling));
            DropDownStyle = ComboBoxStyle.DropDownList;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }
        public SamplingTypeEnum SelectedSamplingType
        {
            get
            {
                return ((SampleTypeObj)SelectedItem).SamplingType;
            }
            set
            {
                foreach (Object obj in Items)
                {
                    var type = obj as SampleTypeObj;

                    if (type != null)
                    {
                        if (type.SamplingType == value)
                        {
                            SelectedItem = obj;
                        }
                    }
                }
            }
        }

    }
    public class SampleTypeObj
    {
        private SamplingTypeEnum _samplingType;
        private string _label;

        public SampleTypeObj(SamplingTypeEnum myType)
        {
            _samplingType = myType;

            switch (SamplingType)
            {
                case SamplingTypeEnum.SimpleRandomSampling:
                    _label = "Simple Random Sampling";
                    break;
                case SamplingTypeEnum.RankedSetSampling:
                    _label = "Ranked Set Sampling";
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

        public SamplingTypeEnum SamplingType
        {
            get { return _samplingType; }
        }

        public override string ToString()
        {
            return Label;
        }
    }
}

