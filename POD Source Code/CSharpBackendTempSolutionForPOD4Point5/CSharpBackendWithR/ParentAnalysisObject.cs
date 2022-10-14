using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpBackendWithR
{
    [Serializable]
    public class ParentAnalysisObject
    {
        //may have to adjust this later
        protected double Pod_all;
        //protected Dictionary<string, List<double>> pod_allDict
        protected double threshold_all;
        protected List<string> Info;
        protected int Info_count;
        protected bool Iret; //it is used as a boolean in hit/miss data and a integer in ahat
        protected List<double> Fnorm;
        protected List<double> Res_sums;
        protected List<double> Pf_covariance;//array with three double values
        protected string PlotTitle;
        protected float Pf_test_crksum;
        protected float Pf_test_phat;
        protected float Pf_test_h0_likihood;
        protected float pf_test_ha_likihood;
        protected float Pf_censor_test_result;
        protected bool Pf_censor_test_pass;


        protected string Asize; //set to a90
        protected string Alevel; //set to a9095
        protected string Clevel; //set to a 95% confidence interval
        //used to set the default model type
        protected int modelType;


        /// <summary>
        /// sets default values for fields that are shared by both AHat and HitMiss data
        /// </summary>
        public ParentAnalysisObject()
        {
            Count = 0; //the original number of data points in a given analysis
            //max and min crack sizes
            Crckmin = 0.0;
            Crckmax = 0.0;
            //key a values
            A25 = 0.0;
            A50 = 0.0;
            A90 = 0.0;
            //sighat --- standard error at a90
            Sighat = 0.0;
            //muhat --- value at a50
            Muhat = 0.0;
            A9095 = 0.0;
            //thesholds-These values remain constant throughout the analysis
            Pod_threshold = .5;
            Pod_level = .9;
            Confidence_level = .95;
            Asize = "a90";
            Alevel = "a090/95";
            Clevel = "95% confidence bound";
        }
        //becomes true when pts are censored
        public bool Pts_censored { set; get; }

        /// <summary>
        /// This class contains all the initialized variables within the pf create analysis class
        /// Anything that only pertains to ahat versus a is not included or commented out
        /// </summary>
        public dynamic Podfile { set; get; }
        

        //stores the name of the analsys
        public string Name { set; get; }

        
        //represents the overall transform being done on an analysis
        //for reference see HMAnalysisObject.cs and AHatAnalysisObject.cs under this.modeltype=0
        public int ModelType
        {

            set { this.modelType = value; }
            get { return this.modelType; }
        }

        public string Flaw_name { set; get; }
        public List<double> Flaws { set; get; }
        public List<double> Flaws_All { set; get; }
        public List<double> LogFlaws_All { set; get; }
        //TODO: ause this with signal response too?
        public List<double> InverseFlaws_All;
        public List<double> ExcludedFlaws { set; get; }
        public Dictionary<string, List<double>> Responses { set; get; }//used for POD with excluded points
        public Dictionary<string, List<double>> Responses_all { set; get; }//used for POD when no points are missing(keeps original data intact)
        public int Count { set; get; } //number of data points

        //flaw ranges
        public double Crckmin { set; get; }
        public double Crckmax { set; get; }


        //POD model Parameters: Muhat= a50, Sighat=standard error at A90
        public double Sighat { set; get; }
        public double Muhat { set; get; }//POD flaw a50

        /// <summary>
        /// KEY A Values (A25 is stored in both analyses, but neither are used in the UI)
        /// </summary>
        public double A25 { set; get; }
        public double A50 { set; get; }
        public double A90 { set; get; }
        public double A9095 { set; get; }
        //Confidence Bound Settings a9095 = crack size 95% confidence
        //confidence level for 90% POD (pod_level=.9)
        public double Pod_threshold { set; get; }
        public double Pod_level { set; get; }
        public double Confidence_level { set; get; }

        /// <summary>
        /// add solve iteration object
        /// protected List<solveIterations>  pf_solve_params;
        /// </summary>

        //progress text when loading POD
        public string ProgressText { set; get; }


    }
    
}
