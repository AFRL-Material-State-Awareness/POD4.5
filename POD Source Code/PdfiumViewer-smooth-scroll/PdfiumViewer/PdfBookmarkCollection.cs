using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Runtime.InteropServices;

#pragma warning disable 1591

namespace PdfiumViewer
{
    public class PdfBookmark
    {
        public string Title { get; set; }
        public int PageIndex { get; set; }

        public PdfBookmarkCollection Children { get; }

        public bool HasX { get; set; }
        public bool HasY { get; set; }
        public float X { get; set; }
        public float Y { get; set; }

        public PdfBookmark()
        {
            Children = new PdfBookmarkCollection();
        }

        public override string ToString()
        {
            return Title;
        }
    }

    public class PdfBookmarkCollection : Collection<PdfBookmark>
    {
    }

    
}
