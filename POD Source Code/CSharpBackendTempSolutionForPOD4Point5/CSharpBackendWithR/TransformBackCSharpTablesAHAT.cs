// <copyright file="TransformBackCSharpTablesAHAT.cs" company="The University of Dayton">
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
using System.Data;
using System.Linq;

namespace CSharpBackendWithR
{
    public class TransformBackCSharpTablesAHAT
    {
        private readonly IDataTransformer displayTransformer;
        private readonly AHatAnalysisObject aHatAnalysisObject;

        public TransformBackCSharpTablesAHAT(IDataTransformer displayTransformer, AHatAnalysisObject aHatAnalysisObjectInput)
        {
            this.displayTransformer = displayTransformer;
            this.aHatAnalysisObject = aHatAnalysisObjectInput;
        }

        public DataTable ConvertFitResidualsTable(DataTable _fitResidualsTable)
        {
            //// The displayTransformer is pre-configured. We call the transform methods,
            //// and it will handle the Linear (no-op) case correctly.
            //for (int i = 0; i < _fitResidualsTable.Rows.Count; i++)
            //{
            //    // Assuming column 0 is X-axis related (e.g., flaw size)
            //    if (_fitResidualsTable.Rows[i][0] != DBNull.Value)
            //        _fitResidualsTable.Rows[i][0] = this.displayTransformer.InverseTransformXAxisValue(Convert.ToDouble(_fitResidualsTable.Rows[i][0]));

            //    // Assuming column 2 is Y-axis related (e.g., fitted value)
            //    if (_fitResidualsTable.Rows[i][2] != DBNull.Value)
            //        _fitResidualsTable.Rows[i][2] = this.displayTransformer.InverseTransformYAxisValue(Convert.ToDouble(_fitResidualsTable.Rows[i][2]));
            //}
            //return _fitResidualsTable;

            if (_fitResidualsTable == null) return _fitResidualsTable;

            // 1. Suspend internal table notifications
            _fitResidualsTable.BeginLoadData();

            try
            {
                int rowCount = _fitResidualsTable.Rows.Count;

                for (int i = 0; i < rowCount; i++)
                {
                    // 2. Access the row object once
                    DataRow row = _fitResidualsTable.Rows[i];

                    // 3. Process Column 0 (X-Axis)
                    object val0 = row[0];
                    if (val0 != DBNull.Value)
                    {
                        row[0] = this.displayTransformer.InverseTransformXAxisValue(Convert.ToDouble(val0));
                    }

                    // 4. Process Column 2 (Y-Axis)
                    object val2 = row[2];
                    if (val2 != DBNull.Value)
                    {
                        row[2] = this.displayTransformer.InverseTransformYAxisValue(Convert.ToDouble(val2));
                    }
                }
            }
            finally
            {
                // 5. Resume and refresh once
                _fitResidualsTable.EndLoadData();
            }

            return _fitResidualsTable;
        }

        public DataTable TransformBackColResidualTables(DataTable myResidTable)
        {
            //// Copy over transformed values to columns 2 and 3
            //for (int i = 0; i < myResidTable.Rows.Count; i++)
            //{
            //    myResidTable.Rows[i][2] = myResidTable.Rows[i][0];
            //    myResidTable.Rows[i][3] = myResidTable.Rows[i][1];
            //}

            //// The displayTransformer is pre-configured. We call the transform methods on columns 0 and 1
            //// to transform them back to their original scale. The transformer will handle the Linear (no-op) case correctly.
            //for (int i = 0; i < myResidTable.Rows.Count; i++)
            //{
            //    // Assuming column 0 is the original flaw (X-axis)
            //    if (myResidTable.Rows[i][0] != DBNull.Value)
            //        myResidTable.Rows[i][0] = this.displayTransformer.InverseTransformXAxisValue(Convert.ToDouble(myResidTable.Rows[i][0]));

            //    // Assuming column 1 is the original response (Y-axis)
            //    if (myResidTable.Rows[i][1] != DBNull.Value)
            //        myResidTable.Rows[i][1] = this.displayTransformer.InverseTransformYAxisValue(Convert.ToDouble(myResidTable.Rows[i][1]));
            //}
            //return myResidTable;

            if (myResidTable == null || myResidTable.Rows.Count == 0) return myResidTable;

            myResidTable.BeginLoadData(); // Performance boost: suspend index/event updates
            try
            {
                int rowCount = myResidTable.Rows.Count;

                for (int i = 0; i < rowCount; i++)
                {
                    DataRow row = myResidTable.Rows[i];

                    // 1. Grab original values once to avoid repeated indexing/casting
                    object val0 = row[0];
                    object val1 = row[1];

                    // 2. Perform the copy to columns 2 and 3
                    row[2] = val0;
                    row[3] = val1;

                    // 3. Transform columns 0 and 1 in place
                    if (val0 != DBNull.Value)
                    {
                        row[0] = this.displayTransformer.InverseTransformXAxisValue(Convert.ToDouble(val0));
                    }

                    if (val1 != DBNull.Value)
                    {
                        row[1] = this.displayTransformer.InverseTransformYAxisValue(Convert.ToDouble(val1));
                    }
                }
            }
            finally
            {
                myResidTable.EndLoadData(); // Re-enable events and indexes
            }

            return myResidTable;
        }

        public void GenCensoredTable(DataTable _residualCensoredTable)
        {
        }

        public DataTable TransformBackPODCurveTable(DataTable _podCurveTable)
        {
            if (_podCurveTable.Columns.Contains("t_fit"))
            {
                _podCurveTable.Columns["t_fit"].ColumnName = "pod";
            }

            for (int i = 0; i < _podCurveTable.Rows.Count; i++)
            {
                // Assuming column 0 is the flaw size (X-axis)
                if (_podCurveTable.Rows[i][0] != DBNull.Value)
                    _podCurveTable.Rows[i][0] = this.displayTransformer.InverseTransformXAxisValue(Convert.ToDouble(_podCurveTable.Rows[i][0]));
            }
            return _podCurveTable;
        }

        public DataTable TransformBackThresholdTable(DataTable _thresholdPlotTable)
        {
            //for (int i = 0; i < _thresholdPlotTable.Rows.Count; i++)
            //{
            //    // Transform back y thresholds (Column 0)
            //    if (_thresholdPlotTable.Rows[i][0] != DBNull.Value)
            //        _thresholdPlotTable.Rows[i][0] = this.displayTransformer.InverseTransformYAxisValue(Convert.ToDouble(_thresholdPlotTable.Rows[i][0]));

            //    // Transform back 'a' flaw sizes (Columns 1, 2, 3)
            //    if (_thresholdPlotTable.Rows[i][1] != DBNull.Value)
            //        _thresholdPlotTable.Rows[i][1] = this.displayTransformer.InverseTransformXAxisValue(Convert.ToDouble(_thresholdPlotTable.Rows[i][1]));
            //    if (_thresholdPlotTable.Rows[i][2] != DBNull.Value)
            //        _thresholdPlotTable.Rows[i][2] = this.displayTransformer.InverseTransformXAxisValue(Convert.ToDouble(_thresholdPlotTable.Rows[i][2]));
            //    if (_thresholdPlotTable.Rows[i][3] != DBNull.Value)
            //        _thresholdPlotTable.Rows[i][3] = this.displayTransformer.InverseTransformXAxisValue(Convert.ToDouble(_thresholdPlotTable.Rows[i][3]));
            //}
            //return _thresholdPlotTable;

            if (_thresholdPlotTable == null) return _thresholdPlotTable;

            // 1. Suspend events to prevent the table from recalculating internal indexes on every edit
            _thresholdPlotTable.BeginLoadData();

            
            if (_thresholdPlotTable == null) return _thresholdPlotTable;

            _thresholdPlotTable.BeginLoadData();
            try
            {
                int rowCount = _thresholdPlotTable.Rows.Count;
                for (int i = 0; i < rowCount; i++)
                {
                    DataRow row = _thresholdPlotTable.Rows[i];

                    // 1. Handle Column 0 (Y-axis) separately
                    if (row[0] != DBNull.Value)
                    {
                        row[0] = this.displayTransformer.InverseTransformYAxisValue(Convert.ToDouble(row[0]));
                    }

                    // 2. Handle Columns 1, 2, and 3 (X-axis) in a tight loop
                    for (int j = 1; j <= 3; j++)
                    {
                        object cellValue = row[j];
                        if (cellValue != DBNull.Value)
                        {
                            row[j] = this.displayTransformer.InverseTransformXAxisValue(Convert.ToDouble(cellValue));
                        }
                    }
                }
            }
            finally
            {
                _thresholdPlotTable.EndLoadData();
            }

            return _thresholdPlotTable;
            
        }
    }
}