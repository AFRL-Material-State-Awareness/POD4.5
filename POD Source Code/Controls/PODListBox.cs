﻿using System;
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

        //gets either the first selected row or it selectes the first row
        public PODListBoxItem SingleSelectedItem
        {
            get
            {
                if (Rows.Count > 0)
                {
                    if (SelectedRows.Count > 0)
                        return SelectedRows[0].Cells[0].Value as PODListBoxItem;
                    /*else
                    {
                        var item = Rows[0].Cells[0].Value as PODListBoxItem;
                        Rows[0].Selected = true;

                        return item;
                    }*/
                }

                return null;
            }
        }

        //gets either the first selected row or it selectes the first row
        public PODListBoxItemWithProps SingleSelectedItemWithProps
        {
            get
            {
                if (Rows.Count > 0)
                {
                    if (SelectedRows.Count > 0)
                        return SelectedRows[0].Cells[0].Value as PODListBoxItemWithProps;
                    /*else
                    {
                        var item = Rows[0].Cells[0].Value as PODListBoxItem;
                        Rows[0].Selected = true;

                        return item;
                    }*/
                }

                return null;
            }
        }

        //gets either the first selected row or it selectes the first row
        public int SingleSelectedIndex
        {
            get
            {
                if (Rows.Count > 0)
                {
                    if (SelectedRows.Count > 0)
                        return SelectedRows[0].Index;
                    /*else
                    {
                        var item = Rows[0].Cells[0].Value as PODListBoxItem;
                        Rows[0].Selected = true;

                        return item;
                    }*/
                }

                return -1;
            }

            set
            {
                if (Rows.Count > 0 && value >= 0 && value < Rows.Count)
                {
                    if(MultiSelect == true)
                    {
                        foreach(DataGridViewRow row in Rows)
                        {
                            row.Selected = false;
                        }
                    }

                    Rows[value].Selected = true;
                    /*else
                    {
                        var item = Rows[0].Cells[0].Value as PODListBoxItem;
                        Rows[0].Selected = true;

                        return item;
                    }*/
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

        /*private void Item_Dropped(object sender, DroppedEventArgs e)
        {

            int matches = 0;
            int droppedIndex = 0;
            int index = 0;

            foreach (Object droppedItem in e.DroppedItems)
            {
                matches = 0;
                index = 0;

                foreach (Object item in Items)
                {
                    if (droppedItem == item)
                    {
                        matches++;
                        droppedIndex = index;
                    }

                    index++;
                }

                if (matches > 1)
                {
                    Items.RemoveAt(droppedIndex);
                }
            }
        }*/

        static public string CreateAutoName(List<PODListBoxItem> listBoxItems)
        {
            string name = "";
            string longestSubString = null;

            var myNames = listBoxItems.Select(item => (item.ResponseColumnName)).ToList();

            //http://stackoverflow.com/questions/13509277/find-a-common-string-within-a-list-of-strings
            //Coded by: Neil Moss, Nov 22 '12 at 9:30 

            // Sort word list by length
            List<string> wordsInLengthOrder = (from word in myNames
                                               orderby word.Length
                                               select word).ToList();

            if (wordsInLengthOrder.Count > 0)
            {
                string shortestWord = wordsInLengthOrder[0];
                int shortWordLength = shortestWord.Length;

                // Work through the consecutive character strings, in length order.
                for (int partLength = shortWordLength; (partLength > 0) && (longestSubString == null); partLength--)
                {
                    for (int partStartIndex = 0; partStartIndex <= shortWordLength - partLength; partStartIndex++)
                    {
                        string part = shortestWord.Substring(partStartIndex, partLength);

                        // Test if all the words in the sorted list contain the part.
                        if (wordsInLengthOrder.All(s => s.StartsWith(part)))
                        {
                            longestSubString = part;
                            break;
                        }
                    }

                }

                if (longestSubString == null)
                    longestSubString = "";

                name = longestSubString;

                if (myNames.Count > 1)
                {
                    //if(longestSubString.Length > 0)
                    name += "(";

                    for (int i = 0; i < myNames.Count; i++)
                    {
                        myNames[i] = myNames[i].Remove(0, longestSubString.Length);

                        name += myNames[i] + ", ";
                    }

                    name = name.Remove(name.Length - 2);

                    //if (longestSubString.Length > 0)
                    name += ")";
                }
            }

            PODListBoxItem firstItem = null;

            if (listBoxItems.Count > 0)
            {
                firstItem = listBoxItems[0];
                name = firstItem.DataSourceName + "." + firstItem.FlawColumnName + "." + name;
            }

            return name;
        }

        public string AutoName
        {
            get
            {
                //string name = "";
                List<PODListBoxItem> names = new List<PODListBoxItem>();

                foreach (DataGridViewRow row in Rows)
                {
                    PODListBoxItem item = (PODListBoxItem)row.Cells[0].Value;

                    names.Add(item);
                }

                return CreateAutoName(names);
            }
        }


        public List<string> GetSelectedRowsAsTextList()
        {
            List<string> names = new List<string>();

            foreach (DataGridViewRow row in SelectedRows)
            {
                PODListBoxItem obj = (PODListBoxItem)row.Cells[0].Value;

                names.Add(obj.ToString());
            }

            //reverse because SelectedRows row order is in the opposite order of what we want
            names.Reverse();

            return names;
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

                if (rowCount > myMaxCount)
                    rowCount = myMaxCount;

                Height = Convert.ToInt32(rowCount * rowHeight) + 3;
            }
        }
    }
}