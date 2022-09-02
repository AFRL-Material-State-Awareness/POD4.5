using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace CSharpBackendWithR
{
    public class TransformBackCSharpTablesAHAT
    {
        private double lambda;
        private int modelType;
        private AHatAnalysisObject aHatAnalysisObject;
        public TransformBackCSharpTablesAHAT(AHatAnalysisObject aHatAnalysisObjectInput)
        {
            this.aHatAnalysisObject = aHatAnalysisObjectInput;
            this.lambda = this.aHatAnalysisObject.Lambda;
            this.modelType = aHatAnalysisObject.ModelType;
        }
        public DataTable ConvertFitResidualsTable(DataTable _fitResidualsTable)
        {
            //used to transform the POD curve back to linear space
            switch (this.modelType)
            {
                case 1:
                    break;
                case 2:
                    for (int i = 0; i < _fitResidualsTable.Rows.Count; i++)
                    {
                        _fitResidualsTable.Rows[i][0] = Math.Exp(Convert.ToDouble(_fitResidualsTable.Rows[i][0]));
                    }
                    break;
                case 3:
                    for (int i = 0; i < _fitResidualsTable.Rows.Count; i++)
                    {
                        _fitResidualsTable.Rows[i][2] = Math.Exp(Convert.ToDouble(_fitResidualsTable.Rows[i][2]));
                    }
                    break;
                case 4:
                    for (int i = 0; i < _fitResidualsTable.Rows.Count; i++)
                    {
                        _fitResidualsTable.Rows[i][0] = Math.Exp(Convert.ToDouble(_fitResidualsTable.Rows[i][0]));
                        _fitResidualsTable.Rows[i][2] = Math.Exp(Convert.ToDouble(_fitResidualsTable.Rows[i][2]));
                    }
                    break;
                case 5:
                    //transform back box cox here;
                    for (int i = 0; i < _fitResidualsTable.Rows.Count; i++)
                    {
                        _fitResidualsTable.Rows[i][2] = NthRoot(Convert.ToDouble(_fitResidualsTable.Rows[i][2]) * this.lambda + 1, this.lambda);
                    }
                    break;
                case 6:
                    //transform back box cox here;
                    for (int i = 0; i < _fitResidualsTable.Rows.Count; i++)
                    {
                        _fitResidualsTable.Rows[i][0] = Math.Exp(Convert.ToDouble(_fitResidualsTable.Rows[i][0]));
                        _fitResidualsTable.Rows[i][2] = NthRoot(Convert.ToDouble(_fitResidualsTable.Rows[i][2]) * this.lambda + 1, this.lambda);
                    }
                    break;
                case 7:
                    //transform back box cox here;
                    for (int i = 0; i < _fitResidualsTable.Rows.Count; i++)
                    {
                        _fitResidualsTable.Rows[i][0] = 1.0 / Convert.ToDouble(_fitResidualsTable.Rows[i][0]);
                        _fitResidualsTable.Rows[i][2] = NthRoot(Convert.ToDouble(_fitResidualsTable.Rows[i][2]) * this.lambda + 1, this.lambda);
                    }
                    break;
                case 8:
                    //transform back box cox here;
                    for (int i = 0; i < _fitResidualsTable.Rows.Count; i++)
                    {
                        _fitResidualsTable.Rows[i][2] = 1.0 / Convert.ToDouble(_fitResidualsTable.Rows[i][2]);
                    }
                    break;
                case 9:
                    //transform back box cox here;
                    for (int i = 0; i < _fitResidualsTable.Rows.Count; i++)
                    {
                        _fitResidualsTable.Rows[i][0] = Math.Exp(Convert.ToDouble(_fitResidualsTable.Rows[i][0]));
                        _fitResidualsTable.Rows[i][2] = 1.0 / Convert.ToDouble(_fitResidualsTable.Rows[i][2]);
                    }
                    break;
                case 10:
                    //transform back box cox here;
                    for (int i = 0; i < _fitResidualsTable.Rows.Count; i++)
                    {
                        _fitResidualsTable.Rows[i][0] = 1.0 / Convert.ToDouble(_fitResidualsTable.Rows[i][0]);
                    }
                    break;
                case 11:
                    //transform back box cox here;
                    for (int i = 0; i < _fitResidualsTable.Rows.Count; i++)
                    {
                        _fitResidualsTable.Rows[i][0] = 1.0 / Convert.ToDouble(_fitResidualsTable.Rows[i][0]);
                        _fitResidualsTable.Rows[i][2] = Math.Exp(Convert.ToDouble(_fitResidualsTable.Rows[i][2]));
                    }
                    break;
                case 12:
                    //transform back box cox here;
                    for (int i = 0; i < _fitResidualsTable.Rows.Count; i++)
                    {
                        _fitResidualsTable.Rows[i][0] = 1.0 / Convert.ToDouble(_fitResidualsTable.Rows[i][0]);
                        _fitResidualsTable.Rows[i][2] = 1.0 / Convert.ToDouble(_fitResidualsTable.Rows[i][2]);
                    }
                    break;
            }
            return _fitResidualsTable;
        }
        public DataTable TransformBackColResidualTables(DataTable myResidTable)
        {
            //copy over transform values
            for (int i = 0; i < myResidTable.Rows.Count; i++)
            {
                myResidTable.Rows[i][2] = Convert.ToDouble(myResidTable.Rows[i][0]);
                myResidTable.Rows[i][3] = Convert.ToDouble(myResidTable.Rows[i][1]);
            }
            switch (modelType)
            {
                case 1:
                    break;
                case 2:
                    for (int i = 0; i < myResidTable.Rows.Count; i++)
                    {
                        myResidTable.Rows[i][0] = Math.Exp(Convert.ToDouble(myResidTable.Rows[i][0]));
                    }
                    break;
                case 3:
                    for (int i = 0; i < myResidTable.Rows.Count; i++)
                    {
                        //tranform back orginal flaws and/or reponses
                        myResidTable.Rows[i][1] = Math.Exp(Convert.ToDouble(myResidTable.Rows[i][1]));
                    }
                    break;
                case 4:
                    for (int i = 0; i < myResidTable.Rows.Count; i++)
                    {
                        //tranform back orginal flaws and/or reponses
                        myResidTable.Rows[i][0] = Math.Exp(Convert.ToDouble(myResidTable.Rows[i][0]));
                        myResidTable.Rows[i][1] = Math.Exp(Convert.ToDouble(myResidTable.Rows[i][1]));
                    }
                    break;
                case 5:
                    for (int i = 0; i < myResidTable.Rows.Count; i++)
                    {
                        myResidTable.Rows[i][1] = NthRoot(Convert.ToDouble(myResidTable.Rows[i][1]) * this.lambda + 1, this.lambda);
                    }
                    break;
                case 6:
                    for (int i = 0; i < myResidTable.Rows.Count; i++)
                    {
                        myResidTable.Rows[i][0] = Math.Exp(Convert.ToDouble(myResidTable.Rows[i][0]));
                        myResidTable.Rows[i][1] = NthRoot(Convert.ToDouble(myResidTable.Rows[i][1]) * this.lambda + 1, this.lambda);
                    }
                    break;
                case 7:
                    for (int i = 0; i < myResidTable.Rows.Count; i++)
                    {
                        myResidTable.Rows[i][0] = 1.0/(Convert.ToDouble(myResidTable.Rows[i][0]));
                        myResidTable.Rows[i][1] = NthRoot(Convert.ToDouble(myResidTable.Rows[i][1]) * this.lambda + 1, this.lambda);
                    }
                    break;
                case 8:
                    for (int i = 0; i < myResidTable.Rows.Count; i++)
                    {
                        //tranform back orginal flaws and/or reponses
                        myResidTable.Rows[i][1] = 1.0 / Convert.ToDouble(myResidTable.Rows[i][1]);
                    }
                    break;
                case 9:
                    for (int i = 0; i < myResidTable.Rows.Count; i++)
                    {
                        //tranform back orginal flaws and/or reponses
                        myResidTable.Rows[i][0] = Math.Exp(Convert.ToDouble(myResidTable.Rows[i][0]));
                        myResidTable.Rows[i][1] = 1.0 / Convert.ToDouble(myResidTable.Rows[i][1]);
                    }
                    break;
                case 10:
                    for (int i = 0; i < myResidTable.Rows.Count; i++)
                    {
                        //tranform back orginal flaws and/or reponses
                        myResidTable.Rows[i][0] = 1.0 / Convert.ToDouble(myResidTable.Rows[i][0]);
                    }
                    break;
                case 11:
                    for (int i = 0; i < myResidTable.Rows.Count; i++)
                    {
                        //tranform back orginal flaws and/or reponses
                        myResidTable.Rows[i][0] = 1.0 / Convert.ToDouble(myResidTable.Rows[i][0]);
                        myResidTable.Rows[i][1] = Math.Exp(Convert.ToDouble(myResidTable.Rows[i][1]));
                    }
                    break;
                case 12:
                    for (int i = 0; i < myResidTable.Rows.Count; i++)
                    {
                        //tranform back orginal flaws and/or reponses
                        myResidTable.Rows[i][0] = 1.0 / Convert.ToDouble(myResidTable.Rows[i][0]);
                        myResidTable.Rows[i][1] = 1.0 / Convert.ToDouble(myResidTable.Rows[i][1]);
                    }
                    break;
            }
            return myResidTable;
        }
        public DataTable DeleteCensoredPointsForRUT(DataTable _residualUncensoredTable)
        {
            for (int i = _residualUncensoredTable.Rows.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < this.aHatAnalysisObject.FlawsCensored.Count(); j++)
                {
                    if (Convert.ToDouble(_residualUncensoredTable.Rows[i][0]) == this.aHatAnalysisObject.FlawsCensored[j])
                    {
                        _residualUncensoredTable.Rows[i].Delete();
                        break;
                    }
                }
            }
            return _residualUncensoredTable;
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
            /*
            if (this.modelType == 1 || this.modelType == 3 || this.modelType == 5)
            {

                for (int i = 0; i < _podCurveTable.Rows.Count; i++)
                {
                    _podCurveTable.Rows[i][0] = Convert.ToDouble(_podCurveTable.Rows[i][0]);
                }
            }
            else if (this.modelType == 2 || this.modelType == 4 || this.modelType == 6)
            {
                for (int i = 0; i < _podCurveTable.Rows.Count; i++)
                {
                    _podCurveTable.Rows[i][0] = Math.Exp(Convert.ToDouble(_podCurveTable.Rows[i][0]));
                }
            }
            */
            switch (this.modelType)
            {
                case 1:
                    for (int i = 0; i < _podCurveTable.Rows.Count; i++)
                    {
                        _podCurveTable.Rows[i][0] = Convert.ToDouble(_podCurveTable.Rows[i][0]);
                    }
                    break;
                case 2:
                    for (int i = 0; i < _podCurveTable.Rows.Count; i++)
                    {
                        _podCurveTable.Rows[i][0] = Math.Exp(Convert.ToDouble(_podCurveTable.Rows[i][0]));
                    }
                    break;
                case 3:
                    for (int i = 0; i < _podCurveTable.Rows.Count; i++)
                    {
                        _podCurveTable.Rows[i][0] = Convert.ToDouble(_podCurveTable.Rows[i][0]);
                    }
                    break;
                case 4:
                    for (int i = 0; i < _podCurveTable.Rows.Count; i++)
                    {
                        _podCurveTable.Rows[i][0] = Math.Exp(Convert.ToDouble(_podCurveTable.Rows[i][0]));
                    }
                    break;
                case 5:
                    for (int i = 0; i < _podCurveTable.Rows.Count; i++)
                    {
                        _podCurveTable.Rows[i][0] = Convert.ToDouble(_podCurveTable.Rows[i][0]);
                    }
                    break;
                case 6:
                    for (int i = 0; i < _podCurveTable.Rows.Count; i++)
                    {
                        _podCurveTable.Rows[i][0] = Math.Exp(Convert.ToDouble(_podCurveTable.Rows[i][0]));
                    }
                    break;
                case 7:
                    for (int i = 0; i < _podCurveTable.Rows.Count; i++)
                    {
                        _podCurveTable.Rows[i][0] = 1.0 / Convert.ToDouble(_podCurveTable.Rows[i][0]);
                    }
                    break;
                case 8:
                    for (int i = 0; i < _podCurveTable.Rows.Count; i++)
                    {
                        _podCurveTable.Rows[i][0] = Convert.ToDouble(_podCurveTable.Rows[i][0]);
                    }
                    break;
                case 9:
                    for (int i = 0; i < _podCurveTable.Rows.Count; i++)
                    {
                        _podCurveTable.Rows[i][0] = Math.Exp(Convert.ToDouble(_podCurveTable.Rows[i][0]));
                    }
                    break;
                case 10:
                case 11:
                case 12:
                    for (int i = 0; i < _podCurveTable.Rows.Count; i++)
                    {
                        _podCurveTable.Rows[i][0] = 1.0 / Convert.ToDouble(_podCurveTable.Rows[i][0]);
                    }
                    break;
            }
            return _podCurveTable;
        }
        public DataTable TransformBackThresholdTable(DataTable _thresholdPlotTable)
        {
            double currThreshold;
            switch (modelType)
            {
                case 1:
                    break;
                case 2:
                    for (int i = 0; i < _thresholdPlotTable.Rows.Count; i++)
                    {
                        _thresholdPlotTable.Rows[i][1] = Math.Exp(Convert.ToDouble(_thresholdPlotTable.Rows[i][1]));
                        _thresholdPlotTable.Rows[i][2] = Math.Exp(Convert.ToDouble(_thresholdPlotTable.Rows[i][2]));
                        _thresholdPlotTable.Rows[i][3] = Math.Exp(Convert.ToDouble(_thresholdPlotTable.Rows[i][3]));
                    }
                    break;
                case 3:
                    for (int i = 0; i < _thresholdPlotTable.Rows.Count; i++)
                    {
                        _thresholdPlotTable.Rows[i][0] = Math.Exp(Convert.ToDouble(_thresholdPlotTable.Rows[i][0]));
                    }
                    break;
                case 4:
                    for (int i = 0; i < _thresholdPlotTable.Rows.Count; i++)
                    {
                        _thresholdPlotTable.Rows[i][0] = Math.Exp(Convert.ToDouble(_thresholdPlotTable.Rows[i][0]));
                        _thresholdPlotTable.Rows[i][1] = Math.Exp(Convert.ToDouble(_thresholdPlotTable.Rows[i][1]));
                        _thresholdPlotTable.Rows[i][2] = Math.Exp(Convert.ToDouble(_thresholdPlotTable.Rows[i][2]));
                        _thresholdPlotTable.Rows[i][3] = Math.Exp(Convert.ToDouble(_thresholdPlotTable.Rows[i][3]));

                    }
                    break;
                case 5:
                    for (int i = 0; i < _thresholdPlotTable.Rows.Count; i++)
                    {
                        currThreshold = Convert.ToDouble(_thresholdPlotTable.Rows[i][0]);
                        _thresholdPlotTable.Rows[i][0] = NthRoot(currThreshold * this.lambda + 1, this.lambda);
                    }
                    break;
                case 6:
                    for (int i = 0; i < _thresholdPlotTable.Rows.Count; i++)
                    {
                        //transform back y thresholds
                        currThreshold = Convert.ToDouble(_thresholdPlotTable.Rows[i][0]);
                        _thresholdPlotTable.Rows[i][0] = NthRoot(currThreshold * this.lambda + 1, this.lambda);
                        //transform back 'a' flaw sizes
                        _thresholdPlotTable.Rows[i][1] = Math.Exp(Convert.ToDouble(_thresholdPlotTable.Rows[i][1]));
                        _thresholdPlotTable.Rows[i][2] = Math.Exp(Convert.ToDouble(_thresholdPlotTable.Rows[i][2]));
                        _thresholdPlotTable.Rows[i][3] = Math.Exp(Convert.ToDouble(_thresholdPlotTable.Rows[i][3]));
                    }
                    break;
                case 7:
                    for (int i = 0; i < _thresholdPlotTable.Rows.Count; i++)
                    {
                        //transform back y thresholds
                        currThreshold = Convert.ToDouble(_thresholdPlotTable.Rows[i][0]);
                        _thresholdPlotTable.Rows[i][0] = NthRoot(currThreshold * this.lambda + 1, this.lambda);
                        //transform back 'a' flaw sizes
                        _thresholdPlotTable.Rows[i][1] = 1.0 / Convert.ToDouble(_thresholdPlotTable.Rows[i][1]);
                        _thresholdPlotTable.Rows[i][2] = 1.0 / Convert.ToDouble(_thresholdPlotTable.Rows[i][2]);
                        _thresholdPlotTable.Rows[i][3] = 1.0 / Convert.ToDouble(_thresholdPlotTable.Rows[i][3]);
                    }
                    break;
                case 8:
                    for (int i = 0; i < _thresholdPlotTable.Rows.Count; i++)
                    {
                        //transform back y thresholds
                        _thresholdPlotTable.Rows[i][0] = 1.0 / Convert.ToDouble(_thresholdPlotTable.Rows[i][0]);
                    }
                    break;
                case 9:
                    for (int i = 0; i < _thresholdPlotTable.Rows.Count; i++)
                    {
                        //transform back y thresholds
                        _thresholdPlotTable.Rows[i][0] = 1.0 / Convert.ToDouble(_thresholdPlotTable.Rows[i][0]);
                        //transform back 'a' flaw sizes
                        _thresholdPlotTable.Rows[i][1] = Math.Exp(Convert.ToDouble(_thresholdPlotTable.Rows[i][1]));
                        _thresholdPlotTable.Rows[i][2] = Math.Exp(Convert.ToDouble(_thresholdPlotTable.Rows[i][2]));
                        _thresholdPlotTable.Rows[i][3] = Math.Exp(Convert.ToDouble(_thresholdPlotTable.Rows[i][3]));
                    }
                    break;
                case 10:
                    for (int i = 0; i < _thresholdPlotTable.Rows.Count; i++)
                    {
                        //transform back 'a' flaw sizes
                        _thresholdPlotTable.Rows[i][1] = 1.0 / Convert.ToDouble(_thresholdPlotTable.Rows[i][1]);
                        _thresholdPlotTable.Rows[i][2] = 1.0 / Convert.ToDouble(_thresholdPlotTable.Rows[i][2]);
                        _thresholdPlotTable.Rows[i][3] = 1.0 / Convert.ToDouble(_thresholdPlotTable.Rows[i][3]);
                    }
                    break;
                case 11:
                    for (int i = 0; i < _thresholdPlotTable.Rows.Count; i++)
                    {
                        //transform back y thresholds
                        _thresholdPlotTable.Rows[i][0] = Math.Exp(Convert.ToDouble(_thresholdPlotTable.Rows[i][0]));
                        //transform back 'a' flaw sizes
                        _thresholdPlotTable.Rows[i][1] = 1.0 / Convert.ToDouble(_thresholdPlotTable.Rows[i][1]);
                        _thresholdPlotTable.Rows[i][2] = 1.0 / Convert.ToDouble(_thresholdPlotTable.Rows[i][2]);
                        _thresholdPlotTable.Rows[i][3] = 1.0 / Convert.ToDouble(_thresholdPlotTable.Rows[i][3]);
                    }
                    break;
                case 12:
                    for (int i = 0; i < _thresholdPlotTable.Rows.Count; i++)
                    {
                        //transform back y thresholds
                        _thresholdPlotTable.Rows[i][0] = 1.0 / Convert.ToDouble(_thresholdPlotTable.Rows[i][0]);
                        //transform back 'a' flaw sizes
                        _thresholdPlotTable.Rows[i][1] = 1.0 / Convert.ToDouble(_thresholdPlotTable.Rows[i][1]);
                        _thresholdPlotTable.Rows[i][2] = 1.0 / Convert.ToDouble(_thresholdPlotTable.Rows[i][2]);
                        _thresholdPlotTable.Rows[i][3] = 1.0 / Convert.ToDouble(_thresholdPlotTable.Rows[i][3]);
                    }
                    break;
            }
            return _thresholdPlotTable;
        }
        private double NthRoot(double A, double root)
        {
            return Math.Pow(A, 1.0 / root);
        }
    }
}
