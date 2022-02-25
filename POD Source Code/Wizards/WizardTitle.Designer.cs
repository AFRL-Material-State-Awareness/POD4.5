using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POD.Wizards
{
    partial class WizardTitle
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this._header = new System.Windows.Forms.Label();
            this._dash = new System.Windows.Forms.Label();
            this._subHeader = new System.Windows.Forms.Label();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.BackColor = System.Drawing.Color.LightSteelBlue;
            this.flowLayoutPanel2.Controls.Add(this._header);
            this.flowLayoutPanel2.Controls.Add(this._dash);
            this.flowLayoutPanel2.Controls.Add(this._subHeader);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(782, 37);
            this.flowLayoutPanel2.TabIndex = 2;
            this.flowLayoutPanel2.WrapContents = false;
            // 
            // _header
            // 
            this._header.AutoSize = true;
            this._header.Dock = System.Windows.Forms.DockStyle.Fill;
            this._header.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._header.ForeColor = System.Drawing.Color.White;
            this._header.Location = new System.Drawing.Point(3, 0);
            this._header.Name = "_header";
            this._header.Size = new System.Drawing.Size(109, 33);
            this._header.TabIndex = 1;
            this._header.Text = "Header";
            // 
            // _dash
            // 
            this._dash.AutoSize = true;
            this._dash.Dock = System.Windows.Forms.DockStyle.Fill;
            this._dash.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._dash.ForeColor = System.Drawing.Color.White;
            this._dash.Location = new System.Drawing.Point(118, 0);
            this._dash.Name = "_dash";
            this._dash.Size = new System.Drawing.Size(25, 33);
            this._dash.TabIndex = 2;
            this._dash.Text = "-";
            // 
            // _subHeader
            // 
            this._subHeader.AutoSize = true;
            this._subHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.SetFlowBreak(this._subHeader, true);
            this._subHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._subHeader.ForeColor = System.Drawing.Color.White;
            this._subHeader.Location = new System.Drawing.Point(149, 0);
            this._subHeader.Name = "_subHeader";
            this._subHeader.Size = new System.Drawing.Size(120, 33);
            this._subHeader.TabIndex = 0;
            this._subHeader.Text = "SubHeader";
            this._subHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // WizardTitle
            // 
            this.AutoScroll = true;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.Controls.Add(this.flowLayoutPanel2);
            this.Name = "WizardTitle";
            this.Size = new System.Drawing.Size(782, 37);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private FlowLayoutPanel flowLayoutPanel2;
        private Label _header;
        private Label _dash;
        private Label _subHeader;
    }
}
