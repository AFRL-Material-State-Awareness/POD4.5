using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POD.Data;

namespace POD
{
    public partial class NewSourceForm : Form
    {
        bool _checkedNameResult = true;
        List<string> _usedNames = new List<string>();

        public NewSourceForm(string myInitialName, List<string> mySources)
        {
            InitializeComponent();

            _usedNames = mySources;

            NameTextBox.Text = myInitialName;
            NameTextBox.Select();
            NameTextBox.SelectAll();

            CheckName(myInitialName);

            UpdateCheckImage();
        }

        private void CheckName(string myName)
        {
            _checkedNameResult = !_usedNames.Contains(myName);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void UpdateCheckImage()
        {
            if (_checkedNameResult == true)
                CheckResult.Image = imageList1.Images["Check.png"];
            else
                CheckResult.Image = imageList1.Images["X.png"];
        }

        private void NameBox_TextChanged(object sender, EventArgs e)
        {
            string name = NameTextBox.Text;

            CheckName(name);

            UpdateCheckImage();
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void NewName_Closing(object sender, FormClosingEventArgs e)
        {
            if (_checkedNameResult == false)
                e.Cancel = true;

            NameTextBox.SelectAll();
        }

        public string NewName
        {
            get
            {
                return NameTextBox.Text;
            }
        }
    }
}
