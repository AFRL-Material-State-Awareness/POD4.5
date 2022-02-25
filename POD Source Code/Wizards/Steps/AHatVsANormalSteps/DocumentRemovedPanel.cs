using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POD;
using POD.ExcelData;
using POD.Controls;

namespace POD.Wizards.Steps.AHatVsANormalSteps
{
    public partial class DocumentRemovedPanel : WizardPanel
    {
        public DocumentRemovedPanel()
        {
            InitializeComponent();

            RemovedPoints.CreateButtons(this);
        }

        public DocumentRemovedPanel(PODToolTip tooltip) : base(tooltip)
        {
            StepToolTip = new PODToolTip();

            InitializeComponent();

            RemovedPoints.CreateButtons(this);
        }

        public override bool SendKeys(Keys keyData)
        {
            var result = base.SendKeys(keyData);

            if (result)
                return true;

            result = RemovedPoints.SendKeys(keyData);

            if (result)
                return true;

            return false;
        }

        public void UpdateMRUList()
        {
            RemovedPoints.UpdateMRUList();
        }

        public override WizardSource Source
        {
            get
            {
                return base.Source;
            }
            set
            {
                base.Source = value;

                RemovedPoints.Analysis = Source as Analyze.Analysis;
            }
        }

        public override bool Stuck
        {
            get
            {
                return !Analysis.Data.EverythingCommented && RemovedPoints.RemovedPointsCount != 0;
            }
        }

        public override bool CheckStuck()
        {
            if (Stuck)
            {
                MessageBox.Show("All removed points must be commented on before finishing the analysis.");
            }

            return Stuck;
        }

        public override void RefreshValues()
        {
            base.RefreshValues();

            RemovedPoints.RefreshPanel();
            
        }

        public void ExportProjectToExcel()
        {
            Analysis.ExportProjectToExcel();
        }

        public void ExportToExcel()
        {
            ExcelExport writer = new ExcelExport();

            string fileName = "";
            DataTable table = null;

            Analysis.GetProjectInfo(out fileName, out table);

            var shouldSave = false;
            var name = writer.AskUserToSave(fileName + Globals.FileTimeStamp, out shouldSave);

            if (shouldSave)
            {
                Analysis.WriteToExcel(writer, false, table);
                writer.SaveToFile(name);
            }
        }

        internal override void PrepareGUI()
        {
            base.PrepareGUI();
        }
    }
}
