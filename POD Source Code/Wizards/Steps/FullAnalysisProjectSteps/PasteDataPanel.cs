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
using POD.Data;
using POD.Analyze;

namespace POD.Wizards.Steps.FullAnalysisProjectSteps
{
    public partial class PasteDataPanel : WizardPanel
    {
        

        private Dictionary<string, DataGridViewDB> _views;

        private DefinitionMode _definitionMode;

        private Dictionary<string, SortedSet<int>> _specIdIndex = new Dictionary<string, SortedSet<int>>();

        private Dictionary<string, SortedSet<int>> _metadataIndex = new Dictionary<string, SortedSet<int>>();

        private Dictionary<string, SortedSet<int>> _flawIndex = new Dictionary<string, SortedSet<int>>();

        private Dictionary<string, SortedSet<int>> _responseIndices = new Dictionary<string, SortedSet<int>>();

        private Dictionary<string, DataTable> _tableBackups = new Dictionary<string, DataTable>();

        private List<Dictionary<string, SortedSet<int>>> _allIndexes = new List<Dictionary<string, SortedSet<int>>>();

        private bool _shiftDown = false;

        private DataGridViewDB _activeGrid;
        private bool _hasShift;
        private int _maxIndex;
        private int _minIndex;
        private Cursor _paintBucket = null;

        private List<RadioButton> _radios = new List<RadioButton>();
        private RadioButton _lastChecked = null;
        private List<Panel> _colorPanels = new List<Panel>();
        private DataGridViewSelectedColumnCollection _lastSelected = null;
        private int _firstMouseOver = 0;
        private bool _movedOutside = false;
        private DefinitionMode _tempMode = DefinitionMode.None;
        private bool _clickedOutside;
        AnalysisList _currentAnalyses = new AnalysisList();
        private bool _addedNew;
        private string _defaultInUseMessage = "This source cannot be modified since it is used by existing analyses.\n\nThis is done in order to ensure your analyses' data integrity.";
        private List<PODTabPage> _removedSources = new List<PODTabPage>();
        private Dictionary<PODTabPage, PODTabPage> _removedTrackConnectingNode = new Dictionary<PODTabPage, PODTabPage>();
        private Color _oldBucketColor = Color.Black;
        private bool _spaceDown;


        public PasteDataPanel(PODToolTip tooltip)
            : base(tooltip)
        {
            InitializeComponent();

            StepToolTip = new PODToolTip(this.components);

            _views = new Dictionary<string, DataGridViewDB>();

            //fix up the first tab
            _views.Add("D01", DefaultDataGrid);
            this.DefaultDataGrid.AutoGenerateColumns = true;
            DefaultDataGrid.ContextMenuStrip = contextMenuStrip1;
            
            //_dataGrid = new clsDgvCopyPasteEx(_views[0]);

            _allIndexes.Add(_specIdIndex);
            _allIndexes.Add(_metadataIndex);
            _allIndexes.Add(_flawIndex);
            _allIndexes.Add(_responseIndices);

            _radios.AddRange(new RadioButton[] {InactiveRadio, DefineIDRadio, DefineMetadataRadio, DefineFlawRadio, DefineResponseRadio, UndefineRadio });
            _colorPanels.AddRange(new Panel[] {InactiveColorPnl, IDColorPnl, MetaDataColorPnl, FlawColorPnl, ResponseColorPnl, UndefineColorPnl});

            foreach (RadioButton radio in _radios)
            {
                radio.BackColor = Color.FromKnownColor(KnownColor.Transparent);
                radio.Parent.BackColor = Color.FromKnownColor(KnownColor.Control);
            }

            GridSetupEventHandling(DefaultDataGrid);        

            UpdateBucketCursorColor(DefaultDataGrid);

            Load += PasteDataPanel_Load;

            StepToolTip.Popup += ToolTip_Show;
            StepToolTip.ShowAlways = true;

            _definitionMode = DefinitionMode.ID;

            IDColorPnl.BackColor = IDColor;
            MetaDataColorPnl.BackColor = MetaColor;
            FlawColorPnl.BackColor = FlawColor;
            ResponseColorPnl.BackColor = ResponseColor;

            Load += PasteDataPanel_Load;

        }

        public void SwitchEdit()
        {
            InactiveRadio.PerformClick();
        }

        public void SwitchID()
        {
            DefineIDRadio.PerformClick();
        }

        public void SwitchMeta()
        {
            DefineMetadataRadio.PerformClick();
        }

        public void SwitchFlaw()
        {
            DefineFlawRadio.PerformClick();
        }

        public void SwitchResponse()
        {
            DefineResponseRadio.PerformClick();
        }

        public void SwitchUndefine()
        {
            UndefineRadio.PerformClick();
        }


        void PasteDataPanel_Load(object sender, EventArgs e)
        {
            PaintControlBackPnl.Width = PaintControlTableLayout.Width + PaintControlBackPnl.Padding.Left * 2 + 3;

            //ALL TOOLTIPS MUST BE DONE IN LOAD FUNCTION!
            StepToolTip.SetToolTip(InactiveRadio, "Edit the data in the table. (Ctrl+1)");
            StepToolTip.SetToolTip(DefineIDRadio, "Define columns as ID columns. (Ctrl+2)");
            StepToolTip.SetToolTip(DefineMetadataRadio, "Define columns as Metadata columns. (Ctrl+3)");
            StepToolTip.SetToolTip(DefineFlawRadio, "Define columns as Flaw columns. (Ctrl+4)");
            StepToolTip.SetToolTip(DefineResponseRadio, "Define columns as Response columns. (Ctrl+5)");
            StepToolTip.SetToolTip(UndefineRadio, "Undefine columns. (Ctrl+6)");
        }

        void Grid_SelectionChanged(object sender, EventArgs e)
        {
            _lastSelected = DefaultDataGrid.SelectedColumns;
        }

        private Color UpdateBucketCursorColor(DataGridViewDB grid, Color color)
        {
            if(_definitionMode == DefinitionMode.None || IsGridOff(grid))
            {
                _paintBucket = Cursors.Default;
                _oldBucketColor = Color.Black;
                return _oldBucketColor;
            }

            if (color != _oldBucketColor)
            {
                Bitmap bucket = Properties.Resources.paint_bucket;

                var startX = 20;
                var endX = 27;
                var startY = 24;
                var endY = 31;
                //var startPrevX = 16;
                //var endPrevX = 20;
                //var startSmallY = 24;
                //var endSmallY = 31;
                //var startNextX = 27;
                //var endNextX = 31;
                Color outlineColor = Color.Black;
                Color nextColor = GetNextColorFromDefinitionMode(grid);
                Color prevColor = GetPreviousColorFromDefinitionMode(grid);

                ColorOutlineOfBox(bucket, startX, endX, startY, endY, outlineColor);
                ColorInsideOfBox(bucket, startX, endX, startY, endY, color);

                //ColorOutlineOfBox(bucket, startPrevX, endPrevX, startSmallY, endSmallY, outlineColor);
                //ColorInsideOfBox(bucket, startPrevX, endPrevX, startSmallY, endSmallY, prevColor);

                //ColorOutlineOfBox(bucket, startNextX, endNextX, startSmallY, endSmallY, outlineColor);
                //ColorInsideOfBox(bucket, startNextX, endNextX, startSmallY, endSmallY, nextColor);


                _paintBucket = Globals.CreateCursorNoResize(bucket, 5, 18);
                _oldBucketColor = color;

                return color;
            }
            else
            {
                return _oldBucketColor;
            }
        }

        private Color UpdateBucketCursorColor(DataGridViewDB grid)
        {
            return UpdateBucketCursorColor(grid, GetColorFromDefinitionMode(grid));
        }

        private static void ColorInsideOfBox(Bitmap bucket, int startX, int endX, int startY, int endY, Color color)
        {
            for (int i = startX + 1; i < endX; i++)
            {
                for (int j = startY + 1; j < endY; j++)
                {
                    bucket.SetPixel(i, j, color);
                }
            }
        }

        private static Color ColorOutlineOfBox(Bitmap bucket, int startX, int endX, int startY, int endY, Color color)
        {
            for (int i = startX; i <= endX; i++)
            {
                for (int j = startY; j <= endY; j++)
                {
                    if (i == startX | i == endX | j == startY | j == endY)
                        bucket.SetPixel(i, j, color);
                }
            }
            return color;
        }

        private Color GetPreviousColorFromDefinitionMode(DataGridViewDB grid)
        {
            return GetColorFromDefinitionMode(grid, GetPreviousDefinition(_definitionMode));
        }

        private Color GetNextColorFromDefinitionMode(DataGridViewDB grid)
        {
            return GetColorFromDefinitionMode(grid, GetNextDefinition(_definitionMode));
        }

        private void PasteFromKeyboard(object sender, EventArgs e)
        {
            PasteFromClipboard();
        }

        public override void RefreshValues()
        {
            //base.RefreshValues();



            //Visible = false;

            DataSourceTabs.SuspendDrawing();
            DataSourceTabs.SuspendLayout();

            _currentAnalyses.Clear();

            Project.RequestAnalyses(ref _currentAnalyses);

            int index = DataSourceTabs.SelectedIndex;

            _views.Clear();
            
            DataSourceTabs.TabPages.Clear();

            Project project = Project;

            //add any existing tabs
            for (int i = 0; i < project.Sources.Count; i++)
            {
                var tempName = AddNewSourcePage(false);

                var sourceName = project.Sources[i].SourceName;

                sourceName = UpdateDictionaries(tempName, sourceName);

                //_views[i].DataSource = project.Sources[i].Source;
                if (!_tableBackups.ContainsKey(sourceName) || _tableBackups[sourceName] == null)
                {
                       
                    _views[sourceName].Table = (DataTable)project.Sources[i].Original;
                }
                else
                {
                    _views[sourceName].Table = _tableBackups[sourceName];
                }

                //update the name
                LastTabToAdd.Text = project.Sources[i].SourceName;
                
                //ChangeToInactive(null);
            }

            //update any tables created but not added to the source yet
            foreach(string key in _tableBackups.Keys)
            {
                //if not a source yet
                if (project.Sources[key] == null)
                {
                    var tempName = AddNewSourcePage(false);

                    var sourceName = key;

                
                    sourceName = UpdateDictionaries(tempName, sourceName);

                    _views[sourceName].Table = _tableBackups[sourceName];

                    //update the name
                    LastTabToAdd.Text = sourceName;
                }

                //UpdateView(sourceName);

                //SetColumnColorsFromIndices(_views[sourceName]);
            }

            if (project.Sources.Count == 0)
            {
                AddNewSourcePage(true);
            }

            AddTabsFromList();

            if (index >= 0 && index < DataSourceTabs.TabPages.Count)
            {
                DataSourceTabs.SelectedIndex = index;
            }

            RadioButton temp = _lastChecked;

            ReloadAllIndicies();

            _definitionMode = GetLastColumnDefinitionMode(_views[SelectedTabName]);

            UpdatePaintControlsFromDefinitionMode(_views[SelectedTabName]);            

            PaintGridBackgroundBasedOnUse();            
            
            if(temp != null)
            {
                //force the grid to the last checked mode
                _lastChecked = null;
                UpdateGridFromRadioButton(temp, _views[SelectedTabName]);
            }

            UpdateView(SelectedTabName);

            DataSourceTabs.ResumeLayout();
            DataSourceTabs.ResumeDrawing();
            
            Visible = true;

            //DefinitionButton_CheckChanged(temp, null);

            //TODO: make function that previews the paste of whatever is in the clipboard
            //as greyed out grid then when the user pastes something make it the traditional white
            //this way they don't have to paste to see what they have
            /*DataSourceTabs.SelectedTab.Controls[0].ContextMenuStrip.Items[0].PerformClick();

            _views[DataSourceTabs.SelectedIndex].DefaultCellStyle.BackColor = _views[DataSourceTabs.SelectedIndex].BackgroundColor;
            _views[DataSourceTabs.SelectedIndex].DefaultCellStyle.ForeColor = SystemColors.ControlDarkDark;
            _views[DataSourceTabs.SelectedIndex].EnableHeadersVisualStyles = false;
            _views[DataSourceTabs.SelectedIndex].ColumnHeadersDefaultCellStyle.ForeColor = SystemColors.ControlDarkDark;
            _views[DataSourceTabs.SelectedIndex].ColumnHeadersDefaultCellStyle
            _views[DataSourceTabs.SelectedIndex].ColumnHeadersDefaultCellStyle.BackColor = _views[DataSourceTabs.SelectedIndex].BackgroundColor;*/
        }

        private void PaintGridBackgroundBasedOnUse()
        {
            foreach (TabPage page in DataSourceTabs.TabPages)
            {
                var grid = page.Controls[0] as DataGridViewDB;

                if (grid != null)
                {
                    if (CanSourceBeEdited(page))
                    {
                        grid.BackgroundColor = Color.FromKnownColor(KnownColor.ControlDark);
                        grid.ForeColor = grid.BackgroundColor;
                        grid.DefaultCellStyle.ForeColor = Color.Black;
                        grid.DefaultCellStyle.SelectionForeColor = Color.Black;
                    }
                    else
                    {
                        grid.BackgroundColor = Color.FromKnownColor(KnownColor.ControlDarkDark);
                        grid.ForeColor = GridOffTextColor;
                        grid.DefaultCellStyle.ForeColor = GridOffTextColor;
                        grid.DefaultCellStyle.SelectionForeColor = GridOffTextColor;
                    }
                }
            }
        }

        private static Color GridOffTextColor
        {
            get
            {
                return Color.FromArgb(64, 64, 64);
            }
        }

        private void UpdateView(string name)
        {
            this.SetColumnSortModeAndColor(_views[name].Columns);

            if (_definitionMode != DefinitionMode.None)
                UpdateSelectionMode(_views[name], DataGridViewSelectionMode.FullColumnSelect);
            else
                UpdateSelectionMode(_views[name], DataGridViewSelectionMode.CellSelect);

            SetColumnColorsFromProject(name, _views[name]);
            GridCheckForProblems(_views[name]);
        }

        internal void PasteFromClipboard()
        {
            try
            {
                //_views[DataSourceTabs.SelectedIndex].DataSource = null;
                //_views[DataSourceTabs.SelectedIndex].Columns.Clear();
                //_views[DataSourceTabs.SelectedIndex].Rows.Clear();
            }
            catch
            {

            }

            // DataSourceTabs.SelectedTab.Controls[0].ContextMenuStrip.Items[0].PerformClick();

            var tab = DataSourceTabs.SelectedTab;
            var grid = _views[SelectedTabName];

            if (tab != null)
            {
                if (CanSourceBeEdited(tab))
                {
                    var clipboardContents = Clipboard.GetText(TextDataFormat.Rtf);

                    //var fileNameCollection = Clipboard.GetFileDropList();
                    //string copiedFilePath = (fileNameCollection != null && fileNameCollection.Count > 0) ? fileNameCollection[0] : "";

                    //MessageBox.Show(copiedFilePath);

                    this.UpdateDataTable(clipboardContents);
                }
                else
                {
                    var result = AskUserToAddDuplicateGrid(grid, ref tab);

                    IsGridOff(grid);

                    _addedNew = result;
                }
            }
        }

        private void UpdateDataTable(string clipboardContents)
        {
            var currentTable = ((DataTable)_views[SelectedTabName].DataSource).Copy();
            var tempTable = DataHelpers.GetTableFromRtfString(clipboardContents);
            int beforeColCount = currentTable.Columns.Count;
            var tabPage = DataSourceTabs.SelectedTab;

            UpdateSelectionMode(_views[SelectedTabName], DataGridViewSelectionMode.CellSelect);


            _views[SelectedTabName].DataSource = null;

            var newCols = new List<DataColumn>();

            if (currentTable == null || currentTable.Rows.Count == 0)
            {
                _views[SelectedTabName].Table = tempTable;

                foreach (DataColumn column in currentTable.Columns)
                    newCols.Add(column);

                var allNames = GetAllTabNamesExceptCurrent(tabPage);
                var newNameForm = new NewSourceForm(tabPage.Text, allNames);

                newNameForm.ShowDialog();

                tabPage.Text = UpdateDictionaries(tabPage.Text, newNameForm.NewName);
            }
            else
            {
                foreach (DataColumn dc in tempTable.Columns)
                {
                    //we want to make sure we never have a column with the same name
                    //since that will crash the program
                    var tempName = dc.ColumnName;
                    var finalName = tempName;
                    int index = 2;

                    while(currentTable.Columns.Contains(finalName))
                    {
                        finalName = tempName + "_" + index.ToString("##");
                        index++;
                    }

                    tempTable.Columns[tempName].ColumnName = finalName;

                    var col = currentTable.Columns.Add(finalName, dc.DataType);

                    newCols.Add(col);
                }

                int i = 0;

                //_views[DataSourceTabs.SelectedIndex].DataSource = null;

                //currentTable.BeginLoadData();

                foreach(DataRow row in tempTable.Rows)
                {       
                    foreach (DataColumn dc in tempTable.Columns)
                    {
                        if (currentTable.Rows.Count <= i)
                            break;

                        currentTable.Rows[i][dc.ColumnName] = row[dc.ColumnName];
                        
                    }

                    i++;
                }

                //tempTable.AcceptChanges();

                //currentTable.Merge(tempTable, false, MissingSchemaAction.Add);

                //currentTable.EndLoadData();

                //currentTable.AcceptChanges();

                _views[SelectedTabName].Table = currentTable;
            }

            foreach (DataGridViewColumn col in _views[SelectedTabName].Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            this.SetColumnSortModeAndColor(_views[SelectedTabName].Columns, newCols);

            var temp = _definitionMode;

            ChangeToInactive(null);

            _definitionMode = temp;

            UpdatePaintControlsFromDefinitionMode(_views[SelectedTabName]);
            SetColumnColorsFromProject(SelectedTabName, _views[SelectedTabName]);
            SetColumnColorsFromIndices(_views[SelectedTabName]);
            
            if (_definitionMode != DefinitionMode.None && _definitionMode != DefinitionMode.ClearOne)
            {

                _views[SelectedTabName].ClearSelection();
                UpdateSelectionMode(_views[SelectedTabName], DataGridViewSelectionMode.FullColumnSelect);

                foreach (DataColumn column in newCols)
                {
                    _views[SelectedTabName].Columns[column.ColumnName].SortMode = DataGridViewColumnSortMode.NotSortable;
                    _views[SelectedTabName].Columns[column.ColumnName].Selected = true;
                }

                ForceColumnData(_views[SelectedTabName]);                
            }

            _definitionMode = GetLastColumnDefinitionMode(_views[SelectedTabName]);

            UpdatePaintControlsFromDefinitionMode(_views[SelectedTabName]);

            _views[SelectedTabName].Refresh();
        }

        private int GetLastColumnIndex(DataGridViewDB myGrid, Dictionary<string, SortedSet<int>> set)
        {
            var name = GetNameFromGrid(myGrid);
            var highestIndex = -1;

            if (set.ContainsKey(name))
            {
                var values = set[name];

                if(values != null && values.Count > 0)
                    highestIndex = values.Max;
            }

            return highestIndex;
        }

        private string GetNameFromGrid(DataGridViewDB myGrid)
        {
            if(myGrid.Parent != null)
            {
                var parent = myGrid.Parent as TabPage;

                if (parent != null)
                    return parent.Text;
            }

            //if grid not set yet try looking in the views dictionary
            foreach(var key in _views.Keys)
            {
                var value = _views[key];
            
                if (value == myGrid)
                    return key;
            }

            //otherwise it has no name
            return "";
        }

        private DefinitionMode GetLastColumnDefinitionMode(DataGridViewDB myGrid)
        {
            var highestIndex = -1;
            var mode = DefinitionMode.ID; //default to ID instead of edit cells so people figure out painting quicker
            var index = 0;

            index = GetLastColumnIndex(myGrid, _specIdIndex);
            if(index > highestIndex)
            {
                highestIndex = index;
                mode = DefinitionMode.ID;
            }

            index = GetLastColumnIndex(myGrid, _metadataIndex);
            if (index > highestIndex)
            {
                highestIndex = index;
                mode = DefinitionMode.MetaData;
            }

            index = GetLastColumnIndex(myGrid, _flawIndex);
            if (index > highestIndex)
            {
                highestIndex = index;
                mode = DefinitionMode.Flaw;
            }

            index = GetLastColumnIndex(myGrid, _responseIndices);
            if (index > highestIndex)
            {
                highestIndex = index;
                mode = DefinitionMode.Response;
            }

            return mode;
        }

        private void SetColumnColorsFromIndices(DataGridViewDB myGrid)
        {
            ColorColumnsByMode(myGrid, _specIdIndex, DefinitionMode.ID);
            ColorColumnsByMode(myGrid, _metadataIndex, DefinitionMode.MetaData);
            ColorColumnsByMode(myGrid, _flawIndex, DefinitionMode.Flaw);
            ColorColumnsByMode(myGrid, _responseIndices, DefinitionMode.Response);
        }

        private void ColorColumnsByMode(DataGridViewDB myGrid, Dictionary<string,SortedSet<int>> set, DefinitionMode definitionMode)
        {
            List<int> list = null;
            var name = GetNameFromGrid(myGrid);

            if (set.ContainsKey(name))
            {
                list = set[name].ToList();

                ColorColumns(myGrid, list, GetColorFromDefinitionMode(myGrid, definitionMode));
            }
        }

        public void SetColumnSortModeAndColor(DataGridViewColumnCollection columnList, List<DataColumn> columns)
        {
            DataGridViewColumn gridColumn = null;

            foreach (DataColumn column in columns)
            {
                if (columnList.Contains(column.ColumnName))
                {
                    gridColumn = columnList[column.ColumnName];

                    gridColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
                    gridColumn.DefaultCellStyle.SelectionForeColor = Color.Black;
                    gridColumn.DefaultCellStyle.SelectionBackColor = Color.White;
                }
            }
        }
        
        public void SetColumnSortModeAndColor(DataGridViewColumnCollection columnList)
        {
            foreach (DataGridViewColumn column in columnList)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
                column.DefaultCellStyle.SelectionForeColor = Color.Black;
                column.DefaultCellStyle.SelectionBackColor = Color.White;
            }
        }

        internal void InsertFromClipboard()
        {
            DataSourceTabs.SelectedTab.Controls[0].ContextMenuStrip.Items[1].PerformClick();
        }

        internal void CopyDataToSource()
        {
            Project project = (Project)this.Source;


            project.Sources.Clear();

            for (int i = 0; i < DataSourceTabs.TabPages.Count; i++)
            {
                TableRange specID = new TableRange(RangeNames.SpecID);
                TableRange metaData = new TableRange(RangeNames.MetaData);
                TableRange flawSize = new TableRange(RangeNames.FlawSize);
                TableRange response = new TableRange(RangeNames.Response);

                var tabText = DataSourceTabs.TabPages[i].Text;
                var data = _views[tabText];
                var table = (DataTable)data.DataSource;
                var sourceName = tabText;

                StoreLastCurrentRow(tabText, data);
                StoreLastCurrentColumn(tabText, data);

                if (table != null && table.Columns.Count > 0 && table.Rows.Count > 0)
                {
                    this.SetTableRangeValues(tabText, specID, this._specIdIndex);
                    this.SetTableRangeValues(tabText, metaData, this._metadataIndex);
                    this.SetTableRangeValues(tabText, flawSize, this._flawIndex);
                    this.SetTableRangeValues(tabText, response, this._responseIndices);

                    project.AddSource(table, specID, metaData, flawSize, response, sourceName);                    

                    _tableBackups[tabText] = table;
                }
            }

            var deleteIndex = new List<string>();

            foreach(TabPage source in _removedSources)
            {
                var info = project.Infos.GetFromOriginalName(source.Text);
                
                deleteIndex.Add(source.Text);

                //this was causing a bug when removed source is same name as new source
                if (!project.Sources.Names.Contains(source.Text))
                    project.Infos.Remove(info);
            }

            foreach(var index in deleteIndex)
            {
                _tableBackups.Remove(index);
            }

            _removedSources.Clear();
            _removedTrackConnectingNode.Clear();
        }

        private void SetLastCurrentCell(string tabText)
        {
            if (!_views.ContainsKey(tabText))
                return;

            if (_views[tabText].FirstDisplayedScrollingRowIndex == -1 ||
                _views[tabText].FirstDisplayedScrollingColumnIndex == -1)
                return;

            if (_lastCurrentRow.ContainsKey(tabText) && _lastCurrentColumn.ContainsKey(tabText))
            {
                _views[tabText].FirstDisplayedScrollingRowIndex = _lastCurrentRow[tabText];
                _views[tabText].FirstDisplayedScrollingColumnIndex = _lastCurrentColumn[tabText];
            }
            else
            {
                if (_views[tabText].Rows.Count > 0 && _views[tabText].Columns.Count > 0)
                {
                    _views[tabText].FirstDisplayedScrollingRowIndex = 0;
                    _views[tabText].FirstDisplayedScrollingColumnIndex = 0;

                    StoreLastCurrentColumn(tabText, _views[tabText]);
                    StoreLastCurrentRow(tabText, _views[tabText]);
                }
            }
        }

        private void StoreLastCurrentColumn(string tabText, DataGridViewDB data)
        {
            if (data.CurrentCell != null)
            {
                if (!_lastCurrentColumn.ContainsKey(tabText))
                    _lastCurrentColumn.Add(tabText, data.FirstDisplayedScrollingColumnIndex);
                else
                    _lastCurrentColumn[tabText] = data.FirstDisplayedScrollingColumnIndex;
            }
        }

        private void StoreLastCurrentRow(string tabText, DataGridViewDB data)
        {
            if (data.CurrentCell != null)
            {
                if (!_lastCurrentRow.ContainsKey(tabText))
                    _lastCurrentRow.Add(tabText, data.FirstDisplayedScrollingRowIndex);
                else
                    _lastCurrentRow[tabText] = data.FirstDisplayedScrollingRowIndex;
            }
        }

        public void BackupTables()
        {
            //for (int i = 0; i < DataSourceTabs.TabPages.Count; i++)
            //{
            //    var sourceName = DataSourceTabs.TabPages[i].Text;
            //    var data = _views[sourceName];
            //    var table = (DataTable)data.Table;

            //    if (table != null && table.Columns.Count > 0 && table.Rows.Count > 0)
            //    {
            //        _tableBackups[sourceName] = table;
            //    }
            //}

            CopyDataToSource();
        }

        static public DataGridViewDB GetGridFromPage(PODTabPage page)
        {
            return page.Controls[0] as DataGridViewDB;
        }

        public void RestoreDeletedSources()
        {
            if(_removedSources.Count > 0)
            {
                var restore = _removedSources[_removedSources.Count - 1];

                RestoreSource(restore);

                _removedSources.RemoveAt(_removedSources.Count - 1);
                _removedTrackConnectingNode.Remove(restore);
            }
        }

        private void SetTableRangeValues(string name, TableRange tableRange, Dictionary<string, SortedSet<int>> rangeValues)
        {
            if (rangeValues.ContainsKey(name))
            {
                tableRange.Range = rangeValues[name].ToList();
                tableRange.Count = rangeValues[name].Count;

                if (tableRange.Count > 0)
                {
                    tableRange.StartIndex = rangeValues[name].Min;
                    tableRange.MaxIndex = rangeValues[name].Max;
                }
            }
        }

        /// <summary>
        /// Are some of the requirements needed to conitnued
        /// the step complete missing?
        /// </summary>
        public override bool Stuck
        {
            get
            {
                return ((Project)Source).Sources.Count == 0 || !AllSourcesHaveRanges();
            }
        }

        private bool AllSourcesHaveRanges()
        {
            var rangesSet = true;

            for (int i = 0; i < Project.Sources.Count; i++)
            {
                var name = Project.Sources[i].SourceName;

                if (!this._specIdIndex.ContainsKey(name) || !this._flawIndex.ContainsKey(name)
                    || !this._responseIndices.ContainsKey(name) || !this._specIdIndex[name].Any() || !this._flawIndex[name].Any()
                    || !this._responseIndices.Any())
                {
                    rangesSet = false;
                } 
            }

            return rangesSet;
        }

        /// <summary>
        /// CheckStuck() will take appropriate action to let user know
        /// that they cannot continue any farther.
        /// </summary>
        /// <returns>Can the wizard move to the next step?</returns>
        public override bool CheckStuck()
        {
            if (Stuck)
            {
                if (((Project)Source).Sources.Count == 0)
                {
                    try
                    {

                    
                    MessageBox.Show("Unfortunately, you need at least one source before you can continue." + Environment.NewLine + Environment.NewLine
                                    + "Please copy and paste data from Excel into the table." + Environment.NewLine
                                    + "Ctrl+V is supported or select Paste from the available actions.");
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e.StackTrace);
                    }
                }
                else if(!AllSourcesHaveRanges())
                {
                    MessageBox.Show("You need to define the data in your tables before you can continue." + Environment.NewLine + Environment.NewLine
                                    + "Select what you want to define from the controls above the table then" + Environment.NewLine
                                    + "simply click on any column in the table to define its purpose." + Environment.NewLine + Environment.NewLine
                                    + "You need at least one ID column, one flaw column and one response column.");
                }
            }
            else
            {
                //_tableBackups.Clear();    
            }

            return Stuck;
        }

        internal string AddNewSourcePage(bool selectAfterAdding)
        {
            DataSourceTabs.SuspendDrawing();

            string newSourceName = "";
            int index = -1;
            bool nameExists = true;

            while (nameExists == true)
            {
                index++;
                newSourceName = "D" + (index + 1).ToString("D2");

                nameExists = _views.ContainsKey(newSourceName) || _allIndexes[3].ContainsKey(newSourceName);
            }

            _maxIndex = Int32.MinValue;
            _minIndex = Int32.MaxValue;

            PODTabPage newTab = new PODTabPage(newSourceName);
            DataGridViewDB view = new DataGridViewDB();

            GridSetupEventHandling(view);        

            view.ContextMenuStrip = contextMenuStrip1;
            newTab.Controls.Add(view);
            newTab.Padding = new Padding(3);

            view.Dock = DockStyle.Fill;
            view.Table.TableName = newSourceName;

            index = DataSourceTabs.TabPages.Count - 1;
            
            _views.Add(newSourceName, view);

            AddToTabList(newTab);
            //DataSourceTabs.TabPages.Add(newTab);

            _selectAfterAdding = selectAfterAdding;

            CanSourceBeEdited(newTab);

            
            DataSourceTabs.ResumeDrawing();

            return newSourceName;
        }

        private List<PODTabPage> _tabsToAdd = new List<PODTabPage>();
        private Dictionary<string, int> _lastCurrentRow = new Dictionary<string, int>();
        private Dictionary<string, int> _lastCurrentColumn = new Dictionary<string, int>();
        private DefinitionMode _beforeSwitchCheck;
        private bool _enteredLastRow;
        private bool _selectAfterAdding;

        private PODTabPage LastTabToAdd
        {
            get
            {
                return _tabsToAdd.Last();
            }
        }
        

        private void AddToTabList(PODTabPage newTab)
        {
            _tabsToAdd.Add(newTab);
        }

        public void AddTabsFromList()
        {
            DataSourceTabs.TabPages.AddRange(_tabsToAdd.ToArray());

            foreach(PODTabPage page in _tabsToAdd)
            {
                //page.SuspendDrawing();

                UpdateView(page.Text);

                SetLastCurrentCell(page.Text);

                /*if (_views[page.Text].Rows.Count > 0 && _views[page.Text].Columns.Count > 0)
                {
                    _views[page.Text].ClearSelection();
                    _lastSelected = null;
                }*/
            }

            foreach (PODTabPage page in _tabsToAdd)
            {
                //page.ResumeDrawing();
            }

            if (_selectAfterAdding && _tabsToAdd.Count > 0)
            {
                DataSourceTabs.SelectedTab = _tabsToAdd.Last();
                //reset for next time
                _selectAfterAdding = false;
            }

            _tabsToAdd.Clear();
        }

        protected void GridSetupEventHandling(DataGridViewDB view)
        {
            view.ControlV += PasteFromKeyboard;
            view.KeyUp += Grid_KeyUp;
            view.KeyDown += Grid_KeyDown;
            view.SelectionChanged += Grid_SelectionChanged;
            view.MouseDown += Grid_MouseDown;
            view.MouseUp += Grid_MouseUp;
            view.MouseMove += Grid_MouseMove;
            view.MouseClick += Grid_Clicked;
            view.MouseEnter += Grid_MouseEnter;
            //view.RowsAdded += Grid_RowsAdded;

            view.ColumnHeaderMouseClick += ColumnHeader_Clicked;
            
            view.CellClick += Cell_Clicked;
            view.CellBeginEdit += Cell_CellBeginEdit;
            view.CellEndEdit += Cell_CellEndEdit;
            view.CellPainting += Cell_CellPainting;            
            view.CellDoubleClick += Cell_CellDoubleClick;
            view.CellMouseEnter += Cell_CellMouseEnter;
            view.CellMouseLeave += Cell_CellMouseLeave;
            view.ShowCellToolTips = false;

            StepToolTip.SetToolTip(view, "");

        }

        void Grid_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            var grid = sender as DataGridViewDB;

            if (grid == null || e.RowCount != 1 || e.RowIndex == 0) 
                return;

            if (grid.Rows[e.RowIndex].Cells[0].Value == DBNull.Value && grid.Rows[e.RowIndex - 1].Cells[0].Value != DBNull.Value)
            {
                for (int colIndex = 0; colIndex < grid.Columns.Count; colIndex++)
                    GridCheckForProblems(grid, e.RowIndex - 1, colIndex, false);

                grid.Invalidate();
            }


        }

        void Grid_MouseEnter(object sender, EventArgs e)
        {
            var grid = sender as DataGridViewDB;

            if (grid == null)
                return;

            grid.Select();
        }

        void Cell_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            var grid = sender as DataGridViewDB;

            if (grid == null)
                return;

            if (e.RowIndex == -1 || e.ColumnIndex == -1)
                return;

            var rowIndex = e.RowIndex;
            var colIndex = e.ColumnIndex;

            var row = grid.Rows[rowIndex];
            var cell = row.Cells[colIndex];

            StepToolTip.SetToolTip(grid, "");
            //cell.ToolTipText = "";
        }

        private static string GetCellToolTip(CellStatus cellStatus, RowStatus rowStatus)
        {
            string statusMessage = "";

            if (cellStatus == CellStatus.InvalidWholeRow)
                statusMessage = "Flaw size is invalid." + System.Environment.NewLine + "The entire row will not be included in the analysis.";
            else if (rowStatus == RowStatus.Invalid)
                statusMessage = "Row contains an invalid flaw size." + System.Environment.NewLine + "The entire row will not be included in the analysis.";
            else if (cellStatus == CellStatus.Invalid)
                statusMessage = "Response value is invalid." + System.Environment.NewLine + "The response will not be included in the analysis.";

            return statusMessage;
        }

        private static DataGridViewCell GetCellAndRowStatus(DataGridViewDB grid, int colIndex, int rowIndex, out CellStatus cellStatus, out RowStatus rowStatus)
        {
            var row = grid.Rows[rowIndex];
            var cell = row.Cells[colIndex];
            var rowTag = row.Tag as RowTag;
            var cellTag = cell.Tag as CellTag;

            if (rowTag == null)
                rowTag = InitializeRowTag(row, RowStatus.Valid);

            if (cellTag == null)
                cellTag = InitializeCellTag(cell, CellStatus.Valid);

            cellStatus = cellTag.Status;
            rowStatus = rowTag.Status;

            return cell;
        }

        void Cell_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            var grid = sender as DataGridViewDB;

            if (grid == null)
                return;

            if (e.RowIndex == -1 || e.ColumnIndex == -1)
                return;

            var statusMessage = "";

            var colIndex = e.ColumnIndex;
            var rowIndex = e.RowIndex;

            //if (_definitionMode != DefinitionMode.None)
            //    grid.CurrentCell = null;

            if(rowIndex == grid.Rows.Count - 1)
            {
                SelectCellUnderMouse(grid);
                
                if(_enteredLastRow == false)
                {
                    //if(_definitionMode != DefinitionMode.None)
                    _beforeSwitchCheck = _definitionMode;                
                    ChangeToInactive(_lastChecked);
                    _enteredLastRow = true;
                }
            }
            else if(rowIndex != grid.Rows.Count - 1 && _enteredLastRow == true)
            {
                _definitionMode = _beforeSwitchCheck;
                _minIndex = Int32.MaxValue;
                _maxIndex = Int32.MinValue;
                if(_definitionMode != DefinitionMode.None)
                    grid.CurrentCell = grid.FirstDisplayedCell;
                UpdatePaintControlsFromDefinitionMode(grid);
                _enteredLastRow = false;
                //grid.Invalidate();
            }

            CellStatus cellStatus;
            RowStatus rowStatus;
            var cell = GetCellAndRowStatus(grid, colIndex, rowIndex, out cellStatus, out rowStatus);

            statusMessage = GetCellToolTip(cellStatus, rowStatus);

            StepToolTip.SetToolTip(grid, statusMessage);
            //if (cell != null)
            //    cell.ToolTipText = statusMessage;            
        }

        void Cell_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            var grid = sender as DataGridViewDB;

            if (grid == null)
                return;

            DataGridViewColumn col = grid.Columns[e.ColumnIndex];
            DataGridViewRow row = grid.Rows[e.RowIndex];
            DataGridViewCell cell = row.Cells[e.ColumnIndex];

            cell.Style = cell.Style.Clone();
            cell.Style.SelectionBackColor = Color.White;
            cell.Style.BackColor = Color.White;
        }

        void Cell_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var grid = sender as DataGridViewDB;

            if (grid == null)
                return;

            DataGridViewColumn col = grid.Columns[e.ColumnIndex];
            DataGridViewRow row = grid.Rows[e.RowIndex];
            DataGridViewCell cell = row.Cells[e.ColumnIndex];

            cell.Style = col.DefaultCellStyle;

            GridCheckForProblems(sender as DataGridViewDB, e.RowIndex, e.ColumnIndex, true);
        }

       

        void Cell_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if(_definitionMode == DefinitionMode.None)
                _views[SelectedTabName].BeginEdit(true);
            else
            {
                //var grid = (DataGridViewDB)sender;

                //IncrementDefinitionMode(grid, _definitionMode);

                Cell_Clicked(sender, e);
            }
        }

        internal void DeleteCurrentSource()
        {
            DataSourceTabs.SuspendLayout();
            DataSourceTabs.SuspendDrawing();

            PODTabPage tab = DataSourceTabs.SelectedTab as PODTabPage;

            

            if (CanSourceBeEdited(tab))
            {
                if (DataSourceTabs.TabPages.Count > 1)
                {
                    var deleteIndex = DataSourceTabs.SelectedIndex;
                    var nextIndex = deleteIndex - 1;

                    PODTabPage nextTab = null;

                    if (nextIndex >= 0)
                        nextTab = DataSourceTabs.TabPages[nextIndex] as PODTabPage;
                    else
                        nextTab = null;// DataSourceTabs.TabPages[1] as PODTabPage;

                    if (nextIndex < 0)
                        nextIndex = 0;

                    //no point in tracking an empty grid
                    if (_views[tab.Text].Rows.Count > 0)
                    {
                        _removedSources.Add(tab);

                        if (!_removedTrackConnectingNode.ContainsKey(tab))
                            _removedTrackConnectingNode.Add(tab, nextTab);
                        else
                            _removedTrackConnectingNode[tab] = nextTab;
                    }
                    else
                        UpdateDictionariesRemove(tab.Text);

                    tab.SuspendDrawing();

                    var shutOffTab = DataSourceTabs.TabPages[nextIndex] as PODTabPage;

                    shutOffTab.SuspendDrawing();

                    DataSourceTabs.SelectedIndex = nextIndex;
                    DataSourceTabs.TabPages.Remove(tab);

                    tab.ResumeDrawing();

                    shutOffTab.ResumeDrawing();

                    //rename all of the tabs
                    /*for(int i = 0; i < DataSourceTabs.TabPages.Count; i++)
                    {
                        DataSourceTabs.TabPages[i].Text = "Data Source " + (DataSourceTabs.TabPages.Count + 1).ToString("D2");
                    }*/
                }
            }
            else
            {
                MessageBox.Show(_defaultInUseMessage, "POD v4 Notice");
            }

            DataSourceTabs.ResumeLayout();
            DataSourceTabs.ResumeDrawing();
        }

        

        private void Grid_NewTabSelected(object sender, EventArgs e)
        {
            //if (_views.Count > DataSourceTabs.SelectedIndex && DataSourceTabs.SelectedIndex >= 0) _dataGrid.Dgv = _views[DataSourceTabs.SelectedIndex];

            if(DataSourceTabs.SelectedTab != null && _views.ContainsKey(SelectedTabName))
            {

                _maxIndex = Int32.MinValue;
                _minIndex = Int32.MaxValue;

                SetColumnsSelectMode(_views[SelectedTabName], _definitionMode);
            }

        }

        private void DefinitionButton_CheckChanged(object obj, EventArgs e)
        {
            RadioButton sender = (RadioButton)obj;

            if (_views.Count > 0)
            {
                DataGridViewDB grid = _views[SelectedTabName];
                var tab = DataSourceTabs.SelectedTab;

                if (sender.Checked == true && tab != null && !CanSourceBeEdited(DataSourceTabs.SelectedTab))
                {
                    if (!AskUserToAddDuplicateGrid(grid, ref tab))
                    {
                        sender.Checked = false;
                    }
                    else
                    {
                        sender.Checked = true;
                        DefinitionButton_CheckChanged(sender, e);
                    }

                    return;
                }

                UpdateGridFromRadioButton(sender, grid);
            }
        }

        private void UpdateGridFromRadioButton(RadioButton sender, DataGridViewDB grid)
        {
            if(sender == InactiveRadio)
            {
                if (grid != null)
                    grid.CurrentCell = grid.FirstDisplayedCell;

                ChangeToInactive(InactiveRadio);   
            }
            else if (sender.Checked)
            {
                if (_lastChecked == sender)
                {
                    ChangeToInactive(sender);
                    return;
                }

                grid.ClearSelection();
                _lastChecked = sender;

                foreach (DataGridViewColumn col in grid.Columns)
                    col.SortMode = DataGridViewColumnSortMode.NotSortable;

                UpdateSelectionMode(grid, DataGridViewSelectionMode.FullColumnSelect);

                foreach (RadioButton button in _radios)
                {
                    if (button != sender && button.Checked)
                    {
                        button.Checked = false;
                    }
                }


                UpdateDefinitionModeFromRadioButton(sender);

                UpdateBucketCursorColor(grid);
                grid.Cursor = _paintBucket;
                grid.ReadOnly = true;

                _maxIndex = Int32.MinValue;
                _minIndex = Int32.MaxValue;

                UpdatePaintControlsFromDefinitionMode(grid);
                SetSelectColorForAllColumns(grid);

                UpdateSelectedCell(grid);

                grid.Invalidate();
            }
        }

        private void UpdateDefinitionModeFromRadioButton(RadioButton sender)
        {
            if (sender == DefineIDRadio)
                _definitionMode = DefinitionMode.ID;
            else if (sender == DefineMetadataRadio)
                _definitionMode = DefinitionMode.MetaData;
            else if (sender == DefineFlawRadio)
                _definitionMode = DefinitionMode.Flaw;
            else if (sender == DefineResponseRadio)
                _definitionMode = DefinitionMode.Response;
            else if (sender == UndefineRadio)
                _definitionMode = DefinitionMode.ClearOne;
            else if (sender == InactiveRadio)
                _definitionMode = DefinitionMode.None;
        }

        private void EditCellValues_CheckChanged(object sender, EventArgs e)
        {
            var grid = _views[SelectedTabName];
            var tab = DataSourceTabs.SelectedTab;

            if (InactiveRadio.Checked == true && tab != null && !CanSourceBeEdited(DataSourceTabs.SelectedTab))
            {
                if (!AskUserToAddDuplicateGrid(grid, ref tab))
                {
                    InactiveRadio.Checked = false;
                }
                else
                {
                    EditCellValues_CheckChanged(sender, e);
                }

                return;
            }

            UpdateGridFromRadioButton(sender as RadioButton, grid);
        }

        private void ChangeToInactive(RadioButton mySelectedButton)
        {
            if (!_views.ContainsKey(SelectedTabName))
                return;

            DataGridViewDB grid = _views[SelectedTabName];

            if (mySelectedButton != null && mySelectedButton != InactiveRadio)
                mySelectedButton.Checked = false;
            
            _definitionMode = DefinitionMode.None;
            grid.ClearSelection();
            UpdateSelectionMode(grid, DataGridViewSelectionMode.CellSelect);            
            _lastChecked = null;

            foreach(RadioButton radio in _radios)
            {
                if(radio != InactiveRadio)
                    radio.Checked = false;
            }

            if (!InactiveRadio.Checked)
                InactiveRadio.Checked = true;

            SetSelectColorForAllColumns(grid);
            UpdatePaintControlsFromDefinitionMode(grid);

            UpdateSelectedCell(grid);

            grid.Invalidate();
        }

        private static void UpdateSelectionMode(DataGridViewDB grid, DataGridViewSelectionMode mode)
        {
            if (IsGridOff(grid))
                return;

            grid.SelectionMode = mode;            

            if (mode == DataGridViewSelectionMode.CellSelect)
            {
                grid.ReadOnly = false;
                grid.MultiSelect = false;
            }
            else
            {
                grid.MultiSelect = true;
                grid.ReadOnly = true;
            }
                
        }

        private void ColumnHeader_Clicked(object sender, EventArgs e)
        {
            /*if (_definitionMode != DefinitionMode.None)
            {
                var grid = (DataGridViewDB)sender;
                this.SetColumnData(grid);
                _activeGrid = grid;
            }*/
        }

        private void Cell_Clicked(object sender, DataGridViewCellEventArgs e)
        {
            /*if (_definitionMode != DefinitionMode.None)
            {
                var grid = (DataGridViewDB)sender;
                this.SetColumnData(grid);
                _activeGrid = grid;
            }*/
        }

        private void SetColumnColorsFromProject(string myName, DataGridViewDB myGrid)
        {
            if (Project != null)
            {
                Project proj = Project;
                DataSource source = Project.Sources[myName];

                if (source == null)
                    return;

                var allColumns = new List<int>();

                for(int i = 0; i < myGrid.Columns.Count; i++)
                    allColumns.Add(i);

                ColorColumns(myGrid, allColumns, GetClearColor(myGrid));

                ColorColumns(myGrid, source.IDColumnRange, IDColor);
                ColorColumns(myGrid, source.MetaDataColumnRange, MetaColor);
                ColorColumns(myGrid, source.FlawColumnRange, FlawColor);
                ColorColumns(myGrid, source.DataColumnRange, ResponseColor);

                UpdateSelectedCell(myGrid);
            }
        }

        private void UpdateSelectedCell(DataGridViewDB grid)
        {
            if (grid == null)
                return;

            if (grid.CurrentCell == null)
                return;

            if (_definitionMode == DefinitionMode.None && !IsGridOff(grid))
            {
                grid.CurrentCell.Selected = true;
            }
            //else
            //{
            //    if(grid.CurrentCell != null)
            //        grid.CurrentCell = null;// grid.Rows[0].Cells[0];
            //}
        }

        private void ColorColumns(DataGridViewDB myGrid, List<int> myRange, Color myColor)
        {
            foreach (var col in myRange)
            {
                if (col < myGrid.Columns.Count)
                {
                    DataGridViewColumn column = myGrid.Columns[col];

                    column.DefaultCellStyle.BackColor = myColor;
                    column.DefaultCellStyle.SelectionBackColor = myColor;
                }
                
            }
        }

        private Color GetColorFromDefinitionMode(DataGridViewDB grid)
        {
            return GetColorFromDefinitionMode(grid, _definitionMode);
        }

        private Color GetTextColorFromDefinitionMode(DataGridViewDB grid)
        {
            return GetTextColorFromDefinitionMode(grid, _definitionMode);
        }

        private void ForceColumnData(DataGridViewDB grid)
        {
            if (grid.SelectedColumns.Count <= 0)
            {
                return;
            }

            Color bgColor = GetColorFromDefinitionMode(grid);
            
            GetIndicesFromDefinitionMode(grid);

            _maxIndex = Int32.MinValue;
            _minIndex = Int32.MaxValue;

            foreach (DataGridViewColumn column in grid.SelectedColumns)
            {
                column.DefaultCellStyle.BackColor = bgColor;
                column.DefaultCellStyle.SelectionBackColor = bgColor;

                if (column.Index > _maxIndex)
                    _maxIndex = column.Index;

                if (column.Index < _minIndex)
                    _minIndex = column.Index;


            }

            grid.Invalidate();



        }

        private void UpdateMinxMax(DataGridViewDB grid)
        {
            if (grid.SelectedColumns.Count <= 0)
            {
                return;
            }

            if (MovedOutside == true)
            {
                _maxIndex = -1;
                _minIndex = -1;
            }
            else
            {
                _maxIndex = Int32.MinValue;
                _minIndex = Int32.MaxValue;

                foreach (DataGridViewColumn column in grid.SelectedColumns)
                {
                    if (column.Index > _maxIndex)
                        _maxIndex = column.Index;

                    if (column.Index < _minIndex)
                        _minIndex = column.Index;
                }
            }

            grid.Invalidate();
            
            /*for(int i = _minIndex; i <= _maxIndex; i++)
            {
                grid.InvalidateColumn(i);
            }*/
        }

        private void SetColumnData(DataGridViewDB grid)
        {
            if (grid.SelectedColumns.Count <= 0)
            {
                return;
            }            

            Color bgColor = GetColorFromDefinitionMode(grid);

            //GetIndicesFromDefinitionMode(grid);

            if (bgColor == grid.Columns[_firstMouseOver].DefaultCellStyle.BackColor)
            {
                bgColor = IncrementDefinitionMode(grid, _definitionMode);
            }

            GetIndicesFromDefinitionMode(grid);

            _maxIndex = Int32.MinValue;
            _minIndex = Int32.MaxValue;

            foreach (DataGridViewColumn column in grid.SelectedColumns)
            {
                column.DefaultCellStyle.BackColor = bgColor;
                column.DefaultCellStyle.SelectionBackColor = bgColor;                

                if (column.Index > _maxIndex)
                    _maxIndex = column.Index;

                if (column.Index < _minIndex)
                    _minIndex = column.Index;
                
            }

            GridCheckForProblems(grid);            
        }

        private void SetColumnDataLight(DataGridViewDB grid)
        {
            Color bgColor = GetColorFromDefinitionMode(grid);

            _tempMode = _definitionMode;

            if (bgColor == grid.Columns[_firstMouseOver].DefaultCellStyle.BackColor)
            {
                bgColor = IncrementDefinitionMode(grid, _definitionMode);
            }

            foreach (DataGridViewColumn column in grid.Columns)
            {
                column.DefaultCellStyle.SelectionBackColor = bgColor;
            }

            UpdateBucketCursorColor(grid);
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            int x = e.X;
            int y = e.Y;

            var grid = (DataGridViewDB)sender;

            var ht = grid.HitTest(x, y);

            UpdateCursorBasedOnMouseLocation(x, y, grid);

            if (e.Button == MouseButtons.Left && ClickedInside)
            {
                //if moved off of the grid, they aren't selected anymore
                if (ht.Type == DataGridViewHitTestType.None)
                {
                    foreach (DataGridViewColumn column in grid.Columns)
                    {
                        column.DefaultCellStyle.SelectionBackColor = column.DefaultCellStyle.BackColor;
                    }

                    MovedOutside = true;

                    var tempDef = _definitionMode;
                    _definitionMode = _tempMode;

                    UpdateBucketCursorColor(grid);
                    UpdatePaintControlsFromDefinitionMode(grid);                   

                    _definitionMode = tempDef;

                    grid.Invalidate();
                }
                else
                {
                    if (MovedOutside)
                    {
                        foreach (DataGridViewColumn column in grid.Columns)
                        {
                            column.DefaultCellStyle.SelectionBackColor = GetColorFromDefinitionMode(grid);
                        }
                        
                        MovedInside = true;

                        UpdatePaintControlsFromDefinitionMode(grid);
                        UpdateBucketCursorColor(grid);

                        grid.Invalidate();
                    }
                }

                UpdateMinxMax(grid);
            }
        }

        private void UpdateCursorBasedOnMouseLocation(int x, int y, DataGridViewDB grid)
        {
            if (grid == null)
                return;

            var ht = grid.HitTest(x, y);

            if (ht.Type == DataGridViewHitTestType.Cell || ht.Type == DataGridViewHitTestType.ColumnHeader)
            {
                var insideColor = grid.Columns[ht.ColumnIndex].DefaultCellStyle.BackColor;
                var newColor = Color.Black;

                if (insideColor == GetColorFromDefinitionMode(grid))
                {
                    if (!_shiftDown)
                        UpdateBucketCursorColor(grid, GetNextColorFromDefinitionMode(grid));
                    else
                        UpdateBucketCursorColor(grid, GetPreviousColorFromDefinitionMode(grid));
                }
                else
                {
                    UpdateBucketCursorColor(grid);
                }

                grid.Cursor = _paintBucket;
            }
            else
            {
                if (grid.Cursor != Cursors.Default)
                    grid.Cursor = Cursors.Default;
            }
        }

        

        private void Grid_MouseDown(object sender, MouseEventArgs e)
        {
            var grid = (DataGridViewDB)sender;
            var tabPage = grid.Parent as TabPage;

            if(tabPage != null && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                var ht = grid.HitTest(e.X, e.Y);

                if (ht.Type != DataGridViewHitTestType.None &&  !CanSourceBeEdited(tabPage))
                {
                    var result = AskUserToAddDuplicateGrid(grid, ref tabPage);

                    IsGridOff(grid);

                    _addedNew = result;

                    return;
                }
                else if(IsGridOff(grid))
                {
                    TurnOnGridForTheUser(grid);
                }           

                

                if (ht.Type == DataGridViewHitTestType.Cell || ht.Type == DataGridViewHitTestType.ColumnHeader)
                {
                    _firstMouseOver = ht.ColumnIndex;
                    ClickedInside = true;

                    this.SetColumnDataLight(grid);
                    _activeGrid = grid;
                }
                else
                {
                    ClickedOutside = true;
                }

                UpdateSelectedCell(grid);

                UpdateMinxMax(grid);
            }

        }

        private bool AskUserToAddDuplicateGrid(DataGridViewDB grid, ref TabPage tabPage)
        {
            var name = tabPage.Text;
            var source = Project.Sources[name];
            var result = true;
            var oldName = name;

            var response = MessageBox.Show(_defaultInUseMessage + "\n\nWould you like to duplicate this data source to create an editable version?", "POD v4 Notice", MessageBoxButtons.YesNo);

            if (response == DialogResult.Yes)
            {
                var newName = AddNewSourcePage(true);

                AddTabsFromList();

                var newView = _views[newName];
                tabPage = newView.Parent as PODTabPage;
                var allNames = GetAllTabNamesExceptCurrent(tabPage);

                var newNameForm = new NewSourceForm(tabPage.Text, allNames);

                newNameForm.ShowDialog();

                newName = UpdateDictionaries(newName, newNameForm.NewName);

                tabPage.Text = newName;

                if (newView != null)
                {
                    UpdateSelectionMode(newView, DataGridViewSelectionMode.CellSelect);
                    newView.Table = (DataTable)source.Original.Copy();
                    UpdateView(newName);

                    //for each existing index set
                    foreach (var indicies in _allIndexes)
                    {
                        //look for the page we are copying
                        if (indicies.ContainsKey(oldName))
                        {
                            var currentSet = indicies[oldName];                            

                            //if the page were copying has a set
                            if (currentSet != null)
                            {
                                //copy the set
                                var setCopy = new SortedSet<int>(currentSet);

                                //
                                if (indicies.ContainsKey(newName))
                                {
                                    indicies[newName] = setCopy;
                                }
                                else
                                {
                                    indicies.Add(newName, setCopy);
                                }
                            }
                                
                        }
                    }

                    var currentInfo = Project.Infos.GetFromNewName(name);

                    Project.Infos.AddCopiedInfo(currentInfo, newName);

                    TurnOnGridForTheUser(newView);
                    GridCheckForProblems(newView);
                    SetSelectColorForAllColumns(newView);
                    UpdateSelectedCell(newView);

                    DataSourceTabs.SelectedTab = DataSourceTabs.TabPages[DataSourceTabs.TabPages.Count - 1];
                }

                

            }
            else
            {
                result = false;
            }

            return result;
        }

        private string UpdateDictionaries(string oldName, string newName)
        {
            //var index = -1;

            if (oldName == newName)
                return newName;

            if(_views.ContainsKey(oldName))
            {
                var view = _views[oldName];
                _views.Remove(oldName);

                if (!_views.ContainsKey(newName))
                    _views.Add(newName, view);
                else
                    _views[newName] = view;
            }

            foreach(var indices in _allIndexes)
            {
                if(indices.ContainsKey(oldName))
                {
                    var set = indices[oldName];
                    indices.Remove(oldName);

                    if (!indices.ContainsKey(newName))
                        indices.Add(newName, set);
                    else
                        indices[newName] = set;
                }
            }

            if(_tableBackups.ContainsKey(oldName))
            {
                var table = _tableBackups[oldName];
                _tableBackups.Remove(oldName);

                if (!_tableBackups.ContainsKey(newName))
                    _tableBackups.Add(newName, table);
                else
                    _tableBackups[newName] = table;
            }

            return newName;
   
        }

        private void UpdateDictionariesRemove(string nameToDelete)
        {
            if (_views.ContainsKey(nameToDelete))
            {                
                _views.Remove(nameToDelete);
            }

            foreach (var indices in _allIndexes)
            {
                if (indices.ContainsKey(nameToDelete))
                {
                    indices.Remove(nameToDelete);
                }
            }

            if (_tableBackups.ContainsKey(nameToDelete))
            {
                _tableBackups.Remove(nameToDelete);
            }
        }

        private bool CanSourceBeEdited(TabPage tabPage)
        {
            var isNotASource = Project.Sources[tabPage.Text] == null;

            if (isNotASource)
            {
                return true;
            }
            else
            {
                var hasAnalyses = _currentAnalyses.UsingDataSource(tabPage.Text);

                if (hasAnalyses)
                {
                    TurnOffGridToTheUser(_views[tabPage.Text]);

                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        private void TurnOnGridForTheUser(DataGridViewDB grid)
        {
            SetColumnSortModeAndColor(grid.Columns);
            SetColumnColorsFromIndices(grid);
            SetColumnsSelectMode(grid, _definitionMode);

            UpdatePaintControlsFromDefinitionMode(grid);
        }

        private static bool IsGridOff(DataGridViewDB grid)
        {
            var result = (grid.SelectionMode == DataGridViewSelectionMode.RowHeaderSelect && 
                          grid.MultiSelect == false &&
                          grid.ReadOnly == true);

            if(result)
                grid.ClearSelection();

            return result;
        }

        private void TurnOffGridToTheUser(DataGridViewDB grid)
        {
            grid.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
            grid.MultiSelect = false;
            grid.ReadOnly = true;
            SetColumnColorsFromIndices(grid);
        }

        private List<string> GetAllTabNamesExceptCurrent(TabPage tabPage)
        {
            var allNames = new List<string>();

            foreach (TabPage page in DataSourceTabs.TabPages)
            {
                if (page != tabPage)
                    allNames.Add(page.Text);
            }

            //can't name to old name either
            foreach (TabPage page in _removedSources)
            {
                allNames.Add(page.Text);
            }

            return allNames;
        }



        private void Grid_MouseUp(object sender, MouseEventArgs e)
        {
            var grid = (DataGridViewDB)sender;

            if (IsGridOff(grid))
                return;

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {

                var ht = grid.HitTest(e.X, e.Y);

                if (ClickedInside)
                    _definitionMode = _tempMode;

                if (ht.Type != DataGridViewHitTestType.None)
                {
                    if (_definitionMode != DefinitionMode.None)
                    {
                        this.SetColumnData(grid);
                        _activeGrid = grid;
                        ClickedInside = false;
                    }
                }
                else
                {
                    UpdatePaintControlsFromDefinitionMode(grid);
                    UpdateBucketCursorColor(grid);
                }

                UpdateSelectedCell(grid);

                UpdateMinxMax(grid);
            }
        }

        private void ReloadAllIndicies()
        {
            foreach(DataGridViewDB grid in _views.Values)
            {
                BuildIndicesFromTable(grid);

                foreach(DataGridViewColumn col in grid.Columns)
                {
                    col.SortMode = DataGridViewColumnSortMode.NotSortable;
                }
            }

            
        }

        private void GetIndicesFromDefinitionMode(DataGridViewDB grid)
        {
            switch (_definitionMode)
            {
                case DefinitionMode.ID:
                    this.GetIndicesFromTable(grid, _specIdIndex);
                    break;
                case DefinitionMode.MetaData:
                    this.GetIndicesFromTable(grid, _metadataIndex);
                    break;
                case DefinitionMode.Flaw:
                    this.GetIndicesFromTable(grid, _flawIndex);
                    break;
                case DefinitionMode.Response:
                    this.GetIndicesFromTable(grid, _responseIndices);
                    break;
                case DefinitionMode.ClearOne:
                    this.GetIndicesFromTable(grid, null);
                    break;
                default:
                    break;
            }
        }

        private Color IncrementDefinitionMode(DataGridViewDB grid, DefinitionMode myDefinitionMode)
        {
            

            if(!_shiftDown)
            {
                _definitionMode = GetNextDefinition(myDefinitionMode);
            }
            else
            {
                _definitionMode = GetPreviousDefinition(myDefinitionMode);
            }

            Color bgColor = GetColorFromDefinitionMode(grid);            
            UpdatePaintControlsFromDefinitionMode(grid);
            SetSelectColorForAllColumns(grid);

            return bgColor;
        }

        private DefinitionMode GetPreviousDefinition(DefinitionMode myDefinitionMode)
        {
            var newDefinition = myDefinitionMode;

            switch (myDefinitionMode)
            {
                case DefinitionMode.ID:
                    newDefinition = DefinitionMode.ClearOne; ;
                    break;
                case DefinitionMode.MetaData:
                    newDefinition = DefinitionMode.ID;
                    break;
                case DefinitionMode.Flaw:
                    newDefinition = DefinitionMode.MetaData;
                    break;
                case DefinitionMode.Response:
                    newDefinition = DefinitionMode.Flaw;
                    break;
                case DefinitionMode.ClearOne:
                    newDefinition = DefinitionMode.Response;
                    break;
                default:
                    break;
            }
            return newDefinition;
        }

        private DefinitionMode GetNextDefinition(DefinitionMode myDefinitionMode)
        {
            var newDefinition = myDefinitionMode;

            switch (myDefinitionMode)
            {
                case DefinitionMode.ID:
                    newDefinition = DefinitionMode.MetaData;
                    break;
                case DefinitionMode.MetaData:
                    newDefinition = DefinitionMode.Flaw;
                    break;
                case DefinitionMode.Flaw:
                    newDefinition = DefinitionMode.Response;
                    break;
                case DefinitionMode.Response:
                    newDefinition = DefinitionMode.ClearOne;
                    break;
                case DefinitionMode.ClearOne:
                    newDefinition = DefinitionMode.ID;
                    break;
                default:
                    break;
            }
            return newDefinition;
        }

        private void SetSelectColorForAllColumns(DataGridViewDB grid)
        {
            var color = GetColorFromDefinitionMode(grid);
            var textColor = GetTextColorFromDefinitionMode(grid);

            foreach (DataGridViewColumn column in grid.Columns)
            {
                column.DefaultCellStyle.SelectionBackColor = color;
                column.DefaultCellStyle.SelectionForeColor = textColor;
            }            
        }

        private void UpdatePaintControlsFromDefinitionMode(DataGridViewDB grid)
        {
            RadioButton button = null;
            Panel colorPanel = null;

            //if (IsGridOff(grid))
            //    //ChangeToInactive(_lastChecked);
            //    _definitionMode = DefinitionMode.None;

            SetColumnsSelectMode(grid, _definitionMode);
            
            switch (_definitionMode)
            {
                case DefinitionMode.MetaData:
                    button = DefineMetadataRadio;
                    colorPanel = MetaDataColorPnl;
                    break;
                case DefinitionMode.Flaw:
                    button = DefineFlawRadio;
                    colorPanel = FlawColorPnl;
                    break;
                case DefinitionMode.Response:
                    button = DefineResponseRadio;
                    colorPanel = ResponseColorPnl;
                    break;
                case DefinitionMode.ClearOne:
                    button = UndefineRadio;
                    colorPanel = UndefineColorPnl;
                    break;
                case DefinitionMode.ID:
                    button = DefineIDRadio;
                    colorPanel = IDColorPnl;
                    break;
                case DefinitionMode.None:
                    button = InactiveRadio;
                    colorPanel = null;
                    break;
                default:
                    break;
            }

            foreach (RadioButton control in _radios)
            {
                if (control != button)
                {
                    control.Parent.BackColor = Color.FromKnownColor(KnownColor.Control);
                    control.Checked = false;
                }
            }

            button.Checked = true;
            _lastChecked = button;
            button.Parent.BackColor = GetControlColorFromDefinitionMode(grid);
            button.Parent.Parent.Invalidate();

            foreach (Panel panel in _colorPanels)
            {
                if(panel != colorPanel)
                    panel.BackgroundImage = null;
            }

            if(colorPanel != null)
            {
                colorPanel.BackgroundImage = Properties.Resources.paint_bucket;
                colorPanel.BackgroundImageLayout = ImageLayout.Stretch;
            }
            else
            {
                InactiveColorPnl.BackgroundImage = Properties.Resources.pencil;
            }

            UpdateBucketCursorColor(grid);

            if (_definitionMode != DefinitionMode.None)
                grid.Cursor = _paintBucket;
            else
                grid.Cursor = Cursors.Default;
        }

        private void SetColumnsSelectMode(DataGridViewDB grid, DefinitionMode definitionMode)
        {
            if (IsGridOff(grid))
                return;

            var mode = grid.SelectionMode;

            switch (definitionMode)
            {
                case DefinitionMode.MetaData:
                    mode = DataGridViewSelectionMode.FullColumnSelect;
                    break;
                case DefinitionMode.Flaw:
                    mode = DataGridViewSelectionMode.FullColumnSelect;
                    break;
                case DefinitionMode.Response:
                    mode = DataGridViewSelectionMode.FullColumnSelect;
                    break;
                case DefinitionMode.ClearOne:
                    mode = DataGridViewSelectionMode.FullColumnSelect;
                    break;
                case DefinitionMode.ID:
                    mode = DataGridViewSelectionMode.FullColumnSelect;
                    break;
                case DefinitionMode.None:
                    mode = DataGridViewSelectionMode.CellSelect;
                    break;
                default:
                    break;
            }

            grid.SelectionMode = mode;

            UpdateSelectionMode(grid, mode);
        }

        private Color GetControlColorFromDefinitionMode(DataGridViewDB grid)
        {
            Color bgColor = GetColorFromDefinitionMode(grid);
            //
            //
            //
            //if (bgColor == grid.DefaultCellStyle.BackColor)
            //    bgColor = Color.FromKnownColor(KnownColor.Control);

            if(_definitionMode == DefinitionMode.None)
                bgColor = Color.FromKnownColor(KnownColor.Control);

            return bgColor;
        }

        void grid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void Cell_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if(!_views.ContainsKey(SelectedTabName))
            {
                e.Handled = false;
                return;
            }

            var grid = _views[SelectedTabName];

            if (grid == sender as DataGridViewDB)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                if (e.RowIndex >= 0 && grid.Rows.Count > 0 && grid.Columns.Count > 0)
                {
                    var cell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    Color currentColor = cell.Style.BackColor;
                    bool marked = MarkUpCell(grid, cell, e.RowIndex, e.ColumnIndex, e.Graphics, e.CellBounds);

                    if (marked)
                    {
                        e.Paint(e.CellBounds, DataGridViewPaintParts.ContentForeground);
                    }
                }

                

                if (_maxIndex == Int32.MinValue && _minIndex == Int32.MaxValue || _definitionMode == DefinitionMode.None)
                {
                    e.Handled = true;
                    return;
                }


                if (e.RowIndex >= -1)
                {
                    if (e.ColumnIndex == _minIndex && e.ColumnIndex == _maxIndex)
                    {
                        DrawLeftSelectLine(e);
                        DrawRightSelectLine(e);
                    }
                    else if (e.ColumnIndex == _minIndex)
                    {
                        DrawLeftSelectLine(e);

                        e.Handled = true;
                    }
                    else if (e.ColumnIndex == _maxIndex)
                    {
                        DrawRightSelectLine(e);
                    }                    
                }

                //if (e.RowIndex == 0 && e.ColumnIndex >= _minIndex && e.ColumnIndex <= _maxIndex)
                //{
                //    DrawTopSelectLine(e);
                //}

                if (e.RowIndex == -1 && e.ColumnIndex >= _minIndex && e.ColumnIndex <= _maxIndex)
                {
                    DrawTopSelectLine(e);

                    DrawBottomSelectLine(e);
                }

                e.Handled = true;
            }
            
        }

        private void DrawTopSelectLine(DataGridViewCellPaintingEventArgs e)
        {
            var p = new Pen(Color.Black, 3);
            var pBG = new Pen(Color.White, 5);
            var rightOffseBlackt = 0;
            var rightOffsetWhite = 0;
            var leftOffsetWhite = 0;
            
            if(e.ColumnIndex == _maxIndex)
            {
                rightOffseBlackt = 1;
                rightOffsetWhite = 4;
            }

            if (e.ColumnIndex == _minIndex)
            {
                leftOffsetWhite = 2;
            }

            //top side
            e.Graphics.DrawLine(pBG, e.CellBounds.X + leftOffsetWhite, e.CellBounds.Y, e.CellBounds.X + e.CellBounds.Width - rightOffsetWhite, e.CellBounds.Y);
            e.Graphics.DrawLine(p, e.CellBounds.X, e.CellBounds.Y, e.CellBounds.X + e.CellBounds.Width - rightOffseBlackt, e.CellBounds.Y);
        }

        private void DrawBottomSelectLine(DataGridViewCellPaintingEventArgs e)
        {
            var p = new Pen(Color.Black, 3);
            var pBG = new Pen(Color.White, 5);
            var rightOffseBlackt = 0;
            var rightOffsetWhite = 0;
            var leftOffsetWhite = 0;
            var newHeight = e.CellBounds.Y + e.CellBounds.Height - 3;

            if (e.ColumnIndex == _maxIndex)
            {
                rightOffseBlackt = 1;
                rightOffsetWhite = 4;
            }

            if (e.ColumnIndex == _minIndex)
            {
                leftOffsetWhite = 2;
            }

            //top side
            e.Graphics.DrawLine(pBG, e.CellBounds.X + leftOffsetWhite, newHeight, e.CellBounds.X + e.CellBounds.Width - rightOffsetWhite, newHeight);
            e.Graphics.DrawLine(p, e.CellBounds.X, newHeight, e.CellBounds.X + e.CellBounds.Width - rightOffseBlackt, newHeight);
        }

        private static void DrawRightSelectLine(DataGridViewCellPaintingEventArgs e)
        {
            var p = new Pen(Color.Black, 3);
            var pBG = new Pen(Color.White, 5);

            //right side
            e.Graphics.DrawLine(pBG, e.CellBounds.X + e.CellBounds.Width - 3, e.CellBounds.Y, e.CellBounds.X + e.CellBounds.Width - 3, e.CellBounds.Y + e.CellBounds.Height);
            e.Graphics.DrawLine(p, e.CellBounds.X + e.CellBounds.Width - 3, e.CellBounds.Y, e.CellBounds.X + e.CellBounds.Width - 3, e.CellBounds.Y + e.CellBounds.Height);
        }

        private static void DrawLeftSelectLine(DataGridViewCellPaintingEventArgs e)
        {
            var p = new Pen(Color.Black, 3);
            var pBG = new Pen(Color.White, 5);

            //left side
            e.Graphics.DrawLine(pBG, e.CellBounds.X, e.CellBounds.Y, e.CellBounds.X, e.CellBounds.Y + e.CellBounds.Height);
            e.Graphics.DrawLine(p, e.CellBounds.X, e.CellBounds.Y, e.CellBounds.X, e.CellBounds.Y + e.CellBounds.Height);
        }

        private void GridCheckForProblems(DataGridViewDB myGrid, int myRowIndex, int myColIndex, bool causeInvalidate)
        {
            if (myGrid == null)
                return;

            if (myRowIndex > myGrid.Rows.Count-1 || myColIndex > myGrid.Columns.Count-1)
                return;

            DataGridViewColumn col = myGrid.Columns[myColIndex];
            DataGridViewRow row = myGrid.Rows[myRowIndex];
            DataGridViewCell cell = row.Cells[myColIndex];            

            DefinitionMode mode = GetDefinitionModeFromColor(myGrid, col.DefaultCellStyle.BackColor);

            var colTag = InitializeColumnTag(col, mode);
            var rowTag = InitializeRowTag(row, RowStatus.Valid);
            var cellTag = InitializeCellTag(cell, CellStatus.Valid);

            var status = UpdateCellStatus(mode, cell);

            cellTag.Status = status;

            var rowStatus = CheckRowStatus(myGrid, row);

            //cell.ToolTipText = GetCellToolTip(status, rowStatus);

            if (cell == myGrid.CurrentCell)
                StepToolTip.SetToolTip(myGrid, GetCellToolTip(status, rowStatus));

            if (causeInvalidate)
            {
                myGrid.InvalidateCell(cell);
                myGrid.InvalidateRow(myRowIndex);

                if (myRowIndex > 0)
                    myGrid.InvalidateRow(myRowIndex - 1);
            }
        }

        private void GridCheckForProblems(DataGridViewDB myGrid)
        {
            if (myGrid == null)
                return;

            //first update the columns that changed
            foreach(DataGridViewColumn col in myGrid.Columns)
            {
                if(myGrid.Rows.Count > 0)
                {
                    int index = myGrid.Columns.IndexOf(col);

                    DefinitionMode mode = GetDefinitionModeFromColor(myGrid, col.DefaultCellStyle.BackColor);
                    CellStatus status = CellStatus.Valid;

                    InitializeColumnTag(col, mode);

                    foreach(DataGridViewRow row in myGrid.Rows)
                    {
                        if (row.Index == myGrid.Rows.Count - 1)
                            continue;

                        var cell = row.Cells[index];
                        var rowStatus = RowStatus.Valid;

                        status = UpdateCellStatus(mode, cell);

                        InitializeCellTag(cell, status);

                        //if at the last column process the row for row errors
                        if(index == myGrid.Columns.Count - 1)
                        {
                            rowStatus = CheckRowStatus(myGrid, row);
                        }

                        //cell.ToolTipText = GetCellToolTip(status, rowStatus);

                        if (cell == myGrid.CurrentCell)
                            StepToolTip.SetToolTip(myGrid, GetCellToolTip(status, rowStatus));
                    }
                }
            }

            ////myGrid.Invalidate();
        }

        private static RowStatus CheckRowStatus(DataGridViewDB myGrid, DataGridViewRow row)
        {
            var rowTag = InitializeRowTag(row, RowStatus.Valid);
            var checkIndex = 0;

            rowTag.Status = RowStatus.Valid;

            foreach (DataGridViewColumn checkCol in myGrid.Columns)
            {
                var tag = row.Cells[checkIndex].Tag as CellTag;
                var checkStatus = CellStatus.Valid;

                if(tag != null)
                    checkStatus = tag.Status;

                if (checkStatus == CellStatus.InvalidWholeRow)
                {
                    rowTag.Status = RowStatus.Invalid;
                    break;
                }

                checkIndex++;
            }

            return rowTag.Status;
        }

        private static CellStatus UpdateCellStatus(DefinitionMode mode, DataGridViewCell cell)
        {
            CellStatus status = CellStatus.Valid;

            if (mode == DefinitionMode.Response || mode == DefinitionMode.Flaw)
            {
                object cellValue = cell.Value;
                double value = 0.0;


                if (cellValue != null)
                {
                    string cellValueString = cellValue.ToString().Trim(); ;

                    if (!Double.TryParse(cellValueString, out value) || cellValueString == "")
                    {
                        if (mode != DefinitionMode.Flaw)
                            status = CellStatus.Invalid;
                        else
                        {
                            status = CellStatus.InvalidWholeRow;
                        }
                    }
                    else
                    {
                        if (mode == DefinitionMode.Flaw && value <= 0.0)
                            status = CellStatus.InvalidWholeRow;
                    }
                }
                if(cellValue == null)
                {
                    if (mode != DefinitionMode.Flaw)
                        status = CellStatus.Invalid;
                    else
                    {
                        status = CellStatus.InvalidWholeRow;
                    }
                }
            }
            return status;
        }

        private static CellTag InitializeCellTag(DataGridViewCell cell, CellStatus status)
        {
            var tag = cell.Tag as CellTag;

            if (tag != null)
                tag.Status = status;
            else
            {
                tag = new CellTag(status);
                cell.Tag = tag;
            }

            return tag;
        }

        private static RowTag InitializeRowTag(DataGridViewRow row, RowStatus status)
        {
            var tag = row.Tag as RowTag;

            if (tag != null)
                tag.Status = status;
            else
            {
                tag = new RowTag(status);
                row.Tag = tag;
            }

            return tag;
        }

        private static ColTag InitializeColumnTag(DataGridViewColumn col, DefinitionMode mode)
        {
            var tag = col.Tag as ColTag;

            if (tag != null)
                tag.Mode = mode;
            else
            {
                tag = new ColTag(mode);
                col.Tag = tag;
            }

            return tag;
        }

        private bool MarkUpCell(DataGridViewDB grid, DataGridViewCell cell, int rowIndex, int colIndex, Graphics graphics, Rectangle cellBounds)
        {
            bool marked = false;
            CellStatus cellStatus = CellStatus.Valid;
            RowStatus rowStatus = RowStatus.Valid;
            DefinitionMode colMode = DefinitionMode.None;

            if (cell.Value == DBNull.Value && rowIndex < grid.Rows.Count - 1)
                GridCheckForProblems(grid, rowIndex, colIndex, false);

            var row = grid.Rows[rowIndex];
            var col = grid.Columns[colIndex];

            var cellTag = cell.Tag as CellTag;
            var rowTag = row.Tag as RowTag;
            var colTag = col.Tag as ColTag;

            

            if(cellTag != null)
            {
                cellStatus = cellTag.Status;
            }

            if(rowTag != null)
            {
                rowStatus = rowTag.Status;
            }
            
            if(colTag != null)
            {
                colMode = colTag.Mode;
            }

            Color backColor = col.DefaultCellStyle.BackColor;

            if(col.Selected)
                backColor = col.DefaultCellStyle.SelectionBackColor;

            Brush backBrush = new SolidBrush(backColor);

            //if not a undefined column
            if (backColor != SystemColors.Window && backColor.A != 0)
            {                
                if (IsCellNotCurrent(grid, cell))
                {
                    //draw colored background
                    var wholeRectanlge = new Rectangle(cellBounds.X, cellBounds.Y, cellBounds.Width - 1, cellBounds.Height);
                    graphics.FillRectangle(backBrush, wholeRectanlge);
                    marked = true;
                }
                else if (_definitionMode != DefinitionMode.None)
                {
                    //draw colored line at bottom to allow select to show through
                    var wholeRectanlge = new Rectangle(cellBounds.X, cellBounds.Y + cellBounds.Height - 1, cellBounds.Width - 1, 1);
                    graphics.FillRectangle(backBrush, wholeRectanlge);
                }
            }

            if (cellStatus != CellStatus.Valid || rowStatus != RowStatus.Valid || rowIndex == grid.RowCount - 1)
            {
                var errorImage = CellIcons.Images[0];
                var rowErrorImage = CellIcons.Images[1];
                var heightDiff = (cellBounds.Height - errorImage.Height) / 2;
                var buffer = 5;
                var errorWidth = errorImage.Width + buffer;
                var finalPoint = new Rectangle(cellBounds.X + cellBounds.Width - errorWidth, cellBounds.Y + heightDiff, cellBounds.Width, cellBounds.Height - heightDiff);
                var smallerRectanlge = new Rectangle(cellBounds.X, cellBounds.Y, cellBounds.Width - (errorWidth + buffer), cellBounds.Height - 1);
                var biggerRectanlge = new Rectangle(cellBounds.X, cellBounds.Y, cellBounds.Width - 1, cellBounds.Height - 1);      

                //if cell is invalid within valid row or cell is causing row to be invalid                
                if ((cellStatus == CellStatus.Invalid && rowStatus == RowStatus.Valid) || cellStatus == CellStatus.InvalidWholeRow)
                {
                    //if not the current cell
                    if (IsCellNotCurrent(grid, cell) || _definitionMode != DefinitionMode.None)
                    {
                        //draw white small white background with grey line at bottom
                        graphics.FillRectangle(Brushes.White, smallerRectanlge);
                        var wholeRectanlge = new Rectangle(cellBounds.X, cellBounds.Y + cellBounds.Height - 1, cellBounds.Width - 1, 1);
                        graphics.FillRectangle(new SolidBrush(grid.BackgroundColor), wholeRectanlge);
                    }

                    //draw the icon
                    if(cellStatus == CellStatus.Invalid)
                        graphics.DrawImageUnscaled(errorImage, finalPoint);
                    else
                        graphics.DrawImageUnscaled(rowErrorImage, finalPoint);

                    marked = true;
                }
                //else if row is invalid but cell is valid
                else if (rowStatus == RowStatus.Invalid || rowIndex == grid.RowCount - 1)
                {
                    //if not the current cell
                    if (IsCellNotCurrent(grid, cell) || _definitionMode != DefinitionMode.None)
                    {
                        //draw a white background with a grey line at the bottom
                        graphics.FillRectangle(Brushes.White, biggerRectanlge);

                        var wholeRectanlge = new Rectangle(cellBounds.X, cellBounds.Y + cellBounds.Height - 1, cellBounds.Width - 1, 1);
                        graphics.FillRectangle(new SolidBrush(grid.BackgroundColor), wholeRectanlge);

                        marked = true;
                    }
                }
            }

            if (GetCellStatusBelow(grid, rowIndex, colIndex) != CellStatus.Valid || rowIndex == grid.Rows.Count-2)
            {
                var wholeRectanlge = new Rectangle(cellBounds.X, cellBounds.Y + cellBounds.Height - 1, cellBounds.Width - 1, 1);
                graphics.FillRectangle(new SolidBrush(grid.BackgroundColor), wholeRectanlge);
            }

            return marked;
        }

        private CellStatus GetCellStatusBelow(DataGridViewDB grid, int rowIndex, int colIndex)
        {
            rowIndex++;

            if (rowIndex >= grid.Rows.Count)
                return CellStatus.Valid;

            CellStatus cellStatus = CellStatus.Valid;
            RowStatus rowStatus = RowStatus.Valid;

            var row = grid.Rows[rowIndex];
            var col = grid.Columns[colIndex];
            var cell = row.Cells[colIndex];

            var cellTag = cell.Tag as CellTag;
            var rowTag = row.Tag as RowTag;
            var colTag = col.Tag as ColTag;

            if (cellTag != null)
            {
                cellStatus = cellTag.Status;
            }

            if (rowTag != null)
            {
                rowStatus = rowTag.Status;
            }

            if (cellStatus != CellStatus.Valid || rowStatus != RowStatus.Valid)
                cellStatus = CellStatus.Invalid;

            return cellStatus;
        }

        private static bool IsCellNotCurrent(DataGridViewDB grid, DataGridViewCell cell)
        {
            return grid.CurrentCell == null || grid.CurrentCell != cell;
        }

        private DefinitionMode GetDefinitionModeFromColor(DataGridViewDB grid, Color myColor)
        {
            DefinitionMode mode;

            
                if(myColor == IDColor)
                {
                    mode = DefinitionMode.ID;
                }
                else if(myColor == MetaColor)
                {
                    mode = DefinitionMode.MetaData;
                }
                else if(myColor == FlawColor)
                {
                    mode = DefinitionMode.Flaw;
                }
                else if(myColor == ResponseColor)
                {
                    mode = DefinitionMode.Response;
                }
                else if(myColor == GetClearColor(grid))
                {
                    mode = DefinitionMode.ClearOne;                    
                }
                else 
                {
                    mode = DefinitionMode.None;
                }


            return mode;
        }

        public static Color GetClearColor(DataGridViewDB grid)
        {
            return grid.DefaultCellStyle.BackColor;
        }

        public static Color IDColor
        {
            get
            {
                //nice blue
                return Color.FromArgb(204, 235, 255);
            }
        }

        public static Color MetaColor
        {
            get
            {
                //nice dark blue
                return Color.FromArgb(190, 198, 247);
                //return Color.FromArgb(255, 245, 201);
            }
        }

        public static Color FlawColor
        {
            get
            {
                //nice dark green
                return Color.FromArgb(84, 184, 96);
            }
        }

        public static Color ResponseColor
        {
            get
            {
                //nice green
                return Color.FromArgb(166, 237, 175);
                ////nice purple
                //return Color.FromArgb(214, 171, 255);
            }
        }

        private void BuildIndicesFromTable(DataGridViewDB grid)
        {

            GetIndicesFromTable(grid, _specIdIndex, GetIndiciesByType(grid, DefinitionMode.ID));
            GetIndicesFromTable(grid, _metadataIndex, GetIndiciesByType(grid, DefinitionMode.MetaData));
            GetIndicesFromTable(grid, _flawIndex, GetIndiciesByType(grid, DefinitionMode.Flaw));
            GetIndicesFromTable(grid, _responseIndices, GetIndiciesByType(grid, DefinitionMode.Response));
        }

        private SortedSet<int> GetIndiciesByType(DataGridViewDB grid, DefinitionMode definitionMode)
        {
            var indices = new SortedSet<int>();
            var color = GetColorFromDefinitionMode(grid, definitionMode);

            foreach (DataGridViewColumn column in grid.Columns)
            {
                if(column.DefaultCellStyle.BackColor == color)
                    indices.Add(column.Index);
            }

            if (indices.Count == 0)
                indices = null;

            return indices;
        }

        private Color GetColorFromDefinitionMode(DataGridViewDB grid, DefinitionMode myDefinitionMode)
        {
            Color bgColor;

            switch (myDefinitionMode)
            {
                case DefinitionMode.ID:
                    bgColor = IDColor;
                    break;
                case DefinitionMode.MetaData:
                    bgColor = MetaColor;
                    break;
                case DefinitionMode.Flaw:
                    bgColor = FlawColor;
                    break;
                case DefinitionMode.Response:
                    bgColor = ResponseColor;
                    break;
                case DefinitionMode.ClearOne:
                    bgColor = GetClearColor(grid);;
                    break;
                default:
                    bgColor = Color.FromKnownColor(KnownColor.Highlight);
                    break;
            }

            return bgColor;
        }

        private Color GetTextColorFromDefinitionMode(DataGridViewDB grid, DefinitionMode myDefinitionMode)
        {
            Color textColor;

            switch (myDefinitionMode)
            {
                case DefinitionMode.ID:
                    textColor = Color.FromKnownColor(KnownColor.WindowText);
                    break;
                case DefinitionMode.MetaData:
                    textColor = Color.FromKnownColor(KnownColor.WindowText);
                    break;
                case DefinitionMode.Flaw:
                    textColor = Color.FromKnownColor(KnownColor.WindowText);
                    break;
                case DefinitionMode.Response:
                    textColor = Color.FromKnownColor(KnownColor.WindowText);
                    break;
                case DefinitionMode.ClearOne:
                    textColor = Color.FromKnownColor(KnownColor.WindowText);
                    break;
                default:
                    textColor = Color.FromKnownColor(KnownColor.HighlightText);
                    break;
            }

            return textColor;
        }

        private void GetIndicesFromTable(DataGridViewDB grid, Dictionary<string, SortedSet<int>> indexDict, SortedSet<int> indices)
        {
            var viewIndex = GetNameFromGrid(grid);

            //if indexDict == null then you're just undefining columns

            //remove selected points
            if (indices != null)
            {              
                //remove from other lists before adding it to its new location
                foreach (Dictionary<string, SortedSet<int>> index in _allIndexes)
                {
                    if (index.ContainsKey(viewIndex) && index[viewIndex] != null)
                    {
                        index[viewIndex].ExceptWith(indices);

                        if (index[viewIndex].Count == 0)
                            index.Remove(viewIndex);
                    }
                }
            }
            
            //add the points back in
            if (indexDict != null && indices != null)
            {
                if (indexDict.ContainsKey(viewIndex) && indexDict[viewIndex] != null)
                {
                    indexDict[viewIndex].UnionWith(indices);
                }
                else
                {
                    indexDict[viewIndex] = indices;
                }
            }
        }

        private void GetIndicesFromTable(DataGridViewDB grid, Dictionary<string,SortedSet<int>> indexDict)
        {
            var indices = new SortedSet<int>();

            
            foreach (DataGridViewColumn column in grid.SelectedColumns)
            {         
                indices.Add(column.Index);
            }

            GetIndicesFromTable(grid, indexDict, indices);
        }

        private void ClearAll_Clicked(object sender, EventArgs e)
        {
            foreach (DataGridViewColumn column in _activeGrid.Columns)
            {
                column.DefaultCellStyle.BackColor = _activeGrid.DefaultCellStyle.BackColor;
                column.DefaultCellStyle.SelectionBackColor = _activeGrid.DefaultCellStyle.SelectionBackColor;
            }

            //remove all the indicies
            foreach (Dictionary<string, SortedSet<int>> index in _allIndexes)
            {
                if (index.ContainsKey(SelectedTabName))
                {
                    index[SelectedTabName].Clear();
                }
            }
        }

        private void CheckShift_Click(object sender, MouseEventArgs e)
        {
            _lastSelected = _activeGrid.SelectedColumns;
        }

        private void toolStripContainer1_TopToolStripPanel_Click(object sender, EventArgs e)
        {

        }

        private void Grid_Clicked(object sender, MouseEventArgs e)
        {
            var grid = (DataGridViewDB)sender;

            if (IsGridOff(grid))
                return;            

            var ht = grid.HitTest(e.X, e.Y);

            if (ht.Type == DataGridViewHitTestType.None)
            {
                grid.ClearSelection();

                _maxIndex = Int32.MinValue;
                _minIndex = Int32.MaxValue;

                UpdateSelectedCell(grid);

                grid.Invalidate();
            }
            
        }

        private void RestoreSource(PODTabPage restorePage)
        {
            if (_removedSources.Contains(restorePage) &&  _removedTrackConnectingNode.ContainsKey(restorePage))
            {
                var insertAt = 0;
                var before = _removedTrackConnectingNode[restorePage];

                if (before != null)
                {
                    var foundBefore = false;

                    while (foundBefore == false)
                    {
                        foreach (TabPage page in DataSourceTabs.TabPages)
                        {
                            if (page.Text == before.Text)
                            {
                                insertAt = DataSourceTabs.TabPages.IndexOf(page) + 1;
                                foundBefore = true;
                                break;
                            }
                        }

                        if (foundBefore == false)
                            before = _removedTrackConnectingNode[before];

                        if (before == null)
                        {
                            insertAt = 0;
                            foundBefore = true;
                        }
                    }


                }

                DataSourceTabs.SuspendDrawing();
                restorePage.SuspendDrawing();

                this.DataSourceTabs.TabPages.Insert(insertAt, restorePage);
                if (_views.ContainsKey(restorePage.Text))
                    _views[restorePage.Text] = GetGridFromPage(restorePage);
                else
                    _views.Add(restorePage.Text, GetGridFromPage(restorePage));

                DataSourceTabs.SelectedTab = restorePage;

                GridCheckForProblems(_views[restorePage.Text]);                

                restorePage.ResumeDrawing();
                DataSourceTabs.ResumeDrawing();
            }
        }



        private void cell_DoubleClicked(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void ColumnHeader_Clicked(object sender, DataGridViewCellMouseEventArgs e)
        {

        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
            {
                _shiftDown = true;
            }

            if(e.KeyCode == Keys.Space)
                _spaceDown = true;

            if(_spaceDown)
            {
                if (_definitionMode != DefinitionMode.None)
                {                
                    ChangeToInactive(_lastChecked);

                    SelectCellUnderMouse(sender as DataGridViewDB);
                }
            }

            RefreshCursor(sender as DataGridViewDB);
        }

        

        private void RefreshCursor(DataGridViewDB grid)
        {
            var pos = grid.PointToClient(Cursor.Position);

            int x = pos.X;
            int y = pos.Y;

            UpdateCursorBasedOnMouseLocation(x, y, grid);
        }

        private void SelectCellUnderMouse(DataGridViewDB grid)
        {
            var pos = grid.PointToClient(Cursor.Position);

            int x = pos.X;
            int y = pos.Y;

            var ht = grid.HitTest(x, y);

            if(ht.Type == DataGridViewHitTestType.Cell)
            {
                if(ht.RowIndex > 0 && ht.ColumnIndex >= 0)
                {
                    var cell = grid.Rows[ht.RowIndex].Cells[ht.ColumnIndex];
                    grid.CurrentCell = cell;
                }

                
            }
        }

        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
            {
                _shiftDown = false;
            }

            if (e.KeyCode == Keys.Space)
                _spaceDown = false;

            RefreshCursor(sender as DataGridViewDB);
        }
        
        private bool ClickedInside
        {
            get
            {
                return !_clickedOutside;
            }
            set
            {
                _clickedOutside = !value;
            }
        }

        private bool ClickedOutside
        {
            get
            {
                return _clickedOutside;
            }
            set
            {
                _clickedOutside = value;
            }
        }

        private bool MovedInside
        {
            get
            {
                return !_movedOutside;
            }
            set
            {
                _movedOutside = !value;
            }
        }

        private bool MovedOutside
        {
            get
            {
                return _movedOutside;
            }
            set
            {
                _movedOutside = value;
            }
        }

        private void ToolTip_Show(object sender, PopupEventArgs e)
        {
            string tip = StepToolTip.GetToolTip(e.AssociatedControl);

            if(tip.Length == 0)
                e.Cancel = true;

            StepToolTip.ShowAlways = true;
        }

        /*private void Check_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Shift == true)
            {
                _activeGrid.SelectedColumns.Clear();
            }
        }*/





        public string SelectedTabName
        {
            get
            {
                if (DataSourceTabs.SelectedTab != null)
                    return DataSourceTabs.SelectedTab.Text;
                else
                    return "";
            }
        }

        private void ColorLabel_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;

            if (sender == null)
                return;

            e.Graphics.DrawLine(Pens.Black, new Point(e.ClipRectangle.Left, e.ClipRectangle.Bottom), new Point(e.ClipRectangle.Left, e.ClipRectangle.Top));
            e.Graphics.DrawLine(Pens.Black, new Point(e.ClipRectangle.Left, e.ClipRectangle.Bottom), new Point(e.ClipRectangle.Left, e.ClipRectangle.Top));

            e.Graphics.DrawLine(Pens.Black, new Point(e.ClipRectangle.Right-1, e.ClipRectangle.Bottom), new Point(e.ClipRectangle.Right-1, e.ClipRectangle.Top));
            e.Graphics.DrawLine(Pens.Black, new Point(e.ClipRectangle.Right-1, e.ClipRectangle.Bottom), new Point(e.ClipRectangle.Right-1, e.ClipRectangle.Top));
        }
    }

    

}
