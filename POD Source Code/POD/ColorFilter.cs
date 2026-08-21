using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace POD
{
    public enum ColorFilterType
    {
        None = 0,
        ProtanopiaSimulation = 1,
        DeuteranopiaSimulation = 2,
        TritanopiaSimulation = 3,
        ProtanopiaCorrection = 4,
        DeuteranopiaCorrection = 5,
        TritanopiaCorrection = 6
    }

    public static class ColorManager
    {
        // Inner metadata structure for cached controls
        private class ImageBinding
        {
            public Control TargetControl { get; set; }
            public PropertyInfo TargetProperty { get; set; }
            public Image OriginalImage { get; set; }
            public bool IsPreviewControl { get; set; } // Set via Tag = "FilterPreview"
        }

        private static ColorFilterType _currentFilter = ColorFilterType.None;

        // Dictionary: [Form Instance] -> List of [Control Image Bindings]
        private static readonly Dictionary<Form, List<ImageBinding>> _registeredForms = new Dictionary<Form, List<ImageBinding>>();

        public static ColorFilterType CurrentFilter => _currentFilter;

        /// <summary>
        /// Call this ONCE in any Form's Load or Constructor event.
        /// Recursively pre-processes all controls with image properties into a dictionary.
        /// </summary>
        public static void RegisterForm(Form form)
        {
            if (form == null || _registeredForms.ContainsKey(form)) return;

            var bindings = new List<ImageBinding>();

            // 1. Recursive Preprocessor Scan
            PreprocessControlTreeRecursive(form, bindings);

            _registeredForms[form] = bindings;

            // 2. Auto-Unregister when Form Closes (Prevents Memory Leaks)
            form.FormClosed += (s, e) => UnregisterForm(form);

            // 3. Immediately apply current active global filter to the newly opened form
            ApplyFilterToForm(form, _currentFilter);
        }

        /// <summary>
        /// Sets the active filter software-wide and updates ALL open, registered forms instantly.
        /// </summary>
        public static void SetGlobalFilter(ColorFilterType newFilter)
        {
            _currentFilter = newFilter;

            foreach (var form in _registeredForms.Keys)
            {
                ApplyFilterToForm(form, _currentFilter);
            }
        }

        /// <summary>
        /// Transforms a standalone image using the currently active global color filter.
        /// </summary>
        public static Image TransformImage(Image original)
        {
            return ProcessImageRouting(original, _currentFilter);
        }

        /// <summary>
        /// Transforms a standalone image using a specific color filter type.
        /// </summary>
        public static Image TransformImage(Image original, ColorFilterType filterType)
        {
            return ProcessImageRouting(original, filterType);
        }

        // --- RECURSIVE PREPROCESSOR ---
        private static void PreprocessControlTreeRecursive(Control parent, List<ImageBinding> bindings)
        {
            foreach (Control control in parent.Controls)
            {
                bool isPreview = control.Tag != null && control.Tag.ToString().Equals("FilterPreview", StringComparison.OrdinalIgnoreCase);

                // A. Check for 'Image' Property (PictureBox, Button, Label, etc.)
                PropertyInfo imageProp = control.GetType().GetProperty("Image", BindingFlags.Public | BindingFlags.Instance);
                if (imageProp != null && typeof(Image).IsAssignableFrom(imageProp.PropertyType))
                {
                    Image img = imageProp.GetValue(control) as Image;
                    if (img != null)
                    {
                        bindings.Add(new ImageBinding
                        {
                            TargetControl = control,
                            TargetProperty = imageProp,
                            OriginalImage = (Image)img.Clone(), // Cache pristine original copy
                            IsPreviewControl = isPreview
                        });
                    }
                }

                // B. Check for 'BackgroundImage' Property (Panels, Forms, GroupBoxes, etc.)
                PropertyInfo bgProp = control.GetType().GetProperty("BackgroundImage", BindingFlags.Public | BindingFlags.Instance);
                if (bgProp != null && typeof(Image).IsAssignableFrom(bgProp.PropertyType))
                {
                    Image bgImg = bgProp.GetValue(control) as Image;
                    if (bgImg != null)
                    {
                        bindings.Add(new ImageBinding
                        {
                            TargetControl = control,
                            TargetProperty = bgProp,
                            OriginalImage = (Image)bgImg.Clone(), // Cache pristine original copy
                            IsPreviewControl = isPreview
                        });
                    }
                }

                // C. Drill down recursively through container controls
                if (control.HasChildren)
                {
                    PreprocessControlTreeRecursive(control, bindings);
                }
            }
        }

        // --- FORM FILTER APPLIER ---
        private static void ApplyFilterToForm(Form form, ColorFilterType filter)
        {
            if (!_registeredForms.TryGetValue(form, out var bindings)) return;

            bool isFilterActive = (filter != ColorFilterType.None);

            foreach (var binding in bindings)
            {
                if (binding.TargetControl.IsDisposed) continue;

                // Handle preview grid visibility (hides if filter is None)
                if (binding.IsPreviewControl)
                {
                    binding.TargetControl.Visible = isFilterActive;
                    if (!isFilterActive) continue;
                }

                // Get currently displayed image so we can safely dispose it later
                Image currentDisplayedImg = binding.TargetProperty.GetValue(binding.TargetControl) as Image;

                if (!isFilterActive)
                {
                    // Restore original pristine image
                    binding.TargetProperty.SetValue(binding.TargetControl, binding.OriginalImage);
                }
                else
                {
                    // Process and set new image from the CACHED original value
                    Image processedImg = ProcessImageRouting(binding.OriginalImage, filter);
                    binding.TargetProperty.SetValue(binding.TargetControl, processedImg);
                }

                // Safe Memory Disposal: Dispose the old rendered bitmap if it's not our cached original
                if (currentDisplayedImg != null && currentDisplayedImg != binding.OriginalImage)
                {
                    currentDisplayedImg.Dispose();
                }
            }
        }

        // --- CLEANUP ---
        private static void UnregisterForm(Form form)
        {
            if (_registeredForms.TryGetValue(form, out var bindings))
            {
                foreach (var binding in bindings)
                {
                    binding.OriginalImage?.Dispose();
                }
                _registeredForms.Remove(form);
            }
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static void ApplyColorMath(ref float r, ref float g, ref float b, ColorFilterType filterType, bool isCorrection)
        {
            float rSim = r, gSim = g, bSim = b;

            // 1. MACHADO MATRICES
            if (filterType == ColorFilterType.ProtanopiaSimulation || filterType == ColorFilterType.ProtanopiaCorrection)
            {
                rSim = (0.152286f * r) + (1.052583f * g) + (-0.204868f * b);
                gSim = (0.114503f * r) + (0.786281f * g) + (0.099216f * b);
                bSim = (-0.003882f * r) + (-0.048116f * g) + (1.051998f * b);
            }
            else if (filterType == ColorFilterType.DeuteranopiaSimulation || filterType == ColorFilterType.DeuteranopiaCorrection)
            {
                rSim = (0.367322f * r) + (0.860646f * g) + (-0.227968f * b);
                gSim = (0.280085f * r) + (0.672501f * g) + (0.047413f * b);
                bSim = (-0.011820f * r) + (0.042940f * g) + (0.968881f * b);
            }
            else if (filterType == ColorFilterType.TritanopiaSimulation || filterType == ColorFilterType.TritanopiaCorrection)
            {
                rSim = (1.255528f * r) + (-0.076749f * g) + (-0.178779f * b);
                gSim = (-0.078411f * r) + (0.930809f * g) + (0.147602f * b);
                bSim = (0.004733f * r) + (0.691367f * g) + (0.303900f * b);
            }

            // 2. CORRECTION ALGORITHM
            if (isCorrection)
            {
                float rErr = r - rSim;
                float gErr = g - gSim;
                float bErr = b - bSim;

                if (filterType == ColorFilterType.ProtanopiaCorrection)
                {
                    g = g + (0.7f * rErr);
                    b = b + (1.0f * rErr);
                }
                else if (filterType == ColorFilterType.DeuteranopiaCorrection)
                {
                    r = r + (1.5f * gErr);
                    b = b + (2.0f * gErr);
                }
                else if (filterType == ColorFilterType.TritanopiaCorrection)
                {
                    r = r + (1.0f * bErr);
                    g = g + (0.7f * bErr);
                }
            }
            else
            {
                // For standard simulations, just return the simulated color
                r = rSim;
                g = gSim;
                b = bSim;
            }
        }

        public static Color TransformColor(Color original)
        {
            return TransformColor(original, _currentFilter);
        }

        public static Color TransformColor(Color original, ColorFilterType filterType)
        {
            if (filterType == ColorFilterType.None || original.A == 0) return original;

            float r = original.R;
            float g = original.G;
            float b = original.B;

            bool isCorrection = filterType == ColorFilterType.ProtanopiaCorrection ||
                                filterType == ColorFilterType.DeuteranopiaCorrection ||
                                filterType == ColorFilterType.TritanopiaCorrection;

            // Run the shared math
            ApplyColorMath(ref r, ref g, ref b, filterType, isCorrection);

            // Clamp values safely between 0 and 255
            int finalR = (int)Math.Max(0, Math.Min(255, r));
            int finalG = (int)Math.Max(0, Math.Min(255, g));
            int finalB = (int)Math.Max(0, Math.Min(255, b));

            return Color.FromArgb(original.A, finalR, finalG, finalB);
        }

        private static Image ProcessImageRouting(Image original, ColorFilterType filterType)
        {
            if (original == null) return null;
            if (filterType == ColorFilterType.None) return new Bitmap(original);

            Bitmap bmp = new Bitmap(original);
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbValues = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, rgbValues, 0, bytes);

            bool isCorrection = filterType == ColorFilterType.ProtanopiaCorrection ||
                                filterType == ColorFilterType.DeuteranopiaCorrection ||
                                filterType == ColorFilterType.TritanopiaCorrection;

            for (int i = 0; i < rgbValues.Length; i += 4)
            {
                float b = rgbValues[i];
                float g = rgbValues[i + 1];
                float r = rgbValues[i + 2];
                byte a = rgbValues[i + 3];

                if (a == 0) continue; // Skip transparent pixels

                // Run the shared math
                ApplyColorMath(ref r, ref g, ref b, filterType, isCorrection);

                // Clamp values safely between 0 and 255
                rgbValues[i] = (byte)Math.Max(0, Math.Min(255, b));
                rgbValues[i + 1] = (byte)Math.Max(0, Math.Min(255, g));
                rgbValues[i + 2] = (byte)Math.Max(0, Math.Min(255, r));
            }

            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, bmpData.Scan0, bytes);
            bmp.UnlockBits(bmpData);

            return bmp;
        }
    }
}