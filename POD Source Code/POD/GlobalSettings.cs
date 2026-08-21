using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace POD
{
    public enum AnalysisContext { FullAHat, FullHitMiss, QuickAHat, QuickHitMiss }

    public class GlobalSettings
    {
        [JsonIgnore]
        public bool IsFirstRun { get; private set; } = true;

        private static GlobalSettings _instance;

        [JsonIgnore]
        public static GlobalSettings Instance => _instance ?? (_instance = Load());

        private static readonly string FolderPath = Globals.PODv4Folder;
        private static readonly string FilePath = Path.Combine(FolderPath, "settings.json");

        // --- PROPERTIES ---

        private ColorFilterType _colorFilter = ColorFilterType.None;
        [JsonConverter(typeof(StringEnumConverter))]
        public ColorFilterType ColorFilter { get => _colorFilter; set { if (_colorFilter != value) { _colorFilter = value; Save(); } } }

        private HelpView _helpView = HelpView.Small;
        [JsonConverter(typeof(StringEnumConverter))]
        public HelpView HelpViewSetting { get => _helpView; set { if (_helpView != value) { _helpView = value; Save(); } } }

        private bool _projectManagerPinned = true;
        public bool ProjectManagerPinned { get => _projectManagerPinned; set { if (_projectManagerPinned != value) { _projectManagerPinned = value; Save(); } } }

        private bool _projectProgressPinned = true;
        public bool ProjectProgressPinned { get => _projectProgressPinned; set { if (_projectProgressPinned != value) { _projectProgressPinned = value; Save(); } } }

        private int _leftSidePanelWidth = 250;
        public int LeftSidePanelWidth { get => _leftSidePanelWidth; set { if (_leftSidePanelWidth != value) { _leftSidePanelWidth = value; Save(); } } }

        private int _rightSidePanelWidthNormal = 250;
        public int RightSidePanelWidthNormal { get => _rightSidePanelWidthNormal; set { if (_rightSidePanelWidthNormal != value) { _rightSidePanelWidthNormal = value; Save(); } } }

        private int _rightSidePanelWidthLarge = 450;
        public int RightSidePanelWidthLarge { get => _rightSidePanelWidthLarge; set { if (_rightSidePanelWidthLarge != value) { _rightSidePanelWidthLarge = value; Save(); } } }

        private int _helpViewVerticalSplitNormal = 300;
        public int HelpViewVerticalSplitNormal { get => _helpViewVerticalSplitNormal; set { if (_helpViewVerticalSplitNormal != value) { _helpViewVerticalSplitNormal = value; Save(); } } }

        private int _helpViewVerticalSplitLarge = 500;
        public int HelpViewVerticalSplitLarge { get => _helpViewVerticalSplitLarge; set { if (_helpViewVerticalSplitLarge != value) { _helpViewVerticalSplitLarge = value; Save(); } } }

        // --- PIPELINE SPECIFIC CHART COUNTS ---

        private int _fullAHatSideChartsCount = 4;
        public int FullAHatSideChartsCount { get => _fullAHatSideChartsCount; set { if (_fullAHatSideChartsCount != value) { _fullAHatSideChartsCount = value; Save(); } } }

        private int _fullHitMissSideChartsCount = 4;
        public int FullHitMissSideChartsCount { get => _fullHitMissSideChartsCount; set { if (_fullHitMissSideChartsCount != value) { _fullHitMissSideChartsCount = value; Save(); } } }

        private int _quickAHatSideChartsCount = 4;
        public int QuickAHatSideChartsCount { get => _quickAHatSideChartsCount; set { if (_quickAHatSideChartsCount != value) { _quickAHatSideChartsCount = value; Save(); } } }

        private int _quickHitMissSideChartsCount = 4;
        public int QuickHitMissSideChartsCount { get => _quickHitMissSideChartsCount; set { if (_quickHitMissSideChartsCount != value) { _quickHitMissSideChartsCount = value; Save(); } } }

        // --- PIPELINE SPECIFIC HIDDEN CHARTS LISTS ---

        private List<string> _fullAHatHiddenCharts = new List<string>();
        public List<string> FullAHatHiddenCharts { get => _fullAHatHiddenCharts; set { _fullAHatHiddenCharts = value ?? new List<string>(); Save(); } }

        private List<string> _fullHitMissHiddenCharts = new List<string>();
        public List<string> FullHitMissHiddenCharts { get => _fullHitMissHiddenCharts; set { _fullHitMissHiddenCharts = value ?? new List<string>(); Save(); } }

        private List<string> _quickAHatHiddenCharts = new List<string>();
        public List<string> QuickAHatHiddenCharts { get => _quickAHatHiddenCharts; set { _quickAHatHiddenCharts = value ?? new List<string>(); Save(); } }

        private List<string> _quickHitMissHiddenCharts = new List<string>();
        public List<string> QuickHitMissHiddenCharts { get => _quickHitMissHiddenCharts; set { _quickHitMissHiddenCharts = value ?? new List<string>(); Save(); } }

        // --- JSON ENGINE ---

        private static GlobalSettings Load()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    string json = File.ReadAllText(FilePath);
                    var loadedSettings = JsonConvert.DeserializeObject<GlobalSettings>(json);

                    if (loadedSettings != null)
                    {
                        // We successfully loaded from the file, so it's NOT the first run
                        loadedSettings.IsFirstRun = false;
                        return loadedSettings;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load settings: {ex.Message}");
            }

            // If no file exists or it failed to load, return a fresh instance (IsFirstRun stays true)
            return new GlobalSettings();
        }

        public void Save()
        {
            try
            {
                if (!Directory.Exists(FolderPath)) Directory.CreateDirectory(FolderPath);
                string json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(FilePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save settings: {ex.Message}");
            }
        }

        private void ApplyFirstRunHeuristics()
        {
            try
            {
                // Grab the primary monitor's working area (excludes taskbar)
                int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;

                if (GlobalSettings.Instance.IsFirstRun)
                {
                    if (HasMultipleMonitors())
                    {
                        GlobalSettings.Instance.HelpViewSetting = HelpView.Dual;
                    }

                    if (IsWideScreen())
                    {
                        GlobalSettings.Instance.HelpViewSetting = HelpView.Large;
                    }
                }

                //// Apply smart defaults based on resolution
                //if (screenWidth > 1920) // Ultrawide or 4K
                //{
                //    _rightSidePanelWidthNormal = 350;
                //    _helpViewVerticalSplitNormal = 400;
                //    _helpView = HelpView.Dual;
                //}
                //else if (screenWidth > 1366) // Standard 1080p
                //{
                //    _rightSidePanelWidthNormal = 250;
                //    _helpViewVerticalSplitNormal = 300;
                //    _helpView = HelpView.Small;
                //}
                //else // Small laptops
                //{
                //    _rightSidePanelWidthNormal = 200;
                //    _helpViewVerticalSplitNormal = 200;
                //    _helpView = HelpView.Small;
                //}
            }
            catch
            {
                // If checking the screen fails for any weird reason, 
                // the class will naturally fall back to the default values assigned to the fields.
            }
        }

        private bool HasMultipleMonitors()
        {
            // Get the number of connected screens.
            int screenCount = Screen.AllScreens.Length;

            // Check if there is more than one screen.
            return screenCount > 1;
        }

        protected static bool IsWideScreen()
        {
            // Get the resolution of the primary screen.
            Screen primaryScreen = Screen.PrimaryScreen;
            Rectangle bounds = primaryScreen.Bounds;
            int width = bounds.Width;
            int height = bounds.Height;

            // Calculate the aspect ratio.
            double aspectRatio = (double)width / height;

            // Check if the aspect ratio is wider than 16/9.
            if (aspectRatio > 17.0 / 9.0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}