// <copyright file="UpdateOutputForHitMissData.cs" company="The University of Dayton">
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

using CSharpBackendWithR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POD;
namespace Data
{
    public class UpdateOutputForHitMissData : IUpdateOutputForHitMissData
    {
        private TransformBackCSharpTablesHITMISS _backwardsTransform;
        private HMAnalysisObject _hmAnalysisObject;
        private IMessageBoxWrap _messageBox;
        public UpdateOutputForHitMissData(IDataTransformer displayTransformer, HMAnalysisObject hmAnalysis, IMessageBoxWrap messageBox = null)
        {
            _backwardsTransform = new TransformBackCSharpTablesHITMISS(hmAnalysis, displayTransformer);
            _hmAnalysisObject = hmAnalysis;
            _messageBox = messageBox ?? new MessageBoxWrap();
        }
        /// <summary>
        /// Updates and transforms back the original hitmiss data
        /// </summary>
        public void UpdateOriginalData(ref DataTable originalData)
        {
            try
            {
                originalData = _backwardsTransform.TransformBackOrigData(_hmAnalysisObject.HitMissDataOrig);
                originalData.DefaultView.Sort = "transformFlaw ASC";
                originalData = originalData.DefaultView.ToTable();

            }
            catch (Exception exp)
            {
                Globals.ShowErrorMessageBox(exp, "Reading Original Data Error", _messageBox);
            }
        }
        /// <summary>
        /// Update Total Flaw Count
        /// </summary>
        public void UpdateTotalFlawCount(ref int totalFlawCount)
        {
            try
            {
                totalFlawCount = _hmAnalysisObject.Flaws.Count();
            }
            catch (Exception exp)
            {
                Globals.ShowErrorMessageBox(exp, "Reading Total Flaw Count Error", _messageBox);
            }
        }
        /// <summary>
        /// Updates PODCurveTable and transform it back if necessary
        /// </summary>
        public void UpdatePODCurveTable(ref DataTable podCurveTable)
        {
            try
            {
                //table to be passed back for the transformations window in pass fail               
                podCurveTable = _backwardsTransform.TransformBackPODCurveTable(_hmAnalysisObject.LogitFitTable);
                if (podCurveTable.Columns.Contains("transformFlaw"))
                {
                    podCurveTable.Columns.Remove("transformFlaw");
                }
                podCurveTable.DefaultView.Sort = "flaw ASC";
                podCurveTable = podCurveTable.DefaultView.ToTable();
            }
            catch (Exception exp)
            {
                Globals.ShowErrorMessageBox(exp, "Reading POD Curve Table Error", _messageBox);
            }
        }
        /// <summary>
        /// updates residual uncensored table and transforms it back if necessary
        /// </summary>
        public void UpdateResidualUncensoredTable(ref DataTable residualUncensoredTable)
        {
            try
            {
                residualUncensoredTable = _backwardsTransform.TransformBackResidualUncensoredTable(_hmAnalysisObject.ResidualTable);
                residualUncensoredTable.DefaultView.Sort = "transformFlaw ASC";
                residualUncensoredTable = residualUncensoredTable.DefaultView.ToTable();
            }
            catch (Exception exp)
            {
                Globals.ShowErrorMessageBox(exp, "Reading Residual Uncensored Error", _messageBox);
            }
        }
        /// <summary>
        /// update residual partial censored table and transforms it back if necessary
        /// </summary>
        public void UpdateResidualPartialUncensoredTable(ref DataTable residualPartialUncensoredTable)
        {
            try
            {
                residualPartialUncensoredTable = _hmAnalysisObject.ResidualTable;
                residualPartialUncensoredTable.DefaultView.Sort = "transformFlaw ASC";
                residualPartialUncensoredTable = residualPartialUncensoredTable.DefaultView.ToTable();
            }
            catch (Exception exp)
            {
                Globals.ShowErrorMessageBox(exp, "Reading Residual Partial Censored Error", _messageBox);
            }
        }
        /// <summary>
        /// update iterations table (will probably end up removing this later
        /// </summary>
        public void UpdateIterationsTable(ref DataTable iterationsTable)
        {
            try
            {
                iterationsTable = _hmAnalysisObject.IterationTable;
            }
            catch (Exception exp)
            {
                Globals.ShowErrorMessageBox(exp, "Reading Iterations Error", _messageBox);
            }
        }
    }
    public interface IUpdateOutputForHitMissData
    {
        void UpdateOriginalData(ref DataTable originalData);
        void UpdateTotalFlawCount(ref int totalFlawCount);
        void UpdatePODCurveTable(ref DataTable podCurveTable);
        void UpdateResidualUncensoredTable(ref DataTable residualUncensoredTable);
        void UpdateResidualPartialUncensoredTable(ref DataTable residualPartialUncensoredTable);
        void UpdateIterationsTable(ref DataTable iterationsTable);
    }
}
