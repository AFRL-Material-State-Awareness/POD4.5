using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POD.Data;
using POD.Analyze;
using POD.Controls;

namespace POD.Wizards
{
    public partial class RemovedPointsPanel : UserControl
    {
        private Analysis _analysis;
        private PODButton _selectAll;
        private PODButton _selectNone;
        private PODButton _selectInvert;
        private PODButton _selectEmpty;
        private PODButton _copyBack;
        private PODButton _removeComment;
        private PODButton _applyComment;
        private PODButton _setTemplate;
        private PODButton _deleteTemplate;
        public PODToolTip PODToolTip;
        private SimpleActionBar RemovedPointsBar;
        private SimpleActionBar DocumentsBar;

        bool HasLeft = true;
        public List<string> DeleteList = new List<string>();
        public List<string> TemplateList
        {
            get
            {
                return GetAllTemplates();
            }
        }

        public bool SendKeys(Keys keyData)
        {           

            if (keyData == (Keys.Control | Keys.A | Keys.Shift))
            {
                _selectNone.PerformClick();
                return true;
            }
            if (keyData == (Keys.Control | Keys.I | Keys.Shift))
            {
                _selectEmpty.PerformClick();
                return true;
            }
            if (keyData == (Keys.Control | Keys.G | Keys.Shift))
            {
                _setTemplate.PerformClick();
                return true;
            }
            if (keyData == (Keys.Control | Keys.D | Keys.Shift))
            {
                _deleteTemplate.PerformClick();
                return true;
            }
            if (keyData == (Keys.Control | Keys.G))
            {
                _applyComment.PerformClick();
                return true;
            }
            if (keyData == (Keys.Control | Keys.A))
            {
                _selectAll.PerformClick();
                return true;
            }
            if (keyData == (Keys.Control | Keys.I))
            {
                _selectInvert.PerformClick();
                return true;
            }
            if (keyData == (Keys.Control | Keys.D))
            {
                _removeComment.PerformClick();
                return true;
            }
            if (keyData == (Keys.Control | Keys.B))
            {
                _copyBack.PerformClick();
                return true;
            }

            return false;
        }

        public void CreateButtons(WizardPanel panel)
        {
            PODToolTip = new PODToolTip();

            RemovedPointsBar = new SimpleActionBar();
            DocumentsBar = new SimpleActionBar();
            RemovedPointsBar.ToolTip = PODToolTip;
            DocumentsBar.ToolTip = PODToolTip;

            //RemovedPointsBar.RemovePadding();

            _selectAll = RemovedPointsBar.AddButton("Select All", All_Click, "Select all points on the list. (Ctrl + A)");
            _selectNone = RemovedPointsBar.AddButton("Select None", None_Click, "Unselect all points on the list. (Ctrl + A + Shift)");
            _selectInvert = RemovedPointsBar.AddButton("Invert Selection", Invert_Click, "Invert selection of data point list. (Ctrl + I)");
            _selectEmpty = RemovedPointsBar.AddButton("Select Empty", Empty_Click, "Select points with no comment. (Ctrl + Shift + I)");
            _copyBack = RemovedPointsBar.AddButton("Copy Back Comment", Copy_Click, "Copy comment back to be edited. (Ctrl + B)");
            _removeComment = RemovedPointsBar.AddButton("Remove Comment", Remove_Click, "Remove comment from selected points. (Ctrl + D)");
            _applyComment = DocumentsBar.AddButton("Apply Comment", Apply_Click, "Apply comment to selected points. (Ctrl + G)");
            _setTemplate = DocumentsBar.AddButton("Add to Templates", Add_Click, "Add current comment to list of available templates. (Ctrl + Shift + G)");
            _deleteTemplate = DocumentsBar.AddButton("Delete from Templates", Delete_Click, "Delete selected templates from list. (Ctrl + Shift + D)");

            _selectAll.TabIndex = 3;
            _selectNone.TabIndex = 4;
            _selectInvert.TabIndex = 5;
            _selectEmpty.TabIndex = 6;
            _copyBack.TabIndex = 7;
            _removeComment.TabIndex = 8;
            _applyComment.TabIndex = 9;
            _setTemplate.TabIndex = 10;
            _deleteTemplate.TabIndex = 11;

            var removedList = new List<ButtonHolder>();
            var documentsList = new List<ButtonHolder>();

            removedList.Add(new ButtonHolder(_selectAll.Name, _selectAll.Image, All_Click, _selectAll.TipForControl));
            removedList.Add(new ButtonHolder(_selectNone.Name, _selectNone.Image, None_Click, _selectNone.TipForControl));
            removedList.Add(new ButtonHolder(_selectInvert.Name, _selectInvert.Image, Invert_Click, _selectInvert.TipForControl));
            removedList.Add(new ButtonHolder(_selectEmpty.Name, _selectEmpty.Image, Empty_Click, _selectEmpty.TipForControl));
            removedList.Add(new ButtonHolder(_copyBack.Name, _copyBack.Image, Copy_Click, _copyBack.TipForControl));
            removedList.Add(new ButtonHolder(_removeComment.Name, _removeComment.Image, Remove_Click, _removeComment.TipForControl));
            documentsList.Add(new ButtonHolder(_applyComment.Name, _applyComment.Image, Apply_Click, _applyComment.TipForControl));
            documentsList.Add(new ButtonHolder(_setTemplate.Name, _setTemplate.Image, Add_Click, _setTemplate.TipForControl));
            documentsList.Add(new ButtonHolder(_deleteTemplate.Name, _deleteTemplate.Image, Delete_Click, _deleteTemplate.TipForControl));

            var removedPanel = ContextMenuStripConnected.MakeNewMenuFlowLayoutPanel("Removed_Points");
            var documentsPanel = ContextMenuStripConnected.MakeNewMenuFlowLayoutPanel("Documented_Points");


            foreach (var item in removedList)
            {
                ContextMenuStripConnected.AddButtonToMenu(removedPanel, item, PODToolTip);
            }

            foreach (var item in documentsList)
            {
                ContextMenuStripConnected.AddButtonToMenu(documentsPanel, item, PODToolTip);
            }

            var tableHost = new ToolStripControlHost(removedPanel);
            var documentHost = new ToolStripControlHost(documentsPanel);

            ContextMenuStripConnected.ForcePanelToDraw(removedPanel);
            ContextMenuStripConnected.ForcePanelToDraw(documentsPanel);

            panel.ContextMenuStrip.Items.Add(tableHost);
            panel.ControlStrip.Add(tableHost);
            panel.TextStrip.Add(tableHost);

            panel.ContextMenuStrip.Items.Add(documentHost);
            panel.ControlStrip.Add(documentHost);
            panel.TextStrip.Add(documentHost);

            //ContextMenuStrip = contextMenu;

            RemovedPointsBar.Dock = DockStyle.Fill;
            DocumentTableLayout.Controls.Add(RemovedPointsBar, 0, 1);
            DocumentTableLayout.SetRowSpan(RemovedPointsBar, 4);

            DocumentsBar.Dock = DockStyle.Fill;
            DocumentTableLayout.Controls.Add(DocumentsBar, 2, 1);
            DocumentTableLayout.SetRowSpan(DocumentsBar, 4);
        }

        public RemovedPointsPanel()
        {
            InitializeComponent();

            if(!DesignMode)
                AddColumns();

            RemovedPointsList.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            TemplateCommentList.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            CurrentCommentTextBox.MouseDown += PODChartNumericUpDown_MouseDown;
            CurrentCommentTextBox.Leave += CurrentReasonTextBox_Leave;

            
        }

        void CurrentReasonTextBox_Leave(object sender, EventArgs e)
        {
            HasLeft = true;
        }

        void PODChartNumericUpDown_MouseDown(object sender, MouseEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox != null && HasLeft)
            {
                textBox.SelectAll();
                HasLeft = false;
            }
        }

        void RemovedPointsPanel_Load(object sender, EventArgs e)
        {
            
        }

        private void AddColumns()
        {
            TemplateCommentList.Columns.Clear();

            var templateColumn = new DataGridViewTextBoxColumn();

            templateColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            templateColumn.HeaderText = "Comment";
            templateColumn.Name = "CommentColumn";
            templateColumn.ReadOnly = true;
            templateColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;

            TemplateCommentList.Columns.Add(templateColumn);

            var idColumn = new DataGridViewTextBoxColumn();
            var responseNameColumn = new DataGridViewTextBoxColumn();
            var flawColumn = new DataGridViewTextBoxColumn();
            var responseColumn = new DataGridViewTextBoxColumn();
            var commentColumn = new DataGridViewTextBoxColumn();
            var rowIndex = new DataGridViewTextBoxColumn();
            var columnIndex = new DataGridViewTextBoxColumn();

            RemovedPointsList.Columns.Clear();

            // 
            // dataGridViewTextBoxColumn4
            // 
            idColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            idColumn.HeaderText = "ID";
            idColumn.Name = "idColumn";
            idColumn.ReadOnly = true;
            idColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            idColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            idColumn.Width = 5;
            // 
            // dataGridViewTextBoxColumn4
            // 
            responseNameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            responseNameColumn.HeaderText = "Response Name";
            responseNameColumn.Name = "idColumn";
            responseNameColumn.ReadOnly = true;
            responseNameColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            responseNameColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            responseNameColumn.Width = 5;
            // 
            // dataGridViewTextBoxColumn2
            // 
            flawColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            flawColumn.HeaderText = "Flaw";
            flawColumn.Name = "dataGridViewTextBoxColumn2";
            flawColumn.ReadOnly = true;
            flawColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            flawColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            flawColumn.Width = 5;
            // 
            // dataGridViewTextBoxColumn1
            // 
            responseColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            responseColumn.HeaderText = "Response";
            responseColumn.Name = "dataGridViewTextBoxColumn1";
            responseColumn.ReadOnly = true;
            responseColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            responseColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            responseColumn.Width = 5;
            // 
            // CommentColumn
            // 
            commentColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            commentColumn.HeaderText = "Comment";
            commentColumn.Name = "CommentColumn";
            commentColumn.ReadOnly = true;
            commentColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // RowIndex
            // 
            rowIndex.HeaderText = "Row Index";
            rowIndex.Name = "RowIndex";
            rowIndex.ReadOnly = true;
            rowIndex.Visible = false;
            // 
            // ColumnIndex
            // 
            columnIndex.HeaderText = "Column Index";
            columnIndex.Name = "ColumnIndex";
            columnIndex.ReadOnly = true;
            columnIndex.Visible = false;

            RemovedPointsList.Columns.AddRange(new DataGridViewColumn[] { idColumn, responseNameColumn, flawColumn, responseColumn, commentColumn, rowIndex, columnIndex });
        }
        
        

        public Analysis Analysis
        {
            set
            {
                _analysis = value;

                RefreshPanel();
            }
            get
            {
                return _analysis;
            }
        }

        public void UpdateMRUList()
        {
            var list = TemplateList;
            var deletes = DeleteList;

            if (list != null)
            {
                foreach (string delete in deletes)
                {
                    Globals.DeleteMRUList(delete, Globals.PODv4CommentsFile);
                }
                
                foreach (string template in list)
                {
                    Globals.UpdateMRUList(template, Globals.PODv4CommentsFile, false, "", 100);
                }
            }
        }

        

        private void Delete_Click(object sender, EventArgs e)
        {
            if (TemplateCommentList.SelectedRows.Count > 0)
            {
                var remove = TemplateCommentList.SelectedRows[0].Cells[0].Value.ToString().Trim();
                DeleteList.Add(remove);

                var index = TemplateCommentList.SelectedRows[0].Index;

                TemplateCommentList.Rows.Remove(TemplateCommentList.SelectedRows[0]);

                index--;

                if (index < 0)
                    index = 0;

                if(index < TemplateCommentList.Rows.Count)
                {
                    TemplateCommentList.Rows[index].Selected = true;
                }
            }

            UpdateMRUList();
        }

        private void Remove_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in RemovedPointsList.SelectedRows)
            {
                row.Cells[4].Value = "";
            }
        }

        private void Empty_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in RemovedPointsList.Rows)
            {
                if (row.Cells[4].Value.ToString().Trim().Length == 0)
                    row.Selected = true;
                else
                    row.Selected = false;
            }
        }

        private void Add_Click(object sender, EventArgs e)
        {
            string comment = CurrentCommentTextBox.Text;

            AddTemplate(comment);

            UpdateMRUList();
        }

        private void AddTemplate(string comment, bool doCheck = true)
        {
            comment = comment.Trim();

            if (comment.Length > 0)
            {
                var checkResult = true;

                if(doCheck)
                { 
                    var templates = GetAllTemplates();

                    checkResult = !templates.Contains(comment);
                }

                if (checkResult)
                {
                    var row = TemplateCommentList.CreateNewCloneRow(1);

                    row.Cells[0].Value = comment;

                    TemplateCommentList.Rows.Insert(0, row);
                    row.Selected = true;

                    CurrentCommentTextBox.AutoCompleteCustomSource.Add(comment);
                }

                
            }
        }

        private List<string> GetAllTemplates()
        {
            var list = new List<string>();

            foreach (DataGridViewRow row in TemplateCommentList.Rows)
            {
                list.Add(row.Cells[0].Value.ToString().Trim());
            }

            var unique = list.Distinct().ToList();

            return unique;
        }

        private void Apply_Click(object sender, EventArgs e)
        {
            ApplyComment(CurrentCommentTextBox.Text);
        }

        private void Copy_Click(object sender, EventArgs e)
        {
            var list = new List<string>();

            foreach (DataGridViewRow row in RemovedPointsList.SelectedRows)
            {
                list.Add(row.Cells[4].Value.ToString() + " ");
            }

            var unique = list.Distinct();

            var final = "";

            foreach(string comment in unique)
            {
                if(comment.Trim().Length > 0)
                    final += comment;
            }

            final = final.Trim();

            CurrentCommentTextBox.Text = final;
        }

        private void Invert_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in RemovedPointsList.Rows)
            {
                row.Selected = !row.Selected;
            }
        }

        private void None_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in RemovedPointsList.Rows)
            {
                row.Selected = false;
            }
        }

        private void All_Click(object sender, EventArgs e)
        {
            foreach(DataGridViewRow row in RemovedPointsList.Rows)
            {
                row.Selected = true;
            }
        }

        public void RefreshPanel()
        {
            if (Analysis == null)
                return;

            RemovedPointsList.ColumnHeadersVisible = true;
            if (RemovedPointsList.Columns.Contains("Items"))
                RemovedPointsList.Columns.Remove("Items");

            var list = Analysis.Data.TurnedOffPoints;
            var flawTable = Analysis.Data.ActivatedFlaws;
            var idTable = Analysis.Data.ActivatedSpecimenIDs;
            var responseTable = Analysis.Data.ActivatedResponses;
            var rows = new List<DataGridViewRow>();

            RemovedPointsList.Rows.Clear();

            foreach(DataPointIndex point in list)
            {
                var row = RemovedPointsList.CreateNewCloneRow(7);

                row.Cells[0].Value = idTable.Rows[point.RowIndex][0].ToString();
                row.Cells[1].Value = responseTable.Columns[point.ColumnIndex].ColumnName;
                var flaw = Convert.ToDouble(flawTable.Rows[point.RowIndex][0]);
                flaw = Analysis.Data.InvertTransformedFlaw(flaw);
                row.Cells[2].Value = flaw.ToString("F3");
                var response = Convert.ToDouble(responseTable.Rows[point.RowIndex][point.ColumnIndex]);
                response = Analysis.Data.InvertTransformedResponse(response);
                row.Cells[3].Value = response.ToString("F3");
                row.Cells[4].Value = Analysis.Data.GetRemovedPointComment(point.ColumnIndex, point.RowIndex);
                row.Cells[5].Value = point.RowIndex.ToString();
                row.Cells[6].Value = point.ColumnIndex.ToString();

                if(!double.IsNaN(flaw) && !double.IsNaN(response))
                    rows.Add(row);
            }

            RemovedPointsList.Rows.AddRange(rows.ToArray());

            var templates = Globals.GetMRUList(Globals.PODv4CommentsFile);

            CurrentCommentTextBox.AutoCompleteCustomSource.Clear();
            TemplateCommentList.Rows.Clear();

            if (templates != null)
            {
                CurrentCommentTextBox.AutoCompleteCustomSource.AddRange(templates.ToArray());

                foreach (string template in templates)
                {
                    AddTemplate(template, false);
                }
            }

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Comment_Validated(object sender, EventArgs e)
        {
            
        }

        private void ApplyComment(string comment)
        {
            foreach (DataGridViewRow row in RemovedPointsList.SelectedRows)
            {
                int colIndex = 0;
                int rowIndex = 0;

                Int32.TryParse(row.Cells[5].Value.ToString(), out rowIndex);
                Int32.TryParse(row.Cells[6].Value.ToString(), out colIndex);

                Analysis.Data.SetRemovedPointComment(colIndex, rowIndex, comment);

                row.Cells[4].Value = comment;
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox == null)
                return;

            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                if (Control.ModifierKeys == Keys.Shift)
                {
                    AddTemplate(CurrentCommentTextBox.Text);

                    UpdateMRUList();
                }
                else
                {
                    ApplyComment(CurrentCommentTextBox.Text);
                }                
            }
        }

        private void List_Resize(object sender, EventArgs e)
        {
            var list = sender as DataGridView;

            if(list != null)
            {
                list.AutoResizeRows();
            }
        }

        private void Cell_Clicked(object sender, DataGridViewCellEventArgs e)
        {
            var index = e.RowIndex;

            var template = TemplateCommentList.Rows[index].Cells[0].Value.ToString();

            if (Control.ModifierKeys == Keys.Shift)
            {
                ApplyComment(template);
            }
            else
            {
                CurrentCommentTextBox.Text = template;
                CurrentCommentTextBox.SelectAll();
            }
        }

        public int RemovedPointsCount
        {
            get
            {
                return RemovedPointsList.Rows.Count;
            }
        }
    }
}
