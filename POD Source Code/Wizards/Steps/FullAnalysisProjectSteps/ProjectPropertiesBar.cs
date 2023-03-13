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
    public partial class ProjectPropertiesBar : WizardActionBar
    {
        PODButton _useLastButton;
        PODBooleanButton _showProperties;
        PODButton _solveAllAnalysesButton;
        public override bool SendKeys(Keys keyData)
        {
            var result = base.SendKeys(keyData);

            if (result)
                return true;

            if (keyData == (Keys.Control | Keys.L))
            {
                _useLastButton.PerformClick();
                return true;
            }

            if (keyData == (Keys.Control | Keys.OemOpenBrackets))
            {
                _showProperties.PerformClick();
                return true;
            }

            return false;
        }

        public ProjectPropertiesBar(PODToolTip tooltip) : base(tooltip)
        {
            InitializeComponent();

            StepToolTip = new PODToolTip();

            _useLastButton = new PODButton("Use Last", "Auto-fill with last values that were used. (Ctrl + L)", StepToolTip);
            _showProperties = new PODBooleanButton("Show Properties", "Hide Properties", true, "Show/hide Project Properties. (Ctrl + [)", StepToolTip);
            _solveAllAnalysesButton= new PODButton("Solve All Created Analyses", "Solves executes all the analyses with default parameters", StepToolTip);
            prevButton.Enabled = false;

            this.AddLeftButton(_useLastButton, UseLast_Click);
            this.AddLeftButton(_showProperties, ShowProperties_Click);
            this.AddLeftButton(_solveAllAnalysesButton, SolveAll_Click);
            //make sure data is copied to source objects
            //when next button is pressed
            AddEventToRightButton(nextButton, NextButton_Click);

            AddIconsToButtons();
        }

        private void ShowProperties_Click(object sender, EventArgs e)
        {
            _showProperties.ButtonState = MyPanel.HideProperties(_showProperties.ButtonState);
        }

        private void UseLast_Click(object sender, EventArgs e)
        {
            MyPanel.UseLastValues();
        }

        void NextButton_Click(object sender, EventArgs e)
        {
            MyPanel.CopySettingsToProject();
        }
        void SolveAll_Click(object sender, EventArgs e)
        {
            bool analysesPresent=MyPanel.RunAllAnalyses(sender, e);
            if (analysesPresent)
                MessageBox.Show("Run All Analyses Finished!");
            else
                MessageBox.Show("Please Add at least one analyses to run!");
        }

        public override bool ProcessKeyboardShortCuts(ref Message msg, Keys keyData)
        {
            //if (keyData == ShortCutKey(Keys.L))
            //{
            //    MyPanel.UseLastValues();

            //    return true;
            //}

            return base.ProcessKeyboardShortCuts(ref msg, keyData);
        }
    }
}
