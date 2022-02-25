using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POD.Controls;
using System.IO;
using POD.Docks;

namespace POD
{
    [Docking(DockingBehavior.Ask)]
    public partial class InitialSelectionForm : Form
    {
        /*protected override CreateParams CreateParams
        {
            get
            {
                const int WS_EX_COMPOSITED = 0x02000000;
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_COMPOSITED;
                return cp;
            }
        }*/

        HelpView _selectedView = HelpView.Small;
        PDFDisplay _helpDisplay = new PDFDisplay("POD v4 Welcome Window Quick Help.pdf");

        public HelpView SelectedView
        {
            get { return _selectedView; }
            set { _selectedView = value; }
        }
        AnalysisTypeEnum _selectedType;
        Dictionary<Label, string> fileInfo = new Dictionary<Label, string>();

        public AnalysisTypeEnum SelectedType
        {
            get { return _selectedType; }
            set { _selectedType = value; }
        }

        SkillLevelEnum _selectedSkill;

        public SkillLevelEnum SelectedSkill
        {
            get { return _selectedSkill; }
            set { _selectedSkill = value; }
        }

        public InitialSelectionForm()
        {
            InitializeComponent();

            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            _selectedType = AnalysisTypeEnum.Full;
            _selectedView = HelpView.Small;

            BuildMRUList();

            GotSize = false;

            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);

            var help = Globals.GetMRUList(Globals.PODv4HelpViewFile);

            if(help.Count > 0)
            {
                try
                {
                    _selectedView = (HelpView)Enum.Parse(typeof(HelpView), help[0]);

                    switch(_selectedView)
                    {
                        case HelpView.Dual:
                            DualViewButton.Checked = true;
                            break;
                        case HelpView.Large:
                            DualViewButton.Checked = true;
                            break;
                        default:
                            DefaultViewButton.Checked = true;
                            break;
                    }
                }
                catch(ArgumentException)
                {
                    DefaultViewButton.Checked = true;

                }
            }
        }

        int originalExStyle = -1;
        bool enableFormLevelDoubleBuffering = true;

        protected override CreateParams CreateParams
        {
            get
            {
                if (originalExStyle == -1)
                    originalExStyle = base.CreateParams.ExStyle;

                CreateParams cp = base.CreateParams;
                if (enableFormLevelDoubleBuffering)
                    cp.ExStyle |= 0x02000000;   // WS_EX_COMPOSITED
                else
                    cp.ExStyle = originalExStyle;

                return cp;
            }
        }

        private void TurnOffFormLevelDoubleBuffering()
        {
            enableFormLevelDoubleBuffering = false;
            this.MaximizeBox = true;
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            TurnOffFormLevelDoubleBuffering();
        }

        private void PaintForm(object sender, PaintEventArgs e)
        {
            var totalHeight = 0;

            if (GotSize == false)
            {
                var heights = layoutTable.GetRowHeights();
                var row = 0;

                foreach (int rowHeight in heights)
                {
                    var control = layoutTable.GetControlFromPosition(0, row);

                    if (control != null)
                    {
                        totalHeight += rowHeight +4;// 4 is a magic number not sure why this is needed // control.Margin.Top + control.Margin.Bottom;
                    }
                    row++;
                }

                Height = totalHeight;

                this.MinimumSize = new Size(Width, Height+6);

                FinalizeMRUList();

                GotSize = true;
            }
        }

        
        private void BuildMRUList()
        {
            string mruFile = Globals.PODv4MRUFile;
            var lines = new List<string>();

            try
            {
                if (!File.Exists(mruFile) || new FileInfo(mruFile).Length == 0)
                {
                    FileStream fs = System.IO.File.Create(mruFile);
                    fs.Close();

                    var welcomeLabel = new Label();
                    var welcomeMessage = "Welcome!\n\nIt looks like you've never used POD v4 before. How about going through our tutorial before you start?\n\nClick on the Start Tutorial... link above.";

                    SetLabelProperties(welcomeLabel, welcomeMessage);

                    AddLabelToMRUList(welcomeLabel, "");       
                }

                using (StreamReader sr = new StreamReader(mruFile))
                {
                    string readLine = "";

                    layoutTable.SuspendLayout();

                    int filesProcessed = 0;

                    while (readLine != null)
                    {
                        readLine = sr.ReadLine();

                        if(readLine != null)
                        {
                            if(File.Exists(readLine))
                            {
                                if (filesProcessed < 6)
                                {
                                    var fileName = Path.GetFileNameWithoutExtension(readLine);

                                    var label = CreateNewMRULabel(fileName);

                                    AddLabelToMRUList(label, readLine);

                                    string projectInfo = WizardController.DeserializeProjectInfo(readLine);

                                    fileInfo.Add(label, projectInfo);                                    
                                }

                                filesProcessed++;

                                if (filesProcessed <= 10)
                                    lines.Add(readLine);
                            }

                            
                        }

                        
                    }

                    sr.Close();

                    layoutTable.ResumeLayout();

                    

                    int extraWidth = 380 - OverviewTextBox.Width;

                    if (extraWidth < 0)
                        extraWidth = 0;

                    Width += extraWidth;

                    StreamWriter sw = new StreamWriter(mruFile);

                    foreach(string line in lines)
                        sw.WriteLine(line);

                    sw.Close();
                }

                
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "File Read Error");
            }

            
        }

        private void FinalizeMRUList()
        {
            //layoutTable.RowStyles.Remove(layoutTable.RowStyles[layoutTable.RowCount - 1]);
            //layoutTable.RowCount--;


            //layoutTable.RowStyles[layoutTable.RowCount - 1].SizeType = SizeType.Percent;
            //layoutTable.RowStyles[layoutTable.RowCount - 1].Height = 100;
          
        }

        private Label CreateNewMRULabel(string fileName)
        {
            var label = new Label();
            SetLabelProperties(label, fileName);
            SetLabelEvents(label);           

            return label;
        }

        private void SetLabelEvents(Label label)
        {
            label.MouseEnter += Label_Enter;
            label.MouseLeave += Label_Leave;
            label.Click += Project_Click;
        }

        private void SetLabelProperties(Label label, string fileName)
        {
            label.Text = fileName;
            label.ForeColor = Color.White;
            label.Margin = new System.Windows.Forms.Padding(8, 6, 8, 6);
            label.Font = new System.Drawing.Font(label.Font.FontFamily, 9.75F);
            label.AutoSize = true;

            label.MaximumSize = new System.Drawing.Size(Convert.ToInt32(OpenProjectLabel.Width*2.0), 0);
        }

        private void AddLabelToMRUList(Label myLabel, string myToolTip)
        {
            //myLabel.Dock = DockStyle.Fill;
            labelToolTip.SetToolTip(myLabel, myToolTip);
            //layoutTable.RowStyles.Insert(7, new RowStyle(SizeType.AutoSize, 100.0F));
            //layoutTable.RowCount = layoutTable.RowStyles.Count;
            layoutTable.Controls.Add(myLabel, 0, layoutTable.RowCount);
            var index = layoutTable.GetRow(myLabel);

            if(layoutTable.RowStyles.Count >= index)
            {
                layoutTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }

            layoutTable.RowStyles[index].SizeType = SizeType.AutoSize;
            layoutTable.RowStyles[index].Height = 100;

            layoutTable.RowStyles.Add(new RowStyle(SizeType.AutoSize, 100));
            layoutTable.RowCount++;
            
            layoutTable.SetRowSpan(OverviewTextBox, layoutTable.RowCount-1);
            layoutTable.SetCellPosition(myLabel, new TableLayoutPanelCellPosition(0, index));

            layoutTable.SetRowSpan(panel1, layoutTable.GetRowSpan(panel1) + 1);
        }

        void Project_Click(object sender, EventArgs e)
        {
            var label = sender as Label;

            if (label != null)
            {
                SelectedFile = labelToolTip.GetToolTip(label);
                OpenedProject = true;
                Close();
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }       

        private bool _newQuickAHat = false;
        private bool _newProject = false;
        private bool _openedFile = false;
        private bool _startTutorial = false;
        private bool _newQuickHitMiss = false;
        public bool ClosedWithoutSelection = false;
        private bool GotSize;
        

        public bool StartTutorial
        {
            get
            {
                return _startTutorial; 
            }
            set 
            {
                ResetAll(value);

                _startTutorial = value;                
            }
        }

        public bool NewQuickHitMissAnalysis
        {
            get
            {
                return _newQuickHitMiss;
            }
            set
            {
                ResetAll(value);

                _newQuickHitMiss = value;
            }
        }

        public bool NewQuickAHatAnalysis
        {
            get
            {
                return _newQuickAHat;
            }
            set
            {
                ResetAll(value);

                _newQuickAHat = value;
            }
        }
        public bool NewProject
        {
            get
            {
                return _newProject;
            }
            set
            {
                ResetAll(value);

                _newProject = value;
            }
        }
        public bool OpenedProject
        {
            get
            {
                return _openedFile;
            }
            set
            {
                ResetAll(value);

                _openedFile = value;
            }
        }

        private void ResetAll(bool value)
        {
            if (value)
            {
                _startTutorial = false;
                _newProject = false;
                _openedFile = false;
                _newQuickAHat = false;
                _newQuickHitMiss = false;
            }
        }

        

        private void FullAnalysis_Load(object sender, EventArgs e)
        {
            
        }

        private void NewProject_Click(object sender, EventArgs e)
        {
            NewProject = true;
            Close();
        }

        

        private void NewQuick_Click(object sender, EventArgs e)
        {
            if (sender == AHatQuickAnalysisLabel)
                NewQuickAHatAnalysis = true;
            else
                NewQuickHitMissAnalysis = true;
            
            Close();
        }

        private void OpenProject_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "POD Projects (*.pod)|*.pod";
            dialog.Multiselect = false;

            DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                SelectedFile = dialog.FileName;
                OpenedProject = true;
                Close();
            }
            
        }

        private void Label_Enter(object sender, EventArgs e)
        {
            Label label = sender as Label;

            if(label != null)
            {
                label.ForeColor = Color.FromArgb(255, 199, 43);
                label.Font = new Font(label.Font.FontFamily, label.Font.Size, FontStyle.Underline);

                HighlightParentLabel(label);
            }

            if(label == NewProjectLabel)
            {
                OverviewTextBox.Text = "Start a new project containing one or more POD analyses. Each project contains a set of data sources and analyses." + Environment.NewLine + Environment.NewLine +
                                       "The entire project or individual analyses can be exported to an Exceil file so the results can be easily shared with others." + Environment.NewLine + Environment.NewLine + 
                                       "Both aHat vs a and Hit/Miss analyses can be mixed together in a single project.";
            }
            else if (label == HitMissQuickAnalysisLabel)
            {
                OverviewTextBox.Text = "Start a quick Hit/Miss analysis. Quick analyses are not to be used in place of a traditional analysis found in a POD project. It is meant to only be used for reference while generating data." + 
                                       Environment.NewLine + Environment.NewLine + "It is not possible to export the results to Excel but the data entered can be exported for later reference.";
            }
            else if (label == AHatQuickAnalysisLabel)
            {
                OverviewTextBox.Text = "Start a quick aHat vs a analysis. Quick analyses are not to be used in place of a traditional analysis found in a POD project. It is meant to only be used for reference while generating data." +
                                       Environment.NewLine + Environment.NewLine + "It is not possible to export the results to Excel but the data entered can be exported for later reference.";
            }
            else if(label == OpenProjectLabel)
            {
                OverviewTextBox.Text = "Open a previously created project and continue working on your POD analyses. To make getting back to your files as easy as possible, your most recent files can be found in the bottom left corner of this window. Place your mouse over each file to see an overview of the project.";
            }
            else if (label == StartTutorialLabel)
            {
                OverviewTextBox.Text = "If you're unfamilair with POD v4, this will open a quick tutorial that shows the major features of POD v4. It is highly recommended for new users. ";
            }
            else if(fileInfo.ContainsKey(label))
            {
                string info = fileInfo[label];

                OverviewTextBox.Text = info;
            }


        }

        private void HighlightParentLabel(Control control)
        {
            var label = control as Label;

            if (label != null)
            {

                HelpViewLabel.ForeColor = Color.White;

                if (label == NewProjectLabel || label == AHatQuickAnalysisLabel || label == HitMissQuickAnalysisLabel || label == OpenProjectLabel || label == StartTutorialLabel)
                {
                    StartLabel.ForeColor = Color.FromArgb(103, 119, 255);
                    RecentLabel.ForeColor = Color.White;
                }
                else
                {
                    StartLabel.ForeColor = Color.White;
                    RecentLabel.ForeColor = Color.FromArgb(103, 119, 255);
                }
            }
            else
            {
                HelpViewLabel.ForeColor = Color.FromArgb(103, 119, 255);
                StartLabel.ForeColor = Color.White;
                RecentLabel.ForeColor = Color.White;
            }
        }

        private void TextBox_Enter(object sender, EventArgs e)
        {
            HelpViewLabel.ForeColor = Color.White;
            StartLabel.ForeColor = Color.White;
            RecentLabel.ForeColor = Color.White;
        }

        private void Update_HelpView(object sender, EventArgs e)
        {
            RadioButton button = sender as RadioButton;

            if (button != null && button.Checked)
            {
                if (button == DefaultViewButton)
                {
                    SelectedView = HelpView.Small;
                }
                else if (button == LargeViewButton)
                {
                    SelectedView = HelpView.Large;
                }
                else if (button == DualViewButton)
                {
                    SelectedView = HelpView.Dual;
                }
            }

            
        }

        private void HelpView_Enter(object sender, EventArgs e)
        {
            RadioButton button = sender as RadioButton;

            if(button != null)
                button.ForeColor = Color.FromArgb(255, 199, 43);

            if (button == DefaultViewButton)
            {
                OverviewTextBox.Text = "This is the default view for the help files. Help panels, which contain links, are located on the right side. " +
                                       "Clicking on a link will open the help file on the main window. Once, you are done reading, " +
                                       "double click on the help file to return to your analysis window." +
                                       Environment.NewLine + Environment.NewLine +
                                       "You can switch views using the toolbar located at the top of the help file or panel.";
            }
            else if (button == LargeViewButton)
            {
                OverviewTextBox.Text = "This view is designed for monitor with a resolution higher than 1080p. Because of the " +
                                       "extra space available, both the help panels, which contain links, and help files are shown all at once on " +
                                       "the right side of the main window." +
                                       Environment.NewLine + Environment.NewLine +
                                       "You can switch views using the toolbar located at the top of the help file or panel.";
            }
            else if (button == DualViewButton)
            {
                OverviewTextBox.Text = "This view is designed for user's that would like to take advantage of the extra space " +
                                       "on a second monitor. The help panels, which contain links, are located on the right side of the main window " +
                                       "while the help files are located on the second monitor." +
                                       Environment.NewLine + Environment.NewLine + 
                                       "You can switch views using the toolbar located at the top of the help file or panel.";
            }

            OverviewTextBox.Refresh();

            HighlightParentLabel(button);
        }

        private void HelpView_Leave(object sender, EventArgs e)
        {
            RadioButton button = sender as RadioButton;

            if (button != null)
                button.ForeColor = Color.White;
        }

        private void Label_Leave(object sender, EventArgs e)
        {
            Label label = sender as Label;

            if (label != null)
            {
                label.ForeColor = Color.White;
                label.Font = new Font(label.Font.FontFamily, label.Font.Size, FontStyle.Regular);
            }
        }

        private void SelectionForm_Closing(object sender, FormClosingEventArgs e)
        {
            

            Globals.UpdateMRUList(_selectedView.ToString(), Globals.PODv4HelpViewFile);

            if (NewProject == false && NewQuickAHatAnalysis == false && NewQuickHitMissAnalysis == false && OpenedProject == false)
            {
                var result = MessageBox.Show("Are you sure you want to quit POD v4?", "POD v4", MessageBoxButtons.YesNo);

                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    ClosedWithoutSelection = true;
                }
                else
                {
                    e.Cancel = true;
                }
            }

            if(e.Cancel == false)
                _helpDisplay.Dispose();
        }

        public string SelectedFile { get; set; }

        private void StartTutorial_Click(object sender, EventArgs e)
        {
            StartTutorial = true;

            Tutorial form = new Tutorial();

            form.ShowDialog();
        }

        private void Help_MouseEnter(object sender, EventArgs e)
        {
            HelpBox.BackgroundImage = imageList1.Images[1];

            OverviewTextBox.Text = "Click to open help window.";
        }

        private void Help_MouseLeave(object sender, EventArgs e)
        {
            HelpBox.BackgroundImage = imageList1.Images[0];
        }

        private void Help_MouseDown(object sender, MouseEventArgs e)
        {
            HelpBox.BackgroundImage = imageList1.Images[2];
        }

        private void Help_MouseUp(object sender, MouseEventArgs e)
        {
            HelpBox.BackgroundImage = imageList1.Images[1];
        }

        private void Open_Help(object sender, EventArgs e)
        {
            if (_helpDisplay.IsDisposed)
                _helpDisplay = new PDFDisplay("POD v4 Welcome Window Quick Help.pdf");

            _helpDisplay.HideOnClose = true;
            _helpDisplay.PdfFilename = "POD v4 Welcome Window Quick Help.pdf";
            _helpDisplay.OpenSimplePDF();
            _helpDisplay.Show();
            _helpDisplay.FitWidth();
        }

        

      



        
    }
}
