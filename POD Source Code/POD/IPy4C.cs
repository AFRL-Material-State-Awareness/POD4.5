using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Utils;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Data;
using IronPython.Hosting;
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
        //count the number of tables ********USED FOR DEBUGGING
        public int tableNum = 0;
        ScriptEngine _pyEngine;
        List<string> _modules;
        Dictionary<string, ScriptScope> _pyScopes;
        Dictionary<string, dynamic> _cpDocs;
        Dictionary<string, HMAnalysisObjectTransform> _hitMissAnalyses;
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
                //if (_statusBar != null)
                //    return _statusBar.ProgressValue;
                //else
                    return _simpleProgress;
            }
            set
            {
                //if (_statusBar != null)
                //    _statusBar.ProgressValue = value;
                //else
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
                //if (_statusBar != null)
                //    return _statusBar.ProgressText;
                //else
                    return _simpleStatus;
            }
            set
            {
                //if (_statusBar != null)
                //    _statusBar.ProgressText = value;
               // else
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
            //if (_statusBar != null)
            //    _statusBar.ResetErrorText();
            //else
                _simpleError = "";
        }

        public EventRaisingStreamWriter ErrorWriter
        {
            get { return _errorWriter; }
        }

        public IPy4C(PyTypeEnum myType, bool myDebug)//, PODStatusBar myBar)
        {
            _outputStream = new MemoryStream();
            _errorStream = new MemoryStream();

            _outputWriter = new EventRaisingStreamWriter(_outputStream);
            _errorWriter = new EventRaisingStreamWriter(_errorStream);

            //if (myBar != null)
            //    _statusBar = myBar;

            //string assembly = Path.GetFullPath("IronPython.dll");
            //Assembly ptrAssembly = Assembly.LoadFile(assembly);

            Dictionary<string, object> options = new Dictionary<string, object>();
            options["Debug"] = myDebug;
            _pyEngine = Python.CreateEngine(options);
            //_pyEngine = Python.CreateEngine();

            _pyEngine.Runtime.IO.SetOutput(_outputStream, _outputWriter);
            _pyEngine.Runtime.IO.SetErrorOutput(_errorStream, _errorWriter);

            _modules = new List<string>();
            _pyScopes = new Dictionary<string, ScriptScope>();
            

            _cpDocs = new Dictionary<string, dynamic>();
            //used to store the hitmiss analyses
            _hitMissAnalyses = new Dictionary<string, HMAnalysisObjectTransform>();
            //if the .dll is being used in the program
            if (myType == PyTypeEnum.DLLFiles)
            {
                var pathFull = Path.GetFullPath("POD_All.dll");

                Assembly pod = Assembly.LoadFile(pathFull);
                _pyEngine.Runtime.LoadAssembly(pod);
                _pyScopes.Add("CPodDoc", _pyEngine.Runtime.ImportModule("CPodDoc"));
            }
            //if just the python files are being used to run the program
            else if(myType == PyTypeEnum.PyFiles)
            {                
                string pyFilesDir = Path.GetFullPath("..\\..\\..\\TestingPythonCode\\");
                string pyLib = Path.GetFullPath("..\\..\\..\\PythonEnvironment\\Lib\\");
                string numerics = Path.GetFullPath("..\\..\\..\\..\\packages\\MathNet.Numerics.3.6.0\\lib\\net40\\");
                _pyEngine.SetSearchPaths(new string[] {pyFilesDir, pyLib, numerics});


                //add each module in the project
                _modules.Add("FileLogger");
                _modules.Add("alogam");
                _modules.Add("gammds");
                _modules.Add("betain");
                _modules.Add("lookup_table");
                _modules.Add("new_pf");
                _modules.Add("bisect");
                _modules.Add("heapq");
                _modules.Add("keyword");
                _modules.Add("collections");
                _modules.Add("PODglobals");
                _modules.Add("smtxinv");
                _modules.Add("PODaccessories");
                _modules.Add("numbers");
                _modules.Add("decimal");
                _modules.Add("_weakrefset");
                _modules.Add("abc");
                _modules.Add("_abcoll");
                _modules.Add("UserDict");
                _modules.Add("weakref");
                _modules.Add("types");
                _modules.Add("copy");
                _modules.Add("alnorm");
                _modules.Add("nrmden");
                _modules.Add("phinv");
                _modules.Add("CPodDoc");
                _modules.Add("linreg");
                _modules.Add("mdnord");
                _modules.Add("sysolv");
                _modules.Add("__future__");
                _modules.Add("fcn");
                _modules.Add("leqslv");
                _modules.Add("funcr");
                _modules.Add("qsort");
                //additional modules for modifications
                _modules.Add("Compare");
                //iterate through each module to add the scope and path to the _pyscope dictionary
                foreach (string path in _modules)
                {
                    string file = "";

                    if (!path.EndsWith(".dll"))
                    {
                        file = pyFilesDir + path + ".py";

                        if (File.Exists(file) == false)
                            file = pyLib + path + ".py";
                    }
                    else
                    {
                        file = numerics + path;
                    }

                    dynamic source = _pyEngine.CreateScriptSourceFromFile(file);
                    dynamic scope = _pyEngine.CreateScope();
                    dynamic op = _pyEngine.Operations;
                    source.Execute(scope);

                    _pyScopes.Add(path, scope);
                }

            }
            
        }

        public dynamic CInfo()
        {
            dynamic CInfo = _pyScopes["CPodDoc"].GetVariable("CInfo");
            dynamic cinfo = CInfo();

            cinfo.row = 10;

            return cinfo;
        }
        //function is used to take a new analsys and make an instance in the cpoddoc class in the python backend
        public dynamic CPodDoc(string myAnalysisName)
        {
            //if analysis name doesn't alredy exist in the dictionary create a new one
            if (_cpDocs.ContainsKey(myAnalysisName) == false)
            {
                //get the python class CPodDoc object from CPodDoc.py through pyScopes
                dynamic CPodDoc = _pyScopes["CPodDoc"].GetVariable("CPodDoc");
                //this is  where the new instance is created
                //If we make separate instances for signal reponse and hit/miss, we could create if statements here
                dynamic cpoddoc = CPodDoc();
                //create the new HM analsysis object

                //the dictionary contains a string as the key and the cPoddoc class object as its value/definition
                _cpDocs.Add(myAnalysisName, cpoddoc);

                return cpoddoc;
            }
            //if it does, simply return that def/key pair from the _cpDocs dictionary
            else
            {
                return _cpDocs[myAnalysisName];
            }
        }
        //object used for hit miss analyses
        public HMAnalysisObjectTransform HitMissAnalsysis(string myAnalysisName)
        {
            //if analysis name doesn't alredy exist in the dictionary create a new one
            if (_hitMissAnalyses.ContainsKey(myAnalysisName) == false)
            {
                //create a new hitmiss analysis object
                HMAnalysisObjectTransform hitMissAnalsyis = new HMAnalysisObjectTransform(myAnalysisName);
                _hitMissAnalyses.Add(myAnalysisName, hitMissAnalsyis);
                return (hitMissAnalsyis);
            }
            //if it does, simply return that def/key pair from the _cpDocs dictionary
            else
            {
                return _hitMissAnalyses[myAnalysisName];
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
        //convert c# dictionary to python dictionary
        //my dictionary is the c# dictionary
        public dynamic DotNetToPythonDictionary(Dictionary<string, List<double>> myDictionary)
        {
            /*dynamic dict = _pyScopes["CPodDoc"].GetVariable("PDictionary")
            dynamic dictionary = dict();*/

            IronPython.Runtime.PythonDictionary dictionary = new IronPython.Runtime.PythonDictionary();
            IronPython.Runtime.List list;
            //copy the values into the new python dictionary
            foreach(string key in myDictionary.Keys)
            {
                list = new IronPython.Runtime.List();

                foreach(double value in myDictionary[key])
                {
                    list.Add(value);
                }

                dictionary[key] = list;
            }

            return dictionary;
        }

        public List<double> PythonToDotNetList(dynamic myList)
        {
            IronPython.Runtime.List list = myList;

            List<double> newList = new List<double>();

            foreach (object value in list)
            {
                newList.Add(Convert.ToDouble(value));
            }

            return newList;
        }

        public DataTable PythonDictionaryToDotNetNumericTable(dynamic myList)
        {
            IronPython.Runtime.PythonDictionary list = myList;

            DataTable table = new DataTable();
            List<IronPython.Runtime.List> data = new List<IronPython.Runtime.List>();

            var names = list["list_names"] as IronPython.Runtime.List;

            foreach(string name in names)
            {
                table.Columns.Add(new DataColumn(name, typeof(Double)));
                data.Add((IronPython.Runtime.List)list[name]);
            }

            int count = data[0].Count;
            object[] row = new object[table.Columns.Count];
            int colIndex = 0;
            int rowIndex = 0;

            var watch = new Stopwatch();

            watch.Start();

            for (int i = 0; i < count; i++)
            {
                table.Rows.Add(row);
            }

            watch.Stop();

            var addRowTime = watch.ElapsedMilliseconds;


            watch.Restart();

            foreach(IronPython.Runtime.List col in data)
            {
                rowIndex = 0;

                foreach (object value in col)
                {
                    if (rowIndex < table.Rows.Count)
                    {
                        table.Rows[rowIndex][colIndex] = Convert.ToDouble(value);
                        rowIndex++;
                    }
                }

                colIndex++;
            }

            watch.Stop();

            var copyRowTime = watch.ElapsedMilliseconds;

            //This produces the tables that appear in the user interface
            tableNum += 1;
            //print datatable from python
            //Debug.WriteLine(table.Rows[rowIndex][colIndex].ToString());
            Debug.WriteLine("This datatable is table" + tableNum.ToString());
            Debug.WriteLine("The data table has " + table.Rows.Count.ToString() + " rows");
            Debug.WriteLine("The data table has " + table.Columns.Count.ToString() + " Columns");
            printDT(table);
            //MessageBox.Show("ADD ROW: " + addRowTime + " COPY ROW: " + copyRowTime);

            return table;
        }

        public void Close()
        {
            _modules.Clear();
            _pyScopes.Clear();
            _cpDocs.Clear();
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
                _pyEngine.Runtime.Shutdown();
            }
            catch
            {

            }
        }
        //This method is for debugging purpose
        //should be removed in the final product
        static void printDT(DataTable data)
        {
            //Console.WriteLine();
            Debug.WriteLine('\n');
            Dictionary<string, int> colWidths = new Dictionary<string, int>();

            foreach (DataColumn col in data.Columns)
            {
                //Console.Write(col.ColumnName);
                Debug.Write(col.ColumnName);
                var maxLabelSize = data.Rows.OfType<DataRow>()
                        .Select(m => (m.Field<object>(col.ColumnName)?.ToString() ?? "").Length)
                        .OrderByDescending(m => m).FirstOrDefault();

                colWidths.Add(col.ColumnName, maxLabelSize);
                for (int i = 0; i < maxLabelSize - col.ColumnName.Length + 10; i++) Debug.Write(" ");
            }

            //Console.WriteLine();
            Debug.WriteLine('\n');
            int rowCounter = 0;
            int limit = 100;
            foreach (DataRow dataRow in data.Rows)
            {
                for (int j = 0; j < dataRow.ItemArray.Length; j++)
                {
                    //Console.Write(dataRow.ItemArray[j]);
                    Debug.Write((dataRow.ItemArray[j]).ToString());
                    for (int i = 0; i < colWidths[data.Columns[j].ColumnName] - dataRow.ItemArray[j].ToString().Length + 10; i++) Debug.Write(" ");
                }
                //Console.WriteLine();
                Debug.WriteLine('\n');
                rowCounter = rowCounter + 1;
                if (rowCounter >= limit)
                {
                    break;
                }
            }
            Debug.WriteLine('\n');
        }




        public int FailedRunCount { get; set; }
    }
}
