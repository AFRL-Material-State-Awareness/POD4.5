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
        private TransformTypeEnum xTransformAll=TransformTypeEnum.Linear;
        private TransformTypeEnum yTransformAll=TransformTypeEnum.Linear;
        //private PODChart _chart;
        public RunAllAnalysesPanel(PODToolTip tooltip)
            : base(tooltip)
        {
            StepToolTip = new PODToolTip();
            
            InitializeComponent();
            //podChart1.SetupChart();
        }
        private void CheckAnalysisType()
        {
            if(_runAllAnalyses.AnalysisDataType== AnalysisDataTypeEnum.HitMiss)
            {
                bothTransformBoxes1.YTransformVisible = false;
            }
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
                CheckAnalysisType();

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
            
            
            this.Invoke((MethodInvoker)delegate ()
            {
                podChart1.InitSetupChart(_runAllAnalyses.PODTables.Count);
                this.xTransformAll = bothTransformBoxes1.xTransformSelected;
                if (_runAllAnalyses.AnalysisDataType == AnalysisDataTypeEnum.AHat)
                    this.yTransformAll = bothTransformBoxes1.yTransformSelected;
                
                podChart1.SetXAxisWindow(_runAllAnalyses.OverallFlawMin, _runAllAnalyses.OverallFlawMax, _runAllAnalyses.OverallResponseMin, _runAllAnalyses.OverallResponseMax);
                //var chart = _charts[yAxisIndex * xRowCount + xAxisIndex];
                //var chart = podChart1;
                //watch.Restart();


                //watch.Stop();
                podChart1.FillChartAll(_runAllAnalyses.PODTables);
                //UpdateChartsWithCurrentView(chart);
                podChart1.Update();
            });
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
