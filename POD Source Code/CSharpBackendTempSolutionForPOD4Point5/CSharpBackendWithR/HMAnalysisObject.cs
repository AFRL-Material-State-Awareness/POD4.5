using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
namespace CSharpBackendWithR
{
    [Serializable]
    public class HMAnalysisObject : ParentAnalysisObject
    {
        //bool Is_solvableHitMiss;
        /// <summary>
        /// This is the HMAnalysis Tranform object. Many of these variables 
        /// are inherited from the ParentAnalysisObjectClass.
        /// the ones that are not are listed below...
        /// </summary> 
        private DataTable hitMissDataOrig;
        private List<double> covarMatrix;
        //private List<double> uncensoredFlaws;
        private int modelType;
        //used for logit, firth, or lasso
        private string regressionType;
        //parameter for ranked set sampling
        private int srsOrRSS;
        private int maxResamples;
        private double goodnessOfFit;
        //tables
        private DataTable logitFitTable;
        private DataTable residualTable;
        public HMAnalysisObject(string nameInput="")
        {
            //original HMDataframe
            this.hitMissDataOrig = null;
            //used to store the covariance matrix as a list
            this.covarMatrix = null;
            //becomes true if any points are censored
            Pts_censored = false;

            Podfile = null;
            //name of the analysis
            Name = nameInput;
            Info_row = 0;
            this.modelType = 1; //becomes 1 if using log transform
            this.regressionType = "Logistic Regression";//becomes logit, firth, or lasso (logit by default)
            Flaw_name = ""; //holds the name of the flaw in the datatable
            HitMiss_name = "";
            //TODO: these may not be necessary
            HitMissMin = 0;
            HitMissMax = 1;

            //decision_thesholds = .5; //this was its value in PODv4 whenever hit/miss is used
            Titles = new List<string>();
            //holds the flaw sizes
            Flaws_All = new List<double>();
            Flaws = new List<double>();
            //current crack size list to be sent to r(can be either normal or log transformed
            //List<double> currFlawsAnalysis;
            //TODO: temporary varaible used to store flaws array until the class is working
            //this.uncensoredFlaws = new List<double>();
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
            A_x_n = 0;
            //int Profile_pts = 0;
            //used for ranked set sampling###
            //k=sample subset size
            Set_m = 6;
            //m=number of cycles
            Set_r = 10;
            //default to standard wald unless overwritten
            CIType = "StandardWald";
            //datatables that will be sent back to the UI
            this.logitFitTable = new DataTable();
            this.residualTable = new DataTable();
            //DataTable LogLogitHMFitTable = new DataTable();

            //0=simple random sampling selected, 1= ranked set sampling(indexed based)
            this.srsOrRSS = 0; //simple random sampling by default
            //max resamples
            this.maxResamples = 30; //1 by default
            //goodness of fit test with likelihood ratio
            this.goodnessOfFit = -1.0;
        }
        public DataTable HitMissDataOrig
        {
            set { this.hitMissDataOrig = value; }
            get { return this.hitMissDataOrig; }
        }
        public DataTable ResidualTable
        {
            set { this.residualTable = value; }
            get { return this.residualTable; }
        }
        public new List<double> CovarianceMatrix
        {
            set { this.covarMatrix = value; }
            get { return this.covarMatrix; }
        }
        public new bool Pts_censored { set; get; }
        public new dynamic Podfile { set; get; }
        public new string Name { set; get; }
        public new int Info_row { set; get; }
        public new int ModelType
        {
            
            set { this.modelType = value; }
            get { return this.modelType; }
        }
        public string RegressionType
        {
            set { this.regressionType = value; }
            get { return this.regressionType; }
        }
        public new string Flaw_name { set; get; }
        public string HitMiss_name { set; get; }
        public new int HitMissMin { set; get; }
        public new int HitMissMax { set; get; }
        public new List<string> Titles { set; get; }
        //public List<HMCrackData> Flaws { set; get; }
        public new List<double> Flaws { set; get; }
        public new List<double> Flaws_All { set; get; }
        public new List<double> LogFlaws_All { set; get; }
        //    set { this.uncensoredFlaws = value; }
        //     get { return this.uncensoredFlaws; } 
        //}
        //TODO: FIX THIS TO MAKE THIS ONLY integers since this object controls hitmiss data only
        public new Dictionary<string, List<double>> Responses { set; get; }
        public new Dictionary<string, List<double>> Responses_all { set; get; }
        public new int Nsets { set; get; }
        public new int Count { set; get; }
        public new double Crckmax { set; get; }
        public new double Crckmin { set; get; }

        public new double Xmin { set; get; }
        public new double Xmax { set; get; }
        public double Npts { set; get; }
        public bool Is_solvableHitMiss { set; get; }
        public new double A25 { set; get; }
        public new double A50 { set; get; }
        public new double A90 { set; get; }
        public new double Sighat { set; get; }
        public new double Muhat { set; get; }
        public new double A9095 { set; get; }
        public new double Pod_threshold { set; get; }
        public new double Pod_level { set; get; }
        public new double Confidence_level { set;  get; }

        public new string Clevel { set; get; }
        public new List<double> Pfit { set; get; }
        public new List<double> Diff { set; get; }
        public int A_x_n { set; get; }
        public int Set_m { set; get; }
        public int Set_r { set; get; }
        public new string CIType { set; get; }
        //progress text
        public new string ProgressText { set; get; }

        //get or set DataTables
        public DataTable LogitFitTable {
            set { this.logitFitTable = value; }
            get { return this.logitFitTable; } 
        }
        public DataTable IterationTable { set; get; }
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
        public double GoodnessOfFit
        {
            set { this.goodnessOfFit = value; }
            get { return this.goodnessOfFit; }
        } 

        public void ClearMetrics()
        {
            /*
            A25 = -1;
            A50 = -1;
            A90 = -1;
            A9095 = -1;
            covarMatrix = null;
            this.goodnessOfFit = -1;
            LogitFitTable = null;
            ResidualTable = null;
            IterationTable = null;
            */
        }
    }
    
}
