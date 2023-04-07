using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using POD;

namespace CSharpBackendWithR
{
    public class TransformBackCSharpTablesHITMISS
    {
        private HMAnalysisObject hmAnalysisObject;
        private TransformPairEnum modelType;
        public TransformBackCSharpTablesHITMISS(HMAnalysisObject hmAnalysisObjectInput, double modelTypeInput=0.0)
        {
            this.hmAnalysisObject = hmAnalysisObjectInput;
            this.modelType = this.hmAnalysisObject.ModelType;
        }
        public DataTable TransformBackOrigData(DataTable originalData)
        {
            switch (this.modelType)
            {
                case TransformPairEnum.LinearLinear:
                    for (int i = 0; i < originalData.Rows.Count; i++)
                    {
                        originalData.Rows[i][1] = Convert.ToDouble(originalData.Rows[i][0]);
                    }
                    break;
                case TransformPairEnum.LogLinear:
                    for (int i = 0; i < originalData.Rows.Count; i++)
                    {
                        originalData.Rows[i][1] = Convert.ToDouble(originalData.Rows[i][0]);
                        originalData.Rows[i][0] = Math.Exp(Convert.ToDouble(originalData.Rows[i][0]));
                    }
                    break;
                case TransformPairEnum.InverseLinear:
                    for (int i = 0; i < originalData.Rows.Count; i++)
                    {
                        originalData.Rows[i][1] = Convert.ToDouble(originalData.Rows[i][0]);
                        originalData.Rows[i][0] = 1.0 / Convert.ToDouble(originalData.Rows[i][0]);
                    }
                    break;
            }
            return originalData;
        }
        public DataTable TransformBackPODCurveTable(DataTable podCurveTable)
        {
            switch(this.modelType)
            {
                case TransformPairEnum.LinearLinear:
                    break;
                case TransformPairEnum.LogLinear:
                    for (int i = 0; i < podCurveTable.Rows.Count; i++)
                    {
                        podCurveTable.Rows[i][0] = Math.Exp(Convert.ToDouble(podCurveTable.Rows[i][0]));
                    }
                    break;
                case TransformPairEnum.InverseLinear:
                    for (int i = 0; i < podCurveTable.Rows.Count; i++)
                    {
                        podCurveTable.Rows[i][0] = 1.0 / Convert.ToDouble(podCurveTable.Rows[i][0]);
                    }
                    break;
            }
            return podCurveTable;
        }
        public DataTable TransformBackResidualUncensoredTable(DataTable residualUncensoredTable)
        {
            if (residualUncensoredTable.Columns.Contains("t_trans"))
            {
                residualUncensoredTable.Columns["t_trans"].ColumnName = "t_fit";
            }
            switch (this.modelType)
            {
                case TransformPairEnum.LinearLinear:
                    for (int i = 0; i < residualUncensoredTable.Rows.Count; i++)
                    {
                        residualUncensoredTable.Rows[i][1] = Convert.ToDouble(residualUncensoredTable.Rows[i][0]);
                    }
                    break;
                case TransformPairEnum.LogLinear:
                    for (int i = 0; i < residualUncensoredTable.Rows.Count; i++)
                    {
                        residualUncensoredTable.Rows[i][1] = Convert.ToDouble(residualUncensoredTable.Rows[i][0]);
                        residualUncensoredTable.Rows[i][0] = Math.Exp(Convert.ToDouble(residualUncensoredTable.Rows[i][0]));
                    }
                    break;
                case TransformPairEnum.InverseLinear:
                    for (int i = 0; i < residualUncensoredTable.Rows.Count; i++)
                    {
                        residualUncensoredTable.Rows[i][1] = Convert.ToDouble(residualUncensoredTable.Rows[i][0]);
                        residualUncensoredTable.Rows[i][0] = 1.0 / Convert.ToDouble(residualUncensoredTable.Rows[i][0]);
                    }
                    break;
            }
            return residualUncensoredTable;
        }
    }
}
