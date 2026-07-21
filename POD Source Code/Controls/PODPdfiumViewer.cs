// <copyright file="PODPdfiumViewer.cs" company="The University of Dayton">
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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PdfiumViewer;

namespace POD.Controls
{
    public class PODPdfiumViewer : PdfRenderer
    {
        IPDFLoader _pdfLoader;
        string _pdfFile;
        bool _loaded;
        IPdfDocument _mypdfDocument;

        public string PdfFileName
        {
            get { return _pdfFile; }
            set { _pdfFile = value; }
        }

        // Inject dependencies in constructor for testing purposes (do not use args in code)
        public PODPdfiumViewer(IPDFLoader pdfLoader = null, bool loaded = false,
            IPdfDocument pdfDoc = null)
        {
            _pdfLoader = pdfLoader ?? new PDFLoader();
            _loaded = loaded;
            _mypdfDocument = pdfDoc;
        }

        void PODPdfViewer_PdfLoaded(object sender, EventArgs args)
        {
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == System.Windows.Forms.Keys.PageUp)
            {
                Page -= 1;
                return true;
            }

            if (keyData == System.Windows.Forms.Keys.PageDown)
            {
                Page += 1;
                return true;
            }

            if (keyData == Keys.Home)
            {
                Page = 1;
                return true;
            }

            if (keyData == Keys.End)
            {
                Page = _mypdfDocument.PageCount;
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        Control _active;

        protected override void OnMouseEnter(EventArgs e)
        {
            var form = FindForm();

            _active = form.ActiveControl;

            Focus();

            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (_active != null)
                _active.Focus();

            base.OnMouseLeave(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            Focus();

            base.OnMouseClick(e);
        }

        public bool OpenPDF()
        {
            if (!_loaded)
            {
                _mypdfDocument = _pdfLoader.LoadPDF(this, this, _pdfFile);

                _loaded = true;

                return true;
            }
            return false;
        }

        public int PageCount => _mypdfDocument?.PageCount ?? 0;

        public PdfBookmarkCollection Bookmarks => _mypdfDocument?.Bookmarks;

        // =====================================================================
        // RESTORED METHOD: SCROLL TO SPECIFIC POINT EMBEDDED IN BOOKMARK
        // =====================================================================
        public void ScrollToBookmark(PdfBookmark bookmark)
        {
            if (bookmark == null || _mypdfDocument == null) return;
            if (bookmark.PageIndex < 0 || bookmark.PageIndex >= _mypdfDocument.PageCount) return;

            // 1. Tell the smooth-scroll renderer to jump to the targeted page
            Page = bookmark.PageIndex;

            // 2. If it defines a specific Y coordinate point, calculate precise pixel scroll
            if (bookmark.HasY)
            {
                // Get the physical page height in structural PDF points (e.g., 792 for Letter)
                double pageHeightPoints = _mypdfDocument.PageSizes[bookmark.PageIndex].Height;

                // PDF points start at bottom-left (0=bottom). WinForms starts at top-left (0=top).
                // Invert the point so it represents distance down from the top of the page.
                double invertYPoints = pageHeightPoints - bookmark.Y;

                // Multiply by the renderer's current Zoom factor to convert points to pixels
                double zoomFactor = Zoom;
                int targetPixelOffset = (int)(invertYPoints * zoomFactor);

                // FIX: Use the custom scroll engine's internal positioning hook.
                // We offset the current DisplayRectangle.Y downwards by our target pixel count.
                SetDisplayRectLocation(new Point(DisplayRectangle.X, DisplayRectangle.Y - targetPixelOffset));
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            // Intercept the mouse wheel event, flip the sign of the Delta (-e.Delta),
            // and pass the corrected direction down to the smooth-scroll engine.
            var invertedArgs = new MouseEventArgs(e.Button, e.Clicks, e.X, e.Y, -e.Delta);

            base.OnMouseWheel(invertedArgs);
        }
    }
}