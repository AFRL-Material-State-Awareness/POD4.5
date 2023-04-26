using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POD.Controls
{
    public abstract class TransformBoxControl : ComboBoxListEx
    {
        public TransformTypeEnum SelectedTransform
        {
            get
            {
                return ((TransformObj)SelectedItem).TransformType;
            }
            set
            {
                foreach (Object obj in Items)
                {
                    var type = obj as TransformObj;

                    if (type?.TransformType == value)
                        SelectedItem = obj;
                }
            }
        }
    }
}
