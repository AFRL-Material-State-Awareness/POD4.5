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

namespace POD.Controls
{

    public partial class DataGridViewDB : DataGridView
    {
        DataTable _table;
        public EventHandler ControlV;

        public DataTable Table
        {
            get { return _table; }
            set
            {
                DataSource = value;
                _table = value;

                
            }
        }

        public DataGridViewDB()
        {
            InitializeComponent();

            DoubleBuffered = true;

            Table = new DataTable();

            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            RowHeadersVisible = false;
            AllowUserToResizeRows = false;

            DefaultCellStyle.SelectionBackColor = Color.White;
            DefaultCellStyle.SelectionForeColor = Color.Black;

            StandardTab = true;
            
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.V))
            {
                if(EditingControl == null)
                {
                    if (ControlV != null)
                    {
                        ControlV.Invoke(this, null);
                    }
                }

                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Click_Cell(object sender, DataGridViewCellEventArgs e)
        {
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
    }
}
