﻿/*
using System;
using System.Collections.Generic;
using System.Text;
using MathNet;
namespace CSharpBackendWithR
{
    /// <summary>
    /// This class holds information about a single crack for the analysis
    /// </summary>
    public class HMCrackData
    {
        private int sequence;
        private double crksize; //crack size
        private double crkf; //transformed crack size (log, inverse, etc)
        //private List<CInspectionClass> rawData;
        private int nsets; //number of measurements
        private int count; // number of inspections showing

        //specifically for pass fail
        private int above;
        private int a_transform;
        private bool censored = false;
        public HMCrackData(double singleCrack)
        {
            this.sequence = 0;
            //size of the given crack
            this.crksize = singleCrack;
            //used for the log tranformation of a hit/miss crack
            this.crkf = Math.Log(this.crksize);
            this.nsets = 0;
            this.count = 0;
            //will end up true if the users censor in a crack
            this.censored = false;

        }
        public double CrkSizeControl => this.crksize;
        public double CrkfControl => this.crkf;
    }
}
for (int i = _residualRawTable.Rows.Count - 1; i >= 0; i--)
                {
                    coordinateFound = false;
                    for (int j = 0; j < _aHatAnalysisObject.FlawsCensored.Count(); j++)
                    {
                        if (coordinateFound == false)
                        {
                            if (Convert.ToDouble(_residualRawTable.Rows[i][0]) == _aHatAnalysisObject.FlawsCensored[j])
                            {
                                //the for loops are necessary in case their are two different crack sizes with different responses
                                List<double> responesLeft = _aHatAnalysisObject.ResponsesCensoredLeft[_aHatAnalysisObject.SignalResponseName];
                                List<double> responesRight = _aHatAnalysisObject.ResponsesCensoredRight[_aHatAnalysisObject.SignalResponseName];

                                for (int k = 0; k < responesLeft.Count(); i++)
                                {
                                    if (Convert.ToDouble(_residualRawTable.Rows[i][1]) == responesLeft[i])
                                    {
                                        _residualCensoredTable.Rows.Add(_residualRawTable.Rows[i].ItemArray);
                                        coordinateFound = true;
                                        break;
                                    }
                                }
                                for (int k = 0; k < responesRight.Count(); i++)
                                {
                                    if (Convert.ToDouble(_residualRawTable.Rows[i][1]) == responesRight[i])
                                    {
                                        _residualCensoredTable.Rows.Add(_residualRawTable.Rows[i].ItemArray);
                                        coordinateFound = true;
                                        break;
                                    }
                                }

                            }

                        }
                        else
                        {
                            break;
                        }
                    }
                }
*/