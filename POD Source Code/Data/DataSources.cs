// <copyright file="DataSources.cs" company="The University of Dayton">
// Copyright © 2025 The University of Dayton. All rights reserved.
//
// Licensed under the GNU General Public License, Version 3.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.gnu.org/licenses/gpl-3.0.txt
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

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
