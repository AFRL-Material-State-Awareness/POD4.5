// <copyright file="Analysis.cs" company="The University of Dayton">
// Copyright © 2025 The University of Dayton. All rights reserved.
//
// Licensed under the GNU General Public License, Version 3.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.gnu.org/licenses/gpl-3.0.txt
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

//for r engine
using CSharpBackendWithR;
using POD.Data;
using POD.ExcelData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace POD.Analyze
{
    /// <summary>
    ///     Holds all the information and data related to the analysis.
    /// </summary>
    [Serializable]
    public class Analysis : WizardSource
    {
        //flag used to keep track of if there is a current error in the Analysis(gets overwritten in the full regression panel every time it is executed
        private bool _errorPresentInAnalysis = false;
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
        [NonSerialized]
        public HMAnalysisObject _finalAnalysis;
        [NonSerialized]
        public AHatAnalysisObject _finalAnalysisAHat;
        [NonSerialized]
        private Lazy<IBoxCoxTransformer> _lazyBoxCoxYTransformer;
        private Lazy<IBoxCoxTransformer> _lazyBoxCoxXTransformer;
        public BoxCoxAnalysisResult BoxCoxResultY { get; private set; }
        public BoxCoxAnalysisResult BoxCoxResultX { get; private set; }
        public IDataTransformer Transformer { get; private set; }
        /// <summary>
        /// No input values or data will be allowed to be changed while the analysis is frozen.
        /// Created this state to allow UI to be sync'd with the analysis values after being loaded from file.
        /// </summary>
        private bool _isFrozen = false;
        /// <summary>
        /// used to notify the user if separated data exists
        /// </summary>
        private bool _isSeparatedFlag;
        /// <summary>
        /// used to notify the user the algorithm failed to converge
        /// </summary>
        private bool _failedToConverge;
        /// <summary>
        /// used to check if previous analyis is being loaded (used to handle when a saved file as exluded points
        /// </summary>
        private bool _fileLoadHitMiss=false;
        /// <summary>
        /// used to update quick analysis (when the user adds or removes rows the chart updates
        /// </summary>
        private bool _quickAnalysis=false;
        /// <summary>
        /// Used to indicate that the run analysis a threshold change in the main regression panel
        /// </summary>
        private RCalculationType _analysisCalculationType = RCalculationType.Full;

        public bool IsFrozen
        {
            get { return _isFrozen; }
            set
            {
                if (Data != null)
                {
                    Data.IsFrozen = value;
                }

                _isFrozen = value; 
            }
        }
        #region Fields

        /// <summary>
        ///     Holds the tabular data associated with the analysis and anything associated data transforms.
        /// </summary>
        private IAnalysisData _data;

        /// <summary>
        ///     the time and date the analysis was performed
        /// </summary>
        private DateTime _analysisDate;

        

        #endregion

        #region Constructors
        /// <summary>
        /// this constructor is used for testing only
        /// </summary>
        /// <param name="data"></param>
        public Analysis(IAnalysisData data=null)
        {
            _data = data ?? new AnalysisData();
            Initialize();

            SetupAnalysisLauncher();

            

            CalculateAndSetBoxCoxParameters();
        }

        public Analysis(DataSource mySource)
        {
            _data = new AnalysisData();
            Initialize();

            SetDataSource(mySource);

            CalculateAndSetBoxCoxParameters();

            SetupAnalysisLauncher();
        }

        

        public Analysis(DataSource mySource, List<string> myFlaws, List<string> myMetaDatas, List<string> myResponses,
            List<string> mySpecIDs)
        {
            _data = new AnalysisData();
            Initialize();

            SetDataSource(mySource, myFlaws, myMetaDatas, myResponses, mySpecIDs);

            CalculateAndSetBoxCoxParameters();

            SetupAnalysisLauncher();
        }

        public Analysis(DataSource source, SourceInfo info, string flawName, List<string> responses, IREngineObject rEngine, I_IPy4C pyEngine, string analysisName, bool hasCustomName)
        {
            _data = new AnalysisData();
            _data.DataType = source.AnalysisDataType;
            Name = analysisName;
            UsingCustomName = hasCustomName;
            SetREngine(rEngine);
            SetPythonEngine(pyEngine);
            Initialize();

            SetDataSource(source, flawName, responses);
            SetInfo(info);

            CalculateAndSetBoxCoxParameters();

            Data.ActivateFlaw(flawName);
            Data.ActivateResponses(responses);            

            CalculateInitialValuesWithNewData();

            SetupAnalysisLauncher();
        }

        public Analysis(DataSource source, SourceInfo info, string flawName, List<string> responses)
        {
            _data = new AnalysisData();
            Initialize();

            SetDataSource(source, flawName, responses);
            SetInfo(info);

            Data.ActivateFlaw(flawName);
            Data.ActivateResponses(responses);

            CalculateAndSetBoxCoxParameters();

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
        public IAnalysisData Data
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

        private HitMissRegressionType _hitMissModel;

        public HitMissRegressionType InHitMissModel
        {
            get { return _hitMissModel; }
            set { if (!IsFrozen) _hitMissModel = value; }
        }

        private ConfidenceIntervalTypeEnum _confIntHMType;
        public ConfidenceIntervalTypeEnum InConfIntervalType
        {
            get { return _confIntHMType; }
            set { if (!IsFrozen) _confIntHMType = value; }
        }
        private SamplingTypeEnum _samplingHMType;
        public SamplingTypeEnum InSamplingType
        {
            get { return _samplingHMType; }
            set { if (!IsFrozen) _samplingHMType = value; }
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
        public double OutRSquaredValue
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


        double _outTestEqualVariance;

        /// <summary>
        ///    Non-constant variance test value for Equal Variance test
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
        ///     Non-constant variance test p value
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
        ///     Pure Error for Lack Of Fit test (Changed to Whole Model)
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

        double _outTestAIC;

        public double OutTestAIC
        {
            get
            {
                return NoBigNumbers(_outTestAIC);
            }

            private set
            {
                if (!IsFrozen)
                    _outTestAIC = value;
            }
        }

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

        string _outTestAICRating = "";

        public string OutTestAICRating
        {
            get { return _outTestAICRating; }
            set { if (!IsFrozen) _outTestAICRating = value; }
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
        /// <summary>
        /// p value for autocorrelation using durbin watson test
        /// </summary>
        double _outTestAutoCorrelation_p;
        public double OutTestAutoCorrelation_p
        {
            get
            {
                return NoBigNumbers(_outTestAutoCorrelation_p);
            }

            private set
            {
                if (!IsFrozen)
                    _outTestAutoCorrelation_p = value;
            }
        }
        string _outTestAutoCorrelationRating = "";

        public string OutTestAutoCorrelationRating
        {
            get { return _outTestAutoCorrelationRating; }
            set { if (!IsFrozen) _outTestAutoCorrelationRating = value; }
        }

        double _outTestNormality;

        /// <summary>
        ///     Shapiro-Wilk test value for Normality test
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
        ///     p-Value for the Shapiro-Wilk value for Normality test
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
            //REngineObject.REngineRunning = true;
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

            if (Data.MissingData)
            {
                return;
            }

            //_data.UpdateHitMissModel(_hmAnalysisObject.ModelType);
            IsBusy = true;
            AnalysisBackendControl newAnalysisControl;
            if (Data.DataType == AnalysisDataTypeEnum.HitMiss)
            {
                newAnalysisControl = new AnalysisBackendControl(_rDotNet, Data.HMAnalysisObject, null, Transformer);
            }
            else
            {
                //NOTE: Data.AHATAnalysisObject.SignalResponses only has crack thresholding done on the last response at this point
                newAnalysisControl = new AnalysisBackendControl(_rDotNet, null, Data.AHATAnalysisObject, Transformer);

            }
            try
            {
                //entry point for pod caluclation in PODv4.5 hit miss
                if (Data.DataType == AnalysisDataTypeEnum.HitMiss)
                {
                    //AnalysisBackendControl newAnalysisControl = new AnalysisBackendControl(_rDotNet, _hmAnalysisObject);
                    newAnalysisControl.ExecuteReqSampleAnalysisTypeHitMiss();
                    _finalAnalysis = newAnalysisControl.HMAnalsysResults;

                }
                //entry point for pod calculate in PODv4.5 ahat
                else
                {
                    //AnalysisBackendControl newAnalysisControl = new AnalysisBackendControl(_rDotNet, null, Data.AHATAnalysisObject);
                    if (_analysisCalculationType == RCalculationType.Full)
                    {
                        newAnalysisControl.ExecuteAnalysisAHat();
                    }                   
                    else if (_analysisCalculationType == RCalculationType.ThresholdChange)
                    {
                        newAnalysisControl.ExecuteThresholdChange();
                    }
                    _finalAnalysisAHat = newAnalysisControl.AHatAnalysisResults;
                }
                    
            }
            //TODO: add exception handling here to ensure the value in the c# datatables are not null
            catch (Exception exp)
            {
                var moreInfo = string.Empty;
                
                try
                {
                    Globals.ShowErrorMessageBox(exp, "Running Analysis");                                      

                    
                }
                catch(Exception)
                {
                    moreInfo = string.Empty;
                }

                if (exp.Message.StartsWith("Error in "))
                {
                    _python.AddErrorText($"R Source Code Error: {exp.Message.Replace("Error in ", "")}");
                }
                else
                {
                    _python.AddErrorText($"Analysis Error: {exp.Message}");
                }
                

                //used to keep the program from breaking completely if an error occurs
                if (Data.DataType == AnalysisDataTypeEnum.HitMiss)
                {
                    newAnalysisControl.ReturnHitMissObjects();
                }
            }
            finally
            {
                
            }

                        
        }

        private void Background_AnalysisProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (Data.MissingData)
            {
                return;
            }

            _python.ProgressText = $"{e.ProgressPercentage}%";
        }

        private void Background_FinishedAnalysis(object sender, RunWorkerCompletedEventArgs e)
        {
            while (analysisLauncher.IsBusy == true) ;

            if (Data.MissingData)
            {
                RaiseAnalysisDone();
                _python.NotifyFinishAnalysis();
                stillRunningAnalysis = false;
                analysisLauncher.Dispose();
                _analysisDate = DateTime.Now;
                IsBusy = false;
                //reset the calculation type after analysis is complete
                _analysisCalculationType = RCalculationType.Full;

                return;
            }

            CopyOutputFromR();

            if ((e.Cancelled == true))
            {
                _python.ProgressText = "Canceled!";
            }

            else if (!(e.Error == null))
            {
                _python.ProgressText = $"Error: {e.Error.Message}";
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
            //reset the calculation type after analysis is complete
            _analysisCalculationType = RCalculationType.Full;
        }

        private void CopyOutputFromR()
        {  
            _python.OutputWriter.StringWritten -= OutputWriter_StringWritten;
            _python.ErrorWriter.StringWritten -= ErrorWriter_StringWritten;
            
            if (AnalysisDataType == AnalysisDataTypeEnum.AHat)
            {
                OutModelIntercept = Data.AHATAnalysisObject.Intercept;
                OutModelInterceptStdError = Data.AHATAnalysisObject.InterceptStdErr;
                OutModelSlope = Data.AHATAnalysisObject.Slope;
                OutModelSlopeStdError = Data.AHATAnalysisObject.SlopeStdErr;
                OutModelResidualError = Data.AHATAnalysisObject.ResidualError;
                OutModelResidualErrorStdError = Data.AHATAnalysisObject.ResidualStdErr;
                OutRSquaredValue = Data.AHATAnalysisObject.RSqaured;
                //replaced normality test with shapiro-wilk
                OutTestNormality_p = Data.AHATAnalysisObject.ShapiroPValue;
                OutTestNormality = Data.AHATAnalysisObject.ShapiroTestStat;
                OutTestNormalityRating = _python.GetPValueDecision(OutTestNormality_p);
                OutTestLackOfFit_p = Data.AHATAnalysisObject.LackOfFitPValue;
                OutTestLackOfFit = Data.AHATAnalysisObject.LackOfFitFCalc;
                OutTestLackOfFitRating = _python.GetPValueDecision(OutTestLackOfFit_p);
                OutTestLackOfFitDegreesFreedom = Convert.ToInt32(Data.AHATAnalysisObject.LackOfFitDegFreedom);
                //this metric was included with the python version (not sure where its used if at all)
                OutTestLackOfFitCalculated = true;
                OutTestEqualVariance_p = Data.AHATAnalysisObject.ChiSqPValue;
                OutTestEqualVariance = Data.AHATAnalysisObject.ChiSqValue;
                OutTestEqualVarianceRating= _python.GetPValueDecision(OutTestEqualVariance_p);
                OutTestAutoCorrelation_p = Data.AHATAnalysisObject.DurbinWatsonPValue;
                OutTestAutoCorrelationRating = _python.GetPValueDecision(OutTestAutoCorrelation_p);
                //A Values and sigma
                OutResponseDecisionPODSigma = Data.AHATAnalysisObject.Sighat;
                OutResponseDecisionPODA50Value = Data.AHATAnalysisObject.A50;
                OutResponseDecisionPODLevelValue = Data.AHATAnalysisObject.A90;
                OutResponseDecisionPODConfidenceValue = Data.AHATAnalysisObject.A9095;

                if (InFlawTransform == TransformTypeEnum.Linear)
                {
                    OutResponseDecisionPODSigma = Data.AHATAnalysisObject.Sighat;
                    OutResponseDecisionPODA50Value = Data.AHATAnalysisObject.A50;
                    OutResponseDecisionPODLevelValue = Data.AHATAnalysisObject.A90;
                    OutResponseDecisionPODConfidenceValue = Data.AHATAnalysisObject.A9095;
                }
                else if (InFlawTransform == TransformTypeEnum.Log)
                {
                    OutResponseDecisionPODSigma = Math.Exp(Data.AHATAnalysisObject.Sighat);
                    OutResponseDecisionPODA50Value = Math.Exp(Data.AHATAnalysisObject.A50);
                    OutResponseDecisionPODLevelValue = Math.Exp(Data.AHATAnalysisObject.A90);
                    OutResponseDecisionPODConfidenceValue = Math.Exp(Data.AHATAnalysisObject.A9095);
                }
                else if (InFlawTransform == TransformTypeEnum.Inverse)
                {
                    OutResponseDecisionPODSigma = 1 / (Data.AHATAnalysisObject.Sighat);
                    OutResponseDecisionPODA50Value = 1 / (Data.AHATAnalysisObject.A50);
                    OutResponseDecisionPODLevelValue = 1 / (Data.AHATAnalysisObject.A90);
                    OutResponseDecisionPODConfidenceValue = 1 / (Data.AHATAnalysisObject.A9095);
                }
                else
                {
                    throw new Exception("OOPS something went wrong with tranforming the a values back to linear space");
                }
                OutPODMu = Data.AHATAnalysisObject.Muhat;
                OutPODSigma = Data.AHATAnalysisObject.Sighat;
                //OutTestLackOfFit = _hmAnalysisObject.GoodnessOfFit;
            }
            else if (AnalysisDataType == AnalysisDataTypeEnum.HitMiss)
            {
                //List<double> covMatrix = _python.PythonToDotNetList(_podDoc.GetPFEstimatedCovarianceMatrix());
                List<double> covMatrix = Data.HMAnalysisObject.CovarianceMatrix;
                if(Data.HMAnalysisObject.CovarianceMatrix != null)
                {
                    OutPFCovarianceV11 = covMatrix[0];
                    OutPFCovarianceV12 = covMatrix[1];
                    OutPFCovarianceV22 = covMatrix[3];   
                }
                else
                {
                    //Debug.WriteLine("Warning: something went wrong with calculating the vcov matrix!");
                    //MessageBox.Show("Warning: something went wrong with calculating the vcov matrix!");
                    OutPFCovarianceV11 = -1;
                    OutPFCovarianceV12 = -1;
                    OutPFCovarianceV22 = -1;
                }
                //switch (InFlawTransform) { }
                if (InFlawTransform == TransformTypeEnum.Linear)
                {
                    OutResponseDecisionPODSigma = Data.HMAnalysisObject.Sighat;
                    OutResponseDecisionPODA50Value = Data.HMAnalysisObject.A50;
                    OutResponseDecisionPODLevelValue = Data.HMAnalysisObject.A90;
                    OutResponseDecisionPODConfidenceValue = Data.HMAnalysisObject.A9095;
                }
                else if (InFlawTransform == TransformTypeEnum.Log)
                {
                    OutResponseDecisionPODSigma =Math.Exp(Data.HMAnalysisObject.Sighat);
                    OutResponseDecisionPODA50Value = Math.Exp(Data.HMAnalysisObject.A50);
                    OutResponseDecisionPODLevelValue = Math.Exp(Data.HMAnalysisObject.A90);
                    OutResponseDecisionPODConfidenceValue = Math.Exp(Data.HMAnalysisObject.A9095);
                }
                else if (InFlawTransform == TransformTypeEnum.Inverse)
                {
                    OutResponseDecisionPODSigma = 1/(Data.HMAnalysisObject.Sighat);
                    OutResponseDecisionPODA50Value = 1 / (Data.HMAnalysisObject.A50);
                    OutResponseDecisionPODLevelValue = 1 / (Data.HMAnalysisObject.A90);
                    OutResponseDecisionPODConfidenceValue = 1 / (Data.HMAnalysisObject.A9095);
                }
                else
                {
                    throw new Exception("OOPS something went wrong with tranforming the a values back to linear space");
                }
                OutPODMu = Data.HMAnalysisObject.Muhat;
                OutPODSigma = Data.HMAnalysisObject.Sighat;
                OutTestLackOfFit = Data.HMAnalysisObject.GoodnessOfFit;
                OutTestAIC = Data.HMAnalysisObject.AicValue;
                //OutTestNormalityRating = _python.GetPValueDecision(OutTestLackOfFit);
                OutTestLackOfFitRating = _python.GetPValueDecision(OutTestLackOfFit);

                //separated flag
                _isSeparatedFlag = Data.HMAnalysisObject.Is_Separated;
                //failure to converge flag
                _failedToConverge = Data.HMAnalysisObject.Failed_To_Converge;
            }

            Data.UpdateOutput(_analysisCalculationType);
            
            //Data.ResponseLeft = InResponseMin;
            //Data.ResponseRight = InResponseMax;

            if (AnalysisDataType == AnalysisDataTypeEnum.HitMiss)
            {
                //_hmAnalysisObject.ClearMetrics();
            }
            else
            {
                //Data.AHATAnalysisObject.ClearMetrics();
            }
        }

        public void CheckLoadHealth()
        {
            Data.CheckLoadHealth();
        }

        public void CalculateAndSetBoxCoxParameters()
        {
            if (Data.DataType == AnalysisDataTypeEnum.AHat)
            {
                try
                {
                    // 1. Get Data
                    var responseNames = Data.ActivatedResponseNames;
                    var analysisData = Data.AHATAnalysisObject;
                    var allFlaws = analysisData.Flaws_All;
                    var allResponseData = analysisData.Responses_all;
                    double xMin = allFlaws.Any() ? allFlaws.Min() : 0.0;
                    double xMax = allFlaws.Any() ? allFlaws.Max() : 1.0;
                    double yMin = allResponseData.Any() ? allResponseData.Values.SelectMany(x => x).DefaultIfEmpty().Min() : 0.0;
                    double yMax = allResponseData.Any() ? allResponseData.Values.SelectMany(x => x).DefaultIfEmpty().Max() : 1.0;

                    // 2. Prepare a DataTable with the entire original data
                    var wholeTable = new DataTable();
                    wholeTable.Columns.Add("Size", typeof(double));
                    foreach (var responseName in responseNames)
                    {
                        wholeTable.Columns.Add(responseName, typeof(double));
                    }
                    for (int i = 0; i < allFlaws.Count; i++)
                    {
                        object[] rowValues = new object[responseNames.Count + 1];
                        rowValues[0] = allFlaws[i];
                        for (int j = 0; j < responseNames.Count; j++)
                        {
                            rowValues[j + 1] = allResponseData[responseNames[j]][i];
                        }
                        wholeTable.Rows.Add(rowValues);
                    }

                    // 3. Prepare a second DataTable with only the NON-CENSORED data
                    var nonCensoredTable = new DataTable();
                    nonCensoredTable.Columns.Add("Size", typeof(double));
                    foreach (var responseName in responseNames)
                    {
                        nonCensoredTable.Columns.Add(responseName, typeof(double));
                    }
                    for (int i = 0; i < allFlaws.Count; i++)
                    {
                        double flaw = allFlaws[i];
                        if (Compute.ApproximatelyMoreThan(flaw, InFlawMin) &&
                            Compute.ApproximatelyLessThan(flaw, InFlawMax) &&
                            CheckForTurnedOffPoints(i))
                        {
                            bool allResponsesInRange = true;
                            object[] rowValues = new object[responseNames.Count + 1];
                            rowValues[0] = flaw;
                            for (int j = 0; j < responseNames.Count; j++)
                            {
                                string responseName = responseNames[j];
                                double response = allResponseData[responseName][i];
                                if (Compute.ApproximatelyLessThanOrEqual(response, InResponseMin) ||
                                    Compute.ApproximatelyMoreThanOrEqual(response, InResponseMax))
                                {
                                    allResponsesInRange = false;
                                    break;
                                }
                                rowValues[j + 1] = response;
                            }
                            if (allResponsesInRange)
                            {
                                nonCensoredTable.Rows.Add(rowValues);
                            }
                        }
                    }

                    // If no data remains after censoring, we can't calculate lambda.
                    if (nonCensoredTable.Rows.Count == 0)
                    {
                        _python.AddErrorText("Warning: No data remains for Box-Cox parameter calculation after applying censor limits.");
                        return;
                    }

                    // 4. Execute Transform, passing both tables
                    var transformer = LazyYBoxCoxTransformer;
                    var resultY = transformer.ExecuteAndVerifyTransformation(wholeTable, nonCensoredTable, responseNames);

                    var transformerX = LazyXBoxCoxTransformer;
                    var resultX = transformerX.ExecuteAndVerifyTransformation(null, null, null);

                    // 5. Store Results
                    BoxCoxResultY = resultY;
                    BoxCoxResultX = resultX;

                    if (BoxCoxResultY != null && BoxCoxResultX != null)
                    {
                        Transformer = new DataTransformer(BoxCoxResultY, InResponseTransform, yMin, yMax, BoxCoxResultX, InFlawTransform, xMin, xMax);
                        _data.SetDisplayTransformer(Transformer);
                        InResponseTransform = TransformTypeEnum.BoxCox;
                    }

                    // 6. Update Lambda
                    InLambdaValue = resultY.Lambda;
                }
                catch (Exception exp)
                {
                    _python.AddErrorText($"Error in CalculateAndSetBoxCoxParameters: {exp.Message}");
                    Console.WriteLine($"Error in CalculateAndSetBoxCoxParameters: {exp}");
                }
            }
            else
            {
                // Lightweight initialization for Hit/Miss or when no data is present yet.
                // This ensures the Transformer is always created.
                BoxCoxResultY = new BoxCoxAnalysisResult();
                BoxCoxResultX = new BoxCoxAnalysisResult();

                // Get the real data range for the X-axis (flaws) to ensure correct plotting.
                double xMin = 0.0;
                double xMax = 1.0;

                if (this.Data?.HMAnalysisObject?.Flaws_All != null)
                {
                    var allFlaws = this.Data.HMAnalysisObject.Flaws_All;
                    if (allFlaws.Any())
                    {
                        xMin = allFlaws.Min();
                        xMax = allFlaws.Max();
                    }
                }

                

                // Y-axis for Hit/Miss is always 0 to 1.
                this.Transformer = new DataTransformer(this.BoxCoxResultY, _yAxisTransformIn, 0, 1, this.BoxCoxResultX, _xAxisTransformIn, xMin, xMax);
                this._data.SetDisplayTransformer(this.Transformer);

                //this.InResponseTransform = TransformTypeEnum.Linear;
                //this.InFlawTransform = TransformTypeEnum.Linear;
            }
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


        private void CalculateInitialValuesWithNewInfo(SourceInfo myInfo)
        {
            if (HasBeenInitialized == false && Data.RowCount > 0)
            {
                Data.UpdateSourceFromInfos(myInfo);

                //get the response range for the responses included in the analysis
                UpdateRangesFromData();

                HasBeenInitialized = true;

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

                double percentage = .4;

                if (forcedType == AnalysisDataTypeEnum.HitMiss)
                {
                    percentage = .5;
                }

                //take a guess at 40% of the total response range
                InResponseDecision = Data.InvertTransformedResponse(min + ((max - min) * percentage));
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
                AnalysisDataType = _data.RecheckAnalysisType(forcedType);
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
                ForceUpdateInputsFromData();

                if (AnalysisDataType == AnalysisDataTypeEnum.HitMiss)
                {
                    InFlawTransform = Transformer.FlawTransform;
                    Data.FlawTransform = InFlawTransform;
                    Data.HMAnalysisObject.ModelType = _python.TransformEnumToInt(InFlawTransform);
                }                

                _initialResponseMinGuess = InResponseMin;
                _initialResponseMaxGuess = InResponseMax;
                _initialResponseDecision = InResponseDecision;

                HasBeenInitialized = true;
            }
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

            double range = Math.Abs(myMax - myMin);
            // AI said that (int) is better for negative values ie values less than 1
            // double logRange = Math.Floor(Math.Log10(range)) - 1;
            double logRange = (int)Math.Log10(range) - 1;
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
            //analysis._podDoc = null;
            //analysis._hmAnalysisObject = null;
            //analysis.Data.AHATAnalysisObject = null;
            analysis._rDotNet = null;

            // Ensure the DisplayTransformer is correctly initialized in the duplicated object.
            var resultForTransformerY = analysis.BoxCoxResultY ?? new BoxCoxAnalysisResult();
            var resultForTransformerX = analysis.BoxCoxResultX ?? new BoxCoxAnalysisResult();
            analysis.Transformer = new DataTransformer(resultForTransformerY, analysis.InResponseTransform, analysis.Data.SmallestResponse, analysis.Data.LargestResponse, resultForTransformerX, analysis.InFlawTransform, analysis.Data.SmallestFlaw, analysis.Data.LargestFlaw);
            analysis.Data.SetDisplayTransformer(analysis.Transformer);

            return analysis;
        }

        /// <summary>
        ///     Gets the range of values for a given table.
        /// </summary>
        /// <param name="myTable">the table to query</param>
        /// <param name="myMin">minimum value in the table</param>
        /// <param name="myMax">maximum value in the table</param>
        public static void GetMinMax(DataTable myTable, out double myMin, out double myMax)
        {
            myMin = double.MaxValue;
            myMax = double.MinValue;

            foreach (DataColumn column in myTable.Columns)
            {

                Compute.MinMax(myTable, column, ref myMin, ref myMax);

            }

            Compute.SanityCheck(ref myMin, ref myMax);
        }

        



                [System.Runtime.Serialization.OnDeserialized]
        private void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
        {
            _lazyBoxCoxYTransformer = new Lazy<IBoxCoxTransformer>(() => new BoxCoxTransformer(RDotNet));
            _lazyBoxCoxXTransformer = new Lazy<IBoxCoxTransformer>(() => new BoxCoxTransformer(RDotNet));
            // Ensure the DisplayTransformer is correctly initialized after deserialization.
            var resultXForTransformer = BoxCoxResultX ?? new BoxCoxAnalysisResult();
            var resultYForTransformer = BoxCoxResultY ?? new BoxCoxAnalysisResult();            
            if (_data != null)
            {
                Transformer = new DataTransformer(resultYForTransformer, InResponseTransform, _data.SmallestResponse, _data.LargestResponse, 
                                                  resultXForTransformer, InFlawTransform, _data.SmallestFlaw, _data.LargestFlaw);
                _data.SetDisplayTransformer(Transformer);
            }
            else
            {
                Transformer = new DataTransformer(resultYForTransformer, InResponseTransform, 1.0, 10.0,
                                                  resultXForTransformer, InFlawTransform, .01, 1.0);
            }
        }

        /// <summary>
        ///     Initialize the Analysis with default values.
        /// </summary>
        private void Initialize()
        {
            this.IsFrozen = true;

            //Report = new Report();
            

            _lazyBoxCoxYTransformer = new Lazy<IBoxCoxTransformer>(() => new BoxCoxTransformer(RDotNet));
            _lazyBoxCoxXTransformer = new Lazy<IBoxCoxTransformer>(() => new BoxCoxTransformer(RDotNet));

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
            InLambdaValue = 1.0;

            InResponseDecisionMin = double.MinValue;
            InResponseDecisionMax = double.MaxValue;

            AnalysisType = AnalysisTypeEnum.Undefined;
            AnalysisDataType = AnalysisDataTypeEnum.Undefined;
            ObjectType = PODObjectTypeEnum.Analysis;

            _archivedOutputs = new List<Analysis>();

            _activeAnalysis = this;
            _activeAnalysisIndex = -1;

            this.IsFrozen = false;
        }
        private bool CheckForTurnedOffPoints(int index)
        {
            //assume the data point is turned on 
            bool turnedOn= true;
            foreach(DataPointIndex dataPointIndex in Data.TurnedOffPoints)
            {
                if(dataPointIndex.RowIndex == index)
                {
                    turnedOn = false;
                    break;
                }
            }
            return turnedOn;
        }
        private void UpdateCensoredData()
        {
            // NOTE: The logic in this function is tightly coupled with the 'createDataFrameinGlobalEnvr' method in 'AHatAnalysisRControl.cs'.
            // Any changes to how censored data or included flaws are determined here must be reflected there to ensure the data sent to R matches the C# state.            
            //Data.AHATAnalysisObject.SignalResponseName = Data.AHATAnalysisObject.Responses_all.ElementAt(i).Key;
            
            //hitmiss analysis
            if (AnalysisDataType == AnalysisDataTypeEnum.HitMiss)
            {
                Data.HMAnalysisObject.ExcludedFlaws.Clear();

                foreach (string signalName in Data.HMAnalysisObject.Responses_all.Keys)
                {
                    int index = 0;
                    List<double> hmInludedFlaws = new List<double>();
                    List<double> hmIncludedResponses = new List<double>();
                    if (Data.HMAnalysisObject.ModelType == (int)RModelTypeEnum.Inverse)
                    {
                        InFlawMax = Data.HMAnalysisObject.Flaws_All.Max();
                    }
                    foreach (double flaw in Data.HMAnalysisObject.Flaws_All)
                    {
                        if (Compute.ApproximatelyMoreThan(flaw, InFlawMin) && Compute.ApproximatelyLessThan(flaw, InFlawMax) && CheckForTurnedOffPoints(index))
                        {
                            hmInludedFlaws.Add(flaw);
                            int responseIndex = Data.HMAnalysisObject.Flaws_All.IndexOf(flaw);
                            hmIncludedResponses.Add(Data.HMAnalysisObject.Responses_all[signalName][responseIndex]);
                        }
                        else
                        {
                            //store the list of exluded flaws
                            Data.HMAnalysisObject.ExcludedFlaws.Add(flaw);
                        }
                        index += 1;
                    }

                    //NOTE: this will overwritten with same flaw array for each response series, TRB (02-27-2025)
                    //overwrite the temporary flaw variables in HitMiss object
                    Data.HMAnalysisObject.Flaws = hmInludedFlaws;

                    //_hmAnalysisObject.Responses["y"] = hmIncludedResponses;
                    Data.HMAnalysisObject.Responses[signalName] = hmIncludedResponses;
                }
            }
            //ahat analysis
            else
            {
                if (Data.AHATAnalysisObject.Signalmax == -1)
                {
                    Data.AHATAnalysisObject.Signalmax = Data.AHATAnalysisObject.GetMaxResponse();
                }

                if (Data.AHATAnalysisObject.Signalmin == -1)
                {
                    Data.AHATAnalysisObject.Signalmin = Data.AHATAnalysisObject.GetMinResponse();
                }

                // === Refactoring Part 1: Process flaws and exclusions once ===
                Data.AHATAnalysisObject.ExcludedFlaws.Clear();
                List<double> includedFlaws = new List<double>();
                // Store indices to avoid re-checking conditions
                List<int> includedIndices = new List<int>();

                int index = 0;
                foreach (double flaw in Data.AHATAnalysisObject.Flaws_All)
                {
                    if (Compute.ApproximatelyMoreThan(flaw, InFlawMin) && Compute.ApproximatelyLessThan(flaw, InFlawMax) && CheckForTurnedOffPoints(index))
                    {
                        includedFlaws.Add(flaw);
                        includedIndices.Add(index);
                    }
                    else
                    {
                        Data.AHATAnalysisObject.ExcludedFlaws.Add(flaw);
                    }
                    index++;
                }

                // Assign the included flaws list. This is only written once, but the result is the same
                // as the original code which overwrote it with the same list every time.
                Data.AHATAnalysisObject.Flaws = includedFlaws;

                // === Refactoring Part 2: Loop through responses for censoring ===
                foreach (string signalName in Data.AHATAnalysisObject.Responses_all.Keys)
                {
                    List<double> includedResponses = new List<double>();
                    List<double> censoredFlaws = new List<double>();
                    List<double> censoredResponsesL = new List<double>();
                    List<double> censoredResponsesR = new List<double>();
                    List<double> censoredFlawsL = new List<double>();
                    List<double> censoredFlawsR = new List<double>();

                    // Iterate only over the included flaws using the pre-calculated indices
                    foreach (int responseIndex in includedIndices)
                    {
                        double flaw = Data.AHATAnalysisObject.Flaws_All[responseIndex];
                        double response = Data.AHATAnalysisObject.Responses_all[signalName][responseIndex];

                        includedResponses.Add(response);

                        if (Compute.ApproximatelyLessThanOrEqual(response, InResponseMin))
                        {
                            censoredFlaws.Add(flaw);
                            censoredFlawsL.Add(flaw);
                            censoredResponsesL.Add(response);
                        }
                        if (Compute.ApproximatelyMoreThanOrEqual(response, InResponseMax))
                        {
                            censoredFlaws.Add(flaw);
                            censoredFlawsR.Add(flaw);
                            censoredResponsesR.Add(response);
                        }
                    }

                    // Preserve the original (buggy) behavior of overwriting FlawsCensored
                    Data.AHATAnalysisObject.FlawsCensored = censoredFlaws;

                    // Assign the response-specific lists to the dictionaries
                    Data.AHATAnalysisObject.Responses[signalName] = includedResponses;
                    Data.AHATAnalysisObject.ResponsesCensoredLeft[signalName] = censoredResponsesL;
                    Data.AHATAnalysisObject.ResponsesCensoredRight[signalName] = censoredResponsesR;
                }
            }
        }
        /// <summary>
        ///     Run analysis with current settings and data. Output data will be overwritten.
        /// </summary>
        public void RunAnalysis()
        {
            if (IsFrozen)
                return;
            //REngineObject.REngineRunning = true;
            if (analysisLauncher == null)
            {
                SetupAnalysisLauncher();
            }            

            if (analysisLauncher.IsBusy == false && stillRunningAnalysis == false)
            {
                stillRunningAnalysis = true;

                CopyInputToR();
                UpdateCensoredData();
                _python.FailedRunCount = 0;
                
                //The analysis is run with parallelization. Use the 'threads' window to see what each worker is computing
                analysisLauncher.RunWorkerAsync();

            }
            else
            {
                _python.ProgressText = "Busy running current analysis.";
                //MessageBox.Show("Analysis code is still running.");
                //analysisLauncher.CancelAsync();

                _python.FailedRunCount++;

                if(_python.FailedRunCount >= 40)
                {
                    Show_R_CodeStuckMessage();                    
                }
            }
        }

        private void Show_R_CodeStuckMessage()
        {
            var result = MessageBox.Show(
                "POD Professional v4.5 analysis code has not terminated.  " +
                "Please contact UDRI for further assistance.  " +
                "If possible make a copy of log.txt to send to them.\n\n" +
                "Would you like to open the folder with the log file?",
                "POD Professional v4.5",
                MessageBoxButtons.YesNo);

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
        //TODO : turn the string for model names and ranked set sampling into enumerations
        private void CopyInputToR()
        {
            if (AnalysisDataType == AnalysisDataTypeEnum.HitMiss)
            {
                Data.HMAnalysisObject.ProgressText = "Starting Analysis";
            }
            else
            {
                Data.AHATAnalysisObject.ProgressText= "Starting Analysis";
            }
            
            // store the analysis name in the python engine object 
            _python.CurrentAnalysisName = Name;

            _python.OutputWriter.StringWritten -= OutputWriter_StringWritten;
            _python.ErrorWriter.StringWritten -= ErrorWriter_StringWritten;
            _python.OutputWriter.StringWritten += OutputWriter_StringWritten;
            _python.ErrorWriter.StringWritten += ErrorWriter_StringWritten;

            //flip them so the calculation code still work properly
            //used if the censor lines cross each other
            if (InResponseMin >= InResponseMax)
            {
                double temp = InResponseMin;
                InResponseMin = InResponseMax;
                InResponseMax = temp;
            }
            
            //used to store the current transformation the program is performing in the 'choose transform' window
            UpdateRTransforms();
            //change the model type of logistic regression to firth logistical regression if using log odds
            if (InHitMissModel.ToString() == "LogisticRegression" && AnalysisDataType == AnalysisDataTypeEnum.HitMiss)
            {
                Data.HMAnalysisObject.RegressionType = "Logistic Regression";
            }
            else if (InHitMissModel.ToString() == "FirthLogisticRegression" && AnalysisDataType == AnalysisDataTypeEnum.HitMiss)
            {
                Data.HMAnalysisObject.RegressionType = "Firth Logistic Regression";
            }

            CheckForLoadFileWithCensoredData();
            
            if (!_fileLoadHitMiss)
            {
                Data.UpdateData(_quickAnalysis);
            }
            _fileLoadHitMiss = false;

            if (AnalysisDataType == AnalysisDataTypeEnum.AHat)
            {
                double transformedDecision = Transformer.TransformYDataValue(InResponseDecision);
                Data.AHATAnalysisObject.Pod_threshold = transformedDecision;
            }

            if (AnalysisDataType == AnalysisDataTypeEnum.HitMiss)
            {
                double transformedDecision = Transformer.TransformYDataValue(InResponseDecision);
                Data.HMAnalysisObject.Pod_threshold = transformedDecision;
                Data.HMAnalysisObject.Pod_level = .90;
                Data.HMAnalysisObject.Confidence_level = .95;
                //used for modified wald, LR, etc.
                Data.HMAnalysisObject.A_x_n = 500;
                Data.HMAnalysisObject.CIType = InConfIntervalType.ToString(); // used to change the confidence interval if the user chooses to
                switch (InSamplingType.ToString())
                {
                    case "SimpleRandomSampling":
                        Data.HMAnalysisObject.SrsOrRSS = 0;
                        break;
                    case "RankedSetSampling":
                        Data.HMAnalysisObject.SrsOrRSS = 1;
                        break;
                    default:
                        Data.HMAnalysisObject.SrsOrRSS = 0; //use simple random sampling by default
                        break;
                }
                //used for RSS
                if (Data.HMAnalysisObject.CIType == "StandardWald")
                {
                    Data.HMAnalysisObject.Set_r = Data.HMAnalysisObject.Count / Data.HMAnalysisObject.Set_m;
                }
                else
                {
                    Data.HMAnalysisObject.Set_r = Data.HMAnalysisObject.A_x_n / Data.HMAnalysisObject.Set_m;
                }
                Data.HMAnalysisObject.MaxResamples = 30;
            }
            List<double> thresholds = new List<double>();

            //double inc = (InResponseDecisionMax - InResponseDecisionMin) / (InResponseDecisionIncCount);
            
        }
        //this function checks to see if the user is loading a saved file in which data was censored. If so, the flag is turned on
        private void CheckForLoadFileWithCensoredData()
        {
            //NOTE: forcing data to always be reloaded on quick analysis for every analysis cause this makes no sense for me (TRB, 03-06-2025)

            //if (Data.HMAnalysisObject != null)
            //{
            //    if (_data.HMAnalysisObject.ExcludedFlaws.Count != 0 && AnalysisDataType == AnalysisDataTypeEnum.HitMiss)
            //    {
            //        _fileLoadHitMiss = true;
            //    }
            //}
            //if (Data.AHATAnalysisObject != null)
            //{
            //    if (_data.AHATAnalysisObject.ExcludedFlaws.Count != 0 && AnalysisDataType == AnalysisDataTypeEnum.AHat)
            //    {
            //        _fileLoadHitMiss = true;
            //    }
            //}
        }
        public void UpdateRTransforms()
        {
            // Pass the transforms into the AnalysisData class to update the HM/AHat AnlysisObject()
            Data.FlawTransform = InFlawTransform;
            Data.ResponseTransform = InResponseTransform;

            if (AnalysisDataType == AnalysisDataTypeEnum.HitMiss)
            {
                Data.HMAnalysisObject.ModelType = _python.TransformEnumToInt(Data.FlawTransform);
            }
            else
            {
                // Apply both transforms to AHat and update the model type
                Data.AHATAnalysisObject.A_transform = _python.TransformEnumToInt(Data.FlawTransform);
                Data.AHATAnalysisObject.Ahat_transform = _python.TransformEnumToInt(Data.ResponseTransform);
                AHatModelUpdate();
            }
        }
        private void AHatModelUpdate()
        {
            //linear- linear
            if (Data.AHATAnalysisObject.A_transform == 1 && Data.AHATAnalysisObject.Ahat_transform == 1)
                Data.AHATAnalysisObject.ModelType = 1;
            //log - linear
            else if (Data.AHATAnalysisObject.A_transform == 2 && Data.AHATAnalysisObject.Ahat_transform == 1)
                Data.AHATAnalysisObject.ModelType = 2;
            //linear- log
            else if (Data.AHATAnalysisObject.A_transform == 1 && Data.AHATAnalysisObject.Ahat_transform == 2)
                Data.AHATAnalysisObject.ModelType = 3;
            // log - log
            else if (Data.AHATAnalysisObject.A_transform == 2 && Data.AHATAnalysisObject.Ahat_transform == 2)
                Data.AHATAnalysisObject.ModelType = 4;
            // linear - box-cox
            else if (Data.AHATAnalysisObject.A_transform == 1 && Data.AHATAnalysisObject.Ahat_transform == 5)
                Data.AHATAnalysisObject.ModelType = 5;
            // log - boxcox
            else if (Data.AHATAnalysisObject.A_transform == 2 && Data.AHATAnalysisObject.Ahat_transform == 5)
                Data.AHATAnalysisObject.ModelType = 6;
            // inverse - boxcox
            else if (Data.AHATAnalysisObject.A_transform == 3 && Data.AHATAnalysisObject.Ahat_transform == 5)
                Data.AHATAnalysisObject.ModelType = 7;
            // linear - inverse
            else if (Data.AHATAnalysisObject.A_transform == 1 && Data.AHATAnalysisObject.Ahat_transform == 3)
                Data.AHATAnalysisObject.ModelType = 8;
            // log - inverse
            else if (Data.AHATAnalysisObject.A_transform == 2 && Data.AHATAnalysisObject.Ahat_transform == 3)
                Data.AHATAnalysisObject.ModelType = 9;
            // inverse - linear
            else if (Data.AHATAnalysisObject.A_transform == 3 && Data.AHATAnalysisObject.Ahat_transform == 1)
                Data.AHATAnalysisObject.ModelType = 10;
            // inverse - log
            else if (Data.AHATAnalysisObject.A_transform == 3 && Data.AHATAnalysisObject.Ahat_transform == 2)
                Data.AHATAnalysisObject.ModelType = 11;
            // inverse x - inverse y
            else if (Data.AHATAnalysisObject.A_transform == 3 && Data.AHATAnalysisObject.Ahat_transform == 3)
                Data.AHATAnalysisObject.ModelType = 12;
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

        #endregion

        public override void SetPythonEngine(I_IPy4C myPy)
        {
            _python = myPy;     
            _data.SetPythonEngine(_python, Name);
            if (Data.DataType == AnalysisDataTypeEnum.AHat && Data.AHATAnalysisObject != null)
            {
                Data.AHATAnalysisObject.Lambda = InLambdaValue;
            }
        }
        public override void SetREngine(IREngineObject myREngine)
        {
            _rDotNet = myREngine;
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

        public void WriteQuickAnalysis(IExcelExport myWriter, DataTable myInputTable, string myOperator, string mySpecimentSet, string mySpecUnits, double mySpecMin, double mySpecMax,
                                       string myInstrument = "", string myInstUnits = "", double myInstMin = 0.0, double myInstMax = 1.0)
        {
            myWriter.Workbook.AddWorksheet($"{WorksheetName} Input".Trim());

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

            myWriter.Workbook.AddWorksheet($"{WorksheetName} Results".Trim());

            myWriter.SetCellValue(rowIndex++, colIndex, "Analysis Name");

            rowIndex++;

            myWriter.SetCellValue(rowIndex++, colIndex, "FLAW:");
            myWriter.SetCellValue(rowIndex++, colIndex, "Flaw Range Min");
            myWriter.SetCellValue(rowIndex++, colIndex, "Flaw Range Max");

            if (AnalysisDataType == AnalysisDataTypeEnum.AHat)
            {
                myWriter.SetCellValue(rowIndex++, colIndex, "Flaw Count");
            }
            else if (AnalysisDataType == AnalysisDataTypeEnum.HitMiss)
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
                myWriter.SetCellValue(rowIndex - 1, colIndex + 2, "Standard Error");
                myWriter.SetCellValue(rowIndex++, colIndex, "Intercept");
                myWriter.SetCellValue(rowIndex++, colIndex, "Slope");
                myWriter.SetCellValue(rowIndex++, colIndex, "Residual Error");
                myWriter.SetCellValue(rowIndex++, colIndex, "R-Squared Value");
                myWriter.SetCellValue(rowIndex++, colIndex, "Mu");
                myWriter.SetCellValue(rowIndex++, colIndex, "Sigma");
            }
            else if (AnalysisDataType == AnalysisDataTypeEnum.HitMiss)
            {
                myWriter.SetCellValue(rowIndex++, colIndex, "Mu");
                myWriter.SetCellValue(rowIndex++, colIndex, "Sigma");
            }

            if (AnalysisDataType == AnalysisDataTypeEnum.HitMiss)
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
                myWriter.SetCellValue(rowIndex++, colIndex, "Normality (Shapiro-Wilk)");
                myWriter.SetCellValue(rowIndex++, colIndex, "Equal Variance (Breusch-Pagan test)");
                myWriter.SetCellValue(rowIndex++, colIndex, $"Whole Model Test");

                rowIndex++;
            }

            if (AnalysisDataType == AnalysisDataTypeEnum.HitMiss)
            {
                myWriter.SetCellValue(rowIndex++, colIndex, "ANALYSIS MODEL TESTS:");
                myWriter.SetCellValue(rowIndex++, colIndex, "McFadden's Pseudo R-Squared");

                // --- ADDED AIC LABEL HERE ---
                myWriter.SetCellValue(rowIndex++, colIndex, "AIC (Akaike Information Criterion)");

                rowIndex++;
            }



            myWriter.SetCellValue(rowIndex++, colIndex, "POD PERCENTILE ESTIMATES:");
            myWriter.SetCellValue(rowIndex++, colIndex, "A50 Value");
            myWriter.SetCellValue(rowIndex++, colIndex, $"Level Value (A{InPODLevel})");
            myWriter.SetCellValue(rowIndex++, colIndex, $"Confidence Value  (A{InPODLevel}_{InPODConfidence})");

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
                myWriter.SetCellValue(rowIndex++, colIndex + 1, OutModelSlopeStdError);
                myWriter.SetCellValue(rowIndex, colIndex, OutModelResidualError);
                myWriter.SetCellValue(rowIndex++, colIndex + 1, OutModelResidualErrorStdError);
                myWriter.SetCellValue(rowIndex++, colIndex, OutRSquaredValue);
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

            if (AnalysisDataType == AnalysisDataTypeEnum.HitMiss)
            {
                // Skip the "ANALYSIS MODEL TESTS:" label
                rowIndex++;

                // Write the Pseudo R-Squared Value and the Rating
                myWriter.SetCellValue(rowIndex, colIndex, OutTestLackOfFit);
                myWriter.SetCellValue(rowIndex++, colIndex + 1, OutTestLackOfFitRating);

                // --- ADDED AIC VALUE HERE ---
                // Notice it writes to 'colIndex' (Column 2) and leaves 'colIndex+1' blank.
                myWriter.SetCellValue(rowIndex++, colIndex, OutTestAIC);

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
            myWriter.Workbook.AddWorksheet($"{WorksheetName} Info".Trim());

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
                //only output the lambda value is boxcox transform was selected
                if (InResponseTransform == TransformTypeEnum.BoxCox)
                {
                    myWriter.SetCellValue(rowIndex++, colIndex, "Lambda Value (Box-Cox only)");
                }
            }

            rowIndex++;

            myWriter.SetCellValue(rowIndex++, colIndex, "ANALYSIS:");
            myWriter.SetCellValue(rowIndex++, colIndex, "Time Stamp");
            myWriter.SetCellValue(rowIndex++, colIndex, "Analysis Type");

            if (AnalysisDataType == AnalysisDataTypeEnum.HitMiss)
            {
                myWriter.SetCellValue(rowIndex++, colIndex, "Confidence Interval Type");
                myWriter.SetCellValue(rowIndex++, colIndex, "Sampling Type");
                myWriter.SetCellValue(rowIndex++, colIndex, "Regression Type");
            }
            else if (AnalysisDataType == AnalysisDataTypeEnum.AHat)
            {
                myWriter.SetCellValue(rowIndex++, colIndex, "POD Threshold");
            }

            myWriter.SetCellValue(rowIndex++, colIndex, "POD Level");
            myWriter.SetCellValue(rowIndex++, colIndex, "POD Confidence");

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
                if (InResponseTransform == TransformTypeEnum.BoxCox)
                {
                    myWriter.SetCellValue(rowIndex++, colIndex, Data.LambdaValue);
                }
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
            if (AnalysisDataType == AnalysisDataTypeEnum.HitMiss)
            {
                myWriter.SetCellValue(rowIndex++, colIndex, InConfIntervalType.ToString());
                myWriter.SetCellValue(rowIndex++, colIndex, InSamplingType.ToString());
                myWriter.SetCellValue(rowIndex++, colIndex, InHitMissModel.ToString());
            }
            
            myWriter.SetCellValue(rowIndex++, colIndex, InPODLevel);
            myWriter.SetCellValue(rowIndex++, colIndex, InPODConfidence);

            myWriter.Workbook.AutoFitColumn(1, 2);

            WriteAnalysisName(myWriter, WorksheetName.Length > 0);
        }

        private void WriteAnalysisName(ExcelExport myWriter, bool myPartOfProject)
        {

            myWriter.SetCellValue(1, 2, Name);
        }

        public bool SeparatedFlag
        {
            get { return _isSeparatedFlag; }
        }
        public bool FailToConverge
        {
            get { return _failedToConverge; }
        }

        public double OutPFCovarianceV11 { get; set; }

        public double OutPFCovarianceV12 { get; set; }

        public double OutPFCovarianceV22 { get; set; }

        public double OutPODMu { get; set; }

        public double OutPODSigma { get; set; }
        
        private TransformTypeEnum _xAxisTransformIn  = TransformTypeEnum.Linear;

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

                    RemakeDisplayTransformer();
                }
            }
        }

        private void RemakeDisplayTransformer()
        {
            // For non-BoxCox transforms, we don't need real BoxCox results, but a DisplayTransformer is still needed for UI consistency.
            // If BoxCoxResult is null (because a Box-Cox analysis hasn't been run), create a dummy one to satisfy the constructor.
            // This ensures the DisplayTransformer is always updated when the transform type changes.
            var resultForTransformerY = BoxCoxResultY ?? new BoxCoxAnalysisResult();
            var resultForTransformerX = BoxCoxResultX ?? new BoxCoxAnalysisResult();

            if (Transformer == null)
            {
                Transformer = new DataTransformer(resultForTransformerY, _yAxisTransformIn, _data.SmallestResponse, _data.LargestResponse,
                                                  resultForTransformerX, _xAxisTransformIn, _data.SmallestFlaw, _data.LargestFlaw);
                _data.SetDisplayTransformer(Transformer);
            }
            else
            {
                Transformer.UpdateAxisTransformParameters(resultForTransformerY, _yAxisTransformIn, _data.SmallestResponse, _data.LargestResponse, 
                                                          resultForTransformerX, _xAxisTransformIn, _data.SmallestFlaw, _data.LargestFlaw);
            }
        }

        public double InLambdaValue
        {
            set
            { 
                if (Data.AHATAnalysisObject != null)
                    Data.AHATAnalysisObject.Lambda = value; 
            }
            get
            {
                if (Data.AHATAnalysisObject != null)
                    return Data.AHATAnalysisObject.Lambda;
                else
                    return 1.0;
            }
        }

        public void UpdateBoxCoxLambda(double newLambda)
        {
            if (BoxCoxResultY == null)
            {
                BoxCoxResultY = new BoxCoxAnalysisResult();
            }

            InLambdaValue = newLambda;
            BoxCoxResultY.Lambda = newLambda;

            if (Data?.AHATAnalysisObject != null)
            {
                Data.AHATAnalysisObject.Lambda = newLambda;
            }

            // The LazyBoxCoxTransformer will be created if it hasn't been already.
            if (_lazyBoxCoxYTransformer.IsValueCreated)
            {
                LazyYBoxCoxTransformer.RecalculateK1(BoxCoxResultY, newLambda);
            }

            RemakeDisplayTransformer();
        }
        public RCalculationType AnalysisCalculationType
        {
            set { _analysisCalculationType = value; }
            get { return _analysisCalculationType; }
        }

        private double _inFlawCalcMax = 0.0;

        public double InFlawCalcMax
        {
            get { return _inFlawCalcMax; }
            set { if(!IsFrozen) _inFlawCalcMax = InvertTransformValueForXAxis(value); }
        }
        private double _inFlawCalcMin = 0.0;
        [NonSerialized]
        private bool stillRunningAnalysis = false;

        public double InFlawCalcMin
        {
            get { return _inFlawCalcMin; }
            set { if (!IsFrozen) _inFlawCalcMin = InvertTransformValueForXAxis(value); }
        }

        private TransformTypeEnum _yAxisTransformIn = TransformTypeEnum.Linear;

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

                    RemakeDisplayTransformer();
                }
            }
        }

        public double TransformValueForXAxis(double myValue)
        {
            return Convert.ToDouble(TransformValueForXAxis(Convert.ToDecimal(myValue)));
        }

        public double TransformXDataValue(double myValue) => Convert.ToDouble(TransformXDataValue(Convert.ToDecimal(myValue)));

        public decimal TransformXDataValue(decimal myValue)
        {
            if (this.Transformer != null)
            {
                return this.Transformer.TransformXDataValue(myValue);
            }
            return myValue;
        }

        public decimal TransformValueForXAxis(decimal myValue)
        {
            if (this.Transformer != null)
            {
                return this.Transformer.TransformXAxisValue(myValue);
            }
            return myValue;
        }

        public double TransformValueForYAxis(double myValue)
        {
            return Convert.ToDouble(TransformValueForYAxis(Convert.ToDecimal(myValue)));
        }
        // the lambdaValueInput arguement is added in order to unit test this method
        public decimal TransformValueForYAxis(decimal myValue)
        {
            if (this.Transformer != null)
            {
                return this.Transformer.TransformYAxisValue(myValue);
            }
            
            return myValue;        }

        public double InvertTransformValueForXAxis(double myValue)
        {
            return Convert.ToDouble(InvertTransformValueForXAxis(Convert.ToDecimal(myValue)));
        }
        public decimal InvertTransformValueForXAxis(decimal myValue)
        {
            if (this.Transformer != null)
            {
                return this.Transformer.InverseTransformXAxisValue(myValue);
            }
            return myValue;
        }

        public decimal InvertTransformValueForYAxis(decimal myValue)
        {
            if (this.Transformer != null)
            {
                return this.Transformer.InverseTransformYAxisValue(myValue);
            }
            return myValue;        
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

        public void RunOnlyFitAnalysis(IAnalysisBackendControl backendAnalysisControl=null)
        {

            CopyInputToR();

            try
            {
                if(AnalysisDataType == AnalysisDataTypeEnum.HitMiss)
                {
                    IAnalysisBackendControl newAnalysisControlHM = backendAnalysisControl ?? new AnalysisBackendControl(_rDotNet, Data.HMAnalysisObject, null, Transformer);
                    if (InFlawTransform != TransformTypeEnum.Inverse)
                    {
                        newAnalysisControlHM.ExecuteAnalysisTransforms_HM();
                    }
                    else
                    {
                        newAnalysisControlHM.ExecuteReqSampleAnalysisTypeHitMiss();
                    }
                    Data.HMAnalysisObject = newAnalysisControlHM.HMAnalsysResults;
                }
                else
                {
                    IAnalysisBackendControl newAnalysisControlAHat = backendAnalysisControl ?? new AnalysisBackendControl(_rDotNet, null, Data.AHATAnalysisObject, Transformer);
                    newAnalysisControlAHat.ExecuteAnalysisTransforms();
                    Data.AHATAnalysisObject = newAnalysisControlAHat.AHatAnalysisResults;
                }
            }
            catch(Exception executeProblemAnalysis)
            {
                Debug.WriteLine(executeProblemAnalysis.ToString());
            }

            CopyOutputFromR();

            stillRunningAnalysis = false;

        }


        public void ClearInitialGuesses()
        {
            _initialResponseDecision = Double.MaxValue;
            _initialResponseMaxGuess = Double.MaxValue;
            _initialResponseMinGuess = Double.MaxValue;
        }
        //get the min flaw size for inverse
        public double MinFlawSize
        {
            get { return Data.HMAnalysisObject.Flaws_All.Min(); }
        }
        //quick analysis units
        public string Operator { get; set; }

        public string SpecimenSet { get; set; }

        public string SpecimentUnit { get; set; }

        public string Instrument { get; set; }

        public string InstrumentUnit { get; set; }
        /// <summary>
        /// the setter and getter for error present in analysis flag
        /// </summary>
        /// <returns></returns>
        public bool ErrorInAnalysis
        {
            get { return _errorPresentInAnalysis; }
            set { _errorPresentInAnalysis = value; }   
        }

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
            return $"{Operator} - {SpecimenSet} - {Instrument} - {Data.DataType}";
        }

        public string CreateQuickHitMissAnalysisName()
        {
            return $"{Operator} - {SpecimenSet} - {Data.DataType}";
        }

        public void ClearAllEvents()
        {
            if(_python != null)
            {
                AnalysisDone = null;
                EventsNeedToBeCleared?.Invoke(this, null);
            }
        }

        public void GetProjectInfo(out string fileName, out DataTable data)
        {
            
            var args = new GetProjectInfoArgs(SourceName);

            GetProjectInfo(args);

            string projFileName = "";

            projFileName = args.ProjectFileName;

            fileName = $"{projFileName} - {Name}";
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
            ExportProject?.Invoke(this, null);
        }

        public string ToolTipText
        {
            get
            {
                string name = Name.Length > 40 ? $"{Name.Substring(0, 40)}..." : Name;
                string text = $"{name}\n";
                text += $"Flaw Transform: {InFlawTransform}\n";

                if (Data.DataType == AnalysisDataTypeEnum.AHat)
                {
                    text += $"Response Transform: {InResponseTransform}\n";

                    if (InResponseTransform == TransformTypeEnum.BoxCox)
                        text += $"Lambda Value:\t{InLambdaValue}\n\n";

                    text += $"POD Decision:\t{InResponseDecision:F3} {ResponseUnits[0]}\n";
                    text += $"a50:\t\t{OutResponseDecisionPODA50Value:F3} {FlawUnit}\n";
                    text += $"a90:\t\t{OutResponseDecisionPODLevelValue:F3} {FlawUnit}\n";
                    text += $"a90/95:\t\t{OutResponseDecisionPODConfidenceValue:F3} {FlawUnit}\n";
                }
                else
                {
                    text += $"POD Model:\t{InHitMissModel}\n";
                    text += $"Conf Int Type:\t{InConfIntervalType}\n";
                    text += $"Sampling Type:\t{InSamplingType}\n\n";
                    text += $"a50:\t{OutResponseDecisionPODA50Value:F3} {FlawUnit}\n";
                    text += $"a90:\t{OutResponseDecisionPODLevelValue:F3} {FlawUnit}\n";
                    text += $"a90/95:\t{OutResponseDecisionPODConfidenceValue:F3} {FlawUnit}\n";
                }
                return text;
            }
        }

        public IBoxCoxTransformer LazyYBoxCoxTransformer
        {
            get
            {
                return _lazyBoxCoxYTransformer.Value;
            }
        }

        public IBoxCoxTransformer LazyXBoxCoxTransformer
        {
            get
            {
                return _lazyBoxCoxXTransformer.Value;
            }
        }

        public string ShortName
        {
            get
            {
                var shortName = Name;

                if(shortName.StartsWith(SourceName))
                    shortName = shortName.Substring(SourceName.Length+1);
                if (shortName.StartsWith(FlawName))
                    shortName = shortName.Substring(FlawName.Length + 1);

                return shortName;
            }
        }

        public bool UsingQuickAnalysis
        {
            get
            {
                return _quickAnalysis;
            }

            set
            {
                _quickAnalysis = value;
            }
        }

        public void RaiseCreatedAnalysis(Analysis clone)
        {
            if(CreatedAnalysis != null)
            {
                AnalysisList analyses = new AnalysisList() { clone };
                var args = new AnalysisListArg(ref analyses);
                CreatedAnalysis.Invoke(this, args);
            }
        }
    }
        
}