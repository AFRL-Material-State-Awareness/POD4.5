using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POD
{
    public partial class Tutorial : Form
    {
        List<Image> _steps = new List<Image>();
        int index;

        public Tutorial()
        {
            InitializeComponent();

            _steps.Add(this.step01.BackgroundImage = global::POD.Properties.Resources.Step_01);
            _steps.Add(this.step01.BackgroundImage = global::POD.Properties.Resources.Step_02);
            _steps.Add(this.step01.BackgroundImage = global::POD.Properties.Resources.Step_03);
            _steps.Add(this.step01.BackgroundImage = global::POD.Properties.Resources.Step_04);
            _steps.Add(this.step01.BackgroundImage = global::POD.Properties.Resources.Step_05);
            _steps.Add(this.step01.BackgroundImage = global::POD.Properties.Resources.Step_06);
            _steps.Add(this.step01.BackgroundImage = global::POD.Properties.Resources.Step_07);
            _steps.Add(this.step01.BackgroundImage = global::POD.Properties.Resources.Step_08);
            _steps.Add(this.step01.BackgroundImage = global::POD.Properties.Resources.Step_09);
            _steps.Add(this.step01.BackgroundImage = global::POD.Properties.Resources.Step_10);
            _steps.Add(this.step01.BackgroundImage = global::POD.Properties.Resources.Step_11);
            _steps.Add(this.step01.BackgroundImage = global::POD.Properties.Resources.Step_12);
            _steps.Add(this.step01.BackgroundImage = global::POD.Properties.Resources.Step_13);
            _steps.Add(this.step01.BackgroundImage = global::POD.Properties.Resources.Step_14);

            KeyPreview = true;

            RefreshStep();

            comboBox1.SelectedIndex = 0;

        }

        private void Previous_Click(object sender, EventArgs e)
        {
            index--;

            if (index < 0)
                index = 0;

            RefreshStep();
        }

        private void Next_Click(object sender, EventArgs e)
        {
            index++;

            if (index >= _steps.Count)
            {
                index = _steps.Count - 1;

                var result = MessageBox.Show("Would you like to quit the tutorial?", "Quit?", MessageBoxButtons.YesNo);

                if (result == System.Windows.Forms.DialogResult.Yes)
                    Close();
            }

            RefreshStep();
        }

        private void RefreshStep()
        {
            SuspendLayout();

            step01.BackgroundImage = _steps[index];

            ResumeLayout();

            comboBox1.SelectedIndex = index;
            label1.Text = "Step " + (index + 1).ToString("00") + " of " + (_steps.Count).ToString("00");
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            var keyData = e.KeyData;

            if (ActiveControl != this.comboBox1)
            {
                if (keyData == (Keys.Right) || keyData == (Keys.Down))
                {
                    this.Next.Select();
                    this.Next.PerformClick();
                    return;
                }
                if (keyData == (Keys.Left) || keyData == (Keys.Up))
                {
                    this.Previous.Select();
                    this.Previous.PerformClick();
                    return;
                }
                if (keyData == (Keys.Escape))
                {
                    Close();
                    return;
                }
            }

            base.OnKeyDown(e);
        }


        private void Mouse_Click(object sender, MouseEventArgs e)
        {
            if(e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.Next.Select();
                this.Next.PerformClick();
            }
            else if(e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                this.Previous.Select();
                this.Previous.PerformClick();
            }
        }

        private void Combo_Changed(object sender, EventArgs e)
        {
            index = comboBox1.SelectedIndex;

            RefreshStep();
        }
    }
}
