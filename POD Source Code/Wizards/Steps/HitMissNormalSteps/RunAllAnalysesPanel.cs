using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POD.Controls;
using POD.Analyze;
using POD.Data;
namespace POD.Wizards.Steps.HitMissNormalSteps
{
    public partial class RunAllAnalysesPanel : WizardPanel
    {
        private RunAllAnalysis _runAllAnalyses;
        private BackgroundWorker _analysisLauncherAll;
        public RunAllAnalysesPanel(PODToolTip tooltip)
            : base(tooltip)
        {
            StepToolTip = new PODToolTip();

            InitializeComponent();
        }
        public override void RefreshValues()
        {
            //base.RefreshValues();
            SetupAnalysisLauncher();
            if (_analysisLauncherAll.IsBusy == false)
            {

                //_updatingTransformResults = true;
                _analysisLauncherAll.RunWorkerAsync();
                //_runAllAnalyses.RunAllAnalyses();
            }
        }
        public override WizardSource Source
        {
            get
            {
                return _source;
            }
            set
            {
                _source = value;

                _runAllAnalyses = _source as RunAllAnalysis;


            }
        }
        private void SetupAnalysisLauncher()
        {
            _analysisLauncherAll = new BackgroundWorker();

            _analysisLauncherAll.WorkerSupportsCancellation = true;
            _analysisLauncherAll.WorkerReportsProgress = true;

            _analysisLauncherAll.DoWork += new DoWorkEventHandler(Background_StartAnalysis);
            _analysisLauncherAll.ProgressChanged += new ProgressChangedEventHandler(Background_AnalysisProgressChanged);
            _analysisLauncherAll.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Background_FinishedAnalysis);
        }
        private void Background_StartAnalysis(object sender, DoWorkEventArgs e)
        {
            _runAllAnalyses.IsBusy = true;
            _runAllAnalyses.RunAllAnalyses();         
        }
        private void Background_AnalysisProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }
        private void Background_FinishedAnalysis(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_analysisLauncherAll != null)
            {
                _analysisLauncherAll.Dispose();
                _analysisLauncherAll = null;
            }

            //SelectChartBasedOnAnalysis();

            //UpdateModelCompares();

            //_updatingTransformResults = false;

            _runAllAnalyses.IsBusy = false;
        }

    }
}
