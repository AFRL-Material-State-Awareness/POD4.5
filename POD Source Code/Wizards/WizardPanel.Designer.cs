namespace POD.Wizards
{
    partial class WizardPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenu = new POD.Controls.ContextMenuStripConnected(this.components);
            this.SuspendLayout();
            // 
            // contextMenu
            // 
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(61, 4);
            // 
            // WizardPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ContextMenuStrip = this.contextMenu;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "WizardPanel";
            this.Size = new System.Drawing.Size(739, 483);
            this.ResumeLayout(false);

        }

        #endregion

        private POD.Controls.ContextMenuStripConnected contextMenu;

    }
}
