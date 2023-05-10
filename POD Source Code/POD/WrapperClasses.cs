using System;
using System.Collections.Generic;
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

    public class PasteToClipBoardWrapper : IPasteToClipBoardWrapper
    {
        public string GetClipBoardContents(TextDataFormat format)
        {
            return Clipboard.GetText(format);
        }
    }
    public interface IPasteToClipBoardWrapper
    {
        string GetClipBoardContents(TextDataFormat format);
    }
}
