using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POD.Data
{
    public class AxisObject : IAxisObject
    {

        public double Max { get; set; }
        public double Min { get; set; }
        public double IntervalOffset { get; set; }
        public double Interval { get; set; }
        public double BufferPercentage { get; set; }


        public AxisObject()
        {
            Max = 1.0;
            Min = -1.0;
            IntervalOffset = 0.0;
            Interval = .25;
            BufferPercentage = 5.0;
        }
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

    public interface IAxisObject
    {
        double Max { get; set; }
        double Min { get; set; }
        double IntervalOffset { get; set; }
        double Interval { get; set; }
        double BufferPercentage { get; set; }

        AxisObject Clone();
    }
}
