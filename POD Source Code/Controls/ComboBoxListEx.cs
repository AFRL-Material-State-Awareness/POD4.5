using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POD.Controls
{
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    /// <summary>
    /// source: https://stackoverflow.com/questions/61152847/raise-an-event-when-i-hover-the-mouse-over-a-combobox-item
    /// author: jimi
    /// author url: https://stackoverflow.com/users/7444103/jimi
    /// </summary>
    [DesignerCategory("Code")]
    public class ComboBoxListEx : ComboBox
    {
        private const int CB_GETCURSEL = 0x0147;
        private int listItem = -1;
        IntPtr listBoxHandle = IntPtr.Zero;

        public event EventHandler<ListItemSelectionChangedEventArgs> ListItemSelectionChanged;

        protected virtual void OnListItemSelectionChanged(ListItemSelectionChangedEventArgs e)
            => this.ListItemSelectionChanged?.Invoke(this, e);

        public ComboBoxListEx() { }

        // .Net Framework prior to 4.8 - get the handle of the ListBox
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            listBoxHandle = GetComboBoxListInternal(this.Handle);
        }

        protected override void WndProc(ref Message m)
        {
            int selItem = -1;
            base.WndProc(ref m);

            switch (m.Msg)
            {
                case CB_GETCURSEL:
                    selItem = m.Result.ToInt32();
                    break;
                // .Net Framework prior to 4.8
                // case CB_GETCURSEL can be left there or removed: it's always -1
                case 0x0134:
                    selItem = SendMessage(listBoxHandle, LB_GETCURSEL, 0, 0);
                    break;
                default:
                    // Add Case switches to handle other events
                    break;
            }
            if (listItem != selItem)
            {
                listItem = selItem;
                OnListItemSelectionChanged(new ListItemSelectionChangedEventArgs(
                    listItem, listItem < 0 ? string.Empty : GetItemText(Items[listItem]))
                );
            }
        }

        public class ListItemSelectionChangedEventArgs : EventArgs
        {
            public ListItemSelectionChangedEventArgs(int idx, string text)
            {
                ItemIndex = idx;
                ItemText = text;
            }
            public int ItemIndex { get; private set; }
            public string ItemText { get; private set; }
        }

        // -------------------------------------------------------------
        // .Net Framework prior to 4.8
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool GetComboBoxInfo(IntPtr hWnd, ref COMBOBOXINFO pcbi);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int SendMessage(IntPtr hWnd, uint uMsg, int wParam, int lParam);

        private const int LB_GETCURSEL = 0x0188;

        [StructLayout(LayoutKind.Sequential)]
        internal struct COMBOBOXINFO
        {
            public int cbSize;
            public Rectangle rcItem;
            public Rectangle rcButton;
            public int buttonState;
            public IntPtr hwndCombo;
            public IntPtr hwndEdit;
            public IntPtr hwndList;
            public void Init() => this.cbSize = Marshal.SizeOf<COMBOBOXINFO>();
        }

        internal static IntPtr GetComboBoxListInternal(IntPtr cboHandle)
        {
            var cbInfo = new COMBOBOXINFO();
            cbInfo.Init();
            GetComboBoxInfo(cboHandle, ref cbInfo);
            return cbInfo.hwndList;
        }
    }
}
