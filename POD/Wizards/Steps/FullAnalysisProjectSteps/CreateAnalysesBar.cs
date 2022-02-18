using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POD.Analyze;
using POD.Controls;

namespace POD.Wizards.Steps.FullAnalysisProjectSteps
{
    public partial class CreateAnalysesBar : WizardActionBar
    {
        PODButton UndoDeletes;

        public override bool SendKeys(Keys keyData)
        {
            var result = base.SendKeys(keyData);

            if (result)
                return true;

            if (keyData == (Keys.Control | Keys.R))
            {
                UndoDeletes.PerformClick();
                return true;
            }

            return false;
        }

        public CreateAnalysesBar(PODToolTip tooltip) : base(tooltip)
        {
            InitializeComponent();

            StepToolTip = new PODToolTip();

            nextButton.Enabled = false;

            UndoDeletes = new PODButton("Restore Analysis", "Restore a deleted analysis. (Ctrl + R)", StepToolTip);

            AddLeftButton(UndoDeletes, UndoDeletes_Click);

            //want this to fire first before any other functions
            AddEventToRightButton(finishButton, finishButton_Click);

            AddIconsToButtons();
            //AddEventToRightButton(prevButton, PrevButton_Click);
        }

        private void UndoDeletes_Click(object sender, EventArgs e)
        {
            MyPanel.UndoDeletedAnalysis();
        }


        void finishButton_Click(object sender, EventArgs e)
        {
            //call panel function to create analysis for each item in the analysis tree
            MyPanel.CheckStuck();

            if (!MyPanel.Stuck)
            {
                AnalysisList analyses = MyPanel.ExistingOrNewAnalysisList;
                AnalysisList removed = MyPanel.RemovedAnalysisList;

                if (Source.FinishArg == null)
                {
                    Source.FinishArg = new ProjectFinishArgs();
                }

                ProjectFinishArgs args = (ProjectFinishArgs)Source.FinishArg;

                args.UpdateAnalyses(analyses, removed);
            }

        }

        public override bool ProcessKeyboardShortCuts(ref Message msg, Keys keyData)
        {
            //if (keyData == ShortCutKey(Keys.R))
            //{
            //    MyPanel.UndoDeletedAnalysis();

            //    return true;
            //}

            return base.ProcessKeyboardShortCuts(ref msg, keyData);
        }
    }
}
