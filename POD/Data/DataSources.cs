using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using POD.ExcelData;

namespace POD.Data
{
    [Serializable]
    public class DataSources : List<DataSource>
    {
        public DataTable GetGraphData(int myGraphIndex, int myXIndex, int myYIndex)
        {
            return this[myGraphIndex].GetGraphData(myXIndex, myYIndex);
        }

        public List<string> Names
        {
            get
            {
                List<string> names = new List<string>();

                foreach(DataSource source in this)
                {
                    names.Add(source.SourceName);
                }

                return names;
            }
        }

        public void WriteDataToExcel(ExcelExport myWriter)
        {
            foreach(DataSource source in this)
            {
                source.WriteDataToExcel(myWriter);
            }
        }

        public void WriteDataPropertiesToExcel(ExcelExport myWriter)
        {
            foreach (DataSource source in this)
            {
                source.WritePropertiesToExcel(myWriter);
            }
        }

        public DataSource this[string myName]
        {
            get
            {
                foreach(DataSource source in this)
                {
                    if(source.SourceName == myName)
                    {
                        return source;
                    }
                }

                return null;
            }
        }

        
    }
}
