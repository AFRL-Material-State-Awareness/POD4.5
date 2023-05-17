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

namespace POD.Wizards
{
    public partial class SideChartPanel : WizardPanel
    {
        public List<DataPointChart> SideCharts;
        protected bool _hasBeenShown = false;
        List<int> _presetSizes = new List<int>();
        List<bool> _includesScrollBar = new List<bool>();
        int _sizeIndex = 0;
        //int prevSize = 0;
        private bool handleEvents = true;

        public FlowLayoutPanel SidePanel { get; set; }

        public SideSplitter SideSplitter { get; set; }

        public BlendPictureBox SplitterBox { get; set; }

        public SideChartPanel() : base()
        {
            Initialize();
        }

        public SideChartPanel(PODToolTip tooltip)
            : base(tooltip)
        {
            StepToolTip = new PODToolTip();

            Initialize();
        }

        private void Initialize()
        {
            InitializeComponent();


            SideCharts = new List<DataPointChart>();

            Load += SideChartPanel_Load;
        }

        void SideChartPanel_Load(object sender, EventArgs e)
        {
            if(!DesignMode)
                SetSideChartSize(3);
        }

        public virtual void AddSideCharts()
        {
            
        }

        protected void DisposeOfSideChartPanelObjects()
        {
            try
            {
                foreach (DataPointChart chart in SideCharts)
                    chart.DisposeOfAllItems();
            }
            catch (NullReferenceException) { }
            SidePanel.Dispose();
            SideSplitter.Dispose();
            SplitterBox.Dispose();
            base.Dispose(true);
            Dispose(true);
        }
        protected void SetSideControls(FlowLayoutPanel graphFlowPanel, SideSplitter graphSplitter, BlendPictureBox myBlendBox)
        {
            SidePanel = graphFlowPanel;
            SideSplitter = graphSplitter;
            SplitterBox = myBlendBox;

            SideSplitter.SplitterMoved += GraphSplitter_Moved;
            SideSplitter.SplitterMoving += SideSplitter_SplitterMoving;
            SideSplitter.SplitterIncrement = 5;
            SidePanel.BackColor = SystemColors.Control;
            SideSplitter.MouseDown += SideSplitter_MouseDown;
            SideSplitter.MouseUp += SideSplitter_MouseUp;
            SideSplitter.MouseLeave += SideSplitter_MouseLeave;
            SplitterBox.Transparency = 70.0F;
            SplitterBox.DrawVerticalLine = true;
            SplitterBox.PossibleLines = _presetSizes;
        }

        void SideSplitter_MouseLeave(object sender, EventArgs e)
        {
            if (SplitterBox.Visible == true)
            {
                SideSplitter.Visible = true;
                SplitterBox.Visible = false;
            }
        }

        void SideSplitter_MouseUp(object sender, MouseEventArgs e)
        {
            GraphSplitter_Moved(null, null);
        }

        void SideSplitter_MouseDown(object sender, MouseEventArgs e)
        {
            Bitmap image = new Bitmap(SideSplitter.Width, SideSplitter.Height);

            SideSplitter.DrawToBitmap(image, new Rectangle(0, 0, SideSplitter.Width, SideSplitter.Height));

            SplitterBox.BackgroundImage = image;

            SplitterBox.Visible = true;
            SplitterBox.BringToFront();
            SideSplitter.Visible = false;
            SideSplitter.SendToBack();

            SplitterBox.MouseX = SideSplitter.SplitterDistance;

            Cursor = Cursors.VSplit;
        }

        public void ResizeAlternateChart()
        {
            //to hopefully fix error Floyd is having
            if (Visible == true && _presetSizes.Count > 0 && SideSplitter != null && SidePanel != null && _includesScrollBar.Count > 0)
            {
                if (_presetSizes.Count <= _sizeIndex)
                    SnapToNearestChartSize(SideSplitter.SplitterDistance);

                int size = _presetSizes[_sizeIndex];

                if (_includesScrollBar[_sizeIndex] == true)
                    size -= ScrollbarWidth();

                foreach (Control control in SidePanel.Controls)
                {
                    var width = size - control.Margin.Left - control.Margin.Right;

                    if (width > 0)
                    {
                        control.Width = width;

                        control.Height = control.Width;
                    }
                }

                CheckSideCharts();
            }
        }

        int ResizeCount = 0;

        protected void GraphSplitter_Moved(object sender, SplitterEventArgs e)
        {

            if (handleEvents == true)
            {
                int newSize = SnapToNearestChartSize(SideSplitter.SplitterDistance);

                if (ResizeCount < 20)
                {

                    if (SideSplitter.SplitterDistance != newSize && newSize > 20)
                    {
                        //if (newSize < 100)
                        //    newSize = 100;
                        ResizeCount++;

                        SideSplitter.SplitterDistance = newSize;

                        ResizeAlternateChart();

                        SplitterBox.VerticalLineX = newSize;

                        SideSplitter.Visible = true;
                        SideSplitter.BringToFront();
                        SplitterBox.Visible = false;
                        SplitterBox.SendToBack();

                        int difference = SideSplitter.PointToClient(System.Windows.Forms.Cursor.Position).X - newSize;

                        //System.Windows.Forms.Cursor lol = new System.Windows.Forms.Cursor(System.Windows.Forms.Cursor.Current.Handle);
                        System.Windows.Forms.Cursor.Position = new Point(System.Windows.Forms.Cursor.Position.X - difference,
                                                                            System.Windows.Forms.Cursor.Position.Y);

                        SideSplitter.Panel2.Controls[0].Refresh();
                    }
                    else
                    {
                        ResizeAlternateChart();

                        SideSplitter.BringToFront();
                        SplitterBox.SendToBack();
                    }
                }
                else
                {
                    if (newSize == SideSplitter.SplitterDistance)
                        ResizeCount = 0;
                }

                GetMainChart().ForceResizeAnnotations();

                Cursor = Cursors.Default;
            }
            
        }
        
        void SideSplitter_SplitterMoving(object sender, SplitterCancelEventArgs e)
        {
            if (handleEvents == true)
            {
                int newSize = SnapToNearestChartSize(e.SplitX);

                if (SplitterBox.VerticalLineX != newSize)
                {
                    SplitterBox.VerticalLineX = newSize;
                }
            }
        }

        private int SnapToNearestChartSize(int myXValue)
        {
            _presetSizes.Clear();
            _includesScrollBar.Clear();
            int length = myXValue;

            for (int i = 6; i >= 1; i-- )
            {
                int width = (SideSplitter.Panel2.Height) / i;

                if (i < SidePanel.Controls.Count)
                {
                    width += ScrollbarWidth();
                    _includesScrollBar.Add(true);
                }
                else
                {
                    _includesScrollBar.Add(false);
                }

                _presetSizes.Add(width);
            }

            _presetSizes.Sort();
            _sizeIndex = _presetSizes.BinarySearch(length);

            if (_sizeIndex < 0)
            {
                _sizeIndex = ~_sizeIndex;

                if (_sizeIndex == _presetSizes.Count)
                    _sizeIndex--;

                if (_sizeIndex > 0)
                {
                    int moreThan = _presetSizes[_sizeIndex] - length;
                    int lessThan = length - _presetSizes[_sizeIndex - 1];

                    if (lessThan < moreThan)
                    {
                        _sizeIndex--;
                    }
                }
            }

            

            length = _presetSizes[_sizeIndex];

            return length;
        }

        public void CheckSideCharts()
        {
            if (SidePanel.Controls.Count == 0)
            {
                SideSplitter.Panel1Collapsed = true;
            }
            else
            {
                SideSplitter.Panel1Collapsed = false;
            }
        }

        public void SetSideChartSize(int myChartShowCount)
        {
            if (SideSplitter != null)
            {
                var length = SnapToNearestChartSize(0);
                _sizeIndex = _presetSizes.Count - myChartShowCount;

                SideSplitter.SplitterDistance = _presetSizes[_sizeIndex];

                ResizeAlternateChart();
            }
        }

        public void SetupSideCharts()
        {
            foreach(DataPointChart chart in SideCharts)
            {
                chart.SetupChart(Analysis.Data.AvailableFlawNames[0], Analysis.Data.AvailableFlawUnits[0], 
                                 Analysis.Data.AvailableResponseNames, Analysis.Data.AvailableResponseUnits);
            }
        }

        private int ScrollbarWidth()
        {
            return SystemInformation.VerticalScrollBarWidth;
        }

        public bool DisplayChart(int myIndex)
        {
            DataPointChart chart = SideCharts[myIndex];
            var indices = new List<int>();
            bool show = false;

            foreach(Control side in SidePanel.Controls)
            {
                var thereChart = side as DataPointChart;

                indices.Add(SideCharts.IndexOf(thereChart));
            }

            if (SidePanel.Contains(chart) == false)
            {
                var relIndex = 0;

                foreach(int index in indices)
                {
                    if(myIndex < index)
                    {                        
                        break;
                    }

                    relIndex++;

                }

                SidePanel.Controls.Add(chart);
                SidePanel.Controls.SetChildIndex(chart, relIndex);

                show = true;
            }
            else
            {
                SidePanel.Controls.Remove(chart);

                show = false;
            }

            //setup splitter distance first time chart is shown
            if (_hasBeenShown == false && show == true)
            {
                _hasBeenShown = true;

                SideSplitter.SplitterDistance = SideSplitter.Panel2.Height / 3;
            }

            CheckSideCharts();
            int newSize = SnapToNearestChartSize(SideSplitter.SplitterDistance);

            if (SideSplitter.SplitterDistance != newSize)
            {
                SideSplitter.SplitterDistance = newSize;
            }

            ResizeAlternateChart();

            SidePanel.ScrollControlIntoView(chart);

            var mainChart = GetMainChart();

            mainChart.ForceResizeAnnotations();

            return show;
        }

        public bool ScrollToChart(int myIndex)
        {
            if(myIndex < SideCharts.Count)
            {
                SidePanel.ScrollControlIntoView(SideCharts[myIndex]);

                return true;
            }

            return false;
        }

        private DataPointChart GetMainChart()
        {
            foreach (Control control in SideSplitter.Panel2.Controls)
            {
                var chart = control as DataPointChart;

                if(chart != null)
                {
                    return chart;
                }
            }

            return null;
        }

        
    }
}
