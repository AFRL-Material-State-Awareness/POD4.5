using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using POD.Data;

namespace POD.Controls
{
    public class RelabelParameters
    {
        public AxisObject Axis;
        public Globals.InvertAxisFunction InvertFunc = null;
        public int LabelCount;
        public bool CenterAtZero;
        public TransformTypeEnum TransformType;
        public Globals.InvertAxisFunction TransformFunc = null;


        public RelabelParameters(IAxisObject axis, Globals.InvertAxisFunction invert, 
                                 int labelCount, bool centerAtZero, 
                                 TransformTypeEnum transformType, Globals.InvertAxisFunction transform)
        {
            Axis = axis?.Clone();

            InvertFunc = invert;
            LabelCount = labelCount;
            CenterAtZero = centerAtZero;
            TransformType = transformType;
            TransformFunc = transform;
        }
    }
}
