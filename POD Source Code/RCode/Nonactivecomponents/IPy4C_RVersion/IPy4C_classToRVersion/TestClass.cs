using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.Scripting.Hosting;
//using Microsoft.Scripting.Utils;
using System.IO;
//using IronPython.Hosting;
using System.Reflection;
using System.Data;
using System.Diagnostics;
using RDotNet;

namespace IPy4C_classToRVersion
{
    public class IR4C
    {
        /*
        private int printThisNumber;
        private int a;
        private int b;
        public TestClass(int printThisNumber=1, int a=2, int b=3)
        {
            this.printThisNumber = printThisNumber;
            this.a = a;
            this.b = b;
        }
        public void AddAandB()
        {
            this.printThisNumber = this.a + this.b;
            Console.WriteLine(this.printThisNumber);
        }
        */
        //ScriptEngine _pyEngine;
        List<string> _modules;
        //Dictionary<string, ScriptScope> _pyScopes;
        Dictionary<string, dynamic> _cpDocs;
        MemoryStream _outputStream;
        MemoryStream _errorStream;
        EventRaisingStreamWriter _outputWriter;
        //PODStatusBar _statusBar;
        //public AnalysisErrorHandler OnAnalysisError;
        //public AnalysisErrorHandler OnProgressUpdate;
        //public AnalysisErrorHandler OnAnalysisFinish;
        public string CurrentAnalysisName;
        /// <summary>
        /// A way to at least record status even if the status bar isn't being used.
        /// </summary>
        string _simpleStatus = "";
        int _simpleProgress = 0;
        string _simpleError = "";

        private static REngine initializeRDotNet()
        {
            //initialize the R engine
            REngine.SetEnvironmentVariables();
            REngine engine = REngine.GetInstance();
            engine.Initialize();
            return engine;

        }


        public EventRaisingStreamWriter OutputWriter
        {
            get { return _outputWriter; }
        }
        EventRaisingStreamWriter _errorWriter;

        public int ProgressOutof100
        {
            get
            {
                return _simpleProgress;
            }
            set
            {
                _simpleProgress = value;

               // if (OnProgressUpdate != null)
                //{
                //    OnProgressUpdate.Invoke(this, new ErrorArgs(CurrentAnalysisName, value));
                //}
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
            //if (OnAnalysisFinish != null)
            //{
              //  OnAnalysisFinish.Invoke(this, new ErrorArgs(CurrentAnalysisName, ""));
            //}
        }
        //This function converts the transform type into an integer and returns it
        //The transformtypeEnum is located in Globals.cs
        /*
        public int TransformEnumToInt(TransformTypeEnum myTransformType)
        {
            int transform = 0;

            switch (myTransformType)
            {
                case TransformTypeEnum.Linear:
                    transform = 1;
                    break;
                case TransformTypeEnum.Log:
                    transform = 2;
                    break;
                case TransformTypeEnum.Inverse:
                    transform = 4;
                    break;
                case TransformTypeEnum.Exponetial:
                    transform = 3;
                    break;
                default:
                    transform = 1;
                    break;
            }

            return transform;
        }
        */


    }
}
