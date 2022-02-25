using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POD.Controls;
using System.IO;
using POD.Analyze;
using POD.Data;
using System.Diagnostics;

namespace POD.Wizards.Steps.FullAnalysisProjectSteps
{
    public partial class ProjectPropertiesPanel : WizardPanel
    {
        private string InsertString = "";
        private string _lastProject = "";
        private string _lastParent = "";
        private string _lastAnalyst = "";
        private string _lastAnalystCo = "";
        private string _lastCustomer = "";
        private string _lastCustomerCo = "";
        private string _lastNotes = "";
        private AnalysisList _currentAnalyses;
        private PODToolTip _propertiesToolTip;


        public ProjectPropertiesPanel(PODToolTip tooltip)
            : base(tooltip)
        {
            InitializeComponent();

            StepToolTip = new PODToolTip();

            Paint += ProjectPropertiesPanel_Paint;
            Load += ProjectPropertiesPanel_Load;

            UpdateTextFromMRU(out _lastAnalyst, Globals.GetMRUList(Globals.PODv4AnalystNameFile));
            UpdateTextFromMRU(out _lastAnalystCo, Globals.GetMRUList(Globals.PODv4AnalystCompanyFile));
            UpdateTextFromMRU(out _lastProject, Globals.GetMRUList(Globals.PODv4ProjectFile));
            UpdateTextFromMRU(out _lastParent, Globals.GetMRUList(Globals.PODv4ParentFile));
            UpdateTextFromMRU(out _lastCustomer, Globals.GetMRUList(Globals.PODv4CustomerNameFile));
            UpdateTextFromMRU(out _lastCustomerCo, Globals.GetMRUList(Globals.PODv4CustomerCompanyFile));
            UpdateTextFromMRU(out _lastNotes, Globals.GetMRUList(Globals.PODv4NotesFile));
            UpdateTextFromMRU(out _lastNotes, Globals.GetMRUListMultiLine(Globals.PODv4NotesFile));
            
        }

        void ProjectPropertiesPanel_Load(object sender, EventArgs e)
        {
            if (projectComboBox.Text != InsertString)
            {
                analystComboBox.Select();
                analystCoComboBox.Select();
                projectComboBox.Select();
                parentComboBox.Select();
                customerComboBox.Select();
                customerCoComboBox.Select();
                projectComboBox.Select();
            }

            //ALL TOOLTIPS MUST BE DONE IN LOAD FUNCTION!
            StepToolTip.SetToolTip(analystComboBox, "Name of the analyst.");
            StepToolTip.SetToolTip(analystCoComboBox, "Analyst's employer.");
            StepToolTip.SetToolTip(projectComboBox, "Name of the project.");
            StepToolTip.SetToolTip(parentComboBox, "Main project this project may belong to.");
            StepToolTip.SetToolTip(customerComboBox, "Point of contact.");
            StepToolTip.SetToolTip(customerCoComboBox, "Point of contact's employer.");
            StepToolTip.SetToolTip(notesTextBox, "Write any notes related to the project here.");
        }

        private void ProjectPropertiesPanel_Paint(object sender, PaintEventArgs e)
        {
            /*if (_fixed == false && analystComboBox.Text != null && analystComboBox.Text.Length > 0)
            {
                analystComboBox.Select(0, 0);
                analystCoComboBox.Select(0, 0);
                projectComboBox.Select(0, 0);
                parentComboBox.Select(0, 0);
                customerComboBox.Select(0, 0);
                customerCoComboBox.Select(0, 0);

                _fixed = true;
            }*/
        }

        public void UseLastValues()
        {
            analystComboBox.Text = _lastAnalyst;
            analystCoComboBox.Text = _lastAnalystCo;
            projectComboBox.Text = _lastProject;
            parentComboBox.Text = _lastParent;
            customerComboBox.Text = _lastCustomer;
            customerCoComboBox.Text = _lastCustomerCo;
            notesTextBox.Text = _lastNotes;
        }

        private void UpdateTextFromMRU(out string p, List<string> list)
        {
            if (list.Count > 0)
            {
                p = list[0];
            }
            else
            {
                p = "";
            }
        }

        private void UpdateTextBoxFromMRU(ComboBox p, List<string> list)
        {
            if(list.Count > 0)
            {
                p.Text = list[0];
            }
        }

        public override bool NeedsRefresh(WizardSource mySource)
        {
            bool needRefresh = false;

            //don't refresh on this so text stays selected
            if (Project.Properties.Name == "" && HasInvalidProjectName)
            {
                //make sure it is the active control so user can easily rename the project
                projectComboBox.Text = InsertString;
                projectComboBox.SelectAll();
                ActiveControl = projectComboBox;
                return false;
            }

            needRefresh = (analystComboBox.Text != Project.Properties.Analyst.Name ||
                           analystCoComboBox.Text != Project.Properties.Analyst.Company ||
                           projectComboBox.Text != Project.Properties.Name ||
                           parentComboBox.Text != Project.Properties.Parent ||
                           customerComboBox.Text != Project.Properties.Customer.Name ||
                           customerCoComboBox.Text != Project.Properties.Customer.Company);            

            return needRefresh;

        }

        internal override void PrepareGUI()
        {
            base.PrepareGUI();
        }

        public override void RefreshValues()
        {
            _currentAnalyses = new AnalysisList();
            Project.RequestAnalyses(ref _currentAnalyses);

            UpdateProjectOverview();

            UpdateDropLists();

            if (NeedsRefresh(Source))
            {
                analystComboBox.Text = Project.Properties.Analyst.Name;
                analystCoComboBox.Text = Project.Properties.Analyst.Company;
                projectComboBox.Text = Project.Properties.Name;
                parentComboBox.Text = Project.Properties.Parent;
                customerComboBox.Text = Project.Properties.Customer.Name;
                customerCoComboBox.Text = Project.Properties.Customer.Company;
                notesTextBox.Text = Project.Properties.Notes;
                               
                base.RefreshValues();
            }

            analystComboBox.Focus();
            ActiveControl = analystComboBox;

            Invalidate();
        }

        public bool HideProperties(bool doHide)
        {
            if(doHide)
            {
                mainLayoutTable.Visible = false;
            }
            else
            {
                mainLayoutTable.Visible = true;
            }

            return !doHide;
        }

        Dictionary<string, Control> _graphsAndLabels;

        public override void RefreshOverview()
        {
            UpdateProjectOverview();
        }

        private void UpdateProjectOverview()
        {
            if (_currentAnalyses == null)
                return;
            
            if (_graphsAndLabels == null)
                _graphsAndLabels = new Dictionary<string, Control>();

            foreach(Control control in _graphsAndLabels.Values)
            {
                if((control as Panel) == null)
                    control.Tag = false;
            }

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            _propertiesToolTip = new PODToolTip();

            _propertiesToolTip.AutoPopDelay = 15000;
            _propertiesToolTip.InitialDelay = 750;
            _propertiesToolTip.ShowAlways = false;

            overviewFlowPanel.SuspendLayout();

            //overviewFlowPanel.Controls.Clear();

            var sourceLabels = new List<Label>();
            var sourceLabelNames = new List<String>();

            foreach(DataSource source in Project.Sources)
            {
                foreach (string flaw in source.FlawLabels)
                {
                    var labelName = source.SourceName + "." + flaw;

                    if (!_graphsAndLabels.ContainsKey(labelName))
                    {
                        var label = new Label();
                        label.Text = labelName;
                        label.AutoSize = true;
                        label.Font = new System.Drawing.Font(label.Font.FontFamily, 14.0F, FontStyle.Bold);
                        label.ForeColor = System.Drawing.Color.White;
                        label.Tag = true;
                        _graphsAndLabels.Add(label.Text, label);

                        var zeroPanel = new Panel();
                        zeroPanel.Size = new System.Drawing.Size(0, 0);
                        zeroPanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);
                        zeroPanel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
                        overviewFlowPanel.Controls.Add(label);
                        overviewFlowPanel.SetFlowBreak(label, true);
                        overviewFlowPanel.Controls.Add(zeroPanel);

                        sourceLabels.Add(label);
                        sourceLabelNames.Add(label.Text);
                        
                    }
                    else
                    {
                        var label = _graphsAndLabels[labelName] as Label;

                        if (label != null)
                        {
                            sourceLabels.Add(label);
                            sourceLabelNames.Add(label.Text);
                            label.Tag = true;
                        }
                    }

                }
            }

            var reverseList = new List<Analysis>();

            foreach(Analysis analysis in _currentAnalyses)
            {
                reverseList.Add(analysis);
            }

            reverseList.Reverse();

            foreach (Analysis analysis in reverseList)
            {
                if (!_graphsAndLabels.ContainsKey(analysis.Name))
                {
                    var chart = new PODChart();
                    chart.SetupChart(analysis.Data.AvailableFlawNames[0], analysis.Data.AvailableFlawUnits[0],
                                     analysis.Data.AvailableResponseNames, analysis.Data.AvailableResponseUnits);
                    chart.Click += chart_Click;
                    chart.ChartToolTip = _propertiesToolTip;
                    chart.DoubleClick += chart_DoubleClick;
                    var overview = new OverviewChart("POD Curve", analysis.ShortName, chart);
                    overview.Tag = true;
                    overview.FullAnalysisName = analysis.Name;

                    if (analysis.Data.PodCurveTable.Rows.Count > 0)
                    {
                        chart.FillChart(analysis.Data, true);
                        chart.SetXAxisRange(analysis.Data.GetUncensoredXBufferedRange(false), analysis.Data, true);
                        chart.UpdateLevelConfidenceLines(analysis.OutResponseDecisionPODA50Value,
                                                         analysis.OutResponseDecisionPODLevelValue,
                                                         analysis.OutResponseDecisionPODConfidenceValue);
                    }

                    _propertiesToolTip.SetToolTip(chart, analysis.ToolTipText);

                    var index = sourceLabelNames.IndexOf(analysis.SourceName + "." + analysis.FlawName);
                    var label = sourceLabels[index];
                    var positionIndex = overviewFlowPanel.Controls.IndexOf(label);
                    overviewFlowPanel.Controls.Add(overview);
                    overviewFlowPanel.Controls.SetChildIndex(overview, positionIndex + 2);

                    _graphsAndLabels.Add(overview.FullAnalysisName, overview);
                }
                else
                {
                    var overview = _graphsAndLabels[analysis.Name] as OverviewChart;

                    if (overview != null)
                    {
                        overview.Tag = true;

                        var chart = overview.Chart as PODChart;

                        if (chart != null)
                        {
                            if (analysis.Data.PodCurveTable.Rows.Count > 0)
                            {
                                _propertiesToolTip.SetToolTip(chart, analysis.ToolTipText);
                                chart.FillChart(analysis.Data, true);
                                chart.SetXAxisRange(analysis.Data.GetUncensoredXBufferedRange(false), analysis.Data, true);
                                chart.UpdateLevelConfidenceLines(analysis.OutResponseDecisionPODA50Value,
                                                                    analysis.OutResponseDecisionPODLevelValue,
                                                                    analysis.OutResponseDecisionPODConfidenceValue);
                            }
                        }
                    }
                }
            }

            foreach(Label label in sourceLabels)
            {
                var positionIndex = overviewFlowPanel.Controls.IndexOf(label);
                positionIndex--;

                if (positionIndex > 0)
                {
                    var prevControl = overviewFlowPanel.Controls[positionIndex];
                    var area = prevControl as OverviewChart;
                    
                    if(area != null)
                        overviewFlowPanel.SetFlowBreak(prevControl, true);
                }
            }

            var removed = new List<string>();


            //if something wasn't flagged as used than we need to delete it
            foreach (string key in _graphsAndLabels.Keys)
            {
                var control = _graphsAndLabels[key];

                //zero panels never have tags
                if (control as OverviewChart != null)
                {
                    var tag = (bool)control.Tag;

                    if (tag == false)
                    {
                        if(overviewFlowPanel.GetFlowBreak(control))
                        {
                            var index = overviewFlowPanel.Controls.GetChildIndex(control);

                            if (index != 0)
                                index--;

                            overviewFlowPanel.SetFlowBreak(overviewFlowPanel.Controls[index], true);
                        }

                        overviewFlowPanel.Controls.Remove(control);

                        removed.Add(key);
                    }
                }
            }

            //delete it from the dictionary too
            foreach(string key in removed)
               _graphsAndLabels.Remove(key);

            removed.Clear();

            //clean up labels with no charts
            foreach(Control control in overviewFlowPanel.Controls)
            {
                var label = control as Label;

                if(label != null)
                {
                    var index = overviewFlowPanel.Controls.GetChildIndex(control);
                    var nextIndex = 0;

                    //use 2, cause also have zero panels right after labels
                    if (index < overviewFlowPanel.Controls.Count - 2)
                    {
                        nextIndex = index + 2;

                        var nextLabel = overviewFlowPanel.Controls[nextIndex] as Label;

                        //if next control is a label
                        if (nextLabel != null)
                        {
                            RemoveLabel(label, index);
                            removed.Add(label.Text);
                        }
                    }
                    else if(index == overviewFlowPanel.Controls.Count - 2)
                    {
                        RemoveLabel(label, index);
                        removed.Add(label.Text);
                    }

                }
            }

            foreach (string key in removed)
                _graphsAndLabels.Remove(key);

            if(_graphsAndLabels.Count > 0)
            {
                NoChartLabel.Visible = false;
            }
            else
            {
                NoChartLabel.Visible = true;
            }

            overviewFlowPanel.ResumeLayout(false);
            overviewFlowPanel.PerformLayout();

            stopWatch.Stop();
        }

        private void RemoveLabel(Label label, int index)
        {
            //remove zero panel
            overviewFlowPanel.Controls.Remove(overviewFlowPanel.Controls[index + 1]);
            //remove chartless label
            overviewFlowPanel.Controls.Remove(label);
        }
               

        void chart_DoubleClick(object sender, EventArgs e)
        {
            var chart = sender as DataPointChart;

            if (chart != null && !chart.MenuIsOpen)
            {
                var overview = chart.Parent.Parent as OverviewChart;

                if (overview != null)
                {
                    Project.OpenAnalysis(overview.FullAnalysisName);
                }
            }
        }

        void chart_Click(object sender, EventArgs e)
        {
            var chart = sender as DataPointChart;

            if(chart != null)
            {
                foreach(Control control in overviewFlowPanel.Controls)
                {
                    var overview = control as OverviewChart;

                    if(overview != null)
                    {
                        var checkChart = overview.Chart;

                        if (checkChart != null && chart != checkChart && checkChart.IsSelected)
                        {
                            checkChart.ForceSelectionOff();
                        }
                    }
                }
            }
        }

        private void comboBox1_DrawItem(object sender, DrawItemEventArgs e)
        {

        }

        void comboBox1_DropDownClosed(object sender, EventArgs e)
        {
            var comboBox1 = sender as ComboBox;

            try
            {
                if (comboBox1 != null && this.Handle != null)
                    this.BeginInvoke(new Action(() => { comboBox1.Select(0, 0); }));
            }
            catch(Exception exp)
            {
                MessageBox.Show("comboBox1_DropDownClosed: " + exp.Message);
            }
        }

        private void UpdateDropLists()
        {
            analystComboBox.Items.Clear();
            analystCoComboBox.Items.Clear();
            projectComboBox.Items.Clear();
            parentComboBox.Items.Clear();
            customerComboBox.Items.Clear();
            customerCoComboBox.Items.Clear();

            analystComboBox.Items.AddRange(Globals.GetMRUListWithoutEmpties(Globals.PODv4AnalystNameFile).ToArray());
            analystCoComboBox.Items.AddRange(Globals.GetMRUListWithoutEmpties(Globals.PODv4AnalystCompanyFile).ToArray());
            projectComboBox.Items.AddRange(Globals.GetMRUListWithoutEmpties(Globals.PODv4ProjectFile).ToArray());
            parentComboBox.Items.AddRange(Globals.GetMRUListWithoutEmpties(Globals.PODv4ParentFile).ToArray());
            customerComboBox.Items.AddRange(Globals.GetMRUListWithoutEmpties(Globals.PODv4CustomerNameFile).ToArray());
            customerCoComboBox.Items.AddRange(Globals.GetMRUListWithoutEmpties(Globals.PODv4CustomerCompanyFile).ToArray());
        }

        public void CopySettingsToProject()
        {
            if (HasInvalidProjectName)
                return;

            Project.Properties.Analyst.Name = analystComboBox.Text;
            Project.Properties.Analyst.Company = analystCoComboBox.Text;

            Project.Properties.Customer.Name = customerComboBox.Text;
            Project.Properties.Customer.Company = customerCoComboBox.Text;

            Project.Properties.Name = projectComboBox.Text;
            Project.Properties.Parent = parentComboBox.Text;
            Project.Properties.Notes = notesTextBox.Text;

            CopyProjectSettingsToMRULists();
        }

        private void CopyProjectSettingsToMRULists()
        {
            Globals.UpdateMRUList(Project.Properties.Analyst.Name, Globals.PODv4AnalystNameFile);
            Globals.UpdateMRUList(Project.Properties.Analyst.Company, Globals.PODv4AnalystCompanyFile);
            Globals.UpdateMRUList(Project.Properties.Customer.Name, Globals.PODv4CustomerNameFile);
            Globals.UpdateMRUList(Project.Properties.Customer.Company, Globals.PODv4CustomerCompanyFile);
            Globals.UpdateMRUList(Project.Properties.Name, Globals.PODv4ProjectFile);
            Globals.UpdateMRUList(Project.Properties.Parent, Globals.PODv4ParentFile);
            Globals.UpdateMRUListMultiLine(Project.Properties.Notes + "|", Globals.PODv4NotesFile);
        }

        

        public override bool CheckStuck()
        {
            if (HasInvalidProjectName)
            {
                MessageBox.Show("Please name the project before continuing any further.");

                ActiveControl = projectComboBox;
                projectComboBox.Text = InsertString;
                projectComboBox.SelectAll();

                return true;
            }

            return false;
        }

        public bool HasInvalidProjectName
        {
            get
            {
                return projectComboBox.Text.Length == 0 || projectComboBox.Text == InsertString;
            }
        }

        private void MainTable_Resize(object sender, EventArgs e)
        {

        }

        
    }
}
