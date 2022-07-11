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
