using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeifenLuo.WinFormsUI.Docking;
using POD.Controls;
using Transitions;
using System.Drawing;

namespace POD.Docks
{
    public class PodDock : DockContent
    {
        string _label;
        public BlendPictureBox _blendBox;
        bool _done;

        public BlendPictureBox BlendBox
        {
          get { return _blendBox; }
          set { _blendBox = value; }
        }
        
        Transition _trans;

        public PodDock()
        {
            _blendBox = new BlendPictureBox();
            _blendBox.Dock = System.Windows.Forms.DockStyle.Fill;
            _blendBox.Visible = false;

            Controls.Add(_blendBox);

            _trans = new Transition(new TransitionType_Linear(300));
            _trans.TransitionCompletedEvent += _trans_TransitionCompletedEvent;
            _trans.add(_blendBox, "Transparency", 100.0F);
            _done = true;
        }

        void _trans_TransitionCompletedEvent(object sender, Transition.Args e)
        {
            //_trans = new Transition(new TransitionType_Linear(200));
            //_trans.TransitionCompletedEvent += _trans_TransitionCompletedEvent2;
            //_trans.add(_blendBox, "Transparency", 0.0F);

            _blendBox.SendToBack();
            _blendBox.Transparency = 0.0F;
            _done = true;
        }

        void _trans_TransitionCompletedEvent2(object sender, Transition.Args e)
        {
            
        }

        public string Label
        {
            get { return _label; }
            set { _label = value; }
        }

        //[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        protected override string GetPersistString()
        {
            return Label;
        }

        public void BlendToNextImage()
        {
            // We alternate the form's background color...
            //Color destination = (BackColor == BACKCOLOR_PINK) ? BACK_COLOR_YELLOW : BACKCOLOR_PINK;
            if (_done == true)
            {
                _trans.run();
                _done = false;

                _blendBox.BringToFront();
                _blendBox.Show();
            }
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PodDock));
            this.SuspendLayout();
            // 
            // PodDock
            // 
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PodDock";
            this.ResumeLayout(false);

        }
    }
}
