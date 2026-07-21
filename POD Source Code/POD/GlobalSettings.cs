using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace POD.Data
{
    public enum HelpViewMode
    {
        Single,
        Dual,
        Large
    }

    public enum AnalysisContext
    {
        FullAHat,
        FullHitMiss,
        QuickAHat,
        QuickHitMiss
    }

    public class GlobalSettings
    {
        // --- 1. THE SINGLETON INSTANCE ---
        private static GlobalSettings _instance;

        [JsonIgnore]
        public static GlobalSettings Instance => _instance ?? (_instance = Load());

        // --- 2. THE FILE LOCATION ---
        private static readonly string FolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PODProfessional");
        private static readonly string FilePath = Path.Combine(FolderPath, "settings.json");


        // --- 3. CORE LAYOUT PROPERTIES ---

        private HelpViewMode _helpView = HelpViewMode.Single;
        [JsonConverter(typeof(StringEnumConverter))]
        public HelpViewMode HelpView
        {
            get => _helpView;
            set
            {
                if (_helpView != value)
                {
                    _helpView = value;
                    Save();
                }
            }
        }

        private bool _projectManagerPinned = true;
        public bool ProjectManagerPinned
        {
            get => _projectManagerPinned;
            set
            {
                if (_projectManagerPinned != value)
                {
                    _projectManagerPinned = value;
                    Save();
                }
            }
        }

        private bool _projectProgressPinned = true;
        public bool ProjectProgressPinned
        {
            get => _projectProgressPinned;
            set
            {
                if (_projectProgressPinned != value)
                {
                    _projectProgressPinned = value;
                    Save();
                }
            }
        }


        // --- 4. PANEL AND SPLIT DISTANCE TRACKING ---

        private int _leftSidePanelWidth = 250;
        public int LeftSidePanelWidth
        {
            get => _leftSidePanelWidth;
            set
            {
                if (_leftSidePanelWidth != value)
                {
                    _leftSidePanelWidth = value;
                    Save();
                }
            }
        }

        private int _rightSidePanelWidthNormal = 250;
        public int RightSidePanelWidthNormal
        {
            get => _rightSidePanelWidthNormal;
            set
            {
                if (_rightSidePanelWidthNormal != value)
                {
                    _rightSidePanelWidthNormal = value;
                    Save();
                }
            }
        }

        private int _rightSidePanelWidthLarge = 450;
        public int RightSidePanelWidthLarge
        {
            get => _rightSidePanelWidthLarge;
            set
            {
                if (_rightSidePanelWidthLarge != value)
                {
                    _rightSidePanelWidthLarge = value;
                    Save();
                }
            }
        }

        private int _helpViewVerticalSplitNormal = 300; // Vertical split size for Normal view
        public int HelpViewVerticalSplitNormal
        {
            get => _helpViewVerticalSplitNormal;
            set
            {
                if (_helpViewVerticalSplitNormal != value)
                {
                    _helpViewVerticalSplitNormal = value;
                    Save();
                }
            }
        }

        private int _helpViewVerticalSplitLarge = 500; // Vertical split size for Large view
        public int HelpViewVerticalSplitLarge
        {
            get => _helpViewVerticalSplitLarge;
            set
            {
                if (_helpViewVerticalSplitLarge != value)
                {
                    _helpViewVerticalSplitLarge = value;
                    Save();
                }
            }
        }


        // --- 5. PIPELINE SPECIFIC CHART COUNTS ---

        private int _fullAHatSideChartsCount = 4;
        public int FullAHatSideChartsCount
        {
            get => _fullAHatSideChartsCount;
            set { if (_fullAHatSideChartsCount != value) { _fullAHatSideChartsCount = value; Save(); } }
        }

        private int _fullHitMissSideChartsCount = 4;
        public int FullHitMissSideChartsCount
        {
            get => _fullHitMissSideChartsCount;
            set { if (_fullHitMissSideChartsCount != value) { _fullHitMissSideChartsCount = value; Save(); } }
        }

        private int _quickAHatSideChartsCount = 4;
        public int QuickAHatSideChartsCount
        {
            get => _quickAHatSideChartsCount;
            set { if (_quickAHatSideChartsCount != value) { _quickAHatSideChartsCount = value; Save(); } }
        }

        private int _quickHitMissSideChartsCount = 4;
        public int QuickHitMissSideChartsCount
        {
            get => _quickHitMissSideChartsCount;
            set { if (_quickHitMissSideChartsCount != value) { _quickHitMissSideChartsCount = value; Save(); } }
        }


        // --- 6. PIPELINE SPECIFIC HIDDEN CHARTS LISTS ---

        private List<string> _fullAHatHiddenCharts = new List<string>();
        public List<string> FullAHatHiddenCharts
        {
            get => _fullAHatHiddenCharts;
            set { _fullAHatHiddenCharts = value ?? new List<string>(); Save(); }
        }

        private List<string> _fullHitMissHiddenCharts = new List<string>();
        public List<string> FullHitMissHiddenCharts
        {
            get => _fullHitMissHiddenCharts;
            set { _fullHitMissHiddenCharts = value ?? new List<string>(); Save(); }
        }

        private List<string> _quickAHatHiddenCharts = new List<string>();
        public List<string> QuickAHatHiddenCharts
        {
            get => _quickAHatHiddenCharts;
            set { _quickAHatHiddenCharts = value ?? new List<string>(); Save(); }
        }

        private List<string> _quickHitMissHiddenCharts = new List<string>();
        public List<string> QuickHitMissHiddenCharts
        {
            get => _quickHitMissHiddenCharts;
            set { _quickHitMissHiddenCharts = value ?? new List<string>(); Save(); }
        }


        // --- 7. CONTEXT-AWARE VISIBILITY HELPERS ---

        public void HideChart(AnalysisContext context, string chartName)
        {
            if (string.IsNullOrEmpty(chartName)) return;
            List<string> targetList = GetRuntimeList(context);

            if (!targetList.Contains(chartName))
            {
                targetList.Add(chartName);
                Save();
            }
        }

        public void ShowChart(AnalysisContext context, string chartName)
        {
            if (string.IsNullOrEmpty(chartName)) return;
            List<string> targetList = GetRuntimeList(context);

            if (targetList.Contains(chartName))
            {
                targetList.Remove(chartName);
                Save();
            }
        }

        public bool IsChartHidden(AnalysisContext context, string chartName)
        {
            if (string.IsNullOrEmpty(chartName)) return false;
            return GetRuntimeList(context).Contains(chartName);
        }

        public int GetSideChartsCount(AnalysisContext context)
        {
            switch (context)
            {
                case AnalysisContext.FullAHat: return FullAHatSideChartsCount;
                case AnalysisContext.FullHitMiss: return FullHitMissSideChartsCount;
                case AnalysisContext.QuickAHat: return QuickAHatSideChartsCount;
                case AnalysisContext.QuickHitMiss: return QuickHitMissSideChartsCount;
                default: return 4;
            }
        }

        public void SetSideChartsCount(AnalysisContext context, int count)
        {
            switch (context)
            {
                case AnalysisContext.FullAHat: FullAHatSideChartsCount = count; break;
                case AnalysisContext.FullHitMiss: FullHitMissSideChartsCount = count; break;
                case AnalysisContext.QuickAHat: QuickAHatSideChartsCount = count; break;
                case AnalysisContext.QuickHitMiss: QuickHitMissSideChartsCount = count; break;
            }
        }

        private List<string> GetRuntimeList(AnalysisContext context)
        {
            switch (context)
            {
                case AnalysisContext.FullAHat: return FullAHatHiddenCharts;
                case AnalysisContext.FullHitMiss: return FullHitMissHiddenCharts;
                case AnalysisContext.QuickAHat: return QuickAHatHiddenCharts;
                case AnalysisContext.QuickHitMiss: return QuickHitMissHiddenCharts;
                default: return new List<string>();
            }
        }


        // --- 8. JSON ENGINE ---

        private static GlobalSettings Load()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    string json = File.ReadAllText(FilePath);
                    return JsonConvert.DeserializeObject<GlobalSettings>(json);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load settings: {ex.Message}");
            }

            return new GlobalSettings();
        }

        public void Save()
        {
            try
            {
                if (!Directory.Exists(FolderPath))
                {
                    Directory.CreateDirectory(FolderPath);
                }

                string json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(FilePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save settings: {ex.Message}");
            }
        }
    }
}