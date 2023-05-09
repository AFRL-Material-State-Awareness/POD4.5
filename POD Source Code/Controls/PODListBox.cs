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
    public partial class PODListBox : DataGridView
    {

        public PODListBox()
        {
            InitializeComponent();

            SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            MultiSelect = true;
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            EditMode = DataGridViewEditMode.EditProgrammatically;
            ShowCellToolTips = true;

            ReadOnly = true;

            DoubleBuffered = true;

            AllowUserToAddRows = false;
            AllowUserToDeleteRows = false;
            AllowUserToOrderColumns = false;
            AllowUserToResizeColumns = false;
            AllowUserToResizeRows = false;

            ColumnHeadersVisible = false;
            RowHeadersVisible = false;

            StandardTab = true;

            BackgroundColor = Color.FromKnownColor(KnownColor.Window);
            BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            GridColor = Color.FromKnownColor(KnownColor.Window);

            if (!DesignMode)
                AddInitialColumn();
        }

        public void AddInitialColumn()
        {
            Columns.Clear();

            var column = new DataGridViewTextBoxColumn();

            column.ValueType = typeof(PODListBoxItem);
            column.Name = "Items";

            Columns.Add(column);
        }

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);
        private const int WM_SETREDRAW = 11;
        private const int TVM_GETEXTENDEDSTYLE = 0x1100 + 45;
        private const int TVM_SETEXTENDEDSTYLE = 0x1100 + 44;
        private const int TVS_EX_DOUBLEBUFFER = 0x4;

        private int suspendCounter = 0;

        //g7ets either the first selected row or it selectes the first row
        public PODListBoxItem SingleSelectedItem
        {
            get
            {
                if(Rows.Count > 0 && SelectedRows?.Count > 0)
                    return SelectedRows[0].Cells[0].Value as PODListBoxItem;
                else
                    return null;
            }
        }

        //gets either the first selected row or it selectes the first row
        public PODListBoxItemWithProps SingleSelectedItemWithProps
        {
            get
            {
                if(Rows.Count > 0 && SelectedRows?.Count > 0)
                    return SelectedRows[0].Cells[0].Value as PODListBoxItemWithProps;
                else
                    return null;
            }
        }

        //gets either the first selected row or it selectes the first row
        public int SingleSelectedIndex
        {
            get
            {
                if(Rows.Count > 0 && SelectedRows?.Count > 0)
                    return SelectedRows[0].Index;
                else
                    return -1;
            }

            set
            {
                if (value >= 0 && value < Rows.Count)
                {
                    if(MultiSelect == true)
                    {
                        foreach(DataGridViewRow row in Rows)
                        {
                            row.Selected = false;
                        }
                    }

                    Rows[value].Selected = true;
                }
            }
        }

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

        /// <summary>
        /// Forces double buffer.
        /// </summary>
        /// <param name="e">Default event args.</param>
        protected override void OnHandleCreated(EventArgs e)
        {
            SendMessage(Handle, TVM_SETEXTENDEDSTYLE, false, TVS_EX_DOUBLEBUFFER);
            base.OnHandleCreated(e);
        }

        static public string CreateAutoName(List<PODListBoxItem> listBoxItems)
        {
            string name = "";

            var myNames = listBoxItems.Select(item => (item.ResponseColumnName)).ToList();

            //http://stackoverflow.com/questions/13509277/find-a-common-string-within-a-list-of-strings
            //Coded by: Neil Moss, Nov 22 '12 at 9:30 

            // Sort word list by length
            List<string> wordsInLengthOrder = (from word in myNames
                                               orderby word.Length
                                               select word).ToList();

            string longestSubString = FindLargestBeginningSubstring(wordsInLengthOrder);
            name = longestSubString;

            if (myNames.Count > 1)
            {
                name += "(";

                for (int i = 0; i < myNames.Count; i++)
                {
                    myNames[i] = myNames[i].Remove(0, longestSubString.Length);

                    name += myNames[i] + ", ";
                }

                name = name.Remove(name.Length - 2);

                name += ")";
            }


            PODListBoxItem firstItem = null;

            if (listBoxItems.Count > 0)
            {
                firstItem = listBoxItems[0];
                name = firstItem.DataSourceName + "." + firstItem.FlawColumnName + "." + name;
            }

            return name;
        }
        private static string FindLargestBeginningSubstring(List<string> wordsInLengthOrder)
        {
            string largestSubString = "";

            string shortestWord = wordsInLengthOrder.FirstOrDefault();
            // FirstOrDefault() returns null if wordsInLengthOrder count is 0
            // '?' checks if null before calling length
            for (int i =0; i < shortestWord?.Length; i++)
            {
                if (HaveTheSameIndex(wordsInLengthOrder, i))
                    largestSubString += shortestWord[i];
                else 
                    break;        
            }
            return largestSubString;
        }
        private static bool HaveTheSameIndex(List<string> stringList, int index)
        {
            return stringList.Select(s => s.ElementAtOrDefault(index)).Distinct().Count() == 1;
        }
        public string AutoName
        {
            get
            {
                List<PODListBoxItem> names = new List<PODListBoxItem>();

                foreach (DataGridViewRow row in Rows)
                {
                    PODListBoxItem item = (PODListBoxItem)row.Cells[0].Value;

                    names.Add(item);
                }

                return CreateAutoName(names);
            }
        }

        public bool GetSelected(int myIndex)
        {
            return SelectedRows.Contains(Rows[myIndex]);
        }

        public void SetSelected(int myIndex, bool mySelect)
        {
            Rows[myIndex].Selected = mySelect;
        }


        public List<PODListBoxItem> GetSelectedRows()
        {
            var list = new List<PODListBoxItem>();

            foreach (DataGridViewRow row in SelectedRows)
            {
                PODListBoxItem obj = (PODListBoxItem)row.Cells[0].Value;

                list.Add(obj);
            }

            return list;
        }

        public List<PODListBoxItemWithProps> GetSelectedRowsWithProps()
        {
            var list = new List<PODListBoxItemWithProps>();

            foreach (DataGridViewRow row in SelectedRows)
            {
                PODListBoxItemWithProps obj = row.Cells[0].Value as PODListBoxItemWithProps;

                list.Add(obj);
            }

            return list;
        }

        public List<int> GetSelectedIndicies()
        {
            var list = new List<int>();

            foreach (DataGridViewRow row in SelectedRows)
            {
                var index = row.Index;

                list.Add(index);
            }

            list.Sort();

            return list;
        }

        public DataGridViewRow CreateNewCloneRow(int columnCount =  1)
        {           
            var newRow = (DataGridViewRow)RowTemplate.Clone();
            newRow.CreateCells(this);

            RemoveExtraColumns(newRow, columnCount);

            return newRow;
        }

        public static void RemoveExtraColumns(DataGridViewRow row, int columnCount = 1)
        {
            while (row.Cells.Count > columnCount)
                row.Cells.RemoveAt(row.Cells.Count - 1);
        }

        public void FitAllRows()
        {
            Height =  Rows.GetRowsHeight(DataGridViewElementStates.None) + 3;
        }

        public void FitAllRows(int myMaxCount)
        {
            if (Rows.Count > 0)
            {
                var rowsHeight = Rows.GetRowsHeight(DataGridViewElementStates.None);
                var rowCount = Rows.Count;
                var rowHeight = (double)rowsHeight / (double)rowCount;

                if (myMaxCount <= rowCount)
                    rowCount = myMaxCount;

                Height = Convert.ToInt32(rowCount * rowHeight) + 3;
            }
        }
    }
}
