using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.Scripting.Hosting;
//using Microsoft.Scripting.Utils;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Data;
//using IronPython.Hosting;
using System.Diagnostics;
using CSharpBackendWithR;
namespace POD
{
    public enum PyTypeEnum
    {
        PyFiles,
        DLLFiles
    }

    public class IPy4C
    {
        Dictionary<string, HMAnalysisObject> _hitMissAnalyses;
        Dictionary<string, AHatAnalysisObject> _ahatAnalyses;
        MemoryStream _outputStream;
        MemoryStream _errorStream;
        EventRaisingStreamWriter _outputWriter;
        //PODStatusBar _statusBar;
        public AnalysisErrorHandler OnAnalysisError;
        public AnalysisErrorHandler OnProgressUpdate;
        public AnalysisErrorHandler OnAnalysisFinish;
        public string CurrentAnalysisName;
        /// <summary>
        /// A way to at least record status even if the status bar isn't being used.
        /// </summary>
        string _simpleStatus = "";
        int _simpleProgress = 0;
        string _simpleError = "";

        public EventRaisingStreamWriter OutputWriter
        {
            get { return _outputWriter; }
        }
        EventRaisingStreamWriter _errorWriter;

        public int ProgressOutOf100
        {
            get
            {
                return _simpleProgress;
            }
            set
            {
                _simpleProgress = value;

                if (OnProgressUpdate != null)
                {
                    OnProgressUpdate.Invoke(this, new ErrorArgs(CurrentAnalysisName, value));
                }
            }
        }

        public string ProgressText
        {
            get
            {
                return _simpleStatus;
            }
            set
            {
                _simpleStatus = value;
            }
        }

        public void NotifyFinishAnalysis()
        {
            if (OnAnalysisFinish != null)
            {
                OnAnalysisFinish.Invoke(this, new ErrorArgs(CurrentAnalysisName, ""));
            }
        }

        public void AddErrorText(string myError)
        {
             _simpleError = _simpleError + "; " + myError;

            if(OnAnalysisError != null)
            {
                OnAnalysisError.Invoke(this, new ErrorArgs(CurrentAnalysisName, myError));
            }
        }

        public void ClearErrorText()
        {
            _simpleError = "";
        }

        public EventRaisingStreamWriter ErrorWriter
        {
            get { return _errorWriter; }
        }

        public IPy4C()//, PODStatusBar myBar)
        {
            _outputStream = new MemoryStream();
            _errorStream = new MemoryStream();

            _outputWriter = new EventRaisingStreamWriter(_outputStream);
            _errorWriter = new EventRaisingStreamWriter(_errorStream);

            //_cpDocs = new Dictionary<string, dynamic>();
            //used to store the hitmiss analyses
            _hitMissAnalyses = new Dictionary<string, HMAnalysisObject>();
            //used to store ahat analyses
            _ahatAnalyses = new Dictionary<string, AHatAnalysisObject>();
        }
        //object used for hit miss analyses
        public HMAnalysisObject HitMissAnalsysis(string myAnalysisName)
        {
            //if analysis name doesn't alredy exist in the dictionary create a new one
            if (_hitMissAnalyses.ContainsKey(myAnalysisName) == false)
            {
                //create a new hitmiss analysis object
                HMAnalysisObject hitMissAnalsyis = new HMAnalysisObject(myAnalysisName);
                _hitMissAnalyses.Add(myAnalysisName, hitMissAnalsyis);
                return (hitMissAnalsyis);
            }
            //if it does, simply return that def/key pair from the _hitMissAnalyses dictionary
            else
            {
                return _hitMissAnalyses[myAnalysisName];
            }

        }
        public AHatAnalysisObject AHatAnalysis(string myAnalysisName)
        {
            //if analysis name doesn't alredy exist in the dictionary create a new one
            if (_ahatAnalyses.ContainsKey(myAnalysisName) == false)
            {
                //create a new hitmiss analysis object
                AHatAnalysisObject aHatAnalsyis = new AHatAnalysisObject(myAnalysisName);
                _ahatAnalyses.Add(myAnalysisName, aHatAnalsyis);
                return (aHatAnalsyis);
            }
            //if it does, simply return that def/key pair from the _ahatAnalyses dictionary
            else
            {
                return _ahatAnalyses[myAnalysisName];
            }
        }

        public IPy4C CreateDuplicate()
        {
            return this;
        }
        //This function converts the transform type into an integer and returns it
        //The transformtypeEnum is located in Globals.cs
        public int TransformEnumToInt(TransformTypeEnum myTransformType)
        {
            int transform = 0;

            switch(myTransformType)
            {
                case TransformTypeEnum.Linear:
                    transform = 1;
                    break;
                case TransformTypeEnum.Log:
                    transform = 2;
                    break;
                case TransformTypeEnum.Inverse:
                    transform = 3;
                    break;
                case TransformTypeEnum.Exponetial:
                    transform = 4;
                    break;
                case TransformTypeEnum.BoxCox:
                    transform = 5;
                    break;
                default:
                    transform = 1;
                    break;
            }

            return transform;
        }
        /*
        //returns the value for the respective data analyis type
        public int AnalysisDataTypeEnumToInt(AnalysisDataTypeEnum myAnalysisDataType)
        {
            int analysisDataType = 0;

            switch (myAnalysisDataType)
            {
                case AnalysisDataTypeEnum.AHat:
                    analysisDataType = 1;
                    break;
                case AnalysisDataTypeEnum.HitMiss:
                    analysisDataType = 2;
                    break;
                default:
                    analysisDataType = 0;
                    break;
            }

            return analysisDataType;
        }
        //used to determine if the normal or odds model is to be generated (it is normal by default)
        public int PFModelEnumToInt(PFModelEnum myModel)
        {
            int modelType = 0;

            switch (myModel)
            {
                case PFModelEnum.Normal:
                    modelType = 0;
                    break;
                case PFModelEnum.Odds:
                    modelType = 2;
                    break;
                default:
                    modelType = 0;
                    break;
            }

            return modelType;
        }
        */
        //used to output the decision value
        //self.choices = ["P > 0.1", "0.05 < P <= 0.1", ".025 < P <= 0.05",
        //                "0.01 < P <= .025", ".005 < P <= 0.01", "P <= .005", "Undefined"]
        public string GetPValueDecision(double myValue)
        {
            string decisionString = "";
            if(myValue > 0.1)
            {
                decisionString = "P > 0.1";
            }
            else if(myValue <= 0.1 && myValue > .05)
            {
                decisionString = "0.05 < P <= 0.1";
            }
            else if(myValue <= .05 && myValue > .025)
            {
                decisionString = ".025 < P <= 0.05";
            }
            else if(myValue <= .025 && myValue > .01)
            {
                decisionString = "0.01 < P <= .025";
            }
            else if (myValue <= .01 && myValue > .005)
            {
                decisionString = ".005 < P <= 0.01";
            }
            else if (myValue <= .005)
            {
                decisionString = "P <= .005";
            }
            else
            {
                decisionString = "Undefined";
            }
            return decisionString;
        }
        //used to calculate the nth root
        public static double NthRoot(double A, double root)
        {
            return Math.Pow(A, 1.0 / root);
        }
        //convert c# dictionary to python dictionary
        //my dictionary is the c# dictionary
        public void Close()
        {
            _hitMissAnalyses.Clear();
            try
            {
                _outputStream.Flush();
            }
            catch
            {

            }

            _outputStream.Close();

            try
            {
                _errorStream.Flush();
            }
            catch
            {

            }

            _errorStream.Close();

            try
            {
                _outputWriter.Flush();
            }
            catch
            {

            }

            try
            {
                _outputWriter.Close();
            }
            catch
            {

            }

            try
            {
                //_pyEngine.Runtime.Shutdown();
                
            }
            catch
            {

            }
        }
        public int FailedRunCount { get; set; }

        
    }
}
