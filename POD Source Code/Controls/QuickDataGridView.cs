// <copyright file="QuickDataGridView.cs" company="The University of Dayton">
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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using POD.Data;

namespace POD.Controls
{

    public partial class QuickDataGridView : DataGridView
    {
        public EventHandler RunAnalysis;
        public EventHandler ControlV;
        public ToolTip ToolTip
        {
            get
            {
                return toolTip1;
            }
            set
            {
                toolTip1 = value;
            }


        }

        public QuickDataGridView()
        {

            toolTip1 = null;

            InitializeComponent();

            DoubleBuffered = true;

            FixDataGridView();


        }

        public QuickDataGridView(ToolTip tooltip)
        {
            toolTip1 = tooltip;


            InitializeComponent();

            DoubleBuffered = true;

            FixDataGridView();

            
        }

        private void FixDataGridView()
        {
            RowHeadersVisible = false;
            AllowUserToResizeRows = false;
            AllowUserToOrderColumns = false;
            AllowUserToResizeColumns = false;
            AllowUserToAddRows = true;
            AllowUserToDeleteRows = true;
            //KeyDown += Grid_KeyDown;
            //PreviewKeyDown += QuickDataGridView_PreviewKeyDown;
            CellPainting += Cell_CellPainting;
            MultiSelect = false;
            UserAddedRow += dataGridView1_UserAddedRow;
            RowsAdded += dataGridView1_RowsAdded;
            CellEndEdit += dataGridView1_CellEndEdit;
            CellDoubleClick += Cell_CellDoubleClick;
            CellBeginEdit += Cell_CellBeginEdit;
            CellEndEdit += Cell_CellEndEdit;
            CellMouseEnter += Cell_CellMouseEnter;
            CellMouseLeave += Cell_CellMouseLeave;
            ShowCellToolTips = false;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.V))
            {
                if (EditingControl == null)
                {
                    PasteFromClipboard();
                }
            }
            else if (keyData == (Keys.Control | Keys.Enter))
            {
                RaiseRunAnalysis();
            }
            else if (keyData == (Keys.Control | Keys.Oemplus))
            {
                AddRow();
            }
            else if (keyData == (Keys.Shift | Keys.Enter))
            {
                if (SelectedCells.Count > 0)
                {
                    var row = SelectedCells[0].RowIndex;
                    var col = SelectedCells[0].ColumnIndex + 1;

                    if (col > Columns.Count - 1)
                    {
                        var tag = Rows[row].Cells[0].Tag as CellTag;

                        if (tag != null && tag.AutoValue == ID(Rows[row]))
                            col = 1;
                        else
                            col = 0;

                        if (!IsAddByUserRow(row))
                            row++;
                    }

                    CurrentCell = Rows[row].Cells[col];
                }

                return true;
            }
            else if (keyData == Keys.Delete)
            {
                DeleteSelectedRow();
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        void Cell_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            BeginEdit(true);
        }

        void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            GridCheckForProblems();

            for(int i = 0; i < e.RowCount; i++)
            {
                int index = i + e.RowIndex;
                if(LastDataRow != null && index <= LastDataRow.Index)
                    AddIdToNewRow(Rows[index]);
            }

            UpdateAutoIDs();
        }

        void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            GridCheckForProblems(e.RowIndex, e.ColumnIndex, true);
        }

        void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            GridCheckForProblems();

            AddIdToNewRow(LastDataRow);

            if (Control.ModifierKeys == Keys.Shift)
            {
                LastDataRow.Cells[SelectedCells[0].ColumnIndex].Selected = true;
            }
            
        }

        private void UpdateAutoIDs()
        {
            if(LastDataRow == null)
                return;

            foreach(DataGridViewRow row in Rows)
            {
                if(!IsAddByUserRow(row.Index))
                {
                    var cell = row.Cells[0];

                    if (cell.Value != null)
                    {
                        var status = cell.Tag as CellTag;

                        if (status != null)
                        {
                            if(status.AutoValue == ID(row))
                            {
                                cell.Value = GenerateID(row);
                                status.AutoValue = ID(row);
                            }
                        }
                    }
                }
            }
        }

        private string ID(DataGridViewRow row)
        {
            return row.Cells[0].Value.ToString();
        }

        private void AddIdToNewRow(DataGridViewRow myRow)
        {
            

            if (myRow != null)
            {
                var cell = myRow.Cells[0];
                var newID = GenerateID(myRow);

                if(cell.Value == null)
                {                
                    cell.Value = newID;

                    var status = cell.Tag as CellTag;

                    if(status != null)
                    {
                        status.AutoValue = newID;
                    }
                    
                }
            }
        }

        private string GenerateID(DataGridViewRow myRow)
        {
            return (myRow.Index + 1).ToString();
        }

        public DataGridViewRow LastDataRow
        {
            get
            {
                if (AllowUserToAddRows)
                {
                    if (Rows.Count > 1)
                        return Rows[Rows.Count - 2];
                    else
                        return null;
                }
                else
                    return Rows[Rows.Count - 1];
            }
        }

        void Cell_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            var grid = sender as DataGridView;

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
            var grid = sender as DataGridView;

            if (grid == null)
                return;

            DataGridViewColumn col = grid.Columns[e.ColumnIndex];
            DataGridViewRow row = grid.Rows[e.RowIndex];
            DataGridViewCell cell = row.Cells[e.ColumnIndex];

            cell.Style = col.DefaultCellStyle;

            GridCheckForProblems(e.RowIndex, e.ColumnIndex, true);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);         
        }

        private void RaiseRunAnalysis()
        {
            RunAnalysis?.Invoke(this, null);
        }

        public void PrepareDataColumns(DataColumnCollection collection)
        {
            foreach (DataColumn col in collection)
            {
                if (!Columns.Contains(col.ColumnName))
                {
                    int index = Columns.Add(col.ColumnName, col.ColumnName);

                    Columns[index].ValueType = typeof(string);
                    Columns[index].SortMode = DataGridViewColumnSortMode.NotSortable;
                    Columns[index].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    Columns[index].FillWeight = 1.0F / Columns.Count;
                }
            }

            SetColumnColors(this);
        }

        public bool CheckForValidDataGrid(DataPointChart chart, string analysisName)
        {
            var result = ValidResponsRowCount >= 8;

            if (!result && chart != null && analysisName != null)
            {
                chart.ResetErrors();
                chart.AddError(new ErrorArgs(analysisName, "Need at least 8 valid data points"));
                chart.AddError(new ErrorArgs(analysisName, "before an analysis can be run."));
                chart.AddError(new ErrorArgs(analysisName, string.Format("The table currently has {0}.", ValidResponsRowCount)));
                chart.AddError(new ErrorArgs(analysisName, "Please add more points to the table."));
                chart.FinalizeErrors(new ErrorArgs(analysisName, ""));
                //Leave this function here for testing purposes
                chart.ForceResizeAnnotations();
            }

            return result;
        }

        public void PasteFromClipboard(IPasteFromClipBoardWrapper clipboardPaster=null)
        {
            var pasteFromClipboard = clipboardPaster ?? new PasteFromClipBoardWrapper();

            var clipboardContents = pasteFromClipboard.GetClipBoardContents(TextDataFormat.Rtf);
            this.UpdateDataTable(clipboardContents);

            RaiseRunAnalysis();
        }

        private void UpdateDataTable(string clipboardContents)
        {
            var tempTable = DataHelpers.GetTableFromRtfString(clipboardContents);

            if (tempTable.Rows.Count == 0)
                return;

            FindPastedTableColumnIndexes(tempTable, out int idIndex, out int flawIndex, out int responseIndex);

            var usingAutoID = true;

            if (idIndex == -1 && flawIndex == -1 && responseIndex == -1)
            {
                return;
            }

            AllowUserToAddRows = false;

            int rowIndex = Rows.Count;

            Rows.Add(tempTable.Rows.Count);

            string id = "";
            string flaw = "";
            string response = "";

            foreach (DataRow row in tempTable.Rows)
            {
                if (idIndex != -1)
                {
                    id = row[idIndex].ToString();
                }
                else
                {
                    id = $"{rowIndex + 1}";
                }

                if (flawIndex != -1)
                {
                    flaw = row[flawIndex].ToString();
                }
                else
                {
                    flaw = "0.0";
                }

                if (responseIndex != -1)
                {
                    response = row[responseIndex].ToString();
                }
                else
                {
                    response = "0.0";
                }

                var newRow = Rows[rowIndex];

                if (usingAutoID)
                    id = (rowIndex+1).ToString();

                newRow.Cells[0].Value = id;
                newRow.Cells[1].Value = flaw;
                newRow.Cells[2].Value = response;

                rowIndex++;
            }

            AllowUserToAddRows = true;

            GridCheckForProblems();
        }

        private void FindPastedTableColumnIndexes(DataTable pastedTable, out int idIndex, out int flawIndex, out int responseIndex)
        {
            idIndex = -1;
            flawIndex = -1;
            responseIndex = -1;

            // if one column, then leave all indexes at -1
            if (pastedTable.Columns.Count <= 1)
            {
                return;
            }

            // if there are more than 3 columns, remove any columns with index greater than 2
            while (pastedTable.Columns.Count > 3)
            {
                pastedTable.Columns.RemoveAt(pastedTable.Columns.Count - 1);
            }

            List<int> nonNumericColumns = new List<int>();
            for (int i = 0; i < pastedTable.Columns.Count; i++)
            {
                if (!IsNumericColumn(pastedTable, i))
                {
                    nonNumericColumns.Add(i);
                }
            }

            if (nonNumericColumns.Count > 1)
            {
                // If more than one column has non-numeric text then leave all indexes at -1
                return;
            }

            if (nonNumericColumns.Count == 1)
            {
                // if a column has text that is not a number, then that column has to be the ID column.
                idIndex = nonNumericColumns[0];
                
                List<int> remainingIndexes = new List<int>();
                for(int i=0; i < pastedTable.Columns.Count; i++)
                {
                    if (i != idIndex)
                    {
                        remainingIndexes.Add(i);
                    }
                }

                if (remainingIndexes.Count == 2)
                {
                    // two remaining columns, apply two column logic
                    int col1 = remainingIndexes[0];
                    int col2 = remainingIndexes[1];

                    double range1 = GetColumnRange(pastedTable, col1);
                    double range2 = GetColumnRange(pastedTable, col2);

                    if (range1 < range2)
                    {
                        flawIndex = col1;
                        responseIndex = col2;
                    }
                    else
                    {
                        flawIndex = col2;
                        responseIndex = col1;
                    }
                }
                else if (remainingIndexes.Count == 1)
                {
                    responseIndex = remainingIndexes[0];
                }
                return; 
            }

            // If we are here, all columns are numeric.
            if (pastedTable.Columns.Count == 2)
            {
                // if two columns then column with smallest range is the flaw column and the other is the response column
                double range1 = GetColumnRange(pastedTable, 0);
                double range2 = GetColumnRange(pastedTable, 1);

                if (range1 < range2)
                {
                    flawIndex = 0;
                    responseIndex = 1;
                }
                else
                {
                    flawIndex = 1;
                    responseIndex = 0;
                }
            }
            else if (pastedTable.Columns.Count == 3)
            {
                // if three columns, then the column with the most consistent increment that is all interger values is the ID index, 
                // for the remaining two columns apply two column logic

                bool[] isIntColumn = new bool[3];
                double[] incrementConsistency = new double[3];

                for (int i = 0; i < 3; i++)
                {
                    isIntColumn[i] = IsIntegerColumn(pastedTable, i);
                    if (isIntColumn[i])
                    {
                        incrementConsistency[i] = GetIncrementConsistency(pastedTable, i);
                    }
                    else
                    {
                        incrementConsistency[i] = double.MaxValue;
                    }
                }

                // Find the column with the minimum increment consistency that is an integer column
                double minConsistency = double.MaxValue;
                int potentialIdIndex = -1;

                for (int i = 0; i < 3; i++)
                {
                    if (isIntColumn[i] && incrementConsistency[i] < minConsistency)
                    {
                        minConsistency = incrementConsistency[i];
                        potentialIdIndex = i;
                    }
                }

                if (potentialIdIndex != -1)
                {
                    idIndex = potentialIdIndex;
                    List<int> remainingIndexes = new List<int> { 0, 1, 2 };
                    remainingIndexes.Remove(idIndex);
                    
                    int col1 = remainingIndexes[0];
                    int col2 = remainingIndexes[1];

                    double range1 = GetColumnRange(pastedTable, col1);
                    double range2 = GetColumnRange(pastedTable, col2);

                    if (range1 < range2)
                    {
                        flawIndex = col1;
                        responseIndex = col2;
                    }
                    else
                    {
                        flawIndex = col2;
                        responseIndex = col1;
                    }
                }
            }
        }

        private bool IsNumericColumn(DataTable table, int columnIndex)
        {
            foreach (DataRow row in table.Rows)
            {
                if (!double.TryParse(row[columnIndex].ToString(), out _))
                {
                    return false;
                }
            }
            return true;
        }

        private double GetColumnRange(DataTable table, int columnIndex)
        {
            double min = double.MaxValue;
            double max = double.MinValue;

            foreach (DataRow row in table.Rows)
            {
                if (double.TryParse(row[columnIndex].ToString(), out double value))
                {
                    if (value < min) min = value;
                    if (value > max) max = value;
                }
            }

            if (min == double.MaxValue || max == double.MinValue)
            {
                return 0; // No numeric data
            }

            return max - min;
        }

        private bool IsIntegerColumn(DataTable table, int columnIndex)
        {
            foreach (DataRow row in table.Rows)
            {
                if (!int.TryParse(row[columnIndex].ToString(), out _))
                {
                    return false;
                }
            }
            return true;
        }

        private double GetIncrementConsistency(DataTable table, int columnIndex)
        {
            if (table.Rows.Count < 2)
            {
                return 0;
            }

            List<double> increments = new List<double>();
            for (int i = 1; i < table.Rows.Count; i++)
            {
                if (double.TryParse(table.Rows[i][columnIndex].ToString(), out double val2) &&
                    double.TryParse(table.Rows[i - 1][columnIndex].ToString(), out double val1))
                {
                    increments.Add(val2 - val1);
                }
            }

            if (increments.Count < 1)
            {
                return double.MaxValue;
            }

            double avg = increments.Average();
            double sumOfSquares = increments.Sum(inc => (inc - avg) * (inc - avg));
            return Math.Sqrt(sumOfSquares / increments.Count); // Standard deviation
        }

        private void SetColumnColors(DataGridView grid)
        {
            foreach (DataGridViewColumn col in grid.Columns)
            {
                col.DefaultCellStyle.BackColor = GetColorFromIndex(col.Index);
            }
        }

        private Color GetColorFromIndex(int colIndex)
        {
            switch (colIndex)
            {
                case 0:
                    return IDColor;
                case 1:
                    return FlawColor;
                case 2:
                    return ResponseColor;
                default:
                    return MetaColor;
            }
        }

        protected bool IsAddByUserRow(int rowIndex)
        {
            return LastDataRow == null || rowIndex == LastDataRow.Index + 1;
        }

        protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
        {
            base.OnCellPainting(e);

            e.Paint(e.CellBounds, DataGridViewPaintParts.All);

            if (e.RowIndex >= 0 && Rows.Count > 0 && Columns.Count > 0)
            {
                var cell = Rows[e.RowIndex].Cells[e.ColumnIndex];
                Color currentColor = cell.Style.BackColor;
                bool marked = MarkUpCell(cell, e.RowIndex, e.ColumnIndex, e.Graphics, e.CellBounds);

                if (marked)
                {
                    e.Paint(e.CellBounds, DataGridViewPaintParts.ContentForeground);
                }
            }

            e.Handled = true;
        }

        private void Cell_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            var grid = sender as DataGridView;

            if (grid != null)
            {
                
            }
        }

        private void GridCheckForProblems(int myRowIndex, int myColIndex, bool causeInvalidate)
        {
            if (myRowIndex > Rows.Count - 1 || myColIndex > Columns.Count - 1)
                return;

            DataGridViewColumn col = Columns[myColIndex];
            DataGridViewRow row = Rows[myRowIndex];
            DataGridViewCell cell = row.Cells[myColIndex];

            DefinitionMode mode = GetDefinitionModeFromColor(col.DefaultCellStyle.BackColor);

            var colTag = InitializeColumnTag(col, mode);
            var rowTag = InitializeRowTag(row, RowStatus.Valid);
            var cellTag = InitializeCellTag(cell, CellStatus.Valid);

            var status = UpdateCellStatus(mode, cell);

            cellTag.Status = status;

            var rowStatus = CheckRowStatus(row);

            //cell.ToolTipText = GetCellToolTip(status, rowStatus);

            if (cell == CurrentCell)
                toolTip1.SetToolTip(this, GetCellToolTip(status, rowStatus));

            if (causeInvalidate)
            {
                InvalidateCell(cell);
                InvalidateRow(myRowIndex);

                if (myRowIndex > 0)
                    InvalidateRow(myRowIndex - 1);
            }
        }

        private static string GetCellToolTip(CellStatus cellStatus, RowStatus rowStatus)
        {
            string statusMessage = "";

            if (cellStatus == CellStatus.InvalidWholeRow)
            {
                statusMessage = "Flaw size is invalid.\n" +
                                "The entire row will not be included in the analysis.";
            }
            else if (rowStatus == RowStatus.Invalid)
            {
                statusMessage = "Row contains an invalid flaw size.\n" +
                                "The entire row will not be included in the analysis.";
            }
            else if (cellStatus == CellStatus.Invalid)
            {
                statusMessage = "Response value is invalid.\n" +
                                "The response will not be included in the analysis.";
            }

            return statusMessage;
        }

        private DefinitionMode GetDefinitionModeFromColor(Color myColor)
        {
            DefinitionMode mode;


            if (myColor == IDColor)
            {
                mode = DefinitionMode.ID;
            }
            else if (myColor == MetaColor)
            {
                mode = DefinitionMode.MetaData;
            }
            else if (myColor == FlawColor)
            {
                mode = DefinitionMode.Flaw;
            }
            else if (myColor == ResponseColor)
            {
                mode = DefinitionMode.Response;
            }
            else if (myColor == GetClearColor())
            {
                mode = DefinitionMode.ClearOne;
            }
            else
            {
                mode = DefinitionMode.None;
            }


            return mode;
        }

        public Color GetClearColor()
        {
            return DefaultCellStyle.BackColor;
        }

        private bool MarkUpCell(DataGridViewCell cell, int rowIndex, int colIndex, Graphics graphics, Rectangle cellBounds)
        {
            bool marked = false;
            CellStatus cellStatus = CellStatus.Valid;
            RowStatus rowStatus = RowStatus.Valid;
            DefinitionMode colMode = DefinitionMode.None;

            if (cell.Value == DBNull.Value && rowIndex < Rows.Count - 1)
                GridCheckForProblems(rowIndex, colIndex, false);

            var row = Rows[rowIndex];
            var col = Columns[colIndex];

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

            if (colTag != null)
            {
                colMode = colTag.Mode;
            }

            Color backColor = col.DefaultCellStyle.BackColor;

            //if not a undefined column
            if (backColor != SystemColors.Window && backColor.A != 0)
            {
                Brush backBrush = new SolidBrush(backColor);

                if (IsCellNotCurrent(cell))
                {
                    //draw colored background
                    var wholeRectanlge = new Rectangle(cellBounds.X, cellBounds.Y, cellBounds.Width - 1, cellBounds.Height);
                    graphics.FillRectangle(backBrush, wholeRectanlge);
                    marked = true;
                }
                else
                {
                    //draw only colored line at bottom to allow select to show through
                    var wholeRectanlge = new Rectangle(cellBounds.X, cellBounds.Y + cellBounds.Height - 1, cellBounds.Width - 1, 1);
                    graphics.FillRectangle(backBrush, wholeRectanlge);
                }
            }

            if ((cellStatus != CellStatus.Valid || rowStatus != RowStatus.Valid) && !IsAddByUserRow(rowIndex))
            {
                var errorImage = CellIcons.Images[0];
                var rowErrorImage = CellIcons.Images[1];
                var heightDiff = (cellBounds.Height - errorImage.Height) / 2;
                var buffer = 5;
                var errorImageWidth = errorImage.Width + buffer;
                var shrinkBy = errorImageWidth + buffer;
                var finalPoint = new Rectangle(cellBounds.X + cellBounds.Width - errorImageWidth, cellBounds.Y + heightDiff, cellBounds.Width, cellBounds.Height - heightDiff);  

                //if cell is invalid within valid row or cell is causing row to be invalid                
                if ((cellStatus == CellStatus.Invalid && rowStatus == RowStatus.Valid) || cellStatus == CellStatus.InvalidWholeRow)
                {
                    //if not the current cell
                    if (IsCellNotCurrent(cell))
                    {
                        //draw white small white background with grey line at bottom
                        cellBounds = DrawSmallWhiteRectangle(graphics, cellBounds, shrinkBy);
                        DrawGreyLineAtBottom(graphics, cellBounds);
                    }

                    //draw the icon
                    if (cellStatus == CellStatus.Invalid)
                        graphics.DrawImageUnscaled(errorImage, finalPoint);
                    else
                        graphics.DrawImageUnscaled(rowErrorImage, finalPoint);

                    marked = true;
                }
                //else if row is invalid but cell is valid
                else if (rowStatus == RowStatus.Invalid || rowIndex == RowCount - 1)
                {
                    //if not the current cell
                    if (IsCellNotCurrent(cell))
                    {
                        cellBounds = FillCellWithWhite(graphics, cellBounds);

                        marked = true;
                    }
                }
            }

            if (GetCellStatusBelow(rowIndex, colIndex) != CellStatus.Valid || row == LastDataRow)
            {
                cellBounds = DrawGreyLineAtBottom(graphics, cellBounds);
            }

            if(IsAddByUserRow(rowIndex) && cell != CurrentCell)
            {
                FillCellWithWhite(graphics, cellBounds);
            }

            return marked;
        }

        private static Rectangle DrawSmallWhiteRectangle(Graphics graphics, Rectangle cellBounds, int shrinkBy)
        {
            var smallerRectanlge = new Rectangle(cellBounds.X, cellBounds.Y, cellBounds.Width - shrinkBy, cellBounds.Height - 1);
            graphics.FillRectangle(Brushes.White, smallerRectanlge);
            return cellBounds;
        }

        private Rectangle DrawGreyLineAtBottom(Graphics graphics, Rectangle cellBounds)
        {
            var wholeRectanlge = new Rectangle(cellBounds.X, cellBounds.Y + cellBounds.Height - 1, cellBounds.Width - 1, 1);
            graphics.FillRectangle(new SolidBrush(BackgroundColor), wholeRectanlge);
            return cellBounds;
        }

        private Rectangle FillCellWithWhite(Graphics graphics, Rectangle cellBounds)
        {
            var biggerRectanlge2 = new Rectangle(cellBounds.X, cellBounds.Y, cellBounds.Width - 1, cellBounds.Height - 1);

            //draw a white background with a grey line at the bottom
            graphics.FillRectangle(Brushes.White, biggerRectanlge2);

            return DrawGreyLineAtBottom(graphics, cellBounds);
        }

        private CellStatus GetCellStatusBelow(int rowIndex, int colIndex)
        {
            rowIndex++;

            if (rowIndex >= Rows.Count)
                return CellStatus.Valid;

            CellStatus cellStatus = CellStatus.Valid;
            RowStatus rowStatus = RowStatus.Valid;

            var row = Rows[rowIndex];
            var col = Columns[colIndex];
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

        private static bool IsCellNotCurrent(DataGridView grid, DataGridViewCell cell)
        {
            return grid.CurrentCell == null || grid.CurrentCell != cell;
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

        private bool IsCellNotCurrent(DataGridViewCell cell)
        {
            return CurrentCell == null || CurrentCell != cell;
        }

        public static Color GetClearColor(DataGridViewDB grid)
        {
            return grid.DefaultCellStyle.BackColor;
        }
        //nice blue
        public static Color IDColor => Color.FromArgb(204, 235, 255);
        //nice dark blue
        public static Color MetaColor => Color.FromArgb(190, 198, 247);
        //nice dark green
        public static Color FlawColor => Color.FromArgb(84, 184, 96);
        //nice green
        public static Color ResponseColor => Color.FromArgb(166, 237, 175);

        private RowStatus CheckRowStatus(DataGridViewRow row)
        {
            var rowTag = InitializeRowTag(row, RowStatus.Valid);
            var checkIndex = 0;

            rowTag.Status = RowStatus.Valid;

            foreach (DataGridViewColumn checkCol in Columns)
            {
                var tag = row.Cells[checkIndex].Tag as CellTag;
                var checkStatus = CellStatus.Valid;

                if (tag != null)
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
                if (cellValue == null)
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

        public void GridCheckForProblems()
        {
            //first update the columns that changed
            foreach (DataGridViewColumn col in Columns)
            {
                if (Rows.Count > 0)
                {
                    int index = Columns.IndexOf(col);

                    DefinitionMode mode = GetDefinitionModeFromColor(col.DefaultCellStyle.BackColor);
                    CellStatus status = CellStatus.Valid;

                    InitializeColumnTag(col, mode);

                    foreach (DataGridViewRow row in Rows)
                    {
                        if (row.Index == Rows.Count - 1)
                            continue;

                        var cell = row.Cells[index];
                        var rowStatus = RowStatus.Valid;

                        status = UpdateCellStatus(mode, cell);

                        InitializeCellTag(cell, status);

                        //if at the last column process the row for row errors
                        if (index == Columns.Count - 1)
                        {
                            rowStatus = CheckRowStatus(row);
                        }

                        //cell.ToolTipText = GetCellToolTip(status, rowStatus);

                        if (cell == CurrentCell)
                            toolTip1.SetToolTip(this, GetCellToolTip(status, rowStatus));
                    }
                }
            }
        }

        void Cell_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 || e.ColumnIndex == -1)
                return;

            var rowIndex = e.RowIndex;
            var colIndex = e.ColumnIndex;

            var row = Rows[rowIndex];
            var cell = row.Cells[colIndex];

            toolTip1.SetToolTip(this, "");
            //cell.ToolTipText = "";
        }

        void Cell_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 || e.ColumnIndex == -1)
                return;

            var statusMessage = "";

            var colIndex = e.ColumnIndex;
            var rowIndex = e.RowIndex;

            CellStatus cellStatus;
            RowStatus rowStatus;
            var cell = GetCellAndRowStatus(colIndex, rowIndex, out cellStatus, out rowStatus);

            statusMessage = GetCellToolTip(cellStatus, rowStatus);

            toolTip1.SetToolTip(this, statusMessage);

            if(LastDataRow == null || e.RowIndex == LastDataRow.Index+1)
            {
                CurrentCell = cell;
            }
        }

        private DataGridViewCell GetCellAndRowStatus(int colIndex, int rowIndex, out CellStatus cellStatus, out RowStatus rowStatus)
        {
            var row = Rows[rowIndex];
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

        /// <summary>
        /// Forces double buffer.
        /// </summary>
        /// <param name="e">Default event args.</param>
        protected override void OnHandleCreated(EventArgs e)
        {
            SendMessage(Handle, TVM_SETEXTENDEDSTYLE, false, TVS_EX_DOUBLEBUFFER);
            base.OnHandleCreated(e);
        }

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);
        private const int WM_SETREDRAW = 11;
        private const int TVM_GETEXTENDEDSTYLE = 0x1100 + 45;
        private const int TVM_SETEXTENDEDSTYLE = 0x1100 + 44;
        private const int TVS_EX_DOUBLEBUFFER = 0x4;

        private int suspendCounter = 0;
        private System.Windows.Forms.ToolTip toolTip1;

        public void SuspendDrawing()
        {
            if (suspendCounter == 0)
                SendMessage(this.Handle, WM_SETREDRAW, false, 0);
            suspendCounter++;
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

        public int ValidResponsRowCount
        {
            get
            {
                if(RowCount <= 1)
                    return 0;

                var rowCount = 0;

                foreach(DataGridViewRow row in Rows)
                {
                    var status = row.Cells[2].Tag as CellTag;
                    var rowStatus = row.Tag as RowTag;
                    if (status?.Status == CellStatus.Valid && rowStatus?.Status == RowStatus.Valid)
                        rowCount++;
                }
                return rowCount;
            }
        }

        public void DeleteSelectedRow()
        {
            if (SelectedCells.Count > 0)
            {
                var row = SelectedCells[0];

                row.Selected = false;

                if (!IsAddByUserRow(row.RowIndex))
                    Rows.RemoveAt(row.RowIndex);
            }
        }

        public void AddRow()
        {
            if (SelectedCells.Count > 0)
            {
                var cell = SelectedCells[0];
                var colIndex = cell.ColumnIndex;
                
                if (IsAddByUserRow(cell.RowIndex))
                {
                    Rows.Insert(cell.RowIndex, 1);
                    cell = LastDataRow.Cells[colIndex];
                }
                else
                {
                    Rows.Insert(cell.RowIndex + 1, 1);                   
                }

                CurrentCell = cell;
            }
            else
            {
                if (LastDataRow == null)
                {
                    Rows.Insert(0, 1);
                    CurrentCell = Rows[0].Cells[0];
                }
                else
                {
                    Rows.Insert(LastDataRow.Index, 1);
                    CurrentCell = Rows[LastDataRow.Index].Cells[0];                
                }
            }

            CurrentCell.Selected = true;
        }

        public DataTable DataTable
        {
            get
            {
                var table = new DataTable("Export Table");

                table.Columns.Add(new DataColumn("ID", typeof(string)));
                table.Columns.Add(new DataColumn("Flaw", typeof(double)));
                table.Columns.Add(new DataColumn("Column", typeof(double)));

                AllowUserToAddRows = false;

                foreach(DataGridViewRow row in Rows)
                {
                    var newRow = table.NewRow();

                    var id = row.Cells[0].Value.ToString();
                    var flaw = Double.NaN;
                    var response = Double.NaN;

                    Double.TryParse(row.Cells[1].Value.ToString(), out flaw);
                    Double.TryParse(row.Cells[2].Value.ToString(), out response);

                    newRow[0] = id;
                    newRow[1] = flaw;
                    newRow[2] = response;

                    table.Rows.Add(newRow);
                }

                AllowUserToAddRows = true;

                return table;
            }
        }
    }
}
