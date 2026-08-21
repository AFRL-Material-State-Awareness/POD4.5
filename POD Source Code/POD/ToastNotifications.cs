using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace POD
{
    public class ToastNotification : Form
    {
        private Timer lifeTimer;
        private Timer fadeTimer;
        private double fadeStep = 0.05;
        private string messageText;
        private Image previewImage;

        // Configuration for the image thumbnail
        private int maxThumbSize = 72;
        private int imagePadding = 20;

        // 1. Enable Native Windows Drop Shadow
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= 0x00020000; // CS_DROPSHADOW
                return cp;
            }
        }

        // 2. Prevent the toast from stealing focus from your main window!
        protected override bool ShowWithoutActivation => true;

        public ToastNotification(string message, int durationMs, Image image = null)
        {
            messageText = message;
            previewImage = image;

            // Form styling
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Opacity = 0;
            this.BackColor = Color.FromArgb(32, 32, 32);
            this.DoubleBuffered = true;

            // 3. Dynamically measure the text and image to size the toast perfectly
            using (Graphics g = this.CreateGraphics())
            using (Font font = new Font("Segoe UI", 12F, FontStyle.Bold))
            {
                SizeF textSize = g.MeasureString(messageText, font, 500);

                int safeTextWidth = (int)Math.Ceiling(textSize.Width) + 5;

                int formWidth;
                int formHeight = (int)textSize.Height + 50;

                if (previewImage != null)
                {
                    formWidth = 20 + maxThumbSize + imagePadding + safeTextWidth + 20;

                    // Ensure the form is tall enough to fit the larger image + top/bottom padding
                    formHeight = Math.Max(formHeight, maxThumbSize + 40);
                }
                else
                {
                    // Text only
                    formWidth = safeTextWidth + 80;
                }

                this.Size = new Size(formWidth, formHeight);
            }

            lifeTimer = new Timer { Interval = durationMs };
            lifeTimer.Tick += LifeTimer_Tick;

            fadeTimer = new Timer { Interval = 15 };
            fadeTimer.Tick += FadeTimer_Tick;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Draw smooth rounded corners
            Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
            int radius = 12;

            using (GraphicsPath path = GetRoundedPath(rect, radius))
            {
                this.Region = new Region(path);

                // Draw a subtle lighter border around the edge
                using (Pen pen = new Pen(Color.FromArgb(60, 60, 60), 1.5f))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            }

            int textX = 0;
            int textWidth = this.Width;
            StringAlignment textAlignment = StringAlignment.Center;

            // Draw the preview image if one was provided
            if (previewImage != null)
            {
                // Calculate dimensions to preserve aspect ratio
                float ratioX = (float)maxThumbSize / previewImage.Width;
                float ratioY = (float)maxThumbSize / previewImage.Height;
                float ratio = Math.Min(ratioX, ratioY);

                int scaledWidth = (int)(previewImage.Width * ratio);
                int scaledHeight = (int)(previewImage.Height * ratio);

                int imgX = 20; // Left margin
                // Center the image vertically, and horizontally within its reserved space
                int actualImgX = imgX + (maxThumbSize - scaledWidth) / 2;
                int actualImgY = (this.Height - scaledHeight) / 2;

                // High quality image scaling
                e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                e.Graphics.DrawImage(previewImage, actualImgX, actualImgY, scaledWidth, scaledHeight);

                // Adjust the text area to start after the image and leave exactly 20px on the right
                textX = imgX + maxThumbSize + imagePadding;
                textWidth = this.Width - textX - 20;
                textAlignment = StringAlignment.Near; // Left-align next to image
            }

            // Draw the text
            using (Font font = new Font("Segoe UI", 12F, FontStyle.Bold))
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(240, 240, 240)))
            using (StringFormat format = new StringFormat { Alignment = textAlignment, LineAlignment = StringAlignment.Center })
            {
                e.Graphics.DrawString(messageText, font, brush, new Rectangle(textX, 0, textWidth, this.Height), format);
            }
        }

        // Helper math for the rounded corners
        private GraphicsPath GetRoundedPath(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Center on screen
            Screen currentScreen = Screen.FromPoint(Cursor.Position);
            this.Location = new Point(
                currentScreen.Bounds.X + (currentScreen.Bounds.Width - this.Width) / 2,
                currentScreen.Bounds.Y + (currentScreen.Bounds.Height - this.Height) / 2
            );

            fadeTimer.Start();
            lifeTimer.Start();
        }

        private void FadeTimer_Tick(object sender, EventArgs e)
        {
            if (lifeTimer.Enabled)
            {
                // Max opacity 0.95 looks slightly translucent and sleek
                if (this.Opacity < 0.95) this.Opacity += fadeStep;
                else fadeTimer.Stop();
            }
            else
            {
                if (this.Opacity > 0) this.Opacity -= fadeStep;
                else
                {
                    fadeTimer.Stop();
                    this.Close();
                }
            }
        }

        private void LifeTimer_Tick(object sender, EventArgs e)
        {
            lifeTimer.Stop();
            fadeTimer.Start();
        }

        // Overload 1: Text only
        public static void Show(string message, int durationMs = 3000)
        {
            ToastNotification toast = new ToastNotification(message, durationMs, null);
            toast.Show();
        }

        // Overload 2: Text AND Image
        public static void Show(string message, Image image, int durationMs = 3000)
        {
            ToastNotification toast = new ToastNotification(message, durationMs, image);
            toast.Show();
        }
    }
}