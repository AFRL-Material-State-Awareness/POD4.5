using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
namespace CSharpBackendWithR
{
    public class HMAnalysisObjectTransform : ParentAnalysisObject
    {
        //bool Is_solvableHitMiss;
        /// <summary>
        /// This is the HMAnalysis Tranform object. Many of these variables 
        /// are inherited from the ParentAnalysisObjectClass.
        /// the ones that are not are listed below...
        /// </summary> 
        private List<double> flawsTemp;
        private int modelType;
        //used for logit, firth, or lasso
        private string regressionType;
        //parameter for ranked set sampling
        private int srsOrRSS;
        private int maxResamples;
        public HMAnalysisObjectTransform(string nameInput="")
        {
            //becomes true if any points are censored
            Pts_censored = false;

            Podfile = null;
            //name of the analysis
            Name = nameInput;
            Info_row = 0;
            this.modelType = 0; //becomes 1 if using log transform
            this.regressionType = "Logistic Regression";//becomes logit, firth, or lasso (logit by default)
            Flaw_name = ""; //holds the name of the flaw in the datatable
            string HitMiss_name = "";
            //TODO: these may not be necessary
            HitMissMin = 0;
            HitMissMax = 1;

            //decision_thesholds = .5; //this was its value in PODv4 whenever hit/miss is used
            Titles = new List<string>();
            //detailed information about flaws
            //Flaws = new List<HMCrackData>();
            //dictionary that holds the flaw sizes
            Flaws = new List<double>();
            //current crack size list to be sent to r(can be either normal or log transformed
            //List<double> currFlawsAnalysis;
            //TODO: temporary varaible used to store flaws array until the class is working
            this.flawsTemp = new List<double>();
            Responses_all = new Dictionary<string, List<double>>();
            Nsets = 0;
            Count = 0; //the original number of data points in a given analysis
            //max and min crack sizes
            Crckmin = 0.0;
            Crckmax = 0.0;
            //log of the max and min crack sizes
            //LogCrckmin = 0.0;
            //LogCrckmax = 0.0;
            //xmin and xmax
            Xmin = 0.0;
            Xmax = 0.0;

            //stored the number of points for the censored data (if none its equal to count)
            Npts = 0;

            //variable used for determining if the logit is solvable
            bool Is_solvableHitMiss = false;

            //key a values
            A25 = 0.0;
            A50 = 0.0;
            A90 = 0.0;
            //sighat --- standard error at a90
            Sighat = 0.0;
            //muhat --- value at a50
            Muhat = 0.0;
            A9095 = 0.0;
            //thesholds
            Pod_threshold = .5;
            Pod_level = .9;
            Confidence_level = .95;
            Asize = "a90";
            Alevel = "a090/95";
            Clevel = "95% confidence bound";

            Pfit = new List<double>();
            Diff = new List<double>(); //found by taking the response value - pfit

            //used for ranked set sampling, modified wald, lr, and mlr
            int A_x_n = 0;
            //int Profile_pts = 0;
            //used for ranked set sampling###
            //k=sample subset size
            int Set_m = 0;
            //m=number of cycles(usually 30 is sufficient
            int Set_r = 0;
            //default to standard wald unless overwritten
            CIType = "Standard Wald";
            //datatables that will be sent back to the UI
            DataTable LogitHMFitTable = new DataTable();
            DataTable LogLogitHMFitTable = new DataTable();

            //0=simple random sampling selected, 1= ranked set sampling(indexed based)
            this.srsOrRSS = 0; //simple random sampling by default
            //max resamples
            this.maxResamples = 1; //1 by default
        }
        //public bool pts_censored{get; set;}
        public bool Pts_censored { set; get; }
        public dynamic Podfile { set; get; }
        public string Name { set; get; }
        public int Info_row { set; get; }
        public int ModelType
        {
            
            set { this.modelType = value; }
            get { return this.modelType; }
        }
        public string RegressionType
        {
            set { this.regressionType = value; }
            get { return this.regressionType; }
        }
        public string Flaw_name { set; get; }
        public string HitMiss_name { set; get; }
        public int HitMissMin { set; get; }
        public int HitMissMax { set; get; }
        public List<string> Titles { set; get; }
        //public List<HMCrackData> Flaws { set; get; }
        public List<double> Flaws { set; get; }
        public List<double> FlawsTemp {
            set { this.flawsTemp = value; }
            get { return this.flawsTemp; } 
        }
        //TODO: FIX THIS TO MAKE THIS ONLY integers since this object controls hitmiss data only
        public Dictionary<string, List<double>> Responses { set; get; }
        public Dictionary<string, List<double>> Responses_all { set; get; }
        public int Nsets { set; get; }
        public int Count { set; get; }
        public double Crckmax { set; get; }
        public double Crckmin { set; get; }

        public double Xmin { set; get; }
        public double Xmax { set; get; }
        public double Npts { set; get; }
        public bool Is_solvableHitMiss { set; get; }
        public double A25 { set; get; }
        public double A50 { set; get; }
        public double A90 { set; get; }
        public double Sighat { set; get; }
        public double Muhat { set; get; }
        public double A9095 { set; get; }
        public double Pod_threshold { set; get; }
        public double Pod_level { set; get; }
        public double Confidence_level { set;  get; }

        public string Clevel { set; get; }
        public List<double> Pfit { set; get; }
        public List<double> Diff { set; get; }
        public int A_x_n { set; get; }
        public int Set_m { set; get; }
        public int Set_r { set; get; }
        public string CIType { set; get; }
        //progress text
        public string ProgressText { set; get; }

        //get or set DataTables
        public DataTable LogitFitTable { set; get; }
        public DataTable LogLogitFitTable { set; get; }
        //RSS
        public int SrsOrRSS
        {
            set { this.srsOrRSS = value; }
            get { return this.srsOrRSS; }
        }
        public int MaxResamples
        {
            set { this.maxResamples = value; }
            get { return this.maxResamples; }
        }


        public void CurrFlawsAnalysis()
        {
            //empty the temp array whenever this function is called
            this.flawsTemp.Clear();
            //iterate through the list of HMCrackDataObjects to make the x-axis
            //a normal tranform o a log tranform depending on what the value of model is 
            /*
            foreach (HMCrackData i in this.Flaws)
            {
                if (this.modelType == 0)
                {
                    this.flawsTemp.Add(i.CrkSizeControl);
                }
                else if(this.modelType == 1)
                {
                    this.flawsTemp.Add(i.CrkfControl);
                }
            } 
            */
        }
    }
    
}
