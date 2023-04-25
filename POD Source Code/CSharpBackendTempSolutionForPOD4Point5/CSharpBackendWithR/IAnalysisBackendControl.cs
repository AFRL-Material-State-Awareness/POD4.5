using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpBackendWithR
{
    public interface IAnalysisBackendControl
    {
        
        void ExecuteReqSampleAnalysisTypeHitMiss();
        void ExecuteAnalysisTransforms_HM();
        void ExecuteAnalysisAHat();
        void ExecuteAnalysisTransforms();
        void ExecuteThresholdChange();
        void ReturnHitMissObjects();
        void ReturnSignalResponseObjects();
        void ReturnThresholdChangeAValues();
        HMAnalysisObject HMAnalsysResults { get; }
        AHatAnalysisObject AHatAnalysisResults { get; }
    }
}
