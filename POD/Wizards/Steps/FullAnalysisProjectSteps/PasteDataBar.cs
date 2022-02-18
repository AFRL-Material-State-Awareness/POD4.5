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
    public partial class PasteDataBar : WizardActionBar
    {
        PODButton _pasteButton;
        PODButton _newButton;
        PODButton _deleteButton;
        PODButton _restoreButton;

        public override bool SendKeys(Keys keyData)
        {
            var result = base.SendKeys(keyData);

            if (result)
                return true;

            if (SelectEditButton(keyData))
                return true;

            if (keyData == (Keys.Control | Keys.V))
            {
                _pasteButton.PerformClick();
                return true;
            }

            if (keyData == (Keys.Control | Keys.G))
            {
                _newButton.PerformClick();
                return true;
            }

            if (keyData == (Keys.Control | Keys.D))
            {
                _deleteButton.PerformClick();
                return true;

            }
            
            if (keyData == (Keys.Control | Keys.R))
            {
                _restoreButton.PerformClick();
                return true;
            }

            return false;
        }

        private bool SelectEditButton(Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.D1))
            {
                MyPanel.SwitchEdit();
                return true;
            }

            if (keyData == (Keys.Control | Keys.D2))
            {
                MyPanel.SwitchID();
                return true;
            }

            if (keyData == (Keys.Control | Keys.D3))
            {
                MyPanel.SwitchMeta();
                return true;

            }

            if (keyData == (Keys.Control | Keys.D4))
            {
                MyPanel.SwitchFlaw();
                return true;
            }

            if (keyData == (Keys.Control | Keys.D5))
            {
                MyPanel.SwitchResponse();
                return true;
            }

            if (keyData == (Keys.Control | Keys.D6))
            {
                MyPanel.SwitchUndefine();
                return true;
            }

            return false;
        }

        public PasteDataBar(PODToolTip tooltip) : base(tooltip)
        {
            InitializeComponent();

            StepToolTip = new PODToolTip();
            
            _pasteButton = new PODButton("Paste", "Paste data from clipBoard. (Ctrl + V)", StepToolTip);
            AddLeftButton(_pasteButton, PasteButton_Click);

            _newButton = new PODButton("New Source", "Add new data source to project. (Ctrl + G)", StepToolTip);
            AddLeftButton(_newButton, NewSourceButton_Click);

            _deleteButton = new PODButton("Delete Source", "Delete data source from project. (Ctrl + D)", StepToolTip);
            AddLeftButton(_deleteButton, DeleteSourceButton_Click);

            _restoreButton = new PODButton("Restore Source", "Restore data source deleted from the project. (Ctrl + R)", StepToolTip);
            AddLeftButton(_restoreButton, RestoreSourceButton_Click);

            //copy sources before going to next panel
            AddEventToRightButton(nextButton, NextButton_Click);

            AddEventToRightButton(prevButton, PrevButton_Click);

            AddIconsToButtons();
        }

        private void RestoreSourceButton_Click(object sender, EventArgs e)
        {
            MyPanel.RestoreDeletedSources();
        }

        

        private void PrevButton_Click(object sender, EventArgs e)
        {
            MyPanel.BackupTables();
        }

        private void DeleteSourceButton_Click(object sender, EventArgs e)
        {
            MyPanel.DeleteCurrentSource();
        }

        private void NewSourceButton_Click(object sender, EventArgs e)
        {
            MyPanel.AddNewSourcePage(true);
            MyPanel.AddTabsFromList();
        }

        void NextButton_Click(object sender, EventArgs e)
        {
            MyPanel.CopyDataToSource();
        }

        private void PasteButton_Click(object sender, EventArgs e)
        {
            MyPanel.PasteFromClipboard();
        }

        private void InsertButton_Click(object sender, EventArgs e)
        {
            MyPanel.InsertFromClipboard();
        }

        public override bool ProcessKeyboardShortCuts(ref Message msg, Keys keyData)
        {
            //if (keyData == ShortCutKey(Keys.V))
            //{
            //    MyPanel.PasteFromClipboard();

            //    return true;
            //}

            //if (keyData == ShortCutKey(Keys.S))
            //{
            //    MyPanel.AddNewSourcePage(true);
            //    MyPanel.AddTabsFromList();

            //    return true;
            //}

            //if (keyData == ShortCutKey(Keys.D))
            //{
            //    MyPanel.DeleteCurrentSource();

            //    return true;
            //}

            //if (keyData == ShortCutKey(Keys.R))
            //{
            //    MyPanel.RestoreDeletedSources();

            //    return true;
            //}

            return base.ProcessKeyboardShortCuts(ref msg, keyData);
        }
    }
}
