// <copyright file="MaxAndMinClass.cs" company="The University of Dayton">
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
using System.Text;
//used for maxmin function
using System.Linq;
namespace CSharpBackendWithR
{
    /// <summary>
    /// used to get the maximum and minimum crack size for any given analysis
    /// </summary>
    class MaxAndMinClass
    {
        
        private Dictionary<string, double> maxAndMinDict;
        private double maxValue;
        private double minValue;
        public MaxAndMinClass()
        {
            this.maxAndMinDict = new Dictionary<string, double>();
            this.maxValue = double.MaxValue;
            this.minValue= double.MinValue;
        }
        //function used to get the maximum and minimum crack
        public void calcCrackRange(List<double> flaws)
        {
            
            this.maxValue = flaws.Max();
            this.minValue = flaws.Min();
            this.maxAndMinDict.Add(key: "Max", value: maxValue);
            this.maxAndMinDict.Add(key: "Min", value: minValue);
        }
        public Dictionary<string, double> maxAndMinListControl{
            get{
                return this.maxAndMinDict;
            }
        }
    }
}
