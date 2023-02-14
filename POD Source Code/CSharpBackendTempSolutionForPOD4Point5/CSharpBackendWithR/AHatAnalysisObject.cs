using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace CSharpBackendWithR
{
    [Serializable]
    public class AHatAnalysisObject : ParentAnalysisObject
    {
        
        private double shapiroTestStat;//Shapiro-Wilk normality test
        private double shapiroPValue;
        private double chiSqValue; //non-constant variance test (the null hypothesis is constant variance)
        private int chiSqDF;
        private double chiSqPValue;
        private double durbinWatson_r; //durbin watson test for auto-correlation
        private double durbinWatsonDW;
        private double durbinWatsonPValue;
        private double lackOfFitDegFreedom;
        private double lackOfFitFVal;
        private double lackOfFitPValue;
        //y decision theshold
        //private double y_decision;
        //private string xAxisName;
        private string signalResponseName;
        //private DataTable originalData;
        private DataTable aHatLinearResults;
        private DataTable aHatResidualResultsUncensored;
        private DataTable aHatResidualResults;
        private DataTable aHatThresholdsTable;
        private DataTable aHatResultsPOD;
        //linear model unique metrics
        private double intercept;
        private double slope;
        private double residualError;
        //standard errors
        private double slopeStdErr;
        private double interceptStdErr;
        private double residualStdErr;
        //stored multiple R-squared value (can also add adjusted R-squared if necessary)
        private double rSquared;
        //store the censored points
        private List<double> flawsCensored;
        private Dictionary<string, List<double>> responsesCensoredLeft;
        private Dictionary<string, List<double>> responsesCensoredRight;
        //box cox
        private double lambda;
        public AHatAnalysisObject(string nameInput = "") : base()
        {
            //name of the analysis
            Name = nameInput;
            Flaw_name = ""; //holds the name of the flaw in the datatable
            //Info_row = 0;
            //KEY:
            //1=no transform,
            //2=x-axis only transform,
            //3=y-axis only transform,
            //4=both axis tranform, 
            //5= Box-cox tranform with linear x;
            //6= Box-cox tranform with log x
            //7= Box-cox transform with inverse x
            //8= linear x with inverse y
            //9= log x with inverse y
            //10= inverse x with linear y
            //11= inverse x with log y
            //12= inverse x with inverse y
            this.modelType = 1; 
            //stores the max and min signal reponses
            Signalmin = -1.0;
            Signalmax = -1.0;
            //holds the flaw sizes
            Flaws = new List<double>();
            Flaws_All = new List<double>();
            ExcludedFlaws = new List<double>();
            ////////////////////
            this.flawsCensored = new List<double> ();
            this.responsesCensoredLeft = new Dictionary<string, List<double>>();
            this.responsesCensoredRight = new Dictionary<string, List<double>>();
            ///this.uncensoredFlaws = new List<double>();
            Responses = new Dictionary<string, List<double>>();
            Responses_all = new Dictionary<string, List<double>>();
            
            A_transform = 1; //1=no transform, 2=log transform
            Ahat_transform = 1; //1=no transform, 2=log transform, 3= box-cox transform
            //lambda is used for box cox transformation only(normalizes the y-values)
            //lambda can never actually be zero in the program (0 would mean that log transform is best),
            //but it starts at 0 so that the program knows if the user already assigned a value for lambda or not
            this.lambda = 0.0;
            //ahat tests
            this.shapiroTestStat = 0.0;//Shapiro-Wilk normality test
            this.shapiroPValue = 0.0;
            this.chiSqValue = 0.0; //non-constant variance test (the null hypothesis is constant variance)
            this.chiSqDF = -1;
            this.chiSqPValue = 0.0;
            this.durbinWatson_r = 0.0; //durbin watson test for auto-correlation
            this.durbinWatsonDW = 0.0;
            this.durbinWatsonPValue = 0.0;
            this.lackOfFitDegFreedom = 0.0; //lack of fit test. Tests how well the model fits the data with and without the slope
            this.lackOfFitFVal = 0.0;
            this.lackOfFitPValue = 0.0;
            //used to store the signal response name
            this.signalResponseName = "";
            //default theshold is zero(will be determined by the user)
            //this.y_decision = 0.0;
            //linear model
            this.slope = 0.0;
            this.intercept = 0.0;
            //r-sqaured
            this.rSquared = 0.0;
            //standard errors for the linear model
            this.slopeStdErr = 0.0;
            this.interceptStdErr = 0.0;
            this.residualStdErr = 0.0;
            //used to store the linear dataframe
            this.aHatLinearResults = new DataTable();
            //censored residual Results
            this.aHatResidualResultsUncensored = new DataTable();
            //used to store the residual dataframe(adds a diff column to the linear df)
            this.aHatResidualResults = new DataTable();
            //used to store and plot the POD at various thresholds in ahat vs a
            this.aHatThresholdsTable = new DataTable();
            //used to store the results dataframe
            this.aHatResultsPOD = new DataTable();
        }
        public double Signalmin { set; get; }
        public double Signalmax { set; get; }
        public List<double> FlawsCensored
        {
            set { this.flawsCensored = value; }
            get { return this.flawsCensored; }
        }
        public Dictionary<string, List<double>> ResponsesCensoredLeft
        {
            set { this.responsesCensoredLeft = value; }
            get { return this.responsesCensoredLeft; }
        }
        public Dictionary<string, List<double>> ResponsesCensoredRight
        {
            set { this.responsesCensoredRight = value; }
            get { return this.responsesCensoredRight; }
        }
        public int A_transform { set; get; }  //identifies transformation on the x-axis
        public int Ahat_transform { set; get; } //identifies transformation on the y-axis (although not really applicable in hit/miss data)
        public double Lambda
        {
            set { this.lambda = value; }
            get { return this.lambda; }
        }
        public double ShapiroTestStat {
            set { this.shapiroTestStat = value; }
            get { return this.shapiroTestStat; }
        }
        public double ShapiroPValue
        {
            set { this.shapiroPValue = value; }
            get { return this.shapiroPValue; }
        }
        public double ChiSqValue
        {
            set { this.chiSqValue = value; }
            get { return this.chiSqValue; }
        }
        public int ChiSqDF
        {
            set { this.chiSqDF = value; }
            get { return this.chiSqDF; }
        }
        public double ChiSqPValue
        {
            set { this.chiSqPValue = value; }
            get { return this.chiSqPValue; }
        }
        public double DurbinWatson_r
        {
            set { this.durbinWatson_r = value; }
            get { return this.durbinWatson_r; }
        }
        public double DurbinWatsonDW
        {
            set { this.durbinWatsonDW = value; }
            get { return this.durbinWatsonDW; }
        }
        public double DurbinWatsonPValue
        {
            set { this.durbinWatsonPValue = value; }
            get { return this.durbinWatsonPValue; }
        }
        public double LackOfFitDegFreedom
        {
            set { this.lackOfFitDegFreedom = value; }
            get { return this.lackOfFitDegFreedom; }
        }
        public double LackOfFitFCalc
        {
            set { this.lackOfFitFVal = value; }
            get { return this.lackOfFitFVal; }
        }
        public double LackOfFitPValue
        {
            set { this.lackOfFitPValue = value; }
            get { return this.lackOfFitPValue; }
        }
        public string SignalResponseName
        {
            set { this.signalResponseName = value; }
            get { return this.signalResponseName; }
        }
        public double Slope
        {
            set { this.slope = value; }
            get { return this.slope; }
        }
        public double Intercept
        {
            set { this.intercept=value; }
            get { return this.intercept; }
        }
        public double ResidualError
        {
            set { this.residualError = value; }
            get { return this.residualError; }
        }
        public double RSqaured
        {
            set { this.rSquared = value; }
            get { return this.rSquared; }
        }
        public double SlopeStdErr
        {
            set { this.slopeStdErr = value; }
            get { return this.slopeStdErr; }
        }
        public double InterceptStdErr
        {
            set { this.interceptStdErr = value; }
            get { return this.interceptStdErr; }
        }
        public double ResidualStdErr
        {
            set { this.residualStdErr = value; }
            get { return this.residualStdErr; }
        }
        public DataTable AHatResultsLinear
        {
            set { this.aHatLinearResults = value; }
            get { return this.aHatLinearResults; }
        }
        public DataTable AHatThresholdsTable
        {
            set { this.aHatThresholdsTable = value; }
            get { return this.aHatThresholdsTable; }
        }
        public DataTable AHatResultsResidUncensored
        {
            set { this.aHatResidualResultsUncensored = value; }
            get { return this.aHatResidualResultsUncensored; }
        }
        public DataTable AHatResultsResid
        {
            set { this.aHatResidualResults = value; }
            get { return this.aHatResidualResults; }
        }
        public DataTable AHatResultsPOD
        {
            set { this.aHatResultsPOD=value; }
            get { return this.aHatResultsPOD; }
        }
        public double GetMaxResponse()
        {
            double overallMax = -1;
            double currMax = -1;
            // iterate over C# Dictionary Keys and Values
            foreach (KeyValuePair<string, List<double>> entry in Responses_all)
            {
                currMax = entry.Value.Max();
                if (currMax > overallMax)
                {
                    overallMax = currMax;
                }
            }
            return overallMax;

        }
        public void ClearMetrics()
        {

            A25 = -1;
            A50 = -1;
            A90 = -1;
            A9095 = -1;
            //CovarianceMatrix = null;
            this.residualStdErr = -1;
            this.durbinWatsonDW = -1;
            this.shapiroTestStat = -1;
            this.shapiroPValue = -1;
            this.rSquared = -1;

            this.aHatThresholdsTable = null;
            this.aHatLinearResults = null;
            this.aHatResidualResults = null;
            this.aHatResultsPOD = null;
        }
    }
}
