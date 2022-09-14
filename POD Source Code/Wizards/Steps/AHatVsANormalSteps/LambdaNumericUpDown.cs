using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace POD.Wizards.Steps.AHatVsANormalSteps
{
    //source: Stack overflow
    //Question: https://stackoverflow.com/questions/63191222/numericupdown-custom-increment-using-default-buttons
    //Author : Jimi -- https://stackoverflow.com/users/7444103/jimi
    public class LambdaNumericUpDown : NumericUpDown
    {
        private decimal customIncrement = 0.1m;
        public LambdaNumericUpDown()
        {

        }

        public override void UpButton() =>
        Value = Math.Min(Value + (customIncrement - (Value % customIncrement)), Maximum);

        public override void DownButton() =>
            Value = Math.Max(Value - (Value % customIncrement == 0 ? customIncrement : Value % customIncrement), Minimum);

        protected override void OnValueChanged(EventArgs e)
        {
            base.OnValueChanged(e);
            Value = Math.Max(Math.Min(Value, Maximum), Minimum);
        }
    }
}
