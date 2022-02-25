using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POD;

namespace POD.Controls
{
    public partial class AnalysisType : UserControl
    {
        AnalysisTypeEnum _type;

        public AnalysisTypeEnum Type
        {
            get { return _type; }
            set { _type = value; }
        }
        SkillLevelEnum _skill;

        public SkillLevelEnum Skill
        {
            get { return _skill; }
            set { _skill = value; }
        }
        bool _started;

        public bool Started
        {
            get { return _started; }
            set { _started = value; }
        }

        public AnalysisType()
        {
            InitializeComponent();

            Started = false;
            Skill = SkillLevelEnum.Tutorial;
            Type = AnalysisTypeEnum.Full;
        }

        public void SetType(AnalysisTypeEnum myType)
        {
            switch(myType)
            {
                case AnalysisTypeEnum.Full:
                    AnalysisTypeLabel.Text = "Full Analysis";
                    SkillLevelCombo.Items.Clear();
                    SkillLevelCombo.Items.Add(Globals.SkillTutorialLabel);
                    SkillLevelCombo.Items.Add(Globals.SkillBeginnerLabel);
                    SkillLevelCombo.Items.Add(Globals.SkillNormalLabel);
                    SkillLevelCombo.Items.Add(Globals.SkillExpertLabel);
                    SkillLevelCombo.Text = Globals.SkillTutorialLabel;
                    AnalysisTextBox.Text = "Full Analysis is used to fully evaluate a set of POD data using the criteria set by the Military Handbook 1823A.  If you want to get a quick overview of recently collected data, start a Quick Analysis instead.";
                    SkillTextBox.Text = "Selecting Tutorial Skill Level helps you learn how to use the user interface with a pre-defined problem. This is perfect for first time users.";
                    StartButton.Text = "Start Full Analysis";
                    break;
                case AnalysisTypeEnum.Quick:
                    AnalysisTypeLabel.Text = "Quick Analysis";
                    SkillLevelCombo.Items.Clear();
                    SkillLevelCombo.Items.Add(Globals.SkillTutorialLabel);
                    SkillLevelCombo.Items.Add(Globals.SkillNormalLabel);
                    SkillLevelCombo.Text = Globals.SkillTutorialLabel;
                    AnalysisTextBox.Text = "Quick Analysis is used to quickly evaluate a set of recently collected POD data before continuing the collection process. Results should not be considered usable under any circumstances for any other purpose. Please start a Full Analysis for everything else.";
                    SkillTextBox.Text = "Selecting Tutorial Skill Level helps you learn how to use the user interface with a pre-defined problem. This is perfect for first time users.";
                    StartButton.Text = "Start Quick Analysis";
                    break;
                default:
                    AnalysisTypeLabel.Text = "Unknown Analysis";
                    SkillLevelCombo.Items.Clear();
                    SkillLevelCombo.Items.Add("Unknown");
                    SkillLevelCombo.Text = "Unknown";
                    AnalysisTextBox.Text = "Unknown Analysis type.";
                    SkillTextBox.Text = "Unknown Analysis type.";
                    StartButton.Text = "Start Unknown Analysis";
                    break;
            }

            Type = myType;
            Skill = SkillLevelEnum.Tutorial;
        }

        private void SkillLevel_Changed(object sender, EventArgs e)
        {
            string item = SkillLevelCombo.Text;

            switch(item)
            {
                case Globals.SkillTutorialLabel:
                    Skill = SkillLevelEnum.Tutorial;
                    SkillTextBox.Text = "Tutorial helps you learn how to use the user interface with a pre-defined problem. This is perfect for first time users.";
                    break;
                case Globals.SkillBeginnerLabel:
                    Skill = SkillLevelEnum.Training;
                    SkillTextBox.Text = "Training is meant for users who are familair with the interface but still don't feel comfortable with major POD concepts yet.";
                    break;
               case Globals.SkillNormalLabel:
                    Skill = SkillLevelEnum.Training;
                    SkillTextBox.Text = "Normal is meant for users familar with the interface and POD concepts. They should should be able to handle any issues that arise.";
                    break;
                case Globals.SkillExpertLabel:
                    Skill = SkillLevelEnum.Advanced;
                    SkillTextBox.Text = "Advanced is meant for users who would like aditional tools beyond what Normal offers.";
                    break;
                default:
                    Skill = SkillLevelEnum.Tutorial;
                    SkillTextBox.Text = "Unknown skill level. Defaulting to tutorial.";
                    break;
            }
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            Started = true;

            ParentForm.Close();
        }
    }
}
