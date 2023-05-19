using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
namespace CSharpBackendWithR
{
    /// <summary>
    /// This is the HMAnalysis Tranform object. Many of these variables 
    /// are inherited from the ParentAnalysisObjectClass.
    /// the ones that are not are listed below...
    /// </summary> 
    [Serializable]
    public class HMAnalysisObject : ParentAnalysisObject
    {
        //bool Is_solvableHitMiss;
        
        private DataTable hitMissDataOrig;
        private List<double> covarMatrix;
        //used for logit, firth, or lasso
        private string regressionType;
        //parameter for ranked set sampling
        private int srsOrRSS;
        //numeber of resamples for ranked set sampling
        private int maxResamples;
        //used to store the goodness of fit test value
        private double goodnessOfFit;
        //tables
        private DataTable logitFitTable;
        private DataTable residualTable;
        //flag used for separated Data and whether or not the alogrithm converged
        private bool isSeparated;
        private bool failureToConverge;
        public HMAnalysisObject(string nameInput) : base()
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
            //Info_row = 0;
            /// <summary>
            /// becomes 1 if linear
            /// 2 if log
            /// 3 if inverse
            /// </summary>
            this.modelType = 1; 
            this.regressionType = "Logistic Regression";//becomes logit, firth, or lasso (logit by default)
            Flaw_name = ""; //holds the name of the flaw in the datatable
            HitMiss_name = "";
            HitMissMin = 0;
            HitMissMax = 1;
            //holds the flaw sizes
            Flaws_All = new List<double>();
            Flaws = new List<double>();
            ExcludedFlaws = new List<double>();
            Responses = new Dictionary<string, List<double>>();
            Responses_all = new Dictionary<string, List<double>>();
            //Nsets = 0;
            //log of the max and min crack sizes
            //LogCrckmin = 0.0;
            //LogCrckmax = 0.0;

            //stored the number of points for the censored data (if none its equal to count)
            Npts = 0;

            //variable used for determining if the logit is solvable
            this.isSeparated = false;
            this.failureToConverge = false;

            //used for ranked set sampling, modified wald, lr, and mlr
            A_x_n = 0;
            //int Profile_pts = 0;
            //used for ranked set sampling###
            //k=sample subset size
            Set_m = 6;
            //m=number of cycles
            Set_r = 10;
            //use this variable for the user to pass in the confidence interval technique they want to use
            //standard wald, modified wald, etc
            //default to standard wald unless overwritten
            CIType = "StandardWald";
            //datatables that will be sent back to the UI
            this.logitFitTable = new DataTable();
            this.residualTable = new DataTable();

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
        public List<double> CovarianceMatrix
        {
            set { this.covarMatrix = value; }
            get { return this.covarMatrix; }
        }
        public string RegressionType
        {
            set { this.regressionType = value; }
            get { return this.regressionType; }
        }
        public string HitMiss_name { set; get; }
        public int HitMissMin { set; get; }
        public int HitMissMax { set; get; }
        public bool Is_Separated
        {
            set { this.isSeparated = value; }
            get { return this.isSeparated; }
        }
        public bool Failed_To_Converge
        {
            set { this.failureToConverge = value; }
            get { return this.failureToConverge; }
        }
        public double Npts { set; get; }

        public int A_x_n { set; get; }
        public int Set_m { set; get; }
        public int Set_r { set; get; }
        public string CIType { set; get; }

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

        public int MaxPrecision
        {
            get { return this.maxPrecisionFlaws; }
        }
        
        public void ClearMetrics()
        {

            A25 = -1;
            A50 = -1;
            A90 = -1;
            A9095 = -1;
            covarMatrix = null;
            this.goodnessOfFit = -1;
            LogitFitTable = null;
            ResidualTable = null;
            IterationTable = null;
        }
    }
    
}
