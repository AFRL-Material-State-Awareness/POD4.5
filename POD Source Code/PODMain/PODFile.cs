// <copyright file="PODFile.cs" company="The University of Dayton">
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
using POD.Analyze;

namespace POD
{
    public class PODFile : SkillTypeHolder
    {
        Project _project;
        Analysis _analysis;
        AnalysisList _analyses;

        public PODFile()
        {
            _analyses = new AnalysisList();
        }

        public Project Project
        {
            get { return _project; }
            set { _project = value; }
        }

        public Analysis Analysis
        {
            get { return _analysis; }
            set { _analysis = value; }
        }

        public void AddAnalysis(Analysis myAnalysis)
        {
            _analyses.Add(myAnalysis);

            if(_analysis == null)
                _analysis = myAnalysis;
        }

        public Analysis GetAnalysis(int myIndex)
        {
            if(myIndex >= 0 && myIndex < _analyses.Count)
                return _analyses[myIndex];

            return null;
        }

        public Analysis GetAnalysis(string myName)
        {
            for(int i = 0; i < _analyses.Count; i++)
            {
                if (myName == _analyses[i].Name)
                    return _analyses[i];
            }

            return null;
        }

        public int Count
        {
            get
            {
                return _analyses.Count;
            }
        }
    }
}
