using System;
using System.Collections.Generic;
using System.Text;
//using RDotNet;
//used for maxmin function
using System.Linq;
namespace CSharpBackendWithR
{
    class MaxAndMinClass
    {
        //use to get the maximum and minimum crack size for any given analysis
        private Dictionary<string, double> maxAndMinDict;
        private double maxValue;
        private double minValue;
        public MaxAndMinClass()
        {
            this.maxAndMinDict = new Dictionary<string, double>();
            maxValue = double.MaxValue;
            minValue= double.MinValue;
        }
        //function used to get the maximum and minimum crack
        public void calcCrackRange(List<double> flaws)
        {
            
            double maxValue = flaws.Max();
            double minValue = flaws.Min();
            maxAndMinDict.Add(key: "Max", value: maxValue);
            maxAndMinDict.Add(key: "Min", value: minValue);
        }
        public Dictionary<string, double> maxAndMinListControl{
            get{
                return maxAndMinDict;
            }
        }
    }
}
