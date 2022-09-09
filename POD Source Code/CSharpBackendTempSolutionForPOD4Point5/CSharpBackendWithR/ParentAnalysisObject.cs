using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpBackendWithR
{
    [Serializable]
    public class ParentAnalysisObject
    {

        //becomes true when pts are censored
        protected bool Pts_censored;

        /// <summary>
        /// This class contains all the initialized variables within the pf create analysis class
        /// Anything that only pertains to ahat versus a is not included or commented out
        /// </summary>
        protected dynamic Podfile;
        protected List<string> Info;
        protected int Info_count;

        protected string Name;

        //Info Worksheet access
        protected int Info_row;

        public int ModelType; //0-linear, 1-log

        protected string Flaw_name;
        protected int Flaw_column;
        protected string Flaw_units;
        protected int Data_column;
        protected string Data_units;

        //called signal min and signal max in the python code
        protected double Signalmin;
        protected double Signalmax;
        //used for hit miss
        protected int HitMissMin;
        protected int HitMissMax;

        protected bool ShowMessages;
        protected List<double> Decision_thresholds;
        //protected List<string> Decision_table_thesholds;
        //protected List<string> Decision_a50;
        //protected List<string> Decision_level;
        //decision_confidence
        //decision_v11
        //decision_v12
        //decision_v22
        protected List<string> Titles;
        /////////////////////////
        ///MAKE flaws CLASS for this variable
        //protected List<HMCrackData> Flaws;
        protected List<double> Flaws;
        protected List<double> Flaws_All;
        protected List<double> LogFlaws_All;
        protected List<double> InverseFlaws_All;
        protected List<double> ExcludedFlaws;
        ////////////////////////
        protected List<double> Flaws_reload; //used for POD when no points are missing
        protected Dictionary<string, List<double>> Responses;//used for POD with censored points
        protected Dictionary<string, List<double>> Responses_all;//used for POD when no points are missing
        protected Dictionary<string, List<int>> Responses_missing;//used for POD when no points are missing
        protected int Nsets; //might remove this, but it may be used to pass from the FlawsClass
        protected int Count; //number of data points

        //flaw ranges
        protected double Crckmin;
        protected double Crckmax;
        //flaw ranges for log transform
        //log of the max and min crack sizes
        //protected double LogCrckmin;
        //protected double LogCrckmax;
        protected double Xmin;
        protected double Xmax;
        protected double Calcmin;
        protected double Calcmax;

        //input to regresion routine
        protected List<double> Rlonx;
        protected List<double> Rlony;

        /////////////////////////
        ///MAKE PFDATA CLASS for this variable
        ///protected List<PFDATAclass> flaws;
        ////////////////////////
        //protected int Npts;

        //protected int acount;//# number of cases with all inspections at maximum
        //protected int bcount;// # number with all at minimum
        //protected int nexcluded;


        //POD model Parameters
        protected double Sighat;
        protected double Muhat;//POD flaw a50
        //protected double mhat; 
        protected double A90;
        //protected double M90;
        //protected double M9095;

        //may have to adjust this later
        protected double Pod_all;
        //protected Dictionary<string, List<double>> pod_allDict
        protected double threshold_all;
        //protected Dictionary<string, List<double>>  threshold_allDict
        protected double A25;
        protected double A50;
        //protected double A90;
        protected double A9095;

        //Confidence Bound Settings a9095 = crack size 95% confidence
        //confidence level for 90% POD (pod_level=.9)
        protected double Pod_threshold;
        protected double Pod_level;
        protected double Confidence_level;
        protected string Asize; //set to a90
        protected string Alevel; //set to a9095
        protected string Clevel; //set to a 95% confidence interval

        //protected LinearRegressionClassObject regression;
        //protected Dictionary<string, float> linfit;
        protected bool Iret; //it is used as a boolean in hit/miss data and a integer in ahat
        protected List<double> Fnorm;
        protected List<double> CovarianceMatrix; //this stores a 2x2 array for hitmiss data
        protected List<double> Res_sums;
        //protected List<double> fiterrors;
        //protected int repeatability_error;
        protected List<double> Pf_covariance;//array with three double values

        //protected List<double> analsysis
        protected int A_transform; //identifies transformation on the x-axis
        protected int Ahat_transform; //identifies transformation on the y-axis (although not really applicable in hit/miss data)
        //protected List<string> Choices;

        //ahat tests
        /*
        protected float astar;
        protected float astar_p;
        protected string astarRating;
        protected float EV_Bartlett;
        protected float EV_Bartlett_p;
        protected string barlettRating;
        */
        protected double Fcalc;
        protected double Fcalc_p;
        protected string LackOfFitRating;
        //protected int lackOfFitDegFrdm;
        //protected bool lackOffitCalcualted;
        /*
        self.pod_crksize = []
        self.pod_crksizef = []
        self.pod = []
        self.pod_bounds = []
        self.pod_bounds_crksize = []
        self.pod_bounds_crksizef = []
        */

        protected List<double> Pf_POD_a;
        protected List<double> Pf_POD_af;
        protected List<double> Pf_POD_poda;
        protected List<double> Pf_POD_conf95;
        protected List<double> pf_POD_ptxpt;

        //self.newPFGamma = 0.0
        //self.newPFBetaodict = 0.0
        //self.newPFPODLevel = 0.0
        protected List<double> New_pf_a;
        protected List<double> New_pf_gamma;
        protected List<double> New_pf_beta;
        protected List<double> New_pf_pod;
        protected List<double> New_pf_old;

        //self.g0= []
        //self.g1= []

        /// <summary>
        /// add solve iteration object
        /// protected List<solveIterations>  pf_solve_params;
        /// </summary>

        protected string PlotTitle;

        //pass fail analysis 
        protected List<double> Pfit;
        protected List<double> Diff;

        protected float Pf_test_crksum;
        protected float Pf_test_phat;
        protected float Pf_test_h0_likihood;
        protected float pf_test_ha_likihood;
        protected float Pf_censor_test_result;
        protected bool Pf_censor_test_pass;

        //not created at initialization
        protected Dictionary<string, List<double>> Meas;
        protected float Deter;
        protected float X90;
        //protected float fcalc;

        //use this variable for the user to pass in the confidence interval technique they want to use
        //standard wald, modified wald, etc
        protected string CIType;

        //progress text when loading POD
        protected string ProgressText;
    }
}
