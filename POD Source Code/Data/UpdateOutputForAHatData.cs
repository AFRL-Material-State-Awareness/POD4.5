using CSharpBackendWithR;
using POD;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class UpdateOutputForAHatData : IUpdateOutputForAHatData
    {
        private TransformBackCSharpTablesAHAT _backwardsTransform;
        private AHatAnalysisObject _aHatAnalysisObject;
        private IMessageBoxWrap _messageBox;
        public UpdateOutputForAHatData(AHatAnalysisObject ahatAnalysis, IMessageBoxWrap messageBox = null)
        {
            _backwardsTransform = new TransformBackCSharpTablesAHAT(ahatAnalysis);
            _aHatAnalysisObject = ahatAnalysis;
            _messageBox = messageBox ?? new MessageBoxWrap();
        }
        /// <summary>
        /// Update and transform back fitResidualsTable
        /// </summary>
        public void UpdateFitResidualsTable(ref DataTable fitResidualsTable)
        {
            try
            {
                fitResidualsTable = _backwardsTransform.ConvertFitResidualsTable(_aHatAnalysisObject.AHatResultsLinear);
                fitResidualsTable.DefaultView.RowFilter = "";
                fitResidualsTable.DefaultView.Sort = "flaw" + " " + "ASC";
                fitResidualsTable = fitResidualsTable.DefaultView.ToTable();
            }
            catch (Exception exp)
            {
                _messageBox.Show(exp.Message, "POD v4 Reading Residual Fit Error");
            }
        }
        /// <summary>
        /// Update and transform back the residual uncensored table (includes points whether or not they are censored)
        /// </summary>
        public void UpdateResidualUncensoredTable(ref DataTable residualUncensoredTable)
        {
            try
            {
                residualUncensoredTable = _backwardsTransform.TransformBackColResidualTables(_aHatAnalysisObject.AHatResultsResidUncensored);
                residualUncensoredTable = _backwardsTransform.DeleteCensoredPointsForRUT(residualUncensoredTable);
                residualUncensoredTable.DefaultView.Sort = "flaw, y" + " " + "ASC";
                residualUncensoredTable = residualUncensoredTable.DefaultView.ToTable();
            }
            catch (Exception exp)
            {
                _messageBox.Show(exp.Message, "POD v4 Reading Residual Uncensored Error");
            }
        }
        /// <summary>   
        /// Update and transform back residual raw table
        /// </summary>
        public void UpdateResidualRawTable(ref DataTable residualRawTable)
        {
            try
            {
                //in the original code, this table does not take the average of the reponses and instead displays the results for inspector 1?
                residualRawTable = _aHatAnalysisObject.AHatResultsResid;
                residualRawTable = _backwardsTransform.TransformBackColResidualTables(residualRawTable);
                residualRawTable.DefaultView.Sort = "flaw, y" + " " + "ASC";
                residualRawTable = residualRawTable.DefaultView.ToTable();
            }
            catch (Exception exp)
            {
                _messageBox.Show(exp.Message, "POD v4 Reading Residual Raw Error");
            }
        }
        /// <summary>
        /// Update and transform back the residual uncensored table (does not include any censored points)
        /// </summary>
        public void UpdateResidualCensoredTable(ref DataTable residualCensoredTable, DataTable residualRawTable)
        {
            try
            {
                residualCensoredTable = _aHatAnalysisObject.AHatResultsResid;
                if (residualCensoredTable.Rows.Count != 0)
                {
                    residualCensoredTable.Rows.Clear();
                }
                residualCensoredTable = residualCensoredTable.DefaultView.ToTable();
                if (_aHatAnalysisObject.FlawsCensored.Count() != 0)
                {
                    int pointsLeft = _aHatAnalysisObject.FlawsCensored.Count();
                    //TODO: need to cover the condition when two flaws have different reponses
                    for (int i = residualRawTable.Rows.Count - 1; i >= 0; i--)
                    {
                        if (pointsLeft > 0)
                        {
                            for (int j = 0; j < _aHatAnalysisObject.FlawsCensored.Count(); j++)
                            {
                                if (Convert.ToDouble(residualRawTable.Rows[i][0]) == _aHatAnalysisObject.FlawsCensored[j])
                                {
                                    residualCensoredTable.Rows.Add(residualRawTable.Rows[i].ItemArray);
                                    pointsLeft -= 1;
                                    break;
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception exp)
            {
                _messageBox.Show(exp.Message, "POD v4 Reading Residual Censored Error");
            }
        }
        public void UpdateResidualFullCensoredTable(ref DataTable residualFullCensoredTable, DataTable residualRawTable)
        {
            try
            {
                residualFullCensoredTable = _aHatAnalysisObject.AHatResultsResid;
                if (residualFullCensoredTable.Rows.Count != 0)
                {
                    residualFullCensoredTable.Rows.Clear();
                }
                residualFullCensoredTable = residualFullCensoredTable.DefaultView.ToTable();
                if (_aHatAnalysisObject.FlawsCensored.Count() != 0)
                {
                    int pointsLeft = _aHatAnalysisObject.FlawsCensored.Count();
                    //TODO: need to cover the condition when two flaws have different reponses
                    for (int i = residualRawTable.Rows.Count - 1; i >= 0; i--)
                    {
                        if (pointsLeft > 0)
                        {
                            for (int j = 0; j < _aHatAnalysisObject.FlawsCensored.Count(); j++)
                            {
                                if (Convert.ToDouble(residualRawTable.Rows[i][0]) == _aHatAnalysisObject.FlawsCensored[j])
                                {
                                    residualFullCensoredTable.Rows.Add(residualRawTable.Rows[i].ItemArray);
                                    pointsLeft -= 1;
                                    break;
                                }
                            }
                        }

                    }
                }
                //_residualFullCensoredTable.DefaultView.Sort = "t_flaw, t_ave_response" + " " + "ASC";
                residualFullCensoredTable = residualFullCensoredTable.DefaultView.ToTable();
            }
            catch (Exception exp)
            {
                _messageBox.Show(exp.Message, "POD v4 Reading Residual Full Censored Error");
            }
        }
        public void UpdateResidualPartialCensoredTable(ref DataTable residualPartialCensoredTable)
        {
            try
            {
                residualPartialCensoredTable = _aHatAnalysisObject.AHatResultsResid;
                residualPartialCensoredTable.DefaultView.Sort = "transformFlaw, transformResponse" + " " + "ASC";
                residualPartialCensoredTable = residualPartialCensoredTable.DefaultView.ToTable();
            }
            catch (Exception exp)
            {
                _messageBox.Show(exp.Message, "POD v4 Reading Residual Partial Censored Error");
            }
        }
        /// <summary>
        /// Updates and transforms back the POD curve table. Any flaw size that is 0.0 or less will get removed when this method is executed
        /// </summary>
        public void UpdatePODCurveTable(ref DataTable podCurveTable)
        {
            try
            {
                podCurveTable = _backwardsTransform.TransformBackPODCurveTable(_aHatAnalysisObject.AHatResultsPOD);
                podCurveTable.DefaultView.Sort = "flaw, pod" + " " + "ASC";
                podCurveTable = podCurveTable.Select("flaw > 0.0").CopyToDataTable();
                podCurveTable = podCurveTable.DefaultView.ToTable();
            }
            catch (Exception exp)
            {
                _messageBox.Show(exp.Message, "POD v4 Reading POD curve Error");
            }
        }
        /// <summary>
        /// Updates and transforms back the POD curve ALL table. This table is what plots the ghost POD curve when points are removed
        /// </summary>
        public void UpdatePODCurveAllTable(ref DataTable podCurveAll)
        {
            try
            {
                // used to plot the  original ghost curve in the event that the user omits points
                podCurveAll = _backwardsTransform.TransformBackPODCurveTable(_aHatAnalysisObject.AHatResultsPOD_All);
                podCurveAll.DefaultView.Sort = "flaw, pod" + " " + "ASC";
                podCurveAll = podCurveAll.Select("flaw > 0.0").CopyToDataTable();
                podCurveAll = podCurveAll.DefaultView.ToTable();
            }
            catch (Exception exp)
            {
                _messageBox.Show(exp.Message, "POD v4 Reading POD Error");
            }
        }
        /// <summary>
        /// Updates the threshold plot table (this tables shows the key A values at various thresholds)
        /// </summary>
        public void UpdateThresholdCurveTable(ref DataTable thresholdTable)
        {
            try
            {
                thresholdTable = _backwardsTransform.TransformBackThresholdTable(_aHatAnalysisObject.AHatThresholdsTable);
                thresholdTable.DefaultView.Sort = "threshold" + " " + "ASC";
            }
            catch (Exception exp)
            {
                _messageBox.Show(exp.Message, "POD v4 Reading Threshold Error");
            }
            try
            {
                thresholdTable = thresholdTable.Select("threshold > 0.0").CopyToDataTable();
                //_thresholdPlotTable = _thresholdPlotTable.Select("a50 > 0.0").CopyToDataTable();
                //_thresholdPlotTable = _thresholdPlotTable.Select("a90 > 0.0").CopyToDataTable();
                //_thresholdPlotTable = _thresholdPlotTable.Select("a9095 > 0.0").CopyToDataTable();
                //remove infiniti values
                thresholdTable = thresholdTable.Select("threshold < 1.7976931348623157E+308").CopyToDataTable();
                thresholdTable = thresholdTable.DefaultView.ToTable();
            }
            catch (Exception)
            {
                Debug.WriteLine("warning: no valid datapoints found threshold table");
            }
        }
        /// <summary>
        /// Updates the threshold plot table ALL (Used to plot the ghost line when points are removed)
        /// </summary>
        public void UpdateThresholdCurveTableAll(ref DataTable thresholdTableAll)
        {
            try
            {
                //_thresholdPlotTable_All = BackwardsTransform.TransformBackThresholdTable(_aHatAnalysisObject.AHatThresholdsTable);
                thresholdTableAll = _backwardsTransform.TransformBackThresholdTable(_aHatAnalysisObject.AHatThresholdsTable_All);
                //_thresholdPlotTable_All = _aHatAnalysisObject.AHatThresholdsTable_All;
                thresholdTableAll.DefaultView.Sort = "threshold" + " " + "ASC";
                //remove negative values
                thresholdTableAll = thresholdTableAll.Select("threshold > 0.0").CopyToDataTable();
                //remove infiniti values
                thresholdTableAll = thresholdTableAll.Select("threshold <  1.7976931348623157E+308").CopyToDataTable();
                thresholdTableAll = thresholdTableAll.DefaultView.ToTable();
            }
            catch (Exception exp)
            {
                _messageBox.Show(exp.Message, "POD v4 Reading Threshold Error All");
            }
        }
        /// <summary>
        /// Updates Normality table (this table is used to plot a histogram to help the user assess normality.
        /// </summary>
        public void UpdateNormalityTable(ref DataTable noramlityTable, ref DataTable normalityCurveTable)
        {
            try
            {
                noramlityTable = _aHatAnalysisObject.AHatNormalityTable;
                noramlityTable.DefaultView.Sort = "Range, Freq" + " " + "ASC";

                normalityCurveTable = _aHatAnalysisObject.AHatNormalCurveTable;
                noramlityTable.DefaultView.Sort = "Range, Freq" + " " + "ASC";
            }
            catch (Exception exp)
            {
                _messageBox.Show(exp.Message, "NormalityTable reading error");
            }
        }
    }
    public interface IUpdateOutputForAHatData
    {
        void UpdateFitResidualsTable(ref DataTable fitResidualsTable);
        void UpdateResidualUncensoredTable(ref DataTable residualUncensoredTable);
        void UpdateResidualRawTable(ref DataTable residualRawTable);
        void UpdateResidualCensoredTable(ref DataTable residualCensoredTable, DataTable residualRawTable);
        void UpdateResidualFullCensoredTable(ref DataTable residualFullCensoredTable, DataTable residualRawTable);
        void UpdateResidualPartialCensoredTable(ref DataTable residualPartialCensoredTable);
        void UpdatePODCurveTable(ref DataTable podCurveTable);
        void UpdatePODCurveAllTable(ref DataTable podCurveAll);
        void UpdateThresholdCurveTable(ref DataTable thresholdTable);
        void UpdateThresholdCurveTableAll(ref DataTable thresholdTableAll);
        void UpdateNormalityTable(ref DataTable noramlityTable, ref DataTable normalityCurveTable);
    }
}
