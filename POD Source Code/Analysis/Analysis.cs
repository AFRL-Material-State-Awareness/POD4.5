﻿using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Data;
using POD.Data;
using System.Diagnostics;
using System.Windows.Forms;
using POD.ExcelData;
using System.IO;
using System.Linq;
using SpreadsheetLight;
using POD.Controls;
using POD;

namespace POD.Analyze
{
    /// <summary>
    ///     Holds all the information and data related to the analysis.
    /// </summary>
    [Serializable]
    public class Analysis : WizardSource
    {
        //TODO: decide if Analysis is going to manage the archived analyses or if an external archive manager will manage everything instead
        private Analysis _activeAnalysis;
        private int _activeAnalysisIndex;

        //TODO: Should this be queried from the Python code since it is the only one that really knows what version it is working with?
        //private string _algorithmVersion;
        private List<Analysis> _archivedOutputs;

        [NonSerialized]
        private BackgroundWorker analysisLauncher;
        [NonSerialized]
        public EventHandler AnalysisDone;
        [NonSerialized]
        public EventHandler EventsNeedToBeCleared;
        [NonSerialized]
        public GetProjectInfoHandler ProjectInfoNeeded;
        [NonSerialized]
        public EventHandler ExportProject;
        [NonSerialized]
        public AnalysisListHandler CreatedAnalysis;

        /// <summary>
        /// No input values or data will be allowed to be changed while the analysis is frozen.
        /// Created this state to allow UI to be sync'd with the analysis values after being loaded from file.
        /// </summary>
        private bool _isFrozen = false;

        public bool AnalysisRunning
        {
            get
            {
                if (stillRunningAnalysis)
                    return true;

                if(analysisLauncher == null)
                    return false;

                return analysisLauncher.IsBusy && stillRunningAnalysis;
            }
        }

        public bool IsFrozen
        {
            get { return _isFrozen; }
            set
            {
                Data.IsFrozen = value;
                _isFrozen = value; 
            }
        }
        #region Fields

        /// <summary>
        ///     Holds the tabular data associated with the analysis and anything associated data transforms.
        /// </summary>
        private AnalysisData _data;

        /// <summary>
        ///     the time and date the analysis was performed
        /// </summary>
        private DateTime _analysisDate;

        

        #endregion

        #region Constructors

        public Analysis()
        {
            Initialize();

            SetupAnalysisLauncher();
        }

        public Analysis(DataSource mySource)
        {
            Initialize();

            SetDataSource(mySource);

            SetupAnalysisLauncher();
        }

        

        public Analysis(DataSource mySource, List<string> myFlaws, List<string> myMetaDatas, List<string> myResponses,
            List<string> mySpecIDs)
        {
            Initialize();

            SetDataSource(mySource, myFlaws, myMetaDatas, myResponses, mySpecIDs);

            SetupAnalysisLauncher();
        }

        public Analysis(DataSource source, SourceInfo info, string flawName, List<string> responses)
        {
            Initialize();

            SetDataSource(source, flawName, responses);
            SetInfo(info);

            Data.ActivateFlaw(flawName);
            Data.ActivateResponses(responses);

            CalculateInitialValuesWithNewData();

            SetupAnalysisLauncher();
        }

        private void SetInfo(SourceInfo info)
        {
            CalculateInitialValuesWithNewInfo(info);
        }

        private void SetDataSource(DataSource source, string flawName, List<string> responses)
        {
            SourceName = source.SourceName;

            _data.SetSource(source, flawName, responses);
        }

        #endregion

        #region Properties

        //output parameters so far

        /// <summary>
        ///     Name of the flaw size column that will be used when performing calculations.
        /// </summary>
        public string InAnalysisFlawColumnName
        {
            get
            {
                return _inAnalysisFlawColumnName;
            }

            set
            {
                if (!IsFrozen)
                    _inAnalysisFlawColumnName = value;
            }
        }

        /// <summary>
        ///     Holds the tabular data associated with the analysis and anything associated data transforms.
        /// </summary>
        public AnalysisData Data
        {
            get { return _data; }
        }

        /// <summary>
        /// When exporting to Excel this is the worksheet name
        /// </summary>
        public string WorksheetName
        {
            get
            {
                return _worksheetName;
            }

            set
            {
                if (!IsFrozen)
                    _worksheetName = value;
            }
        }

        /// <summary>
        ///     The largest flaw that will be used in calculations.
        /// </summary>
        public double InFlawMax
        {
            get
            {
                return _inFlawMax;
            }

            set
            {
                if (!IsFrozen)
                    _inFlawMax = value;
            }
        }

        /// <summary>
        /// The smallest transformed crack size.
        /// </summary>
        //double _inFlawMin = 0.0;

        /// <summary>
        ///     The smallest flaw that will be used in calculations.
        /// </summary>
        public double InFlawMin
        {
            get
            {
                return _inFlawMin;
            }

            set
            {
                if (!IsFrozen)
                    _inFlawMin = value;
            }
        }

        /*{ 
            get
            {
                return _podDoc.GetInvtransformedFlawValue(_inFlawMin);
            }
            
            set
            {
                _inFlawMin = _podDoc.GetTransformedFlawValue(value);
            }
        }*/

        /// <summary>
        ///     Name of the flaw size.
        /// </summary>
        public string FlawName
        {
            get
            {
                return Data.AvailableFlawNames[0];
            }
        }
        
        /// <summary>
        ///     Units flaw size is measured in.
        /// </summary>
        public string FlawUnit
        {
            get
            {
                return Data.AvailableFlawUnits[0];
            }
        }

        private PFModelEnum _pfModelIn;

        public PFModelEnum InHitMissModel
        {
            get { return _pfModelIn; }
            set { if (!IsFrozen) _pfModelIn = value; }
        }

        

        private double NoBigNumbers(double myValue)
        {
            if (myValue > 1E9)
            {
                return Double.NaN;
            }

            return myValue;
        }

        double _outModelIntercept;

        /// <summary>
        ///     Linear fit model's Y axis intercept (y = ax+b)
        /// </summary>
        public double OutModelIntercept
        {
            get
            {
                return NoBigNumbers(_outModelIntercept);
            }

            private set
            {
                if (!IsFrozen)
                    _outModelIntercept = value;
            }
        }

        double _outModelResidualError;

        /// <summary>
        ///     Linear fit model's residual error (y = ax+b)
        /// </summary>
        public double OutModelResidualError
        {
            get
            {
                return NoBigNumbers(_outModelResidualError);
            }

            private set
            {
                if (!IsFrozen)
                    _outModelResidualError = value;
            }
        }

        double _outResponseDecisionPODSigma;

        /// <summary>
        ///    POD model's sigma value
        /// </summary>
        public double OutResponseDecisionPODSigma
        {
            get
            {
                return NoBigNumbers(_outResponseDecisionPODSigma);
            }

            private set
            {
                if (!IsFrozen)
                    _outResponseDecisionPODSigma = value;
            }
        }

        double _outModelSlope;

        /// <summary>
        ///     Linear fit model's slope (y = ax+b)
        /// </summary>
        public double OutModelSlope
        {
            get
            {
                return NoBigNumbers(_outModelSlope);
            }

            private set
            {
                if (!IsFrozen)
                    _outModelSlope = value;
            }
        }

        double _outModelSlopeError;

        /// <summary>
        ///     Linear fit model's slope std error (y = ax+b)
        /// </summary>
        public double OutModelSlopeStdError
        {
            get
            {
                return NoBigNumbers(_outModelSlopeError);
            }

            private set
            {
                if (!IsFrozen)
                    _outModelSlopeError = value;
            }
        }

        double _outModelInterceptStdError;

        /// <summary>
        ///     Linear fit model's intercept std error (y = ax+b)
        /// </summary>
        public double OutModelInterceptStdError
        {
            get
            {
                return NoBigNumbers(_outModelInterceptStdError);
            }

            private set
            {
                if (!IsFrozen)
                    _outModelInterceptStdError = value;
            }
        }

        double _outModelResidualErrorStdError;

        /// <summary>
        ///     Linear fit model's residual error std error (y = ax+b)
        /// </summary>
        public double OutModelResidualErrorStdError
        {
            get
            {
                return NoBigNumbers(_outModelResidualErrorStdError);
            }

            private set
            {
                if (!IsFrozen)
                    _outModelResidualErrorStdError = value;
            }
        }

        double _outRepeatabilityError;

        /// <summary>
        ///     Repeatability error for the data set
        /// </summary>
        public double OutRepeatabilityError
        {
            get
            {
                return NoBigNumbers(_outRepeatabilityError);
            }

            private set
            {
                if (!IsFrozen)
                    _outRepeatabilityError = value;
            }
        }

        double _outResponseDecisionPODA50Value;
        

        /// <summary>
        ///     A50 flaw size for the response data decision threshold
        /// </summary>
        public double OutResponseDecisionPODA50Value
        {
            get
            {
                return NoBigNumbers(_outResponseDecisionPODA50Value);
            }

            private set
            {
                if (!IsFrozen)
                    _outResponseDecisionPODA50Value = value;
            }
        }

        //double _outResponseDecisionPODA50Value_All;

        ///// <summary>
        /////     A50 flaw size for the response data decision threshold
        ///// </summary>
        //public double OutResponseDecisionPODA50Value_All
        //{
        //    get
        //    {
        //        return NoBigNumbers(_outResponseDecisionPODA50Value_All);
        //    }

        //    private set
        //    {
        //        if (!IsFrozen)
        //            _outResponseDecisionPODA50Value_All = value;
        //    }
        //}

        //public double OutResponseDecisionPODA50Plot
        //{
        //    get
        //    {
        //        return _podDoc.GetTransformedFlawValue(OutResponseDecisionPODA50Value);
        //    }

        //}

        

        public void RestoreBackup(double mya50, double mya90, double mya9095)
        {
            _outResponseDecisionPODA50Value = mya50;
            _outResponseDecisionPODLevelValue = mya90;
            _outResponseDecisionPODConfidenceValue = mya9095;
        }

        double _outResponseDecisionPODConfidenceValue;

        /// <summary>
        ///     POD flaw size for the response data decision threshold confidence value (a90/95)
        /// </summary>
        public double OutResponseDecisionPODConfidenceValue
        {
            get
            {
                return NoBigNumbers(_outResponseDecisionPODConfidenceValue);
            }

            private set
            {
                if (!IsFrozen)
                    _outResponseDecisionPODConfidenceValue = value;
            }
        }

        //double _outResponseDecisionPODConfidenceValue_All;

        ///// <summary>
        /////     POD flaw size for the response data decision threshold confidence value (a90/95)
        ///// </summary>
        //public double OutResponseDecisionPODConfidenceValue_All
        //{
        //    get
        //    {
        //        return NoBigNumbers(_outResponseDecisionPODConfidenceValue_All);
        //    }

        //    private set
        //    {
        //        if (!IsFrozen)
        //            _outResponseDecisionPODConfidenceValue_All = value;
        //    }
        //}

        

        //public double OutResponseDecisionPODConfidencePlot
        //{
        //    get
        //    {
        //        return _podDoc.GetTransformedFlawValue(OutResponseDecisionPODConfidenceValue);
        //    }

        //}

        double _outResponseDecisionPODLevelValue;

        /// <summary>
        ///     POD flaw size for the response data decision threshold value (a90)
        /// </summary>
        public double OutResponseDecisionPODLevelValue
        {
            get
            {
                return NoBigNumbers(_outResponseDecisionPODLevelValue);
            }

            private set
            {
                if (!IsFrozen)
                    _outResponseDecisionPODLevelValue = value;
            }
        }

        //double _outResponseDecisionPODLevelValue_All;

        ///// <summary>
        /////     POD flaw size for the response data decision threshold value (a90)
        ///// </summary>
        //public double OutResponseDecisionPODLevelValue_All
        //{
        //    get
        //    {
        //        return NoBigNumbers(_outResponseDecisionPODLevelValue_All);
        //    }

        //    private set
        //    {
        //        if (!IsFrozen)
        //            _outResponseDecisionPODLevelValue_All = value;
        //    }
        //}

        //public double OutResponseDecisionPODLevelPlot
        //{
        //    get
        //    {
        //        return _podDoc.GetTransformedFlawValue(OutResponseDecisionPODLevelValue);
        //    }

        //}

        double _outTestEqualVariance;

        /// <summary>
        ///     Bartlett value for Equal Variance test
        /// </summary>
        public double OutTestEqualVariance
        {
            get
            {
                return NoBigNumbers(_outTestEqualVariance);
            }

            private set
            {
                if (!IsFrozen)
                    _outTestEqualVariance = value;
            }
        }

        double _outTestEqualVariance_p;

        /// <summary>
        ///     Bartlett value for Equal Variance test
        /// </summary>
        public double OutTestEqualVariance_p
        {
            get
            {
                return NoBigNumbers(_outTestEqualVariance_p);
            }

            private set
            {
                if (!IsFrozen)
                    _outTestEqualVariance_p = value;
            }
        }

        string _outTestEqualVarianceRating;

        public string OutTestEqualVarianceRating
        {
            get { return _outTestEqualVarianceRating; }
            set { if (!IsFrozen) _outTestEqualVarianceRating = value; }
        }

        double _outTestLackOfFit;

        /// <summary>
        ///     Pure Error for Lack Of Fit test
        /// </summary>
        public double OutTestLackOfFit
        {
            get
            {
                return NoBigNumbers(_outTestLackOfFit);
            }

            private set
            {
                if (!IsFrozen)
                    _outTestLackOfFit = value;
            }
        }

        double _outTestLackOfFit_p;

        /// <summary>
        ///     p-Value for Pure Error for Lack Of Fit test
        /// </summary>
        public double OutTestLackOfFit_p
        {
            get
            {
                return NoBigNumbers(_outTestLackOfFit_p);
            }

            private set
            {
                if (!IsFrozen)
                    _outTestLackOfFit_p = value;
            }
        }

        string _outTestLackOfFitRating = "";

        public string OutTestLackOfFitRating
        {
            get { return _outTestLackOfFitRating; }
            set { if (!IsFrozen) _outTestLackOfFitRating = value; }
        }

        int _outTestLackOfFitDegreesFreedom;

        public int OutTestLackOfFitDegreesFreedom
        {
            get { return _outTestLackOfFitDegreesFreedom; }
            set { if (!IsFrozen) _outTestLackOfFitDegreesFreedom = value; }
        }

        bool _outTestLackOfFitCalculated;

        public bool OutTestLackOfFitCalculated
        {
            get { return _outTestLackOfFitCalculated; }
            set { if (!IsFrozen) _outTestLackOfFitCalculated = value; }
        }

        double _outTestNormality;

        /// <summary>
        ///     Anderson-Darling value for Normality test
        /// </summary>
        public double OutTestNormality
        {
            get
            {
                return NoBigNumbers(_outTestNormality);
            }

            private set
            {
                if (!IsFrozen)
                    _outTestNormality = value;
            }
        }

        double _outTestNormality_p;

        /// <summary>
        ///     p-Value for the Anderson-Darling value for Normality test
        /// </summary>
        public double OutTestNormality_p
        {
            get
            {
                return NoBigNumbers(_outTestNormality_p);
            }

            private set
            {
                if (!IsFrozen)
                    _outTestNormality_p = value;
            }
        }

        string _outTestNormalityRating = "";

        public string OutTestNormalityRating
        {
            get { return _outTestNormalityRating; }
            set { if (!IsFrozen) _outTestNormalityRating = value; }
        }
        
        /// <summary>
        ///     POD confidence level that is being calculated (95 in a90/95)
        /// </summary>
        public double InPODConfidence
        {
            get
            {
                return _inPODConfidence;
            }

            set
            {
                if (!IsFrozen)
                    _inPODConfidence = value;
            }
        }

        /// <summary>
        ///     POD % we are interested in (90 in a90/95)
        /// </summary>
        public double InPODLevel
        {
            get
            {
                return _inPODLevel;
            }

            set
            {
                if (!IsFrozen)
                    _inPODLevel = value;
            }
        }

        /// <summary>
        ///     Holds report that is generated based on the analysis settings and data
        /// </summary>
        //public Report Report
        //{
        //    get
        //    {
        //        return _report;
        //    }

        //    set
        //    {
        //        if (!IsFrozen)
        //            _report = value;
        //    }
        //}

        /// <summary>
        ///     The decision threshold for the analysis
        /// </summary>
        public double InResponseDecision
        {
            get
            {
                return _inResponseDecision;
            }

            set
            {
                if (!IsFrozen)
                    _inResponseDecision = value;
            }
        }

        /// <summary>
        ///     Increment to use when creating POD threshold graph
        /// </summary>
        public int InResponseDecisionIncCount
        {
            get
            {
                return _inResponseDecisionIncCount;
            }

            set
            {
                if (!IsFrozen)
                    _inResponseDecisionIncCount = value;
            }
        }

        /// <summary>
        ///     The maximum value of the range of threshold decisions to consider for the POD threshold graph
        /// </summary>
        public double InResponseDecisionMax
        {
            get
            {
                return _inResponseDecisionMax;
            }

            set
            {
                if (!IsFrozen)
                    _inResponseDecisionMax = value;
            }
        }

        /// <summary>
        ///     The minimum value of the range of threshold decisions to consider for the POD threshold graph
        /// </summary>
        public double InResponseDecisionMin
        {
            get
            {
                return _inResponseDecisionMin;
            }

            set
            {
                if (!IsFrozen)
                    _inResponseDecisionMin = value;
            }
        }

        /// <summary>
        ///     Maximum response value to use when performing calculations. (censor right)
        /// </summary>
        public double InResponseMax
        {
            get
            {
                return _inResponseMax;
            }

            set
            {
                if (!IsFrozen)
                    _inResponseMax = value;
            }
        }

        /// <summary>
        ///     Minimum response value to use when performing calculations. (censor left)
        /// </summary>
        public double InResponseMin
        {
            get
            {
                return _inResponseMin;
            }

            set
            {
                if (!IsFrozen)
                    _inResponseMin = value;
            }
        }

        /// <summary>
        ///     Name of the response values.
        /// </summary>
        public List<string> ResponseNames
        {
            get
            {
                return Data.AvailableResponseNames;
            }
        }

        /// <summary>
        ///     Unit response values are measured in.
        /// </summary>
        public List<string> ResponseUnits
        {
            get
            {
                return Data.AvailableResponseUnits;
            }
        }

        /// <summary>
        ///     the time and date the analysis was performed
        /// </summary>
        public DateTime AnalysisDate
        {
            get { return _analysisDate; }
            set { if (!IsFrozen) _analysisDate = value; }
        }

        public override string AdditionalWorksheet1Name
        {
            get
            {
                return Data.AdditionalWorksheet1Name;
            }
        }

        #endregion

        #region Methods

        private void SetupAnalysisLauncher()
        {
            analysisLauncher = new BackgroundWorker();

            analysisLauncher.WorkerSupportsCancellation = true;
            analysisLauncher.WorkerReportsProgress = true;

            analysisLauncher.DoWork += new DoWorkEventHandler(Background_StartAnalysis);
            analysisLauncher.ProgressChanged += new ProgressChangedEventHandler(Background_AnalysisProgressChanged);
            analysisLauncher.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Background_FinishedAnalysis);

            
        }

        public TestRating GetTestRatingFromLabel(string myLabel)
        {
            return TestRatingLabels.ValueFromLabel(myLabel);
        }

        private void Background_StartAnalysis(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            Stopwatch watch = new Stopwatch();

            watch.Start();

            IsBusy = true;

            try
            {
                if (Data.DataType == AnalysisDataTypeEnum.HitMiss)
                    _podDoc.OnAnalysis();
                else
                    _podDoc.OnFullAnalysis();
            }
            catch (Exception exp)
            {
                var moreInfo = string.Empty;

                try
                {
                    //essageBox.Show("Analysis Error:" + Environment.NewLine + Environment.NewLine + exp.Message);
                    var fullString = exp.ToString();
                    var lineIndex = fullString.IndexOf(".py:line");

                    var fileString = fullString.Substring(lineIndex - 100, 100);
                    var fileIndex = 100 - fileString.LastIndexOf("\\");
                    var endLineIndex = fullString.IndexOf(Environment.NewLine, lineIndex);
                    var startIndex = lineIndex - fileIndex + 1;
                    var endLength = endLineIndex - startIndex;

                    var pathString = fullString.Substring(0, lineIndex);
                    var fileStart = pathString.LastIndexOf(" in ") + 3;
                    var fullFileString = fullString.Substring(fileStart, endLineIndex - fileStart + 1);
                    var fileEnd = fullFileString.LastIndexOf(":") + fileStart;

                    var filePath = fullString.Substring(fileStart, fileEnd - fileStart);

                    var numberStart = fileEnd + 5;

                    var lineNumberStr = fullString.Substring(numberStart, endLineIndex - numberStart);
                    if (Int32.TryParse(lineNumberStr, out int lineNumber) && File.Exists(filePath))
                    {
                        var lines = File.ReadAllLines(filePath);

                        var line = string.Empty;

                        while (!line.TrimStart().StartsWith("def"))
                        {
                            line = lines[lineNumber];
                            lineNumber--;
                        }

                        moreInfo = "Error Occurs In Function " + line.Trim() + " starting at " + lineNumber.ToString() + " in " + Path.GetFileName(filePath);

                    }
                    else
                    {
                        moreInfo = "Error Occurs In Function That Ends At: " + fullString.Substring(startIndex, endLength);
                    }

                    moreInfo = Environment.NewLine + Environment.NewLine + moreInfo;
                }
                catch(Exception exp2)
                {
                    moreInfo = string.Empty;
                }


                _python.AddErrorText("Analysis Error: " + exp.Message);

                //_python.ProgressText = exp.Message;
            }
            finally
            {

            }

            watch.Stop();            
        }

        private void Background_AnalysisProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _python.ProgressText = (e.ProgressPercentage.ToString() + "%");
        }

        private void Background_FinishedAnalysis(object sender, RunWorkerCompletedEventArgs e)
        {
            while (analysisLauncher.IsBusy == true);

            CopyOutputFromPython();

            if ((e.Cancelled == true))
            {
                _python.ProgressText = "Canceled!";
            }

            else if (!(e.Error == null))
            {
                _python.ProgressText = ("Error: " + e.Error.Message);
            }

            else
            {
                _python.ProgressText = "Done!";
            }

            RaiseAnalysisDone();
            _python.NotifyFinishAnalysis();

            stillRunningAnalysis = false;

            analysisLauncher.Dispose();

            _analysisDate = DateTime.Now;

            IsBusy = false;
        }

        private void CopyOutputFromPython()
        {
            if (_podDoc != null && _podDoc.OnNewProgress != null && _podDoc.OnNewStatus != null)
                _podDoc.DeregisterUpdateProgressEvent(this);

            _python.OutputWriter.StringWritten -= OutputWriter_StringWritten;
            _python.ErrorWriter.StringWritten -= ErrorWriter_StringWritten;

            var watch = new Stopwatch();

            watch.Start();

            OutModelIntercept = _podDoc.GetModelIntercept();
            OutModelInterceptStdError = _podDoc.GetModelInterceptError();
            OutModelSlope = _podDoc.GetModelSlope();
            OutModelSlopeStdError = _podDoc.GetModelSlopeError();
            OutModelResidualError = _podDoc.GetModelResidual();
            OutModelResidualErrorStdError = _podDoc.GetModelResidualError();
            OutRepeatabilityError = _podDoc.GetRepeatabilityError();
            OutTestNormality_p = _podDoc.GetNormality_p();
            OutTestNormality = _podDoc.GetNormality();
            OutTestNormalityRating = _podDoc.GetNormalityRating();
            OutTestLackOfFit_p = _podDoc.GetLackOfFit_p();
            OutTestLackOfFit = _podDoc.GetLackOfFit();
            OutTestLackOfFitRating = _podDoc.GetLackOfFitRating();
            OutTestEqualVariance_p = _podDoc.GetEqualVariance_p();
            OutTestEqualVariance = _podDoc.GetEqualVariance();
            OutTestEqualVarianceRating = _podDoc.GetEqualVarianceRating();
            OutResponseDecisionPODSigma = _podDoc.GetPODSigma();
            OutResponseDecisionPODA50Value = _podDoc.GetPODFlaw50();
            OutResponseDecisionPODLevelValue = _podDoc.GetPODFlawLevel();
            OutResponseDecisionPODConfidenceValue = _podDoc.GetPODFlawConfidence();
            OutTestLackOfFitDegreesFreedom = Convert.ToInt32(_podDoc.GetLackOfDegreesFreedom());
            OutTestLackOfFitCalculated = _podDoc.GetLackOfFitCalculated();

            if (AnalysisDataType == AnalysisDataTypeEnum.HitMiss)
            {
                List<double> covMatrix = _python.PythonToDotNetList(_podDoc.GetPFEstimatedCovarianceMatrix());

                OutPFCovarianceV11 = covMatrix[0];
                OutPFCovarianceV12 = covMatrix[1];
                OutPFCovarianceV22 = covMatrix[2];

                OutPODMu = _podDoc.GetPODMu();
                OutPODSigma = _podDoc.GetPODSigma();
            }

            watch.Stop();

            var inputTime = watch.ElapsedMilliseconds;

            //MessageBox.Show(watch.ElapsedMilliseconds.ToString());

            watch.Restart();

            Data.UpdateOutput();

            watch.Stop();

            var outputTime = watch.ElapsedMilliseconds;
            
            //MessageBox.Show("INPUTS: " + inputTime + " DATA: " + outputTime);

            Data.ResponseLeft = InResponseMin;
            Data.ResponseRight = InResponseMax;
        }

        public void UpdateProgress(Object sender, int myProgressOutOf100)
        {
            _python.ProgressOutOf100 = myProgressOutOf100;
        }

        public void UpdateStatus(Object sender, string myCurrentStatus)
        {
            _python.ProgressText = myCurrentStatus;
        }

        public void AddError(Object sender, string myNewError)
        {
            _python.AddErrorText(myNewError);
        }

        public void ClearErrors(Object sender)
        {
            _python.ClearErrorText();
        }

        private void RaiseAnalysisDone()
        {
            if (AnalysisDone != null)
                AnalysisDone.Invoke(this, null);
        }

        /// <summary>
        ///     Archive output so it can be used when generating charts after analysis is finished.
        /// </summary>
        /// <param name="myArchiveName">the name used to store the analysis under</param>
        /// <returns>does the name already exist?</returns>
        public bool ArchiveOutput(string myArchiveName)
        {
            bool nameIsNew = false;

            foreach (Analysis analysis in _archivedOutputs)
            {
                if (myArchiveName == analysis.Name)
                {
                    nameIsNew = true;
                }
            }

            if (nameIsNew == false && _archivedOutputs != null)
            {
                _archivedOutputs.Add(CreateDuplicate());
                _archivedOutputs[_archivedOutputs.Count - 1].Name = myArchiveName;
            }

            return nameIsNew;
        }

        public void CalculateInitialValuesWithNewInfo(SourceInfo myInfo)
        {
            if (HasBeenInitialized == false && Data.RowCount > 0)
            {
                UpdateXAxisTransform();

                Data.UpdateSourceFromInfos(myInfo);

                //get the response range for the responses included in the analysis
                UpdateRangesFromData();

                HasBeenInitialized = true;

            }
        }

        private void UpdateXAxisTransform()
        {
            if (Data.DataType == AnalysisDataTypeEnum.HitMiss)
            {
                InFlawTransform = TransformTypeEnum.Log;
                Data.FlawTransform = InFlawTransform;

                if(_python != null)
                    _podDoc.a_transform = _python.TransformEnumToInt(Data.FlawTransform);
            }
        }

        public void ForceUpdateInputsFromData(bool recheckAnalysisType = false, AnalysisDataTypeEnum forcedType = AnalysisDataTypeEnum.Undefined)
        {
            double min;
            double max;

            if (!UserSuppliedRanges)
            {
                GetBufferedMinMax(Data.ActivatedResponses, out min, out max);

                InResponseMin = Data.InvertTransformedResponse(min);
                InResponseMax = Data.InvertTransformedResponse(max);

                //take a guess at 15% of the total response range
                InResponseDecision = Data.InvertTransformedResponse(min + ((max - min) * .15));
            }

            //default to looking at 10% (+-5%) around the decision guess
            InResponseDecisionMin = .05;
            InResponseDecisionMax = .05;
            InResponseDecisionIncCount = 30;

            if (!UserSuppliedRanges)
            {
                GetBufferedMinMax(Data.ActivatedFlaws, out min, out max);

                InFlawMin = Data.InvertTransformedFlaw(min);
                InFlawMax = Data.InvertTransformedFlaw(max);
            }

            //analysis data will figure out what kind of analysis it is
            if (recheckAnalysisType)
            {
                AnalysisDataType = _data.RecheckAnalysisType(forcedType);
            }
            else 
                AnalysisDataType = _data.DataType;
        }

        /// <summary>
        ///     Estimate initial values of parameters based on new data.
        /// </summary>
        public void CalculateInitialValuesWithNewData()
        {
            if (_python != null && HasBeenInitialized == false)
            {
                if (AnalysisDataType == AnalysisDataTypeEnum.HitMiss)
                {
                    InFlawTransform = TransformTypeEnum.Log;
                    Data.FlawTransform = InFlawTransform;
                    _podDoc.a_transform = _python.TransformEnumToInt(Data.FlawTransform);
                }

                ForceUpdateInputsFromData();

                _initialResponseMinGuess = InResponseMin;
                _initialResponseMaxGuess = InResponseMax;
                _initialResponseDecision = InResponseDecision;

                HasBeenInitialized = true;
            }

            //force data table to be transformed to current transform type
            //XAxisTransformIn = _data.FlawTransform;// _xAxisTransformIn;
            //YAxisTransformIn = _data.ResponseTransform;// _yAxisTransformIn;
        }

        public bool UsingInitialGuesses
        {
            get
            {
                return (_initialResponseDecision == InResponseDecision && _initialResponseMaxGuess == InResponseMax && _initialResponseMinGuess == InResponseMin);
            }
        }

        public static void GetBufferedMinMax(DataTable myTable, out double myMin, out double myMax)
        {
            GetMinMax(myTable, out myMin, out myMax);

            /*double bufMax = myMax;
            double range = myMax - myMin;
            double buffer = range * .05;

            double bufMin = myMin;

            bufMax += buffer;
            bufMin -= buffer;

            myMin = bufMin;
            myMax = bufMax;*/

            double range = Math.Abs(myMax - myMin);
            double logRange = Math.Floor(Math.Log10(range)) - 1;
            double buffer = 2.5 / 100.0 * range;
            double max = myMax + buffer;
            double min = myMin - buffer;
            double correctedRange = Math.Pow(10.0, logRange);

            myMax = Math.Ceiling(max / correctedRange) * correctedRange;
            myMin = Math.Floor(min / correctedRange) * correctedRange;
        }

        /// <summary>
        ///     Create a new analysis with duplicated data and settings.
        /// </summary>
        /// <returns></returns>
        public Analysis CreateDuplicate()
        {
            var analysis = (Analysis)MemberwiseClone();

            analysis.AnalysisDone = null;
            analysis.EventsNeedToBeCleared = null;
            analysis.ProjectInfoNeeded = null;
            analysis.ExportProject = null;
            analysis.CreatedAnalysis = null;
            analysis.JumpTo = null;
            analysis.StepNext = null;
            analysis.StepPrevious = null;
            analysis.WizardFinished = null;

            analysis.analysisLauncher = null;

            //archived analysis can't archive more analysis
            analysis._archivedOutputs = null;

            //create duplicates of any objects associated with the analysis
            analysis._data = _data.CreateDuplicate();
            analysis._python = null;
            analysis._podDoc = null;
            //analysis.Report = Report.CreateDuplicate();


            return analysis;
        }

        /// <summary>
        ///     Gets the range of values for a given table.
        /// </summary>
        /// <param name="myTable">the table to query</param>
        /// <param name="myMin">minimum value in the table</param>
        /// <param name="myMax">maximum value in the table</param>
        private static void GetMinMax(DataTable myTable, out double myMin, out double myMax)
        {
            myMin = double.MaxValue;
            myMax = double.MinValue;

            foreach (DataColumn column in myTable.Columns)
            {
                //string columnName = column.ColumnName;
                //string compute = "MIN(" + "[" + column.ColumnName + "]" + ")";

                //var value = (double)myTable.Compute(compute, "");
                Compute.MinMax(myTable, column, ref myMin, ref myMax);


                /*if (value < myMin)
                    myMin = value;

                compute = "MAX(" + "[" + column.ColumnName + "]" + ")";

                value = (double)myTable.Compute(compute, "");

                if (value > myMax)
                    myMax = value;*/
            }

            Compute.SanityCheck(ref myMin, ref myMax);
        }

        



        /// <summary>
        ///     Initialize the Analysis with default values.
        /// </summary>
        private void Initialize()
        {
            //Report = new Report();
            _data = new AnalysisData();

            InPODConfidence = 95.0;
            InPODLevel = 90.0;


            InFlawMin = double.MinValue;
            InFlawMax = double.MaxValue;
            InFlawCalcMin = 0.0;
            InFlawCalcMax = 1.0;
            InResponseMin = double.MinValue;
            InResponseMax = double.MaxValue;

            InFlawTransform = TransformTypeEnum.Linear;
            InResponseTransform = TransformTypeEnum.Linear;

            InResponseDecisionMin = double.MinValue;
            InResponseDecisionMax = double.MaxValue;

            AnalysisType = AnalysisTypeEnum.Undefined;
            AnalysisDataType = AnalysisDataTypeEnum.Undefined;
            ObjectType = PODObjectTypeEnum.Analysis;

            _archivedOutputs = new List<Analysis>();

            _activeAnalysis = this;
            _activeAnalysisIndex = -1;
        }

        /// <summary>
        ///     Run analysis with current settings and data. Output data will be overwritten.
        /// </summary>
        public void RunAnalysis()
        {
            if (IsFrozen)
                return;

            if (analysisLauncher == null)
            {
                SetupAnalysisLauncher();
            }

            if (analysisLauncher.IsBusy == false && stillRunningAnalysis == false)
            {
                stillRunningAnalysis = true;

                CopyInputToPython();

                _python.FailedRunCount = 0;

                analysisLauncher.RunWorkerAsync();
            }
            else
            {
                _python.ProgressText = "Busy running current analysis.";
                //MessageBox.Show("Analysis code is still running.");
                //analysisLauncher.CancelAsync();

                _python.FailedRunCount++;

                if(_python.FailedRunCount >= 4)
                {
                    ShowPythonCodeStuckMessage();                    
                }
            }
        }

        private void ShowPythonCodeStuckMessage()
        {
            var result = MessageBox.Show("POD v4 analysis code has not terminated.  " +
                            "Please contact UDRI for further assistance.  " +
                            "If possible make a copy of log.txt to send to them." + Environment.NewLine + Environment.NewLine +
                            "Would you like to open the folder with the log file?", "POD v4", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = "C:\\UDRI\\PODv4\\Logs",
                    UseShellExecute = true,
                    Verb = "open"
                });
            }

            _python.FailedRunCount = 0;
        }

        private void CopyInputToPython()
        {
            if (_podDoc != null && _podDoc.OnNewProgress != null && _podDoc.OnNewStatus != null)
                _podDoc.RegisterUpdateProgressEvent(this);

            _python.ProgressText = "Starting Analysis...";

            _python.CurrentAnalysisName = Name;

            _python.OutputWriter.StringWritten += OutputWriter_StringWritten;
            _python.ErrorWriter.StringWritten += ErrorWriter_StringWritten;

            //TODO: Add code here to call python analysis code with the current analysis data and settings
            //only use the column specified by InAnalysisFlawColumnName for the x-axis of the calculations
            _podDoc.analysis = _python.AnalysisDataTypeEnumToInt(AnalysisDataType);
            //_podDoc.ahat_transform = 0;

            //flip them so the calculation code still work properly
            if (InResponseMin >= InResponseMax)
            {
                double temp = InResponseMin;
                InResponseMin = InResponseMax;
                InResponseMax = temp;
            }

            UpdatePythonTransforms();

            if (AnalysisDataType == AnalysisDataTypeEnum.HitMiss)
                _podDoc.model = _python.PFModelEnumToInt(InHitMissModel);

            _podDoc.name = Name;

            Data.UpdateData();

            //_podDoc.SetFlawMin(InFlawMin);
            //_podDoc.SetFlawMax(InFlawMax);
            _podDoc.SetCalcMin(InFlawCalcMin);
            _podDoc.SetCalcMax(InFlawCalcMax);
            _podDoc.SetResponseMin(InResponseMin);
            _podDoc.SetResponseMax(InResponseMax);
            _podDoc.SetPODThreshold(InResponseDecision);
            _podDoc.SetPODLevel(.90);
            _podDoc.SetPODConfidence(.95);

            List<double> thresholds = new List<double>();

            double value = 0.0;
            double inc = (InResponseDecisionMax - InResponseDecisionMin) / (InResponseDecisionIncCount);


            for (int i = 0; i <= InResponseDecisionIncCount; i++)
            {
                value = InResponseDecisionMin + (inc * i);

                thresholds.Add(value);
            }

            _podDoc.SetDecisionThresholds(thresholds);
        }

        public void UpdatePythonTransforms()
        {
            Data.FlawTransform = InFlawTransform;
            Data.ResponseTransform = InResponseTransform;

            _podDoc.a_transform = _python.TransformEnumToInt(Data.FlawTransform);
            _podDoc.ahat_transform = _python.TransformEnumToInt(Data.ResponseTransform);
        }

        private void OutputWriter_StringWritten(object sender, MyEvtArgs<string> e)
        {
            _python.AddErrorText(e.Value);
        }

        private void ErrorWriter_StringWritten(object sender, MyEvtArgs<string> e)
        {
            _python.AddErrorText(e.Value);
        }

        /// <summary>
        ///     Sets a new data source and estimates initial setting values. All columns are copied over.
        /// </summary>
        /// <param name="mySource">the data source to copy from</param>
        public void SetDataSource(DataSource mySource)
        {
            SourceName = mySource.SourceName;

            _data.SetSource(mySource);

            CalculateInitialValuesWithNewData();
        }

        /// <summary>
        ///     Sets a new data source and estimates initial setting values with only selected columns copied over.
        /// </summary>
        /// <param name="mySource">the data source to copy from</param>
        /// <param name="myFlaws">the list flaw column names to copy</param>
        /// <param name="myMetaDatas">list metadata column names to copy</param>
        /// <param name="myResponses">list of response data column names to copy</param>
        /// <param name="mySpecIDs">list of specimen ID columns to copy</param>
        public void SetDataSource(DataSource mySource, List<string> myFlaws, List<string> myMetaDatas,
            List<string> myResponses, List<string> mySpecIDs)
        {
            SourceName = mySource.SourceName;

            _data.SetSource(mySource, myFlaws, myMetaDatas, myResponses, mySpecIDs);

            CalculateInitialValuesWithNewData();
        }

        /// <summary>
        ///     Switches the active analysis to an archived analysis.
        /// </summary>
        /// <param name="myName">name of the archived analysis</param>
        /// <returns>Did it switch to the archived analysis specified?</returns>
        public bool SwitchToArchivedOutput(string myName)
        {
            //assume it will not be found
            int index = -1;

            //find output with the same name
            for (int i = 0; i < _archivedOutputs.Count; i++)
            {
                if (_archivedOutputs[i].Name == myName)
                {
                    index = i;
                }
            }

            return SwitchToArchivedOutput(index);
        }

        /// <summary>
        ///     Switches the active analysis to an archived analysis.
        /// </summary>
        /// <param name="myIndex">index of archived analysis to switch to</param>
        /// <returns>Did it switch to the archived analysis specified?</returns>
        public bool SwitchToArchivedOutput(int myIndex)
        {
            //assume it will not be allowed
            bool valid = false;

            if (myIndex >= 0 && myIndex < _archivedOutputs.Count)
            {
                valid = true;
                _activeAnalysisIndex = myIndex;
            }

            return valid;
        }

        #endregion

        public override void SetPythonEngine(IPy4C myPy)
        {
            _python = myPy;

            if (_podDoc == null)
                _podDoc = _python.CPodDoc(Name);         

            _data.SetPythonEngine(_python, Name);
        }

        #region Event Handling

        #endregion

        public bool AutoOpen { get; set; }

        public void WriteToExcel(ExcelExport myWriter, bool myPartOfProject = true, DataTable table = null)
        {
            string temp = WorksheetName;


            if (!myPartOfProject)
            {
                WorksheetName = "";
            }

            if(table != null)
            {
                var rowIndex = 1;
                var colIndex = 1;
                myWriter.Workbook.AddWorksheet(SourceName);
                myWriter.WriteTableToExcel(table, ref rowIndex, ref colIndex);
            }

            WriteInfoSheet(myWriter);
            WriteResultsSheet(myWriter);

            Data.WriteToExcel(myWriter, Name, WorksheetName, myPartOfProject);

            if (!myPartOfProject)
            {
                myWriter.RemoveDefaultSheet();
            }

            myWriter.Workbook.SelectWorksheet("Info");

            WorksheetName = temp;
        }

        public void WriteQuickAnalysis(ExcelExport myWriter, DataTable myInputTable, string myOperator, string mySpecimentSet, string mySpecUnits, double mySpecMin, double mySpecMax,
                                       string myInstrument = "", string myInstUnits = "", double myInstMin = 0.0, double myInstMax = 1.0)
        {
            myWriter.Workbook.AddWorksheet((WorksheetName + " Input").Trim());

            int rowIndex = 1;
            int colIndex = 1;

            myWriter.SetCellValue(rowIndex++, colIndex, "Quick Analysis");
            myWriter.SetCellValue(rowIndex++, colIndex, "Analysis Type");

            myWriter.SetCellValue(rowIndex++, colIndex, "Operator");
            myWriter.SetCellValue(rowIndex++, colIndex, "Specimen Set");
            myWriter.SetCellValue(rowIndex++, colIndex, "Specimen Set Units");

            if (AnalysisDataType == AnalysisDataTypeEnum.AHat)
            {
                myWriter.SetCellValue(rowIndex++, colIndex, "Instrument");
                myWriter.SetCellValue(rowIndex++, colIndex, "Instrument Units");
            }

            rowIndex++;

            myWriter.SetCellValue(rowIndex++, colIndex, "FLAW:");
            myWriter.SetCellValue(rowIndex++, colIndex, "Flaw Range Min");
            myWriter.SetCellValue(rowIndex++, colIndex, "Flaw Range Max");

            rowIndex++;

            if (AnalysisDataType == AnalysisDataTypeEnum.AHat)
            {
                myWriter.SetCellValue(rowIndex++, colIndex, "RESPONSE:");
                myWriter.SetCellValue(rowIndex++, colIndex, "Response Min");
                myWriter.SetCellValue(rowIndex++, colIndex, "Response Max");

                rowIndex++;
            }


            //write table here
            myWriter.WriteTableToExcel(myInputTable, ref rowIndex, ref colIndex);

            rowIndex = 3;
            colIndex = 2;            

            myWriter.SetCellValue(rowIndex++, colIndex, myOperator);
            myWriter.SetCellValue(rowIndex++, colIndex, mySpecimentSet);
            myWriter.SetCellValue(rowIndex++, colIndex, mySpecUnits);

            if (AnalysisDataType == AnalysisDataTypeEnum.AHat)
            {                
                myWriter.SetCellValue(rowIndex++, colIndex, myInstrument);
                myWriter.SetCellValue(rowIndex++, colIndex, myInstUnits);
            }

            rowIndex++;

            rowIndex++;
            myWriter.SetCellValue(rowIndex++, colIndex, mySpecMin);
            myWriter.SetCellValue(rowIndex++, colIndex, mySpecMax);

            rowIndex++;

            if (AnalysisDataType == AnalysisDataTypeEnum.AHat)
            {
                rowIndex++;
                myWriter.SetCellValue(rowIndex++, colIndex, myInstMin);
                myWriter.SetCellValue(rowIndex++, colIndex, myInstMax);

                rowIndex++;
            }

            myWriter.Workbook.AutoFitColumn(1, 3);

            myWriter.SetCellValue(1, 2, "POD Output not exported since this was only intended to aid the operator during data collection.");
            myWriter.SetCellValue(2, 2, Data.DataType.ToString());

            myWriter.RemoveDefaultSheet();
        }

        private void WriteResultsSheet(ExcelExport myWriter)
        {
            int rowIndex = 1;
            int colIndex = 1;

            myWriter.Workbook.AddWorksheet((WorksheetName + " Results").Trim());

            myWriter.SetCellValue(rowIndex++, colIndex, "Analysis Name");

            rowIndex++;

            myWriter.SetCellValue(rowIndex++, colIndex, "FLAW:");
            myWriter.SetCellValue(rowIndex++, colIndex, "Flaw Range Min");
            myWriter.SetCellValue(rowIndex++, colIndex, "Flaw Range Max");

            if (AnalysisDataType == AnalysisDataTypeEnum.AHat)
            {
                myWriter.SetCellValue(rowIndex++, colIndex, "Flaw Count");
            }
            else if(AnalysisDataType == AnalysisDataTypeEnum.HitMiss)
            {
                myWriter.SetCellValue(rowIndex++, colIndex, "Flaw Count Unique");
                myWriter.SetCellValue(rowIndex++, colIndex, "Flaw Count Total");
            }

            rowIndex++;

            if (AnalysisDataType == AnalysisDataTypeEnum.AHat)
            {
                myWriter.SetCellValue(rowIndex++, colIndex, "RESPONSE:");
                myWriter.SetCellValue(rowIndex++, colIndex, "Response Min");
                myWriter.SetCellValue(rowIndex++, colIndex, "Response Max");
                myWriter.SetCellValue(rowIndex++, colIndex, "Response Partial Below Min Count");
                myWriter.SetCellValue(rowIndex++, colIndex, "Response Partial Above Max Count");
                myWriter.SetCellValue(rowIndex++, colIndex, "Response Complete Below Min Count");
                myWriter.SetCellValue(rowIndex++, colIndex, "Response Complete Above Max Count");
                myWriter.SetCellValue(rowIndex++, colIndex, "Response Between Count");

                rowIndex++;
            }

            myWriter.SetCellValue(rowIndex++, colIndex, "ANALYSIS MODEL:");
            

            if (AnalysisDataType == AnalysisDataTypeEnum.AHat)
            {
                myWriter.SetCellValue(rowIndex-1, colIndex+2, "Standard Error");
                myWriter.SetCellValue(rowIndex++, colIndex, "Intercept");
                myWriter.SetCellValue(rowIndex++, colIndex, "Slope");                
                myWriter.SetCellValue(rowIndex++, colIndex, "Residual Error");
                myWriter.SetCellValue(rowIndex++, colIndex, "Repeatability Error");
                myWriter.SetCellValue(rowIndex++, colIndex, "Mu");
                myWriter.SetCellValue(rowIndex++, colIndex, "Sigma");
            }
            else if (AnalysisDataType == AnalysisDataTypeEnum.HitMiss)
            {
                myWriter.SetCellValue(rowIndex++, colIndex, "Mu");
                myWriter.SetCellValue(rowIndex++, colIndex, "Sigma");
            }

            if(AnalysisDataType == AnalysisDataTypeEnum.HitMiss)
            {
                rowIndex++;

                myWriter.SetCellValue(rowIndex++, colIndex, "ESTIMATED COVARIANCE MATRIX:");

                myWriter.SetCellValue(rowIndex++, colIndex, "V11");
                myWriter.SetCellValue(rowIndex++, colIndex, "V12");
                myWriter.SetCellValue(rowIndex++, colIndex, "V22");
            }


            rowIndex++;

            if (AnalysisDataType == AnalysisDataTypeEnum.AHat)
            {
                myWriter.SetCellValue(rowIndex++, colIndex, "ANALYSIS MODEL TESTS:");
                myWriter.SetCellValue(rowIndex++, colIndex, "Normality (Anderson Darling)");
                myWriter.SetCellValue(rowIndex++, colIndex, "Equal Variance (Bartlett)");
                myWriter.SetCellValue(rowIndex++, colIndex, "Lack of Fit (Pure Error, df=" + _outTestLackOfFitDegreesFreedom.ToString() + ")");

                rowIndex++;
            }

            

            myWriter.SetCellValue(rowIndex++, colIndex, "POD PERCENTILE ESTIMATES:");
            myWriter.SetCellValue(rowIndex++, colIndex, "A50 Value");
            myWriter.SetCellValue(rowIndex++, colIndex, "Level Value (A" + InPODLevel + ")");
            myWriter.SetCellValue(rowIndex++, colIndex, "Confidence Value  (A" + InPODLevel + "_" + InPODConfidence + ")");
            
            rowIndex = 1;
            colIndex = 2;

            if (WorksheetName.Length > 0)
            {
                //myWriter.Workbook.MergeWorksheetCells(rowIndex, colIndex, rowIndex, colIndex + 1);
                myWriter.InsertReturnToTableOfContents(1, 1, WorksheetName);
                rowIndex++;
            }
            else
                rowIndex++;

            rowIndex++;

            //skip flaw label
            rowIndex++;
            myWriter.SetCellValue(rowIndex++, colIndex, Data.InvertTransformedFlaw(Data.FlawRangeMin));
            myWriter.SetCellValue(rowIndex++, colIndex, Data.InvertTransformedFlaw(Data.FlawRangeMax));

            if (AnalysisDataType == AnalysisDataTypeEnum.AHat)
            {
                myWriter.SetCellValue(rowIndex++, colIndex, Data.FlawCount);
            }
            else if (AnalysisDataType == AnalysisDataTypeEnum.HitMiss)
            {
                myWriter.SetCellValue(rowIndex++, colIndex, Data.FlawCountUnique);
                myWriter.SetCellValue(rowIndex++, colIndex, Data.FlawCount);
            }

            rowIndex++;

            if (AnalysisDataType == AnalysisDataTypeEnum.AHat)
            {
                //skip response label
                rowIndex++;
                myWriter.SetCellValue(rowIndex++, colIndex, InResponseMin);
                myWriter.SetCellValue(rowIndex++, colIndex, InResponseMax);
                myWriter.SetCellValue(rowIndex++, colIndex, Data.ResponsePartialBelowMinCount);
                myWriter.SetCellValue(rowIndex++, colIndex, Data.ResponsePartialAboveMaxCount);
                myWriter.SetCellValue(rowIndex++, colIndex, Data.ResponseCompleteBelowMinCount);
                myWriter.SetCellValue(rowIndex++, colIndex, Data.ResponseCompleteAboveMaxCount);
                myWriter.SetCellValue(rowIndex++, colIndex, Data.ResponseBetweenCount);

                rowIndex++;
            }

            //skip analysis model label
            rowIndex++;
            if (AnalysisDataType == AnalysisDataTypeEnum.AHat)
            {
                myWriter.SetCellValue(rowIndex, colIndex, OutModelIntercept);
                myWriter.SetCellValue(rowIndex++, colIndex + 1, OutModelInterceptStdError);
                myWriter.SetCellValue(rowIndex, colIndex, OutModelSlope);
                myWriter.SetCellValue(rowIndex++, colIndex+1, OutModelSlopeStdError);                
                myWriter.SetCellValue(rowIndex, colIndex, OutModelResidualError);
                myWriter.SetCellValue(rowIndex++, colIndex+1, OutModelResidualErrorStdError);
                myWriter.SetCellValue(rowIndex++, colIndex, OutRepeatabilityError);
                myWriter.SetCellValue(rowIndex++, colIndex, OutResponseDecisionPODA50Value);
                myWriter.SetCellValue(rowIndex++, colIndex, OutResponseDecisionPODSigma);

            }
            else if (AnalysisDataType == AnalysisDataTypeEnum.HitMiss)
            {
                myWriter.SetCellValue(rowIndex++, colIndex, OutPODMu);
                myWriter.SetCellValue(rowIndex++, colIndex, OutPODSigma);
            }

            rowIndex++;

            if (AnalysisDataType == AnalysisDataTypeEnum.HitMiss)
            {
                //skip covariance label                
                rowIndex++;
                myWriter.SetCellValue(rowIndex++, colIndex, OutPFCovarianceV11);
                myWriter.SetCellValue(rowIndex++, colIndex, OutPFCovarianceV12);
                myWriter.SetCellValue(rowIndex++, colIndex, OutPFCovarianceV22);

                rowIndex++;
            }            

            if (AnalysisDataType == AnalysisDataTypeEnum.AHat)
            {
                //write assumption hypothesis test label

                myWriter.Workbook.AutoFitColumn(colIndex + 1, colIndex + 1);

                myWriter.SetCellValue(rowIndex, colIndex, "Test Statistic");
                myWriter.SetCellValue(rowIndex, colIndex + 1, "Assumption Compatibility");
                myWriter.SetCellValue(rowIndex, colIndex + 2, "P-Value");

                myWriter.SetCellTextWrapped(rowIndex, colIndex + 1, true);

                myWriter.SetRowSize(rowIndex, 2.0);

                //skip analysis model test label
                rowIndex++;
                myWriter.SetCellValue(rowIndex, colIndex, OutTestNormality);
                myWriter.SetCellValue(rowIndex, colIndex + 1, OutTestNormalityRating);
                myWriter.SetCellValue(rowIndex++, colIndex + 2, OutTestNormality_p);
                myWriter.SetCellValue(rowIndex, colIndex, OutTestEqualVariance);
                myWriter.SetCellValue(rowIndex, colIndex + 1, OutTestEqualVarianceRating);
                myWriter.SetCellValue(rowIndex++, colIndex + 2, OutTestEqualVariance_p);
                myWriter.SetCellValue(rowIndex, colIndex, OutTestLackOfFit);
                myWriter.SetCellValue(rowIndex, colIndex + 1, OutTestLackOfFitRating);
                myWriter.SetCellValue(rowIndex++, colIndex + 2, OutTestLackOfFit_p);

                rowIndex++;
            }

            //skip POD model label
            rowIndex++;
            myWriter.SetCellValue(rowIndex++, colIndex, OutResponseDecisionPODA50Value);
            myWriter.SetCellValue(rowIndex++, colIndex, OutResponseDecisionPODLevelValue);
            myWriter.SetCellValue(rowIndex++, colIndex, OutResponseDecisionPODConfidenceValue);

            

            myWriter.Workbook.AutoFitColumn(1, 2);

            WriteAnalysisName(myWriter, WorksheetName.Length > 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myWriter"></param>
        private void WriteInfoSheet(ExcelExport myWriter)
        {
            myWriter.Workbook.AddWorksheet((WorksheetName + " Info").Trim());

            int rowIndex = 1;
            int colIndex = 1;

            myWriter.SetCellValue(rowIndex++, colIndex, "Analysis Name");

            rowIndex++;

            myWriter.SetCellValue(rowIndex++, colIndex, "FLAW:");
            myWriter.SetCellValue(rowIndex++, colIndex, "Flaw Name");
            myWriter.SetCellValue(rowIndex++, colIndex, "Flaw Unit");
            myWriter.SetCellValue(rowIndex++, colIndex, "Flaw Min");
            myWriter.SetCellValue(rowIndex++, colIndex, "Flaw Max");
            myWriter.SetCellValue(rowIndex++, colIndex, "Flaw Transform");

            rowIndex++;

            myWriter.SetCellValue(rowIndex++, colIndex, "RESPONSE:");
            myWriter.SetCellValue(rowIndex++, colIndex, "Response Names");
            
            if (AnalysisDataType == AnalysisDataTypeEnum.AHat)
            {
                myWriter.SetCellValue(rowIndex++, colIndex, "Response Units");
            }

            if (AnalysisDataType == AnalysisDataTypeEnum.HitMiss)
            {
                myWriter.SetCellValue(rowIndex++, colIndex, "Model");
            }

            if (AnalysisDataType == AnalysisDataTypeEnum.AHat)
            {
                myWriter.SetCellValue(rowIndex++, colIndex, "Response Min");
                myWriter.SetCellValue(rowIndex++, colIndex, "Response Max");
                myWriter.SetCellValue(rowIndex++, colIndex, "Response Transform");
            }

            rowIndex++;

            myWriter.SetCellValue(rowIndex++, colIndex, "ANALYSIS:");
            myWriter.SetCellValue(rowIndex++, colIndex, "Time Stamp");
            myWriter.SetCellValue(rowIndex++, colIndex, "Analysis Type");

            if (AnalysisDataType == AnalysisDataTypeEnum.AHat)
            {
                myWriter.SetCellValue(rowIndex++, colIndex, "POD Threshold");
            }

            myWriter.SetCellValue(rowIndex++, colIndex, "POD Level");
            myWriter.SetCellValue(rowIndex++, colIndex, "POD Confidence");

            //if (AnalysisDataType == AnalysisDataTypeEnum.AHat)
            //{
            //    myWriter.SetCellValue(rowIndex++, colIndex, "POD Threshold Range Min");
            //    myWriter.SetCellValue(rowIndex++, colIndex, "POD Threshold Range Max");
            //    myWriter.SetCellValue(rowIndex++, colIndex, "POD Threshold Range Increment Count");
            //}

            rowIndex = 1;
            colIndex = 2;

            if (WorksheetName.Length > 0)
            {
                //myWriter.Workbook.MergeWorksheetCells(rowIndex, colIndex, rowIndex, colIndex + 1);
                myWriter.InsertReturnToTableOfContents(1, 1, WorksheetName);
                rowIndex++;
            }
            else
                rowIndex++;

            rowIndex++;

            //skip flaw header
            rowIndex++;
            myWriter.SetCellValue(rowIndex++, colIndex, FlawName);
            myWriter.SetCellValue(rowIndex++, colIndex, FlawUnit);
            myWriter.SetCellValue(rowIndex++, colIndex, InFlawMin);
            myWriter.SetCellValue(rowIndex++, colIndex, InFlawMax);
            myWriter.SetCellValue(rowIndex++, colIndex, Data.FlawTransform.ToString());

            rowIndex++;

            //skip response header
            rowIndex++;

            //write all responses info
            var addTo = 0;


            foreach (var name in ResponseNames)
            {
                myWriter.SetCellValue(rowIndex, colIndex + addTo, name);
                
                if (AnalysisDataType == AnalysisDataTypeEnum.AHat)
                {
                    myWriter.SetCellValue(rowIndex + 1, colIndex + addTo, ResponseUnits[addTo]);
                }
                
                addTo++;
            }

            if (AnalysisDataType == AnalysisDataTypeEnum.AHat)
            {
                rowIndex = rowIndex + 2;
            }
            else
                rowIndex++;

            if (AnalysisDataType == AnalysisDataTypeEnum.HitMiss)
            {
                myWriter.SetCellValue(rowIndex++, colIndex, InHitMissModel.ToString());
            }

            if (AnalysisDataType == AnalysisDataTypeEnum.AHat)
            {
                myWriter.SetCellValue(rowIndex++, colIndex, InResponseMin);
                myWriter.SetCellValue(rowIndex++, colIndex, InResponseMax);
                myWriter.SetCellValue(rowIndex++, colIndex, Data.ResponseTransform.ToString());
            }

            rowIndex++;

            //skip analysis header
            rowIndex++;
            myWriter.SetCellValue(rowIndex++, colIndex, AnalysisDate);
            myWriter.SetCellValue(rowIndex++, colIndex, Data.DataType.ToString());

            if (AnalysisDataType == AnalysisDataTypeEnum.AHat)
            {
                myWriter.SetCellValue(rowIndex++, colIndex, InResponseDecision);
            }

            myWriter.SetCellValue(rowIndex++, colIndex, InPODLevel);
            myWriter.SetCellValue(rowIndex++, colIndex, InPODConfidence);

            //if (AnalysisDataType == AnalysisDataTypeEnum.AHat)
            //{
            //    myWriter.SetCellValue(rowIndex++, colIndex, InResponseDecisionMin);
            //    myWriter.SetCellValue(rowIndex++, colIndex, InResponseDecisionMax);
            //    myWriter.SetCellValue(rowIndex++, colIndex, InResponseDecisionIncCount);
            //}

            myWriter.Workbook.AutoFitColumn(1, 2);

            WriteAnalysisName(myWriter, WorksheetName.Length > 0);
        }

        private void WriteAnalysisName(ExcelExport myWriter, bool myPartOfProject)
        {
            //if (myPartOfProject)
            //{
            //    myWriter.SetCellValue(1, 3, Name);
            //}
            //else
            //{
            //    myWriter.SetCellValue(1, 2, Name);
            //}

            myWriter.SetCellValue(1, 2, Name);
        }

        public double OutPFCovarianceV11 { get; set; }

        public double OutPFCovarianceV12 { get; set; }

        public double OutPFCovarianceV22 { get; set; }

        /*public void InverseTransformXAxis(Controls.AHatVsAChart myChart)
        {
            double xMin = myChart.XAxis.Minimum;
            double xMax = myChart.XAxis.Maximum;
            double xIntv = myChart.XAxis.Interval;

            myChart.XAxis.Minimum = _podDoc.GetInvtransformedFlawValue(xMin);
            myChart.XAxis.Maximum = _podDoc.GetInvtransformedFlawValue(xMax);
            myChart.XAxis.Interval = _podDoc.GetInvtransformedFlawValue(xIntv);

            //myChart.OnlyGrowXAxis = true;
            //myChart.AutoBufferXRange();
        }*/

        public double OutPODMu { get; set; }

        public double OutPODSigma { get; set; }

        private TransformTypeEnum _xAxisTransformIn;

        public TransformTypeEnum InFlawTransform
        {
            get { return _xAxisTransformIn; }
            set
            {
                if (!IsFrozen)
                {
                    Data.MinFlaw = InFlawMin;
                    Data.MaxFlaw = InFlawMax;
                    _xAxisTransformIn = value;
                    Data.FlawTransform = _xAxisTransformIn;
                }
            }
        }

        

        private double _inFlawCalcMax = 0.0;

        public double InFlawCalcMax
        {
            get { return _inFlawCalcMax; }
            set { if(!IsFrozen) _inFlawCalcMax = InvertTransformValueForXAxis(value); }
        }
        private double _inFlawCalcMin = 0.0;
        private bool stillRunningAnalysis = false;

        public double InFlawCalcMin
        {
            get { return _inFlawCalcMin; }
            set { if (!IsFrozen) _inFlawCalcMin = InvertTransformValueForXAxis(value); }
        }

        private TransformTypeEnum _yAxisTransformIn;

        public TransformTypeEnum InResponseTransform
        {
            get { return _yAxisTransformIn; }
            set
            {
                if (!IsFrozen)
                {

                    Data.MinSignal = InResponseMin;
                    Data.MaxSignal = InResponseMax;
                    _yAxisTransformIn = value;
                    Data.ResponseTransform = _yAxisTransformIn;
                }
            }
        }

        public double TransformValueForXAxis(double myValue)
        {
            return Convert.ToDouble(TransformValueForXAxis(Convert.ToDecimal(myValue)));
        }

        public decimal TransformValueForXAxis(decimal myValue)
        {
            decimal value = 0.0M;

            if (myValue <= 0.0M && InFlawTransform == TransformTypeEnum.Log)
            {
                try
                {
                    return Convert.ToDecimal(_podDoc.GetTransformedValue(_data.SmallestFlaw / 2.0, _python.TransformEnumToInt(InFlawTransform)));
                }
                catch (OverflowException overflow)
                {
                    value = 0.0M;
                }
                catch (DivideByZeroException divide)
                {
                    value = 0;
                }
            }           

            try
            {
                value = Convert.ToDecimal(_podDoc.GetTransformedValue(Convert.ToDouble(myValue), _python.TransformEnumToInt(InFlawTransform)));
            }
            catch(OverflowException overflow)
            {
                value = myValue;
            }
            catch (DivideByZeroException divide)
            {
                value = 0;
            }

            return value;
        }

        public double TransformValueForYAxis(double myValue)
        {
            return Convert.ToDouble(TransformValueForYAxis(Convert.ToDecimal(myValue)));
        }

        public decimal TransformValueForYAxis(decimal myValue)
        {
            decimal value = 0.0M;

            if (myValue <= 0.0M && InResponseTransform == TransformTypeEnum.Log)
            {
                try
                {
                    return Convert.ToDecimal(_podDoc.GetTransformedValue(_data.SmallestResponse / 2.0, _python.TransformEnumToInt(InResponseTransform)));
                }
                catch (OverflowException overflow)
                {
                    value = 0.0M;
                }
                catch (DivideByZeroException divide)
                {
                    value = 0;
                }
            }

            

            try
            {
                value = Convert.ToDecimal(_podDoc.GetTransformedValue(Convert.ToDouble(myValue), _python.TransformEnumToInt(InResponseTransform)));
            }
            catch(OverflowException overflow)
            {
                value = myValue;
            }
            catch (DivideByZeroException divide)
            {
                value = 0;
            }

            return value;
        }

        public double InvertTransformValueForXAxis(double myValue)
        {
            return Convert.ToDouble(InvertTransformValueForXAxis(Convert.ToDecimal(myValue)));
        }

        public decimal InvertTransformValueForXAxis(decimal myValue)
        {
            decimal value = 0.0M;

            try
            {
                if (_python != null)
                    value = Convert.ToDecimal(_podDoc.GetInvtTransformedValue(Convert.ToDouble(myValue), _python.TransformEnumToInt(InFlawTransform)));
                else
                    value = myValue;
            }
            catch
            {
                value = myValue;
            }

            return value;
        }

        public double InvertTransformValueForYAxis(double myValue)
        {
            return Convert.ToDouble(InvertTransformValueForYAxis(Convert.ToDecimal(myValue)));
        }

        public decimal InvertTransformValueForYAxis(decimal myValue)
        {
            decimal value = 0.0M;

            try
            {
                if (_python != null)
                    value = Convert.ToDecimal(_podDoc.GetInvtTransformedValue(Convert.ToDouble(myValue), _python.TransformEnumToInt(InResponseTransform)));
                else
                    value = myValue;
            }
            catch
            {
                value = myValue;
            }

            return value;
        }

        public bool HasBeenInitialized { get; set; }

        string _sourceName = string.Empty;
        private string _inAnalysisFlawColumnName;
        private string _worksheetName;
        private double _inFlawMax;
        private double _inFlawMin;
        private double _inPODConfidence;
        private double _inPODLevel;
        private Report _report; //must stay for backwards compatibility of saved files
        private double _inResponseDecision;
        private int _inResponseDecisionIncCount;
        private double _inResponseDecisionMax;
        private double _inResponseDecisionMin;
        private double _inResponseMax;
        private double _inResponseMin;
        private double _initialResponseMinGuess;
        private double _initialResponseMaxGuess;
        private double _initialResponseDecision;
        private bool _usingSuppiledValues;

        public bool UserSuppliedRanges
        {
            get { return _usingSuppiledValues; }
            set { _usingSuppiledValues = value; }
        }

        public string SourceName
        {
            set
            {
                _sourceName = value;
            }
            get
            {
                //if it wasn't set before try to get it from the name of the analysis
                if(_sourceName == null || _sourceName == string.Empty)
                {
                    var splits = Name.Split(new char[] { '.' });

                    if (splits.Length > 0)
                        _sourceName = splits[0];
                }

                return _sourceName;
            }
        }

        public bool UsingCustomName { get; set; }

        public void UpdateRangesFromData()
        {
            double newValue = 0.0;

            if (HasBeenInitialized == true)
            {
                Data.GetUpdatedValue(ColType.Flaw, ExtColProperty.Max, _inFlawMax, out newValue);
                _inFlawMax = newValue;
                Data.GetUpdatedValue(ColType.Flaw, ExtColProperty.Min, _inFlawMin, out newValue);
                _inFlawMin = newValue;

                Data.GetUpdatedValue(ColType.Response, ExtColProperty.Max, _inResponseMax, out newValue);
                _inResponseMax = newValue;
                Data.GetUpdatedValue(ColType.Response, ExtColProperty.Min, _inResponseMin, out newValue);
                _inResponseMin = newValue;
                Data.GetUpdatedValue(ColType.Response, ExtColProperty.Thresh, _inResponseDecision, out newValue);
                _inResponseDecision = newValue;
            }
            else
            {
                Data.GetNewValue(ColType.Flaw, ExtColProperty.Max, out newValue);
                _inFlawMax = newValue;
                Data.GetNewValue(ColType.Flaw, ExtColProperty.Min, out newValue);
                _inFlawMin = newValue;

                Data.GetNewValue(ColType.Response, ExtColProperty.Max, out newValue);
                _inResponseMax = newValue;
                Data.GetNewValue(ColType.Response, ExtColProperty.Min, out newValue);
                _inResponseMin = newValue;
                Data.GetNewValue(ColType.Response, ExtColProperty.Thresh, out newValue);
                _inResponseDecision = newValue;
            }
        }

        public void RunOnlyFitAnalysis()
        {
            var watch = new Stopwatch();

            watch.Start();

            CopyInputToPython();

            watch.Stop();

            var inputTime = watch.ElapsedMilliseconds;

            watch.Restart();

            try
            {
                _podDoc.OnFitOnlyAnalysis();
            }
            catch
            {

            }

            watch.Stop();

            var runTime = watch.ElapsedMilliseconds;

            watch.Restart();

            CopyOutputFromPython();

            watch.Stop();

            var outputTime = watch.ElapsedMilliseconds;

            //TRB removed because it kept causing regression analysis panel to update causing cross thread violation
            //despite the fact that I removed the event handler everytime I switched between steps
            //RaiseAnalysisDone();
            //TRB removing this also since there is no good reason to display errors on chart for fits
            //_python.NotifyFinishAnalysis();  

            stillRunningAnalysis = false;

            //MessageBox.Show("IN: " + inputTime + " OUT: " + outputTime + " RUN: " + runTime);
        }

        public void ClearInitialGuesses()
        {
            _initialResponseDecision = Double.MaxValue;
            _initialResponseMaxGuess = Double.MaxValue;
            _initialResponseMinGuess = Double.MaxValue;
        }

        //quick analysis units
        public string Operator { get; set; }

        public string SpecimenSet { get; set; }

        public string SpecimentUnit { get; set; }

        public string Instrument { get; set; }

        public string InstrumentUnit { get; set; }

        public override string GenerateFileName()
        {
            if(Data.DataType == AnalysisDataTypeEnum.AHat)
            {
                return CreateQuickAHatAnalysisName();
            }
            else
            {
                return CreateQuickHitMissAnalysisName();
            }
        }

        public string CreateQuickAHatAnalysisName()
        {
            return Operator + " - " + SpecimenSet + " - " + Instrument + " - " + Data.DataType.ToString();
        }
        
        public string CreateQuickHitMissAnalysisName()
        {
            return Operator + " - " + SpecimenSet + " - " + Data.DataType.ToString();
        }

        public void ClearAllEvents()
        {
            if(_python != null)
            {
                RaiseClearEvents();
            }
        }

        private void RaiseClearEvents()
        {
            AnalysisDone = null;

            if(EventsNeedToBeCleared != null)
            {
                EventsNeedToBeCleared.Invoke(this, null);
            }
        }

        public void GetProjectInfo(out string fileName, out DataTable data)
        {
            
            var args = new GetProjectInfoArgs(SourceName);

            GetProjectInfo(args);

            string projFileName = "";

            projFileName = args.ProjectFileName;

            fileName = projFileName + " - " + Name;
            data = args.SourceTable;
        }

        private void GetProjectInfo(GetProjectInfoArgs args)
        {
            if(ProjectInfoNeeded != null)
            {
                ProjectInfoNeeded.Invoke(this, args);
            }


        }

        public void ExportProjectToExcel()
        {
            if(ExportProject != null)
            {
                ExportProject.Invoke(this, null);
            }
        }

        public string ToolTipText
        {
            get
            {
                string text = "";
                var name = Name;

                if (name.Length > 40)
                {
                    name = name.Substring(0, 40);
                    name += "...";
                    
                    //var lastIndex = name.LastIndexOf(" ", 0);

                    //if(lastIndex != -1 || lastIndex < 40)
                    //{
                    //    name = name.Substring(0, lastIndex + 1) + Environment.NewLine + name.Substring(lastIndex + 1);
                    //}
                    //else
                    //{
                    //    name = name.Substring(0, 40);
                    //    name += "...";
                    //}
                }

                text += name + Environment.NewLine;
                text += "Flaw Transform: " + InFlawTransform.ToString() + Environment.NewLine;

                if (Data.DataType == AnalysisDataTypeEnum.AHat)
                {
                    text += "Response Transform: " + InResponseTransform.ToString() + Environment.NewLine + Environment.NewLine;
                    text += "POD Decision:\t" + InResponseDecision.ToString("F3") + " " + ResponseUnits[0] + Environment.NewLine;
                    text += "a50:\t\t" + OutResponseDecisionPODA50Value.ToString("F3") + " " + FlawUnit + Environment.NewLine;
                    text += "a90:\t\t" + OutResponseDecisionPODLevelValue.ToString("F3") + " " + FlawUnit + Environment.NewLine;
                    text += "a90/95:\t\t" + OutResponseDecisionPODConfidenceValue.ToString("F3") + " " + FlawUnit + Environment.NewLine;
                }
                else
                {
                    text += "POD Model: " + InHitMissModel.ToString() + Environment.NewLine + Environment.NewLine;
                    text += "a50:\t" + OutResponseDecisionPODA50Value.ToString("F3") + " " + FlawUnit + Environment.NewLine;
                    text += "a90:\t" + OutResponseDecisionPODLevelValue.ToString("F3") + " " + FlawUnit + Environment.NewLine;
                    text += "a90/95:\t" + OutResponseDecisionPODConfidenceValue.ToString("F3") + " " + FlawUnit + Environment.NewLine;
                }
                
                

                return text;
            }
        }

        public string ShortName
        {
            get
            {
                var shortName = Name;

                if(shortName.StartsWith(SourceName))
                {
                    shortName = shortName.Substring(SourceName.Length+1);
                }

                if (shortName.StartsWith(FlawName))
                {
                    shortName = shortName.Substring(FlawName.Length + 1);
                }

                return shortName;
            }


        }

        public void RaiseCreatedAnalysis(Analysis clone)
        {
            if(CreatedAnalysis != null)
            {
                AnalysisList analyses = new AnalysisList();

                analyses.Add(clone);

                var args = new AnalysisListArg(ref analyses);

                CreatedAnalysis.Invoke(this, args);

            }
        }
    }
        
}