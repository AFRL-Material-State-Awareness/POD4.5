using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Drawing;

namespace POD.Data
{
    

    public class PODListBoxItem
    {
        //private Color color;

        public Color RowColor { get; set; }

        public string ResponseColumnName { get; set; }

        public string DataSourceName { get; set; }

        public string FlawColumnName { get; set; }

        public string ResponseOriginalName { get; set; }
        public string FlawOriginalName { get; set; }
        public string DataSourceOriginalName { get; set; }
        

        public PODListBoxItem()
        {

        }

        public PODListBoxItem(Color color, string responseColumn, string flawColumn, string dataSource)
        {
            //don't let names have periods since that the splitting character
            ResponseColumnName = responseColumn.Replace(".", "");
            FlawColumnName = flawColumn.Replace(".", "");
            DataSourceName = dataSource.Replace(".", "");

            this.RowColor = color;
            this.ResponseColumnName = responseColumn;
            this.ResponseOriginalName = responseColumn;
            this.FlawColumnName = flawColumn;
            this.FlawOriginalName = flawColumn;
            this.DataSourceName = dataSource;
            this.DataSourceOriginalName = dataSource;
        }

        public PODListBoxItem(Color color, string responseColumn, string responseOriginal, string flawColumn, string flawOriginal, string dataSource, string dataOriginal)
        {
            //don't let names have periods since that the splitting character
            ResponseColumnName = responseColumn.Replace(".", "");
            FlawColumnName = flawColumn.Replace(".", "");
            DataSourceName = dataSource.Replace(".", "");

            this.RowColor = color;
            this.ResponseColumnName = responseColumn;
            this.ResponseOriginalName = responseOriginal;
            this.FlawColumnName = flawColumn;
            this.FlawOriginalName = flawOriginal;
            this.DataSourceName = dataSource;
            this.DataSourceOriginalName = dataOriginal;
        }

        

        public override string ToString()
        {
            var name = DataSourceName;
            if (FlawColumnName == "" && ResponseColumnName == "")
                return name;
            if (name != "")
                name += ".";
            if (ResponseColumnName == "")
                return  name + FlawColumnName;
            else if (FlawColumnName == "")
                return name + ResponseColumnName;
            else
                return name + FlawColumnName + "." + ResponseColumnName;
        }

        public override int GetHashCode()
        {
 	         return base.GetHashCode();
        }

        public override bool Equals(Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            PODListBoxItem p = obj as PODListBoxItem;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (ResponseColumnName == p.ResponseColumnName) && (FlawColumnName == p.FlawColumnName) &&
                   (DataSourceName == p.DataSourceName);
        }

        public bool Equals(PODListBoxItem p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (ResponseColumnName == p.ResponseColumnName) && (FlawColumnName == p.FlawColumnName) &&
                   (DataSourceName == p.DataSourceName);
        }        
    }

    public class PODListBoxItemWithProps : PODListBoxItem
    {
        public string Unit { get; set; }
        public double Max { get; set; }
        public double Min { get; set; }
        public ColType BoxType { get; set; }
        public double Threshold { get; set; }
        public bool HideExtendedText { get; set; }

        public PODListBoxItemWithProps()
        {

        }

        public PODListBoxItemWithProps(Color color, ColType myType, string columnName, string dataSource, string units, double min, double max, double thresh)
        {
            this.RowColor = color;
            this.BoxType = myType;

            if (this.BoxType == ColType.Response)
            {
                this.FlawColumnName = "";
                this.FlawOriginalName = "";
                this.ResponseColumnName = columnName;
                this.ResponseOriginalName = columnName;
                this.Threshold = thresh;
            }
            else
            {
                this.ResponseColumnName = "";
                this.ResponseOriginalName = "";
                this.FlawColumnName = columnName;
                this.FlawOriginalName = columnName;
                this.Threshold = 0.0;
            }

            this.DataSourceName = dataSource;
            this.Unit = units;
            this.Min = min;
            this.Max = max;

        }

        public PODListBoxItem ToPODItem()
        {
            var item = new PODListBoxItem(RowColor, ResponseColumnName, ResponseOriginalName, FlawColumnName, FlawOriginalName, DataSourceName, DataSourceOriginalName);

            return item;
        }

        public PODListBoxItemWithProps(Color color, ColType myType, string columnName, string originalName, string dataSource, string units, double min, double max, double thresh)
        {
            this.RowColor = color;
            this.BoxType = myType;

            if (this.BoxType == ColType.Response)
            {
                this.FlawColumnName = "";
                this.FlawOriginalName = "";
                this.ResponseColumnName = columnName;
                this.ResponseOriginalName = originalName;
                this.Threshold = thresh;
            }
            else
            {
                this.ResponseColumnName = "";
                this.ResponseOriginalName = "";
                this.FlawColumnName = columnName;
                this.FlawOriginalName = originalName;
                this.Threshold = 0.0;
            }

            this.DataSourceName = dataSource;
            this.DataSourceOriginalName = dataSource;
            this.Unit = units;
            this.Min = min;
            this.Max = max;

        }

        private void SetColumnName(string columnName)
        {
            if (this.BoxType == ColType.Response)
            {
                this.FlawColumnName = "";
                this.ResponseColumnName = columnName;
            }
            else
            {
                this.ResponseColumnName = "";
                this.FlawColumnName = columnName;
            }
        }

        private void SetOriginalName(string columnName)
        {
            if (this.BoxType == ColType.Response)
            {
                this.FlawOriginalName = "";
                this.ResponseOriginalName = columnName;
            }
            else
            {
                this.ResponseOriginalName = "";
                this.FlawOriginalName = columnName;
            }
        }

        public string GetColumnName()
        {
            if (this.BoxType == ColType.Response)
            {
                return this.ResponseColumnName;
            }
            else
            {
                return this.FlawColumnName;
            }
        }

        private string GetOriginalName()
        {
            if (this.BoxType == ColType.Response)
                return ResponseOriginalName;
            else
                return FlawOriginalName;
        }
        
        public override string ToString()
        {
            var text = base.ToString();

            if (!HideExtendedText)
            {
                var flawLabel = "";

                if (Unit.Length > 0)
                    flawLabel = " (" + Unit + "),";

                text = text + flawLabel + " [" + Min + "," + Max + "]";

                if (BoxType == ColType.Response)
                    text = text + ", " + Threshold;
            }

            return text;
        }

        public string GetStringValue(InfoType myDataType)
        {
            if (myDataType == InfoType.Unit)
                return Unit;
            else if (myDataType == InfoType.Unit)
                return GetOriginalName();
            else if (myDataType == InfoType.NewName)
                return GetColumnName();
            else
                return "";
        }

        public string SetStringValue(InfoType myDataType, string myValue)
        {
            var val = myValue.Trim();

            if (myDataType == InfoType.Unit)
                Unit = val;
            else if (myDataType == InfoType.Unit)
                SetOriginalName(val);
            else if (myDataType == InfoType.NewName)
                SetColumnName(val);

            return val;
        }

        public double GetDoubleValue(InfoType myDataType)
        {
            if (myDataType == InfoType.Max)
                return Max;
            else if (myDataType == InfoType.Min)
                return Min;
            else if (myDataType == InfoType.Threshold)
                return Threshold;
            else
                return 0.0;
        }

        public double SetDoubleValue(InfoType myDataType, double myValue)
        {
            if (myDataType == InfoType.Max)
                Max = myValue;
            else if (myDataType == InfoType.Min)
                Min = myValue;
            else if (myDataType == InfoType.Threshold)
                Threshold = myValue;

            return myValue;
        }

        
    }
}
