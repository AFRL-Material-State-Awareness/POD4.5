using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POD.Controls;

namespace POD.Wizards.Steps.FullAnalysisProjectSteps
{
    public partial class ViewImportBar: WizardActionBar
    {
        PODBooleanButton _individualButton;
        PODBooleanButton _overlayButton;

        //private PODButton _prevDataButton;
        //private PODButton _nextDataButton;

        public override bool SendKeys(Keys keyData)
        {
            var result = base.SendKeys(keyData);

            if (result)
                return true;

            if (keyData == (Keys.Control | Keys.T))
            {
                _individualButton.PerformClick();
                return true;
            }

            if (keyData == (Keys.Control | Keys.I))
            {
                _overlayButton.PerformClick();
                return true;
            }

            return false;
        }
        
        public ViewImportBar(PODToolTip tooltip) : base(tooltip)
        {
            InitializeComponent();

            StepToolTip = new PODToolTip();

            _individualButton = new PODBooleanButton("Fit All Graphs", "Two Columns", false, "Select how to organize graphs. (Ctrl + T)", StepToolTip);
            AddLeftButton(_individualButton, resizeView_Click);

            _overlayButton = new PODBooleanButton("Group By Flaw", "Individual Graphs", true, "Display overlayed or individial graphs. (Ctrl + I)", StepToolTip);
            AddLeftButton(_overlayButton, stackView_Click);

            //_prevDataButton = new PODButton("Previous Data Source");
            //this.AddLeftButton(_prevDataButton, prevData_Click);

            //_nextDataButton = new PODButton("Next Data Source");
            //this.AddLeftButton(_nextDataButton, nextData_Click);

            //copy sources before going to next panel
            AddEventToRightButton(nextButton, NextButton_Click);

            AddIconsToButtons();
        }
        
        private void stackView_Click(object sender, EventArgs e)
        {
            _overlayButton.ButtonState = MyPanel.SwitchStackMode();
        }

        private void resizeView_Click(object sender, EventArgs e)
        {
            _individualButton.ButtonState = MyPanel.SwitchViewMode();
        }

        /*private void prevData_Click(object sender, EventArgs e)
        {
            MyPanel.PreviousDataSource();
        }

        private void nextData_Click(object sender, EventArgs e)
        {
            MyPanel.NextDataSource();
        }*/

        /*public void HideDataSourceButtons()
        {
            _nextDataButton.Hide();
            _prevDataButton.Hide();
        }

        public void ShowDataSourceButtons()
        {
            _nextDataButton.Show();
            _prevDataButton.Show();
        }*/


        void NextButton_Click(object sender, EventArgs e)
        {
            MyPanel.CopyInfoToSource();
        }

        public override bool ProcessKeyboardShortCuts(ref Message msg, Keys keyData)
        {
            //if (keyData == ShortCutKey(Keys.G))
            //{
            //    _individualButton.ButtonState = MyPanel.SwitchViewMode();

            //    return true;
            //}

            //if (keyData == ShortCutKey(Keys.I))
            //{
            //    _overlayButton.ButtonState = MyPanel.SwitchStackMode();

            //    return true;
            //}

            return base.ProcessKeyboardShortCuts(ref msg, keyData);
        }
    }
}
