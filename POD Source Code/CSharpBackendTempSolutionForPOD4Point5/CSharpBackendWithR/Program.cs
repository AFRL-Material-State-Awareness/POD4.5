
using System;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Globalization;
using RDotNet;
//used for maxmin function
using System.Linq;
namespace CSharpBackendWithR
{
    class Program
    {
        static void Main(string[] args)
        {
            //REngineObject newREngine = new REngineObject();
            //newREngine.InitializeRScripts();
            //string csvInputPathDF = @"C:\Users\gohmancm\Desktop\PODv4Point5FullProjectFolder\CSharpBackendTempSolution\CSharpBackendTempSolutionForPOD4Point5\RCode\RBackend\HitMissInfo_BadLL.csv";
            //string csvInputPathDF2 = @"C:\Users\gohmancm\Desktop\PODv4Point5FullProjectFolder\PODv4Point5Attemp1\PODv4\POD Source Code\CSharpBackendTempSolutionForPOD4Point5\RCode\RBackend\SignalResponseCode\dataFromPlots.csv";
            //DataTable mydt= GetDataTableFromCsv(csvInputPathDF);
            //printDT(mydt);
            //DataTable myAhat = GetDataTableFromCsv(csvInputPathDF2);
            //printDT(myAhat);
            //GetDataFromPFTransform(newREngine, mydt);
            //GetDataFromAHatTransform(newREngine, myAhat);
            ///
            /// These values are attrifial and inserted to test the backend
            ///
            //newAnalysis

        }
        /*
        public static void GetDataFromPFTransform(REngineObject analysisEngine, DataTable testDataTable)
        {
            //convert the the necessary columns in the datatables to a list
            List<double> cracksList = new List<double>();
            List<double> responsesList = new List<double>();
            foreach (DataRow row in testDataTable.Rows)
            {
                cracksList.Add(Convert.ToDouble(row[1]));
                responsesList.Add(Convert.ToDouble(row[2]));
            }


            HMAnalysisObject newAnalysis = new HMAnalysisObject();
            //assign values we know so far
            newAnalysis.Name = "test analysis";
            //needed to make this public for some reason?
            //newAnalysis.ModelType = 1; //do not transform x axis by default
            newAnalysis.Flaw_name = "My Crack sizes";
            newAnalysis.HitMiss_name = "y";
            //store the responses as a dictionary
            //this dictionary may get  larger with more than one inspector
            newAnalysis.Responses_all.Add(key: newAnalysis.HitMiss_name, value: responsesList);
            //for loop used to create flaws class
            //for..
            //foreach(double i in cracksList)
            //{
            //    newAnalysis.Flaws.Add(new HMCrackData(i));
            //}
            //newAnalysis.FlawsTemp = cracksList;
            newAnalysis.Flaws = cracksList;
            //get the max and min values

            //these values will be dependent on the UI
            newAnalysis.Xmax = 1.0;
            newAnalysis.Xmin = 0.0;
            //set a_x_n and profile_pts
            newAnalysis.A_x_n = 500;
            //newAnalysis.Profile_pts = 500;
            //confidence interval control
            newAnalysis.CIType = "Modified Wald";
            newAnalysis.ModelType = 1;//0=standard, 1=log transform
            newAnalysis.RegressionType = "Logistic Regression";//  "Logistic Regression"  ,"Firth Logistic Regression"
            newAnalysis.SrsOrRSS = 0; // 0 = simple random sampling, 1= index ranked set sampling
            //Ranked set sampling
            newAnalysis.Set_m = 6;
            newAnalysis.Set_r = newAnalysis.Flaws.Count()/newAnalysis.Set_m;
            newAnalysis.MaxResamples = 60;
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            //HitMissAnalysisRControl newHitMissControl = new HitMissAnalysisRControl(analysisEngine);
            //newHitMissControl.ExecuteAnalysis(newAnalysis);
            AnalysistypeTransform newAnalysisControl = new AnalysistypeTransform(analysisEngine, newAnalysis);
            newAnalysisControl.ExecuteReqSampleAnalysisTypeHitMiss();
            HMAnalysisObject finalAnalysis = newAnalysisControl.HMAnalsysResults;
            //HMAnalysisObject finalAnalysis= newAnalysisControl.ExecutePFAnalysisTrans();
            watch.Stop();
            printcSharpDatatables(finalAnalysis);
            
            var time=watch.ElapsedMilliseconds/1000.00;
            Console.WriteLine("The total runtime was this: " + time +" seconds");

            //newAnalysisControl.ExecuteRankedSetSampling();
        }
        public static void GetDataFromAHatTransform(REngineObject analysisEngine, DataTable testDataTable)
        {
            //convert the the necessary columns in the datatables to a list
            List<double> cracksList = new List<double>();
            List<double> responsesList = new List<double>();
            foreach (DataRow row in testDataTable.Rows)
            {
                cracksList.Add(Convert.ToDouble(row[1]));
                responsesList.Add(Convert.ToDouble(row[2]));
            }
            AHatAnalysisObject newAnalysis = new AHatAnalysisObject();
            //assign values we know so far
            newAnalysis.Name = "test analysis";
            //needed to make this public for some reason?
            //newAnalysis.ModelType = 1; //do not transform x axis by default
            newAnalysis.Flaw_name = "My Crack sizes";
            newAnalysis.SignalResponseName = "y";
            //store the responses as a dictionary
            //this dictionary may get  larger with more than one inspector
            newAnalysis.Responses_all.Add(key: newAnalysis.SignalResponseName, value: responsesList);
            //add flaws
            newAnalysis.Flaws = cracksList;
            //set y decision threshold
            newAnalysis.YDecision = 5;
            AnalysistypeTransform newAnalysisControl = new AnalysistypeTransform(analysisEngine, null, newAnalysis);
            newAnalysisControl.ExecuteAnalysisAHat();
            AHatAnalysisObject finalAnalysis = newAnalysisControl.AHatAnalysisResults;
        }
        static void printDT(DataTable data)
        {
            //Console.WriteLine();
            Console.WriteLine('\n');
            Dictionary<string, int> colWidths = new Dictionary<string, int>();
            //if (data != null)
            //{
                foreach (DataColumn col in data.Columns)
                {
                    //Console.Write(col.ColumnName);
                    Console.Write(col.ColumnName);
                    var maxLabelSize = data.Rows.OfType<DataRow>()
                            .Select(m => (m.Field<object>(col.ColumnName)?.ToString() ?? "").Length)
                            .OrderByDescending(m => m).FirstOrDefault();

                    colWidths.Add(col.ColumnName, maxLabelSize);
                    for (int i = 0; i < maxLabelSize - col.ColumnName.Length + 10; i++) Console.Write(" ");
                }

                //Console.WriteLine();
                Console.WriteLine('\n');
                int rowCounter = 0;
                int limit = 100;
                foreach (DataRow dataRow in data.Rows)
                {
                    for (int j = 0; j < dataRow.ItemArray.Length; j++)
                    {
                        //Console.Write(dataRow.ItemArray[j]);
                        Console.Write((dataRow.ItemArray[j]).ToString());
                        for (int i = 0; i < colWidths[data.Columns[j].ColumnName] - dataRow.ItemArray[j].ToString().Length + 10; i++) Console.Write(" ");
                    }
                    //Console.WriteLine();
                    Console.WriteLine('\n');
                    rowCounter = rowCounter + 1;
                    if (rowCounter >= limit)
                    {
                        break;
                    }
                }
                Console.WriteLine('\n');
            //}
        }
        public static void printcSharpDatatables(HMAnalysisObject finalAnalysis)
        {
            try
            {
                printDT(finalAnalysis.LogitFitTable);
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine("no normal regression table found!");
            }
            try
            {
                Console.WriteLine("table 2");
                printDT(finalAnalysis.LogLogitFitTable);
            }
            catch (NullReferenceException e1)
            {
                Console.WriteLine("no normal LOG regression table found!");
            }

        }
        public static DataTable GetDataTableFromCsv(string path, bool isFirstRowHeader=true)
        {
            string[] Lines = File.ReadAllLines(path);
            string[] Fields;
            Fields = Lines[0].Split(new char[] { ',' });
            int Cols = Fields.GetLength(0);
            DataTable dt = new DataTable();
            //1st row must be column names; force lower case to ensure matching later on.
            for (int i = 0; i < Cols; i++)
                dt.Columns.Add(Fields[i].ToLower(), typeof(string));
            DataRow Row;
            for (int i = 1; i < Lines.GetLength(0); i++)
            {
                Fields = Lines[i].Split(new char[] { ',' });
                Row = dt.NewRow();
                for (int f = 0; f < Cols; f++)
                    Row[f] = Fields[f];
                dt.Rows.Add(Row);
            }
            return dt;
        }*/
    }
}