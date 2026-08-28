﻿// <copyright file="DataPointChart.cs" company="The University of Dayton">
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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing.Imaging;
using System.IO;
using POD.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using CSharpBackendWithR;

namespace POD.Controls
{
    public partial class DataPointChart : Chart
    {
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, bool wParam, int lParam);

        private const int WM_SETREDRAW = 11;

        private int suspendCounter = 0;

        public void SuspendDrawing()
        {
            if (this.IsHandleCreated && suspendCounter++ == 0)
                SendMessage(this.Handle, WM_SETREDRAW, false, 0);
        }

        public void ResumeDrawing()
        {
            suspendCounter--;
            if (suspendCounter == 0)
            {
                SendMessage(this.Handle, WM_SETREDRAW, true, 0);
                this.Refresh();
            }
        }

     
        /// <summary>
        /// image lists
        /// </summary>
        public static System.Windows.Forms.ImageList ContextMenuImageList;
        private static System.Windows.Forms.ImageList ButtonImageList;
        private static System.Windows.Forms.ImageList MenuOffImageList;
        private static System.Windows.Forms.ImageList MenuOnImageList;
        private static System.Windows.Forms.ImageList MenuPressedImageList;
        private static System.Windows.Forms.ImageList WideImageList;
        private static System.Windows.Forms.ImageList FullSizeImageList;
        //
        bool _isSelected = false;
        bool _isHighlighted = false;
        bool mouseInside = false;
        bool _selectable = false;
        bool _canUnselect = false;
        protected TextAnnotation _error = null;
        protected PolygonAnnotation _progressBox = null;
        protected PolygonAnnotation _progressBar = null;
        protected PolygonAnnotation _errorBox = null;
        protected Legend _legend = new Legend();
        protected List<string> _errorsList = new List<string>();
        protected Dictionary<string, Color> _colorMap = new Dictionary<string, Color>();
        protected string _chartTitle = "";
        protected string _xAxisTitle = "";
        protected string _xAxisUnit = "";
        protected string _yAxisTitle = "";
        protected string _yAxisUnit = "";
        public bool AutoNameYAxis = true;
        public bool YAxisNameIsUnitlessConstant = false;
        public bool XAxisNameIsUnitlessConstant = false;
        public Button exportButton = new Button();
        protected bool _showChartTitle = true;
        protected Bitmap _bitmap;
        protected Bitmap _wideBitmap;
        protected Bitmap _aspectBitmap;
        protected Bitmap _sizeBitmap;
        protected Bitmap[,] MenuBitmaps = new Bitmap[3,4];
        protected ToolTip _chartToolTip = null;
        private int _lastWidth = -1;
        protected PODPanel _menuPanel = new PODPanel();
        /// <summary>
        /// This is needed to keep track the lambda value in the event
        /// the user tries to load a POD analysis with a boxcox transformation
        /// </summary>
        private IAnalysisData _analysisData;
        protected IDataTransformer _transformer;
        protected bool _forceZeroXAxis = false;
        protected bool _forceZeroYAxis = false;
        // Controls the clean PowerPoint presentation behavior
        public bool _clampXAxisToZeroOnExport = false;
        public bool _clampYAxisToZeroOnExport = false;

        public ToolTip ChartToolTip
        {
            get { return _chartToolTip; }
            set { _chartToolTip = value; }
        }

        public bool ForceZeroXAxis
        {
            get { return _forceZeroXAxis; }
            set { _forceZeroXAxis = value; }
        }

        public bool ForceZeroYAxis
        {
            get { return _forceZeroYAxis; }
            set { _forceZeroYAxis = value; }
        }

        public bool ClampXAxisToZeroOnExport
        {
            get { return _clampXAxisToZeroOnExport; }
            set { _clampXAxisToZeroOnExport = value; }
        }

        public bool ClampYAxisToZeroOnExport
        {
            get { return _clampYAxisToZeroOnExport; }
            set { _clampYAxisToZeroOnExport = value; }
        }

        public bool ShowChartTitle
        {
            get
            {
                return _showChartTitle;
            }
            set
            {
                _showChartTitle = value;

                UpdateChartTitle();
            }
        }

        protected virtual void ApplyDynamicFontScaling()
        {
            if (ChartAreas.Count > 0 && this.Width > 0 && this.Height > 0)
            {
                // 1. Find the shortest edge
                float minDimension = Math.Min(this.Width, this.Height);

                // 2. Baseline math: 8pt font at 300px
                float calculatedSize = (minDimension / 300f) * 8f;

                // 3. Clamp the sizes so titles never get unreadable or comically huge
                float titleFontSize = Math.Max(7.5f, Math.Min(11f, calculatedSize));

                var titleFont = new Font("Segoe UI", titleFontSize, FontStyle.Regular);

                // 4. Apply dynamically ONLY to Axis Titles
                ChartAreas[0].AxisX.TitleFont = titleFont;
                ChartAreas[0].AxisY.TitleFont = titleFont;

                // (Tick mark labels - LabelStyle.Font - are intentionally left untouched 
                //  so the data numbers remain at a static, highly readable size.)

                // 5. Apply to Chart Title (Proportionally larger and Bold)
                if (Titles.Count > 0)
                    Titles[0].Font = new Font("Segoe UI", titleFontSize + 1.5f, FontStyle.Regular);
            }
        }

        /// <summary>
        /// Load analysis data into the parent class in addition to the regression panel
        /// </summary>
        /// <param name="data"></param>
        public virtual void LoadChartData(IAnalysisData data)
        {
            _analysisData = data;
        }

        public void UpdateTransformer(IDataTransformer transformer)
        {
            _transformer = transformer;
        }

        public void UpdateTransformer(IDataTransformer transformer, TransformTypeEnum yAxis, TransformTypeEnum xAxis)
        {
            _transformer = transformer;
            _transformer.ForceAxisTransforms(yAxis, xAxis);
        }

        public virtual void LoadChartData(IAnalysisData data, IDataTransformer transformer)
        {
            _analysisData = data;
            _transformer = transformer;
        }

        public DataPointChart()
        {
            _transformer = new DataTransformer();

            InitializeComponent();
            if (ContextMenuImageList == null || ContextMenuImageList.Images.Count==0)
                SetUpImageLists();

            // --- ADD THESE TWO LINES TO FIX THE "||" TEXT RENDERING BUG ---
            this.TextAntiAliasingQuality = TextAntiAliasingQuality.High;
            this.AntiAliasing = AntiAliasingStyles.All;

            _errorsList = new List<string>();

            MouseEnter += AHatVsAChart_MouseEnter;
            MouseLeave += AHatVsAChart_MouseLeave;
            MouseClick += AHatVsAChart_MouseClick;
            MouseDoubleClick += AHatVsAChart_MouseClick;
            Resize += DataPointChart_Resize;
            Customize += DataPointChart_Customize;
            HasFailedDrawing += DataPointChart_HasFailedDrawing;

            _selectable = true;

            DoubleBuffered = true;
            //Scale(new SizeF(5.0F, 5.0F));

            IsSoftShadows = true;

            //all charts should be added through SetupChart()
            Series.Clear();

            Legends.Clear();

            ChartAreas.Clear();

            if (!this.DesignMode)
            {
                _legend.Name = "Data Point Legend";
                _legend.LegendStyle = LegendStyle.Table;
                _legend.Docking = Docking.Top;
                _legend.Alignment = StringAlignment.Center;
                _legend.Font = new System.Drawing.Font(_legend.Font.FontFamily, 10.0F);

                ChartAreas.Add(new ChartArea("podArea"));

                // Force a modern UI font globally for the axes
                ChartAreas[0].AxisX.TitleFont = new Font("Segoe UI", 10.0f, FontStyle.Regular);
                ChartAreas[0].AxisY.TitleFont = new Font("Segoe UI", 10.0f, FontStyle.Regular);
            }

            PostPaint += DataPointChart_PostPaint;

            SingleSeriesCount = 1;

            _bitmap = new Bitmap(ColorManager.TransformImage(new Bitmap(ButtonImageList.Images[0])));
            _wideBitmap = new Bitmap(ColorManager.TransformImage(new Bitmap(WideImageList.Images[0])));
            _aspectBitmap = new Bitmap(ColorManager.TransformImage(new Bitmap(WideImageList.Images[5])));
            _sizeBitmap = new Bitmap(ColorManager.TransformImage(new Bitmap(WideImageList.Images[5])));

            //ChartAreas.Add(new ChartArea());
            DoPostPaint = true;

            TurnOffAllMenuButtons();

            this.GetToolTipText += DataPointChart_GetToolTipText;

            TabStop = false;

            if (FullSizeImageList.Images.Count > 0)
            {
                _menuPanel.Visible = false;
                _menuPanel.Size = FullSizeImageList.Images[0].Size;
                _menuPanel.Location = new Point(0, 0);
                _menuPanel.BackColor = Color.Transparent;
                _menuPanel.MouseLeave += _menuPanel_MouseLeave;
                _menuPanel.MouseMove += _menuPanel_MouseMove;
                _menuPanel.MouseDown += _menuPanel_MouseDown;
                _menuPanel.Paint += _menuPanel_Paint;
                _menuPanel.MouseClick += _menuPanel_MouseClick;
                this.Controls.Add(_menuPanel);
            }
        }

        private void DataPointChart_GetToolTipText(object sender, ToolTipEventArgs e)
        {
            // If your menu is open (or your new panel is visible), 
            // force the tooltip text to be empty. 
            // This entirely cancels the tooltip from rendering.
            if (MenuIsOpen)
            {
                e.Text = string.Empty;
            }
        }


        private void _menuPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (MenuIsOpen)
            {


                int rowIndex = 0;
                int colIndex = 0;
                int width = 0;
                int height = 0;
                float fontSize = 0.0F;
                int lineWidth = 0;
                int axisWidth = 0;
                int markerHeight = 0;
                int dpi = 0;

                MenuIsOpen = false;
                InvalidateMenu();

                GetMenuIndex(e.X, e.Y, out rowIndex, out colIndex);

                switch (rowIndex)
                {
                    case 0:
                        height = 480;
                        fontSize = 10.0F;
                        lineWidth = 2;
                        markerHeight = 7;
                        axisWidth = 1;
                        dpi = 120;
                        switch (colIndex)
                        {
                            case 1:
                                width = 480;
                                break;
                            case 2:
                                width = 640;
                                break;
                            case 3:
                                width = 854;
                                break;
                            default:
                                break;
                        }
                        break;
                    case 1:
                        height = 720;
                        fontSize = 12.0F;
                        lineWidth = 3;
                        markerHeight = 9;
                        axisWidth = 1;
                        dpi = 120;
                        switch (colIndex)
                        {
                            case 1:
                                width = 720;
                                break;
                            case 2:
                                width = 960;
                                break;
                            case 3:
                                width = 1280;
                                break;
                            default:
                                break;
                        }
                        break;
                    case 2:
                        height = 1080;
                        fontSize = 18.0F;
                        lineWidth = 5;
                        markerHeight = 11;
                        axisWidth = 3;
                        dpi = 120;
                        switch (colIndex)
                        {
                            case 1:
                                width = 1080;
                                break;
                            case 2:
                                width = 1440;
                                break;
                            case 3:
                                width = 1920;
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;

                }

                if (width > 0 && height > 0)
                {
                    //Enter your chart building code here
                    DataPointChart clone = CloneAndScaleChartForClipboard(width, height, fontSize, lineWidth, axisWidth, markerHeight);

                    try
                    {
                        if (clone != null)
                        {
                            var image = CreateImageForClipboardAndOffice(dpi, clone, this);

                            if (_chartTitle == string.Empty && (_xAxisTitle == null || _xAxisTitle == string.Empty || _yAxisTitle == null || _yAxisTitle == string.Empty))
                            {
                                ToastNotification.Show($"Copied chart to clipboard.", image, 2500);
                            }
                            else if (_chartTitle == string.Empty)
                            {
                                ToastNotification.Show($"Copied {_xAxisTitle} vs. {_yAxisTitle} chart to clipboard.", image, 2500);
                            }
                            else
                            {
                                ToastNotification.Show($"Copied {_chartTitle} chart to clipboard.", image, 2500);
                            }
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Chart Image file being used by another application.", "POD Professional v4.5 Error");
                    }

                    return;
                }
            }
        }

        private void _menuPanel_MouseMove(object sender, MouseEventArgs e)
        {
            // We are already inside the panel, so its ONLY job is to highlight the buttons
            if (InMenuBounds(e.X, e.Y))
            {
                HighlightMenuOption(e.X, e.Y);
                InvalidateMenu();
            }
        }

        private void _menuPanel_Paint(object sender, PaintEventArgs e)
        {
            if (MenuIsOpen)
            {
                if (FullSizeImageList.Images.Count > 0)
                    e.Graphics.DrawImage(ColorManager.TransformImage(FullSizeImageList.Images[0]), 0, 0);

                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        e.Graphics.DrawImage(MenuBitmaps[j, i], i * ImageSize + MenuMinX, j * ImageSize + MenuMinY);
                    }
                }

                e.Graphics.DrawImage(_wideBitmap, 0, 4 * ImageSize);
                e.Graphics.DrawImage(_aspectBitmap, 0, 100);
                e.Graphics.DrawImage(_sizeBitmap, 0, 120);
                if (WideImageList.Images.Count > 0)
                    e.Graphics.DrawImage(WideImageList.Images[1], 0, 0);

            }
        }

        private void _menuPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (MenuIsOpen)
            {
                int rowIndex = 0;
                int colIndex = 0;

                GetMenuIndex(e.X, e.Y, out rowIndex, out colIndex);

                TurnOffAllMenuButtons();
                PressMenuButton(rowIndex, colIndex);
                InvalidateMenu();


            }

            base.OnMouseDown(e);
        }

        private void _menuPanel_MouseLeave(object sender, EventArgs e)
        {
            // Close the menu when the mouse completely leaves the panel
            MenuIsOpen = false;
            TurnOffAllMenuButtons(); // Resets the button images so they aren't stuck on "highlighted" next time
        }


        private void DataPointChart_HasFailedDrawing(DataPointChart chart, string myError)
        {
            foreach (var series in this.Series)
            {
                series.Points.Clear();
            }

            foreach (var area in this.ChartAreas)
            {
                area.AxisX.Minimum = double.NaN;
                area.AxisX.Maximum = double.NaN;
                area.AxisY.Minimum = double.NaN;
                area.AxisY.Maximum = double.NaN;
            }

            this.Width += 1;
            this.Height += 1;
            this.Invalidate();
            this.Update();
            this.Width -= 1;
            this.Height -= 1;
        }

        int ImageSize = 20;
        int MenuMaxY = 80;
        int MenuMaxX = 80;
        int MenuMinY = 20;
        int MenuMinX = 0;

        bool _menuIsOpen = false;

        public bool MenuIsOpen
        {
            get
            {
                return _menuIsOpen;
            }
            set
            {
                if (ChartToolTip != null)
                {
                    if (value)
                    {
                        // Menu is opening: turn custom tooltip off
                        ChartToolTip.Active = false;
                    }
                    else
                    {
                        // Menu is closing: turn custom tooltip back on
                        ChartToolTip.Active = true;
                    }
                }

                _menuPanel.Visible = value;
                _menuIsOpen = value;
            }
        }

        private void SetUpImageLists()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataPointChart));
            ContextMenuImageList = new System.Windows.Forms.ImageList(this.components);
            ButtonImageList = new System.Windows.Forms.ImageList(this.components);
            MenuOffImageList = new System.Windows.Forms.ImageList(this.components);
            MenuOnImageList = new System.Windows.Forms.ImageList(this.components);
            MenuPressedImageList = new System.Windows.Forms.ImageList(this.components);
            WideImageList = new System.Windows.Forms.ImageList(this.components);
            FullSizeImageList = new System.Windows.Forms.ImageList(this.components);
            ContextMenuImageList = new System.Windows.Forms.ImageList(this.components);
            ButtonImageList = new System.Windows.Forms.ImageList(this.components);
            MenuOffImageList = new System.Windows.Forms.ImageList(this.components);
            MenuOnImageList = new System.Windows.Forms.ImageList(this.components);
            MenuPressedImageList = new System.Windows.Forms.ImageList(this.components);
            WideImageList = new System.Windows.Forms.ImageList(this.components);
            FullSizeImageList = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // ContextMenuImageList
            // 
            ContextMenuImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ContextMenuImageList.ImageStream")));
            ContextMenuImageList.TransparentColor = System.Drawing.Color.Transparent;
            ContextMenuImageList.Images.SetKeyName(0, "Paste.png");
            ContextMenuImageList.Images.SetKeyName(1, "Clipboard Selected.png");
            // 
            // ButtonImageList
            // 
            ButtonImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ButtonImageList.ImageStream")));
            ButtonImageList.TransparentColor = System.Drawing.Color.Transparent;
            ButtonImageList.Images.SetKeyName(0, "Paste.png");
            ButtonImageList.Images.SetKeyName(1, "Clipboard Selected.png");
            // 
            // MenuOffImageList
            // 
            MenuOffImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("MenuOffImageList.ImageStream")));
            MenuOffImageList.TransparentColor = System.Drawing.Color.Transparent;
            MenuOffImageList.Images.SetKeyName(0, "Size Top.png");
            MenuOffImageList.Images.SetKeyName(1, "Size Middle.png");
            MenuOffImageList.Images.SetKeyName(2, "Size Bottom.png");
            MenuOffImageList.Images.SetKeyName(3, "Small 11 OFF.png");
            MenuOffImageList.Images.SetKeyName(4, "Medium 11 OFF.png");
            MenuOffImageList.Images.SetKeyName(5, "Large 11 OFF.png");
            MenuOffImageList.Images.SetKeyName(6, "Small 4 3 OFF.png");
            MenuOffImageList.Images.SetKeyName(7, "Medium 4 3 OFF.png");
            MenuOffImageList.Images.SetKeyName(8, "Large 4 3 OFF.png");
            MenuOffImageList.Images.SetKeyName(9, "Small 16 9 OFF.png");
            MenuOffImageList.Images.SetKeyName(10, "Medium 16 9 OFF.png");
            MenuOffImageList.Images.SetKeyName(11, "Large 16 9 OFF.png");
            // 
            // MenuOnImageList
            // 
            MenuOnImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("MenuOnImageList.ImageStream")));
            MenuOnImageList.TransparentColor = System.Drawing.Color.Transparent;
            MenuOnImageList.Images.SetKeyName(0, "Size Top.png");
            MenuOnImageList.Images.SetKeyName(1, "Size Middle.png");
            MenuOnImageList.Images.SetKeyName(2, "Size Bottom.png");
            MenuOnImageList.Images.SetKeyName(3, "Small 11 ON.png");
            MenuOnImageList.Images.SetKeyName(4, "Medium 11 ON.png");
            MenuOnImageList.Images.SetKeyName(5, "Large 11 ON.png");
            MenuOnImageList.Images.SetKeyName(6, "Small 4 3 ON.png");
            MenuOnImageList.Images.SetKeyName(7, "Medium 4 3 ON.png");
            MenuOnImageList.Images.SetKeyName(8, "Large 4 3 ON.png");
            MenuOnImageList.Images.SetKeyName(9, "Small 16 9 ON.png");
            MenuOnImageList.Images.SetKeyName(10, "Medium 16 9 ON.png");
            MenuOnImageList.Images.SetKeyName(11, "Large 16 9 ON.png");
            // 
            // MenuPressedImageList
            // 
            MenuPressedImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("MenuPressedImageList.ImageStream")));
            MenuPressedImageList.TransparentColor = System.Drawing.Color.Transparent;
            MenuPressedImageList.Images.SetKeyName(0, "Size Top.png");
            MenuPressedImageList.Images.SetKeyName(1, "Size Middle.png");
            MenuPressedImageList.Images.SetKeyName(2, "Size Bottom.png");
            MenuPressedImageList.Images.SetKeyName(3, "Small 11 PRESS.png");
            MenuPressedImageList.Images.SetKeyName(4, "Medium 11 PRESS.png");
            MenuPressedImageList.Images.SetKeyName(5, "Large 11 PRESS.png");
            MenuPressedImageList.Images.SetKeyName(6, "Small 4 3 PRESS.png");
            MenuPressedImageList.Images.SetKeyName(7, "Medium 4 3 PRESS.png");
            MenuPressedImageList.Images.SetKeyName(8, "Large 4 3 PRESS.png");
            MenuPressedImageList.Images.SetKeyName(9, "Small 16 9 PRESSED.png");
            MenuPressedImageList.Images.SetKeyName(10, "Medium 16 9 PRESS.png");
            MenuPressedImageList.Images.SetKeyName(11, "Large 16 9 PRESS.png");
            // 
            // WideImageList
            // 
            WideImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("WideImageList.ImageStream")));
            WideImageList.TransparentColor = System.Drawing.Color.Transparent;
            WideImageList.Images.SetKeyName(0, "CopyChart.png");
            WideImageList.Images.SetKeyName(1, "RatioLabelChart.png");
            WideImageList.Images.SetKeyName(2, "11 Label.png");
            WideImageList.Images.SetKeyName(3, "43 Label.png");
            WideImageList.Images.SetKeyName(4, "169 Label.png");
            WideImageList.Images.SetKeyName(5, "0 Label.png");
            WideImageList.Images.SetKeyName(6, "480.png");
            WideImageList.Images.SetKeyName(7, "720.png");
            WideImageList.Images.SetKeyName(8, "1080.png");
            // 
            // FullSizeImageList
            // 
            FullSizeImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("FullSizeImageList.ImageStream")));
            FullSizeImageList.TransparentColor = System.Drawing.Color.Transparent;
            FullSizeImageList.Images.SetKeyName(0, "Shadow.png");
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            // The chart's ONLY job is to open the menu if you hover the top-left corner
            if (!MenuIsOpen && e != null && e.X < ImageSize && e.Y < ImageSize)
            {
                MenuIsOpen = true;
            }

            // Only process normal chart mouse events (like tooltips) if the menu is CLOSED
            if (!MenuIsOpen && this.Visible && this.ChartAreas.Count > 0)
            {
                try
                {
                    base.OnMouseMove(e);
                }
                catch (OverflowException) { }
                catch (NullReferenceException) { }
            }
        }

        private void HighlightMenuOption(int x, int y)
        {
            int rowIndex = 0;
            int colIndex = 0;

            GetMenuIndex(x, y, out rowIndex, out colIndex);

            TurnOffAllMenuButtons();
            TurnOnMenuButton(rowIndex, colIndex);
        }

        private void TurnOnMenuButton(int rowIndex, int colIndex)
        {
            if(MenuOnImageList.Images == null || MenuOnImageList.Images.Count == 0)
            {
                return;
            }

            MenuBitmaps[rowIndex, colIndex] = new Bitmap(ColorManager.TransformImage(MenuOnImageList.Images[colIndex * 3 + rowIndex]));

            switch(colIndex)
            {
                case 1:
                    _aspectBitmap = new Bitmap(ColorManager.TransformImage(WideImageList.Images[2]));
                    break;
                case 2:
                    _aspectBitmap = new Bitmap(ColorManager.TransformImage(WideImageList.Images[3]));
                    break;
                case 3:
                    _aspectBitmap = new Bitmap(ColorManager.TransformImage(WideImageList.Images[4]));
                    break;
                default:
                    _aspectBitmap = new Bitmap(ColorManager.TransformImage(WideImageList.Images[5]));
                    break;

            }

            switch (rowIndex)
            {
                case 0:
                    _sizeBitmap = new Bitmap(ColorManager.TransformImage(WideImageList.Images[6]));
                    break;
                case 1:
                    _sizeBitmap = new Bitmap(ColorManager.TransformImage(WideImageList.Images[7]));
                    break;
                case 2:
                    _sizeBitmap = new Bitmap(ColorManager.TransformImage(WideImageList.Images[8]));
                    break;
                default:
                    _sizeBitmap = new Bitmap(ColorManager.TransformImage(WideImageList.Images[5]));
                    break;

            }
            
        }

        private void PressMenuButton(int rowIndex, int colIndex)
        {
            MenuBitmaps[rowIndex, colIndex] = new Bitmap(ColorManager.TransformImage(MenuPressedImageList.Images[colIndex * 3 + rowIndex]));
        }

        private void TurnOffAllMenuButtons()
        {
            if(MenuOffImageList.Images.Count < 1)
            {
                return;
            }
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    MenuBitmaps[j, i] = new Bitmap(ColorManager.TransformImage(MenuOffImageList.Images[i * 3 + j])); //0,0 = 0; 1,0 = 1; 2,0 = 2, 0,1 = 3
                }
            }
        }

        private void GetMenuIndex(int x, int y, out int row, out int col)
        {
            row = (y - ImageSize) / ImageSize;
            col = x / ImageSize;

            if (row < 0)
                row = 0;
            if (col < 1)
                col = 1;

            if (row >= 3)
                row = 2;
            if (col >= 3)
                col = 3;
        }

        private bool InMenuBounds(int x, int y)
        {
            var inMenu = x >= 0 && x <= (MenuMaxX + ImageSize) && y >= 0 && y <= (MenuMaxY + ImageSize * 3);
            var inButton = (x >= 0 && x <= ImageSize && y >= 0 && y <= ImageSize) || (x >= 0 &&  x <= ImageSize * 4 && y >= ImageSize / 2 && y <= ImageSize);

            return inMenu || inButton;
        }

        private void InvalidateMenu()
        {
            if (FullSizeImageList.Images.Count > 0)
            {
                _menuPanel.Invalidate(new Rectangle(0, 0, FullSizeImageList.Images[0].Width, FullSizeImageList.Images[0].Height));
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
        }

        

        protected override void OnMouseUp(MouseEventArgs e)
        {

            base.OnMouseUp(e);
        }

        

        private void AddStripsBack(Axis axis, List<StripLine> strips)
        {
            foreach (var strip in strips)
            {
                axis.StripLines.Add(strip);
            }
        }

        private DataPointChart CloneAndScaleChartForClipboard(int width, int height, float fontSize, int lineWidth, int axisWidth, int markerHeight)
        {
            // 1. BACKUP: Move all current annotations to a temporary list
            var tempAnnotations = this.Annotations.ToList();
            var tempYStrips = this.YAxis.StripLines.ToList();
            var tempXStrips = this.XAxis.StripLines.ToList();

            // 2. DETACH: Clear the original chart's annotations so the Serializer is "blind" to them
            this.Annotations.Clear();
            this.YAxis.StripLines.Clear();
            this.XAxis.StripLines.Clear();

            DataPointChart clone = new DataPointChart();

            try
            {
                using (System.IO.MemoryStream myStream = new System.IO.MemoryStream())
                {
                    this.Serializer.Content = SerializationContents.All;
                    clone.Serializer.Content = SerializationContents.All;
                    clone.Serializer.IsResetWhenLoading = true;

                    // 3. SERIALIZE: Save/Load the "Skeleton" (Axes, Series, etc.)
                    // This will NOT crash because the problematic annotations are gone.
                    this.Serializer.Save(myStream);
                    myStream.Position = 0;
                    clone.Serializer.Load(myStream);
                }
            }
            finally
            {
                // 4. RESTORE SOURCE: Put the original annotations back where they belong
                // We do this in 'finally' so your UI is safe even if an error occurs.
                foreach (var anno in tempAnnotations)
                {
                    this.Annotations.Add(anno);
                }

                AddStripsBack(this.YAxis, tempYStrips);
                AddStripsBack(this.XAxis, tempXStrips);
            }

            // --- Start Building the Clone ---
            clone.Height = height;
            clone.Width = width;

            // make sure clone follows same axis flags as original
            clone.ForceZeroXAxis = ForceZeroXAxis;
            clone.ForceZeroYAxis = ForceZeroYAxis;
            clone.ClampXAxisToZeroOnExport = ClampXAxisToZeroOnExport;
            clone.ClampYAxisToZeroOnExport = ClampYAxisToZeroOnExport;

            // 5. COPY TO CLONE: Manually create copies for the new chart
            // 5. COPY TO CLONE: Manually create copies for the new chart
            foreach (var source in tempAnnotations)
            {
                // Skip Text and Polygon annotations entirely for a cleaner presentation export
                if (source is TextAnnotation || source is PolygonAnnotation)
                    continue;

                // Use Activator to create the exact same type (e.g., VerticalLineAnnotation)
                Annotation copy = (Annotation)Activator.CreateInstance(source.GetType());

                // Copy critical layout properties
                copy.Name = source.Name;
                copy.X = source.X;
                copy.Y = source.Y;
                copy.Width = source.Width;
                copy.Height = source.Height;

                // Point to the CLONE'S axes so the dashed lines scale correctly
                if (source.AxisX != null) copy.AxisX = clone.ChartAreas[0].AxisX;
                if (source.AxisY != null) copy.AxisY = clone.ChartAreas[0].AxisY;

                copy.ClipToChartArea = source.ClipToChartArea;
                copy.Alignment = source.Alignment;

                // Visual properties (Colors, line thickness, dashes)
                copy.ForeColor = source.ForeColor;
                copy.BackColor = source.BackColor;
                copy.LineColor = source.LineColor;
                copy.LineDashStyle = source.LineDashStyle;
                copy.LineWidth = source.LineWidth;

                // Specific handling to make the spanning lines visible
                if (source is HorizontalLineAnnotation sourceHLine && copy is HorizontalLineAnnotation copyHLine)
                {
                    copyHLine.IsInfinitive = sourceHLine.IsInfinitive;
                }
                else if (source is VerticalLineAnnotation sourceVLine && copy is VerticalLineAnnotation copyVLine)
                {
                    copyVLine.IsInfinitive = sourceVLine.IsInfinitive;
                }
                else if (source is LineAnnotation sourceLine && copy is LineAnnotation copyLine)
                {
                    copyLine.IsSizeAlwaysRelative = sourceLine.IsSizeAlwaysRelative;
                    copyLine.StartCap = sourceLine.StartCap;
                    copyLine.EndCap = sourceLine.EndCap;
                }

                clone.Annotations.Add(copy);
            }

            foreach (var source in tempXStrips)
            {
                NamedStripLine copy = DuplicateNamedStripLine((NamedStripLine)source);

                clone.XAxis.StripLines.Add(copy);
            }

            foreach (var source in tempYStrips)
            {
                NamedStripLine copy = DuplicateNamedStripLine((NamedStripLine)source);

                clone.YAxis.StripLines.Add(copy);
            }

            // --- Standard Scaling Logic for Series, Axes, and Titles ---
            foreach (Series series in clone.Series)
            {
                series.Font = new Font(series.Font.FontFamily, fontSize);
                series.BorderWidth = lineWidth;
                series.MarkerSize = markerHeight;
                foreach (DataPoint point in series.Points)
                {
                    point.MarkerSize = markerHeight;
                    point.MarkerBorderWidth = 0;
                }
            }

            foreach (Legend legend in clone.Legends)
            {
                legend.Font = new Font(legend.Font.FontFamily, fontSize);
            }

            if (clone.ChartAreas.Count > 0)
            {
                foreach (Axis axis in clone.ChartAreas[0].Axes)
                {
                    axis.TitleFont = new Font(axis.TitleFont.FontFamily, fontSize);
                    axis.LabelStyle.Font = new Font(axis.LabelStyle.Font.FontFamily, fontSize);
                    axis.LineWidth = axisWidth;
                    axis.MajorTickMark.LineWidth = axisWidth;
                }
            }

            foreach (Title title in clone.Titles)
            {
                title.Font = new Font(title.Font.FontFamily, fontSize);
            }

            return clone;
        }

        private NamedStripLine DuplicateNamedStripLine(NamedStripLine source)
        {
            // Use Activator to create the exact same type (e.g., NamedStripLine)
            NamedStripLine copy = (NamedStripLine)Activator.CreateInstance(source.GetType());

            // Copy your custom subclass name identifier
            copy.Name = source.Name;

            // 1. Copy critical placement properties (StripLines use Offset and Width instead of X/Y/Height)
            copy.IntervalOffset = source.IntervalOffset;
            copy.StripWidth = source.StripWidth;
            copy.Interval = source.Interval;
            copy.IntervalType = source.IntervalType;
            copy.IntervalOffsetType = source.IntervalOffsetType;

            // 2. Copy Text and Label layout properties
            copy.Text = source.Text;
            copy.TextAlignment = source.TextAlignment;
            copy.TextLineAlignment = source.TextLineAlignment;
            copy.Font = source.Font;
            copy.ForeColor = source.ForeColor;

            // 3. Copy Background visual properties (Fills and Gradients)
            copy.BackColor = source.BackColor;
            copy.BackGradientStyle = source.BackGradientStyle;
            copy.BackSecondaryColor = source.BackSecondaryColor;
            copy.BackHatchStyle = source.BackHatchStyle;

            // 4. Copy Edge/Line properties (MS Chart StripLines use Border properties instead of Line properties)
            copy.BorderColor = source.BorderColor;
            copy.BorderDashStyle = source.BorderDashStyle;
            copy.BorderWidth = source.BorderWidth;

            // 5. Copy administrative helpers
            copy.ToolTip = source.ToolTip;

            return copy;
        }

        public virtual void PrepareCloneSeries(DataPointChart clone, double finalMinY)
        {
            // The base class does nothing by default.
            // Derived charts will override this to clean up their specific series.
        }

        private static Image CreateImageForClipboardAndOffice(int dpi, DataPointChart clone, DataPointChart parent)
        {
            Bitmap chartBitmap = new Bitmap(clone.DisplayRectangle.Width, clone.DisplayRectangle.Height);
            clone.DoPostPaint = false;
            
            if (clone.ClampXAxisToZeroOnExport && clone.ChartAreas[0].AxisX.Minimum < 0.0)
            {
                clone.ChartAreas[0].AxisX.Minimum = 0.0;
            }

            if (clone.ClampYAxisToZeroOnExport && clone.ChartAreas[0].AxisY.Minimum < 0.0)
            {
                clone.ChartAreas[0].AxisY.Minimum = 0.0;
            }

            // 1. Force the layout engine to calculate the final visible bounds (e.g., 33)
            clone.ChartAreas[0].RecalculateAxesScale();
            double finalMinY = clone.ChartAreas[0].AxisY.Minimum;

            parent.PrepareCloneSeries(clone, finalMinY);

            clone.Invalidate();
            clone.DrawToBitmap(chartBitmap, clone.DisplayRectangle);
            chartBitmap.SetResolution(dpi, dpi);

            var fileName = Globals.RandomImageFileName;

            chartBitmap.Save(fileName, ImageFormat.Png);

            Image resolution = Image.FromFile(fileName);

            var dataObject = new DataObject();

            var list = new StringCollection();
            list.Add(fileName);

            dataObject.SetImage(resolution);
            dataObject.SetFileDropList(list);

            Clipboard.SetDataObject(dataObject);

            return resolution;
        }

        public void FixUpLegend(bool showLegend)
        {
            if (showLegend)
            {
                Legend legend;

                if (Legends.Count > 0)
                    legend = Legends[0];
                else
                {
                    Legends.Add(_legend);
                    legend = _legend;
                }
                
                legend.CustomItems.Clear();

                foreach (Series series in Series)
                {
                    if (series.Name != "Selected")
                    {
                        var item = new LegendItem();
                        item.ImageStyle = LegendImageStyle.Marker;
                        item.MarkerStyle = MarkerStyle.Circle;
                        item.MarkerSize = 8;
                        item.MarkerBorderWidth = 0;
                        item.MarkerColor = GetColor(series);

                        if (item.MarkerColor == Color.Transparent && series.Points.Count > 0)
                            item.MarkerColor = series.Points[0].Color;

                        item.Cells.Add(LegendCellType.SeriesSymbol, "", ContentAlignment.MiddleCenter);
                        item.Cells.Add(LegendCellType.Text, series.Name, ContentAlignment.MiddleCenter);
                        legend.CustomItems.Add(item);
                    }
                }

                ShowLegend();

                Refresh();
            }
            else
                HideLegend();
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (!MenuIsOpen)
                base.OnMouseDoubleClick(e);
        }

        private void DrawBitmapButton(object sender, ChartPaintEventArgs e)
        {
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                base.OnPaint(e);
            }
            catch(Exception exp)
            {
                //MessageBox.Show(exp.Message, "Error While Drawing Chart");
                e.Graphics.FillRectangle(Brushes.White, e.ClipRectangle);
                e.Graphics.DrawString(exp.Message, DefaultFont, Brushes.DarkSlateGray, new PointF(10.0F, DefaultFont.Height + 5));


                RaiseHasFailedEvent(exp);
            }
        }

        private void RaiseHasFailedEvent(Exception exp)
        {
            if(HasFailedDrawing != null)
            {
                HasFailedDrawing.Invoke(this, exp.Message);
            }
        }

        void DataPointChart_Customize(object sender, EventArgs e)
        {
            BuildColorMap();
        }

        public string ChartTitle
        {
            get
            {
                if (!DesignMode && Titles.Count > 0)
                {
                    UpdateChartTitle();

                    return Titles[0].Text;
                }
                return "";
            }
            set
            {
                if (!DesignMode)
                {
                    _chartTitle = value;

                    UpdateChartTitle();
                }
            }
        }

        public int DataPointCount => Series.Where(x => x.ChartType == SeriesChartType.Point).Count();

        protected void UpdateChartTitle()
        {
            if (!DesignMode)
            {
                Titles.Clear();

                if (ShowChartTitle && ChartAreas.Count > 0)
                {
                    if (Titles.Count == 0)
                    {
                        var title = Titles.Add("");

                        title.Font = new Font(title.Font.FontFamily, 10.0F, FontStyle.Regular);
                    }

                    if (_chartTitle == "")
                    {
                        if (DataPointCount > SingleSeriesCount)
                            Titles[0].Text = $"Combined Responses vs{XAxisTitle}";
                        else
                            Titles[0].Text = $"{YAxisTitle} vs {XAxisTitle}";
                    }
                    else
                    {
                        Titles[0].Text = _chartTitle;
                    }
                }
            }
        }

        private void UpdateYAxisTitle()
        {
            if (!DesignMode && ChartAreas.Count > 0)
            {
                if (YAxisNameIsUnitlessConstant)
                {
                    YAxis.Title = _yAxisTitle;
                }
                else
                {
                    if (DataPointCount > SingleSeriesCount && AutoNameYAxis)
                        YAxis.Title = "Combined Responses";
                    else
                        YAxis.Title = YAxisTitle;
                }
            }
        }

        private void UpdateXAxisTitle()
        {
            if (!DesignMode && ChartAreas.Count > 0)
            {
                if (XAxisNameIsUnitlessConstant)
                {
                    XAxis.Title = _xAxisTitle;
                }
                else
                {
                    XAxis.Title = XAxisTitle;
                }
            }
        }

        
        public string XAxisTitle
        {
            get
            {
                if (!DesignMode)
                {
                    if (_xAxisTitle == "")
                        return $"Flaw{XAxisUnitForTitle}";
                    else
                        return $"{_xAxisTitle}{XAxisUnitForTitle}";
                }
                else
                {
                    return "";
                }
            }
            set
            {
                if (!DesignMode)
                {
                    _xAxisTitle = value;

                    UpdateChartTitle();
                    UpdateXAxisTitle();
                }
            }
        }
        public string XAxisUnit
        {
            get
            {
                if (!DesignMode)
                {
                    return _xAxisUnit;
                }

                return "";
            }

            set
            {
                if (!DesignMode)
                {
                    _xAxisUnit = value;


                    UpdateChartTitle();
                    UpdateXAxisTitle();
                }
            }
        }

        public string XAxisUnitForTitle
        {
            get
            {
                if (!DesignMode)
                {
                    return _xAxisUnit.Length > 0 ? $" ({_xAxisUnit})" : "";
                }

                return "";
            }
        }

        

        public string YAxisTitle
        {
            get
            {
                if (!DesignMode)
                {
                    if (_yAxisTitle == "")
                        return $"Response{YAxisUnitForTitle}";
                    else
                        return $"{_yAxisTitle}{YAxisUnitForTitle}";
                }

                return "";
            }
            set
            {
                if (!DesignMode)
                {
                    _yAxisTitle = value;

                    UpdateChartTitle();
                    UpdateYAxisTitle();
                }
            }
        }

        public string YAxisUnit
        {
            get
            {
                if (!DesignMode)
                {
                    return _yAxisUnit;
                }

                return "";
            }
            set
            {
                if (!DesignMode)
                {
                    _yAxisUnit = value;

                    UpdateChartTitle();
                    UpdateYAxisTitle();
                }
            }
        }

        public string YAxisUnitForTitle
        {
            get
            {
                if (!DesignMode)
                {
                    return _yAxisUnit.Length > 0 ? $" ({_yAxisUnit})" : "";
                }

                return "";
            }

        }

        public virtual void ShowLegend()
        {
            if(_legend != null)
            {
                _legend.Enabled = true;
                if (!DesignMode && !Legends.Contains(_legend))
                    Legends.Add(_legend);
            }              
        }

        public virtual void HideLegend()
        {
            if (_legend != null)
            {
                _legend.Enabled = false;
                if (!DesignMode && Legends.Contains(_legend))
                    Legends.Remove(_legend);
            }
        }


        protected void MakeFlatLine(DataPointCollection myPoints)
        {
            var middle = (YAxis.Maximum - YAxis.Minimum) / 2.0 + YAxis.Minimum;

            foreach (DataPoint point in myPoints)
            {
                point.YValues[0] = middle;
            }
        }

        protected void ClearIntervals(Axis axis)
        {
            axis.LabelStyle.Interval = Double.NaN;
            axis.LabelStyle.IntervalOffset = Double.NaN;
            axis.MajorTickMark.Interval = Double.NaN;
            axis.MajorTickMark.IntervalOffset = Double.NaN;
            axis.MinorTickMark.Interval = Double.NaN;
            axis.MinorTickMark.IntervalOffset = Double.NaN;
            axis.MajorGrid.Interval = Double.NaN;
            axis.MajorGrid.IntervalOffset = Double.NaN;
            axis.MinorGrid.Interval = Double.NaN;
            axis.MinorGrid.IntervalOffset = Double.NaN;
        }

        #region OLD RELABEL CODE
        //protected virtual void RelabelAxes(AxisObject xAxis, AxisObject yAxis, InvertAxisFunction invertX, InvertAxisFunction invertY, 
        //                                   int xLabelCount, int yLabelCount, bool myCenterXAtZero = false, bool myCenterYAtZero = false, 
        //                                   TransformTypeEnum xAxisTransform = TransformTypeEnum.Linear, 
        //                                   TransformTypeEnum yAxisTransform = TransformTypeEnum.Linear)
        //{
        //    double xOffset = 0.0;
        //    double yOffset = 0.0;


        //    ChartAreas[0].AxisX.LabelAutoFitStyle = LabelAutoFitStyles.None;
        //    ChartAreas[0].AxisY.LabelAutoFitStyle = LabelAutoFitStyles.None;

        //    ClearIntervals(ChartAreas[0].AxisX);
        //    ClearIntervals(ChartAreas[0].AxisY);

        //    ChartAreas[0].AxisX.CustomLabels.Clear();
        //    ChartAreas[0].AxisY.CustomLabels.Clear();

        //    ChartAreas[0].AxisX.Maximum = xAxis.Max;
        //    ChartAreas[0].AxisX.Minimum = xAxis.Min;
        //    ChartAreas[0].AxisY.Maximum = yAxis.Max;
        //    ChartAreas[0].AxisY.Minimum = yAxis.Min;

        //    ChartAreas[0].AxisX.Interval = xAxis.Interval;
        //    ChartAreas[0].AxisX.IntervalOffset = xAxis.IntervalOffset;

        //    ChartAreas[0].AxisY.Interval = yAxis.Interval;
        //    ChartAreas[0].AxisY.IntervalOffset = yAxis.IntervalOffset;

        //    if(myCenterXAtZero == true)
        //    {
        //        xOffset = -(xAxis.Min % xAxis.Interval);
        //        ChartAreas[0].AxisX.IntervalOffset += xOffset;
        //    }

        //    if (myCenterYAtZero == true)
        //    {
        //        yOffset = -(yAxis.Min % yAxis.Interval);
        //        ChartAreas[0].AxisY.IntervalOffset += yOffset;
        //    }

        //    if (yLabelCount > 1)
        //        yAxis.Interval = (yAxis.Max - yAxis.Min) / (yLabelCount-1);

        //    if(xLabelCount > 1)
        //        xAxis.Interval = (xAxis.Max - xAxis.Min) / (xLabelCount-1);

        //    Random rand = new Random();

        //    for (int i = 0; i < yLabelCount; i++)
        //    {

        //        var modify = 0.0;// (rand.Next(50, 100) / 100.0) * yAxis.Interval;

        //        double intv = (yAxis.Interval * i + yAxis.Min + yAxis.IntervalOffset + yOffset + modify);
        //        string intvString = intv.ToString("0.##");

        //        if (invertY != null)
        //            intvString = invertY(intv).ToString("0.##");


        //        CustomLabel label = new CustomLabel
        //        {
        //            FromPosition = yAxis.Interval * i - 1 + yAxis.Min + yAxis.IntervalOffset + yOffset + modify,
        //            ToPosition = yAxis.Interval * i + 1 + yAxis.Min + yAxis.IntervalOffset + yOffset + modify,
        //            Text = intvString,
        //            RowIndex = 0,
        //            GridTicks = GridTickTypes.All
        //        };

        //        ChartAreas[0].AxisY.CustomLabels.Add(label);
        //    }

        //    ChartAreas[0].AxisY.LabelAutoFitMinFontSize = 5;

        //    for (int i = 0; i < xLabelCount; i++)
        //    {
        //        var modify = 0.0;// (rand.Next(50, 100) / 100.0) * xAxis.Interval;

        //        double intv = (xAxis.Interval * i + xAxis.Min + xAxis.IntervalOffset + xOffset + modify);
        //        string intvString = intv.ToString("0.##"); 

        //        if(invertX != null)
        //            intvString = invertX(intv).ToString("0.##");

        //        CustomLabel label = new CustomLabel
        //        {
        //            FromPosition = xAxis.Interval * i - 1 + xAxis.Min + xAxis.IntervalOffset + xOffset + modify,
        //            ToPosition = xAxis.Interval * i + 1 + xAxis.Min + xAxis.IntervalOffset + xOffset + modify,
        //            Text = intvString,
        //            RowIndex = 0,
        //            GridTicks = GridTickTypes.All
        //        };

        //        ChartAreas[0].AxisX.CustomLabels.Add(label);
        //    }

        //    ChartAreas[0].AxisX.LabelAutoFitMinFontSize = 5;
        //}
        #endregion

        protected virtual void GetBufferedRangeWrapper(Control chart, IAxisObject myAxis, double myMin, double myMax, AxisKind kind)
        {
            AnalysisData.GetBufferedRange(chart, myAxis, myMin, myMax, kind);
        }

        private void RefreshAxisLabelingBasedOnCurrentSize()
        {
            if(_xRelabel != null && _yRelabel != null)
            {
                if (_xRelabel.Axis != null)
                {
                    _xRelabel.Axis.BufferPercentage = 0;
                    AnalysisData.GetBufferedRange(this, _xRelabel.Axis, _xRelabel.Axis.Min, _xRelabel.Axis.Max, AxisKind.X);
                    //_xRelabel.LabelCount = Globals.GetLabelIntervalBasedOnChartSize(this, AxisKind);//Convert.ToInt32(Math.Abs(_xRelabel.Axis.Max - _xRelabel.Axis.Min) / _xRelabel.Axis.Interval + 1);
                }

                if (_yRelabel.Axis != null)
                {
                    _yRelabel.Axis.BufferPercentage = 0;
                    AnalysisData.GetBufferedRange(this, _yRelabel.Axis, _yRelabel.Axis.Min, _yRelabel.Axis.Max, AxisKind.Y);
                    //_yRelabel.LabelCount = Globals.GetLabelIntervalBasedOnChartSize(this, AxisKind);//Convert.ToInt32(Math.Abs(_yRelabel.Axis.Max - _yRelabel.Axis.Min) / _yRelabel.Axis.Interval + 1);
                }
                //the data point chart may not have analysisData because it doesn't need it.
                //check to make sure analysis data is not null and that the transform is boxcox before running relabel
                if(_analysisData != null && _yRelabel.TransformType == TransformTypeEnum.BoxCox)
                {
                    if (_analysisData.DataType == AnalysisDataTypeEnum.AHat)
                        RelabelAxesBetter(_xRelabel.Axis, _yRelabel.Axis,
                                      _xRelabel.InvertFunc, _yRelabel.InvertFunc,
                                      _xRelabel.LabelCount, _yRelabel.LabelCount,
                                      _xRelabel.CenterAtZero, _yRelabel.CenterAtZero,
                                      _xRelabel.TransformType, _yRelabel.TransformType,
                                      _xRelabel.TransformFunc, _yRelabel.TransformFunc, false, false, _analysisData.LambdaValue);
                }
                else
                    RelabelAxesBetter(_xRelabel.Axis, _yRelabel.Axis,
                                  _xRelabel.InvertFunc, _yRelabel.InvertFunc,
                                  _xRelabel.LabelCount, _yRelabel.LabelCount,
                                  _xRelabel.CenterAtZero, _yRelabel.CenterAtZero,
                                  _xRelabel.TransformType, _yRelabel.TransformType,
                                  _xRelabel.TransformFunc, _yRelabel.TransformFunc);

                /*
                if (.DataType == AnalysisDataTypeEnum.HitMiss) 
                {
                    if (ChartAreas[0].AxisX.Maximum < ChartAreas[0].AxisX.Minimum)
                    {
                        ChartAreas[0].AxisX.Maximum = 1.0 / (_analysisData.HMAnalysisObject.Flaws_All.Min() - (.02) * _analysisData.HMAnalysisObject.Flaws_All.Min());
                        ChartAreas[0].AxisX.Minimum = 0;
                    }
                }
                */

            }
        }

        protected virtual void RelabelAxesBetter(IAxisObject xAxis, IAxisObject yAxis, 
                                                 Globals.InvertAxisFunction invertX, Globals.InvertAxisFunction invertY,
                                                 int xLabelCount, int yLabelCount, 
                                                 bool myCenterXAtZero = false, bool myCenterYAtZero = false,
                                                 TransformTypeEnum xAxisTransform = TransformTypeEnum.Linear,
                                                 TransformTypeEnum yAxisTransform = TransformTypeEnum.Linear, 
                                                 Globals.InvertAxisFunction transformX = null, Globals.InvertAxisFunction transformY = null,
                                                 bool forceKeepCountX = false, bool forceKeepCountY = false, double lambda=Double.NaN)
        {
            double xOffset = 0.0;
            double yOffset = 0.0;

            if(xAxis != null && !forceKeepCountX)
                xLabelCount = Convert.ToInt32(Math.Abs(xAxis.Max - xAxis.Min) / xAxis.Interval + 1);

            if (yAxis != null && !forceKeepCountY)
                yLabelCount = Convert.ToInt32(Math.Abs(yAxis.Max - yAxis.Min) / yAxis.Interval + 1);

            StoreLabelingParameters(xAxis, yAxis, invertX, invertY, xLabelCount, yLabelCount, 
                                    myCenterXAtZero, myCenterYAtZero, xAxisTransform, yAxisTransform,
                                    transformX, transformY);

            xOffset = UpdateChartAxis(ChartAreas[0].AxisX, xAxis, myCenterXAtZero);
            yOffset = UpdateChartAxis(ChartAreas[0].AxisY, yAxis, myCenterYAtZero);
            
            Random rand = new Random();

            if (yAxis != null && yAxisTransform == TransformTypeEnum.Linear)
            {
                LabelLinearAxis(ChartAreas[0].AxisY, yAxis, invertY, yLabelCount, yOffset, false, yAxisTransform);
            }
            else if (yAxis != null && yAxisTransform == TransformTypeEnum.BoxCox)
            {
                LabelLinearAxis(ChartAreas[0].AxisY, yAxis, invertY, yLabelCount, yOffset, false, yAxisTransform, lambda);
            }
            else
            {
                if (yAxis != null && yAxisTransform == TransformTypeEnum.Log)
                {
                    LabelLog10Axis(ChartAreas[0].AxisY, yAxis, transformY);
                }

            }

            ChartAreas[0].AxisY.LabelAutoFitMinFontSize = 5;

            if (xAxis != null && xAxisTransform == TransformTypeEnum.Linear)
            {
                LabelLinearAxis(ChartAreas[0].AxisX, xAxis, invertX, xLabelCount, xOffset, false, xAxisTransform);
            }
            else if (xAxis != null && xAxisTransform == TransformTypeEnum.BoxCox)
            {
                LabelLinearAxis(ChartAreas[0].AxisX, xAxis, invertX, xLabelCount, xOffset, false, xAxisTransform, lambda);
            }
            else
            {
                if (xAxis != null && xAxisTransform == TransformTypeEnum.Log)
                {
                    LabelLog10Axis(ChartAreas[0].AxisX, xAxis, transformX);
                }
            }

            ChartAreas[0].AxisX.LabelAutoFitMinFontSize = 5;

            //used to keep inverse from breaking if the user selected it in the transform panel
            if(ChartAreas[0].AxisX.Minimum> ChartAreas[0].AxisX.Maximum)
            {
                var temp = ChartAreas[0].AxisX.Maximum;
                ChartAreas[0].AxisX.Maximum = ChartAreas[0].AxisX.Minimum;
                ChartAreas[0].AxisX.Minimum = temp;
            }

            if (ForceZeroXAxis && xAxisTransform != TransformTypeEnum.Log && ChartAreas[0].AxisX.Minimum < 0.0)
            {
                ChartAreas[0].AxisX.Minimum = 0.0;
            }

            if (ForceZeroYAxis && yAxisTransform != TransformTypeEnum.Log && ChartAreas[0].AxisY.Minimum < 0.0)
            {
                ChartAreas[0].AxisY.Minimum = 0.0;
            }
        }

        private void StoreLabelingParameters(IAxisObject xAxis, IAxisObject yAxis, Globals.InvertAxisFunction invertX, Globals.InvertAxisFunction invertY, 
                                             int xLabelCount, int yLabelCount, bool xCenterAtZero, bool yCenterAtZero, 
                                             TransformTypeEnum xAxisTransform, TransformTypeEnum yAxisTransform,
                                             Globals.InvertAxisFunction xTransform, Globals.InvertAxisFunction yTransform)
        {
            if(_xRelabel == null || _xRelabel.Axis == null || xAxis != null)
                _xRelabel = new RelabelParameters(xAxis, invertX, xLabelCount, xCenterAtZero, xAxisTransform, xTransform);

            if (_yRelabel == null || _yRelabel.Axis == null || yAxis != null)
                _yRelabel = new RelabelParameters(yAxis, invertY, yLabelCount, yCenterAtZero, yAxisTransform, yTransform);
        }

        private double UpdateChartAxis(Axis axis, IAxisObject axisObj, bool myCenterZero)
        {
            var offset = 0.0;

            if (axisObj != null)
            {
                axis.LabelAutoFitStyle = LabelAutoFitStyles.None;
                ClearIntervals(axis);
                axis.CustomLabels.Clear();
                axis.Maximum = axisObj.Max;
                axis.Minimum = axisObj.Min;
                axis.Interval = axisObj.Interval;
                axis.IntervalOffset = axisObj.IntervalOffset;

                if (myCenterZero == true)
                {
                    offset = -(axisObj.Min % axisObj.Interval);
                    ChartAreas[0].AxisX.IntervalOffset += offset;
                }
            }

            return offset;
        }

        public virtual string TooltipText
        {
            get
            {
                return "";
            }
        }
        private int GetPrecisionFromData()
        {
            int maxPrecision=10;
            if (_analysisData.RecheckAnalysisType(_analysisData.DataType) == AnalysisDataTypeEnum.AHat)
                maxPrecision = _analysisData.AHATAnalysisObject.MaxPrecision;
            else if (_analysisData.RecheckAnalysisType(_analysisData.DataType) == AnalysisDataTypeEnum.HitMiss)
                maxPrecision = _analysisData.HMAnalysisObject.MaxPrecision;

            return maxPrecision;
        }
        //private void LabelBoxCoxAxis(Axis chartaxis, AxisObject axis, Globals.InvertAxisFunction invert, int labelCount, double offest, bool redoing= false, Transform)
        private void LabelLinearAxis(Axis chartAxis, IAxisObject axis, Globals.InvertAxisFunction invert, int labelCount, double offset, bool redoing = false, TransformTypeEnum myTransform=TransformTypeEnum.Linear, double lambdaValue=double.NaN)
        {
            var lastString = "";

            // when lambda value is negative. The top label will be negative due to the nature of the backwards transform function
            // thus, the top label is simply omitted. DO NOT DELETE.
            if (lambdaValue < 0)
            {
                labelCount -= 1;
            }
            // Determine the format for "normal" numbers based on the overall scale of the axis.
            // This ensures consistent precision for all non-scientific labels.
            string numberFormat;
            if (invert != null)
            {
                // Find the largest absolute value on the axis after transformation.
                double maxDisplayValue = Math.Max(
                    Math.Abs(TransformBackValue(axis.Axis, axis.Max)),
                    Math.Abs(TransformBackValue(axis.Axis, axis.Min))
                );

                // Use G (general) format for most cases to avoid trailing zeros, and N0 for large integers.
                if (maxDisplayValue < 100)
                {
                    numberFormat = "G3"; // General, 3 significant digits is a good default
                }
                else // maxDisplayValue >= 100
                {
                    numberFormat = "N0"; // Integer with comma for large numbers
                }
            }
            else
            {
                numberFormat = "G3"; // A reasonable default for non-transformed axes
            }

            for (int i = 0; i < labelCount; i++)
            {                
                var modify = 0.0;
                
                double intv = (axis.Interval * i + axis.Min + axis.IntervalOffset + offset + modify);
                string intvString;

                // Get the actual value to be displayed on the label.
                double displayValue;
                if (invert != null && intv != 0.0)
                {
                    displayValue = TransformBackValue(axis.Axis, intv);
                }
                else
                {
                    displayValue = intv;
                }

                double absValue = Math.Abs(displayValue);

                // HYBRID FORMATTING LOGIC:
                // 1. If the number is very small, use scientific notation.
                if (absValue > 0 && absValue < 0.01)
                {
                    intvString = displayValue.ToString("G2"); // e.g., "4.5E-06"
                }
                // 2. Otherwise, use the consistent format determined earlier.
                else
                {
                    intvString = displayValue.ToString(numberFormat);
                }

                // Avoid duplicate labels
                if (lastString == intvString)
                {
                    continue;
                }

                if (intv >= axis.Min && intv <= axis.Max)
                {
                    CustomLabel label = new CustomLabel
                    {
                        // Centering the label on the tick mark
                        FromPosition = intv - (axis.Interval / 2.0),
                        ToPosition = intv + (axis.Interval / 2.0),
                        Text = intvString,
                        RowIndex = 0,
                        GridTicks = GridTickTypes.All
                    };

                    chartAxis.CustomLabels.Add(label);
                    lastString = intvString;
                }
            }

            if(chartAxis.CustomLabels.Count <= 2 && redoing == false)
            {
                axis.Interval = axis.Interval / 2.0;
                chartAxis.CustomLabels.Clear();
                LabelLinearAxis(chartAxis, axis, invert, labelCount * 2 - 1, offset, true, myTransform);
            }
        }

        private string GetFormat(int precision)
        {
            var format = "0.#";

            for (int i = 1; i < precision; i++)
            {
                format = $"{format}#";
            }

            return format;
        }

        private void LabelLog10Axis(Axis chartAxis, IAxisObject axis, Globals.InvertAxisFunction transform, bool forceAll = false)
        {
            var labeledTick = 0;

            for (int exp = -6; exp < 6; exp = exp + 1)
            {
                for (int subExp = 1; subExp < 10; subExp++)
                {
                    var value = Math.Pow(10, exp) * subExp;
                    //var intv = transform(value);
                    var intv = TransformValue(axis.Axis, value, TransformTypeEnum.Log);
                    var exponent = exp;
                    var valLabel = value.ToString();

                    if(exponent > 3)
                    {
                        valLabel = $"1E+{exponent:###}";
                    }

                    if (subExp != 1 && subExp != 2 && subExp != 5 && forceAll == false)
                        valLabel = "";

                    if (intv >= axis.Min && intv <= axis.Max)
                    {
                        CustomLabel label = new CustomLabel
                        {
                            FromPosition = intv - 1,
                            ToPosition = intv + 1,
                            Text = valLabel,
                            RowIndex = 0,
                            GridTicks = GridTickTypes.All
                        };

                        if (label.Text.Length > 0)
                            labeledTick++;

                        chartAxis.CustomLabels.Add(label);
                    }
                    else if (intv > axis.Max)
                        break;
                }
            }

            if (labeledTick < 3 && forceAll == false)
            {
                LabelLog10Axis(chartAxis, axis, transform, true);
            }
        }

        public void UpdateTransformer(IAnalysisData data)
        {
            if (_transformer == null)
            {
                _transformer = data.DuplicateTransformer();
            }
            else
            {
                data.UpdateTransformer(_transformer);
            }
        }

        private double TransformBackValue(AxisKind axis, double myValue)
        {
            if (_transformer != null)
            {
                return _transformer.InverseTransformAxisValue(axis, myValue);
            }

            throw new MissingMemberException("Transformer missing for DataPointChart. Check object initialization.");
        }

        private double TransformValue(AxisKind axis, double myValue, TransformTypeEnum forcedTransform)
        {
            if (_transformer != null)
            {
                return _transformer.TransformAxisValue(axis, myValue, forcedTransform);
            }

            throw new MissingMemberException("Transformer missing for DataPointChart. Check object initialization.");
        }

        private  double TransformValue(AxisKind axis, double myValue)
        {
            if (_transformer != null)
            {
                return _transformer.TransformAxisValue(axis, myValue);
            }

            throw new MissingMemberException("Transformer missing for DataPointChart. Check object initialization.");
        }

        public virtual void ClearEverythingButPoints()
        {

        }

        public bool CanUnselect
        {
            get { return _canUnselect; }
            set { _canUnselect = value; }
        }

        public bool Selectable
        {
            get { return _selectable; }
            set { _selectable = value; }
        }

        bool _isSquare = false;
        private bool _resetErrors = false;
        protected bool DoPostPaint = true;

        public bool IsSquare
        {
            get { return _isSquare; }
            set { _isSquare = value; }
        }

        public virtual void SetupChart(string flawName, string flawUnit, List<string> responseNames, List<string> responseUnits)
        {
            DoPostPaint = true;
        }

        public virtual void ForceResizeAnnotations()
        {

        }

        public bool IsSelected
        {
            get { return _isSelected; }

            set
            {
                if (Selectable && mouseInside == false)
                {
                    _isSelected = value;
                    if (_isSelected == false)
                        Invalidate();
                }
                else
                {
                    Invalidate();
                }

            }
        }

        

        public Axis XAxis
        {
            get
            {
                return ChartAreas[0].AxisX;
            }
        }

        public Axis YAxis
        {
            get
            {
                return ChartAreas[0].AxisY;
            }
        }

        void DataPointChart_Resize(object sender, EventArgs e)
        {
            if(IsSquare == true)
            {
                Height = Width;
            }

            if(_lastWidth != Width)
            {
                RefreshAxisLabelingBasedOnCurrentSize();
            }
            
            _lastWidth = Width;

            ApplyDynamicFontScaling();
        }

        

        public void SelectChart()
        {
            AHatVsAChart_MouseClick(this, null);
        }

        public void HighlightChart()
        {
            AHatVsAChart_MouseEnter(this, null);
        }

        public void RemoveChartHighlight()
        {
            AHatVsAChart_MouseLeave(this, null);
        }

        public bool IsHighlighted
        {
            get
            {
                return _isHighlighted;
            }
        }

        protected virtual void AHatVsAChart_MouseClick(object sender, MouseEventArgs e)
        {
            if (Selectable)
            {
                if (CanUnselect)
                    _isSelected = !_isSelected;
                else
                    _isSelected = true;

                Select();
                Invalidate();
            }
        }

        protected virtual void AHatVsAChart_MouseLeave(object sender, EventArgs e)
        {
            if (Selectable)
            {
                _isHighlighted = false;
                Invalidate();

                mouseInside = false;
            }

            // Unconditionally force the top-left corner to repaint so the _bitmap clears!
            Invalidate(new Rectangle(0, 0, 20, 20));

            InvalidateMenu();
        }

        protected virtual void AHatVsAChart_MouseEnter(object sender, EventArgs e)
        {
            if (Selectable)
            {
                _isHighlighted = true;
                Invalidate();

                mouseInside = true;
            }

            Invalidate(new Rectangle(0, 0, 20, 20));
            
        }

        public static List<Color> GetLargeColorList(bool designMode)
        {
            var list = new List<Color>();

            if (designMode == false)
            {

                var chart = new DataPointChart();


                for (int i = 0; i < 50; i++)
                {
                    var series = new Series(i.ToString());

                    series.ChartType = SeriesChartType.Point;

                    chart.Series.Add(series);

                    chart.Series[i].Points.AddXY(0.0, 0.0);
                }

                chart.ApplyPaletteColors();

                chart.BuildColorMap();

                for (int k = 0; k < chart.Series.Count; k++)
                {
                    list.Add(chart.GetColor(chart.Series[k]));
                }


            }

            return list;
        }

        public void CleanUpDataSeries()
        {
            // One of our points flaw size values is NaN. Just drop for now.
            foreach (var dataSeries in this.Series)          
            {
                var pointsToRemove = dataSeries.Points.Where(point => Double.IsNaN(point.XValue)).ToList();
                if (pointsToRemove.Any())
                {
                    foreach (var dataPoint in pointsToRemove)
                    {
                        dataSeries.Points.Remove(dataPoint);
                    }
                }
            }
        }

        public void ReloadChartData(DataRowCollection xRows, DataTable yData, DataTable names)
        {
            //if (this.IsDisposed) return;

            var nameColumnNames = new List<string>();

            foreach (DataColumn col in names.Columns)
                nameColumnNames.Add(col.ColumnName);

            //clear input data
            foreach(DataColumn col in yData.Columns)
            {
                if (Series.FindByName(col.ColumnName) != null)
                    this.Series[col.ColumnName].Points.Clear();
                else
                {
                    col.ColumnName = Series[col.Ordinal + 3].Name;
                    this.Series[col.ColumnName].Points.Clear();
                }
            }

            for (int i = 0; i < xRows.Count; i++)
            {
                for (var j = 0; j < yData.Columns.Count; j++)
                {
                    this.Series[j + 3].IsValueShownAsLabel = false;

                    var row = yData.Rows[i];

                    var xValue = Convert.ToDouble(xRows[i].ItemArray[0]);
                    var yValue = Convert.ToDouble(row.ItemArray[j]);
                    if (!Double.IsNaN(xValue) && !Double.IsNaN(yValue))
                    {
                        this.Series[j+3].Points.AddXY(xValue, yValue);
                        var lastPoint = this.Series[j + 3].Points.Last();
                        var pointName = CreateNameFromRow(names.Rows[i], nameColumnNames);
                        lastPoint.SetCustomProperty("Name", pointName);
                        lastPoint.SetCustomProperty("Row Index", i.ToString());
                        lastPoint.SetCustomProperty("Column Index", j.ToString());
                        lastPoint.ToolTip = "";
                        
                        
                    }
                    else
                    {
                        this.Series[j + 3].Points.AddXY(xValue, yValue);
                        var lastPoint = this.Series[j + 3].Points.Last();
                        lastPoint.IsEmpty = true;
                        var pointName = CreateNameFromRow(names.Rows[i], nameColumnNames);
                        lastPoint.SetCustomProperty("Name", pointName);
                        lastPoint.SetCustomProperty("Row Index", i.ToString());
                        lastPoint.SetCustomProperty("Column Index", j.ToString());
                        lastPoint.ToolTip = "";
                    }
                }
                
            }

            
        }

        private string CreateNameFromRow(DataRow dataRow, List<string> names)
        {
            // Determine the safe limit: whichever collection is smaller
            int safeCount = Math.Min(names.Count, dataRow.ItemArray.Length);

            // Only iterate up to the safeCount
            var values = names
                .Take(safeCount)
                .Select((_, i) => Convert.ToString(dataRow.ItemArray[i]));

            return string.Join("|", values);
        }

        public void SetXAxisRange(IAxisObject myAxis, IAnalysisData data, bool forceLinear = false, bool keepLabelCount = false, 
            bool transformResidView=false)
        {
            //don't create buffering if user is switching between show residuals and show fit in signal response transform window
            if (!transformResidView)
                AnalysisData.GetBufferedRange(this, myAxis, myAxis.Min, myAxis.Max, AxisKind.X);

            if (myAxis.Max < myAxis.Min)
            {
                myAxis.Max = 1.0;
                myAxis.Min = 0.0;
                myAxis.Interval = .5;
            }

            CopyAxisObjectToAxis(ChartAreas[0].AxisX, myAxis);

            if (!forceLinear)
                RelabelAxesBetter(myAxis, null, data.InvertTransformedFlaw, data.InvertTransformedResponse, Globals.GetLabelIntervalBasedOnChartSize(this, AxisKind.X), 
                    Globals.GetLabelIntervalBasedOnChartSize(this, AxisKind.Y),false, true, data.FlawTransform, data.ResponseTransform, data.TransformValueForXAxis, data.TransformValueForYAxis, keepLabelCount, false);
            else
                RelabelAxesBetter(myAxis, null, data.DoNoTransform, data.DoNoTransform, Globals.GetLabelIntervalBasedOnChartSize(this, AxisKind.X), Globals.GetLabelIntervalBasedOnChartSize(this, AxisKind.Y),
                    false, true, TransformTypeEnum.Linear, TransformTypeEnum.Linear, data.DoNoTransform, data.DoNoTransform, keepLabelCount, false);
        }

        public void SetYAxisRange(IAxisObject myAxis, IAnalysisData data, bool forceLinear = false, bool keepLabelCount=false,
            bool transformResidView = false)
        {
            //don't create buffering if user is switching between show residuals and show fit in signal response transform window
            if (!transformResidView)
                AnalysisData.GetBufferedRange(this, myAxis, myAxis.Min, myAxis.Max, AxisKind.Y);

            if (myAxis.Max < myAxis.Min)
            {
                myAxis.Max = 1.0;
                myAxis.Min = 0.0;
                myAxis.Interval = .5;
            }

            CopyAxisObjectToAxis(ChartAreas[0].AxisY, myAxis);

            if (!forceLinear)
            {
                if(data.DataType== AnalysisDataTypeEnum.HitMiss)
                    RelabelAxesBetter(null, myAxis, data.InvertTransformedFlaw, data.InvertTransformedResponse, Globals.GetLabelIntervalBasedOnChartSize(this, AxisKind.X), Globals.GetLabelIntervalBasedOnChartSize(this, AxisKind.Y),
                                  false, true, data.FlawTransform, data.ResponseTransform, data.TransformValueForXAxis, data.TransformValueForYAxis, false, keepLabelCount);
                else
                    RelabelAxesBetter(null, myAxis, data.InvertTransformedFlaw, data.InvertTransformedResponse, Globals.GetLabelIntervalBasedOnChartSize(this, AxisKind.X), Globals.GetLabelIntervalBasedOnChartSize(this, AxisKind.Y),
                                      false, true, data.FlawTransform, data.ResponseTransform, data.TransformValueForXAxis, data.TransformValueForYAxis, false, keepLabelCount, data.LambdaValue);
            }
            else
                RelabelAxesBetter(null, myAxis, data.DoNoTransform, data.DoNoTransform, Globals.GetLabelIntervalBasedOnChartSize(this, AxisKind.X), Globals.GetLabelIntervalBasedOnChartSize(this, AxisKind.Y),
                    false, true, TransformTypeEnum.Linear, TransformTypeEnum.Linear, data.DoNoTransform, data.DoNoTransform, false, keepLabelCount);
        }
        
        private void CopyAxisObjectToAxis(Axis myAxis, IAxisObject myAxisObj)
        {
            myAxis.Maximum = myAxisObj.Max;
            myAxis.Minimum = myAxisObj.Min;

            myAxis.MajorTickMark.Interval = myAxisObj.Interval;
            myAxis.MajorTickMark.IntervalOffset = myAxisObj.IntervalOffset;
            myAxis.Interval = myAxisObj.Interval;
            myAxis.IntervalOffset = myAxisObj.IntervalOffset;
        }

        private void DataPointChart_PostPaint(object sender, ChartPaintEventArgs e)
        {
            if (e.ChartElement is Chart)
            {
                UpdateErrorLocation(e);
            }
            else if(e.ChartElement is ChartArea)
            {
                // setting chart area clip to size of chart control because it can get messed up when errors occur plotting the data (e.g., pod chart's visible clip bounds on 2nd plotting error)
                e.ChartGraphics.Graphics.SetClip(new RectangleF(0.0f, 0.0f, this.Width, this.Height));

                if (DoPostPaint)
                {
                    if (IsHighlighted)
                    {
                        var chart = e.Chart;
                        var rect = chart.ClientRectangle;
                        rect.Width -= 9;
                        rect.Height -= 9;
                        rect.X += 4;
                        rect.Y += 4;
                        var pen = new Pen(HighlightColor, 9.0F);

                        e.ChartGraphics.Graphics.DrawRectangle(pen, rect);
                    }

                    if (IsSelected)
                    {
                        var chart = e.Chart;
                        var rect = chart.ClientRectangle;
                        rect.Width -= 3;
                        rect.Height -= 3;
                        rect.X += 1;
                        rect.Y += 1;
                        var pen = new Pen(SelectColor, 3.0F);

                        e.ChartGraphics.Graphics.DrawRectangle(pen, rect);
                    }

                    if (ClientRectangle.Contains(PointToClient(Control.MousePosition)))
                    {
                        e.ChartGraphics.Graphics.DrawImage(_bitmap, 0.0F, 0.0F);
                    }

                }                
            }
        }

        public void BuildColorMap()
        {
            foreach (Series series in Series)
            {
                if (_colorMap.ContainsKey(series.Name))
                    continue;

                Color color;

                if (Series[series.Name].Points.Count > 0)
                {
                    color = Series[series.Name].Points.First().Color;

                    if(color == Color.Gray || color.A == 0)
                        color = Series[series.Name].Color;
                }
                else
                    color = Series[series.Name].Color;

                _colorMap.Add(series.Name, color);

                Series[series.Name].Color = ColorManager.TransformColor(color);
            }
        }

        public Color GetColor(Series series)
        {
            if (_colorMap.ContainsKey(series.Name))
                return ColorManager.TransformColor(_colorMap[series.Name]);
            else
                return Color.Transparent;
        }

        public void FillFromSource(DataSource source, string originalFlawName, string originalResponseName, List<Color> colors)
        {
            var flawIndex = source.Originals(ColType.Flaw).IndexOf(originalFlawName);
            var responseIndex = source.Originals(ColType.Response).IndexOf(originalResponseName);

            FillFromSource(source, flawIndex, responseIndex, colors);
        }

        public void AddEmptySeriesToForceDraw()
        {
            if (Series.FindByName("Empty") == null)
            {
                Series.Add("Empty");
                Series["Empty"].Points.Add();
                Series["Empty"].Points[0].IsEmpty = true;
                Series["Empty"].IsVisibleInLegend = false;
            }
        }

        public void FillFromSource(DataSource source, int flawIndex, int responseIndex, List<Color> colors)
        {
            ChartArea area = null;

            if (ChartAreas.Count == 0)
                area = ChartAreas.Add("Main");
            else
                area = ChartAreas[0];

            var view = source.GetGraphData(flawIndex, responseIndex).AsDataView();
            var flaws = source.Flaws;
            var responses = source.Responses;

            RemoveNullRowsFromView(view);

            //Series.Add(new Series(Series.Count.ToString()));
            //Series.Last().ChartType = SeriesChartType.Point;

            DataBindTable(view, flaws[flawIndex].GetColumnName());

            var last = Series.Last();
            //var rowIndex = 0;                        

            last.Name = responses[responseIndex].GetColumnName();

            //if(SingleSeriesCount > 1)
            //{
                
            last.IsVisibleInLegend = false;
                
            //}

            last.ChartType = SeriesChartType.Point;
            last.Color = colors[responseIndex];

            foreach (DataPoint point in last.Points)
            {
                point.MarkerStyle = MarkerStyle.Circle;
                point.MarkerSize = 10;
            }

            XAxisTitle = flaws[flawIndex].GetColumnName();
            XAxisUnit = flaws[flawIndex].Unit;

            YAxisTitle = responses[responseIndex].GetColumnName();
            YAxisUnit = responses[responseIndex].Unit;

            XAxis.RoundAxisValues();
            YAxis.RoundAxisValues();
            //AnalysisData.GetBufferedRange(yAxisAxis, source.GetData, true);

            UpdateChartTitle();

            if (Series.Count > SingleSeriesCount)
                ShowLegend();
            else
                HideLegend();
        }

        private void RemoveNullRowsFromView(DataView view)
        {
            var deletes = new List<DataRow>();

            foreach (DataRow row in view.Table.Rows)
            {
                if (row.IsNull(1))
                {
                    deletes.Add(row);
                }
            }

            foreach (DataRow row in deletes)
            {
                view.Table.Rows.Remove(row);
            }
        }

        public void FinalizeErrors(ErrorArgs e, IMessageBoxWrap messageBoxInput = null)
        {
            var messageBox = messageBoxInput ?? new MessageBoxWrap();

            var finalErrorString = string.Join(Environment.NewLine, _errorsList);

            _errorsList.Clear();

            try
            {
                if (IsHandleCreated)
                {
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        if (_error != null)
                        {
                            _error.Text = finalErrorString;
                        }
                    });
                }
                
            }
            catch(Exception exp)
            {
                messageBox.Show($"FinalizeErrors: {exp.Message}", "POD Professional v4.5 Error");
            }
        }

        public void AddError(ErrorArgs e)
        {
            if(_resetErrors)
            {
                _errorsList.Clear();
            }

            _errorsList.Add(e.Error);

            //always flip back to false after first error addition
            _resetErrors = false;

            if (_error == null && _errorBox == null)
            {
                _error = new TextAnnotation();

                _error.X = 50;// (XAxis.Maximum - XAxis.Minimum) / 2.0 + XAxis.Minimum;
                _error.Y = 50;// (YAxis.Maximum - YAxis.Minimum) / 2.0 + YAxis.Minimum;
                _error.ForeColor = Color.Black;
                _error.Font = new Font("Arial", 11.0F, FontStyle.Bold);
                //_error.Text = e.Error;
                _error.Visible = true;

                _errorBox = new PolygonAnnotation();

                _errorBox.BackColor = Color.White;
                _errorBox.LineColor = Color.Gray;

                _errorBox.GraphicsPath.AddRectangle(new Rectangle(0, 0, 100, 100));
                _errorBox.Alignment = ContentAlignment.TopLeft;
                _errorBox.Visible = true;
                _errorBox.Height = 8.0;
                _errorBox.Width = 43.0;
                _errorBox.X = 50.0 - _errorBox.Width / 2.0;
                _errorBox.Y = 50.0 - _errorBox.Height / 2.0;
                _errorBox.LineWidth = 2;

                //Annotations.Insert(0, _equation);
                if (IsHandleCreated)
                {
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        Annotations.Add(_errorBox);
                        Annotations.Add(_error);
                    });
                }
                
            }
        }

        public void ShowProgressBar(ErrorArgs e)
        {
            double fullWidth = 20.0;

            if (_progressBar == null && _progressBox == null)
            {
                _progressBar = new PolygonAnnotation();

                _progressBar.BackColor = Color.Green;
                _progressBar.LineColor = Color.DarkGreen;

                _progressBar.GraphicsPath.AddRectangle(new Rectangle(0, 0, 100, 100));
                _progressBar.Alignment = ContentAlignment.TopLeft;
                _progressBar.Visible = true;
                _progressBar.Height = 2.5;
                _progressBar.Width = fullWidth * (e.Progress / 100.0);
                _progressBar.X = 50.0 - fullWidth / 2.0;
                _progressBar.Y = 50.0 - _progressBar.Height / 2.0;      
                
                _progressBar.LineWidth = 2;

                _progressBox = new PolygonAnnotation();

                _progressBox.BackColor = Color.Gray;
                _progressBox.LineColor = Color.DarkGray;

                _progressBox.GraphicsPath.AddRectangle(new Rectangle(0, 0, 100, 100));
                _progressBox.Alignment = ContentAlignment.TopLeft;
                _progressBox.Visible = true;
                _progressBox.Height = 4.0;
                _progressBox.Width = 21.5;
                _progressBox.X = 50.0 - _progressBox.Width / 2.0;        
                _progressBox.Y = 50.0 - _progressBox.Height / 2.0;                
                _progressBox.LineWidth = 2;
                

                //Annotations.Insert(0, _equation);

                
                this.Invoke((MethodInvoker)delegate()
                {
                    Annotations.Add(_progressBox);
                    Annotations.Add(_progressBar);
                });
            }
            else
            {
                this.Invoke((MethodInvoker)delegate()
                {
                    if (_progressBar != null)
                        _progressBar.Width = fullWidth * (e.Progress / 100.0);
                });
            }
        }

        public void ClearProgressBar()
        {
            if (_progressBar != null && _progressBox != null)
            {
                Annotations.Remove(_progressBar);
                _progressBar = null;

                Annotations.Remove(_progressBox);
                _progressBox = null;
            }
        }

        protected void UpdateErrorLocation(ChartPaintEventArgs e)
        {
            if (_error != null && _errorBox != null)
            {
                var chartArea = ChartAreas[0];

                double heightPixel = 100.0 / Height;
                double widthPixel = 100.0 / Width;

                _error.ResizeToContent();
                

                if (_error.Height > 0 && _error.Width > 0)
                {

                    double hPercent = _error.Height * heightPixel;
                    double wPercent = _error.Width * widthPixel;

                    double hbPercent = _errorBox.Height * heightPixel;
                    double wbPercent = _errorBox.Width * widthPixel;

                    //var plotWidthStart = chartArea.AxisX.ValueToPosition();
                    //var plotHeightStart = chartArea.AxisY.ValueToPosition((chartArea.AxisY.Maximum - chartArea.AxisY.Minimum) / 2.0 + chartArea.AxisY.Minimum);

                    var xValue = 50 - _error.Width / 2.0;//chartArea.AxisX.PositionToValue(0.0);
                    var yValue = 50 - _error.Height / 2.0;// chartArea.AxisY.PositionToValue(0.0);



                    //var xCenter = (chartArea.AxisX.Maximum - chartArea.AxisX.Minimum) / 2.0 + chartArea.AxisX.Minimum;
                    //var yCenter = (chartArea.AxisY.Maximum - chartArea.AxisY.Minimum) / 2.0 + chartArea.AxisY.Minimum;
                    //var xValue = xCenter - wPercent / 2.0;
                    //var yValue = yCenter - hPercent / 2.0;

                    if (_error.X != xValue)
                    {
                        _error.X = xValue;
                        _error.X = xValue;// -(widthPixel * .5);
                    }

                    if (_error.Y != yValue)
                    {
                        _error.Y = yValue;
                        _error.Y = yValue;// -(heightPixel * .5);
                    }



                    _error.Text = _error.Text;

                    _error.ResizeToContent();

                    _errorBox.Width = _error.Width * 1.1;
                    _errorBox.Height = _error.Height * 1.1;

                    var xValueB = 50 - _errorBox.Width / 2.0;//chartArea.AxisX.PositionToValue(0.0);
                    var yValueB = 50 - _errorBox.Height / 2.0;// chartArea.AxisY.PositionToValue(0.0);

                    if (_errorBox.X != xValueB)
                    {
                        _errorBox.X = xValueB;
                        _errorBox.X = xValueB;// -(widthPixel * .5);
                    }

                    if (_errorBox.Y != yValueB)
                    {
                        _errorBox.Y = yValueB;
                        _errorBox.Y = yValueB;// -(heightPixel * .5);
                    }

                    _errorBox.Visible = true;
                    _error.Visible = true;
                }
                else
                {
                    _error.Visible = false;
                    _errorBox.Visible = false;
                }
            }
            
        }

        public void ClearErrors()
        {
            if (_error != null)
            {
                Annotations.Remove(_error);
                _error = null;
                Annotations.Remove(_errorBox);
                _errorBox = null;
            }
        }

        public void ResetErrors()
        {
            _resetErrors = true;
        }

        public int SingleSeriesCount { get; set; }



        public Color HighlightColor
        {
            get
            {
                return ColorManager.TransformColor(Color.LightSteelBlue);
            }
        }

        public Color SelectColor
        {
            get
            {
                return ColorManager.TransformColor(Color.SteelBlue);
            }
        }

        public void ForceSelectionOff()
        {
            _isSelected = false;
            Invalidate();
        }

        //all code taken from http://stackoverflow.com/questions/12162013/mschart-line-graph-with-dashed-style-and-large-amount-of-data-points
        //written by Goz, http://stackoverflow.com/users/131140/goz
        //I did slight modifcations so my point series wouldn't be drawn like line series
        protected List<int> mBorderWidths = null;
        public delegate void FailedDrawingHandler(DataPointChart chart, string myError);
        public event FailedDrawingHandler HasFailedDrawing;
        private RelabelParameters _xRelabel = null;
        private RelabelParameters _yRelabel = null;
        
        protected void LineChartPrePaint(object sender, System.Windows.Forms.DataVisualization.Charting.ChartPaintEventArgs e)
        {
            if (e.ChartElement.GetType() == typeof(System.Windows.Forms.DataVisualization.Charting.ChartArea))
            {
                System.Windows.Forms.DataVisualization.Charting.Chart c = (System.Windows.Forms.DataVisualization.Charting.Chart)e.Chart;
                System.Windows.Forms.DataVisualization.Charting.ChartArea ca = (System.Windows.Forms.DataVisualization.Charting.ChartArea)e.ChartElement;

                mBorderWidths = new List<int>();
                foreach (System.Windows.Forms.DataVisualization.Charting.Series s in c.Series)
                {
                    mBorderWidths.Add(s.BorderWidth);
                    s.BorderWidth = 0;
                    s.ShadowOffset = 0;
                }

                RectangleF rectF = ca.Position.ToRectangleF();
                rectF = e.ChartGraphics.GetAbsoluteRectangle(rectF);

                e.ChartGraphics.Graphics.FillRectangle(new SolidBrush(ca.BackColor), rectF);
            }
            if (e.ChartElement.GetType() == typeof(System.Windows.Forms.DataVisualization.Charting.Chart))
            {
                RectangleF rectF = e.Position.ToRectangleF();
                rectF = e.ChartGraphics.GetAbsoluteRectangle(rectF);

                e.ChartGraphics.Graphics.FillRectangle(new SolidBrush(e.Chart.BackColor), rectF);
            }
        }

        protected System.Drawing.Drawing2D.DashStyle ChartToDrawingDashStyle(System.Windows.Forms.DataVisualization.Charting.ChartDashStyle cds)
        {
            switch (cds)
            {
                case System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet:
                case System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid:
                    return System.Drawing.Drawing2D.DashStyle.Solid;
                case System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash:
                    return System.Drawing.Drawing2D.DashStyle.Dash;
                case System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.DashDot:
                    return System.Drawing.Drawing2D.DashStyle.DashDot;
                case System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.DashDotDot:
                    return System.Drawing.Drawing2D.DashStyle.DashDotDot;
                case System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot:
                    return System.Drawing.Drawing2D.DashStyle.Dot;
            }
            return System.Drawing.Drawing2D.DashStyle.Solid;
        }

        protected void LineChartPostPaint(object sender, System.Windows.Forms.DataVisualization.Charting.ChartPaintEventArgs e)
        {
            if (e.ChartElement.GetType() == typeof(System.Windows.Forms.DataVisualization.Charting.ChartArea))
            {
                System.Windows.Forms.DataVisualization.Charting.Chart c = (System.Windows.Forms.DataVisualization.Charting.Chart)e.Chart;
                System.Windows.Forms.DataVisualization.Charting.ChartArea ca = (System.Windows.Forms.DataVisualization.Charting.ChartArea)e.ChartElement;

                RectangleF clipRect = e.ChartGraphics.GetAbsoluteRectangle(e.Position.ToRectangleF());
                RectangleF oldClip = e.ChartGraphics.Graphics.ClipBounds;
                e.ChartGraphics.Graphics.SetClip(clipRect);

                int seriesIdx = 0;
                foreach (System.Windows.Forms.DataVisualization.Charting.Series s in c.Series)
                {
                    try
                    {
                        PointF ptFLast = new PointF(0.0f, 0.0f);
                        List<PointF> points = new List<PointF>();
                        foreach (System.Windows.Forms.DataVisualization.Charting.DataPoint dp in s.Points)
                        {
                            double dx = (double)dp.XValue;
                            double dy = (double)dp.YValues[0];

                            // Log the value if its axis is logarithmic.
                            if (ca.AxisX.IsLogarithmic)
                            {
                                dx = Math.Log10(dx);
                            }
                            if (ca.AxisY.IsLogarithmic)
                            {
                                dy = Math.Log10(dy);
                            }

                            dx = e.ChartGraphics.GetPositionFromAxis(ca.Name, System.Windows.Forms.DataVisualization.Charting.AxisName.X, dx);
                            dy = e.ChartGraphics.GetPositionFromAxis(ca.Name, System.Windows.Forms.DataVisualization.Charting.AxisName.Y, dy);

                            PointF ptFThis = e.ChartGraphics.GetAbsolutePoint(new PointF((float)dx, (float)dy));
                            points.Add(ptFThis);
                        }


                        if (points.Count > 0 && s.ChartType == SeriesChartType.Line && s.Enabled)
                        {
                            Pen pen = new Pen(Color.FromArgb(255, s.Color));
                            pen.Width = mBorderWidths[seriesIdx];
                            pen.DashStyle = ChartToDrawingDashStyle(s.BorderDashStyle);
                            //pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;
                            //pen.DashPattern   = new float[]{ 4.0f, 4.0f, 1.0f, 3.0f, 2.0f, 3.0f };
                            pen.DashCap = System.Drawing.Drawing2D.DashCap.Round;

                            e.ChartGraphics.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                            e.ChartGraphics.Graphics.DrawLines(pen, points.ToArray());

                        }
                        s.BorderWidth = mBorderWidths[seriesIdx];
                    }
                    catch
                    {

                    }
                }

                e.ChartGraphics.Graphics.SetClip(oldClip);
            }
        }

    }

}
