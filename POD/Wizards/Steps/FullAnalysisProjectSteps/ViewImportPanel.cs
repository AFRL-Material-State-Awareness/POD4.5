using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using POD.Controls;
using POD.Data;

namespace POD.Wizards.Steps.FullAnalysisProjectSteps
{
    public partial class ViewImportPanel : WizardPanel
    {
        public ViewInfoPanel DefaultPanel
        {
            get
            {
                if (DataSourceTabs.TabPages.Count == 0)
                {
                    return null;
                }
                else
                {
                    if (DataSourceTabs.SelectedTab.Controls.Count == 0)
                        return null;

                    return DataSourceTabs.SelectedTab.Controls[0] as ViewInfoPanel;
                }
            }
        }

        private int _currentDataSourceIndex;

        private int _numberOfDataSources;
        private bool _lastStack;
        private bool _lastView;
        private ViewInfoPanel _lastPanel;

        public ViewImportPanel(PODToolTip tooltip)
            : base(tooltip)
        {

            InitializeComponent();

            StepToolTip = new PODToolTip();
                       
            _currentDataSourceIndex = 0;
            _numberOfDataSources = 0;

            _lastStack = true;
            _lastView = false;

            //DefaultPanel.Source = Project.Sources[0];
        }

        internal override void PrepareGUI()
        {
            _numberOfDataSources = Project.Sources.Count;

            //add any existing tabs
            for (int i = 0; i < _numberOfDataSources; i++)
            {
                AddNewInfoPageQuick(Project.Sources[i]);
            }
        }

        public override void RefreshValues()
        {
            DataSourceTabs.SuspendLayout();

            var selectFirst = false;

            if (DataSourceTabs.TabCount == 0)
                selectFirst = true;


            _numberOfDataSources = Project.Sources.Count;

            //add any existing tabs
            for (int i = 0; i < _numberOfDataSources; i++)
            {
                var source = Project.Sources[i];
                var info = Project.Infos.GetFromOriginalName(source.SourceName);

                AddNewInfoPage(source, info);

                ViewInfoPanel.EstimateMinMaxValues(source, info);
            }

            //get rdi of any pages where source was deleted
            var deleteTabs = new List<TabPage>();

            foreach(TabPage page in DataSourceTabs.TabPages)
            {
                var source = Project.Sources[page.Text];

                if (source == null)
                    deleteTabs.Add(page);
            }

            foreach (var page in deleteTabs)
                DataSourceTabs.TabPages.Remove(page);

            if(selectFirst && DataSourceTabs.TabPages.Count > 0)
            {
                DataSourceTabs.SelectedIndex = 0;
            }

            DataSourceTabs.ResumeLayout();

            DataSource_Changed(null, null);
        }

        private void AddNewInfoPageQuick(DataSource source)
        {
            TabPage page = null;
            //ViewInfoPanel panel = null;
            var name = "";
            if (source != null)
                name = source.SourceName;
            else
                return;

            if (!DataSourceTabs.TabPages.ContainsKey(name))
            {
                DataSourceTabs.TabPages.Add(name, name);

                page = DataSourceTabs.TabPages[name];
                page.Padding = new Padding(0, 0, 3, 3);

                CreatePanelForPage(page, false);
            }
        }

        private void AddNewInfoPage(DataSource source, SourceInfo info)
        {
            TabPage page = null;
            ViewInfoPanel panel = null;
            var name = "";
            if (source != null)
                name = source.SourceName;
            else
                return;

            if (!DataSourceTabs.TabPages.ContainsKey(name))
            {
                DataSourceTabs.TabPages.Add(name, name);

                page = DataSourceTabs.TabPages[name];
                page.Padding = new Padding(0, 0, 3, 3);

                CreatePanelForPage(page, true);
            }
            else
            {
                page = DataSourceTabs.TabPages[name];

                if (page.Controls.Count > 0)
                {
                    panel = page.Controls[0] as ViewInfoPanel;

                    UpdatePanel(page, panel, true);
                }
            }
        }

        private void DataSource_Changed(object sender, EventArgs e)
        {
            if (DataSourceTabs.TabPages.Count == 0)
            {
                return;
            }

            var page = DataSourceTabs.SelectedTab;

            DataSourceTabs.SuspendLayout();

            CreatePanelForPage(page, true);

            DataSourceTabs.ResumeLayout();
        }

        private void CreatePanelForPage(TabPage page, bool forceViewInfoRefresh)
        {            
            ViewInfoPanel panel = null;

            if (page.Controls.Count == 0)
            {
                panel = new ViewInfoPanel();
            }
            else
            {
                panel = page.Controls[0] as ViewInfoPanel;
            }

            UpdatePanel(page, panel, forceViewInfoRefresh);
        }

        private void UpdatePanel(TabPage page, ViewInfoPanel panel, bool forceViewInfoRefresh)
        {
            if (page != null && panel != null)
            {
                panel.SuspendDrawing();

                if (!page.Controls.Contains(panel))
                {
                    page.Controls.Add(panel);
                    panel.Dock = DockStyle.Fill;
                }

                if(Project.Sources[page.Text] == null)
                {
                    DataSourceTabs.TabPages.Remove(page);

                    return;
                }

                if (!forceViewInfoRefresh)
                {
                    panel.ResumeDrawing();
                    return;
                }

                panel.SuspendLayout();

                panel.HandleUserInteraction = false;
                
                panel.Source = Project.Sources[page.Text];
                panel.Info = Project.Infos.GetFromOriginalName(page.Text);

                panel.RefreshListBoxes();
                panel.RefreshMetaDataControls();

                panel.RefreshDataTable();

                if (_lastPanel != null)
                {
                    panel.SetSplitterSizes(_lastPanel.InputTableSplitterSize, _lastPanel.TableGraphsSplitterSize);
                }

                if (panel.StackAll != _lastStack)
                    panel.SwitchStackMode();

                if (panel.ViewMode != _lastView)
                    panel.SwitchViewMode();

                panel.ClearGraphs();
                panel.FillGraphs();

                //select first graph
                panel.RefreshChartSelection();

                panel.ResizePanel();

                panel.HandleUserInteraction = true;

                panel.ResumeLayout();

                DataSourceTabs.ResumeLayout();

                panel.ResumeDrawing();

                _lastPanel = panel;
            }           
        }

        public void CopyInfoToSource()
        {
            var infoPanels = GetAvailableInfoPanels();

            foreach (var panel in infoPanels)
                panel.UpdateSourceFromInfo();
        }

        private List<ViewInfoPanel> GetAvailableInfoPanels()
        {
            var list = new List<ViewInfoPanel>();

            foreach (TabPage page in DataSourceTabs.TabPages)
            {
                if (page.Controls.Count > 0)
                {
                    var infoPanel = page.Controls[0] as ViewInfoPanel;

                    if (infoPanel != null)
                    {
                        list.Add(infoPanel);
                    }
                }
            }

            return list;
        }        

        public bool SwitchViewMode()
        {
            var infoPanels = GetAvailableInfoPanels();

            foreach (var panel in infoPanels)
            {
                panel.SwitchViewMode();
                panel.ResizePanel();
            }

            if (DefaultPanel == null)
            {
                _lastView = false;
                return false;
            }

            _lastView = DefaultPanel.ViewMode;

            return DefaultPanel.ViewMode;
        }

        private void Resize_Panel(object sender, EventArgs e)
        {
            if (DefaultPanel != null)
            {
                DefaultPanel.ResizePanel();

                //fixes bug with scroll bars not displaying when window is maximized
                //DefaultPanel.GraphsPanel.AutoScroll = false;
                //DefaultPanel.GraphsPanel.AutoScroll = !DefaultPanel.StackAll;
            }
        }

        public bool SwitchStackMode()
        {
            var infoPanels = GetAvailableInfoPanels();

            foreach (var panel in infoPanels)
            {
                panel.SwitchStackMode();
                panel.ClearGraphs();
                panel.FillGraphs();
                panel.ResizePanel();
            }

            if (DefaultPanel == null)
            {
                _lastStack = false;
                return false;
            }

            _lastStack = DefaultPanel.StackAll;

            return DefaultPanel.StackAll;
        }

        

        /*public void PreviousDataSource()
        {
            if (_currentDataSourceIndex == 0)
            {
                _currentDataSourceIndex = _numberOfDataSources - 1;
            }
            else
            {
                _currentDataSourceIndex--;
            }

            if (DefaultPanel != null)
                DefaultPanel.ClearDataTable();

            this.RefreshValues();
        }

        public void NextDataSource()
        {
            if (_currentDataSourceIndex < _numberOfDataSources - 1)
            {
                _currentDataSourceIndex++;
            }
            else
            {
                _currentDataSourceIndex = 0;
            }

            if (DefaultPanel != null)
                DefaultPanel.ClearDataTable();

            this.RefreshValues();
        }*/
    }
}
