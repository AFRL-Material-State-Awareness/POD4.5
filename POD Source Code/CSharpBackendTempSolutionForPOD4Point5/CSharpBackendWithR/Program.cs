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
            REngineObject newREngine = new REngineObject();
            newREngine.InitializeRScripts();
            string csvInputPath = @"C:\Users\gohmancm\Desktop\PODv4Point5FullProjectFolder\CSharpBackendTempSolution\CSharpBackendTempSolutionForPOD4Point5\RCode\RBackend\HitMissInfo_BadLL.csv";
            DataTable mydt= GetDataTableFromCsv(csvInputPath);
            printDT(mydt);
            getDataFromPFTransform(newREngine, mydt);
            ///
            /// These values are attrifial and inserted to test the backend
            ///
            //newTranformAnalysis

        }
        public static void getDataFromPFTransform(REngineObject analysisEngine, DataTable testDataTable)
        {
            //convert the the necessary columns in the datatables to a list
            List<double> cracksList = new List<double>();
            List<double> responsesList = new List<double>();
            foreach (DataRow row in testDataTable.Rows)
            {
                cracksList.Add(Convert.ToDouble(row[1]));
                responsesList.Add(Convert.ToDouble(row[2]));
            }


            HMAnalysisObjectTransform newTranformAnalysis = new HMAnalysisObjectTransform();
            //assign values we know so far
            newTranformAnalysis.Name = "test analysis";
            //needed to make this public for some reason?
            newTranformAnalysis.ModelType = 0; //do not transform x axis
            newTranformAnalysis.Flaw_name = "My Crack sizes";
            newTranformAnalysis.HitMiss_name = "INSP 1";
            //store the responses as a dictionary
            //this dictionary may get  larger with more than one inspector
            newTranformAnalysis.Responses_all.Add(key: newTranformAnalysis.HitMiss_name, value: responsesList);
            //for loop used to create flaws class
            //for..
            //foreach(double i in cracksList)
            //{
            //    newTranformAnalysis.Flaws.Add(new HMCrackData(i));
            //}
            //newTranformAnalysis.FlawsTemp = testFlawList;
            //get the max and min values

            //these values will be dependent on the UI
            newTranformAnalysis.Xmax = 1.0;
            newTranformAnalysis.Xmin = 0.0;
            //set a_x_n and profile_pts
            newTranformAnalysis.A_x_n = 500;
            //newTranformAnalysis.Profile_pts = 500;
            //confidence interval control
            newTranformAnalysis.CIType = "Modified Wald";
            newTranformAnalysis.ModelType = 0;//0=standard, 1=log transform
            newTranformAnalysis.RegressionType = "Firth Logistic Regression";//  "Logistic Regression"  ,"Firth Logistic Regression"
            newTranformAnalysis.SrsOrRSS = 1; // 0 = simple random sampling, 1= index ranked set sampling
            //Ranked set sampling
            newTranformAnalysis.Set_m = 6;
            newTranformAnalysis.Set_r = newTranformAnalysis.Flaws.Count()/newTranformAnalysis.Set_m;
            newTranformAnalysis.MaxResamples = 60;
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            //HitMissAnalysisRControl newHitMissControl = new HitMissAnalysisRControl(analysisEngine);
            //newHitMissControl.ExecuteAnalysis(newTranformAnalysis);
            AnalysistypeTransform newAnalysisControl = new AnalysistypeTransform(newTranformAnalysis, analysisEngine);
            newAnalysisControl.ExecuteReqSampleAnalysisType();
            HMAnalysisObjectTransform finalAnalysis = newAnalysisControl.HMAnalsysResults;
            //HMAnalysisObjectTransform finalAnalysis= newAnalysisControl.ExecutePFAnalysisTrans();
            watch.Stop();
            printcSharpDatatables(finalAnalysis);
            
            var time=watch.ElapsedMilliseconds/1000.00;
            Console.WriteLine("The total runtime was this: " + time +" seconds");

            //newAnalysisControl.ExecuteRankedSetSampling();
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
        public static void printcSharpDatatables(HMAnalysisObjectTransform finalAnalysis)
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
        }
    }
}
