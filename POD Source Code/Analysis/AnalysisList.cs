// <copyright file="AnalysisList.cs" company="The University of Dayton">
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

namespace POD.Analyze
{
    [Serializable]
    public class AnalysisList : List<Analysis>
    {
        public List<string> Names
        {
            get
            {
                var names = new List<string>();

                foreach(Analysis analysis in this)
                {
                    names.Add(analysis.Name);
                }

                return names;
            }
        }

        public bool UsingDataSource(string p)
        {
            foreach (Analysis analysis in this)
            {
                if (analysis.SourceName == p)
                    return true;
            }

            return false;
        }
    }

    
}
