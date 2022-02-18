namespace POD.Docks
{
    partial class WizardProgressDock
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

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Intro");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Project Settings");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Copy and Paste");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Verify Import");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Import", new System.Windows.Forms.TreeNode[] {
            treeNode3,
            treeNode4});
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Create Analyses");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Augment Data");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Select Analysis");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Finished");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizardProgressDock));
            this.treeView1 = new System.Windows.Forms.TreeView();
            ((System.ComponentModel.ISupportInitialize)(this._blendBox)).BeginInit();
            this.SuspendLayout();
            // 
            // _blendBox
            // 
            this._blendBox.Size = new System.Drawing.Size(284, 262);
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeView1.FullRowSelect = true;
            this.treeView1.HideSelection = false;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            treeNode1.Name = "Intro";
            treeNode1.Text = "Intro";
            treeNode2.Name = "Node4";
            treeNode2.Text = "Project Settings";
            treeNode3.Name = "copy";
            treeNode3.Text = "Copy and Paste";
            treeNode4.Name = "Validate";
            treeNode4.Text = "Verify Import";
            treeNode5.Name = "Import";
            treeNode5.Text = "Import";
            treeNode6.Name = "Select Fit";
            treeNode6.Text = "Create Analyses";
            treeNode7.Name = "Node2";
            treeNode7.Text = "Augment Data";
            treeNode8.Name = "Node3";
            treeNode8.Text = "Select Analysis";
            treeNode9.Name = "Node5";
            treeNode9.Text = "Finished";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode5,
            treeNode6,
            treeNode7,
            treeNode8,
            treeNode9});
            this.treeView1.ShowLines = false;
            this.treeView1.Size = new System.Drawing.Size(284, 262);
            this.treeView1.TabIndex = 0;
            this.treeView1.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.Item_Select);
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.Item_Selected);
            // 
            // WizardProgressDock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.treeView1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "WizardProgressDock";
            this.TabText = "Project Setup Progress";
            this.Text = "WizardTree";
            this.Controls.SetChildIndex(this.treeView1, 0);
            this.Controls.SetChildIndex(this._blendBox, 0);
            ((System.ComponentModel.ISupportInitialize)(this._blendBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
    }
}