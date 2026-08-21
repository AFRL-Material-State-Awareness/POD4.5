using System;
using System.IO;
using System.Runtime.InteropServices;

namespace POD
{
    public static class FileExplorerUtility
    {
        // P/Invoke for freeing PIDLs (Pointer to an Item ID List)
        [DllImport("shell32.dll", ExactSpelling = true)]
        private static extern void ILFree(IntPtr pidlList);

        // P/Invoke to convert a file path to a PIDL
        [DllImport("shell32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern IntPtr ILCreateFromPathW(string pszPath);

        // P/Invoke to open a folder and select specific items
        [DllImport("shell32.dll")]
        private static extern int SHOpenFolderAndSelectItems(
            IntPtr pidlFolder,
            uint cidl,
            [In, MarshalAs(UnmanagedType.LPArray)] IntPtr[] apidl,
            int dwFlags);

        /// <summary>
        /// Opens File Explorer, navigates to the folder, and highlights the specified file.
        /// If the folder is already open, it brings the window to the front and selects the new file.
        /// </summary>
        /// <param name="filePath">The full path of the file to highlight.</param>
        public static void OpenFolderAndHighlightFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));

            filePath = Path.GetFullPath(filePath);

            // Verify the target actually exists before trying to open it
            if (!File.Exists(filePath) && !Directory.Exists(filePath))
                return;

            string dirPath = Path.GetDirectoryName(filePath);
            if (string.IsNullOrEmpty(dirPath))
                return; // Occurs if the path is a root drive

            IntPtr dirPidl = IntPtr.Zero;
            IntPtr filePidl = IntPtr.Zero;

            try
            {
                // Create PIDLs for both the directory and the file
                dirPidl = ILCreateFromPathW(dirPath);
                filePidl = ILCreateFromPathW(filePath);

                if (dirPidl != IntPtr.Zero && filePidl != IntPtr.Zero)
                {
                    // 0 flag = Default behavior (reuse existing window if open, highlight item)
                    SHOpenFolderAndSelectItems(dirPidl, 1, new IntPtr[] { filePidl }, 0);
                }
            }
            finally
            {
                // Clean up unmanaged memory to prevent leaks
                if (dirPidl != IntPtr.Zero) ILFree(dirPidl);
                if (filePidl != IntPtr.Zero) ILFree(filePidl);
            }
        }
    }
}