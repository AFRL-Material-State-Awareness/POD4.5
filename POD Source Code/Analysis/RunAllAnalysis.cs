﻿using System;
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
        private List<DataTable> podTables;
        private double maxFlawSize;
        private double minFlawSize;
        private double maxReponseSize;
        private double minReponseSize;
        public RunAllAnalysis(AnalysisList allAnalysesInput, string flawNameInput = "NONE")
        {
            this.flawName = flawNameInput;
            this.allAnalyses = allAnalysesInput;
            this.podTables = new List<DataTable>();
            this.maxFlawSize = -1;
            this.minFlawSize = Double.MaxValue;
            this.maxReponseSize = -1;
            this.minReponseSize = Double.MaxValue;
        }
        public void RunAllAnalyses()
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
                analysis.RunAnalysis();
                //podTables.Add(analysis.Data.PodCurveTable);
                while (analysis.IsAnalysisBusy) { Thread.Sleep(100); }
            }
            var number = 0;
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
        public double OverallFlawMax => this.maxFlawSize;
        public double OverallFlawMin => this.minFlawSize;
        public double OverallResponseMin => this.minReponseSize;
        public double OverallResponseMax => this.maxReponseSize;
    }
}