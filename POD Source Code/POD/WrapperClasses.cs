using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POD
{
    /// <summary>
    /// This interface and class are used for dependency injection for unit testing methods containing MessageBox.Show(...)
    /// </summary>
    public interface IMessageBoxWrap
    {
        DialogResult Show(string text, string caption);
        DialogResult Show(string text);
    }
    public class MessageBoxWrap : IMessageBoxWrap
    {
        public DialogResult Show(string text, string caption)
        {
            return MessageBox.Show(text, caption);
        }
        public DialogResult Show(string text)
        {
            return MessageBox.Show(text);
        }
    }
    public interface IFileExistsWrapper
    {
        bool Exists(string path);
    }
    public class FileExistsWrapper : IFileExistsWrapper
    {
        public bool Exists(string path)
        {
            return File.Exists(path);
        }
    }
    public interface IStreamReaderWrapper
    {
        StreamReader CreateStreamReader(string path, Encoding encoding);
    }
    public class StreamReaderWrapper : IStreamReaderWrapper
    {
        public StreamReader CreateStreamReader(string path, Encoding encoding)
        {
            return new StreamReader(path, encoding);
        }
    }

    public class PasteFromClipBoardWrapper : IPasteFromClipBoardWrapper
    {
        public string GetClipBoardContents(TextDataFormat format)
        {
            return Clipboard.GetText(format);
        }
    }
    public interface IPasteFromClipBoardWrapper
    {
        string GetClipBoardContents(TextDataFormat format);
    }

    public class RichTextBoxWrapper : IRichTextBoxWrapper
    {
        private readonly RichTextBox richTextBox;
        public RichTextBoxWrapper(RichTextBox rtbInput)
        {
            this.richTextBox = rtbInput;
        }

        public int Width
        {
            set { this.richTextBox.Width = value; }
            get { return this.richTextBox.Width; }
        }
        public int Height
        {
            set { this.richTextBox.Height = value; }
            get { return this.richTextBox.Height; }
        }
        public Size Size
        {
            set { this.richTextBox.Size = value; }
            get { return this.richTextBox.Size; }
        }
        public void Dispose() => this.richTextBox.Dispose();
        public bool IsDisposed => this.richTextBox.IsDisposed;
        public void Update() => this.richTextBox.Update();
        public Point PointToScreen(Point p) => this.richTextBox.PointToScreen(p);

        public static explicit operator RichTextBox(RichTextBoxWrapper wrapper) => wrapper.richTextBox;

    }
    public interface IRichTextBoxWrapper
    {
        int Width { get; set; }
        int Height { get; set; }
        Size Size { get; set; }
        void Dispose();
        bool IsDisposed { get; }
        void Update();
        Point PointToScreen(Point p);
    }
    public class DataTableWrapper : IDataTableWrapper
    {
        private DataTable _dataTable;
        public DataTableWrapper(DataTable inputTable)
        {
            _dataTable = inputTable;
        }
        public DataRowCollection Rows => _dataTable.Rows;
        public DataColumnCollection Columns => _dataTable.Columns;
    }
    public interface IDataTableWrapper
    {
        DataRowCollection Rows { get; }
        DataColumnCollection Columns { get; }
    }
}
