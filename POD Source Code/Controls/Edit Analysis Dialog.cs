using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POD.Controls
{
    public partial class EditAnalysisDialog : Form
    {

        public EditAnalysisDialog()
        {
            InitializeComponent();
        }

        public EditAnalysisDialog(SimpleActionBar myAvailableBar, TableLayoutPanel myAvailablePanel,
                                  SimpleActionBar myAnalysisBar, TableLayoutPanel myAnalysisPanel)
        {

            InitializeComponent();

            createTableLayout.Controls.Clear();

            createTableLayout.Controls.Add(myAvailableBar, 0, 1);
            createTableLayout.Controls.Add(myAvailablePanel, 1, 1);
            createTableLayout.Controls.Add(myAnalysisBar, 2, 1);
            createTableLayout.Controls.Add(myAnalysisPanel, 3, 1);

            this.DialogResult = DialogResult.Cancel;
        }

        private void CloseDialog_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;

            

            Close();
        }

        public Button OkayButton
        {
            get
            {
                return UpdateBtn;
            }
        }

        private void From_Closed(object sender, FormClosedEventArgs e)
        {
            createTableLayout.Controls.Clear();
        }
    }
}
