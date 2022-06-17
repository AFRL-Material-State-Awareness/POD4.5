using System;
using System.Data;
using System.Collections.Generic;
using RDotNet;
//used for maxmin function
using System.Linq;
namespace CSharpBackendWithR
{
    class Program
    {
        public static List<double> testFlawList;
        public static List<int> testReponses;
        static void Main(string[] args)
        {
            REngineObject newREngine = new REngineObject();
            newREngine.InitializeRScripts();
            getDataFromPFTransform(newREngine);

            ///
            /// These values are attrifial and inserted to test the backend
            ///
            //newTranformAnalysis
            
        }
        
        

        public static void getDataFromPFTransform(REngineObject analysisEngine)
        {
            //Create the transform analysis Object
            //HMAnalysisObjectTransform newTransformAnalysis = new HMAnalysisObjectTransform();

            testFlawList = new List<double>()
            {
                0.153917693,
                0.166910359,
                0.181461883,
                0.187918014,
                0.191985489,
                0.206296735,
                0.22125183,
                0.22254802,
                0.223172984,
                0.226625926,
                0.234836897,
                0.240048821,
                0.245869667,
                0.247247879,
                0.24864422,
                0.254521076,
                0.259783658,
                0.260924841,
                0.270902464,
                0.283500563,
                0.284831232,
                0.28762478,
                0.291295773,
                0.296634941,
                0.308812298,
                0.319062008,
                0.322503578,
                0.345716849,
                0.356358449,
                0.366939348

            };

            testReponses = new List<int>()
            {
                0,
                0,
                0,
                0,
                0,
                0,
                1,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                1,
                1,
                0,
                1,
                1,
                1,
                1,
                0,
                1,
                0,
                1,
                1,
                1,
                1,
                1,
                1
            };
            HMAnalysisObjectTransform newTranformAnalysis = new HMAnalysisObjectTransform();
            //assign values we know so far
            newTranformAnalysis.Name = "test analysis";
            //needed to make this public for some reason?
            newTranformAnalysis.ModelType = 0; //do not transform x axis
            newTranformAnalysis.Flaw_name = "My Crack sizes";
            newTranformAnalysis.HitMiss_name = "did I find it, INSP 1?";
            //store the responses as a dictionary
            //this dictionary may get  larger with more than one inspector
            newTranformAnalysis.Responses_all.Add(key: newTranformAnalysis.HitMiss_name, value: testReponses);
            //for loop used to create flaws class
            //for..
            foreach(double i in testFlawList)
            {
                newTranformAnalysis.Flaws.Add(new HMCrackData(i));
            }
            //newTranformAnalysis.FlawsTemp = testFlawList;
            //get the max and min values

            //these values will be dependent on the UI
            newTranformAnalysis.Xmax = 1.0;
            newTranformAnalysis.Xmin = 0.0;
            //set a_x_n and profile_pts
            newTranformAnalysis.A_x_n = 500;
            //newTranformAnalysis.Profile_pts = 500;
            //confidence interval control
            newTranformAnalysis.CIType = "MLR";
            newTranformAnalysis.ModelType = 0;

            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            //HitMissAnalysisRControl newHitMissControl = new HitMissAnalysisRControl(analysisEngine);
            //newHitMissControl.ExecuteAnalysis(newTranformAnalysis);
            AnalysistypeTransform newAnalysisControl = new AnalysistypeTransform(newTranformAnalysis, analysisEngine);
            HMAnalysisObjectTransform finalAnalysis= newAnalysisControl.executePFAnalysisTrans();
            watch.Stop();
            //printcSharpDatatables(finalAnalysis);

            var time=watch.ElapsedMilliseconds/1000.00;
            Console.WriteLine("The runtime was: " + time +" seconds");
        }
        public static void printDT(DataTable data)
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
                int limit = 10;
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
    }
}
