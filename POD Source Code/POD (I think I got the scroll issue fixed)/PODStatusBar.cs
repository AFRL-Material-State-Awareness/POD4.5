using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POD
{
    public partial class PODStatusBar : StatusStrip
    {
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel5;        

        public PODStatusBar()
        {
            InitializeComponent();

            AddItems();
        }

        private void AddItems()
        {
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel5 = new System.Windows.Forms.ToolStripStatusLabel();

            // 
            // statusStrip1
            // 
            this.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripProgressBar1,
            this.toolStripStatusLabel4,
            this.toolStripStatusLabel5,
            this.toolStripStatusLabel2,
            this.toolStripStatusLabel3});
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(52, 17);
            this.toolStripStatusLabel1.Text = "Progress";
            this.toolStripStatusLabel1.Font = new Font(this.toolStripStatusLabel1.Font, FontStyle.Bold);
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(85, 17);
            this.toolStripStatusLabel2.Text = "Current Status:";
            this.toolStripStatusLabel2.Font = new Font(this.toolStripStatusLabel2.Font, FontStyle.Bold);
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(34, 17);
            this.toolStripStatusLabel3.Text = "Looks good.";

            // 
            // toolStripStatusLabel4
            // 
            this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            this.toolStripStatusLabel4.Size = new System.Drawing.Size(85, 17);
            this.toolStripStatusLabel4.Text = "Error Report:";
            this.toolStripStatusLabel4.Font = new Font(this.toolStripStatusLabel4.Font, FontStyle.Bold);
            // 
            // toolStripStatusLabel5
            // 
            this.toolStripStatusLabel5.Name = "toolStripStatusLabel5";
            this.toolStripStatusLabel5.Size = new System.Drawing.Size(34, 17);
            this.toolStripStatusLabel5.Text = "";
            this.toolStripStatusLabel5.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            this.toolStripStatusLabel5.Image = imageList1.Images[0];

        }

        //override so the dockpanelsyite doesn't crash
        public override string Text
        {
            get
            {
                return "";
            }

            set
            {
                string temporary = value;
            }
        }


        public string ProgressText
        {
            get
            {
                string value = "";

                this.Invoke((MethodInvoker)delegate()
                {
                    value = toolStripStatusLabel3.Text;
                });

                return value;
            }

            set
            {
                this.Invoke((MethodInvoker)delegate()
                {
                    toolStripStatusLabel3.Text = value;
                });
            }
        }

        public string AddErrorText(string myErrorText)
        {
            if(toolStripStatusLabel5.Text.Length > 0)
                toolStripStatusLabel5.Text = toolStripStatusLabel5.Text + "; " + myErrorText;
            else
                toolStripStatusLabel5.Text = myErrorText;

            toolStripStatusLabel5.Image = imageList1.Images[2];

            return toolStripStatusLabel5.Text;
        }

        public string ResetErrorText()
        {
            toolStripStatusLabel5.Text = "";
            toolStripStatusLabel5.Image = imageList1.Images[0];

            return toolStripStatusLabel5.Text;
        }

        public int ProgressValue
        {
            get
            {
                int value = 0;

                this.Invoke((MethodInvoker)delegate()
                {
                    value = toolStripProgressBar1.Value;
                });

                return value;
            }

            set
            {
                this.Invoke((MethodInvoker)delegate()
                {
                    toolStripProgressBar1.Value = value;
                });
            }
        }
    }
}
