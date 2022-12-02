using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Threading;
namespace POD.Analyze
{
    //the analysis class is inherited in order to easily integrate the class into the wizard dock pair list. Most of the inherited fields will
    //not be used
    public class RunAllAnalysis : Analysis
    {
        private string flawName;
        private AnalysisList allAnalyses;
        private int analysisCount;
        private List<DataTable> podTables;
        private List<string> responseLabelNames;
        private double maxFlawSize;
        private double minFlawSize;
        private double maxReponseSize;
        private double minReponseSize;
        public RunAllAnalysis(AnalysisList allAnalysesInput, string flawNameInput = "NONE")
        {
            this.flawName = flawNameInput;
            this.allAnalyses = allAnalysesInput;
            // -1 is so that the RunAllAnalysisObject is excluded from the list
            this.analysisCount = this.allAnalyses.Count;
            this.podTables = new List<DataTable>();
            this.responseLabelNames = new List<string>();
            this.maxFlawSize = -1;
            this.minFlawSize = Double.MaxValue;
            this.maxReponseSize = -1;
            this.minReponseSize = Double.MaxValue;
            //get the reponse names from the analysis
            GatherResponsesNames();
        }
        public void RunAllAnalyses(TransformTypeEnum xTransformAll, TransformTypeEnum yTransformAll)
        {
            UpdateTransformsAll();
            //this.allAnalyses.RemoveAt(this.allAnalyses.Count - 1);
            foreach (Analysis analysis in this.allAnalyses)
            {
                if (analysis is RunAllAnalysis)
                {
                    continue;
                }
                analysis.AnalysisCalculationType = RCalculationType.Full;
                analysis.InFlawTransform = xTransformAll;
                analysis.InResponseTransform= yTransformAll;
                analysis.RunAnalysis();
                //podTables.Add(analysis.Data.PodCurveTable);
                while (analysis.IsAnalysisBusy) { Thread.Sleep(100); }
            }
            foreach (Analysis analysis in this.allAnalyses)
            {
                //analysis.RunAnalysis();
                if (analysis is RunAllAnalysis)
                {
                    continue;
                }
                this.podTables.Add(analysis.Data.PodCurveTable);
            }
            GenerateMinMaxFlaws();
            GenerateMinMaxResponses();
        }
        public void UpdateTransformsAll()
        {
            //This will need to iterate through all the tab with analysis objects to add the 
            //appropriate transforms and/or model types
        }
        public void GatherResponsesNames()
        {
            foreach(Analysis analysis in this.allAnalyses)
            {
                if (analysis is RunAllAnalysis)
                {
                    continue;
                }
                responseLabelNames.Add(analysis.ResponseNames[0]);
            }
        }
        private void GenerateMinMaxFlaws()
        {
            foreach (Analysis analysis in this.allAnalyses)
            {
                if (analysis is RunAllAnalysis)
                {
                    continue;
                }
                if(analysis.InFlawMin < this.minFlawSize)
                {
                    this.minFlawSize = analysis.InFlawMin;
                }
                if (analysis.InFlawMax > this.maxFlawSize)
                {
                    this.maxFlawSize = analysis.InFlawMax;
                }
            }

        }
        private void GenerateMinMaxResponses()
        {
            foreach (Analysis analysis in this.allAnalyses)
            {
                if (analysis is RunAllAnalysis)
                {
                    continue;
                }
                if (analysis.InResponseMin < this.minReponseSize)
                {
                    this.minReponseSize = analysis.InResponseMin;
                }
                if (analysis.InResponseMax > this.maxReponseSize)
                {
                    this.maxReponseSize = analysis.InResponseMax;
                }
            }

        }
        public List<DataTable> PODTables => this.podTables;
        public int AnalysisCount => this.analysisCount;
        public List<string> ResponseNamesAll => this.responseLabelNames;
        public double OverallFlawMax => this.maxFlawSize;
        public double OverallFlawMin => this.minFlawSize;
        public double OverallResponseMin => this.minReponseSize;
        public double OverallResponseMax => this.maxReponseSize;
    }
}
