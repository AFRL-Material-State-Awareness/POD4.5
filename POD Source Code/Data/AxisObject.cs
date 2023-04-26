using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POD.Data
{
    public class AxisObject
    {
        public double Max = 1.0;
        public double Min = -1.0;
        public double IntervalOffset = 0.0;
        public double Interval = .25;
        public double BufferPercentage = 5.0;


        public AxisObject Clone()
        {
            var axis = new AxisObject();

            axis.Max = Max;
            axis.Min = Min;
            axis.IntervalOffset = IntervalOffset;
            axis.Interval = Interval;
            axis.BufferPercentage = BufferPercentage;

            return axis;
        }
    }
}
