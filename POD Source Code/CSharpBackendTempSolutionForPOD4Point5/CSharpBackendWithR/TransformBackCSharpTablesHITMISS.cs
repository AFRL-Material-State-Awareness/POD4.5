// <copyright file="TransformBackCSharpTablesHITMISS.cs" company="The University of Dayton">
// Copyright © 2025 The University of Dayton. All rights reserved.
//
// Licensed under the GNU General Public License, Version 3.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.gnu.org/licenses/gpl-3.0.txt
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

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
        private readonly HMAnalysisObject hmAnalysisObject;
        private readonly double modelType;
        private readonly IDataTransformer displayTransformer;

        public TransformBackCSharpTablesHITMISS(HMAnalysisObject hmAnalysisObjectInput, IDataTransformer displayTransformer)
        {
            this.hmAnalysisObject = hmAnalysisObjectInput;
            this.modelType = this.hmAnalysisObject.ModelType;
            this.displayTransformer = displayTransformer;
        }

        private void PerformBackwardsTransform(DataTable dataTable, string flawColumnName)
        {
            if (!dataTable.Columns.Contains(flawColumnName)) return;

            // Ensure the target transformed column exists.
            const string transformedColumnName = "transformFlaw";
            if (!dataTable.Columns.Contains(transformedColumnName))
            {
                dataTable.Columns.Add(transformedColumnName, typeof(double));
            }

            foreach (DataRow row in dataTable.Rows)
            {
                // The incoming column contains the transformed value from R.
                double transformedValue = Convert.ToDouble(row[flawColumnName]);

                // 1. Copy the transformed value to the 'transformFlaw' column for the charts.
                row[transformedColumnName] = transformedValue;

                // 2. Inverse-transform the value to get the raw flaw size.
                double rawValue = this.displayTransformer.InverseTransformXAxisValue(transformedValue);


                // 3. Overwrite the original column with the calculated raw value.
                row[flawColumnName] = rawValue;
            }
        }

        public DataTable TransformBackOrigData(DataTable originalData)
        {
            // The incoming table from R has transformed flaw data in the 'flaw' column.
            PerformBackwardsTransform(originalData, "flaw");
            return originalData;
        }

        public DataTable TransformBackPODCurveTable(DataTable podCurveTable)
        {
            // The incoming table from R has transformed flaw data in the 'flaw' column.
            PerformBackwardsTransform(podCurveTable, "flaw");
            return podCurveTable;
        }

        public DataTable TransformBackResidualUncensoredTable(DataTable residualUncensoredTable)
        {
            if (residualUncensoredTable.Columns.Contains("t_trans"))
            {
                residualUncensoredTable.Columns["t_trans"].ColumnName = "t_fit";
            }
            
            // The incoming table from R has transformed flaw data in the 'flaw' column.
            PerformBackwardsTransform(residualUncensoredTable, "flaw");
            return residualUncensoredTable;
        }
    }
}
