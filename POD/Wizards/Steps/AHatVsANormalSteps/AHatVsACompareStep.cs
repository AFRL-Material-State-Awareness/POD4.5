using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POD.Controls;
using System.Windows.Forms.DataVisualization.Charting;

namespace POD.Wizards.Steps.AHatVsANormalSteps
{
    public partial class AHatVsACompareStep01 : WizardPanel
    {
        List<PointXY[]> allPoints;
        List<PointXY> allEqus;
        List<PointXY[]> allEquPoints;

       
        public AHatVsACompareStep01(PODToolTip tooltip) : base(tooltip)
        {
            InitializeComponent();

            StepToolTip = new PODToolTip();

            allPoints = new List<PointXY[]>();

            allPoints.Add(new PointXY[100]);
            allPoints.Add(new PointXY[100]);
            allPoints.Add(new PointXY[100]);
            allPoints.Add(new PointXY[100]);

            allEquPoints = new List<PointXY[]>();

            allEquPoints.Add(new PointXY[100]);
            allEquPoints.Add(new PointXY[100]);
            allEquPoints.Add(new PointXY[100]);
            allEquPoints.Add(new PointXY[100]);

            allEqus = new List<PointXY>();

            allEqus.Add(new PointXY());
            allEqus.Add(new PointXY());
            allEqus.Add(new PointXY());
            allEqus.Add(new PointXY());

            logLogChart.ChartAreas[0].AxisX.Title = "Log10(a) (mm)";
            logLinearChart.ChartAreas[0].AxisX.Title = "a (mm)";
            linearLogChart.ChartAreas[0].AxisX.Title = "Log10(a) (mm)";
            linearLinearChart.ChartAreas[0].AxisX.Title = "a (mm)";

            logLogChart.ChartAreas[0].AxisX.TitleFont = new System.Drawing.Font(logLogChart.ChartAreas[0].AxisX.TitleFont.FontFamily, 10.0F, FontStyle.Regular);
            logLinearChart.ChartAreas[0].AxisX.TitleFont = new System.Drawing.Font(logLogChart.ChartAreas[0].AxisX.TitleFont.FontFamily, 10.0F, FontStyle.Regular);
            linearLogChart.ChartAreas[0].AxisX.TitleFont = new System.Drawing.Font(logLogChart.ChartAreas[0].AxisX.TitleFont.FontFamily, 10.0F, FontStyle.Regular);
            linearLinearChart.ChartAreas[0].AxisX.TitleFont = new System.Drawing.Font(logLogChart.ChartAreas[0].AxisX.TitleFont.FontFamily, 10.0F, FontStyle.Regular);

            logLogChart.ChartAreas[0].AxisY.Title = "Log10(a hat) (mm)";
            logLinearChart.ChartAreas[0].AxisY.Title = "Log10(a hat) (mm)";
            linearLogChart.ChartAreas[0].AxisY.Title = "a hat (mm)";
            linearLinearChart.ChartAreas[0].AxisY.Title = "a hat (mm)";

            logLogChart.ChartAreas[0].AxisY.TitleFont = new System.Drawing.Font(logLogChart.ChartAreas[0].AxisX.TitleFont.FontFamily, 10.0F, FontStyle.Regular);
            logLinearChart.ChartAreas[0].AxisY.TitleFont = new System.Drawing.Font(logLogChart.ChartAreas[0].AxisX.TitleFont.FontFamily, 10.0F, FontStyle.Regular);
            linearLogChart.ChartAreas[0].AxisY.TitleFont = new System.Drawing.Font(logLogChart.ChartAreas[0].AxisX.TitleFont.FontFamily, 10.0F, FontStyle.Regular);
            linearLinearChart.ChartAreas[0].AxisY.TitleFont = new System.Drawing.Font(logLogChart.ChartAreas[0].AxisX.TitleFont.FontFamily, 10.0F, FontStyle.Regular);

            logLogChart.Titles.Add(new Title("Log(a hat) vs Log(a)"));
            logLinearChart.Titles.Add(new Title("Log(a hat) vs a"));
            linearLogChart.Titles.Add(new Title("a hat vs Log(a)"));
            linearLinearChart.Titles.Add(new Title("a hat vs a"));

            logLogChart.Titles[0].Font = new System.Drawing.Font(logLogChart.Titles[0].Font.FontFamily, 12.0F, FontStyle.Bold);
            logLinearChart.Titles[0].Font = new System.Drawing.Font(logLogChart.Titles[0].Font.FontFamily, 12.0F, FontStyle.Bold);
            linearLogChart.Titles[0].Font = new System.Drawing.Font(logLogChart.Titles[0].Font.FontFamily, 12.0F, FontStyle.Bold);
            linearLinearChart.Titles[0].Font = new System.Drawing.Font(logLogChart.Titles[0].Font.FontFamily, 12.0F, FontStyle.Bold);


            AddData(logLogChart, allPoints[DataPlots.LogLog], allEqus[DataPlots.LogLog]);
            AddData(logLinearChart, allPoints[DataPlots.LogLin], allEqus[DataPlots.LogLin]);
            AddData(linearLogChart, allPoints[DataPlots.LinLog], allEqus[DataPlots.LinLog]);
            AddData(linearLinearChart, allPoints[DataPlots.LinLin], allEqus[DataPlots.LinLin]);

            SwitchToResiduals();
            SwitchToLinear();

            
        }

        private void AddData(DataPointChart myChart, PointXY[] myPoints, PointXY myEq)
        {
            myChart.Series.Clear();

            Random rand = new Random();            
            Series points2 = myChart.Series.Add(rand.Next(0, 1000).ToString());
            Series points = myChart.Series.Add(rand.Next(0, 1000).ToString());

            myChart.Legends.Clear();
            points.ChartType = SeriesChartType.Point;
            points2.ChartType = SeriesChartType.Line;

            //points2.MarkerStyle = MarkerStyle.None;
            points.MarkerStyle = MarkerStyle.Circle;
            points2.Color = Color.Red;
            points2.BorderWidth = 2;



            for (int i = 0; i < 100; i++)
            {
                myPoints[i] = new PointXY();
                myPoints[i].X = Math.Pow(10, i / 100.0) +rand.Next(1, 10) / 10.0;
                myPoints[i].Y = Math.Pow(10, i / 100.0) +rand.Next(1, 10) / 10.0;
                points.Points.AddXY( myPoints[i].X, myPoints[i].Y); //rand.Next(1, i+2) + i*i
            }

            myEq.X = 1.0;
            myEq.Y = 0.0;
        }

        private void AddEquData(DataPointChart myChart, int myType, PointXY[] myEqPoints, double m, double b)
        {
            Random rand = new Random();
            Series points = myChart.Series[PlotType.Fit];

            myChart.Legends.Clear();
            points.Points.Clear();

            switch(myType)
            { 
                case 3: //linlin
                for (int i = 0; i < 100; i++)
                {
                    myEqPoints[i] = new PointXY();
                    myEqPoints[i].X = myChart.Series[PlotType.Data].Points[i].XValue;
                    myEqPoints[i].Y = myChart.Series[PlotType.Data].Points[i].XValue * m + b;
                    points.Points.AddXY(myEqPoints[i].X, myEqPoints[i].Y); //rand.Next(1, i+2) + i*i
                }
                break;
                case 2: //linlog
                for (int i = 0; i < 100; i++)
                {
                    myEqPoints[i] = new PointXY();
                    myEqPoints[i].X = myChart.Series[PlotType.Data].Points[i].XValue;
                    myEqPoints[i].Y = m * Math.Log10(myChart.Series[PlotType.Data].Points[i].XValue) + b;
                    points.Points.AddXY(myEqPoints[i].X, myEqPoints[i].Y); //rand.Next(1, i+2) + i*i
                }
                break;
                case 1: //loglin
                for (int i = 0; i < 100; i++)
                {
                    myEqPoints[i] = new PointXY();
                    myEqPoints[i].X = myChart.Series[PlotType.Data].Points[i].XValue;
                    myEqPoints[i].Y = Math.Pow(10, m * myChart.Series[PlotType.Data].Points[i].XValue + b);
                    points.Points.AddXY(myEqPoints[i].X, myEqPoints[i].Y); //rand.Next(1, i+2) + i*i*/
                }
                break;
                default: //loglog
                for (int i = 0; i < 100; i++)
                {
                    myEqPoints[i] = new PointXY();
                    myEqPoints[i].X = myChart.Series[PlotType.Data].Points[i].XValue;
                    myEqPoints[i].Y = Math.Pow(10, m * Math.Log10(myChart.Series[PlotType.Data].Points[i].XValue) + b);
                    points.Points.AddXY(myEqPoints[i].X, myEqPoints[i].Y); //rand.Next(1, i+2) + i*i
                }
                break;
            }
        }

        public void SwitchToResiduals()
        {
            double m = 0.0, b = 0.0;

            logLogChart.ChartAreas[0].AxisY.IsLogarithmic = false;
            logLinearChart.ChartAreas[0].AxisY.IsLogarithmic = false;
            logLogChart.ChartAreas[0].AxisX.IsLogarithmic = false;
            linearLogChart.ChartAreas[0].AxisX.IsLogarithmic = false;

            SwitchXtoLog(logLogChart.Series[PlotType.Data].Points);
            SwitchYtoLog(logLogChart.Series[PlotType.Data].Points);
            SwitchXtoLog(linearLogChart.Series[PlotType.Data].Points);
            SwitchYtoLog(logLinearChart.Series[PlotType.Data].Points);

            LeastSquaresFitLinear(linearLinearChart.Series[PlotType.Data].Points, ref m, ref b);
            allEqus[DataPlots.LinLin].X = m;
            allEqus[DataPlots.LinLin].Y = b;
            
            LeastSquaresFitLinear(linearLogChart.Series[PlotType.Data].Points, ref m, ref b);
            allEqus[DataPlots.LinLog].X = m;
            allEqus[DataPlots.LinLog].Y = b;
            
            LeastSquaresFitLinear(logLinearChart.Series[PlotType.Data].Points, ref m, ref b);
            allEqus[DataPlots.LogLin].X = m;
            allEqus[DataPlots.LogLin].Y = b;
            
            LeastSquaresFitLinear(logLogChart.Series[PlotType.Data].Points, ref m, ref b);
            allEqus[DataPlots.LogLog].X = m;
            allEqus[DataPlots.LogLog].Y = b;

            SwitchXtoLinear(logLogChart.Series[PlotType.Data].Points);
            SwitchYtoLinear(logLogChart.Series[PlotType.Data].Points);
            SwitchXtoLinear(linearLogChart.Series[PlotType.Data].Points);
            SwitchYtoLinear(logLinearChart.Series[PlotType.Data].Points);

            AddEquData(linearLinearChart, DataPlots.LinLin, allEquPoints[DataPlots.LinLin], allEqus[DataPlots.LinLin].X, allEqus[DataPlots.LinLin].Y);
            AddEquData(linearLogChart, DataPlots.LinLog, allEquPoints[DataPlots.LinLog], allEqus[DataPlots.LinLog].X, allEqus[DataPlots.LinLog].Y);
            AddEquData(logLinearChart, DataPlots.LogLin, allEquPoints[DataPlots.LogLin], allEqus[DataPlots.LogLin].X, allEqus[DataPlots.LogLin].Y);
            AddEquData(logLogChart, DataPlots.LogLog, allEquPoints[DataPlots.LogLog], allEqus[DataPlots.LogLog].X, allEqus[DataPlots.LogLog].Y);

            SubtractExpected(linearLinearChart.Series[PlotType.Data].Points, DataPlots.LinLin, allEqus[DataPlots.LinLin].X, allEqus[DataPlots.LinLin].Y);
            SubtractExpected(linearLogChart.Series[PlotType.Data].Points, DataPlots.LinLog, allEqus[DataPlots.LinLog].X, allEqus[DataPlots.LinLog].Y);
            SubtractExpected(logLinearChart.Series[PlotType.Data].Points, DataPlots.LogLin, allEqus[DataPlots.LogLin].X, allEqus[DataPlots.LogLin].Y);
            SubtractExpected(logLogChart.Series[PlotType.Data].Points, DataPlots.LogLog, allEqus[DataPlots.LogLog].X, allEqus[DataPlots.LogLog].Y);

            /*SwitchXtoLinear(logLogChart.Series[PlotType.Data].Points);
            SwitchYtoLinear(logLogChart.Series[PlotType.Data].Points);
            SwitchXtoLinear(linearLogChart.Series[PlotType.Data].Points);
            SwitchYtoLinear(logLinearChart.Series[PlotType.Data].Points);

            SwitchXtoLinear(logLogChart.Series[PlotType.Fit].Points);
            SwitchYtoLinear(logLogChart.Series[PlotType.Fit].Points);
            SwitchXtoLinear(linearLogChart.Series[PlotType.Fit].Points);
            SwitchYtoLinear(logLinearChart.Series[PlotType.Fit].Points);*/

            for (int i = 0; i < 100; i++)
            {
                logLogChart.Series[PlotType.Fit].Points[i].YValues[0] = 0.0;
                logLinearChart.Series[PlotType.Fit].Points[i].YValues[0] = 0.0;
                linearLogChart.Series[PlotType.Fit].Points[i].YValues[0] = 0.0;
                linearLinearChart.Series[PlotType.Fit].Points[i].YValues[0] = 0.0;
            }

            logLogChart.ChartAreas[0].RecalculateAxesScale();
            logLinearChart.ChartAreas[0].RecalculateAxesScale();
            linearLogChart.ChartAreas[0].RecalculateAxesScale();
            linearLinearChart.ChartAreas[0].RecalculateAxesScale();

            ScaleYAxisToMaxRange();

            logLogChart.ChartAreas[0].AxisX.Title = "a (mm)";
            logLinearChart.ChartAreas[0].AxisX.Title = "a (mm)";
            linearLogChart.ChartAreas[0].AxisX.Title = "a (mm)";
            linearLinearChart.ChartAreas[0].AxisX.Title = "a (mm)";

            logLogChart.ChartAreas[0].AxisY.Title = "Residual for â (mm)";
            logLinearChart.ChartAreas[0].AxisY.Title = "Residual for â (mm)";
            linearLogChart.ChartAreas[0].AxisY.Title = "Residual for â (mm)";
            linearLinearChart.ChartAreas[0].AxisY.Title = "Residual for â (mm)";

            logLogChart.Titles[0].Text = "Residual: Log(â) vs Log(a)";
            logLinearChart.Titles[0].Text = "Residual: Log(â) vs a";
            linearLogChart.Titles[0].Text = "Residual: â vs Log(a)";
            linearLinearChart.Titles[0].Text = "Residual: â vs a";

            IsLinearView = false;
        }

        private void ScaleYAxisToMaxRange()
        {
            List<double> maxList = new List<double>();
            List<double> minList = new List<double>();
            double max;
            double min;

            maxList.Add(logLogChart.ChartAreas[0].AxisY.Maximum);
            maxList.Add(logLinearChart.ChartAreas[0].AxisY.Maximum);
            maxList.Add(linearLogChart.ChartAreas[0].AxisY.Maximum);
            maxList.Add(linearLinearChart.ChartAreas[0].AxisY.Maximum);

            max = maxList.Max();

            minList.Add(logLogChart.ChartAreas[0].AxisY.Minimum);
            minList.Add(logLinearChart.ChartAreas[0].AxisY.Minimum);
            minList.Add(linearLogChart.ChartAreas[0].AxisY.Minimum);
            minList.Add(linearLinearChart.ChartAreas[0].AxisY.Minimum);

            min = minList.Min();

            logLogChart.ChartAreas[0].AxisY.Maximum = max;
            logLinearChart.ChartAreas[0].AxisY.Maximum = max;
            linearLogChart.ChartAreas[0].AxisY.Maximum = max;
            linearLinearChart.ChartAreas[0].AxisY.Maximum = max;
            logLogChart.ChartAreas[0].AxisY.Minimum = min;
            logLinearChart.ChartAreas[0].AxisY.Minimum = min;
            linearLogChart.ChartAreas[0].AxisY.Minimum = min;
            linearLinearChart.ChartAreas[0].AxisY.Minimum = min;
        }

        private void SubtractExpected(DataPointCollection points, int index, double m, double b)
        {
            points.SuspendUpdates();

            switch (index)
            {
                case 3: //linlin
                    for (int i = 0; i < 100; i++)
                    {
                        double x = points[i].XValue;
                        double y = points[i].YValues[0];

                        y = y - (m * x + b);

                        points.AddXY(x, y);
                    }
                    break;
                case 2: //linlog
                    for (int i = 0; i < 100; i++)
                    {
                        double x = points[i].XValue;
                        double y = points[i].YValues[0];

                        y = y - (m * Math.Log10(x) + b);

                        points.AddXY(x, y);
                    }
                    break;
                case 1: //loglin
                    for (int i = 0; i < 100; i++)
                    {
                        double x = points[i].XValue;
                        double y = points[i].YValues[0];

                        y = y - Math.Pow(10, (m * x + b));

                        points.AddXY(x, y);
                    }
                    break;
                default: //loglog
                    for (int i = 0; i < 100; i++)
                    {
                        double x = points[i].XValue;
                        double y = points[i].YValues[0];

                        y = y - Math.Pow(10, (m * Math.Log10(x) + b));

                        points.AddXY(x, y);
                    }
                    break;
            }


            for (int i = 0; i < 100; i++)
            {
                points.RemoveAt(0);
            }

            points.ResumeUpdates();
        }

        public void SwitchToLinear()
        {      
            logLogChart.ChartAreas[0].AxisY.IsLogarithmic = true;
            logLinearChart.ChartAreas[0].AxisY.IsLogarithmic = true;
            logLogChart.ChartAreas[0].AxisX.IsLogarithmic = true;
            linearLogChart.ChartAreas[0].AxisX.IsLogarithmic = true;

            logLogChart.Series[PlotType.Data].Points.Clear();
            logLinearChart.Series[PlotType.Data].Points.Clear();
            linearLogChart.Series[PlotType.Data].Points.Clear();
            linearLinearChart.Series[PlotType.Data].Points.Clear();

            logLogChart.Series[PlotType.Fit].Points.Clear();
            logLinearChart.Series[PlotType.Fit].Points.Clear();
            linearLogChart.Series[PlotType.Fit].Points.Clear();
            linearLinearChart.Series[PlotType.Fit].Points.Clear();

            for (int i = 0; i < 100; i++)
            {
                logLogChart.Series[PlotType.Data].Points.AddXY(allPoints[DataPlots.LogLog][i].X, allPoints[DataPlots.LogLog][i].Y);
                logLinearChart.Series[PlotType.Data].Points.AddXY(allPoints[DataPlots.LogLin][i].X, allPoints[DataPlots.LogLin][i].Y);
                linearLogChart.Series[PlotType.Data].Points.AddXY(allPoints[DataPlots.LinLog][i].X, allPoints[DataPlots.LinLog][i].Y);
                linearLinearChart.Series[PlotType.Data].Points.AddXY(allPoints[DataPlots.LinLin][i].X, allPoints[DataPlots.LinLin][i].Y);                
            }

            AddEquData(logLogChart, DataPlots.LogLog, allEquPoints[DataPlots.LogLog], allEqus[DataPlots.LogLog].X, allEqus[DataPlots.LogLog].Y);
            AddEquData(logLinearChart, DataPlots.LogLin, allEquPoints[DataPlots.LogLin], allEqus[DataPlots.LogLin].X, allEqus[DataPlots.LogLin].Y);
            AddEquData(linearLogChart, DataPlots.LinLog, allEquPoints[DataPlots.LinLog], allEqus[DataPlots.LinLog].X, allEqus[DataPlots.LinLog].Y);
            AddEquData(linearLinearChart, DataPlots.LinLin, allEquPoints[DataPlots.LinLin], allEqus[DataPlots.LinLin].X, allEqus[DataPlots.LinLin].Y);

            logLogChart.Series[PlotType.Fit].Enabled = true;
            logLinearChart.Series[PlotType.Fit].Enabled = true;
            linearLogChart.Series[PlotType.Fit].Enabled = true;
            linearLinearChart.Series[PlotType.Fit].Enabled = true;     

            logLogChart.ChartAreas[0].AxisY.Maximum = Double.NaN;
            logLinearChart.ChartAreas[0].AxisY.Maximum = Double.NaN;
            linearLogChart.ChartAreas[0].AxisY.Maximum = Double.NaN;
            linearLinearChart.ChartAreas[0].AxisY.Maximum = Double.NaN;
            logLogChart.ChartAreas[0].AxisY.Minimum = Double.NaN;
            logLinearChart.ChartAreas[0].AxisY.Minimum = Double.NaN;
            linearLogChart.ChartAreas[0].AxisY.Minimum = Double.NaN;
            linearLinearChart.ChartAreas[0].AxisY.Minimum = Double.NaN;

            logLogChart.ChartAreas[0].AxisX.RoundAxisValues();
            logLogChart.ChartAreas[0].AxisY.RoundAxisValues();
            logLinearChart.ChartAreas[0].AxisX.RoundAxisValues();
            logLinearChart.ChartAreas[0].AxisY.RoundAxisValues();
            linearLogChart.ChartAreas[0].AxisX.RoundAxisValues();
            linearLogChart.ChartAreas[0].AxisY.RoundAxisValues();
            linearLinearChart.ChartAreas[0].AxisX.RoundAxisValues();
            linearLinearChart.ChartAreas[0].AxisY.RoundAxisValues();

            logLogChart.ChartAreas[0].AxisX.Title = "Log10(a) (mm)";
            logLinearChart.ChartAreas[0].AxisX.Title = "a (mm)";
            linearLogChart.ChartAreas[0].AxisX.Title = "Log10(a) (mm)";
            linearLinearChart.ChartAreas[0].AxisX.Title = "a (mm)";

            logLogChart.ChartAreas[0].AxisY.Title = "Log10(â) (mm)";
            logLinearChart.ChartAreas[0].AxisY.Title = "Log10(â) (mm)";
            linearLogChart.ChartAreas[0].AxisY.Title = "â (mm)";
            linearLinearChart.ChartAreas[0].AxisY.Title = "â (mm)";

            logLogChart.Titles[0].Text = "Log(â) vs Log(a)";
            logLinearChart.Titles[0].Text = "Log(â) vs a";
            linearLogChart.Titles[0].Text = "â vs Log(a)";
            linearLinearChart.Titles[0].Text = "â vs a";

            IsLinearView = true;
        }

        public static void SwitchXtoLog(DataPointCollection points)
        {
            for (int i = 0; i < points.Count; i++)
            {
                points[i].XValue = Math.Log10(points[i].XValue);
            }
        }

        public static void SwitchYtoLog(DataPointCollection points)
        {
            for (int i = 0; i < points.Count; i++)
            {
                points[i].YValues[0] = Math.Log10(points[i].YValues[0]);
            }
        }

        private void SwitchXtoLinear(DataPointCollection points)
        {
            for (int i = 0; i < points.Count; i++)
            {
                points[i].XValue = Math.Pow(10, points[i].XValue);
            }
        }

        private void SwitchYtoLinear(DataPointCollection points)
        {
            for (int i = 0; i < points.Count; i++)
            {
                points[i].YValues[0] = Math.Pow(10, points[i].YValues[0]);
            }
        }

        public static void LeastSquaresFitLinear(DataPointCollection points, ref double M, ref double B)
		{
			//Gives best fit of data to line Y = MC + B
			double x1, y1, xy, x2, J;
			int i;

			x1 = 0.0;
			y1 = 0.0;
			xy = 0.0;
			x2 = 0.0;

			for (i = 0; i < points.Count; i++)
			{
                x1 = x1 + points[i].XValue;
                y1 = y1 + points[i].YValues[0];
                xy = xy + points[i].XValue * points[i].YValues[0];
                x2 = x2 + points[i].XValue * points[i].XValue;
			}

            J = ((double)points.Count * x2) - (x1 * x1);
			if (J != 0.0)
			{
                M = (((double)points.Count * xy) - (x1 * y1)) / J;
				M = Math.Floor(1.0E3 * M + 0.5) / 1.0E3;
				B = ((y1 * x2) - (x1 * xy)) / J;
				B = Math.Floor(1.0E3 * B + 0.5) / 1.0E3;
			}
			else
			{
				M = 0;
				B = 0;
			}
		}


        public bool IsLinearView { get; set; }

        private void Panel_Click(object sender, EventArgs e)
        {
            logLogChart.BorderlineColor = Color.Transparent;
            logLinearChart.BorderlineColor = Color.Transparent;
            linearLogChart.BorderlineColor = Color.Transparent;
            linearLinearChart.BorderlineColor = Color.Transparent;

            logLogChart.IsSelected = false;
            logLinearChart.IsSelected = false;
            linearLogChart.IsSelected = false;
            linearLinearChart.IsSelected = false;
            
        }
    }

    public static class DataPlots
    {
        public const int LogLog = 0;
        public const int LogLin = 1;
        public const int LinLog = 2;
        public const int LinLin = 3;
    }

    public static class PlotType
    {
        public const int Fit = 0;
        public const int Data = 1;
    }
}
