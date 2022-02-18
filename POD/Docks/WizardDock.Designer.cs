namespace POD.Docks
{
    partial class WizardDock
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizardDock));
            this.contextMenuStrip1 = new POD.Controls.ContextMenuStripConnected(this.components);
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.moveBarBox = new System.Windows.Forms.PictureBox();
            this.movePanelBox = new System.Windows.Forms.PictureBox();
            this.moveTitleBox = new System.Windows.Forms.PictureBox();
            this.nextTitleBox = new System.Windows.Forms.PictureBox();
            this.nextBarBox = new System.Windows.Forms.PictureBox();
            this.nextPanelBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this._blendBox)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.moveBarBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.movePanelBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.moveTitleBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nextTitleBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nextBarBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nextPanelBox)).BeginInit();
            this.SuspendLayout();
            // 
            // _blendBox
            // 
            this._blendBox.Size = new System.Drawing.Size(634, 555);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.IsTextboxMenu = false;
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.toolStripSeparator1});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.ShowOnlyButtons = false;
            this.contextMenuStrip1.Size = new System.Drawing.Size(103, 76);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(99, 6);
            // 
            // moveBarBox
            // 
            this.moveBarBox.Location = new System.Drawing.Point(265, 160);
            this.moveBarBox.Name = "moveBarBox";
            this.moveBarBox.Size = new System.Drawing.Size(100, 50);
            this.moveBarBox.TabIndex = 1;
            this.moveBarBox.TabStop = false;
            this.moveBarBox.Visible = false;
            // 
            // movePanelBox
            // 
            this.movePanelBox.Location = new System.Drawing.Point(325, 183);
            this.movePanelBox.Name = "movePanelBox";
            this.movePanelBox.Size = new System.Drawing.Size(100, 50);
            this.movePanelBox.TabIndex = 2;
            this.movePanelBox.TabStop = false;
            this.movePanelBox.Visible = false;
            // 
            // moveTitleBox
            // 
            this.moveTitleBox.Location = new System.Drawing.Point(325, 269);
            this.moveTitleBox.Name = "moveTitleBox";
            this.moveTitleBox.Size = new System.Drawing.Size(100, 50);
            this.moveTitleBox.TabIndex = 2;
            this.moveTitleBox.TabStop = false;
            this.moveTitleBox.Visible = false;
            // 
            // nextTitleBox
            // 
            this.nextTitleBox.Location = new System.Drawing.Point(340, 375);
            this.nextTitleBox.Name = "nextTitleBox";
            this.nextTitleBox.Size = new System.Drawing.Size(100, 50);
            this.nextTitleBox.TabIndex = 2;
            this.nextTitleBox.TabStop = false;
            this.nextTitleBox.Visible = false;
            // 
            // nextBarBox
            // 
            this.nextBarBox.Location = new System.Drawing.Point(175, 422);
            this.nextBarBox.Name = "nextBarBox";
            this.nextBarBox.Size = new System.Drawing.Size(100, 50);
            this.nextBarBox.TabIndex = 2;
            this.nextBarBox.TabStop = false;
            this.nextBarBox.Visible = false;
            // 
            // nextPanelBox
            // 
            this.nextPanelBox.Location = new System.Drawing.Point(410, 431);
            this.nextPanelBox.Name = "nextPanelBox";
            this.nextPanelBox.Size = new System.Drawing.Size(100, 50);
            this.nextPanelBox.TabIndex = 2;
            this.nextPanelBox.TabStop = false;
            this.nextPanelBox.Visible = false;
            // 
            // WizardDock
            // 
            this.AllowEndUserDocking = false;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 555);
            this.Controls.Add(this.nextPanelBox);
            this.Controls.Add(this.nextBarBox);
            this.Controls.Add(this.nextTitleBox);
            this.Controls.Add(this.moveTitleBox);
            this.Controls.Add(this.movePanelBox);
            this.Controls.Add(this.moveBarBox);
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HideOnClose = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "WizardDock";
            this.Text = "WizardDock";
            this.Activated += new System.EventHandler(this.WizardDock_Activated);
            this.Controls.SetChildIndex(this._blendBox, 0);
            this.Controls.SetChildIndex(this.moveBarBox, 0);
            this.Controls.SetChildIndex(this.movePanelBox, 0);
            this.Controls.SetChildIndex(this.moveTitleBox, 0);
            this.Controls.SetChildIndex(this.nextTitleBox, 0);
            this.Controls.SetChildIndex(this.nextBarBox, 0);
            this.Controls.SetChildIndex(this.nextPanelBox, 0);
            ((System.ComponentModel.ISupportInitialize)(this._blendBox)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.moveBarBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.movePanelBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.moveTitleBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nextTitleBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nextBarBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nextPanelBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private POD.Controls.ContextMenuStripConnected contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.PictureBox moveBarBox;
        private System.Windows.Forms.PictureBox movePanelBox;
        private System.Windows.Forms.PictureBox moveTitleBox;
        private System.Windows.Forms.PictureBox nextTitleBox;
        private System.Windows.Forms.PictureBox nextBarBox;
        private System.Windows.Forms.PictureBox nextPanelBox;
    }
}