using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POD.Data;

namespace POD.Controls
{
    using System.Drawing;
    using System.Windows.Forms;

    public class PODTreeNode : HiddenCheckBoxTreeNode
    {
        public string DataSource { get; set; }
        public string DataOriginal { get; set; }

        public string ResponseLabel { get; set; }
        public string FlawLabel { get; set; }
        
        private string _responseOriginal = "";
        private string _flawOriginal = "";
        

        public string ResponseOriginal
        {
            get
            {
                if (_responseOriginal == "")
                    _responseOriginal = ResponseLabel;

                return _responseOriginal;
            }
            set
            {
                _responseOriginal = value;
            }
        }

        public string FlawOriginal
        {
            get
            {
                if (_flawOriginal == "")
                    _flawOriginal = FlawLabel;

                return _flawOriginal;
            }
            set
            {
                _flawOriginal = value;
            }
        }

        public string GetOriginalLabel(ColType myType)
        {
            switch (myType)
            {
                case ColType.Response:
                    return ResponseOriginal;
                case ColType.Flaw:
                    return FlawOriginal;
                default:
                    return "";
            }
        }

        public void Label(ColType myType, string myName)
        {
            if (myType == ColType.Response)
                ResponseLabel = myName;
            else if (myType == ColType.Flaw)
                FlawLabel = myName;
        }

        /// <summary>
        /// This is the original name of the analysis, set when created
        /// </summary>
        public string OriginalAnalysisName
        {
            get
            {
                return _originalAnalysisName;
            }
            set
            {
                _originalAnalysisName = value;
            }
        }

        public bool HasCustomName = false;
        private string _customName;
        private string _originalAnalysisName;

        /// <summary>
        /// Gets/sets the custom name of the analysis
        /// </summary>
        public string CustomAnalysisName
        {
            set
            {
                HasCustomName = true;

                _customName = value;
            }
            get
            {
                return _customName;
            }
        }

        /// <summary>
        /// Returns what the most recent name of the analysis should be
        /// </summary>
        public string NewAnalysisName
        {
            get
            {
                if (HasCustomName)
                    return CustomAnalysisName;
                else
                    return AnalysisAutoName;
            }
        }


        /// <summary>
        /// this is what the name of the analysis will be if no custom name is used
        /// </summary>
        public string AnalysisAutoName { get; set; }

        public Color RowColor { get; set; }

        public PODTreeNode(PODListBoxItem listItem, string name, bool hasCustomName) : base(name)
        {
            this.DataSource = listItem.DataSourceName;
            this.DataOriginal = listItem.DataSourceOriginalName;
            this.ResponseLabel = listItem.ResponseColumnName;
            this.ResponseOriginal = listItem.ResponseOriginalName;
            this.FlawLabel = listItem.FlawColumnName;
            this.FlawOriginal = listItem.FlawOriginalName;
            
            this.HasCustomName = hasCustomName;

            if (HasCustomName)
            {
                this._customName = name;
                this.AnalysisAutoName = "";
            }
            else
            {
                this.AnalysisAutoName = name;
                this._customName = "";
            }

            this._originalAnalysisName = name;
            this.RowColor = listItem.RowColor;
        }

        public PODListBoxItem CreateListBox()
        {
            PODListBoxItem item = new PODListBoxItem(RowColor, ResponseLabel, FlawLabel, DataSource);

            return item;
        }

        public bool IsNewAnalysis { get; set; }
    }
}
