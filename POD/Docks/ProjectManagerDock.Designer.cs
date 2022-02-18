namespace POD.Docks
{
    partial class ProjectDock
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Setup...");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Log Log Fit");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("SemiLog Fit");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Probe 1-(1,2)", new System.Windows.Forms.TreeNode[] {
            treeNode2,
            treeNode3});
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Probe 2-(1,2)");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Probe 3-(1,2)");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Probe (1-1,1-2,2-1,2-2,3-1,3-2)");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Probe 1-1");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Linear Fit");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Specimen 127 Removed");
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("Probe (1-1,2-1,3-1)", new System.Windows.Forms.TreeNode[] {
            treeNode9,
            treeNode10});
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("Analyses", new System.Windows.Forms.TreeNode[] {
            treeNode4,
            treeNode5,
            treeNode6,
            treeNode7,
            treeNode8,
            treeNode11});
            System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("Project Name", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode12});
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectDock));
            this.treeView1 = new System.Windows.Forms.TreeView();
            ((System.ComponentModel.ISupportInitialize)(this._blendBox)).BeginInit();
            this.SuspendLayout();
            // 
            // _blendBox
            // 
            this._blendBox.Size = new System.Drawing.Size(320, 374);
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            treeNode1.Name = "Node7";
            treeNode1.Text = "Setup...";
            treeNode2.Name = "Node0";
            treeNode2.Text = "Log Log Fit";
            treeNode3.Name = "Node1";
            treeNode3.Text = "SemiLog Fit";
            treeNode4.Name = "Node5";
            treeNode4.Text = "Probe 1-(1,2)";
            treeNode5.Name = "Node6";
            treeNode5.Text = "Probe 2-(1,2)";
            treeNode6.Name = "Node0";
            treeNode6.Text = "Probe 3-(1,2)";
            treeNode7.Name = "Node1";
            treeNode7.Text = "Probe (1-1,1-2,2-1,2-2,3-1,3-2)";
            treeNode8.Name = "Node2";
            treeNode8.Text = "Probe 1-1";
            treeNode9.Name = "Node2";
            treeNode9.Text = "Linear Fit";
            treeNode10.Name = "Node3";
            treeNode10.Text = "Specimen 127 Removed";
            treeNode11.Name = "Node3";
            treeNode11.Text = "Probe (1-1,2-1,3-1)";
            treeNode12.Name = "Node4";
            treeNode12.Text = "Analyses";
            treeNode13.Name = "Node0";
            treeNode13.Text = "Project Name";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode13});
            this.treeView1.Size = new System.Drawing.Size(320, 374);
            this.treeView1.TabIndex = 0;
            // 
            // ProjectDock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(320, 374);
            this.Controls.Add(this.treeView1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ProjectDock";
            this.Text = "Project Manager";
            this.Controls.SetChildIndex(this.treeView1, 0);
            this.Controls.SetChildIndex(this._blendBox, 0);
            ((System.ComponentModel.ISupportInitialize)(this._blendBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
    }
}