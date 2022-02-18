using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POD.Controls
{
    public class TutorialButton : Button
    {
        protected override bool IsInputKey(Keys keyData)
        {
            if (keyData == Keys.Right || keyData == Keys.Left || keyData == Keys.Up || keyData == Keys.Down || keyData == Keys.Space)
            {
                return true;
            }
            else
            {
                return base.IsInputKey(keyData);
            }
        }
    }
}
